using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using Raster;

namespace _009floodfill
{
  public partial class Form1 : Form
  {
    public Form1 ()
    {
      InitializeComponent();
    }

    private void redraw ()
    {
      const int SIZE = 800;

      Cursor.Current = Cursors.WaitCursor;
      Bitmap output = new Bitmap( SIZE, SIZE, System.Drawing.Imaging.PixelFormat.Format24bppRgb );
      int seed = (int)numericSeed.Value;
      Random rnd = (seed == 0) ? new Random() : new Random( seed );

      for ( int i = 0; i++ < 100;  )
      {
        int x1 = rnd.Next( SIZE );
        int y1 = rnd.Next( SIZE );
        int x2 = rnd.Next( SIZE );
        int y2 = rnd.Next( SIZE );
        Draw.Line( output, x1, y1, x2, y2, Color.Cyan );
      }

      // call flood-fill N times:
      int fills = (int)numericSize.Value;
      Stopwatch sw = new Stopwatch();
      sw.Start();

      for ( int i = 0; i++ < fills; )
      {
        int x = rnd.Next( SIZE );
        int y = rnd.Next( SIZE );
        Color col = Color.FromArgb( rnd.Next( 255 ), rnd.Next( 255 ), rnd.Next( 255 ) );
        Draw.FloodFill4( output, x, y, col );
      }

      sw.Stop();
      labelElapsed.Text = String.Format( "Elapsed: {0:f} s", 1.0e-3 * sw.ElapsedMilliseconds );

      long hash = Draw.Hash( output );
      labelHash.Text = String.Format( "{0:X}", hash );

      pictureBox1.Image = output;
      Cursor.Current = Cursors.Default;
    }

    private void buttonSave_Click ( object sender, EventArgs e )
    {
      SaveFileDialog sfd = new SaveFileDialog();
      sfd.Title = "Save PNG file";
      sfd.Filter = "PNG Files|*.png";
      sfd.AddExtension = true;
      sfd.FileName = "";
      if ( sfd.ShowDialog() != DialogResult.OK )
        return;

      pictureBox1.Image.Save( sfd.FileName, System.Drawing.Imaging.ImageFormat.Png );
    }

    private void buttonRedraw_Click ( object sender, EventArgs e )
    {
      redraw();
    }
  }
}
