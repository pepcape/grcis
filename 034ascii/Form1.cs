using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace _034ascii
{
  public partial class Form1 : Form
  {
    static readonly string rev = "$Rev$".Split( ' ' )[ 1 ];

    /// <summary>
    /// The bitmap to be used as source data
    /// </summary>
    protected Bitmap inputImage = null;

    /// <summary>
    /// Default output size in characters.
    /// </summary>
    protected const int BiggerSize = 100;

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

    private void btnOpen_Click ( object sender, EventArgs e )
    {
      OpenFileDialog dlg = new OpenFileDialog();

      dlg.Title = "Open Image File";
      dlg.Filter = "Bitmap Files|*.bmp" +
          "|Gif Files|*.gif" +
          "|JPEG Files|*.jpg" +
          "|PNG Files|*.png" +
          "|TIFF Files|*.tif" +
          "|All image types|*.bmp;*.gif;*.jpg;*.png;*.tif";

      dlg.FilterIndex = 6;

      if ( dlg.ShowDialog() != DialogResult.OK )
        return;

      try
      {
        setImage( ref inputImage, (Bitmap)Image.FromFile( dlg.FileName ) );
      }
      catch ( Exception exc )
      {
        setImage( ref inputImage, null );
        MessageBox.Show( exc.Message );
      }

      if ( inputImage == null ) return;

      int w = inputImage.Width;
      int h = (inputImage.Height * 7) / 11;
      // Lucida Console size: 7 x 11 px
      double factor = 1.0;

      if ( w > h )
        factor = (double)BiggerSize / (double)w;
      else
        factor = (double)BiggerSize / (double)h;

      txtHeight.Text = Math.Round( (double)h * factor ).ToString();
      txtWidth.Text  = Math.Round( (double)w * factor ).ToString();

    }

    private void btnConvert_Click ( object sender, EventArgs e )
    {
      if ( inputImage == null ) return;

      int w = 100, h = 100;
      int.TryParse( txtWidth.Text, out w );
      int.TryParse( txtHeight.Text, out h );

      string text = AsciiArt.Process( inputImage, w, h, textParam.Text );
      Font fnt = AsciiArt.GetFont();

      Output dlgOut = new Output();
      dlgOut.WndText = text;
      dlgOut.Fnt = fnt;
      dlgOut.ShowDialog();
    }
  }
}
