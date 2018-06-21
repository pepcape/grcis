using System;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace MathSupport
{
  class Ellipse
  {
    float a, b, c;
    Vector3 center;

    // Sphere constructor
    public Ellipse ( float r, Vector3 center ) : this( r, r, r, center )
    {
    }

    // Ellipse constructor
    public Ellipse ( float a, float b, float c, Vector3 center )
    {
      this.a = a;
      this.b = b;
      this.c = c;
      this.center = center;
    }

    // "polar coordinates" method
    public Vector3 IntersectionI ( float x, float y )
    {
      Vector3d o = new Vector3d( 0, 0, -c );
      Vector3d m = new Vector3d( x - center.X, y - center.Y, c );
      Vector3d v = o - m;
      v.Normalize();
      double A = v.X * v.X * b * b * c * c + v.Y * v.Y * a * a * c * c + v.Z * v.Z * a * a * b * b;
      double B = 2 * (v.X * b * b * c * c + v.Y * a * a * c * c + v.Z * a * a * b * b);
      double C = v.X * v.X * b * b * c * c + v.Y * v.Y * a * a * c * c + v.Z * a * a * b * b - a * a * b * b * c * c;
      double D = Math.Sqrt( B * B - 4 * A * C );
      double t = (-B - D) / (2 * A);
      double X = m.X + t * v.X;
      double Y = m.Y + t * v.Y;
      double Z = m.Z + t * v.Z;
      return new Vector3( (float)X, -(float)Y, (float)Z );
    }

    // "parallel rays" method
    public Vector3? Intersection ( float x, float y, bool restricted )
    {
      x -= center.X;
      y -= center.Y;

      if ( (x < -a) || (x > a) || (y < -b) || (y > b) )
      {
        float x1 = (float)Math.Sqrt( (a * a * b * b * y * y) / (b * b * y * y + x * x) );
        float x2 = -x1;
        float y1 = (y * x1) / -x;
        float y2 = (y * x2) / -x;
        if ( Math.Abs( x - x1 ) < Math.Abs( x - x2 ) )
          return new Vector3( x1, y1, 0 );
        else
          return new Vector3( x2, y2, 0 );
      }

      float z = (1 - (x * x) / (a * a) - (y * y) / (b * b)) * c * c;
      if ( z < 0 )
        return null;
      z = (float)Math.Sqrt( z );
      return new Vector3( x, -y, z );
    }
  }

  /// <summary>
  /// Trackball interactive 3D scene navigation
  /// Original code: Matyas Brenner
  /// </summary>
  public class Trackball
  {
    /// <summary>
    /// Center of the rotation (world coords).
    /// </summary>
    public Vector3 Center
    {
      get;
      set;
    }

    /// <summary>
    /// Scene diameter (for default zoom factor only).
    /// </summary>
    private float diameter = 5.0f;

    public float Diameter
    {
      get
      {
        return diameter;
      }
      set
      {
        diameter = value;
      }
    }

    /// <summary>
    /// Current camera position (world coords).
    /// </summary>
    public Vector3 Eye
    {
      get
      {
        return Vector3.TransformPosition( Vector3.Zero, ModelViewInv );
      }
    }

    /// <summary>
    /// Vertical field-of-view angle in radians.
    /// </summary>
    public float Fov
    {
      get;
      set;
    }

    /// <summary>
    /// Zoom factor (multiplication).
    /// </summary>
    public float Zoom
    {
      get;
      set;
    }

    /// <summary>
    /// Which mouse button is used for trackball movement?
    /// </summary>
    public MouseButtons Button
    {
      get;
      set;
    }

    public Trackball ( Vector3 cent, float diam =5.0f )
    {
      Center      =  cent;
      Diameter    =  diam;
      MinZoom     =  0.1f;
      MaxZoom     = 20.0f;
      Zoom        =  1.0f;
      Fov         =  1.0f;
      Perspective =  true;
      Button      = MouseButtons.Left;
    }

    Matrix4 prevRotation = Matrix4.Identity;
    Matrix4 rotation     = Matrix4.Identity;

    Ellipse ellipse;
    Vector3? a, b;

    Matrix4 perspectiveProjection;
    Matrix4 ortographicProjection;

    /// <summary>
    /// Perspective / orthographic projection?
    /// </summary>
    public bool Perspective
    {
      get;
      set;
    }

    public Matrix4 PerspectiveProjection
    {
      get
      {
        return perspectiveProjection;
      }
    }

    public Matrix4 OrthographicProjection
    {
      get
      {
        return ortographicProjection;
      }
    }

    public Matrix4 Projection
    {
      get
      {
        return Perspective ? perspectiveProjection : ortographicProjection;
      }
    }

    public float MinZoom
    {
      get;
      set;
    }

    public float MaxZoom
    {
      get;
      set;
    }

    /// <summary>
    /// Sets up a projective viewport
    /// </summary>
    public void GLsetupViewport ( int width, int height, float near =0.01f, float far =1000.0f )
    {
      // 1. set ViewPort transform:
      GL.Viewport( 0, 0, width, height );

      // 2. set projection matrix
      perspectiveProjection = Matrix4.CreatePerspectiveFieldOfView( Fov, width / (float)height, near, far );
      float minSize = 2.0f * Math.Min( width, height );
      ortographicProjection = Matrix4.CreateOrthographic( diameter * width / minSize,
                                                          diameter * height / minSize,
                                                          near, far );
      GLsetProjection();
      setEllipse( width, height );
    }

    /// <summary>
    /// Setup of a camera called for every frame prior to any rendering.
    /// </summary>
    public void GLsetCamera ()
    {
      // not needed if shaders are active .. but doesn't make any harm..
      Matrix4 modelview = ModelView;
      GL.MatrixMode( MatrixMode.Modelview );
      GL.LoadMatrix( ref modelview );
    }

    public Matrix4 ModelView
    {
      get
      {
        return Matrix4.CreateTranslation( -Center ) *
               Matrix4.CreateScale( Zoom / diameter ) *
               prevRotation *
               rotation *
               Matrix4.CreateTranslation( 0.0f, 0.0f, -1.5f );
      }
    }

    public Matrix4 ModelViewInv
    {
      get
      {
        Matrix4 rot = prevRotation * rotation;
        rot.Transpose();

        return Matrix4.CreateTranslation( 0.0f, 0.0f, 1.5f ) *
               rot *
               Matrix4.CreateScale( diameter / Zoom ) *
               Matrix4.CreateTranslation( Center );
      }
    }

    public void Reset ()
    {
      Zoom         = 1.0f;
      rotation     = Matrix4.Identity;
      prevRotation = Matrix4.Identity;
    }

    private void setEllipse ( int width, int height )
    {
      width  /= 2;
      height /= 2;

      ellipse = new Ellipse( Math.Min( width, height ), new Vector3( width, height, 0 ) );
    }

    private Matrix4 calculateRotation ( Vector3? a, Vector3? b, bool sensitive )
    {
      if ( !a.HasValue || !b.HasValue )
        return rotation;

      if ( a.Value == b.Value )
        return Matrix4.Identity;

      Vector3 axis = Vector3.Cross( a.Value, b.Value );
      float angle = Vector3.CalculateAngle( a.Value, b.Value );
      if ( sensitive )
        angle *= 0.4f;
      return Matrix4.CreateFromAxisAngle( axis, angle );
    }

    public void GLtogglePerspective ()
    {
      Perspective = !Perspective;
      GLsetProjection();
    }

    public void GLsetProjection ()
    {
      // not needed if shaders are active .. but doesn't make any harm..
      GL.MatrixMode( MatrixMode.Projection );
      if ( Perspective )
        GL.LoadMatrix( ref perspectiveProjection );
      else
        GL.LoadMatrix( ref ortographicProjection );
    }

    //--- GUI interaction ---

    /// <summary>
    /// Handles mouse-button down.
    /// </summary>
    /// <returns>True if handled.</returns>
    public bool MouseDown ( MouseEventArgs e )
    {
      if ( e.Button != Button )
        return false;

      a = ellipse.IntersectionI( e.X, e.Y );
      return true;
    }

    /// <summary>
    /// Handles mouse-button up.
    /// </summary>
    /// <returns>True if handled.</returns>
    public bool MouseUp ( MouseEventArgs e )
    {
      if ( e.Button != Button )
        return false;

      prevRotation *= rotation;
      rotation = Matrix4.Identity;
      a = null;
      b = null;
      return true;
    }

    /// <summary>
    /// Handles mouse move.
    /// </summary>
    /// <returns>True if handled.</returns>
    public bool MouseMove ( MouseEventArgs e )
    {
      if ( e.Button != Button )
        return false;
       
      b = ellipse.IntersectionI( e.X, e.Y );
      rotation = calculateRotation( a, b, (Control.ModifierKeys & Keys.Shift) != Keys.None );
      return true;
    }

    /// <summary>
    /// Handles mouse-wheel change.
    /// </summary>
    /// <returns>True if handled.</returns>
    public bool MouseWheel ( MouseEventArgs e )
    {
      float dZoom = e.Delta / 120.0f;
      Zoom *= (float)Math.Pow( 1.04, dZoom );

      // zoom bounds:
      Zoom = Arith.Clamp( Zoom, MinZoom, MaxZoom );
      return true;
    }

    /// <summary>
    /// Handle keyboard-key down.
    /// </summary>
    /// <returns>True if handled.</returns>
    public bool KeyDown ( KeyEventArgs e )
    {
      // nothing yet
      return false;
    }

    /// <summary>
    /// Handle keyboard-key up.
    /// </summary>
    /// <returns>True if handled.</returns>
    public bool KeyUp ( KeyEventArgs e )
    {
      if ( e.KeyCode == Keys.O )
      {
        e.Handled = true;
        GLtogglePerspective();
        return true;
      }

      return false;
    }
  }
}
