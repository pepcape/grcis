using System;
using System.Collections.Generic;
using System.Drawing;
using MathSupport;
using OpenTK;
using System.Diagnostics;

namespace Rendering
{
  /// <summary>
  /// Unit sphere as a simple solid able to compute ray-intersection, normal vector
  /// and 2D texture coordinates.
  /// </summary>
  public class Sphere : DefaultSceneNode, ISolid
  {
    /// <summary>
    /// Computes the complete intersection of the given ray with the object.
    /// </summary>
    /// <param name="p0">Ray origin.</param>
    /// <param name="p1">Ray direction vector.</param>
    /// <returns>Sorted list of intersection records.</returns>
    public override LinkedList<Intersection> Intersect ( Vector3d p0, Vector3d p1 )
    {
      double OD; Vector3d.Dot( ref p0, ref p1, out OD );
      double DD; Vector3d.Dot( ref p1, ref p1, out DD );
      double OO; Vector3d.Dot( ref p0, ref p0, out OO );
      double  d = OD * OD + DD * (1.0 - OO); // discriminant
      if ( d <= 0.0 ) return null;           // no intersections

      d = Math.Sqrt( d );

      // there will be two intersections: (-OD - d) / DD, (-OD + d) / DD
      LinkedList<Intersection> result = new LinkedList<Intersection>();
      Intersection i;

      // first intersection (-OD - d) / DD:
      i = new Intersection( this );
      i.T = (-OD - d) / DD;
      i.Enter =
      i.Front = true;
      i.CoordLocal.X = p0.X + i.T * p1.X;
      i.CoordLocal.Y = p0.Y + i.T * p1.Y;
      i.CoordLocal.Z = p0.Z + i.T * p1.Z;
      result.AddLast( i );

      // second intersection (-OD + d) / DD:
      i = new Intersection( this );
      i.T = (-OD + d) / DD;
      i.Enter =
      i.Front = false;
      i.CoordLocal.X = p0.X + i.T * p1.X;
      i.CoordLocal.Y = p0.Y + i.T * p1.Y;
      i.CoordLocal.Z = p0.Z + i.T * p1.Z;
      result.AddLast( i );

      return result;
    }

    /// <summary>
    /// Complete all relevant items in the given Intersection object.
    /// </summary>
    /// <param name="inter">Intersection instance to complete.</param>
    public override void CompleteIntersection ( Intersection inter )
    {
      // normal vector:
      Vector3d tu, tv;
      Geometry.GetAxes( ref inter.CoordLocal, out tu, out tv );
      tu = Vector3d.TransformVector( tu, inter.LocalToWorld );
      tv = Vector3d.TransformVector( tv, inter.LocalToWorld );
      Vector3d.Cross( ref tu, ref tv, out inter.Normal );

      // 2D texture coordinates:
      double r = Math.Sqrt( inter.CoordLocal.X * inter.CoordLocal.X + inter.CoordLocal.Y * inter.CoordLocal.Y );
      inter.TextureCoord.X = Geometry.IsZero( r ) ? 0.0 : (Math.Atan2( inter.CoordLocal.Y, inter.CoordLocal.X ) / (2.0 * Math.PI) + 0.5 );
      inter.TextureCoord.Y = Math.Atan2( r, inter.CoordLocal.Z ) / Math.PI;
    }
  }

  /// <summary>
  /// Normalized (unit) cube as a simple solid able to compute ray-intersection, normal vector
  /// and 2D texture coordinates. [0,1]^3
  /// </summary>
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
      new Vector3d(  1,  0,  0 ),
      new Vector3d( -1,  0,  0 ),
      new Vector3d(  0,  1,  0 ),
      new Vector3d(  0, -1,  0 ),
      new Vector3d(  0,  0,  1 ),
      new Vector3d(  0,  0, -1 )
    };

    /// <summary>
    /// Computes the complete intersection of the given ray with the object.
    /// </summary>
    /// <param name="p0">Ray origin.</param>
    /// <param name="p1">Ray direction vector.</param>
    /// <returns>Sorted list of intersection records.</returns>
    public override LinkedList<Intersection> Intersect ( Vector3d p0, Vector3d p1 )
    {
      double tMin = Double.MinValue;
      double tMax = Double.MaxValue;
      CubeFaces fMin = CubeFaces.PositiveX;
      CubeFaces fMax = CubeFaces.PositiveX;
      double mul, t1, t2;

      // normal = X axis:
      if ( Geometry.IsZero( p1.X ) )
      {
        if ( p0.X <= 0.0 || p0.X >= 1.0 )
          return null;
      }
      else
      {
        mul = 1.0 / p1.X;
        t1 = -p0.X * mul;
        t2 = t1 + mul;

        if ( mul > 0.0 )
        {
          if ( t1 > tMin )
          {
            tMin = t1;
            fMin = CubeFaces.NegativeX;
          }
          if ( t2 < tMax )
          {
            tMax = t2;
            fMax = CubeFaces.PositiveX;
          }
        }
        else
        {
          if ( t2 > tMin )
          {
            tMin = t2;
            fMin = CubeFaces.PositiveX;
          }
          if ( t1 < tMax )
          {
            tMax = t1;
            fMax = CubeFaces.NegativeX;
          }
        }

        if ( tMin >= tMax )
          return null;
      }

      // normal = Y axis:
      if ( Geometry.IsZero( p1.Y ) )
      {
        if ( p0.Y <= 0.0 || p0.Y >= 1.0 )
          return null;
      }
      else
      {
        mul = 1.0 / p1.Y;
        t1 = -p0.Y * mul;
        t2 = t1 + mul;

        if ( mul > 0.0 )
        {
          if ( t1 > tMin )
          {
            tMin = t1;
            fMin = CubeFaces.NegativeY;
          }
          if ( t2 < tMax )
          {
            tMax = t2;
            fMax = CubeFaces.PositiveY;
          }
        }
        else
        {
          if ( t2 > tMin )
          {
            tMin = t2;
            fMin = CubeFaces.PositiveY;
          }
          if ( t1 < tMax )
          {
            tMax = t1;
            fMax = CubeFaces.NegativeY;
          }
        }

        if ( tMin >= tMax )
          return null;
      }

      // normal = Z axis:
      if ( Geometry.IsZero( p1.Z ) )
      {
        if ( p0.Z <= 0.0 || p0.Z >= 1.0 )
          return null;
      }
      else
      {
        mul = 1.0 / p1.Z;
        t1 = -p0.Z * mul;
        t2 = t1 + mul;

        if ( mul > 0.0 )
        {
          if ( t1 > tMin )
          {
            tMin = t1;
            fMin = CubeFaces.NegativeZ;
          }
          if ( t2 < tMax )
          {
            tMax = t2;
            fMax = CubeFaces.PositiveZ;
          }
        }
        else
        {
          if ( t2 > tMin )
          {
            tMin = t2;
            fMin = CubeFaces.PositiveZ;
          }
          if ( t1 < tMax )
          {
            tMax = t1;
            fMax = CubeFaces.NegativeZ;
          }
        }

        if ( tMin >= tMax )
          return null;
      }

      // Finally assemble the two intersections:
      LinkedList<Intersection> result = new LinkedList<Intersection>();

      Intersection i;
      i = new Intersection( this );
      i.T = tMin;
      i.Enter =
      i.Front = true;
      i.CoordLocal = p0 + tMin * p1;
      i.SolidData = fMin;
      result.AddLast( i );

      i = new Intersection( this );
      i.T = tMax;
      i.Enter =
      i.Front = false;
      i.CoordLocal = p0 + tMax * p1;
      i.SolidData = fMax;
      result.AddLast( i );

      return result;
    }

    /// <summary>
    /// Complete all relevant items in the given Intersection object.
    /// </summary>
    /// <param name="inter">Intersection instance to complete.</param>
    public override void CompleteIntersection ( Intersection inter )
    {
      // normal vector:
      Vector3d tu, tv;
      Geometry.GetAxes( ref Normals[ (int)inter.SolidData ], out tu, out tv );
      tu = Vector3d.TransformVector( tu, inter.LocalToWorld );
      tv = Vector3d.TransformVector( tv, inter.LocalToWorld );
      Vector3d.Cross( ref tu, ref tv, out inter.Normal );

      // 2D texture coordinates:
      switch ( (CubeFaces)inter.SolidData )
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
  }

  /// <summary>
  /// Plane objects (infinite plane, rectangle, triangle) as elementary 3D solids.
  /// </summary>
  public class Plane : DefaultSceneNode, ISolid
  {
    /// <summary>
    /// Lower bound for x coordinate.
    /// </summary>
    public double XMin
    {
      get;
      set;
    }

    /// <summary>
    /// Upper bound for x coordinate (or triangle).
    /// </summary>
    public double XMax
    {
      get;
      set;
    }

    /// <summary>
    /// Lower bound for y coordinate.
    /// </summary>
    public double YMin
    {
      get;
      set;
    }

    /// <summary>
    /// Upper bound for y coordinate (or triangle).
    /// </summary>
    public double YMax
    {
      get;
      set;
    }

    /// <summary>
    /// Use three restrictions instead of four (the 3rd one is: xMax * x + yMax * y &le; 1.0).
    /// </summary>
    public bool Triangle
    {
      get;
      set;
    }

    /// <summary>
    /// Use canonic u coordinate (=x)?
    /// </summary>
    public bool CanonicU
    {
      get;
      set;
    }

    /// <summary>
    /// Use canonic v coordinate (=y)?
    /// </summary>
    public bool CanonicV
    {
      get;
      set;
    }

    /// <summary>
    /// Scale coefficient for u coordinate.
    /// </summary>
    public double ScaleU
    {
      get;
      set;
    }

    /// <summary>
    /// Scale coefficient for v coordinate.
    /// </summary>
    public double ScaleV
    {
      get;
      set;
    }

    /// <summary>
    /// Default constructor - infinite plane.
    /// </summary>
    public Plane ()
      : this( Double.NegativeInfinity, Double.PositiveInfinity,
              Double.NegativeInfinity, Double.PositiveInfinity )
    {
    }

    /// <summary>
    /// Rectangle (four restrictions).
    /// </summary>
    public Plane ( double xMi, double xMa, double yMi, double yMa )
    {
      XMin = xMi;
      XMax = xMa;
      YMin = yMi;
      YMax = yMa;
      Triangle = false;
      CanonicU =
      CanonicV = true;
      ScaleU =
      ScaleV = 1.0;

      // texture coordinates:
      if ( !Double.IsInfinity( XMin ) &&
           !Double.IsInfinity( XMax ) )
      {
        CanonicU = false;
        ScaleU   = 1.0 / (XMax - XMin);
      }
      if ( !Double.IsInfinity( YMin ) &&
           !Double.IsInfinity( YMax ) )
      {
        CanonicV = false;
        ScaleV   = 1.0 / (YMax - YMin);
      }
    }

    /// <summary>
    /// Triangle (three restrictions).
    /// xMax * x + yMax * y &le; 1.0
    /// </summary>
    public Plane ( double xMa, double yMa )
    {
      YMin     = XMin = 0.0;
      XMax     = (xMa < Double.Epsilon) ? 1.0 : 1.0 / xMa;
      YMax     = (yMa < Double.Epsilon) ? 1.0 : 1.0 / yMa;
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
    public override LinkedList<Intersection> Intersect ( Vector3d p0, Vector3d p1 )
    {
      if ( Geometry.IsZero( p1.Z ) )
        return null;

      double t = -p0.Z / p1.Z;
      double x = p0.X + t * p1.X;
      if ( x < XMin ) return null;
      double y = p0.Y + t * p1.Y;
      if ( y < YMin ) return null;
      double u, v;

      if ( Triangle )
      {
        u = x * XMax;
        v = y * YMax;
        if ( u + v > 1.0 ) return null;
      }
      else
        if ( x > XMax ||
             y > YMax ) return null;
        else
        {
          u = CanonicU ? x : (x - XMin) * ScaleU;
          v = CanonicV ? y : (y - YMin) * ScaleV;
        }

      // there will be one intersection..
      LinkedList<Intersection> result = new LinkedList<Intersection>();
      Intersection i = new Intersection( this );
      i.T = t;
      i.Enter =
      i.Front = p1.Z < 0.0;
      i.CoordLocal.X = x;
      i.CoordLocal.Y = y;
      i.CoordLocal.Z = 0.0;
      i.TextureCoord.X = u;
      i.TextureCoord.Y = v;
      result.AddLast( i );

      return result;
    }

    /// <summary>
    /// Complete all relevant items in the given Intersection object.
    /// </summary>
    /// <param name="inter">Intersection instance to complete.</param>
    public override void CompleteIntersection ( Intersection inter )
    {
      // normal vector:
      Vector3d tu = Vector3d.TransformVector( Vector3d.UnitX, inter.LocalToWorld );
      Vector3d tv = Vector3d.TransformVector( Vector3d.UnitY, inter.LocalToWorld );
      Vector3d.Cross( ref tu, ref tv, out inter.Normal );

      // 2D texture coordinates:
      // done
    }
  }

  /// <summary>
  /// Unit cylinder (optionally restrictead by one or two bases) able to compute ray-intersection, normal vector
  /// and 2D texture coordinates.
  /// </summary>
  public class Cylinder : DefaultSceneNode, ISolid
  {
    /// <summary>
    /// Lower bound for z coordinate (optional)
    /// </summary>
    public double ZMin
    {
      get;
      set;
    }

    /// <summary>
    /// Upper bound for z coordinate (optional).
    /// </summary>
    public double ZMax
    {
      get;
      set;
    }

    /// <summary>
    /// Default constructor - infinite cylindric surface.
    /// </summary>
    public Cylinder ()
      : this( Double.NegativeInfinity, Double.PositiveInfinity )
    {
    }

    /// <summary>
    /// Restricted cylinder.
    /// </summary>
    public Cylinder ( double zMi, double zMa )
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
    public override LinkedList<Intersection> Intersect ( Vector3d p0, Vector3d p1 )
    {
      double A = p1.X * p1.X + p1.Y * p1.Y;
      double DA = A + A;
      double B = p0.X * p1.X + p0.Y * p1.Y;
      B += B;
      double C = p0.X * p0.X + p0.Y * p0.Y - 1.0;

      double d = B * B - (DA + DA) * C;    // discriminant
      if ( d <= 0.0 ) return null;         // no intersection

      d = Math.Sqrt( d );
      // there will be two intersections: (-B +- d) / DA
      double t1 = (-B - d) / DA;
      double t2 = (-B + d) / DA;           // t1 < t2
      Vector3d loc1 = p0 + t1 * p1;
      Vector3d loc2 = p0 + t2 * p1;

      double lzmin = Math.Min( loc1.Z, loc2.Z );
      if ( lzmin >= ZMax )
        return null;
      double lzmax = Math.Max( loc1.Z, loc2.Z );
      if ( lzmax <= ZMin )
        return null;

      // there indeed will be two intersections
      LinkedList<Intersection> result = new LinkedList<Intersection>();
      Intersection i;

      // test the two bases:
      double tb1 = 0.0, tb2 = 0.0;
      bool base1 = ZMin > lzmin && ZMin < lzmax;
      if ( base1 )
        tb1 = (ZMin - p0.Z) / p1.Z;
      bool base2 = ZMax > lzmin && ZMax < lzmax;
      if ( base2 )
        tb2 = (ZMax - p0.Z) / p1.Z;

      // Enter the solid:
      i = new Intersection( this );
      i.Enter =
      i.Front = true;
      if ( base1 && p1.Z > 0.0 )
      {
        // enter through the 1st base:
        i.T = tb1;
        i.CoordLocal = p0 + tb1 * p1;
        i.SolidData = -1;
      }
      else if ( base2 && p1.Z < 0.0 )
      {
        // enter through the 2nd base:
        i.T = tb2;
        i.CoordLocal = p0 + tb2 * p1;
        i.SolidData = 1;
      }
      else
      {
        // cylinder surface:
        i.T = t1;
        i.CoordLocal = loc1;
        i.SolidData = 0;
      }
      result.AddLast( i );

      // Leave the solid:
      i = new Intersection( this );
      i.Enter =
      i.Front = false;
      if ( base1 && p1.Z < 0.0 )
      {
        // enter through the 1st base:
        i.T = tb1;
        i.CoordLocal = p0 + tb1 * p1;
        i.SolidData = -1;
      }
      else if ( base2 && p1.Z > 0.0 )
      {
        // enter through the 2nd base:
        i.T = tb2;
        i.CoordLocal = p0 + tb2 * p1;
        i.SolidData = 1;
      }
      else
      {
        // cylinder surface:
        i.T = t2;
        i.CoordLocal = loc2;
        i.SolidData = 0;
      }
      result.AddLast( i );

      return result;
    }

    /// <summary>
    /// Complete all relevant items in the given Intersection object.
    /// </summary>
    /// <param name="inter">Intersection instance to complete.</param>
    public override void CompleteIntersection ( Intersection inter )
    {
      // normal vector:
      Vector3d tu, tv;
      Vector3d ln;
      ln.Z = (int)inter.SolidData;
      if ( (int)inter.SolidData == 0 )
      {
        ln.X = inter.CoordLocal.X;
        ln.Y = inter.CoordLocal.Y;
        inter.TextureCoord.X = Math.Atan2( inter.CoordLocal.Y, inter.CoordLocal.X ) / (2.0 * Math.PI) + 0.5;
      }
      else
      {
        ln.X = ln.Y = 0.0;
        inter.TextureCoord.X = 0.0;
      }
      Geometry.GetAxes( ref ln, out tu, out tv );
      tu = Vector3d.TransformVector( tu, inter.LocalToWorld );
      tv = Vector3d.TransformVector( tv, inter.LocalToWorld );
      Vector3d.Cross( ref tu, ref tv, out inter.Normal );

      // 2D texture coordinates:
      inter.TextureCoord.Y = inter.CoordLocal.Z;
    }
  }

  /// <summary>
  /// Torus (original author: Jan Navratil, (c) 2012).
  /// </summary>
  public class Torus : DefaultSceneNode, ISolid
  {
    public double bigRadius = 1.0;

    public double smallRadius = 0.5;

    protected CSGInnerNode wrapper = new CSGInnerNode( SetOperation.Union );

    public Torus ( double big, double small )
    {
      bigRadius = big;
      smallRadius = small;
      wrapper.InsertChild( new Sphere(), Matrix4d.Scale( bigRadius + smallRadius ) );
    }

    /// <summary>
    /// Computes the complete intersection of the given ray with the object. 
    /// </summary>
    /// <param name="p0">Ray origin.</param>
    /// <param name="p1">Ray direction vector.</param>
    /// <returns>Sorted list of intersection records.</returns>
    public override LinkedList<Intersection> Intersect ( Vector3d p0, Vector3d p1 )
    {
      if ( wrapper.Intersect( p0, p1 ) == null )
        return null;

      // vlastni implementace podle http://www.emeyex.com/site/projects/raytorus.pdf
      double OD, DD, OO;
      Vector3d.Dot( ref p0, ref p1, out OD );
      Vector3d.Dot( ref p1, ref p1, out DD );
      Vector3d.Dot( ref p0, ref p0, out OO );

      double bRbR = bigRadius * bigRadius;
      double sRsR = smallRadius * smallRadius;
      double OObRbRsRsR = OO - bRbR - sRsR;

      double a = DD * DD;
      double b = 4 * DD * OD;
      double c = 4 * OD * OD + 2 * DD * OObRbRsRsR + 4 * bRbR * p1.Z * p1.Z;
      double d = 4 * OD * OObRbRsRsR + 8 * bRbR * p1.Z * p0.Z;
      double e = OObRbRsRsR * OObRbRsRsR + 4 * bRbR * ( p0.Z * p0.Z - sRsR);

      Polynomial poly = new Polynomial( a, b, c, d, e );
      double[] roots = new double[ 4 ];
      int nRoots = poly.SolveQuartic( roots );
      if ( nRoots == 0 )
        return null;

      for ( int i = 0; i < nRoots; i++ )
        if ( !Geometry.IsZero( roots[ i ] ) )
          roots[ i ] = 1.0 / roots[ i ];

      if ( nRoots >= 2 )
        Array.Sort( roots, 0, nRoots );

      Debug.Assert( (nRoots % 2 == 0), "Roots(" + nRoots + "): " + roots[ 0 ] + " " + roots[ 1 ] + " " + roots[ 2 ] + " " + roots[ 3 ] );

      LinkedList<Intersection> result = new LinkedList<Intersection>();
      Intersection ix;
      for ( int j = 0; j < nRoots; j++ )
      {
        double t = roots[ j ];
        ix = new Intersection( this );
        ix.T = t;
        ix.Enter =
        ix.Front = (j % 2 == 0);
        ix.CoordLocal = p0 + t * p1;

        result.AddLast( ix );
      }

      return result;
    }

    /// <summary>
    /// Complete all relevant items in the given Intersection object.
    /// </summary>
    /// <param name="inter">Intersection instance to complete.</param>
    public override void CompleteIntersection ( Intersection inter )
    {
      // normal vector:
      Vector3d tu, tv;

      // normal vector of an intersection with a torus in the z plain is the same as the intersection coordinates
      // minus the coordinates of the center of the circle it lays on.
      Vector3d circleCenter = getSmallCircleCenter( inter );
      if ( Geometry.IsZero( inter.CoordLocal.X ) &&
           Geometry.IsZero( inter.CoordLocal.Y ) )
        inter.Normal = new Vector3d( 0.0, 0.0, Math.Sign( inter.CoordLocal.Z ) );
      else
      {
        Vector3d smallCircleRadius = inter.CoordLocal - circleCenter;

        //Debug.Assert( Geometry.IsZero( smallCircleRadius.Length - smallRadius ) );
        //Debug.Assert( Geometry.IsZero( circleCenter.Length - bigRadius ) );

        Geometry.GetAxes( ref smallCircleRadius, out tu, out tv );
        tu = Vector3d.TransformVector( tu, inter.LocalToWorld );
        tv = Vector3d.TransformVector( tv, inter.LocalToWorld );
        Vector3d.Cross( ref tu, ref tv, out inter.Normal );
      }

      // 2D texture coordinates:
      double r = Math.Sqrt( inter.CoordLocal.X * inter.CoordLocal.X + inter.CoordLocal.Y * inter.CoordLocal.Y );
      inter.TextureCoord.X = Geometry.IsZero( r ) ? 0.0 : (Math.Atan2( inter.CoordLocal.Y, inter.CoordLocal.X ) / (2.0 * Math.PI) + 0.5);

      Vector3d circleCenterToIntersection = inter.CoordLocal - circleCenter;
      double r2 = Math.Sqrt( circleCenterToIntersection.X * circleCenterToIntersection.X + circleCenterToIntersection.Y * circleCenterToIntersection.Y );
      if ( r < bigRadius )
        r2 = -r2;
      inter.TextureCoord.Y = Math.Atan2( r2, inter.CoordLocal.Z ) / (2.0 * Math.PI) + 0.5;
    }

    private Vector3d getSmallCircleCenter ( Intersection inter )
    {
      Vector3d circleCenter = new Vector3d();
      double torusCenter_intersectionDistanceInPlane = Math.Sqrt( inter.CoordLocal.X * inter.CoordLocal.X + inter.CoordLocal.Y * inter.CoordLocal.Y );
      double centerCalculationRatio = bigRadius / torusCenter_intersectionDistanceInPlane;
      circleCenter.X = inter.CoordLocal.X * centerCalculationRatio;
      circleCenter.Y = inter.CoordLocal.Y * centerCalculationRatio;
      circleCenter.Z = 0.0;

      double assHoleZSquared = smallRadius * smallRadius - bigRadius * bigRadius;
      if ( bigRadius < smallRadius &&                                   // mame prdelni torus
           centerCalculationRatio > 1.0 &&                              // prusecik je bliz centru nez obvodu
           assHoleZSquared >= inter.CoordLocal.Z * inter.CoordLocal.Z ) // Z pruseciku je niz nez asshole
      {
        circleCenter.X = -circleCenter.X;
        circleCenter.Y = -circleCenter.Y;
        inter.Front = false;
      }
      return circleCenter;
    }
  }
}
