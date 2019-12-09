using System;
using System.Collections.Generic;
using System.Drawing;
using System.Diagnostics;

// Support math code.
namespace MathSupport
{
  /// <summary>
  /// Various arithmetics/calculus.
  /// </summary>
  public class Arith
  {
    public static T Clamp<T> (T val, T min, T max) where T : IComparable<T>
    {
      if (val.CompareTo(min) < 0) return min;
      if (val.CompareTo(max) > 0) return max;
      return val;
    }

    public static double Pow (double a, int e)
    {
      if (e < 0)
      {
        e = -e;
        a = 1.0 / a;
      }

      double acc = (( e & 1 ) != 0) ? a : 1.0;
      while ((e >>= 1) != 0)
      {
        a *= a;
        if ((e & 1) != 0)
          acc *= a;
      }

      return acc;
    }

    public static int Mod (int a, int b)
    {
      return ((a < 0) ? ((a + 1) % b + b - 1) : (a % b));
    }

    public static double DegreeToRadian (double deg)
    {
      return (deg * Math.PI / 180.0);
    }

    public static double RadianToDegree (double rad)
    {
      return (rad * 180.0 / Math.PI);
    }

    public static double GCD (double a, double b)
    {
      long longA;
      long longB;

      double mult = 1.0;
      double mmult = 2.0;
      while (true)
      {
        longA = (long)a;
        longB = (long)b;

        if (longA == a &&
            longB == b)
          break;

        a *= mmult;
        b *= mmult;
        if (a > long.MaxValue ||
            b > long.MaxValue)
          break;

        mult *= mmult;
        mmult += 1.0;
      }

      return GCD(longA, longB) / mult;
    }

    public static long GCD (long a, long b)
    {
      void SwapAB ()
      {
        long temp = a;
        a = b;
        b = temp;
      }

      if (b > a)
        SwapAB();

      while (b != 0L)
      {
        a %= b;
        SwapAB();
      }

      return a;
    }


    /// <summary>
    /// Converts .NET Color type (24-bit true color) to triple HSV.
    /// </summary>
    /// <param name="color">Input color, [0, 255]^3</param>
    /// <param name="hue">Hue from range [0.0, 360.0), in degrees.</param>
    /// <param name="saturation">Saturation from range [0.0, 1.0]. 1.0 means pure saturated color.</param>
    /// <param name="value">Value from range [0.0, 1.0].</param>
    public static void ColorToHSV (
      Color color,
      out double hue, out double saturation, out double value)
    {
      int max = Math.Max(color.R, Math.Max(color.G, color.B));
      int min = Math.Min(color.R, Math.Min(color.G, color.B));

      hue        = color.GetHue();
      saturation = (max == 0) ? 0.0 : 1.0 - min / (double)max;
      value      = max / 255.0;
    }

    /// <summary>
    /// Creates .NET Color object from the given HSV triple.
    /// </summary>
    /// <param name="hue">Hue from range [0.0, 360.0), in degrees.</param>
    /// <param name="saturation">Saturation from range [0.0, 1.0]. 1.0 means pure saturated color.</param>
    /// <param name="value">Value from range [0.0, 1.0].</param>
    public static Color HSVToColor (double hue, double saturation, double value)
    {
      int hi = Convert.ToInt32(Math.Floor(hue / 60.0));
      hi = (hi < 0) ? (hi % 6 + 6) % 6 : hi % 6;
      double f = hue / 60.0 - Math.Floor(hue / 60.0);

      value *= 255.0;
      int v = Clamp(Convert.ToInt32(value), 0, 255);
      int p = Clamp(Convert.ToInt32(value * (1.0 - saturation)), 0, 255);
      int q = Clamp(Convert.ToInt32(value * (1.0 - f * saturation)), 0, 255);
      int t = Clamp(Convert.ToInt32(value * (1.0 - (1.0 - f) * saturation)), 0, 255);

      switch (hi)
      {
        case 0:
          return Color.FromArgb(v, t, p);
        case 1:
          return Color.FromArgb(q, v, p);
        case 2:
          return Color.FromArgb(p, v, t);
        case 3:
          return Color.FromArgb(p, q, v);
        case 4:
          return Color.FromArgb(t, p, v);
        default:
          return Color.FromArgb(v, p, q);
      }
    }

    /// <summary>
    /// Converts a RGB color (double values) to the HSV representation (double values).
    /// </summary>
    /// <param name="R">Red color component (arbitrary non-negative value).</param>
    /// <param name="G">Green color component (arbitrary non-negative value).</param>
    /// <param name="B">Blue color component (arbitrary non-negative value).</param>
    /// <param name="hue">Hue from range [0.0, 360.0), in degrees.</param>
    /// <param name="saturation">Saturation from range [0.0, 1.0]. 1.0 means pure saturated color.</param>
    /// <param name="value">Value from the same range as input RGB values (value = max{R, G, B}).</param>
    public static void RGBtoHSV (
      double R, double G, double B,
      out double hue, out double saturation, out double value)
    {
      double max = Math.Max(R, Math.Max(G, B));
      double min = Math.Min(R, Math.Min(G, B));
      double delta = max - min;

      // Saturation and value are easy.
      saturation = (max <= 0.0) ? 0.0 : delta / max;
      value = max;

      // Hue.
      if (delta > 0.0)
      {
        if (R == max)
          hue = (G - B) / delta;
        else if (G == max)
          hue = 2.0 + (B - R) / delta;
        else
          hue = 4.0 + (R - G) / delta;

        // To degrees.
        hue *= 60.0;
        if (hue < 0.0)
          hue += 360.0;
        else if (hue >= 360.0)
          hue -= 360.0;
      }
      else
        hue = 0.0;
    }

    /// <summary>
    /// Converts a HSV color (double values) to the RGB representation (double values).
    /// </summary>
    /// <param name="hue">Hue from range [0.0, 360.0), in degrees.</param>
    /// <param name="saturation">Saturation from range [0.0, 1.0]. 1.0 means pure saturated color.</param>
    /// <param name="value">Value - arbitrary non-negative value.</param>
    /// <param name="R">Red color component from [0.0, value] range.</param>
    /// <param name="G">Green color component from [0.0, value] range.</param>
    /// <param name="B">Blue color component from [0.0, value] range.</param>
    public static void HSVToRGB (
      double hue, double saturation, double value,
      out double R, out double G, out double B)
    {
      int hi = Convert.ToInt32(Math.Floor(hue / 60.0));
      hi = (hi < 0) ? (hi % 6 + 6) % 6 : hi % 6;
      double f = hue / 60.0 - Math.Floor(hue / 60.0);

      double v = Math.Max(value, 0.0);
      double p = Math.Max(value * (1.0 - saturation), 0.0);
      double q = Math.Max(value * (1.0 - f * saturation), 0.0);
      double t = Math.Max(value * (1.0 - (1.0 - f) * saturation), 0.0);

      switch (hi)
      {
        case 0:
          R = v; G = t; B = p;
          break;
        case 1:
          R = q; G = v; B = p;
          break;
        case 2:
          R = p; G = v; B = t;
          break;
        case 3:
          R = p; G = q; B = v;
          break;
        case 4:
          R = t; G = p; B = v;
          break;
        default:
          R = v; G = p; B = q;
          break;
      }
    }

    /// <summary>
    /// Converts .NET Color type (24-bit RGB triple) into the CIE L*a*b* representation.
    /// See http://www.brucelindbloom.com for details and formulae.
    /// </summary>
    /// <param name="color">Input color, [0, 255]^3</param>
    /// <param name="L">L* component.</param>
    /// <param name="A">a* component.</param>
    /// <param name="B">b* component.</param>
    public static void ColorToCIELab (
      Color color,
      out double L, out double A, out double B)
    {
      // adopted from http://www.brucelindbloom.com

      double fx, fy, fz, xr, yr, zr;
      double eps = 216.0 / 24389.0;
      double k   = 24389.0 / 27.0;

      double Xr = 0.964221; // reference white D50
      double Yr = 1.0;
      double Zr = 0.825211;

      // RGB to XYZ
      double r = color.R / 255.0;
      double g = color.G / 255.0;
      double b = color.B / 255.0;

      // assuming sRGB (D65)
      if (r <= 0.04045)
        r = r / 12;
      else
        r = Math.Pow((r + 0.055) / 1.055, 2.4);

      if (g <= 0.04045)
        g = g / 12;
      else
        g = Math.Pow((g + 0.055) / 1.055, 2.4);

      if (b <= 0.04045)
        b = b / 12;
      else
        b = (float)Math.Pow((b + 0.055) / 1.055, 2.4);

      double X = 0.436052025 * r + 0.385081593 * g + 0.143087414 * b;
      double Y = 0.222491598 * r + 0.71688606  * g + 0.060621486 * b;
      double Z = 0.013929122 * r + 0.097097002 * g + 0.71418547  * b;

      // XYZ to L*a*b*
      xr = X / Xr;
      yr = Y / Yr;
      zr = Z / Zr;

      if (xr > eps)
        fx = Math.Pow(xr, 1.0 / 3.0);
      else
        fx = (k * xr + 16.0) / 116.0;

      if (yr > eps)
        fy = Math.Pow(yr, 1.0 / 3.0);
      else
        fy = (k * yr + 16.0) / 116.0;

      if (zr > eps)
        fz = Math.Pow(zr, 1.0 / 3.0);
      else
        fz = (k * zr + 16.0) / 116.0;

      L = (116.0 * fy) - 16.0;
      A = 500.0 * (fx - fy);
      B = 200.0 * (fy - fz);
    }

    /// <summary>
    /// Conversion from Radiance's RGBe 32-bit format into HDR floating-point RGB format. 
    /// </summary>
    public static void RGBeToRGB (byte[] rgbe, int startRgbe, float[] rgb, int startRgb)
    {
      if (rgbe == null || startRgbe + 4 > rgbe.Length ||
          rgb == null  || startRgb  + 3 > rgb.Length)
        return;

      if (rgbe[startRgbe + 3] == 0)
        rgb[startRgb] =
        rgb[startRgb + 1] =
        rgb[startRgb + 2] = 0.0f;
      else
      {
        double f = Pow(2.0, rgbe[startRgbe + 3] - 136);
        rgb[startRgb]     = (float)((rgbe[startRgbe    ] + 0.5) * f);
        rgb[startRgb + 1] = (float)((rgbe[startRgbe + 1] + 0.5) * f);
        rgb[startRgb + 2] = (float)((rgbe[startRgbe + 2] + 0.5) * f);
      }
    }

    /// <summary>
    /// Conversion from HDR floating-point RGB format to Radiance's RGBe 32-bit format.
    /// </summary>
    public static void RGBToRGBe (byte[] rgbe, int startRgbe, double R, double G, double B)
    {
      if (rgbe == null ||
          startRgbe + 4 > rgbe.Length)
        return;

      double m = Math.Max(Math.Max(R, G), B);

      if (m < 1.0e-32)
        rgbe[startRgbe] =
        rgbe[startRgbe + 1] =
        rgbe[startRgbe + 2] =
        rgbe[startRgbe + 3] = 0;
      else
      {
        int    exp = 128;
        double mul = 255.9999;
        while (m < 0.5)
        {
          mul *= 2.0;
          m *= 2.0;
          exp--;
        }

        while (m >= 1.0)
        {
          mul *= 0.5;
          m *= 0.5;
          exp++;
        }

        rgbe[startRgbe]     = (byte)(R * mul);
        rgbe[startRgbe + 1] = (byte)(G * mul);
        rgbe[startRgbe + 2] = (byte)(B * mul);
        rgbe[startRgbe + 3] = (byte)exp;
      }
    }

    /// <summary>
    /// Ray vs. line segment intersection in 2D.
    /// </summary>
    /// <param name="ox">Ray origin - x-coordinate.</param>
    /// <param name="oy">Ray origin - y-coordinate.</param>
    /// <param name="dx">Ray direction - x-coordinate.</param>
    /// <param name="dy">Ray direction - y-coordinate.</param>
    /// <param name="ax">A endpoint - x-coordinate.</param>
    /// <param name="ay">A endpoint - y-coordinate.</param>
    /// <param name="bx">B endpoint - x-coordinate.</param>
    /// <param name="by">B endpoint - y-coordinate.</param>
    /// <returns>Parameter coordinate of the intersection or double.NegativeInfinity if none exists.</returns>
    public static double RaySegment2D (double ox, double oy, double dx, double dy,
                                       double ax, double ay, double bx, double by)
    {
      double nx  = ay - by;
      double ny  = bx - ax;
      double den = nx * dx + ny * dy;
      if (den > -2.0 * double.Epsilon &&
          den <  2.0 * double.Epsilon)
        return double.NegativeInfinity;

      double t = (nx * (ax - ox) + ny * (ay - oy)) / den;
      double resol;

      if (Math.Abs(ny) > Math.Abs(nx))
      {
        // use X coordinate
        resol = ox + t * dx;
        if (resol < Math.Min(ax, bx) ||
            resol > Math.Max(ax, bx))
          return double.NegativeInfinity;
      }
      else
      {
        // use Y coordinate
        resol = oy + t * dy;
        if (resol < Math.Min(ay, by) ||
            resol > Math.Max(ay, by))
          return double.NegativeInfinity;
      }

      return t;
    }
  }


  /// <summary>
  /// Data class keeping info about current progress of a computation.
  /// </summary>
  [Serializable]
  public class Progress
  {
    /// <summary>
    /// Relative amount of work finished so far (0.0f to 1.0f).
    /// </summary>
    public float Finished { get; set; }

    /// <summary>
    /// Optional message. Any string.
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// Continue in an associated computation.
    /// </summary>
    public bool Continue { get; set; }

    /// <summary>
    /// Sync interval in milliseconds.
    /// </summary>
    public long SyncInterval { get; set; }

    /// <summary>
    /// Any message from computing unit to the GUI main.
    /// </summary>
    public virtual void Sync ( object msg ) { }

    /// <summary>
    /// Set all the harmless values.
    /// </summary>
    public Progress ()
    {
      Finished     = 0.0f;
      Message      = "";
      Continue     = true;
      SyncInterval = 8000L;
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
    protected int n;

    protected double A;
    protected double B;
    protected double C;
    protected double D;
    protected double E;

    public Polynomial ()
    {
      n = 0;
      A = B = C = D = E = 0.0;
    }

    public Polynomial (Polynomial p)
    {
      n = p.n;
      A = p.A;
      B = p.B;
      C = p.C;
      D = p.D;
      E = p.E;
    }

    public Polynomial (double a, double b, double c, double d, double e)
    {
      Set(a, b, c, d, e);
    }

    public Polynomial (double a, double b, double c, double d)
    {
      Set(a, b, c, d);
    }

    public Polynomial (double a, double b, double c)
    {
      Set(a, b, c);
    }

    public static Polynomial NullPolynomial ()
    {
      return new Polynomial();
    }

    public void Set (double a, double b, double c, double d, double e)
    {
      n = 4;
      A = a;
      B = b;
      C = c;
      D = d;
      E = e;
    }

    public void Set (double a, double b, double c, double d)
    {
      n = 3;
      A = a;
      B = b;
      C = c;
      D = d;
    }

    public void Set (double a, double b, double c)
    {
      n = 2;
      A = a;
      B = b;
      C = c;
    }

    public static Polynomial operator * (Polynomial u, double e)
    {
      // Identity
      if (e == 1.0)
        return u;
      // Null polynomial
      if (e == 0.0)
        return NullPolynomial();

      Polynomial r = new Polynomial(u);
      r.A *= e;
      r.B *= e;
      r.C *= e;
      r.D *= e;
      r.E *= e;
      return r;
    }

    public static Polynomial operator * (double a, Polynomial p)
    {
      return p * a;
    }

    public static Polynomial operator / (Polynomial p, double a)
    {
      return p * (1.0 / a);
    }

    /// <summary>
    /// Solve quadratic polynomial.
    /// </summary>
    /// <param name="y">Sorted real roots.</param>
    /// <returns>Number of real roots.</returns>
    public int SolveQuadratic (double[] y)
    {
      double d, t;
      if (C == 0.0)
      {
        if (B == 0.0)
          return 0;
        y[0] = y[1] = -A / B;
        return 1;
      }

      d = B * B - 4.0 * C * A;
      if (d < 0.0)
        return 0;
      if (Math.Abs(d) < COEFF_LIMIT)
      {
        y[0] = y[1] = -0.5 * B / A;
        return 1;
      }

      d = Math.Sqrt(d);
      t = 0.5 / A;
      if (t > 0.0)
      {
        y[0] = (-B - d) * t;
        y[1] = (-B + d) * t;
      }
      else
      {
        y[0] = (-B + d) * t;
        y[1] = (-B - d) * t;
      }

      return 2;
    }

    /// <summary>
    /// Solve cubic polynomial.
    /// </summary>
    /// <param name="y">Sorted real roots.</param>
    /// <returns>Number of real roots.</returns>
    public int SolveCubic (double[] y)
    {
      double Q,  R,  Q3, R2, sQ, d, an, theta;
      double A2, a1, a2, a3;

      if (Math.Abs(D) < EPSILON)
        return SolveQuadratic(y);

      if (D != 1.0)
      {
        a1 = C / D;
        a2 = B / D;
        a3 = A / D;
      }
      else
      {
        a1 = C;
        a2 = B;
        a3 = A;
      }

      A2 = a1 * a1;

      Q = (A2 - 3.0 * a2) / 9.0;

      R = (a1 * (A2 - 4.5 * a2) + 13.5 * a3) / 27.0;

      Q3 = Q * Q * Q;

      R2 = R * R;

      d = Q3 - R2;

      an = a1 / 3.0;

      if (d >= 0.0)
      {
        // Three real roots

        d = R / Math.Sqrt(Q3);

        theta = Math.Acos(d) / 3.0;

        sQ = -2.0 * Math.Sqrt(Q);

        y[0] = sQ * Math.Cos(theta) - an;
        y[1] = sQ * Math.Cos(theta + PI_2_3) - an;
        y[2] = sQ * Math.Cos(theta + PI_4_3) - an;

        return 3;
      }

      sQ = Math.Pow(Math.Sqrt(R2 - Q3) + Math.Abs(R), 1.0 / 3.0);

      if (R < 0)
        y[0] =  (sQ + Q / sQ) - an;
      else
        y[0] = -(sQ + Q / sQ) - an;

      return 1;
    }

    /// <summary>
    /// Solve quartic polynomial.
    /// We are using the method of Francois Vieta (Circa 1735).
    /// </summary>
    /// <param name="results">Sorted real roots.</param>
    /// <returns>Number of real roots.</returns>
    public int SolveQuartic (double[] results)
    {
      double[] roots = new double[3];
      double   c12, z,  p,  q, q1, q2, r, d1, d2;
      double   c1,  c2, c3, c4;
      int      i;

      // See if the higher order term has vanished
      if (Math.Abs(E) < COEFF_LIMIT)
        return SolveCubic(results);

      // See if the constant term has vanished
      if (Math.Abs(A) < COEFF_LIMIT)
      {
        Polynomial y = new Polynomial(E, D, C, B);
        return y.SolveCubic(results);
        // !!! and what happened to a zero root?
      }

      // Make sure the quartic has a leading coefficient = 1.0
      if (E != 1.0)
      {
        c1 = D / E;
        c2 = C / E;
        c3 = B / E;
        c4 = A / E;
      }
      else
      {
        c1 = D;
        c2 = C;
        c3 = B;
        c4 = A;
      }

      // Compute the cubic resolvant
      c12 = c1 * c1;
      p   = -0.375 * c12 + c2;
      q   = 0.125 * c12 * c1 - 0.5 * c1 * c2 + c3;
      r   = -0.01171875 * c12 * c12 + 0.0625 * c12 * c2 - 0.25 * c1 * c3 + c4;

      Polynomial cubic = new Polynomial(0.5 * r * p - 0.125 * q * q, -r, -0.5 * p, 1.0);

      i = cubic.SolveCubic(roots);
      if (i > 0)
        z = roots[0];
      else
        return 0;

      d1 = 2.0 * z - p;

      if (d1 < 0.0)
      {
        if (d1 > -EPSILON)
          d1 = 0.0;
        else
          return 0;
      }

      if (d1 < EPSILON)
      {
        d2 = z * z - r;
        if (d2 < 0.0)
          return 0;
        d2 = Math.Sqrt(d2);
      }
      else
      {
        d1 = Math.Sqrt(d1);
        d2 = 0.5 * q / d1;
      }

      // Set up useful values for the quadratic factors
      q1 = d1 * d1;
      q2 = -0.25 * c1;
      i  = 0;

      // Solve the first quadratic
      p = q1 - 4.0 * (z - d2);
      if (p == 0)
      {
        results[i++] = -0.5 * d1 - q2;
        results[i++] = -0.5 * d1 - q2;
      }
      else if (p > 0)
      {
        p            = Math.Sqrt(p);
        results[i++] = -0.5 * (d1 + p) + q2;
        results[i++] = -0.5 * (d1 - p) + q2;
      }

      // Solve the second quadratic
      p = q1 - 4.0 * (z + d2);
      if (p == 0)
      {
        results[i++] = 0.5 * d1 - q2;
        results[i++] = 0.5 * d1 - q2;
      }
      else if (p > 0)
      {
        p            = Math.Sqrt(p);
        results[i++] = 0.5 * (d1 + p) + q2;
        results[i++] = 0.5 * (d1 - p) + q2;
      }

      return i;
    }
  }

  /// <summary>
  /// Simple static pseudo-random generators.
  /// Mostly LCG (Linear Congruential Generators).
  /// </summary>
  public class RandomStatic
  {
    protected const long BITS_32 = 0xffffffffL;
    protected const long BITS_31 = 0x7fffffffL;

    /// <summary>
    /// LCG pseudo-random generator from "Numeric Recipes in C".
    /// </summary>
    /// <param name="v">Random seed.</param>
    /// <returns>The next pseudo-random number in the sequence.</returns>
    public static long numericRecipes (long v)
    {
      return ((v * 1664525L + 1013904223L) & BITS_32);
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
    public static long ibmRandu (long v)
    {
      return ((v * 65539L) & BITS_31);
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
    public static long parkMiller (long v)
    {
      return ((v * 16807L) % BITS_31);
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
    public static long maple (long v)
    {
      return ((v * 427419669081L) % 999999999989L);
    }

    public static long mapleMax ()
    {
      return 999999999989L;
    }
  }

  /// <summary>
  /// Generic Heap data structure with fast GetMin(), RemoveMin() and Add() operations.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public class HeapMin<T> where T : IComparable<T>
  {
    protected List<T> arr;

    public int Count
    {
      get { return arr.Count; }
    }

    public HeapMin ()
    {
      arr = new List<T> ();
    }

    public HeapMin (ICollection<T> c)
      : this()
    {
      foreach (T i in c)
        Add(i);
    }

    public void Add (T i)
    {
      arr.Add(i);
      if (arr.Count > 1)
        CheckUp(arr.Count - 1);
    }

    public T GetMin ()
    {
      if (arr.Count > 0)
        return arr[0];
      else
        return default(T);
    }

    public T RemoveMin ()
    {
      if (arr.Count == 0)
        return default(T);
      T min = arr [ 0 ];
      arr[0] = arr[arr.Count - 1];
      arr.RemoveAt(arr.Count - 1);
      if (arr.Count > 1)
        CheckDown(0);
      return min;
    }

    protected void Swap (int i, int j)
    {
      T ti = arr [ i ];
      arr[i] = arr[j];
      arr[j] = ti;
    }

    protected void CheckDown (int i)
    {
      do
      {
        int s = i + i + 1;
        if (s >= arr.Count)
          return;

        if (s + 1 < arr.Count &&
            arr[s].CompareTo(arr[s + 1]) > 0)
          s++;

        if (arr[i].CompareTo(arr[s]) <= 0)
          return;

        Swap(i, s);
        i = s;
      } while (true);
    }

    protected void CheckUp (int i)
    {
      while (i > 0)
      {
        int p = ( i - 1 ) / 2;
        if (arr[p].CompareTo(arr[i]) <= 0)
          return;

        Swap(p, i);
        i = p;
      }
    }
  }

  /// <summary>
  /// Generic Heap data structure with fast GetTop(), RemoveTop(), SwapTop() and Add() operations.
  /// </summary>
  /// <typeparam name="T">Object type to be stored in the heap.</typeparam>
  public class HeapTop<T>: IEnumerable<T>
  {
    protected List<T> arr;

    /// <summary>
    /// Top = minimum (according to this comparer).
    /// </summary>
    public IComparer<T> Comp { get; set; }

    public T this[int index]
    {
      get => arr[index];
      set => arr[index] = value;
    }

    public IEnumerator<T> GetEnumerator ()
    {
      return arr.GetEnumerator();
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()
    {
      return GetEnumerator();
    }

    public int Count => arr.Count;

    public HeapTop (IComparer<T> co = null)
    {
      arr  = new List<T>();
      Comp = co;
    }

    public HeapTop (ICollection<T> c, IComparer<T> co = null)
      : this(co)
    {
      foreach (T i in c)
        Add(i);
    }

    public void Add (T t)
    {
      arr.Add(t);
      if (arr.Count > 1)
        CheckUp(arr.Count - 1);
    }

    public T GetTop ()
    {
      if (arr.Count > 0)
        return arr[0];
      else
        return default;
    }

    public T RemoveTop ()
    {
      if (arr.Count == 0)
        return default;

      T top = arr [ 0 ];
      arr[0] = arr[arr.Count - 1];
      arr.RemoveAt(arr.Count - 1);
      if (arr.Count > 1)
        CheckDown(0);

      return top;
    }

    public T SwapTop (T t)
    {
      if (arr.Count == 0)
      {
        Add(t);
        return default;
      }

      T top = arr [ 0 ];
      arr[0] = t;
      if (arr.Count > 1)
        CheckDown(0);

      return top;
    }

    public void Clear ()
    {
      arr.Clear();
    }

    public void Sort ()
    {
      if (arr.Count < 2)
        return;

      for (int i = 1; i < arr.Count; i++)
        CheckUp(i);
    }

    protected void Swap (int i, int j)
    {
      T ti = arr [ i ];
      arr[i] = arr[j];
      arr[j] = ti;
    }

    public void CheckDown (int i)
    {
      Debug.Assert(Comp != null);
      do
      {
        int s = i + i + 1;
        if (s >= arr.Count)
          return;

        if (s + 1 < arr.Count &&
            Comp.Compare(arr[s], arr[s + 1]) > 0)
          s++;

        if (Comp.Compare(arr[i], arr[s]) <= 0)
          return;

        Swap(i, s);
        i = s;
      } while (true);
    }

    protected void CheckUp (int i)
    {
      Debug.Assert(Comp != null);

      while (i > 0)
      {
        int p = ( i - 1 ) / 2;
        if (Comp.Compare(arr[p], arr[i]) <= 0)
          return;

        Swap(p, i);
        i = p;
      }
    }
  }

  /// <summary>
  /// Integer implementation of the Summed-area table (a.k.a. Integral Image)
  /// http://en.wikipedia.org/wiki/Summed_area_table
  /// </summary>
  public class SummedAreaTableLong
  {
    public long[,] table = null;

    public SummedAreaTableLong ()
    {}

    public SummedAreaTableLong (int xres, int yres)
    {
      table = new long[xres, yres];
    }

    public void ComputeTable ()
    {
      Debug.Assert(table != null);

      int xres = table.GetLength ( 0 );
      int yres = table.GetLength ( 1 );
      int x, y;

      // 1st row:
      for (y = 1; y < yres; y++)
        table[0, y] += table[0, y - 1];

      // rest of the rows:
      for (x = 1; x < xres; x++)
      {
        table[x, 0] += table[x - 1, 0];
        for (y = 1; y < yres; y++)
          table[x, y] += table[x - 1, y] + table[x, y - 1] - table[x - 1, y - 1];
      }
    }

    public long Sum (int x1, int x2, int y1, int y2)
    {
      Debug.Assert(x1 >= 0  && x1 < table.GetLength(0));
      Debug.Assert(x2 >= x1 && x2 < table.GetLength(0));
      Debug.Assert(y1 >= 0  && y1 < table.GetLength(1));
      Debug.Assert(y2 >= y1 && y2 < table.GetLength(1));

      x1--;
      y1--;
      long A = ( x1 < 0 || y1 < 0 ) ? 0L : table [ x1, y1 ];
      long B = ( y1 < 0 ) ? 0L : table [ x2, y1 ];
      long C = ( x1 < 0 ) ? 0L : table [ x1, y2 ];
      long D = table [ x2, y2 ];
      return D - B - C + A;
    }
  }

  /// <summary>
  /// Integer implementation of the Summed-area table (a.k.a. Integral Image)
  /// http://en.wikipedia.org/wiki/Summed_area_table
  /// </summary>
  public class SummedAreaTableInt
  {
    public int[,] table = null;

    public SummedAreaTableInt ()
    {}

    public SummedAreaTableInt (int xres, int yres)
    {
      table = new int[xres, yres];
    }

    public void ComputeTable ()
    {
      Debug.Assert(table != null);

      int xres = table.GetLength ( 0 );
      int yres = table.GetLength ( 1 );
      int x, y;

      // 1st row:
      for (y = 1; y < yres; y++)
        table[0, y] += table[0, y - 1];

      // rest of the rows:
      for (x = 1; x < xres; x++)
      {
        table[x, 0] += table[x - 1, 0];
        for (y = 1; y < yres; y++)
          table[x, y] += table[x - 1, y] + table[x, y - 1] - table[x - 1, y - 1];
      }
    }

    public int Sum (int x1, int x2, int y1, int y2)
    {
      Debug.Assert(x1 >= 0  && x1 < table.GetLength(0));
      Debug.Assert(x2 >= x1 && x2 < table.GetLength(0));
      Debug.Assert(y1 >= 0  && y1 < table.GetLength(1));
      Debug.Assert(y2 >= y1 && y2 < table.GetLength(1));

      x1--;
      y1--;
      int A = ( x1 < 0 || y1 < 0 ) ? 0 : table [ x1, y1 ];
      int B = ( y1 < 0 ) ? 0 : table [ x2, y1 ];
      int C = ( x1 < 0 ) ? 0 : table [ x1, y2 ];
      int D = table [ x2, y2 ];
      return D - B - C + A;
    }
  }

  /// <summary>
  /// Mitchell-style generator for 2D sample sets.
  /// Naive implementation (no spatial search structures are used).
  /// </summary>
  class Mitchell2D
  {
    /// <summary>
    /// Sample generator in 2D.
    /// </summary>
    public delegate PointF SampleGenerator ();

    /// <summary>
    /// Set of already accepted samples.
    /// </summary>
    public List<PointF> samples = null;

    /// <summary>
    /// Quality-controlling constant. The algorithm is too slow if set higher than 5.
    /// </summary>
    public int K = 5;

    /// <summary>
    /// Current generator of new samples.
    /// </summary>
    public SampleGenerator Generator = null;

    public Mitchell2D (SampleGenerator g, int k = 5)
    {
      Generator = g;
      K         = Math.Max(1, k);

      samples   = new List<PointF>();
    }

    public void Reset (SampleGenerator g = null, int k = 0)
    {
      if (g != null)
        Generator = g;
      if (k > 0)
        K = k;

      samples.Clear();
    }

    /// <summary>
    /// Generates at least n samples.
    /// </summary>
    /// <returns>Distance of the last sample or 0.0f.</returns>
    public float Generate (int n)
    {
      int i = samples.Count;

      float lastMaxDist = 0.0f;

      while (i < n)
      {
        int ns = i * K;

        float  maxDist   = 0.0f;
        PointF maxSample = PointF.Empty;
        for (int j = 0; j++ < ns;)
        {
          PointF c       = Generator ();
          float  minDist = float.MaxValue;
          foreach (var sample in samples)
          {
            float dx   = sample.X - c.X;
            float dy   = sample.Y - c.Y;
            float dist = dx * dx + dy * dy;
            if (dist < minDist)
              minDist = dist;
          }

          if (minDist > maxDist)
          {
            maxDist = minDist;
            maxSample = c;
          }
        }

        samples.Add(maxSample);
        i++;
        lastMaxDist = 0.9f * lastMaxDist + 0.1f * maxDist;
      }

      return (float)Math.Sqrt(lastMaxDist);
    }
  }
}
