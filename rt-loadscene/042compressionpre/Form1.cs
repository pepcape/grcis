using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using Raster;

namespace _042compressionpre
{
  public partial class Form1 : Form
  {
    static readonly string rev = "$Rev$".Split( ' ' )[ 1 ];

    protected Bitmap inputImage = null;
    protected Bitmap outputImage = null;
    protected Bitmap diffImage = null;

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

    private void resetImage ( ref Bitmap bakImage )
    {
      if ( bakImage != null )
        bakImage.Dispose();
      bakImage = null;
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
      resetImage( ref outputImage );
      resetImage( ref diffImage );
    }

    private void buttonRecode_Click ( object sender, EventArgs e )
    {
      if ( inputImage == null ) return;
      Cursor.Current = Cursors.WaitCursor;

      pictureBox1.Image = inputImage;
      resetImage( ref outputImage );
      resetImage( ref diffImage );

      Stopwatch sw = new Stopwatch();
      sw.Start();

      // 1. image encoding
      PreCodec codec = new PreCodec();
      FileStream fs = new FileStream( "code.bin", FileMode.Create );
      codec.EncodeImage( inputImage, fs );

      // 2. code size
      fs.Flush();
      long fileSize = fs.Position;

      sw.Stop();
      labelElapsed.Text = string.Format( CultureInfo.InvariantCulture, "Enc: {0:f3}s, {1:f1}kb", 1.0e-3 * sw.ElapsedMilliseconds, fileSize / 1024.0 );

      // 3. image decoding
      fs.Seek( 0L, SeekOrigin.Begin );
      outputImage = codec.DecodeImage( fs );
      fs.Close();

      // 5. comparison
      if ( outputImage != null )
      {
        diffImage = new Bitmap( inputImage.Width, inputImage.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb );
        long diffHash = Draw.ImageCompare( inputImage, outputImage, diffImage );
        labelResult.Text = string.Format( CultureInfo.InvariantCulture, "Errs: {0}", diffHash );
        pictureBox1.Image = checkDiff.Checked ? diffImage : outputImage;
      }
      else
      {
        labelResult.Text = "File error";
        pictureBox1.Image = null;
        diffImage = null;
      }

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

      if ( checkDiff.Checked )
        diffImage.Save( sfd.FileName, System.Drawing.Imaging.ImageFormat.Png );
      else
        outputImage.Save( sfd.FileName, System.Drawing.Imaging.ImageFormat.Png );
    }

    private void checkDiff_CheckedChanged ( object sender, EventArgs e )
    {
      pictureBox1.Image = checkDiff.Checked ? diffImage : outputImage;
    }
  }
}
