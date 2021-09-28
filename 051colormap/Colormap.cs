using System.Drawing;
using MathSupport;

namespace _051colormap
{
  class Colormap
  {
    /// <summary>
    /// Form data initialization.
    /// </summary>
    public static void InitForm (out string author)
    {
      author = "Josef Pelikán";
    }

    /// <summary>
    /// Generate a colormap based on input image.
    /// </summary>
    /// <param name="input">Input raster image.</param>
    /// <param name="numCol">Required colormap size (ignore it if you must).</param>
    /// <param name="colors">Output palette (array of colors).</param>
    public static void Generate (Bitmap input, int numCol, out Color[] colors)
    {
      // !!!{{ TODO - generate custom palette based on the given image

      int width  = input.Width;
      int height = input.Height;

      colors = new Color[numCol];            // accepting the required palette size..

      colors[0] = input.GetPixel(0, 0);      // upper left image corner

      double H, S, V;
      Color center = input.GetPixel(width / 2, height / 2);   // image center
      Arith.ColorToHSV(center, out H, out S, out V);
      if (S > 1.0e-3)
        colors[numCol - 1] = Arith.HSVToColor(H, 1.0, 1.0);   // non-monochromatic color => using Hue only
      else
        colors[numCol - 1] = center;                          // monochromatic color => using it directly

      // color-ramp linear interpolation:
      float r = colors[0].R;
      float g = colors[0].G;
      float b = colors[0].B;
      float dr = (colors[numCol - 1].R - r) / (numCol - 1.0f);
      float dg = (colors[numCol - 1].G - g) / (numCol - 1.0f);
      float db = (colors[numCol - 1].B - b) / (numCol - 1.0f);

      for (int i = 1; i < numCol; i++)
      {
        r += dr;
        g += dg;
        b += db;
        colors[i] = Color.FromArgb((int)r, (int)g, (int)b);
      }

      // !!!}}
    }
  }
}
