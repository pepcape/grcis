using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Scene3D;

namespace _015avatar
{
  public partial class Form1 : Form
  {
    protected SceneBrep scene = new SceneBrep();

    protected Bitmap outputImage = null;

    bool loaded = false;

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
      redraw();
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
      redraw();
    }

    private void redraw ()
    {
      if ( scene == null ) return;

      Cursor.Current = Cursors.WaitCursor;

      int width  = glControl1.Width;
      int height = glControl1.Height;
      outputImage = new Bitmap( width, height, System.Drawing.Imaging.PixelFormat.Format24bppRgb );

      /*
      Wireframe renderer = new Wireframe();
      renderer.Perspective = checkPerspective.Checked;
      renderer.Azimuth     = (double)numericAzimuth.Value;
      renderer.Elevation   = (double)numericElevation.Value;
      renderer.ViewVolume  = 30.0;
      renderer.Distance    = 10.0;
      renderer.DrawNormals = checkNormals.Checked;
      renderer.Render( outputImage, scene );
      */

      Cursor.Current = Cursors.Default;
    }

    private void buttonSave_Click ( object sender, EventArgs e )
    {
      if ( outputImage == null ) return;

      SaveFileDialog sfd = new SaveFileDialog();
      sfd.Title = "Save PNG file";
      sfd.Filter = "PNG Files|*.png";
      sfd.AddExtension = true;
      sfd.FileName = "";
      if ( sfd.ShowDialog() != DialogResult.OK )
        return;

      outputImage.Save( sfd.FileName, System.Drawing.Imaging.ImageFormat.Png );
    }

    private void glControl1_Load ( object sender, EventArgs e )
    {
      loaded = true;
      GL.ClearColor( Color.DarkBlue );
      SetupViewport();
    }

    private void SetupViewport ()
    {
      int w = glControl1.Width;
      int h = glControl1.Height;
      GL.MatrixMode( MatrixMode.Projection );
      GL.LoadIdentity();
      GL.Ortho( 0, w, 0, h, -1, 1 ); // Bottom-left corner pixel has coordinate (0, 0)
      GL.Viewport( 0, 0, w, h ); // Use all of the glControl painting area
    }

    private void glControl1_Resize ( object sender, EventArgs e )
    {
      SetupViewport();
      glControl1.Invalidate();
    }

    private void glControl1_Paint ( object sender, PaintEventArgs e )
    {
      if ( !loaded ) return;

      GL.Clear( ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit );

      GL.MatrixMode( MatrixMode.Modelview );
      GL.LoadIdentity();
      GL.Color3( Color.Yellow );
      GL.Begin( BeginMode.Triangles );
      GL.Vertex2( 10, 20 );
      GL.Vertex2( 100, 20 );
      GL.Vertex2( 100, 50 );
      GL.End();

      glControl1.SwapBuffers();
    }
  }
}
