using System;
using System.Collections.Generic;
using System.Diagnostics;
using MathSupport;
using OpenTK;
using Scene3D;

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
      CSGInnerNode.countBoundingBoxes++;
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

      int j = 0;
      for ( int i = 0; i < nRoots; i++ )
        if ( !Geometry.IsZero( roots[ i ] ) )
          roots[ j++ ] = 1.0 / roots[ i ];
      nRoots = j;

      if ( nRoots >= 2 )
        Array.Sort( roots, 0, nRoots );

      //Debug.Assert( (nRoots % 2 == 0), "Roots(" + nRoots + "): " + roots[ 0 ] + " " + roots[ 1 ] + " " + roots[ 2 ] + " " + roots[ 3 ] );

      LinkedList<Intersection> result = new LinkedList<Intersection>();
      Intersection ix;
      for ( j = 0; j < nRoots; j++ )
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

    public BezierIntersection ( BezierPatch p, double _t )
    {
      patch = p;
      t = _t;
    }

    public int CompareTo ( BezierIntersection bi )
    {
      if ( t < bi.t ) return -1;
      if ( t > bi.t ) return 1;
      return 0;
    }
  }

  /// <summary>
  /// Support data container, holds info about one (subdivided) Bezier patch and its AABB.
  /// Static shareable data, multi-thread safe.
  /// </summary>
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
      p = new Vector3d[ 16 ];
    }

    /// <summary>
    /// Creates its own copy of control point array.
    /// </summary>
    public BezierPatch ( BezierPatch b )
    {
      p = (Vector3d[])b.p.Clone();
      bbMin = b.bbMin;
      bbSize = b.bbSize;
      tex00 = b.tex00;
      tex33 = b.tex33;
    }

    public object Clone ()
    {
      return new BezierPatch( this );
    }

    /// <summary>
    /// [Re-]builds a bounding box for this patch.
    /// </summary>
    public void UpdateBB ()
    {
      bbMin = bbSize = p[ 0 ];
      for ( int i = 1; i < 16; i++ )
      {
        double pa = p[ i ].X;
        if ( pa < bbMin.X )  bbMin.X  = pa;
        if ( pa > bbSize.X ) bbSize.X = pa;
        pa = p[ i ].Y;
        if ( pa < bbMin.Y )  bbMin.Y  = pa;
        if ( pa > bbSize.Y ) bbSize.Y = pa;
        pa = p[ i ].Z;
        if ( pa < bbMin.Z )  bbMin.Z  = pa;
        if ( pa > bbSize.Z ) bbSize.Z = pa;
      }
      bbSize -= bbMin;
    }

    protected void DivideHorizontal ( bool left )
    {
      for ( int off = 0; off < 16; off += 4 )    // one horizontal row
      {
        Vector3d p01 = 0.5 * (p[ off ]     + p[ off + 1 ]);
        Vector3d p12 = 0.5 * (p[ off + 1 ] + p[ off + 2 ]);
        Vector3d p23 = 0.5 * (p[ off + 2 ] + p[ off + 3 ]);
        Vector3d p0112 = 0.5 * (p01 + p12);
        Vector3d p1223 = 0.5 * (p12 + p23);
        Vector3d p01112223 = 0.5 * (p0112 + p1223);
        if ( left )
        {
          p[ off + 1 ] = p01;
          p[ off + 2 ] = p0112;
          p[ off + 3 ] = p01112223;
        }
        else
        {
          p[ off ]     = p01112223;
          p[ off + 1 ] = p1223;
          p[ off + 2 ] = p23;
        }
      }
      double texHalf = 0.5 * (tex00.X + tex33.X);
      if ( left )
        tex33.X = texHalf;
      else
        tex00.X = texHalf;
      UpdateBB();
    }

    protected void DivideVertical ( bool up )
    {
      for ( int off = 0; off < 4; off++ )    // one vertical column
      {
        Vector3d p01 = 0.5 * (p[ off ] + p[ off + 4 ]);
        Vector3d p12 = 0.5 * (p[ off + 4 ] + p[ off + 8 ]);
        Vector3d p23 = 0.5 * (p[ off + 8 ] + p[ off + 12 ]);
        Vector3d p0112 = 0.5 * (p01 + p12);
        Vector3d p1223 = 0.5 * (p12 + p23);
        Vector3d p01112223 = 0.5 * (p0112 + p1223);
        if ( up )
        {
          p[ off + 4 ]  = p01;
          p[ off + 8 ]  = p0112;
          p[ off + 12 ] = p01112223;
        }
        else
        {
          p[ off ] = p01112223;
          p[ off + 4 ] = p1223;
          p[ off + 8 ] = p23;
        }
      }
      double texHalf = 0.5 * (tex00.Y + tex33.Y);
      if ( up )
        tex33.Y = texHalf;
      else
        tex00.Y = texHalf;
      UpdateBB();
    }

    /// <summary>
    /// Re-entrant intersection function. Doesn't modify the patch instance at all.
    /// </summary>
    /// <returns>Double.NegativeInfinity if negative.</returns>
    public double IntersectInv ( ref Vector3d p0, ref Vector3d p1 )
    {
      CSGInnerNode.countBoundingBoxes++;
      Vector2d result;
      return( Geometry.RayBoxIntersectionInv( ref p0, ref p1, ref bbMin, ref bbSize, out result ) ? result.X : Double.NegativeInfinity );
    }

    /// <summary>
    /// Builds a R-tree for this patch.
    /// Uses this patch as R-tree root.
    /// </summary>
    public BezierPatch BuildTree ()
    {
      if ( bbSize.LengthSquared < Epsilon )
      {
        left = right = null;
        return this;
      }

      // subdivide the patch:
      BezierPatch b1 = (BezierPatch)Clone();  // left child
      BezierPatch b2 = (BezierPatch)Clone();  // right child

      double hor = (p[ 3 ] - p[ 0 ]).LengthFast + (p[ 15 ] - p[ 12 ]).LengthFast;
      double ver = (p[ 12 ] - p[ 0 ]).LengthFast + (p[ 15 ] - p[ 3 ]).LengthFast;
      if ( hor > ver )
      {
        // first child:
        b1.DivideHorizontal( true );
        // second child:
        b2.DivideHorizontal( false );
      }
      else
      {
        // first child:
        b1.DivideVertical( true );
        // second child:
        b2.DivideVertical( false );
      }

      left  = b1.BuildTree();
      right = b2.BuildTree();

      return this;
    }
  }

  /// <summary>
  /// Bezier surface able to compute ray-intersection, normal vector
  /// and 2D texture coordinates.
  /// </summary>
  public class BezierSurface : DefaultSceneNode, ISolid
  {
    /// <summary>
    /// Root Bezier patches R-trees.
    /// Shared, must not be modified during computation!
    /// </summary>
    private List<BezierPatch> patches;

    /// <summary>
    /// Compute normal vectors using Gouraud interpolation?
    /// </summary>
    public bool PreciseNormals
    {
      get;
      set;
    }

    /// <summary>
    /// Subdivision threshold.
    /// </summary>
    public static double Epsilon
    {
      get
      {
        return BezierPatch.Epsilon;
      }
      set
      {
        BezierPatch.Epsilon = value;
      }
    }

    public BezierSurface ( int K, int L, double[] v )
    {
      Debug.Assert( K > 0 && L > 0 );
      Debug.Assert( v != null && v.Length >= 3 * (3 * K + 1) * (3 * L + 1) );

      PreciseNormals = true;

      patches = new List<BezierPatch>( K * L );
      int stride = 3 * (3 * L + 1);
      int ik = 0;
      for ( int k = 0; k < K; k++, ik += 3 * stride )
      {
        int il = ik;
        for ( int l = 0; l < L; l++, il += 9 )
        {
          BezierPatch p = new BezierPatch();
          int oi = 0;
          for ( int i = 0; i < 4 * stride; i += stride )
            for ( int j = il + i; j < il + i + 12; oi++ )
            {
              p.p[ oi ].X = v[ j++ ];
              p.p[ oi ].Y = v[ j++ ];
              p.p[ oi ].Z = v[ j++ ];
            }
          p.tex00.X = l;
          p.tex00.Y = k;
          p.tex33.X = l + 1.0;
          p.tex33.Y = k + 1.0;
          p.UpdateBB();
          patches.Add( p.BuildTree() );
        }
      }
    }

    /// <summary>
    /// Computes the complete intersection of the given ray with the object.
    /// </summary>
    /// <param name="p0">Ray origin.</param>
    /// <param name="p1">Ray direction vector.</param>
    /// <returns>Sorted list of intersection records.</returns>
    public override LinkedList<Intersection> Intersect ( Vector3d p0, Vector3d p1 )
    {
      HeapMin<BezierIntersection> h = new HeapMin<BezierIntersection>();
      Vector3d p1inv;
      p1inv.X = Geometry.IsZero( p1.X ) ? Double.PositiveInfinity : 1.0 / p1.X;
      p1inv.Y = Geometry.IsZero( p1.Y ) ? Double.PositiveInfinity : 1.0 / p1.Y;
      p1inv.Z = Geometry.IsZero( p1.Z ) ? Double.PositiveInfinity : 1.0 / p1.Z;

      foreach ( BezierPatch b in patches )
      {
        double t = b.IntersectInv( ref p0, ref p1inv );
        if ( !Double.IsInfinity( t ) )
        {
          BezierIntersection bi = new BezierIntersection( b, t );
          h.Add( bi );
        }
      }

      LinkedList<Intersection> result = null;
      Intersection i;

      while ( h.Count > 0 )
      {
        BezierIntersection bi = h.RemoveMin();
        if ( bi.final > 0 )
        {                                   // intersection
          if ( result == null )
            result = new LinkedList<Intersection>();
          i = new Intersection( this );
          i.T = bi.t;
          i.Enter =
          i.Front = bi.enter;
          i.CoordLocal = p0 + bi.t * p1;
          i.SolidData = bi;
          result.AddLast( i );
        }
        else
        {                                   // patch (to subdivide?)
          if ( bi.patch.left != null )      // subdivision
          {
            BezierPatch right = bi.patch.right;

            // left child
            double t = bi.patch.left.IntersectInv( ref p0, ref p1inv );
            if ( !Double.IsInfinity( t ) )
            {
              bi.patch = bi.patch.left;
              bi.t = t;
              h.Add( bi );
              bi = null;
            }

            // right child:
            t = right.IntersectInv( ref p0, ref p1inv );
            if ( !Double.IsInfinity( t ) )
            {
              if ( bi == null )
                bi = new BezierIntersection( right, t );
              else
              {
                bi.patch = right;
                bi.t = t;
              }
              h.Add( bi );
            }
          }
          else                              // patch is too small => intersect the two triangles..
          {
            bi.final = 0;
            Vector3d[] cp = bi.patch.p;
            CSGInnerNode.countTriangles++;
            bi.t = Geometry.RayTriangleIntersection( ref p0, ref p1, ref cp[ 12 ], ref cp[ 3 ], ref cp[ 0 ], out bi.uv );
            if ( !Double.IsInfinity( bi.t ) )
            {
              bi.final = 1;
              bi.tu = cp[ 0 ] - cp[ 12 ];
              bi.tv = cp[ 3 ] - cp[ 12 ];
            }
            else
            {
              CSGInnerNode.countTriangles++;
              bi.t = Geometry.RayTriangleIntersection( ref p0, ref p1, ref cp[ 12 ], ref cp[ 15 ], ref cp[ 3 ], out bi.uv );
              if ( !Double.IsInfinity( bi.t ) )
              {
                bi.final = 2;
                bi.tu = cp[ 3 ] - cp[ 12 ];
                bi.tv = cp[ 15 ] - cp[ 12 ];
              }
            }
            if ( bi.final > 0 )
            {
              Vector3d n;
              Vector3d.Cross( ref bi.tu, ref bi.tv, out n );
              double dot;
              Vector3d.Dot( ref n, ref p1, out dot );
              bi.enter = dot < 0.0;
              h.Add( bi );
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
    public override void CompleteIntersection ( Intersection inter )
    {
      BezierIntersection bi = inter.SolidData as BezierIntersection;
      if ( bi != null )
      {
        // normal vector:
        Vector3d tu, tv;
        if ( PreciseNormals )               // Gouraud interpolation of exact normal vectors at the corners..
        {
          Vector3d na, nb, nc;
          Vector3d[] cp = bi.patch.p;
          if ( bi.final == 1 )
          {                                 // upper left triangle
            tu = Vector3d.TransformVector( cp[ 8 ] - cp[ 12 ], inter.LocalToWorld );
            tv = Vector3d.TransformVector( cp[ 13 ] - cp[ 12 ], inter.LocalToWorld );
            Vector3d.Cross( ref tu, ref tv, out na );
            tu = Vector3d.TransformVector( cp[ 7 ] - cp[ 3 ], inter.LocalToWorld );
            tv = Vector3d.TransformVector( cp[ 2 ] - cp[ 3 ], inter.LocalToWorld );
            Vector3d.Cross( ref tu, ref tv, out nb );
            tu = Vector3d.TransformVector( cp[ 1 ] - cp[ 0 ], inter.LocalToWorld );
            tv = Vector3d.TransformVector( cp[ 4 ] - cp[ 0 ], inter.LocalToWorld );
            Vector3d.Cross( ref tu, ref tv, out nc );
          }
          else
          {                                 // lower right triangle
            tu = Vector3d.TransformVector( cp[ 8 ] - cp[ 12 ], inter.LocalToWorld );
            tv = Vector3d.TransformVector( cp[ 13 ] - cp[ 12 ], inter.LocalToWorld );
            Vector3d.Cross( ref tu, ref tv, out na );
            tu = Vector3d.TransformVector( cp[ 14 ] - cp[ 15 ], inter.LocalToWorld );
            tv = Vector3d.TransformVector( cp[ 11 ] - cp[ 15 ], inter.LocalToWorld );
            Vector3d.Cross( ref tu, ref tv, out nb );
            tu = Vector3d.TransformVector( cp[ 7 ] - cp[ 3 ], inter.LocalToWorld );
            tv = Vector3d.TransformVector( cp[ 2 ] - cp[ 3 ], inter.LocalToWorld );
            Vector3d.Cross( ref tu, ref tv, out nc );
          }
          na.Normalize();
          nb.Normalize();
          nc.Normalize();
          inter.Normal = (1.0 - bi.uv.X - bi.uv.Y) * na + bi.uv.X * nb + bi.uv.Y * nc;
        }
        else                                // flat normal
        {
          bi.tu = Vector3d.TransformVector( bi.tu, inter.LocalToWorld );
          bi.tv = Vector3d.TransformVector( bi.tv, inter.LocalToWorld );
          Vector3d.Cross( ref bi.tu, ref bi.tv, out inter.Normal );
        }

        // 2D texture coordinates:
        double x00 = bi.patch.tex00.X;
        double y00 = bi.patch.tex00.Y;
        double x33 = bi.patch.tex33.X;
        double y33 = bi.patch.tex33.Y;
        if ( bi.final == 1 )
        {                                   // upper left triangle
          inter.TextureCoord.X = x00 + bi.uv.X * (x33 - x00);
          inter.TextureCoord.Y = y33 + (bi.uv.X + bi.uv.Y) * (y00 - y33);
        }
        else
        {                                   // lower right triangle
          inter.TextureCoord.X = x00 + (bi.uv.X + bi.uv.Y) * (x33 - x00);
          inter.TextureCoord.Y = y33 + bi.uv.Y * (y00 - y33);
        }
      }
    }
  }

  /// <summary>
  /// Triangle mesh able to compute ray-intersection and normal vector.
  /// </summary>
  public class TriangleMesh : DefaultSceneNode, ISolid
  {
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

      // Vertex ids.
      //int va, vb, vc;

      /// <summary>
      /// Normal vector in B-rep coordinates.
      /// </summary>
      public Vector3 normal;
    }

    /// <summary>
    /// Original mesh object (triangles in a "Corner table").
    /// </summary>
    protected SceneBrep mesh;

    /// <summary>
    /// Shell mode: surface is considered as a thin shell (double-sided).
    /// </summary>
    public bool ShellMode
    {
      get;
      set;
    }

    /// <summary>
    /// Smooth mode: smooth interpolation of surface normals (a la Phong shading).
    /// </summary>
    public bool Smooth
    {
      get;
      set;
    }

    public TriangleMesh ( SceneBrep m )
    {
      mesh = m;
      ShellMode = false;
      Smooth = true;
    }

    /// <summary>
    /// Computes the complete intersection of the given ray with the object.
    /// This is unefficient template used as a base class for FastTriangleMesh.
    /// </summary>
    /// <param name="p0">Ray origin.</param>
    /// <param name="p1">Ray direction vector.</param>
    /// <returns>Sorted list of intersection records.</returns>
    public override LinkedList<Intersection> Intersect ( Vector3d p0, Vector3d p1 )
    {
      if ( mesh == null || mesh.Triangles < 1 )
        return null;

      List<Intersection> result = null;
      Intersection i;

      for ( int id = 0; id < mesh.Triangles; id++ )
      {
        Vector3 a, b, c;
        mesh.GetTriangleVertices( id, out a, out b, out c );
        Vector2d uv;
        CSGInnerNode.countTriangles++;
        double t = Geometry.RayTriangleIntersection( ref p0, ref p1, ref a, ref b, ref c, out uv );
        if ( Double.IsInfinity( t ) )
          continue;

        if ( result == null )
          result = new List<Intersection>();

        // Compile the 1st Intersection instance:
        i = new Intersection( this );
        i.T = t;
        i.Enter =
        i.Front = true;
        i.CoordLocal = p0 + i.T * p1;

        // Tmp data object
        TmpData tmp = new TmpData();
        tmp.face = id;
        tmp.uv = uv;
        Vector3 ba = b - a;    // temporary value for flat shading
        Vector3 ca = c - a;
        Vector3.Cross( ref ba, ref ca, out tmp.normal );
        i.SolidData = tmp;

        result.Add( i );

        if ( !ShellMode )
          continue;

        // Compile the 2nd Intersection instance:
        i = new Intersection( this );
        i.T = t + 1.0e-4;
        i.Enter =
        i.Front = false;
        i.CoordLocal = p0 + i.T * p1;

        // Tmp data object
        TmpData tmp2 = new TmpData();
        tmp2.face = id;
        tmp2.uv = uv;
        tmp2.normal = -tmp.normal;
        i.SolidData = tmp2;

        result.Add( i );
      }

      if ( result == null )
        return null;

      // Finalizing the result: sort the result list
      result.Sort();
      return new LinkedList<Intersection>( result );
    }

    /// <summary>
    /// Complete all relevant items in the given Intersection object.
    /// </summary>
    /// <param name="inter">Intersection instance to complete.</param>
    public override void CompleteIntersection ( Intersection inter )
    {
      // !!!{{ TODO: add your actual completion code here

      // normal vector:
      TmpData tmp = inter.SolidData as TmpData;
      if ( tmp != null )
      {
        if ( Smooth && mesh.Normals > 0 )   // smooth interpolation of normal vectors
        {
          int v1, v2, v3;
          mesh.GetTriangleVertices( tmp.face, out v1, out v2, out v3 );
          tmp.normal  = mesh.GetNormal( v1 ) * (float)(1.0 - tmp.uv.X - tmp.uv.Y);
          tmp.normal += mesh.GetNormal( v2 ) * (float)tmp.uv.X;
          tmp.normal += mesh.GetNormal( v3 ) * (float)tmp.uv.Y;
        }

        Vector3d tu, tv;
        Vector3d normal = (Vector3d)tmp.normal;
        Geometry.GetAxes( ref normal, out tu, out tv );
        tu = Vector3d.TransformVector( tu, inter.LocalToWorld );
        tv = Vector3d.TransformVector( tv, inter.LocalToWorld );
        Vector3d.Cross( ref tu, ref tv, out inter.Normal );
      }

      // 2D texture coordinates (not yet):
      inter.TextureCoord.X =
      inter.TextureCoord.Y = 0.0;

      // !!!}}
    }
  }
}
