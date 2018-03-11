using System;
using System.Drawing;
using System.Windows.Forms;

namespace _052colortransform
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

    private void setImage ( ref Bitmap bakImage, Bitmap newImage )
    {
      pictureBox1.Image = newImage;
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

      setImage( ref outputImage, null );
      setImage( ref inputImage, (Bitmap)Image.FromFile( ofd.FileName ) );

      recompute();
    }

    private void recompute ()
    {
      if ( inputImage == null ) return;

      Bitmap newImage;
      Transform.TransformImage( inputImage, out newImage, textParam.Text );

      setImage( ref outputImage, newImage );
      checkOriginal.Checked = false;
    }

    private void buttonSave_Click ( object sender, EventArgs e )
    {
      if ( inputImage == null ||
           outputImage == null ) return;

      SaveFileDialog sfd = new SaveFileDialog();
      sfd.Title = "Save PNG file";
      sfd.Filter = "PNG Files|*.png";
      sfd.AddExtension = true;
      sfd.FileName = "";
      if ( sfd.ShowDialog() != DialogResult.OK )
        return;

      outputImage.Save( sfd.FileName, System.Drawing.Imaging.ImageFormat.Png );
    }

    private void buttonRedraw_Click ( object sender, EventArgs e )
    {
      recompute();
    }

    private void pictureBox1_MouseDoubleClick ( object sender, MouseEventArgs e )
    {
      int x = e.Location.X;
      int y = e.Location.Y;
      string oldParam = textParam.Text;
      if ( e.Button == MouseButtons.Left )
      {
        int pos = oldParam.IndexOf( '[' );
        if ( pos >= 0 )
          oldParam = oldParam.Substring( 0, pos );
      }

      if ( inputImage == null ) return;

      Color pix = inputImage.GetPixel( x, y );
      textParam.Text = oldParam + '[' + pix.R + ',' + pix.G + ',' + pix.B + ']';
    }

    private void checkOriginal_CheckedChanged ( object sender, EventArgs e )
    {
      if ( inputImage == null ) return;

      if ( checkOriginal.Checked )
        pictureBox1.Image = inputImage;
      else
        if ( outputImage == null )
          recompute();
        else
          pictureBox1.Image = outputImage;
    }
  }
}
