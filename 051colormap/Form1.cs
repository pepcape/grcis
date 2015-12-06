using System;
using System.Drawing;
using System.Windows.Forms;

namespace _051colormap
{
  public partial class Form1 : Form
  {
    static readonly string rev = "$Rev$".Split( ' ' )[ 1 ];

    public static Color[] colors;
    private static Bitmap inputImage = null;
    public static int numCol = 3;

    public Form1 ()
    {
      InitializeComponent();
      Text += " (rev: " + rev + ')';

      colors = new Color[ 3 ];
      colors[ 0 ] = Color.FromArgb(   0, 100, 127 );
      colors[ 1 ] = Color.FromArgb( 150, 150,  80 );
      colors[ 2 ] = Color.FromArgb( 255, 100,  20 );
      pictureBox1.Invalidate();
    }

    private void setImage ( ref Bitmap bakImage, Bitmap newImage )
    {
      pictureInput.Image = newImage;
      if ( bakImage != null )
        bakImage.Dispose();
      bakImage = newImage;
    }

    private void buttonClose_Click ( object sender, EventArgs e )
    {
      this.Close();
    }

    private void numColors_ValueChanged ( object sender, EventArgs e )
    {
      numCol = (int)numColors.Value;
      if ( numCol > 8 ) numCol = 8;
      if ( numCol < 3 ) numCol = 3;

      if ( inputImage != null )
      {
        Colormap.Generate( inputImage, numCol, out colors );
        pictureBox1.Invalidate();
      }
    }

    public static string color2string ( Color col )
    {
      return "[" + col.R + ',' + col.G + ',' + col.B + ']';
    }

    private void buttonLoad_Click ( object sender, EventArgs e )
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

      setImage( ref inputImage, (Bitmap)Image.FromFile( ofd.FileName ) );

      Colormap.Generate( inputImage, numCol, out colors );
      pictureBox1.Invalidate();
    }
  }
}
