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
    /// <summary>
    /// Renders a 3D scene, rendering code separated for clarity.
    /// </summary>
    private void RenderScene ()
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
