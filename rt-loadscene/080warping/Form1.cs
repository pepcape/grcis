using System;
using System.Drawing;
using System.Windows.Forms;

namespace _080warping
{
  public partial class Form1 : Form
  {
    static readonly string rev = "$Rev$".Split( ' ' )[ 1 ];

    protected Image inputImage  = null;
    protected Image outputImage = null;

    public Form1 ()
    {
      InitializeComponent();
      string author;
      int grid;
      Warping.InitForm( out author, out grid );
      Text += " (rev: " + rev + ") '" + author + '\'';
      numericParam.Value = grid;
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

      Image inp = Image.FromFile( ofd.FileName );
      if ( inputImage != null )
        inputImage.Dispose();
      inputImage = new Bitmap( inp );
      if ( outputImage != null )
        outputImage.Dispose();
      outputImage = new Bitmap( inp );
      inp.Dispose();

      Reset();
    }

    /// <summary>
    /// Reset source/target images with the new regular grid.
    /// </summary>
    private void Reset ()
    {
      if ( inputImage == null ) return;
      outputImage = new Bitmap( inputImage );

      int columns = (int)numericParam.Value;
      if ( columns < 4 ) columns = 4;
      int rows = (columns * inputImage.Height) / inputImage.Width;

      pictureSource.InitPicture( this, (Bitmap)inputImage, columns, rows );
      pictureTarget.InitPicture( this, (Bitmap)outputImage, columns, rows );
    }

    /// <summary>
    /// Recompute the target image according to current meshes.
    /// </summary>
    public void Recompute ()
    {
      if ( inputImage == null ) return;

      outputImage = pictureTarget.warp.Warp( (Bitmap)inputImage, pictureSource.warp );
      pictureTarget.SetPicture( (Bitmap)outputImage );

      pictureSource.Invalidate();
      pictureTarget.Invalidate();
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

    private void numericParam_ValueChanged ( object sender, EventArgs e )
    {
      Reset();
    }
  }
}
