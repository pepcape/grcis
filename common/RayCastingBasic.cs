using System;
using System.Diagnostics;
using System.Drawing;
using MathSupport;
using OpenTK;
using System.Collections.Generic;

namespace Rendering
{
  /// <summary>
  /// Image synthesizer w/o antialiasing or any other fancy stuff.
  /// Uses just one sample per pixel.
  /// </summary>
  public class SimpleImageSynthesizer : IRenderer
  {
    /// <summary>
    /// Image width in pixels.
    /// </summary>
    public int Width
    {
      get;
      set;
    }

    /// <summary>
    /// Image height in pixels.
    /// </summary>
    public int Height
    {
      get;
      set;
    }

    /// <summary>
    /// Gamma pre-compensation (gamma encoding, compression). Values 0.0 or 1.0 mean "no compensation".
    /// </summary>
    public double Gamma
    {
      get;
      set;
    }

    /// <summary>
    /// Sample-based rendering specifics: rendered image is defined by
    /// a continuous-argument image function.
    /// </summary>
    public IImageFunction ImageFunction
    {
      get;
      set;
    }

    /// <summary>
    /// Cell-size for adaptive rendering, 0 or 1 to turn off adaptivitiy.
    /// </summary>
    public int Adaptive
    {
      get;
      set;
    }

    /// <summary>
    /// Current progress object (can be null).
    /// </summary>
    public Progress ProgressData
    {
      get;
      set;
    }

    public SimpleImageSynthesizer ()
      : this( 1 )
    {
    }

    public SimpleImageSynthesizer ( int adaptive )
    {
      Adaptive = adaptive;
      Gamma = 2.2;              // the right value "by the book"
      ProgressData = null;
    }

    /// <summary>
    /// Renders the single pixel of an image.
    /// </summary>
    /// <param name="x">Horizontal coordinate.</param>
    /// <param name="y">Vertical coordinate.</param>
    /// <param name="color">Computed pixel color.</param>
    /// <param name="rnd">Shared random generator.</param>
    public virtual void RenderPixel ( int x, int y, double[] color, RandomJames rnd )
    {
      ImageFunction.GetSample( x + 0.5, y + 0.5, color );

      // gamma-encoding:
      if ( Gamma > 0.001 )
      {                                     // gamma-encoding and clamping
        double g = 1.0 / Gamma;
        for ( int b = 0; b < color.Length; b++ )
          color[ b ] = Arith.Clamp( Math.Pow( color[ b ], g ), 0.0, 1.0 );
      }
                                           // else: no gamma, no clamping (for HDRI)
    }

    /// <summary>
    /// Renders the given rectangle into the given raster image.
    /// </summary>
    /// <param name="image">Pre-initialized raster image.</param>
    /// <param name="rnd">Shared random generator.</param>
    public virtual void RenderRectangle ( Bitmap image, int x1, int y1, int x2, int y2, RandomJames rnd )
    {
      RenderRectangle( image, x1, y1, x2, y2, ( n ) => true, rnd );
    }

    /// <summary>
    /// Renders the given rectangle into the given raster image.
    /// Has to be re-entrant since this code is started in multiple parallel threads.
    /// </summary>
    /// <param name="image">Pre-initialized raster image.</param>
    /// <param name="sel">Selector for this working thread.</param>
    /// <param name="rnd">Thread-specific random generator.</param>
    public virtual void RenderRectangle ( Bitmap image, int x1, int y1, int x2, int y2, ThreadSelector sel, RandomJames rnd )
    {
      bool lead = sel( 0L );
      if ( lead &&
           ProgressData != null )
        lock ( ProgressData )
        {
          ProgressData.Finished = 0.0f;
          ProgressData.Message = "";
        }

      double[] color = new double[ 3 ];     // pixel color

      // run several phases of image rendering:
      int cell = 32;                        // cell size
      while ( cell > 1 && cell > Adaptive )
        cell >>= 1;
      int initCell = cell;

      int x, y;
      bool xParity, yParity;
      float total = (x2 - x1) * (y2 - y1);
      long counter = 0L;
      long units = 0;

      do                                    // do one phase
      {
        for ( y = y1, yParity = false;
              y < y2;                       // one image row
              y += cell, yParity = !yParity )

          for ( x = x1, xParity = false;
                x < x2;                     // one image cell
                x += cell, xParity = !xParity )

            if ( cell == initCell ||
                 xParity || yParity )       // process the cell
            {
              if ( !sel( counter++ ) )
                continue;

              // determine sample color ..
              RenderPixel( x, y, color, rnd );

              if ( Gamma <= 0.001 )
                for ( int b = 0; b < color.Length; b++ )
                  color[ b ] = Arith.Clamp( color[ b ], 0.0, 1.0 );

              // .. and render it:
              Color c = Color.FromArgb( (int)(color[ 0 ] * 255.0),
                                        (int)(color[ 1 ] * 255.0),
                                        (int)(color[ 2 ] * 255.0) );
              lock ( image )
              {
                if ( cell == 1 )
                  image.SetPixel( x, y, c );
                else
                {
                  int xMax = x + cell;
                  if ( xMax > x2 )
                    xMax = x2;
                  int yMax = y + cell;
                  if ( yMax > y2 )
                    yMax = y2;
                  for ( int iy = y; iy < yMax; iy++ )
                    for ( int ix = x; ix < xMax; ix++ )
                      image.SetPixel( ix, iy, c );
                }
              }

              if ( (++units & 63L) == 0L &&
                   ProgressData != null )
                lock ( ProgressData )
                {
                  if ( !ProgressData.Continue )
                    return;
                  if ( lead )
                  {
                    ProgressData.Finished = counter / total;
                    ProgressData.Sync( image );
                  }
                }
            }
      }
      while ( (cell >>= 1) > 0 );         // do one phase
    }
  }

  /// <summary>
  /// Supersampling image synthesizer (antialiasing by jittering).
  /// </summary>
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
      get
      {
        return superXY * superXY;
      }
      set
      {
        superXY = 1;
        while ( superXY * superXY < value )
          superXY++;
      }
    }

    /// <summary>
    /// Supersampling method: 0.0 for regular sampling, 1.0 for full jittering.
    /// </summary>
    public double Jittering
    {
      get;
      set;
    }

    public SupersamplingImageSynthesizer ()
      : this( 1 )
    {
    }

    public SupersamplingImageSynthesizer ( int adaptive )
      : base( adaptive )
    {
      Jittering = 1.0;
    }

    /// <summary>
    /// Renders the single pixel of an image.
    /// </summary>
    /// <param name="x">Horizontal coordinate.</param>
    /// <param name="y">Vertical coordinate.</param>
    /// <param name="color">Computed pixel color.</param>
    /// <param name="rnd">Shared random generator.</param>
    public override void RenderPixel ( int x, int y, double[] color, RandomJames rnd )
    {
      Debug.Assert( color != null );
      Debug.Assert( rnd != null );

      int bands = color.Length;
      int b;
      Array.Clear( color, 0, bands );
      double[] tmp = new double[ bands ];

      int i, j, ord;
      double step = 1.0 / superXY;
      double amplitude = Jittering * step;
      double origin = 0.5 * (step - amplitude);
      double x0, y0;
      for ( j = ord = 0, y0 = y + origin; j++ < superXY; y0 += step )
        for ( i = 0, x0 = x + origin; i++ < superXY; x0 += step )
        {
          ImageFunction.GetSample( x0 + amplitude * rnd.UniformNumber(),
                                   y0 + amplitude * rnd.UniformNumber(),
                                   ord++, Supersampling, rnd, tmp );
          for ( b = 0; b < bands; b++ )
            color[ b ] += tmp[ b ];
        }

      double mul = step / superXY;
      if ( Gamma > 0.001 )
      {                                     // gamma-encoding and clamping
        double g = 1.0 / Gamma;
        for ( b = 0; b < bands; b++ )
          color[ b ] = Arith.Clamp( Math.Pow( color[ b ] * mul, g ), 0.0, 1.0 );
      }
      else                                  // no gamma, no clamping (for HDRI)
        for ( b = 0; b < bands; b++ )
          color[ b ] *= mul;
    }
  }

  /// <summary>
  /// Simple camera with center of projection and planar projection surface.
  /// </summary>
  public class StaticCamera : ICamera
  {
    /// <summary>
    /// Width / Height of the viewing area (viewport, frustum).
    /// </summary>
    public double AspectRatio
    {
      get { return width / height; }
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
      get { return width; }
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
      get { return height; }
      set
      {
        height = value;
        prepare();
      }
    }

    protected Vector3d center;

    protected Vector3d direction;

    protected Vector3d up = new Vector3d( 0.0, 1.0, 0.0 );

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
      Vector3d left = Vector3d.Cross( direction, up );
      origin = center + direction;
      double halfWidth  = Math.Tan( 0.5 * hAngle );
      double halfHeight = halfWidth / AspectRatio;
      origin += up * halfHeight + left * halfWidth;
      dx = left * (-2.0 * halfWidth / width);
      dy = -up * (2.0 * halfHeight / height);
    }

    /// <summary>
    /// Initializing constructor, able to set all camera parameters.
    /// </summary>
    /// <param name="cen">Center of the projection.</param>
    /// <param name="dir">View direction (must not be zero).</param>
    /// <param name="ang">Horizontal viewing angle in degrees.</param>
    public StaticCamera ( Vector3d cen, Vector3d dir, double ang )
    {
      center    = cen;
      direction = dir;
      direction.Normalize();
      hAngle = MathHelper.DegreesToRadians( (float)ang );
      Width  = 300;
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
    public bool GetRay ( double x, double y, out Vector3d p0, out Vector3d p1 )
    {
      p0 = center;
      p1 = origin + x * dx + y * dy;
      p1 = p1 - center;
      p1.Normalize();
      return true;
    }

    /// <summary>
    /// Ray-generator. Internal integration support.
    /// </summary>
    /// <param name="x">Origin position within a viewport (horizontal coordinate).</param>
    /// <param name="y">Origin position within a viewport (vertical coordinate).</param>
    /// <param name="rank">Rank of this ray, 0 <= rank < total (for integration).</param>
    /// <param name="total">Total number of rays (for integration).</param>
    /// <param name="rnd">Global (per-thread) instance of the random generator.</param>
    /// <param name="p0">Ray origin.</param>
    /// <param name="p1">Ray direction vector.</param>
    /// <returns>True if the ray (viewport position) is valid.</returns>
    public bool GetRay ( double x, double y, int rank, int total, RandomJames rnd, out Vector3d p0, out Vector3d p1 )
    {
      return GetRay( x, y, out p0, out p1 );
    }
  }

  /// <summary>
  /// Point light source w/o intensity attenuation.
  /// </summary>
  public class PointLightSource : ILightSource
  {
    /// <summary>
    /// 3D coordinate of the source.
    /// </summary>
    protected Vector3d position;

    /// <summary>
    /// Intensity of the source expressed as color tuple.
    /// </summary>
    protected double[] intensity;

    /// <summary>
    /// Monochromatic light source.
    /// </summary>
    public PointLightSource ( Vector3d pos, double intens )
    {
      position = pos;
      intensity = new double[] { intens, intens, intens };
    }

    /// <summary>
    /// Color light source.
    /// </summary>
    public PointLightSource ( Vector3d pos, double[] intens )
    {
      position = pos;
      intensity = intens;
    }

    /// <summary>
    /// Returns intensity (incl. color) of the source contribution to the given scene point.
    /// Simple variant, w/o an integration support.
    /// </summary>
    /// <param name="intersection">Scene point (only world coordinates and normal vector are needed).</param>
    /// <param name="dir">Direction to the source is set here (zero vector for omnidirectional source). Not normalized!</param>
    /// <returns>Intensity vector in current color space or null if the point is not lit.</returns>
    public virtual double[] GetIntensity ( Intersection intersection, out Vector3d dir )
    {
      dir = position - intersection.CoordWorld;
      if ( Vector3d.Dot( dir, intersection.Normal ) <= 0.0 )
        return null;

      return intensity;
    }

    /// <summary>
    /// Returns intensity (incl. color) of the source contribution to the given scene point.
    /// Internal integration support.
    /// </summary>
    /// <param name="intersection">Scene point (only world coordinates and normal vector are needed).</param>
    /// <param name="rank">Rank of this sample, 0 <= rank < total (for integration).</param>
    /// <param name="total">Total number of samples (for integration).</param>
    /// <param name="rnd">Global (per-thread) instance of the random generator.</param>
    /// <param name="dir">Direction to the source is set here (zero vector for omnidirectional source). Not normalized!</param>
    /// <returns>Intensity vector in current color space or null if the point is not lit.</returns>
    public virtual double[] GetIntensity ( Intersection intersection, int rank, int total, RandomJames rnd, out Vector3d dir )
    {
      return GetIntensity( intersection, out dir );
    }
  }

  /// <summary>
  /// Ambient white light source.
  /// </summary>
  public class AmbientLightSource : ILightSource
  {
    protected double[] intensity;

    public AmbientLightSource ( double intens )
    {
      intensity = new double[] { intens, intens, intens };
    }

    /// <summary>
    /// Returns intensity (incl. color) of the source contribution to the given scene point.
    /// Simple variant, w/o an integration support.
    /// </summary>
    /// <param name="intersection">Scene point (only world coordinates and normal vector are needed).</param>
    /// <param name="dir">Direction to the source is set here (zero vector for omnidirectional source).</param>
    /// <returns>Intensity vector in current color space or null if the point is not lit.</returns>
    public double[] GetIntensity ( Intersection intersection, out Vector3d dir )
    {
      dir = Vector3d.Zero;
      return intensity;
    }

    /// <summary>
    /// Returns intensity (incl. color) of the source contribution to the given scene point.
    /// Internal integration support.
    /// </summary>
    /// <param name="intersection">Scene point (only world coordinates and normal vector are needed).</param>
    /// <param name="rank">Rank of this sample, 0 <= rank < total (for integration).</param>
    /// <param name="total">Total number of samples (for integration).</param>
    /// <param name="rnd">Global (per-thread) instance of the random generator.</param>
    /// <param name="dir">Direction to the source is set here (zero vector for omnidirectional source).</param>
    /// <returns>Intensity vector in current color space or null if the point is not lit.</returns>
    public double[] GetIntensity ( Intersection intersection, int rank, int total, RandomJames rnd, out Vector3d dir )
    {
      return GetIntensity( intersection, out dir );
    }
  }

  /// <summary>
  /// Rectangle light source with intensity attenuation.
  /// </summary>
  public class RectangleLightSource : PointLightSource
  {
    /// <summary>
    ///  "Horizontal" edge of the rectangle.
    /// </summary>
    protected Vector3d width = new Vector3d( 1.0, 0.0, 0.0 );

    /// <summary>
    /// "Vertical" edge of the rectangle.
    /// </summary>
    protected Vector3d height = new Vector3d( 0.0, 1.0, 0.0 );

    /// <summary>
    /// Dimming polynom coefficients: light is dimmed by the factor of
    /// Dim[0] + Dim[1] * D + Dim[2] * D^2.
    /// Can be "null" for no dimming..
    /// </summary>
    public double[] Dim
    {
      get;
      set;
    }

    /// <summary>
    /// Support data container for sampling states.
    /// </summary>
    public class SamplingState
    {
      protected RectangleLightSource source;

      protected int rank;
      protected int total;

      protected RandomJames rnd;
      protected RandomJames.Permutation permU;
      protected RandomJames.Permutation permV;

      protected double u, v;
      public Vector3d sample;

      public SamplingState ( RectangleLightSource src, RandomJames _rnd )
      {
        source = src;
        rnd = _rnd;
        permU = new RandomJames.Permutation();
        permV = new RandomJames.Permutation();
        rank = total = 0;
      }

      public void generateSample ( int r, int t )
      {
        if ( t > 1 )
        {
          if ( r == rank &&
               t == total )
            return;

          int uCell, vCell;
          if ( t != total || r < rank )       // [re-]initialization
          {
            total = t;
            uCell = rnd.PermutationFirst( total, ref permU );
            vCell = rnd.PermutationFirst( total, ref permV );
          }
          else
          {
            uCell = Math.Max( rnd.PermutationNext( ref permU ), 0 );
            vCell = Math.Max( rnd.PermutationNext( ref permV ), 0 );
          }

          rank = r;

          // point sample will be placed into [ uCell, vCell ] cell:
          u = (uCell + rnd.UniformNumber()) / total;
          v = (vCell + rnd.UniformNumber()) / total;
        }
        else
        {
          u = rnd.UniformNumber();
          v = rnd.UniformNumber();
        }

        // TODO: do something like:
        sample = source.position + u * source.width + v * source.height;
      }
    }

    /// <summary>
    /// Set of sampling states (one per random-generator instance / thread).
    /// </summary>
    protected Dictionary<int, SamplingState> states = new Dictionary<int, SamplingState>();

    /// <summary>
    /// Monochromatic light source.
    /// </summary>
    public RectangleLightSource ( Vector3d pos, Vector3d wid, Vector3d hei, double intens )
      : base( pos, intens )
    {
      width = wid;
      height = hei;
      Dim = new double[] { 0.5, 1.0, 0.1 };
    }

    /// <summary>
    /// Color light source.
    /// </summary>
    public RectangleLightSource ( Vector3d pos, Vector3d wid, Vector3d hei, double[] intens )
      : base( pos, intens )
    {
      width = wid;
      height = hei;
      Dim = new double[] { 0.5, 1.0, 0.1 };
    }

    /// <summary>
    /// Returns intensity (incl. color) of the source contribution to the given scene point.
    /// Internal integration support.
    /// </summary>
    /// <param name="intersection">Scene point (only world coordinates and normal vector are needed).</param>
    /// <param name="rank">Rank of this sample, 0 <= rank < total (for integration).</param>
    /// <param name="total">Total number of samples (for integration).</param>
    /// <param name="rnd">Global (per-thread) instance of the random generator.</param>
    /// <param name="dir">Direction to the source is set here (zero vector for omnidirectional source). Not normalized!</param>
    /// <returns>Intensity vector in current color space or null if the point is not lit.</returns>
    public override double[] GetIntensity ( Intersection intersection, int rank, int total, RandomJames rnd, out Vector3d dir )
    {
      if ( rnd == null )
        return GetIntensity( intersection, out dir );

      SamplingState ss;
      lock ( states )
      {
        if ( !states.TryGetValue( rnd.GetHashCode(), out ss ) )
        {
          ss = new SamplingState( this, rnd );
          states.Add( rnd.GetHashCode(), ss );
        }
      }

      // generate a [new] sample:
      ss.generateSample( rank, total );
      dir = ss.sample - intersection.CoordWorld;
      if ( Vector3d.Dot( dir, intersection.Normal ) <= 0.0 )
        return null;

      if ( Dim == null || Dim.Length < 3 ) return intensity;

      double dist = dir.Length;
      double dimCoef = 1.0 / (Dim[0] + dist * (Dim[1] + dist * Dim[2]));
      int bands = intensity.Length;
      double[] result = new double[ bands ];
      for ( int i = 0; i < bands; i++ )
        result[ i ] = intensity[ i ] * dimCoef;

      return result;
    }
  }

  /// <summary>
  /// Simple Phong-like reflectance model: material description.
  /// </summary>
  public class PhongMaterial : IMaterial
  {
    /// <summary>
    /// Base surface color.
    /// </summary>
    public double[] Color
    {
      get;
      set;
    }

    /// <summary>
    /// Coefficient of diffuse reflection.
    /// </summary>
    public double Kd
    {
      get;
      set;
    }

    /// <summary>
    /// Coefficient of specular reflection.
    /// </summary>
    public double Ks
    {
      get;
      set;
    }

    /// <summary>
    /// Specular exponent.
    /// </summary>
    public int H
    {
      get;
      set;
    }

    /// <summary>
    /// Ambient term (for ambient light sources only).
    /// </summary>
    public double Ka
    {
      get;
      set;
    }

    /// <summary>
    /// Coefficient of transparency.
    /// </summary>
    public double Kt
    {
      get;
      set;
    }

    protected double _n;

    /// <summary>
    /// Absolute index of refraction.
    /// </summary>
    public double n
    {
      get { return _n; }
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

    public PhongMaterial ( PhongMaterial m )
    {
      Color = (double[])m.Color.Clone();
      Ka = m.Ka;
      Kd = m.Kd;
      Ks = m.Ks;
      H = m.H;
      Kt = m.Kt;
      n = m.n;
    }

    public PhongMaterial ( double[] color, double ka, double kd, double ks, int h )
    {
      Color = color;
      Ka = ka;
      Kd = kd;
      Ks = ks;
      H  = h;
      Kt = 0.0;
      n  = 1.5;
    }

    public PhongMaterial () : this( new double[] { 1.0, 0.9, 0.4 }, 0.2, 0.7, 0.2, 16 )
    {
    }

    public object Clone ()
    {
      return new PhongMaterial( this );
    }
  }

  /// <summary>
  /// Simple Phong-like reflectance model: the model itself.
  /// </summary>
  public class PhongModel : IReflectanceModel
  {
    public IMaterial DefaultMaterial ()
    {
      return new PhongMaterial();
    }

    public double[] ColorReflection ( Intersection intersection, Vector3d input, Vector3d output, ReflectionComponent comp )
    {
      return ColorReflection( intersection.Material, intersection.Normal, input, output, comp );
    }

    public double[] ColorReflection ( IMaterial material, Vector3d normal, Vector3d input, Vector3d output, ReflectionComponent comp )
    {
      if ( !(material is PhongMaterial) ) return null;

      PhongMaterial mat = (PhongMaterial)material;
      int bands = mat.Color.Length;
      double[] result = new double[ bands ];
      bool viewOut = Vector3d.Dot( output, normal ) > 0.0;
      double coef;

      if ( input.Equals( Vector3d.Zero ) )    // ambient term only..
      {
        // dim ambient light if viewer is inside
        coef = viewOut ? mat.Ka : (mat.Ka * mat.Kt);
        for ( int i = 0; i < bands; i++ )
          result[ i ] = coef * mat.Color[ i ];

        return result;
      }

      // directional light source:
      input.Normalize();
      double cosAlpha = Vector3d.Dot( input, normal );
      bool   lightOut = cosAlpha > 0.0;
      double ks = mat.Ks;
      double kd = mat.Kd;
      double kt = mat.Kt;

      Vector3d r = Vector3d.Zero;
      coef       = 1.0;
      if ( viewOut == lightOut )            // viewer and source are on the same side..
      {
        if ( (comp & ReflectionComponent.SPECULAR_REFLECTION) != 0 )
        {
          double cos2 = cosAlpha + cosAlpha;
          r = normal * cos2 - input;

          if ( !lightOut &&                   // total reflection check
               -cosAlpha <= mat.cosTotal )
            if ( (ks += kt) + kd > 1.0 )
              ks = 1.0 - kd;
        }
      }
      else                                  // opposite sides => use specular refraction
      {
        if ( (comp & ReflectionComponent.SPECULAR_REFRACTION) != 0 )
          r = Geometry.SpecularRefraction( normal, mat.n, input );
        coef = kt;
      }

      double diffuse = (comp & ReflectionComponent.DIFFUSE) == 0 ? 0.0 : coef * kd * Math.Abs( cosAlpha );
      double specular = 0.0;

      if ( r != Vector3d.Zero )
      {
        double cosBeta = Vector3d.Dot( r, output );
        if ( cosBeta > 0.0 )
          specular = coef * ks * Arith.Pow( cosBeta, mat.H );
      }

      for ( int i = 0; i < bands; i++ )
        result[i] = diffuse * mat.Color[i] + specular;

      return result;
    }
  }

  /// <summary>
  /// Simple texture able to modulate surface color.
  /// </summary>
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

    public CheckerTexture ( double fu, double fv, double[] color )
    {
      Fu = fu;
      Fv = fv;
      Color2 = (double[])color.Clone();
    }

    /// <summary>
    /// Apply the relevant value-modulation in the given Intersection instance.
    /// Simple variant, w/o an integration support.
    /// </summary>
    /// <param name="inter">Data object to modify.</param>
    /// <returns>Hash value (texture signature) for adaptive subsampling.</returns>
    public long Apply ( Intersection inter )
    {
      double u = inter.TextureCoord.X * Fu;
      double v = inter.TextureCoord.Y * Fv;

      long ui = (long)Math.Floor( u );
      long vi = (long)Math.Floor( v );

      if ( ((ui + vi) & 1) != 0 )
        Array.Copy( Color2, inter.SurfaceColor, Math.Min( Color2.Length, inter.SurfaceColor.Length ) );

      return (ui + RandomStatic.numericRecipes( vi ));
    }

    /// <summary>
    /// Apply the relevant value-modulation in the given Intersection instance.
    /// Internal integration support.
    /// </summary>
    /// <param name="inter">Data object to modify.</param>
    /// <param name="rank">Rank of this sample, 0 <= rank < total (for integration).</param>
    /// <param name="total">Total number of samples (for integration).</param>
    /// <param name="rnd">Global (per-thread) instance of the random generator.</param>
    /// <returns>Hash value (texture signature) for adaptive subsampling.</returns>
    public long Apply ( Intersection inter, int rank, int total, RandomJames rnd )
    {
      return Apply( inter );
    }
  }
}
