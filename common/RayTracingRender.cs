using System;
using System.Collections.Generic;
using OpenTK;
using MathSupport;
using Utilities;

namespace Rendering
{
  /// <summary>
  /// Class representing ray-surface light interaction in the form of direct
  /// light contribution and potentially many subsequent (recursive) rays
  /// associated with weight color coefficients.
  /// "result = DirectContribution + foreach(ray in Rays) ray.weight * shade(ray)"
  /// </summary>
  [Serializable]
  public class RayRecursion
  {
    /// <summary>
    /// Individual ray including it's quantitative contribution.
    /// </summary>
    public struct RayContribution
    {
      /// <summary>
      /// Ray origin.
      /// </summary>
      public Vector3d origin;

      /// <summary>
      /// Ray directional vector.
      /// </summary>
      public Vector3d direction;

      /// <summary>
      /// Multiplicative coefficient, it could be null (for 1.0), single-component
      /// array (for uniform multiplicator) or full-length color multiplicator.
      /// </summary>
      public double[] coefficient;

      /// <summary>
      /// Importance for recursive shade() call.
      /// </summary>
      public double importance;

      public RayContribution (
        in Vector3d p0,
        in Vector3d p1,
        in double[] coeff = null,
        in double imp = 1.0)
      {
        origin = p0 + Intersection.RAY_EPSILON * p1;
        direction = p1;
        coefficient = coeff;
        importance = imp;
      }

      /// <summary>
      /// Plain subsequent ray from the given intersection.
      /// </summary>
      public RayContribution (
        in Intersection i,
        in Vector3d dir,
        in double imp)
      {
        origin      = i.CoordWorld + Intersection.RAY_EPSILON * dir;
        direction   = dir;
        coefficient = null;
        importance  = imp;
      }

      public RayContribution (RayContribution r)
      {
        origin      = r.origin;
        direction   = r.direction;
        coefficient = r.coefficient;
        importance  = r.importance;
      }
    }

    /// <summary>
    /// Direct contribution to the light. It can be null (no contribution) or
    /// single-component array (global/monochromatic contribution) or regular
    /// full-length color array.
    /// </summary>
    public double[] DirectContribution;

    /// <summary>
    /// List of ray contributions. Individual recursive rays.
    /// </summary>
    public List<RayContribution> Rays;

    public RayRecursion ()
    {
      DirectContribution = null;
      Rays = null;
    }

    public RayRecursion (in double[] direct, in RayContribution? ray = null)
    {
      DirectContribution = direct;
      Rays = new List<RayContribution>();
      if (ray != null)
        Rays.Add(ray.Value);
    }

    public RayRecursion (in double[] direct, in List<RayContribution> rays)
    {
      DirectContribution = direct;
      Rays = rays;
    }
  }

  /// <summary>
  /// Optional delegate function defining how the rest of "ray vs. surface" interaction
  /// should be computed. It is associated with scene dodes via attribute PropertyName.RECURSION
  /// and if found, it is run instead the default ray-tracing "shading + recursion" phases.
  /// </summary>
  /// <param name="i">Completed intersection with the solid.</param>
  /// <param name="importance">Input importance factor (don't recurse if the accumulated importance is too small).</param>
  /// <param name="rr">Rules for further processing.</param>
  /// <returns></returns>
  [Serializable]
  public delegate long RecursionFunction (
    Intersection i,
    Vector3d dir,
    double importance,
    out RayRecursion rr);

  /// <summary>
  /// Ray-tracing rendering (w all secondary rays).
  /// </summary>
  [Serializable]
  public class RayTracing : RayCasting
  {
    /// <summary>
    /// Hash-multiplier for refracted rays.
    /// </summary>
    protected const long HASH_REFRACT = 13L;

    /// <summary>
    /// Hash-multiplier for reflected rays.
    /// </summary>
    protected const long HASH_REFLECT = 17L;

    /// <summary>
    /// Hash-multiplier for overrided ray-recursion.
    /// </summary>
    protected const long HASH_RECURSION = 101L;

    /// <summary>
    /// Recursion-termination parameter - maximal recursion depth.
    /// </summary>
    public int MaxLevel { get; set; }

    /// <summary>
    /// Recursion-termination parameter - minimal importance value which causes
    /// ray reflection and/or refraction.
    /// </summary>
    public double MinImportance { get; set; }

    /// <summary>
    /// Compute reflected secondary rays?
    /// </summary>
    public bool DoReflections { get; set; }

    /// <summary>
    /// Compute refracted secondary rays?
    /// </summary>
    public bool DoRefractions { get; set; }

    /// <summary>
    /// Use secondary shadow rays to determine light source visibility?
    /// </summary>
    public bool DoShadows { get; set; }

    /// <summary>
    /// Do ray-recursion in overrided ray-surface interaction?
    /// </summary>
    public bool DoRecursion { get; set; }

    public RayTracing (IRayScene sc = null)
      : base(sc)
    {
      MaxLevel      = 12;
      MinImportance = 0.05;
      DoReflections =
      DoRefractions =
      DoShadows     =
      DoRecursion   = true;
    }

    [NonSerialized]
    public AbstractRayRegisterer rayRegisterer;

    /// <summary>
    /// Computes one image sample. Internal integration support.
    /// </summary>
    /// <param name="x">Horizontal coordinate.</param>
    /// <param name="y">Vertical coordinate.</param>
    /// <param name="color">Computed sample color.</param>
    /// <returns>Hash-value used for adaptive subsampling.</returns>
    public override long GetSample (double x, double y, double[] color)
    {
      MT.doubleX = x;
      MT.doubleY = y;

      // initial color = black
      Array.Clear(color, 0, color.Length);

      Vector3d p0, p1;
      if (scene.Camera.GetRay(x, y, out p0, out p1))
      {
        long hash = shade(0, 1.0, ref p0, ref p1, color);

        return hash;
      }

      return 11L;
    }

    /// <summary>
    /// Recursive shading function - computes color contribution of the given ray (shot from the
    /// origin 'p0' into direction vector 'p1''). Recursion is stopped
    /// by a hybrid method: 'importance' and 'level' are checked.
    /// Internal integration support.
    /// </summary>
    /// <param name="depth">Current recursion depth.</param>
    /// <param name="importance">Importance of the current ray.</param>
    /// <param name="p0">Ray origin.</param>
    /// <param name="p1">Ray direction vector.</param>
    /// <param name="color">Result color.</param>
    /// <returns>Hash-value (ray sub-signature) used for adaptive subsampling.</returns>
    protected virtual long shade (int depth,
                                  double importance,
                                  ref Vector3d p0,
                                  ref Vector3d p1,
                                  double[] color)
    {
      Vector3d direction = p1;

      int bands = color.Length;
      LinkedList<Intersection> intersections = scene.Intersectable.Intersect(p0, p1);

      // If the ray is primary, increment both counters
      Statistics.IncrementRaysCounters(1, depth == 0);

      Intersection i = Intersection.FirstIntersection(intersections, ref p1);

      if (i == null)
      {
        // No intersection -> background color
        rayRegisterer?.RegisterRay(AbstractRayRegisterer.RayType.rayVisualizerNormal, depth, p0, direction * 100000);

        return scene.Background.GetColor(p1, color);
      }

      // There was at least one intersection
      i.Complete();

      rayRegisterer?.RegisterRay(AbstractRayRegisterer.RayType.unknown, depth, p0, i);

      // Hash code for adaptive supersampling
      long hash = i.Solid.GetHashCode();

      // Apply all the textures first
      if (i.Textures != null)
        foreach (ITexture tex in i.Textures)
          hash = hash * HASH_TEXTURE + tex.Apply(i);

      if (MT.pointCloudCheckBox && !MT.pointCloudSavingInProgress && !MT.singleRayTracing)
      {
        foreach (Intersection intersection in intersections)
        {
          if (!intersection.completed)
            intersection.Complete();

          if (intersection.Textures != null && !intersection.textureApplied)
            foreach (ITexture tex in intersection.Textures)
              tex.Apply(intersection);

          double[] vertexColor = new double[3];
          Util.ColorCopy(intersection.SurfaceColor, vertexColor);
          Master.singleton?.pointCloud?.AddToPointCloud(intersection.CoordWorld, vertexColor, intersection.Normal, MT.threadID);
        }
      }

      // Color accumulation.
      Array.Clear(color, 0, bands);
      double[] comp = new double[bands];

      // Optional override ray-processing (procedural).
      if (DoRecursion &&
          i.Solid?.GetAttribute(PropertyName.RECURSION) is RecursionFunction rf)
      {
        hash += HASH_RECURSION * rf(i, p1, importance, out RayRecursion rr);

        if (rr != null)
        {
          // Direct contribution.
          if (rr.DirectContribution != null &&
              rr.DirectContribution.Length > 0)
            if (rr.DirectContribution.Length == 1)
              Util.ColorAdd(rr.DirectContribution[0], color);
            else
              Util.ColorAdd(rr.DirectContribution, color);

          // Recursive rays.
          if (rr.Rays != null &&
              depth++ < MaxLevel)
            foreach (var ray in rr.Rays)
            {
              RayRecursion.RayContribution rc = ray;
              hash += HASH_REFLECT * shade(depth, rc.importance, ref rc.origin, ref rc.direction, comp);

              // Combine colors.
              if (ray.coefficient == null)
                Util.ColorAdd(comp, color);
              else
              if (ray.coefficient.Length == 1)
                Util.ColorAdd(comp, ray.coefficient[0], color);
              else
                Util.ColorAdd(comp, ray.coefficient, color);
            }

          return hash;
        }
      }

      // Default (Whitted) ray-tracing interaction (lights [+ reflection] [+ refraction]).
      p1 = -p1; // viewing vector
      p1.Normalize();

      if (scene.Sources == null || scene.Sources.Count < 1)
        // No light sources at all.
        Util.ColorAdd(i.SurfaceColor, color);
      else
      {
        // Apply the reflectance model for each source.
        i.Material = (IMaterial)i.Material.Clone();
        i.Material.Color = i.SurfaceColor;

        foreach (ILightSource source in scene.Sources)
        {
          double[] intensity = source.GetIntensity(i, out Vector3d dir);

          if (MT.singleRayTracing && source.position != null)
            // Register shadow ray for RayVisualizer.
            rayRegisterer?.RegisterRay(AbstractRayRegisterer.RayType.rayVisualizerShadow, i.CoordWorld, (Vector3d)source.position);

          if (intensity != null)
          {
            if (DoShadows && dir != Vector3d.Zero)
            {
              intersections = scene.Intersectable.Intersect(i.CoordWorld, dir);
              Statistics.allRaysCount++;
              Intersection si = Intersection.FirstRealIntersection(intersections, ref dir);
              // Better shadow testing: intersection between 0.0 and 1.0 kills the lighting.
              if (si != null && !si.Far(1.0, ref dir))
                continue;
            }

            double[] reflection = i.ReflectanceModel.ColorReflection(i, dir, p1, ReflectionComponent.ALL);
            if (reflection != null)
            {
              Util.ColorAdd(intensity, reflection, color);
              hash = hash * HASH_LIGHT + source.GetHashCode();
            }
          }
        }
      }

      // Check the recursion depth.
      if (depth++ >= MaxLevel || !DoReflections && !DoRefractions)
        // No further recursion.
        return hash;

      Vector3d r;
      double   maxK;
      double   newImportance;

      if (DoReflections)
      {
        // Shooting a reflected ray.
        Geometry.SpecularReflection(ref i.Normal, ref p1, out r);
        double[] ks = i.ReflectanceModel.ColorReflection(i, p1, r, ReflectionComponent.SPECULAR_REFLECTION);
        if (ks != null)
        {
          maxK = ks[0];
          for (int b = 1; b < bands; b++)
            if (ks[b] > maxK)
              maxK = ks[b];

          newImportance = importance * maxK;
          if (newImportance >= MinImportance)
          {
            // Do compute the reflected ray.
            hash += HASH_REFLECT * shade(depth, newImportance, ref i.CoordWorld, ref r, comp);
            Util.ColorAdd(comp, ks, color);
          }
        }
      }

      if (DoRefractions)
      {
        // Shooting a refracted ray.
        maxK = i.Material.Kt;   // simple solution - no influence of reflectance model yet
        newImportance = importance * maxK;
        if (newImportance < MinImportance)
          return hash;

        // Refracted ray.
        if ((r = Geometry.SpecularRefraction(i.Normal, i.Material.n, p1)) == Vector3d.Zero)
          return hash;

        hash += HASH_REFRACT * shade(depth, newImportance, ref i.CoordWorld, ref r, comp);
        Util.ColorAdd(comp, maxK, color);
      }

      return hash;
    }
  }
}
