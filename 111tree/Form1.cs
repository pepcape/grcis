using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.Text;
using System.Windows.Forms;
using OpenTK;
using Raster;
using Utilities;

namespace _111tree
{
  public partial class Form1 : Form
  {
    static readonly string rev = Util.SetVersion( "$Rev$" );

    static string name;

    public Form1 ()
    {
      InitializeComponent();

      int setSize, querySize, K, seed;
      Tree.InitParams( out setSize, out querySize, out K, out seed, out name );
      numericSize.Value  = setSize;
      numericQuery.Value = querySize;
      numericK.Value     = K;
      numericSeed.Value  = seed;
      Text += " (" + rev + ") '" + name + '\'';
    }

    private List<Vector2> points = null;

    private Bitmap output = null;

    private Tree tree = null;

    const int SIZE = 600;

    const double PEN_SIZE = 1.5;

    private void buttonGenerate_Click ( object sender, EventArgs e )
    {
      // Lorentz attractor constants:
      // see http://paulbourke.net/fractals/lorenz/
      const double DT    = 0.004;
      const double DELTA = 10.0;
      const double R     = 28.0;
      const double B     = 8.0 / 3.0;

      // Lorentz attractor variables:
      double lx = 0.1;
      double ly = 0.0;
      double lz = 0.0;
      double dt, dx, dy, dz;

      Cursor.Current = Cursors.WaitCursor;

      // target image:
      pictureBox1.Image = null;
      if ( output != null )
        output.Dispose();
      if ( checkVisual.Checked )
        output = new Bitmap( SIZE, SIZE, PixelFormat.Format24bppRgb );
      else
        output = null;

      // random number generator:
      int seed = (int)numericSeed.Value;
      Random rnd = (seed == 0) ? new Random() : new Random( seed );

      // set size (number of points):
      int size = (int)numericSize.Value;
      points = new List<Vector2>( size );

      double x, y;
      for ( int i = 0; i < size; i++ )
      {
        do
        {
          dt = DT * (1.0 + rnd.NextDouble());
          dx = dt * (DELTA * (ly - lx));
          dy = dt * (lx * (R - lz) - ly);
          dz = dt * (lx * ly - B * lz);
          lx += dx;
          ly += dy;
          lz += dz;
          x =  0.5 + 0.04 * lx;
          y = -0.2 + 0.03 * lz;
        } while ( x < 0.0 || x > 1.0 ||
                  y < 0.0 || y > 1.0 );

        x *= SIZE;
        y *= SIZE;

        // the new point is ready:
        points.Add( new Vector2( (float)x, (float)y ) );

        // draw the point eventually:
        if ( output != null )
          Draw.Dot( output, (int)x, (int)y, PEN_SIZE, Color.Blue );
      }

      // Stopwatch - measuring the real time of KD-tree construction:
      Stopwatch sw = new Stopwatch();
      sw.Start();

      // Build the tree:
      tree = new Tree();
      tree.BuildTree( points );

      sw.Stop();
      // Build-phase statistics:
      long pointStat, boxStat, heapStat;
      tree.GetStatistics( out pointStat, out boxStat, out heapStat );
      string stat1 = string.Format( CultureInfo.InvariantCulture, "Elapsed: {0:f} s [{1:D}-{2:D}-{3:D}]",
                                    1.0e-3 * sw.ElapsedMilliseconds,
                                    pointStat, boxStat, heapStat );
      labelStat.Text = stat1;
      Util.Log( name + ':' );
      Util.LogFormat( "Build[ {0}, {1} ]: {2}", size, seed, stat1 );

      if ( output != null )
        pictureBox1.Image = output;

      Cursor.Current = Cursors.Default;
    }

    private void buttonQuery_Click ( object sender, EventArgs e )
    {
      Cursor.Current = Cursors.WaitCursor;

      // random number generator:
      int seed = (int)numericSeed.Value;
      Random rnd = (seed == 0) ? new Random() : new Random( seed );

      // set size (number of lookups):
      int size  = (int)numericQuery.Value;
      bool visual = (output != null) && checkVisual.Checked;

      int K = (int)numericK.Value;

      // hash accumulator:
      long hash = 0L;
      int equals = 0;
      int[] result = new int[ K ];
      SortedSet<int> resultSet = new SortedSet<int>();
      double colorMul = 1.0 / K;

      // reset the picture:
      if ( visual )
        foreach ( Vector2 p in points )
          Draw.Dot( output, (int)p.X, (int)p.Y, PEN_SIZE, Color.Blue );

      // Stopwatch - measuring the real time of tree queries:
      Stopwatch sw = new Stopwatch();
      long pointStat, boxStat, heapStat;
      tree.GetStatistics( out pointStat, out boxStat, out heapStat ); // to be sure
      sw.Start();

      // Query loop:
      for ( int i = 0; i++ < size; )
      {
        double x0 = (1.0 + (SIZE - 2.0) * rnd.NextDouble());
        double y0 = (1.0 + (SIZE - 2.0) * rnd.NextDouble());

        int n = tree.FindNearest( x0, y0, K, result );
        if ( n == 0 ) continue;

        // check (and draw) the intersections:
        double last = 0.0;  // distance of the last intersection

        for ( int j = 0; j < n; j++ )
        {
          double dist = (x0 - points[ result[ j ] ].X) * (x0 - points[ result[ j ] ].X) +
                        (y0 - points[ result[ j ] ].Y) * (y0 - points[ result[ j ] ].Y);

          if ( dist < last )
            throw new Exception( $"Error in result ordering: [{i},{j}]" );
          if ( dist == last )
            equals++;

          last = dist;

          // eventually draw the found point in nice color:
          if ( visual )
            Draw.Dot( output,
                      (int)points[ result[ j ] ].X, (int)points[ result[ j ] ].Y, PEN_SIZE, 
                      Draw.ColorRamp( 1.0 - colorMul * j ) );
        }

        resultSet.Clear();
        for ( int j = 0; j < n; j++ )
          resultSet.Add( result[ j ] );
        foreach ( int id in resultSet )
          hash = hash * 348937L + id * 8999L + 4831L;

#if LOG
        StringBuilder sb = new StringBuilder( $"Result[{i}]:" );
        for ( int j = 0; j < n; j++ )
          sb.Append( ' ' ).Append( result[ j ] );
        Util.Log( sb.ToString() );
#endif
      }

      sw.Stop();

      // Query-phase statistics:
      tree.GetStatistics( out pointStat, out boxStat, out heapStat );
      string stat1 = string.Format( CultureInfo.InvariantCulture, "Elapsed: {0:f} s [{1:D}-{2:D}-{3:D}-{4:D}]",
                                    1.0e-3 * sw.ElapsedMilliseconds,
                                    pointStat, boxStat, heapStat, equals );
      labelStat.Text = stat1;
      string stat2 = string.Format( "{0:X}", hash );
      labelHash.Text = stat2;
      Util.LogFormat( "Query[ {0}, {1}, {2}, {3} ]: hash: {4,16}, {5}",
                      (int)numericSize.Value, seed, size, K,
                      stat2, stat1 );

      if ( visual )
        pictureBox1.Invalidate();
      Cursor.Current = Cursors.Default;
    }

    private void buttonSave_Click ( object sender, EventArgs e )
    {
      if ( output == null )
        return;

      SaveFileDialog sfd = new SaveFileDialog();
      sfd.Title = "Save PNG file";
      sfd.Filter = "PNG Files|*.png";
      sfd.AddExtension = true;
      sfd.FileName = "";
      if ( sfd.ShowDialog() != DialogResult.OK )
        return;

      output.Save( sfd.FileName, System.Drawing.Imaging.ImageFormat.Png );
    }
  }
}
