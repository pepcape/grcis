using System;
using OpenTK;

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
    /// <param name="p"></param>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    public static void GetAxes ( Vector3d p, out Vector3d p1, out Vector3d p2 )
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
