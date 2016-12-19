// Author: 
// Original authors: Jan Benes, Josef Pelikan

using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using MathSupport;
using OpenglSupport;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using TexLib;

namespace _039terrain
{
  public partial class Form1
  {
    /// <summary>
    /// Optional data initialization.
    /// </summary>
    static void InitParams ( out int iterations, out float roughness, out string param, out string name )
    {
      iterations = 0;
      roughness  = 0.2f;
      param      = "";
      name       = "pilot";
    }

    #region GPU data

    /// <summary>
    /// Texture identifier (for one texture only, extend the source code if necessary)
    /// </summary>
    private int textureId = 0;

    private uint[] VBOid = new uint[ 2 ];   // [ 0 ] .. vertex array, [ 1 ] .. index buffer

    // vertex-buffer offsets:
    private int textureCoordOffset = 0;
    private int colorOffset        = 0;
    private int normalOffset       = 0;
    private int vertexOffset       = 0;
    private int stride             = 0;

    #endregion

    #region Lighting data

    // light:
    float[] ambientColor  = { 0.1f, 0.1f, 0.1f };
    float[] diffuseColor  = { 1.0f, 1.0f, 1.0f };
    float[] specularColor = { 1.0f, 1.0f, 1.0f };
    float[] lightPosition = { 1.0f, 1.0f, 0.0f, };

    // material:
    float[] materialAmbient  = { 0.1f, 0.1f, 0.1f };
    float[] materialDiffuse  = { 0.8f, 0.8f, 0.8f };
    float[] materialSpecular = { 0.5f, 0.5f, 0.5f };
    float  materialShininess = 60.0f;

    /// <summary>
    /// Current light position.
    /// </summary>
    Vector4 lightPos = Vector4.UnitY * 4.0f;

    /// <summary>
    /// Current light angle in radians.
    /// </summary>
    double lightAngle = 0.0;

    #endregion

    /// <summary>
    /// OpenGL init code.
    /// </summary>
    void InitOpenGL ()
    {
      // log OpenGL info just for curiosity:
      GlInfo.LogGLProperties();

      // OpenGL init code:
      glControl1.VSync = true;
      GL.ClearColor( Color.DarkBlue );
      GL.Enable( EnableCap.DepthTest );

      // VBO init:
      GL.GenBuffers( 2, VBOid ); // two buffers, one for vertex data, one for index data
      if ( GL.GetError() != ErrorCode.NoError )
        throw new Exception( "Couldn't create VBOs" );

      GL.Light( LightName.Light0, LightParameter.Ambient,  ambientColor );
      GL.Light( LightName.Light0, LightParameter.Diffuse,  diffuseColor );
      GL.Light( LightName.Light0, LightParameter.Specular, specularColor );
    }

    private void glControl1_Load ( object sender, EventArgs e )
    {
      // cold start
      InitOpenGL();

      // warm start
      SetupViewport();

      // initialize the scene
      int iterations  = (int)upDownIterations.Value;
      float roughness = (float)upDownRoughness.Value;
      Regenerate( iterations, roughness, textParam.Text );
      labelStatus.Text = "Triangles: " + scene.Triangles;

      // initialize the simulation
      InitSimulation( true );

      loaded = true;
      Application.Idle += new EventHandler( Application_Idle );
    }

    /// <summary>
    /// [Re-]generate the terrain data.
    /// </summary>
    /// <param name="iterations">Number of subdivision iteratons.</param>
    /// <param name="roughness">Roughness parameter.</param>
    /// <param name="param">Optional text parameter.</param>
    private void Regenerate ( int iterations, float roughness, string param )
    {
      // !!!{{ TODO: add terrain regeneration code here (to reflect the given terrain parameters)

      scene.Reset();

      // dummy rectangle, facing to the camera
      // notice that your terrain is supposed to be placed
      // in the XZ plane (elevation increases along the positive Y axis)
      scene.AddVertex( new Vector3( -0.5f, roughness, -0.5f ) );   // 0
      scene.AddVertex( new Vector3( -0.5f, 0.0f, +0.5f ) );        // 1
      scene.AddVertex( new Vector3( +0.5f, 0.0f, -0.5f ) );        // 2
      scene.AddVertex( new Vector3( +0.5f, 0.0f, +0.5f ) );        // 3

      scene.SetNormal( 0, (Vector3.UnitY + roughness * (Vector3.UnitX + Vector3.UnitZ)).Normalized() );
      scene.SetNormal( 1, (Vector3.UnitY + roughness * 0.5f * (Vector3.UnitX + Vector3.UnitZ)).Normalized() );
      scene.SetNormal( 2, (Vector3.UnitY + roughness * 0.5f * (Vector3.UnitX + Vector3.UnitZ)).Normalized() );
      scene.SetNormal( 3, Vector3.UnitY );

      float txtExtreme = 1.0f + iterations;
      scene.SetTxtCoord( 0, new Vector2( 0.0f, 0.0f ) );
      scene.SetTxtCoord( 1, new Vector2( 0.0f, txtExtreme ) );
      scene.SetTxtCoord( 2, new Vector2( txtExtreme, 0.0f ) );
      scene.SetTxtCoord( 3, new Vector2( txtExtreme, txtExtreme ) );

      scene.SetColor( 0, Vector3.UnitX );                    // red
      scene.SetColor( 1, Vector3.UnitY );                    // green
      scene.SetColor( 2, Vector3.UnitZ );                    // blue
      scene.SetColor( 3, new Vector3( 1.0f, 1.0f, 1.0f ) );  // white

      scene.AddTriangle( 1, 2, 0 );                          // last vertex is red
      scene.AddTriangle( 2, 1, 3 );                          // last vertex is white

      // this function uploads the data to the graphics card
      PrepareData();

      // load a texture
      if ( textureId > 0 )
      {
        GL.DeleteTexture( textureId );
        textureId = 0;
      }
      textureId = TexUtil.CreateTextureFromFile( "cgg256.png", "../../cgg256.png" );

      // simulation / hovercraft [re]initialization?
      InitSimulation( false );

      // !!!}}
    }

    /// <summary>
    /// last simulated time in seconds.
    /// </summary>
    double simTime = 0.0;

    /// <summary>
    /// Are we doing the terrain-flyover?
    /// </summary>
    bool hovercraft = false;

    /// <summary>
    /// Init of animation / hovercraft simulation, ...
    /// </summary>
    /// <param name="cold">True for global reset (including light-source/vehicle position..)</param>
    private void InitSimulation ( bool cold )
    {
      if ( hovercraft )
      {
        // !!! TODO: hovercraft init
      }
      else
        if ( cold )
        {
          lightPos = new Vector4( lightPosition[ 0 ], lightPosition[ 1 ], lightPosition[ 2 ], 1.0f );
          lightAngle = 0.0;
        }

      long nowTicks = DateTime.Now.Ticks;
      simTime = nowTicks* 1.0e-7;
    }

    private void glControl1_Paint ( object sender, PaintEventArgs e )
    {
      if ( checkAnim.Checked )
        Simulate( DateTime.Now.Ticks * 1.0e-7 );

      Render();
    }

    /// <summary>
    /// One step of animation / hovercraft simulation.
    /// </summary>
    /// <param name="time"></param>
    private void Simulate ( double time )
    {
      if ( !loaded ||
           time <= simTime )
        return;

      double dt = time - simTime;   // delta-time in seconds

      if ( hovercraft )
      {
        // !!! TODO: hovercraft simulation
      }

      lightAngle += dt;             // one radian per second..
      Matrix4 m;
      Matrix4.CreateRotationY( (float)lightAngle, out m );
      lightPos = Vector4.Transform( m, new Vector4( lightPosition[ 0 ], lightPosition[ 1 ], lightPosition[ 2 ], 1.0f ) );

      simTime = time;
    }

    /// <summary>
    /// Rendering of one frame.
    /// </summary>
    private void Render ()
    {
      if ( !loaded ) return;

      frameCounter++;

      // frame init:
      GL.Clear( ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit );
      GL.ShadeModel( checkSmooth.Checked ? ShadingModel.Smooth : ShadingModel.Flat );
      GL.PolygonMode( MaterialFace.FrontAndBack,
                      checkWireframe.Checked ? PolygonMode.Line : PolygonMode.Fill );

      // camera:
      SetCamera();

      // OpenGL lighting:
      GL.MatrixMode( MatrixMode.Modelview );
      GL.Light( LightName.Light0, LightParameter.Position, lightPos );
      GL.Enable( EnableCap.Light0 );
      GL.Enable( EnableCap.Lighting );

      // texturing:
      bool useTexture = scene.HasTxtCoords() &&
                        checkTexture.Checked &&
                        textureId > 0;
      if ( useTexture )
      {
        // set up the texture:
        GL.BindTexture( TextureTarget.Texture2D, textureId );
        GL.Enable( EnableCap.Texture2D );
      }
      else
      {
        GL.Disable( EnableCap.Texture2D );

        GL.ColorMaterial( MaterialFace.Front, ColorMaterialParameter.AmbientAndDiffuse );
        GL.Enable( EnableCap.ColorMaterial );
      }

      // common lighting colors/parameters:
      GL.Material( MaterialFace.Front, MaterialParameter.Ambient,   materialAmbient );
      GL.Material( MaterialFace.Front, MaterialParameter.Diffuse,   materialDiffuse );
      GL.Material( MaterialFace.Front, MaterialParameter.Specular,  materialSpecular );
      GL.Material( MaterialFace.Front, MaterialParameter.Shininess, materialShininess );

      // scene -> vertex buffer & index buffer

      // bind the vertex buffer:
      GL.BindBuffer( BufferTarget.ArrayBuffer, VBOid[ 0 ] );

      // tell OGL what sort of data we have and where in the buffer they could be found
      // the buffers we get from SceneBrep are interleaved => stride != 0
      if ( useTexture )
        GL.TexCoordPointer( 2, TexCoordPointerType.Float, stride, textureCoordOffset );

      if ( scene.HasColors() )
        GL.ColorPointer( 3, ColorPointerType.Float, stride, colorOffset );

      if ( scene.HasNormals() )
        GL.NormalPointer( NormalPointerType.Float, stride, normalOffset );

      GL.VertexPointer( 3, VertexPointerType.Float, stride, vertexOffset );

      // bind the index buffer:
      GL.BindBuffer( BufferTarget.ElementArrayBuffer, VBOid[ 1 ] );

      // draw the geometry:
      triangleCounter += scene.Triangles;
      GL.DrawElements( BeginMode.Triangles, scene.Triangles * 3, DrawElementsType.UnsignedInt, 0 );

      if ( useTexture )
        GL.BindTexture( TextureTarget.Texture2D, 0 );
      else
        GL.Disable( EnableCap.ColorMaterial );
      GL.Disable( EnableCap.Light0 );
      GL.Disable( EnableCap.Lighting );

      // light-source rendering (small white rectangle):
      GL.PointSize( 3.0f );
      GL.Begin( PrimitiveType.Points );
      GL.Color3( 1.0f, 1.0f, 1.0f );
      GL.Vertex4( lightPos );
      GL.End();

      // swap buffers:
      glControl1.SwapBuffers();
    }

    #region Camera attributes

    /// <summary>
    /// Current "up" vector.
    /// </summary>
    private Vector3 up = Vector3.UnitY;

    /// <summary>
    /// Vertical field-of-view angle in radians.
    /// </summary>
    private float fov = 1.0f;

    /// <summary>
    /// Camera's near point.
    /// </summary>
    private float near = 0.1f;

    /// <summary>
    /// Camera's far point.
    /// </summary>
    private float far = 200.0f;

    /// <summary>
    /// Current elevation angle in radians.
    /// </summary>
    private double elevationAngle = 0.1;

    /// <summary>
    /// Current azimuth angle in radians.
    /// </summary>
    private double azimuthAngle = 0.0;

    /// <summary>
    /// Current zoom factor.
    /// </summary>
    private double zoom = 2.0;

    #endregion

    /// <summary>
    /// Function called whenever the main application is idle..
    /// </summary>
    private void Application_Idle ( object sender, EventArgs e )
    {
      while ( glControl1.IsIdle )
      {
        glControl1.Invalidate();                // causes the GLcontrol 'repaint' action

        long now = DateTime.Now.Ticks;
        if ( now - lastFpsTime > 5000000 )      // more than 0.5 sec
        {
          lastFps = 0.5 * lastFps + 0.5 * (frameCounter    * 1.0e7 / (now - lastFpsTime));
          lastTps = 0.5 * lastTps + 0.5 * (triangleCounter * 1.0e7 / (now - lastFpsTime));
          lastFpsTime = now;
          frameCounter = 0;
          triangleCounter = 0L;

          if ( lastTps < 5.0e5 )
            labelFps.Text = string.Format( CultureInfo.InvariantCulture, "Fps: {0:f1}, tps: {1:f0}k",
                                           lastFps, (lastTps * 1.0e-3) );
          else
            labelFps.Text = string.Format( CultureInfo.InvariantCulture, "Fps: {0:f1}, tps: {1:f1}m",
                                           lastFps, (lastTps * 1.0e-6) );
        }
      }
    }

    /// <summary>
    /// Called in case the GLcontrol geometry changes.
    /// </summary>
    private void SetupViewport ()
    {
      int width  = glControl1.Width;
      int height = glControl1.Height;

      // 1. set ViewPort transform:
      GL.Viewport( 0, 0, width, height );

      // 2. set projection matrix
      GL.MatrixMode( MatrixMode.Projection );
      Matrix4 proj = Matrix4.CreatePerspectiveFieldOfView( fov, width / (float)height, near, far );
      GL.LoadMatrix( ref proj );
    }

    private void ResetCamera ()
    {
      elevationAngle = 0.1;
      azimuthAngle = 0.0;
      zoom = 2.0;
    }

    /// <summary>
    /// Camera setup, called for every frame prior to any rendering.
    /// </summary>
    private void SetCamera ()
    {
      if ( hovercraft )
      {
        // !!! TODO: hovercraft camera
      }
      else
      {
        Vector3 cameraPosition = new Vector3( 0.0f, 0, (float)zoom );

        Matrix4 rotateX = Matrix4.CreateRotationX( (float)-elevationAngle );
        Matrix4 rotateY = Matrix4.CreateRotationY( (float)azimuthAngle );

        cameraPosition = Vector3.TransformPosition( cameraPosition, rotateX );
        cameraPosition = Vector3.TransformPosition( cameraPosition, rotateY );

        GL.MatrixMode( MatrixMode.Modelview );
        Matrix4 lookAt = Matrix4.LookAt( cameraPosition, Vector3.Zero, up );

        GL.LoadMatrix( ref lookAt );
      }
    }

    /// <summary>
    /// Prepare VBO content and upload it to the GPU.
    /// You probably don't need to change this function..
    /// </summary>
    private void PrepareData ()
    {
      Debug.Assert( scene != null, "Missing scene" );

      if ( scene.Triangles == 0 )
        return;

      // enable the respective client states
      GL.EnableClientState( ArrayCap.VertexArray );   // vertex array (positions?)

      if ( scene.HasColors() )                        // colors, if any
        GL.EnableClientState( ArrayCap.ColorArray );

      if ( scene.HasNormals() )                       // normals, if any
        GL.EnableClientState( ArrayCap.NormalArray );

      if ( scene.HasTxtCoords() )                     // textures, if any
        GL.EnableClientState( ArrayCap.TextureCoordArray );

      // bind the vertex array (interleaved)
      GL.BindBuffer( BufferTarget.ArrayBuffer, VBOid[ 0 ] );

      // query the size of the buffer in bytes
      int vertexBufferSize = scene.VertexBufferSize(
          true, // we always have vertex data
          scene.HasTxtCoords(),
          scene.HasColors(),
          scene.HasNormals() );

      // fill vertexData with data we will upload to the (vertex) buffer on the graphics card
      float[] vertexData = new float[ vertexBufferSize / sizeof( float ) ];

      // calculate the offsets in the interleaved array
      textureCoordOffset = 0;
      colorOffset = textureCoordOffset + scene.TxtCoordsBytes();
      normalOffset = colorOffset + scene.ColorBytes();
      vertexOffset = normalOffset + scene.NormalBytes();

      // convert data from SceneBrep to float[] (interleaved array)
      unsafe
      {
        fixed ( float* fixedVertexData = vertexData )
        {
          stride = scene.FillVertexBuffer(
              fixedVertexData,
              true,
              scene.HasTxtCoords(),
              scene.HasColors(),
              scene.HasNormals() );

          // upload vertex data to the graphics card
          GL.BufferData(
              BufferTarget.ArrayBuffer,
              (IntPtr)vertexBufferSize,
              (IntPtr)fixedVertexData,        // still pinned down to fixed address..
              BufferUsageHint.StaticDraw );
        }
      }

      // index buffer:
      GL.BindBuffer( BufferTarget.ElementArrayBuffer, VBOid[ 1 ] );

      // convert indices from SceneBrep to uint[]
      uint[] indexData = new uint[ scene.Triangles * 3 ];

      unsafe
      {
        fixed ( uint* unsafeIndexData = indexData )
        {
          scene.FillIndexBuffer( unsafeIndexData );

          // upload index data to video memory
          GL.BufferData(
              BufferTarget.ElementArrayBuffer,
              (IntPtr)(scene.Triangles * 3 * sizeof( uint )),
              (IntPtr)unsafeIndexData,        // still pinned down to fixed address..
              BufferUsageHint.StaticDraw );
        }
      }
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

    private int dragFromX = 0;
    private int dragFromY = 0;
    private bool dragging = false;

    private void glControl1_MouseDown ( object sender, MouseEventArgs e )
    {
      if ( hovercraft )
      {
        // !!! TODO: hovercraft
      }
      else
      {
        dragFromX = e.X;
        dragFromY = e.Y;
        dragging = true;
      }
    }

    private void glControl1_MouseUp ( object sender, MouseEventArgs e )
    {
      if ( hovercraft )
      {
        // !!! TODO: hovercraft
      }
      else
      {
        dragging = false;
      }
    }

    private void glControl1_MouseMove ( object sender, MouseEventArgs e )
    {
      if ( hovercraft )
      {
        // !!! TODO: hovercraft
      }
      else
      {
        if ( !dragging ) return;

        int delta;
        if ( e.X != dragFromX )       // change the azimuth angle
        {
          delta = e.X - dragFromX;
          dragFromX = e.X;
          azimuthAngle -= delta * 4.0 / glControl1.Width;
        }

        if ( e.Y != dragFromY )       // change the elevation angle
        {
          delta = e.Y - dragFromY;
          dragFromY = e.Y;
          elevationAngle += delta * 2.0 / glControl1.Height;
          elevationAngle = Arith.Clamp( elevationAngle, -1.0, 1.5 );
        }
      }
    }

    private void glControl1_MouseWheel ( object sender, MouseEventArgs e )
    {
      if ( e.Delta != 0 )
        if ( hovercraft )
        {
          // !!! TODO: hovercraft
        }
        else
        {
          float change = e.Delta / 120.0f;
          zoom = Arith.Clamp( zoom * Math.Pow( 1.05, change ), 0.5, 100.0 );
        }
    }
  }
}
