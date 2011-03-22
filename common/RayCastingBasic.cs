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
    public void RenderPixel ( int x, int y, double[] color )
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
    public void RenderRectangle ( Bitmap image, int x1, int y1, int x2, int y2 )
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

  public class StaticCamera : ICamera
  {
    /// <summary>
    /// Width / Height of the viewing area (viewport, frustum).
    /// </summary>
    public double AspectRatio
    {
      get { return width / height };
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
      get { return width };
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
      get { return height };
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
    /// <returns></returns>
    public bool GetRay ( double x, double y, Vector4d p0, Vector3d p1 )
    {
      p0 = center;
      p1 = origin + x * dx + y * dy;
      p1 = p1 - center;
      p1.Normalize();
    }
  }

  /// <summary>
  /// White point light source w/o intensity attenuation.
  /// </summary>
  public class PointLightSource : ILightSource
  {
    protected Vector4d coordinate;

    protected double[] intensity;

    public PointLightSource ( Vector4d coord, double intens )
    {
      coordinate = coord;
      intensity = new double[] { intens, intens, intens };
    }

    /// <summary>
    /// Returns intensity (incl. color) of the source contribution to the given scene point.
    /// </summary>
    /// <param name="intersection">Scene point (only world coordinates and normal vector are needed).</param>
    /// <param name="dir">Direction to the source is set here (optional, can be null).</param>
    /// <returns></returns>
    public double[] GetIntensity ( Intersection intersection, Vector3d dir )
    {
      if ( intersection == null ) return null;
      Vector3d d = coordinate - intersection.CoordWorld;
      if ( Vector3d.Dot( d, intersection.Normal ) <= 0.0 ) return null;

      if ( dir != null )
      {
        d.Normalize();
        dir.X = d.X;
        dir.Y = d.Y;
        dir.Z = d.Z;
      }

      return intensity;
    }
  }

}
