using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
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
      param           = "domain=[-1.0;1.0;-1.0;1.0]";
      tooltip         = "domain=[x_min,x_max,y_min,y_max]";
      expr            = "1.0";
      trackballButton = MouseButtons.Left;

      name            = "Josef Pelikán";
    }

    /// <summary>
    /// Cached expression.
    /// </summary>
    string expression = "";

    /// <summary>
    /// Cached param string.
    /// </summary>
    string param = "";

    //====================================================================
    // Scene data
    //====================================================================

    /// <summary>
    /// Current model's center point.
    /// </summary>
    public Vector3 center = Vector3.Zero;

    /// <summary>
    /// Current model's diameter.
    /// </summary>
    public float diameter = 4.0f;

    /// <summary>
    /// Near distance for the current model (must be positive).
    /// </summary>
    public float near = 0.1f;

    /// <summary>
    /// Far distance for the current model.
    /// </summary>
    public float far = 20.0f;

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
    /// Current texture.
    /// </summary>
    int texName = 0;

    /// <summary>
    /// Number of vertices (indices) to draw.. (default - triangles)
    /// </summary>
    int vertices = 0;

    /// <summary>
    /// Use texture coordinate in vertices.
    /// </summary>
    bool useTexture = false;

    /// <summary>
    /// Use vertex colors for rendering.
    /// </summary>
    bool useColors = true;

    /// <summary>
    /// Use normal vectors for rendering.
    /// </summary>
    bool useNormals = false;

    /// <summary>
    /// Phong shading interpolation.
    /// </summary>
    bool shadingPhong = true;

    /// <summary>
    /// Gouraud shading interpolation.
    /// </summary>
    bool shadingGouraud = true;

    /// <summary>
    /// Use ambient (Ka) shading term.
    /// </summary>
    bool phongAmbient = true;

    /// <summary>
    /// Use diffuse (Kd) shading term.
    /// </summary>
    bool phongDiffuse = true;

    /// <summary>
    /// Use specular (Ks) shading term.
    /// </summary>
    bool phongSpecular = true;

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

      //------------------------------------------------------------------
      // Input text params (form).

      Dictionary<string, string> p = Util.ParseKeyValueList(param);

      // System (rendering) params.
      Form1.form.UpdateRenderingParams(p);

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

      //------------------------------------------------------------------
      // Expression evaluation  - THIS HAS TO BE CHANGED!

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

      // Everything seems to be OK.
      expression = expr;
      param = par;
      Vector3  v = new Vector3((float)x, (float)result, (float)z);

      //------------------------------------------------------------------
      // Data for VBO.

      // This has to be in sync with the actual buffer filling loop (see the unsafe
      // block below)!
      useTexture = false;
      useColors  = true;
      useNormals = false;

      stride = Vector3.SizeInBytes;
      if (useTexture)
        stride += Vector2.SizeInBytes;
      if (useColors)
        stride += Vector3.SizeInBytes;
      if (useNormals)
        stride += Vector3.SizeInBytes;

      long newVboSize = stride * 3;     // pilot .. three vertices
      vertices = 3;                     // pilot .. three indices
      long newIndexSize = sizeof(uint) * vertices;

      // Vertex array: [texture:2D] [color:3D] [normal:3D] coordinate:3D
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

        // !!! TODO: you definitely need to change this part (only one triangle is defined here)!
        float r = 0.1f;
        float g = 0.9f;
        float b = 0.5f;

        // [s t] [R G B] [N_x N_y N_z] x y z

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
        // !!! TODO: only one triangle is defined here, you have to change it!

        uint* ptr = (uint*)videoMemoryPtr.ToPointer();
        for (uint i = 0; i < 3; i++)
          *ptr++ = i;
      }
      GL.UnmapBuffer(BufferTarget.ElementArrayBuffer);
      GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

      // !!! TODO: store any information about extension data here
      // !!! Example: M x N x 2 triangles, then 3 * K segments used for the axes...
      // !!! Rendering code 'RenderScene()' must reflect that!

      // Change the graph dimension.
      center   = v;
      diameter =  2.0f;
      near     =  0.1f;
      far      = 20.0f;

      Form1.form.SetStatus($"Tri: {vertices / 3}");

      return null;

      // !!!}}
    }

    //====================================================================
    // Rendering - OpenGL
    //====================================================================

    /// <summary>
    /// OpenGL init code (cold init).
    /// </summary>
    public void InitOpenGL (GLControl glc)
    {
      // Log OpenGL info just for curiosity.
      GlInfo.LogGLProperties();

      // General OpenGL.
      glc.VSync = true;
      GL.ClearColor(Color.FromArgb(14, 20, 40));    // darker "navy blue"
      GL.Enable(EnableCap.DepthTest);
      GL.Enable(EnableCap.VertexProgramPointSize);
      GL.ShadeModel(ShadingModel.Flat);

      // VBO init.
      VBOid = new uint[2];            // one big buffer for vertex data, another buffer for tri/line indices
      GL.GenBuffers(2, VBOid);
      if (GlInfo.LogError("VBO init"))
        Application.Exit();

      VBOlen = new long[2];           // current buffer lenghts in bytes

      // Shaders.
      InitShaderRepository();
      if (!SetupShaders())
      {
        Util.Log("Shader setup failed, giving up...");
        Application.Exit();
      }

      // Texture.
      texName = GenerateTexture();
    }

    /// <summary>
    /// Generate static procedural texture.
    /// </summary>
    /// <returns>Texture handle or 0 if no texture will be used.</returns>
    int GenerateTexture ()
    {
      // !!! TODO: change this part if you want, return 0 if texture is not used.

      // Generated (procedural) texture.
      const int TEX_SIZE = 128;
      const int TEX_CHECKER_SIZE = 8;
      Vector3 colWhite = new Vector3(0.85f, 0.75f, 0.30f);
      Vector3 colBlack = new Vector3(0.15f, 0.15f, 0.60f);
      Vector3 colShade = new Vector3(0.15f, 0.15f, 0.15f);

      GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);
      int texName = GL.GenTexture();
      GL.BindTexture(TextureTarget.Texture2D, texName);

      Vector3[] data = new Vector3[TEX_SIZE * TEX_SIZE];
      for (int y = 0; y < TEX_SIZE; y++)
        for (int x = 0; x < TEX_SIZE; x++)
        {
          int i = y * TEX_SIZE + x;
          bool odd = ((x / TEX_CHECKER_SIZE + y / TEX_CHECKER_SIZE) & 1) > 0;
          data[i] = odd ? colBlack : colWhite;

          // Add some fancy shading on the edges.
          if ((x % TEX_CHECKER_SIZE) == 0 || (y % TEX_CHECKER_SIZE) == 0)
            data[i] += colShade;
          if (((x + 1) % TEX_CHECKER_SIZE) == 0 || ((y + 1) % TEX_CHECKER_SIZE) == 0)
            data[i] -= colShade;

          // Add top-half texture markers.
          if (y < TEX_SIZE / 2)
          {
            if (x % TEX_CHECKER_SIZE == TEX_CHECKER_SIZE / 2 &&
                y % TEX_CHECKER_SIZE == TEX_CHECKER_SIZE / 2)
              data[i] -= colShade;
          }
        }

      GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, TEX_SIZE, TEX_SIZE, 0, PixelFormat.Rgb, PixelType.Float, data);

      GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
      GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
      GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
      GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMagFilter.Linear);

      GlInfo.LogError("create-texture");

      return texName;
    }

    /// <summary>
    /// De-allocated all the data associated with the given texture object.
    /// </summary>
    /// <param name="texName"></param>
    void DestroyTexture (ref int texName)
    {
      int tHandle = texName;
      texName = 0;
      if (tHandle != 0)
        GL.DeleteTexture(tHandle);
    }

    /// <summary>
    /// Application exit.
    /// </summary>
    public void Destroy ()
    {
      // Texture.
      DestroyTexture(ref texName);

      // Buffers.
      if (VBOid != null &&
          VBOid[0] != 0)
      {
        GL.DeleteBuffers(2, VBOid);
        VBOid = null;
      }

      // Shaders.
      DestroyShaders();
    }

    //====================================================================
    // Rendering - shaders
    //====================================================================

    /// <summary>
    /// Global GLSL program repository.
    /// </summary>
    Dictionary<string, GlProgramInfo> programs = new Dictionary<string, GlProgramInfo>();

    /// <summary>
    /// Current (active) GLSL program.
    /// </summary>
    GlProgram activeProgram = null;

    /// <summary>
    /// Fill the shader-repository.
    /// </summary>
    void InitShaderRepository ()
    {
      programs.Clear();
      GlProgramInfo pi;

      // Default program.
      pi = new GlProgramInfo("default", new GlShaderInfo[]
      {
        new GlShaderInfo(ShaderType.VertexShader,   "shaderVertex.glsl",   "113graph"),
        new GlShaderInfo(ShaderType.FragmentShader, "shaderFragment.glsl", "113graph")
      });
      programs[pi.name] = pi;

      // put more programs here:
      // pi = new GlProgramInfo( ..
      //   ..
      // programs[ pi.name ] = pi;
    }

    /// <summary>
    /// Init shaders registered in global repository 'programs'.
    /// </summary>
    /// <returns>True if succeeded.</returns>
    bool SetupShaders ()
    {
      activeProgram = null;

      foreach (var programInfo in programs.Values)
        if (programInfo.Setup())
          activeProgram = programInfo.program;

      if (activeProgram == null)
        return false;

      GlProgramInfo defInfo;
      if (programs.TryGetValue("default", out defInfo) &&
          defInfo.program != null)
        activeProgram = defInfo.program;

      return true;
    }

    void DestroyShaders ()
    {
      foreach (var prg in programs.Values)
        prg.Destroy();
    }

    //====================================================================
    // Rendering - graph
    //====================================================================

    /// <summary>
    /// Rendering code itself (separated for clarity).
    /// </summary>
    /// <param name="cam">Camera parameters.</param>
    /// <param name="a">Current appearance parameters.</param>
    /// <param name="primitiveCounter">Number of GL primitives rendered.</param>
    public void RenderScene (
      IDynamicCamera cam,
      Appearance a,
      ref long primitiveCounter)
    {
      // Only shader VertexAttribPointers() or the very old glVertex() stuff.
      GL.DisableClientState(ArrayCap.VertexArray);
      GL.DisableClientState(ArrayCap.TextureCoordArray);
      GL.DisableClientState(ArrayCap.NormalArray);
      GL.DisableClientState(ArrayCap.ColorArray);

      // Scene rendering.
      if (Form1.form.drawGraph &&
          VBOlen[0] > 0L &&
          activeProgram != null)    // buffers are nonempty & shaders are ready => render
      {
        // Vertex buffer: [texture] [color] [normal] coordinate
        GL.BindBuffer(BufferTarget.ArrayBuffer, VBOid[0]);

        // GLSL shaders.
        activeProgram.EnableVertexAttribArrays();

        GL.UseProgram(activeProgram.Id);

        // Uniforms.
        Matrix4 modelView  = cam.ModelView;
        Matrix4 projection = cam.Projection;
        Vector3 eye        = cam.Eye;
        GL.UniformMatrix4(activeProgram.GetUniform("matrixModelView"),  false, ref modelView);
        GL.UniformMatrix4(activeProgram.GetUniform("matrixProjection"), false, ref projection);

        GL.Uniform3(activeProgram.GetUniform("globalAmbient"), ref a.globalAmbient);
        GL.Uniform3(activeProgram.GetUniform("lightColor"), ref a.whiteLight);
        GL.Uniform3(activeProgram.GetUniform("lightPosition"), ref a.lightPosition);
        GL.Uniform3(activeProgram.GetUniform("eyePosition"), ref eye);
        GL.Uniform3(activeProgram.GetUniform("Ka"), ref a.matAmbient);
        GL.Uniform3(activeProgram.GetUniform("Kd"), ref a.matDiffuse);
        GL.Uniform3(activeProgram.GetUniform("Ks"), ref a.matSpecular);
        GL.Uniform1(activeProgram.GetUniform("shininess"), a.matShininess);

        // Color handling.
        bool useGlobalColor = a.useGlobalColor;
        if (!useColors)
          useGlobalColor = true;
        GL.Uniform1(activeProgram.GetUniform("globalColor"), useGlobalColor ? 1 : 0);

        // Shading.
        if (!shadingGouraud)
          shadingPhong = false;
        GL.Uniform1(activeProgram.GetUniform("shadingPhong"), shadingPhong ? 1 : 0);
        GL.Uniform1(activeProgram.GetUniform("shadingGouraud"), shadingGouraud ? 1 : 0);
        GL.Uniform1(activeProgram.GetUniform("useAmbient"), phongAmbient ? 1 : 0);
        GL.Uniform1(activeProgram.GetUniform("useDiffuse"), phongDiffuse ? 1 : 0);
        GL.Uniform1(activeProgram.GetUniform("useSpecular"), phongSpecular ? 1 : 0);
        GlInfo.LogError("set-uniforms");

        // Texture handling.
        GL.Uniform1(activeProgram.GetUniform("useTexture"), useTexture ? 1 : 0);
        GL.Uniform1(activeProgram.GetUniform("texSurface"), 0);
        if (useTexture)
        {
          GL.ActiveTexture(TextureUnit.Texture0);
          GL.BindTexture(TextureTarget.Texture2D, texName);
        }
        GlInfo.LogError("set-texture");

        // Vertex attributes.
        IntPtr p = IntPtr.Zero;

        if (activeProgram.HasAttribute("texCoords"))
          GL.VertexAttribPointer(activeProgram.GetAttribute("texCoords"), 2, VertexAttribPointerType.Float, false, stride, p);
        if (useTexture)
          p += Vector2.SizeInBytes;

        if (activeProgram.HasAttribute("color"))
          GL.VertexAttribPointer(activeProgram.GetAttribute("color"), 3, VertexAttribPointerType.Float, false, stride, p);
        if (useColors)
          p += Vector3.SizeInBytes;

        if (activeProgram.HasAttribute("normal"))
          GL.VertexAttribPointer(activeProgram.GetAttribute("normal"), 3, VertexAttribPointerType.Float, false, stride, p);
        if (useNormals)
          p += Vector3.SizeInBytes;

        GL.VertexAttribPointer(activeProgram.GetAttribute("position"), 3, VertexAttribPointerType.Float, false, stride, p);
        GlInfo.LogError("set-attrib-pointers");

        // Index buffer.
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, VBOid[1]);

        // Engage!

        // !!!{{ CHANGE THIS PART if you want to add axes, legend, etc...

        // Triangle part of the scene.
        GL.DrawElements(PrimitiveType.Triangles, vertices, DrawElementsType.UnsignedInt, IntPtr.Zero);
        GlInfo.LogError("draw-elements-shader");

        primitiveCounter += vertices / 3;

        // !!!}}

        // Cleanup.
        GL.UseProgram(0);
        if (useTexture)
          GL.BindTexture(TextureTarget.Texture2D, 0);
      }
      else
      {
        // Color cube in very old OpenGL style!
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

    //====================================================================
    // Interaction
    //====================================================================

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
  }
}
