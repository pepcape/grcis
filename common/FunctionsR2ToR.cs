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
      variant = 0;
      MinX = -3.0;
      MaxX =  3.0;
      MinZ = -3.0;
      MaxZ =  3.0;
    }

    public double f ( double x, double z )
    {
      return Math.Cos( x * x + z * z );
    }

    public int Variant
    {
      get;
      set;
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
