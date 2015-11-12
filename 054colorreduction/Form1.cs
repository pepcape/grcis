using System;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using Raster;
using System.Globalization;
using System.Diagnostics;

namespace _054colorreduction
{
  public partial class Form1 : Form
  {
    protected Image inputImage = null;

    protected Bitmap outputImage = null;

    public Form1 ()
    {
      InitializeComponent();

      String[] tok = "$Rev$".Split( ' ' );
      Text += " (rev: " + tok[ 1 ] + ')';
    }

    protected Thread aThread = null;

    volatile public static bool cont = true;

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
        pictureBox1.Image = null;
        if ( outputImage != null )
          outputImage.Dispose();
        pictureBox1.Image = outputImage = newImage;
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

      if ( inputImage != null )
        inputImage.Dispose();
      inputImage = Image.FromFile( ofd.FileName );

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
        Bitmap ibmp = (Bitmap)inputImage;
        Bitmap bmp;
        Stopwatch sw = new Stopwatch();
        sw.Start();

        ColorReduction.Reduce( ibmp, out bmp, textParam.Text );

        sw.Stop();
        float elapsed = 1.0e-3f * sw.ElapsedMilliseconds;
        SetImage( bmp );

        // Image differences:
        FloatImage ar, ag, ab, br, bg, bb;
        FloatImage.BitmapBands( ibmp, 1, out ar, out ag, out ab );
        FloatImage.BitmapBands( bmp, 1, out br, out bg, out bb );

        // Simple differences:
        float dr = ar.MAD( br );
        float dg = ag.MAD( bg );
        float db = ab.MAD( bb );
        float diff = (dr + dg + db) / 3.0f;
        float diffw = (dr * Draw.RED_WEIGHT + dg * Draw.GREEN_WEIGHT + db * Draw.BLUE_WEIGHT) / Draw.WEIGHT_TOTAL;

        // Conical blur differences:
        ar.Blur(); ar.Blur();
        ag.Blur(); ag.Blur();
        ab.Blur(); ab.Blur();
        br.Blur(); br.Blur();
        bg.Blur(); bg.Blur();
        bb.Blur(); bb.Blur();
        dr = ar.MAD( br );
        dg = ag.MAD( bg );
        db = ab.MAD( bb );
        float diffg = (dr * Draw.RED_WEIGHT + dg * Draw.GREEN_WEIGHT + db * Draw.BLUE_WEIGHT) / Draw.WEIGHT_TOTAL;

        MessageBox.Show( string.Format( CultureInfo.InvariantCulture, "Image: {5}x{6} ({7}){0}Time: {1:f3}s{0}plain MAD: {2:f5}{0}weighted MAD: {3:f5}{0}filtered weighted MAD: {4:f5}",
                                        Environment.NewLine, elapsed, diff, diffw, diffg, ibmp.Width, ibmp.Height, ibmp.PixelFormat.ToString() ), "MAE Difference" );
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
      sfd.ShowDialog();

      if ( sfd.FileName == "" )
        return;

      outputImage.Save( sfd.FileName, System.Drawing.Imaging.ImageFormat.Png );
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
