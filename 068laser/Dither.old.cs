// Author: Josef Pelikan

using System.Drawing;
using MathSupport;
using System.Drawing.Imaging;
using System;
using Raster;
using System.Globalization;

namespace _068laser
{
  class Dither
  {
    /// <summary>
    /// Separator for string parameter.
    /// </summary>
    static readonly char COMMA = ',';

    /// <summary>
    /// Retrieves a gray value from the memory-mapped image. Uses bilinear interpolation.
    /// </summary>
    /// <param name="x">X-coordinate (fractions of a pixel can be used).</param>
    /// <param name="y">X-coordinate (fractions of a pixel can be used).</param>
    /// <param name="dataIn">Memory-mapped input image.</param>
    /// <param name="bpp">Bytes per pixel of the input image.</param>
    /// <returns>Gray value ranging from 0.0 to 1.0.</returns>
    protected unsafe static float GetGray ( float x, float y, BitmapData dataIn, int bpp )
    {
      x = Arith.Clamp( x, 0.0f, dataIn.Width - 1.0f );
      y = Arith.Clamp( y, 0.0f, dataIn.Height - 1.0f );
      int x0 = (int)Math.Floor( x );
      int y0 = (int)Math.Floor( y );
      byte *iptr00 = (byte*)dataIn.Scan0 + y0 * dataIn.Stride + x0 * bpp;
      byte* iptr01 = iptr00;
      if ( y0 < dataIn.Height - 1 )
        iptr01 += dataIn.Stride;
      byte* iptr10 = iptr00;
      byte* iptr11 = iptr01;
      if ( x0 < dataIn.Width - 1 )
      {
        iptr10 += bpp;
        iptr11 += bpp;
      }

      float g00 = Draw.RgbToGray( iptr00[ 2 ], iptr00[ 1 ], iptr00[ 0 ] ) / 255.0f;
      float g01 = Draw.RgbToGray( iptr01[ 2 ], iptr01[ 1 ], iptr01[ 0 ] ) / 255.0f;
      float g10 = Draw.RgbToGray( iptr10[ 2 ], iptr10[ 1 ], iptr10[ 0 ] ) / 255.0f;
      float g11 = Draw.RgbToGray( iptr11[ 2 ], iptr11[ 1 ], iptr11[ 0 ] ) / 255.0f;
      x -= x0;
      y -= y0;
      g00 = (1.0f - x) * g00 + x * g10;
      g01 = (1.0f - x) * g01 + x * g11;
      return( (1.0f - y) * g00 + y * g01 );
    }

    public static void TransformImage ( Bitmap input, out Bitmap output, int oWidth, int oHeight, string param )
    {
      // !!!{{ TODO: write your own image dithering code here

      int iWidth  = input.Width;
      int iHeight = input.Height;

      // Text parameter = scale[,randomness]
      float scale = 1.0f;
      float randomness = 0.1f;
      if ( param.Length > 0 )
      {
        string[] par = param.Split( COMMA );
        if ( par.Length > 0 )
        {
          if ( float.TryParse( par[ 0 ], NumberStyles.Float, CultureInfo.InvariantCulture, out scale ) )
          {
            oWidth  = (int)(iWidth * scale);
            oHeight = (int)(iHeight * scale);
          }
          if ( par.Length > 1 )
          {
            float.TryParse( par[ 1 ], NumberStyles.Float, CultureInfo.InvariantCulture, out randomness );
            randomness = Arith.Clamp( randomness, 0.0f, 1.0f );
          }
        }
      }

      output = new Bitmap( oWidth, oHeight, PixelFormat.Format1bppIndexed );
      float dx = (iWidth - 1.0f) / (oWidth - 1.0f);
      float dy = (iHeight - 1.0f) / (oHeight - 1.0f);

      // set the B/W palette (0 .. black, 1 .. white):
      ColorPalette pal = output.Palette;
      pal.Entries[ 0 ] = Color.Black;
      pal.Entries[ 1 ] = Color.White;
      output.Palette = pal;

      int x, y;
      float fx, fy;
      Random rnd = new Random();

      // convert pixel data (fast memory-mapped code):
      PixelFormat iFormat = input.PixelFormat;
      if ( !PixelFormat.Format24bppRgb.Equals( iFormat ) &&
           !PixelFormat.Format32bppArgb.Equals( iFormat ) &&
           !PixelFormat.Format32bppPArgb.Equals( iFormat ) &&
           !PixelFormat.Format32bppRgb.Equals( iFormat ) )
        iFormat = PixelFormat.Format24bppRgb;

      BitmapData dataIn = input.LockBits( new Rectangle( 0, 0, iWidth, iHeight ), ImageLockMode.ReadOnly, iFormat );
      BitmapData dataOut = output.LockBits( new Rectangle( 0, 0, oWidth, oHeight ), ImageLockMode.WriteOnly, output.PixelFormat );
      unsafe
      {
        byte* optr;
        int buffer;
        int dI = Image.GetPixelFormatSize( iFormat ) / 8;

        for ( y = 0, fy = 0.0f; y < oHeight; y++, fy += dy )
        {
          if ( !Form1.cont ) break;

          optr = (byte*)dataOut.Scan0 + y * dataOut.Stride;
          buffer = 0;

          for ( x = 0, fx = 0.0f; x < oWidth; fx += dx )
          {
            float gray = GetGray( fx, fy, dataIn, dI );
            float threshold = (float)(0.5 - randomness * (rnd.NextDouble() - 0.5));
            buffer += buffer;
            if ( gray > threshold ) buffer++;

            if ( (++x & 7) == 0 )
            {
              *optr++ = (byte)buffer;
              buffer = 0;
            }
          }

          // finish the last byte of the scanline:
          if ( (x & 7) != 0 )
          {
            while ( (x++ & 7) != 0 )
              buffer += buffer;
            *optr = (byte)buffer;
          }
        }
      }
      output.UnlockBits( dataOut );
      input.UnlockBits( dataIn );

      // !!!}}
    }
  }
}
