using OpenTK;
using Rendering;
using System;

namespace JosefPelikan
{
  /// <summary>
  /// Property-animator using Catmull-Rom spline with uniform set of control knots.
  /// </summary>
  [Serializable]
  public class CatmullRomAnimator : PropertyAnimator
  {
    /// <summary>
    /// Can be inncluded in both general-animator and property-based animator chains.
    /// </summary>
    public CatmullRomAnimator (
      in ITimeDependent nxtGen = null,
      in ITimeDependentProperty nxtProp = null)
      : base(nxtGen, nxtProp)
    {}

    /// <summary>
    /// Clone the object, share the data.
    /// </summary>
    public override object Clone ()
    {
      ITimeDependent nxtGen = (ITimeDependent)nextGeneral?.Clone();
      ITimeDependentProperty nxtProp = (ITimeDependentProperty)nextProperty?.Clone();

      CatmullRomAnimator a = new CatmullRomAnimator(nxtGen, nxtProp)
      {
        properties = properties,
        Start      = Start,
        End        = End,
        Time       = Time
      };

      return a;
    }

    protected static void CatmullRomWeights (
      double fraction,
      out double c0,
      out double c1,
      out double c2,
      out double c3)
    {
      double thalf  = 0.5    * fraction;
      double t2half = thalf  * fraction;
      double t3half = t2half * fraction;
             c0     =       -t3half + 2.0 * t2half - thalf;
             c1     =  3.0 * t3half - 5.0 * t2half + 1.0;
             c2     = -3.0 * t3half + 4.0 * t2half + thalf;
             c3     =        t3half       - t2half;
    }

    /// <summary>
    /// Catmull-Rom interpolation of scalar double.
    /// </summary>
    public override bool TryGetValue (in string name, ref double d)
    {
      if (properties.TryGetValue(name, out object op) &&
          op is Property<double> p)
      {
        if (p.data == null)
          return false;

        // Compute the current value from 'p'.
        p.prepareCubic(
          time,
          out double fraction,
          out int i0, out int i1, out int i2, out int i3);

        // Use [fraction, i0,. i1, i2, i3] to interpolate the quantity.
        CatmullRomWeights(fraction, out double c0, out double c1, out double c2, out double c3);
        d = c0 * p.data[i0] +
            c1 * p.data[i1] +
            c2 * p.data[i2] +
            c3 * p.data[i3];

        return true;
      }

      // Connected property can have the same name.
      return base.TryGetValue(name, ref d);
    }

    /// <summary>
    /// Catmull-Rom interpolation of 3D vectors.
    /// </summary>
    public override bool TryGetValue (in string name, ref Vector3d v3)
    {
      if (properties.TryGetValue(name, out object op) &&
          op is Property<Vector3d> p)
      {
        if (p.data == null)
          return false;

        // Compute the current value from 'p'.
        p.prepareCubic(
          time,
          out double fraction,
          out int i0, out int i1, out int i2, out int i3);

        // Use [fraction, i0,. i1, i2, i3] to interpolate the quantity.
        CatmullRomWeights(fraction, out double c0, out double c1, out double c2, out double c3);
        v3 = c0 * p.data[i0] +
             c1 * p.data[i1] +
             c2 * p.data[i2] +
             c3 * p.data[i3];

        return true;
      }

      // Connected property can have the same name.
      return base.TryGetValue(name, ref v3);
    }

    /// <summary>
    /// Catmull-Rom interpolation of quaternions.
    /// </summary>
    public override bool TryGetValue (in string name, ref Quaterniond q)
    {
      if (properties.TryGetValue(name, out object op) &&
          op is Property<Quaterniond> p)
      {
        if (p.data == null)
          return false;

        // Compute the current value from 'p'.
        p.prepareCubic(
          time,
          out double fraction,
          out int i0, out int i1, out int i2, out int i3);

        // Using a method from a Grapnics Gems II chapter:
        // John Schlag: USING GEOMETRIC CONSTRUCTIONS TO INTERPOLATE ORIENTATION WITH QUATERNIONS,
        // Graphics Gems II, Morgan Kaufmann, 1991, Pages 377 - 380, ISBN 9780080507545,
        // https://doi.org/10.1016/B978-0-08-050754-5.50081-5

        // Use [fraction, i0,. i1, i2, i3] to interpolate the quantity.
        Quaterniond q10 = Quaterniond.Slerp(p.data[i0], p.data[i1], fraction + 1.0);
        Quaterniond q11 = Quaterniond.Slerp(p.data[i1], p.data[i2], fraction);
        Quaterniond q12 = Quaterniond.Slerp(p.data[i2], p.data[i3], fraction - 1.0);
        Quaterniond q20 = Quaterniond.Slerp(q10, q11, 0.5 * (fraction + 1.0));
        Quaterniond q21 = Quaterniond.Slerp(q11, q12, 0.5 * fraction);
        q = Quaterniond.Slerp(q20, q21, fraction);

        return true;
      }

      // Connected property can have the same name.
      return base.TryGetValue(name, ref q);
    }

    /// <summary>
    /// Catmull-Rom interpolation on arrays of numeric objects.
    /// </summary>
    public override object GetValue (in string name)
    {
      if (properties.TryGetValue(name, out object op))
      {
        int dim, i;

        // Array of doubles.
        if (op is Property<double[]> pad)
        {
          if (pad.data == null ||
              pad.data.Count == 0 ||
              (dim = pad.data[0].Length) == 0)
            return false;

          // Item: double[dim].
          pad.prepareCubic(
            time,
            out double fraction,
            out int i0, out int i1, out int i2, out int i3);

          double[] d0 = pad.data[i0];
          double[] d1 = pad.data[i1];
          double[] d2 = pad.data[i1];
          double[] d3 = pad.data[i2];
          CatmullRomWeights(fraction, out double c0, out double c1, out double c2, out double c3);

          double[] result = new double[dim];
          for (i = 0; i < dim; i++)
            result[i] = c0 * d0[i] + c1 * d1[i] + c2 * d2[i] + c3 * d3[i];

          return result;
        }

        // Array of Vector3d.
        if (op is Property<Vector3d[]> pav)
        {
          if (pav.data == null ||
              pav.data.Count == 0 ||
              (dim = pav.data[0].Length) == 0)
            return false;

          // Item: double[dim].
          pav.prepareCubic(
            time,
            out double fraction,
            out int i0, out int i1, out int i2, out int i3);

          Vector3d[] d0 = pav.data[i0];
          Vector3d[] d1 = pav.data[i1];
          Vector3d[] d2 = pav.data[i1];
          Vector3d[] d3 = pav.data[i2];
          CatmullRomWeights(fraction, out double c0, out double c1, out double c2, out double c3);

          Vector3d[] result = new Vector3d[dim];
          for (i = 0; i < dim; i++)
            result[i] = c0 * d0[i] + c1 * d1[i] + c2 * d2[i] + c3 * d3[i];

          return result;
        }

        // Array of Quaterniond.
        if (op is Property<Quaterniond[]> paq)
        {
          if (paq.data == null ||
              paq.data.Count == 0 ||
              (dim = paq.data[0].Length) == 0)
            return false;

          // Item: double[dim].
          paq.prepareCubic(
            time,
            out double fraction,
            out int i0, out int i1, out int i2, out int i3);

          // Using a method from a Grapnics Gems II chapter:
          // John Schlag: USING GEOMETRIC CONSTRUCTIONS TO INTERPOLATE ORIENTATION WITH QUATERNIONS,
          // Graphics Gems II, Morgan Kaufmann, 1991, Pages 377 - 380, ISBN 9780080507545,
          // https://doi.org/10.1016/B978-0-08-050754-5.50081-5

          Quaterniond[] d0 = paq.data[i0];
          Quaterniond[] d1 = paq.data[i1];
          Quaterniond[] d2 = paq.data[i1];
          Quaterniond[] d3 = paq.data[i2];

          Quaterniond[] result = new Quaterniond[dim];
          for (i = 0; i < dim; i++)
          {
            Quaterniond q10 = Quaterniond.Slerp(d0[i], d1[i], fraction + 1.0);
            Quaterniond q11 = Quaterniond.Slerp(d1[i], d2[i], fraction);
            Quaterniond q12 = Quaterniond.Slerp(d2[i], d3[i], fraction - 1.0);
            Quaterniond q20 = Quaterniond.Slerp(q10, q11, 0.5 * (fraction + 1.0));
            Quaterniond q21 = Quaterniond.Slerp(q11, q12, 0.5 * fraction);
            result[i] = Quaterniond.Slerp(q20, q21, fraction);
          }

          return result;
        }
      }

      return nextProperty?.GetValue(name);
    }
  }
}
