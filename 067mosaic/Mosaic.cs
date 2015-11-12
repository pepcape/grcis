using System.Drawing;
using MathSupport;
using System.Drawing.Imaging;
using System;

namespace _067mosaic
{
  class Mosaic
  {
    /// <summary>
    /// Separator for string parameter.
    /// </summary>
    static readonly char COMMA = ',';

    public static void TransformImage ( Bitmap input, out Bitmap output, string param )
    {
      // !!!{{ TODO: write your own image transformation code here

      // Text parameter = block size
      int cellW = 12;
      int cellH = 12;
      if ( param.Length > 0 )
      {
        string[] size = param.Split( COMMA );
        if ( size.Length >= 2 )
        {
          int.TryParse( size[ 0 ], out cellW );
          int.TryParse( size[ 1 ], out cellH );
        }
      }

      int width  = input.Width;
      int height = input.Height;
      output = new Bitmap( width, height, PixelFormat.Format24bppRgb );
      int x0, y0;
      int x, y;
      int x1, y1;

      // convert pixel data (fast memory-mapped code):
      PixelFormat iFormat = input.PixelFormat;
      if ( !PixelFormat.Format24bppRgb.Equals( iFormat ) &&
           !PixelFormat.Format32bppArgb.Equals( iFormat ) &&
           !PixelFormat.Format32bppPArgb.Equals( iFormat ) &&
           !PixelFormat.Format32bppRgb.Equals( iFormat ) )
        iFormat = PixelFormat.Format24bppRgb;

      BitmapData dataIn  = input.LockBits( new Rectangle( 0, 0, width, height ), ImageLockMode.ReadOnly, iFormat );
      BitmapData dataOut = output.LockBits( new Rectangle( 0, 0, width, height ), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb );
      unsafe
      {
        byte* iptr, optr;
        byte r, g, b;
        int sR, sG, sB;
        int pixels;
        int dI = Image.GetPixelFormatSize( iFormat ) / 8;
        int dO = Image.GetPixelFormatSize( PixelFormat.Format24bppRgb ) / 8;

        for ( y0 = 0; y0 < height; y0 += cellH )
        {
          if ( !Form1.cont ) break;

          for ( x0 = 0; x0 < width; x0 += cellW )     // one output cell
          {
            sR = sG = sB = 0;
            x1 = Math.Min( x0 + cellW, width );
            y1 = Math.Min( y0 + cellH, height );

            for ( y = y0; y < y1; y++ )
            {
              iptr = (byte*)dataIn.Scan0 + y * dataIn.Stride + x0 * dI;
              for ( x = x0; x < x1; x++, iptr += dI )
              {
                sB += (int)iptr[ 0 ];
                sG += (int)iptr[ 1 ];
                sR += (int)iptr[ 2 ];
              }
            }

            pixels = (x1 - x0) * (y1 - y0);
            r = (byte)( (sR + pixels / 2) / pixels );
            g = (byte)( (sG + pixels / 2) / pixels );
            b = (byte)( (sB + pixels / 2) / pixels );

            for ( y = y0; y < y1; y++ )
            {
              optr = (byte*)dataOut.Scan0 + y * dataOut.Stride + x0 * dO;
              for ( x = x0; x < x1; x++, optr += dO )
              {
                optr[ 0 ] = b;
                optr[ 1 ] = g;
                optr[ 2 ] = r;
              }
            }
          }
        }
      }
      output.UnlockBits( dataOut );
      input.UnlockBits( dataIn );

      // !!!}}
    }
  }
}
