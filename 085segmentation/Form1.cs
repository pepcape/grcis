using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Windows.Forms;

namespace _085segmentation
{
  public partial class Form1 : Form
  {
    static readonly string rev = "$Rev$".Split( ' ' )[ 1 ];

    protected Bitmap inputImage  = null;
    protected string inputImageName = null;
    protected Bitmap outputImage = null;

    public Form1 ()
    {
      InitializeComponent();
      Text += " (rev: " + rev + ')';
    }

    private void setImage ( ref Bitmap bakImage, Bitmap newImage )
    {
      pictureTarget.Image = newImage;
      if ( bakImage != null )
        bakImage.Dispose();
      bakImage = newImage;
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
      inputImageName = Path.GetFileName( ofd.FileName );
      labelElapsed.Text = string.Format( "Input: {0}", inputImageName ?? "" );

      Reset();
    }

    /// <summary>
    /// Reset source/target images.
    /// </summary>
    private void Reset ()
    {
      if ( inputImage == null ) return;

      setImage( ref outputImage, pictureSource.InitPicture( this, inputImage ) );
    }

    /// <summary>
    /// Recompute the target image according to current mask.
    /// </summary>
    public void Recompute ()
    {
      if ( inputImage == null ) return;

      Stopwatch sw = new Stopwatch();
      sw.Start();

      Bitmap newImage = pictureSource.segm.DoSegmentation( inputImage, checkBoxWhite.Checked );

      sw.Stop();
      labelElapsed.Text = string.Format( CultureInfo.InvariantCulture, "Elapsed: {0:f1}s ({1})",
                                         1.0e-3 * sw.ElapsedMilliseconds, inputImageName ?? "" );

      setImage( ref outputImage, newImage );
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
