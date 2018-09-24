using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using MathSupport;
using OpenTK;
using Utilities;

namespace _114transition
{
  public partial class Form1 : Form
  {
    static readonly string rev = Util.SetVersion( "$Rev$" );

    volatile bool transitionDirty = false;
    Bitmap inputImage1 = null;
    Bitmap inputImage2 = null;
    Bitmap outputImage = null;

    /// <summary>
    /// Time parameter (0.0 to 1.0).
    /// </summary>
    double time = 0.0;

    string winTitle;
    ToolTip tt = new ToolTip();

    public Form1 ()
    {
      InitializeComponent();

      string param;
      string name;
      Transition.InitParams( out param, out name );
      textParam.Text = param ?? "";
      winTitle = (Text += " (" + rev + ") '" + name + '\'');

      Application.Idle += new EventHandler( Application_Idle );
    }

    protected Thread aThread = null;

    volatile public static bool cont = true;

    private void setImage ( ref Bitmap bakImage, Bitmap newImage )
    {
      pictureBox1.Image = newImage;
      if ( bakImage == newImage )
        return;

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
      trackBarExp.Enabled =
      textParam.Enabled   =
      buttonOpen2.Enabled =
      buttonOpen1.Enabled = enable;
      buttonStop.Enabled  = !enable;
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
      if ( inputImage1 == null ||
           inputImage2 == null )
      {
        setWindowTitle( null );
        return;
      }

      StringBuilder sb = new StringBuilder();
      sb.Append( $" {inputImage1.Width} x {inputImage1.Height}", ,  );
      if ( x >= 0 && x < inputImage1.Width &&
           y >= 0 && y < inputImage1.Height )
      {
        sb.Append( $": [{x},{y}]: " );
        Color c = inputImage1.GetPixel( x, y );
        sb.Append( $" [{c.R},{c.G},{c.B}]" );
        if ( inputImage2 != null &&
             inputImage2.Width  == inputImage1.Width &&
             inputImage2.Height == inputImage1.Height )
        {
          c = inputImage2.GetPixel( x, y );
          sb.Append( $"--[{c.R},{c.G},{c.B}]" );
        }
      }

      setWindowTitle( sb.ToString() );
    }

    /// <summary>
    /// Shared timer.
    /// </summary>
    static Stopwatch sw = new Stopwatch();

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
        Util.TryParse( p, "t", ref time );
      }

      time = Util.Clamp( time, 0.0, 1.0 );

      sw.Restart();
      outputImage = inputImage.Exposure( outputImage, exposure, gamma );
      sw.Stop();
      labelStatus.Text = string.Format( CultureInfo.InvariantCulture, "{0:f1} EV, exp: {1} ms",
                                        contrast, sw.ElapsedMilliseconds );

      setImage( ref outputImage, outputImage );
    }

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

      pictureBox1.Image = null;
      setImage( ref inputImage, inp );

      recompute();

      return true;
    }

    private void buttonOpen1_Click ( object sender, EventArgs e )
    {
      OpenFileDialog ofd = new OpenFileDialog();

      ofd.Title = "Open Image 1";
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

      // Load HDR file
      if ( ofd.FileName.EndsWith( ".hdr" ) ||
           ofd.FileName.EndsWith( ".pic" ) )
      {
        LoadHDR( ofd.FileName, textParam.Text );
        return;
      }

      // Load PFM file
      if ( ofd.FileName.EndsWith( ".pfm" ) )
      {
        MessageBox.Show( string.Format( "PFM format is not implemented yet '{0}'", ofd.FileName ), "PFM load error" );
      }
    }

    private void buttonOpen2_Click ( object sender, EventArgs e )
    {
      recompute();
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

    private void tonemap ()
    {
      if ( inputImage != null )
      {
        Stopwatch swt = new Stopwatch();
        swt.Start();

        Bitmap newImage = ToneMapping.ToneMap( inputImage, outputImage, textParam.Text );

        swt.Stop();
        SetText( string.Format( CultureInfo.InvariantCulture, "tonemap: {0} ms",
                                swt.ElapsedMilliseconds ) );
        SetImage( newImage );
      }

      StopComputation();
    }

    private void recompute ()
    {
      if ( aThread != null )
        return;

      SetGUI( false );
      cont = true;

      aThread = new Thread( new ThreadStart( tonemap ) );
      aThread.Start();
    }

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
      // extended log2 scale
      double aveLog2 = 0.5 * (minLog2 + maxLog2);
      exposure = aveLog2 + (contrast + EXTENDED_CONTRAST) * ( (trackBarExp.Value - trackBarExp.Minimum) / (double)(trackBarExp.Maximum - trackBarExp.Minimum) - 0.5 );
      labelExpValue.Text = string.Format( CultureInfo.InvariantCulture, "{0:f1} EV", exposure );

      // multiplication coefficient:
      exposure = Math.Pow( 2.0, exposure - aveLog2 );
    }

    private void trackBarExp_ValueChanged ( object sender, EventArgs e )
    {
      changeLabelExp();
      exposureDirty = true;
    }

    private void placeLabelExp ()
    {
      Point old = labelExpValue.Location;
      int half = labelExpValue.Size.Width / 2;
      int barLocCenter = trackBarExp.Location.X + trackBarExp.Size.Width / 2;
      old.X = barLocCenter - half;
      old.Y = trackBarExp.Location.Y + 30;
      labelExpValue.Location = old;
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
      tt.Show( Util.TargetFramework + " (" + Util.RunningFramework + "), OpenTK " + Util.AssemblyVersion( typeof( Vector2 ) ),
               (IWin32Window)sender, 10, -25, 4000 );
    }
  }
}
