using MathSupport;
using OpenTK;
using Rendering;
using System;
using Utilities;

namespace JosefPelikan
{
  [Serializable]
  public class AnimatedNodeTranslate : AnimatedCSGInnerNode
  {
    /// <summary>
    /// Property name (see 'PropertyAnimator').
    /// </summary>
    protected string name;

    /// <summary>
    /// Current translation vector.
    /// </summary>
    Vector3d translate;

    /// <summary>
    /// Original transform matrix (used for 'translate = Vector3d.Zero').
    /// </summary>
    Matrix4d origin;

    protected override void setTime (double newTime)
    {
      time = newTime;

      // Animator was already Time-updated.
      if (!((MT.scene?.Animator ?? null) is PropertyAnimator pa) ||
          pa == null ||
          !pa.TryGetValue(name, ref translate))
        return;

      // New translation vector.
      ToParent   = origin * Matrix4d.CreateTranslation(translate);
      FromParent = ToParent.Inverted();
    }

    public AnimatedNodeTranslate (
      in string _name,
      in Vector3d _translate,
      in Matrix4d _origin,
      in double start = 0.0,
      in double end = 0.0)
      : base(SetOperation.Union)
    {
      name      = _name;
      translate = _translate;
      origin    = _origin;
      Start     = start;
      End       = end;
      time      = double.NegativeInfinity;
    }

    /// <summary>
    /// Clone all the time-dependent components, share the others.
    /// </summary>
    public override object Clone ()
    {
      AnimatedNodeTranslate n = new AnimatedNodeTranslate(name, translate, origin, Start, End);
      ShareCloneAttributes(n);
      ShareCloneChildren(n);
      n.Time = time;
      return n;
    }
  }
}
