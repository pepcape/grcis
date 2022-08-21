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
    private static readonly double EpsMin = 1e-03;
    public delegate double SignedDistanceFunction (Vector3d point);

    public static readonly SignedDistanceFunction Sphere =
      p => p.Length - 1.0;

    public static readonly SignedDistanceFunction Box =
      p =>
      {
        // Mirror p into the positive quadrant
        var pp = new Vector3d(Math.Abs(p.X) - 1.0, Math.Abs(p.Y) - 1.0, Math.Abs(p.Z) - 1.0);

        // Makes the sdf negative on the inside
        double magic = Math.Min(Math.Max(pp.X, Math.Max(pp.Y, pp.Z)), 0.0);

        return Vector3d.ComponentMax(pp, Vector3d.Zero).Length + magic;
      };

    public static Vector3d GetNextRayOrigin (Intersection i, RayType type)
    {
      if (i.Solid is DistanceField == false || type == RayType.REFRACT)
      {
        return i.CoordWorld;
      }

      var iData = i.SolidData as ImplicitData;
      Debug.Assert(iData is null == false, "Implicit had no data");

      double epsDynamic = 2 * Math.Max(Math.Abs(iData.distance), EpsMin);
      return i.CoordWorld + i.Normal * epsDynamic;
    }
  }

  public enum RayType
  {
    SHADOW,
    REFLECT,
    REFRACT
  }


  public class ImplicitData
  {
    public double distance { get; set; }
  }

}
