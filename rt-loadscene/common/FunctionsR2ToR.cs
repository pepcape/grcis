using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Globalization;

namespace Scene3D
{
  /// <summary>
  /// Functions R^2 -> R.
  /// </summary>
  public interface IFunctionR2ToR
  {
    double f ( double x, double z );

    int Variant
    {
      get;
      set;
    }

    double MinX
    {
      get;
      set;
    }

    double MaxX
    {
      get;
      set;
    }

    double MinZ
    {
      get;
      set;
    }

    double MaxZ
    {
      get;
      set;
    }
  }

  /// <summary>
  /// Sample functions R^2 -> R.
  /// </summary>
  public class FunctionsR2ToR : IFunctionR2ToR
  {
    protected int variant = 0;

    public FunctionsR2ToR ()
    {
      Variant = 0;
    }

    public double f ( double x, double z )
    {
      double r;
      switch ( variant )
      {
        case 0:
          return Math.Cos( x * x + z * z );

        case 1:
          return Math.Cos( x ) + Math.Cos( z );

        case 2:
          r = Math.Sqrt( x * x + z * z );
          return (r <= Double.Epsilon) ? 20.0 : (20.0 * Math.Sin( r ) / r );

        case 3:
          r = x * x + z * z;
          return (r <= Double.Epsilon) ? 20.0 : (20.0 * Math.Sin( r ) / r);

        default:
          return 1.0;
      }
    }

    public int Variant
    {
      get
      {
        return variant;
      }

      set
      {
        variant = value;
        switch ( variant )
        {
          case 0:
            MinX = -3.0;
            MaxX =  3.0;
            MinZ = -3.0;
            MaxZ =  3.0;
            break;

          case 1:
            MinX = -5.0;
            MaxX =  5.0;
            MinZ = -5.0;
            MaxZ =  5.0;
            break;

          case 2:
            MinX = -30.0;
            MaxX =  30.0;
            MinZ = -30.0;
            MaxZ =  30.0;
            break;

          case 3:
            MinX = -10.0;
            MaxX =  10.0;
            MinZ = -10.0;
            MaxZ =  10.0;
            break;
        }
      }
    }

    public double MinX
    {
      get;
      set;
    }

    public double MaxX
    {
      get;
      set;
    }

    public double MinZ
    {
      get;
      set;
    }

    public double MaxZ
    {
      get;
      set;
    }
  }
}
