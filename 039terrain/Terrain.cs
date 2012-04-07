using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Scene3D;
using TexLib;

namespace _039terrain
{
  public partial class Form1
  {
    /// <summary>
    /// Texture identifier (for one texture only, if you need more, extend the source code as needed)
    /// </summary>
    private int textureId = -1;

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

      // initialize the  scene
      Regenerate( 0, 0.1f );

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
    /// [Re-]generate the terrain data.
    /// </summary>
    /// <param name="iterations">Number of subdivision iteratons</param>
    /// <param name="roughness">Roughness parameter</param>
    private void Regenerate ( int iterations, float roughness )
    {
      // !!!{{ TODO: add terrain regeneration code here (to reflect the given terrain parameters)

      scene.Reset();

      // dummy rectangle, facing to the camera
      // notice that your terrain is supposed to be placed
      // in the XZ plane (elevation increases along the positive Y axis)
      scene.AddVertex( new Vector3( -0.5f, roughness, -0.5f ) );   // 0
      scene.AddVertex( new Vector3( -0.5f, 0.0f, +0.5f ) );   // 1
      scene.AddVertex( new Vector3( +0.5f, 0.0f, -0.5f ) );   // 2
      scene.AddVertex( new Vector3( +0.5f, 0.0f, +0.5f ) );   // 3

      scene.SetNormal( 0, Vector3.UnitY );
      scene.SetNormal( 1, Vector3.UnitY );
      scene.SetNormal( 2, Vector3.UnitY );
      scene.SetNormal( 3, Vector3.UnitY );

      float txtExtreme = 1.0f + iterations;
      scene.SetTxtCoord( 0, new Vector2( txtExtreme, 0.0f ) );
      scene.SetTxtCoord( 1, new Vector2( txtExtreme, txtExtreme ) );
      scene.SetTxtCoord( 2, new Vector2( 0.0f, 0.0f ) );
      scene.SetTxtCoord( 3, new Vector2( 0.0f, txtExtreme ) );

      scene.AddTriangle( 2, 1, 0 );
      scene.AddTriangle( 2, 3, 1 );

      // this function uploads the data to the graphics card
      PrepareData();

      // load a texture
      TexUtil.InitTexturing();
      if ( textureId >= 0 )
        GL.DeleteTexture( textureId );
      textureId = TexUtil.CreateTextureFromFile( "cgg256.png", "../../cgg256.png" );

      // !!!}}
    }

    /// <summary>
    /// Rendering of one frame.
    /// </summary>
    private void Render ()
    {
      if ( !loaded ) return;

      frameCounter++;
      GL.Clear( ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit );

      // OpenGL light:
      GL.MatrixMode( MatrixMode.Modelview );
      GL.PushMatrix();
      GL.LoadIdentity();
      GL.Light( LightName.Light1, LightParameter.Position, new Vector4( 5.0f, 2.0f, 0.0f, 1.0f ) );
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
  }
}
