// Author: Josef Pelikan

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using OpenglSupport;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace _087fireworks
{
  /// <summary>
  /// Rocket/particle launcher.
  /// Primary purpose: generate rockets/particles.
  /// Can be rendered using triangles (DataSize(), FillData()).
  /// </summary>
  public class Launcher
  {
    public Vector3 position;

    public Vector3 aim;

    public float frequency;

    public float simTime;

    public void Simulate ( float time, Fireworks fw )
    {
      if ( time <= simTime )
        return;

      // generate new particles for the [simTime-time] interval:
      // ...

      simTime = time;
    }

    public Launcher ()
    {
      position = Vector3.Zero;
      aim = new Vector3( 0.1f, 1.0f, -0.05f );
      frequency = 10.0f;
      simTime = 0.0f;
    }

    /// <summary>
    /// How many triangles are used for rendering this launcher?
    /// </summary>
    public int Triangles
    {
      get
      {
        return 0;
      }
    }

    /// <summary>
    /// Rendering-data size for one instance.
    /// </summary>
    /// <param name="size">Vertex size (stride).</param>
    /// <param name="col">Use color attribute?</param>
    /// <param name="txt">Use txtCoord attribute?</param>
    /// <param name="normal">Use normal vector attribute?</param>
    /// <param name="ptsize">Use point-size attribute?</param>
    /// <returns>Data size for the whole launcher.</returns>
    public int DataSize ( out int size, bool txt, bool col, bool normal, bool ptsize )
    {
      size = 3 * sizeof( float );
      if ( txt )
        size += 2 * sizeof( float );
      if ( col )
        size += 3 * sizeof( float );
      if ( normal )
        size += 3 * sizeof( float );

      return Triangles * size;
    }

    /// <summary>
    /// Fills rendering-data for this particle.
    /// </summary>
    /// <param name="ptr">Data pointer.</param>
    /// <param name="col">Use color attribute?</param>
    /// <param name="txt">Use txtCoord attribute?</param>
    /// <param name="normal">Use normal vector attribute?</param>
    /// <param name="ptsize">Use point-size attribute?</param>
    public unsafe void FillData ( ref float* ptr, bool txt, bool col, bool normal, bool ptsize )
    {
      for ( int i = 0; i < Triangles; i++ )
      {
        if ( txt )
        {
          *ptr++ = position.X;
          *ptr++ = position.Y;
        }
        if ( col )
        {
          *ptr++ = 1.0f;
          *ptr++ = 1.0f;
          *ptr++ = 0.2f;
        }
        if ( normal )
        {
          *ptr++ = position.X;
          *ptr++ = position.Y;
          *ptr++ = position.Z;
        }

        *ptr++ = position.X;
        *ptr++ = position.Y;
        *ptr++ = position.Z;
      }
    }
  }

  public class Particle
  {
    public Vector3 position;

    public Vector3 velocity;

    public Vector3 color;

    public float simTime;

    public void Simulate ( float time, Fireworks fw )
    {
      if ( time <= simTime )
        return;

      // fly the particle:
      float dt = time - simTime;
      position += dt * velocity;

      simTime = time;
    }

    /// <summary>
    /// Rendering-data size for one instance.
    /// </summary>
    /// <param name="size">Vertex size (stride).</param>
    /// <param name="col">Use color attribute?</param>
    /// <param name="txt">Use txtCoord attribute?</param>
    /// <param name="normal">Use normal vector attribute?</param>
    /// <param name="ptsize">Use point-size attribute?</param>
    /// <returns>Data size for one point (here equal to stride).</returns>
    public int DataSize ( out int size, bool txt, bool col, bool normal, bool ptsize )
    {
      size = 3 * sizeof( float );
      if ( txt )
        size += 2 * sizeof( float );
      if ( col )
        size += 3 * sizeof( float );
      if ( normal )
        size += 3 * sizeof( float );
      if ( ptsize )
        size += sizeof( float );

      return size;
    }

    /// <summary>
    /// Fills rendering-data for this particle.
    /// </summary>
    /// <param name="ptr">Data pointer.</param>
    /// <param name="col">Use color attribute?</param>
    /// <param name="txt">Use txtCoord attribute?</param>
    /// <param name="normal">Use normal vector attribute?</param>
    /// <param name="ptsize">Use point-size attribute?</param>
    public unsafe void FillData ( ref float* ptr, bool txt, bool col, bool normal, bool ptsize )
    {
      if ( txt )
      {
        *ptr++ = position.X;
        *ptr++ = position.Y;
      }
      if ( col )
      {
        *ptr++ = color.X;
        *ptr++ = color.Y;
        *ptr++ = color.Z;
      }
      if ( normal )
      {
        *ptr++ = position.X;
        *ptr++ = position.Y;
        *ptr++ = position.Z;
      }
      if ( ptsize )
      {
        *ptr++ = 2.0f;
      }

      *ptr++ = position.X;
      *ptr++ = position.Y;
      *ptr++ = position.Z;
    }
  }

  public class Fireworks
  {
    #region Form initialization

    /// <summary>
    /// Optional data initialization.
    /// </summary>
    public static void InitParams ( out string param, out Vector3 center, out float diameter )
    {
      param = "";
      center = Vector3.Zero;
      diameter = 4.0f;
    }

    #endregion

    List<Particle> particles;

    List<Launcher> launchers;

    int maxParticles;

    /// <summary>
    /// This limit is used for render-buffer allocation.
    /// </summary>
    public int MaxParticles
    {
      get
      {
        return maxParticles;
      }
    }

    public Fireworks ( int maxPart =1000 )
    {
      maxParticles = maxPart;
      particles = new List<Particle>( maxParticles );
      launchers = new List<Launcher>();
    }

    public void InitFireworks ()
    {
      particles.Clear();
      launchers.Clear();

      Launcher la = new Launcher();
      launchers.Add( la );
    }

  }

  public partial class Form1
  {
    /// <summary>
    /// Can we use shaders?
    /// </summary>
    bool canShaders = false;

    /// <summary>
    /// Are we currently using shaders?
    /// </summary>
    bool useShaders = false;

    uint[] VBOid = null;         // vertex array (colors, normals, coords), index array
    int triangleStride = 0;      // stride for vertex array (triangle part)
    int pointStride = 0;         // stride for vertex array (point part)
    int triangleVertices = 0;    // triangle vertices come first
    int pointVertices = 0;       // then there are point vertices

    /// <summary>
    /// Actual simulated fireworks.
    /// </summary>
    Fireworks fw;

    bool useColors = true;
    bool useTexture = false;
    bool useNormals = false;
    bool usePointSize = true;

    /// <summary>
    /// Global GLSL program repository.
    /// </summary>
    Dictionary<string, GlProgramInfo> programs = new Dictionary<string, GlProgramInfo>();

    /// <summary>
    /// Current (active) GLSL program.
    /// </summary>
    GlProgram activeProgram = null;

    long lastFpsTime = 0L;
    int frameCounter = 0;
    long triangleCounter = 0L;
    double lastFps = 0.0;
    double lastTps = 0.0;

    /// <summary>
    /// Function called whenever the main application is idle..
    /// </summary>
    void Application_Idle ( object sender, EventArgs e )
    {
      while ( glControl1.IsIdle )
      {
        glControl1.MakeCurrent();
        Simulate();
        Render();

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
    /// OpenGL init code.
    /// </summary>
    void InitOpenGL ()
    {
      // log OpenGL info just for curiosity:
      GlInfo.LogGLProperties();

      // general OpenGL:
      glControl1.VSync = true;
      GL.ClearColor( Color.DarkBlue );
      GL.Enable( EnableCap.DepthTest );
      GL.ShadeModel( ShadingModel.Flat );

      // VBO init:
      VBOid = new uint[ 2 ];           // one big buffer for vertex data, another buffer for triangle indices
      GL.GenBuffers( 2, VBOid );
      GlInfo.LogError( "VBO init" );

      // shaders:
      canShaders = SetupShaders();

      // texture:
      GenerateTexture();
    }

    bool SetupShaders ()
    {
      activeProgram = null;

      foreach ( var programInfo in programs.Values )
        if ( programInfo.Setup() )
          activeProgram = programInfo.program;

      if ( activeProgram == null )
        return false;

      GlProgramInfo defInfo;
      if ( programs.TryGetValue( "default", out defInfo ) &&
           defInfo.program != null )
        activeProgram = defInfo.program;

      return true;
    }

    // generated texture:
    const int TEX_SIZE = 128;
    const int TEX_CHECKER_SIZE = 8;
    static Vector3 colWhite = new Vector3( 0.85f, 0.75f, 0.30f );
    static Vector3 colBlack = new Vector3( 0.15f, 0.15f, 0.50f );
    static Vector3 colShade = new Vector3( 0.15f, 0.15f, 0.15f );

    /// <summary>
    /// Texture handle
    /// </summary>
    int texName = 0;

    /// <summary>
    /// Generate the texture.
    /// </summary>
    void GenerateTexture ()
    {
      // {{ 
      GL.PixelStore( PixelStoreParameter.UnpackAlignment, 1 );
      texName = GL.GenTexture();
      GL.BindTexture( TextureTarget.Texture2D, texName );

      Vector3[,] data = new Vector3[ TEX_SIZE, TEX_SIZE ];
      for ( int y = 0; y < TEX_SIZE; y++ )
        for ( int x = 0; x < TEX_SIZE; x++ )
        {
          bool odd = ((x / TEX_CHECKER_SIZE + y / TEX_CHECKER_SIZE) & 1) > 0;
          data[ y, x ] = odd ? colBlack : colWhite;
          // add some fancy shading on the edges:
          if ( (x % TEX_CHECKER_SIZE) == 0 || (y % TEX_CHECKER_SIZE) == 0 )
            data[ y, x ] += colShade;
          if ( ((x+1) % TEX_CHECKER_SIZE) == 0 || ((y+1) % TEX_CHECKER_SIZE) == 0 )
            data[ y, x ] -= colShade;
        }

      GL.TexImage2D( TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, TEX_SIZE, TEX_SIZE, 0, PixelFormat.Rgb, PixelType.Float, data );

      GL.TexParameter( TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat );
      GL.TexParameter( TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat );
      GL.TexParameter( TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear );
      GL.TexParameter( TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMagFilter.Linear );

      GlInfo.LogError( "create-texture" );
    }

    /// <summary>
    /// Initialize VBO content and upload it to the GPU.
    /// </summary>
    void InitDataBuffers ()
    {
      // init data buffers for current simulation state (current number of launchers + max number of particles):
#if false
      // Vertex array: color [normal] coord
      GL.BindBuffer( BufferTarget.ArrayBuffer, VBOid[ 0 ] );
      int vertexBufferSize = scene.VertexBufferSize( true, true, true, true );
      GL.BufferData( BufferTarget.ArrayBuffer, (IntPtr)vertexBufferSize, IntPtr.Zero, BufferUsageHint.StaticDraw );
      IntPtr videoMemoryPtr = GL.MapBuffer( BufferTarget.ArrayBuffer, BufferAccess.WriteOnly );
      unsafe
      {
        triangleStride = scene.FillVertexBuffer( (float*)videoMemoryPtr.ToPointer(), true, true, true, true );
      }
      GL.UnmapBuffer( BufferTarget.ArrayBuffer );
      GL.BindBuffer( BufferTarget.ArrayBuffer, 0 );
      GlInfo.LogError( "fill vertex-buffer" );

      // Index buffer
      GL.BindBuffer( BufferTarget.ElementArrayBuffer, VBOid[ 1 ] );
      GL.BufferData( BufferTarget.ElementArrayBuffer, (IntPtr)(scene.Triangles * 3 * sizeof( uint )), IntPtr.Zero, BufferUsageHint.StaticDraw );
      videoMemoryPtr = GL.MapBuffer( BufferTarget.ElementArrayBuffer, BufferAccess.WriteOnly );
      unsafe
      {
        scene.FillIndexBuffer( (uint*)videoMemoryPtr.ToPointer() );
      }
      GL.UnmapBuffer( BufferTarget.ElementArrayBuffer );
      GL.BindBuffer( BufferTarget.ElementArrayBuffer, 0 );
      GlInfo.LogError( "fill index-buffer" );
#endif
    }

    /// <summary>
    /// Update data bufers for current simulation frame.
    /// </summary>
    void UpdateDataBuffers ()
    {
      // take data from the simulation
    }

    // appearance:
    Vector3 globalAmbient = new Vector3(  0.2f,  0.2f,  0.2f );
    Vector3 matAmbient    = new Vector3(  0.8f,  0.6f,  0.2f );
    Vector3 matDiffuse    = new Vector3(  0.8f,  0.6f,  0.2f );
    Vector3 matSpecular   = new Vector3(  0.8f,  0.8f,  0.8f );
    float   matShininess  = 100.0f;
    Vector3 whiteLight    = new Vector3(  1.0f,  1.0f,  1.0f );
    Vector3 lightPosition = new Vector3(-20.0f, 10.0f, 10.0f );
    Vector3 eyePosition   = new Vector3(  0.0f,  0.0f, 10.0f );

    void SetLightEye ( float size )
    {
      size += size;
      lightPosition = new Vector3( -2.0f * size, size, size );
      eyePosition   = new Vector3(         0.0f, 0.0f, size );
    }

    // attribute/vertex arrays:
    private void SetVertexAttrib ( bool on )
    {
      if ( activeProgram != null )
        if ( on )
          activeProgram.EnableVertexAttribArrays();
        else
          activeProgram.DisableVertexAttribArrays();
    }

    void InitShaderRepository ()
    {
      programs.Clear();
      GlProgramInfo pi;

      // default program:
      pi = new GlProgramInfo( "default", new GlShaderInfo[] {
        new GlShaderInfo( ShaderType.VertexShader, "vertex.glsl", "087fireworks" ),
        new GlShaderInfo( ShaderType.FragmentShader, "fragment.glsl", "087fireworks" ) } );
      programs[ pi.name ] = pi;

      // put more programs here:
      // pi = new GlProgramInfo( ..
      //   ..
      // programs[ pi.name ] = pi;
    }

    /// <summary>
    /// Simulate one frame.
    /// </summary>
    private void Simulate ()
    {

    }

    /// <summary>
    /// Render one frame.
    /// </summary>
    private void Render ()
    {
      if ( !loaded )
        return;

      frameCounter++;
      useShaders = canShaders &&
                   activeProgram != null &&
                   checkShaders.Checked;

      GL.Clear( ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit );
      GL.ShadeModel( checkSmooth.Checked ? ShadingModel.Smooth : ShadingModel.Flat );
      GL.PolygonMode( checkTwosided.Checked ? MaterialFace.FrontAndBack : MaterialFace.Front,
                      PolygonMode.Fill );
      if ( checkTwosided.Checked )
        GL.Disable( EnableCap.CullFace );
      else
        GL.Enable( EnableCap.CullFace );

      SetCamera();
      RenderScene();

      glControl1.SwapBuffers();
    }

    /// <summary>
    /// Rendering code itself (separated for clarity).
    /// </summary>
    void RenderScene ()
    {
      // Scene rendering:
      // [txt] [colors] [normals] vertices
      GL.BindBuffer( BufferTarget.ArrayBuffer, VBOid[ 0 ] );
      IntPtr p = IntPtr.Zero;

      if ( useShaders )
      {
        SetVertexAttrib( true );

        // using GLSL shaders:
        GL.UseProgram( activeProgram.Id );

        // uniforms:
        Matrix4 modelView = GetModelView();
        Matrix4 modelViewInv = GetModelViewInv();
        Vector3 relEye = Vector3.TransformVector( eyePosition, modelViewInv );
        GL.UniformMatrix4( activeProgram.GetUniform( "matrixModelView" ), false, ref modelView );
        if ( perspective )
          GL.UniformMatrix4( activeProgram.GetUniform( "matrixProjection" ), false, ref perspectiveProjection );
        else
          GL.UniformMatrix4( activeProgram.GetUniform( "matrixProjection" ), false, ref ortographicProjection );

        GL.Uniform3( activeProgram.GetUniform( "globalAmbient" ), ref globalAmbient );
        GL.Uniform3( activeProgram.GetUniform( "lightColor" ), ref whiteLight );
        GL.Uniform3( activeProgram.GetUniform( "lightPosition" ), ref lightPosition );
        GL.Uniform3( activeProgram.GetUniform( "eyePosition" ), ref relEye );
        GL.Uniform3( activeProgram.GetUniform( "Ka" ), ref matAmbient );
        GL.Uniform3( activeProgram.GetUniform( "Kd" ), ref matDiffuse );
        GL.Uniform3( activeProgram.GetUniform( "Ks" ), ref matSpecular );
        GL.Uniform1( activeProgram.GetUniform( "shininess" ), matShininess );

        // color handling:
        bool useGlobalColor = checkGlobalColor.Checked;
        if ( !useColors )
          useGlobalColor = true;
        GL.Uniform1( activeProgram.GetUniform( "globalColor" ), useGlobalColor ? 1 : 0 );
        GlInfo.LogError( "set-uniforms" );

        // texture handling:
        bool useTxt = checkTexture.Checked;
        if ( !useTexture )
          useTxt = false;
        GL.Uniform1( activeProgram.GetUniform( "useTexture" ), useTxt ? 1 : 0 );
        GL.Uniform1( activeProgram.GetUniform( "texSurface" ), 0 );
        if ( useTxt )
        {
          GL.ActiveTexture( TextureUnit.Texture0 );
          GL.BindTexture( TextureTarget.Texture2D, texName );
        }
        GlInfo.LogError( "set-texture" );

        if ( activeProgram.HasAttribute( "texCoords" ) )
        {
          GL.VertexAttribPointer( activeProgram.GetAttribute( "texCoords" ), 2, VertexAttribPointerType.Float, false, triangleStride, p );
          if ( useTexture )
            p += Vector2.SizeInBytes;
        }

        if ( activeProgram.HasAttribute( "color" ) )
        {
          GL.VertexAttribPointer( activeProgram.GetAttribute( "color" ), 3, VertexAttribPointerType.Float, false, triangleStride, p );
          if ( useColors )
            p += Vector3.SizeInBytes;
        }

        if ( activeProgram.HasAttribute( "normal" ) )
        {
          GL.VertexAttribPointer( activeProgram.GetAttribute( "normal" ), 3, VertexAttribPointerType.Float, false, triangleStride, p );
          if ( useNormals )
            p += Vector3.SizeInBytes;
        }

        GL.VertexAttribPointer( activeProgram.GetAttribute( "position" ), 3, VertexAttribPointerType.Float, false, triangleStride, p );
        GlInfo.LogError( "set-attrib-pointers" );

        // index buffer
        GL.BindBuffer( BufferTarget.ElementArrayBuffer, VBOid[ 1 ] );

        // engage!
        GL.DrawElements( PrimitiveType.Triangles, /* fw.Triangles */ 12 * 3, DrawElementsType.UnsignedInt, IntPtr.Zero );
        GlInfo.LogError( "draw-elements-shader" );
        GL.UseProgram( 0 );
      }
      else
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
