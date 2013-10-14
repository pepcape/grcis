using System;
using System.Drawing;
using System.Windows.Forms;

namespace _066histogram
{
  public partial class Form1 : Form
  {
    /// <summary>
    /// Source image.
    /// </summary>
    public Image inputImage = null;

    /// <summary>
    /// Input file-name.
    /// </summary>
    public string fileName = null;

    /// <summary>
    /// Modeless histogram window.
    /// </summary>
    public HistogramForm histogramForm = null;

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

      Image inp = Image.FromFile( fileName = ofd.FileName );
      inputImage = new Bitmap( inp );
      inp.Dispose();

      recompute();
    }

    /// <summary>
    /// Recomputes the histogram window (forces input image pass).
    /// </summary>
    private void recompute ()
    {
      if ( inputImage == null ) return;

      dirty = true;
      pictureBox1.Image = (Bitmap)inputImage;

      if ( histogramForm == null )
      {
        histogramForm = new HistogramForm( this );
        histogramForm.Show();
      }
      else
        histogramForm.Invalidate();

      histogramForm.Text = "Histogram (" + fileName + ')';
    }

    private void buttonHistogram_Click ( object sender, EventArgs e )
    {
      recompute();
    }
  }
}
