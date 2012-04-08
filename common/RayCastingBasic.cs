using System;
using System.Collections.Generic;
using System.Drawing;
using MathSupport;
using OpenTK;

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
      Gamma = 2.2;              // the right value "by the book" */
      ProgressData = null;
    }

    /// <summary>
    /// Renders the single pixel of an image.
    /// </summary>
    /// <param name="x">Horizontal coordinate.</param>
    /// <param name="y">Vertical coordinate.</param>
    /// <param name="color">Computed pixel color.</param>
    public virtual void RenderPixel ( int x, int y, double[] color )
    {
      ImageFunction.GetSample( x, y, color );

      // gamma-encoding:
      if ( Gamma > 0.001 )
      {
        double g = 1.0 / Gamma;
        for ( int b = 0; b < color.Length; b++ )
          color[ b ] = Arith.Clamp( Math.Pow( color[ b ], g ), 0.0, 1.0 );
      }
    }

    /// <summary>
    /// Renders the given rectangle into the given raster image.
    /// </summary>
    /// <param name="image">Pre-initialized raster image.</param>
    /// <param name="x1"></param>
    /// <param name="y1"></param>
    /// <param name="x2"></param>
    /// <param name="y2"></param>
    public virtual void RenderRectangle ( Bitmap image, int x1, int y1, int x2, int y2 )
    {
      if ( ProgressData != null )
        lock ( ProgressData )
        {
          ProgressData.Finished = 0.0f;
          ProgressData.Message = "";
          if ( !ProgressData.Continue )
            return;
        }
      double[] color = new double[ 3 ];     // pixel color
      double g = (Gamma > 0.001) ? 1.0 / Gamma : 0.0;

      // run several phases of image rendering:
      int cell = 32;                        // cell size
      while ( cell > 1 && cell > Adaptive )
        cell >>= 1;
      int initCell = cell;

      int x, y;
      bool xParity, yParity;
      float total = (x2 - x1) * (y2 - y1);
      long counter = 0L;

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
              // determine sample color ..
              ImageFunction.GetSample( x + 0.5, y + 0.5, color );

              for ( int b = 0; b < color.Length; b++ )
                color[ b ] = Arith.Clamp( (g > 0.0) ? Math.Pow( color[ b ], g ) : color[ b ], 0.0, 1.0 );

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

              counter++;
              if ( ProgressData != null )
                lock ( ProgressData )
                {
                  if ( !ProgressData.Continue )
                    return;
                  ProgressData.Finished = counter / total;
                  if ( (counter & 0xFFFL) == 0 )
                    ProgressData.Sync( image );
                }
            }
      }
      while ( (cell >>= 1) > 0 );         // do one phase
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
    /// <param name="p0">Ray origin.</param>
    /// <param name="p1">Ray direction vector.</param>
    /// <returns>True if the ray (viewport position) is valid.</returns>
    public bool GetRay ( double x, double y, int rank, int total, out Vector3d p0, out Vector3d p1 )
    {
      return GetRay( x, y, out p0, out p1 );
    }
  }

  /// <summary>
  /// White point light source w/o intensity attenuation.
  /// </summary>
  public class PointLightSource : ILightSource
  {
    /// <summary>
    /// 3D coordinate of the source.
    /// </summary>
    protected Vector3d coordinate;

    /// <summary>
    /// Intensity of the source expressed as color tuple.
    /// </summary>
    protected double[] intensity;

    public PointLightSource ( Vector3d coord, double intens )
    {
      coordinate = coord;
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
      dir = coordinate - intersection.CoordWorld;
      if ( Vector3d.Dot( dir, intersection.Normal ) <= 0.0 )
        return null;

      dir.Normalize();
      return intensity;
    }

    /// <summary>
    /// Returns intensity (incl. color) of the source contribution to the given scene point.
    /// Internal integration support.
    /// </summary>
    /// <param name="intersection">Scene point (only world coordinates and normal vector are needed).</param>
    /// <param name="rank">Rank of this sample, 0 <= rank < total (for integration).</param>
    /// <param name="total">Total number of samples (for integration).</param>
    /// <param name="dir">Direction to the source is set here (zero vector for omnidirectional source).</param>
    /// <returns>Intensity vector in current color space or null if the point is not lit.</returns>
    public double[] GetIntensity ( Intersection intersection, int rank, int total, out Vector3d dir )
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
    /// <param name="dir">Direction to the source is set here (zero vector for omnidirectional source).</param>
    /// <returns>Intensity vector in current color space or null if the point is not lit.</returns>
    public double[] GetIntensity ( Intersection intersection, int rank, int total, out Vector3d dir )
    {
      return GetIntensity( intersection, out dir );
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

    public double cosTotal = 0.0;

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

    public PhongMaterial () : this( new double[] { 1.0, 0.9, 0.4 }, 0.2, 0.5, 0.3, 16 )
    {
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
          specular = coef * ks * Math.Pow( cosBeta, mat.H );
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
    /// <returns>Hash value (texture signature) for adaptive subsampling.</returns>
    public long Apply ( Intersection inter, int rank, int total )
    {
      return Apply( inter );
    }
  }
}
