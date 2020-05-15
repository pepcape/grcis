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
        double thalf  = 0.5    * fraction;
        double t2half = thalf  * fraction;
        double t3half = t2half * fraction;
        double c0 =       -t3half + 2.0 * t2half - thalf;
        double c1 =  3.0 * t3half - 5.0 * t2half + 1.0;
        double c2 = -3.0 * t3half + 4.0 * t2half + thalf;
        double c3 =        t3half       - t2half;
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
        double thalf  = 0.5    * fraction;
        double t2half = thalf  * fraction;
        double t3half = t2half * fraction;
        double c0 =       -t3half + 2.0 * t2half - thalf;
        double c1 =  3.0 * t3half - 5.0 * t2half + 1.0;
        double c2 = -3.0 * t3half + 4.0 * t2half + thalf;
        double c3 =        t3half       - t2half;
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
  }
}
