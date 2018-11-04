using System.Drawing;
using MathSupport;
using System.Drawing.Imaging;
using System;

namespace _084filter
{
  class Filter
  {
    /// <summary>
    /// Optional data initialization.
    /// </summary>
    /// <param name="name">Return your full name.</param>
    public static void InitParams ( out string name, out string param, out string tooltip )
    {
      // {{

      name = "Josef Pelikán";
      param = "12,8";
      tooltip = "<boxw>[,<boxh>] .. box size in pixels";

      // }}
    }

    /// <summary>
    /// Separator for string parameter.
    /// </summary>
    static readonly char COMMA = ',';

    /// <summary>
    /// Recompute the output image.
    /// </summary>
    /// <param name="input">Input image.</param>
    /// <param name="param">Optional string parameter (its content and format is entierely up to you).</param>
    public static Bitmap Recompute ( Bitmap input, string param )
    {
      // {{ TODO: write your own image transform code here

      if ( input == null )
        return null;

      // Text parameter = image size
      int wid = input.Width;
      int hei = input.Height;
      int cellW = 8;
      int cellH = 8;
      if ( param.Length > 0 )
      {
        string[] size = param.Split( COMMA );
        if ( size.Length > 0 )
        {
          if ( !int.TryParse( size[ 0 ], out cellW ) )
            cellW = 1;
          if ( cellW < 1 ) cellW = 1;
          cellH = cellW;

          if ( size.Length > 1 )
          {
            if ( !int.TryParse( size[ 1 ], out cellH ) )
              cellH = cellW;
            if ( cellH < 1 ) cellH = 1;
          }
        }
      }

      Bitmap output = new Bitmap( wid, hei, PixelFormat.Format24bppRgb );
           
      // convert pixel data:
      int x, y;

#if false
      // slow GetPixel-SetPixel code:
      for ( y = 0; y < hei; y++ )
      {
        if ( !Form1.cont ) break;

        for ( x = 0; x < wid; x++ )
        {
          Color ic = input.GetPixel( x, y );
          Color oc = Color.FromArgb( 255 - ic.R, 255 - ic.G, 255 - ic.B );
          output.SetPixel( x, y, oc );
        }
      }
#else
      // fast memory-mapped code:
      PixelFormat iFormat = input.PixelFormat;
      if ( !PixelFormat.Format24bppRgb.Equals( iFormat ) &&
           !PixelFormat.Format32bppArgb.Equals( iFormat ) &&
           !PixelFormat.Format32bppPArgb.Equals( iFormat ) &&
           !PixelFormat.Format32bppRgb.Equals( iFormat ) )
        iFormat = PixelFormat.Format24bppRgb;

      int x0, y0;
      int x1, y1;
      BitmapData dataIn  = input.LockBits( new Rectangle( 0, 0, wid, hei ), ImageLockMode.ReadOnly, iFormat );
      BitmapData dataOut = output.LockBits( new Rectangle( 0, 0, wid, hei ), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb );
      unsafe
      {
        byte* iptr, optr;
        byte r, g, b;
        int sR, sG, sB;
        int pixels;
        int dI = Image.GetPixelFormatSize( iFormat ) / 8;
        int dO = Image.GetPixelFormatSize( PixelFormat.Format24bppRgb ) / 8;

        for ( y0 = 0; y0 < hei; y0 += cellH )
        {
          if ( !Form1.cont ) break;

          for ( x0 = 0; x0 < wid; x0 += cellW )     // one output cell
          {
            sR = sG = sB = 0;
            x1 = Math.Min( x0 + cellW, wid );
            y1 = Math.Min( y0 + cellH, hei );

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
            r = (byte)((sR + pixels / 2) / pixels);
            g = (byte)((sG + pixels / 2) / pixels);
            b = (byte)((sB + pixels / 2) / pixels);

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
#endif

      return output;

      // }}
    }
  }
}
