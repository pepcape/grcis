// Author: Josef Pelikan

using System.Drawing;
using MathSupport;
using System.Globalization;

namespace _033colormap
{
  public partial class Form1
  {
    /// <summary>
    /// Initialize the input data.
    /// </summary>
    void InitPaletteData ()
    {
      baseColor1 = Color.FromArgb( 180,  60,   0 );
      baseColor2 = Color.FromArgb( 255, 240, 220 );
      numCol = 4;
      param = "180.0";
    }
  }

  class Colormap
  {
    /// <summary>
    /// Generate custom palette based on 1-2 input colors.
    /// </summary>
    /// <param name="baseColor1">Mandatory primary input color.</param>
    /// <param name="baseColor2">Optional secondary input color.</param>
    /// <param name="numCol">Suggested palette size (could be ignored).</param>
    /// <param name="param">Optional string parameter.</param>
    /// <param name="colors">Output color array.</param>
    public static void Generate ( Color baseColor1, Color baseColor2, int numCol, string param, out Color[] colors )
    {
      // !!!{{ TODO - insert your custom palette generation code here

      //--------------------------------------------------
      // Sample code for HSV<->RGB conversion:
      //double H, S, V;
      //Arith.ColorToHSV( baseColor1, out H, out S, out V );
      //Color col = Arith.HSVToColor( H, S, V );

      colors = new Color[ numCol ];

      float r;
      //--------------------------------------------------
      // Sample code for parsing float from param string:
      if ( !float.TryParse( param, NumberStyles.Number, CultureInfo.InvariantCulture, out r ) )
        r = baseColor1.R;

      r = Arith.Clamp( r, 0, 255 );
      float g = baseColor1.G;
      float b = baseColor1.B;

      float dr = (baseColor2.R - r) / (numCol - 1.0f);
      float dg = (baseColor2.G - g) / (numCol - 1.0f);
      float db = (baseColor2.B - b) / (numCol - 1.0f);

      for ( int i = 0; i < numCol; i++ )
      {
        colors[ i ] = Color.FromArgb( (int)r, (int)g, (int)b );
        r += dr; g += dg; b += db;
      }

      // !!!}}

    }
  }
}
