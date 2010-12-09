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

namespace _015avatar
{
  public partial class Form1 : Form
  {
    protected SceneBrep scene = new SceneBrep();

    /// <summary>
    /// GLControl guard flag.
    /// </summary>
    bool loaded = false;

    #region OpenGL globals

    private uint[] VBOid = new uint[ 2 ];
    private int stride = 0;

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

    private void buttonOpen_Click ( object sender, EventArgs e )
    {
      OpenFileDialog ofd = new OpenFileDialog();

      ofd.Title = "Open Scene File";
      ofd.Filter = "Wavefront OBJ Files|*.obj" +
          "|All scene types|*.obj";

      ofd.FilterIndex = 1;
      ofd.FileName = "";
      if ( ofd.ShowDialog() != DialogResult.OK )
        return;

      WavefrontObj objReader = new WavefrontObj();
      objReader.MirrorConversion = false;
      StreamReader reader = new StreamReader( new FileStream( ofd.FileName, FileMode.Open ) );
      int faces = objReader.ReadBrep( reader, scene );
      reader.Close();
      scene.BuildCornerTable();
      int errors = scene.CheckCornerTable( null );

      labelFaces.Text = String.Format( "{0} faces, {1} errors", faces, errors );
      PrepareDataBuffers();
      glControl1.Invalidate();
    }

    private void buttonGenerate_Click ( object sender, EventArgs e )
    {
      Cursor.Current = Cursors.WaitCursor;
      float variant = (float)numericVariant.Value;

      scene.Reset();
      Construction cn = new Construction();
      int faces = cn.AddMesh( scene, Matrix4.Identity, variant );
      scene.BuildCornerTable();
      int errors = scene.CheckCornerTable( null );

      Cursor.Current = Cursors.Default;

      labelFaces.Text = String.Format( "{0} faces, {1} errors", faces, errors );
      PrepareDataBuffers();
      glControl1.Invalidate();
    }

    private void buttonSave_Click ( object sender, EventArgs e )
    {
      //if ( outputImage == null ) return;

      SaveFileDialog sfd = new SaveFileDialog();
      sfd.Title = "Save PNG file";
      sfd.Filter = "PNG Files|*.png";
      sfd.AddExtension = true;
      sfd.FileName = "";
      if ( sfd.ShowDialog() != DialogResult.OK )
        return;

      //outputImage.Save( sfd.FileName, System.Drawing.Imaging.ImageFormat.Png );
    }

    private void glControl1_Load ( object sender, EventArgs e )
    {
      loaded = true;

      // OpenGL init code:
      GL.ClearColor( Color.DarkBlue );
      GL.Enable( EnableCap.DepthTest );

      // VBO init:
      GL.GenBuffers( 2, VBOid );

      SetupViewport();

      Application.Idle += new EventHandler( Application_Idle );
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
      if ( scene != null &&
           scene.Triangles > 0 )
      {
        GL.EnableClientState( ArrayCap.VertexArray );
        if ( scene.Normals > 0 )
          GL.EnableClientState( ArrayCap.NormalArray );
        GL.EnableClientState( ArrayCap.IndexArray );

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
      }
      else
      {
        GL.DisableClientState( ArrayCap.VertexArray );
        GL.DisableClientState( ArrayCap.NormalArray );
        GL.DisableClientState( ArrayCap.IndexArray );

        GL.BindBuffer( BufferTarget.ArrayBuffer, VBOid[ 0 ] );
        GL.BufferData( BufferTarget.ArrayBuffer, (IntPtr)0, IntPtr.Zero, BufferUsageHint.StaticDraw );
        GL.BindBuffer( BufferTarget.ArrayBuffer, 0 );
        GL.BindBuffer( BufferTarget.ElementArrayBuffer, VBOid[ 1 ] );
        GL.BufferData( BufferTarget.ElementArrayBuffer, (IntPtr)0, IntPtr.Zero, BufferUsageHint.StaticDraw );
        GL.BindBuffer( BufferTarget.ElementArrayBuffer, 0 );
      }
    }

  }
}
