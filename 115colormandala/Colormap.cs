using System;
using System.Collections.Generic;
using System.Drawing;
using MathSupport;
using Utilities;

namespace _115colormandala
{
  class Colormap
  {
    /// <summary>
    /// Form data initialization.
    /// </summary>
    /// <param name="name">Your first-name and last-name.</param>
    /// <param name="param">Optional text to initialize the form's text-field.</param>
    /// <param name="tooltip">Optional tooltip = param help.</param>
    public static void InitParams ( out string name, out string param, out string tooltip )
    {
      // {{
      name = "Josef Pelikán";
      param = "";
      tooltip = "base=[R;G;B], minS=<double>, minV=<byte>";
      // }}
    }

    /// <summary>
    /// Generate a cyclic colormap.
    /// </summary>
    /// <param name="numCol">Required colormap size (ignore it if you must).</param>
    /// <param name="colors">Output palette (array of colors).</param>
    /// <param name="param">Optional string parameter (its content and format is entierely up to you).</param>
    public static void Generate ( int numCol, out Color[] colors, string param )
    {
      // {{ TODO - generate custom palette based on the given image

      // Base color, the whole colormap will have the same Hue.
      List<int> baseColor = new List<int> { 40, 200, 0 };

      // Minimum saturation (maximum saturation will be 1.0).
      double minSat = 0.2;

      // Minimum value (maximum value will be 255).
      int minVal = 32;

      Dictionary<string, string> p = Util.ParseKeyValueList( param );
      if ( p.Count > 0 )
      {
        // Base color.
        // base=[R;G;B]
        if ( Util.TryParse( p, "base", ref baseColor, ';' ) )
          while ( baseColor.Count < 3 )
            baseColor.Add( 0 );

        // Minimum saturation.
        // minS=<double>
        if ( Util.TryParse( p, "minS", ref minSat ) )
          minSat = Util.Clamp( minSat, 0.0, 1.0 );

        // Minimum value.
        // minV=<int>
        if ( Util.TryParse( p, "minV", ref minVal ) )
          minVal = Util.Clamp( minVal, 0, 255 );

        // ... you can add more parameters here ...
      }

      colors = new Color[ numCol ];                   // accepting the required palette size..

      double H, S, V;
      Arith.ColorToHSV( Color.FromArgb( baseColor[ 0 ], baseColor[ 1 ], baseColor[ 2 ] ), out H, out S, out V );
      // Only Hue (H) will be used..

      double a = 0.0;
      double da = 2.0 * Math.PI / numCol;
      double Vmean = 0.5 * (1.0 + minVal / 255.0);    // V must be in the [0, 1] domain for the Arith.HSVToColor() function
      double Vamplitude = 1.0 - Vmean;
      double Smean = 0.5 * (1.0 + minSat);
      double Samplitude = 1.0 - Smean;

      for ( int i = 0; i < numCol; i++, a += da )
      {
        S = Smean + Samplitude * Math.Sin( a );
        V = Vmean + Vamplitude * Math.Cos( a );
        colors[ i ] = Arith.HSVToColor( H, S, V );
      }

      // }}
    }
  }
}
