using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Text;

namespace _033colormap
{
  class Colormap
  {
    public static void generate ( Color baseColor1, Color baseColor2, int numCol, out Color[] colors )
    {
      // !!!{{ TODO - generate custom palette based on numCol, baseColor1 [and baseColor2]

      colors = new Color[ numCol ];
      colors[ 0 ] = baseColor1;
      float r = baseColor1.R;
      float g = baseColor1.G;
      float b = baseColor1.B;
      float dr = (baseColor2.R - r) / (numCol - 1.0f);
      float dg = (baseColor2.G - g) / (numCol - 1.0f);
      float db = (baseColor2.B - b) / (numCol - 1.0f);
      for ( int i = 1; i < numCol; i++ )
      {
        r += dr; g += dg; b += db;
        colors[ i ] = Color.FromArgb( (int)r, (int)g, (int)b );
      }

      // !!!}}

    }
  }
}
