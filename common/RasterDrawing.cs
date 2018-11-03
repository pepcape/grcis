using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using MathSupport;

namespace Raster
{
  public partial class Draw
  {
    /// <summary>
    /// Red component importance weight.
    /// </summary>
    public const int RED_WEIGHT   = 4897;

    /// <summary>
    /// Green component importance weight.
    /// </summary>
    public const int GREEN_WEIGHT = 9611;

    /// <summary>
    /// Blue component importance weight.
    /// </summary>
    public const int BLUE_WEIGHT  = 1876;

    /// <summary>
    /// Color importance shift.
    /// </summary>
    public const int WEIGHT_SHIFT = 14;

    /// <summary>
    /// Color importance sum.
    /// </summary>
    public const int WEIGHT_TOTAL = 16384;

    /// <summary>
    /// Inverted value for converting 8-bit RGB images into gray-scale [0.0,1.0]
    /// </summary>
    public const float WEIGHT_INV = 1.0f / (WEIGHT_TOTAL * 255.0f);

    /// <summary>
    /// RGB -> gray value convertor. Keeps original data amplitude..
    /// </summary>
    /// <param name="r">Red component</param>
    /// <param name="g">Green component</param>
    /// <param name="b">Blue component</param>
    /// <returns>Gray (monochromatic) value</returns>
    public static int RgbToGray ( int r, int g, int b )
    {
      return( ( r * RED_WEIGHT +
                g * GREEN_WEIGHT +
                b * BLUE_WEIGHT ) >> WEIGHT_SHIFT );
    }

    /// <summary>
    /// Converts a Color to float gray value from [0.0f, 1.0f]
    /// </summary>
    public static float ColorToGray ( Color c )
    {
      return( (c.R * RED_WEIGHT +
               c.G * GREEN_WEIGHT +
               c.B * BLUE_WEIGHT) * WEIGHT_INV );
    }

    /// <summary>
    /// Draws line into the Bitmap. integer coordinates, no anti-aliasing.
    /// </summary>
    /// <param name="img"></param>
    /// <param name="x1"></param>
    /// <param name="y1"></param>
    /// <param name="x2"></param>
    /// <param name="y2"></param>
    /// <param name="color"></param>
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

    public static void Dot ( Bitmap img, int x, int y, double penSize, Color color )
    {
      int wid = img.Width;
      int hei = img.Height;
      if ( x < 0 || x >= wid ||
           y < 0 || y >= hei ) return;

      img.SetPixel( x, y, color );
      if ( penSize > 1.0 )
        for ( int pi = 1; pi < squares.Length && squares[ pi ] <= penSize; pi++ )
        {
          int pix = x + penPixels[ pi ].Item1;
          int piy = y + penPixels[ pi ].Item2;
          if ( pix < 0 || pix >= wid ||
               piy < 0 || piy >= hei )
            continue;

          img.SetPixel( pix, piy, color );
        }
    }

    /// <summary>
    /// Random colormap for debugging purposes.
    /// </summary>
    public static List<Color> debugColors = null;

    /// <summary>
    /// Allocate large enough colormap..
    /// </summary>
    public static void AssertColors ( int num )
    {
      int i;
      if ( debugColors == null ||
           debugColors.Count < num )
      {
        debugColors = new List<Color>( num );
        RandomJames rnd = new RandomJames();
        for ( i = 0; i < num; i++ )
        {
          int R = rnd.RandomInteger( 0, 255 );
          int G = rnd.RandomInteger( 0, 255 );
          int B = rnd.RandomInteger( 0, 255 );
          if ( R >= 128 && G >= 128 && B >= 128 )
            if ( rnd.UniformNumber() > 0.5 )
              R -= 128;
            else
              G -= 128;
          debugColors.Add( Color.FromArgb( R, G, B ) );
        }
      }
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

    /// <summary>
    /// Creates a static 3-3-2 colormap.
    /// </summary>
    /// <param name="pal">Instance of palette to modify.</param>
    /// <param name="errDistr">Will be the palette used for error-distribution?</param>
    public static void Palette332 ( ColorPalette pal, bool errDistr =false )
    {
      // Modify the palette.
      double R, G, B;
      double R0, G0, B0;
      double dRG, dB;
      int r, g, b, i;

      if ( errDistr )                     // convex closure of the colormap should be equal
      {                                   // to the whole RGB space
        R0  =
        G0  =
        B0  = 0.0;
        dRG = 255.0 /  7;
        dB  = 255.0 /  3;
      }
      else
      {                                   // the colormap will be optimal for rounding
        R0  =
        G0  = 255.0 / 16;
        B0  = 255.0 /  8;
        dRG = 255.0 /  8;
        dB  = 255.0 /  4;
      }

      for ( r = i = 0, R = R0; r < 8; r++, R += dRG )
        for ( g = 0, G = G0; g < 8; g++, G += dRG )
          for ( b = 0, B = B0; b < 4; b++, B += dB )
            pal.Entries[ i++ ] = Color.FromArgb( (int)( R + 0.5 ), (int)( G + 0.5 ), (int)( B + 0.5 ) );
    }

    /// <summary>
    /// Creates a static gray colormap.
    /// </summary>
    /// <param name="pal">Instance of palette to modify.</param>
    /// <param name="num">Size of grayscale ramp</param>
    public static void PaletteGray ( ColorPalette pal, int num =256 )
    {
      // Modify the palette.
      if ( num > 256 )
        num = 256;
      double RGB = 0.0;
      double dRGB = 255.0 / (num - 1.0);

      for ( int i = 0; i < num; RGB += dRGB )
        pal.Entries[ i++ ] = Color.FromArgb( (int)Math.Round(RGB), (int)Math.Round( RGB ), (int)Math.Round( RGB ) );
    }

    public static long Hash ( Bitmap img )
    {
      int width  = img.Width;
      int height = img.Height;

      PixelFormat fmt = img.PixelFormat;

      long result = 0L;
      BitmapData data = img.LockBits( new Rectangle( 0, 0, width, height ), ImageLockMode.ReadOnly, fmt );
      unsafe
      {
        byte* ptr;

        for ( int y = 0; y < height; y++ )
        {
          ptr = (byte*)data.Scan0 + y * data.Stride;

          for ( int x = data.Stride; x-- > 0 ; ptr++ )
            result = result * 101L + 147L * ptr[ 0 ];
        }
      }

      img.UnlockBits( data );
      return result;
    }

    /// <summary>
    /// Compares two color images for the merest difference
    /// </summary>
    /// <param name="img1">1st image to compare</param>
    /// <param name="img2">2nd image to compare</param>
    /// <param name="xor">Optional output image (XOR)</param>
    /// <returns>Number of byte-differences</returns>
    public static long ImageCompare ( Bitmap img1, Bitmap img2, Bitmap xor )
    {
      if ( img1 == null || img2 == null )
        return 7L;

      int width  = img1.Width;
      int height = img1.Height;

      if ( width  != img2.Width ||
           height != img2.Height ) return 11L;

      long result = 0L;
      PixelFormat fmt = img1.PixelFormat;
      if ( !fmt.Equals( img2.PixelFormat ) )
      {
        // slow version:
        if ( (xor != null) &&
             (xor.Width < width || xor.Height < height ) )
          xor = null;

        for ( int y = 0; y < height; y++ )
          for ( int x = 0; x < width; x++ )
          {
            Color c1 = img1.GetPixel( x, y );
            Color c2 = img2.GetPixel( x, y );
            byte xorR = (byte)(c1.R ^ c2.R);
            if ( xorR != 0 ) result++;
            byte xorG = (byte)(c1.G ^ c2.G);
            if ( xorG != 0 ) result++;
            byte xorB = (byte)(c1.B ^ c2.B);
            if ( xorB != 0 ) result++;
            if ( xor != null )
              xor.SetPixel( x, y, Color.FromArgb( xorR, xorG, xorB ) );
          }

        return result;
      }

      if ( (xor != null) &&
           (xor.Width < width || xor.Height < height || !fmt.Equals( xor.PixelFormat )) )
        xor = null;

      int pixelSize = Image.GetPixelFormatSize( fmt ) / 8;

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

    public static float ImageRMSE ( Bitmap img1, Bitmap img2, Bitmap xor )
    {
      if ( img1 == null || img2 == null )
        return 1000.0f;

      int width  = img1.Width;
      int height = img1.Height;

      if ( width != img2.Width ||
           height != img2.Height ) return 2000.0f;

      if ( xor != null )
      {
        if ( width != xor.Width ||
             height != xor.Height )
          xor = null;
      }

      double sum = 0.0;
      for ( int y = 0; y < height; y++ )
        for ( int x = 0; x < width; x++ )
        {
          int gr1 = (int)(img1.GetPixel( x, y ).GetBrightness() * 255.0f);
          int gr2 = (int)(img2.GetPixel( x, y ).GetBrightness() * 255.0f);
          sum += (gr1 - gr2) * (gr1 - gr2);
          if ( xor != null )
          {
            gr1 ^= gr2;
            xor.SetPixel( x, y, Color.FromArgb( gr1, gr1, gr1 ) );
          }
        }

      return (float)Math.Sqrt( sum / (width * height) );
    }

    public static long ColorNumber ( Bitmap input )
    {
      int width  = input.Width;
      int height = input.Height;
      int xi, yi;
      BitArray usedColors = new BitArray( 1 << 24 );

      BitmapData dataIn = input.LockBits( new Rectangle( 0, 0, width, height ), ImageLockMode.ReadOnly, input.PixelFormat );
      unsafe
      {
        byte* iptr;
        int ri, gi, bi, c;
        int dI = Image.GetPixelFormatSize( input.PixelFormat ) / 8;   // pixel size in bytes

        for ( yi = 0; yi < height; yi++ )
        {
          iptr = (byte*)dataIn.Scan0 + yi * dataIn.Stride;

          for ( xi = 0; xi < width; xi++ )
          {
            // read input colors
            bi = iptr[ 0 ];
            gi = iptr[ 1 ];
            ri = iptr[ 2 ];
            c = (((ri << 8) + gi) << 8) + bi;
            usedColors[ c ] = true;

            iptr += dI;
          }
        }
      }
      input.UnlockBits( dataIn );

      long count = 0L;
      foreach ( bool bit in usedColors )
        if ( bit ) count++;

      return count;
    }

    /// <summary>
    /// Generates a test image in grayscale which can be used for print tests.
    /// </summary>
    public static Bitmap TestImageGray ( int width, int height, int seed )
    {
      Random rnd = new Random( seed );
      Bitmap bmp = new Bitmap( width, height, PixelFormat.Format24bppRgb );
      Graphics gr = Graphics.FromImage( bmp );

      // pens:
      Pen[] pens = new Pen[ 256 ];
      int i;
      for ( i = 0; i < 256; i++ )
        pens[ i ] = new Pen( Color.FromArgb( i, i, i ) );

      // brushes:
      Brush[] brushes = new Brush[ 256 ];
      for ( i = 0; i < 256; i++ )
        brushes[ i ] = new SolidBrush( Color.FromArgb( i, i, i ) );

      gr.Clear( Color.White );

      int fifth = (2 * height) / 9;
      int square = 16;
      int cx = width / 2;
      int cy = height / 2;
      int lines = (width * height) / 800;
      int color, x, y;

      // random lines:
      for ( i = 0; i < lines; i++ )
      {
        color = rnd.Next( 255 );
        x = rnd.Next( width );
        y = rnd.Next( height );
        gr.DrawLine( pens[ color ], x, y, cx, cy );
      }

      // vignettes:
      for ( x = 0; x < width; x++ )
      {
        gr.DrawLine( pens[ 255 - (x * 256) / width ], x, 0, x, fifth );
        gr.DrawLine( pens[ (x * 256) / width ], x, height - fifth, x, height - 1 );
      }

      // squares:
      for ( y = cy; y + square <= cy + fifth; y += square )
        for ( x = 0; x + square <= width; x += square )
        {
          color = rnd.Next( 256 );
          gr.FillRectangle( brushes[ color ], x, y, square, square );
        }

      gr.Dispose();
      return bmp;
    }

    /// <summary>
    /// Support for fractional pen sizes.
    /// </summary>
    public static double[] squares;

    /// <summary>
    /// Support for fractional pen sizes.
    /// </summary>
    public static List<Tuple<int, int>> penPixels;

    public static void SetPens ( int n )
    {
      if ( (n |= 1) < 3 )
        n = 3;

      if ( squares != null &&
           squares.Length >= n * n )
        return;

      penPixels = new List<Tuple<int, int>>( n * n );
      squares = new double[ n * n ];

      for ( int i = 0; i < n * n; i++ )
        squares[ i ] = Math.Sqrt( i + 1.0 );

      int c = n / 2;
      for ( int y = 0; y < n; y++ )
        for ( int x = 0; x < n; x++ )
          penPixels.Add( new Tuple<int, int>( x - c, y - c ) );

      const float CX = 0.05f;
      const float CY = 0.91f;
      penPixels.Sort( ( a, b ) =>
      {
        float x = a.Item1 - CX;
        float y = a.Item2 - CY;
        float da = x * x + y * y;
        x = b.Item1 - CX;
        y = b.Item2 - CY;
        float db = x * x + y * y;
        if ( da == db )
          return a.Item1.CompareTo( b.Item1 );
        return da.CompareTo( db );
      } );
    }

    static Draw ()
    {
      SetPens( 7 );
    }
  }
}
