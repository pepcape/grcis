using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;
using Raster;

namespace _054colorreduction
{
  public partial class Form1 : Form
  {
    static readonly string rev = "$Rev$".Split( ' ' )[ 1 ];

    protected Bitmap inputImage = null;
    protected Bitmap outputImage = null;

    public Form1 ()
    {
      InitializeComponent();
      Text += " (rev: " + rev + ')';
    }

    protected Thread aThread = null;

    volatile public static bool cont = true;

    private void setImage ( ref Bitmap bakImage, Bitmap newImage )
    {
      pictureBox1.Image = newImage;
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

      setImage( ref inputImage, (Bitmap)Image.FromFile( ofd.FileName ) );
      setImage( ref outputImage, null );

      recompute();
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
        buttonRedraw.Enabled = true;
        buttonOpen.Enabled = true;
        buttonSave.Enabled = true;
        buttonStop.Enabled = false;
      }
    }

    private void reduce ()
    {
      if ( inputImage != null )
      {
        Stopwatch sw = new Stopwatch();
        sw.Start();

        Bitmap bmp;
        ColorReduction.Reduce( inputImage, out bmp, textParam.Text );

        sw.Stop();
        float elapsed = 1.0e-3f * sw.ElapsedMilliseconds;
        SetImage( (Bitmap)bmp.Clone() );

        // Image differences:
        FloatImage a = new FloatImage( inputImage, 1 );
        FloatImage b = new FloatImage( bmp, 1 );

        // Simple differences:
        float dr = a.MAD( b, 0 );
        float dg = a.MAD( b, 1 );
        float db = a.MAD( b, 2 );
        float diff = (dr + dg + db) / 3.0f;
        float diffw = (dr * Draw.RED_WEIGHT + dg * Draw.GREEN_WEIGHT + db * Draw.BLUE_WEIGHT) / Draw.WEIGHT_TOTAL;

        // Conical blur differences:
        a.Blur();
        b.Blur();
        dr = a.MAD( b );

        MessageBox.Show( string.Format( CultureInfo.InvariantCulture, "Image: {5}x{6} ({7}){0}Time: {1:f3}s{0}plain MAD: {2:f5}{0}weighted MAD: {3:f5}{0}filtered MAD: {4:f5}",
                                        Environment.NewLine, elapsed, diff, diffw, dr,
                                        inputImage.Width, inputImage.Height, inputImage.PixelFormat.ToString() ), "MAE Difference" );
        bmp.Dispose();
      }

      StopComputation();
    }

    private void recompute ()
    {
      if ( aThread != null )
        return;

      buttonRedraw.Enabled = false;
      buttonOpen.Enabled = false;
      buttonSave.Enabled = false;
      buttonStop.Enabled = true;
      cont = true;

      aThread = new Thread( new ThreadStart( this.reduce ) );
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
  }
}
