using System;
using System.Collections.Generic;
using OpenTK;
using MathSupport;
using Utilities;

namespace Rendering
{
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

    public RayTracing (IRayScene sc = null)
      : base(sc)
    {
      MaxLevel      = 12;
      MinImportance = 0.05;
      DoReflections =
      DoRefractions =
      DoShadows     = true;
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
    /// <param name="level">Current recursion depth.</param>
    /// <param name="importance">Importance of the current ray.</param>
    /// <param name="p0">Ray origin.</param>
    /// <param name="p1">Ray direction vector.</param>
    /// <param name="color">Result color.</param>
    /// <returns>Hash-value (ray sub-signature) used for adaptive subsampling.</returns>
    protected virtual long shade (int level,
                                  double importance,
                                  ref Vector3d p0,
                                  ref Vector3d p1,
                                  double[] color)
    {
      Vector3d direction = p1;

      int bands = color.Length;
      LinkedList<Intersection> intersections = scene.Intersectable.Intersect(p0, p1);

      // If the ray is primary, increment both counters
      Statistics.IncrementRaysCounters(1, level == 0);

      Intersection i = Intersection.FirstIntersection(intersections, ref p1);
      int b;

      if (i == null)
      {
        // No intersection -> background color
        rayRegisterer?.RegisterRay(AbstractRayRegisterer.RayType.rayVisualizerNormal, level, p0, direction * 100000);

        return scene.Background.GetColor(p1, color);
      }

      // There was at least one intersection
      i.Complete();

      rayRegisterer?.RegisterRay(AbstractRayRegisterer.RayType.unknown, level, p0, i);

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

      p1 = -p1; // viewing vector
      p1.Normalize();

      // !!! TODO: optional light-source processing (controlled by an attribute?) !!!

      if (scene.Sources == null || scene.Sources.Count < 1)
        // No light sources at all.
        Util.ColorCopy(i.SurfaceColor, color);
      else
      {
        // Apply the reflectance model for each source.
        i.Material = (IMaterial)i.Material.Clone();
        i.Material.Color = i.SurfaceColor;
        Array.Clear(color, 0, bands);

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
              Intersection si = Intersection.FirstIntersection(intersections, ref dir);
              // Better shadow testing: intersection between 0.0 and 1.0 kills the lighting.
              if (si != null && !si.Far(1.0, ref dir))
                continue;
            }

            double[] reflection = i.ReflectanceModel.ColorReflection(i, dir, p1, ReflectionComponent.ALL);
            if (reflection != null)
            {
              for (b = 0; b < bands; b++)
                color[b] += intensity[b] * reflection[b];
              hash = hash * HASH_LIGHT + source.GetHashCode();
            }
          }
        }
      }

      // Check the recursion depth.
      if (level++ >= MaxLevel || !DoReflections && !DoRefractions)
        // No further recursion.
        return hash;

      Vector3d r;
      double   maxK;
      double[] comp = new double[bands];
      double   newImportance;

      // !!! TODO: alternative intersection handling, different from reflection + refraction !!!
      // Controlled by an attribute (containing a callback-function)?

      if (DoReflections)
      {
        // Shooting a reflected ray.
        Geometry.SpecularReflection(ref i.Normal, ref p1, out r);
        double[] ks = i.ReflectanceModel.ColorReflection(i, p1, r, ReflectionComponent.SPECULAR_REFLECTION);
        if (ks != null)
        {
          maxK = ks[0];
          for (b = 1; b < bands; b++)
            if (ks[b] > maxK)
              maxK = ks[b];

          newImportance = importance * maxK;
          if (newImportance >= MinImportance)
          {
            // Do compute the reflected ray.
            hash += HASH_REFLECT * shade(level, newImportance, ref i.CoordWorld, ref r, comp);
            for (b = 0; b < bands; b++)
              color[b] += ks[b] * comp[b];
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

        hash += HASH_REFRACT * shade(level, newImportance, ref i.CoordWorld, ref r, comp);
        for (b = 0; b < bands; b++)
          color[b] += maxK * comp[b];
      }

      return hash;
    }
  }
}
