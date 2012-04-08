using System;
using System.Collections.Generic;
using System.Drawing;
using MathSupport;
using OpenTK;

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
            fMin = CubeFaces.NegativeX;
          }
          if ( t1 < tMax )
          {
            tMax = t1;
            fMax = CubeFaces.PositiveX;
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
            fMin = CubeFaces.NegativeY;
          }
          if ( t1 < tMax )
          {
            tMax = t1;
            fMax = CubeFaces.PositiveY;
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
            fMin = CubeFaces.NegativeZ;
          }
          if ( t1 < tMax )
          {
            tMax = t1;
            fMax = CubeFaces.PositiveZ;
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
}
