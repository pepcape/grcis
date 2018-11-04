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
using Raster;
using Utilities;

namespace _094tonemapping
{
  public partial class Form1 : Form
  {
    static readonly string rev = Util.SetVersion( "$Rev$" );

    /// <summary>
    /// Log2 contrast extension.
    /// </summary>
    const double EXTENDED_CONTRAST = 8.0;

    /// <summary>
    /// Minimum Value of the non-black pixels of the current HDR image.
    /// </summary>
    double minY = 1.0;

    double minLog2 = 0.0;

    /// <summary>
    /// Maximum Value of the non-black pixels of the current HDR image.
    /// </summary>
    double maxY = 128.0;

    double maxLog2 = 7.0;

    /// <summary>
    /// Current image contrast (see minY, maxY) in log2 scale.
    /// </summary>
    double contrast = 7.0;

    double exposure = 1.0;
    volatile bool exposureDirty = false;
    FloatImage origInputImage = null;
    int subFactor = 1;

    FloatImage inputImage = null;
    Bitmap outputImage = null;

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
      ToneMapping.InitParams( out name, out param, out tooltip );
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
      trackBarExp.Enabled  =
      textParam.Enabled    =
      buttonRedraw.Enabled =
      buttonOpen.Enabled   =
      buttonSave.Enabled   = enable;
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
      if ( inputImage == null )
      {
        setWindowTitle( null );
        return;
      }

      StringBuilder sb = new StringBuilder();
      sb.AppendFormat( " {0} x {1}", inputImage.Width, inputImage.Height );
      if ( x >= 0 && x < inputImage.Width &&
           y >= 0 && y < inputImage.Height )
      {
        sb.AppendFormat( ": [{0},{1}] ->", x, y );
        float[] pix = new float[ 3 ];
        if ( inputImage.GetPixel( x, y, pix ) )
          sb.AppendFormat( CultureInfo.InvariantCulture, " [{0},{1},{2}]", pix[ 0 ], pix[ 1 ], pix[ 2 ] );
        if ( outputImage != null &&
             outputImage.Width == inputImage.Width &&
             outputImage.Height == inputImage.Height )
        {
          Color c = outputImage.GetPixel( x, y );
          sb.AppendFormat( " [{0},{1},{2}]", c.R, c.G, c.B );
        }
      }
      setWindowTitle( sb.ToString() );
    }

    /// <summary>
    /// Shared timer.
    /// </summary>
    static Stopwatch sw = new Stopwatch();

    protected void LoadHDR ( string fn, string param )
    {
      inputImage = origInputImage = RadianceHDRFormat.FromFile( fn );
      subFactor = 1;
      contrast = 0.0;
      if ( inputImage == null )
        return;

      inputImage.Contrast( out minY, out maxY );
      if ( minY > double.Epsilon )
      {
        minLog2 = Math.Log( minY ) / Math.Log( 2.0 );
        maxLog2 = Math.Log( maxY ) / Math.Log( 2.0 );
        contrast = maxLog2 - minLog2;

        // GUI update:
        labelMin.Text = string.Format( CultureInfo.InvariantCulture, "{0:f1} EV", minLog2 );
        labelMax.Text = string.Format( CultureInfo.InvariantCulture, "{0:f1} EV", maxLog2 );
        changeLabelExp();
      }

      Exposure( param );
    }

    /// <summary>
    /// Function called whenever the main application is idle..
    /// </summary>
    void Application_Idle ( object sender, EventArgs e )
    {
      if ( exposureDirty )
      {
        exposureDirty = false;
        Exposure( textParam.Text );
      }
    }

    protected void Exposure ( string param )
    {
      if ( inputImage == null )
        return;

      Dictionary<string, string> p = Util.ParseKeyValueList( param );
      double gamma = 0.0;
      if ( p.Count > 0 )
      {
        // gamma=<float-number>
        Util.TryParse( p, "gamma", ref gamma );

        // exp=<float-number>
        // must not change the value if the 'exp' key is not present
        Util.TryParse( p, "exp", ref exposure );

        // sub=<sub-factor>
        if ( Util.TryParse( p, "sub", ref subFactor ) )
        {
          if ( subFactor < 1 )
            subFactor = 1;

          if ( subFactor == 1 )
          {
            if ( inputImage != origInputImage )
              inputImage = origInputImage;
          }
          else
            if ( inputImage.Width * subFactor != origInputImage.Width )
            {
              inputImage = new FloatImage( origInputImage );
              inputImage.Resize( subFactor );
            }
        }
      }

      sw.Restart();
      outputImage = inputImage.Exposure( outputImage, exposure, gamma );
      sw.Stop();
      labelStatus.Text = string.Format( CultureInfo.InvariantCulture, "{0:f1} EV, exp: {1} ms",
                                        contrast, sw.ElapsedMilliseconds );

      setImage( ref outputImage, outputImage );
    }

    private void buttonOpen_Click ( object sender, EventArgs e )
    {
      OpenFileDialog ofd = new OpenFileDialog();

      ofd.Title = "Open Image File";
      ofd.Filter = "Radiance HDR Files|*.hdr;*.pic" +
          "|PFM Files|*.pfm" +
          "|All image types|*.hdr;*.pic;*.pfm";

      ofd.FilterIndex = 3;
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

    delegate void StopComputationCallback ();

    protected void StopComputation ()
    {
      if ( aThread == null )
        return;

      if ( buttonRedraw.InvokeRequired )
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

    private void buttonRedraw_Click ( object sender, EventArgs e )
    {
      recompute();
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
               (IWin32Window)sender, 10, -25, 2000 );
    }

    private void textParam_MouseHover ( object sender, EventArgs e )
    {
      if ( !string.IsNullOrEmpty( tooltip ) )
        tt.Show( tooltip,
                 (IWin32Window)sender, 10, -25, 2000 );
    }

    private void textParam_KeyPress ( object sender, System.Windows.Forms.KeyPressEventArgs e )
    {
      if ( e.KeyChar == (char)Keys.Enter )
      {
        e.Handled = true;
        recompute();
      }
    }
  }
}
