using System;
using System.Collections.Generic;
using OpenTK;
using MathSupport;

namespace Rendering
{
  /// <summary>
  /// Ray-tracing rendering (w all secondary rays).
  /// </summary>
  [Serializable]
  public class RayTracing: RayCasting
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

    public RayTracing ( IRayScene sc ): base ( sc )
    {
      MaxLevel      = 12;
      MinImportance = 0.05;
      DoReflections =
        DoRefractions =
          DoShadows = true;
    }

    /// <summary>
    /// Computes one image sample. Internal integration support.
    /// </summary>
    /// <param name="x">Horizontal coordinate.</param>
    /// <param name="y">Vertical coordinate.</param>
    /// <param name="color">Computed sample color.</param>
    /// <returns>Hash-value used for adaptive subsampling.</returns>
    public override long GetSample ( double x, double y, double[] color )
    {
      MT.doubleX = x;
      MT.doubleY = y;

      // initial color = black
      Array.Clear ( color, 0, color.Length );

      Vector3d p0, p1;
      if ( !scene.Camera.GetRay ( x, y, out p0, out p1 ) )
        return 11L;

      long hash = shade ( 0, 1.0, ref p0, ref p1, color );

      return hash;
    }

    /// <summary>
    /// Recursive shading function - computes color contribution of the given ray (shot from the
    /// origin 'rayOrigin' into direction vector 'p1''). Recursion is stopped
    /// by a hybrid method: 'importance' and 'level' are checked.
    /// Internal integration support.
    /// </summary>
    /// <param name="level">Current recursion depth.</param>
    /// <param name="importance">Importance of the current ray.</param>
    /// <param name="p0">Ray origin.</param>
    /// <param name="p1">Ray direction vector.</param>
    /// <param name="color">Result color.</param>
    /// <returns>Hash-value (ray sub-signature) used for adaptive subsampling.</returns>
    protected virtual long shade ( int level, double importance, ref Vector3d p0, ref Vector3d p1,
                                   double[] color )
    {
      Vector3d direction = p1;

      int bands = color.Length;
      LinkedList<Intersection> intersections = scene.Intersectable.Intersect ( p0, p1 );

	    Statistics.IncrementRaysCounters ( 1, level == 0 );	// if ray is primary, increment both counters

      Intersection i = Intersection.FirstIntersection ( intersections, ref p1 );
      int b;

      if ( i == null ) // no intersection -> background color
      {
        RegisterRay ( RayType.rayVisualizerNormal, level, p0, direction * 1000 );

        Array.Copy ( scene.BackgroundColor, color, bands );
        return 1L;
      }

      // there was at least one intersection
      i.Complete ();

      RegisterRay ( RayType.unknown, level, p0, i ); // moved lower to also register rays for shadows
      Vertex vertex = new Vertex ( i.CoordWorld, new float[] { 1, 1, 1 }, i.Normal );
      Master.instance?.pointCloud?.cloud.Add ( vertex );

      // hash code for adaptive supersampling:
      long hash = i.Solid.GetHashCode ();

      // apply all the textures fist..
      if ( i.Textures != null )
        foreach ( ITexture tex in i.Textures )
          hash = hash * HASH_TEXTURE + tex.Apply ( i );

      p1 = -p1; // viewing vector
      p1.Normalize ();

      if ( scene.Sources == null || scene.Sources.Count < 1 )
        // no light sources at all
        Array.Copy ( i.SurfaceColor, color, bands );
      else
      {
        // apply the reflectance model for each source
        i.Material = (IMaterial) i.Material.Clone ();
        i.Material.Color = i.SurfaceColor;
        Array.Clear ( color, 0, bands );

        foreach ( ILightSource source in scene.Sources )
        {
          Vector3d dir;
          double[] intensity = source.GetIntensity ( i, out dir );

	        if ( MT.singleRayTracing && source.position != null )
		        // register shadow ray for RayVisualizer
		        RegisterRay ( RayType.rayVisualizerShadow, level, i.CoordWorld, (Vector3d) source.position );        

          if ( intensity != null )
          {
            if ( DoShadows && dir != Vector3d.Zero )
            {
              intersections = scene.Intersectable.Intersect ( i.CoordWorld, dir );
              Statistics.allRaysCount++;
              Intersection si = Intersection.FirstIntersection ( intersections, ref dir );
              // Better shadow testing: intersection between 0.0 and 1.0 kills the lighting
              if ( si != null && !si.Far ( 1.0, ref dir ) )
                continue;
            }

            double[] reflection = i.ReflectanceModel.ColorReflection ( i, dir, p1, ReflectionComponent.ALL );
            if ( reflection != null )
            {
              for ( b = 0; b < bands; b++ )
                color [ b ] += intensity [ b ] * reflection [ b ];
              hash = hash * HASH_LIGHT + source.GetHashCode ();
            }
          }
        }
      }

      // check the recursion depth:
      if ( level++ >= MaxLevel || !DoReflections && !DoRefractions )
        return hash; // no further recursion

      Vector3d r;
      double   maxK;
      double[] comp = new double[bands];
      double   newImportance;

      if ( DoReflections ) // trying to shoot a reflected ray
      {
        Geometry.SpecularReflection ( ref i.Normal, ref p1, out r );
        double[] ks = i.ReflectanceModel.ColorReflection ( i, p1, r, ReflectionComponent.SPECULAR_REFLECTION );
        if ( ks != null )
        {
          maxK = ks [ 0 ];
          for ( b = 1; b < bands; b++ )
            if ( ks [ b ] > maxK )
              maxK = ks [ b ];

          newImportance = importance * maxK;
          if ( newImportance >= MinImportance ) // do compute the reflected ray
          {
            hash += HASH_REFLECT * shade ( level, newImportance, ref i.CoordWorld, ref r, comp );
            for ( b = 0; b < bands; b++ )
              color [ b ] += ks [ b ] * comp [ b ];
          }
        }
      }

      if ( DoRefractions ) // trying to shoot a refracted ray
      {
        maxK = i.Material.Kt; // simple solution, no influence of reflectance model yet
        newImportance = importance * maxK;
        if ( newImportance < MinImportance )
          return hash;

        // refracted ray:
        if ( ( r = Geometry.SpecularRefraction ( i.Normal, i.Material.n, p1 ) ) == null )
          return hash;

        hash += HASH_REFRACT * shade ( level, newImportance, ref i.CoordWorld, ref r, comp );
        for ( b = 0; b < bands; b++ )
          color [ b ] += maxK * comp [ b ];
      }

      return hash;
    }


    /// <summary>
    /// Translate function for several ray register methods from AdvancedTools and RayVisualizer
    /// Be careful with params - not type safe
    /// </summary>
    /// <param name="rayType">RayType which chooses method to be called</param>
    /// <param name="parameters">All possible parameters that can be passed to individual ray register methods</param>
    private void RegisterRay ( RayType rayType, params object[] parameters )
    {
	    if ( rayType == RayType.unknown )
	    {
		    rayType = DetermineRayType ();
	    }

	    switch ( rayType )
	    {
	      //ray for statistics and maps (AdvancedTools)
        case RayType.mapsNormal:
	        //register ray for statistics and maps
          AdvancedTools.instance?.Register ( (int) parameters [ 0 ], (Vector3d) parameters [ 1 ], (Intersection) parameters [ 2 ] );
          break;

	      //ray for RayVisualizer
        case RayType.rayVisualizerNormal:
			    if ( !MT.singleRayTracing )
            return;
		      if ( parameters[2] is Vector3d vector )
		        RayVisualizer.instance?.RegisterRay ( (int) parameters[0], (Vector3d) parameters[1], vector );
		      if ( parameters[2] is Intersection intersection )
		        RayVisualizer.instance?.RegisterRay ( (int) parameters[0], (Vector3d) parameters[1], intersection.CoordWorld );
          break;

	      //shadow ray for RayVisualizer
        case RayType.rayVisualizerShadow:         
			    if ( !MT.singleRayTracing )
				    return;
          RayVisualizer.instance?.RegisterShadowRay ( (int) parameters [ 0 ], (Vector3d) parameters [ 1 ], (Vector3d) parameters [ 2 ] );
          break;
      }	    
    }

    /// <summary>
    /// Determined whether ray comes from normal rendering or single ray rendering (for information about pixel and RayVisualizer)
    /// </summary>
    /// <returns>Returns chosen RayType</returns>
	  private RayType DetermineRayType ()
	  {
		  if ( MT.singleRayTracing )
		  {
			  return RayType.rayVisualizerNormal;
		  }
		  else
		  {
			  return RayType.mapsNormal;
      }
	  }

    /// <summary>
    /// Types of rays
    /// Unknown used when further classification of it is needed and will be changed later
    /// </summary>
    enum RayType
	  {
		  rayVisualizerNormal,
		  rayVisualizerShadow,
		  mapsNormal,
			unknown
	  };
  }
}