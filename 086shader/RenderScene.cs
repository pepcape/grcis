//#define USE_INVALIDATE

using System;
using System.Globalization;
using System.Threading;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace _086shader
{
  public partial class Form1
  {
    /// <summary>
    /// Function called whenever the main application is idle..
    /// </summary>
    private void Application_Idle ( object sender, EventArgs e )
    {
      while ( glControl1.IsIdle )
      {
#if USE_INVALIDATE
        glControl1.Invalidate();
#else
        glControl1.MakeCurrent();
        Render();
#endif

        long now = DateTime.Now.Ticks;
        if ( now - lastFpsTime > 5000000 )      // more than 0.5 sec
        {
          lastFps = 0.5 * lastFps + 0.5 * (frameCounter * 1.0e7 / (now - lastFpsTime));
          lastTps = 0.5 * lastTps + 0.5 * (triangleCounter * 1.0e7 / (now - lastFpsTime));
          lastFpsTime = now;
          frameCounter = 0;
          triangleCounter = 0L;

          if ( lastTps < 5.0e5 )
            labelFps.Text = String.Format( CultureInfo.InvariantCulture, "Fps: {0:f1}, Tps: {1:f0}k",
                                           lastFps, (lastTps * 1.0e-3) );
          else
            labelFps.Text = String.Format( CultureInfo.InvariantCulture, "Fps: {0:f1}, Tps: {1:f1}m",
                                           lastFps, (lastTps * 1.0e-6) );
        }
      }
    }

    /// <summary>
    /// Rendering code itself (separated for clarity).
    /// </summary>
    private void RenderScene ()
    {
      // Scene rendering:
      if ( scene != null &&
           scene.Triangles > 0 &&        // scene is nonempty => render it
           useVBO )
      {
        // [txt] [colors] [normals] vertices
        GL.BindBuffer( BufferTarget.ArrayBuffer, VBOid[ 0 ] );
        IntPtr p = IntPtr.Zero;

        if ( useShaders &&
             activeProgram != null )
        {
          if ( scene.HasTxtCoords() )
          {
            if ( activeProgram.HasAttribute( "texCoords" ) )
              GL.VertexAttribPointer( activeProgram.GetAttribute( "texCoords" ), 2, VertexAttribPointerType.Float, false, stride, p );
            p += Vector2.SizeInBytes;
          }

          if ( scene.HasColors() )
          {
            if ( activeProgram.HasAttribute( "color" ) )
              GL.VertexAttribPointer( activeProgram.GetAttribute( "color" ), 3, VertexAttribPointerType.Float, false, stride, p );
            p += Vector3.SizeInBytes;
          }

          if ( scene.HasNormals() )
          {
            if ( activeProgram.HasAttribute( "normal" ) )
              GL.VertexAttribPointer( activeProgram.GetAttribute( "normal" ), 3, VertexAttribPointerType.Float, false, stride, p );
            p += Vector3.SizeInBytes;
          }

          GL.VertexAttribPointer( activeProgram.GetAttribute( "position" ), 3, VertexAttribPointerType.Float, false, stride, p );

          // index buffer
          GL.BindBuffer( BufferTarget.ElementArrayBuffer, VBOid[ 1 ] );

          // engage!
          GL.UseProgram( activeProgram.Id );
          GL.DrawElements( PrimitiveType.Triangles, scene.Triangles * 3, DrawElementsType.UnsignedInt, IntPtr.Zero );
          GL.UseProgram( 0 );
        }
        else
        {
          // FFP:
          if ( scene.HasTxtCoords() )
          {
            p += Vector2.SizeInBytes;
          }

          if ( scene.HasColors() )
          {
            GL.ColorPointer( 3, ColorPointerType.Float, stride, p );
            p += Vector3.SizeInBytes;
          }

          if ( scene.HasNormals() )
          {
            GL.NormalPointer( NormalPointerType.Float, stride, p );
            p += Vector3.SizeInBytes;
          }

          GL.VertexPointer( 3, VertexPointerType.Float, stride, p );

          // index buffer
          GL.BindBuffer( BufferTarget.ElementArrayBuffer, VBOid[ 1 ] );

          // engage!
          GL.DrawElements( PrimitiveType.Triangles, scene.Triangles * 3, DrawElementsType.UnsignedInt, IntPtr.Zero );
        }

        triangleCounter += scene.Triangles;
      }
      else                              // color cube (JB)
      {
        GL.Begin( PrimitiveType.Quads );

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
