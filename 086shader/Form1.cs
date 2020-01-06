using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
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
    static readonly string rev = Util.SetVersion("$Rev$");

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

    float near =   0.05f;     // 5cm
    float far  = 200.0f;      // 200m

    /// <summary>
    /// Light source position in the world-space.
    /// </summary>
    Vector3 light = new Vector3(-2, 1, 1);

    /// <summary>
    /// Point in the 3D scene pointed out by an user, or null.
    /// </summary>
    Vector3? spot = null;

    Vector3? pointOrigin = null;
    Vector3 pointTarget;
    Vector3 eye;

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
    /// Current dynamic camera object.
    /// </summary>
    IDynamicCamera cam = null;

    /// <summary>
    /// Associated Trackball instance.
    /// </summary>
    Trackball tb = null;

    /// <summary>
    /// Camera definition file-name.
    /// </summary>
    string cameraDefinition;

    /// <summary>
    /// Associated animation camera object.
    /// </summary>
    IDynamicCamera camera = null;

    /// <summary>
    /// Real-time = timeOrigin => time in the [MinTime, MaxTime] interal
    /// </summary>
    double timeOrigin = 0.0;

    bool animation = false;

    string modelStatus = "-- cube --";

    /// <summary>
    /// Param string tooltip = help.
    /// </summary>
    string tooltip = "";

    /// <summary>
    /// Shared ToolTip instance.
    /// </summary>
    ToolTip tt = new ToolTip();

    public Form1 ()
    {
      InitializeComponent();

      // I'm going to merge init data from Construction and AnimatedCamera.
      string cparam;
      string cname;
      string ctooltip;
      Construction.InitParams(out cname, out cparam, out ctooltip);

      string mparam;
      string mname;
      string mtooltip;
      AnimatedCamera.InitParams(out mname, out mparam, out mtooltip);

      tooltip = ctooltip + '\n' + mtooltip;
      textParam.Text = cparam + ", " + mparam;

      Text += " (" + rev + ") '" + cname + ',' + mname + '\'';

      // Default = Trackball.
      cam = tb = new Trackball(center, diameter);
      camera = new AnimatedCamera(textParam.Text);

      InitShaderRepository();
    }

    /// <summary>
    /// Current system time in seconds.
    /// </summary>
    private static double nowInSeconds ()
    {
      return DateTime.Now.Ticks / 10000000.0;
    }

    /// <summary>
    /// Viewport-change handling (both animation camera and Trackball must be notified).
    /// </summary>
    private void SetupViewport ()
    {
      tb.GLsetupViewport(glControl1.Width, glControl1.Height, near, far);
      camera.GLsetupViewport(glControl1.Width, glControl1.Height, near, far);
    }

    private void glControl1_Load (object sender, EventArgs e)
    {
      InitOpenGL();
      UpdateParams(textParam.Text);
      SetupViewport();

      loaded = true;
      Application.Idle += new EventHandler(Application_Idle);
    }

    private void glControl1_Resize (object sender, EventArgs e)
    {
      if (!loaded)
        return;

      SetupViewport();
      glControl1.Invalidate();
    }

    private void glControl1_Paint (object sender, PaintEventArgs e)
    {
      Render();
    }

    private void checkVsync_CheckedChanged (object sender, EventArgs e)
    {
      glControl1.VSync = checkVsync.Checked;
    }

    /// <summary>
    /// Sets the status line (behaves differently in animation and Trackball modes).
    /// </summary>
    private void SetStatus ()
    {
      if (checkAnimation.Checked)
        labelFile.Text = string.Format(CultureInfo.InvariantCulture, "{0:f2}s - {1:f2}s - {2:f2}s",
                                       cam.MinTime, cam.Time, cam.MaxTime);
      else
        labelFile.Text = modelStatus;
    }

    private void buttonOpen_Click (object sender, EventArgs e)
    {
      OpenFileDialog ofd = new OpenFileDialog();

      ofd.Title = "Open Scene File";
      ofd.Filter = "Wavefront OBJ Files|*.obj;*.obj.gz" +
                   "|All scene types|*.obj";

      ofd.FilterIndex = 1;
      ofd.FileName = "";
      if (ofd.ShowDialog() != DialogResult.OK)
        return;

      WavefrontObj objReader = new WavefrontObj();
      objReader.MirrorConversion = false;
      objReader.TextureUpsideDown = checkOrientation.Checked;
      objReader.ReadBrep(ofd.FileName, scene);

      // Scene postprocessing.
      scene.BuildCornerTable();
      scene.GenerateColors(12);
      scene.ComputeNormals();
      diameter = scene.GetDiameter(out center);

      // Viewport update.
      UpdateParams(textParam.Text);
      tb.Center   = center;
      tb.Diameter = diameter;
      tb.Reset();

      camera.Center   = center;
      camera.Diameter = diameter;
      camera.Reset();

      SetupViewport();
      SetLight(diameter, ref light);

      modelStatus = $"{ofd.SafeFileName}: {scene.Vertices}v, {scene.statEdges}e({scene.statShared}), {scene.Triangles + scene.Lines}f";
      labelFile.Text = modelStatus;

      // Prepare rendering system.
      PrepareDataBuffers();
      glControl1.Invalidate();
    }

    private void buttonLoadTexture_Click (object sender, EventArgs e)
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
      if (ofd.ShowDialog() != DialogResult.OK)
        return;

      Bitmap inputImage = (Bitmap)Image.FromFile(ofd.FileName);

      if (texName == 0)
        texName = GL.GenTexture();

      GL.BindTexture(TextureTarget.Texture2D, texName);

      Rectangle rect = new Rectangle(0, 0, inputImage.Width, inputImage.Height);
      BitmapData bmpData = inputImage.LockBits(rect, ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
      GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, inputImage.Width, inputImage.Height, 0,
                    OpenTK.Graphics.OpenGL.PixelFormat.Bgr, PixelType.UnsignedByte, bmpData.Scan0);
      inputImage.UnlockBits(bmpData);
      inputImage.Dispose();
      inputImage = null;

      GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
      GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
      GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
      GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMagFilter.Linear);
    }

    private void buttonGenerate_Click (object sender, EventArgs e)
    {
      bool doCheck = checkCorner.Checked;

      scene.Reset();

      Stopwatch sw = new Stopwatch();
      sw.Start();

      Construction cn = new Construction();

      int faces = cn.AddMesh(scene, Matrix4.Identity, textParam.Text);
      diameter = scene.GetDiameter(out center);

      if (checkMulti.Checked)
      {
        Matrix4 translation, rotation;

        Matrix4.CreateTranslation(diameter, 0.0f, 0.0f, out translation);
        Matrix4.CreateRotationX(90.0f, out rotation);
        faces += cn.AddMesh(scene, translation * rotation, textParam.Text);

        Matrix4.CreateTranslation(0.0f, diameter, 0.0f, out translation);
        faces += cn.AddMesh(scene, translation, textParam.Text);

        Matrix4.CreateTranslation(diameter, diameter, 0.0f, out translation);
        faces += cn.AddMesh(scene, translation, textParam.Text);

        Matrix4.CreateTranslation(0.0f, 0.0f, diameter, out translation);
        faces += cn.AddMesh(scene, translation, textParam.Text);

        Matrix4.CreateTranslation(diameter, 0.0f, diameter, out translation);
        faces += cn.AddMesh(scene, translation, textParam.Text);

        Matrix4.CreateTranslation(0.0f, diameter, diameter, out translation);
        faces += cn.AddMesh(scene, translation, textParam.Text);

        Matrix4.CreateTranslation(diameter, diameter, diameter, out translation);
        faces += cn.AddMesh(scene, translation, textParam.Text);

        diameter = scene.GetDiameter(out center);
      }

      sw.Stop();
      long elapsed = sw.ElapsedMilliseconds;

      // Scene postprocessing.
      scene.BuildCornerTable();
      int errors = doCheck ? scene.CheckCornerTable(null) : 0;
      scene.GenerateColors(12);

      // Viewport update.
      UpdateParams(textParam.Text);
      tb.Center   = center;
      tb.Diameter = diameter;
      tb.Reset();

      camera.Center   = center;
      camera.Diameter = diameter;
      camera.Reset();

      SetupViewport();
      SetLight(diameter, ref light);

      modelStatus = $"{scene.Triangles + scene.Lines}f ({faces}rep), {scene.Vertices}v, {errors}err, {elapsed}ms";
      labelFile.Text = modelStatus;

      // Prepare rendering system.
      PrepareDataBuffers();
      glControl1.Invalidate();
    }

    private void textParam_KeyPress (object sender, System.Windows.Forms.KeyPressEventArgs e)
    {
      if (e.KeyChar == (char)Keys.Enter)
      {
        e.Handled = true;
        UpdateParams(textParam.Text);
      }
    }

    private void Form1_FormClosing (object sender, FormClosingEventArgs e)
    {
      DestroyTexture(ref texName);

      if (VBOid != null &&
          VBOid[0] != 0)
      {
        GL.DeleteBuffers(2, VBOid);
        VBOid = null;
      }

      DestroyShaders();
    }

    /// <summary>
    /// Unproject support
    /// </summary>
    Vector3 screenToWorld (int x, int y, float z = 0.0f)
    {
      Matrix4 modelViewMatrix, projectionMatrix;
      GL.GetFloat(GetPName.ModelviewMatrix, out modelViewMatrix);
      GL.GetFloat(GetPName.ProjectionMatrix, out projectionMatrix);

      return Geometry.UnProject(ref projectionMatrix, ref modelViewMatrix, glControl1.Width, glControl1.Height, x, glControl1.Height - y, z);
    }

    private void glControl1_MouseDown (object sender, MouseEventArgs e)
    {
      if (!tb.MouseDown(e))
        if (checkAxes.Checked)
        {
          // pointing to the scene:
          pointOrigin = screenToWorld(e.X, e.Y, 0.0f);
          pointTarget = screenToWorld(e.X, e.Y, 1.0f);
          eye = cam.Eye;
          pointDirty = true;
        }
    }

    private void glControl1_MouseUp (object sender, MouseEventArgs e)
    {
      tb.MouseUp(e);
    }

    private void glControl1_MouseMove (object sender, MouseEventArgs e)
    {
      tb.MouseMove(e);
    }

    private void glControl1_MouseWheel (object sender, MouseEventArgs e)
    {
      tb.MouseWheel(e);
    }

    private void glControl1_KeyDown (object sender, KeyEventArgs e)
    {
      tb.KeyDown(e);
    }

    private void glControl1_KeyUp (object sender, KeyEventArgs e)
    {
      if (!tb.KeyUp(e))
        if (e.KeyCode == Keys.F)
        {
          e.Handled = true;
          if (frustumFrame.Count > 0)
            frustumFrame.Clear();
          else
          {
            float N = 0.0f;
            float F = 1.0f;
            int R = glControl1.Width - 1;
            int B = glControl1.Height - 1;
            frustumFrame.Add(screenToWorld(0, 0, N));
            frustumFrame.Add(screenToWorld(R, 0, N));
            frustumFrame.Add(screenToWorld(0, B, N));
            frustumFrame.Add(screenToWorld(R, B, N));
            frustumFrame.Add(screenToWorld(0, 0, F));
            frustumFrame.Add(screenToWorld(R, 0, F));
            frustumFrame.Add(screenToWorld(0, B, F));
            frustumFrame.Add(screenToWorld(R, B, F));
          }
        }
    }

    private void buttonExportPly_Click (object sender, EventArgs e)
    {
      if (scene == null ||
          scene.Triangles < 1)
        return;

      SaveFileDialog sfd = new SaveFileDialog();
      sfd.Title = "Save OBJ/PLY file";
      sfd.Filter = "OBJ Files|*.obj|PLY Files|*.ply";
      sfd.AddExtension = true;
      sfd.FileName = "";
      if (sfd.ShowDialog() != DialogResult.OK)
        return;

      if (sfd.FileName.EndsWith(".ply"))
      {
        // Stanford PLY format.
        StanfordPly plyWriter = new StanfordPly();
        plyWriter.TextFormat = true;
        plyWriter.NativeNewLine = true;
        plyWriter.Orientation = checkOrientation.Checked;
        //plyWriter.DoNormals     = false;
        //plyWriter.DoTxtCoords   = false;
        plyWriter.DoColors = false;
        using (StreamWriter writer = new StreamWriter(new FileStream(sfd.FileName, FileMode.Create)))
        {
          plyWriter.WriteBrep(writer, scene);
        }
      }
      else
      {
        // Wavefront OBJ format.
        WavefrontObj objWriter = new WavefrontObj();
        objWriter.MirrorConversion = true;
        using (StreamWriter writer = new StreamWriter(new FileStream(sfd.FileName, FileMode.Create)))
        {
          objWriter.WriteBrep(writer, scene);
        }
      }
    }

    private void labelFile_MouseHover (object sender, EventArgs e)
    {
      tt.Show(Util.TargetFramework + " (" + Util.RunningFramework + "), OpenTK " + Util.AssemblyVersion(typeof(Vector3)),
              (IWin32Window)sender, 10, -25, 4000);
    }

    private void textParam_MouseHover (object sender, EventArgs e)
    {
      tt.Show(tooltip, (IWin32Window)sender,
              10, -30 - 15 * Util.CharsInString(tooltip, '\n'), 4000);
    }

    private void buttonLoadCamera_Click (object sender, EventArgs e)
    {
      OpenFileDialog ofd = new OpenFileDialog();

      ofd.Title = "Open Camera Definition File";
      ofd.Filter = "Text Files|*.txt" +
                   "|All types|*.*";

      ofd.FilterIndex = 0;
      ofd.FileName = "";
      if (ofd.ShowDialog() != DialogResult.OK)
      {
        cameraDefinition = "";
        return;
      }

      cameraDefinition = ofd.FileName;
      if (camera != null)
        camera.Update(textParam.Text, cameraDefinition);
    }

    private void buttonReset_Click (object sender, EventArgs e)
    {
      cam.Center   = center;
      cam.Diameter = diameter;
      cam.Reset();
      cam.Update(textParam.Text, cameraDefinition);
      timeOrigin = nowInSeconds() - camera.Time;
    }

    private void buttonStartStop_Click (object sender, EventArgs e)
    {
      animation = !animation;

      if (animation)
      {
        // camera.Time should be valid now.
        timeOrigin = nowInSeconds() - camera.Time;

        buttonStartStop.Text = "Stop";
        buttonLoadCamera.Enabled = false;
      }
      else
      {
        buttonStartStop.Text = "Start";
        buttonLoadCamera.Enabled = true;
      }
    }

    private void checkAnimation_CheckedChanged (object sender, EventArgs e)
    {
      animation = false;

      cam = checkAnimation.Checked ? camera : tb;

      cam.Center   = center;
      cam.Diameter = diameter;
      cam.Reset();
      cam.Update(textParam.Text, cameraDefinition);

      SetStatus();

      // GUI.
      buttonStartStop.Text = "Start";
      buttonStartStop.Enabled = checkAnimation.Checked;
      buttonLoadCamera.Enabled = true;
    }
  }
}
