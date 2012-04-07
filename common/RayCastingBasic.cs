using System;
using System.Collections.Generic;
using System.Drawing;
using OpenTK;
using MathSupport;

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
          for ( int b = 0; b < 3; b++ )
            if ( color[ b ] > 1.0 )
              color[ b ] = 1.0;
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

  /// <summary>
  /// Unit sphere as a simple solid able to compute ray-intersection, normal vector
  /// and 2D texture coordinates.
  /// </summary>
  public class Sphere : DefaultSceneNode, ISolid
  {
    /// <summary>
    /// Computes the complete intersection of the given ray with the object. 
    /// </summary>
    /// <param name="p0">Ray origin.</param>
    /// <param name="p1">Ray direction vector.</param>
    /// <returns>Sorted list of intersection records.</returns>
    public override LinkedList<Intersection> Intersect ( Vector3d p0, Vector3d p1 )
    {
      double OD; Vector3d.Dot( ref p0, ref p1, out OD );
      double DD; Vector3d.Dot( ref p1, ref p1, out DD );
      double OO; Vector3d.Dot( ref p0, ref p0, out OO );
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
      i.CoordLocal.X = p0.X + i.T * p1.X;
      i.CoordLocal.Y = p0.Y + i.T * p1.Y;
      i.CoordLocal.Z = p0.Z + i.T * p1.Z;
      result.AddLast( i );

      // second intersection (-OD + d) / DD:
      i = new Intersection( this );
      i.T = (-OD + d) / DD;
      i.Enter =
      i.Front = false;
      i.CoordLocal.X = p0.X + i.T * p1.X;
      i.CoordLocal.Y = p0.Y + i.T * p1.Y;
      i.CoordLocal.Z = p0.Z + i.T * p1.Z;
      result.AddLast( i );

      return result;
    }

    /// <summary>
    /// Complete all relevant items in the given Intersection object.
    /// </summary>
    /// <param name="inter">Intersection instance to complete.</param>
    public override void CompleteIntersection ( Intersection inter )
    {
      // normal vector:
      Vector3d tu, tv;
      Geometry.GetAxes( ref inter.CoordLocal, out tu, out tv );
      tu = Vector3d.TransformVector( tu, inter.LocalToWorld );
      tv = Vector3d.TransformVector( tv, inter.LocalToWorld );
      Vector3d.Cross( ref tu, ref tv, out inter.Normal );

      // 2D texture coordinates:
      double r = Math.Sqrt( inter.CoordLocal.X * inter.CoordLocal.X + inter.CoordLocal.Y * inter.CoordLocal.Y );
      inter.TextureCoord.X = Geometry.IsZero( r ) ? 0.0 : (Math.Atan2( inter.CoordLocal.Y, inter.CoordLocal.X ) / (2.0 * Math.PI) + 0.5 );
      inter.TextureCoord.Y = Math.Atan2( r, inter.CoordLocal.Z ) / Math.PI;
    }
  }

  /// <summary>
  /// Normalized (unit) cube as a simple solid able to compute ray-intersection, normal vector
  /// and 2D texture coordinates. [0,1]^3
  /// </summary>
  public class Cube : DefaultSceneNode, ISolid
  {
    protected enum CubeFaces
    {
      PositiveX,
      NegativeX,
      PositiveY,
      NegativeY,
      PositiveZ,
      NegativeZ
    };

    protected static Vector3d[] Normals =
    {
      new Vector3d(  1,  0,  0 ),
      new Vector3d( -1,  0,  0 ),
      new Vector3d(  0,  1,  0 ),
      new Vector3d(  0, -1,  0 ),
      new Vector3d(  0,  0,  1 ),
      new Vector3d(  0,  0, -1 )
    };

    /// <summary>
    /// Computes the complete intersection of the given ray with the object. 
    /// </summary>
    /// <param name="p0">Ray origin.</param>
    /// <param name="p1">Ray direction vector.</param>
    /// <returns>Sorted list of intersection records.</returns>
    public override LinkedList<Intersection> Intersect ( Vector3d p0, Vector3d p1 )
    {
      double tMin = Double.MinValue;
      double tMax = Double.MaxValue;
      CubeFaces fMin = CubeFaces.PositiveX;
      CubeFaces fMax = CubeFaces.PositiveX;
      double mul, t1, t2;

      // normal = X axis:
      if ( Geometry.IsZero( p1.X ) )
      {
        if ( p0.X <= 0.0 || p0.X >= 1.0 )
          return null;
      }
      else
      {
        mul = 1.0 / p1.X;
        t1 = -p0.X * mul;
        t2 = t1 + mul;

        if ( mul > 0.0 )
        {
          if ( t1 > tMin )
          {
            tMin = t1;
            fMin = CubeFaces.NegativeX;
          }
          if ( t2 < tMax )
          {
            tMax = t2;
            fMax = CubeFaces.PositiveX;
          }
        }
        else
        {
          if ( t2 > tMin )
          {
            tMin = t2;
            fMin = CubeFaces.NegativeX;
          }
          if ( t1 < tMax )
          {
            tMax = t1;
            fMax = CubeFaces.PositiveX;
          }
        }

        if ( tMin >= tMax )
          return null;
      }

      // normal = Y axis:
      if ( Geometry.IsZero( p1.Y ) )
      {
        if ( p0.Y <= 0.0 || p0.Y >= 1.0 )
          return null;
      }
      else
      {
        mul = 1.0 / p1.Y;
        t1 = -p0.Y * mul;
        t2 = t1 + mul;

        if ( mul > 0.0 )
        {
          if ( t1 > tMin )
          {
            tMin = t1;
            fMin = CubeFaces.NegativeY;
          }
          if ( t2 < tMax )
          {
            tMax = t2;
            fMax = CubeFaces.PositiveY;
          }
        }
        else
        {
          if ( t2 > tMin )
          {
            tMin = t2;
            fMin = CubeFaces.NegativeY;
          }
          if ( t1 < tMax )
          {
            tMax = t1;
            fMax = CubeFaces.PositiveY;
          }
        }

        if ( tMin >= tMax )
          return null;
      }

      // normal = Z axis:
      if ( Geometry.IsZero( p1.Z ) )
      {
        if ( p0.Z <= 0.0 || p0.Z >= 1.0 )
          return null;
      }
      else
      {
        mul = 1.0 / p1.Z;
        t1 = -p0.Z * mul;
        t2 = t1 + mul;

        if ( mul > 0.0 )
        {
          if ( t1 > tMin )
          {
            tMin = t1;
            fMin = CubeFaces.NegativeZ;
          }
          if ( t2 < tMax )
          {
            tMax = t2;
            fMax = CubeFaces.PositiveZ;
          }
        }
        else
        {
          if ( t2 > tMin )
          {
            tMin = t2;
            fMin = CubeFaces.NegativeZ;
          }
          if ( t1 < tMax )
          {
            tMax = t1;
            fMax = CubeFaces.PositiveZ;
          }
        }

        if ( tMin >= tMax )
          return null;
      }

      // Finally assemble the two intersections:
      LinkedList<Intersection> result = new LinkedList<Intersection>();

      Intersection i;
      i = new Intersection( this );
      i.T = tMin;
      i.Enter =
      i.Front = true;
      i.CoordLocal = p0 + tMin * p1;
      i.SolidData = fMin;
      result.AddLast( i );

      i = new Intersection( this );
      i.T = tMax;
      i.Enter =
      i.Front = false;
      i.CoordLocal = p0 + tMax * p1;
      i.SolidData = fMax;
      result.AddLast( i );

      return result;
    }

    /// <summary>
    /// Complete all relevant items in the given Intersection object.
    /// </summary>
    /// <param name="inter">Intersection instance to complete.</param>
    public override void CompleteIntersection ( Intersection inter )
    {
      // normal vector:
      Vector3d tu, tv;
      Geometry.GetAxes( ref Normals[ (int)inter.SolidData ], out tu, out tv );
      tu = Vector3d.TransformVector( tu, inter.LocalToWorld );
      tv = Vector3d.TransformVector( tv, inter.LocalToWorld );
      Vector3d.Cross( ref tu, ref tv, out inter.Normal );

      // 2D texture coordinates:
      switch ( (CubeFaces)inter.SolidData )
      {
        case CubeFaces.NegativeX:
        case CubeFaces.PositiveX:
          inter.TextureCoord.X = inter.CoordLocal.Y;
          inter.TextureCoord.Y = inter.CoordLocal.Z;
          break;
        case CubeFaces.NegativeY:
        case CubeFaces.PositiveY:
          inter.TextureCoord.X = inter.CoordLocal.X;
          inter.TextureCoord.Y = inter.CoordLocal.Z;
          break;
        default:
          inter.TextureCoord.X = inter.CoordLocal.X;
          inter.TextureCoord.Y = inter.CoordLocal.Y;
          break;
      }
    }
  }
}
