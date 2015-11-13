using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace _005denoise
{
  public partial class Form1 : Form
  {
    protected Bitmap inputImage = null;
    protected Bitmap outputImage = null;

    static readonly string rev = "$Rev$".Split( ' ' )[ 1 ];

    public Form1 ()
    {
      InitializeComponent();
      Text += " (rev: " + rev + ')';
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

      pictureBox1.Image = null;
      if ( inputImage != null )
        inputImage.Dispose();
      inputImage = (Bitmap)Image.FromFile( ofd.FileName );

      recompute();
    }

    private void recompute ()
    {
      if ( inputImage == null ) return;

      Cursor.Current = Cursors.WaitCursor;
      pictureBox1.Image = inputImage;
      if ( outputImage != null )
        outputImage.Dispose();

      Denoise.TransformImage( inputImage, out outputImage, (double)numericParam.Value );

      pictureBox1.Image = outputImage;
      Cursor.Current = Cursors.Default;
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

    private void buttonRecalc_Click ( object sender, EventArgs e )
    {
      recompute();
    }
  }
}
