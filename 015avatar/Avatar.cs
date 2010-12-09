using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Windows.Forms;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Threading;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Scene3D;

namespace _015avatar
{
  public partial class Form1
  {
    // Realtime based animation:
    private bool animate = true;
    private double timeInSeconds = 0.0;
    private double timeOffset = 0.0;

    #region Camera attributes

    /// <summary>
    /// Current camera position.
    /// </summary>
    private Vector3 eye = new Vector3( 1.0f, 2.0f, 10.0f );

    /// <summary>
    /// Current point to look at.
    /// </summary>
    private Vector3 pointAt = Vector3.Zero;

    /// <summary>
    /// Current "up" vector.
    /// </summary>
    private Vector3 up = Vector3.UnitY;

    /// <summary>
    /// Horizontal field-of-view angle in radians.
    /// </summary>
    private float fov = 1.2f;

    #endregion

    #region Dynamics

    private double lastFrameTime = DateTime.Now.Ticks * 1.0e-7;

    /// <summary>
    /// Current camera speed vector [1/sec].
    /// </summary>
    private Vector3 dEye = new Vector3( 0.4f, 0.0f, -0.8f );

    /// <summary>
    /// Second derivative of the camera position (used in mouse-wheel handler).
    /// </summary>
    private Vector3 ddEye = new Vector3( 0.2f, 0.0f, -0.4f );

    /// <summary>
    /// Current point-at speed vector [1/sec].
    /// </summary>
    private Vector3 dPointAt = Vector3.Zero;

    #endregion

    /// <summary>
    /// Function called whenever the main application is idle..
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Application_Idle ( object sender, EventArgs e )
    {
      while ( glControl1.IsIdle )
      {
        glControl1.Invalidate();
        Thread.Sleep( 5 );

        long now = DateTime.Now.Ticks;
        if ( now - lastFpsTime > 10000000 )      // more than 1 sec
        {
          double fps = fpsCounter * 1.0e7 / (now - lastFpsTime);
          lastFpsTime = now;
          fpsCounter = 0;
          labelFps.Text = String.Format( "FPS: {0:0.0}", fps );
        }
      }
    }

    /// <summary>
    /// Called in case the GLcontrol geometry changes.
    /// </summary>
    private void SetupViewport ()
    {
      int wid = glControl1.Width;
      int hei = glControl1.Height;

      // 1. set ViewPort transform:
      GL.Viewport( 0, 0, wid, hei );

      // 2. set projection matrix
      GL.MatrixMode( MatrixMode.Projection );
      Matrix4 proj = Matrix4.CreatePerspectiveFieldOfView( fov, wid / (float)hei, 0.1f, 100.0f );
      GL.LoadMatrix( ref proj );
    }

    /// <summary>
    /// Setup of a camera called for every frame prior to any rendering.
    /// </summary>
    private void SetCamera ()
    {
      // !!!{{ TODO: add camera setup here

      // Animation based on simple linear dynamics:
      timeInSeconds = DateTime.Now.Ticks * 1.0e-7;
      if ( animate )
      {
        float dTime = (float)(timeInSeconds - lastFrameTime);
        eye += dEye * dTime;
        pointAt += dPointAt * dTime;
      }
      lastFrameTime = timeInSeconds;

      // Current modelview matrix based on eye and pointAt position:
      GL.MatrixMode( MatrixMode.Modelview );
      Matrix4 lookAt = Matrix4.LookAt( eye, pointAt, up );
      GL.LoadMatrix( ref lookAt );

      // !!!}}
    }

    /// <summary>
    /// Rendering of one frame.
    /// </summary>
    private void Render ()
    {
      if ( !loaded ) return;

      fpsCounter++;
      GL.Clear( ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit );

      SetCamera();

      // Scene rendering:
      if ( scene != null &&
           scene.Triangles > 0 )        // scene is nonempty => render it
      {
        GL.Color3( Color.Yellow );

        GL.BindBuffer( BufferTarget.ArrayBuffer, VBOid[ 0 ] );
        GL.VertexPointer( 3, VertexPointerType.Float, stride, IntPtr.Zero );
        if ( scene.Normals > 0 )
          GL.NormalPointer( NormalPointerType.Float, stride, (IntPtr)Vector3.SizeInBytes );

        GL.BindBuffer( BufferTarget.ElementArrayBuffer, VBOid[ 1 ] );
        GL.DrawElements( BeginMode.Triangles, scene.Triangles * 3, DrawElementsType.UnsignedInt, IntPtr.Zero );
      }
      else                              // single yellow triangle..
      {
        GL.Color3( Color.Yellow );
        GL.Begin( BeginMode.Triangles );
        GL.Vertex3( -2.0f, -2.0f, 0.0f );
        GL.Vertex3( 3.0f, -2.0f, 0.0f );
        GL.Vertex3( 0.0f, 4.0f, 0.0f );
        GL.End();
      }

      glControl1.SwapBuffers();
    }

    #region 2D variant

    private void SetupViewport2D ()
    {
      int wid = glControl1.Width;
      int hei = glControl1.Height;

      GL.MatrixMode( MatrixMode.Projection );
      GL.LoadIdentity();
      GL.Ortho( 0, wid, 0, hei, -1, 1 );
      GL.Viewport( 0, 0, wid, hei );
    }

    private void Render2D ()
    {
      if ( !loaded ) return;

      fpsCounter++;
      GL.Clear( ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit );

      GL.MatrixMode( MatrixMode.Modelview );
      GL.LoadIdentity();
      GL.Translate( 200.0f, 200.0f, 0.0f );

      // Animation:
      if ( animate )
        timeInSeconds = DateTime.Now.Ticks * 1.0e-7;
      GL.Rotate( (float)Math.IEEERemainder( timeInSeconds * 45.0, 360.0 ), Vector3.UnitZ );

      GL.Color3( Color.Yellow );
      GL.Begin( BeginMode.Triangles );
      GL.Vertex3( -20.0f, -20.0f, 0.0f );
      GL.Vertex3( 30.0f, -25.0f, 0.0f );
      GL.Vertex3( 0.0f, 40.0f, 0.0f );
      GL.End();

      glControl1.SwapBuffers();
    }

    #endregion

    private void glControl1_MouseDown ( object sender, MouseEventArgs e )
    {
      // !!!{{ TODO: add the event handler here
      // !!!}}
    }

    private void glControl1_MouseUp ( object sender, MouseEventArgs e )
    {
      // !!!{{ TODO: add the event handler here
      // !!!}}
    }

    private void glControl1_MouseMove ( object sender, MouseEventArgs e )
    {
      // !!!{{ TODO: add the event handler here
      // !!!}}
    }

    private void glControl1_MouseWheel ( object sender, MouseEventArgs e )
    {
      // !!!{{ TODO: add the event handler here

      if ( e.Delta != 0 )
      {
        float notches = e.Delta / 120.0f;
        dEye += ddEye * notches;
      }

      // !!!}}
    }

    private void glControl1_KeyDown ( object sender, KeyEventArgs e )
    {
      // !!!{{ TODO: add the event handler here
      // !!!}}
    }

    private void glControl1_KeyUp ( object sender, KeyEventArgs e )
    {
      // !!!{{ TODO: add the event handler here

      if ( e.KeyCode == Keys.R )              // R => reset camera position
        eye = new Vector3( 1.0f, 2.0f, 10.0f );

      if ( e.KeyCode == Keys.Space )          // space => toggle animation mode
      {
        if ( !animate )        // restart the animation
          timeOffset += DateTime.Now.Ticks * 1.0e-7 - timeInSeconds;
        animate = !animate;
      }

      // !!!}}
    }

  }

}
