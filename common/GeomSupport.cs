using System;
using OpenTK;
using System.Collections.Generic;

// Support math code.
namespace MathSupport
{
  /// <summary>
  /// Geometric support.
  /// </summary>
  public class Geometry
  {
    public static bool IsZero ( double a )
    {
      return( a <= Double.Epsilon && a >= -Double.Epsilon );
    }

    public static void SpecularReflection ( ref Vector3d normal, ref Vector3d input, out Vector3d output )
    {
      double k;
      Vector3d.Dot( ref normal, ref input, out k );
      output = (k + k) * normal - input;
    }

    public static Vector3d SpecularRefraction ( Vector3d normal, double n, Vector3d input )
    {
      normal.Normalize();
      input.Normalize();
      double d = Vector3d.Dot( normal, input );

      if ( d < 0.0 )                        // (N*L) should be > 0.0 (N and L in the same half-space)
      {
        d  = -d;
        normal = -normal;
      }
      else
        n  = 1.0 / n;

      double cos2 = 1.0 - n * n * (1.0 - d * d);
      if ( cos2 <= 0.0 ) return Vector3d.Zero; // total reflection

      d = n * d - Math.Sqrt( cos2 );
      return( normal * d - input * n );
    }

    /// <summary>
    /// Finds two other axes to the given vector, their vector product will
    /// give the original vector, all three vectors will be perpendicular to each other.
    /// </summary>
    /// <param name="p">Original non-zero vector.</param>
    /// <param name="p1">First axis perpendicular to p.</param>
    /// <param name="p2">Second axis perpendicular to p.</param>
    public static void GetAxes ( ref Vector3d p, out Vector3d p1, out Vector3d p2 )
    {
      double ax = Math.Abs( p.X );
      double ay = Math.Abs( p.Y );
      double az = Math.Abs( p.Z );

      if ( ax >= az &&
           ay >= az )                       // ax, ay are dominant
      {
        p1.X = -p.Y;
        p1.Y =  p.X;
        p1.Z =  0.0;
      }
      else
      if ( ax >= ay &&
           az >= ay )                       // ax, az are dominant
      {
        p1.X = -p.Z;
        p1.Y =  0.0;
        p1.Z =  p.X;
      }
      else                                  // ay, az are dominant
      {
        p1.X =  0.0;
        p1.Y = -p.Z;
        p1.Z =  p.Y;
      }

      Vector3d.Cross( ref p, ref p1, out p2 );
    }

    /// <summary>
    /// Random direction around the [0,0,1] vector.
    /// Normal distribution is used, using required variance.
    /// </summary>
    /// <param name="rnd">Random generator to be used.</param>
    /// <param name="variance">Variance of the deviation angle in radians.</param>
    /// <returns></returns>
    public static Vector3d RandomDirectionNormal ( RandomJames rnd, double variance )
    {
      // result: [0,0,1] * rotX( deviation ) * rotZ( orientation )

      double deviation = rnd.Normal( 0.0, variance );
      double orientation = rnd.RandomDouble( 0.0, Math.PI );
      Matrix4d mat = Matrix4d.CreateRotationX( deviation ) * Matrix4d.CreateRotationZ( orientation );

      return new Vector3d( mat.Row2 ); // [0,0,1] * mat
    }

    /// <summary>
    /// Random direction around the givent center vector.
    /// Normal distribution is used, using required variance.
    /// </summary>
    /// <param name="rnd">Random generator to be used.</param>
    /// <param name="dir">Central direction.</param>
    /// <param name="variance">Variance of the deviation angle in radians.</param>
    /// <returns></returns>
    public static Vector3d RandomDirectionNormal ( RandomJames rnd, Vector3d dir, double variance )
    {
      // Matrix3d fromz: [0,0,1] -> dir
      // Vector4d delta: [0,0,1] * rotX( deviation ) * rotZ( orientation )
      // result: delta * fromz

      dir.Normalize();
      Vector3d axis1, axis2;
      GetAxes( ref dir, out axis1, out axis2 );
      Matrix4d fromz = new Matrix4d( new Vector4d( axis1 ), new Vector4d( axis2 ), new Vector4d( dir ), Vector4d.UnitW );
      //fromz.Transpose();
      double deviation = rnd.Normal( 0.0, variance );
      double orientation = rnd.RandomDouble( 0.0, Math.PI );
      Matrix4d mat = Matrix4d.CreateRotationX( deviation ) * Matrix4d.CreateRotationZ( orientation ) * fromz;
      
      return new Vector3d( mat.Row2 ); // [0,0,1] * mat
    }

    /// <summary>
    /// Ray vs. AABB intersection, direction vector in regular form,
    /// box defined by lower-left corner and size.
    /// </summary>
    /// <param name="result">Parameter (t) bounds: [min, max].</param>
    /// <returns>True if intersections exist.</returns>
    public static bool RayBoxIntersection ( ref Vector3d p0, ref Vector3d p1, ref Vector3d ul, ref Vector3d size, out Vector2d result )
    {
      result.X =
      result.Y = -1.0;
      double tMin = Double.NegativeInfinity;
      double tMax = Double.PositiveInfinity;
      double t1, t2, mul;

      // X axis:
      if ( IsZero( p1.X ) )
      {
        if ( p0.X <= ul.X ||
             p0.X >= ul.X + size.X )
          return false;
      }
      else
      {
        mul = 1.0 / p1.X;
        t1 = (ul.X - p0.X) * mul;
        t2 = t1 + size.X * mul;

        if ( mul > 0.0 )
        {
          if ( t1 > tMin ) tMin = t1;
          if ( t2 < tMax ) tMax = t2;
        }
        else
        {
          if ( t2 > tMin ) tMin = t2;
          if ( t1 < tMax ) tMax = t1;
        }

        if ( tMin > tMax ) return false;
      }

      // Y axis:
      if ( IsZero( p1.Y ) )
      {
        if ( p0.Y <= ul.Y ||
             p0.Y >= ul.Y + size.Y )
          return false;
      }
      else
      {
        mul = 1.0 / p1.Y;
        t1 = (ul.Y - p0.Y) * mul;
        t2 = t1 + size.Y * mul;

        if ( mul > 0.0 )
        {
          if ( t1 > tMin ) tMin = t1;
          if ( t2 < tMax ) tMax = t2;
        }
        else
        {
          if ( t2 > tMin ) tMin = t2;
          if ( t1 < tMax ) tMax = t1;
        }

        if ( tMin > tMax ) return false;
      }

      // Z axis:
      if ( IsZero( p1.Z ) )
      {
        if ( p0.Z <= ul.Z ||
             p0.Z >= ul.Z + size.Z )
          return false;
      }
      else
      {
        mul = 1.0 / p1.Z;
        t1 = (ul.Z - p0.Z) * mul;
        t2 = t1 + size.Z * mul;

        if ( mul > 0.0 )
        {
          if ( t1 > tMin ) tMin = t1;
          if ( t2 < tMax ) tMax = t2;
        }
        else
        {
          if ( t2 > tMin ) tMin = t2;
          if ( t1 < tMax ) tMax = t1;
        }

        if ( tMin > tMax ) return false;
      }

      result.X = tMin;
      result.Y = tMax;
      return true;
    }

    /// <summary>
    /// Ray vs. AABB intersection, direction vector in inverted form,
    /// box defined by lower-left corner and size.
    /// </summary>
    /// <param name="result">Parameter (t) bounds: [min, max].</param>
    /// <returns>True if intersections exist.</returns>
    public static bool RayBoxIntersectionInv ( ref Vector3d p0, ref Vector3d p1inv, ref Vector3d ul, ref Vector3d size, out Vector2d result )
    {
      result.X =
      result.Y = -1.0;
      double tMin = Double.NegativeInfinity;
      double tMax = Double.PositiveInfinity;
      double t1, t2;

      // X axis:
      if ( Double.IsInfinity( p1inv.X ) )
      {
        if ( p0.X <= ul.X ||
             p0.X >= ul.X + size.X )
          return false;
      }
      else
      {
        t1 = (ul.X - p0.X) * p1inv.X;
        t2 = t1 + size.X * p1inv.X;

        if ( p1inv.X > 0.0 )
        {
          if ( t1 > tMin ) tMin = t1;
          if ( t2 < tMax ) tMax = t2;
        }
        else
        {
          if ( t2 > tMin ) tMin = t2;
          if ( t1 < tMax ) tMax = t1;
        }

        if ( tMin > tMax )
          return false;
      }

      // Y axis:
      if ( Double.IsInfinity( p1inv.Y ) )
      {
        if ( p0.Y <= ul.Y ||
             p0.Y >= ul.Y + size.Y )
          return false;
      }
      else
      {
        t1 = (ul.Y - p0.Y) * p1inv.Y;
        t2 = t1 + size.Y * p1inv.Y;

        if ( p1inv.Y > 0.0 )
        {
          if ( t1 > tMin ) tMin = t1;
          if ( t2 < tMax ) tMax = t2;
        }
        else
        {
          if ( t2 > tMin ) tMin = t2;
          if ( t1 < tMax ) tMax = t1;
        }

        if ( tMin > tMax )
          return false;
      }

      // Z axis:
      if ( Double.IsInfinity( p1inv.Z ) )
      {
        if ( p0.Z <= ul.Z ||
             p0.Z >= ul.Z + size.Z )
          return false;
      }
      else
      {
        t1 = (ul.Z - p0.Z) * p1inv.Z;
        t2 = t1 + size.Z * p1inv.Z;

        if ( p1inv.Z > 0.0 )
        {
          if ( t1 > tMin ) tMin = t1;
          if ( t2 < tMax ) tMax = t2;
        }
        else
        {
          if ( t2 > tMin ) tMin = t2;
          if ( t1 < tMax ) tMax = t1;
        }

        if ( tMin > tMax )
          return false;
      }

      result.X = tMin;
      result.Y = tMax;
      return true;
    }

    /// <summary>
    /// Ray vs. AABB intersection, direction vector in inverted form,
    /// box defined by lower-left corner and size.
    /// </summary>
    /// <param name="result">Parameter (t) bounds: [min, max].</param>
    /// <returns>True if intersections exist.</returns>
    public static bool RayBoxIntersectionInv ( ref Vector3d p0, ref Vector3d p1inv, ref Vector3 ul, ref Vector3 size, out Vector2d result )
    {
      result.X =
      result.Y = -1.0;
      double tMin = Double.NegativeInfinity;
      double tMax = Double.PositiveInfinity;
      double t1, t2;

      // X axis:
      if ( Double.IsInfinity( p1inv.X ) )
      {
        if ( p0.X <= ul.X ||
             p0.X >= ul.X + size.X )
          return false;
      }
      else
      {
        t1 = (ul.X - p0.X) * p1inv.X;
        t2 = t1 + size.X * p1inv.X;

        if ( p1inv.X > 0.0 )
        {
          if ( t1 > tMin ) tMin = t1;
          if ( t2 < tMax ) tMax = t2;
        }
        else
        {
          if ( t2 > tMin ) tMin = t2;
          if ( t1 < tMax ) tMax = t1;
        }

        if ( tMin > tMax )
          return false;
      }

      // Y axis:
      if ( Double.IsInfinity( p1inv.Y ) )
      {
        if ( p0.Y <= ul.Y ||
             p0.Y >= ul.Y + size.Y )
          return false;
      }
      else
      {
        t1 = (ul.Y - p0.Y) * p1inv.Y;
        t2 = t1 + size.Y * p1inv.Y;

        if ( p1inv.Y > 0.0 )
        {
          if ( t1 > tMin ) tMin = t1;
          if ( t2 < tMax ) tMax = t2;
        }
        else
        {
          if ( t2 > tMin ) tMin = t2;
          if ( t1 < tMax ) tMax = t1;
        }

        if ( tMin > tMax )
          return false;
      }

      // Z axis:
      if ( Double.IsInfinity( p1inv.Z ) )
      {
        if ( p0.Z <= ul.Z ||
             p0.Z >= ul.Z + size.Z )
          return false;
      }
      else
      {
        t1 = (ul.Z - p0.Z) * p1inv.Z;
        t2 = t1 + size.Z * p1inv.Z;

        if ( p1inv.Z > 0.0 )
        {
          if ( t1 > tMin ) tMin = t1;
          if ( t2 < tMax ) tMax = t2;
        }
        else
        {
          if ( t2 > tMin ) tMin = t2;
          if ( t1 < tMax ) tMax = t1;
        }

        if ( tMin > tMax )
          return false;
      }

      result.X = tMin;
      result.Y = tMax;
      return true;
    }

    /// <summary>
    /// Ray-triangle intersection test in 3D.
    /// According to Tomas Moller, <a href="http://www.acm.org/jgt/">JGT</a>.
    /// (origin + t * direction = (1 - u - v) * a + u * b + v * c)
    /// </summary>
    /// <param name="p0">Ray origin.</param>
    /// <param name="p1">Ray direction vector.</param>
    /// <param name="a">Vertex A of the triangle.</param>
    /// <param name="b">Vertex B of the triangle.</param>
    /// <param name="c">Vertex C of the triangle.</param>
    /// <param name="uv">Barycentric coordinates of the intersection.</param>
    /// <returns>Parametric coordinate on the ray if succeeded, Double.NegativeInfinity otherwise.</returns>
    public static double RayTriangleIntersection ( ref Vector3d p0, ref Vector3d p1,
                                                   ref Vector3d a, ref Vector3d b, ref Vector3d c,
                                                   out Vector2d uv )
    {
      Vector3d e1 = b - a;
      Vector3d e2 = c - a;
      Vector3d pvec;
      Vector3d.Cross( ref p1, ref e2, out pvec );
      double det;
      Vector3d.Dot( ref e1, ref pvec, out det );
      uv.X = uv.Y = 0.0;
      if ( IsZero( det ) ) return Double.NegativeInfinity;

      double detInv = 1.0 / det;
      Vector3d tvec = p0 - a;
      Vector3d.Dot( ref tvec, ref pvec, out uv.X );
      uv.X *= detInv;
      if ( uv.X < 0.0 || uv.X > 1.0 ) return Double.NegativeInfinity;

      Vector3d qvec;
      Vector3d.Cross( ref tvec, ref e1, out qvec );
      Vector3d.Dot( ref p1, ref qvec, out uv.Y );
      uv.Y *= detInv;
      if ( uv.Y < 0.0 || uv.X + uv.Y > 1.0 ) return Double.NegativeInfinity;

      Vector3d.Dot( ref e2, ref qvec, out det );
      return( detInv * det );
    }

    /// <summary>
    /// Ray-triangle intersection test in 3D.
    /// According to Tomas Moller, <a href="http://www.acm.org/jgt/">JGT</a>.
    /// (origin + t * direction = (1 - u - v) * a + u * b + v * c)
    /// </summary>
    /// <param name="p0">Ray origin.</param>
    /// <param name="p1">Ray direction vector.</param>
    /// <param name="a">Vertex A of the triangle.</param>
    /// <param name="b">Vertex B of the triangle.</param>
    /// <param name="c">Vertex C of the triangle.</param>
    /// <param name="uv">Barycentric coordinates of the intersection.</param>
    /// <returns>Parametric coordinate on the ray if succeeded, Double.NegativeInfinity otherwise.</returns>
    public static double RayTriangleIntersection ( ref Vector3d p0, ref Vector3d p1,
                                                   ref Vector3 a, ref Vector3 b, ref Vector3 c,
                                                   out Vector2d uv )
    {
      Vector3d e1 = (Vector3d)b - (Vector3d)a;
      Vector3d e2 = (Vector3d)c - (Vector3d)a;
      Vector3d pvec;
      Vector3d.Cross( ref p1, ref e2, out pvec );
      double det;
      Vector3d.Dot( ref e1, ref pvec, out det );
      uv.X = uv.Y = 0.0;
      if ( IsZero( det ) ) return Double.NegativeInfinity;

      double detInv = 1.0 / det;
      Vector3d tvec = p0 - (Vector3d)a;
      Vector3d.Dot( ref tvec, ref pvec, out uv.X );
      uv.X *= detInv;
      if ( uv.X < 0.0 || uv.X > 1.0 ) return Double.NegativeInfinity;

      Vector3d qvec;
      Vector3d.Cross( ref tvec, ref e1, out qvec );
      Vector3d.Dot( ref p1, ref qvec, out uv.Y );
      uv.Y *= detInv;
      if ( uv.Y < 0.0 || uv.X + uv.Y > 1.0 ) return Double.NegativeInfinity;

      Vector3d.Dot( ref e2, ref qvec, out det );
      return( detInv * det );
    }
  }
}
