using System;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace MathSupport
{
  public interface IRealtimeCamera
  {
    /// <summary>
    /// Center of the scene in world coordinates (rotation center if applicable).
    /// </summary>
    Vector3 Center
    {
      get;
      set;
    }

    /// <summary>
    /// Scene diameter in world coordinates (for default zoom).
    /// </summary>
    float Diameter
    {
      get;
      set;
    }

    /// <summary>
    /// Zoom factor (multiplication).
    /// </summary>
    float Zoom
    {
      get;
      set;
    }

    /// <summary>
    /// Zoom factor lower bound (if applicable).
    /// </summary>
    float MinZoom
    {
      get;
      set;
    }

    /// <summary>
    /// Zoom factor upper bound (if applicable).
    /// </summary>
    float MaxZoom
    {
      get;
      set;
    }

    /// <summary>
    /// Camera time (in seconds).
    /// </summary>
    double Time
    {
      get;
      set;
    }

    /// <summary>
    /// Camera time lower bound (in seconds).
    /// </summary>
    double MinTime
    {
      get;
      set;
    }

    /// <summary>
    /// Camera time upper bound (in seconds).
    /// </summary>
    double MaxTime
    {
      get;
      set;
    }

    /// <summary>
    /// Update the camera instance.
    /// </summary>
    /// <param name="param">Text param from the UI.</param>
    /// <param name="cameraFile">Optional camera definition file.</param>
    void Update ( string param, string cameraFile );

    /// <summary>
    /// Resets the camera (whatever it means).
    /// </summary>
    void Reset ();

    /// <summary>
    /// Setup of a camera called for every frame prior to any rendering.
    /// </summary>
    void GLsetCamera ();

    /// <summary>
    /// Sets projection matrix to the openGL system.
    /// </summary>
    void GLsetProjection ();

    /// <summary>
    /// Toggles perspective/orthographic projection.
    /// </summary>
    void GLtogglePerspective ();

    /// <summary>
    /// Sets up a projective viewport.
    /// Can ignore all the arguments in case of scripted camera.
    /// </summary>
    /// <param name="width">Viewport width in pixels.</param>
    /// <param name="height">Viewport height in pixels.</param>
    /// <param name="near">Near frustum distance if applicable.</param>
    /// <param name="far">Far frustum distance if applicable.</param>
    void GLsetupViewport ( int width, int height, float near = 0.01f, float far = 1000.0f );

    /// <summary>
    /// Gets a current model-view transformation matrix.
    /// </summary>
    Matrix4 ModelView
    {
      get;
    }

    /// <summary>
    /// Perspective / orthographic projection?
    /// </summary>
    bool UsePerspective
    {
      get;
      set;
    }

    /// <summary>
    /// Gets a current projection matrix. 
    /// </summary>
    Matrix4 Projection
    {
      get;
    }

    /// <summary>
    /// Gets a current eye/camera position in world coordinates.
    /// </summary>
    Vector3 Eye
    {
      get;
    }

    /// <summary>
    /// Vertical field-of-view angle in radians.
    /// </summary>
    float Fov
    {
      get;
      set;
    }

    /// <summary>
    /// Handle keyboard-key down.
    /// </summary>
    /// <returns>True if handled.</returns>
    bool KeyDown ( KeyEventArgs e );

    /// <summary>
    /// Handle keyboard-key up.
    /// </summary>
    /// <returns>True if handled.</returns>
    bool KeyUp ( KeyEventArgs e );

    /// <summary>
    /// Handles mouse-button down.
    /// </summary>
    /// <returns>True if handled.</returns>
    bool MouseDown ( MouseEventArgs e );

    /// <summary>
    /// Handles mouse-button up.
    /// </summary>
    /// <returns>True if handled.</returns>
    bool MouseUp ( MouseEventArgs e );

    /// <summary>
    /// Handles mouse move.
    /// </summary>
    /// <returns>True if handled.</returns>
    bool MouseMove ( MouseEventArgs e );

    /// <summary>
    /// Handles mouse-wheel change.
    /// </summary>
    /// <returns>True if handled.</returns>
    bool MouseWheel ( MouseEventArgs e );
  }

  public class DefaultRealtimeCamera : IRealtimeCamera
  {
    /// <summary>
    /// Center of the scene in world coordinates (rotation center if applicable).
    /// </summary>
    public virtual Vector3 Center
    {
      get;
      set;
    }

    /// <summary>
    /// Scene diameter in world coordinates (for default zoom).
    /// </summary>
    public virtual float Diameter
    {
      get;
      set;
    } = 5.0f;

    /// <summary>
    /// Zoom factor (multiplication).
    /// </summary>
    public virtual float Zoom
    {
      get;
      set;
    }

    /// <summary>
    /// Zoom factor lower bound (if applicable).
    /// </summary>
    public virtual float MinZoom
    {
      get;
      set;
    }

    /// <summary>
    /// Zoom factor upper bound (if applicable).
    /// </summary>
    public virtual float MaxZoom
    {
      get;
      set;
    }

    /// <summary>
    /// Camera time (in seconds).
    /// </summary>
    public virtual double Time
    {
      get;
      set;
    } = 0.0;

    /// <summary>
    /// Camera time lower bound (in seconds).
    /// </summary>
    public virtual double MinTime
    {
      get;
      set;
    } = 0.0;

    /// <summary>
    /// Camera time upper bound (in seconds).
    /// </summary>
    public virtual double MaxTime
    {
      get;
      set;
    } = 1.0;

    /// <summary>
    /// Update the camera instance.
    /// </summary>
    /// <param name="param">Text param from the UI.</param>
    /// <param name="cameraFile">Optional camera definition file.</param>
    public virtual void Update ( string param, string cameraFile )
    {
    }

    /// <summary>
    /// Resets the camera (whatever it means).
    /// </summary>
    public virtual void Reset ()
    {
      Time = MinTime;
    }

    /// <summary>
    /// Setup of a camera called for every frame prior to any rendering.
    /// </summary>
    public virtual void GLsetCamera ()
    {
      // not needed if shaders are active .. but doesn't make any harm..
      Matrix4 modelview = ModelView;
      GL.MatrixMode( MatrixMode.Modelview );
      GL.LoadMatrix( ref modelview );
    }

    /// <summary>
    /// Sets projection matrix to the openGL system.
    /// </summary>
    public virtual void GLsetProjection ()
    {
    }

    /// <summary>
    /// Toggles perspective/orthographic projection.
    /// </summary>
    public virtual void GLtogglePerspective ()
    {
      UsePerspective = !UsePerspective;
      GLsetProjection();
    }

    /// <summary>
    /// Sets up a projective viewport.
    /// Can ignore all the arguments in case of scripted camera.
    /// </summary>
    /// <param name="width">Viewport width in pixels.</param>
    /// <param name="height">Viewport height in pixels.</param>
    /// <param name="near">Near frustum distance if applicable.</param>
    /// <param name="far">Far frustum distance if applicable.</param>
    public virtual void GLsetupViewport ( int width, int height, float near = 0.01f, float far = 1000.0f )
    {
    }

    /// <summary>
    /// Gets a current model-view transformation matrix.
    /// </summary>
    public virtual Matrix4 ModelView
    {
      get;
    }

    /// <summary>
    /// Gets a current model-view transformation matrix.
    /// </summary>
    public virtual Matrix4 ModelViewInv
    {
      get;
    }

    /// <summary>
    /// Perspective / orthographic projection?
    /// </summary>
    public virtual bool UsePerspective
    {
      get;
      set;
    } = true;

    /// <summary>
    /// Gets a current projection matrix. 
    /// </summary>
    public virtual Matrix4 Projection
    {
      get;
    }

    /// <summary>
    /// Gets a current eye/camera position in world coordinates.
    /// </summary>
    public virtual Vector3 Eye
    {
      get
      {
        return Vector3.TransformPosition( Vector3.Zero, ModelViewInv );
      }
    }

    /// <summary>
    /// Vertical field-of-view angle in radians.
    /// </summary>
    public virtual float Fov
    {
      get;
      set;
    } = 1.0f;

    /// <summary>
    /// Handle keyboard-key down.
    /// </summary>
    /// <returns>True if handled.</returns>
    public virtual bool KeyDown ( KeyEventArgs e )
    {
      return false;
    }

    /// <summary>
    /// Handle keyboard-key up.
    /// </summary>
    /// <returns>True if handled.</returns>
    public virtual bool KeyUp ( KeyEventArgs e )
    {
      return false;
    }

    /// <summary>
    /// Handles mouse-button down.
    /// </summary>
    /// <returns>True if handled.</returns>
    public virtual bool MouseDown ( MouseEventArgs e )
    {
      return false;
    }

    /// <summary>
    /// Handles mouse-button up.
    /// </summary>
    /// <returns>True if handled.</returns>
    public virtual bool MouseUp ( MouseEventArgs e )
    {
      return false;
    }

    /// <summary>
    /// Handles mouse move.
    /// </summary>
    /// <returns>True if handled.</returns>
    public virtual bool MouseMove ( MouseEventArgs e )
    {
      return false;
    }

    /// <summary>
    /// Handles mouse-wheel change.
    /// </summary>
    /// <returns>True if handled.</returns>
    public virtual bool MouseWheel ( MouseEventArgs e )
    {
      return false;
    }
  }

  /// <summary>
  /// Trackball interactive 3D scene navigation
  /// Original code: Matyas Brenner
  /// </summary>
  public class Trackball : DefaultRealtimeCamera
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
    /// Which mouse button is used for trackball movement?
    /// </summary>
    public MouseButtons Button
    {
      get;
      set;
    }

    public Trackball ( Vector3 cent, float diam =5.0f )
    {
      Center         =  cent;
      Diameter       =  diam;
      MinZoom        =  0.1f;
      MaxZoom        = 20.0f;
      Zoom           =  1.0f;
      UsePerspective =  true;
      Button         = MouseButtons.Left;
    }

    Matrix4 prevRotation = Matrix4.Identity;
    Matrix4 rotation     = Matrix4.Identity;

    Ellipse ellipse;
    Vector3? a, b;

    Matrix4 perspectiveProjection;
    Matrix4 ortographicProjection;

    public override Matrix4 Projection
    {
      get
      {
        return UsePerspective ? perspectiveProjection : ortographicProjection;
      }
    }

    /// <summary>
    /// Sets up a projective viewport
    /// </summary>
    public override void GLsetupViewport ( int width, int height, float near =0.01f, float far =1000.0f )
    {
      // 1. set ViewPort transform:
      GL.Viewport( 0, 0, width, height );

      // 2. set projection matrix
      perspectiveProjection = Matrix4.CreatePerspectiveFieldOfView( Fov, width / (float)height, near, far );
      float minSize = 2.0f * Math.Min( width, height );
      ortographicProjection = Matrix4.CreateOrthographic( Diameter * width / minSize,
                                                          Diameter * height / minSize,
                                                          near, far );
      GLsetProjection();
      setEllipse( width, height );
    }

    public override Matrix4 ModelView
    {
      get
      {
        return Matrix4.CreateTranslation( -Center ) *
               Matrix4.CreateScale( Zoom / Diameter ) *
               prevRotation *
               rotation *
               Matrix4.CreateTranslation( 0.0f, 0.0f, -1.5f );
      }
    }

    public override Matrix4 ModelViewInv
    {
      get
      {
        Matrix4 rot = prevRotation * rotation;
        rot.Transpose();

        return Matrix4.CreateTranslation( 0.0f, 0.0f, 1.5f ) *
               rot *
               Matrix4.CreateScale( Diameter / Zoom ) *
               Matrix4.CreateTranslation( Center );
      }
    }

    /// <summary>
    /// Resets the camera - it will look in the negative Z direction. Y axis will point upwards.
    /// </summary>
    public override void Reset ()
    {
      base.Reset();
      Zoom         = 1.0f;
      rotation     = Matrix4.Identity;
      prevRotation = Matrix4.Identity;
    }

    /// <summary>
    /// Resets the camera by a caller-provided rotational matrix.
    /// </summary>
    /// <param name="rot">Matrix of rotation from view direction to the negative Z axis (must be matrix of rotation).</param>
    public void Reset ( Matrix4 rot )
    {
      base.Reset();
      Zoom         = 1.0f;
      rotation     = Matrix4.Identity;
      prevRotation = rot;
    }

    /// <summary>
    /// Resets the camera to look at the required direction. Y axis will point upwards.
    /// </summary>
    /// <param name="dir">Required view direction.</param>
    public void Reset ( Vector3 dir )
    {
      dir.Normalize();
      if ( Geometry.IsZero( dir.Length ) )
      {
        Reset();
        return;
      }

      // 1. rotation around vertical (Y) axis
      float len = (float)Math.Sqrt( dir.X * dir.X + dir.Z * dir.Z );
      Matrix4 roty = Matrix4.Identity;
      if ( !Geometry.IsZero( len ) )
      {
        roty.M11 = roty.M33 = -dir.Z / len;
        roty.M13 = -(roty.M31 = dir.X / len);
      }

      // 2. rotation around X axis (to look in the negative Z direction)
      float len2 = (float)Math.Sqrt( len * len + dir.Y * dir.Y );
      Matrix4 rotx = Matrix4.Identity;
      if ( !Geometry.IsZero( len2 ) )
      {
        rotx.M22 = rotx.M33 = len / len2;
        rotx.M23 = -(rotx.M32 = dir.Y / len2);
      }

      Reset( roty * rotx );
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

    /// <summary>
    /// Sets projection matrix to the openGL system.
    /// </summary>
    public override void GLsetProjection ()
    {
      // not needed if shaders are active .. but doesn't make any harm..
      GL.MatrixMode( MatrixMode.Projection );
      if ( UsePerspective )
        GL.LoadMatrix( ref perspectiveProjection );
      else
        GL.LoadMatrix( ref ortographicProjection );
    }

    //--- GUI interaction ---

    /// <summary>
    /// Handles mouse-button down.
    /// </summary>
    /// <returns>True if handled.</returns>
    public override bool MouseDown ( MouseEventArgs e )
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
    public override bool MouseUp ( MouseEventArgs e )
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
    public override bool MouseMove ( MouseEventArgs e )
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
    public override bool MouseWheel ( MouseEventArgs e )
    {
      float dZoom = -e.Delta / 120.0f;
      Zoom *= (float)Math.Pow( 1.04, dZoom );

      // zoom bounds:
      Zoom = Arith.Clamp( Zoom, MinZoom, MaxZoom );
      return true;
    }

    /// <summary>
    /// Handle keyboard-key down.
    /// </summary>
    /// <returns>True if handled.</returns>
    public override bool KeyDown ( KeyEventArgs e )
    {
      // nothing yet
      return false;
    }

    /// <summary>
    /// Handle keyboard-key up.
    /// </summary>
    /// <returns>True if handled.</returns>
    public override bool KeyUp ( KeyEventArgs e )
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
