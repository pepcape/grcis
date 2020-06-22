using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using MathSupport;
using OpenTK;
using Utilities;

namespace Rendering
{
  /// <summary>
  /// Image synthesizer w/o antialiasing or any other fancy stuff.
  /// Uses just one sample per pixel.
  /// </summary>
  [Serializable]
  public class SimpleImageSynthesizer : IRenderer
  {
    /// <summary>
    /// Image width in pixels.
    /// </summary>
    public int Width { get; set; }

    /// <summary>
    /// Image height in pixels.
    /// </summary>
    public int Height { get; set; }

    /// <summary>
    /// Gamma pre-compensation (gamma encoding, compression). Values 0.0 or 1.0 mean "no compensation".
    /// </summary>
    public double Gamma { get; set; }

    /// <summary>
    /// Cell-size for adaptive rendering, 0 or 1 to turn off adaptivitiy.
    /// </summary>
    public int Adaptive { get; set; }

    /// <summary>
    /// Current progress object (can be null).
    /// </summary>
    public Progress ProgressData { get; set; }

    public SimpleImageSynthesizer ()
      : this(1)
    {}

    public SimpleImageSynthesizer (int adaptive)
    {
      Adaptive     = adaptive;
      Gamma        = 2.2; // the right value "by the book"
      ProgressData = null;
    }

    /// <summary>
    /// Renders the single pixel of an image.
    /// </summary>
    /// <param name="x">Horizontal coordinate.</param>
    /// <param name="y">Vertical coordinate.</param>
    /// <param name="color">Computed pixel color.</param>
    public virtual void RenderPixel (int x, int y, double[] color)
    {
      MT.StartPixel(x, y, 1);
      MT.imageFunction.GetSample(x + 0.5, y + 0.5, color);

      // Gamma-encoding?
      if (Gamma > 0.001)
      {
        // Gamma-encoding and clamping.
        double g = 1.0 / Gamma;
        for (int b = 0; b < color.Length; b++)
          color[b] = Arith.Clamp(Math.Pow(color[b], g), 0.0, 1.0);
      }

      // Else: no gamma, no clamping (for HDRI).
    }

    /// <summary>
    /// Renders the given rectangle into the given raster image.
    /// Has to be re-entrant since this code is usually started in multiple parallel threads.
    /// </summary>
    /// <param name="image">Pre-initialized raster image.</param>
    public virtual void RenderRectangle (Bitmap image, int x1, int y1, int x2, int y2)
    {
      bool lead = MT.threadID == 0;
      if (lead &&
          ProgressData != null)
        lock (ProgressData)
        {
          ProgressData.Finished = 0.0f;
          ProgressData.Message = "";
        }

      double[] color = new double[3]; // pixel color

      // Run several phases of image rendering.
      int cell = 32; // cell size
      while (cell > 1 && cell > Adaptive)
        cell >>= 1;
      int initCell = cell;

      int   x,       y;
      bool  xParity, yParity;
      float total   = (x2 - x1) * (y2 - y1);
      long  counter = 0L;
      long  units   = 0;

      do // do one phase
      {
        for (y = y1, yParity = false;
             y < y2; // one image row
             y += cell, yParity = !yParity)

          for (x = x1, xParity = false;
               x < x2; // one image cell
               x += cell, xParity = !xParity)

            if (cell == initCell ||
                xParity || yParity) // process the cell
            {
              counter++;

              // Determine sample color...
              RenderPixel(x, y, color);

              if (Gamma <= 0.001)
                for (int b = 0; b < color.Length; b++)
                  color[b] = Arith.Clamp(color[b], 0.0, 1.0);

              // .. and render it.
              Color c = Color.FromArgb((int)(color[0] * 255.0),
                                       (int)(color[1] * 255.0),
                                       (int)(color[2] * 255.0));
              lock (image)
              {
                if (cell == 1)
                  image.SetPixel(x, y, c);
                else
                {
                  int xMax = x + cell;
                  if (xMax > x2)
                    xMax = x2;
                  int yMax = y + cell;
                  if (yMax > y2)
                    yMax = y2;
                  for (int iy = y; iy < yMax; iy++)
                    for (int ix = x; ix < xMax; ix++)
                      image.SetPixel(ix, iy, c);
                }
              }

              if ((++units & 63L) == 0L &&
                   ProgressData != null)
                lock (ProgressData)
                {
                  if (!ProgressData.Continue)
                    return;
                  if (lead)
                  {
                    ProgressData.Finished = counter / total;
                    ProgressData.Sync(image);
                  }
                }
            }
      } while ((cell >>= 1) > 0); // do one phase
    }
  }

  /// <summary>
  /// Supersampling image synthesizer (antialiasing by jittering).
  /// </summary>
  [Serializable]
  public class SupersamplingImageSynthesizer : SimpleImageSynthesizer
  {
    /// <summary>
    /// 1D super-sampling factor.
    /// </summary>
    protected int superXY = 1;

    /// <summary>
    /// 2D supersampling (number of samples per pixel). Rounded up to the next square.
    /// </summary>
    public int Supersampling
    {
      get => superXY * superXY;
      set
      {
        superXY = 1;
        while (superXY * superXY < value)
          superXY++;
      }
    }

    /// <summary>
    /// Supersampling method: 0.0 for regular sampling, 1.0 for full jittering.
    /// </summary>
    public double Jittering { get; set; }

    public SupersamplingImageSynthesizer ()
      : this(1)
    {}

    public SupersamplingImageSynthesizer (int adaptive)
      : base(adaptive)
    {
      Jittering = 1.0;
    }

    /// <summary>
    /// Renders the single pixel of an image.
    /// </summary>
    /// <param name="x">Horizontal coordinate.</param>
    /// <param name="y">Vertical coordinate.</param>
    /// <param name="color">Computed pixel color.</param>
    public override void RenderPixel (int x, int y, double[] color)
    {
      Debug.Assert(color != null);

      int bands = color.Length;
      int b;
      Array.Clear(color, 0, bands);
      double[] tmp = new double[bands];

      int    i, j;
      double step      = 1.0 / superXY;
      double amplitude = Jittering * step;
      double origin    = 0.5 * (step - amplitude);
      double x0, y0;
      MT.StartPixel(x, y, Supersampling);

      for (j = 0, y0 = y + origin; j++ < superXY; y0 += step)
        for (i = 0, x0 = x + origin; i++ < superXY; x0 += step)
        {
          MT.imageFunction.GetSample(x0 + amplitude * MT.rnd.UniformNumber(),
                                     y0 + amplitude * MT.rnd.UniformNumber(),
                                     tmp);
          MT.NextSample();
          Util.ColorAdd(tmp, color);
        }

      double mul = step / superXY;

      // Gamma-encoding?
      if (Gamma > 0.001)
      {
        // Gamma-encoding and clamping.
        double g = 1.0 / Gamma;
        for (b = 0; b < bands; b++)
          color[b] = Arith.Clamp(Math.Pow(color[b] * mul, g), 0.0, 1.0);
      }
      else
        // No gamma, no clamping (for HDRI).
        Util.ColorMul(mul, color);
    }
  }

  /// <summary>
  /// Adaptive supersampling inspired by a quad-tree.
  /// Original author: Jan Roztocil, 2012.
  /// Update: Josef Pelikan, 2018.
  /// </summary>
  [Serializable]
  public class AdaptiveSupersamplingJR : SupersamplingImageSynthesizer
  {
    public AdaptiveSupersamplingJR (double colThreshold = 0.004)
      : base(16)
    {
      bands = 0;
      colorThreshold = colThreshold;
    }

    protected int bands;

    protected double colorThreshold;

    /// <summary>
    /// Ternary tree to sture sampling results.
    /// </summary>
    class Node
    {
      public double x;
      public double y;
      public double step;

      public Node[] children;
      public Result result;

      public Node (double x, double y, double step)
      {
        this.x = x;
        this.y = y;
        this.step = step;
      }

      public void sample (int bands)
      {
        result = new Result(bands,
                            x + step * MT.rnd.UniformNumber(),
                            y + step * MT.rnd.UniformNumber());
        result.hash = MT.imageFunction.GetSample(result.x, result.y, result.color);
        MT.NextSample();
      }
    }

    /// <summary>
    /// Intersection result stored together with color & hash.
    /// </summary>
    class Result
    {
      public Result (int bands, double x, double y)
      {
        color  = new double[bands];
        this.x = x;
        this.y = y;
        hash   = 0L;
      }

      public double   x, y;
      public double[] color;
      public long     hash;
    }

    /// <summary>
    /// Returns true if the two colors are similar
    /// </summary>
    private bool similarColor (double[] col1, double[] col2)
    {
      double r_diff = col1[0] - col2[0];
      double g_diff = col1[1] - col2[1];
      double b_diff = col1[2] - col2[2];
      return Math.Sqrt(r_diff * r_diff + g_diff * g_diff + b_diff * b_diff) < colorThreshold;
    }

    private bool subdivisionNeeded (Result res1, Result res2)
    {
      if (res1.hash != res2.hash)
        return true; // two different objects were hit

      return !similarColor(res1.color, res2.color);
    }

    /// <summary>
    /// Subdivides the node into quadrants.
    /// </summary>
    private void subdivide (Node root, int division, int maxDivision)
    {
      double step   = root.step * 0.5;
      Node[] ch     = new Node[4];
      root.children = ch;

      // child[0] .. root.x, root.y
      ch[0] = new Node(root.x, root.y, step);
      if (root.result.x < root.x + step &&
          root.result.y < root.y + step)
      {
        // Use the old sample.
        ch[0].result = root.result;
        root.result  = null;
      }
      else
        ch[0].sample(bands);

      // child[1] .. root.x, root.y + step
      ch[1] = new Node(root.x, root.y + step, step);
      if (root.result != null &&
          root.result.x < root.x + step)
      {
        // Use the old sample.
        ch[1].result = root.result;
        root.result  = null;
      }
      else
        ch[1].sample(bands);

      // child[2] .. root.x + step, root.y
      ch[2] = new Node(root.x + step, root.y, step);
      if (root.result != null &&
          root.result.y < root.y + step)
      {
        // Use the old sample.
        ch[2].result = root.result;
        root.result  = null;
      }
      else
        ch[2].sample(bands);

      // child[3] .. root.x + step, root.y + step
      ch[3] = new Node(root.x + step, root.y + step, step);
      if (root.result != null)
      {
        // Use the old sample.
        ch[3].result = root.result;
        root.result  = null;
      }
      else
        ch[3].sample(bands);

      // Tree-depth check.
      if ((division += division) >= maxDivision)
        return;

      bool toSubdivide0 = false;
      bool toSubdivide1 = false;
      bool toSubdivide2 = false;
      bool toSubdivide3 = false;

      // Neighbour checks.
      if (subdivisionNeeded(ch[0].result, ch[1].result))
        toSubdivide0 = toSubdivide1 = true;

      if (subdivisionNeeded(ch[1].result, ch[3].result))
        toSubdivide1 = toSubdivide3 = true;

      if (subdivisionNeeded(ch[0].result, ch[2].result))
        toSubdivide0 = toSubdivide2 = true;

      if (subdivisionNeeded(ch[2].result, ch[3].result))
        toSubdivide2 = toSubdivide3 = true;

      // Divide and conquer.
      if (toSubdivide0)
        subdivide(ch[0], division, maxDivision);
      if (toSubdivide1)
        subdivide(ch[1], division, maxDivision);
      if (toSubdivide2)
        subdivide(ch[2], division, maxDivision);
      if (toSubdivide3)
        subdivide(ch[3], division, maxDivision);
    }

    /// <summary>
    /// Final colour gathering.
    /// </summary>
    private void gatherColors (Node node, double[] color)
    {
      if (node.children != null)
        // Inner node.
        foreach (Node child in node.children)
          gatherColors(child, color);
      else
      {
        // Leaf node.
        double mult = node.step * node.step;
        Util.ColorAdd(node.result.color, mult, color);
      }
    }

    /// <summary>
    /// Renders the single pixel of an image (using required super-sampling).
    /// </summary>
    /// <param name="x">Horizontal coordinate.</param>
    /// <param name="y">Vertical coordinate.</param>
    /// <param name="color">Computed pixel color.</param>
    public override void RenderPixel (int x, int y, double[] color)
    {
      Debug.Assert(color != null);
      Debug.Assert(MT.rnd != null);

      MT.StartPixel(x, y, Supersampling);

      bands = color.Length;
      Array.Clear(color, 0, bands);

      // We are starting from the whole pixel area = unit square.
      Node root = new Node(x, y, 1.0);
      root.sample(bands);
      if (superXY > 1)
        subdivide(root, 1, superXY);

      // Gather result color.
      gatherColors(root, color);

      // Gamma-encoding?
      if (Gamma > 0.001)
      {
        // Gamma-encoding and clamping.
        double g = 1.0 / Gamma;
        for (int b = 0; b < bands; b++)
          color[b] = Arith.Clamp(Math.Pow(color[b], g), 0.0, 1.0);
      }
    }
  }

  /// <summary>
  /// Simple camera with center of projection and planar projection surface.
  /// </summary>
  [Serializable]
  public class StaticCamera : ICamera
  {
    /// <summary>
    /// Width / Height of the viewing area (viewport, frustum).
    /// </summary>
    public double AspectRatio
    {
      get => width / height;
      set
      {
        height = width / value;
        prepare();
      }
    }

    protected double width = 640.0;

    /// <summary>
    /// Viewport width.
    /// </summary>
    public double Width
    {
      get => width;
      set
      {
        width = value;
        prepare();
      }
    }

    protected double height = 480.0;

    /// <summary>
    /// Viewport height.
    /// </summary>
    public double Height
    {
      get => height;
      set
      {
        height = value;
        prepare();
      }
    }

    protected Vector3d center;

    protected Vector3d direction;

    protected Vector3d up = new Vector3d(0.0, 1.0, 0.0);

    /// <summary>
    /// Horizontal viewing angle in radians.
    /// </summary>
    protected double hAngle = 1.0;

    protected Vector3d origin;

    protected Vector3d dx, dy;

    /// <summary>
    /// Should be called after every parameter change..
    /// </summary>
    protected void prepare ()
    {
      Vector3d left = Vector3d.Cross(direction, up).Normalized ();
      Vector3d top  = Vector3d.Cross(left, direction).Normalized ();

      origin = center + direction;
      double halfWidth  = Math.Tan ( 0.5 * hAngle );
      double halfHeight = halfWidth / AspectRatio;
      origin += top * halfHeight + left * halfWidth;
      dx = left * (-2.0 * halfWidth / width);
      dy = -top * (2.0 * halfHeight / height);
    }

    public StaticCamera ()
    {}

    /// <summary>
    /// Initializing constructor, able to set all camera parameters.
    /// </summary>
    /// <param name="cen">Center of the projection.</param>
    /// <param name="dir">View direction (must not be zero).</param>
    /// <param name="ang">Horizontal viewing angle in degrees.</param>
    public StaticCamera (Vector3d cen, Vector3d dir, double ang)
    {
      center = cen;
      direction = dir;
      direction.Normalize();
      hAngle = MathHelper.DegreesToRadians((float)ang);
      Width = 300;
      Height = 400;
    }

    /// <summary>
    /// Initializing constructor, able to set all camera parameters.
    /// </summary>
    /// <param name="cen">Center of the projection.</param>
    /// <param name="dir">View direction (must not be zero).</param>
    /// <param name="u">Up vector.</param>
    /// <param name="ang">Horizontal viewing angle in degrees.</param>
    public StaticCamera (Vector3d cen, Vector3d dir, Vector3d u, double ang)
    {
      center = cen;
      direction = dir;
      direction.Normalize();
      up = u;
      hAngle = MathHelper.DegreesToRadians((float)ang);
      Width = 300;
      Height = 400;
    }

    /// <summary>
    /// Ray-generator. Simple variant, w/o an integration support.
    /// </summary>
    /// <param name="x">Origin position within a viewport (horizontal coordinate).</param>
    /// <param name="y">Origin position within a viewport (vertical coordinate).</param>
    /// <param name="p0">Ray origin.</param>
    /// <param name="p1">Ray direction vector.</param>
    /// <returns>True if the ray (viewport position) is valid.</returns>
    public bool GetRay (double x, double y, out Vector3d p0, out Vector3d p1)
    {
      p0 = center;
      p1 = origin + x * dx + y * dy;
      p1 = p1 - center;
      p1.Normalize();
      return true;
    }
  }

  /// <summary>
  /// Point light source w/o intensity attenuation.
  /// </summary>
  [Serializable]
  public class PointLightSource : ILightSource
  {
    /// <summary>
    /// 3D coordinate of the source.
    /// </summary>
    public Vector3d? position { get; set; }

    /// <summary>
    /// Intensity of the source expressed as color tuple.
    /// </summary>
    protected double[] intensity;

    /// <summary>
    /// Monochromatic light source.
    /// </summary>
    public PointLightSource (Vector3d pos, double intens)
    {
      position  = pos;
      intensity = new double[] {intens, intens, intens};
    }

    /// <summary>
    /// Color light source.
    /// </summary>
    public PointLightSource (Vector3d pos, double[] intens)
    {
      position  = pos;
      intensity = intens;
    }

    /// <summary>
    /// Returns intensity (incl. color) of the source contribution to the given scene point.
    /// Simple variant, w/o an integration support.
    /// </summary>
    /// <param name="intersection">Scene point (only world coordinates and normal vector are needed).</param>
    /// <param name="dir">Direction to the source is set here (zero vector for omnidirectional source). Not normalized!</param>
    /// <returns>Intensity vector in current color space or null if the point is not lit.</returns>
    public virtual double[] GetIntensity (Intersection intersection, out Vector3d dir)
    {
      dir = (Vector3d)position - intersection.CoordWorld;
      if (Vector3d.Dot(dir, intersection.Normal) <= 0.0)
        return null;

      return intensity;
    }
  }

  /// <summary>
  /// Ambient white light source.
  /// </summary>
  [Serializable]
  public class AmbientLightSource : ILightSource
  {
    protected double[] intensity;

    public Vector3d? position { get; set; }

    public AmbientLightSource ()
    {}

    public AmbientLightSource (double intens)
    {
      intensity = new double[] {intens, intens, intens};
    }

    /// <summary>
    /// Returns intensity (incl. color) of the source contribution to the given scene point.
    /// Simple variant, w/o an integration support.
    /// </summary>
    /// <param name="intersection">Scene point (only world coordinates and normal vector are needed).</param>
    /// <param name="dir">Direction to the source is set here (zero vector for omnidirectional source).</param>
    /// <returns>Intensity vector in current color space or null if the point is not lit.</returns>
    public double[] GetIntensity (Intersection intersection, out Vector3d dir)
    {
      dir = Vector3d.Zero;
      return intensity;
    }
  }

  /// <summary>
  /// Rectangle light source with intensity attenuation.
  /// </summary>
  [Serializable]
  public class RectangleLightSource : PointLightSource
  {
    /// <summary>
    ///  "Horizontal" edge of the rectangle.
    /// </summary>
    protected Vector3d width = new Vector3d ( 1.0, 0.0, 0.0 );

    /// <summary>
    /// "Vertical" edge of the rectangle.
    /// </summary>
    protected Vector3d height = new Vector3d ( 0.0, 1.0, 0.0 );

    /// <summary>
    /// Dimming polynom coefficients: light is dimmed by the factor of
    /// Dim[0] + Dim[1] * D + Dim[2] * D^2.
    /// Can be "null" for no dimming..
    /// </summary>
    public double[] Dim { get; set; }

    /// <summary>
    /// Support data container for sampling states.
    /// </summary>
    public class SamplingState
    {
      protected RectangleLightSource source;

      protected int cachedRank;
      protected int cachedTotal;

      protected RandomJames.Permutation permU;
      protected RandomJames.Permutation permV;

      protected double   u, v;
      public    Vector3d sample;

      public SamplingState (RectangleLightSource src)
      {
        source = src;
        permU = new RandomJames.Permutation();
        permV = new RandomJames.Permutation();
        cachedRank = cachedTotal = 0;
      }

      public void generateSample ()
      {
        if (MT.total > 1)
        {
          if (MT.rank == cachedRank &&
               MT.total == cachedTotal)
            return;

          int uCell, vCell;
          if (MT.total != cachedTotal || MT.rank < cachedRank) // [re-]initialization
          {
            cachedTotal = MT.total;
            uCell = MT.rnd.PermutationFirst(cachedTotal, ref permU);
            vCell = MT.rnd.PermutationFirst(cachedTotal, ref permV);
          }
          else
          {
            uCell = Math.Max(MT.rnd.PermutationNext(ref permU), 0);
            vCell = Math.Max(MT.rnd.PermutationNext(ref permV), 0);
          }

          cachedRank = MT.rank;

          // point sample will be placed into [ uCell, vCell ] cell:
          u = (uCell + MT.rnd.UniformNumber()) / cachedTotal;
          v = (vCell + MT.rnd.UniformNumber()) / cachedTotal;
        }
        else
        {
          u = MT.rnd.UniformNumber();
          v = MT.rnd.UniformNumber();
        }

        // TODO: do something like:
        sample = (Vector3d)source.position + u * source.width + v * source.height;
      }
    }

    /// <summary>
    /// Set of sampling states (one per random-generator instance / thread).
    /// </summary>
    protected Dictionary<int, SamplingState> states = new Dictionary<int, SamplingState>();

    /// <summary>
    /// Monochromatic light source.
    /// </summary>
    public RectangleLightSource (Vector3d pos, Vector3d wid, Vector3d hei, double intens)
      : base(pos, intens)
    {
      width  = wid;
      height = hei;
      Dim    = new double[] { 0.5, 1.0, 0.1 };
    }

    /// <summary>
    /// Color light source.
    /// </summary>
    public RectangleLightSource (Vector3d pos, Vector3d wid, Vector3d hei, double[] intens)
      : base(pos, intens)
    {
      width  = wid;
      height = hei;
      Dim    = new double[] { 0.5, 1.0, 0.1 };
    }

    /// <summary>
    /// Returns intensity (incl. color) of the source contribution to the given scene point.
    /// Internal integration support.
    /// </summary>
    /// <param name="intersection">Scene point (only world coordinates and normal vector are needed).</param>
    /// <param name="dir">Direction to the source is set here (zero vector for omnidirectional source). Not normalized!</param>
    /// <returns>Intensity vector in current color space or null if the point is not lit.</returns>
    public override double[] GetIntensity (Intersection intersection, out Vector3d dir)
    {
      if (MT.rnd == null)
        return GetIntensity(intersection, out dir);

      SamplingState ss;
      lock (states)
      {
        if (!states.TryGetValue(MT.rnd.GetHashCode(), out ss))
        {
          ss = new SamplingState(this);
          states.Add(MT.rnd.GetHashCode(), ss);
          // !!! TODO: check the states ???
        }
      }

      // generate a [new] sample:
      ss.generateSample();
      dir = ss.sample - intersection.CoordWorld;
      if (Vector3d.Dot(dir, intersection.Normal) <= 0.0)
        return null;

      if (Dim == null || Dim.Length < 3)
        return intensity;

      double   dist    = dir.Length;
      double   dimCoef = 1.0 / (Dim[0] + dist * (Dim[1] + dist * Dim[2]));
      int      bands   = intensity.Length;
      double[] result  = new double[bands];
      Util.ColorCopy(intensity, dimCoef, result);

      return result;
    }
  }

  /// <summary>
  /// Default Background implementation - constant color defined
  /// by 'IRayScene.BackgroundColor'
  /// </summary>
  [Serializable]
  public class DefaultBackground : IBackground
  {
    protected static readonly double[] DEFAULT = {0.0, 0.02, 0.03};

    /// <summary>
    /// Reference to the scene object ()
    /// </summary>
    protected IRayScene scene;

    public IRayScene Scene
    {
      get => scene;
      set => scene = value;
    }

    public double[] Color;

    public DefaultBackground (in double[] color)
    {
      scene = null;
      Color = null;
      if (color == null ||
          color.Length < 1)
        return;

      Color = (double[])color.Clone();
    }

    public DefaultBackground (in IRayScene rtsc = null)
    {
      scene = rtsc;
      Color = null;
    }

    /// <summary>
    /// Returns background color = function of direction vector.
    /// </summary>
    /// <param name="p1">Direction vector</param>
    /// <param name="color">Output color</param>
    public virtual long GetColor (Vector3d p1, double[] color)
    {
      if (scene != null &&
          scene.BackgroundColor != null &&
          scene.BackgroundColor.Length > 0)
        Util.ColorCopy(scene.BackgroundColor, color);
      else
      if (Color != null &&
          Color.Length > 0)
        Util.ColorCopy(Color, color);
      else
        Util.ColorCopy(DEFAULT, color);

      return 1L;
    }
  }

  /// <summary>
  /// Simple Phong-like reflectance model: material description.
  /// </summary>
  [Serializable]
  public class PhongMaterial : IMaterial
  {
    /// <summary>
    /// Base surface color.
    /// </summary>
    public double[] Color { get; set; }

    /// <summary>
    /// Coefficient of diffuse reflection.
    /// </summary>
    public double Kd { get; set; }

    /// <summary>
    /// Coefficient of specular reflection.
    /// </summary>
    public double Ks { get; set; }

    /// <summary>
    /// Specular exponent.
    /// </summary>
    public int H { get; set; }

    /// <summary>
    /// Ambient term (for ambient light sources only).
    /// </summary>
    public double Ka { get; set; }

    /// <summary>
    /// Coefficient of transparency.
    /// </summary>
    public double Kt { get; set; }

    /// <summary>
    /// Schlick adjustment (0.0 for pure Phong, 1.0 for Schlick).
    /// </summary>
    public double Sch { get; set; }

    protected double _n;

    /// <summary>
    /// Absolute index of refraction.
    /// </summary>
    public double n
    {
      get => _n;
      set
      {
        _n = value;
        cosTotal = _n > 1.0 ? (Math.Sqrt(_n * _n - 1.0) / _n) : 0.0;
      }
    }

    /// <summary>
    /// Cosine of total reflection angle.
    /// </summary>
    public double cosTotal = 0.0;

    public PhongMaterial (PhongMaterial m)
    {
      Color = (double[])m.Color.Clone();
      Ka    = m.Ka;
      Kd    = m.Kd;
      Ks    = m.Ks;
      H     = m.H;
      Kt    = m.Kt;
      Sch   = m.Sch;
      n     = m.n;
    }

    public PhongMaterial (double[] color, double ka, double kd, double ks, int h, double sch = 1.0)
    {
      Color = color;
      Ka    = ka;
      Kd    = kd;
      Ks    = ks;
      H     = h;
      Kt    = 0.0;
      Sch   = sch;
      n     = 1.5;
    }

    public PhongMaterial ()
      : this(new double[] {1.0, 0.9, 0.4}, 0.2, 0.7, 0.2, 16)
    {}

    public object Clone ()
    {
      return new PhongMaterial(this);
    }
  }

  /// <summary>
  /// Simple Phong-like reflectance model: the model itself.
  /// </summary>
  [Serializable]
  public class PhongModel : IReflectanceModel
  {
    public IMaterial DefaultMaterial ()
    {
      return new PhongMaterial();
    }

    public double[] ColorReflection (
      Intersection intersection,
      Vector3d input,
      Vector3d output,
      ReflectionComponent comp)
    {
      return ColorReflection(intersection.Material, intersection.Normal, input, output, comp);
    }

    public double[] ColorReflection (
      IMaterial material,
      Vector3d normal,
      Vector3d input,
      Vector3d output,
      ReflectionComponent comp)
    {
      if (!(material is PhongMaterial))
        return null;

      PhongMaterial mat     = (PhongMaterial)material;
      int           bands   = mat.Color.Length;
      double[]      result  = new double[bands];
      bool          viewOut = Vector3d.Dot(output, normal) > 0.0;
      double        coeff;

      if (input == Vector3d.Zero) // ambient term only..
      {
        // Dim ambient light if viewer is inside.
        coeff = viewOut ? mat.Ka : (mat.Ka * mat.Kt);
        Util.ColorCopy(mat.Color, coeff, result);
        return result;
      }

      // Directional light source.
      input.Normalize();
      double cosAlpha = Vector3d.Dot(input, normal);
      bool   lightOut = cosAlpha > 0.0;
      double ks       = mat.Ks;
      double kd       = mat.Kd;
      double kt       = mat.Kt;

      // Schlick's adjustment.
      if (mat.Sch > 0.0)
      {
        double oneU        = 1.0 - Math.Abs(cosAlpha);         //  1-u
        double oneUsq      = oneU * oneU;                      // (1-u)^2
        double schlick     = mat.Sch * oneUsq * oneUsq * oneU; // (1-u)^5 * mat.Sch
        double schlickComp = 1.0 - schlick;
        ks += schlick * (kd + kt);
        kd *= schlickComp;
        kt *= schlickComp;
      }

      Vector3d r = Vector3d.Zero;
      coeff = 1.0;
      if (viewOut == lightOut)
      {
        // Viewer and source are on the same side..
        if ((comp & ReflectionComponent.SPECULAR_REFLECTION) != 0)
        {
          double cos2 = cosAlpha + cosAlpha;
          r = normal * cos2 - input;

          if (!lightOut && // total reflection check
              -cosAlpha <= mat.cosTotal)
            if ((ks += kt) + kd > 1.0)
              ks = 1.0 - kd;
        }
      }
      else
      {
        // Opposite sides => use specular refraction.
        if ((comp & ReflectionComponent.SPECULAR_REFRACTION) != 0)
          r = Geometry.SpecularRefraction(normal, mat.n, input);
        coeff = kt;
      }

      double diffuse  = (comp & ReflectionComponent.DIFFUSE) == 0 ? 0.0 : coeff * kd * Math.Abs(cosAlpha);
      double specular = 0.0;

      if (r != Vector3d.Zero)
      {
        double cosBeta = Vector3d.Dot(r, output);
        if (cosBeta > 0.0)
          specular = coeff * ks * Arith.Pow(cosBeta, mat.H);
      }

      for (int i = 0; i < bands; i++)
        result[i] = diffuse * mat.Color[i] + specular;

      return result;
    }
  }

  /// <summary>
  /// Simple texture able to modulate surface color.
  /// </summary>
  [Serializable]
  public class CheckerTexture : ITexture
  {
    /// <summary>
    /// Alternative color.
    /// </summary>
    public double[] Color2;

    /// <summary>
    /// Frequency in the u-direction.
    /// </summary>
    public double Fu;

    /// <summary>
    /// Frequency in the v-direction.
    /// </summary>
    public double Fv;

    public CheckerTexture (double fu, double fv, double[] color)
    {
      Fu     = fu;
      Fv     = fv;
      Color2 = (double[])color.Clone();
    }

    /// <summary>
    /// Apply the relevant value-modulation in the given Intersection instance.
    /// Simple variant, w/o an integration support.
    /// </summary>
    /// <param name="inter">Data object to modify.</param>
    /// <returns>Hash value (texture signature) for adaptive subsampling.</returns>
    public virtual long Apply (Intersection inter)
    {
      double u = inter.TextureCoord.X * Fu;
      double v = inter.TextureCoord.Y * Fv;

      long ui = (long)Math.Floor(u);
      long vi = (long)Math.Floor(v);

      if (((ui + vi) & 1) != 0)
        Util.ColorCopy(Color2, inter.SurfaceColor);

      inter.textureApplied = true; // warning - this changes textureApplied bool even when only one texture was applied - not all of them

      return ui + (long)RandomStatic.numericRecipes((ulong)vi);
    }
  }
}
