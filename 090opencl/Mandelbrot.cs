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
      radius = Arith.Clamp( radius * (float)coeff, 1.0e-5f, 4.0f );

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
                            bool useDouble )
    {
      assertBuffer( width, height );

      dxy = (radius + radius) / Math.Min( width, height );
      double xOrig = center.X - width * 0.5 * dxy;
      double yOrig = center.Y - height * 0.5 * dxy;

      string src = ClInfo.ReadSourceFile( "mandel.cl", "090opencl" );
      if ( string.IsNullOrEmpty( src ) )
        return;

      // buffers:
      ComputeBuffer<byte> result = new ComputeBuffer<byte>( clContext, ComputeMemoryFlags.WriteOnly, width * height * 4 );
      ComputeBuffer<byte> cmap   = new ComputeBuffer<byte>( clContext, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.CopyHostPointer, colormap );

      // program & kernel:
      ComputeProgram clProgram = new ComputeProgram( clContext, src );
      clProgram.Build( clContext.Devices, null, null, IntPtr.Zero );
      ComputeKernel clKernel = clProgram.CreateKernel( useDouble ? "mandelDouble" : "mandelSingle" );
      clKernel.SetMemoryArgument( 0, result );
      clKernel.SetValueArgument(  1, width );
      clKernel.SetValueArgument(  2, height );
      clKernel.SetValueArgument(  3, iter );
      clKernel.SetValueArgument(  4, xOrig );
      clKernel.SetValueArgument(  5, yOrig );
      clKernel.SetValueArgument(  6, dxy );
      clKernel.SetMemoryArgument( 7, cmap );
      clKernel.SetValueArgument(  8, colormap.Length );

      ComputeCommandQueue clCommands = new ComputeCommandQueue( clContext, clContext.Devices[ 0 ], ComputeCommandQueueFlags.None );

      long globalWidth = (width + 7) & -8L;
      long globalHeight = (height + 7) & -8L;

      clCommands.Execute( clKernel, null, new long[] { globalWidth, globalHeight }, new long[] { 8, 8 }, null );

      clCommands.ReadFromBuffer( result, ref swBuffer, false, null );

      clCommands.Finish();

      result.Dispose();
      cmap.Dispose();
      clCommands.Dispose();
      clKernel.Dispose();
      clProgram.Dispose();

      GL.BindTexture( TextureTarget.Texture2D, texName );
      GL.TexSubImage2D( TextureTarget.Texture2D, 0, 0, 0, width, height, PixelFormat.Rgba, PixelType.UnsignedByte, swBuffer );
      Debug.Assert( GL.GetError() == ErrorCode.NoError, "glTexSubImage2D" );
      GL.BindTexture( TextureTarget.Texture2D, 0 );
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

          labelFps.Text = string.Format( CultureInfo.InvariantCulture, "Fps: {0:f1}, pps: {1:f1} MPx/s ({2}), compute: {3:f1} ms",
                                         lastFps, (lastPps * 1.0e-6), checkDouble.Checked ? "double" : "single", (lastCompute * 1000.0) );
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
      GL.TexImage2D( TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, texWidth, texHeight, 0, PixelFormat.Rgba, PixelType.UnsignedByte, (IntPtr)0 );
      Debug.Assert( GL.GetError() == ErrorCode.NoError, "glTexImage2D" );

      GL.TexParameter( TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat );
      GL.TexParameter( TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat );
      GL.TexParameter( TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear );
      GL.TexParameter( TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMagFilter.Linear );

      PrepareClBuffers();

      GlInfo.LogError( "resize-texture" );
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
    /// Local working buffer for the OpenCL solution.
    /// </summary>
    ComputeImage2D outBuffer = null;

    /// <summary>
    /// Prepare OpenCL data buffer[s].
    /// </summary>
    void PrepareClBuffers ()
    {
      if ( texName == 0 ||
           !checkOpenCL.Checked ||
           clContext == null )
      {
        DestroyClBuffers();
        return;
      }

      GL.BindTexture( TextureTarget.Texture2D, texName );
      try
      {
        outBuffer = new ComputeImage2D( clContext, ComputeMemoryFlags.WriteOnly, new ComputeImageFormat( ComputeImageChannelOrder.Rgba, ComputeImageChannelType.UnsignedInt8 ),
                                        texWidth, texHeight, 0, (IntPtr)0 );
        //outBuffer = ComputeImage2D.CreateFromGLTexture2D( clContext, ComputeMemoryFlags.WriteOnly, (int)TextureTarget.Texture2D, 0, texName );
      }
      catch ( Exception exc )
      {
        Util.LogFormat( "clCreateFromGLTexture2D error: {0}", exc.Message );
        outBuffer = null;
      }
    }

    void DestroyClBuffers ()
    {
      if ( outBuffer != null )
      {
        outBuffer.Dispose();
        outBuffer = null;
      }
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
      float dZoom = e.Delta / 120.0f;

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
        mandel.ComputeCL( clContext, texName, texWidth, texHeight, colormap, checkDouble.Checked );
      }
      else
      {
        // SW version:
        mandel.ComputeSW( texName, texWidth, texHeight, colormap, checkDouble.Checked, checkPalette.Checked );
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
