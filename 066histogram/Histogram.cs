using System.Drawing;
using System.Windows.Forms;
using MathSupport;
using System;

namespace _066histogram
{
  public partial class Form1 : Form
  {
    /// <summary>
    /// Has the source image changed?
    /// </summary>
    public bool dirty = false;

    /// <summary>
    /// Cached histogram data.
    /// </summary>
    protected int[] histArray = null;

    /// <summary>
    /// Recomputes image histogram and draws the result in the given raster image.
    /// </summary>
    /// <param name="input">Input image.</param>
    /// <param name="graph">Result image (for the graph).</param>
    /// <param name="param">Textual parameter.</param>
    public void ComputeHistogram ( Bitmap input, Bitmap graph, string param )
    {
      // !!!{{ TODO: write your own histogram computation and drawing code here

      // Text parameters:
      param = param.ToLower().Trim();
      // Histogram mode (0 .. red, 1 .. green, 2 .. blue, 3 .. gray)
      int mode = 0;
      if ( param.IndexOf( "gray" ) >= 0 )
        mode = 3;

      // Graph appearance:
      bool alt = param.IndexOf( "alt" ) >= 0;

      int x, y;

      // 1. Histogram recomputation:
      if ( dirty )
      {
        dirty = false;

        // recompute histogram data:
        histArray = new int[ 256 ];

        int width = input.Width;
        int height = input.Height;
        for ( y = 0; y < height; y++ )
          for ( x = 0; x < width; x++ )
          {
            Color col = input.GetPixel( x, y );

            //double H, S, V;
            //Arith.ColorToHSV( col, out H, out S, out V );

            histArray[ col.R ]++;
          }
      }

      // 2. Graph drawing:
      float max = 0.0f;
      foreach ( var f in histArray )
        if ( f > max ) max = f;

      Graphics gfx = Graphics.FromImage( graph );
      gfx.Clear( Color.White );

      // Graph scaling:
      float x0 = graph.Width * 0.05f;
      float y0 = graph.Height * 0.95f;
      float kx = graph.Width * 0.9f / histArray.Length;
      float ky = -graph.Height * 0.9f / max;

      // Pens:
      Pen graphPen = new Pen( Color.Red );
      Brush graphBrush = new SolidBrush( Color.Red );
      Pen axisPen = new Pen( Color.Black );

      // Histogram:
      for ( x = 0; x < histArray.Length; x++ )
      {
        float yHeight = -histArray[ x ] * ky;
        if ( alt && yHeight > 3.0 ) yHeight = 3.0f;
        gfx.FillRectangle( graphBrush, x0 + x * kx, y0 + histArray[ x ] * ky, kx, yHeight );
      }

      // Axes:
      gfx.DrawLine( axisPen, x0, y0, x0 + histArray.Length * kx, y0 );
      gfx.DrawLine( axisPen, x0, y0, x0, y0 + max * ky );

      gfx.Dispose();

      // !!!}}
    }
  }
}
