using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Raster;
using System.Drawing.Imaging;
using System.Globalization;

namespace _010quadtree
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

    private void buttonGenerate_Click ( object sender, EventArgs e )
    {
      Cursor.Current = Cursors.WaitCursor;

      int width  = (int)numericXres.Value;
      int height = (int)numericYres.Value;
      Bitmap newImage = new Bitmap( width, height, PixelFormat.Format24bppRgb );
      int seed   = (int)numericSeed.Value;
      Random rnd = (seed == 0) ? new Random() : new Random( seed );

      for ( int i = 0; i++ < 32; )
      {
        int x1 = rnd.Next( width );
        int y1 = rnd.Next( height );
        int x2 = rnd.Next( width );
        int y2 = rnd.Next( height );
        Draw.Line( newImage, x1, y1, x2, y2, Color.Yellow );
      }

      setImage( ref inputImage, newImage );
      resetImage( ref outputImage );
      resetImage( ref diffImage );

      Cursor.Current = Cursors.Default;
    }

    private void buttonRecode_Click ( object sender, EventArgs e )
    {
      Cursor.Current = Cursors.WaitCursor;

      if ( inputImage == null )
        inputImage = (Bitmap)Image.FromFile( "toucan.png" );

      pictureBox1.Image = inputImage;
      resetImage( ref outputImage );
      resetImage( ref diffImage );

      Stopwatch sw = new Stopwatch();
      sw.Start();

      // 1. quad-tree encoding
      QuadTree qt = new QuadTree();
      qt.EncodeTree( inputImage );

      // 2. quad-tree write (disk file)
      FileStream fs = new FileStream( "qtree.txt", FileMode.Create );
      qt.WriteTree( fs );
      fs.Flush();
      long fileSize = fs.Position;

      sw.Stop();
      labelElapsed.Text = string.Format( CultureInfo.InvariantCulture, "Enc: {0:f}s, {1}kb",
                                         1.0e-3 * sw.ElapsedMilliseconds, (fileSize + 1023L) >> 10 );

      // 3. quad-tree re-read (disk file)
      fs.Seek( 0L, SeekOrigin.Begin );
      qt = new QuadTree();
      bool result = qt.ReadTree( fs );
      fs.Close();
      if ( result )
      {
        // 4. quad-tree rendering
        outputImage = qt.DecodeTree();

        // 5. comparison
        diffImage = new Bitmap( inputImage.Width, inputImage.Height, PixelFormat.Format24bppRgb );
        long diffHash = Draw.ImageCompare( inputImage, outputImage, diffImage );
        labelResult.Text = string.Format( "Errs: {0}", diffHash );
        pictureBox1.Image = checkDiff.Checked ? diffImage : outputImage;
      }
      else
      {
        labelResult.Text = "File error";
        pictureBox1.Image = null;
        outputImage = diffImage = null;
      }

      Cursor.Current = Cursors.Default;
    }

    private void buttonSave_Click ( object sender, EventArgs e )
    {
      if ( outputImage == null ||
           diffImage == null ) return;

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
