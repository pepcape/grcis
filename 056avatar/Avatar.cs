////////////////////////////////////////////////////////////////////////////////
// Based on (c) 2012 Pavel Ševeček's code
// Original template & final modifications: (c) 2010-2015 Josef Pelikán
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Drawing;
using System.Windows.Forms;
using MathSupport;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace _056avatar
{
  public partial class Form1
  {
    // Realtime based animation:
    private double lastFrameTime = DateTime.Now.Ticks * 1.0e-7;

    #region Camera attributes

    /// <summary>
    /// Camera position.
    /// </summary>
    private Vector3 eye;

    /// <summary>
    /// Camera direction.
    /// </summary>
    private Vector3 dir;

    /// <summary>
    /// Up vector.
    /// </summary>
    private Vector3 up;

    /// <summary>
    /// Camera velocity.
    /// </summary>
    private Vector3 dEye;

    /// <summary>
    /// Acceleration (for translation).
    /// </summary>
    private float acc = 100.0f;

    /// <summary>
    /// Angular velocity around viewing vector (v-rotation).
    /// </summary>
    private float dAng = 0.0f;

    /// <summary>
    /// Acceleration (for rotation).
    /// </summary>
    private float accRot = 5.0f;

    /// <summary>
    /// Deceleration (inertia).
    /// </summary>
    private float dec = 0.95f;

    /// <summary>
    /// Vertical field-of-view angle in radians.
    /// </summary>
    private float fov = 1.0f;

    /// <summary>
    /// FoV increment (coefficient).
    /// </summary>
    private double fovInc = 1.04;

    /// <summary>
    /// Sensitivity of the +/- keys.
    /// </summary>
    private float plusInc = 1.1f;

    /// <summary>
    /// Should the Viewport be recalculated? (for FoV change..)
    /// </summary>
    private bool setupViewport = false;

    /// <summary>
    /// Camera's near point.
    /// </summary>
    private float near = 0.01f;

    /// <summary>
    /// Camera's far point.
    /// </summary>
    private float far = 1000.0f;

    /// <summary>
    /// Center of the scene.
    /// </summary>
    public Vector3 center = Vector3.Zero;

    /// <summary>
    /// Scene diameter (for default view only).
    /// </summary>
    public float diameter = 10.0f;

    #endregion

    /// <summary>
    /// Last location of a mouse pointer.
    /// </summary>
    private Point mouse;

    /// <summary>
    /// Time of the last mouse-pointer move.
    /// </summary>
    private double lastMouseMove = DateTime.Now.Ticks * 1.0e-7;

    /// <summary>
    /// Last dTime for mouse dragging.
    /// </summary>
    private double lastDelta = 0.0f;

    /// <summary>
    /// Rotation coefficients.
    /// </summary>
    private float ax = 0.0f, ay = 0.0f;

    /// <summary>
    /// Panning coefficients.
    /// </summary>
    private float panx = 0.0f, pany = 0.0f;

    /// <summary>
    /// Mouse sensitivity.
    /// </summary>
    private float sens = 0.005f;

    /// <summary>
    /// Mouse sensitivity for panning.
    /// </summary>
    private float sensPan = 0.05f;

    /// <summary>
    /// Mouse sensitivity for acceleration.
    /// </summary>
    private float sensAcc = 0.1f;

    /// <summary>
    /// Current canvas width in pixels (affects mouse movement sensitivity).
    /// </summary>
    private int width;

    /// <summary>
    /// Current canvas height in pixels (affects mouse movement sensitivity).
    /// </summary>
    private int height;

    /// <summary>
    /// Standard size of shortest rectangle side.
    /// </summary>
    private int standardSize = 350;

    /// <summary>
    /// Key is pressed? (no modifier keys).
    /// </summary>
    bool[] keys = new bool[ 256 ];

    /// <summary>
    /// Reset camera.
    /// </summary>
    private void SetDefault ()
    {
      eye = center;
      eye.Z += (float)((0.5 * diameter) / Math.Tan( fov * 0.5 ));
      dir = -Vector3.UnitZ;
      up = Vector3.UnitY;
      dEye = Vector3.Zero;
    }

    /// <summary>
    /// Rotate around local x axis
    /// </summary>
    private void Elevator ( ref Vector3 dir, ref Vector3 up, float angle )
    {
      Vector3 left = Vector3.Cross( up, dir );
      Matrix3 rot  = Matrix3.CreateFromAxisAngle( left, angle );
      dir = Vector3.Transform( dir, rot );
      up  = Vector3.Transform( up, rot );
    }

    /// <summary>
    /// Rotate around local y axis.
    /// </summary>
    private void Rudder ( ref Vector3 dir, Vector3 up, float angle )
    {
      Matrix3 rot = Matrix3.CreateFromAxisAngle( up, angle );
      dir = Vector3.Transform( dir, rot );
    }

    /// <summary>
    /// Rotate around local z axis.
    /// </summary>
    private void Rotation ( Vector3 dir, ref Vector3 up, float angle )
    {
      Matrix3 rot = Matrix3.CreateFromAxisAngle( dir, angle );
      up = Vector3.Transform( up, rot );
    }

    /// <summary>
    /// Must be called after window geometry change.
    /// </summary>
    private void SetupViewport ( bool setDefault )
    {
      width = glControl1.Width;
      height = glControl1.Height;

      // 1. set ViewPort transform:
      GL.Viewport( 0, 0, width, height );

      // 2. set projection matrix
      GL.MatrixMode( MatrixMode.Projection );
      Matrix4 proj = Matrix4.CreatePerspectiveFieldOfView( fov, width / (float)height, near, far );
      GL.LoadMatrix( ref proj );

      if ( setDefault )
        SetDefault();
    }

    /// <summary>
    /// Setup of a camera called for every frame prior to any rendering.
    /// </summary>
    private void SetCamera ()
    {
      double timeInSeconds = DateTime.Now.Ticks * 1.0e-7;

      float dTime = (float)(timeInSeconds - lastFrameTime);
      lastFrameTime = timeInSeconds;

      if ( setupViewport )
      {
        setupViewport = false;
        SetupViewport( false );
      }

      GL.MatrixMode( MatrixMode.Modelview );
      // look from "eye", in direction of "dir"
      Matrix4 lookAt = Matrix4.LookAt( eye, eye + dir, up );
      GL.LoadMatrix( ref lookAt );

      Vector3 left = Vector3.Cross( up, dir );

      // global velocity adjustments:
      if ( keys[ (int)Keys.Add ] )
      {
        acc *= plusInc;
        accRot *= plusInc;
        sensPan *= plusInc;
        sensAcc *= plusInc;
      }
      if ( keys[ (int)Keys.Subtract ] )
      {
        acc /= plusInc;
        accRot /= plusInc;
        sensPan /= plusInc;
        sensAcc /= plusInc;
      }

      // change camera velocity
      if ( keys[ (int)Keys.W ] || keys[ (int)Keys.Up ] )
        dEye += dir * acc * dTime;
      if ( keys[ (int)Keys.S ] || keys[ (int)Keys.Down ] )
        dEye -= dir * acc * dTime;
      if ( keys[ (int)Keys.A ] || keys[ (int)Keys.Left ] )
        dEye += left * acc * dTime;
      if ( keys[ (int)Keys.D ] || keys[ (int)Keys.Right ] )
        dEye -= left * acc * dTime;
      if ( keys[ (int)Keys.R ] || keys[ (int)Keys.PageUp ] )
        dEye += up * acc * dTime;
      if ( keys[ (int)Keys.F ] || keys[ (int)Keys.PageDown ] )
        dEye -= up * acc * dTime;

      // move camera
      eye += dEye * dTime;

      // deceleration (translation)
      if ( !keys[ (int)Keys.LButton ] )
        dEye *= dec;

      // v-rotation:
      if ( keys[ (int)Keys.Q ] || keys[ (int)Keys.Home ] )
        dAng += accRot * dTime;
      if ( keys[ (int)Keys.E ] || keys[ (int)Keys.End ] )
        dAng -= accRot * dTime;

      Rotation( dir, ref up, dAng * dTime );
      dAng *= dec;

      // deceleration (rotation)
      if ( !keys[ (int)Keys.RButton ] )
      {
        Elevator( ref dir, ref up, ay * dTime );
        Rudder( ref dir, up, -ax * dTime );
        ax *= dec;
        ay *= dec;
      }

      // deceleration (panning)
      if ( !keys[ (int)Keys.MButton ] )
      {
        eye -= (left * panx + up * pany) * dTime;
        panx *= dec;
        pany *= dec;
      }
    }

    /// <summary>
    /// Rendering of one frame.
    /// </summary>
    private void Render ()
    {
      if ( !loaded )
        return;

      frameCounter++;
      GL.Clear( ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit );
      GL.ShadeModel( checkSmooth.Checked ? ShadingModel.Smooth : ShadingModel.Flat );
      GL.PolygonMode( checkTwosided.Checked ? MaterialFace.FrontAndBack : MaterialFace.Front,
                      checkWireframe.Checked ? PolygonMode.Line : PolygonMode.Fill );
      if ( checkTwosided.Checked )
        GL.Disable( EnableCap.CullFace );
      else
        GL.Enable( EnableCap.CullFace );

      SetCamera();
      RenderScene();

      glControl1.SwapBuffers();
    }

    /// <summary>
    /// Mouse button down.
    /// </summary>
    private void glControl1_MouseDown ( object sender, MouseEventArgs e )
    {
      int but;
      switch ( e.Button )
      {
        case MouseButtons.Middle:
          but = (int)Keys.MButton;
          break;

        case MouseButtons.Right:
          but = (int)Keys.RButton;
          break;

        default:
          but = (int)Keys.LButton;
          break;
      }
      keys[ but ] = true;
      mouse = e.Location;
      lastMouseMove = DateTime.Now.Ticks * 1.0e-7;

      if ( e.Button == MouseButtons.Right )
        ax = ay = 0.0f;

      if ( e.Button == MouseButtons.Middle )
        panx = pany = 0.0f;
    }

    /// <summary>
    /// Mouse button up.
    /// </summary>
    private void glControl1_MouseUp ( object sender, MouseEventArgs e )
    {
      int but;
      switch ( e.Button )
      {
        case MouseButtons.Middle:
          but = (int)Keys.MButton;
          break;

        case MouseButtons.Right:
          but = (int)Keys.RButton;
          break;

        default:
          but = (int)Keys.LButton;
          break;
      }
      keys[ but ] = false;

      double now = DateTime.Now.Ticks * 1.0e-7;
      bool pause = now - lastMouseMove > 0.1;

      if ( e.Button == MouseButtons.Right )
        if ( pause )
          ax = ay = 0.0f;
        else
          if ( lastDelta > 0.0 )
          {
            ax /= (float)lastDelta;
            ay /= (float)lastDelta;
          }

      if ( e.Button == MouseButtons.Middle )
        if ( pause )
          panx = pany = 0.0f;
        else
          if ( lastDelta > 0.0 )
          {
            panx /= (float)lastDelta;
            pany /= (float)lastDelta;
          }

      mouse = e.Location;
    }

    /// <summary>
    /// Mouse pointer move.
    /// </summary>
    private void glControl1_MouseMove ( object sender, MouseEventArgs e )
    {
      double now = DateTime.Now.Ticks * 1.0e-7;
      lastDelta = now - lastMouseMove;
      lastMouseMove = now;
      float coef = standardSize / (float)Math.Min( width, height );

      if ( keys[ (int)Keys.RButton ] )
      {
        // rotate camera
        ax = (mouse.X - e.Location.X) * sens * fov * coef;
        ay = (mouse.Y - e.Location.Y) * sens * fov * coef;
        Elevator( ref dir, ref up, ay );
        Rudder( ref dir, up, -ax );
      }

      if ( keys[ (int)Keys.MButton ] )
      {
        // mouse panning
        panx = (mouse.X - e.Location.X) * sensPan * coef;
        pany = (mouse.Y - e.Location.Y) * sensPan * coef;
        eye -= Vector3.Cross( up, dir ) * panx + up * pany;
      }

      if ( keys[ (int)Keys.LButton ] )
      {
        dEye += ((mouse.Y - e.Location.Y) * sensAcc * acc * (float)lastDelta) * dir;
      }

      mouse = e.Location;
    }

    /// <summary>
    /// Mouse wheel event.
    /// </summary>
    private void glControl1_MouseWheel ( object sender, MouseEventArgs e )
    {
      if ( e.Delta != 0 )
      {
        double notches = e.Delta / 120.0f;
        fov *= (float)Math.Pow( fovInc, notches );
        fov = Arith.Clamp( fov, 0.05f, 1.6f );
        setupViewport = true;
      }
    }

    /// <summary>
    /// Keyboard key pressed.
    /// </summary>
    private void glControl1_KeyDown ( object sender, KeyEventArgs e )
    {
      keys[ e.KeyValue ] = true;
    }

    /// <summary>
    /// Keyboard key released.
    /// </summary>
    private void glControl1_KeyUp ( object sender, KeyEventArgs e )
    {
      keys[ e.KeyValue ] = false;

      if ( e.KeyCode == Keys.Space )
        SetDefault();   // Reset all
    }
  }
}
