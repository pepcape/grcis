using System;
using System.Collections.Generic;
using MathSupport;
using OpenTK;
using Rendering;

namespace JosefPelikan
{
  /// <summary>
  /// Set of glowing particles able to be animated via ITimeDependentProperty.
  /// </summary>
  [Serializable]
  public class ChaoticParticles : DefaultSceneNode, ISolid, ITimeDependent
  {
#if LOGGING
    private static volatile int nextSerial = 0;
    private readonly int serial = nextSerial++;
    public int getSerial () => serial;
#endif

    /// <summary>
    /// Initial/current particle positions and sizes.
    /// [X, Y, Z] .. particle center, [W] .. particle radius.
    /// </summary>
    protected Vector4d[] particles;

    /// <summary>
    /// Initial/current particle color [R, G, B].
    /// </summary>
    protected Vector3d[] colors;

    /// <summary>
    /// Bounding box.
    /// </summary>
    protected Vector3d minCorner;
    protected Vector3d maxCorner;

    /// <summary>
    /// Property name for data link between this object and ITimeDependentProperty.
    /// Particle position & size.
    /// </summary>
    protected string positionName;

    /// <summary>
    /// Property name for data link between this object and ITimeDependentProperty.
    /// Particle color.
    /// </summary>
    protected string colorName;

    public void SetBoundingBox (in Vector3d corner1, in Vector3d corner2)
    {
      minCorner = corner1;
      maxCorner = corner2;
    }

    public virtual void GetBoundingBox (out Vector3d corner1, out Vector3d corner2)
    {
      corner1 = minCorner;
      corner2 = maxCorner;
    }

    public ChaoticParticles (
      in Vector4d[] ps,
      in Vector3d[] col,
      in string posName,
      in string colName = null)
    {
      particles    = ps;
      colors       = col;
      positionName = posName;
      colorName    = colName;
    }

    public ChaoticParticles (
      in Vector4[] ps,
      in Vector3[] col,
      in string posName,
      in string colName = null)
    {
      particles    = Geometry.Vector4dArrayFrom(ps);
      colors       = Geometry.Vector3dArrayFrom(col);
      positionName = posName;
      colorName    = colName;
    }

    public ChaoticParticles (
      in Vector4d[] ps,
      in Vector3[] col,
      in string posName,
      in string colName = null)
    {
      particles    = ps;
      colors       = Geometry.Vector3dArrayFrom(col);
      positionName = posName;
      colorName    = colName;
    }

    public ChaoticParticles (
      in Vector4[] ps,
      in Vector3d[] col,
      in string posName,
      in string colName = null)
    {
      particles    = Geometry.Vector4dArrayFrom(ps);
      colors       = col;
      positionName = posName;
      colorName    = colName;
    }

    public ChaoticParticles (
      in Vector4d particle,
      in Vector3d? color,
      in string posName,
      in string colName = null)
      : this(
          new Vector4d[] { particle },
          color.HasValue ? new Vector3d[] { color.Value } : null,
          posName, colName)
    {
    }

    /// <summary>
    /// Temporary object Intersect => CompleteIntersection.
    /// </summary>
    class Support
    {
      // Ray direction.
      public Vector3d dir;

      // Particle being hit (index into 'ChaoticParticles.particles' array.
      public int index;
    }

    /// <summary>
    /// Translated sphere vs. ray intersections.
    /// </summary>
    /// <param name="offset">Local sphere's translation.</param>
    /// <param name="p0">Ray origin.</param>
    /// <param name="p1">Ray direction.</param>
    /// <param name="index">Sphere index.</param>
    /// <param name="cut">The nearest intersection so far.</param>
    /// <returns>Two intersections or null.</returns>
    protected LinkedList<Intersection> SphereIntersect (
      Vector3d offset,
      double scale,
      ref Vector3d p0,
      ref Vector3d p1,
      ref double cut,
      int index = 0)
    {
      Vector3d origin = (p0 - offset) * scale;
      Vector3d dir = p1 * scale;
      double OD;
      Vector3d.Dot(ref origin, ref dir, out OD);
      double DD;
      Vector3d.Dot(ref dir, ref dir, out DD);
      double OO;
      Vector3d.Dot(ref origin, ref origin, out OO);
      double d = OD * OD + DD * (1.0 - OO); // discriminant
      if (d <= 0.0)
        return null;            // no intersections

      // Single intersection: (-OD - d) / DD.
      LinkedList<Intersection> result = new LinkedList<Intersection> ();
      double t = (-OD - Math.Sqrt(d)) / DD;

      // Cut test.
      if (t >= cut ||
          t < 0.0)
        return null;
      cut = t;

      Vector3d loc = p0 + t * p1;

      // Shared support object.
      Support sup = new Support
      {
        dir   = p1,
        index = index
      };

      Intersection i = new Intersection(this)
      {
        T          = t,
        Enter      = true,
        Front      = true,
        CoordLocal = loc,
        Normal     = loc - offset,
        SolidData  = sup
      };
      result.AddLast(i);

      i = new Intersection(this)
      {
        T          = t + Intersection.SHELL_THICKNESS,
        Enter      = false,
        Front      = false,
        CoordLocal = loc + Intersection.SHELL_THICKNESS * p1,
        Normal     = loc - offset,
        SolidData  = sup
      };
      result.AddLast(i);

      return result;
    }

    /// <summary>
    /// Computes the complete intersection of the given ray with the object.
    /// </summary>
    /// <param name="p0">Ray origin.</param>
    /// <param name="p1">Ray direction vector.</param>
    /// <returns>Sorted list of intersection records.</returns>
    public override LinkedList<Intersection> Intersect (Vector3d p0, Vector3d p1)
    {
      if (particles == null ||
          particles.Length == 0)
        return null;

      // Cutting support.
      double cut = double.MaxValue;

      // Particle size not implemented yet.
      Vector3d offset = new Vector3d(particles[0].X, particles[0].Y, particles[0].Z);
      double scale = 1.0 / particles[0].W;
      int nearest = 0;
      LinkedList<Intersection> list = SphereIntersect(offset, scale, ref p0, ref p1, ref cut, 0);
      for (int i = 1; i < particles.Length; i++)
      {
        offset = new Vector3d(particles[i].X, particles[i].Y, particles[i].Z);
        scale = 1.0 / particles[i].W;
        LinkedList<Intersection> tmp = SphereIntersect(offset, scale, ref p0, ref p1, ref cut, i);
        if (tmp != null)
        {
          list    = tmp;
          nearest = i;
        }
      }

      // Optional individual color (probably from a ITimeDependentProperty).
      if (list != null &&
          colors != null &&
          nearest < colors.Length)
        list.First.Value.SurfaceColor = Geometry.ColorCreate(colors[nearest]);

      return list;
    }

    /// <summary>
    /// Complete all relevant items in the given Intersection object.
    /// </summary>
    /// <param name="inter">Intersection instance to complete.</param>
    public override void CompleteIntersection (Intersection inter)
    {
      Vector3d locNormal = inter.Normal;

      // Normal vector.
      Vector3d tu, tv;
      Geometry.GetAxes(ref inter.Normal, out tu, out tv);
      tu = Vector3d.TransformVector(tu, inter.LocalToWorld);
      tv = Vector3d.TransformVector(tv, inter.LocalToWorld);
      Vector3d.Cross(ref tu, ref tv, out inter.Normal);

      // 2D texture coordinates - projection to 2D from the ray-direction.
      if (inter.SolidData is Support sup)
      {
        sup.dir.Normalize();
        locNormal.Normalize();
        double cosa = Vector3d.Dot(sup.dir, locNormal);
        double sina = Math.Sqrt(1.0 - cosa * cosa);
        // sina = distance_from_center
        inter.TextureCoord.X =
        inter.TextureCoord.Y = sina;
      }
      else
      {
        double r = Math.Max(1.0e-12, Math.Sqrt(locNormal.X * locNormal.X + locNormal.Y * locNormal.Y));
        inter.TextureCoord.X = locNormal.X / r;
        inter.TextureCoord.Y = locNormal.Y / r;
      }
    }

    //--- ITimeDependent ---

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

      if (!((MT.scene?.Animator ?? null) is ITimeDependentProperty pa) ||
          pa == null)
        return;

      // Update all the particles positions+sizes via ITimeDependentProperty.
      // I'm only interested in Vector4d[] properties, everything else is not for me.
      if (!string.IsNullOrEmpty(positionName))
      {
        if (pa.GetValue(positionName) is Vector4d[] v4d)
          particles = v4d;
        else
        if (pa.GetValue(positionName) is Vector4[] v4)
          particles = Geometry.Vector4dArrayFrom(v4);
      }

      // Update all the particles colors via ITimeDependentProperty.
      // I'm only interested in Vector3d[] properties, everything else is not for me.
      if (!string.IsNullOrEmpty(colorName))
      {
        if (pa.GetValue(colorName) is Vector3d[] v3d)
          colors = v3d;
        else
        if (pa.GetValue(colorName) is Vector3[] v3)
          colors = Geometry.Vector3dArrayFrom(v3);
      }
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
      ChaoticParticles n = new ChaoticParticles(particles, colors, positionName, colorName);
      ShareCloneAttributes(n);
      n.Time = time;
      return n;
    }
  }
}
