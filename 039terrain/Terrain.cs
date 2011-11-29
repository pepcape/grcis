using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Scene3D;
using TexLib;

namespace _039terrain
{
  public partial class Form1
  {
    // Realtime based animation:

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
    /// Camera's far point.
    /// </summary>
    private float far = 200.0f;

    #endregion
      
    #region OpenGL globals

    private uint[] VBOid = new uint[ 2 ];

    private int textureCoordOffset = 0;
    private int colorOffset = 0;
    private int normalOffset = 0;
    private int vertexOffset = 0;
    private int stride = 0;

    #endregion

    /// <summary>
    /// Function called whenever the main application is idle..
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Application_Idle ( object sender, EventArgs e )
    {
      while ( glControl1.IsIdle )
      {
        glControl1.Invalidate();
        Thread.Sleep( 5 );

        long now = DateTime.Now.Ticks;
        if ( now - lastFpsTime > 10000000 )      // more than 1 sec
        {
          double fps = frameCounter * 1.0e7 / (now - lastFpsTime);
          double tps = triangleCounter * 1.0e7 / (now - lastFpsTime);
          lastFpsTime = now;
          frameCounter = 0;
          triangleCounter = 0L;
          if ( tps < 5.0e5 )
            labelFps.Text = String.Format( "Fps: {0:0.0}, Tps: {1:0}k", fps, (tps * 1.0e-3) );
          else
            labelFps.Text = String.Format( "Fps: {0:0.0}, Tps: {1:0.0}m", fps, (tps * 1.0e-6) );
        }
      }
    }

    private int textureId = -1; // only one texture, if you need more, extend the source code as needed

    private void glControl1_Load ( object sender, EventArgs e )
    {
      loaded = true;

      // OpenGL init code:
      GL.ClearColor( Color.DarkBlue );
      GL.Enable( EnableCap.DepthTest );
      GL.ShadeModel( ShadingModel.Flat );

      // VBO init:
      GL.GenBuffers( 2, VBOid ); // two buffers, one for vertex data, one for index data
      if ( GL.GetError() != ErrorCode.NoError )
        throw new Exception( "Couldn't create VBOs" );

      // setup the viewport
      SetupViewport();

      Application.Idle += new EventHandler( Application_Idle );

      // create a new scene
      scene = new SceneBrep();

      // dummy rectangle, facing to the camera
      // notice that your terrain is supposed to be placed
      // in the XZ plane (elevation increases along the positive Y axis)
      scene.AddVertex( new Vector3( -0.5f, -0.5f, 0.0f ) );   // 0
      scene.AddVertex( new Vector3( -0.5f, +0.5f, 0.0f ) );   // 1
      scene.AddVertex( new Vector3( +0.5f, -0.5f, 0.0f ) );   // 2
      scene.AddVertex( new Vector3( +0.5f, +0.5f, 0.0f ) );   // 3

      scene.SetNormal( 0, Vector3.UnitZ );
      scene.SetNormal( 1, Vector3.UnitZ );
      scene.SetNormal( 2, Vector3.UnitZ );
      scene.SetNormal( 3, Vector3.UnitZ );

      scene.SetTxtCoord( 0, new Vector2( 1.0f, 0.0f ) );
      scene.SetTxtCoord( 1, new Vector2( 1.0f, 1.0f ) );
      scene.SetTxtCoord( 2, new Vector2( 0.0f, 0.0f ) );
      scene.SetTxtCoord( 3, new Vector2( 0.0f, 1.0f ) );

      scene.AddTriangle( 2, 1, 0 );
      scene.AddTriangle( 2, 3, 1 );

      // this function upload the above data to the graphics card
      UploadDataToGraphicsCard();

      // load a texture
      TexUtil.InitTexturing();
      string path = "cgg256.png";
      if ( !File.Exists( path ) )
        path = "../../" + path;
      textureId = TexUtil.CreateTextureFromFile( path );

      // some global setups
      GL.ShadeModel( ShadingModel.Smooth );
      GL.PolygonMode( MaterialFace.FrontAndBack, PolygonMode.Fill );
      //GL.Enable( EnableCap.CullFace );
      //GL.CullFace( CullFaceMode.Back );

      // setup a light
      float[] ambientColor = { 1.0f, 1.0f, 1.0f };
      float[] diffuseColor = { 1.0f, 1.0f, 1.0f };
      float[] lightPosition = { 50.0f, 10.0f, 0.0f, };

      GL.Light( LightName.Light1, LightParameter.Ambient, ambientColor );
      GL.Light( LightName.Light1, LightParameter.Diffuse, diffuseColor );

      GL.Enable( EnableCap.Light1 );
      GL.Enable( EnableCap.Lighting );
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
      Matrix4 proj = Matrix4.CreatePerspectiveFieldOfView( fov, width / (float)height, 0.1f, far );
      GL.LoadMatrix( ref proj );
    }

    private double elevationAngle = 0.0;
    private double azimuthAngle = 0.0;
    private double zoom = 3.0;

    /// <summary>
    /// Camera setup, called for every frame prior to any rendering.
    /// </summary>
    private void SetCamera ()
    {
      Vector3 cameraPosition = new Vector3( 0.0f, 0, (float)upDownZoom.Value );

      Matrix4 rotateX = Matrix4.CreateRotationX( (float)-elevationAngle );
      Matrix4 rotateY = Matrix4.CreateRotationY( (float)azimuthAngle );

      cameraPosition = Vector3.Transform( cameraPosition, rotateX );
      cameraPosition = Vector3.Transform( cameraPosition, rotateY );

      GL.MatrixMode( MatrixMode.Modelview );
      Matrix4 lookAt = Matrix4.LookAt( cameraPosition, Vector3.Zero, up );

      GL.LoadMatrix( ref lookAt );
    }

    /// <summary>
    /// Rendering of one frame.
    /// </summary>
    private void Render ()
    {
      if ( !loaded ) return;

      frameCounter++;
      GL.Clear( ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit );

      GL.PushMatrix();
      GL.LoadIdentity();
      GL.Light( LightName.Light1, LightParameter.Position, new Vector4( 50.0f, 2.0f, 0.0f, 1.0f ) );
      //GL.Light( LightName.Light1, LightParameter.SpotDirection, new Vector4( 0.0f, 0.0f, -1.0f, 0.0f ) ); 
      GL.PopMatrix();

      SetCamera();

      /* IF YOU'RE NOT GOING TO USE TEXTURE(S), YOU CAN USE THIS TO COLOR
       * THE GEOMETRY IN THE BUFFERS
       
      float[] materialAmbient = { 0.1f, 0.3f, 0.1f };
      float[] materialDiffuse = { 1.0f, 1.0f, 1.0f };

      GL.Material( MaterialFace.Front, MaterialParameter.Ambient, materialAmbient );
      GL.Material( MaterialFace.Front, MaterialParameter.Diffuse, materialDiffuse );
      
      GL.ColorMaterial( MaterialFace.Front, ColorMaterialParameter.Diffuse );
      GL.Enable( EnableCap.ColorMaterial );
      GL.ColorMaterial( MaterialFace.Front, ColorMaterialParameter.Diffuse );
      */

      // set up texture
      if ( scene.HasTxtCoords() )
        GL.BindTexture( TextureTarget.Texture2D, textureId );

      // scene -> vertex buffer & index buffer

      // bind the vertex buffer
      GL.BindBuffer( BufferTarget.ArrayBuffer, VBOid[ 0 ] );

      // tell OGL what sort of data we have and where in the buffer they could be found
      // the buffers we get from SceneBrep are interleaved => stride != 0
      if ( scene.HasTxtCoords() )
        GL.TexCoordPointer( 2, TexCoordPointerType.Float, stride, textureCoordOffset );

      if ( scene.HasColors() )
        GL.ColorPointer( 3, ColorPointerType.Float, stride, colorOffset );

      if ( scene.HasNormals() )
        GL.NormalPointer( NormalPointerType.Float, stride, normalOffset );

      GL.VertexPointer( 3, VertexPointerType.Float, stride, vertexOffset );

      // bind the index buffer
      GL.BindBuffer( BufferTarget.ElementArrayBuffer, VBOid[ 1 ] );

      // draw the geometry
      triangleCounter += scene.Triangles;
      GL.DrawElements( BeginMode.Triangles, scene.Triangles * 3, DrawElementsType.UnsignedInt, 0 );

      // swap buffers
      glControl1.SwapBuffers();
    }

    /// <summary>
    /// Prepare VBO content and upload it to the GPU.
    /// </summary>
    private void UploadDataToGraphicsCard ()
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
      colorOffset  = textureCoordOffset + scene.TxtCoordsBytes();
      normalOffset = colorOffset        + scene.ColorBytes();
      vertexOffset = normalOffset       + scene.NormalBytes();

      // convert data from SceneBrep to float[]
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
        }
      }

      // upload vertex data to the graphics card
      GL.BufferData(
          BufferTarget.ArrayBuffer,
          (IntPtr)vertexBufferSize,
          vertexData,
          BufferUsageHint.StaticDraw );

      // index buffer

      // convert from SceneBrep -> uint[]
      uint[] indexData = new uint[ scene.Triangles * 3 ];

      unsafe
      {
        fixed ( uint* unsafeIndexData = indexData )
        {
          scene.FillIndexBuffer( unsafeIndexData );
        }
      }

      // upload to video memory
      GL.BindBuffer( BufferTarget.ElementArrayBuffer, VBOid[ 1 ] );
      GL.BufferData(
          BufferTarget.ElementArrayBuffer,
          (IntPtr)(scene.Triangles * 3 * sizeof( uint )),
          indexData,
          BufferUsageHint.StaticDraw );
    }

    private void buttonRegenerate_Click ( object sender, EventArgs e )
    {
      // !!!{{ TODO: add terrain regeneration code here (to reflect changed terrain parameters)

      int iterations = (int)upDownIterations.Value;
      float roughness = (float)upDownRoughness.Value;

      // !!!}}
    }

    private void glControl1_MouseDown ( object sender, MouseEventArgs e )
    {
      // !!!{{ TODO: add the event handler here
      // !!!}}
    }

    private void glControl1_MouseUp ( object sender, MouseEventArgs e )
    {
      // !!!{{ TODO: add the event handler here
      // !!!}}
    }

    private void glControl1_MouseMove ( object sender, MouseEventArgs e )
    {
      // !!!{{ TODO: add the event handler here
      // !!!}}
    }

    private void glControl1_MouseWheel ( object sender, MouseEventArgs e )
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

    private void upDownAzimuth_ValueChanged ( object sender, EventArgs e )
    {
      azimuthAngle = (float)upDownAzimuth.Value;

      if ( azimuthAngle < 0.0 )
        azimuthAngle += Math.PI * 2.0f;
      else if ( azimuthAngle > Math.PI * 2.0f )
        azimuthAngle -= Math.PI * 2.0f;

      upDownAzimuth.Value = (decimal)azimuthAngle;
    }

    private void upDownElevation_ValueChanged ( object sender, EventArgs e )
    {
      elevationAngle = (float)upDownElevation.Value;
    }

    private void upDownZoom_ValueChanged ( object sender, EventArgs e )
    {
      zoom = (float)upDownZoom.Value;
    }
  }

}
