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

    public IImageFunction ImageFunction
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
    public virtual void RenderPixel ( int x, int y, double[] color )
    {
      ImageFunction.GetSample( x, y, color );
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
      double[] color = new double[ 3 ];
      for ( int y = y1; y < y2; y++ )
        for ( int x = x1; x < x2; x++ )
        {
          ImageFunction.GetSample( x, y, color );
          image.SetPixel( x, y, Color.FromArgb( (int)(color[ 0 ] * 255.0),
                                                (int)(color[ 1 ] * 255.0),
                                                (int)(color[ 2 ] * 255.0) ) );
        }
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
      Vector3d left = Vector3d.Cross( up, direction );
      origin = center + direction;
      double halfWidth  = Math.Tan( 0.5 * hAngle );
      double halfHeight = halfWidth / AspectRatio;
      origin += up * halfHeight + left * halfWidth;
      dx = left * (-2.0 * halfWidth / width);
      dy = up * (2.0 * halfHeight / height);
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
    /// Ray-generator.
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
  }

  /// <summary>
  /// White point light source w/o intensity attenuation.
  /// </summary>
  public class PointLightSource : ILightSource
  {
    protected Vector3d coordinate;

    protected double[] intensity;

    public PointLightSource ( Vector3d coord, double intens )
    {
      coordinate = coord;
      intensity = new double[] { intens, intens, intens };
    }

    /// <summary>
    /// Returns intensity (incl. color) of the source contribution to the given scene point.
    /// </summary>
    /// <param name="intersection">Scene point (only world coordinates and normal vector are needed).</param>
    /// <param name="dir">Direction to the source is set here (optional, can be null).</param>
    /// <returns>Intensity vector in current color space or null if the point is not lit.</returns>
    public double[] GetIntensity ( Intersection intersection, ref Vector3d dir )
    {
      if ( intersection == null ) return null;
      Vector3d d = coordinate - intersection.CoordWorld;
      if ( Vector3d.Dot( d, intersection.Normal ) <= 0.0 ) return null;

      if ( dir != null )
      {
        d.Normalize();
        dir = d;
      }

      return intensity;
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
  public class PhongModel : IReflectionModel
  {
    public double[] ColorReflection ( IMaterial material, Intersection intersection, Vector3d input, Vector3d output )
    {
      return ColorReflection( material, intersection.Normal, input, output );
    }

    public double[] ColorReflection ( IMaterial material, Vector3d normal, Vector3d input, Vector3d output )
    {
      if ( !(material is PhongMaterial) ) return null;

      PhongMaterial mat = (PhongMaterial)material;
      int bands = mat.Color.Length;
      double[] result = new double[ bands ];
      bool viewOut = Vector3d.Dot( output, normal ) > 0.0;
      double coef;

      if ( input.LengthSquared <= Double.Epsilon )    // ambient term only..
      {
          // dim ambient light if viewer is inside
        coef = viewOut ? mat.Ka : (mat.Ka * mat.Kt);
        for ( int i = 0; i < bands; i++ )
          result[i] = coef * mat.Color[i];

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
        double cos2 = cosAlpha + cosAlpha;
        r = normal * cos2 - input;

        if ( !lightOut &&                   // total reflection check
             -cosAlpha <= mat.cosTotal )
          if ( (ks += kt) + kd > 1.0 )
            ks = 1.0 - kd;
      }
      else                                  // opposite sides => use specular refraction
      {
        r = Geometry.SpecularRefraction( normal, mat.n, input );
        coef = kt;
      }

      double diffuse = coef * kd * Math.Abs( cosAlpha );
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

  public class Sphere : DefaultSceneNode, ISolid
  {
    public override LinkedList<Intersection> Intersect( Vector3d p0, Vector3d p1 )
    {
      // ray origin:
      double Ox = p0.X;
      double Oy = p0.Y;
      double Oz = p0.Z;

      // ray direction vector (of arbitrary size):
      double Dx = p1.X;
      double Dy = p1.Y;
      double Dz = p1.Z;

      double OD = Ox * Dx + Oy * Dy + Oz * Dz;
      double DD = Dx * Dx + Dy * Dy + Dz * Dz;
      double OO = Ox * Ox + Oy * Oy + Oz * Oz;
      double  d = OD * OD + DD * (1.0 - OO); // discriminant
      if ( d <= 0.0 ) return null;           // no intersections

      d = Math.Sqrt( d );

      // there will be two intersections: (-OD - d) / DD, (-OD + d) / DD
      LinkedList<Intersection> result = new LinkedList<Intersection>();

      Intersection i;

      // first intersection (-OD - d) / DD:
      i = new Intersection( this );
      i.T = (-OD - d) / DD;
      i.Enter =
      i.Front = true;
      i.CoordLocal = new Vector3d(
        Ox + i.T * Dx,
        Oy + i.T * Dy,
        Oz + i.T * Dz );
      result.AddLast( i );

      // second intersection (-OD + d) / DD:
      i = new Intersection( this );
      i.T = (-OD + d) / DD;
      i.Enter =
      i.Front = false;
      i.CoordLocal = new Vector3d(
        Ox + i.T * Dx,
        Oy + i.T * Dy,
        Oz + i.T * Dz );
      result.AddLast( i );

      return result;
    }

    public virtual void CompleteIntersection ( Intersection inter )
    {
      inter.LocalToWorld = ToWorld();
      inter.WorldToLocal = inter.LocalToWorld;
      inter.WorldToLocal.Invert();
      inter.CoordWorld = Vector3d.TransformPosition( inter.CoordLocal, inter.LocalToWorld );
        // normal vector:
      Vector3d tu, tv;
      Geometry.GetAxes( inter.CoordLocal, out tu, out tv );
      tu = Vector3d.TransformVector( tu, inter.LocalToWorld );
      tv = Vector3d.TransformVector( tv, inter.LocalToWorld );
      inter.Normal = Vector3d.Cross( tu, tv );
        // 2D texture coordinates:
      double r = Math.Sqrt( inter.CoordLocal.X * inter.CoordLocal.X + inter.CoordLocal.Y * inter.CoordLocal.Y );
      inter.TextureCoord = new Vector2d( Geometry.IsZero( r ) ? 0.0 : (Math.Atan2( inter.CoordLocal.Y, inter.CoordLocal.X ) / (2.0 * Math.PI) + 0.5 ),
                                         Math.Atan2( r, inter.CoordLocal.Z ) / Math.PI );
    }
  }
}
