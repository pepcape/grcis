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
#if DEBUG
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

    /// <summary>
    /// Internal variable for the current time.
    /// </summary>
    protected double time;

    /// <summary>
    /// Changes the current time - internal routine.
    /// Override it if you need time-dependent background color..
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
    /// <returns></returns>
    public virtual object Clone ()
    {
#if DEBUG
      Util.Log("Clone: AnimatedRayScene");
#endif
      AnimatedRayScene sc = new AnimatedRayScene ();

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
  public class AnimatedCSGInnerNode : CSGInnerNode, ITimeDependent
  {
#if DEBUG
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
#if DEBUG
      Util.Log( "Clone: AnimatedCSGInnerNode" );
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
  public class AnimatedCamera : StaticCamera, ITimeDependent
  {
#if DEBUG
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

#if DEBUG
      Debug.WriteLine($"Camera #{getSerial()} setTime({newTime})");
#endif

      time = newTime;    // Here Start & End define a periodicity, not bounds!

      // change the camera position:
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
        Start = Start,
        End   = End,
        Time  = Time
      };
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
}
