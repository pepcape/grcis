using System;
using System.Collections.Generic;
using OpenTK;

namespace Rendering
{
  /// <summary>
  /// Ray-tracing rendering (w all secondary rays).
  /// </summary>
  public class RayTracing : RayCasting
  {
    public RayTracing ( IRayScene sc ) : base( sc )
    { }

    /// <summary>
    /// Computes one image sample.
    /// </summary>
    /// <param name="x">Horizontal coordinate.</param>
    /// <param name="y">Vertical coordinate.</param>
    /// <param name="color">Computed sample color.</param>
    /// <returns>Hash-value used for adaptive subsampling.</returns>
    public override long GetSample ( double x, double y, double[] color )
    {
      Vector3d p0, p1;
      if ( !scene.Camera.GetRay( x, y, out p0, out p1 ) )
      {
        Array.Clear( color, 0, color.Length );                  // invalid ray -> black color
        return 1L;
      }

      LinkedList<Intersection> intersections = scene.Intersectable.Intersect( p0, p1 );
      if ( intersections == null || intersections.Count == 0 )  // no intersection -> background color
      {
        Array.Copy( scene.BackgroundColor, color, Math.Min( scene.BackgroundColor.Length, color.Length ) );
        return 0L;
      }

      // there was at least one intersection
      Intersection i = intersections.First.Value;
      i.Complete();

      // hash code for adaptive supersampling:
      long hash = i.Solid.GetHashCode();

      // apply all the textures fist..
      if ( i.Textures != null )
        foreach ( ITexture tex in i.Textures )
          hash = hash * HASH_TEXTURE + tex.Apply( i );

      // terminate if light sources are missing
      if ( scene.Sources == null || scene.Sources.Count < 1 )
      {
        Array.Copy( i.SurfaceColor, color, Math.Min( i.SurfaceColor.Length, color.Length ) );
        return hash;
      }

      // .. else apply the reflectance model for each source
      p1 = -p1;
      p1.Normalize();
      double[] colorBak = i.Material.Color;
      i.Material.Color = i.SurfaceColor;
      Array.Clear( color, 0, color.Length );

      foreach ( ILightSource source in scene.Sources )
      {
        Vector3d dir;
        double[] intensity = source.GetIntensity( i, out dir );
        if ( intensity != null )
        {
          double[] reflection = i.ReflectanceModel.ColorReflection( i.Material, i, dir, p1 );
          if ( reflection != null )
          {
            int bands = Math.Min( color.Length, Math.Min( reflection.Length, intensity.Length ) );
            for ( int b = 0; b < bands; b++ )
              color[ b ] += intensity[ b ] * reflection[ b ];
            hash = hash * HASH_LIGHT + source.GetHashCode();
          }
        }
      }

      i.Material.Color = colorBak;
      return hash;
    }
  }

}
