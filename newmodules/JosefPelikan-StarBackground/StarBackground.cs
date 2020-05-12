using MathSupport;
using OpenTK;
using Rendering;
using System;
using Utilities;

namespace JosefPelikan
{
  [Serializable]
  public class StarBackground : DefaultBackground
  {
    /// <summary>
    /// Probability of a star appearing in a cell.
    /// </summary>
    double probability;

    /// <summary>
    /// Rectanguler grid resolution (for one cube side).
    /// </summary>
    int resolution;

    /// <summary>
    /// Color range - from 0.0 (full-color) to 1.0 (monochromatic).
    /// </summary>
    double colorRange;

    /// <summary>
    /// Star intensity (maximal value in the middle of the star).
    /// </summary>
    double intensity = 1.4;

    /// <summary>
    /// Relative star size.
    /// </summary>
    double size = 1.0;

    /// <summary>
    /// Random seed. Different seed generates different star-field.
    /// But the generation process is deterministic.
    /// </summary>
    ulong seed = 0L;

    /// <summary>
    /// Default color.
    /// </summary>
    double[] reference;

    /// <summary>
    /// Pre-computed bounds for fast probability tests.
    /// </summary>
    ulong min, max;

    /// <summary>
    /// Cached half-range of hash function.
    /// </summary>
    static readonly ulong rangeMax2 = RandomStatic.numericRecipesMax() / 2 - 1u;

    /// <summary>
    /// Constructor with parameters.
    /// </summary>
    /// <param name="c">Default background color.</param>
    /// <param name="res">Grid resolution (affects star size).</param>
    /// <param name="p">Probability (affects star-field density).</param>
    /// <param name="color">Color coefficient (0.0 fo monochromatic, 1.0 for full color).</param>
    /// <param name="intens">Color intensity/amplitude, can be negative for negative effect.</param>
    /// <param name="siz">Star size coefficient (1.0 .. normal size).</param>
    /// <param name="rand">Random base.</param>
    public StarBackground (
      in double[] c,
      in int res = 2000,
      in double p = 0.01,
      in double color = 0.5,
      in double intens = 1.4,
      in double siz = 1.0,
      in ulong rand = 0L) :
      base()
    {
      resolution  = Util.Clamp(res, 10, 100000);
      reference   = c;
      probability = Util.Clamp(p, 0.0, 1.0);
      colorRange  = Util.Clamp(color, 0.0, 1.0);
      intensity   = Util.Clamp(intens, -10.0, 10.0);
      size        = Util.Clamp(siz, 0.01, 40.0);
      seed        = rand;

      ulong half = Math.Min(rangeMax2, (ulong)(probability * rangeMax2));
      min = rangeMax2 - half;
      max = rangeMax2 + half;
    }

    /// <summary>
    /// Returns a color for the given direction vector.
    /// </summary>
    public override long GetColor (Vector3d p1, double[] color)
    {
      uint plane;
      double absX = Math.Abs(p1.X);
      double absY = Math.Abs(p1.Y);
      double absZ = Math.Abs(p1.Z);
      double x, y;
      if (absX > absY)
        if (absX > absZ)
        {
          // Dominant X.
          plane = (p1.X > 0.0) ? 0u : 1u;
          x = p1.Y / absX;
          y = p1.Z / absX;
        }
        else
        {
          // Dominant Z.
          plane = (p1.Z > 0.0) ? 2u : 3u;
          x = p1.X / absZ;
          y = p1.Y / absZ;
        }
      else
        if (absY > absZ)
        {
          // Dominant Y.
          plane = (p1.Y > 0.0) ? 4u : 5u;
          x = p1.X / absY;
          y = p1.Z / absY;
        }
        else
        {
          // Dominant Z.
          plane = (p1.Z > 0.0) ? 2u : 3u;
          x = p1.X / absZ;
          y = p1.Y / absZ;
        }
      // Make 0 <= x, y <= 1.
      x = 0.5 * resolution * (x + 1.0);
      y = 0.5 * resolution * (y + 1.0);
      uint xi = (uint)x;
      uint yi = (uint)y;

      ulong b = RandomStatic.numericRecipes(xi * 2357u + 101u * plane) ^
                RandomStatic.numericRecipes(yi * 6229u) ^
                seed;
      b = RandomStatic.numericRecipes(b);
      b = RandomStatic.numericRecipes(b);

      // Sky.
      double[] c = reference;
      if (c == null &&
          scene != null &&
          scene.BackgroundColor != null &&
          scene.BackgroundColor.Length > 0)
        c = scene.BackgroundColor;

      if (b >= min && b <= max)
      {
        // Star.
        double sizeRec = 60.0 / (Math.Sqrt(size) * Math.Max(2, (b & 0xf00) >> 8));
        // sizeRec = 60 / (sqrt(size) * <2 to 15>)
        x -= xi + 0.5;    // sub-star sampling.
        y -= yi + 0.5;
        double coeff = intensity * Math.Exp(-(x * x + y * y) * sizeRec * sizeRec);
        double R = coeff * (1.0 + colorRange * (((b & 0b00001110) >> 1) / 7.0 - 1.0));
        double G = coeff * (1.0 + colorRange * (((b & 0b00111000) >> 3) / 7.0 - 1.0));
        double B = coeff * (1.0 + colorRange * (((b & 0b11100000) >> 5) / 7.0 - 1.0));
        Util.ColorCopy(new double[] {R + c[0], G + c[1], B + c[2]}, color);
        return (long)(10000000.0 * coeff);
      }

      Util.ColorCopy(c, color);
      return 1L;
    }
  }
}
