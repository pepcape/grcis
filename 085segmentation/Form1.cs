using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Diagnostics;
using System.Globalization;

namespace _085segmentation
{
  public partial class Form1 : Form
  {
    protected Bitmap inputImage  = null;

    protected Bitmap outputImage = null;

    public Form1 ()
    {
      InitializeComponent();
      String[] tok = "$Rev$".Split( ' ' );
      Text += " (rev: " + tok[ 1 ] + ')';
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
      inputImage = (Bitmap)Image.FromFile( ofd.FileName );

      Reset();
    }

    /// <summary>
    /// Reset source/target images.
    /// </summary>
    private void Reset ()
    {
      if ( inputImage == null ) return;

      pictureTarget.Image = null;
      if ( outputImage != null )
        outputImage.Dispose();

      pictureTarget.Image = outputImage = pictureSource.InitPicture( this, inputImage );
    }

    /// <summary>
    /// Recompute the target image according to current mask.
    /// </summary>
    public void Recompute ()
    {
      if ( inputImage == null ) return;

      pictureTarget.Image = null;
      if ( outputImage != null )
        outputImage.Dispose();

      Stopwatch sw = new Stopwatch();
      sw.Start();
      
      outputImage = pictureSource.segm.DoSegmentation( inputImage, checkBoxWhite.Checked );

      sw.Stop();
      labelElapsed.Text = String.Format( CultureInfo.InvariantCulture, "Elapsed: {0:f1}s", 1.0e-3 * sw.ElapsedMilliseconds );
      pictureTarget.Image = outputImage;
    }

    private void buttonSave_Click ( object sender, EventArgs e )
    {
      if ( outputImage == null ) return;

      SaveFileDialog sfd = new SaveFileDialog();
      sfd.Title = "Save result image";
      sfd.Filter = "PNG Files|*.png";
      sfd.AddExtension = true;
      sfd.FileName = "";
      if ( sfd.ShowDialog() != DialogResult.OK )
        return;

      outputImage.Save( sfd.FileName, ImageFormat.Png );
    }

    private void buttonRecompute_Click ( object sender, EventArgs e )
    {
      Recompute();
    }

    private void buttonReset_Click ( object sender, EventArgs e )
    {
      Reset();
    }

    private void buttonSaveMask_Click ( object sender, EventArgs e )
    {
      if ( pictureSource == null ||
           pictureSource.segm == null )
        return;

      Bitmap mask = pictureSource.segm.Mask;

      if ( mask == null )
        return;

      SaveFileDialog sfd = new SaveFileDialog();
      sfd.Title = "Save input mask";
      sfd.Filter = "PNG Files|*.png";
      sfd.AddExtension = true;
      sfd.FileName = "";
      if ( sfd.ShowDialog() != DialogResult.OK )
        return;

      mask.Save( sfd.FileName, ImageFormat.Png );
    }

    private void buttonLoadMask_Click ( object sender, EventArgs e )
    {
      if ( pictureSource == null ||
           pictureSource.segm == null )
        return;

      OpenFileDialog ofd = new OpenFileDialog();

      ofd.Title = "Open input mask";
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

      pictureSource.segm.Mask = (Bitmap)Image.FromFile( ofd.FileName );
      pictureSource.Invalidate();
    }

    private void numericPen_ValueChanged ( object sender, EventArgs e )
    {
      if ( pictureSource == null ||
           pictureSource.segm == null )
        return;

      pictureSource.segm.TraceSize = (double)numericPen.Value;
    }
  }
}
