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
        v3 = (1.0 - fraction) * p.data[i1] + fraction * p.data[i2];

        return true;
      }

      return false;
    }
  }
}
