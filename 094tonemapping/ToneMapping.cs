using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using MathSupport;
using Raster;
using Utilities;

namespace _094tonemapping
{
  class ToneMapping
  {
    /// <summary>
    /// Optional data initialization.
    /// </summary>
    /// <param name="name">Your first-name and last-name.</param>
    /// <param name="param">Optinal text parameter from the form's text-field.</param>
    /// <param name="tooltip">Optional tooltip = param help.</param>
    public static void InitParams ( out string name, out string param, out string tooltip )
    {
      // {{
      name = "Josef Pelikán";
      param = "gamma=2.5";
      tooltip = "gamma=<double> (0 for no gamma pre-compensation)";
      // }}
    }

    /// <summary>
    /// Tone-mapping of the input HDR image.
    /// </summary>
    /// <param name="input">Mandatory HDR image.</param>
    /// <param name="result">Optional (pre-allocatied) output LDR image.</param>
    /// <param name="param">Optional text parameters (from form's text-field).</param>
    /// <returns>Output LDR image (use pre-allocated 'result' if possible).</returns>
    public static unsafe Bitmap ToneMap ( FloatImage input, Bitmap result, string param )
    {
      if ( input == null )
        return null;

      // {{ TODO: write your own tone-mapping reduction code here

      // custom parameters from the text-field:
      Dictionary<string, string> p = Util.ParseKeyValueList( param );
      double g = 0.0;
      if ( p.Count > 0 )
      {
        // gamma=<float-number>
        if ( Util.TryParse( p, "gamma", ref g ) &&
             g > 0.001 &&
             Math.Abs( g - 1.0 ) > 0.001 )
          g = 1.0 / g;
        else
          g = 0.0;
      }

      int width  = input.Width;
      int height = input.Height;

      // re-allocate the output image if necessary:
      if ( result == null ||
           result.Width != width ||
           result.Height != height ||
           result.PixelFormat != PixelFormat.Format24bppRgb )
        result = new Bitmap( width, height, PixelFormat.Format24bppRgb );

      // input range:
      double minY, maxY;
      input.Contrast( out minY, out maxY );
      double coef = 1.0 / maxY;   // the whole range is mapped into (0.0,1.0>

      BitmapData dataOut = result.LockBits( new Rectangle( 0, 0, width, height ), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb );
      fixed ( float* id = input.Data )
      {
        float* iptr;
        byte* optr;
        int dO = Image.GetPixelFormatSize( PixelFormat.Format24bppRgb ) / 8;    // BGR

        for ( int y = 0; y < height; y++ )
        {
          iptr = id + input.Scan0 + y * input.Stride;
          optr = (byte*)dataOut.Scan0 + y * dataOut.Stride;

          if ( g > 0.0 )
            for ( int x = 0; x++ < width; iptr += input.Channels, optr += dO )
            {
              optr[ 2 ] = (byte)Arith.Clamp( 255.999 * Math.Pow( iptr[ 0 ] * coef, g ), 0.0, 255.0 );
              optr[ 1 ] = (byte)Arith.Clamp( 255.999 * Math.Pow( iptr[ 1 ] * coef, g ), 0.0, 255.0 );
              optr[ 0 ] = (byte)Arith.Clamp( 255.999 * Math.Pow( iptr[ 2 ] * coef, g ), 0.0, 255.0 );
            }
          else
            for ( int x = 0; x++ < width; iptr += input.Channels, optr += dO )
            {
              optr[ 2 ] = (byte)Arith.Clamp( 255.999 * iptr[ 0 ] * coef, 0.0, 255.0 );
              optr[ 1 ] = (byte)Arith.Clamp( 255.999 * iptr[ 1 ] * coef, 0.0, 255.0 );
              optr[ 0 ] = (byte)Arith.Clamp( 255.999 * iptr[ 2 ] * coef, 0.0, 255.0 );
            }
        }
      }
      result.UnlockBits( dataOut );

      // }}

      return result;
    }
  }
}
