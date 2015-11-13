// Author: Josef Pelikan

using System.Drawing;
using System.Drawing.Imaging;

namespace _007bluenoise
{
  class Dither
  {
    public static void TransformImage ( Bitmap input, out Bitmap output, double param )
    {
      // !!!{{ TODO: modify the Floyd-Steinberg dithering here

      int width = input.Width;
      int height = input.Height;
      output = new Bitmap( width, height, PixelFormat.Format24bppRgb );
      float[ , ] buf = new float[ 2, width + 2 ];
      int bi = 0;
      for ( int i = 0; i < 2; i++ )
        for ( int j = 0; j < width + 2; j++ )
          buf[ i, j ] = 0.0f;

      for ( int y = 0; y < height; y++, bi = 1 - bi )
        for ( int x = 0; x < width; x++ )
        {
          float gray = input.GetPixel( x, y ).GetBrightness() + buf[ bi, x + 1 ];
          int res;
          float err;
          if ( gray > 0.5f )
          {
            res = 255;
            err = gray - 1.0f;
          }
          else
          {
            res = 0;
            err = gray;
          }
          Color col = Color.FromArgb( res, res, res );
          output.SetPixel( x, y, col );

          // error distribution
          err /= 16.0f;
          buf[ bi, x + 1 ]      = 0.0f;
          buf[ bi, x + 2 ]     += err * 7.0f;
          buf[ 1 - bi, x ]     += err * 3.0f;
          buf[ 1 - bi, x + 1 ] += err * 5.0f;
          buf[ 1 - bi, x + 2 ]  = err;
        }

      // !!!}}
    }
  }
}
