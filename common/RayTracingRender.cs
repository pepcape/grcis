using System;
using System.Collections.Generic;
using OpenTK;
using MathSupport;

namespace Rendering
{
  /// <summary>
  /// Ray-tracing rendering (w all secondary rays).
  /// </summary>
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
    public int MaxLevel
    {
      get;
      set;
    }

    /// <summary>
    /// Recursion-termination parameter - minimal importance value which causes
    /// ray reflection and/or refraction.
    /// </summary>
    public double MinImportance
    {
      get;
      set;
    }

    /// <summary>
    /// Compute reflected secondary rays?
    /// </summary>
    public bool DoReflections
    {
      get;
      set;
    }

    /// <summary>
    /// Compute refracted secondary rays?
    /// </summary>
    public bool DoRefractions
    {
      get;
      set;
    }

    /// <summary>
    /// Use secondary shadow rays to determine light source visibility?
    /// </summary>
    public bool DoShadows
    {
      get;
      set;
    }

    public RayTracing ( IRayScene sc ) : base( sc )
    {
      MaxLevel = 12;
      MinImportance = 0.05;
      DoReflections =
      DoRefractions =
      DoShadows     = true;
    }

    /// <summary>
    /// Computes one image sample. Internal integration support.
    /// </summary>
    /// <param name="x">Horizontal coordinate.</param>
    /// <param name="y">Vertical coordinate.</param>
    /// <param name="rank">Rank of this sample, 0 <= rank < total (for integration).</param>
    /// <param name="total">Total number of samples (for integration).</param>
    /// <param name="rnd">Global (per-thread) instance of the random generator.</param>
    /// <param name="color">Computed sample color.</param>
    /// <returns>Hash-value used for adaptive subsampling.</returns>
    public override long GetSample ( double x, double y, int rank, int total, RandomJames rnd, double[] color )
    {
      // initial color = black
      Array.Clear( color, 0, color.Length );

      Vector3d p0, p1;
      if ( !scene.Camera.GetRay( x, y, rank, total, rnd, out p0, out p1 ) )
        return 11L;

      long hash = shade( 0, 1.0, ref p0, ref p1, rank, total, rnd, color );

      return hash;
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
    /// <param name="rank">Rank of this sample, 0 <= rank < total (for integration).</param>
    /// <param name="total">Total number of samples (for integration).</param>
    /// <param name="rnd">Global (per-thread) instance of the random generator.</param>
    /// <param name="color">Result color.</param>
    /// <returns>Hash-value (ray sub-signature) used for adaptive subsampling.</returns>
    protected virtual long shade ( int level, double importance, ref Vector3d p0, ref Vector3d p1,
                                   int rank, int total, RandomJames rnd, double[] color )
    {
      int bands = color.Length;
      LinkedList<Intersection> intersections = scene.Intersectable.Intersect( p0, p1 );
      Intersection.countRays++;
      Intersection i = Intersection.FirstIntersection( intersections, ref p1 );
      int b;

      if ( i == null )          // no intersection -> background color
      {
        Array.Copy( scene.BackgroundColor, color, bands );
        return 1L;
      }

      // there was at least one intersection
      i.Complete();

      // hash code for adaptive supersampling:
      long hash = i.Solid.GetHashCode();

      // apply all the textures fist..
      if ( i.Textures != null )
        foreach ( ITexture tex in i.Textures )
          hash = hash * HASH_TEXTURE + tex.Apply( i, rank, total, rnd );

      p1 = -p1;   // viewing vector
      p1.Normalize();

      if ( scene.Sources == null || scene.Sources.Count < 1 )
        // no light sources at all
        Array.Copy( i.SurfaceColor, color, bands );
      else
      {
        // apply the reflectance model for each source
        i.Material = (IMaterial)i.Material.Clone();
        i.Material.Color = i.SurfaceColor;
        Array.Clear( color, 0, bands );

        foreach ( ILightSource source in scene.Sources )
        {
          Vector3d dir;
          double[] intensity = source.GetIntensity( i, rank, total, rnd, out dir );
          if ( intensity != null )
          {
            if ( DoShadows && !dir.Equals( Vector3d.Zero ) )
            {
              intersections = scene.Intersectable.Intersect( i.CoordWorld, dir );
              Intersection.countRays++;
              Intersection si = Intersection.FirstIntersection( intersections, ref dir );
              // Better shadow testing: intersection between 0.0 and 1.0 kills the lighting
              if ( si != null && !si.Far( 1.0, ref dir ) ) continue;
            }

            double[] reflection = i.ReflectanceModel.ColorReflection( i, dir, p1, ReflectionComponent.ALL );
            if ( reflection != null )
            {
              for ( b = 0; b < bands; b++ )
                color[ b ] += intensity[ b ] * reflection[ b ];
              hash = hash * HASH_LIGHT + source.GetHashCode();
            }
          }
        }
      }

      // check the recursion depth:
      if ( level++ >= MaxLevel ||
           !DoReflections && !DoRefractions )
        return hash;                        // no further recursion

      Vector3d r;
      double maxK;
      double[] comp = new double[ bands ];
      double newImportance;

      if ( DoReflections )                  // trying to shoot a reflected ray..
      {
        Geometry.SpecularReflection( ref i.Normal, ref p1, out r );
        double[] ks = i.ReflectanceModel.ColorReflection( i, p1, r, ReflectionComponent.SPECULAR_REFLECTION );
        if ( ks != null )
        {
          maxK = ks[ 0 ];
          for ( b = 1; b < bands; b++ )
            if ( ks[ b ] > maxK )
              maxK = ks[ b ];

          newImportance = importance * maxK;
          if ( newImportance >= MinImportance ) // do compute the reflected ray
          {
            hash += HASH_REFLECT * shade( level, newImportance, ref i.CoordWorld, ref r, rank, total, rnd, comp );
            for ( b = 0; b < bands; b++ )
              color[ b ] += ks[ b ] * comp[ b ];
          }
        }
      }

      if ( DoRefractions )                  // trying to shoot a refracted ray..
      {
        maxK = i.Material.Kt;               // simple solution, no influence of reflectance model yet
        newImportance = importance * maxK;
        if ( newImportance < MinImportance )
          return hash;

        // refracted ray:
        if ( (r = Geometry.SpecularRefraction( i.Normal, i.Material.n, p1 )) == null )
          return hash;

        hash += HASH_REFRACT * shade( level, newImportance, ref i.CoordWorld, ref r, rank, total, rnd, comp );
        for ( b = 0; b < bands; b++ )
          color[ b ] += maxK * comp[ b ];
      }

      return hash;
    }
  }

}
