using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.IO;
using System.Diagnostics;
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
        double[] col = (double[])i.Solid.GetAttribute( PropertyName.COLOR );
        if ( col != null )
          Array.Copy( col, color, Math.Min( col.Length, color.Length ) );
        return 12L;
      }

      return 0L;
    }

  }

}
