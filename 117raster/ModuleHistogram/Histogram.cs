using System.Drawing;
using MathSupport;
using Raster;
using System;

namespace Utilities
{
  public class ImageHistogram
  {
    /// <summary>
    /// Cached histogram data.
    /// </summary>
    protected static int[] histArray = null;

    // Histogram mode (0 .. red, 1 .. green, 2 .. blue, 3 .. gray)
    protected static int mode = 3;

    // Graph appearance (just an example of second visualization option
    // read from param string).
    protected static bool alt = false;

    /// <summary>
    /// Draws the current histogram to the given raster image.
    /// </summary>
    /// <param name="graph">Result image (already scaled to the desired size).</param>
    public static void DrawHistogram (
      Bitmap graph)
    {
      if (histArray == null)
        return;

      float max = 0.0f;
      foreach (int f in histArray)
        if (f > max)
          max = f;

      using (Graphics gfx = Graphics.FromImage(graph))
      {
        gfx.Clear(Color.White);

        // Graph scaling:
        float x0 = graph.Width * 0.05f;
        float y0 = graph.Height * 0.95f;
        float kx = graph.Width * 0.9f / histArray.Length;
        float ky = -graph.Height * 0.9f / max;

        // Pens:
        Color c = mode == 0 ? Color.Red : (mode == 1 ? Color.DarkGreen : (mode == 2 ? Color.Blue : Color.Gray));
        Pen graphPen = new Pen(c);
        Brush graphBrush = new SolidBrush(c);
        Pen axisPen = new Pen(Color.Black);

        // Histogram:
        for (int x = 0; x < histArray.Length; x++)
        {
          float yHeight = -histArray[x] * ky;
          if (alt && yHeight > 3.0)
            yHeight = 3.0f;
          gfx.FillRectangle(graphBrush, x0 + x * kx, y0 + histArray[x] * ky, kx, yHeight);
        }

        // Axes:
        gfx.DrawLine(axisPen, x0, y0, x0 + histArray.Length * kx, y0);
        gfx.DrawLine(axisPen, x0, y0, x0, y0 + max * ky);
      }
    }

    /// <summary>
    /// Recomputes image histogram and draws the result in the given raster image.
    /// </summary>
    /// <param name="input">Input image.</param>
    /// <param name="param">Textual parameter.</param>
    public static void ComputeHistogram (
      Bitmap input,
      string param)
    {
      // Text parameters:
      param = param.ToLower().Trim();

      // Histogram mode (0 .. red, 1 .. green, 2 .. blue, 3 .. gray)
      mode = 3;
      if (param.IndexOf("red") >= 0)
        mode = 0;
      if (param.IndexOf("green") >= 0)
        mode = 1;
      if (param.IndexOf("blue") >= 0)
        mode = 2;

      // Sorted histogram:
      bool sort = param.IndexOf("sort") >= 0;

      // Graph appearance:
      alt = param.IndexOf("alt") >= 0;

      int x, y;

      // 1. Histogram recomputation.
      histArray = new int[256];

      int width = input.Width;
      int height = input.Height;
      for (y = 0; y < height; y++)
        for (x = 0; x < width; x++)
        {
          Color col = input.GetPixel( x, y );
          int Y = Draw.RgbToGray( col.R, col.G, col.B );

          //double H, S, V;
          //Arith.ColorToHSV( col, out H, out S, out V );

          histArray[mode == 0 ? col.R : (mode == 1 ? col.G : (mode == 2 ? col.B : Y))]++;
        }

      // 2. optional sorting.
      if (sort)
        Array.Sort(histArray, new ReverseComparer<int>().Compare);
    }
  }
}
