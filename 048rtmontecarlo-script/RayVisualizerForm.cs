using MathSupport;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Scene3D;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using _048rtmontecarlo;

namespace Rendering
{
  public partial class RayVisualizerForm: Form
  {
    public static RayVisualizerForm instance; //singleton

    /// <summary>
    /// Scene read from file.
    /// </summary>
    SceneBrep scene = new SceneBrep ();

    /// <summary>
    /// Scene center point.
    /// </summary>
    Vector3 center = Vector3.Zero;

    /// <summary>
    /// Scene diameter.
    /// </summary>
    float diameter = 4.0f;

    float near = 0.1f;
    float far  = 25.0f;

    Vector3 light = new Vector3 ( -2, 1, 1 );

    /// <summary>
    /// GLControl guard flag.
    /// </summary>
    bool loaded = false;

    /// <summary>
    /// Associated Trackball instance.
    /// </summary>
    internal Trackball trackBall = null;

    /// <summary>
    /// Frustum vertices, 0 or 8 vertices
    /// </summary>
    List<Vector3> frustumFrame = new List<Vector3> ();

    /// <summary>
    /// Point in the 3D scene pointed out by an user, or null.
    /// </summary>
    Vector3? spot = null;

    Vector3? pointOrigin = null;
    Vector3  pointTarget;
    Vector3  eye;

    bool pointDirty = false;

    public RayVisualizerForm ()
    {
      InitializeComponent ();

      Form1.singleton.RayVisualiserButton.Enabled = false;

      trackBall = new Trackball ( center, diameter );

      InitShaderRepository ();

      RayVisualizerForm.instance = this;

      RayVisualizer.instance = new RayVisualizer ();

      Cursor.Current = Cursors.Default;
    }

    private void GenerateMesh () // formerly: "buttonGenerate_Click ( object sender, EventArgs e )"
    {
      Cursor.Current = Cursors.WaitCursor;

      bool doCheck = false;

      scene.Reset ();

      Construction cn = new Construction ();

      cn.AddMesh ( scene, Matrix4.Identity, null );
      diameter = scene.GetDiameter ( out center );

      scene.BuildCornerTable ();
      int errors = doCheck ? scene.CheckCornerTable ( null ) : 0;
      scene.GenerateColors ( 12 );

      trackBall.Center   = center;
      trackBall.Diameter = diameter;
      SetLight ( diameter, ref light );
      trackBall.Reset ();

      PrepareDataBuffers ();

      glControl1.Invalidate ();

      Cursor.Current = Cursors.Default;
    }

    private void glControl1_Load ( object sender, EventArgs e )
    {
      InitOpenGL ();

      trackBall.GLsetupViewport ( glControl1.Width, glControl1.Height, near, far );

      loaded = true;

      Application.Idle += new EventHandler ( Application_Idle );
    }

    private void glControl1_Resize ( object sender, EventArgs e )
    {
      if ( !loaded )
        return;

      trackBall.GLsetupViewport ( glControl1.Width, glControl1.Height, near, far );

      glControl1.Invalidate ();
    }

    private void glControl1_Paint ( object sender, PaintEventArgs e )
    {
      Render ();
    }

    private void Form3_FormClosing ( object sender, FormClosingEventArgs e )
    {
      DestroyTexture ( ref texName );

      if ( VBOid != null && VBOid [ 0 ] != 0 )
      {
        GL.DeleteBuffers ( 2, VBOid );
        VBOid = null;
      }

      DestroyShaders ();
    }

    /// <summary>
    /// Unproject support
    /// </summary>
    Vector3 screenToWorld ( int x, int y, float z = 0.0f )
    {
      Matrix4 modelViewMatrix, projectionMatrix;
      GL.GetFloat ( GetPName.ModelviewMatrix, out modelViewMatrix );
      GL.GetFloat ( GetPName.ProjectionMatrix, out projectionMatrix );

      return Geometry.UnProject ( ref projectionMatrix, ref modelViewMatrix, glControl1.Width, glControl1.Height, x,
                                  glControl1.Height - y, z );
    }

    private void glControl1_MouseDown ( object sender, MouseEventArgs e )
    {
      if ( !trackBall.MouseDown ( e ) )
        if ( checkAxes.Checked )
        {
          // pointing to the scene:
          pointOrigin = screenToWorld ( e.X, e.Y, 0.0f );
          pointTarget = screenToWorld ( e.X, e.Y, 1.0f );

          eye        = trackBall.Eye;
          pointDirty = true;
        }
    }

    private void glControl1_MouseUp ( object sender, MouseEventArgs e )
    {
      trackBall.MouseUp ( e );
    }

    private void glControl1_MouseMove ( object sender, MouseEventArgs e )
    {
      if ( AllignCameraCheckBox.Checked && e.Button == trackBall.Button )
      {
        MessageBox.Show ( "You can not use mouse to rotate scene while \"Keep alligned\" box is checked." );
        return;
      }

      trackBall.MouseMove ( e );
    }

    private void glControl1_MouseWheel ( object sender, MouseEventArgs e )
    {
      trackBall.MouseWheel ( e );
    }

    private void glControl1_KeyDown ( object sender, KeyEventArgs e )
    {
      trackBall.KeyDown ( e );
    }

    private void glControl1_KeyUp ( object sender, KeyEventArgs e )
    {
      if ( !trackBall.KeyUp ( e ) )
      {
        if ( e.KeyCode == Keys.F )
        {
          e.Handled = true;
          if ( frustumFrame.Count > 0 )
            frustumFrame.Clear ();
          else
          {
            float N = 0.0f;
            float F = 1.0f;
            int   R = glControl1.Width - 1;
            int   B = glControl1.Height - 1;
            frustumFrame.Add ( screenToWorld ( 0, 0, N ) );
            frustumFrame.Add ( screenToWorld ( R, 0, N ) );
            frustumFrame.Add ( screenToWorld ( 0, B, N ) );
            frustumFrame.Add ( screenToWorld ( R, B, N ) );
            frustumFrame.Add ( screenToWorld ( 0, 0, F ) );
            frustumFrame.Add ( screenToWorld ( R, 0, F ) );
            frustumFrame.Add ( screenToWorld ( 0, B, F ) );
            frustumFrame.Add ( screenToWorld ( R, B, F ) );
          }
        }
      }
    }

    private void Form3_FormClosed ( object sender, FormClosedEventArgs e )
    {
      Form1.singleton.RayVisualiserButton.Enabled = true;

      instance = null;
    }

    private void AllignCamera ( object sender, EventArgs e )
    {
      if ( RayVisualizer.instance?.rays.Count < 2 )
      {
        return;
      }

      trackBall.Center = (Vector3) RayVisualizer.instance.rays [ 1 ];
      trackBall.Reset ( (Vector3) ( RayVisualizer.instance.rays [ 1 ] - RayVisualizer.instance.rays [ 0 ] ) );
      trackBall.Zoom = (float) Vector3d.Distance ( RayVisualizer.instance.rays [ 1 ], RayVisualizer.instance.rays [ 0 ] );
    }
  }
}