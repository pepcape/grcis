using MathSupport;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Utilities;

// Objects for animation stuff.
namespace Rendering
{
  /// <summary>
  /// Base implementation of time-dependent Ray-scene.
  /// </summary>
  [Serializable]
  public class AnimatedRayScene : DefaultRayScene, ITimeDependent
  {
    /// <summary>
    /// Starting (minimal) time in seconds.
    /// </summary>
    public double Start { get; set; }

    /// <summary>
    /// Ending (maximal) time in seconds.
    /// </summary>
    public double End { get; set; }

    /// <summary>
    /// Internal variable for the current time.
    /// </summary>
    protected double time;

    /// <summary>
    /// Changes the current time - internal routine.
    /// Override it if you need something very nonstandard.
    /// </summary>
    protected virtual void setTime (double newTime)
    {
      time = Arith.Clamp(newTime, Start, End);

      // Animator?
      if (Animator != null)
        Animator.Time = time;

      // Time-dependent scene?
      if (Intersectable is ITimeDependent intersectable)
        intersectable.Time = time;

      // Time-dependent Background?
      if (Background is ITimeDependent bckgr)
        bckgr.Time = time;

      // Time-dependent camera?
      if (Camera is ITimeDependent camera)
        camera.Time = time;

      // Time-dependent light sources?
      foreach (ILightSource light in Sources)
        if (light is ITimeDependent li)
          li.Time = time;
    }

    /// <summary>
    /// Current time in seconds.
    /// </summary>
    public double Time
    {
      get => time;
      set => setTime(value);
    }

    /// <summary>
    /// Clone all the time-dependent components, share the others.
    /// </summary>
    public virtual object Clone ()
    {
      AnimatedRayScene sc = new AnimatedRayScene();
#if LOGGING
      Util.Log($"Clone(thr={MT.threadID}): AnimatedRayScene, {getSerial()}->{sc.getSerial()}");
#endif

      sc.Animator = (ITimeDependent)Animator?.Clone();

      sc.Intersectable = Intersectable;
      if (sc.Intersectable is ITimeDependent intersectable)
        sc.Intersectable = (IIntersectable)intersectable.Clone();

      sc.Background = Background;
      if (sc.Background is ITimeDependent bckgr)
        sc.Background = (IBackground)bckgr.Clone();

      sc.BackgroundColor = (double[])BackgroundColor.Clone();

      sc.Camera = Camera;
      if (sc.Camera is ITimeDependent camera)
        sc.Camera = (ICamera)camera.Clone();

      ILightSource[] tmp = new ILightSource[Sources.Count];
      Sources.CopyTo(tmp, 0);
      for (int i = 0; i < tmp.Length; i++)
      {
        if (tmp[i] is ITimeDependent source)
          tmp[i] = (ILightSource)source.Clone();
      }
      sc.Sources = new LinkedList<ILightSource>(tmp);

      sc.Start = Start;
      sc.End   = End;
      sc.setTime(Time); // propagates the current time to all time-dependent components..

      return sc;
    }

    /// <summary>
    /// Creates default ray-rendering scene.
    /// </summary>
    public AnimatedRayScene ()
    {
      Start = 0.0;
      End   = 10.0;
      time  = 0.0;
    }
  }

  /// <summary>
  /// CSG node used in animated scenes (able to propagate Time and Clone() to descendants
  /// and attributes.
  /// </summary>
  [Serializable]
  public class AnimatedCSGInnerNode : CSGInnerNode, ITimeDependent
  {
#if LOGGING
    private static volatile int nextSerial = 0;
    private readonly int serial = nextSerial++;
    public int getSerial () => serial;
#endif

    /// <summary>
    /// Starting (minimal) time in seconds.
    /// </summary>
    public double Start { get; set; }

    /// <summary>
    /// Ending (maximal) time in seconds.
    /// </summary>
    public double End { get; set; }

    protected double time;

    /// <summary>
    /// Propagates time to descendants.
    /// </summary>
    protected virtual void setTime (double newTime)
    {
      time = newTime;

      // set time in relevant children:
      foreach (ISceneNode child in children)
        if (child is ITimeDependent cha)
          cha.Time = newTime;
    }

    /// <summary>
    /// Current time in seconds.
    /// </summary>
    public double Time
    {
      get => time;
      set => setTime(value);
    }

    public AnimatedCSGInnerNode (
      SetOperation op,
      double start = 0.0,
      double end = 0.0)
      : base(op)
    {
      Start = start;
      End   = end;
      time  = start;
    }

    protected AnimatedCSGInnerNode (
      Operation _bop,
      double start = 0.0,
      double end = 0.0)
      : base(_bop)
    {
      Start = start;
      End   = end;
      time  = start;
    }

    /// <summary>
    /// Clone all the time-dependent components, share the others.
    /// </summary>
    public virtual object Clone ()
    {
#if LOGGING
      Util.Log($"Clone(thr={MT.threadID}): AnimatedCSGInnerNode");
#endif
      AnimatedCSGInnerNode n = new AnimatedCSGInnerNode(bop, Start, End);
      ShareCloneAttributes(n);
      ShareCloneChildren(n);
      n.Time = time;
      return n;
    }
  }

  /// <summary>
  /// Sample implementation of time-dependent camera.
  /// It simply goes round the central point (look-at point).
  /// </summary>
  [Serializable]
  public class AnimatedCamera : StaticCamera, ITimeDependent
  {
#if LOGGING
    private static volatile int nextSerial = 0;
    private readonly int serial = nextSerial++;
    public int getSerial () => serial;
#endif

    /// <summary>
    /// Starting (minimal) time in seconds.
    /// </summary>
    public double Start { get; set; }

    /// <summary>
    /// Ending (maximal) time in seconds.
    /// </summary>
    public double End { get; set; }

    protected double time;

    /// <summary>
    /// Goes round the central point (lookAt).
    /// One complete turn for the whole time interval.
    /// </summary>
    protected virtual void setTime (double newTime)
    {
      Debug.Assert(Start != End);

#if LOGGING && VERBOSE
      Util.Log($"Camera(thr={MT.threadID}) #{getSerial()} setTime({newTime:f3})");
#endif

      time = newTime;    // Here Start & End define a periodicity, not bounds!

      // Change the camera position.
      double angle = MathHelper.TwoPi * (time - Start) / (End - Start);
      Vector3d radial = Vector3d.TransformVector(center0 - lookAt, Matrix4d.CreateRotationY(-angle));
      center = lookAt + radial;
      direction = -radial;
      direction.Normalize();
      prepare();
    }

    /// <summary>
    /// Current time in seconds.
    /// </summary>
    public double Time
    {
      get => time;
      set => setTime(value);
    }

    /// <summary>
    /// Central point (to look at).
    /// </summary>
    protected Vector3d lookAt;

    /// <summary>
    /// Center for time == Start;
    /// </summary>
    protected Vector3d center0;

    /// <summary>
    /// Clone all the time-dependent components, share the others.
    /// </summary>
    public virtual object Clone ()
    {
      AnimatedCamera c = new AnimatedCamera(lookAt, center0, MathHelper.RadiansToDegrees((float)hAngle))
      {
        Start  = Start,
        End    = End,
        Time   = Time,
        Width  = Width,
        Height = Height,
      };
#if LOGGING
      Util.Log($"Clone(thr={MT.threadID}): AnimatedCamera, {getSerial()}->{c.getSerial()}");
#endif
      return c;
    }

    public AnimatedCamera (Vector3d lookat, Vector3d cen, double ang)
      : base(cen, lookat - cen, ang)
    {
      lookAt  = lookat;
      center0 = cen;
      Start   = 0.0;
      End     = 10.0;
      time    = 0.0;
    }
  }

  /// <summary>
  /// Animated object with named properties.
  /// The most popular numeric values so far.
  /// Usually used as a value provider by other animated objects.
  /// Implementing only some of mentioned data types is ok,
  /// just return null for the rest..
  /// </summary>
  public interface ITimeDependentProperty : ITimeDependent
  {
    /// <summary>
    /// Can be ssed for array types or any other types..
    /// </summary>
    /// <param name="name">Unique name of the property.</param>
    /// <returns>Current value of the property or null if not present.</returns>
    object GetValue (in string name);

    /// <summary>
    /// Float property getter.
    /// </summary>
    /// <param name="name">Unique name of the property.</param>
    /// <param name="f">Property value (not changed if failed).</param>
    /// <returns>True if the property is present.</returns>
    bool TryGetValue (in string name, ref float f);

    /// <summary>
    /// Double property getter.
    /// </summary>
    /// <param name="name">Unique name of the property.</param>
    /// <param name="d">Property value (not changed if failed).</param>
    /// <returns>True if the property is present.</returns>
    bool TryGetValue (in string name, ref double d);

    /// <summary>
    /// Vector3 (float components) property getter.
    /// </summary>
    /// <param name="name">Unique name of the property.</param>
    /// <param name="v3">Property value (not changed if failed).</param>
    /// <returns>True if the property is present.</returns>
    bool TryGetValue (in string name, ref Vector3 v3);

    /// <summary>
    /// Vector4 (float components) property getter.
    /// </summary>
    /// <param name="name">Unique name of the property.</param>
    /// <param name="v4">Property value (not changed if failed).</param>
    /// <returns>True if the property is present.</returns>
    bool TryGetValue (in string name, ref Vector4 v4);

    /// <summary>
    /// Vector3d (double components) property getter.
    /// </summary>
    /// <param name="name">Unique name of the property.</param>
    /// <param name="v3">Property value (not changed if failed).</param>
    /// <returns>True if the property is present.</returns>
    bool TryGetValue (in string name, ref Vector3d v3);

    /// <summary>
    /// Vector4d (double components) property getter.
    /// </summary>
    /// <param name="name">Unique name of the property.</param>
    /// <param name="v4">Property value (not changed if failed).</param>
    /// <returns>True if the property is present.</returns>
    bool TryGetValue (in string name, ref Vector4d v4);

    /// <summary>
    /// Quaterniond (double components) property getter.
    /// </summary>
    /// <param name="name">Unique name of the property.</param>
    /// <param name="q">Property value (not changed if failed).</param>
    /// <returns>True if the property is present.</returns>
    bool TryGetValue (in string name, ref Quaterniond q);

    /// <summary>
    /// Matrix4d (double components) property getter.
    /// </summary>
    /// <param name="name">Unique name of the property.</param>
    /// <param name="m4">Property value (not changed if failed).</param>
    /// <returns>True if the property is present.</returns>
    bool TryGetValue (in string name, ref Matrix4d m4);
  }

  /// <summary>
  /// Shared logic for ITimeDependentProperty animators -
  /// able to interpolate numeric quantities ('properties').
  /// </summary>
  [Serializable]
  public class PropertyAnimator : ITimeDependentProperty
  {
#if LOGGING
    private static volatile int nextSerial = 0;
    private readonly int serial = nextSerial++;
    public int getSerial () => serial;
#endif

    [Serializable]
    public enum InterpolationStyle
    {
      /// <summary>
      /// The animation is computed only once.
      /// Value before the start is equal to the start value,
      /// value after the end keeps constant.
      /// </summary>
      Once,

      /// <summary>
      /// Pure cyclic animation: the end value is (noncontinuously) followed
      /// by the start value again. If you manage to connect start and end
      /// properly, you can get nice cyclic animation.
      /// </summary>
      Cyclic,

      /// <summary>
      /// Continuous animation: start --- end --- start --- ...
      /// </summary>
      Pendulum
    }

    /// <summary>
    /// Data-holder class, it can be read-only (i.e. shared) after initialization.
    /// </summary>
    /// <typeparam name="T">Numeric type {double | Vector3d | Matrix4d...}</typeparam>
    [Serializable]
    public class Property<T>
    {
      /// <summary>
      /// Property name used for Dictionary lookup.
      /// </summary>
      public string name;

      /// <summary>
      /// List of Catmull-Rome knots.
      ///
      /// For open curves:
      /// [0] and [N - 1] are outside knots (interpolation is done
      /// between [1] and [N - 2]).
      ///
      /// For closed curves:
      /// [0] is successor of [N - 1] and vice versa.
      /// </summary>
      public List<T> data;

      /// <summary>
      /// Cyclic set of knots = closed curve.
      /// </summary>
      public bool cyclic;

      /// <summary>
      /// Start of the animation.
      /// </summary>
      public double tStart;

      /// <summary>
      /// End of the animation.
      /// </summary>
      public double tEnd;

      /// <summary>
      /// Period definition (if applicable).
      /// </summary>
      public double tPeriod;

      /// <summary>
      /// Animation style (how to utilize the animation curve).
      /// </summary>
      public InterpolationStyle style;

      /// <summary>
      /// Interval (key to key) length.
      /// </summary>
      public virtual double interval ()
      {
        int ints = Math.Max(intervals(), 1);
        return ((style == InterpolationStyle.Once) ? (tEnd - tStart) : tPeriod) / ints;
      }

      /// <summary>
      /// Returns number of active intervals (key to key).
      /// Default implementation os for cubic curves (one outer knot on each of the boundaries).
      /// </summary>
      public virtual int intervals ()
      {
        if (data == null ||
            data.Count == 0)
          return 0;

        if (cyclic)
          return data.Count == 1 ? 0 : data.Count;

        return Math.Max(data.Count - 3, 0);
      }

      /// <summary>
      /// Return configuration (fractional time and indices)
      /// for cubic spline curve.
      /// </summary>
      /// <param name="t">Time/parameter (input).</param>
      /// <param name="fraction">Fractional time between 0.0 and 1.0.</param>
      /// <param name="i0">Knot index 0.</param>
      /// <param name="i1">Knot index 1.</param>
      /// <param name="i2">Knot index 2.</param>
      /// <param name="i3">Knot index 3.</param>
      /// <returns>True if ok.</returns>
      public virtual bool prepareCubic (
        double t,
        out double fraction,
        out int i0,
        out int i1,
        out int i2,
        out int i3)
      {
        int segments = intervals();
        int len = (data == null) ? 0 : data.Count;

        if (segments == 0)
        {
          // No interpolation at all => constant value.
          fraction = 0.0;
          if (len == 0)
          {
            i0 = i1 = i2 = i3 = -1;
            return false;
          }
          i0 = i1 = i2 = i3 = 0;
          if (len == 1)
            return true;
          i3 = 1;
          if (len == 2)
            return true;
          i1 = i2 = 1;
          i3 = 2;
          return true;
        }

        // There's at least one interval knot - knot.
        double intvl = interval();
        double t0;
        double myStart = tStart;
        double myEnd   = tEnd;
        bool before = t <= tStart;

        if (!before)
        {
          // Shared preprocessing.
          if (style != InterpolationStyle.Once)
          {
            // Cyclic movement.
            if (t > tEnd)
              t = tEnd;
            t0 = (t - tStart) / tPeriod; // t0 = t expressed in the period-scale
            int fullCycles = (int)Math.Floor(t0);
            t = t0 - fullCycles;         // fractional part

            if (style == InterpolationStyle.Pendulum &&
                (fullCycles & 1) == 1)
              t = 1.0 - t;

            myStart = 0.0;
            myEnd = 1.0;
          }
        }

        if (cyclic)
        {
          // Cyclic data.

          // Before the start.
          if (before)
          {
            fraction = 0.0;
            i0 = len - 1;
            i1 = 0;
            i2 = 1;
            i3 = (len == 2) ? 0 : 2;
            return true;
          }

          // After the end.
          if (t >= myEnd)
          {
            fraction = 1.0;
            i0 = (len == 2) ? 1 : len - 3;
            i1 = len - 2;
            i2 = len - 1;
            i3 = 0;
            return true;
          }

          // In the middle.
          // myStart < t < myEnd.

          t = (t - myStart) / (myEnd - myStart); // 0.0 < t < 1.0
          t *= segments;                         // 0.0 < t < segments
          t0 = Math.Floor(t);
          fraction = t - t0;
          i1 = (int)t0;                          // 0 <= i1 < segments
          i0 = i1 - 1;
          if (i0 < 0)
            i0 = len - 1;
          i2 = i1 + 1;
          i3 = i2 + 1;
          if (i2 >= len)
            i2 -= len;
          if (i3 >= len)
            i3 -= len;
          return true;
        }

        // Non-cyclic data.
        // We've got at least 4 knots.

        // Before the start.
        if (before)
        {
          fraction = 0.0;
          i0 = 0;
          i1 = 1;
          i2 = 2;
          i3 = 3;
          return true;
        }

        // After the end.
        if (t >= myEnd)
        {
          fraction = 1.0;
          i0 = len - 4;
          i1 = len - 3;
          i2 = len - 2;
          i3 = len - 1;
          return true;
        }

        // myStart < t < myEnd.
        t = (t - myStart) / (myEnd - myStart); // 0.0 < t < 1.0
        t *= segments;                         // 0.0 < t < segments
        t0 = Math.Floor(t);
        fraction = t - t0;
        i1 = (int)t0 + 1;                      // 0 <= i1 < segments
        i0 = i1 - 1;
        i2 = i1 + 1;
        i3 = i2 + 1;
        if (i3 >= len)
        {
          fraction = 1.0;
          i0--;
          i1--;
          i2--;
          i3--;
        }

        return true;
      }
    }

    /// <summary>
    /// Link to the next general Animator (chain of ITimeDependent objects).
    /// </summary>
    protected ITimeDependent nextGeneral;

    /// <summary>
    /// Chain of the property-based animators.
    /// </summary>
    protected ITimeDependentProperty nextProperty;

    protected double start = 0.0;

    /// <summary>
    /// Starting (minimal) time in seconds.
    /// </summary>
    public double Start
    {
      get => start;
      set
      {
        if (nextGeneral != null)
          nextGeneral.Start = value;
        if (nextProperty != null)
          nextProperty.Start = value;
        start = value;
      }
    }

    protected double end = 1.0;

    /// <summary>
    /// Ending (maximal) time in seconds.
    /// </summary>
    public double End
    {
      get => end;
      set
      {
        if (nextGeneral != null)
          nextGeneral.End = value;
        if (nextProperty != null)
          nextProperty.End = value;
        end = value;
      }
    }

    /// <summary>
    /// Internal variable for the current time.
    /// </summary>
    protected double time = double.NegativeInfinity;

    /// <summary>
    /// Current time in seconds.
    /// </summary>
    public double Time
    {
      get => time;
      set
      {
        if (nextGeneral != null)
          nextGeneral.Time = value;
        if (nextProperty != null)
          nextProperty.Time = value;
        setTime(value);
      }
    }

    /// <summary>
    /// Changes the current time - internal routine.
    /// </summary>
    protected virtual void setTime (double newTime)
    {
      time = Arith.Clamp(newTime, Start, End);

      // You usually don't need to override this.
      // If you do, put your custom code here.
    }

    /// <summary>
    /// Only Property&lt;T&gt; are used here.
    /// </summary>
    protected Dictionary<string, object> properties;

    public PropertyAnimator (
      in ITimeDependent nxtGen = null,
      in ITimeDependentProperty nxtProp = null)
    {
      nextGeneral  = nxtGen;
      nextProperty = nxtProp;
      properties   = new Dictionary<string, object>();
      Start        =  0.0;
      End          = 10.0;
      time         = double.NegativeInfinity;
    }

    /// <summary>
    /// Clone the object, share the data.
    /// </summary>
    public virtual object Clone ()
    {
      ITimeDependent         nxtGen  = (ITimeDependent)nextGeneral?.Clone();
      ITimeDependentProperty nxtProp = (ITimeDependentProperty)nextProperty?.Clone();

      PropertyAnimator a = new PropertyAnimator(nxtGen, nxtProp)
      {
        properties = properties,
        Start      = Start,
        End        = End,
        Time       = Time
      };

      return a;
    }

    public virtual void setProperty<T> (
      in Property<T> property)
    {
      if (property != null)
        properties[property.name] = property;
    }

    public virtual void newProperty<T> (
      in string name,
      in double start,
      in double end,
      in double period,
      in InterpolationStyle style,
      List<T> data,
      bool cyclic = false)
    {
      setProperty(new Property<T>
      {
        name    = name,
        tStart  = start,
        tEnd    = end,
        tPeriod = period,
        data    = data,
        cyclic  = cyclic,
        style   = style
      });
    }

    public virtual object GetValue (in string name)
    {
      // Override me if you need to define this functionality.
      // Call the base.GetValue() if failed.

      return nextProperty?.GetValue(name);
    }

    public virtual bool TryGetValue (in string name, ref float f)
    {
      // Override me if you need to define this functionality.
      // Call the base.TryGetValue() if failed.

      return (nextProperty?.TryGetValue(name, ref f)).Value;
    }

    public virtual bool TryGetValue (in string name, ref double d)
    {
      // Override me if you need to define this functionality.
      // Call the base.TryGetValue() if failed.

      return (nextProperty?.TryGetValue(name, ref d)).Value;
    }

    public virtual bool TryGetValue (in string name, ref Vector3 v3)
    {
      // Override me if you need to define this functionality.
      // Call the base.TryGetValue() if failed.

      return (nextProperty?.TryGetValue(name, ref v3)).Value;
    }

    public virtual bool TryGetValue (in string name, ref Vector4 v4)
    {
      // Override me if you need to define this functionality.
      // Call the base.TryGetValue() if failed.

      return (nextProperty?.TryGetValue(name, ref v4)).Value;
    }

    public virtual bool TryGetValue (in string name, ref Vector3d v3)
    {
      // Override me if you need to define this functionality.
      // Call the base.TryGetValue() if failed.

      return (nextProperty?.TryGetValue(name, ref v3)).Value;
    }

    public virtual bool TryGetValue (in string name, ref Vector4d v4)
    {
      // Override me if you need to define this functionality.
      // Call the base.TryGetValue() if failed.

      return (nextProperty?.TryGetValue(name, ref v4)).Value;
    }

    public virtual bool TryGetValue (in string name, ref Quaterniond q)
    {
      // Override me if you need to define this functionality.
      // Call the base.TryGetValue() if failed.

      return (nextProperty?.TryGetValue(name, ref q)).Value;
    }

    public virtual bool TryGetValue (in string name, ref Matrix4d m4)
    {
      // Override me if you need to define this functionality.
      // Call the base.TryGetValue() if failed.

      return (nextProperty?.TryGetValue(name, ref m4)).Value;
    }
  }
}
