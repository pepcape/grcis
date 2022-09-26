using System;
using MathSupport;
using OpenTK;
using Rendering;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using Utilities;

namespace EduardHopfer
{

  // TODO: also make a static class for all the different SDFs
  // TODO: implement CSG operations on implicits
  public static class ImplicitCommon
  {
    private static readonly double EpsMin   = 1e-03;
    public static readonly  double MIN_STEP = Intersection.RAY_EPSILON;
    public delegate double SignedDistanceFunction (Vector3d point);

    public static readonly SignedDistanceFunction Sphere =
      p => p.Length - 1.0;

    public static readonly SignedDistanceFunction Box =
      p =>
      {
        // Mirror p into the positive quadrant
        var pp = new Vector3d(Math.Abs(p.X) - 1.0, Math.Abs(p.Y) - 1.0, Math.Abs(p.Z) - 1.0);

        // Make the sdf negative on the inside
        double magic = Math.Min(Math.Max(pp.X, Math.Max(pp.Y, pp.Z)), 0.0);

        return Vector3d.ComponentMax(pp, Vector3d.Zero).Length + magic;
      };

    public static readonly SignedDistanceFunction MandelBulb =
      p =>
      {
        Vector3d w = p;
        double m = Vector3d.Dot(w, w);

        Vector4d trap = new Vector4d(Math.Abs(w.X), Math.Abs(w.Y), Math.Abs(w.Z), m);
        double dz = 1.0;

        for (int i = 0; i < 4; i++)
        {
          // trigonometric version (MUCH faster than polynomial)

          // dz = 8*z^7*dz
          dz = 8.0 * Math.Pow(m, 3.5) * dz + 1.0;

          // z = z^8+c
          double r = w.Length;
          double b = 8.0 * Math.Acos(w.Y / r);
          double a = 8.0 * Math.Atan2(w.X, w.Z);
          w = p + Math.Pow(r, 8.0) * new Vector3d(Math.Sin(b) * Math.Sin(a), Math.Cos(b), Math.Sin(b) * Math.Cos(a));

          trap = Vector4d.ComponentMin(trap, new Vector4d(Math.Abs(w.X), Math.Abs(w.Y), Math.Abs(w.Z), m));

          m = Vector3d.Dot(w, w);
          if (m > 256.0)
            break;
        }

        //resColor = vec4(m,trap.yzw);

        // distance estimation (through the Hubbard-Douady potential)
        return 0.25 * Math.Log(m) * Math.Sqrt(m) / dz;
      };

    public static readonly SignedDistanceFunction Torus =
      p =>
      {
        Vector3d w = p.Xzy;
        Vector2d t = new Vector2d(0.7, 0.3);
        Vector2d q = new Vector2d(w.Xz.Length - t.X, w.Y);

        return q.Length - t.Y;
      };

    public static Vector3d GetNextRayOrigin (Intersection i, RayType type)
    {
      if (i.Solid is DistanceField == false || type == RayType.REFRACT)
      {
        return i.CoordWorld;
      }

      var iData = i.SolidData as ImplicitData;
      Debug.Assert(iData is null == false, "Implicit had no data");

      double epsDynamic = 2 * Math.Max(Math.Abs(iData.Distance), EpsMin);
      return i.CoordWorld + i.Normal * epsDynamic;
    }
  }

  public enum RayType
  {
    SHADOW,
    REFLECT,
    REFRACT
  }

  sealed class Ray
  {
    public Vector3d Origin { get; set; }
    public Vector3d Dir    { get; set; }
  }

  public sealed class WeightedSurface
  {
    public DistanceField Solid  { get; set; }
    public double        Weight { get; set; }
  }

  sealed class SDFResult
  {
    public double                       Distance { get; set; }
    public DistanceField                Chosen   { get; set; }
    public IEnumerable<WeightedSurface> Weights  { get; set; }
  }

  public sealed class ImplicitData
  {
    public double Distance { get; set; }

    public IEnumerable<WeightedSurface> Weights { get; set; }
  }

  public interface IImplicitOperation
  {
    bool ChooseRight (double leftSdfResult, double rightSdfResult);

    void Transform (ref double left, ref double right);

    double GetWeightRight (double left, double right);
  }

  public sealed class ImplicitIntersection : IImplicitOperation
  {
    public bool ChooseRight (double leftSdfResult, double rightSdfResult)
    {
      return rightSdfResult > leftSdfResult;
    }

    public void Transform(ref double left, ref double right)
    {}

    public double GetWeightRight (double left, double right)
    {
      bool chooseRight = this.ChooseRight(left, right);

      return chooseRight ? 1.0 : 0.0;
    }
  }

  // TODO: add blending
  public sealed class ImplicitUnion : IImplicitOperation
  {
    private readonly double blendCoefficient;

    private ImplicitUnion (){}

    public ImplicitUnion (double blendCoefficient = 0.0)
    {
      this.blendCoefficient = blendCoefficient;
    }

    public bool ChooseRight (double left, double right)
    {
      return right < left;
    }

    public void Transform (ref double left, ref double right)
    {
      if (this.blendCoefficient <= 0.0)
      {
        return;
      }

      double res = Math.Exp( -this.blendCoefficient * left ) + Math.Exp( -this.blendCoefficient * right );
      double smoothness = -Math.Log(res) / this.blendCoefficient;
      //return -log2( res )/k;

      //double h = Math.Max(this.blendCoefficient - Math.Abs(left - right), 0.0) / this.blendCoefficient;
      //double smoothness = h * h * this.blendCoefficient * (1.0 / 4.0);

      if (right < left)
      {
        //right -= smoothness;
        right = smoothness;
      }
      else
      {
        //left -= smoothness;
        left = smoothness;
      }
    }

    public double GetWeightRight (double left, double right)
    {
      double weight = Math.Exp(-this.blendCoefficient * right);
      return Math.Min(Math.Max(0.0, weight), 1.0);

      //double h = Math.Max(this.blendCoefficient - Math.Abs(left - right), 0.0) / this.blendCoefficient;
      //return 1.0 - (h * h * 0.5);
    }
  }

  public sealed class ImplicitDifference : IImplicitOperation
  {
    public bool ChooseRight (double leftSdfResult, double rightSdfResult)
    {
      return rightSdfResult > leftSdfResult;
    }

    public void Transform (ref double left, ref double right)
    {
      right *= -1; // invert the shape
    }

    public double GetWeightRight (double left, double right)
    {
      bool chooseRight = this.ChooseRight(left, right);

      return chooseRight ? 1.0 : 0.0;
    }
  }

}
