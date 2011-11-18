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
    /// <summary>
    /// Rendering code itself (separated for clarity).
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

        // Multiple instancing of the scene:
        Vector3 center;
        float delta = scene.GetDiameter( out center ) * 0.8f;
        int n = (int)numericInstances.Value;
        for ( int k = 0; k++ < n; )
        {
          GL.PushMatrix();
          for ( int j = 0; j++ < n; )
          {
            GL.PushMatrix();
            for ( int i = 0; i++ < n; )
            {
              triangleCounter += scene.Triangles;
              GL.DrawElements( BeginMode.Triangles, scene.Triangles * 3, DrawElementsType.UnsignedInt, IntPtr.Zero );
              GL.Translate( delta, 0.0f, 0.0f );
            }
            GL.PopMatrix();
            GL.Translate( 0.0f, 0.0f, delta );
          }
          GL.PopMatrix();
          GL.Translate( 0.0f, delta, 0.0f );
        }
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
