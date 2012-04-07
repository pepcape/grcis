using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Text;

namespace _005denoise
{
  class Denoise
  {
    public static void TransformImage ( Bitmap input, out Bitmap output, double param )
    {
      // !!!{{ TODO: write your own denoising code here

      int width = input.Width;
      int height = input.Height;
      output = new Bitmap( width, height, System.Drawing.Imaging.PixelFormat.Format24bppRgb );

      for ( int y = 0; y < height; y++ )
        for ( int x = 0; x < width; x++ )
        {
          int xm1 = x > 0 ? x - 1 : x + 1;
          int ym1 = y > 0 ? y - 1 : y + 1;
          float gray0 = input.GetPixel( x,   y   ).GetBrightness();
          float gray1 = input.GetPixel( xm1, y   ).GetBrightness();
          float gray2 = input.GetPixel( x,   ym1 ).GetBrightness();
          float gray3 = input.GetPixel( xm1, ym1 ).GetBrightness();
          int gray = (int)(63.75 * (gray0 + gray1 + gray2 + gray3));
          Color col = Color.FromArgb( gray, gray, gray );
          output.SetPixel( x, y, col );
        }

      // !!!}}
    }
  }
}
