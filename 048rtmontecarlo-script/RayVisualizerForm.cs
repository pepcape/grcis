using MathSupport;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Scene3D;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using OpenglSupport;
using System.Drawing;
using System.Globalization;


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

    /// <summary>
    /// Are we allowed to use VBO?
    /// </summary>
    bool useVBO = true;

    /// <summary>
    /// Can we use shaders?
    /// </summary>
    bool canShaders = false;

    /// <summary>
    /// Are we currently using shaders?
    /// </summary>
    bool useShaders = false;

    uint[] VBOid  = null; // vertex array (colors, normals, coords), index array
    int    stride = 0;    // stride for vertex array

    /// <summary>
    /// Current texture.
    /// </summary>
    int texName = 0;

    /// <summary>
    /// Global GLSL program repository.
    /// </summary>
    Dictionary<string, GlProgramInfo> programs = new Dictionary<string, GlProgramInfo> ();

    /// <summary>
    /// Current (active) GLSL program.
    /// </summary>
    GlProgram activeProgram = null;

    // appearance:
    Vector3 globalAmbient = new Vector3 ( 0.2f, 0.2f, 0.2f );
    Vector3 matAmbient    = new Vector3 ( 0.8f, 0.6f, 0.2f );
    Vector3 matDiffuse    = new Vector3 ( 0.8f, 0.6f, 0.2f );
    Vector3 matSpecular   = new Vector3 ( 0.8f, 0.8f, 0.8f );
    float   matShininess  = 100.0f;
    Vector3 whiteLight    = new Vector3 ( 1.0f, 1.0f, 1.0f );
    Vector3 lightPosition = new Vector3 ( -20.0f, 10.0f, 10.0f );

    long   lastFPSTime     = 0L;
    int    frameCounter    = 0;
    long   triangleCounter = 0L;
    double lastFPS         = 0.0;
    double lastTPS         = 0.0;

		public RayVisualizerForm ()
    {
      InitializeComponent ();

      Form1.singleton.RayVisualiserButton.Enabled = false;

      trackBall = new Trackball ( center, diameter );

      InitShaderRepository ();

      RayVisualizerForm.instance = this;

      RayVisualizer.singleton = new RayVisualizer ();

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

    /// <summary>
    /// Moves camera so that primary ray is perpendicular to screen
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void AllignCamera ( object sender, EventArgs e )
    {
      if ( RayVisualizer.singleton?.rays.Count < 2 )
      {
        return;
      }

      trackBall.Center = (Vector3) RayVisualizer.singleton.rays [ 1 ];
      trackBall.Reset ( (Vector3) ( RayVisualizer.singleton.rays [ 1 ] - RayVisualizer.singleton.rays [ 0 ] ) );

      double distanceOfEye = Vector3.Distance ( trackBall.Center, trackBall.Eye );
      double distanceOfCamera = Vector3d.Distance ( RayVisualizer.singleton.rays [ 1 ], RayVisualizer.singleton.rays [ 0 ]) * 0.9;

      trackBall.Zoom = (float) (distanceOfEye / distanceOfCamera);
    }


		void InitOpenGL ()
		{
			// log OpenGL info
			GlInfo.LogGLProperties ();

			// general OpenGL
			glControl1.VSync = true;
			GL.ClearColor ( Color.Black );
			GL.Enable ( EnableCap.DepthTest );
			GL.ShadeModel ( ShadingModel.Flat );

			// VBO initialization
			VBOid = new uint[2];
			GL.GenBuffers ( 2, VBOid );
			useVBO = ( GL.GetError () == ErrorCode.NoError );

			// shaders
			if ( useVBO )
				canShaders = SetupShaders ();

			// texture
			texName = GenerateTexture ();
		}

		/// <summary>
		/// Init shaders registered in global repository 'programs'.
		/// </summary>
		/// <returns>True if succeeded.</returns>
		bool SetupShaders ()
		{
			activeProgram = null;

			foreach ( var programInfo in programs.Values )
				if ( programInfo.Setup () )
					activeProgram = programInfo.program;

			if ( activeProgram == null )
				return false;

			GlProgramInfo defInfo;

			if ( programs.TryGetValue ( "default", out defInfo ) && defInfo.program != null )
				activeProgram = defInfo.program;

			return true;
		}

		/// <summary>
		/// De-allocated all the data associated with the given texture object.
		/// </summary>
		/// <param name="texName"></param>
		void DestroyTexture ( ref int texName )
		{
			int tHandle = texName;

			texName = 0;

			if ( tHandle != 0 )
				GL.DeleteTexture ( tHandle );
		}

		/// <summary>
		/// Generate static procedural texture.
		/// </summary>
		/// <returns>Texture handle.</returns>
		int GenerateTexture ()
		{
			// generated texture:
			const int TEX_SIZE         = 128;
			const int TEX_CHECKER_SIZE = 8;
			Vector3   colWhite         = new Vector3 ( 0.85f, 0.75f, 0.30f );
			Vector3   colBlack         = new Vector3 ( 0.15f, 0.15f, 0.60f );
			Vector3   colShade         = new Vector3 ( 0.15f, 0.15f, 0.15f );

			GL.PixelStore ( PixelStoreParameter.UnpackAlignment, 1 );
			int texName = GL.GenTexture ();
			GL.BindTexture ( TextureTarget.Texture2D, texName );

			Vector3[] data = new Vector3[TEX_SIZE * TEX_SIZE];

			for ( int y = 0; y < TEX_SIZE; y++ )
			{
				for ( int x = 0; x < TEX_SIZE; x++ )
				{
					int  i   = y * TEX_SIZE + x;
					bool odd = ( ( x / TEX_CHECKER_SIZE + y / TEX_CHECKER_SIZE ) & 1 ) > 0;
					data[i] = odd ? colBlack : colWhite;

					// add some fancy shading on the edges:
					if ( ( x % TEX_CHECKER_SIZE ) == 0 || ( y % TEX_CHECKER_SIZE ) == 0 )
						data[i] += colShade;
					if ( ( ( x + 1 ) % TEX_CHECKER_SIZE ) == 0 || ( ( y + 1 ) % TEX_CHECKER_SIZE ) == 0 )
						data[i] -= colShade;

					// add top-half texture markers:
					if ( y < TEX_SIZE / 2 )
					{
						if ( x % TEX_CHECKER_SIZE == TEX_CHECKER_SIZE / 2 && y % TEX_CHECKER_SIZE == TEX_CHECKER_SIZE / 2 )
							data[i] -= colShade;
					}
				}
			}

			GL.TexImage2D ( TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, TEX_SIZE, TEX_SIZE, 0, PixelFormat.Rgb,
							PixelType.Float, data );

			GL.TexParameter ( TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int) TextureWrapMode.Repeat );
			GL.TexParameter ( TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int) TextureWrapMode.Repeat );
			GL.TexParameter ( TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int) TextureMagFilter.Linear );
			GL.TexParameter ( TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int) TextureMagFilter.Linear );

			GlInfo.LogError ( "create-texture" );

			return texName;
		}

		/// <summary>
		/// Prepare VBO content and upload it to the GPU.
		/// </summary>
		void PrepareDataBuffers ()
		{
			if ( useVBO && scene != null && scene.Triangles > 0 )
			{
				// Vertex array: color [normal] coord
				GL.BindBuffer ( BufferTarget.ArrayBuffer, VBOid[0] );

				int vertexBufferSize = scene.VertexBufferSize ( true, true, true, true );

				GL.BufferData ( BufferTarget.ArrayBuffer, (IntPtr) vertexBufferSize, IntPtr.Zero, BufferUsageHint.StaticDraw );

				IntPtr videoMemoryPtr = GL.MapBuffer ( BufferTarget.ArrayBuffer, BufferAccess.WriteOnly );

				unsafe
				{
					stride = scene.FillVertexBuffer ( (float*) videoMemoryPtr.ToPointer (), true, true, true, true );
				}

				GL.UnmapBuffer ( BufferTarget.ArrayBuffer );
				GL.BindBuffer ( BufferTarget.ArrayBuffer, 0 );
				GlInfo.LogError ( "fill vertex-buffer" );

				// Index buffer
				GL.BindBuffer ( BufferTarget.ElementArrayBuffer, VBOid[1] );
				GL.BufferData ( BufferTarget.ElementArrayBuffer, (IntPtr) ( scene.Triangles * 3 * sizeof ( uint ) ),
								IntPtr.Zero, BufferUsageHint.StaticDraw );
				videoMemoryPtr = GL.MapBuffer ( BufferTarget.ElementArrayBuffer, BufferAccess.WriteOnly );

				unsafe
				{
					scene.FillIndexBuffer ( (uint*) videoMemoryPtr.ToPointer () );
				}

				GL.UnmapBuffer ( BufferTarget.ElementArrayBuffer );
				GL.BindBuffer ( BufferTarget.ElementArrayBuffer, 0 );
				GlInfo.LogError ( "fill index-buffer" );
			}
			else
			{
				if ( useVBO )
				{
					GL.BindBuffer ( BufferTarget.ArrayBuffer, VBOid[0] );
					GL.BufferData ( BufferTarget.ArrayBuffer, (IntPtr) 0, IntPtr.Zero, BufferUsageHint.StaticDraw );
					GL.BindBuffer ( BufferTarget.ArrayBuffer, 0 );
					GL.BindBuffer ( BufferTarget.ElementArrayBuffer, VBOid[1] );
					GL.BufferData ( BufferTarget.ElementArrayBuffer, (IntPtr) 0, IntPtr.Zero, BufferUsageHint.StaticDraw );
					GL.BindBuffer ( BufferTarget.ElementArrayBuffer, 0 );
				}
			}
		}

		/// <summary>
		/// Set light-source coordinate in the world-space.
		/// </summary>
		/// <param name="size">Relative size (based on the scene size).</param>
		/// <param name="light">Relative light position (default=[-2,1,1],viewer=[0,0,1]).</param>
		void SetLight ( float size, ref Vector3 light )
		{
			lightPosition = 2.0f * size * light;
		}

		void InitShaderRepository ()
		{
			programs.Clear ();
			GlProgramInfo pi;

			// default program:
			pi = new GlProgramInfo ( "default", new GlShaderInfo[]
			{
		new GlShaderInfo ( ShaderType.VertexShader, "vertex.glsl", "048rtmontecarlo-script" ),
		new GlShaderInfo ( ShaderType.FragmentShader, "fragment.glsl", "048rtmontecarlo-script" )
			} );

			programs[pi.name] = pi;
		}

		void DestroyShaders ()
		{
			foreach ( GlProgramInfo prg in programs.Values )
				prg.Destroy ();
		}

		private void Render ()
		{
			if ( !loaded )
				return;

			frameCounter++;
			useShaders = ( scene != null ) &&
						 scene.Triangles > 0 &&
						 useVBO &&
						 canShaders &&
						 activeProgram != null &&
						 checkShaders.Checked;

			GL.Clear ( ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit );
			GL.ShadeModel ( checkSmooth.Checked ? ShadingModel.Smooth : ShadingModel.Flat );
			GL.PolygonMode ( checkTwosided.Checked ? MaterialFace.FrontAndBack : MaterialFace.Front,
							 checkWireframe.Checked ? PolygonMode.Line : PolygonMode.Fill );

			if ( checkTwosided.Checked )
				GL.Disable ( EnableCap.CullFace );
			else
				GL.Enable ( EnableCap.CullFace );

			trackBall.GLsetCamera ();
			RenderScene ();

			glControl1.SwapBuffers ();
		}

		void Application_Idle ( object sender, EventArgs e )
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
					Vector2d uv;
					double   nearest = double.PositiveInfinity;

					if ( scene != null && scene.Triangles > 0 )
					{
						Vector3 A, B, C;

						for ( int i = 0; i < scene.Triangles; i++ )
						{
							scene.GetTriangleVertices ( i, out A, out B, out C );

							double curr = Geometry.RayTriangleIntersection ( ref p0, ref p1, ref A, ref B, ref C, out uv );

							if ( !double.IsInfinity ( curr ) && curr < nearest )
								nearest = curr;
						}
					}
					else
					{
						Vector3d ul   = new Vector3d ( -1.0, -1.0, -1.0 );
						Vector3d size = new Vector3d ( 2.0, 2.0, 2.0 );

						if ( Geometry.RayBoxIntersection ( ref p0, ref p1, ref ul, ref size, out uv ) )
							nearest = uv.X;
					}

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

		// attribute/vertex arrays:
		bool vertexAttribOn  = false;
		bool vertexPointerOn = false;

		private void SetVertexAttrib ( bool on )
		{
			if ( vertexAttribOn == on )
				return;

			if ( activeProgram != null )
				if ( on )
					activeProgram.EnableVertexAttribArrays ();
				else
					activeProgram.DisableVertexAttribArrays ();

			vertexAttribOn = on;
		}

		private void SetVertexPointer ( bool on )
		{
			if ( vertexPointerOn == on )
				return;

			if ( on )
			{
				GL.EnableClientState ( ArrayCap.VertexArray );

				if ( scene.TxtCoords > 0 )
					GL.EnableClientState ( ArrayCap.TextureCoordArray );

				if ( scene.Normals > 0 )
					GL.EnableClientState ( ArrayCap.NormalArray );

				if ( scene.Colors > 0 )
					GL.EnableClientState ( ArrayCap.ColorArray );
			}
			else
			{
				GL.DisableClientState ( ArrayCap.VertexArray );
				GL.DisableClientState ( ArrayCap.TextureCoordArray );
				GL.DisableClientState ( ArrayCap.NormalArray );
				GL.DisableClientState ( ArrayCap.ColorArray );
			}

			vertexPointerOn = on;
		}

		/// <summary>
		/// Rendering code itself (separated for clarity).
		/// </summary>
		void RenderScene ()
		{
			// Scene rendering:
			if ( scene != null && scene.Triangles > 0 && useVBO ) // scene is nonempty => render it
			{
				// [txt] [colors] [normals] vertices
				GL.BindBuffer ( BufferTarget.ArrayBuffer, VBOid[0] );
				IntPtr p = IntPtr.Zero;

				if ( useShaders )
				{
					SetVertexPointer ( false );
					SetVertexAttrib ( true );

					// using GLSL shaders:
					GL.UseProgram ( activeProgram.Id );

					// uniforms:
					Matrix4 modelView  = trackBall.ModelView;
					Matrix4 projection = trackBall.Projection;
					Vector3 eye        = trackBall.Eye;

					GL.UniformMatrix4 ( activeProgram.GetUniform ( "matrixModelView" ), false, ref modelView );
					GL.UniformMatrix4 ( activeProgram.GetUniform ( "matrixProjection" ), false, ref projection );

					GL.Uniform3 ( activeProgram.GetUniform ( "globalAmbient" ), ref globalAmbient );
					GL.Uniform3 ( activeProgram.GetUniform ( "lightColor" ), ref whiteLight );
					GL.Uniform3 ( activeProgram.GetUniform ( "lightPosition" ), ref lightPosition );
					GL.Uniform3 ( activeProgram.GetUniform ( "eyePosition" ), ref eye );
					GL.Uniform3 ( activeProgram.GetUniform ( "Ka" ), ref matAmbient );
					GL.Uniform3 ( activeProgram.GetUniform ( "Kd" ), ref matDiffuse );
					GL.Uniform3 ( activeProgram.GetUniform ( "Ks" ), ref matSpecular );
					GL.Uniform1 ( activeProgram.GetUniform ( "shininess" ), matShininess );

					// color handling:
					bool useGlobalColor = checkGlobalColor.Checked;
					if ( !scene.HasColors () )
						useGlobalColor = true;
					GL.Uniform1 ( activeProgram.GetUniform ( "globalColor" ), useGlobalColor ? 1 : 0 );

					// shading:
					bool shadingPhong   = checkPhong.Checked;
					bool shadingGouraud = checkSmooth.Checked;
					if ( !shadingGouraud )
						shadingPhong = false;
					GL.Uniform1 ( activeProgram.GetUniform ( "shadingPhong" ), shadingPhong ? 1 : 0 );
					GL.Uniform1 ( activeProgram.GetUniform ( "shadingGouraud" ), shadingGouraud ? 1 : 0 );
					GL.Uniform1 ( activeProgram.GetUniform ( "useAmbient" ), checkAmbient.Checked ? 1 : 0 );
					GL.Uniform1 ( activeProgram.GetUniform ( "useDiffuse" ), checkDiffuse.Checked ? 1 : 0 );
					GL.Uniform1 ( activeProgram.GetUniform ( "useSpecular" ), checkSpecular.Checked ? 1 : 0 );
					GlInfo.LogError ( "set-uniforms" );

					// texture handling:
					bool useTexture = checkTexture.Checked;
					if ( !scene.HasTxtCoords () || texName == 0 )
						useTexture = false;
					GL.Uniform1 ( activeProgram.GetUniform ( "useTexture" ), useTexture ? 1 : 0 );
					GL.Uniform1 ( activeProgram.GetUniform ( "texSurface" ), 0 );
					if ( useTexture )
					{
						GL.ActiveTexture ( TextureUnit.Texture0 );
						GL.BindTexture ( TextureTarget.Texture2D, texName );
					}

					GlInfo.LogError ( "set-texture" );

					if ( activeProgram.HasAttribute ( "texCoords" ) )
						GL.VertexAttribPointer ( activeProgram.GetAttribute ( "texCoords" ), 2, VertexAttribPointerType.Float,
												 false, stride, p );
					if ( scene.HasTxtCoords () )
						p += Vector2.SizeInBytes;

					if ( activeProgram.HasAttribute ( "color" ) )
						GL.VertexAttribPointer ( activeProgram.GetAttribute ( "color" ), 3, VertexAttribPointerType.Float, false,
												 stride, p );
					if ( scene.HasColors () )
						p += Vector3.SizeInBytes;

					if ( activeProgram.HasAttribute ( "normal" ) )
						GL.VertexAttribPointer ( activeProgram.GetAttribute ( "normal" ), 3, VertexAttribPointerType.Float, false,
												 stride, p );
					if ( scene.HasNormals () )
						p += Vector3.SizeInBytes;

					GL.VertexAttribPointer ( activeProgram.GetAttribute ( "position" ), 3, VertexAttribPointerType.Float, false,
											 stride, p );
					GlInfo.LogError ( "set-attrib-pointers" );

					// index buffer
					GL.BindBuffer ( BufferTarget.ElementArrayBuffer, VBOid[1] );

					// engage!
					GL.DrawElements ( PrimitiveType.Triangles, scene.Triangles * 3, DrawElementsType.UnsignedInt, IntPtr.Zero );
					GlInfo.LogError ( "draw-elements-shader" );

					// cleanup:
					GL.UseProgram ( 0 );
					if ( useTexture )
						GL.BindTexture ( TextureTarget.Texture2D, 0 );
				}
				else
				{
					SetVertexAttrib ( false );
					SetVertexPointer ( true );

					// texture handling:
					bool useTexture = checkTexture.Checked;
					if ( !scene.HasTxtCoords () || texName == 0 )
						useTexture = false;
					if ( useTexture )
					{
						GL.Enable ( EnableCap.Texture2D );
						GL.ActiveTexture ( TextureUnit.Texture0 );
						GL.BindTexture ( TextureTarget.Texture2D, texName );
						GL.TexEnv ( TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, (int) TextureEnvMode.Replace );
					}

					// using FFP:
					if ( scene.HasTxtCoords () )
					{
						GL.TexCoordPointer ( 2, TexCoordPointerType.Float, stride, p );
						p += Vector2.SizeInBytes;
					}

					if ( scene.HasColors () )
					{
						GL.ColorPointer ( 3, ColorPointerType.Float, stride, p );
						p += Vector3.SizeInBytes;
					}

					if ( scene.HasNormals () )
					{
						GL.NormalPointer ( NormalPointerType.Float, stride, p );
						p += Vector3.SizeInBytes;
					}

					GL.VertexPointer ( 3, VertexPointerType.Float, stride, p );

					// index buffer
					GL.BindBuffer ( BufferTarget.ElementArrayBuffer, VBOid[1] );

					// engage!
					GL.DrawElements ( PrimitiveType.Triangles, scene.Triangles * 3, DrawElementsType.UnsignedInt, IntPtr.Zero );
					GlInfo.LogError ( "draw-elements-ffp" );

					if ( useTexture )
					{
						GL.BindTexture ( TextureTarget.Texture2D, 0 );
						GL.Disable ( EnableCap.Texture2D );
					}
				}

				triangleCounter += scene.Triangles;
			}
			else // actual scene and ray visualization
			{
				bool renderFirst = true;

				if ( AllignCameraCheckBox.Checked )
				{
					AllignCamera ( null, null );
					renderFirst = false;
				}

				FillSceneObjects ();
				BoundingBoxesVisualization ();

				RenderRays ( renderFirst );
				RenderCamera ();
				RenderLightSources ();
			}

			// Support: axes
			if ( checkAxes.Checked )
			{
				float origWidth = GL.GetFloat ( GetPName.LineWidth );
				float origPoint = GL.GetFloat ( GetPName.PointSize );

				// axes:
				RenderAxes ();

				// Support: pointing
				if ( pointOrigin != null )
				{
					RenderPointing ();
				}

				// Support: frustum
				if ( frustumFrame.Count >= 8 )
				{
					RenderFrustum ();
				}

				GL.LineWidth ( origWidth );
				GL.PointSize ( origPoint );
			}
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

		private void RenderAxes ()
		{
			GL.LineWidth ( 2.0f );
			GL.Begin ( PrimitiveType.Lines );

			GL.Color3 ( 1.0f, 0.1f, 0.1f );
			GL.Vertex3 ( center );
			GL.Vertex3 ( center + new Vector3 ( 1.5f, 0.0f, 0.0f ) * diameter );

			GL.Color3 ( 0.0f, 1.0f, 0.0f );
			GL.Vertex3 ( center );
			GL.Vertex3 ( center + new Vector3 ( 0.0f, 1.5f, 0.0f ) * diameter );

			GL.Color3 ( 0.2f, 0.2f, 1.0f );
			GL.Vertex3 ( center );
			GL.Vertex3 ( center + new Vector3 ( 0.0f, 0.0f, 1.5f ) * diameter );

			GL.End ();
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
		private void RenderRays ( bool renderFirst )
		{
			SetVertexPointer ( false ); // ??
			SetVertexAttrib ( false );

			GL.LineWidth ( 2.0f );

			GL.Begin ( PrimitiveType.Lines );

			if ( NormalRaysCheckBox.Checked ) // Render normal rays
			{
				int offset = 0;

				if ( !renderFirst )
				{
					offset = 2;
				}

				GL.Color3 ( Color.Red );
				for ( int i = offset; i < RayVisualizer.singleton.rays.Count; i += 2 )
				{
					GL.Vertex3 ( RayVisualizer.singleton.rays[i] );
					GL.Vertex3 ( RayVisualizer.singleton.rays[i + 1] );
				}
			}

			if ( ShadowRaysCheckBox.Checked ) // Render shadow rays
			{
				GL.Color3 ( Color.Yellow );
				for ( int i = 0; i < RayVisualizer.singleton.shadowRays.Count; i += 2 )
				{
					GL.Vertex3 ( RayVisualizer.singleton.shadowRays[i] );
					GL.Vertex3 ( RayVisualizer.singleton.shadowRays[i + 1] );
				}
			}

			GL.End ();
		}

		/// <summary>
		/// Renders representation of camera (initially at position of rayOrigin of first primary ray)
		/// </summary>
		private void RenderCamera ()
		{
			if ( RayVisualizer.singleton.rays.Count == 0 || !CameraCheckBox.Checked )
			{
				return;
			}

			RenderCube ( RayVisualizer.singleton.rays[0], 0.2f, Color.Turquoise );
		}

		/// <summary>
		/// Renders representation of all light sources (except those in with null as position - usually ambient and directional lights which position does not matter)
		/// </summary>
		private void RenderLightSources ()
		{
			if ( /*RayVisualizer.instance?.rays.Count == 0 || */ rayScene?.Sources == null || !LightSourcesCheckBox.Checked )
			{
				return;
			}

			foreach ( ILightSource lightSource in rayScene.Sources )
			{
				if ( lightSource.position != null )
				{
					RenderCube ( RayVisualizer.AxesCorrector ( lightSource.position ), 0.07f, Color.Yellow );
				}
			}
		}

		/// <summary>
		/// Renders simple cube of uniform color
		/// Initially used as placeholder so several objects
		/// </summary>
		/// <param name="position">Position in space</param>
		/// <param name="size">Size of cube</param>
		/// <param name="color">Uniform color of cube</param>
		private void RenderCube ( Vector3d position, float size, Color color )
		{
			SetVertexPointer ( false );
			SetVertexAttrib ( false );

			GL.Begin ( PrimitiveType.Quads );
			GL.Color3 ( color );


			GL.Vertex3 ( ( new Vector3d ( 1.0f, 1.0f, -1.0f ) * size + position ) );  // Top Right Of The Quad (Top)
			GL.Vertex3 ( ( new Vector3d ( -1.0f, 1.0f, -1.0f ) * size + position ) ); // Top Left Of The Quad (Top)
			GL.Vertex3 ( ( new Vector3d ( -1.0f, 1.0f, 1.0f ) * size + position ) );  // Bottom Left Of The Quad (Top)
			GL.Vertex3 ( ( new Vector3d ( 1.0f, 1.0f, 1.0f ) * size + position ) );   // Bottom Right Of The Quad (Top)

			GL.Vertex3 ( ( new Vector3d ( 1.0f, -1.0f, 1.0f ) * size + position ) );   // Top Right Of The Quad (Bottom)
			GL.Vertex3 ( ( new Vector3d ( -1.0f, -1.0f, 1.0f ) * size + position ) );  // Top Left Of The Quad (Bottom)
			GL.Vertex3 ( ( new Vector3d ( -1.0f, -1.0f, -1.0f ) * size + position ) ); // Bottom Left Of The Quad (Bottom)
			GL.Vertex3 ( ( new Vector3d ( 1.0f, -1.0f, -1.0f ) * size + position ) );  // Bottom Right Of The Quad (Bottom)

			GL.Vertex3 ( ( new Vector3d ( 1.0f, 1.0f, 1.0f ) * size + position ) );   // Top Right Of The Quad (Front)
			GL.Vertex3 ( ( new Vector3d ( -1.0f, 1.0f, 1.0f ) * size + position ) );  // Top Left Of The Quad (Front)
			GL.Vertex3 ( ( new Vector3d ( -1.0f, -1.0f, 1.0f ) * size + position ) ); // Bottom Left Of The Quad (Front)
			GL.Vertex3 ( ( new Vector3d ( 1.0f, -1.0f, 1.0f ) * size + position ) );  // Bottom Right Of The Quad (Front)

			GL.Vertex3 ( ( new Vector3d ( 1.0f, -1.0f, -1.0f ) * size + position ) );  // Bottom Left Of The Quad (Back)
			GL.Vertex3 ( ( new Vector3d ( -1.0f, -1.0f, -1.0f ) * size + position ) ); // Bottom Right Of The Quad (Back)
			GL.Vertex3 ( ( new Vector3d ( -1.0f, 1.0f, -1.0f ) * size + position ) );  // Top Right Of The Quad (Back)
			GL.Vertex3 ( ( new Vector3d ( 1.0f, 1.0f, -1.0f ) * size + position ) );   // Top Left Of The Quad (Back)

			GL.Vertex3 ( ( new Vector3d ( -1.0f, 1.0f, 1.0f ) * size + position ) );   // Top Right Of The Quad (Left)
			GL.Vertex3 ( ( new Vector3d ( -1.0f, 1.0f, -1.0f ) * size + position ) );  // Top Left Of The Quad (Left)
			GL.Vertex3 ( ( new Vector3d ( -1.0f, -1.0f, -1.0f ) * size + position ) ); // Bottom Left Of The Quad (Left)
			GL.Vertex3 ( ( new Vector3d ( -1.0f, -1.0f, 1.0f ) * size + position ) );  // Bottom Right Of The Quad (Left)

			GL.Vertex3 ( ( new Vector3d ( 1.0f, 1.0f, -1.0f ) * size + position ) );  // Top Right Of The Quad (Right)
			GL.Vertex3 ( ( new Vector3d ( 1.0f, 1.0f, 1.0f ) * size + position ) );   // Top Left Of The Quad (Right)
			GL.Vertex3 ( ( new Vector3d ( 1.0f, -1.0f, 1.0f ) * size + position ) );  // Bottom Left Of The Quad (Right)
			GL.Vertex3 ( ( new Vector3d ( 1.0f, -1.0f, -1.0f ) * size + position ) ); // Bottom Right Of The Quad (Right)

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

			GL.PolygonMode ( checkTwosided.Checked ? MaterialFace.FrontAndBack : MaterialFace.Front,
							 WireframeBoundingBoxesCheckBox.Checked ? PolygonMode.Line : PolygonMode.Fill );

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

			GL.PolygonMode ( checkTwosided.Checked ? MaterialFace.FrontAndBack : MaterialFace.Front,
							 checkWireframe.Checked ? PolygonMode.Line : PolygonMode.Fill );

			triangleCounter += 12;
		}

		/// <summary>
		/// Applies tranformation matrix to vector
		/// </summary>
		/// <param name="vector">1x3 vector</param>
		/// <param name="transformation">4x4 transformation matrix</param>
		/// <returns>Transformed vector 1x3</returns>
		public Vector3d ApplyTransformation ( Vector3d vector, Matrix4d transformation )
		{
			Vector4d transformedVector = MultiplyVectorByMatrix ( new Vector4d ( vector, 1 ), transformation ); //( vector, 1 ) is extention [x  y  z] -> [x  y  z  1]

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


		List<SceneObject> sceneObjects;
		IRayScene rayScene;
		/// <summary>
		/// Fills sceneObjects list with objects from current scene
		/// </summary>
		private void FillSceneObjects ()
		{
			if ( RayVisualizer.rayScene == rayScene ) // prevents filling whole list in case scene did not change (most of the time)
			{
				return;
			}
			else
			{
				rayScene = RayVisualizer.rayScene;
			}

			DefaultSceneNode root = rayScene.Intersectable as DefaultSceneNode;

			if ( root == null )
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
			foreach ( DefaultSceneNode children in parent.Children )
			{
				Matrix4d localTransformation = children.ToParent * transformation;


				double[] newColor;

				// take color from child's attribute COLOR, child's attribute MATERIAL.Color or from parent
				if ( (double[]) children.GetAttribute ( PropertyName.COLOR ) == null )
				{
					if ( ( (IMaterial) children.GetAttribute ( PropertyName.MATERIAL ) ).Color == null )
					{
						newColor = color;
					}
					else
					{
						newColor = ( (IMaterial) children.GetAttribute ( PropertyName.MATERIAL ) ).Color;
					}
				}
				else
				{
					newColor = (double[]) children.GetAttribute ( PropertyName.COLOR );
				}


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
		class SceneObject
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
	}
}