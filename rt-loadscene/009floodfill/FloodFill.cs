// Author: Josef Pelikan

using System.Drawing;

namespace Raster
{
  public partial class Draw
  {
    public static void FloodFill4 ( Bitmap img, int x, int y, Color color )
    {
      // !!!{{ TODO: put your 4-continuous flood fill algorithm here

      int width  = img.Width;
      int height = img.Height;
      if ( x < 0 ) x = 0;
      else
      if ( x >= width ) x = width - 1;
      if ( y < 0 ) y = 0;
      else
      if ( y >= height ) y = height - 1;

      Color orig = img.GetPixel( x, y );
      if ( orig ==color ) return;

      do
      {
        img.SetPixel( x, y, color );
        if ( ++x >= width ) break;
      }
      while ( orig == img.GetPixel( x, y ) );

      // !!!}}
    }

    public static void FloodFill8 ( Bitmap img, int x, int y, Color color )
    {
      // !!!{{ TODO: put your 8-continuous flood fill algorithm here

      FloodFill4( img, x, y, color );

      // !!!}}
    }

  }
}
