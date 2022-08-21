using System;
using System.Collections.Generic;
using OpenTK;
using MathSupport;
using Utilities;
using Rendering;

namespace EduardHopfer
{
  /// <summary>
  /// Ray-tracing rendering (w all secondary rays).
  /// Modified shade function that accounts for the quirks of sphere casting
  /// used to render implicits
  /// </summary>
  [Serializable]
  public sealed class SphereTracing : RayTracing
  {
    public SphereTracing ()
    {
      MaxLevel      = 12;
      MinImportance = 0.05;
      DoReflections =
      DoRefractions =
      DoShadows     =
      DoRecursion   = true;
    }


    /// <summary>
    /// Computes one image sample. Internal integration support.
    /// </summary>
    /// <param name="x">Horizontal coordinate.</param>
    /// <param name="y">Vertical coordinate.</param>
    /// <param name="color">Computed sample color.</param>
    /// <returns>Hash-value used for adaptive subsampling.</returns>
    // public override long GetSample (double x, double y, double[] color)
    // {
    //   MT.doubleX = x;
    //   MT.doubleY = y;
    //
    //   // initial color = black
    //   Array.Clear(color, 0, color.Length);
    //
    //   Vector3d p0, p1;
    //   if (MT.scene.Camera.GetRay(x, y, out p0, out p1))
    //   {
    //     long hash = shade(0, 1.0, ref p0, ref p1, color);
    //
    //     return hash;
    //   }
    //
    //   return 11L;
    // }

    /// <summary>
    /// Recursive shading function - computes color contribution of the given ray (shot from the
    /// origin 'p0' into direction vector 'p1''). Recursion is stopped
    /// by a hybrid method: 'importance' and 'level' are checked.
    /// Internal integration support.
    /// </summary>
    /// <param name="depth">Current recursion depth.</param>
    /// <param name="importance">Importance of the current ray.</param>
    /// <param name="origin">Ray origin.</param>
    /// <param name="dir">Ray direction vector.</param>
    /// <param name="color">Result color.</param>
    /// <returns>Hash-value (ray sub-signature) used for adaptive subsampling.</returns>
    public override long shade (int depth,
                               double importance,
                               ref Vector3d origin,
                               ref Vector3d dir,
                               double[] color)
    {
      Vector3d direction = dir;

      int bands = color.Length;
      var intersections = MT.scene.Intersectable.Intersect(origin, dir);

      // If the ray is primary, increment both counters
      Statistics.IncrementRaysCounters(1, depth == 0);

      var i = Intersection.FirstIntersection(intersections, ref dir);

      if (i == null)
      {
        // No intersection -> background color
        rayRegisterer?.RegisterRay(AbstractRayRegisterer.RayType.rayVisualizerNormal, depth, origin, direction * 100000);

        return MT.scene.Background.GetColor(dir, color);
      }

      // There was at least one intersection
      i.Complete();

      rayRegisterer?.RegisterRay(AbstractRayRegisterer.RayType.unknown, depth, origin, i);

      // Hash code for adaptive supersampling
      long hash = i.Solid.GetHashCode();

      // Apply all the textures first
      if (i.Textures != null)
      {
        foreach (var tex in i.Textures)
        {
          hash = hash * HASH_TEXTURE + tex.Apply(i);
        }
      }

      // Point cloud shenanigans
      if (MT.pointCloudCheckBox && !MT.pointCloudSavingInProgress && !MT.singleRayTracing)
      {
        foreach (Intersection intersection in intersections)
        {
          if (!intersection.completed)
          {
            intersection.Complete();
          }

          if (intersection.Textures != null && !intersection.textureApplied)
          {
            foreach (var tex in intersection.Textures)
            {
              tex.Apply(intersection);
            }
          }

          double[] vertexColor = new double[3];
          Util.ColorCopy(intersection.SurfaceColor, vertexColor);
          Master.singleton?.pointCloud?.AddToPointCloud(intersection.CoordWorld, vertexColor, intersection.Normal, MT.threadID);
        }
      }

      // Color accumulation.
      Array.Clear(color, 0, bands);
      double[] comp = new double[bands];

      dir = -dir; // Reflect the viewing vector
      dir.Normalize();

      if (MT.scene.Sources == null ||
          MT.scene.Sources.Count < 1)
      {
        // No light sources at all.
        Util.ColorAdd(i.SurfaceColor, color);
      }
      else
      {
        // Apply the reflectance model for each source.
        i.Material = (IMaterial)i.Material.Clone();
        i.Material.Color = i.SurfaceColor;

        foreach (ILightSource source in MT.scene.Sources)
        {
          double[] intensity = source.GetIntensity(i, out Vector3d toLight);

          if (MT.singleRayTracing && source.position != null)
          {
            // Register shadow ray for RayVisualizer.
            rayRegisterer?.RegisterRay(AbstractRayRegisterer.RayType.rayVisualizerShadow, i.CoordWorld, (Vector3d)source.position);
          }

          if (intensity != null)
          {
            if (DoShadows && toLight != Vector3d.Zero)
            {
              toLight.Normalize(); // We need the direction vector to be normalized for proper sphere tracing
              var newOrigin = ImplicitCommon.GetNextRayOrigin(i, RayType.SHADOW);
              intersections = MT.scene.Intersectable.Intersect(newOrigin, toLight);

              Statistics.allRaysCount++;
              Intersection si = Intersection.FirstRealIntersection(intersections, ref toLight);
              // Better shadow testing: intersection between 0.0 and 1.0 kills the lighting.
              if (si != null && !si.Far(1.0, ref toLight))
              {
                continue;
              }
            }

            double[] reflection = i.ReflectanceModel.ColorReflection(i, toLight, dir, ReflectionComponent.ALL);
            if (reflection != null)
            {
              Util.ColorAdd(intensity, reflection, color);
              hash = hash * HASH_LIGHT + source.GetHashCode();
            }
          }
        }
      }

      // Check the recursion depth.
      if (depth++ >= MaxLevel || (!DoReflections && !DoRefractions))
      {
        // No further recursion.
        return hash;
      }

      Vector3d r;
      double   maxK;
      double   newImportance;

      if (DoReflections)
      {
        // Shooting a reflected ray.
        Geometry.SpecularReflection(ref i.Normal, ref dir, out r);
        double[] ks = i.ReflectanceModel.ColorReflection(i, dir, r, ReflectionComponent.SPECULAR_REFLECTION);
        if (ks != null)
        {
          maxK = ks[0];
          for (int b = 1; b < bands; b++)
            if (ks[b] > maxK)
              maxK = ks[b];

          newImportance = importance * maxK;
          if (newImportance >= MinImportance)
          {
            var newOrigin = ImplicitCommon.GetNextRayOrigin(i, RayType.REFLECT);

            // Do compute the reflected ray.
            hash += HASH_REFLECT * shade(depth, newImportance, ref newOrigin, ref r, comp);
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
        if ((r = Geometry.SpecularRefraction(i.Normal, i.Material.n, dir)) == Vector3d.Zero)
          return hash;

        var newOrigin = ImplicitCommon.GetNextRayOrigin(i, RayType.REFRACT);
        hash += HASH_REFRACT * shade(depth, newImportance, ref newOrigin, ref r, comp);
        Util.ColorAdd(comp, maxK, color);
      }

      return hash;
    }
  }
}
