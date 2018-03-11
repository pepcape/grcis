// Author: Josef Pelikan

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using MathSupport;
using Raster;
using Utilities;

namespace _068laser
{
  class Dither
  {
    /// <summary>
    /// Optional data initialization.
    /// </summary>
    /// <param name="param">Initial value for the 'Params' text-field.</param>
    /// <param name="tooltip">Tooltip for the text-field.</param>
    /// <param name="name">Your first-name and last-name.</param>
    public static void InitParams ( out string param, out string tooltip, out string name )
    {
      param   = "scale=2.0";
      tooltip = "scale, gamma, rnd, dot, sampling";
      name    = "pilot";
    }

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

    /// <summary>
    /// Draws a black (color=0) dot into 1bpp raster image.
    /// The image must be locked.
    /// </summary>
    /// <param name="x">X-coordinate of dot center.</param>
    /// <param name="y">Y-coordinate of dot center.</param>
    /// <param name="size">Dot size in pixels.</param>
    /// <param name="dataOut">Memory-mapped 1bpp output image.</param>
    protected unsafe static void Dot1bpp ( int x, int y, double size, BitmapData dataOut )
    {
      int wid = dataOut.Width;
      int hei = dataOut.Height;

      if ( x < 0 || x >= wid ||
           y < 0 || y >= hei ) return;

      for ( int pi = 0; pi < Draw.squares.Length && Draw.squares[ pi ] <= size; pi++ )
      {
        int pix = x + Draw.penPixels[ pi ].Item1;
        int piy = y + Draw.penPixels[ pi ].Item2;
        if ( pix < 0 || pix >= wid ||
             piy < 0 || piy >= hei )
          continue;

        byte* optr = (byte*)dataOut.Scan0 + piy * dataOut.Stride + (pix / 8);
        int mask = ~(1 << (7 - (pix % 8)));
        *optr = (byte)(*optr & mask);
      }
    }

    /// <summary>
    /// Converts the given image into B/W (1bpp) output suitable for high-resolution printer.
    /// </summary>
    /// <param name="input">Input image.</param>
    /// <param name="output">Output (1bpp) image.</param>
    /// <param name="oWidth">Default output image width in pixels.</param>
    /// <param name="oHeight">Default output image height in pixels.</param>
    /// <param name="param">Set of optional text parameters.</param>
    /// <returns>Number of dots printed.</returns>
    public static long TransformImage ( Bitmap input, out Bitmap output, int oWidth, int oHeight, string param )
    {
      // !!!{{ TODO: write your own image dithering code here

      int iWidth = input.Width;
      int iHeight = input.Height;
      long dots = 0L;

      // custom parameters from the text-field:
      double randomness = 0.0;
      double dot = 0.0;
      double gamma = 0.0;
      bool sampling = false;
      Dictionary<string, string> p = Util.ParseKeyValueList( param );
      if ( p.Count > 0 )
      {
        double scale = 0.0;

        // scale=<float-number>
        if ( Util.TryParse( p, "scale", ref scale ) &&
             scale > 0.01 )
        {
          oWidth = (int)(iWidth * scale);
          oHeight = (int)(iHeight * scale);
        }

        // rnd=<float-number>
        if ( Util.TryParse( p, "rnd", ref randomness ) )
          randomness = Arith.Clamp( randomness, 0.0, 1.0 );

        // dot=<float-number>
        if ( Util.TryParse( p, "dot", ref dot ) )
          dot = Math.Max( dot, 0.0 );

        // gamma=<float-number>
        if ( Util.TryParse( p, "gamma", ref gamma ) )
          gamma = Math.Max( gamma, 0.0 );

        // sampling=<bool>
        Util.TryParse( p, "sampling", ref sampling );
      }

      // create output 1bpp Bitmap
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
      RandomJames rnd = new RandomJames();

      // convert pixel data (fast memory-mapped code):
      PixelFormat iFormat = input.PixelFormat;
      if ( !PixelFormat.Format24bppRgb.Equals( iFormat ) &&
           !PixelFormat.Format32bppArgb.Equals( iFormat ) &&
           !PixelFormat.Format32bppPArgb.Equals( iFormat ) &&
           !PixelFormat.Format32bppRgb.Equals( iFormat ) )
        iFormat = PixelFormat.Format24bppRgb;

      BitmapData dataOut = output.LockBits( new Rectangle( 0, 0, oWidth, oHeight ), ImageLockMode.WriteOnly, output.PixelFormat );
      unsafe
      {
        byte* optr;

        // A. placing reasonable number of random dots on the paper
        if ( sampling )
        {
          dot = Math.Max( dot, 1.0 );

          // clear output image:
          optr = (byte*)dataOut.Scan0;
          for ( x = 0; x++ < oHeight * dataOut.Stride; )
            *optr++ = 255;

          // create grayscale image able to sample points from itself:
          FloatImage fi = new FloatImage( input );
          fi = fi.GrayImage( true, gamma );
          fi.PrepareCdf();

          // sample 'dots' random dots:
          dots = (long)(1.2 * oWidth * oHeight / (dot * dot));
          double xx, yy;
          for ( long i = 0; i++ < dots; )
          {
            fi.GetSample( out xx, out yy, rnd.UniformNumber(), rnd );
            xx = oWidth * (xx / iWidth);
            yy = oHeight * (yy / iHeight);
            Dot1bpp( (int)xx, (int)yy, dot, dataOut );
          }
        }
        else
        {
          BitmapData dataIn = input.LockBits( new Rectangle( 0, 0, iWidth, iHeight ), ImageLockMode.ReadOnly, iFormat );

          // B. random screen using dots bigger than 1px
          if ( dot > 0.0 )
          {
            // clear output image:
            optr = (byte*)dataOut.Scan0;
            for ( x = 0; x++ < oHeight * dataOut.Stride; )
              *optr++ = 255;

            int dI = Image.GetPixelFormatSize( iFormat ) / 8;

            for ( y = 0, fy = 0.0f; y < oHeight; y++, fy += dy )
            {
              if ( !Form1.cont ) break;

              for ( x = 0, fx = 0.0f; x < oWidth; x++, fx += dx )
              {
                float gray = GetGray( fx, fy, dataIn, dI );
                if ( gamma > 0.0 )
                  gray = (float)Math.Pow( gray, gamma );

                float threshold = (float)(0.5 - randomness * (rnd.UniformNumber() - 0.5));
                if ( gray < threshold )
                {
                  dots++;
                  Dot1bpp( x, y, dot, dataOut );
                }
              }
            }
          }
          else

          // C. random screen using individual pixels
          {
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
                if ( gamma > 0.0 )
                  gray = (float)Math.Pow( gray, gamma );

                float threshold = (float)(0.5 - randomness * (rnd.UniformNumber() - 0.5));
                buffer += buffer;
                if ( gray >= threshold )
                  buffer++;
                else
                  dots++;

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
          input.UnlockBits( dataIn );
        }

        output.UnlockBits( dataOut );
      }

      return dots;

      // !!!}}
    }
  }
}
