using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Runtime.ExceptionServices;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Raster;
using Support;

namespace _055quadtree
{
  public partial class Form1 : Form
  {
    static readonly string rev = "$Rev$".Split( ' ' )[ 1 ];

    protected Bitmap inputImage = null;
    protected Bitmap outputImage = null;
    protected Bitmap diffImage = null;

    const string CONFIG_FILE = "config.txt";

    const string LOG_FILE = "log.txt";

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
      if ( inputImage == null )
      {
        if ( File.Exists( CONFIG_FILE ) )
        {
          BatchRecode();
          return;
        }

        setImage( ref inputImage, (Bitmap)Image.FromFile( "toucan.png" ) );
      }

      setImage( ref outputImage, null );
      resetImage( ref diffImage );

      Cursor.Current = Cursors.WaitCursor;

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
      labelElapsed.Text = string.Format( CultureInfo.InvariantCulture, "Enc: {0:f2}s, {1}b",
                                         1.0e-3 * sw.ElapsedMilliseconds, fileSize );

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
        setImage( ref outputImage, null );
        resetImage( ref diffImage );
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
        diffImage.Save( sfd.FileName, ImageFormat.Png );
      else
        outputImage.Save( sfd.FileName, ImageFormat.Png );
    }

    private void checkDiff_CheckedChanged ( object sender, EventArgs e )
    {
      pictureBox1.Image = checkDiff.Checked ? diffImage : outputImage;
    }

    [HandleProcessCorruptedStateExceptions]
    private void BatchRecode ()
    {
      StreamReader inp = new StreamReader( CONFIG_FILE );
      string line;

      line = inp.ReadLine();
      if ( line == null )
      {
        inp.Close();
        return;
      }

      // 1st line: author's name
      string fullName = line.Trim();
      string strippedName = Regex.Replace( TextUtils.RemoveDiacritics( fullName ), @"\s", "" );

      // following lines: test images
      List<string> fn = new List<string>();
      while ( (line = inp.ReadLine()) != null )
        if ( (line = line.Trim()).Length > 0 &&
             File.Exists( line ) )
          fn.Add( line );
      int fnWidth = 0;
      foreach ( string name in fn )
        if ( name.Length > fnWidth )
          fnWidth = name.Length;
      inp.Close();

      // input images one by one:
      Stopwatch sw = new Stopwatch();
      StreamWriter log = new StreamWriter( LOG_FILE, true );
      // <input-file> <txt-size> <enc-elapsed> <dec-elapsed> <author-name>
      string fmt = "{0,-" + fnWidth + "}{1,10}{2,9}{3,9} {4} {5}";
      long diffHash = 1L;

      foreach ( string name in fn )
      {
        Image input = Image.FromFile( name );
        Bitmap inputImage = new Bitmap( input );
        FileStream fs;
        long fileSize = 0L;
        long encElapsed = 0L;
        long decElapsed = 0L;
        string err = "";
        input.Dispose();

        try
        {
          sw.Restart();

          // 1. quad-tree encoding
          QuadTree qt = new QuadTree();
          qt.EncodeTree( inputImage );

          // 2. quad-tree write (disk file)
          fs = new FileStream( string.Format( "{0}.{1}.txt", name, strippedName ), FileMode.Create );
          qt.WriteTree( fs );
          fs.Flush();
          fileSize = fs.Position;

          sw.Stop();
          encElapsed = sw.ElapsedMilliseconds;

          // 3. quad-tree re-read (disk file)
          fs.Seek( 0L, SeekOrigin.Begin );
          sw.Restart();
          qt = new QuadTree();
          bool result = qt.ReadTree( fs );
          fs.Close();
          decElapsed = 0L;
          if ( result )
          {
            // 4. quad-tree rendering
            outputImage = qt.DecodeTree();
            sw.Stop();
            decElapsed = sw.ElapsedMilliseconds;

            // 5. comparison
            diffHash = Draw.ImageCompare( inputImage, outputImage, null );
            if ( diffHash != 0L )
              err = diffHash.ToString();
          }
          else
          {
            err = "ENCODING-ERROR";
            outputImage = null;
          }
        }
        catch ( Exception )
        {
          err = "EXCEPTION";
          outputImage = null;
        }

        // 6. output log
        log.WriteLine( string.Format( CultureInfo.InvariantCulture, fmt, name, fileSize, encElapsed, decElapsed, err, fullName ) );
      }

      log.Close();
      labelResult.Text = string.Format( "Errs: {0}", diffHash );
      pictureBox1.Image = outputImage;
    }
  }
}
