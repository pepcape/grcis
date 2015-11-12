using System;
using System.Drawing;
using System.Windows.Forms;

namespace _036circles
{
  public partial class Form1 : Form
  {
    protected Image inputImage = null;

    protected Image outputImage = null;

    static readonly string rev = "$Rev$".Split( ' ' )[ 1 ];

    public Form1 ()
    {
      InitializeComponent();
      Text += " (rev: " + rev + ')';
    }

    private void buttonLoadImage_Click ( object sender, EventArgs e )
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

      string txt = string.Format( "Input: {0} ({1}x{2}, {3})",
                                  ofd.FileName, inputImage.Width, inputImage.Height, inputImage.PixelFormat.ToString() );
      labelImageName.Text = txt;
    }

    private void buttonRedraw_Click ( object sender, EventArgs e )
    {
      int width    = (int)numericXres.Value;
      int height   = (int)numericYres.Value;

      Canvas c = new Canvas( width, height );

      Circles.Draw( c, (Bitmap)inputImage, textParam.Text );

      pictureBox1.Image = outputImage = c.Finish();
      buttonSave.Enabled = true;
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
  }
}
