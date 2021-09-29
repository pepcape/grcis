using System;
using System.Drawing;
using System.Windows.Forms;
using MathSupport;

namespace _051colormap
{
  public partial class Form1 : Form
  {
    static readonly string rev = "$Rev: 974 $".Split(' ')[1];

    public static Color[] colors;
    private static Bitmap inputImage = null;
    public static int numCol = 6;

    public Form1 ()
    {
      InitializeComponent();
      string author;
      Colormap.InitForm(out author);
      Text += " (rev: " + rev + ") '" + author + '\'';

      colors = new Color[ 6 ];
      colors[0] = Color.FromArgb(  0, 100, 127);
      colors[1] = Color.FromArgb(150, 150,  80);
      colors[2] = Color.FromArgb(255, 100,  20);
      colors[3] = Color.FromArgb( 20, 200,  20);
      colors[4] = Color.FromArgb(  0, 250, 160);
      colors[5] = Color.FromArgb( 20,  20, 255);
      pictureBox1.Invalidate();
    }

    private void setImage (ref Bitmap bakImage, Bitmap newImage)
    {
      pictureInput.Image = newImage;
      if (bakImage != null)
        bakImage.Dispose();
      bakImage = newImage;
    }

    private void buttonClose_Click (object sender, EventArgs e)
    {
      Close();
    }

    private void numColors_ValueChanged (object sender, EventArgs e)
    {
      numCol = Arith.Clamp((int)numColors.Value, 3, 10);

      if (inputImage != null)
      {
        Colormap.Generate( inputImage, numCol, out colors );
        pictureBox1.Invalidate();
      }
    }

    public static string color2string (Color col)
    {
      return "[" + col.R + ',' + col.G + ',' + col.B + ']';
    }

    private void buttonLoad_Click (object sender, EventArgs e)
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
      if (ofd.ShowDialog() != DialogResult.OK)
        return;

      setImage(ref inputImage, (Bitmap)Image.FromFile(ofd.FileName));

      Colormap.Generate(inputImage, numCol, out colors);
      pictureBox1.Invalidate();
    }
  }
}
