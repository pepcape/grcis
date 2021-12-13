using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;
using MathSupport;
using NCalc;
using OpenglSupport;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Utilities;

namespace _113graph
{
  public class Graph
  {
    /// <summary>
    /// Form-data initialization.
    /// </summary>
    public static void InitParams (out string param, out string tooltip, out string expr, out string name, out MouseButtons trackballButton)
    {
      param = "domain=[-1.0;1.0;-1.0;1.0]";
      tooltip = "domain";
      expr = "1.0";
      trackballButton = MouseButtons.Left;

      name = "Josef Pelikán";
    }

    /// <summary>
    /// Vertex array ([color] [normal] coord), index array
    /// </summary>
    uint[] VBOid = null;

    /// <summary>
    /// Currently allocated lengths of VBOs (in bytes)
    /// </summary>
    long[] VBOlen = null;

    /// <summary>
    /// Stride for vertex-array (in bytes).
    /// </summary>
    int stride = 0;

    /// <summary>
    /// Number of vertices (indices) to draw..
    /// </summary>
    int vertices = 0;

    /// <summary>
    /// Use vertex colors for rendering.
    /// </summary>
    bool useColors = true;

    /// <summary>
    /// Use normal vectors for rendering.
    /// </summary>
    bool useNormals = false;

    /// <summary>
    /// Cached expression.
    /// </summary>
    string expression = "";

    /// <summary>
    /// Cached param string.
    /// </summary>
    string param = "";

    /// <summary>
    /// Current model's center point.
    /// </summary>
    public Vector3 center = Vector3.Zero;

    /// <summary>
    /// Current model's diameter.
    /// </summary>
    public float diameter = 4.0f;

    /// <summary>
    /// Near point for the current model.
    /// </summary>
    public float near = 0.1f;

    /// <summary>
    /// Far point for the current model.
    /// </summary>
    public float far = 20.0f;

    public void InitOpenGL (GLControl glc)
    {
      // log OpenGL info just for curiosity:
      GlInfo.LogGLProperties();

      // General OpenGL.
      glc.VSync = true;
      GL.ClearColor(Color.FromArgb(14, 20, 40));    // darker "navy blue"
      GL.Enable(EnableCap.DepthTest);
      GL.Enable(EnableCap.VertexProgramPointSize);
      GL.ShadeModel(ShadingModel.Flat);

      // VBO init.
      VBOid = new uint[2];           // one big buffer for vertex data, another buffer for tri/line indices
      GL.GenBuffers(2, VBOid);
      GlInfo.LogError("VBO init");
      VBOlen = new long[2];
    }

    public void InitSimulation (string par, string expr)
    {
      param = "";
      expression = "";
      RegenerateGraph(par, expr);
    }

    /// <summary>
    /// Recompute the graph, prepare VBO content and upload it to the GPU...
    /// </summary>
    /// <param name="param">New param string.</param>
    /// <param name="expr">New expression string.</param>
    /// <returns>null if OK, error message otherwise.</returns>
    public string RegenerateGraph (string par, string expr)
    {
      // !!!{{ TODO: add graph data regeneration code here

      if (expr == expression &&
          par == param)
        return null;                // nothing to do..

      double xMin = -1.0, xMax = 1.0, yMin = -1.0, yMax = 1.0;

      // input params:
      Dictionary<string, string> p = Util.ParseKeyValueList(param);

      // domain: [xMin;xMax;yMin;yMax]
      List<double> dom = null;
      if (Util.TryParse(p, "domain", ref dom, ';') &&
          dom != null &&
          dom.Count >= 4)
      {
        xMin = dom[0];
        xMax = Math.Max(xMin + 1.0e-6, dom[1]);
        yMin = dom[2];
        yMax = Math.Max(yMin + 1.0e-6, dom[3]);
      }

      // Expression evaluation (THIS HAS TO BE CHANGED).
      double x = 0.0, z = 1.0;
      Expression e = null;
      double result;
      try
      {
        e = new Expression(expr);
        e.Parameters["x"] = x;
        e.Parameters["y"] = z;
        e.Parameters["z"] = z;
        result = (double)e.Evaluate();
        if (double.IsNaN(result))
          throw new Exception("NCalc: NaN");
      }
      catch (Exception ex)
      {
        return ex.Message;
      }

      // Everything seems to be OK:
      expression = expr;
      param = par;
      Vector3  v = new Vector3((float)x, (float)result, (float)z);

      // Data for VBO:
      useColors = true;
      useNormals = false;
      stride = Vector3.SizeInBytes * (1 + (useColors ? 1 : 0) + (useNormals ? 1 : 0));
      long newVboSize = stride * 3;     // pilot .. three vertices
      vertices = 3;                     // pilot .. three indices
      long newIndexSize = sizeof( uint ) * vertices;

      // OpenGL stuff
      GL.EnableClientState(ArrayCap.VertexArray);
      if (useColors)
        GL.EnableClientState(ArrayCap.ColorArray);
      if (useNormals)
        GL.EnableClientState(ArrayCap.NormalArray);

      // Vertex array: [color] [normal] coordinate
      GL.BindBuffer(BufferTarget.ArrayBuffer, VBOid[0]);
      if (newVboSize != VBOlen[0])
      {
        VBOlen[0] = newVboSize;
        GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)VBOlen[0], IntPtr.Zero, BufferUsageHint.DynamicDraw);
      }

      IntPtr videoMemoryPtr = GL.MapBuffer(BufferTarget.ArrayBuffer, BufferAccess.WriteOnly);
      unsafe
      {
        float* ptr = (float*)videoMemoryPtr.ToPointer();
        // !!! TODO: you need to change this part (only one triangle is defined here) !!!
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
        *ptr++ = v.Y;
        *ptr++ = v.Z + 1.0f;
      }
      GL.UnmapBuffer(BufferTarget.ArrayBuffer);
      GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

      // Index buffer
      GL.BindBuffer(BufferTarget.ElementArrayBuffer, VBOid[1]);
      if (newIndexSize != VBOlen[1])
      {
        VBOlen[1] = newIndexSize;
        GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)VBOlen[1], IntPtr.Zero, BufferUsageHint.StaticDraw);
      }

      videoMemoryPtr = GL.MapBuffer(BufferTarget.ElementArrayBuffer, BufferAccess.WriteOnly);
      unsafe
      {
        uint* ptr = (uint*)videoMemoryPtr.ToPointer();
        for (uint i = 0; i < 3; i++)
          *ptr++ = i;
      }
      GL.UnmapBuffer(BufferTarget.ElementArrayBuffer);
      GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

      // Change the graph dimension.
      center = v;
      diameter = 2.0f;
      near = 0.1f;
      far = 20.0f;

      Form1.form.SetStatus($"Tri: {vertices / 3}");

      return null;

      // !!!}}
    }

    /// <summary>
    /// Rendering code itself (separated for clarity).
    /// </summary>
    public void RenderScene (ref long primitiveCounter)
    {
      // Scene rendering:
      if (Form1.form.drawGraph &&
          VBOlen[0] > 0L)        // buffers are nonempty => render
      {
        // [color] [normal] coordinate
        GL.BindBuffer(BufferTarget.ArrayBuffer, VBOid[0]);
        IntPtr p = IntPtr.Zero;
        if (useColors)             // are colors present?
        {
          GL.ColorPointer(3, ColorPointerType.Float, stride, p);
          p += Vector3.SizeInBytes;
        }
        if (useNormals)            // are normals present?
        {
          GL.NormalPointer(NormalPointerType.Float, stride, p);
          p += Vector3.SizeInBytes;
        }
        GL.VertexPointer(3, VertexPointerType.Float, stride, p);

        // !!!{{ Change this part if you want to add axes, legend, etc...

        // Index buffer for triangle mesh.
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, VBOid[1]);

        // Triangle part of the scene.
        GL.DrawElements(PrimitiveType.Triangles, vertices, DrawElementsType.UnsignedInt, IntPtr.Zero);

        // !!!}}

        primitiveCounter += vertices / 3;
      }
      else                           // color cube
      {
        GL.Begin(PrimitiveType.Quads);

        GL.Color3(0.0f, 1.0f, 0.0f);          // Set The Color To Green
        GL.Vertex3(1.0f, 1.0f, -1.0f);        // Top Right Of The Quad (Top)
        GL.Vertex3(-1.0f, 1.0f, -1.0f);       // Top Left Of The Quad (Top)
        GL.Vertex3(-1.0f, 1.0f, 1.0f);        // Bottom Left Of The Quad (Top)
        GL.Vertex3(1.0f, 1.0f, 1.0f);         // Bottom Right Of The Quad (Top)

        GL.Color3(1.0f, 0.5f, 0.0f);          // Set The Color To Orange
        GL.Vertex3(1.0f, -1.0f, 1.0f);        // Top Right Of The Quad (Bottom)
        GL.Vertex3(-1.0f, -1.0f, 1.0f);       // Top Left Of The Quad (Bottom)
        GL.Vertex3(-1.0f, -1.0f, -1.0f);      // Bottom Left Of The Quad (Bottom)
        GL.Vertex3(1.0f, -1.0f, -1.0f);       // Bottom Right Of The Quad (Bottom)

        GL.Color3(1.0f, 0.0f, 0.0f);          // Set The Color To Red
        GL.Vertex3(1.0f, 1.0f, 1.0f);         // Top Right Of The Quad (Front)
        GL.Vertex3(-1.0f, 1.0f, 1.0f);        // Top Left Of The Quad (Front)
        GL.Vertex3(-1.0f, -1.0f, 1.0f);       // Bottom Left Of The Quad (Front)
        GL.Vertex3(1.0f, -1.0f, 1.0f);        // Bottom Right Of The Quad (Front)

        GL.Color3(1.0f, 1.0f, 0.0f);          // Set The Color To Yellow
        GL.Vertex3(1.0f, -1.0f, -1.0f);       // Bottom Left Of The Quad (Back)
        GL.Vertex3(-1.0f, -1.0f, -1.0f);      // Bottom Right Of The Quad (Back)
        GL.Vertex3(-1.0f, 1.0f, -1.0f);       // Top Right Of The Quad (Back)
        GL.Vertex3(1.0f, 1.0f, -1.0f);        // Top Left Of The Quad (Back)

        GL.Color3(0.0f, 0.0f, 1.0f);          // Set The Color To Blue
        GL.Vertex3(-1.0f, 1.0f, 1.0f);        // Top Right Of The Quad (Left)
        GL.Vertex3(-1.0f, 1.0f, -1.0f);       // Top Left Of The Quad (Left)
        GL.Vertex3(-1.0f, -1.0f, -1.0f);      // Bottom Left Of The Quad (Left)
        GL.Vertex3(-1.0f, -1.0f, 1.0f);       // Bottom Right Of The Quad (Left)

        GL.Color3(1.0f, 0.0f, 1.0f);          // Set The Color To Violet
        GL.Vertex3(1.0f, 1.0f, -1.0f);        // Top Right Of The Quad (Right)
        GL.Vertex3(1.0f, 1.0f, 1.0f);         // Top Left Of The Quad (Right)
        GL.Vertex3(1.0f, -1.0f, 1.0f);        // Bottom Left Of The Quad (Right)
        GL.Vertex3(1.0f, -1.0f, -1.0f);       // Bottom Right Of The Quad (Right)

        GL.End();

        primitiveCounter += 12;
      }
    }

    public void Intersect (ref Vector3d p0, ref Vector3d p1, ref double nearest)
    {
      // Compute intersection of the given ray (p0, p1) with the scene,
      // act upon that (i.e. set status string, ..)
      Vector2d uv;

#if false

      Vector3 A, B, C;
      double curr = Geometry.RayTriangleIntersection(ref p0, ref p1, ref A, ref B, ref C, out uv);
      if (!double.IsInfinity(curr) &&
          curr < nearest)
        nearest = curr;

#else

      Vector3d ul   = new Vector3d(-1.0, -1.0, -1.0);
      Vector3d size = new Vector3d( 2.0,  2.0,  2.0);
      if (Geometry.RayBoxIntersection(ref p0, ref p1, ref ul, ref size, out uv))
      {
        nearest = uv.X;
        Form1.form.SetStatus(string.Format(CultureInfo.InvariantCulture, "[{0:f2},{1:f2},{2:f2}]",
                                           p0.X + nearest * p1.X,
                                           p0.Y + nearest * p1.Y,
                                           p0.Z + nearest * p1.Z));
      }

#endif
    }

    /// <summary>
    /// Handles mouse-button push.
    /// </summary>
    /// <returns>True if handled.</returns>
    public bool MouseButtonDown (MouseEventArgs e)
    {
      return false;
    }

    /// <summary>
    /// Handles mouse-button release.
    /// </summary>
    /// <returns>True if handled.</returns>
    public bool MouseButtonUp (MouseEventArgs e)
    {
      return false;
    }

    /// <summary>
    /// Handles mouse move.
    /// </summary>
    /// <returns>True if handled.</returns>
    public bool MousePointerMove (MouseEventArgs e)
    {
      return false;
    }

    /// <summary>
    /// Handles keyboard key press.
    /// </summary>
    /// <returns>True if handled.</returns>
    public bool KeyHandle (KeyEventArgs e)
    {
      return false;
    }

    public void Destroy ()
    {
      if (VBOid != null &&
           VBOid[0] != 0)
      {
        GL.DeleteBuffers(2, VBOid);
        VBOid = null;
      }
    }
  }
}
