using System;
using System.Collections.Generic;
using OpenTK;

namespace Rendering
{
  /// <summary>
  /// Ray-casting rendering (ray-tracing w/o all secondary rays).
  /// </summary>
  public class RayCasting : IImageFunction
  {
    /// <summary>
    /// Domain width.
    /// </summary>
    public double Width
    {
      get { return scene.Camera.Width; }
      set { scene.Camera.Width = value; }
    }

    /// <summary>
    /// Domain height.
    /// </summary>
    public double Height
    {
      get { return scene.Camera.Height; }
      set { scene.Camera.Height = value; }
    }

    protected IRayScene scene;

    public RayCasting ( IRayScene sc )
    {
      scene = sc;
    }

    /// <summary>
    /// Computes one image sample.
    /// </summary>
    /// <param name="x">Horizontal coordinate.</param>
    /// <param name="y">Vertical coordinate.</param>
    /// <param name="color">Computed sample color.</param>
    /// <returns>Hash-value used for adaptive subsampling.</returns>
    public virtual long GetSample ( double x, double y, double[] color )
    {
      Vector3d p0, p1;
      Array.Copy( scene.BackgroundColor, color, Math.Min( scene.BackgroundColor.Length, color.Length ) );
      if ( !scene.Camera.GetRay( x, y, out p0, out p1 ) )
        return 1L;

      LinkedList<Intersection> intersections = scene.Intersectable.Intersect( p0, p1 );
      if ( intersections != null && intersections.Count > 0 )
      {
        Intersection i = intersections.First.Value;
        i.Complete();

        if ( i.Textures != null )
          foreach ( ITexture tex in i.Textures )
            tex.Apply( i );

        Array.Copy( i.SurfaceColor, color, Math.Min( i.SurfaceColor.Length, color.Length ) );

        return 12L;
      }

      return 0L;
    }

  }

}
