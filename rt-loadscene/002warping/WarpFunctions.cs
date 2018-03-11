using System;

namespace _002warping
{
  public class WarpMagniGlass : DefaultWarp
  {
    protected double factor = 1.0;

    public override double Factor
    {
      get
      {
        return factor;
      }
      set
      {
        factor = value;
        double aspectRatio = iwidth / (double)iheight;
        aspectRatioTrimmed = 0.5 * Math.Min( 1.0, aspectRatio );
        halfFactorOffseted = 0.5 * (factor - 1.0);
        halfInverseFactorOffseted = 0.5 * (1.0 / factor - 1.0);
      }
    }

    protected double aspectRatioTrimmed, halfFactorOffseted, halfOfAspectRatioX, halfInverseFactorOffseted, halfOfAspectRatioY;

    public override void F ( double x, double y, out double u, out double v )
    {
      x /= iwidth;
      y /= iheight;
      double ax = (x - 0.5) / aspectRatioTrimmed;
      double ay = (y - 0.5) / aspectRatioTrimmed;
      double rr = ax * ax + ay * ay;    // radius squared
      double mag = rr == 0 ? 0 : aspectRatioTrimmed * Math.Exp(halfFactorOffseted * Math.Log(rr));
      u = 0.5 + mag * ax;
      v = 0.5 + mag * ay;
      u *= owidth;
      v *= oheight;
    }

    public override void FInv ( double u, double v, out double x, out double y )
    {
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

  /// <summary>
  /// Rotation angle is proportional to radius..
  /// (c) Petr Vevoda
  /// </summary>
  public class WarpSpiral : DefaultWarp
  {
    protected double factor = 1.0;

    protected double f2PI = 2 * Math.PI; // Compensation: factor * 2 * Pi

    public override double Factor
    {
      get
      {
        return factor;
      }
      set
      {
        factor = value;
        f2PI = factor * 2 * Math.PI;
      }
    }

    public override void F ( double x, double y, out double u, out double v )
    {
      x /= iwidth;
      y /= iheight;
      double ax = x - 0.5;
      double ay = y - 0.5;

      // Polar space.
      double r = Math.Sqrt( ax * ax + ay * ay );
      double angle = Math.Atan2( ay, ax );

      // Rotation.
      angle += r * f2PI;

      // Inverse mapping.
      ax = r * Math.Cos( angle );
      ay = r * Math.Sin( angle );

      u = 0.5 + ax;
      v = 0.5 + ay;
      u *= owidth;
      v *= oheight;
    }

    public override void FInv ( double u, double v, out double x, out double y )
    {
      u /= owidth;
      v /= oheight;
      double ax = u - 0.5;
      double ay = v - 0.5;

      // Polar space.
      double r = Math.Sqrt( ax * ax + ay * ay );
      double angle = Math.Atan2( ay, ax );

      // Inverse rotation.
      angle -= r * f2PI;

      // Inverse mapping.
      ax = r * Math.Cos( angle );
      ay = r * Math.Sin( angle );

      x = 0.5 + ax;
      y = 0.5 + ay;
      x *= iwidth;
      y *= iheight;
    }
  }

  /// <summary>
  /// Rotation angle is reciprocal to radius..
  /// (c) Petr Vevoda
  /// </summary>
  public class WarpInvSpiral : DefaultWarp
  {
    protected double factor = 1.0;

    public override double Factor
    {
      get
      {
        return factor;
      }
      set
      {
        factor = value;
      }
    }

    public override void F ( double x, double y, out double u, out double v )
    {
      x /= iwidth;
      y /= iheight;
      double ax = x - 0.5;
      double ay = y - 0.5;

      // Polar space.
      double r = Math.Sqrt( ax * ax + ay * ay );
      double angle = Math.Atan2( ay, ax );

      // Rotation.
      angle += r == 0 ? 0 : 0.5 * factor / r;

      // Inverse mapping.
      ax = r * Math.Cos( angle );
      ay = r * Math.Sin( angle );

      u = 0.5 + ax;
      v = 0.5 + ay;
      u *= owidth;
      v *= oheight;
    }

    public override void FInv ( double u, double v, out double x, out double y )
    {
      u /= owidth;
      v /= oheight;
      double ax = u - 0.5;
      double ay = v - 0.5;

      // Polar space.
      double r = Math.Sqrt( ax * ax + ay * ay );
      double angle = Math.Atan2( ay, ax );

      // Inverse rotation.
      angle -= r == 0 ? 0 : 0.5 * factor / r;

      // Inverse mapping.
      ax = r * Math.Cos( angle );
      ay = r * Math.Sin( angle );

      x = 0.5 + ax;
      y = 0.5 + ay;
      x *= iwidth;
      y *= iheight;
    }
  }
}
