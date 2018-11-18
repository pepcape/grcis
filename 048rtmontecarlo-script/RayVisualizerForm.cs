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
using System.Linq;
using _048rtmontecarlo.Properties;

namespace Rendering
{
  public partial class RayVisualizerForm: Form
  {
    /// <summary>
    /// Scene center point.
    /// </summary>
    private Vector3 center = Vector3.Zero;

    /// <summary>
    /// Scene diameter.
    /// </summary>
    private float diameter = 4.0f;

    private const float near = 0.1f;
    private const float far = float.PositiveInfinity;

    private Vector3 light = new Vector3 ( -2, 1, 1 );

    /// <summary>
    /// GLControl guard flag.
    /// </summary>
    private bool loaded = false;

    /// <summary>
    /// Associated Trackball instance.
    /// </summary>
    internal Trackball trackBall = null;

    /// <summary>
    /// Frustum vertices, 0 or 8 vertices
    /// </summary>
    private readonly List<Vector3> frustumFrame = new List<Vector3> ();

    /// <summary>
    /// Point in the 3D scene pointed out by an user, or null.
    /// </summary>
    private Vector3? spot = null;

    private Vector3? pointOrigin = null;
    private Vector3  pointTarget;
    private Vector3  eye;

    private bool pointDirty = false;

    /// <summary>
    /// Are we allowed to use VBO?
    /// </summary>
    private bool useVBO = true;

    /// <summary>
    /// Can we use shaders?
    /// </summary>
    private bool canShaders = false;

    /// <summary>
    /// Are we currently using shaders?
    /// </summary>
    private bool useShaders = false;

    private uint[] VBOid  = null; // vertex array (colors, normals, coords), index array

    /// <summary>
    /// Global GLSL program repository.
    /// </summary>
    private Dictionary<string, GlProgramInfo> programs = new Dictionary<string, GlProgramInfo> ();

    /// <summary>
    /// Current (active) GLSL program.
    /// </summary>
    private GlProgram activeProgram = null;

    // appearance:
    private Vector3 globalAmbient = new Vector3 ( 0.3f, 0.3f, 0.3f );
    private Vector3 matAmbient    = new Vector3 ( 0.8f, 0.6f, 0.2f );
    private Vector3 matDiffuse    = new Vector3 ( 0.8f, 0.6f, 0.2f );
    private Vector3 matSpecular   = new Vector3 ( 0.8f, 0.8f, 0.8f );
    private float   matShininess  = 100.0f;
    private Vector3 whiteLight    = new Vector3 ( 1.0f, 1.0f, 1.0f );
    private Vector3 lightPosition = new Vector3 ( -20.0f, 10.0f, 10.0f );

    private long   lastFPSTime     = 0L;
    private int    frameCounter    = 0;
    private long   triangleCounter = 0L;
    private double lastFPS         = 0.0;
    private double lastTPS         = 0.0;

    private Color defaultBackgroundColor = Color.Black;

    private RayVisualizer rayVisualizer;

    public RayVisualizerForm ( RayVisualizer rayVisualizer )
    {
      InitializeComponent ();

      Form1.singleton.RayVisualiserButton.Enabled = false;

      trackBall = new Trackball ( center, diameter );

      InitShaderRepository ();      

      this.rayVisualizer = rayVisualizer;
      rayVisualizer.form = this;

      Cursor.Current = Cursors.Default;

      if ( AdditionalViews.singleton.pointCloud == null || AdditionalViews.singleton.pointCloud.IsCloudEmpty )
        PointCloudButton.Enabled = false;
      else
        PointCloudButton.Enabled = true;
    }

    private void glControl1_Load ( object sender, EventArgs e )
    {
      InitOpenGL ();

      trackBall.GLsetupViewport ( glControl1.Width, glControl1.Height, near, far );

      loaded = true;

      Application.Idle += new EventHandler ( Application_Idle );

      InitializeTextures ();
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

    private void RayVisualizerForm_FormClosing ( object sender, FormClosingEventArgs e )
    {
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
    private Vector3 screenToWorld ( int x, int y, float z = 0.0f )
    {
      GL.GetFloat ( GetPName.ModelviewMatrix, out Matrix4 modelViewMatrix );
      GL.GetFloat ( GetPName.ProjectionMatrix, out Matrix4 projectionMatrix );

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
        MessageBox.Show ( @"You can not use mouse to rotate scene while ""Keep alligned"" box is checked." );
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

    private void RayVisualizerForm_FormClosed ( object sender, FormClosedEventArgs e )
    {
      Form1.singleton.RayVisualiserButton.Enabled = true;

      rayVisualizer.form = null; // removes itself from associated RayVisualizer
    }

    /// <summary>
    /// Moves camera so that primary ray is perpendicular to screen
    /// </summary>
    /// <param name="sender">Not needed</param>
    /// <param name="e">Not needed</param>
    private void AllignCamera ( object sender, EventArgs e )
    {
      if ( rayVisualizer.rays.Count < 2 )
        return;

      trackBall.Center = (Vector3) rayVisualizer.rays [ 1 ];
      trackBall.Reset ( (Vector3) ( rayVisualizer.rays [ 1 ] - rayVisualizer.rays [ 0 ] ) );

      double distanceOfEye = Vector3.Distance ( trackBall.Center, trackBall.Eye );
      double distanceOfCamera = Vector3.Distance ( rayVisualizer.rays [ 1 ], rayVisualizer.rays [ 0 ]) * 0.9;

      trackBall.Zoom = (float) (distanceOfEye / distanceOfCamera);
    }

    private void InitOpenGL ()
		{
			// log OpenGL info
			GlInfo.LogGLProperties ();

			// general OpenGL
			GL.Enable ( EnableCap.DepthTest );
		  GL.Enable ( EnableCap.Blend );
		  GL.BlendFunc ( BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha );
      GL.ShadeModel ( ShadingModel.Flat );

			// VBO initialization
			VBOid = new uint[2];
			GL.GenBuffers ( 2, VBOid );
			useVBO = ( GL.GetError () == ErrorCode.NoError );

			// shaders
			if ( useVBO )
				canShaders = SetupShaders ();

		  InitializeAxesVBO ();
		  InitializeVideoCameraVBO ();

		}

		/// <summary>
		/// Init shaders registered in global repository 'programs'.
		/// </summary>
		/// <returns>True if succeeded.</returns>
		private bool SetupShaders ()
		{
			activeProgram = null;

			foreach ( var programInfo in programs.Values )
				if ( programInfo.Setup () )
					activeProgram = programInfo.program;

			if ( activeProgram == null )
				return false;

			if ( programs.TryGetValue ( "default", out GlProgramInfo defInfo ) && defInfo.program != null )
				activeProgram = defInfo.program;

			return true;
		}

		/// <summary>
		/// Set light-source coordinate in the world-space.
		/// </summary>
		/// <param name="size">Relative size (based on the scene size).</param>
		/// <param name="light">Relative light position (default=[-2,1,1],viewer=[0,0,1]).</param>
		private void SetLight ( float size, ref Vector3 light )
		{
			lightPosition = 2.0f * size * light;
		}

    private void InitShaderRepository ()
		{
			programs.Clear ();

      // default program:
		  GlProgramInfo pi = new GlProgramInfo ( "default", new GlShaderInfo[]
			{
		    new GlShaderInfo ( ShaderType.VertexShader, "vertex.glsl", "048rtmontecarlo-script" ),
		    new GlShaderInfo ( ShaderType.FragmentShader, "fragment.glsl", "048rtmontecarlo-script" )
			} );

			programs[pi.name] = pi;
		}

    private void DestroyShaders ()
		{
			foreach ( GlProgramInfo prg in programs.Values )
				prg.Destroy ();
		}

    private int lightSourceTextureID;
    private int videoCameraTextureID;
    /// <summary>
    /// Initializes all needed textures from files and assigns associated texture IDs
    /// </summary>
    private void InitializeTextures ()
    {
      GL.Enable ( EnableCap.Texture2D );

      lightSourceTextureID = LoadTexture ( Resources.LightSource );
      videoCameraTextureID = LoadTexture ( Resources.VideoCamera );
    }

    /// <summary>
    /// Loads texture from external image file, generates new OpenGL texture based on it and returns ID to this texture
    /// </summary>
    /// <param name="bitmap">Bitmap to use for texture</param>
    /// <returns>Texture ID to new texture</returns>
    private static int LoadTexture ( Bitmap bitmap )
    {
      GL.Hint ( HintTarget.PerspectiveCorrectionHint, HintMode.Nicest );

      GL.GenTextures ( 1, out int tex );
      GL.BindTexture ( TextureTarget.Texture2D, tex );

      BitmapData data = bitmap.LockBits ( new Rectangle ( 0, 0, bitmap.Width, bitmap.Height ),
                                          ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb );

      GL.TexImage2D ( TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0,
                      OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0 );
      bitmap.UnlockBits ( data );


      GL.TexParameter ( TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int) TextureMinFilter.Linear );
      GL.TexParameter ( TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int) TextureMagFilter.Linear );
      GL.TexParameter ( TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int) TextureWrapMode.Repeat );
      GL.TexParameter ( TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int) TextureWrapMode.Repeat );

      return tex;
    }

    /// <summary>
    /// Enables/disables vertex attributes
    /// </summary>
    /// <param name="on">TRUE to enable; FALSE to disable</param>
    private void SetVertexAttrib ( bool on )
    {
      if ( activeProgram != null )
        if ( on )
          activeProgram.EnableVertexAttribArrays ();
        else
          activeProgram.DisableVertexAttribArrays ();
    }

    /// <summary>
    /// Enables/disables vertex pointers
    /// </summary>
    /// <param name="on">TRUE to enable; FALSE to disable</param>
    private static void SetVertexPointer ( bool on )
    {
      if ( on )
      {
        GL.EnableClientState ( ArrayCap.VertexArray );
        GL.EnableClientState ( ArrayCap.TextureCoordArray );
        GL.EnableClientState ( ArrayCap.NormalArray );
        GL.EnableClientState ( ArrayCap.ColorArray );
      }
      else
      {
        GL.DisableClientState ( ArrayCap.VertexArray );
        GL.DisableClientState ( ArrayCap.TextureCoordArray );
        GL.DisableClientState ( ArrayCap.NormalArray );
        GL.DisableClientState ( ArrayCap.ColorArray );
      }
    }

    /// <summary>
    /// Preparations before rendering scene itself
    /// Called every frame
    /// </summary>
    private void Render ()
		{
			if ( !loaded )
				return;

		  Color backgroundColor;

		  if ( rayVisualizer.backgroundColor == null )
		    backgroundColor = defaultBackgroundColor;
		  else
		    backgroundColor = Color.FromArgb ( 0,
		                                       ( rayVisualizer.backgroundColor [0] ),
		                                       ( rayVisualizer.backgroundColor [1] ),
		                                       ( rayVisualizer.backgroundColor [2] ) );

		  GL.ClearColor ( backgroundColor );

			frameCounter++;
		  useShaders = useVBO &&
		               canShaders &&
		               activeProgram != null &&
		               checkShaders.Checked;

			GL.Clear ( ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit );
			GL.ShadeModel ( checkSmooth.Checked ? ShadingModel.Smooth : ShadingModel.Flat );
		  GL.PolygonMode ( MaterialFace.FrontAndBack, PolygonMode.Fill );

		  glControl1.VSync = checkVsync.Checked;

      trackBall.GLsetCamera ();

			RenderScene ();

			glControl1.SwapBuffers ();
		}

    private void Application_Idle ( object sender, EventArgs e )
		{
			if ( glControl1.IsDisposed )
				return;

			while ( glControl1.IsIdle )
			{
#if USE_INVALIDATE
        glControl1.Invalidate ();
#else
				glControl1.MakeCurrent ();
				Render ();
#endif

				long now = DateTime.Now.Ticks;
				if ( now - lastFPSTime > 5000000 ) // more than 0.5 sec
				{
					lastFPS = 0.5 * lastFPS + 0.5 * ( frameCounter * 1.0e7 / ( now - lastFPSTime ) );
					lastTPS = 0.5 * lastTPS + 0.5 * ( triangleCounter * 1.0e7 / ( now - lastFPSTime ) );
					lastFPSTime = now;
					frameCounter = 0;
					triangleCounter = 0L;

					if ( lastTPS < 5.0e5 )
						labelFPS.Text = string.Format ( CultureInfo.InvariantCulture, "FPS: {0:f1}, TPS: {1:f0}k",
														lastFPS, ( lastTPS * 1.0e-3 ) );
					else
						labelFPS.Text = string.Format ( CultureInfo.InvariantCulture, "FPS: {0:f1}, TPS: {1:f1}m",
														lastFPS, ( lastTPS * 1.0e-6 ) );
				}

				// pointing:
				if ( pointOrigin != null &&
					 pointDirty )
				{
					Vector3d p0 = new Vector3d ( pointOrigin.Value.X, pointOrigin.Value.Y, pointOrigin.Value.Z );
					Vector3d p1 = new Vector3d ( pointTarget.X, pointTarget.Y, pointTarget.Z ) - p0;
					double   nearest = double.PositiveInfinity;

				  Vector3d ul   = new Vector3d ( -1.0, -1.0, -1.0 );
				  Vector3d size = new Vector3d ( 2.0, 2.0, 2.0 );

				  if ( Geometry.RayBoxIntersection ( ref p0, ref p1, ref ul, ref size, out Vector2d uv ) )
				    nearest = uv.X;

          if ( double.IsInfinity ( nearest ) )
						spot = null;
					else
						spot = new Vector3 ( (float) ( p0.X + nearest * p1.X ),
											 (float) ( p0.Y + nearest * p1.Y ),
											 (float) ( p0.Z + nearest * p1.Z ) );

					pointDirty = false;
				}
			}
		}

		/// <summary>
		/// Rendering code itself (separated for clarity)
		/// </summary>
		private void RenderScene ()
		{	 
      if ( useShaders )
      {
        bool renderFirst = true;

        if ( AllignCameraCheckBox.Checked )
        {
          AllignCamera ( null, null );
          renderFirst = false;
        }                

        SetVertexPointer ( false );
        SetVertexAttrib ( true );

        // using GLSL shaders:
        GL.UseProgram ( activeProgram.Id );

        // uniforms:
        Matrix4 modelView  = trackBall.ModelView;
        Matrix4 projection = trackBall.Projection;
        Vector3 localEye   = trackBall.Eye;

        GL.UniformMatrix4 ( activeProgram.GetUniform ( "matrixModelView" ), false, ref modelView );
        GL.UniformMatrix4 ( activeProgram.GetUniform ( "matrixProjection" ), false, ref projection );

        GL.Uniform3 ( activeProgram.GetUniform ( "globalAmbient" ), ref globalAmbient );
        GL.Uniform3 ( activeProgram.GetUniform ( "lightColor" ), ref whiteLight );
        GL.Uniform3 ( activeProgram.GetUniform ( "lightPosition" ), ref lightPosition );
        GL.Uniform3 ( activeProgram.GetUniform ( "eyePosition" ), ref localEye );
        GL.Uniform3 ( activeProgram.GetUniform ( "Ka" ), ref matAmbient );
        GL.Uniform3 ( activeProgram.GetUniform ( "Kd" ), ref matDiffuse );
        GL.Uniform3 ( activeProgram.GetUniform ( "Ks" ), ref matSpecular );
        GL.Uniform1 ( activeProgram.GetUniform ( "shininess" ), matShininess );

        FillSceneObjects ();
        //BoundingBoxesVisualization ();

        if ( NormalRaysCheckBox.Checked || ShadowRaysCheckBox.Checked )
          RenderRays ( renderFirst );

        // actual rendering
        if ( checkAxes.Checked )
          RenderAxes ();       

        if ( pointCloudVBO != 0 && PointCloudCheckBox.Checked )
          RenderPointCloud ();

        if ( scene?.Sources != null && LightSourcesCheckBox.Checked && lightSourcesVBO != 0 )
          RenderLightSources ();

        if ( rayVisualizer.rays.Count != 0 && CameraCheckBox.Checked )
          RenderVideoCamera ();

        // clean-up
        GL.UseProgram ( 0 );
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
      SetVertexPointer ( false );
      SetVertexAttrib ( true );

      // color handling:
      bool useGlobalColor = checkGlobalColor.Checked;
      GL.Uniform1 ( activeProgram.GetUniform ( "globalColor" ), useGlobalColor ? 1 : 0 );

      // shading:
      bool shadingPhong = checkPhong.Checked;
      bool shadingGouraud = checkSmooth.Checked;

      if ( !shadingGouraud )
        shadingPhong = false;

      GL.Uniform1 ( activeProgram.GetUniform ( "useTexture" ), 0 );
      GL.Uniform1 ( activeProgram.GetUniform ( "shadingPhong" ), shadingPhong ? 1 : 0 );
      GL.Uniform1 ( activeProgram.GetUniform ( "shadingGouraud" ), shadingGouraud ? 1 : 0 );
      GL.Uniform1 ( activeProgram.GetUniform ( "useAmbient" ), checkAmbient.Checked ? 1 : 0 );
      GL.Uniform1 ( activeProgram.GetUniform ( "useDiffuse" ), checkDiffuse.Checked ? 1 : 0 );
      GL.Uniform1 ( activeProgram.GetUniform ( "useSpecular" ), checkSpecular.Checked ? 1 : 0 );
      GlInfo.LogError ( "set-uniforms" );

      const int stride = 9 * sizeof ( float );

      GL.BindBuffer ( BufferTarget.ArrayBuffer, pointCloudVBO );

      // positions
      if ( activeProgram.HasAttribute ( "position" ) )
        GL.VertexAttribPointer ( activeProgram.GetAttribute ( "position" ), 3, VertexAttribPointerType.Float, false, stride,
                                 (IntPtr) 0 );

      // colors
      if ( activeProgram.HasAttribute ( "color" ) )
        GL.VertexAttribPointer ( activeProgram.GetAttribute ( "color" ), 3, VertexAttribPointerType.Float, false, stride,
                                 (IntPtr) ( 3 * sizeof ( float ) ) );

      // normals
      if ( activeProgram.HasAttribute ( "normal" ) )
        GL.VertexAttribPointer ( activeProgram.GetAttribute ( "normal" ), 3, VertexAttribPointerType.Float, false, stride,
                                 (IntPtr) ( 6 * sizeof ( float ) ) );

      // texture coordinates - ignored
      if ( activeProgram.HasAttribute ( "texCoords" ) )
        GL.DisableVertexAttribArray ( activeProgram.GetAttribute ( "texCoords" ) );

      GlInfo.LogError ( "set-attrib-pointers" );

      GL.DrawArrays ( PrimitiveType.Points, 0, pointCloud.numberOfElements );
    }

    private void RenderPointing ()
		{
			GL.Begin ( PrimitiveType.Lines );
			GL.Color3 ( 1.0f, 1.0f, 0.0f );
			GL.Vertex3 ( pointOrigin.Value );
			GL.Vertex3 ( pointTarget );
			GL.Color3 ( 1.0f, 0.0f, 0.0f );
			GL.Vertex3 ( pointOrigin.Value );
			GL.Vertex3 ( eye );
			GL.End ();

			GL.PointSize ( 4.0f );
			GL.Begin ( PrimitiveType.Points );
			GL.Color3 ( 1.0f, 0.0f, 0.0f );
			GL.Vertex3 ( pointOrigin.Value );
			GL.Color3 ( 0.0f, 1.0f, 0.2f );
			GL.Vertex3 ( pointTarget );
			GL.Color3 ( 1.0f, 1.0f, 1.0f );

			if ( spot != null )
				GL.Vertex3 ( spot.Value );

			GL.Vertex3 ( eye );
			GL.End ();
		}

		private void RenderFrustum ()
		{
			GL.LineWidth ( 2.0f );
			GL.Begin ( PrimitiveType.Lines );

			GL.Color3 ( 1.0f, 0.0f, 0.0f );
			GL.Vertex3 ( frustumFrame[0] );
			GL.Vertex3 ( frustumFrame[1] );
			GL.Vertex3 ( frustumFrame[1] );
			GL.Vertex3 ( frustumFrame[3] );
			GL.Vertex3 ( frustumFrame[3] );
			GL.Vertex3 ( frustumFrame[2] );
			GL.Vertex3 ( frustumFrame[2] );
			GL.Vertex3 ( frustumFrame[0] );

			GL.Color3 ( 1.0f, 1.0f, 1.0f );
			GL.Vertex3 ( frustumFrame[0] );
			GL.Vertex3 ( frustumFrame[4] );
			GL.Vertex3 ( frustumFrame[1] );
			GL.Vertex3 ( frustumFrame[5] );
			GL.Vertex3 ( frustumFrame[2] );
			GL.Vertex3 ( frustumFrame[6] );
			GL.Vertex3 ( frustumFrame[3] );
			GL.Vertex3 ( frustumFrame[7] );

			GL.Color3 ( 0.0f, 1.0f, 0.0f );
			GL.Vertex3 ( frustumFrame[4] );
			GL.Vertex3 ( frustumFrame[5] );
			GL.Vertex3 ( frustumFrame[5] );
			GL.Vertex3 ( frustumFrame[7] );
			GL.Vertex3 ( frustumFrame[7] );
			GL.Vertex3 ( frustumFrame[6] );
			GL.Vertex3 ( frustumFrame[6] );
			GL.Vertex3 ( frustumFrame[4] );

			GL.End ();
		}

    /// <summary>
    /// Renders 3 axes in origin
    /// </summary>
    private void RenderAxes ()
    {
      SetVertexPointer ( false );
      SetVertexAttrib ( true );

      // color handling:
      GL.Uniform1 ( activeProgram.GetUniform ( "globalColor" ), 0 );

      // shading:
      GL.Uniform1 ( activeProgram.GetUniform ( "useTexture" ), 0 );
      GL.Uniform1 ( activeProgram.GetUniform ( "shadingPhong" ), 0 );
      GL.Uniform1 ( activeProgram.GetUniform ( "shadingGouraud" ), 0 );
      GL.Uniform1 ( activeProgram.GetUniform ( "useAmbient" ), 0 );
      GL.Uniform1 ( activeProgram.GetUniform ( "useDiffuse" ), 0 );
      GL.Uniform1 ( activeProgram.GetUniform ( "useSpecular" ), 0 );
      GlInfo.LogError ( "set-uniforms" );

      const int stride = 6 * sizeof ( float );

      GL.BindBuffer ( BufferTarget.ArrayBuffer, axesVBO );

      // positions
      if ( activeProgram.HasAttribute ( "position" ) )
        GL.VertexAttribPointer ( activeProgram.GetAttribute ( "position" ), 3, VertexAttribPointerType.Float, false, stride,
                                 (IntPtr) 0 );

      // texture coordinate
      if ( activeProgram.HasAttribute ( "color" ) )
        GL.VertexAttribPointer ( activeProgram.GetAttribute ( "color" ), 3, VertexAttribPointerType.Float, false, stride,
                                 (IntPtr) ( 3 * sizeof ( float ) ) );

      // Ignored attributes
      if ( activeProgram.HasAttribute ( "texCoords" ) )
        GL.DisableVertexAttribArray ( activeProgram.GetAttribute ( "texCoords" ) );
      if ( activeProgram.HasAttribute ( "normal" ) )
        GL.DisableVertexAttribArray ( activeProgram.GetAttribute ( "normal" ) );

      GlInfo.LogError ( "set-attrib-pointers" );      

      GL.LineWidth ( 4.0f );

      GL.DrawArrays ( PrimitiveType.Lines, 0, 6 );
    }

    private void RenderPlaceholderScene ()
		{
			SetVertexPointer ( false );
			SetVertexAttrib ( false );

			GL.Begin ( PrimitiveType.Quads );

			GL.Color3 ( 0.0f, 1.0f, 0.0f );    // Set The Color To Green
			GL.Vertex3 ( 1.0f, 1.0f, -1.0f );  // Top Right Of The Quad (Top)
			GL.Vertex3 ( -1.0f, 1.0f, -1.0f ); // Top Left Of The Quad (Top)
			GL.Vertex3 ( -1.0f, 1.0f, 1.0f );  // Bottom Left Of The Quad (Top)
			GL.Vertex3 ( 1.0f, 1.0f, 1.0f );   // Bottom Right Of The Quad (Top)

			GL.Color3 ( 1.0f, 0.5f, 0.0f );     // Set The Color To Orange
			GL.Vertex3 ( 1.0f, -1.0f, 1.0f );   // Top Right Of The Quad (Bottom)
			GL.Vertex3 ( -1.0f, -1.0f, 1.0f );  // Top Left Of The Quad (Bottom)
			GL.Vertex3 ( -1.0f, -1.0f, -1.0f ); // Bottom Left Of The Quad (Bottom)
			GL.Vertex3 ( 1.0f, -1.0f, -1.0f );  // Bottom Right Of The Quad (Bottom)

			GL.Color3 ( 1.0f, 0.0f, 0.0f );    // Set The Color To Red
			GL.Vertex3 ( 1.0f, 1.0f, 1.0f );   // Top Right Of The Quad (Front)
			GL.Vertex3 ( -1.0f, 1.0f, 1.0f );  // Top Left Of The Quad (Front)
			GL.Vertex3 ( -1.0f, -1.0f, 1.0f ); // Bottom Left Of The Quad (Front)
			GL.Vertex3 ( 1.0f, -1.0f, 1.0f );  // Bottom Right Of The Quad (Front)

			GL.Color3 ( 1.0f, 1.0f, 0.0f );     // Set The Color To Yellow
			GL.Vertex3 ( 1.0f, -1.0f, -1.0f );  // Bottom Left Of The Quad (Back)
			GL.Vertex3 ( -1.0f, -1.0f, -1.0f ); // Bottom Right Of The Quad (Back)
			GL.Vertex3 ( -1.0f, 1.0f, -1.0f );  // Top Right Of The Quad (Back)
			GL.Vertex3 ( 1.0f, 1.0f, -1.0f );   // Top Left Of The Quad (Back)

			GL.Color3 ( 0.0f, 0.0f, 1.0f );     // Set The Color To Blue
			GL.Vertex3 ( -1.0f, 1.0f, 1.0f );   // Top Right Of The Quad (Left)
			GL.Vertex3 ( -1.0f, 1.0f, -1.0f );  // Top Left Of The Quad (Left)
			GL.Vertex3 ( -1.0f, -1.0f, -1.0f ); // Bottom Left Of The Quad (Left)
			GL.Vertex3 ( -1.0f, -1.0f, 1.0f );  // Bottom Right Of The Quad (Left)

			GL.Color3 ( 1.0f, 0.0f, 1.0f );    // Set The Color To Violet
			GL.Vertex3 ( 1.0f, 1.0f, -1.0f );  // Top Right Of The Quad (Right)
			GL.Vertex3 ( 1.0f, 1.0f, 1.0f );   // Top Left Of The Quad (Right)
			GL.Vertex3 ( 1.0f, -1.0f, 1.0f );  // Bottom Left Of The Quad (Right)
			GL.Vertex3 ( 1.0f, -1.0f, -1.0f ); // Bottom Right Of The Quad (Right)

			GL.End ();

			triangleCounter += 12;
		}

    /// <summary>
    /// Renders all normal and shadow rays (further selection done via check boxes)
    /// </summary>
    /// <param name="renderFirst">FALSE if first ray should not be rendered (used when camera is perpendicular with this first ray)</param>
    private void RenderRays ( bool renderFirst )
    {
      SetVertexPointer ( false );
      SetVertexAttrib ( true );

      // color handling:
      GL.Uniform1 ( activeProgram.GetUniform ( "globalColor" ), 0 );

      // shading:
      GL.Uniform1 ( activeProgram.GetUniform ( "useTexture" ), 0 );
      GL.Uniform1 ( activeProgram.GetUniform ( "shadingPhong" ), 0 );
      GL.Uniform1 ( activeProgram.GetUniform ( "shadingGouraud" ), 0 );
      GL.Uniform1 ( activeProgram.GetUniform ( "useAmbient" ), 0 );
      GL.Uniform1 ( activeProgram.GetUniform ( "useDiffuse" ), 0 );
      GL.Uniform1 ( activeProgram.GetUniform ( "useSpecular" ), 0 );
      GlInfo.LogError ( "set-uniforms" );

      const int stride = 6 * sizeof ( float );

      GL.BindBuffer ( BufferTarget.ArrayBuffer, raysVBO );

      // positions
      if ( activeProgram.HasAttribute ( "position" ) )
        GL.VertexAttribPointer ( activeProgram.GetAttribute ( "position" ), 3, VertexAttribPointerType.Float, false, stride,
                                 (IntPtr) 0 );

      // texture coordinate
      if ( activeProgram.HasAttribute ( "color" ) )
        GL.VertexAttribPointer ( activeProgram.GetAttribute ( "color" ), 3, VertexAttribPointerType.Float, false, stride,
                                 (IntPtr) ( 3 * sizeof ( float ) ) );

      // Ignored attributes
      if ( activeProgram.HasAttribute ( "texCoords" ) )
        GL.DisableVertexAttribArray ( activeProgram.GetAttribute ( "texCoords" ) );
      if ( activeProgram.HasAttribute ( "normal" ) )
        GL.DisableVertexAttribArray ( activeProgram.GetAttribute ( "normal" ) );

      GlInfo.LogError ( "set-attrib-pointers" );

      GL.LineWidth ( 3.0f );

      if ( NormalRaysCheckBox.Checked )
        if ( renderFirst )
          GL.DrawArrays ( PrimitiveType.Lines, 0, rays.Count );
        else
          GL.DrawArrays ( PrimitiveType.Lines, 2, rays.Count - 2 );

      if ( ShadowRaysCheckBox.Checked )
        GL.DrawArrays ( PrimitiveType.Lines, rays.Count, shadowRays.Count );
    }

    /// <summary>
    /// Renders representation of camera (initially at position of rayOrigin of first primary ray)
    /// </summary>
    private void RenderCameraOLD ()
		{
			if ( rayVisualizer.rays.Count == 0 || !CameraCheckBox.Checked )
				return;

			RenderCube ( rayVisualizer.rays[0], 0.2f, Color.Turquoise );
		}

    /// <summary>
    /// Renders representation of camera (initially at position of rayOrigin of first primary ray)
    /// </summary>
    private void RenderVideoCamera ()
    {
      SetVertexPointer ( false );
      SetVertexAttrib ( true );

      // billboarding
      Matrix4 translation = Matrix4.CreateTranslation ( rays [0] );
      Matrix4 modelView  = trackBall.ModelView;

      modelView = translation * modelView;
      modelView = ApplyBillboarding ( modelView );

      GL.UniformMatrix4 ( activeProgram.GetUniform ( "matrixModelView" ), false, ref modelView );

      // color handling:
      GL.Uniform1 ( activeProgram.GetUniform ( "globalColor" ), 0 );

      // shading:
      GL.Uniform1 ( activeProgram.GetUniform ( "useTexture" ), 1 );
      GL.Uniform1 ( activeProgram.GetUniform ( "shadingPhong" ), 0 );
      GL.Uniform1 ( activeProgram.GetUniform ( "shadingGouraud" ), 0 );
      GL.Uniform1 ( activeProgram.GetUniform ( "useAmbient" ), 0 );
      GL.Uniform1 ( activeProgram.GetUniform ( "useDiffuse" ), 0 );
      GL.Uniform1 ( activeProgram.GetUniform ( "useSpecular" ), 0 );
      GlInfo.LogError ( "set-uniforms" );

      const int stride = 5 * sizeof ( float );

      GL.BindBuffer ( BufferTarget.ArrayBuffer, videoCameraVBO );

      GL.BindTexture ( TextureTarget.Texture2D, videoCameraTextureID );

      // positions
      if ( activeProgram.HasAttribute ( "position" ) )
        GL.VertexAttribPointer ( activeProgram.GetAttribute ( "position" ), 3, VertexAttribPointerType.Float, false, stride,
                                 (IntPtr) 0 );

      // texture coordinate
      if ( activeProgram.HasAttribute ( "texCoords" ) )
        GL.VertexAttribPointer ( activeProgram.GetAttribute ( "texCoords" ), 2, VertexAttribPointerType.Float, false, stride,
                                 (IntPtr) ( 3 * sizeof ( float ) ) );

      // Ignored attributes
      if ( activeProgram.HasAttribute ( "color" ) )
        GL.DisableVertexAttribArray ( activeProgram.GetAttribute ( "color" ) );
      if ( activeProgram.HasAttribute ( "normal" ) )
        GL.DisableVertexAttribArray ( activeProgram.GetAttribute ( "normal" ) );

      GlInfo.LogError ( "set-attrib-pointers" );

      GL.DrawArrays ( PrimitiveType.Quads, 0, 4 );
    }

    private Matrix4 ApplyBillboarding ( Matrix4 modelView )
    {
      modelView [0, 0] = 1;
      modelView [0, 1] = 0;
      modelView [0, 2] = 0;

      modelView [1, 0] = 0;
      modelView [1, 1] = 1;
      modelView [1, 2] = 0;

      modelView [2, 0] = 0;
      modelView [2, 1] = 0;
      modelView [2, 2] = 1;

      modelView = Matrix4.CreateScale ( trackBall.Zoom / diameter ) * modelView;
      return modelView;
    }

    /// <summary>
    /// Renders representation of all light sources (except those in with null as position
    /// - usually ambient and directional lights which position does not matter)
    /// </summary>
    private void RenderLightSources ()
    {
      SetVertexPointer ( false );
      SetVertexAttrib ( true );

      // color handling:
      GL.Uniform1 ( activeProgram.GetUniform ( "globalColor" ), 0 );

      // shading:
      GL.Uniform1 ( activeProgram.GetUniform ( "useTexture" ), 1 );
      GL.Uniform1 ( activeProgram.GetUniform ( "shadingPhong" ), 0 );
      GL.Uniform1 ( activeProgram.GetUniform ( "shadingGouraud" ), 0 );
      GL.Uniform1 ( activeProgram.GetUniform ( "useAmbient" ), 0 );
      GL.Uniform1 ( activeProgram.GetUniform ( "useDiffuse" ), 0 );     
      GL.Uniform1 ( activeProgram.GetUniform ( "useSpecular" ), 0 );
      GlInfo.LogError ( "set-uniforms" );

      const int stride = 5 * sizeof ( float );

      GL.BindBuffer ( BufferTarget.ArrayBuffer, lightSourcesVBO );

      GL.BindTexture ( TextureTarget.Texture2D, lightSourceTextureID );

      // positions
      if ( activeProgram.HasAttribute ( "position" ) )
        GL.VertexAttribPointer ( activeProgram.GetAttribute ( "position" ), 3, VertexAttribPointerType.Float, false, stride,
                                 (IntPtr) 0 );

      // texture coordinate
      if ( activeProgram.HasAttribute ( "texCoords" ) )
        GL.VertexAttribPointer ( activeProgram.GetAttribute ( "texCoords" ), 2, VertexAttribPointerType.Float, false, stride,
                                 (IntPtr) ( 3 * sizeof ( float ) ) );

      // Ignored attributes
      if ( activeProgram.HasAttribute ( "color" ) )
        GL.DisableVertexAttribArray ( activeProgram.GetAttribute ( "color" ) );
      if ( activeProgram.HasAttribute ( "normal" ) )
        GL.DisableVertexAttribArray ( activeProgram.GetAttribute ( "normal" ) );

      GlInfo.LogError ( "set-attrib-pointers" );

      foreach ( ILightSource lightSource in scene.Sources )
      {
        if ( lightSource.position == null )
          continue;

        // billboarding
        Matrix4 translation = Matrix4.CreateTranslation ( (Vector3) RayVisualizer.AxesCorrector ( lightSource.position ) );
        Matrix4 modelView  = trackBall.ModelView;

        modelView = translation * modelView;
        modelView = ApplyBillboarding ( modelView );

        GL.UniformMatrix4 ( activeProgram.GetUniform ( "matrixModelView" ), false, ref modelView );

        //drawing
        GL.DrawArrays ( PrimitiveType.Quads, 0, 4 );
      }     
    }

    /// <summary>
    /// Renders simple cube of uniform color
    /// Initially used as placeholder so several objects
    /// </summary>
    /// <param name="position">Position in space</param>
    /// <param name="size">Size of cube</param>
    /// <param name="color">Uniform color of cube</param>
    private void RenderCube ( Vector3 position, float size, Color color )
		{
			SetVertexPointer ( false );
			SetVertexAttrib ( false );

			GL.Begin ( PrimitiveType.Quads );
			GL.Color3 ( color );


			GL.Vertex3 ( ( new Vector3 ( 1.0f, 1.0f, -1.0f ) * size + position ) );  // Top Right Of The Quad (Top)
			GL.Vertex3 ( ( new Vector3 ( -1.0f, 1.0f, -1.0f ) * size + position ) ); // Top Left Of The Quad (Top)
			GL.Vertex3 ( ( new Vector3 ( -1.0f, 1.0f, 1.0f ) * size + position ) );  // Bottom Left Of The Quad (Top)
			GL.Vertex3 ( ( new Vector3 ( 1.0f, 1.0f, 1.0f ) * size + position ) );   // Bottom Right Of The Quad (Top)

			GL.Vertex3 ( ( new Vector3 ( 1.0f, -1.0f, 1.0f ) * size + position ) );   // Top Right Of The Quad (Bottom)
			GL.Vertex3 ( ( new Vector3 ( -1.0f, -1.0f, 1.0f ) * size + position ) );  // Top Left Of The Quad (Bottom)
			GL.Vertex3 ( ( new Vector3 ( -1.0f, -1.0f, -1.0f ) * size + position ) ); // Bottom Left Of The Quad (Bottom)
			GL.Vertex3 ( ( new Vector3 ( 1.0f, -1.0f, -1.0f ) * size + position ) );  // Bottom Right Of The Quad (Bottom)

			GL.Vertex3 ( ( new Vector3 ( 1.0f, 1.0f, 1.0f ) * size + position ) );   // Top Right Of The Quad (Front)
			GL.Vertex3 ( ( new Vector3 ( -1.0f, 1.0f, 1.0f ) * size + position ) );  // Top Left Of The Quad (Front)
			GL.Vertex3 ( ( new Vector3 ( -1.0f, -1.0f, 1.0f ) * size + position ) ); // Bottom Left Of The Quad (Front)
			GL.Vertex3 ( ( new Vector3 ( 1.0f, -1.0f, 1.0f ) * size + position ) );  // Bottom Right Of The Quad (Front)

			GL.Vertex3 ( ( new Vector3 ( 1.0f, -1.0f, -1.0f ) * size + position ) );  // Bottom Left Of The Quad (Back)
			GL.Vertex3 ( ( new Vector3 ( -1.0f, -1.0f, -1.0f ) * size + position ) ); // Bottom Right Of The Quad (Back)
			GL.Vertex3 ( ( new Vector3 ( -1.0f, 1.0f, -1.0f ) * size + position ) );  // Top Right Of The Quad (Back)
			GL.Vertex3 ( ( new Vector3 ( 1.0f, 1.0f, -1.0f ) * size + position ) );   // Top Left Of The Quad (Back)

			GL.Vertex3 ( ( new Vector3 ( -1.0f, 1.0f, 1.0f ) * size + position ) );   // Top Right Of The Quad (Left)
			GL.Vertex3 ( ( new Vector3 ( -1.0f, 1.0f, -1.0f ) * size + position ) );  // Top Left Of The Quad (Left)
			GL.Vertex3 ( ( new Vector3 ( -1.0f, -1.0f, -1.0f ) * size + position ) ); // Bottom Left Of The Quad (Left)
			GL.Vertex3 ( ( new Vector3 ( -1.0f, -1.0f, 1.0f ) * size + position ) );  // Bottom Right Of The Quad (Left)

			GL.Vertex3 ( ( new Vector3 ( 1.0f, 1.0f, -1.0f ) * size + position ) );  // Top Right Of The Quad (Right)
			GL.Vertex3 ( ( new Vector3 ( 1.0f, 1.0f, 1.0f ) * size + position ) );   // Top Left Of The Quad (Right)
			GL.Vertex3 ( ( new Vector3 ( 1.0f, -1.0f, 1.0f ) * size + position ) );  // Bottom Left Of The Quad (Right)
			GL.Vertex3 ( ( new Vector3 ( 1.0f, -1.0f, -1.0f ) * size + position ) ); // Bottom Right Of The Quad (Right)

			GL.End ();

			triangleCounter += 12;
		}

		/// <summary>
		/// Renders cuboid (of uniform color) based on 2 opposite corners (with sides parallel to axes) and transforms it with transformation matrix
		/// </summary>
		/// <param name="c1">Position of first corner</param>
		/// <param name="c2">Position of corner opposite to first corner</param>
		/// <param name="transformation">4x4 transformation matrix</param>
		/// <param name="color">Uniform color of cuboid</param>
		private void RenderCuboid ( Vector3d c1, Vector3d c2, Matrix4d transformation, Color color )
		{
			SetVertexPointer ( false );
			SetVertexAttrib ( false );

			GL.PolygonMode ( MaterialFace.FrontAndBack, WireframeBoundingBoxesCheckBox.Checked ? PolygonMode.Line : PolygonMode.Fill );

			GL.Begin ( PrimitiveType.Quads );
			GL.Color3 ( color );


			GL.Vertex3 ( RayVisualizer.AxesCorrector ( ApplyTransformation ( new Vector3d ( c2.X, c2.Y, c1.Z ), transformation ) ) ); // Top Right Of The Quad (Top)
			GL.Vertex3 ( RayVisualizer.AxesCorrector ( ApplyTransformation ( new Vector3d ( c1.X, c2.Y, c1.Z ), transformation ) ) ); // Top Left Of The Quad (Top)
			GL.Vertex3 ( RayVisualizer.AxesCorrector ( ApplyTransformation ( new Vector3d ( c1.X, c2.Y, c2.Z ), transformation ) ) ); // Bottom Left Of The Quad (Top)
			GL.Vertex3 ( RayVisualizer.AxesCorrector ( ApplyTransformation ( new Vector3d ( c2.X, c2.Y, c2.Z ), transformation ) ) ); // Bottom Right Of The Quad (Top)

			GL.Vertex3 ( RayVisualizer.AxesCorrector ( ApplyTransformation ( new Vector3d ( c2.X, c1.Y, c2.Z ), transformation ) ) ); // Top Right Of The Quad (Bottom)
			GL.Vertex3 ( RayVisualizer.AxesCorrector ( ApplyTransformation ( new Vector3d ( c1.X, c1.Y, c2.Z ), transformation ) ) ); // Top Left Of The Quad (Bottom)
			GL.Vertex3 ( RayVisualizer.AxesCorrector ( ApplyTransformation ( new Vector3d ( c1.X, c1.Y, c1.Z ), transformation ) ) ); // Bottom Left Of The Quad (Bottom)
			GL.Vertex3 ( RayVisualizer.AxesCorrector ( ApplyTransformation ( new Vector3d ( c2.X, c1.Y, c1.Z ), transformation ) ) ); // Bottom Right Of The Quad (Bottom)

			GL.Vertex3 ( RayVisualizer.AxesCorrector ( ApplyTransformation ( new Vector3d ( c2.X, c2.Y, c2.Z ), transformation ) ) ); // Top Right Of The Quad (Front)
			GL.Vertex3 ( RayVisualizer.AxesCorrector ( ApplyTransformation ( new Vector3d ( c1.X, c2.Y, c2.Z ), transformation ) ) ); // Top Left Of The Quad (Front)
			GL.Vertex3 ( RayVisualizer.AxesCorrector ( ApplyTransformation ( new Vector3d ( c1.X, c1.Y, c2.Z ), transformation ) ) ); // Bottom Left Of The Quad (Front)
			GL.Vertex3 ( RayVisualizer.AxesCorrector ( ApplyTransformation ( new Vector3d ( c2.X, c1.Y, c2.Z ), transformation ) ) ); // Bottom Right Of The Quad (Front)

			GL.Vertex3 ( RayVisualizer.AxesCorrector ( ApplyTransformation ( new Vector3d ( c2.X, c1.Y, c1.Z ), transformation ) ) ); // Bottom Left Of The Quad (Back)
			GL.Vertex3 ( RayVisualizer.AxesCorrector ( ApplyTransformation ( new Vector3d ( c1.X, c1.Y, c1.Z ), transformation ) ) ); // Bottom Right Of The Quad (Back)
			GL.Vertex3 ( RayVisualizer.AxesCorrector ( ApplyTransformation ( new Vector3d ( c1.X, c2.Y, c1.Z ), transformation ) ) ); // Top Right Of The Quad (Back)
			GL.Vertex3 ( RayVisualizer.AxesCorrector ( ApplyTransformation ( new Vector3d ( c2.X, c2.Y, c1.Z ), transformation ) ) ); // Top Left Of The Quad (Back)

			GL.Vertex3 ( RayVisualizer.AxesCorrector ( ApplyTransformation ( new Vector3d ( c1.X, c2.Y, c2.Z ), transformation ) ) ); // Top Right Of The Quad (Left)
			GL.Vertex3 ( RayVisualizer.AxesCorrector ( ApplyTransformation ( new Vector3d ( c1.X, c2.Y, c1.Z ), transformation ) ) ); // Top Left Of The Quad (Left)
			GL.Vertex3 ( RayVisualizer.AxesCorrector ( ApplyTransformation ( new Vector3d ( c1.X, c1.Y, c1.Z ), transformation ) ) ); // Bottom Left Of The Quad (Left)
			GL.Vertex3 ( RayVisualizer.AxesCorrector ( ApplyTransformation ( new Vector3d ( c1.X, c1.Y, c2.Z ), transformation ) ) ); // Bottom Right Of The Quad (Left)

			GL.Vertex3 ( RayVisualizer.AxesCorrector ( ApplyTransformation ( new Vector3d ( c2.X, c2.Y, c1.Z ), transformation ) ) ); // Top Right Of The Quad (Right)
			GL.Vertex3 ( RayVisualizer.AxesCorrector ( ApplyTransformation ( new Vector3d ( c2.X, c2.Y, c2.Z ), transformation ) ) ); // Top Left Of The Quad (Right)
			GL.Vertex3 ( RayVisualizer.AxesCorrector ( ApplyTransformation ( new Vector3d ( c2.X, c1.Y, c2.Z ), transformation ) ) ); // Bottom Left Of The Quad (Right)
			GL.Vertex3 ( RayVisualizer.AxesCorrector ( ApplyTransformation ( new Vector3d ( c2.X, c1.Y, c1.Z ), transformation ) ) ); // Bottom Right Of The Quad (Right)


			GL.End ();

		  GL.PolygonMode ( MaterialFace.FrontAndBack, PolygonMode.Fill );

      triangleCounter += 12;
		}

		/// <summary>
		/// Applies transformation matrix to vector
		/// </summary>
		/// <param name="vector">1x3 vector</param>
		/// <param name="transformation">4x4 transformation matrix</param>
		/// <returns>Transformed vector 1x3</returns>
		public Vector3d ApplyTransformation ( Vector3d vector, Matrix4d transformation )
		{
			Vector4d transformedVector = MultiplyVectorByMatrix ( new Vector4d ( vector, 1 ), transformation ); //( vector, 1 ) is extenstion [x  y  z] -> [x  y  z  1]

			return new Vector3d ( transformedVector.X / transformedVector.W, //[x  y  z  w] -> [x/w  y/w  z/w]
								  transformedVector.Y / transformedVector.W,
								  transformedVector.Z / transformedVector.W );
		}

		/// <summary>
		/// Multiplication of Vector4d and Matrix4d
		/// </summary>
		/// <param name="vector">Vector 1x4 on left side</param>
		/// <param name="matrix">Matrix 4x4 on right side</param>
		/// <returns>Result of multiplication - 1x4 vector</returns>
		public Vector4d MultiplyVectorByMatrix ( Vector4d vector, Matrix4d matrix )
		{
			Vector4d result = new Vector4d (0, 0, 0, 0);

			for ( int i = 0; i < 4; i++ )
			{
				for ( int j = 0; j < 4; j++ )
				{
					result[i] += vector[j] * matrix[j, i];
				}
			}

			return result;
		}

		/// <summary>
		/// Renders scene using bounding boxes
		/// </summary>
		private void BoundingBoxesVisualization ()
		{
			if ( sceneObjects == null || sceneObjects.Count == 0 || !BoundingBoxesCheckBox.Checked )
			{
				return;
			}

			int index = 0;

			foreach ( SceneObject sceneObject in sceneObjects )
			{
				if ( sceneObject.solid is Plane plane )
				{
					plane.GetBoundingBox ( out Vector3d c1, out Vector3d c2 );


					Color color = Color.FromArgb ( (int) ( sceneObject.color [ 0 ] * 255 ),
										 (int) ( sceneObject.color [ 1 ] * 255 ),
										 (int) ( sceneObject.color [ 2 ] * 255 ) );

					c1 = RayVisualizer.AxesCorrector ( c1 );
					c2 = RayVisualizer.AxesCorrector ( c2 );

					SetVertexPointer ( false );
					SetVertexAttrib ( false );



					if ( !plane.Triangle )
					{
						GL.Begin ( PrimitiveType.Quads );
						GL.Color3 ( color );

						GL.Vertex3 ( RayVisualizer.AxesCorrector ( ApplyTransformation ( new Vector3d ( c1.X, c1.Y, c2.Z ), sceneObject.transformation ) ) );
						GL.Vertex3 ( RayVisualizer.AxesCorrector ( ApplyTransformation ( new Vector3d ( c2.X, c1.Y, c2.Z ), sceneObject.transformation ) ) );
						GL.Vertex3 ( RayVisualizer.AxesCorrector ( ApplyTransformation ( new Vector3d ( c2.X, c2.Y, c1.Z ), sceneObject.transformation ) ) );
						GL.Vertex3 ( RayVisualizer.AxesCorrector ( ApplyTransformation ( new Vector3d ( c1.X, c2.Y, c1.Z ), sceneObject.transformation ) ) );

						triangleCounter += 2;

						GL.End ();
					}
					else
					{
						GL.Begin ( PrimitiveType.Triangles );
						GL.Color3 ( color );

						GL.Vertex3 ( RayVisualizer.AxesCorrector ( ApplyTransformation ( new Vector3d ( c1.X, c1.Y, c2.Z ), sceneObject.transformation ) ) );
						GL.Vertex3 ( RayVisualizer.AxesCorrector ( ApplyTransformation ( new Vector3d ( c2.X, c1.Y, c2.Z ), sceneObject.transformation ) ) );
						GL.Vertex3 ( RayVisualizer.AxesCorrector ( ApplyTransformation ( new Vector3d ( c2.X, c2.Y, c1.Z ), sceneObject.transformation ) ) );

						triangleCounter += 1;

						GL.End ();
					}
				}
				else
				{
					sceneObject.solid.GetBoundingBox ( out Vector3d c1, out Vector3d c2 );

					Color color;

					if ( sceneObject.color != null )
					{
						color = Color.FromArgb ( (int) ( sceneObject.color[0] * 255 ),
												 (int) ( sceneObject.color[1] * 255 ),
												 (int) ( sceneObject.color[2] * 255 ) );
					}
					else
					{
						color = GenerateColor ( index, sceneObjects.Count );
					}

					RenderCuboid ( c1, c2, sceneObject.transformation, color );
				}

				index++;
			}
		}

		/// <summary>
		/// Linearly generates shade of gray between approximately [0.25, 0.25, 0.25] and [0.75, 0.75, 0.75] (in 0 to 1 for RGB channels)
		/// Closer the currentValue is to the maxValue, closer is returned color to [0.75, 0.75, 0.75]
		/// </summary>
		/// <param name="currentValue">Indicates current position between 0 and maxValue</param>
		/// <param name="maxValue">Max value currentValue can have</param>
		/// <returns>Gray color between approximately [0.25, 0.25, 0.25] and [0.75, 0.75, 0.75]</returns>
		private Color GenerateColor ( int currentValue, int maxValue )
		{
			Arith.Clamp ( currentValue, 0, maxValue );

			int colorValue = (int) ( ( currentValue / (double) maxValue / 1.333 + 0.25 ) * 255 );

			return Color.FromArgb ( colorValue, colorValue, colorValue );
		}

    private List<SceneObject> sceneObjects;
    private IRayScene scene;
		/// <summary>
		/// Fills sceneObjects list with objects from current scene
		/// </summary>
		private void FillSceneObjects ()
		{
		  if ( rayVisualizer.rayScene == scene ) // prevents filling whole list in case scene did not change (most of the time)
		    return;
		  else
		    scene = rayVisualizer.rayScene;

      if ( !( scene.Intersectable is DefaultSceneNode root ) )
      {
        sceneObjects = null;
        return;
      }

      sceneObjects = new List<SceneObject> ();

			Matrix4d transformation = Matrix4d.Identity;

			double[] color;

			if ( (double[]) root.GetAttribute ( PropertyName.COLOR ) != null )
			{
				color = (double[]) root.GetAttribute ( PropertyName.COLOR );
			}
			else
			{
				color = ( (IMaterial) root.GetAttribute ( PropertyName.MATERIAL ) ).Color;
			}

			EvaluateSceneNode ( root, transformation, color );
		}

		/// <summary>
		/// Recursively goes through all children nodes of parent
		/// If child is ISolid, adds a new object to sceneObjects based on it
		/// If child is another CSGInnerNode, recursively goes through its children
		/// Meanwhile counts transformation matrix for SceneObjects
		/// </summary>
		/// <param name="parent">Parent node</param>
		/// <param name="transformation"></param>
		private void EvaluateSceneNode ( DefaultSceneNode parent, Matrix4d transformation, double[] color )
		{
			foreach ( ISceneNode sceneNode in parent.Children )
			{
			  ISceneNode children = (DefaultSceneNode) sceneNode;
			  Matrix4d localTransformation = children.ToParent * transformation;


				double[] newColor;

				// take color from child's attribute COLOR, child's attribute MATERIAL.Color or from parent
				if ( (double[]) children.GetAttribute ( PropertyName.COLOR ) == null )
				  newColor = ( (IMaterial) children.GetAttribute ( PropertyName.MATERIAL ) ).Color ?? color;
				else
					newColor = (double[]) children.GetAttribute ( PropertyName.COLOR );


				if ( children is ISolid solid )
				{
					sceneObjects.Add ( new SceneObject ( solid, localTransformation, newColor ) );
					continue;
				}

				if ( children is CSGInnerNode node )
				{
					EvaluateSceneNode ( node, localTransformation, newColor );
				}
			}
		}

		/// <summary>
		/// Data class containing info about ISolid and its absolute position in scene (through transformation matrix)
		/// </summary>
		private class SceneObject
		{
			public ISolid solid;
			public Matrix4d transformation;
			public double[] color;

			public SceneObject ( ISolid solid, Matrix4d transformation, double[] color )
			{
				this.solid = solid;
				this.transformation = transformation;
				this.color = color;
			}
		}

    private PointCloud pointCloud;
    private int pointCloudVBO;

    /// <summary>
    /// Gets reference to point cloud, calls initialization of VBO for point cloud and changes GUI elements respectively 
    /// </summary>
    /// <param name="sender">Not needed</param>
    /// <param name="e">Not needed</param>
    private void PointCloudButton_Click ( object sender, EventArgs e )
    {
      pointCloud = AdditionalViews.singleton.pointCloud;

      if ( pointCloud.cloud == null || pointCloud.IsCloudEmpty )
        return;

      InitializePointCloudVBO ();

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
      if ( pointCloudVBO != 0 )
        GL.DeleteBuffer ( pointCloudVBO );

      pointCloudVBO = GL.GenBuffer ();
      GL.BindBuffer ( BufferTarget.ArrayBuffer, pointCloudVBO );

      int size = pointCloud.numberOfElements * 9 * sizeof(float);

      GL.BufferData ( BufferTarget.ArrayBuffer, (IntPtr) ( size ), IntPtr.Zero, BufferUsageHint.StaticDraw );

      int currentLength = 0;

      foreach ( List<float> list in pointCloud.cloud )
      {      
        GL.BufferSubData ( BufferTarget.ArrayBuffer, (IntPtr) currentLength, (IntPtr) ( list.Count * sizeof ( float ) ) - 1, list.ToArray () );
        currentLength += list.Count * sizeof ( float );
      }

      BoundingBoxesCheckBox.Checked = false;
      WireframeBoundingBoxesCheckBox.Enabled = false;
    }

    private void BoundingBoxesCheckBox_CheckedChanged ( object sender, EventArgs e )
    {
      WireframeBoundingBoxesCheckBox.Enabled = BoundingBoxesCheckBox.Checked;
    }

    /// <summary>
    /// Called when scene has changed
    /// Sets new light sources and sets GUI (must be invoked if called from different thread)
    /// </summary>
    /// <param name="newScene">New IRayScene</param>
    public void UpdateRayScene ( IRayScene newScene )
    {
      scene = newScene;

      InitializeLightSourcesVBO ();

      PointCloudButton.Enabled = false;
      PointCloudCheckBox.Enabled = false;
      PointCloudCheckBox.Checked = false;

      BoundingBoxesCheckBox.Checked = true;
      WireframeBoundingBoxesCheckBox.Enabled = true;
    }

    private int lightSourcesVBO;
    private int usableLightSources;
    private const float lightSize = 1.0f;
    /// <summary>
    /// Initializes VBO for light sources
    /// X, Y, Z, u, v
    /// 1 quad total - this VBO is expected to be called for each light with different view model matrices
    /// No attributes are expected to change
    /// </summary>
    private void InitializeLightSourcesVBO ()
    {
      if ( lightSourcesVBO != 0 )
        GL.DeleteBuffer ( lightSourcesVBO );

      lightSourcesVBO = GL.GenBuffer ();
      GL.BindBuffer ( BufferTarget.ArrayBuffer, lightSourcesVBO );

      float[] billboard =
      {
        // positions                            // texture coords
        lightSize / 2, lightSize / 2, 0.0f,     1.0f, 0.0f,
        lightSize / 2, -lightSize / 2, 0.0f,    1.0f, 1.0f,
        -lightSize / 2, -lightSize / 2, 0.0f,   0.0f, 1.0f,
        -lightSize / 2, lightSize / 2, 0.0f,    0.0f, 0.0f,
      };

      GL.BufferData ( BufferTarget.ArrayBuffer, (IntPtr) ( billboard.Length * sizeof ( float ) ), billboard, BufferUsageHint.StaticDraw );
    }

    /// <summary>
    /// Initializes VBO for light sources
    /// X, Y, Z, u, v
    /// 1 quad per light source
    /// Coordinates are expected to change with every camera move - used as billboarding (technically only rotation is changed)
    /// Texture mapping is not expected to change
    /// </summary>
    private void InitializeLightSourcesVBO_OLD ()
    {
      if ( scene?.Sources == null )
        return;

      if ( lightSourcesVBO != 0 )
        GL.DeleteBuffer ( lightSourcesVBO );

      const int stride = 20 * sizeof ( float );

      Vector3d[] lightPositions = ( from lightSource in scene.Sources where lightSource.position != null select lightSource.position.Value ).ToArray ();

      lightSourcesVBO = GL.GenBuffer ();
      GL.BindBuffer ( BufferTarget.ArrayBuffer, lightSourcesVBO );

      GL.BufferData ( BufferTarget.ArrayBuffer, (IntPtr) ( lightPositions.Length * stride ), IntPtr.Zero, BufferUsageHint.DynamicDraw );

      for ( int i = 0; i < lightPositions.Length; i++ )
      {
        Vector3 corrected = (Vector3) RayVisualizer.AxesCorrector ( lightPositions [i] );

        float[] lightBillboard = 
        {
          // positions                                                                // texture coords
          corrected.X + lightSize / 2, corrected.Y + lightSize / 2, corrected.Z,      1.0f, 0.0f,
          corrected.X + lightSize / 2, corrected.Y - lightSize / 2, corrected.Z,      1.0f, 1.0f,
          corrected.X - lightSize / 2, corrected.Y - lightSize / 2, corrected.Z,      0.0f, 1.0f,
          corrected.X - lightSize / 2, corrected.Y + lightSize / 2, corrected.Z,      0.0f, 0.0f,
        };

        GL.BufferSubData ( BufferTarget.ArrayBuffer, (IntPtr) ( i * stride ), (IntPtr) stride, lightBillboard );      
      }

      usableLightSources = lightPositions.Length;
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
      if ( axesVBO != 0 )
        GL.DeleteBuffer ( axesVBO );

      axesVBO = GL.GenBuffer ();
      GL.BindBuffer ( BufferTarget.ArrayBuffer, axesVBO );

      Vector3[] points =
      {
        // start of line (XYZ) - color if start ( RGB ) - end of line ( XYZ) -color of end ( RGB)
        center, new Vector3 ( 1.0f, 0.1f, 0.1f ), center + new Vector3 ( 1.5f, 0.0f, 0.0f ) * diameter, new Vector3 ( 1.0f, 0.1f, 0.1f ), // Red axis
        center, new Vector3 ( 0.0f, 1.0f, 0.0f ), center + new Vector3 ( 0.0f, 1.5f, 0.0f ) * diameter, new Vector3 ( 0.0f, 1.0f, 0.0f ), // Green axis
        center, new Vector3 ( 0.2f, 0.2f, 1.0f ), center + new Vector3 ( 0.0f, 0.0f, 1.5f ) * diameter, new Vector3 ( 0.2f, 0.2f, 1.0f ), // Blue axis
      };

      GL.BufferData ( BufferTarget.ArrayBuffer, (IntPtr) ( points.Length * 3 * sizeof ( float ) ), points, BufferUsageHint.StaticDraw );
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
      if ( videoCameraVBO != 0 )
        GL.DeleteBuffer ( videoCameraVBO );

      videoCameraVBO = GL.GenBuffer ();
      GL.BindBuffer ( BufferTarget.ArrayBuffer, videoCameraVBO );

      float[] billboard =
      {
        // positions                                        // texture coords
        videoCameraSize / 2, videoCameraSize / 2, 0.0f,     1.0f, 0.0f,
        videoCameraSize / 2, -videoCameraSize / 2, 0.0f,    1.0f, 1.0f,
        -videoCameraSize / 2, -videoCameraSize / 2, 0.0f,   0.0f, 1.0f,
        -videoCameraSize / 2, videoCameraSize / 2, 0.0f,    0.0f, 0.0f,
      };

      GL.BufferData ( BufferTarget.ArrayBuffer, (IntPtr) ( billboard.Length * sizeof ( float ) ), billboard, BufferUsageHint.DynamicDraw );
    }

    private List<Vector3> rays = new List<Vector3>();
    private List<Vector3> shadowRays = new List<Vector3>();
    private readonly Vector3 rayColor = new Vector3 ( 1.0f, 0.0f, 0.0f );
    private readonly Vector3 shadowRayColor = new Vector3 ( 1.0f, 1.0f, 0.0f );
    private int raysVBO;
    /// <summary>
    /// Initializes VBO for normal and shadow rays
    /// X, Y, Z, R, G, B
    /// 2 points for each ray (start and end of line)
    /// None of the attributes are expected to change - rather whole VBO is re-allocated again when new rays are available
    /// Normal rays have different color than shadow rays
    /// </summary>
    /// <param name="newRays">List of Vector3 denoting position of normal rays - both start and end of each ray</param>
    /// <param name="newShadowRays">List of Vector3 denoting position of shadow rays - both start and end of each shadow ray</param>
    public void InitializeRaysVBO ( List<Vector3> newRays, List<Vector3> newShadowRays )
    {
      if ( raysVBO != 0 )
        GL.DeleteBuffer ( raysVBO );

      this.rays = newRays;
      this.shadowRays = newShadowRays;

      raysVBO = GL.GenBuffer ();
      GL.BindBuffer ( BufferTarget.ArrayBuffer, raysVBO );       

      Vector3[] temp = new Vector3[( newRays.Count + newShadowRays.Count ) * 2];

      for ( int i = 0; i < newRays.Count; i++ )
      {
        temp [i * 2] = newRays [i];
        temp [i * 2 + 1] = rayColor;
      }

      for ( int i = 0; i < newShadowRays.Count; i++ )
      {
        temp[newRays.Count * 2 + i * 2] = newShadowRays[i];
        temp[newRays.Count * 2 + i * 2 + 1] = shadowRayColor;
      }

      int size = ( newRays.Count + newShadowRays.Count ) * 3 * 2 * sizeof ( float );

      GL.BufferData ( BufferTarget.ArrayBuffer, (IntPtr) ( size ), temp, BufferUsageHint.StaticDraw );
    }
  }
}