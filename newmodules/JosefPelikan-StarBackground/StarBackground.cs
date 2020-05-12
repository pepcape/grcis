using MathSupport;
using OpenTK;
using Rendering;
using System;
using Utilities;

namespace JosefPelikan
{
  public class StarBackground : DefaultBackground
  {
    double probability;
    int resolution;
    double[] reference;
    ulong min, max;
    static readonly ulong rangeMax2 = RandomStatic.numericRecipesMax() / 2 - 1u;

    public StarBackground (
      in double[] c,
      in int res = 2000,
      in double p = 0.01) :
      base()
    {
      resolution = Util.Clamp(res, 10, 100000);
      reference = c;
      probability = Util.Clamp(p, 0.0, 1.0);
      ulong half = Math.Min(rangeMax2, (ulong)(probability * rangeMax2));
      min = rangeMax2 - half;
      max = rangeMax2 + half;
    }

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
                RandomStatic.numericRecipes(yi * 6229u);
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
        double sizeRec = 50.0 / Math.Max(2, (b & 0xf00) >> 8);
        // sizeRec = 50 / <2 to 15>
        x -= xi + 0.5;    // sub-star sampling.
        y -= yi + 0.5;
        double coeff = 1.6 * Math.Exp(-(x * x + y * y) * sizeRec * sizeRec);
        double R = coeff * (0.5 + ((b & 0b00001110) >> 1) / 14.0);
        double G = coeff * (0.5 + ((b & 0b00111000) >> 3) / 14.0);
        double B = coeff * (0.5 + ((b & 0b11100000) >> 5) / 14.0);
        Util.ColorCopy(new double[] {R + c[0], G + c[1], B + c[2]}, color);
        return (long)(10000000.0 * coeff);
      }

      Util.ColorCopy(c, color);
      return 1L;
    }
  }
}
