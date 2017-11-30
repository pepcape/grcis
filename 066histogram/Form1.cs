using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Compression;
using Utilities;

namespace _066histogram
{
  public partial class Form1 : Form
  {
    static readonly string rev = Util.SetVersion( "$Rev$" );

    /// <summary>
    /// Source image.
    /// </summary>
    public Bitmap inputImage = null;

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

    /// <summary>
    /// Has the source image changed?
    /// </summary>
    public static bool dirty = false;

    public Form1 ()
    {
      InitializeComponent();

      string name, param;
      ImageHistogram.InitParams( out param, out name );
      textParam.Text = param;
      Text += " (" + rev + ") '" + name + '\'';
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
      if ( inputImage != null )
        inputImage.Dispose();
      inputImage = null;
      try
      {
        inputImage = (Bitmap)Image.FromFile( fn );
      }
      catch ( Exception )
      {
        return false;
      }
      fileName = fn;

      recomputeHistogram();

      return true;
    }

    /// <summary>
    /// Recomputes the histogram window (forces input image pass).
    /// </summary>
    private void recomputeHistogram ()
    {
      if ( inputImage == null ) return;

      dirty = true;
      pictureBox1.Image = inputImage;
      param = textParam.Text;

      if ( histogramForm == null )
      {
        histogramForm = new HistogramForm( this );
        histogramForm.Show();
      }
      else
        histogramForm.Invalidate();

      histogramForm.Text = "Histogram (" + fileName + ", " + inputImage.PixelFormat.ToString() + ')';
      labelStatus.Text = Path.GetFileName( fileName );
    }

    private void buttonHistogram_Click ( object sender, EventArgs e )
    {
      recomputeHistogram();
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
        recomputeHistogram();
      }
    }

    private void buttonPCM_Click ( object sender, EventArgs e )
    {
      if ( inputImage != null )
        labelStatus.Text = Path.GetFileName( fileName ) + ", PCM code = " + Compress.PCM( inputImage, textParam.Text ) + 'b';
    }

    private void buttonDPCM_Click ( object sender, EventArgs e )
    {
      if ( inputImage != null )
        labelStatus.Text = Path.GetFileName( fileName ) + ", DPCM code = " + Compress.DPCM( inputImage, textParam.Text ) + 'b';
    }
  }
}
