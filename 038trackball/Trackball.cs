using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace _038trackball
{
  public partial class Form1
  {
    #region Camera attributes

    /// <summary>
    /// Current camera position.
    /// </summary>
    private Vector3 eye = new Vector3( 0.0f, 0.0f, 10.0f );

    /// <summary>
    /// Current point to look at.
    /// </summary>
    private Vector3 pointAt = Vector3.Zero;

    /// <summary>
    /// Current "up" vector.
    /// </summary>
    private Vector3 up = Vector3.UnitY;

    /// <summary>
    /// Vertical field-of-view angle in radians.
    /// </summary>
    private float fov = 1.0f;

    /// <summary>
    /// Camera's far point.
    /// </summary>
    private float far = 200.0f;

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
          double fps = frameCounter * 1.0e7 / (now - lastFpsTime);
          double tps = triangleCounter * 1.0e7 / (now - lastFpsTime);
          lastFpsTime = now;
          frameCounter = 0;
          triangleCounter = 0L;
          if ( tps < 5.0e5 )
            labelFps.Text = String.Format( "Fps: {0:0.0}, Tps: {1:0}k", fps, (tps * 1.0e-3) );
          else
            labelFps.Text = String.Format( "Fps: {0:0.0}, Tps: {1:0.0}m", fps, (tps * 1.0e-6) );
        }
      }
    }

    /// <summary>
    /// Sets up a projective viewport
    /// </summary>
    private void SetupViewport ()
    {
      int width  = glControl1.Width;
      int height = glControl1.Height;

      // 1. set ViewPort transform:
      GL.Viewport( 0, 0, width, height );

      // 2. set projection matrix
      GL.MatrixMode( MatrixMode.Projection );
      Matrix4 proj = Matrix4.CreatePerspectiveFieldOfView( fov, (float)width / (float)height, 0.1f, far );
      GL.LoadMatrix( ref proj );
    }

    /// <summary>
    /// Rendering of one frame.
    /// </summary>
    private void Render ()
    {
      if ( !loaded ) return;

      frameCounter++;
      GL.Clear( ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit );
      GL.ShadeModel( ShadingModel.Flat );
      GL.PolygonMode( MaterialFace.Front, PolygonMode.Fill );
      GL.Enable( EnableCap.CullFace );

      SetupViewport();

      GL.MatrixMode( MatrixMode.Modelview );
      Matrix4 translation = Matrix4.CreateTranslation( 0.0f, 0.0f, -10.0f );
      GL.LoadMatrix( ref translation );

      RenderScene();

      glControl1.SwapBuffers();
    }

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
      // HINT: for most mouses, e.delta / 120 is the number of wheel clicks, +/- indicated the direction

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
      // !!!}}
    }

    private void buttonReset_Click ( object sender, EventArgs e )
    {
      // !!!{{ TODO: add the event handler here
      // !!!}}
    }
  }
}
