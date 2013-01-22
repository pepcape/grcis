////////////////////////////////////////////////////////////////////////////////
// Based on (c) 2012 Pavel Ševeček's code
// Original template & final modifications: (c) 2010-2013 Josef Pelikán
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Drawing;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace _056avatar
{
  public partial class Form1
  {
    // Realtime based animation:
    private double timeInSeconds = 0.0;
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
    /// Acceleration.
    /// </summary>
    private float acc = 100;

    /// <summary>
    /// Deceleration (inertia).
    /// </summary>
    private float dec = 0.95f;

    /// <summary>
    /// Vertical field-of-view angle in radians.
    /// </summary>
    private float fov = 1.0f;

    /// <summary>
    /// Camera's far point.
    /// </summary>
    private float far = 1000.0f;

    #endregion

    /// <summary>
    /// Last location of a mouse pointer.
    /// </summary>
    private Point mouse;

    /// <summary>
    /// Rotation coefficients;
    /// </summary>
    private float ax = 0, ay = 0;

    /// <summary>
    /// Mouse sensitivity.
    /// </summary>
    private float sens = 0.004f;

    /// <summary>
    /// Key is pressed? (no modifier keys).
    /// </summary>
    bool[] keys = new bool[ 256 ];

    /// <summary>
    /// Reset camera.
    /// </summary>
    private void SetDefault ()
    {
      eye = new Vector3( 0.0f, 0.0f, 30.0f );
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
      Matrix4 rot = Matrix4.CreateFromAxisAngle( left, angle );
      dir = Vector3.Transform( dir, rot );
      up = Vector3.Transform( up, rot );
    }

    /// <summary>
    /// Rotate around local y axis.
    /// </summary>
    private void Rudder ( ref Vector3 dir, Vector3 up, float angle )
    {
      Matrix4 rot = Matrix4.CreateFromAxisAngle( up, angle );
      dir = Vector3.Transform( dir, rot );
    }

    /// <summary>
    /// Rotate around local z axis.
    /// </summary>
    private void Rotation ( Vector3 dir, ref Vector3 up, float angle )
    {
      Matrix4 rot = Matrix4.CreateFromAxisAngle( dir, angle );
      up = Vector3.Transform( up, rot );
    }

    /// <summary>
    /// Must be called after window geometry change.
    /// </summary>
    private void SetupViewport ()
    {
      int wid = glControl1.Width;
      int hei = glControl1.Height;

      // 1. set ViewPort transform:
      GL.Viewport( 0, 0, wid, hei );

      // 2. set projection matrix
      GL.MatrixMode( MatrixMode.Projection );
      Matrix4 proj = Matrix4.CreatePerspectiveFieldOfView( fov, wid / (float)hei, 0.1f, far );
      GL.LoadMatrix( ref proj );

      SetDefault();
    }

    /// <summary>
    /// Setup of a camera called for every frame prior to any rendering.
    /// </summary>
    private void SetCamera ()
    {
      timeInSeconds = DateTime.Now.Ticks * 1.0e-7;

      float dTime = (float)(timeInSeconds - lastFrameTime);
      lastFrameTime = timeInSeconds;

      GL.MatrixMode( MatrixMode.Modelview );
      // look from "eye", in direction of "dir"
      Matrix4 lookAt = Matrix4.LookAt( eye, eye + dir, up );
      GL.LoadMatrix( ref lookAt );

      Vector3 left = Vector3.Cross( up, dir );

      // set camera velocity
      if ( keys[ (int)Keys.W ] ) dEye += dir * acc * dTime;
      if ( keys[ (int)Keys.S ] ) dEye -= dir * acc * dTime;
      if ( keys[ (int)Keys.A ] ) dEye += left * acc * dTime;
      if ( keys[ (int)Keys.D ] ) dEye -= left * acc * dTime;
      if ( keys[ (int)Keys.Prior ] ) dEye += up * acc * dTime;
      if ( keys[ (int)Keys.Next ] ) dEye -= up * acc * dTime;

      // move camera
      eye += dEye * dTime;

      // deceleration (translation)
      dEye *= dec;

      // deceleration (rotation)
      if ( !keys[ (int)Keys.LButton ] )
      {
        Elevator( ref dir, ref up, ay );
        Rudder( ref dir, up, -ax );
        ax *= dec;
        ay *= dec;
      }
    }

    /// <summary>
    /// Rendering of one frame.
    /// </summary>
    private void Render ()
    {
      if ( !loaded ) return;

      frameCounter++;
      GL.Clear( ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit );
      GL.ShadeModel( checkSmooth.Checked ? ShadingModel.Smooth : ShadingModel.Flat );
      GL.PolygonMode( MaterialFace.FrontAndBack, checkWireframe.Checked ? PolygonMode.Line : PolygonMode.Fill );

      SetCamera();

      // Scene rendering:
      RenderScene();

      glControl1.SwapBuffers();
    }

    /// <summary>
    /// Mouse button down.
    /// </summary>
    private void glControl1_MouseDown ( object sender, MouseEventArgs e )
    {
      keys[ (int)Keys.LButton ] = true;
      mouse = e.Location;
    }

    /// <summary>
    /// Mouse button up.
    /// </summary>
    private void glControl1_MouseUp ( object sender, MouseEventArgs e )
    {
      keys[ (int)Keys.LButton ] = false;
    }

    /// <summary>
    /// Mouse pointer move.
    /// </summary>
    private void glControl1_MouseMove ( object sender, MouseEventArgs e )
    {
      if ( keys[ (int)Keys.LButton ] )
      {
        // rotate camera
        ax = (mouse.X - e.Location.X) * sens;
        ay = (mouse.Y - e.Location.Y) * sens;
        Elevator( ref dir, ref up, ay );
        Rudder( ref dir, up, -ax );
        mouse = e.Location;
      }
    }

    /// <summary>
    /// Mouse wheel event.
    /// </summary>
    private void glControl1_MouseWheel ( object sender, MouseEventArgs e )
    {
      if ( e.Delta != 0 )
      {
        float notches = e.Delta / 300.0f;
        Rotation( dir, ref up, notches );
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
