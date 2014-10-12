// Author: Josef Pelikan

using System.Drawing;

namespace _051colormap
{
  class Colormap
  {
    /// <summary>
    /// Generate a colormap based on input image.
    /// </summary>
    /// <param name="input">Input raster image.</param>
    /// <param name="numCol">Required colormap size (ignore it if you must)</param>
    /// <param name="colors">Output palette (array of colors).</param>
    public static void Generate ( Bitmap input, int numCol, out Color[] colors )
    {
      // !!!{{ TODO - generate custom palette based on the given image

      int width  = input.Width;
      int height = input.Height;

      colors = new Color[ numCol ];
      colors[ 0 ]          = input.GetPixel( 0, 0 );
      colors[ numCol - 1 ] = input.GetPixel( width/2, height/2 );

      float r = colors[ 0 ].R;
      float g = colors[ 0 ].G;
      float b = colors[ 0 ].B;
      float dr = (colors[ numCol - 1 ].R - r) / (numCol - 1.0f);
      float dg = (colors[ numCol - 1 ].G - g) / (numCol - 1.0f);
      float db = (colors[ numCol - 1 ].B - b) / (numCol - 1.0f);

      for ( int i = 1; i < numCol; i++ )
      {
        r += dr; g += dg; b += db;
        colors[ i ] = Color.FromArgb( (int)r, (int)g, (int)b );
      }

      // !!!}}
    }
  }
}
