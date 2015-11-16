// Author: Josef Pelikan

using System.Drawing;
using MathSupport;
using System.Drawing.Imaging;
using System;

namespace _081fullcolor
{
  class Generator
  {
    /// <summary>
    /// Optional data initialization.
    /// </summary>
    /// <param name="param"></param>
    public static void InitParams ( out string param )
    {
      param = "4096,4096";
    }

    /// <summary>
    /// Separator for string parameter.
    /// </summary>
    static readonly char COMMA = ',';

    /// <summary>
    /// Recompute the output image.
    /// </summary>
    /// <param name="input">Input image or null if not set.</param>
    /// <param name="output">You should put your output full-color image here.</param>
    /// <param name="fast">Use fast bitmap access?</param>
    /// <param name="param">Optional string parameter (its content and format is entierely up to you).</param>
    public static void Recompute ( Bitmap input, out Bitmap output, bool fast, string param )
    {
      // !!!{{ TODO: write your own image generation code here

      // Text parameter = image size
      int wid = 1024;
      int hei = 1024;
      if ( param.Length > 0 )
      {
        string[] size = param.Split( COMMA );
        if ( size.Length >= 2 )
        {
          if ( !int.TryParse( size[ 0 ], out wid ) )
            wid = 1024;
          if ( !int.TryParse( size[ 1 ], out hei ) )
            hei = 1024;

          if ( wid < 1 ) wid = 1;
          if ( hei < 1 ) hei = 1;
        }
      }

      output = new Bitmap( wid, hei, System.Drawing.Imaging.PixelFormat.Format24bppRgb );
      int xo, yo;
      byte ro, go, bo;

      if ( input != null )
      {
        // convert pixel data (fast memory-mapped code):
        PixelFormat iFormat = input.PixelFormat;
        if ( !PixelFormat.Format24bppRgb.Equals( iFormat ) &&
             !PixelFormat.Format32bppArgb.Equals( iFormat ) &&
             !PixelFormat.Format32bppPArgb.Equals( iFormat ) &&
             !PixelFormat.Format32bppRgb.Equals( iFormat ) )
          iFormat = PixelFormat.Format24bppRgb;

        int width  = input.Width;
        int height = input.Height;
        int xi, yi;
        BitmapData dataIn = input.LockBits( new Rectangle( 0, 0, width, height ), ImageLockMode.ReadOnly, iFormat );
        BitmapData dataOut = output.LockBits( new Rectangle( 0, 0, wid, hei ), ImageLockMode.WriteOnly, output.PixelFormat );
        unsafe
        {
          byte* iptr, optr;
          byte ri, gi, bi;
          int dI = Image.GetPixelFormatSize( iFormat ) / 8;             // pixel size in bytes
          int dO = Image.GetPixelFormatSize( output.PixelFormat ) / 8;  // pixel size in bytes

          yi = 0;
          for ( yo = 0; yo < hei; yo++ )
          {
            if ( !Form1.cont ) break;

            iptr = (byte*)dataIn.Scan0 + yi * dataIn.Stride;
            optr = (byte*)dataOut.Scan0 + yo * dataOut.Stride;

            xi = 0;
            for ( xo = 0; xo < wid; xo++ )
            {
              // read input colors
              bi = iptr[ 0 ];
              gi = iptr[ 1 ];
              ri = iptr[ 2 ];

              // !!! TODO: do anything with the colors
              bo = bi;
              go = (byte)((gi + xo) & 0xFF);
              ro = (byte)((ri + yo) & 0xFF);

              // write output colors
              optr[ 0 ] = bo;
              optr[ 1 ] = go;
              optr[ 2 ] = ro;

              iptr += dI;
              optr += dO;
              if ( ++xi >= width )
              {
                xi = 0;
                iptr = (byte*)dataIn.Scan0 + yi * dataIn.Stride;
              }
            }

            if ( ++yi >= height )
              yi = 0;
          }
        }
        output.UnlockBits( dataOut );
        input.UnlockBits( dataIn );
      }
      else
      {
        int col;
        if ( fast )
        {
          // generate pixel data (fast memory-mapped code):
          BitmapData dataOut = output.LockBits( new Rectangle( 0, 0, wid, hei ), ImageLockMode.WriteOnly, output.PixelFormat );
          unsafe
          {
            byte* optr;
            int dO = Image.GetPixelFormatSize( output.PixelFormat ) / 8;  // pixel size in bytes

            col = 0;
            for ( yo = 0; yo < hei; yo++ )
            {
              if ( !Form1.cont ) break;

              optr = (byte*)dataOut.Scan0 + yo * dataOut.Stride;

              for ( xo = 0; xo < wid; xo++, col++ )
              {
                // !!! TODO: do anything with the colors
                bo = (byte)((col >> 16) & 0xFF);
                go = (byte)((col >> 8) & 0xFF);
                ro = (byte)(col & 0xFF);

                // write output colors
                optr[ 0 ] = bo;
                optr[ 1 ] = go;
                optr[ 2 ] = ro;

                optr += dO;
              }
            }
          }
          output.UnlockBits( dataOut );
        }
        else
        {
          // generate pixel data (slow mode):
          col = 0;
          for ( yo = 0; yo < hei; yo++ )
          {
            if ( !Form1.cont ) break;

            for ( xo = 0; xo < wid; xo++, col++ )
            {
              // !!! TODO: do anything with the colors
              bo = (byte)((col >> 16) & 0xFF);
              go = (byte)((col >> 8) & 0xFF);
              ro = (byte)(col & 0xFF);

              // write output colors
              output.SetPixel( xo, yo, Color.FromArgb( ro, go, bo ) );
            }
          }
        }
      }

      // !!!}}
    }
  }
}
