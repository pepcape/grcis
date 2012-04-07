using System;
using System.Threading;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace _038trackball
{
  public partial class Form1
  {
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
    /// Renders a 3D scene, rendering code separated for clarity.
    /// </summary>
    private void RenderScene ()
    {
      // Scene rendering:
      if ( useVBO &&
           scene != null &&
           scene.Triangles > 0 )        // scene is nonempty => render it
      {
        // colors [normals] vertices
        GL.BindBuffer( BufferTarget.ArrayBuffer, VBOid[ 0 ] );
        GL.ColorPointer( 3, ColorPointerType.Float, stride, IntPtr.Zero );
        if ( scene.Normals > 0 )
        {
          GL.NormalPointer( NormalPointerType.Float, stride, (IntPtr)Vector3.SizeInBytes );
          GL.VertexPointer( 3, VertexPointerType.Float, stride, (IntPtr)(Vector3.SizeInBytes * 2) );
        }
        else
          GL.VertexPointer( 3, VertexPointerType.Float, stride, (IntPtr)Vector3.SizeInBytes );

        // index buffer
        GL.BindBuffer( BufferTarget.ElementArrayBuffer, VBOid[ 1 ] );

        // engage!
        triangleCounter += scene.Triangles;
        GL.DrawElements( BeginMode.Triangles, scene.Triangles * 3, DrawElementsType.UnsignedInt, IntPtr.Zero );
      }
      else                              // color cube (JB)
      {
        GL.Begin( BeginMode.Quads );

        GL.Color3( 0.0f, 1.0f, 0.0f );          // Set The Color To Green
        GL.Vertex3( 1.0f, 1.0f, -1.0f );        // Top Right Of The Quad (Top)
        GL.Vertex3( -1.0f, 1.0f, -1.0f );       // Top Left Of The Quad (Top)
        GL.Vertex3( -1.0f, 1.0f, 1.0f );        // Bottom Left Of The Quad (Top)
        GL.Vertex3( 1.0f, 1.0f, 1.0f );         // Bottom Right Of The Quad (Top)

        GL.Color3( 1.0f, 0.5f, 0.0f );          // Set The Color To Orange
        GL.Vertex3( 1.0f, -1.0f, 1.0f );        // Top Right Of The Quad (Bottom)
        GL.Vertex3( -1.0f, -1.0f, 1.0f );       // Top Left Of The Quad (Bottom)
        GL.Vertex3( -1.0f, -1.0f, -1.0f );      // Bottom Left Of The Quad (Bottom)
        GL.Vertex3( 1.0f, -1.0f, -1.0f );       // Bottom Right Of The Quad (Bottom)

        GL.Color3( 1.0f, 0.0f, 0.0f );          // Set The Color To Red
        GL.Vertex3( 1.0f, 1.0f, 1.0f );         // Top Right Of The Quad (Front)
        GL.Vertex3( -1.0f, 1.0f, 1.0f );        // Top Left Of The Quad (Front)
        GL.Vertex3( -1.0f, -1.0f, 1.0f );       // Bottom Left Of The Quad (Front)
        GL.Vertex3( 1.0f, -1.0f, 1.0f );        // Bottom Right Of The Quad (Front)

        GL.Color3( 1.0f, 1.0f, 0.0f );          // Set The Color To Yellow
        GL.Vertex3( 1.0f, -1.0f, -1.0f );       // Bottom Left Of The Quad (Back)
        GL.Vertex3( -1.0f, -1.0f, -1.0f );      // Bottom Right Of The Quad (Back)
        GL.Vertex3( -1.0f, 1.0f, -1.0f );       // Top Right Of The Quad (Back)
        GL.Vertex3( 1.0f, 1.0f, -1.0f );        // Top Left Of The Quad (Back)

        GL.Color3( 0.0f, 0.0f, 1.0f );          // Set The Color To Blue
        GL.Vertex3( -1.0f, 1.0f, 1.0f );        // Top Right Of The Quad (Left)
        GL.Vertex3( -1.0f, 1.0f, -1.0f );       // Top Left Of The Quad (Left)
        GL.Vertex3( -1.0f, -1.0f, -1.0f );      // Bottom Left Of The Quad (Left)
        GL.Vertex3( -1.0f, -1.0f, 1.0f );       // Bottom Right Of The Quad (Left)

        GL.Color3( 1.0f, 0.0f, 1.0f );          // Set The Color To Violet
        GL.Vertex3( 1.0f, 1.0f, -1.0f );        // Top Right Of The Quad (Right)
        GL.Vertex3( 1.0f, 1.0f, 1.0f );         // Top Left Of The Quad (Right)
        GL.Vertex3( 1.0f, -1.0f, 1.0f );        // Bottom Left Of The Quad (Right)
        GL.Vertex3( 1.0f, -1.0f, -1.0f );       // Bottom Right Of The Quad (Right)

        GL.End();

        triangleCounter += 12;
      }
    }
  }
}
