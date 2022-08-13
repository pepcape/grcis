using System;
using MathSupport;
using OpenTK;
using Rendering;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;

namespace EduardHopfer
{

  // TODO: also make a static class for all the different SDFs
  // TODO: implement CSG operations on implicits
  public static class ImplicitCommon
  {
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

  }

}
