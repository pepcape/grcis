using System;
using System.Drawing;
using System.Windows.Forms;

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

      pictureTarget.Image = outputImage = pictureSource.segm.DoSegmentation( inputImage );

      //pictureSource.Invalidate();
      //pictureTarget.Invalidate();
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

      outputImage.Save( sfd.FileName, System.Drawing.Imaging.ImageFormat.Png );
    }

    private void buttonRecompute_Click ( object sender, EventArgs e )
    {
      Recompute();
    }
  }
}
