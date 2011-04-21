using System;
using System.Collections.Generic;
using System.Drawing;
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
    bool GetRay ( double x, double y, out Vector3d p0, out Vector3d p1 );
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
    LinkedList<Intersection> Intersect ( Vector3d p0, Vector3d p1 );

    /// <summary>
    /// Complete all relevant items in the given Intersection object.
    /// </summary>
    /// <param name="inter">Intersection instance to complete.</param>
    void CompleteIntersection ( Intersection inter );
  }

  /// <summary>
  /// Texture object: general value-modulator (value = color, normal vector..).
  /// </summary>
  public interface ITexture
  {
    /// <summary>
    /// Apply the relevant value-modulation in the given Intersection instance.
    /// </summary>
    /// <param name="inter">Data object to modify.</param>
    void Apply ( Intersection inter );
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
    /// (mandatory: Solid, InnerNode)
    /// </summary>
    public bool Enter
    {
      get;
      set;
    }

    /// <summary>
    /// True if the ray enters solid geometry from outside (ray and surface normal have different directions).
    /// (mandatory: Solid)
    /// </summary>
    public bool Front
    {
      get;
      set;
    }

    /// <summary>
    /// Parametric coordinate on the ray
    /// (mandatory: Solid).
    /// </summary>
    public double T
    {
      get;
      set;
    }

    /// <summary>
    /// World coordinates of an intersection.
    /// (deferred: Scene graph)
    /// </summary>
    public Vector3d CoordWorld
    {
      get;
      set;
    }

    /// <summary>
    /// Object coordinates of an intersection (object is an animation unit).
    /// (deferred: Scene graph)
    /// </summary>
    public Vector3d CoordObject
    {
      get;
      set;
    }

    /// <summary>
    /// Local (solid's) coordinates of an intersection.
    /// (deferred: Solid)
    /// </summary>
    public Vector3d CoordLocal
    {
      get;
      set;
    }

    /// <summary>
    /// 2D texture coordinates.
    /// (deferred: Solid)
    /// </summary>
    public Vector2d TextureCoord
    {
      get;
      set;
    }

    /// <summary>
    /// Normal vector in world coordinates.
    /// (deferred: Solid)
    /// </summary>
    public Vector3d Normal
    {
      get;
      set;
    }

    /// <summary>
    /// Transform matrix from the local (Solid) space to the World space.
    /// (deferred: Scene graph)
    /// </summary>
    public Matrix4d LocalToWorld
    {
      get;
      set;
    }

    /// <summary>
    /// Transform matrix from the World space to the local (Solid) space.
    /// (deferred: Scene graph)
    /// </summary>
    public Matrix4d WorldToLocal
    {
      get;
      set;
    }

    /// <summary>
    /// Transform matrix from the local (Solid) space to the Object space.
    /// (deferred: Scene graph)
    /// </summary>
    public Matrix4d LocalToObject
    {
      get;
      set;
    }

    /// <summary>
    /// Current surface color, used by textures and utilized by rendering algorithm in the end.
    /// (deferred: Scene graph, textures..)
    /// </summary>
    public double[] SurfaceColor
    {
      get;
      set;
    }

    /// <summary>
    /// Priority-list of relevant textures (less important to more important).
    /// </summary>
    public LinkedList<ITexture> Textures
    {
      get;
      set;
    }

    /// <summary>
    /// Solid this intersection comes from..
    /// (mandatory: Solid)
    /// </summary>
    public ISolid Solid
    {
      get;
      set;
    }

    /// <summary>
    /// Supplement data object for intersection completion.
    /// (mandatory: Solid)
    /// </summary>
    public object SolidData
    {
      get;
      set;
    }

    public Intersection ( ISolid s )
    {
      Solid = s;
    }

    /// <summary>
    /// Complete non-mandatory (deferred) values in the Intersection instance.
    /// </summary>
    public void Complete ()
    {
      if ( Solid != null )
      {
        // world coordinates:
        LocalToWorld = Solid.ToWorld();
        WorldToLocal = LocalToWorld;
        WorldToLocal.Invert();
        CoordWorld = Vector3d.TransformPosition( CoordLocal, LocalToWorld );

        // object coordinates:
        LocalToObject = Solid.ToObject();
        CoordObject = Vector3d.TransformPosition( CoordLocal, LocalToObject );

        // appearance:
        SurfaceColor = (double[])Solid.GetAttribute( PropertyName.COLOR );
        Textures = Solid.GetTextures();

        // Solid is responsible for completing remaining values:
        Solid.CompleteIntersection( this );
        if ( Enter != Front )
          Normal = Vector3d.Multiply( Normal, -1.0 );
      }
    }
  }

  #endregion

  #region Support

  public class Geometry
  {
    public static bool IsZero ( double a )
    {
      return( a <= Double.Epsilon && a >= -Double.Epsilon );
    }

    public static Vector3d SpecularRefraction ( Vector3d normal, double n, Vector3d input )
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

    /// <summary>
    /// Finds two other axes to the given vector, their vector product will
    /// give the original vector, all three vectors will be perpendicular to each other.
    /// </summary>
    /// <param name="p"></param>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    public static void GetAxes ( Vector3d p, out Vector3d p1, out Vector3d p2 )
    {
      double ax = Math.Abs( p.X );
      double ay = Math.Abs( p.Y );
      double az = Math.Abs( p.Z );

      if ( ax >= az &&
           ay >= az )                       // ax, ay are dominant
      {
        p1.X = -p.Y;
        p1.Y =  p.X;
        p1.Z =  0.0;
      }
      else
      if ( ax >= ay &&
           az >= ay )                       // ax, az are dominant
      {
        p1.X = -p.Z;
        p1.Y =  0.0;
        p1.Z =  p.X;
      }
      else                                  // ay, az are dominant
      {
        p1.X =  0.0;
        p1.Y = -p.Z;
        p1.Z =  p.Y;
      }

      Vector3d.Cross( ref p, ref p1, out p2 );
    }
  }

  #endregion
}
