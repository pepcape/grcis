using System;
using System.Collections.Generic;
using System.Drawing;
using MathSupport;
using OpenTK;

/// <summary>
/// Common code for ray-based rendering.
/// </summary>
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
    double Width { get; set; }

    /// <summary>
    /// Domain height.
    /// </summary>
    double Height { get; set; }

    /// <summary>
    /// Computes one image sample. Simple variant, w/o an integration support.
    /// </summary>
    /// <param name="x">Horizontal coordinate.</param>
    /// <param name="y">Vertical coordinate.</param>
    /// <param name="color">Computed sample color.</param>
    /// <returns>Hash-value used for adaptive subsampling.</returns>
    long GetSample (double x, double y, double[] color);
  }

  /// <summary>
  /// Delegate function for thread-selection.
  /// </summary>
  /// <param name="unit">Number of working unit (thread selected for unit==0 is leader).</param>
  public delegate bool ThreadSelector (long unit);

  /// <summary>
  /// Algorithm capable of synthesizing raster image from other representation (e.g. IImageFunction).
  /// Usually associated with an IImageFunction object (which does the actual job).
  /// </summary>
  public interface IRenderer
  {
    /// <summary>
    /// Image width in pixels.
    /// </summary>
    int Width { get; set; }

    /// <summary>
    /// Image height in pixels.
    /// </summary>
    int Height { get; set; }

    /// <summary>
    /// Gamma pre-compensation (gamma encoding, compression). Values 0.0 or 1.0 mean "no compensation".
    /// </summary>
    double Gamma { get; set; }

    /// <summary>
    /// Cell-size for adaptive rendering, 0 or 1 to turn off adaptivitiy.
    /// </summary>
    int Adaptive { get; set; }

    /// <summary>
    /// Current progress object (can be null).
    /// </summary>
    Progress ProgressData { get; set; }

    /// <summary>
    /// Sample-based rendering specifics: rendered image is defined by
    /// a continuous-argument image function.
    /// Need not be used if IRenderer is not image-based.
    /// </summary>
    IImageFunction ImageFunction { get; set; }

    /// <summary>
    /// Renders the single pixel of an image.
    /// </summary>
    /// <param name="x">Horizontal coordinate.</param>
    /// <param name="y">Vertical coordinate.</param>
    /// <param name="color">Computed pixel color.</param>
    void RenderPixel (int x, int y, double[] color);

    /// <summary>
    /// Renders the given rectangle into the given raster image.
    /// </summary>
    /// <param name="image">Pre-initialized raster image.</param>
    void RenderRectangle (Bitmap image, int x1, int y1, int x2, int y2);

    /// <summary>
    /// Renders the given rectangle into the given raster image.
    /// Has to be re-entrant since this code is started in multiple parallel threads.
    /// </summary>
    /// <param name="image">Pre-initialized raster image.</param>
    /// <param name="sel">Selector for this working thread.</param>
    void RenderRectangle (Bitmap image, int x1, int y1, int x2, int y2, ThreadSelector sel);
  }

  /// <summary>
  /// Ray generator (camera for ray-based methods).
  /// </summary>
  public interface ICamera
  {
    /// <summary>
    /// Width / Height of the viewing area (viewport, frustum).
    /// </summary>
    double AspectRatio { get; set; }

    /// <summary>
    /// Viewport width.
    /// </summary>
    double Width { get; set; }

    /// <summary>
    /// Viewport height.
    /// </summary>
    double Height { get; set; }

    /// <summary>
    /// Ray-generator. Simple variant, w/o an integration support.
    /// </summary>
    /// <param name="x">Origin position within a viewport (horizontal coordinate).</param>
    /// <param name="y">Origin position within a viewport (vertical coordinate).</param>
    /// <param name="p0">Ray origin.</param>
    /// <param name="p1">Ray direction vector.</param>
    /// <returns>True if the ray (viewport position) is valid.</returns>
    bool GetRay (double x, double y, out Vector3d p0, out Vector3d p1);
  }

  /// <summary>
  /// General light source.
  /// </summary>
  public interface ILightSource
  {
    /// <summary>
    /// Returns intensity (incl. color) of the source contribution to the given scene point.
    /// Simple variant, w/o an integration support.
    /// </summary>
    /// <param name="intersection">Scene point (only world coordinates and normal vector are needed).</param>
    /// <param name="dir">Direction to the source is set here (zero vector for omnidirectional source). Not normalized!</param>
    /// <returns>Intensity vector in current color space or null if the point is not lit.</returns>
    double[] GetIntensity (Intersection intersection, out Vector3d dir);

    Vector3d? position { get; set; }
  }

  public enum ReflectionComponent
  {
    DIFFUSE             = 1,
    SPECULAR_REFLECTION = 2,
    SPECULAR_REFRACTION = 4,
    SPECULAR            = 6,
    ALL                 = 7
  }

  /// <summary>
  /// Reflection model - deals with local interaction between light and material surface.
  /// Entry point of the light ray is assumed to be equal to the exit point.
  /// </summary>
  public interface IReflectanceModel
  {
    IMaterial DefaultMaterial ();

    double[] ColorReflection (Intersection intersection, Vector3d input, Vector3d output, ReflectionComponent comp);

    double[] ColorReflection (IMaterial material, Vector3d normal, Vector3d input, Vector3d output, ReflectionComponent comp);
  }

  /// <summary>
  /// Abstract material description.
  /// Each IReflectionModel should define its own derivation with specific properties.
  /// </summary>
  public interface IMaterial : ICloneable
  {
    /// <summary>
    /// Base surface color.
    /// </summary>
    double[] Color { get; set; }

    /// <summary>
    /// Coefficient of transparency.
    /// </summary>
    double Kt { get; set; }

    /// <summary>
    /// Absolute index of refraction.
    /// </summary>
    double n { get; set; }
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
    LinkedList<Intersection> Intersect (Vector3d p0, Vector3d p1);

    /// <summary>
    /// Complete all relevant items in the given Intersection object.
    /// </summary>
    /// <param name="inter">Intersection instance to complete.</param>
    void CompleteIntersection (Intersection inter);
  }

  /// <summary>
  /// Simple bounding volume, able to compute the closest positive intersection.
  /// </summary>
  public interface IBoundingVolume
  {
    /// <summary>
    /// Bounding volume vs. ray intersection.
    /// </summary>
    /// <param name="p0">Ray origin.</param>
    /// <param name="p1">Ray direction vector.</param>
    /// <returns>Negative number (-1.0) if there is no intersection, zero if the origin is inside the solid,
    /// otherwise the closest intersection point.</returns>
    double Intersect (Vector3d p0, Vector3d p1);
  }

  /// <summary>
  /// Texture object: general value-modulator (value = color, normal vector..).
  /// </summary>
  public interface ITexture
  {
    /// <summary>
    /// Apply the relevant value-modulation in the given Intersection instance.
    /// Simple variant, w/o an integration support.
    /// </summary>
    /// <param name="inter">Data object to modify.</param>
    /// <returns>Hash value (texture signature) for adaptive subsampling.</returns>
    long Apply (Intersection inter);
  }

  /// <summary>
  /// Object providing bacground color for a ray-based rendering.
  /// </summary>
  public interface IBackground
  {
    /// <summary>
    /// Returns background color = function of direction vector.
    /// </summary>
    /// <param name="p1">Direction vector</param>
    /// <param name="c">Pre-allocated output color</param>
    long GetColor (Vector3d p1, double[] c);
  }

  /// <summary>
  /// Data container for Ray-based scene rendering: complete scene definition including camera.
  /// </summary>
  public interface IRayScene
  {
    /// <summary>
    /// Scene model (whatever is able to compute ray intersections).
    /// </summary>
    IIntersectable Intersectable { get; set; }

    /// <summary>
    /// Optional object for animations. It will be the first to receive new
    /// 'Time' values (before everything else in the scene).
    /// </summary>
    ITimeDependent Animator { get; set; }

    /// <summary>
    /// Background color object.
    /// </summary>
    IBackground Background { get; set; }

    /// <summary>
    /// Constant background color.
    /// </summary>
    double[] BackgroundColor { get; set; }

    /// <summary>
    /// Camera = primary ray generator.
    /// </summary>
    ICamera Camera { get; set; }

    /// <summary>
    /// Set of light sources.
    /// </summary>
    ICollection<ILightSource> Sources { get; set; }
  }

  /// <summary>
  /// Abstract time-dependency of an object.
  /// Each instance has to be able to clone itself (multi-thread implementations).
  /// </summary>
  public interface ITimeDependent : ICloneable
  {
#if DEBUG
    /// <summary>
    /// Debugging - tracking of object instances/clones.
    /// </summary>
    int getSerial ();
#endif

    /// <summary>
    /// Starting (minimal) time in seconds.
    /// </summary>
    double Start { get; set; }

    /// <summary>
    /// Ending (maximal) time in seconds.
    /// </summary>
    double End { get; set; }

    /// <summary>
    /// Current time in seconds.
    /// </summary>
    double Time { get; set; }
  }

  #endregion

  #region Basic objects

  /// <summary>
  /// Intersection of a ray with a solid surface.
  /// </summary>
  public class Intersection : IComparable
  {
    /// <summary>
    /// Thickness of very thin objects.
    /// </summary>
    public const double SHELL_THICKNESS = 1.0e-5;

    /// <summary>
    /// Smallest distance on the ray.
    /// </summary>
    public const double RAY_EPSILON = 1.0e-4;

    /// <summary>
    /// Smallest distance on the ray squared.
    /// </summary>
    public const double RAY_EPSILON2 = 1.0e-8;

    /// <summary>
    /// True if the ray enters a solid interior of an object here (transition from an air to solid material).
    /// (mandatory: Solid, InnerNode)
    /// </summary>
    public bool Enter;

    /// <summary>
    /// True if the ray enters solid geometry from outside (ray and surface normal have different directions).
    /// (mandatory: Solid)
    /// </summary>
    public bool Front;

    /// <summary>
    /// Parametric coordinate on the ray
    /// (mandatory: Solid).
    /// </summary>
    public double T;

    /// <summary>
    /// World coordinates of an intersection.
    /// (deferred: Scene graph)
    /// </summary>
    public Vector3d CoordWorld;

    /// <summary>
    /// Object coordinates of an intersection (object is an animation unit).
    /// (deferred: Scene graph)
    /// </summary>
    public Vector3d CoordObject;

    /// <summary>
    /// Local (solid's) coordinates of an intersection.
    /// (deferred: Solid)
    /// </summary>
    public Vector3d CoordLocal;

    /// <summary>
    /// 2D texture coordinates.
    /// (deferred: Solid)
    /// </summary>
    public Vector2d TextureCoord;

    /// <summary>
    /// Normal vector in world coordinates.
    /// (deferred: Solid)
    /// </summary>
    public Vector3d Normal;

    /// <summary>
    /// Transform matrix from the local (Solid) space to the World space.
    /// (deferred: Scene graph)
    /// </summary>
    public Matrix4d LocalToWorld;

    /// <summary>
    /// Transform matrix from the World space to the local (Solid) space.
    /// (deferred: Scene graph)
    /// </summary>
    public Matrix4d WorldToLocal;

    /// <summary>
    /// Transform matrix from the local (Solid) space to the Object space.
    /// (deferred: Scene graph)
    /// </summary>
    public Matrix4d LocalToObject;

    /// <summary>
    /// Current surface color, used by textures and utilized by rendering algorithm in the end.
    /// (deferred: Scene graph, textures..)
    /// </summary>
    public double[] SurfaceColor;

    /// <summary>
    /// Current reflectance model.
    /// (deferred: Scene graph, textures..)
    /// </summary>
    public IReflectanceModel ReflectanceModel;

    /// <summary>
    /// Matching material record.
    /// (deferred: Scene graph, textures..)
    /// </summary>
    public IMaterial Material;

    /// <summary>
    /// Priority-list of relevant textures (less important to more important).
    /// </summary>
    public LinkedList<ITexture> Textures;

    /// <summary>
    /// Solid this intersection comes from..
    /// (mandatory: Solid)
    /// </summary>
    public ISolid Solid;

    /// <summary>
    /// Supplement data object for intersection completion.
    /// (mandatory: Solid)
    /// </summary>
    public object SolidData;

    /// <summary>
    /// Number of rays casted into the scene.
    /// </summary>
    public static long countRays = 0L;

    /// <summary>
    /// Number of individual ray x solid intersections computed
    /// (each surface has its intersection).
    /// </summary>
    public static long countIntersections = 0L;

    public bool completed = false;

    // Warning - this bool is changed even when only one texture was applied - not all of them.
    public bool textureApplied = false;

    public Intersection (ISolid s)
    {
      Solid = s;
    }

    /// <summary>
    /// Complete non-mandatory (deferred) values in the Intersection instance.
    /// </summary>
    public void Complete ()
    {
      if (Solid != null)
      {
        // World coordinates.
        LocalToWorld = Solid.ToWorld();
        WorldToLocal = LocalToWorld;
        WorldToLocal.Invert();
        Vector3d.TransformPosition(ref CoordLocal, ref LocalToWorld, out CoordWorld);

        // Object coordinates.
        LocalToObject = Solid.ToObject();
        Vector3d.TransformPosition(ref CoordLocal, ref LocalToObject, out CoordObject);

        // Appearance.
        // Reflectance model.
        ReflectanceModel = (IReflectanceModel)Solid.GetAttribute(PropertyName.REFLECTANCE_MODEL);
        if (ReflectanceModel == null)
          ReflectanceModel = new PhongModel();

        // Material.
        Material = (IMaterial)Solid.GetAttribute(PropertyName.MATERIAL);
        if (Material == null)
          Material = ReflectanceModel.DefaultMaterial();

        // Surface color (accepts a color provided by a Solid).
        if (SurfaceColor == null)
        {
          double[] col = (double[]) Solid.GetAttribute(PropertyName.COLOR);
          SurfaceColor = (double[])((col != null) ? col.Clone() : Material.Color.Clone());
        }

        // List of textures.
        Textures = Solid.GetTextures();

        // Solid is responsible for completing remaining values.
        // Usually: Normal, TextureCoord.
        Solid.CompleteIntersection(this);
        Normal.Normalize();
        if (Enter != Front)
          Vector3d.Multiply(ref Normal, -1.0, out Normal);
      }

      if (SurfaceColor == null)
        SurfaceColor = new double[] { 0.0, 0.0, 0.0 };

      completed = true;
    }

    public static Intersection FirstIntersection (
      LinkedList<Intersection> list,
      ref Vector3d p1)
    {
      if (list == null || list.Count < 1)
        return null;

      double p1Squared = Vector3d.Dot(p1, p1);
      foreach (Intersection i in list)
        if (i.T > 0.0 &&
            i.T * i.T * p1Squared > RAY_EPSILON2)
          return i;

      return null;
    }

    public static Intersection FirstRealIntersection (
      LinkedList<Intersection> list,
      ref Vector3d p1)
    {
      if (list == null || list.Count < 1)
        return null;

      double p1Squared = Vector3d.Dot(p1, p1);
      foreach (Intersection i in list)
        if (i.T > 0.0 &&
            i.T * i.T * p1Squared > RAY_EPSILON2 &&
            i.Solid?.GetLocalAttribute(PropertyName.NO_SHADOW) == null)
          return i;

      return null;
    }

    /// <summary>
    /// Compares the given t against the stored intersection.
    /// Uses local-space epsilon tolerance and provided direction vector.
    /// </summary>
    /// <returns>True if this intersection is far than t.</returns>
    public bool Far (double t, ref Vector3d p1)
    {
      if (T <= t)
        return false;

      double p1Squared = p1.X * p1.X + p1.Y * p1.Y + p1.Z * p1.Z;
      t = T - t;
      return (t * t * p1Squared > RAY_EPSILON2);
    }

    /// <summary>
    /// Canonic sort order: by a T value.
    /// </summary>
    /// <param name="obj">Instance to be compared to..</param>
    /// <returns>-1, 0 or 1.</returns>
    public int CompareTo (object obj)
    {
      Intersection i = (Intersection) obj;
      return T.CompareTo(i.T);
    }
  }

  #endregion

  #region multi-threading

  /// <summary>
  /// Multi-threading support class.
  /// Contains ThreadStatic data.
  /// </summary>
  public class MT
  {
    // Shared random generator (within a thred).
    [ThreadStatic] public static RandomJames rnd;

    // Current pixel cooredinates.
    [ThreadStatic] public static int x;
    [ThreadStatic] public static int y;

    [ThreadStatic] public static double doubleX;
    [ThreadStatic] public static double doubleY;

    // Internal sampling parameters.
    [ThreadStatic] public static int rank;
    [ThreadStatic] public static int total;

    // Current threda's id.
    [ThreadStatic] public static int threadID;

    // Uplinks to roof objects for ray-based rendering.
    [ThreadStatic] public static IImageFunction imageFunction;
    [ThreadStatic] public static IRenderer renderer;
    [ThreadStatic] public static IRayScene scene;

    public static bool singleRayTracing    = false;
    public static bool sceneRendered       = false;
    public static bool renderingInProgress = false;
    public static bool pointCloudSavingInProgress = false;
    public static bool pointCloudCheckBox;

    // Put more TLS data here..

    /// <summary>
    /// Cold init of the thread data.
    /// </summary>
    public static void InitThreadData ()
    {
      if (rnd == null)
        rnd = new RandomJames(System.Threading.Thread.CurrentThread.GetHashCode() ^ DateTime.Now.Ticks);

      // Put TLS data init here..
    }

    /// <summary>
    /// Start rendering in the current thread ... set rendering globals.
    /// </summary>
    public static void SetRendering (
      in IRayScene sc,
      in IImageFunction imf,
      in IRenderer rend)
    {
      scene         = sc;
      imageFunction = imf;
      renderer      = rend;
    }

    /// <summary>
    /// Finished rendering ... unset rendering globals.
    /// </summary>
    public static void ResetRendering ()
    {
      scene         = null;
      imageFunction = null;
      renderer      = null;
    }

    /// <summary>
    /// Pixel rendering start.
    /// </summary>
    /// <param name="_x">Horizontal pixel coordinate.</param>
    /// <param name="_y">Vertical pixel coordnate.</param>
    /// <param name="tot">Designed supersampling factor.</param>
    public static void StartPixel (
      int _x,
      int _y,
      int tot)
    {
      x = _x;
      y = _y;
      rank = 0;
      total = tot;
    }

    /// <summary>
    /// Start the next sample of the current pixel.
    /// </summary>
    public static void NextSample ()
    {
      rank++;
    }
  }
  #endregion

  public static class Statistics
  {
    public static int primaryRaysCount;

    public static int allRaysCount;

    public static void IncrementRaysCounters (int amount, bool primary)
    {
      allRaysCount += amount;

      if (primary)
      {
        primaryRaysCount += amount;
      }
    }

    public static void Reset ()
    {
      primaryRaysCount = 0;
      allRaysCount = 0;
    }
  }
}
