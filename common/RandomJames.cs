using System;
using OpenTK;

// Support math code.
namespace MathSupport
{
  /// <summary>
  /// Random number generator by F. James adopted by Phil Linttell, James F. Hickling & Josef Pelikan.
  ///
  /// This random number generator originally appeared in "Toward a Universal
  /// Random Number Generator" by George Marsaglia and Arif Zaman.
  /// Florida State University Report: FSU-SCRI-87-50 (1987)
  /// https://www.researchgate.net/publication/23636505_Toward_a_Universal_Random_Number_Generator
  /// 
  /// It was later modified by F. James and published in "A Review of Pseudo-random Number Generators"
  ///
  /// Converted from FORTRAN to C by Phil Linttell, James F. Hickling
  /// Management Consultants Ltd, Aug. 14, 1989.
  ///
  /// This is one of the best known random number generators available (However, a newly discovered
  /// technique can yield a period of 10^600. But that is still in the development stage.)
  ///
  /// It passes ALL of the tests for random number generators and has a period of 2^144, is completely
  /// portable (gives bit identical results on all machines with at least 24-bit mantissas in the
  /// floating point representation).
  ///
  /// The algorithm is a combination of a Fibonacci sequence (with lags of 97 and 33, and operation
  /// "subtraction plus one, modulo one") and an "arithmetic sequence" (using subtraction).
  ///
  /// On a Vax 11/780, this random number generator can produce a number in 13 microseconds.
  ///
  /// Ported to Java (2004) and C# (2012) by Josef Pelikan (http://cgg.mff.cuni.cz/~pepca/)
  /// </summary>
  public class RandomJames
  {
    //==========================================================================
    //  Data:

    protected double[] u;

    protected double c, cd, cm;

    protected int i97, j97;

    //==========================================================================
    //  Public members:

    /// <summary>
    /// Success flag.
    /// </summary>
    public bool Ok;

    /// <summary>
    /// Initializing constructor.
    /// </summary>
    public RandomJames (long ijkl)
    {
      u = new double[97];
      Reset(ijkl);
    }

    /// <summary>
    /// Initializing constructor.
    /// </summary>
    public RandomJames (int ij, int kl)
    {
      u = new double[97];
      Reset(ij, kl);
    }

    /// <summary>
    /// Default constructor ( ij = 1802, kl = 9373 ).
    /// </summary>
    public RandomJames ()
    {
      u = new double[97];
      Reset(1802, 9373);
    }

    /// <summary>
    /// Deterministic restart of a sequence.
    /// </summary>
    /// <param name="ijkl">Random seed modulo-converted to ij, kl.</param>
    public void Reset (long ijkl)
    {
      int s = (int) ( ijkl % ( 31329L * 30082L ) );
      Reset(s / 30082, s % 30082);
    }

    /// <summary>
    /// Deterministic restart of a sequence.
    /// </summary>
    /// <param name="ij">Random seed #1 [0,31328]</param>
    /// <param name="kl">Random seed #2 [0,30081]</param>
    public void Reset (int ij, int kl)
    {
      double s,  t;
      int    i,  j, k, l, m;
      int    ii, jj;

      ij = Arith.Clamp(ij, 0, 31328);
      kl = Arith.Clamp(kl, 0, 30081);
      Ok = true;

      i = ((ij / 177) % 177) + 2;
      j = (ij % 177) + 2;
      k = ((kl / 169) % 178) + 1;
      l = (kl % 169);

      for (ii = 0; ii < 97; ii++)
      {
        s = 0.0;
        t = 0.5;
        for (jj = 0; jj++ < 24;)
        {
          m = ((i * j % 179) * k) % 179;
          i = j;
          j = k;
          k = m;
          l = (53 * l + 1) % 169;
          if (l * m % 64 >= 32)
            s += t;
          t *= 0.5;
        }

        u[ii] = s;
      }

      c  = 362436.0 / 16777216.0;
      cd = 7654321.0 / 16777216.0;
      cm = 16777213.0 / 16777216.0;

      i97 = 96;
      j97 = 32;
    }

    /// <summary>
    /// Undeterministic restart of a sequence (uses system time).
    /// </summary>
    public long Randomize ()
    {
      DateTime now = DateTime.UtcNow;
      int      s   = now.Second;
      int      m   = now.Minute;
      int      h   = now.Hour;
      int      d   = now.DayOfYear;

      double maxs_sig   = 60.0 + 60.0 / 60.0 + 24.0 / 60.0 / 60.0 + 366.0 / 24.0 / 60.0 / 60.0;
      double maxs_insig = 60.0 + 60.0 * 60.0 + 24.0 * 60.0 * 60.0 + 366.0 * 24.0 * 60.0 * 60.0;

      double s_sig   = s + m / 60.0 + h / 60.0 / 60.0 + d / 24.0 / 60.0 / 60.0;
      double s_insig = s + m * 60.0 + h * 60.0 * 60.0 + d * 24.0 * 60.0 * 60.0;

      int s1 = (int) ( s_sig / maxs_sig * 31328.0 );
      int s2 = (int) ( s_insig / maxs_insig * 30081.0 );

      Reset(s1, s2);

      return s1 * 30082L + s2;
    }

    /// <summary>
    /// Validation test of a generator algorithm.
    /// </summary>
    /// <returns>True if everything went allright.</returns>
    public bool Validate ()
    {
      Reset(1802, 9373);
      if (!Ok)
        return false;

      double[] temp = new double[1000];
      int      i;
      for (i = 0; i++ < 20;)
      {
        UniformNumbers(temp);
        if (!Ok)
          return false;
      }

      UniformNumbers(temp, 0, 6);
      for (i = 0; i < 6; i++)
        temp[i] = temp[i] * 4096.0 * 4096.0 + 0.5;

      return (int)temp[0] == 6533892 &&
             (int)temp[1] == 14220222 &&
             (int)temp[2] == 7275067 &&
             (int)temp[3] == 6172232 &&
             (int)temp[4] == 8354498 &&
             (int)temp[5] == 10633180;
    }

    /// <summary>
    /// Uniform random number generator from [0,1).
    /// Not synchronized, use lock() if you need mt-safety.
    /// </summary>
    /// <returns>Number from [0,1).</returns>
    public double UniformNumber ()
    {
      if (!Ok) return 0.0;

      double uni           = u [i97] - u [j97];
      if (uni < 0.0) uni  += 1.0;
      u [i97] = uni;
      if (--i97 < 0) i97         =  96;
      if (--j97 < 0) j97         =  96;
      if ((c -= cd) < 0.0) c    += cm;
      if ((uni -= c) < 0.0) uni += 1.0;

      return uni;
    }

    /// <summary>
    /// Vector of uniform random numbers (from [0,1)).
    /// </summary>
    /// <param name="vec"></param>
    public void UniformNumbers (double[] vec)
    {
      UniformNumbers(vec, 0, vec.Length);
    }

    /// <summary>
    /// Vector of uniform random numbers (from [0,1)).
    /// </summary>
    /// <param name="vec">Array to write to.</param>
    /// <param name="from">Starting index.</param>
    /// <param name="number">Segment size.</param>
    public void UniformNumbers (double[] vec, int from, int number)
    {
      if (vec == null || from >= vec.Length)
        return;

      number += from;
      if (number > vec.Length)
        number = vec.Length;
      while (from < number)
        vec[from++] = UniformNumber();
    }

    /// <summary>
    /// Support type for generating random permutations.
    /// </summary>
    public class Permutation
    {
      public int[] perm;
      public int   permPtr;
      public int   permSize;
    }

    /// <summary>
    /// Random permutation setup.
    /// </summary>
    /// <param name="size">Permutation size.</param>
    /// <param name="perm">Temporary object assigned to this permutation generation.</param>
    /// <returns>The 1st item ("0" to "size-1").</returns>
    public int PermutationFirst (int size, ref Permutation perm)
    {
      if (size < 1)
        return -1;

      if (perm == null)
        perm = new Permutation();
      perm.permSize = size;
      if (perm.perm == null ||
           perm.perm.Length < size)
        perm.perm = new int[size];

      int i;
      for (i = 0; i < size - 1; i++)
        perm.perm[i] = i + 1;
      perm.perm[i] = 0;
      perm.permPtr = 0;

      return PermutationNext(ref perm);
    }

    /// <summary>
    /// Computes next permutation item.
    /// </summary>
    /// <param name="perm">Temporary object assigned to this permutation generation.</param>
    /// <returns>Next random item or -1 at the end.</returns>
    public int PermutationNext (ref Permutation perm)
    {
      if (perm == null || perm.permSize <= 0)
        return -1;

      int steps = (int) ( UniformNumber () * perm.permSize );
      while (steps-- > 0)
        perm.permPtr = perm.perm[perm.permPtr];

      int result = perm.perm [ perm.permPtr ];
      perm.perm[perm.permPtr] = perm.perm[result];
      perm.permSize--;

      return result;
    }

    /// <summary>
    /// Generates random integer number from the given range.
    /// </summary>
    /// <param name="min">Lower bound (included).</param>
    /// <param name="max">Upper bound (included).</param>
    /// <returns>Random integer.</returns>
    public int RandomInteger (int min, int max)
    {
      if (min >= max)
        return min;

      return (int)(min + Math.Floor((max + 1.0 - min) * UniformNumber()));
    }

    /// <summary>
    /// Generates random double number from the given range.
    /// </summary>
    public double RandomDouble (double min, double max)
    {
      if (min >= max)
        return min;

      return min + (max - min) * UniformNumber();
    }

    /// <summary>
    /// Generates random float number from the given range.
    /// </summary>
    public float RandomFloat (float min, float max)
    {
      if (min >= max)
        return min;

      return min + (float)((max - min) * UniformNumber());
    }

    protected bool normalReady = false;

    protected double normalVal = 0.0;

    /// <summary>
    /// Generates one sample from normal distribution N(mu,sigma^2).
    /// Using the polar form of Box-Muller transform (https://en.wikipedia.org/wiki/Box%E2%80%93Muller_transform)
    /// </summary>
    /// <param name="mu">Mean value.</param>
    /// <param name="sigma">Square root of the variance.</param>
    public double Normal (double mu, double sigma)
    {
      double val;
      if (normalReady = !normalReady)
      {
        double u, v, s;
        do
        {
          u = RandomDouble(-1.0, 1.0);
          v = RandomDouble(-1.0, 1.0);
          s = u * u + v * v;
        } while (s < 2.0 * double.Epsilon ||
                 s >= 1.0);

        s = Math.Sqrt(-2.0 * Math.Log(s) / s);
        val = u * s;
        normalVal = v * s;
      }
      else
        val = normalVal;

      return mu + sigma * val;
    }

    /// <summary>
    /// Generates random point in the given triangle.
    /// Works in either dimension (2D, 3D).
    /// </summary>
    /// <param name="a">Triangle vertex A.</param>
    /// <param name="b">Triangle vertex B.</param>
    /// <param name="c">Triangle vertex C.</param>
    /// <param name="result">Random 3D point.</param>
    public void RandomPointFromTriangle (Vector3d a, Vector3d b, Vector3d c, out Vector3d result)
    {
      double u = UniformNumber ();
      double v = UniformNumber ();
      if (u + v > 1.0)
      {
        u = 1.0 - u;
        v = 1.0 - v;
      }

      result = a + u * (b - a) + v * (c - a);
    }

    /// <summary>
    /// Uniformly generates random unit vector from the given latitude-defined strip.
    /// <br />To generate arbitrary direction, use UniformDirection( -1.0, 1.0, result ),
    /// for direction from the upper hemisphere, use UniformDirection( 0.0, 1.0, result ).
    /// </summary>
    /// <param name="minLat">Sine of the minimum latitude.</param>
    /// <param name="maxLat">Sine of the maximum latitude.</param>
    /// <param name="result">Random 3D (unit) vector.</param>
    public void UniformDirection (double minLat, double maxLat, out Vector3d result)
    {
      if (minLat < -1.0) minLat = -1.0;
      if (maxLat > 1.0)  maxLat = 1.0;
      double sinLat             = RandomDouble ( minLat, maxLat );
      double cosLat             = Math.Sqrt ( 1.0 - sinLat * sinLat );
      double longitude          = UniformNumber () * 2.0 * Math.PI;

      result.X = Math.Cos(longitude) * cosLat;
      result.Y = Math.Sin(longitude) * cosLat;
      result.Z = sinLat;
    }
  }
}
