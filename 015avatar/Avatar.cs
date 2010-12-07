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
    private bool animate = true;
    double timeInSeconds = 0.0;

    private void Application_Idle ( object sender, EventArgs e )
    {
      while ( glControl1.IsIdle )
      {
        glControl1.Invalidate();
        Thread.Sleep( 5 );

        long now = DateTime.Now.Ticks;
        if ( now - lastFpsTime > 10000000 )
        {
          double fps = fpsCounter * 1.0e7 / (now - lastFpsTime);
          lastFpsTime = now;
          fpsCounter = 0;
          labelFps.Text = String.Format( "FPS: {0:0.0}", fps );
        }
      }
    }

    private void Render ()
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
      GL.Vertex3(  30.0f, -25.0f, 0.0f );
      GL.Vertex3(   0.0f,  40.0f, 0.0f );
      GL.End();

      glControl1.SwapBuffers();
    }

    private void SetupViewport ()
    {
      int wid = glControl1.Width;
      int hei = glControl1.Height;

      GL.MatrixMode( MatrixMode.Projection );
      GL.LoadIdentity();
      GL.Ortho( 0, wid, 0, hei, -1, 1 );
      GL.Viewport( 0, 0, wid, hei );
    }

    private void glControl1_MouseDown ( object sender, MouseEventArgs e )
    {
      // !!!{{ TODO: add the event handler here
      // !!!}}
    }

    private void glControl1_MouseUp ( object sender, MouseEventArgs e )
    {
      // !!!{{ TODO: add the event handler here

      animate = !animate;

      // !!!}}
    }

    private void glControl1_MouseMove ( object sender, MouseEventArgs e )
    {
      // !!!{{ TODO: add the event handler here
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

  }

}
