using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using MathSupport;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Scene3D;
using Utilities;

namespace _086shader
{
  public partial class Form1 : Form
  {
    static readonly string rev = Util.SetVersion( "$Rev$" );

    /// <summary>
    /// Scene read from file.
    /// </summary>
    SceneBrep scene = new SceneBrep();

    /// <summary>
    /// Scene center point.
    /// </summary>
    Vector3 center = Vector3.Zero;

    /// <summary>
    /// Scene diameter.
    /// </summary>
    float diameter = 4.0f;

    float near = 0.1f;
    float far  = 5.0f;

    Vector3 light = new Vector3( -2, 1, 1 );

    /// <summary>
    /// Point in the 3D scene pointed out by an user, or null.
    /// </summary>
    Vector3? spot = null;

    Vector3? pointOrigin = null;
    Vector3 pointTarget;

    bool pointDirty = false;

    /// <summary>
    /// Frustum vertices, 0 or 8 vertices
    /// </summary>
    List<Vector3> frustumFrame = new List<Vector3>();

    /// <summary>
    /// GLControl guard flag.
    /// </summary>
    bool loaded = false;

    /// <summary>
    /// Associated Trackball instance.
    /// </summary>
    Trackball tb = null;

    ToolTip tt = new ToolTip();

    public Form1 ()
    {
      InitializeComponent();

      string param;
      string name;
      Construction.InitParams( out param, out name );
      textParam.Text = param ?? "";
      Text += " (" + rev + ") '" + name + '\'';

      // Trackball:
      tb = new Trackball( center, diameter );

      InitShaderRepository();
    }

    private void glControl1_Load ( object sender, EventArgs e )
    {
      InitOpenGL();
      UpdateParams( textParam.Text );
      tb.GLsetupViewport( glControl1.Width, glControl1.Height, near, far );

      loaded = true;
      Application.Idle += new EventHandler( Application_Idle );
    }

    private void glControl1_Resize ( object sender, EventArgs e )
    {
      if ( !loaded ) return;

      tb.GLsetupViewport( glControl1.Width, glControl1.Height, near, far );
      glControl1.Invalidate();
    }

    private void glControl1_Paint ( object sender, PaintEventArgs e )
    {
      Render();
    }

    private void checkVsync_CheckedChanged ( object sender, EventArgs e )
    {
      glControl1.VSync = checkVsync.Checked;
    }

    private void buttonOpen_Click ( object sender, EventArgs e )
    {
      OpenFileDialog ofd = new OpenFileDialog();

      ofd.Title = "Open Scene File";
      ofd.Filter = "Wavefront OBJ Files|*.obj;*.obj.gz" +
                   "|All scene types|*.obj";

      ofd.FilterIndex = 1;
      ofd.FileName = "";
      if ( ofd.ShowDialog() != DialogResult.OK )
        return;

      WavefrontObj objReader = new WavefrontObj();
      objReader.MirrorConversion = false;

      int faces = objReader.ReadBrep( ofd.FileName, scene );

      scene.BuildCornerTable();
      diameter = scene.GetDiameter( out center );
      scene.GenerateColors( 12 );
      UpdateParams( textParam.Text );
      tb.Center   = center;
      tb.Diameter = diameter;
      SetLight( diameter, ref light );
      tb.Reset();

      labelFile.Text = string.Format( "{0}: {1} faces", ofd.SafeFileName, faces );
      PrepareDataBuffers();
      glControl1.Invalidate();
    }

    private void buttonLoadTexture_Click ( object sender, EventArgs e )
    {
      OpenFileDialog ofd = new OpenFileDialog();

      ofd.Title = "Open Image File";
      ofd.Filter = "Bitmap Files|*.bmp" +
          "|Gif Files|*.gif" +
          "|JPEG Files|*.jpg" +
          "|PNG Files|*.png" +
          "|TIFF Files|*.tif" +
          "|All image types|*.bmp;*.gif;*.jpg;*.png;*.tif";

      ofd.FilterIndex = 6;
      ofd.FileName = "";
      if ( ofd.ShowDialog() != DialogResult.OK )
        return;

      Bitmap inputImage = (Bitmap)Image.FromFile( ofd.FileName );

      if ( texName == 0 )
        texName = GL.GenTexture();

      GL.BindTexture( TextureTarget.Texture2D, texName );

      Rectangle rect = new Rectangle( 0, 0, inputImage.Width, inputImage.Height );
      BitmapData bmpData = inputImage.LockBits( rect, ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb );
      GL.TexImage2D( TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, inputImage.Width, inputImage.Height, 0,
                     OpenTK.Graphics.OpenGL.PixelFormat.Bgr, PixelType.UnsignedByte, bmpData.Scan0 );
      inputImage.UnlockBits( bmpData );
      inputImage.Dispose();
      inputImage = null;

      GL.TexParameter( TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat );
      GL.TexParameter( TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat );
      GL.TexParameter( TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear );
      GL.TexParameter( TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMagFilter.Linear );
    }

    private void buttonGenerate_Click ( object sender, EventArgs e )
    {
      Cursor.Current = Cursors.WaitCursor;

      scene.Reset();
      Construction cn = new Construction();

      int faces = cn.AddMesh( scene, Matrix4.Identity, textParam.Text );
      diameter = scene.GetDiameter( out center );

      if ( checkMulti.Checked )
      {
        Matrix4 translation, rotation;

        Matrix4.CreateTranslation( diameter, 0.0f, 0.0f, out translation );
        Matrix4.CreateRotationX( 90.0f, out rotation );
        faces += cn.AddMesh( scene, translation * rotation, textParam.Text );

        Matrix4.CreateTranslation( 0.0f, diameter, 0.0f, out translation );
        faces += cn.AddMesh( scene, translation, textParam.Text );

        Matrix4.CreateTranslation( diameter, diameter, 0.0f, out translation );
        faces += cn.AddMesh( scene, translation, textParam.Text );

        Matrix4.CreateTranslation( 0.0f, 0.0f, diameter, out translation );
        faces += cn.AddMesh( scene, translation, textParam.Text );

        Matrix4.CreateTranslation( diameter, 0.0f, diameter, out translation );
        faces += cn.AddMesh( scene, translation, textParam.Text );

        Matrix4.CreateTranslation( 0.0f, diameter, diameter, out translation );
        faces += cn.AddMesh( scene, translation, textParam.Text );

        Matrix4.CreateTranslation( diameter, diameter, diameter, out translation );
        faces += cn.AddMesh( scene, translation, textParam.Text );

        diameter = scene.GetDiameter( out center );
      }

      scene.BuildCornerTable();
      int errors = scene.CheckCornerTable( null );
      scene.GenerateColors( 12 );
      UpdateParams( textParam.Text );
      tb.Center   = center;
      tb.Diameter = diameter;
      SetLight( diameter, ref light );
      tb.Reset();

      labelFile.Text = string.Format( "{0} faces ({1} rep), {2} errors", scene.Triangles, faces, errors );
      PrepareDataBuffers();
      glControl1.Invalidate();

      Cursor.Current = Cursors.Default;
    }

    private void textParam_KeyPress ( object sender, System.Windows.Forms.KeyPressEventArgs e )
    {
      if ( e.KeyChar == (char)Keys.Enter )
      {
        e.Handled = true;
        UpdateParams( textParam.Text );
      }
    }

    private void Form1_FormClosing ( object sender, FormClosingEventArgs e )
    {
      DestroyTexture( ref texName );

      if ( VBOid != null &&
           VBOid[ 0 ] != 0 )
      {
        GL.DeleteBuffers( 2, VBOid );
        VBOid = null;
      }

      DestroyShaders();
    }

    private void glControl1_MouseDown ( object sender, MouseEventArgs e )
    {
      if ( !tb.MouseDown( e ) )
        if ( checkAxes.Checked )
        {
          // pointing to the scene:
          pointOrigin = convertScreenToWorldCoords( e.X, e.Y, 0.0f );
          pointTarget = convertScreenToWorldCoords( e.X, e.Y, 1.0f );
          pointDirty = true;
        }
    }

    private void glControl1_MouseUp ( object sender, MouseEventArgs e )
    {
      tb.MouseUp( e );
    }

    private void glControl1_MouseMove ( object sender, MouseEventArgs e )
    {
      tb.MouseMove( e );
    }

    private void glControl1_MouseWheel ( object sender, MouseEventArgs e )
    {
      tb.MouseWheel( e );
    }

    private void glControl1_KeyDown ( object sender, KeyEventArgs e )
    {
      tb.KeyDown( e );
    }

    private void glControl1_KeyUp ( object sender, KeyEventArgs e )
    {
      if ( !tb.KeyUp( e ) )
        if ( e.KeyCode == Keys.F )
        {
          e.Handled = true;
          if ( frustumFrame.Count > 0 )
            frustumFrame.Clear();
          else
          {
            float N = 0.0f;
            float F = 1.0f;
            int R = glControl1.Width - 1;
            int B = glControl1.Height - 1;
            frustumFrame.Add( convertScreenToWorldCoords( 0, 0, N ) );
            frustumFrame.Add( convertScreenToWorldCoords( R, 0, N ) );
            frustumFrame.Add( convertScreenToWorldCoords( 0, B, N ) );
            frustumFrame.Add( convertScreenToWorldCoords( R, B, N ) );
            frustumFrame.Add( convertScreenToWorldCoords( 0, 0, F ) );
            frustumFrame.Add( convertScreenToWorldCoords( R, 0, F ) );
            frustumFrame.Add( convertScreenToWorldCoords( 0, B, F ) );
            frustumFrame.Add( convertScreenToWorldCoords( R, B, F ) );
          }
        }
    }

    private void buttonReset_Click ( object sender, EventArgs e )
    {
      tb.Reset();
    }

    private void buttonExportPly_Click ( object sender, EventArgs e )
    {
      if ( scene == null ||
           scene.Triangles < 1 ) return;

      SaveFileDialog sfd = new SaveFileDialog();
      sfd.Title = "Save PLY file";
      sfd.Filter = "PLY Files|*.ply";
      sfd.AddExtension = true;
      sfd.FileName = "";
      if ( sfd.ShowDialog() != DialogResult.OK )
        return;

      StanfordPly plyWriter   = new StanfordPly();
      plyWriter.TextFormat    = true;
      plyWriter.NativeNewLine = true;
      plyWriter.Orientation   = checkOrientation.Checked;
      //plyWriter.DoNormals     = false;
      //plyWriter.DoTxtCoords   = false;
      //plyWriter.DoColors      = false;
      using ( StreamWriter writer = new StreamWriter( new FileStream( sfd.FileName, FileMode.Create ) ) )
      {
        plyWriter.WriteBrep( writer, scene );
      }
    }

    private void labelFile_MouseHover ( object sender, EventArgs e )
    {
      tt.Show( Util.TargetFramework + " (" + Util.RunningFramework + "), OpenTK " + Util.AssemblyVersion( typeof( Vector3 ) ),
               (IWin32Window)sender, 10, -25, 4000 );
    }

    // Unproject support functions:

    public Vector3 convertScreenToWorldCoords ( int x, int y, float z =0.0f )
    {
      Matrix4 modelViewMatrix, projectionMatrix;
      GL.GetFloat( GetPName.ModelviewMatrix,  out modelViewMatrix );
      GL.GetFloat( GetPName.ProjectionMatrix, out projectionMatrix );

      Vector2 mouse;
      mouse.X = x;
      mouse.Y = glControl1.Height - y;
      Vector3 vector = UnProject( ref projectionMatrix, modelViewMatrix, new Size( glControl1.Width, glControl1.Height ), mouse, z );
      return vector;
    }

    public static Vector3 UnProject ( ref Matrix4 projection, Matrix4 view, Size viewport, Vector2 mouse, float z =0.0f )
    {
      Vector4 vec;
      vec.X = 2.0f * mouse.X / (float)viewport.Width  - 1;
      vec.Y = 2.0f * mouse.Y / (float)viewport.Height - 1;
      vec.Z = z;
      vec.W = 1.0f;

      Matrix4 viewInv = Matrix4.Invert( view );
      Matrix4 projInv = Matrix4.Invert( projection );

      Vector4.Transform( ref vec, ref projInv, out vec );
      Vector4.Transform( ref vec, ref viewInv, out vec );

      if ( vec.W > float.Epsilon ||
           vec.W < float.Epsilon )
      {
        vec.X /= vec.W;
        vec.Y /= vec.W;
        vec.Z /= vec.W;
        vec.W  = 1.0f;
      }

      return new Vector3( vec );
    }
  }
}
