// author: Josef Pelikan

#define USE_INVALIDATE

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using Cloo;
using MathSupport;
using OpenclSupport;
using OpenglSupport;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Raster;
using Utilities;

namespace _090opencl
{
  public class Mandelbrot
  {
    public static void InitParams ( out string param )
    {
      param = "center=[-0.5;0.0],radius=1.5,iter=200";
    }

    Form1 form;

    public Mandelbrot ( Form1 f )
    {
      form = f;
    }

    /// <summary>
    /// View center point.
    /// </summary>
    public Vector2d center = new Vector2d( -0.5, 0.0 );

    /// <summary>
    /// View radius.
    /// </summary>
    public double radius = 1.5;

    /// <summary>
    /// Maximum number of Mandelbrot iterations.
    /// </summary>
    public int iter = 200;

    /// <summary>
    /// Shared x/y scale factor (x/y units per pixel).
    /// </summary>
    double dxy = 0.0;

    /// <summary>
    /// Local working buffer for the SW implementation.
    /// </summary>
    protected byte[] swBuffer = null;

    protected void assertBuffer ( int width, int height )
    {
      int len = width * height * 4;
      if ( swBuffer != null &&
           swBuffer.Length >= len )
        return;

      swBuffer = new byte[ len ];
    }

    /// <summary>
    /// Update Mandelbrot parameters.
    /// </summary>
    /// <param name="param">User-provided parameter string.</param>
    public void UpdateParams ( string param )
    {
      // input params:
      Dictionary<string, string> p = Util.ParseKeyValueList( param );
      if ( p.Count == 0 )
        return;

      // mandelbrot: center point
      Geometry.TryParse( p, "center", ref center );

      // Mandelbrot: view radius
      if ( Util.TryParse( p, "radius", ref radius ) )
        radius = Arith.Clamp( radius, 1.0e-7, 4.0 );

      // Mandelbrot: iterations
      if ( Util.TryParse( p, "iter", ref iter ) )
        iter = Arith.Clamp( iter, 2, 10000 );
    }

    public string CurrentParam ()
    {
      return string.Format( CultureInfo.InvariantCulture, "center=[{0};{1}],radius={2},iter={3}",
                            center.X, center.Y, radius, iter );
    }

    public string ResetView ()
    {
      center = new Vector2d( -0.5, 0.0 );
      radius = 1.5;

      return CurrentParam();
    }

    public string UpdateRadius ( double coeff )
    {
      // check radius bounds:
      radius = Arith.Clamp( radius * coeff, 1.0e-8, 4.0 );

      return CurrentParam();
    }

    public string UpdateCenter ( int dx, int dy )
    {
      center.X += dx * dxy;
      center.Y -= dy * dxy;

      return CurrentParam();
    }

    public void ComputeSW ( int texName, int width, int height, byte[] colormap,
                            bool useDouble, bool drawPalette )
    {
      assertBuffer( width, height );

      dxy = (radius + radius) / Math.Min( width, height );
      double xOrig = center.X - width  * 0.5 * dxy;
      double yOrig = center.Y - height * 0.5 * dxy;

      int xi, yi;
      int bufi = 0;
      double yd = yOrig;
      for ( yi = 0; yi++ < height; yd += dxy )
      {
        double xd = xOrig;
        for ( xi = 0; xi++ < width; bufi += 4, xd += dxy )
        {
          int m;

          if ( drawPalette &&
               xi >= width - 5 )
            m = (yi * colormap.Length) / (height * 4);
          else
          {
            // compute one pixel
            m = useDouble ? mandelbrotDouble( xd, yd, iter ) :
                            mandelbrotSingle( (float)xd, (float)yd, iter );
            m = m > 0 ? iter - m : 0;
          }

          if ( m <= 0 )
            swBuffer[ bufi ] =
            swBuffer[ bufi + 1 ] =
            swBuffer[ bufi + 2 ] =
            swBuffer[ bufi + 3 ] = 0;
          else
          {
            m = (m * 4) % colormap.Length;
            swBuffer[ bufi ]     = colormap[ m ];
            swBuffer[ bufi + 1 ] = colormap[ m + 1 ];
            swBuffer[ bufi + 2 ] = colormap[ m + 2 ];
            swBuffer[ bufi + 3 ] = colormap[ m + 3 ];
          }
        }
      }

      GL.BindTexture( TextureTarget.Texture2D, texName );
      GL.TexSubImage2D( TextureTarget.Texture2D, 0, 0, 0, width, height, PixelFormat.Rgba, PixelType.UnsignedByte, swBuffer );
      Debug.Assert( GL.GetError() == ErrorCode.NoError, "glTexSubImage2D" );
      GL.BindTexture( TextureTarget.Texture2D, 0 );
    }

    protected int mandelbrotSingle ( float x0, float y0, int iter )
    {
      float y = y0;
      float x = x0;
      float yy = y * y;
      float xx = x * x;
      int i = iter;

      while ( --i > 0 &&
              xx + yy < 4.0f )
      {
        y = x * y * 2.0f + y0;
        x = xx - yy + x0;
        yy = y * y;
        xx = x * x;
      }

      return i;
    }

    protected int mandelbrotDouble ( double x0, double y0, int iter )
    {
      double y = y0;
      double x = x0;
      double yy = y * y;
      double xx = x * x;
      int i = iter;

      while ( --i > 0 &&
              xx + yy < 4.0 )
      {
        y = x * y * 2.0 + y0;
        x = xx - yy + x0;
        yy = y * y;
        xx = x * x;
      }

      return i;
    }

    public void ComputeCL ( ComputeContext clContext, int texName, int width, int height, byte[] colormap,
                            bool useDouble, bool useInterop )
    {
      assertBuffer( width, height );

      dxy = (radius + radius) / Math.Min( width, height );
      double xOrig = center.X - width  * 0.5 * dxy;
      double yOrig = center.Y - height * 0.5 * dxy;

      form.PrepareClBuffers( false );

      if ( form.clDirty )
        return;               // something went wrong..

      if ( form.clImage == null )
        useInterop = false;

      // output buffer: OpenCL buffer (direct data write to GL texture was failing so I gave up)
      form.clKernel.SetMemoryArgument( 0, form.result );

      form.clKernel.SetValueArgument(  1, width );
      form.clKernel.SetValueArgument(  2, height );
      form.clKernel.SetValueArgument(  3, iter );
      if ( form.CanUseDouble )
      {
        form.clKernel.SetValueArgument( 4, xOrig );
        form.clKernel.SetValueArgument( 5, yOrig );
        form.clKernel.SetValueArgument( 6, dxy );
      }
      else
      {
        form.clKernel.SetValueArgument( 4, (float)xOrig );
        form.clKernel.SetValueArgument( 5, (float)yOrig );
        form.clKernel.SetValueArgument( 6, (float)dxy );
      }
      form.clKernel.SetMemoryArgument( 7, form.cmap );
      form.clKernel.SetValueArgument(  8, colormap.Length / 4 );

      form.clCommands.Execute( form.clKernel, null,
                               new long[] { form.globalWidth, form.globalHeight },
                               new long[] { form.groupSize,   form.groupSize },
                               null );
      if ( useInterop )
      {
        // with (limited) OpenGL interop:

        // 1. acquire ownership of GL texture for OpenCL image
        List<ComputeMemory> images = new List<ComputeMemory>() { form.clImage };
        form.clCommands.AcquireGLObjects( images, null );

        // 2. copy data from the result buffer to the texture
        form.clCommands.CopyBufferToImage( form.result, (ComputeImage)form.clImage, null );

        // 3. release ownership of the texture
        form.clCommands.ReleaseGLObjects( images, null );

        // 4. force pending CL commands to get executed
        form.clCommands.Finish();
      }
      else
      {
        // w/o OpenGL interop:

        // 1. read data from GPU
        form.clCommands.ReadFromBuffer( form.result, ref swBuffer, false, null );

        // 2. force pending CL commands to get executed
        form.clCommands.Finish();

        // 3. write data back to OpenGL texture
        GL.BindTexture( TextureTarget.Texture2D, texName );
        GL.TexSubImage2D( TextureTarget.Texture2D, 0, 0, 0, width, height, PixelFormat.Rgba, PixelType.UnsignedByte, swBuffer );
        Debug.Assert( GL.GetError() == ErrorCode.NoError, "glTexSubImage2D" );
        GL.BindTexture( TextureTarget.Texture2D, 0 );
      }
    }
  }

  public partial class Form1
  {
    /// <summary>
    /// Current texture.
    /// </summary>
    int texName = 0;

    /// <summary>
    /// Current texture's width in pixels.
    /// </summary>
    int texWidth = 0;

    /// <summary>
    /// Current texture's height in pixels.
    /// </summary>
    int texHeight = 0;

    /// <summary>
    /// Colormap in RGBA format.
    /// </summary>
    byte[] colormap = null;

    /// <summary>
    /// Global GLSL program repository.
    /// </summary>
    Dictionary<string, GlProgramInfo> programs = new Dictionary<string, GlProgramInfo>();

    /// <summary>
    /// Current (active) GLSL program.
    /// </summary>
    GlProgram activeProgram = null;

    //--- OpenCL global variables ---

    /// <summary>
    /// OpenCL image bound to OpenGL texture.
    /// </summary>
    public ComputeMemory clImage = null;

    public ComputeBuffer<byte> result = null;

    public ComputeBuffer<byte> cmap = null;

    ComputeProgram clProgram = null;

    public ComputeKernel clKernel = null;

    public ComputeCommandQueue clCommands = null;

    public long globalWidth = 1;

    public long globalHeight = 1;

    public long groupSize = 8L;

    //--- Form global variables ---

    long lastFpsTime = 0L;
    int frameCounter = 0;
    long pixelCounter = 0L;
    double lastFps = 0.0;
    double lastPps = 0.0;
    double lastCompute = 0.0;
    double computeCounter = 0.0f;

    /// <summary>
    /// Function called whenever the main application is idle..
    /// It actually contains the redraw-loop.
    /// </summary>
    void Application_Idle ( object sender, EventArgs e )
    {
      while ( glControl1.IsIdle )
      {
#if USE_INVALIDATE
        glControl1.Invalidate();
#else
        glControl1.MakeCurrent();
        ComputeRender();
#endif

        long now = DateTime.Now.Ticks;
        if ( now - lastFpsTime > 5000000 )      // more than 0.5 sec
        {
          lastFps = 0.5 * lastFps + 0.5 * (frameCounter * 1.0e7 / (now - lastFpsTime));
          lastPps = 0.5 * lastPps + 0.5 * (pixelCounter * 1.0e7 / (now - lastFpsTime));
          lastCompute = 0.5 * lastCompute + 0.5 * computeCounter / frameCounter;
          lastFpsTime = now;
          frameCounter = 0;
          pixelCounter = 0L;
          computeCounter = 0.0;

          bool isDouble = checkDouble.Checked && (!checkOpenCL.Checked || CanUseDouble);
          bool isInterop = checkInterop.Checked && CanUseInterop && (clImage != null);
          labelFps.Text = string.Format( CultureInfo.InvariantCulture, "Fps: {0:f1}, pps: {1:f1} MPx/s ({2}{3}), compute: {4:f2} ms",
                                         lastFps, (lastPps * 1.0e-6), isDouble ? "double" : "single",
                                         isInterop ? ",gl" : "", (lastCompute * 1000.0) );
          string clStat = checkOpenCL.Checked ? string.Format( ", {0} grps {1}x{1}", (globalWidth * globalHeight) / (groupSize * groupSize), groupSize ) : "";
          labelSize.Text = string.Format( "{0}x{1}px{2}", texWidth, texHeight, clStat );
        }
      }
    }

    /// <summary>
    /// OpenGL init code (cold init).
    /// </summary>
    void InitOpenGL ()
    {
      // log OpenGL info just for curiosity:
      GlInfo.LogGLProperties();

      // general OpenGL:
      glControl1.VSync = true;
      GL.ClearColor( Color.Black );
      GL.Disable( EnableCap.DepthTest );
      GL.ShadeModel( ShadingModel.Flat );

      // shaders:
      if ( programs.Count > 0 )
        SetupShaders();

      // texture:
      GL.Enable( EnableCap.Texture2D );
      GL.ActiveTexture( TextureUnit.Texture0 );
      GL.TexEnv( TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, (int)TextureEnvMode.Replace );
      ResizeTexture( 10, 10 );

      // colormap:
      int colors = 256;
      colormap = new byte[ colors * 4 ];
      double x = 0.04;
      double dx = 0.96 / colors;
      for ( int i = 0; i < colors * 4; x += dx )
      {
        Color col = Draw.ColorRamp( x );
        colormap[ i++ ] = col.R;
        colormap[ i++ ] = col.G;
        colormap[ i++ ] = col.B;
        colormap[ i++ ] = 0;
      }
    }

    /// <summary>
    /// Init shaders registered in global repository 'programs'.
    /// </summary>
    /// <returns>True if succeeded.</returns>
    bool SetupShaders ()
    {
      activeProgram = null;

      foreach ( var programInfo in programs.Values )
        if ( programInfo.Setup() )
          activeProgram = programInfo.program;

      if ( activeProgram == null )
        return false;

      GlProgramInfo defInfo;
      if ( programs.TryGetValue( "default", out defInfo ) &&
           defInfo.program != null )
        activeProgram = defInfo.program;

      return true;
    }

    /// <summary>
    /// Resize the texture object.
    /// </summary>
    void ResizeTexture ( int width, int height )
    {
      // check the texture name:
      if ( texName == 0 )
      {
        texName = GL.GenTexture();
        texWidth = 0;
      }

      if ( texWidth  == width &&
           texHeight == height )
        return;

      texWidth  = width;
      texHeight = height;

      GL.BindTexture( TextureTarget.Texture2D, texName );

      GL.PixelStore( PixelStoreParameter.UnpackAlignment, 1 );
      GL.PixelStore( PixelStoreParameter.PackAlignment, 1 );
      GL.TexImage2D( TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba8, texWidth, texHeight, 0, PixelFormat.Rgba, PixelType.UnsignedByte, (IntPtr)0 );
      Debug.Assert( GL.GetError() == ErrorCode.NoError, "glTexImage2D" );

      GL.TexParameter( TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge );
      GL.TexParameter( TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge );
      GL.TexParameter( TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest );
      GL.TexParameter( TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMagFilter.Nearest );

      GlInfo.LogError( "resize-texture" );

      PrepareClBuffers();
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
        GL.DeleteTexture( tHandle );
    }

    /// <summary>
    /// Prepare OpenCL program, data buffers, etc.
    /// </summary>
    public void PrepareClBuffers ( bool dirty =true )
    {
      clDirty = clDirty || dirty;

      if ( texName == 0 ||
           !checkOpenCL.Checked )
      {
        DestroyClBuffers();
        return;
      }

      if ( !clDirty )
        return;

      DestroyClBuffers();

      if ( clContext == null )
        SetupClContext();
      if ( clContext == null )   // to be sure
      {
        Util.Log( "OpenCL error" );
        clImage = null;
        clDirty = true;
        return;
      }

      GL.BindTexture( TextureTarget.Texture2D, 0 );
      GL.Finish();
      try
      {
        // OpenCL C source:
        string src = ClInfo.ReadSourceFile( CanUseDouble ? "mandel.cl" : "mandelSingle.cl", "090opencl" );
        if ( string.IsNullOrEmpty( src ) )
          return;

        // program & kernel:
        clProgram = new ComputeProgram( clContext, src );
        clProgram.Build( clContext.Devices, null, null, IntPtr.Zero );
        clKernel = clProgram.CreateKernel( (checkDouble.Checked && CanUseDouble) ? "mandelDouble" : "mandelSingle" );
        clCommands = new ComputeCommandQueue( clContext, clContext.Devices[ 0 ], ComputeCommandQueueFlags.None );
        globalWidth  = (texWidth  + groupSize - 1) & -groupSize;
        globalHeight = (texHeight + groupSize - 1) & -groupSize;

        // buffers:
        // 1. colormap array
        cmap = new ComputeBuffer<byte>( clContext, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.CopyHostPointer, colormap );

        bool interopOk = checkInterop.Checked;
        if ( interopOk )
        {
          // 2. CL image for OpenGL interop
          clImage = ComputeImage2D.CreateFromGLTexture2D( clContext, ComputeMemoryFlags.ReadWrite, (int)TextureTarget.Texture2D, 0, texName );
          if ( clImage == null )
          {
            Util.Log( "OpenCL cannot reference OpenGL texture!" );
            interopOk = false;
          }
        }

        // 3. CL output array
        result = new ComputeBuffer<byte>( clContext, ComputeMemoryFlags.ReadWrite, texWidth * texHeight * 4 );

        // synced..
        clDirty = false;
      }
      catch ( Exception exc )
      {
        Util.LogFormat( "OpenCL build error: {0}", exc.Message );
        clImage = null;
        clDirty = true;
      }
    }

    void DestroyClBuffers ()
    {
      if ( clImage != null )
      {
        clImage.Dispose();
        clImage = null;
      }

      if ( result != null )
      {
        result.Dispose();
        result = null;
      }
      if ( cmap != null )
      {
        cmap.Dispose();
        cmap = null;
      }
      if ( clCommands != null )
      {
        clCommands.Dispose();
        clCommands = null;
      }
      if ( clKernel != null )
      {
        clKernel.Dispose();
        clKernel = null;
      }
      if ( clProgram != null )
      {
        clProgram.Dispose();
        clProgram = null;
      }

      clDirty = true;
    }

    /// <summary>
    /// Sets up a projective viewport
    /// </summary>
    private void SetupViewport ()
    {
      int width  = glControl1.Width;
      int height = glControl1.Height;

      // 1. resize the texture:
      ResizeTexture( width, height );

      // 2. set ViewPort transform:
      GL.Viewport( 0, 0, width, height );

      // 3. set projection matrix
      GL.MatrixMode( MatrixMode.Projection );
      GL.LoadIdentity();        // NDS: [-1.0,-1.0] to [1.0,1.0]
    }

    int dragX = -1;
    int dragY = -1;

    private void glControl1_MouseDown ( object sender, MouseEventArgs e )
    {
      if ( e.Button != MouseButtons.Left )
        return;

      dragX = e.X;
      dragY = e.Y;
    }

    private void glControl1_MouseUp ( object sender, MouseEventArgs e )
    {
      if ( e.Button != MouseButtons.Left )
        return;

      dragX = dragY = -1;
    }

    private void glControl1_MouseMove ( object sender, MouseEventArgs e )
    {
      if ( e.Button != MouseButtons.Left )
        return;

      if ( e.X == dragX &&
           e.Y == dragY )
        return;

      textParam.Text = mandel.UpdateCenter( dragX - e.X, dragY - e.Y );
      dragX = e.X;
      dragY = e.Y;
    }

    private void glControl1_MouseWheel ( object sender, MouseEventArgs e )
    {
      float dZoom = -e.Delta / 120.0f;

      textParam.Text = mandel.UpdateRadius( Math.Pow( 1.05, dZoom ) );
    }

    private void glControl1_KeyDown ( object sender, KeyEventArgs e )
    {
      //if ( e.KeyCode == Keys.O )
      //  handleKeyO();
    }

    private void glControl1_KeyUp ( object sender, KeyEventArgs e )
    {
      //if ( e.KeyCode == Keys.O )
      //  handleKeyO();
    }

    private void buttonReset_Click ( object sender, EventArgs e )
    {
      textParam.Text = mandel.ResetView();
    }

    /// <summary>
    /// Fill the shader-repository.
    /// </summary>
    void InitShaderRepository ()
    {
      programs.Clear();
#if false   // by default we don't need any shaders
      GlProgramInfo pi;

      // default program:
      pi = new GlProgramInfo( "default", new GlShaderInfo[] {
        new GlShaderInfo( ShaderType.VertexShader, "vertex.glsl", "090opencl" ),
        new GlShaderInfo( ShaderType.FragmentShader, "fragment.glsl", "090opencl" ) } );
      programs[ pi.name ] = pi;

      // put more programs here:
      // pi = new GlProgramInfo( ..
      //   ..
      // programs[ pi.name ] = pi;
#endif
    }

    void DestroyShaders ()
    {
      foreach ( var prg in programs.Values )
        prg.Destroy();
    }

    /// <summary>
    /// Compute and render one frame.
    /// </summary>
    public void ComputeRender ()
    {
      if ( !loaded ||
           texName == 0 )
        return;

      frameCounter++;
      pixelCounter += texWidth * texHeight;

      // 1. compute Mandelbrot set into the texture:
      long startTicks = DateTime.Now.Ticks;
      if ( checkOpenCL.Checked )
      {
        // OpenCL version:
        mandel.ComputeCL( clContext, texName, texWidth, texHeight, colormap,
                          checkDouble.Checked, checkInterop.Checked );
      }
      else
      {
        // SW version:
        mandel.ComputeSW( texName, texWidth, texHeight, colormap,
                          checkDouble.Checked, checkPalette.Checked );
      }
      computeCounter += 1.0e-7 * (DateTime.Now.Ticks - startTicks);

      // 2. rendering data from the texture:
      Render();

      glControl1.SwapBuffers();
    }

    /// <summary>
    /// Rendering code itself (separated for clarity).
    /// </summary>
    void Render ()
    {
      if ( texName == 0 )
        return;

      GL.BindTexture( TextureTarget.Texture2D, texName );

      GL.Begin( PrimitiveType.Quads );

      //GL.Color3( 1.0f, 1.0f, 0.0f );

      GL.TexCoord2( 0.0f,  0.0f );
      GL.Vertex2(  -1.0f, -1.0f );

      GL.TexCoord2( 1.0f,  0.0f );
      GL.Vertex2(   1.0f, -1.0f );

      GL.TexCoord2( 1.0f,  1.0f );
      GL.Vertex2(   1.0f,  1.0f );

      GL.TexCoord2( 0.0f,  1.0f );
      GL.Vertex2(  -1.0f,  1.0f );

      GL.End();

      GL.BindTexture( TextureTarget.Texture2D, 0 );
    }
  }
}
