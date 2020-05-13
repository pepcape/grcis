using MathSupport;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using OpenglSupport;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using _048rtmontecarlo.Properties;
using Utilities;

namespace Rendering
{
  public partial class RayVisualizerForm : Form
  {
    /// <summary>
    /// Scene center point.
    /// </summary>
    private readonly Vector3 center = Vector3.Zero;

    /// <summary>
    /// Scene diameter.
    /// </summary>
    private const float diameter = 4.0f;

    private const float near = 0.1f;
    private const float far = float.PositiveInfinity;

    /// <summary>
    /// GLControl guard flag.
    /// </summary>
    private bool loaded = false;

    /// <summary>
    /// Associated Trackball instance.
    /// </summary>
    internal Trackball trackBall = null;

    private Vector3? pointOrigin = null;
    private Vector3  pointTarget;

    private bool pointDirty = false;

    /// <summary>
    /// Can we use shaders?
    /// </summary>
    private bool canShaders = false;

    /// <summary>
    /// Are we currently using shaders?
    /// </summary>
    private bool useShaders = false;

    /// <summary>
    /// Global GLSL program repository.
    /// </summary>
    private readonly Dictionary<string, GlProgramInfo> programs = new Dictionary<string, GlProgramInfo> ();

    /// <summary>
    /// Current (active) GLSL program.
    /// </summary>
    private GlProgram activeProgram = null;

    // appearance:
    private Vector3 globalAmbient = new Vector3(0.3f, 0.3f, 0.3f);
    private Vector3 matAmbient    = new Vector3(0.8f, 0.6f, 0.2f);
    private Vector3 matDiffuse    = new Vector3(0.8f, 0.6f, 0.2f);
    private Vector3 matSpecular   = new Vector3(0.8f, 0.8f, 0.8f);
    private float   matShininess  = 100.0f;
    private Vector3 whiteLight    = new Vector3(1.0f, 1.0f, 1.0f);
    private Vector3 lightPosition = new Vector3(-20.0f, 10.0f, 10.0f);

    private long   lastFPSTime    = 0L;
    private int    frameCounter   = 0;
    private double lastFPS        = 0.0;

    private readonly Color defaultBackgroundColor = Color.Black;

    private readonly RayVisualizer rayVisualizer;

    private readonly List<int> allVBOs = new List<int>();

    private Action formClosedCallback;

    public RayVisualizerForm (RayVisualizer rayVisualizer, Action formClosedCallback)
    {
      trackBall = new Trackball(center, diameter);

      this.formClosedCallback = formClosedCallback;

      InitShaderRepository();

      this.rayVisualizer = rayVisualizer;
      rayVisualizer.form = this;

      Cursor.Current = Cursors.Default;

      InitializeComponent();

      if (AdditionalViews.singleton.pointCloud == null || AdditionalViews.singleton.pointCloud.IsCloudEmpty)
        PointCloudButton.Enabled = false;
      else
        PointCloudButton.Enabled = true;
    }

    private void glControl1_Load (object sender, EventArgs e)
    {
      InitializeOpenGL();

      trackBall.GLsetupViewport(glControl1.Width, glControl1.Height, near, far);

      loaded = true;

      Application.Idle += new EventHandler(Application_Idle);

      InitializeTextures();

      if (rayVisualizer.scene != null)
      {
        scene = rayVisualizer.scene;
        NewSceneVBOInitialization();
      }
    }

    private void glControl1_Resize (object sender, EventArgs e)
    {
      if (!loaded)
        return;

      trackBall.GLsetupViewport(glControl1.Width, glControl1.Height, near, far);

      glControl1.Invalidate();
    }

    private void glControl1_Paint (object sender, PaintEventArgs e)
    {
      Render();
    }

    private void RayVisualizerForm_FormClosing (object sender, FormClosingEventArgs e)
    {
      GL.DeleteBuffers(allVBOs.Count, allVBOs.ToArray());

      DestroyShaders();
    }

    /// <summary>
    /// Unproject support
    /// </summary>
    private Vector3 screenToWorld (int x, int y, float z = 0.0f)
    {
      GL.GetFloat(GetPName.ModelviewMatrix, out Matrix4 modelViewMatrix);
      GL.GetFloat(GetPName.ProjectionMatrix, out Matrix4 projectionMatrix);

      return Geometry.UnProject(ref projectionMatrix, ref modelViewMatrix, glControl1.Width, glControl1.Height, x,
                                  glControl1.Height - y, z);
    }

    private void glControl1_MouseDown (object sender, MouseEventArgs e)
    {
      if (!trackBall.MouseDown(e))
        if (checkAxes.Checked)
        {
          // pointing to the scene:
          pointOrigin = screenToWorld(e.X, e.Y, 0.0f);
          pointTarget = screenToWorld(e.X, e.Y, 1.0f);

          pointDirty = true;
        }
    }

    private void glControl1_MouseUp (object sender, MouseEventArgs e)
    {
      trackBall.MouseUp(e);
    }

    private void glControl1_MouseMove (object sender, MouseEventArgs e)
    {
      if (AllignCameraCheckBox.Checked && e.Button == trackBall.Button)
      {
        MessageBox.Show(@"You can not use mouse to rotate scene while ""Keep alligned"" box is checked.");
        return;
      }

      trackBall.MouseMove(e);
    }

    private void glControl1_MouseWheel (object sender, MouseEventArgs e)
    {
      trackBall.MouseWheel(e);
    }

    private void glControl1_KeyDown (object sender, KeyEventArgs e)
    {
      trackBall.KeyDown(e);
    }

    private void glControl1_KeyUp (object sender, KeyEventArgs e)
    {
      e.Handled = trackBall.KeyUp(e);
    }

    private void RayVisualizerForm_FormClosed (object sender, FormClosedEventArgs e)
    {
      formClosedCallback();

      rayVisualizer.form = null; // removes itself from associated RayVisualizer
    }

    /// <summary>
    /// Moves camera so that primary ray is perpendicular to screen
    /// </summary>
    /// <param name="sender">Not needed</param>
    /// <param name="e">Not needed</param>
    private void AllignCamera (object sender, EventArgs e)
    {
      if (rayVisualizer.rays.Count < 2)
        return;

      trackBall.Center = rayVisualizer.rays[1];
      trackBall.Reset(rayVisualizer.rays[1] - rayVisualizer.rays[0]);

      double distanceOfEye = Vector3.Distance(trackBall.Center, trackBall.Eye);
      double distanceOfCamera = Vector3.Distance(rayVisualizer.rays[1], rayVisualizer.rays[0]) * 0.9;

      trackBall.Zoom = (float)(distanceOfEye / distanceOfCamera);
    }

    private void InitializeOpenGL ()
    {
      // log OpenGL info
      GlInfo.LogGLProperties();

      // general OpenGL
      GL.Enable(EnableCap.DepthTest);
      GL.Enable(EnableCap.Blend);
      GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
      GL.ShadeModel(ShadingModel.Flat);

      // shaders
      canShaders = SetupShaders();

      InitializeAxesVBO();
      InitializeVideoCameraVBO();
      InitializeRaysVBO();
    }

    /// <summary>
    /// Init shaders registered in global repository 'programs'.
    /// </summary>
    /// <returns>True if succeeded.</returns>
    private bool SetupShaders ()
    {
      activeProgram = null;

      foreach (var programInfo in programs.Values)
        if (programInfo.Setup())
          activeProgram = programInfo.program;

      if (activeProgram == null)
        return false;

      if (programs.TryGetValue("default", out GlProgramInfo defInfo) && defInfo.program != null)
        activeProgram = defInfo.program;

      return true;
    }

    private void InitShaderRepository ()
    {
      programs.Clear();

      // default program:
      GlProgramInfo pi = new GlProgramInfo("default", new GlShaderInfo[]
      {
        new GlShaderInfo(ShaderType.VertexShader, "vertex.glsl", "048rtmontecarlo-script"),
        new GlShaderInfo(ShaderType.FragmentShader, "fragment.glsl", "048rtmontecarlo-script")
      } );

      programs[pi.name] = pi;
    }

    private void DestroyShaders ()
    {
      foreach (GlProgramInfo prg in programs.Values)
        prg.Destroy();
    }

    private int lightSourceTextureID;
    private int videoCameraTextureID;
    /// <summary>
    /// Initializes all needed textures from files and assigns associated texture IDs
    /// </summary>
    private void InitializeTextures ()
    {
      GL.Enable(EnableCap.Texture2D);

      lightSourceTextureID = LoadTexture(Resources.LightSource);
      videoCameraTextureID = LoadTexture(Resources.VideoCamera);
    }

    /// <summary>
    /// Loads texture from external image file, generates new OpenGL texture based on it and returns ID to this texture
    /// </summary>
    /// <param name="bitmap">Bitmap to use for texture</param>
    /// <returns>Texture ID to new texture</returns>
    private static int LoadTexture (Bitmap bitmap)
    {
      GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);

      GL.GenTextures(1, out int tex);
      GL.BindTexture(TextureTarget.Texture2D, tex);

      BitmapData data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                                        ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

      GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0,
                    OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
      bitmap.UnlockBits(data);


      GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
      GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
      GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
      GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

      return tex;
    }

    /// <summary>
    /// Enables/disables vertex attributes
    /// </summary>
    /// <param name="on">TRUE to enable; FALSE to disable</param>
    private void SetVertexAttrib (bool on)
    {
      if (activeProgram != null)
        if (on)
          activeProgram.EnableVertexAttribArrays();
        else
          activeProgram.DisableVertexAttribArrays();
    }

    /// <summary>
    /// Enables/disables vertex pointers
    /// </summary>
    /// <param name="on">TRUE to enable; FALSE to disable</param>
    private static void SetVertexPointer (bool on)
    {
      if (on)
      {
        GL.EnableClientState(ArrayCap.VertexArray);
        GL.EnableClientState(ArrayCap.TextureCoordArray);
        GL.EnableClientState(ArrayCap.NormalArray);
        GL.EnableClientState(ArrayCap.ColorArray);
      }
      else
      {
        GL.DisableClientState(ArrayCap.VertexArray);
        GL.DisableClientState(ArrayCap.TextureCoordArray);
        GL.DisableClientState(ArrayCap.NormalArray);
        GL.DisableClientState(ArrayCap.ColorArray);
      }
    }

    /// <summary>
    /// Preparations before rendering scene itself
    /// Called every frame
    /// </summary>
    private void Render ()
    {
      if (!loaded)
        return;

      Color backgroundColor;

      if (rayVisualizer.backgroundColor == null)
        backgroundColor = defaultBackgroundColor;
      else
        backgroundColor = Color.FromArgb((rayVisualizer.backgroundColor[0]),
                                           (rayVisualizer.backgroundColor[1]),
                                           (rayVisualizer.backgroundColor[2]));


      GL.ClearColor(backgroundColor);

      frameCounter++;
      useShaders = canShaders &&
                   activeProgram != null &&
                   checkShaders.Checked;

      GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
      GL.ShadeModel(checkSmooth.Checked ? ShadingModel.Smooth : ShadingModel.Flat);
      GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);

      glControl1.VSync = checkVsync.Checked;

      trackBall.GLsetCamera();

      if (!updatingScene)
        RenderScene();

      glControl1.SwapBuffers();
    }

    private void Application_Idle (object sender, EventArgs e)
    {
      if (glControl1.IsDisposed)
        return;

      while (glControl1.IsIdle)
      {
#if USE_INVALIDATE
        glControl1.Invalidate ();
#else
        glControl1.MakeCurrent();
        Render();
#endif

        long now = DateTime.Now.Ticks;
        if (now - lastFPSTime > 5000000) // more than 0.5 sec
        {
          lastFPS = 0.5 * lastFPS + 0.5 * (frameCounter * 1.0e7 / (now - lastFPSTime));
          lastFPSTime = now;
          frameCounter = 0;

          labelFPS.Text = string.Format(CultureInfo.InvariantCulture, "FPS: {0:f1}",
                                          lastFPS);
        }

        // pointing:
        if (pointOrigin != null &&
           pointDirty)
        {
          Vector3d p0 = new Vector3d(pointOrigin.Value.X, pointOrigin.Value.Y, pointOrigin.Value.Z);
          Vector3d p1 = new Vector3d(pointTarget.X, pointTarget.Y, pointTarget.Z) - p0;

          Vector3d ul   = new Vector3d(-1.0, -1.0, -1.0);
          Vector3d size = new Vector3d(2.0, 2.0, 2.0);

          pointDirty = false;
        }
      }
    }

    /// <summary>
    /// Rendering code itself (separated for clarity)
    /// </summary>
    private void RenderScene ()
    {
      if (useShaders)
      {
        bool renderFirst = true;

        if (AllignCameraCheckBox.Checked)
        {
          AllignCamera(null, null);
          renderFirst = false;
        }

        SetVertexPointer(false);
        SetVertexAttrib(true);

        // using GLSL shaders
        GL.UseProgram(activeProgram.Id);

        // uniforms
        Matrix4 modelView  = trackBall.ModelView;
        Matrix4 projection = trackBall.Projection;
        Vector3 localEye   = trackBall.Eye;

        GL.UniformMatrix4(activeProgram.GetUniform("matrixModelView"), false, ref modelView);
        GL.UniformMatrix4(activeProgram.GetUniform("matrixProjection"), false, ref projection);

        GL.Uniform3(activeProgram.GetUniform("globalAmbient"), ref globalAmbient);
        GL.Uniform3(activeProgram.GetUniform("lightColor"), ref whiteLight);
        GL.Uniform3(activeProgram.GetUniform("lightPosition"), ref lightPosition);
        GL.Uniform3(activeProgram.GetUniform("eyePosition"), ref localEye);
        GL.Uniform3(activeProgram.GetUniform("Ka"), ref matAmbient);
        GL.Uniform3(activeProgram.GetUniform("Kd"), ref matDiffuse);
        GL.Uniform3(activeProgram.GetUniform("Ks"), ref matSpecular);
        GL.Uniform1(activeProgram.GetUniform("shininess"), matShininess);


        // actual rendering
        if (NormalRaysCheckBox.Checked || ShadowRaysCheckBox.Checked)
          RenderRays(renderFirst);

        if (checkAxes.Checked)
          RenderAxes();

        if (pointCloudVBO != 0 && PointCloudCheckBox.Checked)
          RenderPointCloud();

        if (sceneObjects != null && sceneObjects.Count != 0 && BoundingBoxesCheckBox.Checked)
          RenderBoundingBoxes();

        if (scene?.Sources != null && LightSourcesCheckBox.Checked && lightSourcesVBO != 0)
          RenderLightSources();

        if (rayVisualizer.rays.Count != 0 && CameraCheckBox.Checked)
          RenderVideoCamera();

        // clean-up
        GL.UseProgram(0);
      }
      else
      {
        //throw new NotImplementedException ();
      }
    }

    /// <summary>
    /// Renders point cloud
    /// </summary>
    private void RenderPointCloud ()
    {
      SetVertexPointer(false);
      SetVertexAttrib(true);

      // color handling:
      bool useGlobalColor = checkGlobalColor.Checked;
      GL.Uniform1(activeProgram.GetUniform("globalColor"), useGlobalColor ? 1 : 0);

      // shading:
      GL.Uniform1(activeProgram.GetUniform("useTexture"), 0);
      GL.Uniform1(activeProgram.GetUniform("shadingPhong"), 1);
      GL.Uniform1(activeProgram.GetUniform("shadingGouraud"), 0);
      GL.Uniform1(activeProgram.GetUniform("useAmbient"), checkAmbient.Checked ? 1 : 0);
      GL.Uniform1(activeProgram.GetUniform("useDiffuse"), checkDiffuse.Checked ? 1 : 0);
      GL.Uniform1(activeProgram.GetUniform("useSpecular"), checkSpecular.Checked ? 1 : 0);
      GlInfo.LogError("set-uniforms");

      const int stride = 9 * sizeof(float);

      GL.BindBuffer(BufferTarget.ArrayBuffer, pointCloudVBO);

      // positions
      if (activeProgram.HasAttribute("position"))
        GL.VertexAttribPointer(activeProgram.GetAttribute("position"), 3, VertexAttribPointerType.Float, false, stride,
                               (IntPtr)0);

      // colors
      if (activeProgram.HasAttribute("color"))
        GL.VertexAttribPointer(activeProgram.GetAttribute("color"), 3, VertexAttribPointerType.Float, false, stride,
                               (IntPtr)(3 * sizeof(float)));

      // normals
      if (activeProgram.HasAttribute("normal"))
        GL.VertexAttribPointer(activeProgram.GetAttribute("normal"), 3, VertexAttribPointerType.Float, false, stride,
                               (IntPtr)(6 * sizeof(float)));

      // texture coordinates - ignored
      if (activeProgram.HasAttribute("texCoords"))
        GL.DisableVertexAttribArray(activeProgram.GetAttribute("texCoords"));

      GlInfo.LogError("set-attrib-pointers");

      // draw
      GL.DrawArrays(PrimitiveType.Points, 0, pointCloud.numberOfElements);
    }

    /// <summary>
    /// Renders 3 axes in origin
    /// </summary>
    private void RenderAxes ()
    {
      SetVertexPointer(false);
      SetVertexAttrib(true);

      // color handling
      GL.Uniform1(activeProgram.GetUniform("globalColor"), 0);

      // shading
      GL.Uniform1(activeProgram.GetUniform("useTexture"), 0);
      GL.Uniform1(activeProgram.GetUniform("shadingPhong"), 0);
      GL.Uniform1(activeProgram.GetUniform("shadingGouraud"), 0);
      GL.Uniform1(activeProgram.GetUniform("useAmbient"), 0);
      GL.Uniform1(activeProgram.GetUniform("useDiffuse"), 0);
      GL.Uniform1(activeProgram.GetUniform("useSpecular"), 0);
      GlInfo.LogError("set-uniforms");

      const int stride = 6 * sizeof(float);

      GL.BindBuffer(BufferTarget.ArrayBuffer, axesVBO);

      // positions
      if (activeProgram.HasAttribute("position"))
        GL.VertexAttribPointer(activeProgram.GetAttribute("position"), 3, VertexAttribPointerType.Float, false, stride,
                                 (IntPtr)0);

      // color
      if (activeProgram.HasAttribute("color"))
        GL.VertexAttribPointer(activeProgram.GetAttribute("color"), 3, VertexAttribPointerType.Float, false, stride,
                                 (IntPtr)(3 * sizeof(float)));

      // ignored attributes
      if (activeProgram.HasAttribute("texCoords"))
        GL.DisableVertexAttribArray(activeProgram.GetAttribute("texCoords"));
      if (activeProgram.HasAttribute("normal"))
        GL.DisableVertexAttribArray(activeProgram.GetAttribute("normal"));

      GlInfo.LogError("set-attrib-pointers");

      GL.LineWidth(4.0f);

      // draw
      GL.DrawArrays(PrimitiveType.Lines, 0, 6);
    }

    /// <summary>
    /// Renders all normal and shadow rays (further selection done via check boxes)
    /// </summary>
    /// <param name="renderFirst">FALSE if first ray should not be rendered (used when camera is perpendicular with this first ray)</param>
    private void RenderRays (bool renderFirst)
    {
      SetVertexPointer(false);
      SetVertexAttrib(true);

      // color handling
      GL.Uniform1(activeProgram.GetUniform("globalColor"), 0);

      // shading
      GL.Uniform1(activeProgram.GetUniform("useTexture"), 0);
      GL.Uniform1(activeProgram.GetUniform("shadingPhong"), 0);
      GL.Uniform1(activeProgram.GetUniform("shadingGouraud"), 0);
      GL.Uniform1(activeProgram.GetUniform("useAmbient"), 0);
      GL.Uniform1(activeProgram.GetUniform("useDiffuse"), 0);
      GL.Uniform1(activeProgram.GetUniform("useSpecular"), 0);
      GlInfo.LogError("set-uniforms");

      const int stride = 6 * sizeof(float);

      GL.BindBuffer(BufferTarget.ArrayBuffer, raysVBO);

      // positions
      if (activeProgram.HasAttribute("position"))
        GL.VertexAttribPointer(activeProgram.GetAttribute("position"), 3, VertexAttribPointerType.Float, false, stride,
                               (IntPtr)0);

      // color
      if (activeProgram.HasAttribute("color"))
        GL.VertexAttribPointer(activeProgram.GetAttribute("color"), 3, VertexAttribPointerType.Float, false, stride,
                               (IntPtr)(3 * sizeof(float)));

      // ignored attributes
      if (activeProgram.HasAttribute("texCoords"))
        GL.DisableVertexAttribArray(activeProgram.GetAttribute("texCoords"));
      if (activeProgram.HasAttribute("normal"))
        GL.DisableVertexAttribArray(activeProgram.GetAttribute("normal"));

      GlInfo.LogError("set-attrib-pointers");

      GL.LineWidth(3.0f);

      // draw
      lock (updatingRays)
      {
        if (NormalRaysCheckBox.Checked)
          if (renderFirst)
            GL.DrawArrays(PrimitiveType.Lines, 0, rays.Count);
          else
            GL.DrawArrays(PrimitiveType.Lines, 2, rays.Count - 2);

        if (ShadowRaysCheckBox.Checked)
          GL.DrawArrays(PrimitiveType.Lines, rays.Count, shadowRays.Count);
      }
    }

    /// <summary>
    /// Renders representation of camera (initially at position of rayOrigin of first primary ray)
    /// </summary>
    private void RenderVideoCamera ()
    {
      if (rays == null ||
          rays.Count == 0)
        return;

      SetVertexPointer(false);
      SetVertexAttrib(true);

      // billboarding
      Matrix4 translation = Matrix4.CreateTranslation(rays [0]);
      Matrix4 modelView  = trackBall.ModelView;

      modelView = translation * modelView;
      modelView = ApplyBillboarding(modelView);

      GL.UniformMatrix4(activeProgram.GetUniform("matrixModelView"), false, ref modelView);

      // color handling:
      GL.Uniform1(activeProgram.GetUniform("globalColor"), 0);

      // shading:
      GL.Uniform1(activeProgram.GetUniform("useTexture"), 1);
      GL.Uniform1(activeProgram.GetUniform("shadingPhong"), 0);
      GL.Uniform1(activeProgram.GetUniform("shadingGouraud"), 0);
      GL.Uniform1(activeProgram.GetUniform("useAmbient"), 0);
      GL.Uniform1(activeProgram.GetUniform("useDiffuse"), 0);
      GL.Uniform1(activeProgram.GetUniform("useSpecular"), 0);
      GlInfo.LogError("set-uniforms");

      const int stride = 5 * sizeof(float);

      GL.BindBuffer(BufferTarget.ArrayBuffer, videoCameraVBO);

      GL.BindTexture(TextureTarget.Texture2D, videoCameraTextureID);

      // positions
      if (activeProgram.HasAttribute("position"))
        GL.VertexAttribPointer(activeProgram.GetAttribute("position"), 3, VertexAttribPointerType.Float, false, stride,
                               (IntPtr)0);

      // texture coordinate
      if (activeProgram.HasAttribute("texCoords"))
        GL.VertexAttribPointer(activeProgram.GetAttribute("texCoords"), 2, VertexAttribPointerType.Float, false, stride,
                               (IntPtr)(3 * sizeof(float)));

      // ignored attributes
      if (activeProgram.HasAttribute("color"))
        GL.DisableVertexAttribArray(activeProgram.GetAttribute("color"));
      if (activeProgram.HasAttribute("normal"))
        GL.DisableVertexAttribArray(activeProgram.GetAttribute("normal"));

      GlInfo.LogError("set-attrib-pointers");

      // draw
      GL.DrawArrays(PrimitiveType.Quads, 0, 4);

      // reset back model view matrix
      modelView = trackBall.ModelView;
      GL.UniformMatrix4(activeProgram.GetUniform("matrixModelView"), false, ref modelView);
    }

    /// <summary>
    /// Applies billboarding effect to input matrix (should be model view matrix)
    /// Removes rotation and scale from this matrix
    /// Standard scale based on zoom is re-applied
    /// </summary>
    /// <param name="modelView">Input model view matrix which should be changed</param>
    /// <returns>Modified input model view matrix</returns>
    private Matrix4 ApplyBillboarding (Matrix4 modelView)
    {
      modelView[0, 0] = 1;
      modelView[0, 1] = 0;
      modelView[0, 2] = 0;

      modelView[1, 0] = 0;
      modelView[1, 1] = 1;
      modelView[1, 2] = 0;

      modelView[2, 0] = 0;
      modelView[2, 1] = 0;
      modelView[2, 2] = 1;

      modelView = Matrix4.CreateScale(trackBall.Zoom / diameter) * modelView;
      return modelView;
    }

    /// <summary>
    /// Renders representation of all light sources (except those in with null as position
    /// - usually ambient and directional lights which position does not matter)
    /// </summary>
    private void RenderLightSources ()
    {
      SetVertexPointer(false);
      SetVertexAttrib(true);

      // color handling
      GL.Uniform1(activeProgram.GetUniform("globalColor"), 0);

      // shading
      GL.Uniform1(activeProgram.GetUniform("useTexture"), 1);
      GL.Uniform1(activeProgram.GetUniform("shadingPhong"), 0);
      GL.Uniform1(activeProgram.GetUniform("shadingGouraud"), 0);
      GL.Uniform1(activeProgram.GetUniform("useAmbient"), 0);
      GL.Uniform1(activeProgram.GetUniform("useDiffuse"), 0);
      GL.Uniform1(activeProgram.GetUniform("useSpecular"), 0);
      GlInfo.LogError("set-uniforms");

      const int stride = 5 * sizeof(float);

      GL.BindBuffer(BufferTarget.ArrayBuffer, lightSourcesVBO);

      GL.BindTexture(TextureTarget.Texture2D, lightSourceTextureID);

      // positions
      if (activeProgram.HasAttribute("position"))
        GL.VertexAttribPointer(activeProgram.GetAttribute("position"), 3, VertexAttribPointerType.Float, false, stride,
                               (IntPtr)0);

      // texture coordinate
      if (activeProgram.HasAttribute("texCoords"))
        GL.VertexAttribPointer(activeProgram.GetAttribute("texCoords"), 2, VertexAttribPointerType.Float, false, stride,
                               (IntPtr)(3 * sizeof(float)));

      // ignored attributes
      if (activeProgram.HasAttribute("color"))
        GL.DisableVertexAttribArray(activeProgram.GetAttribute("color"));
      if (activeProgram.HasAttribute("normal"))
        GL.DisableVertexAttribArray(activeProgram.GetAttribute("normal"));

      GlInfo.LogError("set-attrib-pointers");

      Matrix4 modelView;

      foreach (ILightSource lightSource in scene.Sources)
      {
        if (lightSource.position == null)
          continue;

        // billboarding
        Matrix4 translation = Matrix4.CreateTranslation((Vector3) RayVisualizer.AxesCorrector(lightSource.position));
        modelView = trackBall.ModelView;

        modelView = translation * modelView;
        modelView = ApplyBillboarding(modelView);

        GL.UniformMatrix4(activeProgram.GetUniform("matrixModelView"), false, ref modelView);

        //draw
        GL.DrawArrays(PrimitiveType.Quads, 0, 4);
      }

      // reset back model view matrix
      modelView = trackBall.ModelView;
      GL.UniformMatrix4(activeProgram.GetUniform("matrixModelView"), false, ref modelView);
    }

    /// <summary>
    /// Renders representation of bounding boxes
    /// </summary>
    private void RenderBoundingBoxes ()
    {
      SetVertexPointer(false);
      SetVertexAttrib(true);

      GL.PolygonMode(MaterialFace.FrontAndBack, WireframeBoundingBoxesCheckBox.Checked ? PolygonMode.Line : PolygonMode.Fill);

      // color handling
      GL.Uniform1(activeProgram.GetUniform("globalColor"), 0);

      // shading
      GL.Uniform1(activeProgram.GetUniform("useTexture"), 0);
      GL.Uniform1(activeProgram.GetUniform("shadingPhong"), 0);
      GL.Uniform1(activeProgram.GetUniform("shadingGouraud"), 0);
      GL.Uniform1(activeProgram.GetUniform("useAmbient"), 0);
      GL.Uniform1(activeProgram.GetUniform("useDiffuse"), 0);
      GL.Uniform1(activeProgram.GetUniform("useSpecular"), 0);
      GlInfo.LogError("set-uniforms");

      const int stride = 6 * sizeof(float);

      GL.BindBuffer(BufferTarget.ArrayBuffer, boundingBoxesVBO);

      // positions
      if (activeProgram.HasAttribute("position"))
        GL.VertexAttribPointer(activeProgram.GetAttribute("position"), 3, VertexAttribPointerType.Float, false, stride,
                               (IntPtr)0);

      // color
      if (activeProgram.HasAttribute("color"))
        GL.VertexAttribPointer(activeProgram.GetAttribute("color"), 3, VertexAttribPointerType.Float, false, stride,
                               (IntPtr)(3 * sizeof(float)));

      // ignored attributes
      if (activeProgram.HasAttribute("texCoords"))
        GL.DisableVertexAttribArray(activeProgram.GetAttribute("texCoords"));
      if (activeProgram.HasAttribute("normal"))
        GL.DisableVertexAttribArray(activeProgram.GetAttribute("normal"));

      GlInfo.LogError("set-attrib-pointers");

      GL.DrawArrays(PrimitiveType.Quads, 0, boundingBoxesQuadsCount * 4);

      GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
    }

    /// <summary>
    /// Gets array of vertices and colors (X Y Z R G B) for cuboid denoted by 2 corners and transofrmation
    /// </summary>
    /// <param name="corner1">Position of first corner</param>
    /// <param name="corner2">Position of corner opposite to first corner</param>
    /// <param name="transformation">4x4 transformation matrix</param>
    /// <param name="color">Uniform color of cuboid</param>
    /// <returns>Array of vertices and colors (X Y Z R G B) stored as Vector3s for specified cuboid</returns>
    private Vector3[] GetCuboid (Vector3d corner1, Vector3d corner2, Matrix4d transformation, Vector3 color)
    {
      Vector3 c1 = (Vector3)corner1;
      Vector3 c2 = (Vector3)corner2;

      float[] cubeVertices =
      {
        // Top
        c2.X, c2.Y, c1.Z, // Top Right Of The Quad
        c1.X, c2.Y, c1.Z, // Top Left Of The Qua
        c1.X, c2.Y, c2.Z, // Bottom Left Of The Quad
        c2.X, c2.Y, c2.Z, // Bottom Right Of The Quad

        //Bottom
        c2.X, c1.Y, c2.Z, // Top Right Of The Quad
        c1.X, c1.Y, c2.Z, // Top Left Of The Quad
        c1.X, c1.Y, c1.Z, // Bottom Left Of The Quad
        c2.X, c1.Y, c1.Z, // Bottom Right Of The Quad

        // Front
        c2.X, c2.Y, c2.Z, // Top Right Of The Quad
        c1.X, c2.Y, c2.Z, // Top Left Of The Quad
        c1.X, c1.Y, c2.Z, // Bottom Left Of The Quad
        c2.X, c1.Y, c2.Z, // Bottom Right Of The Quad

        // Back
        c2.X, c1.Y, c1.Z, // Bottom Left Of The Quad
        c1.X, c1.Y, c1.Z, // Bottom Right Of The Quad
        c1.X, c2.Y, c1.Z, // Top Right Of The Quad
        c2.X, c2.Y, c1.Z, // Top Left Of The Quad

        // Left
        c1.X, c2.Y, c2.Z, // Top Right Of The Quad
        c1.X, c2.Y, c1.Z, // Top Left Of The Quad
        c1.X, c1.Y, c1.Z, // Bottom Left Of The Quad
        c1.X, c1.Y, c2.Z, // Bottom Right Of The Quad

        // Right
        c2.X, c2.Y, c1.Z, // Top Right Of The Quad
        c2.X, c2.Y, c2.Z, // Top Left Of The Quad
        c2.X, c1.Y, c2.Z, // Bottom Left Of The Quad
        c2.X, c1.Y, c1.Z, // Bottom Right Of The Quad
      };

      Vector3[] verticesPositionAndColor = new Vector3[cubeVertices.Length / 3 * 2];

      for (int i = 0; i < verticesPositionAndColor.Length; i += 2)
      {
        verticesPositionAndColor[i] = (Vector3)RayVisualizer.AxesCorrector(Transformations.ApplyTransformation(new Vector3d(cubeVertices[i / 2 * 3],
                                                                                                                                  cubeVertices[i / 2 * 3 + 1],
                                                                                                                                  cubeVertices
                                                                                                                                    [i / 2 * 3 + 2]),
                                                                                                                   transformation));
        verticesPositionAndColor[i + 1] = new Vector3(color);
      }

      return verticesPositionAndColor;
    }

    /// <summary>
    /// Linearly generates shade of gray between approximately [0.25, 0.25, 0.25] and [0.75, 0.75, 0.75] (in 0 to 1 for RGB channels)
    /// Closer the currentValue is to the maxValue, closer is returned color to [0.75, 0.75, 0.75]
    /// </summary>
    /// <param name="currentValue">Indicates current position between 0 and maxValue</param>
    /// <param name="maxValue">Max value currentValue can have</param>
    /// <returns>Gray color between approximately [0.25, 0.25, 0.25] and [0.75, 0.75, 0.75]</returns>
    private static Vector3 GenerateColor (int currentValue, int maxValue)
    {
      Arith.Clamp(currentValue, 0, maxValue);

      int colorValue = Util.Clamp((int)((currentValue / (double)maxValue / 1.333 + 0.25) * 255), 0, 255);

      return new Vector3(colorValue, colorValue, colorValue);
    }

    private List<SceneObject> sceneObjects;
    private IRayScene scene;
    /// <summary>
    /// Fills sceneObjects list with objects from current scene
    /// </summary>
    private void FillSceneObjects ()
    {
      if (!(scene.Intersectable is DefaultSceneNode root))
      {
        sceneObjects = null;
        return;
      }

      sceneObjects = new List<SceneObject>();

      Matrix4d transformation = Matrix4d.Identity;

      double[] color;

      if ((double[])root.GetAttribute(PropertyName.COLOR) != null)
        color = (double[])root.GetAttribute(PropertyName.COLOR);
      else
        color = ((IMaterial)root.GetAttribute(PropertyName.MATERIAL)).Color;

      EvaluateSceneNode(root, transformation, color);
    }

    /// <summary>
    /// Recursively goes through all children nodes of parent
    /// If child is ISolid, adds a new object to sceneObjects based on it
    /// If child is another CSGInnerNode, recursively goes through its children
    /// Meanwhile counts transformation matrix for SceneObjects
    /// </summary>
    /// <param name="parent">Parent node</param>
    /// <param name="transformation">Transofrmation matrix from parent node</param>
    /// <param name="color">Color from parent node</param>
    private void EvaluateSceneNode (DefaultSceneNode parent, Matrix4d transformation, double[] color)
    {
      foreach (ISceneNode sceneNode in parent.Children)
      {
        ISceneNode children = (DefaultSceneNode) sceneNode;
        Matrix4d localTransformation = children.ToParent * transformation;


        double[] newColor;

        // take color from child's attribute COLOR, child's attribute MATERIAL.Color or from parent
        if ((double[])children.GetAttribute(PropertyName.COLOR) == null)
          newColor = ((IMaterial)children.GetAttribute(PropertyName.MATERIAL)).Color ?? color;
        else
          newColor = (double[])children.GetAttribute(PropertyName.COLOR);


        switch (children)
        {
          case ISolid solid:
            sceneObjects.Add(new SceneObject(solid, localTransformation, newColor));
            continue;
          case CSGInnerNode node:
            EvaluateSceneNode(node, localTransformation, newColor);
            break;
        }
      }
    }

    /// <summary>
    /// Data class containing info about ISolid and its absolute position in scene (through transformation matrix)
    /// </summary>
    private class SceneObject
    {
      public readonly ISolid solid;
      public readonly Matrix4d transformation;
      public readonly double[] color;

      public SceneObject (ISolid solid, Matrix4d transformation, double[] color)
      {
        this.solid = solid;
        this.transformation = transformation;
        this.color = color;
      }
    }

    private bool updatingScene;
    private readonly object updatingRays = new object();
    /// <summary>
    /// Called when scene has changed
    /// Sets new light sources and sets GUI (must be invoked if called from different thread)
    /// </summary>
    /// <param name="newScene">New IRayScene</param>
    public void UpdateScene (IRayScene newScene)
    {
      scene = newScene;

      NewSceneVBOInitialization();

      PointCloudButton.Enabled = false;
      PointCloudCheckBox.Enabled = false;
      PointCloudCheckBox.Checked = false;

      BoundingBoxesCheckBox.Checked = true;
      WireframeBoundingBoxesCheckBox.Enabled = true;
    }

    /// <summary>
    /// Should be called every time new scene is about to be rendered
    /// Prepares all needed VBOs for a new scene (lights and bounding boxes)
    /// </summary>
    private void NewSceneVBOInitialization ()
    {
      updatingScene = true;

      InitializeLightSourcesVBO();

      FillSceneObjects();
      InitializeBoundingBoxesVBO();

      updatingScene = false;
    }

    private void BoundingBoxesCheckBox_CheckedChanged (object sender, EventArgs e)
    {
      WireframeBoundingBoxesCheckBox.Enabled = BoundingBoxesCheckBox.Checked;
    }

    private PointCloud pointCloud;
    private int pointCloudVBO;
    /// <summary>
    /// Gets reference to point cloud, calls initialization of VBO for point cloud and changes GUI elements respectively
    /// </summary>
    /// <param name="sender">Not needed</param>
    /// <param name="e">Not needed</param>
    private void PointCloudButton_Click (object sender, EventArgs e)
    {
      pointCloud = AdditionalViews.singleton.pointCloud;

      if (pointCloud.cloud == null || pointCloud.IsCloudEmpty)
        return;

      InitializePointCloudVBO();

      PointCloudCheckBox.Enabled = true;
      PointCloudCheckBox.Checked = true;
    }

    /// <summary>
    /// Initializes Vertex Buffer Object (VBO) for point cloud
    /// X, Y, Z, R, G, B, nX, nY, nZ
    /// Expected is number of points in small millions
    /// No attributes are expected to change
    /// </summary>
    private void InitializePointCloudVBO ()
    {
      GenerateAndBindVBO(ref pointCloudVBO);

      GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(pointCloud.numberOfElements * 9 * sizeof(float)), IntPtr.Zero, BufferUsageHint.StaticDraw);

      int currentLength = 0;

      foreach (List<float> list in pointCloud.cloud)
      {
        GL.BufferSubData(BufferTarget.ArrayBuffer, (IntPtr)currentLength, (IntPtr)(list.Count * sizeof(float)) - 1, list.ToArray());
        currentLength += list.Count * sizeof(float);
      }

      BoundingBoxesCheckBox.Checked = false;
      WireframeBoundingBoxesCheckBox.Enabled = false;
    }

    private int lightSourcesVBO;
    private const float lightSize = 1.0f;
    /// <summary>
    /// Initializes VBO for light sources
    /// X, Y, Z, u, v
    /// 1 quad total - this VBO is expected to be called for each light with different view model matrices
    /// No attributes are expected to change
    /// </summary>
    private void InitializeLightSourcesVBO ()
    {
      GenerateAndBindVBO(ref lightSourcesVBO);

      float[] billboard =
      {
        // positions                            // texture coords
        lightSize / 2, lightSize / 2, 0.0f,     1.0f, 0.0f,
        lightSize / 2, -lightSize / 2, 0.0f,    1.0f, 1.0f,
        -lightSize / 2, -lightSize / 2, 0.0f,   0.0f, 1.0f,
        -lightSize / 2, lightSize / 2, 0.0f,    0.0f, 0.0f,
      };

      GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(billboard.Length * sizeof(float)), billboard, BufferUsageHint.StaticDraw);
    }

    private int axesVBO;
    /// <summary>
    /// Initializes VBO for axes
    /// Vector3s is form of: start of line (XYZ) - color if start(RGB) - end of line(XYZ) - color of end(RGB)
    /// 6 points = 3 lines in total
    /// No attributes are expected to change - center is in (0, 0, 0)
    /// </summary>
    private void InitializeAxesVBO ()
    {
      GenerateAndBindVBO(ref axesVBO);

      Vector3[] points =
      {
        // start of line (XYZ) - color if start (RGB) - end of line (XYZ) - color of end (RGB)
        center, new Vector3(1.0f, 0.1f, 0.1f), center + new Vector3(1.5f, 0.0f, 0.0f) * diameter, new Vector3(1.0f, 0.1f, 0.1f), // Red axis
        center, new Vector3(0.0f, 1.0f, 0.0f), center + new Vector3(0.0f, 1.5f, 0.0f) * diameter, new Vector3(0.0f, 1.0f, 0.0f), // Green axis
        center, new Vector3(0.2f, 0.2f, 1.0f), center + new Vector3(0.0f, 0.0f, 1.5f) * diameter, new Vector3(0.2f, 0.2f, 1.0f), // Blue axis
      };

      GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(points.Length * 3 * sizeof(float)), points, BufferUsageHint.StaticDraw);
    }

    private int videoCameraVBO;
    private const float videoCameraSize = 1.0f;
    /// <summary>
    /// Initializes VBO for Video Camera
    /// X, Y, Z, u, v
    /// 1 quad total
    /// No attributes are expected to change
    /// </summary>
    private void InitializeVideoCameraVBO ()
    {
      GenerateAndBindVBO(ref videoCameraVBO);

      float[] billboard =
      {
        // positions                                        // texture coords
        videoCameraSize / 2, videoCameraSize / 2, 0.0f,     1.0f, 0.0f,
        videoCameraSize / 2, -videoCameraSize / 2, 0.0f,    1.0f, 1.0f,
        -videoCameraSize / 2, -videoCameraSize / 2, 0.0f,   0.0f, 1.0f,
        -videoCameraSize / 2, videoCameraSize / 2, 0.0f,    0.0f, 0.0f,
      };

      GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(billboard.Length * sizeof(float)), billboard, BufferUsageHint.DynamicDraw);
    }

    private List<Vector3> rays = new List<Vector3>();
    private List<Vector3> shadowRays = new List<Vector3>();
    private readonly Vector3 rayColor = new Vector3(1.0f, 0.0f, 0.0f);
    private readonly Vector3 shadowRayColor = new Vector3(1.0f, 1.0f, 0.0f);
    private int raysVBO;

    /// <summary>
    /// Initializes VBO for normal and shadow rays
    /// X, Y, Z, R, G, B
    /// 2 points for each ray (start and end of line)
    /// VBO is allocated once with enough memory for over 20 000 rays and rewritten with UpdateRaysVBO
    /// Normal rays have different color than shadow rays
    /// </summary>
    private void InitializeRaysVBO ()
    {
      GenerateAndBindVBO(ref raysVBO);

      GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(1000 * 1000), IntPtr.Zero, BufferUsageHint.StaticDraw); // 1 MB is enough for over 20 000 rays at once
    }

    /// <summary>
    /// Updates VBO for normal and shadow rays
    /// X, Y, Z, R, G, B
    /// 2 points for each ray (start and end of line)
    /// Normal rays have different color than shadow rays
    /// </summary>
    /// <param name="newRays">List of Vector3 denoting position of normal rays - both start and end of each ray</param>
    /// <param name="newShadowRays">List of Vector3 denoting position of shadow rays - both start and end of each shadow ray</param>
    public void UpdateRaysVBO (List<Vector3> newRays, List<Vector3> newShadowRays)
    {
      rays = newRays;
      shadowRays = newShadowRays;

      Vector3[] temp = new Vector3[(newRays.Count + newShadowRays.Count) * 2];

      for (int i = 0; i < newRays.Count; i++)
      {
        temp[i * 2]     = newRays[i];
        temp[i * 2 + 1] = rayColor;
      }

      for (int i = 0; i < newShadowRays.Count; i++)
      {
        temp[newRays.Count * 2 + i * 2]     = newShadowRays[i];
        temp[newRays.Count * 2 + i * 2 + 1] = shadowRayColor;
      }

      int size = (newRays.Count + newShadowRays.Count) * 3 * 2 * sizeof(float);

      lock (updatingRays)
      {
        GL.BindBuffer(BufferTarget.ArrayBuffer, raysVBO);

        GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, (IntPtr)size, temp);
      }
    }

    private int boundingBoxesVBO;
    private int boundingBoxesQuadsCount;

    /// <summary>
    /// Initializes VBO for bounding boxes
    /// No attributes are expected to change
    /// </summary>
    private void InitializeBoundingBoxesVBO ()
    {
      GenerateAndBindVBO(ref boundingBoxesVBO);

      List<Vector3> toBeBuffer = new List<Vector3>(sceneObjects.Count * 48);

      int index = 0;

      foreach (SceneObject sceneObject in sceneObjects)
      {
        Vector3 color;

        if (sceneObject.color != null)
          color = new Vector3((float)sceneObject.color[0], (float)sceneObject.color[1], (float)sceneObject.color[2]);
        else
          color = GenerateColor(index, sceneObjects.Count);

        if (sceneObject.solid is Plane plane)
        {
          plane.GetBoundingBox(out Vector3d c1, out Vector3d c2);

          Vector3[] verticesAndColors = new Vector3[]
          {
            (Vector3) RayVisualizer.AxesCorrector(Transformations.ApplyTransformation(new Vector3d(c1.X, c1.Y, c2.Z), sceneObject.transformation)), color,
            (Vector3) RayVisualizer.AxesCorrector(Transformations.ApplyTransformation(new Vector3d(c2.X, c1.Y, c2.Z), sceneObject.transformation)), color,
            (Vector3) RayVisualizer.AxesCorrector(Transformations.ApplyTransformation(new Vector3d(c2.X, c2.Y, c1.Z), sceneObject.transformation)), color,
            (Vector3) RayVisualizer.AxesCorrector(Transformations.ApplyTransformation(new Vector3d(c1.X, c2.Y, c1.Z), sceneObject.transformation)), color,
          };

          toBeBuffer.AddRange(verticesAndColors);
        }
        else
        {
          sceneObject.solid.GetBoundingBox(out Vector3d c1, out Vector3d c2);

          toBeBuffer.AddRange(GetCuboid(c1, c2, sceneObject.transformation, color));
        }

        index++;
      }

      boundingBoxesQuadsCount = toBeBuffer.Count / 4 / 2;


      GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(toBeBuffer.Count * 3 * sizeof(float)), toBeBuffer.ToArray(), BufferUsageHint.StaticDraw);
    }

    /// <summary>
    /// Generates new buffer (previous one is deleted, if exists) and binds it as ArrayBuffer
    /// Manages allVBOs list
    /// </summary>
    /// <param name="vbo"></param>
    private void GenerateAndBindVBO (ref int vbo)
    {
      if (vbo != 0)
      {
        GL.DeleteBuffer(vbo);
        allVBOs.Remove(vbo);
      }

      vbo = GL.GenBuffer();
      allVBOs.Add(vbo);
      GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
    }

    /// <summary>
    /// Takes screenshot of OpenGL window, opens save file dialog window and save screenshot as .PNG image
    /// </summary>
    /// <param name="sender">Not needed</param>
    /// <param name="e">Not needed</param>
    private void SaveScreenshotButton_Click (object sender, EventArgs e)
    {
      Image outputImage = Snapshots.TakeScreenshot(glControl1);

      if (outputImage == null)
        return;

      SaveFileDialog sfd = new SaveFileDialog
      {
        Title        = @"Save PNG file",
        Filter       = @"PNG Files|*.png",
        AddExtension = true,
        FileName     = "Screenshot"
      };
      if (sfd.ShowDialog() != DialogResult.OK)
        return;

      outputImage.Save(sfd.FileName, ImageFormat.Png);
    }
  }
}
