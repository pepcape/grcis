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

    protected double aspectRatioTrimmed, halfFactorOffseted, halfOfAspectRatioX, halfInverseFactorOffseted, halfOfAspectRatioY;

    public override void F ( double x, double y, out double u, out double v )
    {
      if ( dirty )
      {
        double aspectRatio = iwidth / (double)iheight;
        aspectRatioTrimmed = 0.5 * Math.Min( 1.0, aspectRatio );
        halfFactorOffseted = 0.5 * (factor - 1.0);
        halfOfAspectRatioX = 0.5 * aspectRatio;
        halfInverseFactorOffseted = 0.5 * (1.0 / factor - 1.0);
        dirty = false;
      }
      x /= iwidth;
      y /= iheight;
      double ax = (x - halfOfAspectRatioX) / aspectRatioTrimmed;
      double ay = (y - 0.5) / aspectRatioTrimmed;
      double rr = ax * ax + ay * ay;    // radius squared
      double mag = aspectRatioTrimmed * Math.Exp( halfFactorOffseted * Math.Log( rr ) );
      u = halfOfAspectRatioX + mag * ax;
      v = 0.5 + mag * ay;
      u *= owidth;
      v *= oheight;
    }

    public override void FInv ( double u, double v, out double x, out double y )
    {
      if ( dirty )
      {
        double aspectRatio = iwidth / (double)iheight;
        aspectRatioTrimmed = 0.5 * Math.Min( 1.0, aspectRatio );
        halfFactorOffseted = 0.5 * (factor - 1.0);
        halfOfAspectRatioX = 0.5 * aspectRatio;
        halfOfAspectRatioY = 0.5 * 1 / aspectRatio;
        halfInverseFactorOffseted = 0.5 * (1.0 / factor - 1.0);
        dirty = false;
      }
      u /= owidth;
      v /= oheight;
      double ax = (u - 0.5) / aspectRatioTrimmed;
      double ay = (v - 0.5) / aspectRatioTrimmed;
      double rr = ax * ax + ay * ay;    // radius squared
      double mag = rr == 0 ? 0 : aspectRatioTrimmed * Math.Exp( halfInverseFactorOffseted * Math.Log( rr ) );
      x = 0.5 + mag * ax;
      y = 0.5 + mag * ay;
      x *= iwidth;
      y *= iheight;
    }
  }
}
