using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utilities;

namespace _114transition
{
  public partial class Form1 : Form
  {
    static readonly string rev = Util.SetVersion( "$Rev$" );

    /// <summary>
    /// The output image has to be recalculated.
    /// </summary>
    volatile bool transitionDirty = false;

    /// <summary>
    /// Input image No1. Can be null.
    /// </summary>
    Bitmap inputImage1 = null;

    /// <summary>
    /// Input image No2. Can be null.
    /// </summary>
    Bitmap inputImage2 = null;

    /// <summary>
    /// Output (computed) image.
    /// </summary>
    Bitmap outputImage = null;

    /// <summary>
    /// Time parameter (0.0 to 1.0).
    /// </summary>
    double time = 0.5;

    /// <summary>
    /// Use parallel computation?
    /// </summary>
    bool par = true;

    /// <summary>
    /// Cached window title.
    /// </summary>
    string winTitle;

    /// <summary>
    /// Param string tooltip = help.
    /// </summary>
    string tooltip = "";

    /// <summary>
    /// Shared ToolTip instance.
    /// </summary>
    ToolTip tt = new ToolTip();

    public Form1 ()
    {
      InitializeComponent();

      string name;
      string param;
      Transition.InitParams( out name, out param, out tooltip );
      textParam.Text = param ?? "";
      winTitle = (Text += " (" + rev + ") '" + name + '\'');

      Application.Idle += new EventHandler( Application_Idle );
    }

    /// <summary>
    /// Not-null only during a computation.
    /// </summary>
    protected Thread aThread = null;

    /// <summary>
    /// If set to false, computation should end ASAP.
    /// </summary>
    volatile public static bool cont = true;

    /// <summary>
    /// Working instance of a Transition object.
    /// </summary>
    protected Transition tr = null;

    protected void assertTransition ( int width, int height )
    {
      if ( tr == null )
        tr = new Transition( width, height, textParam.Text );
      else
        tr.Reset( width, height, textParam.Text );
    }

    private void setImage ( ref Bitmap bakImage, Bitmap newImage )
    {
      if ( bakImage != null )
        bakImage.Dispose();
      bakImage = newImage;
    }

    delegate void SetImageCallback ( Bitmap newImage );

    protected void SetImage ( Bitmap newImage )
    {
      if ( pictureBox1.InvokeRequired )
      {
        SetImageCallback si = new SetImageCallback( SetImage );
        BeginInvoke( si, new object[] { newImage } );
      }
      else
      {
        pictureBox1.Image = newImage;
        setImage( ref outputImage, newImage );
        pictureBox1.Invalidate();
      }
    }

    delegate void SetTextCallback ( string text );

    protected void SetText ( string text )
    {
      if ( labelStatus.InvokeRequired )
      {
        SetTextCallback st = new SetTextCallback( SetText );
        BeginInvoke( st, new object[] { text } );
      }
      else
        labelStatus.Text = text;
    }

    void SetGUI ( bool enable )
    {
      trackBarTime.Enabled =
      textParam.Enabled    =
      buttonOpen2.Enabled  =
      buttonOpen1.Enabled  = enable;
      buttonSave.Enabled   = enable && outputImage != null;
      buttonRun.Enabled    = enable && inputImage1 != null && inputImage2 != null;
      buttonStop.Enabled   = !enable;
    }

    void setWindowTitle ( string suffix )
    {
      if ( string.IsNullOrEmpty( suffix ) )
        Text = winTitle;
      else
        Text = winTitle + ' ' + suffix;
    }

    /// <summary>
    /// Shows data of the given pixel (and of the HDR & LDR images).
    /// </summary>
    /// <param name="x">X-coordinate inside the raster image.</param>
    /// <param name="y">Y-coordinate inside the raster image.</param>
    private void showPixel ( int x, int y )
    {
      if ( outputImage == null )
      {
        setWindowTitle( null );
        return;
      }

      StringBuilder sb = new StringBuilder();
      sb.Append( $" {outputImage.Width} x {outputImage.Height}" );
      if ( x >= 0 && x < outputImage.Width &&
           y >= 0 && y < outputImage.Height )
      {
        sb.Append( $": [{x},{y}]:" );
        Color c = outputImage.GetPixel( x, y );
        sb.Append( $" [{c.R},{c.G},{c.B}]" );
      }

      setWindowTitle( sb.ToString() );
    }

    /// <summary>
    /// Function called whenever the main application is idle..
    /// </summary>
    void Application_Idle ( object sender, EventArgs e )
    {
      if ( transitionDirty )
      {
        transitionDirty = false;
        Transform( textParam.Text );
      }
    }

    protected void Transform ( string param )
    {
      if ( inputImage1 == null ||
           inputImage2 == null )
        return;

      Dictionary<string, string> p = Util.ParseKeyValueList( param );
      if ( p.Count > 0 )
      {
        // t=<float-number>
        // Has priority over the trackBarTime!
        Util.TryParse( p, "t", ref time );

        // par=<bool>
        // use Parallel.For?
        Util.TryParse( p, "par", ref par );
      }

      time = Util.Clamp( time, 0.0, 1.0 );    // to be sure
      cont = true;
      transition();
    }

    /// <summary>
    /// Accepts a new input image (either #1 or #2).
    /// </summary>
    /// <param name="fn">Image file full-path.</param>
    /// <param name="firstImage">Use the 1st image?</param>
    /// <returns>True if ok.</returns>
    private bool newImage ( string fn, bool firstImage )
    {
      Bitmap inp = null;
      try
      {
        inp = (Bitmap)Image.FromFile( fn );
      }
      catch ( Exception )
      {
        return false;
      }

      if ( inp == null )
        return false;

      if ( firstImage )
      {
        buttonOpen1.Text = Path.GetFileName( fn );
        setImage( ref inputImage1, inp );
      }
      else
      {
        buttonOpen2.Text = Path.GetFileName( fn );
        setImage( ref inputImage2, inp );
      }

      recompute();

      return true;
    }

    private void fileOpen ( bool firstImage )
    {
      OpenFileDialog ofd = new OpenFileDialog();

      ofd.Title = $"Open Image {(firstImage ? 1 : 2)}";
      ofd.Filter = "Bitmap Files|*.bmp" +
          "|Gif Files|*.gif" +
          "|JPEG Files|*.jpg" +
          "|PNG Files|*.png" +
          "|TIFF Files|*.tif" +
          "|All image types|*.bmp;*.gif;*.jpg;*.png;*.tif";

      ofd.FilterIndex = 6;
      ofd.FileName = "";
      if ( ofd.ShowDialog() != DialogResult.OK )
        return;

      newImage( ofd.FileName, firstImage );
    }

    private void buttonOpen1_Click ( object sender, EventArgs e )
    {
      fileOpen( true );
    }

    private void buttonOpen2_Click ( object sender, EventArgs e )
    {
      fileOpen( false );
    }

    delegate void StopComputationCallback ();

    protected void StopComputation ()
    {
      if ( aThread == null )
        return;

      if ( buttonOpen2.InvokeRequired )
      {
        StopComputationCallback ea = new StopComputationCallback( StopComputation );
        BeginInvoke( ea );
      }
      else
      {
        // actually stop the computation:
        cont = false;
        aThread.Join();
        aThread = null;

        // GUI stuff:
        SetGUI( true );
      }
    }

    /// <summary>
    /// Computes image transition from inputImage* using text params from the form.
    /// Puts result into the outputImage and displays it in the form.
    /// </summary>
    private void transition ()
    {
      if ( inputImage1 != null &&
           inputImage2 != null )
      {
        Stopwatch sw = new Stopwatch();
        sw.Start();

        // Do the transformation job.
        int width  = Math.Min( inputImage1.Width, inputImage2.Width );
        int height = Math.Min( inputImage1.Height, inputImage2.Height );
        Bitmap nImage = new Bitmap( width, height, PixelFormat.Format24bppRgb );
        time = Util.Clamp( time, 0.0, 1.0 );     // to be sure
        assertTransition( width, height );

        // Fast memory-mapped code.
        PixelFormat iFormat1 = inputImage1.PixelFormat;
        if ( !PixelFormat.Format24bppRgb.Equals( iFormat1 ) &&
             !PixelFormat.Format32bppArgb.Equals( iFormat1 ) &&
             !PixelFormat.Format32bppPArgb.Equals( iFormat1 ) &&
             !PixelFormat.Format32bppRgb.Equals( iFormat1 ) )
          iFormat1 = PixelFormat.Format24bppRgb;
        PixelFormat iFormat2 = inputImage2.PixelFormat;
        if ( !PixelFormat.Format24bppRgb.Equals( iFormat2 ) &&
             !PixelFormat.Format32bppArgb.Equals( iFormat2 ) &&
             !PixelFormat.Format32bppPArgb.Equals( iFormat2 ) &&
             !PixelFormat.Format32bppRgb.Equals( iFormat2 ) )
          iFormat2 = PixelFormat.Format24bppRgb;

        BitmapData dataIn1 = inputImage1.LockBits( new Rectangle( 0, 0, width, height ), ImageLockMode.ReadOnly, iFormat1 );
        BitmapData dataIn2 = inputImage2.LockBits( new Rectangle( 0, 0, width, height ), ImageLockMode.ReadOnly, iFormat2 );
        BitmapData dataOut = nImage.LockBits( new Rectangle( 0, 0, width, height ), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb );
        unsafe
        {
          int dI1 = Image.GetPixelFormatSize( iFormat1 ) / 8;
          int dI2 = Image.GetPixelFormatSize( iFormat2 ) / 8;
          int dO  = Image.GetPixelFormatSize( PixelFormat.Format24bppRgb ) / 8;

          Action<int> body = y =>
          {
            if ( !cont ) return;

            double x1, y1, x2, y2, t, t1;
            int ix, iy;
            byte* iptr1, iptr2, optr;

            optr = (byte*)dataOut.Scan0 + y * dataOut.Stride;

            if ( tr.UseMorphing() )
            {
              // Geometric transform + blending.
              for ( int x = 0; x < width; x++, optr += dO )
              {
                tr.MorphingFunction( time, x, y, out x1, out y1, out x2, out y2, out t );
                t = Util.Clamp( t, 0.0, 1.0 );
                t1 = 1.0 - t;

                ix = Util.Clamp( (int)Math.Round( x1 ), 0, width - 1 );
                iy = Util.Clamp( (int)Math.Round( y1 ), 0, height - 1 );
                iptr1 = (byte*)dataIn1.Scan0 + iy * dataIn1.Stride + ix * dI1;
                ix = Util.Clamp( (int)Math.Round( x2 ), 0, width - 1 );
                iy = Util.Clamp( (int)Math.Round( y2 ), 0, height - 1 );
                iptr2 = (byte*)dataIn2.Scan0 + iy * dataIn2.Stride + ix * dI2;

                // Linear blend of two input pixels (three components = R,G,B).
                optr[ 0 ] = (byte)Math.Round( t1 * iptr1[ 0 ] + t * iptr2[ 0 ] );
                optr[ 1 ] = (byte)Math.Round( t1 * iptr1[ 1 ] + t * iptr2[ 1 ] );
                optr[ 2 ] = (byte)Math.Round( t1 * iptr1[ 2 ] + t * iptr2[ 2 ] );
              }
            }
            else
            {
              // Simple blending w/o geometric transform.
              iptr1 = (byte*)dataIn1.Scan0 + y * dataIn1.Stride;
              iptr2 = (byte*)dataIn2.Scan0 + y * dataIn2.Stride;

              for ( int x = 0; x < width; x++, iptr1 += dI1, iptr2 += dI2, optr += dO )
              {
                t = Util.Clamp( tr.BlendingFunction( time, x, y ), 0.0, 1.0 );
                t1 = 1.0 - t;

                // Linear blend of two input pixels (three components = R,G,B).
                optr[ 0 ] = (byte)Math.Round( t1 * iptr1[ 0 ] + t * iptr2[ 0 ] );
                optr[ 1 ] = (byte)Math.Round( t1 * iptr1[ 1 ] + t * iptr2[ 1 ] );
                optr[ 2 ] = (byte)Math.Round( t1 * iptr1[ 2 ] + t * iptr2[ 2 ] );
              }
            }
          };

          if ( par )
            Parallel.For( 0, height, body );
          else
            for ( int y = 0; y < height; y++ )
              body( y );
        }
        nImage.UnlockBits( dataOut );
        inputImage1.UnlockBits( dataIn1 );
        inputImage2.UnlockBits( dataIn2 );

        sw.Stop();
        SetText( string.Format( CultureInfo.InvariantCulture, "Elapsed: {0} ms",
                                sw.ElapsedMilliseconds ) );
        SetImage( nImage );
      }

      StopComputation();
    }

    /// <summary>
    /// Recomputes the outputImage in a separate thread.
    /// </summary>
    private void recompute ()
    {
      if ( aThread != null )
        return;

      SetGUI( false );
      cont = true;

      aThread = new Thread( new ThreadStart( transition ) );
      aThread.Start();
    }

    /// <summary>
    /// Saves the output image (if ready).
    /// </summary>
    private void buttonSave_Click ( object sender, EventArgs e )
    {
      if ( outputImage == null ) return;

      SaveFileDialog sfd = new SaveFileDialog();
      sfd.Title = "Save PNG file";
      sfd.Filter = "PNG Files|*.png";
      sfd.AddExtension = true;
      sfd.FileName = "";
      if ( sfd.ShowDialog() != DialogResult.OK )
        return;

      outputImage.Save( sfd.FileName, ImageFormat.Png );
    }

    private void buttonStop_Click ( object sender, EventArgs e )
    {
      StopComputation();
    }

    private void changeLabelExp ()
    {
      time = (trackBarTime.Value - trackBarTime.Minimum) / (double)(trackBarTime.Maximum - trackBarTime.Minimum);
      labelTimeValue.Text = string.Format( CultureInfo.InvariantCulture, "{0:f2}", time );
    }

    private void trackBarTime_ValueChanged ( object sender, EventArgs e )
    {
      changeLabelExp();
      transitionDirty = true;
    }

    private void placeLabelExp ()
    {
      Point old = labelTimeValue.Location;
      int half = labelTimeValue.Size.Width / 2;
      int barLocCenter = trackBarTime.Location.X + trackBarTime.Size.Width / 2;
      old.X = barLocCenter - half;
      old.Y = trackBarTime.Location.Y + 30;
      labelTimeValue.Location = old;
    }

    private void Form1_SizeChanged ( object sender, EventArgs e )
    {
      placeLabelExp();
    }

    private void pictureBox1_MouseDown ( object sender, MouseEventArgs e )
    {
      if ( aThread == null && e.Button == MouseButtons.Left )
        showPixel( e.X, e.Y );
    }

    private void pictureBox1_MouseMove ( object sender, MouseEventArgs e )
    {
      if ( aThread == null && e.Button == MouseButtons.Left )
        showPixel( e.X, e.Y );
    }

    private void labelStatus_MouseHover ( object sender, EventArgs e )
    {
      tt.Show( Util.TargetFramework + " (" + Util.RunningFramework + ")",
               (IWin32Window)sender, 10, -25, 2000 );
    }

    private void buttonRun_Click ( object sender, EventArgs e )
    {
      transitionDirty = true;
    }

    private void textParam_KeyPress ( object sender, KeyPressEventArgs e )
    {
      if ( e.KeyChar == (char)Keys.Enter )
      {
        e.Handled = true;
        transitionDirty = true;
      }
    }

    private void buttonOpen1_MouseHover ( object sender, EventArgs e )
    {
      tt.Show( buttonOpen1.Text,
               (IWin32Window)sender, 10, -25, 2000 );
    }

    private void buttonOpen2_MouseHover ( object sender, EventArgs e )
    {
      tt.Show( buttonOpen2.Text,
               (IWin32Window)sender, 10, -25, 2000 );
    }

    private void textParam_MouseHover ( object sender, EventArgs e )
    {
      if ( !string.IsNullOrEmpty(tooltip) )
        tt.Show( tooltip,
                 (IWin32Window)sender, 10, -25, 2000 );
    }
  }
}
