using MathSupport;
using OpenTK;
using Rendering;
using System;
using Utilities;

namespace JosefPelikan
{
  [Serializable]
  public class CatmullRomAnimator : PropertyAnimator
  {
    public CatmullRomAnimator ()
    {
    }

    /// <summary>
    /// Clone the object, share the data.
    /// </summary>
    public override object Clone ()
    {
      CatmullRomAnimator a = new CatmullRomAnimator
      {
        properties = properties,
        Start      = Start,
        End        = End,
        Time       = Time
      };

      return a;
    }

    /// <summary>
    /// Catmull-Rom interpolation of 3D vector.
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

        // Use [fraction, i0,. i1, i2, i3] for interpolating the result value.
        double thalf  = 0.5    * fraction;
        double t2half = thalf  * fraction;
        double t3half = t2half * fraction;
        double c0 =       -t3half + 2.0 * t2half - thalf;
        double c1 =  3.0 * t3half - 5.0 * t2half + 1.0;
        double c2 = -3.0 * t3half + 4.0 * t2half + thalf;
        double c3 =        t3half       - t2half;
        v3 = c0 * p.data[i0] + c1 * p.data[i1] + c2 * p.data[i2] + c3 * p.data[i3];

        return true;
      }

      return false;
    }
  }
}
