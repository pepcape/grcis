using System;
using System.Drawing;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;
using NCalc;
using OpenglSupport;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace _113graph
{
  public class Graph
  {
    /// <summary>
    /// Form-data initialization.
    /// </summary>
    public static void InitParams ( out string param, out string tooltip, out string expr, out string name, out MouseButtons trackballButton )
    {
      param = "domain=[-1.0;1.0;-1.0;1.0]";
      tooltip = "domain";
      expr = "1.0";
      trackballButton = MouseButtons.Left;

      name = "josef Pelikán";
    }

    /// <summary>
    /// Vertex array (colors, normals, coords), index array
    /// </summary>
    uint[] VBOid = null;

    /// <summary>
    /// Currently allocated lengths of VBOs
    /// </summary>
    int[] VBOlen = null;

    /// <summary>
    /// Stride for vertex-array.
    /// </summary>
    int stride = 0;

    /// <summary>
    /// Number of vertices to draw..
    /// </summary>
    int vertices = 0;

    // !!!{{ TODO: add more graph data representation here..

    /// <summary>
    /// Cached expression.
    /// </summary>
    string expression = "";

    /// <summary>
    /// Cached param string.
    /// </summary>
    string param = "";

    // !!!}}

    public void InitOpenGL ( GLControl glc )
    {
      // log OpenGL info just for curiosity:
      GlInfo.LogGLProperties();

      // general OpenGL:
      glc.VSync = true;
      GL.ClearColor( Color.FromArgb( 14, 20, 40 ) );    // darker "navy blue"
      GL.Enable( EnableCap.DepthTest );
      GL.Enable( EnableCap.VertexProgramPointSize );
      GL.ShadeModel( ShadingModel.Flat );

      // VBO init:
      VBOid = new uint[ 2 ];           // one big buffer for vertex data, another buffer for tri/line indices
      GL.GenBuffers( 2, VBOid );
      GlInfo.LogError( "VBO init" );
    }

    /// <summary>
    /// Recompute the graph, prepare VBO content and upload it to the GPU...
    /// </summary>
    /// <param name="expr">New expression string.</param>
    /// <param name="par">New param string.</param>
    /// <returns>null if OK, error message otherwise.</returns>
    string RegenerateGraph ( string expr, string par )
    {
      // !!!{{ TODO: add graph data regeneration code here

      if ( expr == expression &&
           par == param )
        return null;                // nothing to do..

      // Parse 'intervals'
      double xMin = -1.0, xMax = 1.0, yMin = -1.0, yMax = 1.0;
      string[] limits = interv.Split( ' ' );
      if ( limits.Length > 0 )
      {
        double.TryParse( limits[ 0 ], NumberStyles.Float, CultureInfo.InvariantCulture, out xMin );
        if ( limits.Length > 1 )
        {
          double.TryParse( limits[ 1 ], NumberStyles.Float, CultureInfo.InvariantCulture, out xMax );
          if ( limits.Length > 2 )
          {
            double.TryParse( limits[ 2 ], NumberStyles.Float, CultureInfo.InvariantCulture, out yMin );
            if ( limits.Length > 3 )
              double.TryParse( limits[ 3 ], NumberStyles.Float, CultureInfo.InvariantCulture, out yMax );
          }
        }
      }

      // Expression evaluation (THIS HAS TO BE CHANGED):
      double x = 0.0, z = 1.0;
      Expression e = null;
      double result;
      try
      {
        e = new Expression( expr );
        e.Parameters[ "x" ] = x;
        e.Parameters[ "y" ] = z;
        result = (double)e.Evaluate();
      }
      catch ( Exception ex )
      {
        return ex.Message;
      }

      // Everything seems to be OK:
      expression = expr;
      param = par;
      Vector3 v = new Vector3( (float)x, (float)result, (float)z );

      // Data for VBO:
      stride = Vector3.SizeInBytes * 2;
      long newVboSize = stride * 3;   // pilot => one triangle
      long newIndexSize = sizeof( uint ) * 3;
      vertices = 3;

      // OpenGL stuff
      GL.EnableClientState( ArrayCap.VertexArray );
      // GL.EnableClientState( ArrayCap.NormalArray );   // uncomment this if necessary
      GL.EnableClientState( ArrayCap.ColorArray );

      // Vertex array: color [normal] coord
      GL.BindBuffer( BufferTarget.ArrayBuffer, VBOid[ 0 ] );
      if ( newVboSize != vboSize )
      {
        vboSize = newVboSize;
        GL.BufferData( BufferTarget.ArrayBuffer, (IntPtr)vboSize, IntPtr.Zero, BufferUsageHint.DynamicDraw );
      }
      IntPtr videoMemoryPtr = GL.MapBuffer( BufferTarget.ArrayBuffer, BufferAccess.WriteOnly );
      unsafe
      {
        float* ptr = (float*)videoMemoryPtr.ToPointer();
        float r = 0.1f;
        float g = 0.9f;
        float b = 0.5f;
        *ptr++ = r;
        *ptr++ = g;
        *ptr++ = b;
        *ptr++ = v.X;
        *ptr++ = v.Y;
        *ptr++ = v.Z;
        *ptr++ = r;
        *ptr++ = g;
        *ptr++ = b;
        *ptr++ = v.X + 1.0f;
        *ptr++ = v.Y;
        *ptr++ = v.Z;
        *ptr++ = r;
        *ptr++ = g;
        *ptr++ = b;
        *ptr++ = v.X;
        *ptr++ = v.Y + 1.0f;
        *ptr++ = v.Z;
      }
      GL.UnmapBuffer( BufferTarget.ArrayBuffer );
      GL.BindBuffer( BufferTarget.ArrayBuffer, 0 );

      // Index buffer
      GL.BindBuffer( BufferTarget.ElementArrayBuffer, VBOid[ 1 ] );
      if ( newIndexSize != indexSize )
      {
        indexSize = newIndexSize;
        GL.BufferData( BufferTarget.ElementArrayBuffer, (IntPtr)indexSize, IntPtr.Zero, BufferUsageHint.StaticDraw );
      }
      videoMemoryPtr = GL.MapBuffer( BufferTarget.ElementArrayBuffer, BufferAccess.WriteOnly );
      unsafe
      {
        uint* ptr = (uint*)videoMemoryPtr.ToPointer();
        for ( uint i = 0; i < 3; i++ )
          *ptr++ = i;
      }
      GL.UnmapBuffer( BufferTarget.ElementArrayBuffer );
      GL.BindBuffer( BufferTarget.ElementArrayBuffer, 0 );

      return null;
    }

    /// <summary>
    /// Rendering code itself (separated for clarity).
    /// </summary>
    private void RenderScene ()
    {
      // Scene rendering:
      if ( indexSize > 0 )        // buffers are nonempty => render
      {
        // colors [normals] vertices
        GL.BindBuffer( BufferTarget.ArrayBuffer, VBOid[ 0 ] );
        IntPtr p = IntPtr.Zero;
        if ( true )               // colors are always here..
        {
          GL.ColorPointer( 3, ColorPointerType.Float, stride, p );
          p += Vector3.SizeInBytes;
        }
        if ( false )              // no normals yet
        {
          GL.NormalPointer( NormalPointerType.Float, stride, p );
          p += Vector3.SizeInBytes;
        }
        GL.VertexPointer( 3, VertexPointerType.Float, stride, p );

        // index buffer
        GL.BindBuffer( BufferTarget.ElementArrayBuffer, VBOid[ 1 ] );

        // Multiple instancing of the scene:
        GL.DrawElements( PrimitiveType.Triangles, vertices, DrawElementsType.UnsignedInt, IntPtr.Zero );

        triangleCounter += 1;
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
