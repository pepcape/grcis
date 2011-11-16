using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Threading;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Scene3D;

namespace _038trackball
{
  public partial class Form1 : Form
  {
    protected SceneBrep scene = new SceneBrep();

    /// <summary>
    /// GLControl guard flag.
    /// </summary>
    bool loaded = false;

    /// <summary>
    /// Are we allowed to use VBO?
    /// </summary>
    bool useVBO = true;

    #region OpenGL globals

    private uint[] VBOid = new uint[ 3 ];       // vertex array (coords, normals), index array, color array
    private int stride = 0;                     // stride for vertex array

    #endregion

    #region FPS counter

    long lastFpsTime = 0L;
    int frameCounter = 0;
    long triangleCounter = 0L;

    #endregion

    public Form1 ()
    {
      InitializeComponent();
    }
    
    private void glControl1_Load ( object sender, EventArgs e )
    {
      loaded = true;

      // OpenGL init code:
      GL.ClearColor( Color.DarkBlue );
      GL.Enable( EnableCap.DepthTest );
      GL.ShadeModel( ShadingModel.Flat );

      // VBO init:
      GL.GenBuffers( 3, VBOid );
      if ( GL.GetError() != ErrorCode.NoError )
        useVBO = false;

      SetupViewport();

      Application.Idle += new EventHandler( Application_Idle );      
      comboTrackballType.SelectedIndex = 0;
    }

    private void glControl1_Resize ( object sender, EventArgs e )
    {
      if ( !loaded ) return;

      SetupViewport();
      glControl1.Invalidate();
    }

    private void glControl1_Paint ( object sender, PaintEventArgs e )
    {
      Render();
    }

    /// <summary>
    /// Prepare VBO content and upload it to the GPU.
    /// </summary>
    private void PrepareDataBuffers ()
    {
      if ( useVBO &&
           scene != null &&
           scene.Triangles > 0 )
      {
        GL.EnableClientState( ArrayCap.VertexArray );
        if ( scene.Normals > 0 )
          GL.EnableClientState( ArrayCap.NormalArray );
        GL.EnableClientState( ArrayCap.ColorArray );

        // Vertex array: coord [normal]
        GL.BindBuffer( BufferTarget.ArrayBuffer, VBOid[ 0 ] );
        int vertexBufferSize = scene.VertexBufferSize( true, true, false );
        GL.BufferData( BufferTarget.ArrayBuffer, (IntPtr)vertexBufferSize, IntPtr.Zero, BufferUsageHint.StaticDraw );
        IntPtr videoMemoryPtr = GL.MapBuffer( BufferTarget.ArrayBuffer, BufferAccess.WriteOnly );
        unsafe
        {
          stride = scene.FillVertexBuffer( (float*)videoMemoryPtr.ToPointer(), true, true, false );
        }
        GL.UnmapBuffer( BufferTarget.ArrayBuffer );
        GL.BindBuffer( BufferTarget.ArrayBuffer, 0 );

        GL.BindBuffer( BufferTarget.ElementArrayBuffer, VBOid[ 1 ] );
        GL.BufferData( BufferTarget.ElementArrayBuffer, (IntPtr)(scene.Triangles * 3 * sizeof( uint )), IntPtr.Zero, BufferUsageHint.StaticDraw );
        videoMemoryPtr = GL.MapBuffer( BufferTarget.ElementArrayBuffer, BufferAccess.WriteOnly );
        unsafe
        {
          scene.FillIndexBuffer( (uint*)videoMemoryPtr.ToPointer() );
        }
        GL.UnmapBuffer( BufferTarget.ElementArrayBuffer );
        GL.BindBuffer( BufferTarget.ElementArrayBuffer, 0 );

        GL.BindBuffer( BufferTarget.ArrayBuffer, VBOid[ 2 ] );
        // generate random color table:
        Random rnd = new Random( 12 );
        float[] colorTable = new float[ scene.Vertices * 3 ];
        for ( int i = 0; i < scene.Vertices * 3; i++ )
          colorTable[i] = (float)rnd.NextDouble();
        unsafe
        {
          fixed ( float* ct = colorTable )
          {
            GL.BufferData( BufferTarget.ArrayBuffer, (IntPtr)(scene.Vertices * 3 * sizeof( float )), (IntPtr)ct, BufferUsageHint.StaticDraw );
          }
        }
        GL.BindBuffer( BufferTarget.ArrayBuffer, 0 );
      }
      else
      {
        GL.DisableClientState( ArrayCap.VertexArray );
        GL.DisableClientState( ArrayCap.NormalArray );
        GL.DisableClientState( ArrayCap.ColorArray );

        if ( useVBO )
        {
          GL.BindBuffer( BufferTarget.ArrayBuffer, VBOid[ 0 ] );
          GL.BufferData( BufferTarget.ArrayBuffer, (IntPtr)0, IntPtr.Zero, BufferUsageHint.StaticDraw );
          GL.BindBuffer( BufferTarget.ArrayBuffer, 0 );
          GL.BindBuffer( BufferTarget.ElementArrayBuffer, VBOid[ 1 ] );
          GL.BufferData( BufferTarget.ElementArrayBuffer, (IntPtr)0, IntPtr.Zero, BufferUsageHint.StaticDraw );
          GL.BindBuffer( BufferTarget.ElementArrayBuffer, 0 );
          GL.BindBuffer( BufferTarget.ArrayBuffer, VBOid[ 2 ] );
          GL.BufferData( BufferTarget.ArrayBuffer, (IntPtr)0, IntPtr.Zero, BufferUsageHint.StaticDraw );
          GL.BindBuffer( BufferTarget.ArrayBuffer, 0 );
        }
      }
    }

    


  }
}
