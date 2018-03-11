using System;
using System.Drawing;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using Scene3D;

namespace _040morph3d
{
  public partial class MorphForm : Form
  {
    static readonly string rev = "$Rev$".Split( ' ' )[ 1 ];

    public MorphForm ()
    {
      InitializeComponent();
      Text += " (rev: " + rev + ')';

      mMorph = new Morph();
      mWavefrontObj = new WavefrontObj();
      //mFirstObject = new SceneBrep();
      //mSecondObject = new SceneBrep();
      mCamera = new Camera();
    }

    private Morph mMorph;
    private SceneBrep mFirstObject;
    private SceneBrep mSecondObject;
    private WavefrontObj mWavefrontObj;
    private Camera mCamera;

    private void SetupViewport ( int wid, int hei )
    {
      // 1. set ViewPort transform:
      GL.Viewport( 0, 0, wid, hei );

      // 2. set projection matrix
      GL.MatrixMode( MatrixMode.Projection );
      Matrix4 proj = Matrix4.CreatePerspectiveFieldOfView( 1.0f, wid / (float)hei, 0.1f, 200.0f );
      GL.LoadMatrix( ref proj );
    }

    private void RenderScene ( SceneBrep scene )
    {
      if ( scene != null && scene.Triangles > 0 )
      {
        if ( scene.HasNormals() && scene.Normals > 0 )
        {
          GL.Begin( PrimitiveType.Triangles );
          for ( int i = 0; i < scene.Triangles; ++i )
          {
            int v1, v2, v3;
            scene.GetTriangleVertices( i, out v1, out v2, out v3 );
            GL.Normal3( scene.GetNormal( v1 ) );
            GL.Vertex3( scene.GetVertex( v1 ) );
            GL.Normal3( scene.GetNormal( v2 ) );
            GL.Vertex3( scene.GetVertex( v2 ) );
            GL.Normal3( scene.GetNormal( v3 ) );
            GL.Vertex3( scene.GetVertex( v3 ) );
          }
          GL.End();
        }
        else
        {
          GL.End();
          GL.Begin( PrimitiveType.Triangles );
          for ( int i = 0; i < scene.Triangles; ++i )
          {
            Vector3 v1, v2, v3;
            scene.GetTriangleVertices( i, out v1, out v2, out v3 );
            GL.Vertex3( v1 );
            GL.Vertex3( v2 );
            GL.Vertex3( v3 );
          }
          GL.End();
        }
      }
      else                              // color cube (JB)
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
      }
    }

    private void loadButtonPressed ( object sender, EventArgs e )
    {
      SceneBrep brep = new SceneBrep();
      if ( sender == buttonLoadFirst )
        mFirstObject = brep;
      else
      if ( sender == buttonLoadSecond )
        mSecondObject = brep;
      else
        return;

      OpenFileDialog ofd = new OpenFileDialog();
      ofd.Title = "Open Scene File";
      ofd.Filter = "Wavefront OBJ Files|*.obj;*.obj.gz" +
          "|All scene types|*.obj";
      ofd.FilterIndex = 1;
      ofd.FileName = "";
      if ( ofd.ShowDialog() != DialogResult.OK )
        return;

      if ( mWavefrontObj.ReadBrep( ofd.FileName, brep ) < 0 )
        MessageBox.Show( this, "Loading failed" );

      mMorph.AssignObjects( mFirstObject, mSecondObject );
      mMorph.Interpolate( ((float)trackBar1.Value) / trackBar1.Maximum );
      redrawAll();
    }

    private void glControl_MouseDown ( object sender, MouseEventArgs e )
    {
      mOrbit = true;
      mClickLocation = e.Location;
    }

    private void glControl_MouseUp ( object sender, MouseEventArgs e )
    {
      mOrbit = false;
    }

    private bool mOrbit = false;
    private Point mClickLocation;

    private void glControl_MouseMove ( object sender, MouseEventArgs e )
    {
      if ( !mOrbit ) return;

      Point p = new Point();
      p.X = mClickLocation.X - e.Location.X;
      p.Y = mClickLocation.Y - e.Location.Y;
      mClickLocation = e.Location;
      double factor = -5.0 / (double)((OpenTK.GLControl)sender).Width;

      mCamera.Orbit( p.X * factor, p.Y * factor );
      redrawAll();
    }

    private void glControl_MouseLeave ( object sender, EventArgs e )
    {
      mOrbit = false;
    }

    private void redrawAll ()
    {
      glControl3.Invalidate();
      glControl1.Invalidate();
      glControl2.Invalidate();
    }

    private void glControl_Load ( object sender, EventArgs e )
    {
      ((OpenTK.GLControl)sender).MakeCurrent();
      SetupViewport( ((OpenTK.GLControl)sender).Width, ((OpenTK.GLControl)sender).Height );

      GL.ClearColor( Color.DarkBlue );
      GL.Enable( EnableCap.DepthTest );
      GL.ShadeModel( ShadingModel.Flat );

      GL.Enable( EnableCap.Lighting );
      GL.Enable( EnableCap.ColorMaterial );
      GL.ColorMaterial( MaterialFace.FrontAndBack, ColorMaterialParameter.AmbientAndDiffuse );
      GL.Enable( EnableCap.Light0 );
      GL.Light( LightName.Light0, LightParameter.Diffuse, new Color4( 0.8f, 0.8f, 0.8f, 1.0f ) );
      GL.Light( LightName.Light0, LightParameter.Position, new Vector4( 50.0f, 50.0f, 150.0f, 1.0f ) );
      //GL.Enable(EnableCap.Light1);
    }

    private void glControl_Resize ( object sender, EventArgs e )
    {
      ((OpenTK.GLControl)sender).MakeCurrent();
      SetupViewport( ((OpenTK.GLControl)sender).Width, ((OpenTK.GLControl)sender).Height );
      ((OpenTK.GLControl)sender).Invalidate();
    }

    private void glControl_Paint ( object sender, PaintEventArgs e )
    {
      ((OpenTK.GLControl)sender).MakeCurrent();
      GL.Clear( ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit );
      mCamera.SetupGLView();

      GL.Light( LightName.Light0, LightParameter.Position, new Vector4( -50.0f, -50.0f, 150.0f, 1.0f ) );
      if ( sender == glControl2 )
        RenderScene( mFirstObject );
      else if ( sender == glControl3 )
        RenderScene( mSecondObject );
      else if ( sender == glControl1 )
        RenderScene( mMorph.InterpolatedObject );

      ((OpenTK.GLControl)sender).SwapBuffers();
    }

    private void trackBar1_ValueChanged ( object sender, EventArgs e )
    {
      mMorph.Interpolate( ((float)trackBar1.Value) / trackBar1.Maximum );
      redrawAll();
    }

    private void glControl_MouseWheel ( object sender, System.Windows.Forms.MouseEventArgs e )
    {
      mCamera.Zoom( Math.Pow( 1.05, -1.0 * ((double)e.Delta / 120.0) ) );
      redrawAll();
    }
  }
}
