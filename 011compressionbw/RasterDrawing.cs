using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;

namespace Raster
{
  public partial class Draw
  {
    public static void Line ( Bitmap img, int x1, int y1, int x2, int y2, Color color )
    {
      int width  = img.Width;
      int height = img.Height;
      if ( x1 < 0 ) x1 = 0;
      else
      if ( x1 >= width ) x1 = width-1;
      if ( y1 < 0 ) y1 = 0;
      else
      if ( y1 >= height ) y1 = height-1;
      if ( x2 < 0 ) x2 = 0;
      else
      if ( x2 >= width ) x2 = width-1;
      if ( y2 < 0 ) y2 = 0;
      else
      if ( y2 >= height ) y2 = height-1;

      int D, ax, ay, sx, sy;

      sx = x2 - x1;
      ax = Math.Abs( sx ) << 1;
      if ( sx < 0 ) sx = -1;
      else
        if ( sx > 0 ) sx = 1;

      sy = y2 - y1;
      ay = Math.Abs( sy ) << 1;
      if ( sy < 0 ) sy = -1;
      else
        if ( sy > 0 ) sy = 1;

      if ( ax > ay )                          // x coordinate is dominant
      {
        D = ay - (ax >> 1);                   // initial D
        ax = ay - ax;                         // ay = increment0; ax = increment1
        while ( x1 != x2 )
        {
          img.SetPixel( x1, y1, color );
          if ( D >= 0 )                       // lift up the Y coordinate
          {
            y1 += sy;
            D += ax;
          }
          else
            D += ay;
          x1 += sx;
        }
      }

      else                                    // y coordinate is dominant
      {
        D = ax - (ay >> 1);                   // initial D
        ay = ax - ay;                         // ax = increment0; ay = increment1
        while ( y1 != y2 )
        {
          img.SetPixel( x1, y1, color );
          if ( D >= 0 )                       // lift up the X coordinate
          {
            x1 += sx;
            D += ay;
          }
          else
            D += ax;
          y1 += sy;
        }
      }
                                              // the very last pixel
      img.SetPixel( x1, y1, color );
    }

    public static Color ColorRamp ( double x )
    {
      if ( x < 0.0 ) x = 0.0;
      else
      if ( x > 1.0 ) x = 1.0;

      int R, G, B;

        // I:   [  0,  0,  0] to [  0,  0,255]
      if ( x <= 0.2 )
      {
        R =
        G = 0;
        B = (int)( x * 1275.0 + 0.5 );
      }
      else
        // II:  [  0,  0,255] to [  0,255,128]
      if ( (x -= 0.2) <= 0.2 )
      {
        R = 0;
        G = (int)( x * 1275.0 + 0.5 );
        B = (int)( 255.5 - x * 635.0 );
      }
      else
        // III: [  0,255,128] to [128,255,  0]
      if ( (x -= 0.2) <= 0.2 )
      {
        R = (int)( x * 640.0 + 0.5 );
        G = 255;
        B = (int)( 128.5 - x * 640.0 );
      }
      else
        // IV:  [128,255,  0] to [255,255,  0]
      if ( (x -= 0.2) <= 0.2 )
      {
        R = (int)( 128.5 + x * 635.0 );
        G = 255;
        B = 0;
      }
      else
        // V:   [255,255,  0] to [255,  0,  0]
      {
        R = 255;
        G = (int)( 255.5 - (x - 0.2) * 1275.0 );
        B = 0;
      }

      return Color.FromArgb( R, G, B );
    }

    public static long Hash ( Bitmap img )
    {
      int width = img.Width;
      int height = img.Height;

      PixelFormat fmt = img.PixelFormat;
      int pixelSize = 1;
      if ( fmt.Equals( PixelFormat.Format24bppRgb ) )
        pixelSize = 3;
      if ( fmt.Equals( PixelFormat.Format32bppArgb ) ||
           fmt.Equals( PixelFormat.Format32bppPArgb ) )
        pixelSize = 4;

      long result = 0L;
      BitmapData data = img.LockBits( new Rectangle( 0, 0, width, height ), ImageLockMode.ReadOnly, fmt );
      unsafe
      {
        byte* ptr;

        for ( int y = 0; y < height; y++ )
        {
          ptr = (byte*)data.Scan0 + y * data.Stride;

          for ( int x = width * pixelSize; x-- > 0; ptr++ )
            result = result * 101L + 147L * ptr[ 0 ];
        }
      }

      img.UnlockBits( data );
      return result;
    }

    public static long ImageCompare ( Bitmap img1, Bitmap img2, Bitmap xor )
    {
      if ( img1 == null || img2 == null )
        return 7L;

      int width  = img1.Width;
      int height = img1.Height;

      if ( width  != img2.Width ||
           height != img2.Height ) return 11L;

      PixelFormat fmt = img1.PixelFormat;
      if ( !fmt.Equals( img2.PixelFormat ) )
        return 11L;

      int pixelSize = 1;
      if ( fmt.Equals( PixelFormat.Format24bppRgb ) )
        pixelSize = 3;
      if ( fmt.Equals( PixelFormat.Format32bppArgb ) ||
           fmt.Equals( PixelFormat.Format32bppPArgb ) )
        pixelSize = 4;

      if ( (xor != null) &&
           (xor.Width < width || xor.Height < height || !fmt.Equals( xor.PixelFormat )) )
        xor = null;

      long result = 0L;
      BitmapData data1 = img1.LockBits( new Rectangle( 0, 0, width, height ), ImageLockMode.ReadOnly, fmt );
      BitmapData data2 = img2.LockBits( new Rectangle( 0, 0, width, height ), ImageLockMode.ReadOnly, fmt );
      BitmapData outp  = (xor == null) ? null : xor.LockBits( new Rectangle( 0, 0, width, height ), ImageLockMode.WriteOnly, fmt );
      unsafe
      {
        byte* ptr1;
        byte* ptr2;
        byte* ptro;

        for ( int y = 0; y < height; y++ )
        {
          ptr1 = (byte*)data1.Scan0 + y * data1.Stride;
          ptr2 = (byte*)data2.Scan0 + y * data2.Stride;
          ptro = (outp == null) ? null : (byte*)outp.Scan0 + y * outp.Stride;

          for ( int x = width * pixelSize; x-- > 0; ptr1++, ptr2++ )
          {
            byte xorbyte = (byte)(ptr1[ 0 ] ^ ptr2[ 0 ]);
            if ( xorbyte != 0 ) result++;
            if ( outp != null )
            {
              ptro[ 0 ] = xorbyte;
              ptro++;
            }
          }
        }
      }

      img1.UnlockBits( data1 );
      img2.UnlockBits( data2 );
      if ( outp != null ) xor.UnlockBits( outp );

      return result;
    }

    public static long ImageCompareBW ( Bitmap img1, Bitmap img2, Bitmap xor )
    {
      if ( img1 == null || img2 == null )
        return 7L;

      int width = img1.Width;
      int height = img1.Height;

      if ( width != img2.Width ||
           height != img2.Height ) return 11L;

      if ( xor != null )
      {
        if ( width != xor.Width ||
             height != xor.Height )
          xor = null;
      }

      long result = 0L;
      for ( int y = 0; y < height; y++ )
        for ( int x = 0; x < width; x++ )
        {
          int gr1 = (img1.GetPixel( x, y ).GetBrightness() < 0.5f) ? 0 : 255;
          int gr2 = (img2.GetPixel( x, y ).GetBrightness() < 0.5f) ? 0 : 255;
          if ( gr1 != gr2 ) result++;
          if ( xor != null )
          {
            gr1 ^= gr2;
            xor.SetPixel( x, y, Color.FromArgb( gr1, gr1, gr1 ) );
          }
        }

      return result;
    }
  }
}
