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
  /// Real polynomial up to the 4th degree.
  /// </summary>
  public class Polynomial
  {
    protected static double EPSILON = 1.0e-10;

    protected static double COEFF_LIMIT = 1.0e-16;

    protected static double PI_2_3 = 2.0943951023931954923084;

    protected static double PI_4_3 = 4.1887902047863909846168;

    /// <summary>
    /// Degree of the polynomial.
    /// </summary>
    protected int n = 0;

    protected double[] coef;

    public Polynomial ()
    {
      n = 0;
      coef = new double[] { 0.0, 0.0, 0.0, 0.0, 0.0 };
    }

    public Polynomial ( Polynomial p )
    {
      n = p.n;
      if ( p.coef != null )
        coef = (double[])p.coef.Clone();
    }

    public Polynomial ( double a, double b, double c, double d, double e )
    {
      Set( a, b, c, d, e );
    }

    public Polynomial ( double a, double b, double c, double d )
    {
      Set( a, b, c, d );
    }

    public Polynomial ( double a, double b, double c )
    {
      Set( a, b, c );
    }

    public static Polynomial NullPolynomial ()
    {
      return new Polynomial();
    }

    public void Set ( double a, double b, double c, double d, double e )
    {
      n = 4;
      coef = new double[] { a, b, c, d, e };
    }

    public void Set ( double a, double b, double c, double d )
    {
      n = 3;
      coef = new double[] { a, b, c, d };
    }

    public void Set ( double a, double b, double c )
    {
      n = 2;
      coef = new double[] { a, b, c };
    }

    /// <summary>
    /// Check the polynomial degree, decrease its degree if higher coeffs are zero.
    /// </summary>
    public void Check ()
    {
      while ( n > 0 )
      {
        if ( coef[ n ] != 0.0 )
          break;
        n--;
      }
    }

    public static Polynomial operator * ( Polynomial u, double e )
    {
      // Identity
      if ( e == 1.0 )
        return u;
      // Null polynomial
      if ( e == 0.0 )
        return NullPolynomial();

      Polynomial r = new Polynomial( u );
      for ( int i = 0; i < r.coef.Length; )
        r.coef[ i++ ] *= e;
      return r;
    }

    public static Polynomial operator + ( Polynomial u, Polynomial v )
    {
      Polynomial r = new Polynomial( u );
      // !!! TODO: add

      return r;
    }

    public static Polynomial operator - ( Polynomial u, Polynomial v )
    {
      Polynomial r = new Polynomial( u );
      // !!! TODO: subtract

      return r;
    }

    public static Polynomial operator * ( Polynomial u, Polynomial v )
    {
      Polynomial r = new Polynomial();

      r.n = u.n + v.n;

      for ( int i = 0; i <= u.n; i++ )
        for ( int j = 0; j <= v.n; j++ )
          r.coef[ i + j ] += u.coef[ i ] * v.coef[ j ];

      return r;
    }

    public static Polynomial operator * ( double a, Polynomial p )
    {
      return p * a;
    }

    public static Polynomial operator / ( Polynomial p, double a )
    {
      return p * (1.0 / a);
    }

    /// <summary>
    /// Solve quadratic polynomial.
    /// </summary>
    /// <param name="y">Sorted real roots.</param>
    /// <returns>Number of real roots.</returns>
    public int SolveQuadratic ( double[] y )
    {
      double d, t, a, b, c;
      a = coef[ 2 ];
      b = -coef[ 1 ];
      c = coef[ 0 ];
      if ( a == 0.0 )
      {
        if ( b == 0.0 )
          return 0;
        y[ 0 ] = y[ 1 ] = c / b;
        return 1;
      }
      d = b * b - 4.0 * a * c;
      if ( d < 0.0 )
        return 0;
      if ( Math.Abs( d ) < COEFF_LIMIT )
      {
        y[ 0 ] = y[ 1 ] = 0.5 * b / a;
        return 1;
      }
      d = Math.Sqrt( d );
      t = 0.5 / a;
      if ( t > 0.0 )
      {
        y[ 0 ] = (b - d) * t;
        y[ 1 ] = (b + d) * t;
      }
      else
      {
        y[ 0 ] = (b + d) * t;
        y[ 1 ] = (b - d) * t;
      }
      return 2;
    }

    /// <summary>
    /// Solve cubic polynomial.
    /// </summary>
    /// <param name="y">Sorted real roots.</param>
    /// <returns>Number of real roots.</returns>
    public int SolveCubic ( double[] y )
    {
      double Q, R, Q3, R2, sQ, d, an, theta;
      double A2, a1, a2, a3;

      double a0 = coef[ 3 ];

      if ( Math.Abs( a0 ) < EPSILON )
        return SolveQuadratic( y );

      if ( a0 != 1.0 )
      {
        a1 = coef[ 2 ] / a0;
        a2 = coef[ 1 ] / a0;
        a3 = coef[ 0 ] / a0;
      }
      else
      {
        a1 = coef[ 2 ];
        a2 = coef[ 1 ];
        a3 = coef[ 0 ];
      }

      A2 = a1 * a1;

      Q = (A2 - 3.0 * a2) / 9.0;

      R = (a1 * (A2 - 4.5 * a2) + 13.5 * a3) / 27.0;

      Q3 = Q * Q * Q;

      R2 = R * R;

      d = Q3 - R2;

      an = a1 / 3.0;

      if ( d >= 0.0 )
      {
        // Three real roots

        d = R / Math.Sqrt( Q3 );

        theta = Math.Acos( d ) / 3.0;

        sQ = -2.0 * Math.Sqrt( Q );

        y[ 0 ] = sQ * Math.Cos( theta ) - an;
        y[ 1 ] = sQ * Math.Cos( theta + PI_2_3 ) - an;
        y[ 2 ] = sQ * Math.Cos( theta + PI_4_3 ) - an;

        return 3;
      }

      sQ = Math.Pow( Math.Sqrt( R2 - Q3 ) + Math.Abs( R ), 1.0 / 3.0 );

      if ( R < 0 )
        y[ 0 ] = (sQ + Q / sQ) - an;
      else
        y[ 0 ] = -(sQ + Q / sQ) - an;

      return 1;
    }

    /// <summary>
    /// Solve quartic polynomial.
    /// We are using the method of Francois Vieta (Circa 1735).
    /// </summary>
    /// <param name="results">Sorted real roots.</param>
    /// <returns>Number of real roots.</returns>
    public int SolveQuartic ( double[] results )
    {
      double[] roots = new double[ 3 ];
      double c12, z, p, q, q1, q2, r, d1, d2;
      double c0, c1, c2, c3, c4;
      int i;

      // See if the higher order term has vanished 
      c0 = coef[ 4 ];
      if ( Math.Abs( c0 ) < COEFF_LIMIT )
        return SolveCubic( results );

      // See if the constant term has vanished 
      if ( Math.Abs( coef[ 0 ] ) < COEFF_LIMIT )
      {
        Polynomial y = new Polynomial( coef[ 4 ], coef[ 3 ], coef[ 2 ], coef[ 1 ] );
        return y.SolveCubic( results );
        // !!! and what happened to a zero root?
      }

      // Make sure the quartic has a leading coefficient of 1.0 
      if ( c0 != 1.0 )
      {
        c1 = coef[ 3 ] / c0;
        c2 = coef[ 2 ] / c0;
        c3 = coef[ 1 ] / c0;
        c4 = coef[ 0 ] / c0;
      }
      else
      {
        c1 = coef[ 3 ];
        c2 = coef[ 2 ];
        c3 = coef[ 1 ];
        c4 = coef[ 0 ];
      }

      // Compute the cubic resolvant 
      c12 = c1 * c1;
      p = -0.375 * c12 + c2;
      q = 0.125 * c12 * c1 - 0.5 * c1 * c2 + c3;
      r = -0.01171875 * c12 * c12 + 0.0625 * c12 * c2 - 0.25 * c1 * c3 + c4;

      Polynomial cubic = new Polynomial( 0.5 * r * p - 0.125 * q * q, -r, -0.5 * p, 1.0 );

      i = cubic.SolveCubic( roots );
      if ( i > 0 )
        z = roots[ 0 ];
      else
        return 0;

      d1 = 2.0 * z - p;

      if ( d1 < 0.0 )
      {
        if ( d1 > -EPSILON )
          d1 = 0.0;
        else
          return 0;
      }

      if ( d1 < EPSILON )
      {
        d2 = z * z - r;
        if ( d2 < 0.0 )
          return 0;
        d2 = Math.Sqrt( d2 );
      }
      else
      {
        d1 = Math.Sqrt( d1 );
        d2 = 0.5 * q / d1;
      }

      // Set up useful values for the quadratic factors 
      q1 = d1 * d1;
      q2 = -0.25 * c1;
      i = 0;

      // Solve the first quadratic
      p = q1 - 4.0 * (z - d2);
      if ( p == 0 )
        results[ i++ ] = -0.5 * d1 - q2;
      else if ( p > 0 )
      {
        p = Math.Sqrt( p );
        results[ i++ ] = -0.5 * (d1 + p) + q2;
        results[ i++ ] = -0.5 * (d1 - p) + q2;
      }

      // Solve the second quadratic 
      p = q1 - 4.0 * (z + d2);
      if ( p == 0 )
        results[ i++ ] = 0.5 * d1 - q2;
      else if ( p > 0 )
      {
        p = Math.Sqrt( p );
        results[ i++ ] = 0.5 * (d1 + p) + q2;
        results[ i++ ] = 0.5 * (d1 - p) + q2;
      }

      return i;
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
