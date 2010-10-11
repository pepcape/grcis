using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _002warping
{
  public class WarpMagniGlass : DefaultWarp
  {
    protected bool dirty = true;

    protected double factor = 1.0;

    public double Factor
    {
      get
      {
        return factor;
      }
      set
      {
        factor = value;
        dirty  = true;
      }
    }

    protected double tmp1, tmp2, tmp3, tmp4;

    public override void F ( double x, double y, out double u, out double v )
    {
      if ( dirty )
      {
        double aspectRatio = iwidth / (double)iheight;
        tmp1 = 0.5 * Math.Min( 1.0, aspectRatio );
        tmp2 = 0.5 * (factor - 1.0);
        tmp3 = 0.5 * aspectRatio;
        tmp4 = 0.5 * (1.0 / factor - 1.0);
        dirty = false;
      }
      x /= iwidth;
      y /= iheight;
      double ax = (x - tmp3) / tmp1;
      double ay = (y - 0.5) / tmp1;
      double rr = ax * ax + ay * ay;    // radius squared
      double mag = tmp1 * Math.Exp( tmp2 * Math.Log( rr ) );
      u = tmp3 + mag * ax;
      v = 0.5 + mag * ay;
      u *= owidth;
      v *= oheight;
    }

    public override void FInv ( double u, double v, out double x, out double y )
    {
      if ( dirty )
      {
        double aspectRatio = iwidth / (double)iheight;
        tmp1 = 0.5 * Math.Min( 1.0, aspectRatio );
        tmp2 = 0.5 * (factor - 1.0);
        tmp3 = 0.5 * aspectRatio;
        tmp4 = 0.5 * (1.0 / factor - 1.0);
        dirty = false;
      }
      u /= owidth;
      v /= oheight;
      double ax = (u - tmp3) / tmp1;
      double ay = (v - 0.5) / tmp1;
      double rr = ax * ax + ay * ay;    // radius squared
      double mag = tmp1 * Math.Exp( tmp4 * Math.Log( rr ) );
      x = tmp3 + mag * ax;
      y = 0.5 + mag * ay;
      x *= iwidth;
      y *= iheight;
    }
  }
}
