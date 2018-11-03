using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Drawing.Imaging;
using Utilities;

namespace _084filter
{
  public partial class Form1 : Form
  {
    static readonly string rev = Util.SetVersion( "$Rev$" );

    protected Bitmap inputImage = null;
    protected Bitmap outputImage = null;

    /// <summary>
    /// Window title prefix.
    /// </summary>
    protected string titlePrefix = null;

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

      string par, name;
      Filter.InitParams( out name, out par, out tooltip );
      textParam.Text = par;

      titlePrefix = (Text += " (" + rev + ") '" + name + '\'');
    }

    protected Thread aThread = null;

    /// <summary>
    /// User-break variable.
    /// </summary>
    volatile public static bool cont = true;

    /// <summary>
    /// Picture-box background color (for alpha-images).
    /// </summary>
    public static Color imageBoxBackground = Color.White;

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
        pictureBox1.BackColor = imageBoxBackground;
        setImage( ref outputImage, newImage );
      }
    }

    delegate void SetTextCallback ( string text );

    protected void SetText ( string text )
    {
      if ( labelElapsed.InvokeRequired )
      {
        SetTextCallback st = new SetTextCallback( SetText );
        BeginInvoke( st, new object[] { text } );
      }
      else
        labelElapsed.Text = text;
    }

    private void imageProbe ( int x, int y )
    {
      if ( outputImage == null )
        return;

      x = Util.Clamp( x, 0, outputImage.Width - 1 );
      y = Util.Clamp( y, 0, outputImage.Height - 1 );

      Color c = outputImage.GetPixel( x, y );
      StringBuilder sb = new StringBuilder( titlePrefix );
      sb.AppendFormat( " image[{0},{1}] = ", x, y );
      if ( outputImage.PixelFormat == PixelFormat.Format32bppArgb ||
           outputImage.PixelFormat == PixelFormat.Format64bppArgb ||
           outputImage.PixelFormat == PixelFormat.Format16bppArgb1555 )
        sb.AppendFormat( "[{0},{1},{2},{3}] = #{0:X02}{1:X02}{2:X02}{3:X02}", c.R, c.G, c.B, c.A );
      else
        sb.AppendFormat( "[{0},{1},{2}] = #{0:X02}{1:X02}{2:X02}", c.R, c.G, c.B );

      Text = sb.ToString();
    }

    private void buttonOpen_Click ( object sender, EventArgs e )
    {
      OpenFileDialog ofd = new OpenFileDialog();

      ofd.Title = "Open Image File";
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

      newImage( ofd.FileName );
    }

    /// <summary>
    /// Sets the new image defined by its file-name. Does all the checks.
    /// </summary>
    private bool newImage ( string fn )
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
      setImage( ref outputImage, null );

      recompute();

      return true;
    }

    protected void EnableGUI ( bool enable )
    {
      buttonRedraw.Enabled =
      buttonOpen.Enabled   =
      buttonSave.Enabled   =
      textParam.Enabled    = enable;
      buttonStop.Enabled   = !enable;
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
        EnableGUI( true );
      }
    }

    private void transform ()
    {
      Stopwatch sw = new Stopwatch();
      sw.Start();

      Bitmap bmp = Filter.Recompute( inputImage, textParam.Text );

      sw.Stop();
      float elapsed = 1.0e-3f * sw.ElapsedMilliseconds;

      SetImage( bmp );
      SetText( string.Format( CultureInfo.InvariantCulture, "Elapsed: {0:f3}s ({1})", elapsed, bmp.PixelFormat.ToString() ) );

      StopComputation();
    }

    private void recompute ()
    {
      if ( aThread != null ||
           inputImage == null )
        return;

      EnableGUI( false );
      cont = true;

      aThread = new Thread( new ThreadStart( this.transform ) );
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

    private void Form1_DragDrop ( object sender, DragEventArgs e )
    {
      string[] strFiles = (string[])e.Data.GetData( DataFormats.FileDrop );
      newImage( strFiles[ 0 ] );
    }

    private void Form1_DragEnter ( object sender, DragEventArgs e )
    {
      if ( e.Data.GetDataPresent( DataFormats.FileDrop ) )
        e.Effect = DragDropEffects.Copy;
    }

    private void Form1_FormClosing ( object sender, FormClosingEventArgs e )
    {
      StopComputation();
    }

    private void pictureBox1_MouseDown ( object sender, MouseEventArgs e )
    {
      if ( aThread == null &&
           e.Button == MouseButtons.Left )
        imageProbe( e.X, e.Y );
    }

    private void pictureBox1_MouseMove ( object sender, MouseEventArgs e )
    {
      if ( aThread == null &&
           e.Button == MouseButtons.Left )
        imageProbe( e.X, e.Y );
    }

    private void textParam_KeyPress ( object sender, KeyPressEventArgs e )
    {
      if ( e.KeyChar == (char)Keys.Enter )
      {
        e.Handled = true;
        recompute();
      }
    }

    private void textParam_MouseHover ( object sender, EventArgs e )
    {
      tt.Show( tooltip, (IWin32Window)sender, 10, -25, 2000 );
    }

    private void labelElapsed_MouseHover ( object sender, EventArgs e )
    {
      tt.Show( Util.TargetFramework + " (" + Util.RunningFramework + ')',
               (IWin32Window)sender, 10, -25, 2000 );
    }
  }
}
