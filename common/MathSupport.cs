using System;
using OpenTK;

// Support math code.
namespace MathSupport
{
  /// <summary>
  /// Various arithmetics/calculus.
  /// </summary>
  public class Arith
  {
    public static T Clamp<T> ( T val, T min, T max ) where T : IComparable<T>
    {
      if ( val.CompareTo( min ) < 0 ) return min;
      if ( val.CompareTo( max ) > 0 ) return max;
      return val;
    }
  }

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
        normal = normal * -1.0;
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

    public static bool RayBoxIntersection ( Vector3d p0, Vector3d p1, Vector3d ul, Vector3d size, out Vector2d result )
    {
      result.X =
      result.Y = -1.0;
      double tMin = Double.NegativeInfinity;
      double tMax = Double.PositiveInfinity;
      double t1, t2, mul;

      // X axis:
      if ( IsZero( p1.X ) )
      {
        if ( p0.X < ul.X ||
             p0.X > ul.X + size.X )
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

        if ( tMax < 0.0 ||
             tMin > tMax ) return false;
      }

      // Y axis:
      if ( IsZero( p1.Y ) )
      {
        if ( p0.Y < ul.Y ||
             p0.Y > ul.Y + size.Y )
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

        if ( tMax < 0.0 ||
             tMin > tMax ) return false;
      }

      // Z axis:
      if ( IsZero( p1.Z ) )
      {
        if ( p0.Z < ul.Z ||
             p0.Z > ul.Z + size.Z )
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

        if ( tMax < 0.0 ||
             tMin > tMax ) return false;
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
    public static double RayTriangleIntersection ( Vector3d p0, Vector3d p1,
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
  }

  /// <summary>
  /// Simple static pseudo-random generators.
  /// Mostly LCG (Linear Congruential Generators).
  /// </summary>
  public class RandomStatic
  {
    protected const long BITS_32  = 0xffffffffL;
    protected const long BITS_31  = 0x7fffffffL;

    /// <summary>
    /// LCG pseudo-random generator from "Numeric Recipes in C".
    /// </summary>
    /// <param name="v">Random seed.</param>
    /// <returns>The next pseudo-random number in the sequence.</returns>
    public static long numericRecipes ( long v )
    {
      return( (v * 1664525L + 1013904223L) & BITS_32 );
    }

    public static long numericRecipesMax ()
    {
      return BITS_32 + 1L;
    }

    /// <summary>
    /// LCG pseudo-random generator: IBM randu.
    /// Bad distribution in 3D space!
    /// </summary>
    /// <param name="v">Random seed.</param>
    /// <returns>The next pseudo-random number in the sequence.</returns>
    public static long ibmRandu ( long v )
    {
      return( (v * 65539L) & BITS_31 );
    }

    public static long ibmRanduMax ()
    {
      return BITS_31 + 1L;
    }

    /// <summary>
    /// LCG pseudo-random generator: minimal standard LCG.
    ///
    /// Proposed in Stephen K. Park and Keith W. Miller:
    /// Random Number Generators: Good Ones Are Hard To Find,
    /// Communications of the ACM, 31(10):1192-1201, 1988.
    /// </summary>
    /// <param name="v">Random seed.</param>
    /// <returns>The next pseudo-random number in the sequence.</returns>
    public static long parkMiller ( long v )
    {
      return( (v * 16807L) % BITS_31 );
    }

    public static long parkMillerMax ()
    {
      return BITS_31;
    }

    /// <summary>
    /// LCG pseudo-random generator from Maple.
    /// </summary>
    /// <param name="v">Random seed.</param>
    /// <returns>The next pseudo-random number in the sequence.</returns>
    public static long maple ( long v )
    {
      return( (v * 427419669081L) % 999999999989L );
    }

    public static long mapleMax ()
    {
      return 999999999989L;
    }
  }
}
