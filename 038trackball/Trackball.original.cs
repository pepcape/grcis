using System;
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
    /// Setup of a camera called for every frame prior to any rendering.
    /// </summary>
    private void SetCamera ()
    {
      // !!!{{ TODO: add camera setup here

      GL.MatrixMode( MatrixMode.Modelview );
      Matrix4 modelview = Matrix4.CreateTranslation( -center ) *
                          Matrix4.Scale( 1.0f / diameter ) *
                          Matrix4.CreateTranslation( 0.0f, 0.0f, -1.5f );
      GL.LoadMatrix( ref modelview );

      // !!!}}
    }

    private void ResetCamera ()
    {
      // !!!{{ TODO: add camera reset code here
      // !!!}}
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

      SetCamera();

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

      ResetCamera();

      // !!!}}
    }
  }
}
