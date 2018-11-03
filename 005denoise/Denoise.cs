using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;
using Raster;
using Utilities;

namespace _005denoise
{
  class Filter
  {
    /// <summary>
    /// Optional data initialization.
    /// </summary>
    public static void InitParams ( out string name, out string param, out string tooltip )
    {
      // {{
      name = "Josef Pelikán";
      param = "d=3, par=true";
      tooltip = "d=<int> .. window diameter in pixels, mode={min|max|median|mid}, par=<bool>";
      // }}
    }

    /// <summary>
    /// Recompute the image from the given input.
    /// </summary>
    /// <param name="input">Input image.</param>
    /// <param name="param">Optional string parameter (its content and format is entierely up to you).</param>
    public static Bitmap Recompute ( Bitmap input, string param )
    {
      // {{ TODO: write your own image transform code here

      if ( input == null )
        return null;

      // !!! TODO: process the param string if you need !!!
      int radius = 1;
      RankHistogram.RankType mode = RankHistogram.RankType.MEDIAN;
      bool par = true;

      Dictionary<string, string> p = Util.ParseKeyValueList( param );
      if ( p.Count > 0 )
      {
        // d=<int>
        if ( Util.TryParse( p, "d", ref radius ) )
          radius >>= 1;

        // mode={min|max|median|mid}
        string mods;
        if ( p.TryGetValue( "mode", out mods ) )
          switch ( mods )
          {
            case "min":
              mode = RankHistogram.RankType.MIN;
              break;

            case "max":
              mode = RankHistogram.RankType.MAX;
              break;

            case "mid":
              mode = RankHistogram.RankType.MIDDLE;
              break;

            default:
              mode = RankHistogram.RankType.MEDIAN;
              break;
          }

        // par=<bool>
        // use Parallel.For?
        Util.TryParse( p, "par", ref par );

        // ... you can add more parameters here ...
      }

      // Pilot implementation = rank filter with rectangular window
      int wid = input.Width;
      int hei = input.Height;

      // Fast memory-mapped code.
      PixelFormat iFormat = input.PixelFormat;
      if ( !PixelFormat.Format24bppRgb.Equals( iFormat ) &&
           !PixelFormat.Format32bppArgb.Equals( iFormat ) &&
           !PixelFormat.Format32bppPArgb.Equals( iFormat ) &&
           !PixelFormat.Format32bppRgb.Equals( iFormat ) )
        iFormat = PixelFormat.Format24bppRgb;

      Bitmap output = new Bitmap( wid, hei, PixelFormat.Format8bppIndexed );

      // Set gray ramp as the output palette.
      ColorPalette pal = output.Palette;
      Draw.PaletteGray( pal );
      output.Palette = pal;

      Rectangle entire = new Rectangle( 0, 0, wid, hei );
      BitmapData dataIn  = input.LockBits( entire, ImageLockMode.ReadOnly, iFormat );
      BitmapData dataOut = output.LockBits( entire, ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed );
      unsafe
      {
        int dI = Image.GetPixelFormatSize( iFormat ) / 8;

        Action<int> body = oy =>
        {
          if ( !Form1.cont ) return;

          int ox;      // output pixel coordinates
          int ix, iy;  // input pixel coordinates

          // Window bounds.
          int miny = Math.Max( 0, oy - radius );
          int maxy = Math.Min( hei - 1, oy + radius );

          byte* iptr;
          byte* optr = (byte*)dataOut.Scan0 + oy * dataOut.Stride;
          RankHistogram rank = new RankHistogram( 255 );

          for ( ox = 0; ox < wid; ox++ )     // one output pixel
          {
            rank.init();

            for ( iy = miny; iy <= maxy; iy++ )
            {
              // One hline of the filter window.
              int minx = Math.Max( 0, ox - radius );
              int maxx = Math.Min( wid - 1, ox + radius );
              iptr = (byte*)dataIn.Scan0 + iy * dataIn.Stride + minx * dI;

              for ( ix = minx; ix <= maxx; ix++, iptr += dI )
                rank.add( Draw.RgbToGray( iptr[ 2 ], iptr[ 1 ], iptr[ 0 ] ) );
            }

            *optr++ = (byte)rank.result( mode );
          }
        };

        if ( par )
          Parallel.For( 0, hei, body );
        else
          for ( int y = 0; y < hei; y++ )
            body( y );
      }
      output.UnlockBits( dataOut );
      input.UnlockBits( dataIn );

      return output;

      // }}
    }
  }

  /// <summary>
  /// Rank collecting class based on histogram implementation.
  /// Able to efficiently compute median/middle/min/max values.
  /// </summary>
  class RankHistogram
  {
    /// <summary>
    /// Maximal allowed input value.
    /// </summary>
    protected int maxV;

    /// <summary>
    /// Working histogram array.
    /// </summary>
    protected int[] h;

    /// <summary>
    /// Current minimum value.
    /// </summary>
    protected int cMin;

    /// <summary>
    /// Current maximum value.
    /// </summary>
    protected int cMax;

    /// <summary>
    /// Histogram size (number of values).
    /// </summary>
    protected int card;

    /// <summary>
    /// 
    /// </summary>
    public enum RankType
    {
      MIN,      // minimum (erosion)
      MAX,      // maximum (dilation)
      MIDDLE,   // middle of the range = (min+max)/2
      MEDIAN    // median
    }

    public RankHistogram ( int maxValue = 255 )
    {
      init( maxValue );
    }

    /// <summary>
    /// [Re-]initialization.
    /// </summary>
    /// <param name="maxValue">If negative, the old histogram domain is preserved.</param>
    public void init ( int maxValue = -1 )
    {
      if ( maxValue >= 0 &&
           (h == null ||
            maxV != maxValue) )
        h = new int[ (maxV = maxValue) + 1 ];
      else
        Array.Clear( h, 0, maxV );

      cMin = maxV;
      cMax = 0;
      card = 0;
    }

    /// <summary>
    /// Add a next value.
    /// </summary>
    public void add ( int value )
    {
      if ( value < 0 ||
           value > maxV ) return;

      h[ value ]++;
      if ( value < cMin ) cMin = value;
      if ( value > cMax ) cMax = value;
      card++;
    }

    /// <summary>
    /// Returns the required result. Nondestructive.
    /// </summary>
    public int result ( RankType type = RankType.MEDIAN )
    {
      switch ( type )
      {
        case RankType.MIN:
          return cMin;

        case RankType.MAX:
          return cMax;

        case RankType.MIDDLE:
          return (cMin + cMax + 1) >> 1;

        default:
          {
            // I'm going to compute median..
            int steps = card >> 1;            // .. which is the (steps+1)-th least value ..
            int median = cMin;
            while ( true )
            {
              steps -= h[ median ];
              if ( steps < 0 )
                break;
              median++;
            }

            return median;
          }
      }
    }
  }
}
