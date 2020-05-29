using MathSupport;
using OpenTK;
using Scene3D;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Rendering
{
  /// <summary>
  /// Unit sphere as a simple solid able to compute ray-intersection, normal vector
  /// and 2D texture coordinates.
  /// </summary>
  [Serializable]
  public class Sphere : DefaultSceneNode, ISolid
  {
    /// <summary>
    /// Computes the complete intersection of the given ray with the object.
    /// </summary>
    /// <param name="p0">Ray origin.</param>
    /// <param name="p1">Ray direction vector.</param>
    /// <returns>Sorted list of intersection records.</returns>
    public override LinkedList<Intersection> Intersect (Vector3d p0, Vector3d p1)
    {
      double OD;
      Vector3d.Dot(ref p0, ref p1, out OD);
      double DD;
      Vector3d.Dot(ref p1, ref p1, out DD);
      double OO;
      Vector3d.Dot(ref p0, ref p0, out OO);
      double d = OD * OD + DD * (1.0 - OO); // discriminant
      if (d <= 0.0)
        return null;            // no intersections

      d = Math.Sqrt(d);

      // there will be two intersections: (-OD - d) / DD, (-OD + d) / DD
      LinkedList<Intersection> result = new LinkedList<Intersection> ();
      Intersection i;
      double t;

      // first intersection (-OD - d) / DD:
      t = (-OD - d) / DD;
      i = new Intersection(this)
      {
        T           = t,
        Enter       = true,
        Front       = true,
        NormalLocal = p0 + t * p1,
        CoordLocal  = p0 + t * p1,
      };
      result.AddLast(i);

      // second intersection (-OD + d) / DD:
      t = (-OD + d) / DD;
      i = new Intersection(this)
      {
        T           = t,
        Enter       = false,
        Front       = false,
        NormalLocal = p0 + t * p1,
        CoordLocal  = p0 + t * p1,
      };
      result.AddLast(i);

      return result;
    }

    /// <summary>
    /// Complete all relevant items in the given Intersection object.
    /// </summary>
    /// <param name="inter">Intersection instance to complete.</param>
    public override void CompleteIntersection (Intersection inter)
    {
      // Normal vector - no need to do anything here as NormalLocal is defined...

      // 2D texture coordinates.
      double r = Math.Sqrt(inter.CoordLocal.X * inter.CoordLocal.X + inter.CoordLocal.Y * inter.CoordLocal.Y);
      inter.TextureCoord.X = Geometry.IsZero(r)
        ? 0.0
        : (Math.Atan2(inter.CoordLocal.Y, inter.CoordLocal.X) / (2.0 * Math.PI) + 0.5);
      inter.TextureCoord.Y = Math.Atan2(r, inter.CoordLocal.Z) / Math.PI;
    }

    public void GetBoundingBox (out Vector3d corner1, out Vector3d corner2)
    {
      corner1 = new Vector3d(-1, -1, -1);
      corner2 = new Vector3d( 1,  1,  1);
    }
  }

  /// <summary>
  /// Simple unit sphere showing only the front face,
  /// texture coordinates are representing projected distance from the center.
  /// </summary>
  [Serializable]
  public class SphereFront : Sphere
  {
    /// <summary>
    /// Compute two intersections? (not needed for glow effects).
    /// </summary>
    public bool shell;

    public SphereFront (in bool sh = true)
    {
      shell = sh;
    }

    /// <summary>
    /// Computes the complete intersection of the given ray with the object.
    /// </summary>
    /// <param name="p0">Ray origin.</param>
    /// <param name="p1">Ray direction vector.</param>
    /// <returns>Single intersection.</returns>
    public override LinkedList<Intersection> Intersect (Vector3d p0, Vector3d p1)
    {
      double OD;
      Vector3d.Dot(ref p0, ref p1, out OD);
      double DD;
      Vector3d.Dot(ref p1, ref p1, out DD);
      double OO;
      Vector3d.Dot(ref p0, ref p0, out OO);
      double d = OD * OD + DD * (1.0 - OO); // discriminant
      if (d <= 0.0)
        return null;            // no intersections

      // Single intersection: (-OD - d) / DD.
      LinkedList<Intersection> result = new LinkedList<Intersection> ();
      double t = (-OD - Math.Sqrt(d)) / DD;
      Vector3d loc = p0 + t * p1;

      Intersection i = new Intersection(this)
      {
        T           = t,
        Enter       = true,
        Front       = true,
        NormalLocal = loc,
        CoordLocal  = loc,
        SolidData   = p1
      };
      result.AddLast(i);

      if (shell)
      {
        i = new Intersection(this)
        {
          T           = t + Intersection.SHELL_THICKNESS,
          Enter       = false,
          Front       = false,
          NormalLocal = loc + Intersection.SHELL_THICKNESS * p1,
          CoordLocal  = loc + Intersection.SHELL_THICKNESS * p1,
          SolidData   = p1
        };
        result.AddLast(i);
      }

      return result;
    }

    /// <summary>
    /// Complete all relevant items in the given Intersection object.
    /// </summary>
    /// <param name="inter">Intersection instance to complete.</param>
    public override void CompleteIntersection (Intersection inter)
    {
      // Normal vector - no need to do anything here as NormalLocal is defined...

      // 2D texture coordinates - projection to 2D from the ray-direction.
      if (inter.SolidData is Vector3d direction)
      {
        direction.Normalize();
        Vector3d locNormal = inter.CoordLocal;
        locNormal.Normalize();
        double cosa = Vector3d.Dot(direction, locNormal);
        double sina = Math.Sqrt(1.0 - cosa * cosa);
        // sina = distance_from_center
        inter.TextureCoord.X =
        inter.TextureCoord.Y = sina;
      }
      else
      {
        double r = Math.Max(1.0e-12, Math.Sqrt(inter.CoordLocal.X * inter.CoordLocal.X + inter.CoordLocal.Y * inter.CoordLocal.Y));
        inter.TextureCoord.X = inter.CoordLocal.X / r;
        inter.TextureCoord.Y = inter.CoordLocal.Y / r;
      }
    }
  }

  public class BoundingSphere : IBoundingVolume
  {
    double rr;

    public BoundingSphere (double r = 1.0)
    {
      rr = r * r;
    }

    public double Intersect (Vector3d p0, Vector3d p1)
    {
      double OD;
      Vector3d.Dot(ref p0, ref p1, out OD);
      double DD;
      Vector3d.Dot(ref p1, ref p1, out DD);
      double OO;
      Vector3d.Dot(ref p0, ref p0, out OO);
      double d = OD * OD + DD * (rr - OO); // discriminant
      if (d <= 0.0)
        return -1.0;           // no intersections

      d = Math.Sqrt(d);

      // There will be two intersections: (-OD - d) / DD, (-OD + d) / DD
      if (d - OD < 0.0) // negative intersection
        return -1.0;

      if ((d += OD) >= 0.0)
        return 0.0;

      return -d / DD;
    }
  }

  /// <summary>
  /// Normalized (unit) cube as a simple solid able to compute ray-intersection, normal vector
  /// and 2D texture coordinates. [0,1]^3
  /// </summary>
  [Serializable]
  public class Cube : DefaultSceneNode, ISolid
  {
    protected enum CubeFaces
    {
      PositiveX,
      NegativeX,
      PositiveY,
      NegativeY,
      PositiveZ,
      NegativeZ
    };

    protected static Vector3d[] Normals =
    {
      new Vector3d( 1,  0,  0),
      new Vector3d(-1,  0,  0),
      new Vector3d( 0,  1,  0),
      new Vector3d( 0, -1,  0),
      new Vector3d( 0,  0,  1),
      new Vector3d( 0,  0, -1)
    };

    /// <summary>
    /// Computes the complete intersection of the given ray with the object.
    /// </summary>
    /// <param name="p0">Ray origin.</param>
    /// <param name="p1">Ray direction vector.</param>
    /// <returns>Sorted list of intersection records.</returns>
    public override LinkedList<Intersection> Intersect (Vector3d p0, Vector3d p1)
    {
      double    tMin = double.MinValue;
      double    tMax = double.MaxValue;
      CubeFaces fMin = CubeFaces.PositiveX;
      CubeFaces fMax = CubeFaces.PositiveX;
      double    mul, t1, t2;

      // normal = X axis:
      if (Geometry.IsZero(p1.X))
      {
        if (p0.X <= 0.0 || p0.X >= 1.0)
          return null;
      }
      else
      {
        mul = 1.0 / p1.X;
        t1 = -p0.X * mul;
        t2 = t1 + mul;

        if (mul > 0.0)
        {
          if (t1 > tMin)
          {
            tMin = t1;
            fMin = CubeFaces.NegativeX;
          }

          if (t2 < tMax)
          {
            tMax = t2;
            fMax = CubeFaces.PositiveX;
          }
        }
        else
        {
          if (t2 > tMin)
          {
            tMin = t2;
            fMin = CubeFaces.PositiveX;
          }

          if (t1 < tMax)
          {
            tMax = t1;
            fMax = CubeFaces.NegativeX;
          }
        }

        if (tMin >= tMax)
          return null;
      }

      // normal = Y axis:
      if (Geometry.IsZero(p1.Y))
      {
        if (p0.Y <= 0.0 || p0.Y >= 1.0)
          return null;
      }
      else
      {
        mul = 1.0 / p1.Y;
        t1 = -p0.Y * mul;
        t2 = t1 + mul;

        if (mul > 0.0)
        {
          if (t1 > tMin)
          {
            tMin = t1;
            fMin = CubeFaces.NegativeY;
          }

          if (t2 < tMax)
          {
            tMax = t2;
            fMax = CubeFaces.PositiveY;
          }
        }
        else
        {
          if (t2 > tMin)
          {
            tMin = t2;
            fMin = CubeFaces.PositiveY;
          }

          if (t1 < tMax)
          {
            tMax = t1;
            fMax = CubeFaces.NegativeY;
          }
        }

        if (tMin >= tMax)
          return null;
      }

      // normal = Z axis:
      if (Geometry.IsZero(p1.Z))
      {
        if (p0.Z <= 0.0 || p0.Z >= 1.0)
          return null;
      }
      else
      {
        mul = 1.0 / p1.Z;
        t1 = -p0.Z * mul;
        t2 = t1 + mul;

        if (mul > 0.0)
        {
          if (t1 > tMin)
          {
            tMin = t1;
            fMin = CubeFaces.NegativeZ;
          }

          if (t2 < tMax)
          {
            tMax = t2;
            fMax = CubeFaces.PositiveZ;
          }
        }
        else
        {
          if (t2 > tMin)
          {
            tMin = t2;
            fMin = CubeFaces.PositiveZ;
          }

          if (t1 < tMax)
          {
            tMax = t1;
            fMax = CubeFaces.NegativeZ;
          }
        }

        if (tMin >= tMax)
          return null;
      }

      // Finally assemble the two intersections:
      LinkedList<Intersection> result = new LinkedList<Intersection> ();

      Intersection i;
      i = new Intersection(this)
      {
        T           = tMin,
        Enter       = true,
        Front       = true,
        NormalLocal = Normals[(int)fMin],
        CoordLocal  = p0 + tMin * p1,
        SolidData   = fMin
      };
      result.AddLast(i);

      i = new Intersection(this)
      {
        T           = tMax,
        Enter       = false,
        Front       = false,
        NormalLocal = Normals[(int)fMax],
        CoordLocal  = p0 + tMax * p1,
        SolidData   = fMax
      };
      result.AddLast(i);

      return result;
    }

    /// <summary>
    /// Complete all relevant items in the given Intersection object.
    /// </summary>
    /// <param name="inter">Intersection instance to complete.</param>
    public override void CompleteIntersection (Intersection inter)
    {
      // Normal vector - no need to do anything here as NormalLocal is defined...

      // 2D texture coordinates.
      switch ((CubeFaces)inter.SolidData)
      {
        case CubeFaces.NegativeX:
        case CubeFaces.PositiveX:
          inter.TextureCoord.X = inter.CoordLocal.Y;
          inter.TextureCoord.Y = inter.CoordLocal.Z;
          break;
        case CubeFaces.NegativeY:
        case CubeFaces.PositiveY:
          inter.TextureCoord.X = inter.CoordLocal.X;
          inter.TextureCoord.Y = inter.CoordLocal.Z;
          break;
        default:
          inter.TextureCoord.X = inter.CoordLocal.X;
          inter.TextureCoord.Y = inter.CoordLocal.Y;
          break;
      }
    }

    public void GetBoundingBox (out Vector3d corner1, out Vector3d corner2)
    {
      corner1 = new Vector3d(0, 0, 0);
      corner2 = new Vector3d(1, 1, 1);
    }
  }

  /// <summary>
  /// Plane objects (infinite plane, rectangle, triangle) as elementary 3D solids.
  /// </summary>
  [Serializable]
  public class Plane : DefaultSceneNode, ISolid
  {
    /// <summary>
    /// Lower bound for x coordinate.
    /// </summary>
    public double XMin { get; set; }

    /// <summary>
    /// Upper bound for x coordinate (or triangle).
    /// </summary>
    public double XMax { get; set; }

    /// <summary>
    /// Lower bound for y coordinate.
    /// </summary>
    public double YMin { get; set; }

    /// <summary>
    /// Upper bound for y coordinate (or triangle).
    /// </summary>
    public double YMax { get; set; }

    /// <summary>
    /// Use three restrictions instead of four (the 3rd one is: xMax * x + yMax * y &le; 1.0).
    /// </summary>
    public bool Triangle { get; set; }

    /// <summary>
    /// Use canonic u coordinate (=x)?
    /// </summary>
    public bool CanonicU { get; set; }

    /// <summary>
    /// Use canonic v coordinate (=y)?
    /// </summary>
    public bool CanonicV { get; set; }

    /// <summary>
    /// Scale coefficient for u coordinate.
    /// </summary>
    public double ScaleU { get; set; }

    /// <summary>
    /// Scale coefficient for v coordinate.
    /// </summary>
    public double ScaleV { get; set; }

    /// <summary>
    /// Default constructor - infinite plane.
    /// </summary>
    public Plane ()
      : this(double.NegativeInfinity, Double.PositiveInfinity,
             double.NegativeInfinity, Double.PositiveInfinity)
    {}

    /// <summary>
    /// Rectangle (four restrictions).
    /// </summary>
    public Plane (double xMi, double xMa, double yMi, double yMa)
    {
      XMin     = xMi;
      XMax     = xMa;
      YMin     = yMi;
      YMax     = yMa;
      Triangle = false;
      CanonicU = CanonicV = true;
      ScaleU   = ScaleV = 1.0;

      // texture coordinates:
      if (!double.IsInfinity(XMin) &&
          !double.IsInfinity(XMax))
      {
        CanonicU = false;
        ScaleU   = 1.0 / (XMax - XMin);
      }

      if (!double.IsInfinity(YMin) &&
          !double.IsInfinity(YMax))
      {
        CanonicV = false;
        ScaleV   = 1.0 / (YMax - YMin);
      }
    }

    /// <summary>
    /// Triangle (three restrictions).
    /// xMax * x + yMax * y &le; 1.0
    /// </summary>
    public Plane (double xMa, double yMa)
    {
      YMin     = XMin = 0.0;
      XMax     = (xMa < double.Epsilon) ? 1.0 : 1.0 / xMa;
      YMax     = (yMa < double.Epsilon) ? 1.0 : 1.0 / yMa;
      Triangle =
      CanonicU =
      CanonicV = true;
      ScaleU   =
      ScaleV   = 1.0;
    }

    /// <summary>
    /// Computes the complete intersection of the given ray with the object.
    /// </summary>
    /// <param name="p0">Ray origin.</param>
    /// <param name="p1">Ray direction vector.</param>
    /// <returns>Sorted list of intersection records.</returns>
    public override LinkedList<Intersection> Intersect (Vector3d p0, Vector3d p1)
    {
      if (Geometry.IsZero(p1.Z))
        return null;

      double t = -p0.Z / p1.Z;
      double x = p0.X + t * p1.X;
      if (x < XMin)
        return null;
      double y = p0.Y + t * p1.Y;
      if (y < YMin)
        return null;
      double u, v;

      if (Triangle)
      {
        u = x * XMax;
        v = y * YMax;
        if (u + v > 1.0)
          return null;
      }
      else if (x > XMax ||
               y > YMax)
        return null;
      else
      {
        u = CanonicU ? x : (x - XMin) * ScaleU;
        v = CanonicV ? y : (y - YMin) * ScaleV;
      }

      // there will be one intersection..
      LinkedList<Intersection> result = new LinkedList<Intersection> ();
      Intersection i = new Intersection(this)
      {
        T            = t,
        Enter        = p1.Z < 0.0,
        Front        = p1.Z < 0.0,
        TangentU     = Vector3d.UnitX,
        TangentV     = Vector3d.UnitY,
        CoordLocal   = { X = x, Y = y, Z = 0.0 },
        TextureCoord = { X = u, Y = v },
      };

      result.AddLast(i);

      return result;
    }

    public void GetBoundingBox (out Vector3d corner1, out Vector3d corner2)
    {
      if (Triangle)
      {
        throw new NotImplementedException();
      }

      // If XMin is positive/negative infinity, it is replaced to +/- infinityPlaceholder value
      double localXMin = double.IsInfinity(XMin) ? (double.IsPositiveInfinity(XMin) ? infinityPlaceholder : -infinityPlaceholder) : XMin;
      double localYMin = double.IsInfinity(YMin) ? (double.IsPositiveInfinity(YMin) ? infinityPlaceholder : -infinityPlaceholder) : YMin;
      double localXMax = double.IsInfinity(XMax) ? (double.IsPositiveInfinity(XMax) ? infinityPlaceholder : -infinityPlaceholder) : XMax;
      double localYMax = double.IsInfinity(YMax) ? (double.IsPositiveInfinity(YMax) ? infinityPlaceholder : -infinityPlaceholder) : YMax;

      corner1 = new Vector3d(localXMin, localYMin, 0);
      corner2 = new Vector3d(localXMax, localYMax, 0);
    }
  }

  /// <summary>
  /// Unit cylinder (optionally restrictead by one or two bases) able to compute ray-intersection, normal vector
  /// and 2D texture coordinates.
  /// </summary>
  [Serializable]
  public class Cylinder : DefaultSceneNode, ISolid
  {
    /// <summary>
    /// Lower bound for z coordinate (optional)
    /// </summary>
    public double ZMin { get; set; }

    /// <summary>
    /// Upper bound for z coordinate (optional).
    /// </summary>
    public double ZMax { get; set; }

    /// <summary>
    /// Default constructor - infinite cylindric surface.
    /// </summary>
    public Cylinder ()
      : this(double.NegativeInfinity, double.PositiveInfinity)
    {}

    /// <summary>
    /// Restricted cylinder.
    /// </summary>
    public Cylinder (double zMi, double zMa)
    {
      ZMin = zMi;
      ZMax = zMa;
    }

    /// <summary>
    /// Computes the complete intersection of the given ray with the object.
    /// </summary>
    /// <param name="p0">Ray origin.</param>
    /// <param name="p1">Ray direction vector.</param>
    /// <returns>Sorted list of intersection records.</returns>
    public override LinkedList<Intersection> Intersect (Vector3d p0, Vector3d p1)
    {
      double A  = p1.X * p1.X + p1.Y * p1.Y;
      double DA = A + A;
      double B  = p0.X * p1.X + p0.Y * p1.Y;
      B += B;
      double C = p0.X * p0.X + p0.Y * p0.Y - 1.0;

      double d = B * B - (DA + DA) * C; // discriminant
      if (d <= 0.0)
        return null;        // no intersection

      d = Math.Sqrt(d);
      // There will be two intersections: (-B +- d) / DA
      double   t1   = (-B - d) / DA;
      double   t2   = (-B + d) / DA; // t1 < t2
      Vector3d loc1 = p0 + t1 * p1;
      Vector3d loc2 = p0 + t2 * p1;

      double lzmin = Math.Min(loc1.Z, loc2.Z);
      if (lzmin >= ZMax)
        return null;
      double lzmax = Math.Max(loc1.Z, loc2.Z);
      if (lzmax <= ZMin)
        return null;

      // There indeed will be two intersections.
      LinkedList<Intersection> result = new LinkedList<Intersection>();
      Intersection i;

      // Test the two bases.
      double tb1 = 0.0, tb2 = 0.0;
      bool base1 = ZMin > lzmin && ZMin < lzmax;
      if (base1) tb1 = (ZMin - p0.Z) / p1.Z;
      bool base2 = ZMax > lzmin && ZMax < lzmax;
      if (base2) tb2 = (ZMax - p0.Z) / p1.Z;

      // Enter the solid.
      i = new Intersection(this)
      {
        Enter = true,
        Front = true
      };
      if (base1 && p1.Z > 0.0)
      {
        // Enter through the 1st base.
        i.T          = tb1;
        i.CoordLocal = p0 + tb1 * p1;
        i.SolidData  = -1;
      }
      else if (base2 && p1.Z < 0.0)
      {
        // Enter through the 2nd base.
        i.T          = tb2;
        i.CoordLocal = p0 + tb2 * p1;
        i.SolidData  = 1;
      }
      else
      {
        // Cylinder surface.
        i.T          = t1;
        i.CoordLocal = loc1;
        i.SolidData  = 0;
      }

      result.AddLast(i);

      // Leave the solid.
      i = new Intersection(this)
      {
        Enter = false,
        Front = false
      };
      if (base1 && p1.Z < 0.0)
      {
        // Enter through the 1st base.
        i.T          = tb1;
        i.CoordLocal = p0 + tb1 * p1;
        i.SolidData  = -1;
      }
      else if (base2 && p1.Z > 0.0)
      {
        // Enter through the 2nd base.
        i.T          = tb2;
        i.CoordLocal = p0 + tb2 * p1;
        i.SolidData  = 1;
      }
      else
      {
        // Cylinder surface.
        i.T          = t2;
        i.CoordLocal = loc2;
        i.SolidData  = 0;
      }

      result.AddLast(i);

      return result;
    }

    /// <summary>
    /// Complete all relevant items in the given Intersection object.
    /// </summary>
    /// <param name="inter">Intersection instance to complete.</param>
    public override void CompleteIntersection (Intersection inter)
    {
      // Normal vector.
      inter.NormalLocal.Z = (int)inter.SolidData;
      if ((int)inter.SolidData == 0)
      {
        inter.NormalLocal.X = inter.CoordLocal.X;
        inter.NormalLocal.Y = inter.CoordLocal.Y;
        inter.TextureCoord.X = Math.Atan2(inter.CoordLocal.Y, inter.CoordLocal.X) / (2.0 * Math.PI) + 0.5;
      }
      else
      {
        inter.NormalLocal.X = inter.NormalLocal.Y = 0.0;
        inter.TextureCoord.X = 0.0;
      }

      // 2D texture coordinates.
      inter.TextureCoord.Y = inter.CoordLocal.Z;
    }

    public void GetBoundingBox (out Vector3d corner1, out Vector3d corner2)
    {
      // If XMin is positive/negative infinity, it is replaced to +/- infinityPlaceholder value
      double localZMin = double.IsInfinity(ZMin) ? (double.IsPositiveInfinity(ZMin) ? infinityPlaceholder : -infinityPlaceholder) : ZMin;
      double localZMax = double.IsInfinity(ZMax) ? (double.IsPositiveInfinity(ZMax) ? infinityPlaceholder : -infinityPlaceholder) : ZMax;

      corner1 = new Vector3d(-1, -1, localZMin);
      corner2 = new Vector3d( 1,  1, localZMax);
    }
  }

  /// <summary>
  /// Unit cylinder showing only the front face,
  /// texture coordinates are representing projected distance from the axis.
  /// </summary>
  [Serializable]
  public class CylinderFront : Cylinder
  {
    /// <summary>
    /// Compute two intersections? (not needed for glow effects).
    /// </summary>
    public bool shell;

    /// <summary>
    /// Default constructor - infinite cylindric surface.
    /// </summary>
    public CylinderFront (in bool sh = true)
      : base(double.NegativeInfinity, double.PositiveInfinity)
    {
      shell = sh;
    }

    /// <summary>
    /// Restricted cylinder.
    /// </summary>
    public CylinderFront (in double zMi, in double zMa, in bool sh = true)
      : base (zMi, zMa)
    {
      shell = sh;
    }

    /// <summary>
    /// Computes the complete intersection of the given ray with the object.
    /// </summary>
    /// <param name="p0">Ray origin.</param>
    /// <param name="p1">Ray direction vector.</param>
    /// <returns>Sorted list of intersection records.</returns>
    public override LinkedList<Intersection> Intersect (Vector3d p0, Vector3d p1)
    {
      double A  = p1.X * p1.X + p1.Y * p1.Y;
      double DA = A + A;
      double B  = p0.X * p1.X + p0.Y * p1.Y;
      B += B;
      double C = p0.X * p0.X + p0.Y * p0.Y - 1.0;

      double d = B * B - (DA + DA) * C; // discriminant
      if (d <= 0.0)
        return null;        // no intersection

      d = Math.Sqrt(d);
      // Two intersections with the infinite surface: (-B +- d) / DA
      double   t1   = (-B - d) / DA;
      double   t2   = (-B + d) / DA; // t1 < t2
      Vector3d loc1 = p0 + t1 * p1;
      Vector3d loc2 = p0 + t2 * p1;

      double lzmin = Math.Min(loc1.Z, loc2.Z);
      if (lzmin >= ZMax)
        return null;
      double lzmax = Math.Max(loc1.Z, loc2.Z);
      if (lzmax <= ZMin)
        return null;

      // Test the two bases.
      bool base1 = ZMin > lzmin && ZMin < lzmax;
      bool base2 = ZMax > lzmin && ZMax < lzmax;
      if ((base1 && p1.Z > 0.0) ||
          (base2 && p1.Z < 0.0))
        return null;

      // There will be an intersection with the surface.
      LinkedList<Intersection> result = new LinkedList<Intersection>();
      Intersection i = new Intersection(this)
      {
        Enter      = true,
        Front      = true,
        T          = t1,
        CoordLocal = loc1,
        SolidData  = p1
      };
      result.AddLast(i);

      if (shell)
      {
        i = new Intersection(this)
        {
          Enter      = false,
          Front      = false,
          T          = t1 + Intersection.SHELL_THICKNESS,
          CoordLocal = loc1 + Intersection.SHELL_THICKNESS * p1,
          SolidData  = p1
        };
        result.AddLast(i);
      }

      return result;
    }

    /// <summary>
    /// Complete all relevant items in the given Intersection object.
    /// </summary>
    /// <param name="inter">Intersection instance to complete.</param>
    public override void CompleteIntersection (Intersection inter)
    {
      // Normal vector.
      inter.NormalLocal.Z = 0.0;
      inter.NormalLocal.X = inter.CoordLocal.X;
      inter.NormalLocal.Y = inter.CoordLocal.Y;

      // Texture coordinates encoding distance from the axis.
      if (inter.SolidData is Vector3d direction)
      {
        Vector2d dirxy = new Vector2d(direction.X, direction.Y);
        dirxy.Normalize();
        double cosa = inter.CoordLocal.X * dirxy.X + inter.CoordLocal.Y * dirxy.Y;
        double sina = Math.Sqrt(1.0 - cosa * cosa);
        // sina = distance_from_center
        inter.TextureCoord.X =
        inter.TextureCoord.Y = sina;
      }
      else
        inter.TextureCoord = Vector2d.Zero;
    }
  }

  /// <summary>
  /// Torus (original author: Jan Navratil, (c) 2012).
  /// </summary>
  [Serializable]
  public class Torus : DefaultSceneNode, ISolid
  {
    public double bigRadius = 1.0;

    public double smallRadius = 0.5;

    protected BoundingSphere boundingSphere;

    public Torus (double big, double small)
    {
      bigRadius      = big;
      smallRadius    = small;
      boundingSphere = new BoundingSphere(bigRadius + smallRadius);
    }

    /// <summary>
    /// Computes the complete intersection of the given ray with the object.
    /// </summary>
    /// <param name="p0">Ray origin.</param>
    /// <param name="p1">Ray direction vector.</param>
    /// <returns>Sorted list of intersection records.</returns>
    public override LinkedList<Intersection> Intersect (Vector3d p0, Vector3d p1)
    // THIS METHOD IS NOT 100% CORRECT (some intersections are incorrect (completely random locations) and some have incorrect T parameter)
    {
      CSGInnerNode.countBoundingBoxes++;
      if (boundingSphere.Intersect(p0, p1) < -0.5)
        return null;

      // Vlastni implementace podle http://www.emeyex.com/site/projects/raytorus.pdf
      double OD, DD, OO;
      Vector3d.Dot(ref p0, ref p1, out OD);
      Vector3d.Dot(ref p1, ref p1, out DD);
      Vector3d.Dot(ref p0, ref p0, out OO);

      double bRbR       = bigRadius   * bigRadius;
      double sRsR       = smallRadius * smallRadius;
      double OObRbRsRsR = OO - bRbR - sRsR;

      double a = DD * DD;
      double b = 4 * DD * OD;
      double c = 4 * OD * OD + 2 * DD * OObRbRsRsR + 4 * bRbR * p1.Z * p1.Z;
      double d = 4 * OD * OObRbRsRsR + 8 * bRbR * p1.Z * p0.Z;
      double e = OObRbRsRsR * OObRbRsRsR + 4 * bRbR * (p0.Z * p0.Z - sRsR);

      Polynomial poly   = new Polynomial(a, b, c, d, e);
      double[]   roots  = new double[4];
      int        nRoots = poly.SolveQuartic(roots);
      if (nRoots == 0)
        return null;

      int j = 0;
      for (int i = 0; i < nRoots; i++)
        if (!Geometry.IsZero(roots[i]))
          roots[j++] = 1.0 / roots[i];
      nRoots = j;

      if (nRoots >= 2)
        Array.Sort(roots, 0, nRoots);

      //Debug.Assert((nRoots % 2 == 0), "Roots(" + nRoots + "): " + roots[ 0 ] + " " + roots[ 1 ] + " " + roots[ 2 ] + " " + roots[ 3 ]);

      LinkedList<Intersection> result = new LinkedList<Intersection>();
      for (j = 0; j < nRoots; j++)
      {
        double t = roots[j];
        Intersection ix = new Intersection(this)
        {
          T          = t - 10.0 * Intersection.RAY_EPSILON,
          Enter      = j % 2 == 0,
          Front      = j % 2 == 0,
          CoordLocal = p0 + t * p1
        };

        result.AddLast(ix);
      }

      return result;
    }

    /// <summary>
    /// Complete all relevant items in the given Intersection object.
    /// </summary>
    /// <param name="inter">Intersection instance to complete.</param>
    public override void CompleteIntersection (Intersection inter)
    {
      // Normal vector.

      // Normal vector of an intersection with a torus in the z plain is the same as the intersection coordinates
      // minus the coordinates of the center of the circle it lays on.
      Vector3d circleCenter = getSmallCircleCenter(inter);
      if (Geometry.IsZero(inter.CoordLocal.X) &&
          Geometry.IsZero(inter.CoordLocal.Y))
        inter.Normal = new Vector3d(0.0, 0.0, Math.Sign(inter.CoordLocal.Z));
      else
        inter.NormalLocal = inter.CoordLocal - circleCenter;

      // 2D texture coordinates.
      double r = Math.Sqrt(inter.CoordLocal.X * inter.CoordLocal.X + inter.CoordLocal.Y * inter.CoordLocal.Y);
      inter.TextureCoord.X = Geometry.IsZero(r)
        ? 0.0
        : (Math.Atan2(inter.CoordLocal.Y, inter.CoordLocal.X) / (2.0 * Math.PI) + 0.5);

      Vector3d circleCenterToIntersection = inter.CoordLocal - circleCenter;
      double r2 = Math.Sqrt(circleCenterToIntersection.X * circleCenterToIntersection.X +
                            circleCenterToIntersection.Y * circleCenterToIntersection.Y);
      if (r < bigRadius)
        r2 = -r2;
      inter.TextureCoord.Y = Math.Atan2(r2, inter.CoordLocal.Z) / (2.0 * Math.PI) + 0.5;
    }

    public void GetBoundingBox (out Vector3d corner1, out Vector3d corner2)
    {
      corner1 = new Vector3d(-bigRadius, -bigRadius, -1);
      corner2 = new Vector3d( bigRadius,  bigRadius,  1);
    }

    private Vector3d getSmallCircleCenter (Intersection inter)
    {
      Vector3d circleCenter = new Vector3d();
      double torusCenter_intersectionDistanceInPlane = Math.Sqrt(inter.CoordLocal.X * inter.CoordLocal.X + inter.CoordLocal.Y * inter.CoordLocal.Y);
      double centerCalculationRatio = bigRadius / torusCenter_intersectionDistanceInPlane;
      circleCenter.X = inter.CoordLocal.X * centerCalculationRatio;
      circleCenter.Y = inter.CoordLocal.Y * centerCalculationRatio;
      circleCenter.Z = 0.0;

      double assHoleZSquared = smallRadius * smallRadius - bigRadius * bigRadius;
      if (bigRadius < smallRadius &&                                  // mame prdelni torus
          centerCalculationRatio > 1.0 &&                             // prusecik je bliz centru nez obvodu
          assHoleZSquared >= inter.CoordLocal.Z * inter.CoordLocal.Z) // Z pruseciku je niz nez asshole
      {
        circleCenter.X = -circleCenter.X;
        circleCenter.Y = -circleCenter.Y;
        inter.Front = false;
      }

      return circleCenter;
    }
  }

  /// <summary>
  /// Support class for R-tree traversal.
  /// </summary>
  class BezierIntersection : IComparable<BezierIntersection>
  {
    /// <summary>
    /// Associated Bezier patch.
    /// </summary>
    public BezierPatch patch;

    /// <summary>
    /// Intersection of the ray and the bounding box / patch.
    /// </summary>
    public double t;

    /// <summary>
    /// This is a final intersection, not a patch any more: 1 .. upper left triangle, 2 .. lower right.
    /// </summary>
    public int final;

    /// <summary>
    /// The ray came from the front side of a triangle.
    /// </summary>
    public bool enter;

    /// <summary>
    /// Barycentric coordinates of an intersection with a triangle.
    /// </summary>
    public Vector2d uv;

    /// <summary>
    /// Tangent vectors.
    /// </summary>
    public Vector3d tu, tv;

    public BezierIntersection (BezierPatch p, double _t)
    {
      patch = p;
      t     = _t;
    }

    public int CompareTo (BezierIntersection bi)
    {
      if (t < bi.t) return -1;
      if (t > bi.t) return 1;
      return 0;
    }
  }

  /// <summary>
  /// Support data container, holds info about one (subdivided) Bezier patch and its AABB.
  /// Static shareable data, multi-thread safe.
  /// </summary>
  [Serializable]
  class BezierPatch : ICloneable
  {
    /// <summary>
    /// Global subdivision threshold.
    /// </summary>
    public static double Epsilon = 1.0e-3;

    /// <summary>
    /// R-tree of the patches.
    /// </summary>
    public BezierPatch left, right;

    /// <summary>
    /// Control points .. p[0] = P_00, p[3] = P_03, p[4] = P_10, .. p[15] = P_33.
    /// </summary>
    public Vector3d[] p;

    /// <summary>
    /// AABB: upper left corner (minima).
    /// </summary>
    public Vector3d bbMin;

    /// <summary>
    /// AABB: size of the box (maxima-minima).
    /// </summary>
    public Vector3d bbSize;

    /// <summary>
    /// Texture coordinates at the patch corner P_00.
    /// </summary>
    public Vector2d tex00;

    /// <summary>
    /// Texture coordinates at the patch corner P_33.
    /// </summary>
    public Vector2d tex33;

    public BezierPatch ()
    {
      p = new Vector3d[16];
    }

    /// <summary>
    /// Creates its own copy of control point array.
    /// </summary>
    public BezierPatch (BezierPatch b)
    {
      p      = (Vector3d[])b.p.Clone();
      bbMin  = b.bbMin;
      bbSize = b.bbSize;
      tex00  = b.tex00;
      tex33  = b.tex33;
    }

    public object Clone ()
    {
      return new BezierPatch(this);
    }

    /// <summary>
    /// [Re-]builds a bounding box for this patch.
    /// </summary>
    public void UpdateBB ()
    {
      bbMin = bbSize = p[0];
      for (int i = 1; i < 16; i++)
      {
        double pa = p[i].X;
        if (pa < bbMin.X)  bbMin.X = pa;
        if (pa > bbSize.X) bbSize.X = pa;
        pa = p[i].Y;
        if (pa < bbMin.Y)  bbMin.Y = pa;
        if (pa > bbSize.Y) bbSize.Y = pa;
        pa = p[i].Z;
        if (pa < bbMin.Z)  bbMin.Z = pa;
        if (pa > bbSize.Z) bbSize.Z = pa;
      }

      bbSize -= bbMin;
    }

    protected void DivideHorizontal (bool left)
    {
      for (int off = 0; off < 16; off += 4) // one horizontal row
      {
        Vector3d p01       = 0.5 * (p[off]     + p[off + 1]);
        Vector3d p12       = 0.5 * (p[off + 1] + p[off + 2]);
        Vector3d p23       = 0.5 * (p[off + 2] + p[off + 3]);
        Vector3d p0112     = 0.5 * (p01 + p12);
        Vector3d p1223     = 0.5 * (p12 + p23);
        Vector3d p01112223 = 0.5 * (p0112 + p1223);
        if (left)
        {
          p[off + 1] = p01;
          p[off + 2] = p0112;
          p[off + 3] = p01112223;
        }
        else
        {
          p[off] = p01112223;
          p[off + 1] = p1223;
          p[off + 2] = p23;
        }
      }

      double texHalf = 0.5 * (tex00.X + tex33.X);
      if (left)
        tex33.X = texHalf;
      else
        tex00.X = texHalf;
      UpdateBB();
    }

    protected void DivideVertical (bool up)
    {
      for (int off = 0; off < 4; off++) // one vertical column
      {
        Vector3d p01       = 0.5 * (p[off]     + p[off + 4]);
        Vector3d p12       = 0.5 * (p[off + 4] + p[off + 8]);
        Vector3d p23       = 0.5 * (p[off + 8] + p[off + 12]);
        Vector3d p0112     = 0.5 * (p01 + p12);
        Vector3d p1223     = 0.5 * (p12 + p23);
        Vector3d p01112223 = 0.5 * (p0112 + p1223);
        if (up)
        {
          p[off + 4] = p01;
          p[off + 8] = p0112;
          p[off + 12] = p01112223;
        }
        else
        {
          p[off] = p01112223;
          p[off + 4] = p1223;
          p[off + 8] = p23;
        }
      }

      double texHalf = 0.5 * (tex00.Y + tex33.Y);
      if (up)
        tex33.Y = texHalf;
      else
        tex00.Y = texHalf;
      UpdateBB();
    }

    /// <summary>
    /// Re-entrant intersection function. Doesn't modify the patch instance at all.
    /// </summary>
    /// <returns>Double.NegativeInfinity if negative.</returns>
    public double IntersectInv (ref Vector3d p0, ref Vector3d p1)
    {
      CSGInnerNode.countBoundingBoxes++;
      Vector2d result;
      return Geometry.RayBoxIntersectionInv(ref p0, ref p1, ref bbMin, ref bbSize, out result)
        ? result.X
        : double.NegativeInfinity;
    }

    /// <summary>
    /// Builds a R-tree for this patch.
    /// Uses this patch as R-tree root.
    /// </summary>
    public BezierPatch BuildTree ()
    {
      if (bbSize.LengthSquared < Epsilon)
      {
        left = right = null;
        return this;
      }

      // subdivide the patch:
      BezierPatch b1 = (BezierPatch)Clone(); // left child
      BezierPatch b2 = (BezierPatch)Clone(); // right child

      double hor = (p[3]  - p[0]).LengthFast + (p[15] - p[12]).LengthFast;
      double ver = (p[12] - p[0]).LengthFast + (p[15] - p[3]).LengthFast;
      if (hor > ver)
      {
        // first child:
        b1.DivideHorizontal(true);
        // second child:
        b2.DivideHorizontal(false);
      }
      else
      {
        // first child:
        b1.DivideVertical(true);
        // second child:
        b2.DivideVertical(false);
      }

      left = b1.BuildTree();
      right = b2.BuildTree();

      return this;
    }
  }

  /// <summary>
  /// Bezier surface able to compute ray-intersection, normal vector
  /// and 2D texture coordinates.
  [Serializable]
  public class BezierSurface : DefaultSceneNode, ISolid
  {
    /// <summary>
    /// Root Bezier patches R-trees.
    /// Shared, must not be modified during computation!
    /// </summary>
    private readonly List<BezierPatch> patches;

    /// <summary>
    /// Compute normal vectors using Gouraud interpolation?
    /// </summary>
    public bool PreciseNormals { get; set; }

    /// <summary>
    /// Subdivision threshold.
    /// </summary>
    public static double Epsilon
    {
      get => BezierPatch.Epsilon;
      set => BezierPatch.Epsilon = value;
    }

    private Vector3d minV, maxV;

    public BezierSurface (int K, int L, double[] v)
    {
      Debug.Assert(K > 0 && L > 0);
      int len = 3 * (3 * K + 1) * (3 * L + 1);
      Debug.Assert(v != null && v.Length >= len);

      PreciseNormals = true;

      // Bounding box.
      maxV = new Vector3d(v[0], v[1], v[2]);
      minV = maxV;
      for (int k = 3; k + 2 < len;)
      {
        if (v[k] > maxV.X) maxV.X = v[k];
        if (v[k] < minV.X) minV.X = v[k];
        k++;
        if (v[k] > maxV.Y) maxV.Y = v[k];
        if (v[k] < minV.Y) minV.Y = v[k];
        k++;
        if (v[k] > maxV.Z) maxV.Z = v[k];
        if (v[k] < minV.Z) minV.Z = v[k];
        k++;
      }

      // Create set of patches.
      patches = new List<BezierPatch>(K * L);
      int stride = 3 * (3 * L + 1);
      int ik     = 0;
      for (int k = 0; k < K; k++, ik += 3 * stride)
      {
        int il = ik;
        for (int l = 0; l < L; l++, il += 9)
        {
          BezierPatch p  = new BezierPatch ();
          int         oi = 0;
          for (int i = 0; i < 4 * stride; i += stride)
            for (int j = il + i; j < il + i + 12; oi++)
            {
              p.p[oi].X = v[j++];
              p.p[oi].Y = v[j++];
              p.p[oi].Z = v[j++];
            }

          p.tex00.X = l;
          p.tex00.Y = k;
          p.tex33.X = l + 1.0;
          p.tex33.Y = k + 1.0;
          p.UpdateBB();
          patches.Add(p.BuildTree());
        }
      }
    }

    /// <summary>
    /// Computes the complete intersection of the given ray with the object.
    /// </summary>
    /// <param name="p0">Ray origin.</param>
    /// <param name="p1">Ray direction vector.</param>
    /// <returns>Sorted list of intersection records.</returns>
    public override LinkedList<Intersection> Intersect (Vector3d p0, Vector3d p1)
    {
      HeapMin<BezierIntersection> h = new HeapMin<BezierIntersection>();
      Vector3d                    p1inv;
      p1inv.X = Geometry.IsZero(p1.X) ? double.PositiveInfinity : 1.0 / p1.X;
      p1inv.Y = Geometry.IsZero(p1.Y) ? double.PositiveInfinity : 1.0 / p1.Y;
      p1inv.Z = Geometry.IsZero(p1.Z) ? double.PositiveInfinity : 1.0 / p1.Z;

      foreach (BezierPatch b in patches)
      {
        double t = b.IntersectInv(ref p0, ref p1inv);
        if (!double.IsInfinity(t))
        {
          BezierIntersection bi = new BezierIntersection(b, t);
          h.Add(bi);
        }
      }

      LinkedList<Intersection> result = null;
      Intersection             i;

      while (h.Count > 0)
      {
        BezierIntersection bi = h.RemoveMin();
        if (bi.final > 0)
        {
          // Intersection.
          if (result == null)
            result = new LinkedList<Intersection>();
          i = new Intersection(this)
          {
            T          = bi.t,
            Enter      = bi.enter,
            Front      = bi.enter,
            CoordLocal = p0 + bi.t * p1,
            SolidData  = bi
          };
          result.AddLast(i);
        }
        else
        {
          // Patch (to subdivide?).
          if (bi.patch.left != null) // subdivision
          {
            BezierPatch right = bi.patch.right;

            // The left child.
            double t = bi.patch.left.IntersectInv(ref p0, ref p1inv);
            if (!double.IsInfinity(t))
            {
              bi.patch = bi.patch.left;
              bi.t = t;
              h.Add(bi);
              bi = null;
            }

            // The right child.
            t = right.IntersectInv(ref p0, ref p1inv);
            if (!double.IsInfinity(t))
            {
              if (bi == null)
                bi = new BezierIntersection(right, t);
              else
              {
                bi.patch = right;
                bi.t = t;
              }

              h.Add(bi);
            }
          }
          else
          {
            // The patch is too small => intersect the two triangles...
            bi.final = 0;
            Vector3d[] cp = bi.patch.p;
            CSGInnerNode.countTriangles++;
            bi.t = Geometry.RayTriangleIntersection(ref p0, ref p1, ref cp[12], ref cp[3], ref cp[0], out bi.uv);
            if (!double.IsInfinity(bi.t))
            {
              bi.final = 1;
#if TRIANGLE_NORMALS_BEZIER
              bi.tu = (cp[ 0] - cp[ 4]).Normalized() +
                      (cp[ 8] - cp[12]).Normalized() +
                      (cp[ 3] - cp[ 7]).Normalized();
              bi.tv = (cp[ 1] - cp[ 0]).Normalized() +
                      (cp[ 3] - cp[ 2]).Normalized() +
                      (cp[13] - cp[12]).Normalized();
#else
              bi.tu = cp[ 0] - cp[12];
              bi.tv = cp[ 3] - cp[ 0];
#endif
            }
            else
            {
              CSGInnerNode.countTriangles++;
              bi.t = Geometry.RayTriangleIntersection(ref p0, ref p1, ref cp[12], ref cp[15], ref cp[3], out bi.uv);
              if (!double.IsInfinity(bi.t))
              {
                bi.final = 2;
#if TRIANGLE_NORMALS_BEZIER
                bi.tu = (cp[ 8] - cp[12]).Normalized() +
                        (cp[ 3] - cp[ 7]).Normalized() +
                        (cp[11] - cp[15]).Normalized();
                bi.tv = (cp[ 3] - cp[ 2]).Normalized() +
                        (cp[13] - cp[12]).Normalized() +
                        (cp[15] - cp[14]).Normalized();
#else
                bi.tu = cp[ 3] - cp[15];
                bi.tv = cp[15] - cp[12];
#endif
              }
            }

            if (bi.final > 0)
            {
              Vector3d n;
              Vector3d.Cross(ref bi.tu, ref bi.tv, out n);
              double dot;
              Vector3d.Dot(ref n, ref p1, out dot);
              bi.enter = dot < 0.0;
              h.Add(bi);
            }
          }
        }
      }

      return result;
    }

    /// <summary>
    /// Complete all relevant items in the given Intersection object.
    /// </summary>
    /// <param name="inter">Intersection instance to complete.</param>
    public override void CompleteIntersection (Intersection inter)
    {
      if (inter.SolidData is BezierIntersection bi)
      {
        // Normal vector.
        if (PreciseNormals)
        {
          // Gouraud interpolation of exact normal vectors at the corners...
          Vector3d   nua, nub, nuc;
          Vector3d   nva, nvb, nvc;
          Vector3d[] cp = bi.patch.p;
          if (bi.final == 1)
          {
            // Upper left triangle.
            // Vertex A.
            nua = (cp[ 8] - cp[12]).Normalized();
            nva = (cp[13] - cp[12]).Normalized();
            // Vertex B.
            nub = (cp[ 3] - cp[ 7]).Normalized();
            nvb = (cp[ 3] - cp[ 2]).Normalized();
            // Vertex C.
            nuc = (cp[ 0] - cp[ 4]).Normalized();
            nvc = (cp[ 1] - cp[ 0]).Normalized();
          }
          else
          {
            // Lower right triangle.
            // Vertex A.
            nua = (cp[ 8] - cp[12]).Normalized();
            nva = (cp[13] - cp[12]).Normalized();
            // Vertex B.
            nub = (cp[11] - cp[15]).Normalized();
            nvb = (cp[15] - cp[14]).Normalized();
            // Vertex C.
            nuc = (cp[ 3] - cp[ 7]).Normalized();
            nvc = (cp[ 3] - cp[ 2]).Normalized();
          }

          inter.TangentU = (1.0 - bi.uv.X - bi.uv.Y) * nua + bi.uv.X * nub + bi.uv.Y * nuc;
          inter.TangentV = (1.0 - bi.uv.X - bi.uv.Y) * nva + bi.uv.X * nvb + bi.uv.Y * nvc;
        }
        else
        {
          // Flat normal.
          inter.TangentU = bi.tu.Normalized();
          inter.TangentV = bi.tv.Normalized();
        }

        // 2D texture coordinates.
        double x00 = bi.patch.tex00.X;
        double y00 = bi.patch.tex00.Y;
        double x33 = bi.patch.tex33.X;
        double y33 = bi.patch.tex33.Y;
        if (bi.final == 1)
        {
          // Upper left triangle.
          inter.TextureCoord.X = x00 + bi.uv.X * (x33 - x00);
          inter.TextureCoord.Y = y33 + (bi.uv.X + bi.uv.Y) * (y00 - y33);
        }
        else
        {
          // Lower right triangle.
          inter.TextureCoord.X = x00 + (bi.uv.X + bi.uv.Y) * (x33 - x00);
          inter.TextureCoord.Y = y33 + bi.uv.Y * (y00 - y33);
        }
      }
    }

    public void GetBoundingBox (out Vector3d corner1, out Vector3d corner2)
    {
      corner1 = minV;
      corner2 = maxV;
    }
  }

  /// <summary>
  /// Triangle mesh able to compute ray-intersection and normal vector.
  /// </summary>
  [Serializable]
  public class TriangleMesh : DefaultSceneNode, ISolid
  {
    [Serializable]
    protected class TmpData
    {
      /// <summary>
      /// Face id (id of the intersected triangle).
      /// </summary>
      public int face;

      /// <summary>
      /// Barycentric coordinates in the intersected triangle.
      /// </summary>
      public Vector2d uv;
    }


    /// <summary>
    /// Original mesh object (triangles in a "Corner table").
    /// </summary>
    protected SceneBrep mesh;

    /// <summary>
    /// Shell mode: surface is considered as a thin shell (double-sided).
    /// </summary>
    public bool ShellMode { get; set; }

    /// <summary>
    /// Smooth mode: smooth interpolation of surface normals (a la Phong shading).
    /// </summary>
    public bool Smooth { get; set; }

    public TriangleMesh (SceneBrep m)
    {
      mesh      = m;
      ShellMode = false;
      Smooth    = true;
    }

    /// <summary>
    /// Computes the complete intersection of the given ray with the object.
    /// This is unefficient template used as a base class for FastTriangleMesh.
    /// </summary>
    /// <param name="p0">Ray origin.</param>
    /// <param name="p1">Ray direction vector.</param>
    /// <returns>Sorted list of intersection records.</returns>
    public override LinkedList<Intersection> Intersect (Vector3d p0, Vector3d p1)
    {
      if (mesh == null || mesh.Triangles < 1)
        return null;

      List<Intersection> result = null;

      for (int id = 0; id < mesh.Triangles; id++)
      {
        Vector3 a, b, c;
        mesh.GetTriangleVertices(id, out a, out b, out c);
        Vector2d uv;
        CSGInnerNode.countTriangles++;
        double t = Geometry.RayTriangleIntersection(ref p0, ref p1, ref a, ref b, ref c, out uv);
        if (double.IsInfinity(t))
          continue;

        if (result == null)
          result = new List<Intersection>();

        // Set the 1st Intersection.
        TmpData tmp = new TmpData
        {
          face = id,
          uv   = uv
        };
        Intersection i = new Intersection(this)
        {
          T          = t,
          Enter      = true,
          Front      = true,
          CoordLocal = p0 + t * p1,
          SolidData  = tmp
        };

        result.Add(i);

        if (!ShellMode)
          continue;

        // Set the 2nd Intersection.
        t += Intersection.RAY_EPSILON;
        i = new Intersection(this)
        {
          T          = t,
          Enter      = false,
          Front      = false,
          CoordLocal = p0 + t * p1,
          SolidData  = tmp
        };

        result.Add(i);
      }

      if (result == null)
        return null;

      // Finalizing the result: sort the result list
      result.Sort();
      return new LinkedList<Intersection>(result);
    }

    /// <summary>
    /// Complete all relevant items in the given Intersection object.
    /// </summary>
    /// <param name="inter">Intersection instance to complete.</param>
    public override void CompleteIntersection (Intersection inter)
    {
      // !!!{{ TODO: add your actual completion code here

      // normal vector:
      if (inter.SolidData is TmpData tmp)
      {
        int v1, v2, v3;
        mesh.GetTriangleVertices(tmp.face, out v1, out v2, out v3);

        if (Smooth && mesh.Normals > 0) // smooth interpolation of normal vectors
        {
          inter.NormalLocal  = (Vector3d)mesh.GetNormal(v1) * (1.0 - tmp.uv.X - tmp.uv.Y);
          inter.NormalLocal += (Vector3d)mesh.GetNormal(v2) * tmp.uv.X;
          inter.NormalLocal += (Vector3d)mesh.GetNormal(v3) * tmp.uv.Y;
        }
        else
        {
          // Local normal vector computed from the triangle.
          Vector3 a  = mesh.GetVertex(v1);
          Vector3 ba = mesh.GetVertex(v2) - a;
          Vector3 ca = mesh.GetVertex(v3) - a;
          Vector3.Cross(ref ba, ref ca, out Vector3 n);
          inter.NormalLocal = (Vector3d)n;
        }
      }

      // 2D texture coordinates (not yet):
      inter.TextureCoord.X =
      inter.TextureCoord.Y = 0.0;

      // !!!}}
    }

    public void GetBoundingBox (out Vector3d corner1, out Vector3d corner2)
    {
      throw new NotImplementedException();
    }
  }
}
