// author: Josef Pelikan

#define USE_INVALIDATE

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using MathSupport;
using OpenglSupport;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Utilities;

namespace _086shader
{
  public partial class Form1
  {
    /// <summary>
    /// Are we allowed to use VBO?
    /// </summary>
    bool useVBO = true;

    /// <summary>
    /// Can we use shaders?
    /// </summary>
    bool canShaders = false;

    /// <summary>
    /// Are we currently using shaders?
    /// </summary>
    bool useShaders = false;

    uint[] VBOid = null;         // vertex array (colors, normals, coords), index array
    int stride = 0;              // stride for vertex array

    /// <summary>
    /// Current texture.
    /// </summary>
    int texName = 0;

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
    /// It actually contains the redraw-loop.
    /// </summary>
    void Application_Idle ( object sender, EventArgs e )
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
    /// OpenGL init code (cold init).
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
      VBOid = new uint[ 2 ];
      GL.GenBuffers( 2, VBOid );
      useVBO = (GL.GetError() == ErrorCode.NoError);

      // shaders:
      if ( useVBO )
        canShaders = SetupShaders();

      // texture:
      texName = GenerateTexture();
    }

    /// <summary>
    /// Init shaders registered in global repository 'programs'.
    /// </summary>
    /// <returns>True if succeeded.</returns>
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

    /// <summary>
    /// Generate static procedural texture.
    /// </summary>
    /// <returns>Texture handle.</returns>
    int GenerateTexture ()
    {
      // generated texture:
      const int TEX_SIZE = 128;
      const int TEX_CHECKER_SIZE = 8;
      Vector3 colWhite = new Vector3( 0.85f, 0.75f, 0.30f );
      Vector3 colBlack = new Vector3( 0.15f, 0.15f, 0.60f );
      Vector3 colShade = new Vector3( 0.15f, 0.15f, 0.15f );

      GL.PixelStore( PixelStoreParameter.UnpackAlignment, 1 );
      int texName = GL.GenTexture();
      GL.BindTexture( TextureTarget.Texture2D, texName );

      Vector3[] data = new Vector3[ TEX_SIZE * TEX_SIZE ];
      for ( int y = 0; y < TEX_SIZE; y++ )
        for ( int x = 0; x < TEX_SIZE; x++ )
        {
          int i = y * TEX_SIZE + x;
          bool odd = ((x / TEX_CHECKER_SIZE + y / TEX_CHECKER_SIZE) & 1) > 0;
          data[ i ] = odd ? colBlack : colWhite;
          // add some fancy shading on the edges:
          if ( (x % TEX_CHECKER_SIZE) == 0 || (y % TEX_CHECKER_SIZE) == 0 )
            data[ i ] += colShade;
          if ( ((x + 1) % TEX_CHECKER_SIZE) == 0 || ((y + 1) % TEX_CHECKER_SIZE) == 0 )
            data[ i ] -= colShade;
        }

      GL.TexImage2D( TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, TEX_SIZE, TEX_SIZE, 0, PixelFormat.Rgb, PixelType.Float, data );

      GL.TexParameter( TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat );
      GL.TexParameter( TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat );
      GL.TexParameter( TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear );
      GL.TexParameter( TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMagFilter.Linear );

      GlInfo.LogError( "create-texture" );

      return texName;
    }

    /// <summary>
    /// De-allocated all the data associated with the given texture object.
    /// </summary>
    /// <param name="texName"></param>
    void DestroyTexture ( ref int texName )
    {
      int tHandle = texName;
      texName = 0;
      if ( tHandle != 0 )
        GL.DeleteTexture( tHandle );
    }

    /// <summary>
    /// Prepare VBO content and upload it to the GPU.
    /// </summary>
    void PrepareDataBuffers ()
    {
      if ( useVBO &&
           scene != null &&
           scene.Triangles > 0 )
      {
        // Vertex array: color [normal] coord
        GL.BindBuffer( BufferTarget.ArrayBuffer, VBOid[ 0 ] );
        int vertexBufferSize = scene.VertexBufferSize( true, true, true, true );
        GL.BufferData( BufferTarget.ArrayBuffer, (IntPtr)vertexBufferSize, IntPtr.Zero, BufferUsageHint.StaticDraw );
        IntPtr videoMemoryPtr = GL.MapBuffer( BufferTarget.ArrayBuffer, BufferAccess.WriteOnly );
        unsafe
        {
          stride = scene.FillVertexBuffer( (float*)videoMemoryPtr.ToPointer(), true, true, true, true );
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
      }
      else
      {
        if ( useVBO )
        {
          GL.BindBuffer( BufferTarget.ArrayBuffer, VBOid[ 0 ] );
          GL.BufferData( BufferTarget.ArrayBuffer, (IntPtr)0, IntPtr.Zero, BufferUsageHint.StaticDraw );
          GL.BindBuffer( BufferTarget.ArrayBuffer, 0 );
          GL.BindBuffer( BufferTarget.ElementArrayBuffer, VBOid[ 1 ] );
          GL.BufferData( BufferTarget.ElementArrayBuffer, (IntPtr)0, IntPtr.Zero, BufferUsageHint.StaticDraw );
          GL.BindBuffer( BufferTarget.ElementArrayBuffer, 0 );
        }
      }
    }

    // appearance:
    Vector3 globalAmbient = new Vector3(   0.2f,  0.2f,  0.2f );
    Vector3 matAmbient    = new Vector3(   0.8f,  0.6f,  0.2f );
    Vector3 matDiffuse    = new Vector3(   0.8f,  0.6f,  0.2f );
    Vector3 matSpecular   = new Vector3(   0.8f,  0.8f,  0.8f );
    float   matShininess  = 100.0f;
    Vector3 whiteLight    = new Vector3(   1.0f,  1.0f,  1.0f );
    Vector3 lightPosition = new Vector3( -20.0f, 10.0f, 10.0f );
    Vector3 eyePosition   = new Vector3(   0.0f,  0.0f, 10.0f );

    /// <summary>
    /// Set light-source and eye coordinates in the world-space.
    /// </summary>
    /// <param name="size">Relative size (based on the scene size).</param>
    /// <param name="light">Relative light position (default=[-2,1,1],viewer=[0,0,1]).</param>
    void SetLightEye ( float size, ref Vector3 light )
    {
      size += size;
      lightPosition = size * light;
      eyePosition   = new Vector3( 0.0f, 0.0f, size );
    }

    /// <summary>
    /// Update rendering/trackball parameters.
    /// </summary>
    /// <param name="param">User-provided parameter string.</param>
    void UpdateParams ( string param )
    {
      // input params:
      Dictionary<string, string> p = Util.ParseKeyValueList( param );
      if ( p.Count == 0 )
        return;

      // trackball: zoom limits
      if ( Util.TryParse( p, "minZoom", ref minZoom ) )
        minZoom = Math.Max( 1.0e-4f, minZoom );
      if ( Util.TryParse( p, "maxZoom", ref maxZoom ) )
        maxZoom = Arith.Clamp( maxZoom, minZoom, 1.0e6f );

      // trackball: zoom
      Util.TryParse( p, "zoom", ref zoom );
      zoom = Arith.Clamp( zoom, minZoom, maxZoom );

      // rendering: perspective/orthographic projection
      Util.TryParse( p, "perspective", ref perspective );

      // rendering: vertical field-of-view
      if ( !Util.TryParse( p, "fov", ref fov ) )
      {
        fov = Arith.Clamp( fov, 0.1f, 2.0f );
        SetupViewport();
      }

      // shading: relative light position
      if ( Geometry.TryParse( p, "light", ref light ) )
      {
        if ( light.Length < 1.0e-3f )
          light = new Vector3( -2, 1, 1 );
        SetLightEye( eyePosition.Z * 0.5f, ref light );
      }

      // shading: global material color
      if ( Geometry.TryParse( p, "color", ref matAmbient ) )
        matDiffuse = matAmbient;

      // shading: shininess
      if ( !Util.TryParse( p, "shininess", ref matShininess ) )
        matShininess = Arith.Clamp( matShininess, 1.0f, 1.0e4f );
    }

    // attribute/vertex arrays:
    bool vertexAttribOn = false;
    bool vertexPointerOn = false;

    private void SetVertexAttrib ( bool on )
    {
      if ( vertexAttribOn == on )
        return;

      if ( activeProgram != null )
        if ( on )
          activeProgram.EnableVertexAttribArrays();
        else
          activeProgram.DisableVertexAttribArrays();

      vertexAttribOn = on;
    }

    private void SetVertexPointer ( bool on )
    {
      if ( vertexPointerOn == on )
        return;

      if ( on )
      {
        GL.EnableClientState( ArrayCap.VertexArray );
        if ( scene.TxtCoords > 0 )
          GL.EnableClientState( ArrayCap.TextureCoordArray );
        if ( scene.Normals > 0 )
          GL.EnableClientState( ArrayCap.NormalArray );
        if ( scene.Colors > 0 )
          GL.EnableClientState( ArrayCap.ColorArray );
      }
      else
      {
        GL.DisableClientState( ArrayCap.VertexArray );
        GL.DisableClientState( ArrayCap.TextureCoordArray );
        GL.DisableClientState( ArrayCap.NormalArray );
        GL.DisableClientState( ArrayCap.ColorArray );
      }

      vertexPointerOn = on;
    }

    /// <summary>
    /// Fill the shader-repository.
    /// </summary>
    void InitShaderRepository ()
    {
      programs.Clear();
      GlProgramInfo pi;

      // default program:
      pi = new GlProgramInfo( "default", new GlShaderInfo[] {
        new GlShaderInfo( ShaderType.VertexShader, "vertex.glsl", "086shader" ),
        new GlShaderInfo( ShaderType.FragmentShader, "fragment.glsl", "086shader" ) } );
      programs[ pi.name ] = pi;

      // put more programs here:
      // pi = new GlProgramInfo( ..
      //   ..
      // programs[ pi.name ] = pi;
    }

    void DestroyShaders ()
    {
      foreach ( var prg in programs.Values )
        prg.Destroy();
    }

    /// <summary>
    /// Rendering one frame.
    /// </summary>
    private void Render ()
    {
      if ( !loaded )
        return;

      frameCounter++;
      useShaders = (scene != null) &&
                   scene.Triangles > 0 &&
                   useVBO &&
                   canShaders &&
                   activeProgram != null &&
                   checkShaders.Checked;

      GL.Clear( ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit );
      GL.ShadeModel( checkSmooth.Checked ? ShadingModel.Smooth : ShadingModel.Flat );
      GL.PolygonMode( checkTwosided.Checked ? MaterialFace.FrontAndBack : MaterialFace.Front,
                      checkWireframe.Checked ? PolygonMode.Line : PolygonMode.Fill );
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
      if ( scene != null &&
           scene.Triangles > 0 &&        // scene is nonempty => render it
           useVBO )
      {
        // [txt] [colors] [normals] vertices
        GL.BindBuffer( BufferTarget.ArrayBuffer, VBOid[ 0 ] );
        IntPtr p = IntPtr.Zero;

        if ( useShaders )
        {
          SetVertexPointer( false );
          SetVertexAttrib( true );

          // using GLSL shaders:
          GL.UseProgram( activeProgram.Id );

          // uniforms:
          Matrix4 modelView    = GetModelView();
          Matrix4 modelViewInv = GetModelViewInv();
          Vector3 relEye       = Vector3.TransformVector( eyePosition, modelViewInv );
          GL.UniformMatrix4( activeProgram.GetUniform( "matrixModelView" ), false, ref modelView );
          if ( perspective )
            GL.UniformMatrix4( activeProgram.GetUniform( "matrixProjection" ), false, ref perspectiveProjection );
          else
            GL.UniformMatrix4( activeProgram.GetUniform( "matrixProjection" ), false, ref ortographicProjection );

          GL.Uniform3( activeProgram.GetUniform( "globalAmbient" ), ref globalAmbient );
          GL.Uniform3( activeProgram.GetUniform( "lightColor" ),    ref whiteLight );
          GL.Uniform3( activeProgram.GetUniform( "lightPosition" ), ref lightPosition );
          GL.Uniform3( activeProgram.GetUniform( "eyePosition" ),   ref relEye );
          GL.Uniform3( activeProgram.GetUniform( "Ka" ),            ref matAmbient );
          GL.Uniform3( activeProgram.GetUniform( "Kd" ),            ref matDiffuse );
          GL.Uniform3( activeProgram.GetUniform( "Ks" ),            ref matSpecular );
          GL.Uniform1( activeProgram.GetUniform( "shininess" ),     matShininess );

          // color handling:
          bool useGlobalColor = checkGlobalColor.Checked;
          if ( !scene.HasColors() )
            useGlobalColor = true;
          GL.Uniform1( activeProgram.GetUniform( "globalColor" ), useGlobalColor ? 1 : 0 );

          // shading:
          bool shadingPhong = checkPhong.Checked;
          bool shadingGouraud = checkSmooth.Checked;
          if ( !shadingGouraud )
            shadingPhong = false;
          GL.Uniform1( activeProgram.GetUniform( "shadingPhong" ), shadingPhong ? 1 : 0 );
          GL.Uniform1( activeProgram.GetUniform( "shadingGouraud" ), shadingGouraud ? 1 : 0 );
          GL.Uniform1( activeProgram.GetUniform( "useAmbient" ), checkAmbient.Checked ? 1 : 0 );
          GL.Uniform1( activeProgram.GetUniform( "useDiffuse" ), checkDiffuse.Checked ? 1 : 0 );
          GL.Uniform1( activeProgram.GetUniform( "useSpecular" ), checkSpecular.Checked ? 1 : 0 );
          GlInfo.LogError( "set-uniforms" );

          // texture handling:
          bool useTexture = checkTexture.Checked;
          if ( !scene.HasTxtCoords() ||
               texName == 0 )
            useTexture = false;
          GL.Uniform1( activeProgram.GetUniform( "useTexture" ), useTexture ? 1 : 0 );
          GL.Uniform1( activeProgram.GetUniform( "texSurface" ), 0 );
          if ( useTexture )
          {
            GL.ActiveTexture( TextureUnit.Texture0 );
            GL.BindTexture( TextureTarget.Texture2D, texName );
          }
          GlInfo.LogError( "set-texture" );

          if ( activeProgram.HasAttribute( "texCoords" ) )
            GL.VertexAttribPointer( activeProgram.GetAttribute( "texCoords" ), 2, VertexAttribPointerType.Float, false, stride, p );
          if ( scene.HasTxtCoords() )
            p += Vector2.SizeInBytes;

          if ( activeProgram.HasAttribute( "color" ) )
            GL.VertexAttribPointer( activeProgram.GetAttribute( "color" ), 3, VertexAttribPointerType.Float, false, stride, p );
          if ( scene.HasColors() )
            p += Vector3.SizeInBytes;

          if ( activeProgram.HasAttribute( "normal" ) )
            GL.VertexAttribPointer( activeProgram.GetAttribute( "normal" ), 3, VertexAttribPointerType.Float, false, stride, p );
          if ( scene.HasNormals() )
            p += Vector3.SizeInBytes;

          GL.VertexAttribPointer( activeProgram.GetAttribute( "position" ), 3, VertexAttribPointerType.Float, false, stride, p );
          GlInfo.LogError( "set-attrib-pointers" );

          // index buffer
          GL.BindBuffer( BufferTarget.ElementArrayBuffer, VBOid[ 1 ] );

          // engage!
          GL.DrawElements( PrimitiveType.Triangles, scene.Triangles * 3, DrawElementsType.UnsignedInt, IntPtr.Zero );
          GlInfo.LogError( "draw-elements-shader" );

          // cleanup:
          GL.UseProgram( 0 );
          if ( useTexture )
            GL.BindTexture( TextureTarget.Texture2D, 0 );
        }
        else
        {
          SetVertexAttrib( false );
          SetVertexPointer( true );

          // texture handling:
          bool useTexture = checkTexture.Checked;
          if ( !scene.HasTxtCoords() ||
               texName == 0 )
            useTexture = false;
          if ( useTexture  )
          {
            GL.Enable( EnableCap.Texture2D );
            GL.ActiveTexture( TextureUnit.Texture0 );
            GL.BindTexture( TextureTarget.Texture2D, texName );
            GL.TexEnv( TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, (int)TextureEnvMode.Replace );
          }

          // using FFP:
          if ( scene.HasTxtCoords() )
          {
            GL.TexCoordPointer( 2, TexCoordPointerType.Float, stride, p );
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
          GlInfo.LogError( "draw-elements-ffp" );

          if ( useTexture  )
          {
            GL.BindTexture( TextureTarget.Texture2D, 0 );
            GL.Disable( EnableCap.Texture2D );
          }
        }

        triangleCounter += scene.Triangles;
      }
      else                              // color cube
      {
        SetVertexPointer( false );
        SetVertexAttrib( false );

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
