// Author: Josef Pelikan

using System.Drawing;
using System.Drawing.Imaging;
using MathSupport;
using Raster;

namespace _054colorreduction
{
  class ColorReduction
  {
    /// <summary>
    /// Separator for string parameter.
    /// </summary>
    static readonly char[] COMMA = new char[] { ',' };

    public static void Reduce ( Bitmap input, out Bitmap output, string param )
    {
      // !!!{{ TODO: write your own color reduction code here

      // text parameter = use dithering palette?
      int dit = 0;
      if ( param.Length > 0 )
        int.TryParse( param, out dit );

      int width  = input.Width;
      int height = input.Height;
      output = new Bitmap( width, height, PixelFormat.Format8bppIndexed );

      // set the palette (3-3-2 by default):
      ColorPalette pal = output.Palette;
      Draw.Palette332( pal, dit > 0 );
      output.Palette = pal;

      // convert pixel data (fast memory-mapped code):
      PixelFormat iFormat = input.PixelFormat;
      if ( !PixelFormat.Format24bppRgb.Equals( iFormat ) &&
           !PixelFormat.Format32bppArgb.Equals( iFormat ) &&
           !PixelFormat.Format32bppPArgb.Equals( iFormat ) &&
           !PixelFormat.Format32bppRgb.Equals( iFormat ) )
        iFormat = PixelFormat.Format24bppRgb;

      BitmapData dataIn = input.LockBits( new Rectangle( 0, 0, width, height ), ImageLockMode.ReadOnly, iFormat );
      BitmapData dataOut = output.LockBits( new Rectangle( 0, 0, width, height ), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed );
      unsafe
      {
        byte* iptr, optr;
        int r, g, b;
        int dI = Image.GetPixelFormatSize( iFormat ) / 8;
        for ( int y = 0; y < height; y++ )
        {
          if ( !Form1.cont ) break;
          iptr = (byte*)dataIn.Scan0 + y * dataIn.Stride;
          optr = (byte*)dataOut.Scan0 + y * dataOut.Stride;
          for ( int x = 0; x < width; x++, iptr += dI )
          {
            b = (int)iptr[0] >> 6;
            g = (int)iptr[1] >> 5;
            r = (int)iptr[2] >> 5;
            *optr++ = (byte)((r << 5) + (g << 2) + b);
          }
        }
      }
      output.UnlockBits( dataOut );
      input.UnlockBits( dataIn );

      // !!!}}
    }
  }
}
