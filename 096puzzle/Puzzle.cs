// Author: Josef Pelikan

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using MathSupport;
using OpenglSupport;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Utilities;

namespace _096puzzle
{
  /// <summary>
  /// Cube object / primary scene - object able to be rendered,
  /// simulated (animated), realtime interaction with the user by mouse pointing.
  /// </summary>
  public class Cube : DefaultRenderObject
  {
    /// <summary>
    /// Cube center.
    /// </summary>
    public Vector3d position;

    /// <summary>
    /// Object to world transform matrix.
    /// </summary>
    public Matrix4d objectMatrix = Matrix4d.Identity;

    /// <summary>
    /// Cobe size.
    /// </summary>
    public double size;

    /// <summary>
    /// Last simulated time in seconds.
    /// </summary>
    public double simTime;

    /// <summary>
    /// Revolution in radians per second.
    /// </summary>
    public double speed = 1.0;

    /// <summary>
    /// Current rotation axis.
    /// </summary>
    public Vector3d axis = Vector3d.Zero;

    /// <summary>
    /// Angle to rotate (0.0 if no rotation is necessary).
    /// angleLeft += 'sign' * speed * dt;
    /// </summary>
    public double angleLeft = 0.0;

    public Cube ( Vector3d pos, double siz, double time )
    {
      position  = pos;
      size      = siz;
      simTime   = time;
      angleLeft = 0.0;
      fillCache();
    }

    /// <summary>
    /// Simulate object to the given time.
    /// </summary>
    /// <param name="time">Required target time.</param>
    /// <param name="puz">Puzzle context.</param>
    /// <returns>False in case of expiry.</returns>
    public bool Simulate ( double time, Puzzle puz )
    {
      if ( time <= simTime )
        return true;

      if ( Math.Abs( angleLeft ) < 0.001 )
        return true;

      // fly the particle:
      double dt = time - simTime;
      double dangle = Math.Min( Math.Abs( angleLeft ), dt * speed );
      int sign = Math.Sign( angleLeft );

      Matrix4d dm = Matrix4d.Rotate( axis, dangle * sign );
      objectMatrix *= dm;

      angleLeft -= dangle * sign;

      simTime = time;

      return true;
    }

    /// <summary>
    /// Pointing to the cube.
    /// </summary>
    /// <param name="p0">Ray origin.</param>
    /// <param name="p1">Ray direction.</param>
    /// <param name="action">Do interaction?</param>
    /// <returns>Intersection parameter or double.PositiveInfinity if no intersection exists.</returns>
    public double Intersect ( ref Vector3d p0, ref Vector3d p1, bool action =false )
    {
      Vector3d A, B, C;
      Vector2d uv;
      double nearest = double.PositiveInfinity;
      int inearest = 0;

      for ( int i = 0; i + 2 < ind.Length; i += 3 )
      {
        A = Vector3d.TransformPosition( new Vector3d( vert[ ind[ i ] ], vert[ ind[ i ] + 1 ], vert[ ind[ i ] + 2 ] ), objectMatrix );
        B = Vector3d.TransformPosition( new Vector3d( vert[ ind[ i + 1 ] ], vert[ ind[ i + 1 ] + 1 ], vert[ ind[ i + 1 ] + 2 ] ), objectMatrix );
        C = Vector3d.TransformPosition( new Vector3d( vert[ ind[ i + 2 ] ], vert[ ind[ i + 2 ] + 1 ], vert[ ind[ i + 2 ] + 2 ] ), objectMatrix );
        double curr = Geometry.RayTriangleIntersection( ref p0, ref p1, ref A, ref B, ref C, out uv );
        if ( !double.IsInfinity( curr ) &&
             curr < nearest )
        {
          nearest = curr;
          inearest = i;
        }
      }

      if ( action )
      {
        inearest /= 3;   // face number

        // rotation axis:
        if ( inearest <= 1 )
          axis = Vector3d.UnitX;
        else
          if ( inearest <= 3 )
            axis = Vector3d.UnitZ;
          else
            axis = Vector3d.UnitY;

        // angle & orientation:
        angleLeft = Math.PI * 0.5 * ((inearest & 1) > 0 ? 1.0 : -1.0);
      }

      return nearest;
    }

    //--- rendering ---

    /// <summary>
    /// Vertex array cache. Object coordinates.
    /// </summary>
    float[] vert = null;

    /// <summary>
    /// Index buffer cache.
    /// </summary>
    uint[] ind = null;

    unsafe void fillCache ()
    {
      uint ori = 0;
      int stride;
      float* ptr = null;
      int vsize = TriangleVertices( ref ptr, ref ori, out stride, false, false, false, false ) / sizeof( float );
      vert = new float[ vsize ];
      fixed ( float* p = vert )
      {
        float* pp = p;
        TriangleVertices( ref pp, ref ori, out stride, false, false, false, false );
      }
      uint* iptr = null;
      int isize = TriangleIndices( ref iptr, 0 ) / sizeof( uint );
      ind = new uint[ isize ];
      fixed ( uint* ip = ind )
      {
        uint* ipp = ip;
        TriangleIndices( ref ipp, 0 );
      }
    }

    public override uint Triangles
    {
      get
      {
        return 12;
      }
    }

    public override uint TriVertices
    {
      get
      {
        return 24;
      }
    }

    unsafe void FaceVertices ( ref float* ptr, ref Vector3d corner, ref Vector3d side1, ref Vector3d side2, ref Vector3d n, ref Vector3 color,
                               bool txt, bool col, bool normal, bool ptsize )
    {
      // upper left
      if ( txt )
        Fill( ref ptr, 0.0f, 0.0f );
      if ( col )
        Fill( ref ptr, ref color );
      if ( normal )
        Fill( ref ptr, ref n );
      if ( ptsize )
        *ptr++ = 1.0f;
      Fill( ref ptr, ref corner );

      // upper right
      if ( txt )
        Fill( ref ptr, 1.0f, 0.0f );
      if ( col )
        Fill( ref ptr, ref color );
      if ( normal )
        Fill( ref ptr, ref n );
      if ( ptsize )
        *ptr++ = 1.0f;
      Fill( ref ptr, corner + side1 );

      // lower left
      if ( txt )
        Fill( ref ptr, 0.0f, 1.0f );
      if ( col )
        Fill( ref ptr, ref color );
      if ( normal )
        Fill( ref ptr, ref n );
      if ( ptsize )
        *ptr++ = 1.0f;
      Fill( ref ptr, corner + side2 );

      // lower right
      if ( txt )
        Fill( ref ptr, 1.0f, 1.0f );
      if ( col )
        Fill( ref ptr, ref color );
      if ( normal )
        Fill( ref ptr, ref n );
      if ( ptsize )
        *ptr++ = 1.0f;
      Fill( ref ptr, corner + side1 + side2 );
    }

    /// <summary>
    /// Triangles: returns vertex-array size (if ptr is null) or fills vertex array.
    /// </summary>
    /// <returns>Data size of the vertex-set (in bytes).</returns>
    public override unsafe int TriangleVertices ( ref float* ptr, ref uint origin, out int stride, bool txt, bool col, bool normal, bool ptsize )
    {
      int total = base.TriangleVertices( ref ptr, ref origin, out stride, txt, col, normal, ptsize );
      if ( ptr == null )
        return total;

      Vector3d corner, n, side1, side2;
      double s2 = size * 0.5;
      Vector3 color;

      // 1. front
      corner.X = position.X - s2;
      corner.Y = position.Y + s2;
      corner.Z = position.Z + s2;
      side1 = Vector3d.UnitX * size;
      side2 = Vector3d.UnitY * -size;
      n = Vector3d.UnitZ;
      color = Vector3.UnitX; // red
      FaceVertices( ref ptr, ref corner, ref side1, ref side2, ref n, ref color, txt, col, normal, ptsize );

      // 2. right
      corner += side1;
      side1 = Vector3d.UnitZ * -size;
      n = Vector3d.UnitX;
      color = Vector3.UnitZ; // blue
      FaceVertices( ref ptr, ref corner, ref side1, ref side2, ref n, ref color, txt, col, normal, ptsize );

      // 3. back
      corner += side1;
      side1 = Vector3d.UnitX * -size;
      n = -Vector3d.UnitZ;
      color = Vector3.UnitX + 0.5f * Vector3.UnitY; // orange
      FaceVertices( ref ptr, ref corner, ref side1, ref side2, ref n, ref color, txt, col, normal, ptsize );

      // 4. left
      corner += side1;
      side1 = Vector3d.UnitZ * size;
      n = -Vector3d.UnitX;
      color = Vector3.UnitY; // green
      FaceVertices( ref ptr, ref corner, ref side1, ref side2, ref n, ref color, txt, col, normal, ptsize );

      // 5. top
      side1 = Vector3d.UnitX * size;
      side2 = Vector3d.UnitZ * size;
      n = Vector3d.UnitY;
      color.X = color.Y = color.Z = 1.0f; // white
      FaceVertices( ref ptr, ref corner, ref side1, ref side2, ref n, ref color, txt, col, normal, ptsize );

      // 6. bottom
      corner.X = position.X - s2;
      corner.Y = position.Y - s2;
      corner.Z = position.Z + s2;
      side1 = Vector3d.UnitX * size;
      side2 = Vector3d.UnitZ * -size;
      n = -Vector3d.UnitY;
      color = Vector3.UnitX + Vector3.UnitY; // orange
      FaceVertices( ref ptr, ref corner, ref side1, ref side2, ref n, ref color, txt, col, normal, ptsize );

      return total;
    }

    uint origin0 = 0;

    /// <summary>
    /// Triangles: returns index-array size (if ptr is null) or fills index array.
    /// </summary>
    /// <returns>Data size of the index-set (in bytes).</returns>
    public override unsafe int TriangleIndices ( ref uint* ptr, uint origin )
    {
      if ( ptr != null )
      {
        origin0 = origin;

        for ( int i = 0; i++ < 6; origin += 4 )
        {
          *ptr++ = origin;
          *ptr++ = origin + 2;
          *ptr++ = origin + 1;

          *ptr++ = origin + 2;
          *ptr++ = origin + 3;
          *ptr++ = origin + 1;
        }
      }

      return 24 * sizeof( uint );
    }
  }

  /// <summary>
  /// Puzzle instance.
  /// </summary>
  public class Puzzle
  {
    /// <summary>
    /// Simulated world = single cube.
    /// </summary>
    public Cube cube = new Cube( Vector3d.Zero, 2.0, 0.0 );

    /// <summary>
    /// Lock-protected simulation state.
    /// Pause-related stuff could be stored/handled elsewhere.
    /// </summary>
    public bool Running
    {
      get;
      set;
    }

    int frames;

    /// <summary>
    /// Number of simulated frames so far.
    /// </summary>
    public int Frames
    {
      get
      {
        return frames;
      }
    }

    double lastTime;

    /// <summary>
    /// Current sim-world time.
    /// </summary>
    public double Time
    {
      get
      {
        return lastTime;
      }
    }

    /// <summary>
    /// Significant change of simulation parameters .. need to reallocate buffers.
    /// </summary>
    public bool Dirty
    {
      get;
      set;
    }

    /// <summary>
    /// Slow motion coefficient.
    /// </summary>
    public static double slow = 0.25;

    /// <summary>
    /// Revolution speed in radians per second.
    /// </summary>
    public double speed = 1.0;

    public Puzzle ()
    {
      frames   = 0;
      lastTime = 0.0;
      Running  = true;
      Dirty    = false;
    }

    /// <summary>
    /// [Re-]initialize the simulation system.
    /// </summary>
    /// <param name="param">User-provided parameter string.</param>
    public void Reset ( string param )
    {
      // input params:
      Update( param );

      // initialization job itself:
      frames   = 0;
      lastTime = 0.0f;
      Running  = true;
    }

    /// <summary>
    /// Update simulation parameters.
    /// </summary>
    /// <param name="param">User-provided parameter string.</param>
    public void Update ( string param )
    {
      // input params:
      Dictionary<string, string> p = Util.ParseKeyValueList( param );
      if ( p.Count == 0 )
        return;

      // launchers: frequency
      if ( Util.TryParse( p, "speed", ref speed ) )
      {
        if ( speed < 0.01 )
          speed = 0.01;
        cube.speed = speed;
      }

      // global: slow-motion coeff
      if ( !Util.TryParse( p, "slow", ref slow ) ||
           slow < 1.0e-4 )
        slow = 0.25;

      // global: screencast
      bool recent = false;
      if ( Util.TryParse( p, "screencast", ref recent ) &&
           (Form1.screencast != null) != recent )
        Form1.StartStopScreencast( recent );
    }

    /// <summary>
    /// Do one step of simulation.
    /// </summary>
    /// <param name="time">Required target time.</param>
    public void Simulate ( double time )
    {
      if ( !Running )
        return;

      frames++;
      cube.Simulate( time, this );

      lastTime = time;
    }

    public double Intersect ( ref Vector3d p0, ref Vector3d p1, bool action =false )
    {
      if ( cube == null )
        return double.PositiveInfinity;

      return cube.Intersect( ref p0, ref p1, action );
    }

    /// <summary>
    /// Prepares (fills) all the triangle-related data into the provided vertex buffer and index buffer.
    /// </summary>
    /// <returns>Number of used indices (to draw).</returns>
    public unsafe int FillTriangleData ( ref float* ptr, ref uint* iptr, out int stride, bool txt, bool col, bool normal, bool ptsize )
    {
      uint* bakIptr = iptr;
      stride = 0;
      uint origin = 0;

      // only one cube:
      uint bakOrigin = origin;
      cube.TriangleVertices( ref ptr, ref origin, out stride, txt, col, normal, ptsize );
      cube.TriangleIndices( ref iptr, bakOrigin );

      return (int)(iptr - bakIptr);
    }
  }

  public partial class Form1
  {
    /// <summary>
    /// Form-data initialization.
    /// </summary>
    static void InitParams ( out string param, out string tooltip, out string name, out Vector3 center, out float diameter )
    {
      param      = "speed=1.0,slow=0.25";
      tooltip    = "speed,slow,screencast";
      name       = "pilot";
      center     = new Vector3( 0.0f, 0.0f, 0.0f );
      diameter   = 4.0f;
    }

    /// <summary>
    /// Set real-world coordinates of the camera/light source.
    /// </summary>
    void SetLightEye ( Vector3 center, float diameter )
    {
      diameter += diameter;
      lightPosition = center + new Vector3( -2.0f * diameter, diameter, diameter );
    }

    uint[] VBOid = null;  // vertex array VBO (colors, normals, coords), index array VBO
    int[] VBOlen = null;  // currently allocated lengths of VBOs

    Vector3 lightPosition = new Vector3( -20.0f, 10.0f, 10.0f );

    /// <summary>
    /// Simulation object.
    /// </summary>
    Puzzle puz = null;

    long lastFpsTime = 0L;
    int frameCounter = 0;
    long primitiveCounter = 0L;
    double lastFps = 0.0;
    double lastPps = 0.0;

    /// <summary>
    /// Function called whenever the main application is idle..
    /// </summary>
    void Application_Idle ( object sender, EventArgs e )
    {
      while ( glControl1.IsIdle )
      {
        glControl1.MakeCurrent();
        Simulate();
        Render( true );

        long now = DateTime.Now.Ticks;
        if ( now - lastFpsTime > 5000000 )      // more than 0.5 sec
        {
          lastFps = 0.5 * lastFps + 0.5 * (frameCounter * 1.0e7 / (now - lastFpsTime));
          lastPps = 0.5 * lastPps + 0.5 * (primitiveCounter * 1.0e7 / (now - lastFpsTime));
          lastFpsTime = now;
          frameCounter = 0;
          primitiveCounter = 0L;

          if ( lastPps < 5.0e5 )
            labelFps.Text = string.Format( CultureInfo.InvariantCulture, "Fps: {0:f1}, Pps: {1:f1}k",
                                           lastFps, (lastPps * 1.0e-3) );
          else
            labelFps.Text = string.Format( CultureInfo.InvariantCulture, "Fps: {0:f1}, Pps: {1:f1}m",
                                           lastFps, (lastPps * 1.0e-6) );

          if ( puz != null )
            labelStatus.Text = string.Format( CultureInfo.InvariantCulture, "time: {0:f1}s, fr: {1}{2}",
                                              puz.Time, puz.Frames,
                                              (screencast != null) ? (" (" + screencast.Queue + ')') : "" );
        }

        // pointing:
        if ( pointOrigin != null &&
             pointDirty )
        {
          Vector3d p0 = new Vector3d( pointOrigin.Value.X, pointOrigin.Value.Y, pointOrigin.Value.Z );
          Vector3d p1 = new Vector3d( pointTarget.X, pointTarget.Y, pointTarget.Z ) - p0;
          Vector2d uv;
          double nearest = double.PositiveInfinity;

          if ( puz != null )
          {
            nearest = puz.Intersect( ref p0, ref p1, false );
          }
          else
          {
            Vector3d ul = new Vector3d( -1.0, -1.0, -1.0 );
            Vector3d size = new Vector3d( 2.0, 2.0, 2.0 );
            if ( Geometry.RayBoxIntersection( ref p0, ref p1, ref ul, ref size, out uv ) )
              nearest = uv.X;
          }

          if ( double.IsInfinity( nearest ) )
            spot = null;
          else
            spot = new Vector3( (float)(p0.X + nearest * p1.X),
                                (float)(p0.Y + nearest * p1.Y),
                                (float)(p0.Z + nearest * p1.Z) );
          pointDirty = false;

        }
      }
    }

    /// <summary>
    /// OpenGL init code.
    /// </summary>
    void InitOpenGL ()
    {
      // log OpenGL info just for curiosity:
      GlInfo.LogGLProperties();

      // general OpenGL:
      glControl1.VSync = true;
      GL.ClearColor( Color.FromArgb( 14, 20, 40 ) );    // darker "navy blue"
      GL.Enable( EnableCap.DepthTest );
      GL.Enable( EnableCap.VertexProgramPointSize );
      GL.ShadeModel( ShadingModel.Flat );

      // VBO init:
      VBOid = new uint[ 2 ];           // one big buffer for vertex data, another buffer for tri/line indices
      GL.GenBuffers( 2, VBOid );
      GlInfo.LogError( "VBO init" );
      VBOlen = new int[ 2 ];           // zeroes..

      // texture:
      GenerateTexture();
    }

    // generated texture:
    const int TEX_SIZE = 128;
    const int TEX_CHECKER_SIZE = 8;
    static Vector3 colWhite = new Vector3( 0.85f, 0.75f, 0.15f );
    static Vector3 colBlack = new Vector3( 0.15f, 0.15f, 0.60f );
    static Vector3 colShade = new Vector3( 0.15f, 0.15f, 0.15f );

    /// <summary>
    /// Texture handle
    /// </summary>
    int texName = 0;

    /// <summary>
    /// Generate the texture.
    /// </summary>
    void GenerateTexture ()
    {
      GL.PixelStore( PixelStoreParameter.UnpackAlignment, 1 );
      texName = GL.GenTexture();
      GL.BindTexture( TextureTarget.Texture2D, texName );

      Vector3[] data = new Vector3[ TEX_SIZE * TEX_SIZE ];
      for ( int y = 0; y < TEX_SIZE; y++ )
        for ( int x = 0; x < TEX_SIZE; x++ )
        {
          int i = y * TEX_SIZE + x;
          bool odd = ((x / TEX_CHECKER_SIZE + y / TEX_CHECKER_SIZE) & 1) > 0;
          data[ i ] = odd ? colBlack : colWhite;
          // add some fancy shading on the edges:
          if ( (x % TEX_CHECKER_SIZE) == 0 || (y % TEX_CHECKER_SIZE) == 0 )
            data[ i ] += colShade;
          if ( ((x + 1) % TEX_CHECKER_SIZE) == 0 || ((y + 1) % TEX_CHECKER_SIZE) == 0 )
            data[ i ] -= colShade;
        }

      GL.TexImage2D( TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, TEX_SIZE, TEX_SIZE, 0, PixelFormat.Rgb, PixelType.Float, data );

      GL.TexParameter( TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat );
      GL.TexParameter( TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat );
      GL.TexParameter( TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear );
      GL.TexParameter( TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMagFilter.Linear );

      GlInfo.LogError( "create-texture" );
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

    static int Align ( int address )
    {
      return ((address + 15) & -16);
    }

    /// <summary>
    /// Reset VBO buffer's size.
    /// Forces InitDataBuffers() call next time buffers will be needed..
    /// </summary>
    void ResetDataBuffers ()
    {
      VBOlen[ 0 ] = VBOlen[ 1 ] = 0;
    }

    /// <summary>
    /// Initialize VBO buffers.
    /// Determine maximum buffer sizes and allocate VBO objects.
    /// Vertex buffer:
    /// <list type=">">
    /// <item>cube - triangles</item>
    /// </list>
    /// Index buffer:
    /// <list type=">">
    /// <item>cube - triangles</item>
    /// </list>
    /// </summary>
    unsafe void InitDataBuffers ()
    {
      puz.Dirty = false;

      // init data buffers for current simulation state (current number of launchers + max number of particles):
      // triangles: determine maximum stride, maximum vertices and indices
      float* ptr = null;
      uint* iptr = null;
      uint origin = 0;
      int stride = 0;

      // vertex-buffer size:
      int maxVB;
      maxVB =  Align( puz.cube.TriangleVertices( ref ptr, ref origin, out stride, true, true, true, true ) );
      // maxVB contains maximal vertex-buffer size for all batches

      // index-buffer size:
      int maxIB;
      maxIB =  Align( puz.cube.TriangleIndices( ref iptr, 0 ) );
      // maxIB contains maximal index-buffer size for all launchers

      VBOlen[ 0 ] = maxVB;
      VBOlen[ 1 ] = maxIB;

      // Vertex buffer in VBO[ 0 ]:
      GL.BindBuffer( BufferTarget.ArrayBuffer, VBOid[ 0 ] );
      GL.BufferData( BufferTarget.ArrayBuffer, (IntPtr)VBOlen[ 0 ], IntPtr.Zero, BufferUsageHint.DynamicDraw );
      GL.BindBuffer( BufferTarget.ArrayBuffer, 0 );
      GlInfo.LogError( "allocate vertex-buffer" );

      // Index buffer in VBO[ 1 ]:
      GL.BindBuffer( BufferTarget.ElementArrayBuffer, VBOid[ 1 ] );
      GL.BufferData( BufferTarget.ElementArrayBuffer, (IntPtr)VBOlen[ 1 ], IntPtr.Zero, BufferUsageHint.DynamicDraw );
      GL.BindBuffer( BufferTarget.ElementArrayBuffer, 0 );
      GlInfo.LogError( "allocate index-buffer" );
    }

    /// <summary>
    /// Simulation time of the last checkpoint in system ticks (100ns units)
    /// </summary>
    long ticksLast = DateTime.Now.Ticks;

    /// <summary>
    /// Simulation time of the last checkpoint in seconds.
    /// </summary>
    double timeLast = 0.0;

    /// <summary>
    /// Prime simulation init.
    /// </summary>
    private void InitSimulation ()
    {
      puz = new Puzzle();
      ResetSimulation();
    }

    /// <summary>
    /// [Re-]initialize the simulation.
    /// </summary>
    private void ResetSimulation ()
    {
      //Snapshots.ResetFrameNumber();
      if ( puz != null )
        lock ( puz )
        {
          ResetDataBuffers();
          puz.Reset( textParam.Text );
          ticksLast = DateTime.Now.Ticks;
          timeLast = 0.0;
        }
    }

    /// <summary>
    /// Pause / restart simulation.
    /// </summary>
    private void PauseRestartSimulation ()
    {
      if ( puz != null )
        lock ( puz )
          puz.Running  = !puz.Running;
    }

    /// <summary>
    /// Update Simulation parameters.
    /// </summary>
    private void UpdateSimulation ()
    {
      if ( puz != null )
        lock ( puz )
          puz.Update( textParam.Text );
    }

    /// <summary>
    /// Simulate one frame.
    /// </summary>
    private void Simulate ()
    {
      if ( puz != null )
        lock ( puz )
        {
          long nowTicks = DateTime.Now.Ticks;
          if ( nowTicks > ticksLast )
          {
            if ( puz.Running )
            {
              double timeScale = checkSlow.Checked ? Puzzle.slow : 1.0;
              timeLast += (nowTicks - ticksLast) * timeScale * 1.0e-7;
              puz.Simulate( timeLast );
            }
            ticksLast = nowTicks;
          }
        }
    }

    /// <summary>
    /// Render one frame.
    /// </summary>
    private void Render ( bool snapshot =false )
    {
      if ( !loaded )
        return;

      frameCounter++;

      GL.Clear( ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit );
      GL.ShadeModel( checkSmooth.Checked ? ShadingModel.Smooth : ShadingModel.Flat );
      GL.PolygonMode( checkTwosided.Checked ? MaterialFace.FrontAndBack : MaterialFace.Front,
                      checkWireframe.Checked ? PolygonMode.Line : PolygonMode.Fill );
      if ( checkTwosided.Checked )
        GL.Disable( EnableCap.CullFace );
      else
        GL.Enable( EnableCap.CullFace );

      tb.GLsetCamera();
      RenderScene();

      if ( snapshot &&
           screencast != null &&
           puz != null &&
           puz.Running )
        screencast.SaveScreenshotAsync( glControl1 );

      glControl1.SwapBuffers();
    }

    /// <summary>
    /// Rendering code itself (separated for clarity).
    /// </summary>
    void RenderScene ()
    {
      if ( puz != null )
      {
        if ( (VBOlen[ 0 ] == 0 &&
              VBOlen[ 1 ] == 0) ||
             puz.Dirty )
          InitDataBuffers();

        if ( VBOlen[ 0 ] > 0 ||
             VBOlen[ 1 ] > 0 )
        {
          // Scene rendering from VBOs:
          GL.EnableClientState( ArrayCap.VertexArray );
          GL.EnableClientState( ArrayCap.TextureCoordArray );
          GL.EnableClientState( ArrayCap.NormalArray );
          GL.EnableClientState( ArrayCap.ColorArray );

          // texture handling:
          bool useTexture = checkTexture.Checked;
          if ( texName == 0 )
            useTexture = false;
          if ( useTexture )
          {
            GL.Enable( EnableCap.Texture2D );
            GL.ActiveTexture( TextureUnit.Texture0 );
            GL.BindTexture( TextureTarget.Texture2D, texName );
            GL.TexEnv( TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, (int)TextureEnvMode.Replace );
          }

          // uniforms:
          Matrix4 modelView    = tb.ModelView;
          Matrix4 projection   = tb.Projection;

          // [txt] [colors] [normals] [ptsize] vertices
          GL.BindBuffer( BufferTarget.ArrayBuffer, VBOid[ 0 ] );
          GL.BindBuffer( BufferTarget.ElementArrayBuffer, VBOid[ 1 ] );
          int stride = 0;       // stride for vertex arrays
          int indices = 0;      // number of indices for index arrays

          //-------------------------
          // draw all triangles:

          IntPtr vertexPtr = GL.MapBuffer( BufferTarget.ArrayBuffer, BufferAccess.WriteOnly );
          IntPtr indexPtr = GL.MapBuffer( BufferTarget.ElementArrayBuffer, BufferAccess.WriteOnly );
          unsafe
          {
            float* ptr = (float*)vertexPtr.ToPointer();
            uint* iptr = (uint*)indexPtr.ToPointer();
            indices = puz.FillTriangleData( ref ptr, ref iptr, out stride, useTexture, !useTexture, false, false );
          }
          GL.UnmapBuffer( BufferTarget.ArrayBuffer );
          GL.UnmapBuffer( BufferTarget.ElementArrayBuffer );
          IntPtr p = IntPtr.Zero;

          // using FFP:
          if ( useTexture )
          {
            GL.TexCoordPointer( 2, TexCoordPointerType.Float, stride, p );
            p += Vector2.SizeInBytes;
          }

          if ( !useTexture )
          {
            GL.ColorPointer( 3, ColorPointerType.Float, stride, p );
            p += Vector3.SizeInBytes;
          }

          //if ( useNormals )
          //{
          //  GL.NormalPointer( NormalPointerType.Float, stride, p );
          //  p += Vector3.SizeInBytes;
          //}

          GL.VertexPointer( 3, VertexPointerType.Float, stride, p );

          // index buffer
          GL.BindBuffer( BufferTarget.ElementArrayBuffer, VBOid[ 1 ] );

          // engage!
          GL.DrawElements( PrimitiveType.Triangles, (int)puz.cube.TriVertices, DrawElementsType.UnsignedInt, IntPtr.Zero );
          GlInfo.LogError( "draw-elements-ffp" );

          if ( useTexture )
          {
            GL.BindTexture( TextureTarget.Texture2D, 0 );
            GL.Disable( EnableCap.Texture2D );
          }

          GL.BindBuffer( BufferTarget.ElementArrayBuffer, 0 );
          GL.BindBuffer( BufferTarget.ArrayBuffer, 0 );

          return;
        }
      }

      // default: draw trivial cube..

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

      primitiveCounter += 12;
    }

    // Unproject support functions:

    public Vector3 convertScreenToWorldCoords ( int x, int y, float z =0.0f )
    {
      Matrix4 modelViewMatrix, projectionMatrix;
      GL.GetFloat( GetPName.ModelviewMatrix, out modelViewMatrix );
      GL.GetFloat( GetPName.ProjectionMatrix, out projectionMatrix );

      Vector2 mouse;
      mouse.X = x;
      mouse.Y = glControl1.Height - y;
      Vector3 vector = UnProject( ref projectionMatrix, modelViewMatrix, new Size( glControl1.Width, glControl1.Height ), mouse, z );
      return vector;
    }

    public static Vector3 UnProject ( ref Matrix4 projection, Matrix4 view, Size viewport, Vector2 mouse, float z = 0.0f )
    {
      Vector4 vec;
      vec.X = 2.0f * mouse.X / (float)viewport.Width - 1;
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
        vec.W = 1.0f;
      }

      return new Vector3( vec );
    }
  }
}
