// Author: Jan Benes, Josef Pelikan

using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace _039terrain
{
  public partial class Form1
  {
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
    /// Prepare VBO content and upload it to the GPU.
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
      colorOffset        = textureCoordOffset + scene.TxtCoordsBytes();
      normalOffset       = colorOffset        + scene.ColorBytes();
      vertexOffset       = normalOffset       + scene.NormalBytes();

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

    private int dragFromX = 0;
    private int dragFromY = 0;
    private bool dragging = false;

    private void glControl1_MouseDown ( object sender, MouseEventArgs e )
    {
      dragFromX = e.X;
      dragFromY = e.Y;
      dragging = true;
    }

    private void glControl1_MouseUp ( object sender, MouseEventArgs e )
    {
      dragging = false;
    }

    private void glControl1_MouseMove ( object sender, MouseEventArgs e )
    {
      if ( !dragging ) return;

      int delta;
      if ( e.X != dragFromX )       // change the azimuth angle
      {
        delta = e.X - dragFromX;
        dragFromX = e.X;
        azimuthAngle -= delta * 4.0f / glControl1.Width;

        upDownAzimuth.Value = (decimal)azimuthAngle;
      }

      if ( e.Y != dragFromY )       // change the elevation angle
      {
        delta = e.Y - dragFromY;
        dragFromY = e.Y;
        elevationAngle += delta * 2.0f / glControl1.Height;
        if ( elevationAngle < 0.0f )
          elevationAngle = 0.0f;
        if ( elevationAngle > 1.5f )
          elevationAngle = 1.5f;
        upDownElevation.Value = (decimal)elevationAngle;
      }
    }

    private void glControl1_MouseWheel ( object sender, MouseEventArgs e )
    {
      if ( e.Delta != 0 )
      {
        float change = e.Delta / 120.0f;
        float coef = change > 0.0f ? 1.05f : 0.95f;

        for ( int i = 0; i++ < Math.Abs( change ); )
          zoom *= coef;

        if ( zoom < 1.0f )   zoom = 1.0f;
        if ( zoom > 100.0f ) zoom = 100.0f;

        upDownZoom.Value = (decimal)zoom;
      }
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

    private void buttonRegenerate_Click ( object sender, EventArgs e )
    {
      int iterations = (int)upDownIterations.Value;
      float roughness = (float)upDownRoughness.Value;
      Regenerate( iterations, roughness );
    }
  }
}
