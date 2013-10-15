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
    /// Parameter read from Form1.
    /// </summary>
    public string param = "";

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

      newImage( ofd.FileName );
    }

    /// <summary>
    /// Sets the new image defined by its file-name. Does all the checks.
    /// </summary>
    private bool newImage ( string fn )
    {
      Image inp = null;
      try
      {
        inp = Image.FromFile( fn );
      }
      catch ( Exception )
      {
        return false;
      }
      fileName = fn;
      inputImage = new Bitmap( inp );
      inp.Dispose();

      recompute();
      return true;
    }

    /// <summary>
    /// Recomputes the histogram window (forces input image pass).
    /// </summary>
    private void recompute ()
    {
      if ( inputImage == null ) return;

      dirty = true;
      pictureBox1.Image = (Bitmap)inputImage;
      param = textParam.Text;

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

    private void Form1_DragDrop ( object sender, DragEventArgs e )
    {
      string[] strFiles = (string[])e.Data.GetData( DataFormats.FileDrop );
      newImage( strFiles[ 0 ] );
    }

    private void Form1_DragEnter ( object sender, DragEventArgs e )
    {
      if ( e.Data.GetDataPresent( DataFormats.FileDrop ) )
        e.Effect = DragDropEffects.Copy;
    }

    private void textParam_KeyPress ( object sender, KeyPressEventArgs e )
    {
      if ( e.KeyChar == (char)Keys.Enter )
      {
        e.Handled = true;
        recompute();
      }
    }
  }
}
