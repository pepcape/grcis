using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Text;

namespace _003colortransform
{
  class Transform
  {
    public static void TransformImage ( Bitmap input, out Bitmap output, double param )
    {
      // !!!{{ TODO: write your own color transformation code here

      int width = input.Width;
      int height = input.Height;
      output = new Bitmap( width, height, System.Drawing.Imaging.PixelFormat.Format24bppRgb );
      for ( int y = 0; y < height; y++ )
        for ( int x = 0; x < width; x++ )
        {
          Color col = input.GetPixel( x, y );
          col = Color.FromArgb( (byte)((255 - col.R) * param + col.R * (1.0 - param)), col.G, col.B );
          output.SetPixel( x, y, col );
        }

      // !!!}}
    }
  }
}
