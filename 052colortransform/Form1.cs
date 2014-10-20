using System;
using System.Drawing;
using System.Windows.Forms;

namespace _052colortransform
{
  public partial class Form1 : Form
  {
    protected Image inputImage = null;

    public Form1 ()
    {
      InitializeComponent();
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
      inputImage = new Bitmap( inp );
      inp.Dispose();

      recompute();
    }

    private void recompute ()
    {
      if ( inputImage == null ) return;

      Bitmap ibmp = (Bitmap)inputImage;
      Bitmap bmp;
      Transform.TransformImage( ibmp, out bmp, textParam.Text );
      pictureBox1.Image = bmp;
    }

    private void buttonSave_Click ( object sender, EventArgs e )
    {
      if ( inputImage == null ) return;

      SaveFileDialog sfd = new SaveFileDialog();
      sfd.Title = "Save PNG file";
      sfd.Filter = "PNG Files|*.png";
      sfd.AddExtension = true;
      sfd.FileName = "";
      sfd.ShowDialog();

      if ( sfd.FileName == "" )
        return;

      pictureBox1.Image.Save( sfd.FileName, System.Drawing.Imaging.ImageFormat.Png );
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
      Bitmap bitmap = inputImage as Bitmap;
      if ( bitmap == null ) return;

      Color pix = bitmap.GetPixel( x, y );
      textParam.Text = oldParam + '[' + pix.R + ',' + pix.G + ',' + pix.B + ']';
    }
  }
}
