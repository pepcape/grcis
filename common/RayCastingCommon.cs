using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.IO;
using System.Diagnostics;
using OpenTK;

// Common code for ray-based rendering.
namespace Rendering
{
  #region Interfaces

  /// <summary>
  /// Function from continuous 2D space (virtual screen) to some color space.
  /// Usually does all the rendering job except for anti-aliasing.
  /// </summary>
  public interface IImageFunction
  {
    /// <summary>
    /// Domain width.
    /// </summary>
    double Width
    {
      get;
      set;
    }

    /// <summary>
    /// Domain height.
    /// </summary>
    double Height
    {
      get;
      set;
    }

    /// <summary>
    /// Computes one image sample.
    /// </summary>
    /// <param name="x">Horizontal coordinate.</param>
    /// <param name="y">Vertical coordinate.</param>
    /// <param name="color">Computed sample color.</param>
    /// <returns>Hash-value used for adaptive subsampling.</returns>
    long GetSample ( double x, double y, double[] color );
  }

  /// <summary>
  /// Algorithm capable of synthesizing raster image from virtual 3D scene.
  /// Usually associated with an IImageFunction object (which does the actual job).
  /// </summary>
  public interface IRenderer
  {
    /// <summary>
    /// Image width in pixels.
    /// </summary>
    int Width
    {
      get;
      set;
    }

    /// <summary>
    /// Image height in pixels.
    /// </summary>
    int Height
    {
      get;
      set;
    }

    /// <summary>
    /// Renders the single pixel of an image.
    /// </summary>
    /// <param name="x">Horizontal coordinate.</param>
    /// <param name="y">Vertical coordinate.</param>
    /// <param name="color">Computed pixel color.</param>
    void RenderPixel ( int x, int y, double[] color );

    /// <summary>
    /// Renders the given rectangle into the given raster image.
    /// </summary>
    /// <param name="image">Pre-initialized raster image.</param>
    /// <param name="x1"></param>
    /// <param name="y1"></param>
    /// <param name="x2"></param>
    /// <param name="y2"></param>
    void RenderRectangle ( Bitmap image, int x1, int y1, int x2, int y2 );
  }

  /// <summary>
  /// Ray generator (camera for ray-based methods).
  /// </summary>
  public interface ICamera
  {
    /// <summary>
    /// Width / Height of the viewing area (viewport, frustum).
    /// </summary>
    double AspectRatio
    {
      get;
      set;
    }

    /// <summary>
    /// Viewport width.
    /// </summary>
    double Width
    {
      get;
      set;
    }

    /// <summary>
    /// Viewport height.
    /// </summary>
    double Height
    {
      get;
      set;
    }

    /// <summary>
    /// Ray-generator.
    /// </summary>
    /// <param name="x">Origin position within a viewport (horizontal coordinate).</param>
    /// <param name="y">Origin position within a viewport (vertical coordinate).</param>
    /// <param name="p0">Ray origin.</param>
    /// <param name="p1">Ray direction vector.</param>
    /// <returns>True if the ray (viewport position) is valid.</returns>
    bool GetRay ( double x, double y, ref Vector4d p0, ref Vector3d p1 );
  }

  /// <summary>
  /// General light source.
  /// </summary>
  public interface ILightSource
  {
    /// <summary>
    /// Returns intensity (incl. color) of the source contribution to the given scene point.
    /// </summary>
    /// <param name="intersection">Scene point (only world coordinates and normal vector are needed).</param>
    /// <param name="dir">Direction to the source is set here (optional, can be null).</param>
    /// <returns>Intensity vector in current color space or null if the point is not lit.</returns>
    double[] GetIntensity ( Intersection intersection, ref Vector3d dir );
  }

  /// <summary>
  /// Reflection model - deals with local interaction between light and material surface.
  /// Entry point of the light ray is assumed to be equal to the exit point.
  /// </summary>
  public interface IReflectionModel
  {
    double[] ColorReflection ( IMaterial material, Intersection intersection, Vector3d input, Vector3d output );

    double[] ColorReflection ( IMaterial material, Vector3d normal, Vector3d input, Vector3d output );
  }

  /// <summary>
  /// Abstract material description.
  /// Each IReflectionModel should define its own derivation with specific properties.
  /// </summary>
  public interface IMaterial
  {
    /// <summary>
    /// Base surface color.
    /// </summary>
    double[] Color
    {
      get;
      set;
    }

    /// <summary>
    /// Absolute index of refraction.
    /// </summary>
    double n
    {
      get;
      set;
    }
  }

  /// <summary>
  /// Any object capable of ray-intersection in 3D.
  /// </summary>
  public interface IIntersectable
  {
    /// <summary>
    /// Computes the complete intersection of the given ray with the object. 
    /// </summary>
    /// <param name="p0">Ray origin.</param>
    /// <param name="p1">Ray direction vector.</param>
    /// <returns>Sorted list of intersection records.</returns>
    LinkedList<Intersection> Intersect ( Vector4d p0, Vector3d p1 );
  }

  /// <summary>
  /// Data container for Ray-based scene rendering: complete scene definition including camera.
  /// </summary>
  public interface IRayScene
  {
    /// <summary>
    /// Scene model (whatever is able to compute ray intersections).
    /// </summary>
    IIntersectable Intersectable
    {
      get;
      set;
    }

    /// <summary>
    /// Background color.
    /// </summary>
    double[] BackgroundColor
    {
      get;
      set;
    }

    /// <summary>
    /// Camera = primary ray generator.
    /// </summary>
    ICamera Camera
    {
      get;
      set;
    }

    /// <summary>
    /// Set of light sources.
    /// </summary>
    ICollection<ILightSource> Sources
    {
      get;
      set;
    }
  }

  #endregion

  #region Basic objects

  /// <summary>
  /// Intersection of a ray with a solid surface.
  /// </summary>
  public class Intersection
  {
    /// <summary>
    /// True if the ray enters a solid interior of an object here (transition from an air to solid material).
    /// </summary>
    public bool Enter
    {
      get;
      set;
    }

    /// <summary>
    /// True if the ray enters solid geometry from outside (ray and surface normal have different directions).
    /// </summary>
    public bool Front
    {
      get;
      set;
    }

    /// <summary>
    /// Parametric coordinate on the ray.
    /// </summary>
    public double T
    {
      get;
      set;
    }

    /// <summary>
    /// World coordinates of an intersection.
    /// </summary>
    public Vector4d CoordWorld
    {
      get;
      set;
    }

    /// <summary>
    /// Object coordinates of an intersection.
    /// </summary>
    public Vector4d CoordObject
    {
      get;
      set;
    }

    public Vector2d TextureCoord
    {
      get;
      set;
    }

    /// <summary>
    /// Normal vector in world coordinates.
    /// </summary>
    public Vector3d Normal
    {
      get;
      set;
    }

    public Matrix4d ObjectToWorld
    {
      get;
      set;
    }

    public Matrix4d WorldToObject
    {
      get;
      set;
    }

    public double[] SurfaceColor
    {
      get;
      set;
    }

    /// <summary>
    /// Solid this intersection comes from..
    /// </summary>
    public ISolid Solid
    {
      get;
      set;
    }

    /// <summary>
    /// Supplement data object for intersection completion.
    /// </summary>
    public object SolidData
    {
      get;
      set;
    }
  }

  #endregion

  #region Support

  public class Geometry
  {
    public static Vector3d specularRefraction ( Vector3d normal, double n, Vector3d input )
    {
      normal.Normalize();
      input.Normalize();
      double d = Vector3d.Dot( normal, input );

      if ( d < 0.0 )                        // (N*L) should be > 0.0 (N and L in the same half-space)
      {
        d  = -d;
        normal = normal * -1.0;
      }
      else
        n  = 1.0 / n;

      double cos2 = 1.0 - n * n * (1.0 - d * d);
      if ( cos2 <= 0.0 ) return Vector3d.Zero; // total reflection

      d = n * d - Math.Sqrt( cos2 );
      return( normal * d - input * n );
    }
  }

  #endregion
}
