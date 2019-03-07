using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;
using MathSupport;
using OpenglSupport;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Utilities;

namespace _058marbles
{
  public partial class Form1
  {
    /// <summary>
    /// Form-data initialization.
    /// </summary>
    public static void InitParams ( out string name, out string param, out string tooltip,
                                    out Vector3 center, out float diameter,
                                    out bool useTexture, out bool globalColor, out bool useNormals, out bool useWireframe, out bool useMT )
    {
      // {{

      name         = "Josef Pelikán";
      param        = "n=500";
      tooltip      = "n = <number-of-balls>";

      // Scene dimensions.
      center       = Vector3.Zero;
      diameter     = 30.0f;

      // GUI.
      useTexture   = false;
      globalColor  = false;
      useNormals   = false;
      useWireframe = false;
      useMT        = true;

      // }}
    }

    /// <summary>
    /// Simulation instance, initialized in glControl1_Load()
    /// </summary>
    MarblesWorld world = null;

    /// <summary>
    /// Renderer instance, initialized in glControl1_Load()
    /// </summary>
    MarblesRenderer renderer = null;

    /// <summary>
    /// Global instance of data for rendering.
    /// <code>renderer</code> instance must be locked for manipulation with this reference.
    /// </summary>
    MarblesRenderData data = null;

    // FPS-related stuff:
    long lastFpsTime = 0L;
    int frameCounter = 0;
    volatile int simCounter = 0;
    long primitiveCounter = 0L;
    double lastFps = 0.0;
    double lastSps = 0.0;
    double lastTps = 0.0;

    /// <summary>
    /// Function called whenever the main application is idle..
    /// </summary>
    private void Application_Idle ( object sender, EventArgs e )
    {
      while ( glControl1.IsIdle )
      {
        glControl1.MakeCurrent();

        // World simulation (sets the 'data' object):
        if ( simThread == null )
          Simulate();

        // Rendering (from the 'data' object);
        Render( true );

        long now = DateTime.Now.Ticks;
        long newTicks = now - lastFpsTime;
        if ( newTicks > 5000000 )      // more than 0.5 sec
        {
          lastFps = 0.5 * lastFps + 0.5 * (frameCounter * 1.0e7 / newTicks);
          lastSps = 0.5 * lastSps + 0.5 * (simCounter * 1.0e7 / newTicks);
          lastTps = 0.5 * lastTps + 0.5 * (primitiveCounter * 1.0e7 / newTicks);
          lastFpsTime = now;
          frameCounter = 0;
          simCounter = 0;
          primitiveCounter = 0L;

          if ( lastTps < 5.0e5 )
            labelFps.Text = string.Format( CultureInfo.InvariantCulture, "Fps: {0:f1}, Sps: {1:f1}, Tps: {2:f0}k",
                                           lastFps, lastSps, (lastTps * 1.0e-3) );
          else
            labelFps.Text = string.Format( CultureInfo.InvariantCulture, "Fps: {0:f1}, Sps: {1:f1}, Tps: {2:f1}m",
                                           lastFps, lastSps, (lastTps * 1.0e-6) );

          if ( world != null )
            labelStat.Text = string.Format( CultureInfo.InvariantCulture, "T: {0:f1}s, fr: {1}{3}{4}, mrbl: {2}",
                                            world.Time, world.Frames, world.Marbles,
                                            (screencast != null) ? (" (" + screencast.Queue + ')') : "",
                                            (simThread == null) ? "" : " mt" );
        }
      }
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
    private void InitSimulation ( string param )
    {
      world    = new MarblesWorld( param );
      renderer = new MarblesRenderer( param );
      data     = null;

      ResetSimulation( param );
    }

    /// <summary>
    /// [Re-]initialize the simulation.
    /// </summary>
    private void ResetSimulation ( string param )
    {
      Snapshots.ResetFrameNumber();

      if ( world != null )
        lock ( world )
        {
          // ResetDataBuffers();
          world.Reset( param );
          ticksLast = DateTime.Now.Ticks;
          timeLast = 0.0;
        }
    }

    /// <summary>
    /// Pause / restart simulation.
    /// </summary>
    private void PauseRestartSimulation ()
    {
      if ( world != null )
        lock ( world )
          world.Running = !world.Running;
    }

    /// <summary>
    /// Update Simulation parameters.
    /// </summary>
    private void UpdateSimulation ( string param )
    {
      if ( world != null )
        lock ( world )
          world.Update( param );
    }

    /// <summary>
    /// Simulate one frame.
    /// </summary>
    private void Simulate ()
    {
      /// <summary>New data to draw (or <code>null</code> if there is nothing to update).</summary>
      MarblesRenderData newData = null;

      if ( world != null )
        lock ( world )
        {
          long nowTicks = DateTime.Now.Ticks;
          if ( nowTicks > ticksLast )
          {
            if ( world.Running )
            {
              // 1000Hz .. 1ms .. 10000 ticks
              long minTicks = 10000000L / world.maxSpeed;       // min ticks between simulation steps
              if ( nowTicks - ticksLast < (3 * minTicks) / 4 )
              {
                // we are going too fast..
                int sleepMs = (int)(((5 * minTicks) / 4 - nowTicks + ticksLast) / 10000L);
                Thread.Sleep( sleepMs );
                nowTicks = DateTime.Now.Ticks;
              }
              double timeScale = checkSlow.Checked ? MarblesWorld.slow : 1.0;
              timeLast += (nowTicks - ticksLast) * timeScale * 1.0e-7;
              newData = world.Simulate( timeLast );
              simCounter++;
            }
            ticksLast = nowTicks;
          }
        }

      if ( newData != null &&
           renderer != null )
        lock ( renderer )
          data = newData;
    }

    /// <summary>
    /// Rendering of one frame, all the stuff.
    /// </summary>
    private void Render ( bool snapshot = false )
    {
      if ( !loaded )
        return;

      frameCounter++;
      OGL.useShaders = OGL.canShaders &&
                       OGL.activeProgram != null;

      GL.Clear( ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit );
      GL.PolygonMode( MaterialFace.FrontAndBack, checkWireframe.Checked ? PolygonMode.Line : PolygonMode.Fill );

      // Current camera:
      tb.GLsetCamera();

      // Scene rendering:
      RenderScene();

      if ( snapshot &&
           screencast != null &&
           world != null &&
           world.Running )
        screencast.SaveScreenshotAsync( glControl1 );

      glControl1.SwapBuffers();
    }

    /// <summary>
    /// OpenGL rendering code itself (separated for clarity).
    /// </summary>
    private void RenderScene ()
    {
      // Simulation scene rendering.
      MarblesRenderData rdata = null;
      if ( renderer != null &&
           OGL.useShaders )
        lock ( renderer )
          rdata = data;

      if ( rdata != null )
      {
        OGL.FormData();

        // Simulated scene.
        primitiveCounter += renderer.Render( OGL, rdata );

        // Light source.
        GL.PointSize( 5.0f );
        GL.Begin( PrimitiveType.Points );
          GL.Color3( 1.0f, 1.0f, 1.0f );
          GL.Vertex3( OGL.lightPosition );
        GL.End();

        primitiveCounter++;
      }
      else
      {
        // color cube:
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
    }

    private void glControl1_MouseDown ( object sender, MouseEventArgs e )
    {
      if ( world == null ||
           !world.MouseButtonDown( e ) )
        tb.MouseDown( e );
    }

    private void glControl1_MouseUp ( object sender, MouseEventArgs e )
    {
      if ( world == null ||
           !world.MouseButtonUp( e ) )
        tb.MouseUp( e );
    }

    private void glControl1_MouseMove ( object sender, MouseEventArgs e )
    {
      if ( world == null ||
           !world.MousePointerMove( e ) )
        tb.MouseMove( e );
    }

    private void glControl1_MouseWheel ( object sender, MouseEventArgs e )
    {
      if ( world == null ||
           !world.MouseWheelChange( e ) )
        tb.MouseWheel( e );
    }

    private void glControl1_KeyDown ( object sender, KeyEventArgs e )
    {
      tb.KeyDown( e );
    }

    private void glControl1_KeyUp ( object sender, KeyEventArgs e )
    {
      // Simulation has the priority.
      if ( world == null ||
           !world.KeyHandle( e ) )
        tb.KeyUp( e );
    }
  }

  /// <summary>
  /// Rendering data passed from MarblesWorld to MarblesRenderer
  /// </summary>
  public class MarblesRenderData
  {
    // {{ TODO: modify/add data

    public List<Vector3> centers;

    public List<float> radii;

    public List<Color> colors;

    public MarblesRenderData ( IEnumerable<Vector3> _centers, IEnumerable<float> _radii, IEnumerable<Color> _colors )
    {
      centers = new List<Vector3>( _centers );
      radii   = new List<float>( _radii );
      colors  = new List<Color>( _colors );
    }

    // }}
  }

  /// <summary>
  /// Renderer: can interpret MarblesRenderData and converts them into
  /// OpenGL commands..
  /// Locked for manipulation with Form1.data pointer.
  /// </summary>
  public class MarblesRenderer
  {
    public MarblesRenderer ( string param )
    {
      // {{ Parse 'param' if needed..

      // }}
    }

    /// <summary>
    /// Number of marbles (triangles) VBOs are allocated for..
    /// </summary>
    int lastMarbles = -1;

    /// <summary>
    /// Renders the simulated data from the special object.
    /// </summary>
    /// <param name="OGL">Current OpenGL state object.</param>
    /// <param name="data">Data to render.</param>
    /// <returns>Number of actually drawn primitives.</returns>
    public long Render ( OpenglState OGL, MarblesRenderData data )
    {
      // {{ TODO: modify the rendering code!

      // Scene rendering from VBOs.
      OGL.SetVertexAttrib( true );

      // Using GLSL shaders.
      GL.UseProgram( OGL.activeProgram.Id );

      // Uniforms.

      // Camera, projection, ..
      Matrix4 modelView    = OGL.GetModelView();
      Matrix4 modelViewInv = OGL.GetModelViewInv();
      Matrix4 projection   = OGL.GetProjection();
      Vector3 eye          = OGL.GetEyePosition();

      // Give matrices to shaders.
      GL.UniformMatrix4( OGL.activeProgram.GetUniform( "matrixModelView" ), false, ref modelView );
      GL.UniformMatrix4( OGL.activeProgram.GetUniform( "matrixProjection" ), false, ref projection );

      // Lighting constants.
      GL.Uniform3( OGL.activeProgram.GetUniform( "globalAmbient" ), ref OGL.globalAmbient );
      GL.Uniform3( OGL.activeProgram.GetUniform( "lightColor" ),    ref OGL.whiteLight );
      GL.Uniform3( OGL.activeProgram.GetUniform( "lightPosition" ), ref OGL.lightPosition );
      GL.Uniform3( OGL.activeProgram.GetUniform( "eyePosition" ),   ref eye );
      GL.Uniform3( OGL.activeProgram.GetUniform( "Ka" ),            ref OGL.matAmbient );
      GL.Uniform3( OGL.activeProgram.GetUniform( "Kd" ),            ref OGL.matDiffuse );
      GL.Uniform3( OGL.activeProgram.GetUniform( "Ks" ),            ref OGL.matSpecular );
      GL.Uniform1( OGL.activeProgram.GetUniform( "shininess" ),     OGL.matShininess );

      // Global color handling.
      bool useColors = !OGL.useGlobalColor;
      GL.Uniform1( OGL.activeProgram.GetUniform( "globalColor" ), useColors ? 0 : 1 );

      // Use varying normals?
      bool useNormals = OGL.useNormals;
      GL.Uniform1( OGL.activeProgram.GetUniform( "useNormal" ), useNormals ? 1 : 0 );
      GlInfo.LogError( "set-uniforms" );

      // Texture handling.
      bool useTexture = OGL.useTexture;
      GL.Uniform1( OGL.activeProgram.GetUniform( "useTexture" ), useTexture ? 1 : 0 );
      GL.Uniform1( OGL.activeProgram.GetUniform( "texSurface" ), 0 );
      if ( useTexture )
      {
        GL.ActiveTexture( TextureUnit.Texture0 );
        GL.BindTexture( TextureTarget.Texture2D, OGL.texName );
      }
      GlInfo.LogError( "set-texture" );

      // [txt] [colors] [normals] vertices
      GL.BindBuffer( BufferTarget.ArrayBuffer, OGL.VBOid[ 0 ] );
      GL.BindBuffer( BufferTarget.ElementArrayBuffer, OGL.VBOid[ 1 ] );

      int marbles = data.radii.Count;
      int stride = sizeof( float ) * 11;   // float[2] txt, float[3] color, float[3] normal, float[3] position
      int triangles = marbles;
      IntPtr videoMemoryPtr;

      if ( marbles != lastMarbles )
      {
        // Relocate the buffers.

        // Vertex array: [ txt color coord ]
        GL.BindBuffer( BufferTarget.ArrayBuffer, OGL.VBOid[ 0 ] );
        int vertexBufferSize = (marbles * 3) * stride;  // TODO: marble should be drawn as a SPHERE, not a triangle..
        GL.BufferData( BufferTarget.ArrayBuffer, (IntPtr)vertexBufferSize, IntPtr.Zero, BufferUsageHint.DynamicDraw );
        GL.BindBuffer( BufferTarget.ArrayBuffer, 0 );

        // Fill index array.
        GL.BindBuffer( BufferTarget.ElementArrayBuffer, OGL.VBOid[ 1 ] );
        GL.BufferData( BufferTarget.ElementArrayBuffer, (IntPtr)((triangles * 3) * sizeof( uint )), IntPtr.Zero, BufferUsageHint.StaticDraw );
        videoMemoryPtr = GL.MapBuffer( BufferTarget.ElementArrayBuffer, BufferAccess.WriteOnly );
        unsafe
        {
          uint* ptr = (uint*)videoMemoryPtr.ToPointer();
          for ( uint i = 0; i < triangles * 3; i++ )
            *ptr++ = i;
        }
        GL.UnmapBuffer( BufferTarget.ElementArrayBuffer );
        GL.BindBuffer( BufferTarget.ElementArrayBuffer, 0 );

        lastMarbles = marbles;
      }

      // Refill vertex buffer.
      GL.BindBuffer( BufferTarget.ArrayBuffer, OGL.VBOid[ 0 ] );
      videoMemoryPtr = GL.MapBuffer( BufferTarget.ArrayBuffer, BufferAccess.WriteOnly );
      unsafe
      {
        float* ptr = (float*)videoMemoryPtr.ToPointer();
        for ( int i = 0; i < marbles; i++ )
        {
          float rad = data.radii[ i ];
          float r = data.colors[ i ].R / 255.0f;
          float g = data.colors[ i ].G / 255.0f;
          float b = data.colors[ i ].B / 255.0f;

          // txt[2], color[3], normal[3], position[3]
          *ptr++ = 0.3f;
          *ptr++ = 0.3f;
          *ptr++ = r;
          *ptr++ = g;
          *ptr++ = b;
          *ptr++ = 0.0f;
          *ptr++ = 0.0f;
          *ptr++ = 1.0f;
          *ptr++ = data.centers[ i ].X;
          *ptr++ = data.centers[ i ].Y;
          *ptr++ = data.centers[ i ].Z;

          // txt[2], color[3], normal[3], position[3]
          *ptr++ = 0.7f;
          *ptr++ = 0.3f;
          *ptr++ = r;
          *ptr++ = g;
          *ptr++ = b;
          *ptr++ = 0.0f;
          *ptr++ = 0.0f;
          *ptr++ = 1.0f;
          *ptr++ = data.centers[ i ].X + rad;
          *ptr++ = data.centers[ i ].Y;
          *ptr++ = data.centers[ i ].Z;

          // txt[2], color[3], normal[3], position[3]
          *ptr++ = 0.3f;
          *ptr++ = 0.7f;
          *ptr++ = r;
          *ptr++ = g;
          *ptr++ = b;
          *ptr++ = 0.0f;
          *ptr++ = 0.0f;
          *ptr++ = 1.0f;
          *ptr++ = data.centers[ i ].X;
          *ptr++ = data.centers[ i ].Y + rad;
          *ptr++ = data.centers[ i ].Z;
        }
      }
      GL.UnmapBuffer( BufferTarget.ArrayBuffer );

      // Index buffer.
      GL.BindBuffer( BufferTarget.ElementArrayBuffer, OGL.VBOid[ 1 ] );

      // Set attribute pointers.
      IntPtr p = IntPtr.Zero;
      if ( OGL.activeProgram.HasAttribute( "texCoords" ) )
        GL.VertexAttribPointer( OGL.activeProgram.GetAttribute( "texCoords" ), 2, VertexAttribPointerType.Float, false, stride, p );
      p += Vector2.SizeInBytes;

      if ( OGL.activeProgram.HasAttribute( "color" ) )
        GL.VertexAttribPointer( OGL.activeProgram.GetAttribute( "color" ), 3, VertexAttribPointerType.Float, false, stride, p );
      p += Vector3.SizeInBytes;

      if ( OGL.activeProgram.HasAttribute( "normal" ) )
        GL.VertexAttribPointer( OGL.activeProgram.GetAttribute( "normal" ), 3, VertexAttribPointerType.Float, false, stride, p );
      p += Vector3.SizeInBytes;

      GL.VertexAttribPointer( OGL.activeProgram.GetAttribute( "position" ), 3, VertexAttribPointerType.Float, false, stride, p );
      GlInfo.LogError( "triangles-set-attrib-pointers" );

      // The drawing command itself.
      GL.DrawElements( PrimitiveType.Triangles, triangles * 3, DrawElementsType.UnsignedInt, IntPtr.Zero );
      GlInfo.LogError( "triangles-draw-elements" );

      GL.BindBuffer( BufferTarget.ArrayBuffer, 0 );
      GL.BindBuffer( BufferTarget.ElementArrayBuffer, 0 );
      GL.UseProgram( 0 );

      OGL.SetVertexAttrib( false );

      return triangles;

      // }}
    }
  }

  /// <summary>
  /// Simulation class. Holds internal state of the virtual world,
  /// generated rendering data.
  /// Doesn't know anything about OpenGL.
  /// </summary>
  public class MarblesWorld
  {
    // {{ TODO: modify/add data

    /// <summary>
    /// Marble centers.
    /// </summary>
    List<Vector3> centers;

    /// <summary>
    /// Marble radii.
    /// </summary>
    List<float> radii;

    /// <summary>
    /// Marble current velocities.
    /// </summary>
    List<Vector3> velocities;

    /// <summary>
    /// marble colors.
    /// </summary>
    List<Color> colors;

    /// <summary>
    /// Required number of marbles.
    /// </summary>
    int marbles;

    /// <summary>
    /// Actual number of active marbles.
    /// </summary>
    public int Marbles
    {
      get
      {
        return radii.Count;
      }
    }

    // }}

    /// <summary>
    /// Lock-protected simulation state.
    /// Pause-related stuff could be stored/handled elsewhere.
    /// </summary>
    public bool Running
    {
      get;
      set;
    }

    /// <summary>
    /// Number of simulated frames so far.
    /// </summary>
    public int Frames
    {
      get;
      private set;
    }

    /// <summary>
    /// Current sim-world time.
    /// </summary>
    public double Time
    {
      get;
      private set;
    }

    /// <summary>
    /// Maximum simulation speed in sims per second.
    /// </summary>
    public int maxSpeed = 1000;

    /// <summary>
    /// Random generator instance.
    /// </summary>
    RandomJames rnd;

    /// <summary>
    /// Slow motion coefficient.
    /// </summary>
    public static double slow = 0.25;

    public MarblesWorld ( string param )
    {
      // {{ Parse 'param' if you need..

      // }}

      rnd        = new RandomJames( 144 );
      centers    = new List<Vector3>();
      radii      = new List<float>();
      velocities = new List<Vector3>();
      colors     = new List<Color>();

      Frames     = 0;
      Time       = 0.0;
      Running    = false;
    }

    /// <summary>
    /// [Re-]initialization of the world.
    /// </summary>
    /// <param name="param">String parameter from user, e.g.: number of marbles.</param>
    public void Reset ( string param )
    {
      Running = false;

      // Param parsing.
      Update( param );

      rnd.Reset( 144 );
      centers.Clear();
      radii.Clear();
      velocities.Clear();
      colors.Clear();

      UpdateMarbles();

      Frames   = 0;
      Time = 0.0;
      Running  = true;
    }

    /// <summary>
    /// Update number of simulated marbles, 'Marbles' is requested to be equal to 'marbles'.
    /// </summary>
    void UpdateMarbles ()
    {
      if ( marbles < Marbles )
      {
        // Truncate the arrays.
        int remove = Marbles - marbles;
        centers.RemoveRange( marbles, remove );
        radii.RemoveRange( marbles, remove );
        velocities.RemoveRange( marbles, remove );
        colors.RemoveRange( marbles, remove );
        return;
      }

      // Extend the arrays.
      for ( int i = Marbles; i < marbles; i++ )
      {
        centers.Add( new Vector3( rnd.RandomFloat( -10.0f, 10.0f ),
                                  rnd.RandomFloat( -10.0f, 10.0f ),
                                  rnd.RandomFloat( -10.0f, 10.0f ) ) );

        radii.Add( rnd.RandomFloat( 0.4f, 1.0f ) );

        velocities.Add( new Vector3( rnd.RandomFloat( -2.0f, 2.0f ),
                                     rnd.RandomFloat( -2.0f, 2.0f ),
                                     rnd.RandomFloat( -2.0f, 2.0f ) ) );

        colors.Add( Color.FromArgb( rnd.RandomInteger( 0, 255 ),
                                    rnd.RandomInteger( 0, 255 ),
                                    rnd.RandomInteger( 0, 255 ) ) );

        // {{ TODO: more initialization?

        // }}
      }
    }

    /// <summary>
    /// Update simulation parameters.
    /// </summary>
    /// <param name="param">User-provided parameter string.</param>
    public void Update ( string param )
    {
      // Input params.
      Dictionary<string, string> p = Util.ParseKeyValueList( param );
      if ( p.Count == 0 )
        return;

      // Simulation: number of marbles.
      if ( Util.TryParse( p, "n", ref marbles ) )
      {
        marbles = Arith.Clamp( marbles, 1, 10000 );

        if ( Running )
          UpdateMarbles();
      }

      // Simulation: maximum simulation speed (simulations per second).
      if ( Util.TryParse( p, "speed", ref maxSpeed ) )
      {
        maxSpeed = Arith.Clamp( maxSpeed, 5, 1000 );
      }

      // Global: screencast.
      bool recent = false;
      if ( Util.TryParse( p, "screencast", ref recent ) &&
           (Form1.screencast != null) != recent )
        Form1.StartStopScreencast( recent );

      // {{ TODO: more parameter-parsing?

      // }}
    }

    /// <summary>
    /// Do one step of simulation.
    /// </summary>
    /// <param name="time">Required target time in seconds.</param>
    /// <returns>Data for rendering or <code>null</code> if nothing has changed.</returns>
    public MarblesRenderData Simulate ( double time )
    {
      if ( !Running )
        return null;

      // {{ TODO: put your own simulation code here

      if ( time > Time )
      {
        Frames++;
        float dTime = (float)( time - Time );

        // Simulate (update) the world.
        for ( int i = 0; i < centers.Count; i++ )
        {
          Vector3 vel = velocities[ i ];
          Vector3 pos = centers[ i ] + vel * dTime;

          if ( Math.Abs( pos.X ) >= 10.0f )
          {
            vel.X = -vel.X;
            pos.X = Arith.Clamp( pos.X, -10.0f, 10.0f );
          }

          if ( Math.Abs( pos.Y ) >= 10.0f )
          {
            vel.Y = -vel.Y;
            pos.Y = Arith.Clamp( pos.Y, -10.0f, 10.0f );
          }

          if ( Math.Abs( pos.Z ) >= 10.0f )
          {
            vel.Z = -vel.Z;
            pos.Z = Arith.Clamp( pos.Z, -10.0f, 10.0f );
          }

          velocities[ i ] = vel;
          centers[ i ] = pos;
        }

        Time = time;
      }

      // Return the current data in a new MarblesRenderData instance.
      return new MarblesRenderData( centers, radii, colors );

      // }}
    }

    /// <summary>
    /// Handles mouse-button push.
    /// </summary>
    /// <returns>True if handled.</returns>
    public bool MouseButtonDown ( MouseEventArgs e )
    {
      if ( e.Button != MouseButtons.Right )
        return false;

      return false;
    }

    /// <summary>
    /// Handles mouse-button release.
    /// </summary>
    /// <returns>True if handled.</returns>
    public bool MouseButtonUp ( MouseEventArgs e )
    {
      if ( e.Button != MouseButtons.Right )
        return false;

      return false;
    }

    /// <summary>
    /// Handles mouse move.
    /// </summary>
    /// <returns>True if handled.</returns>
    public bool MousePointerMove ( MouseEventArgs e )
    {
      if ( e.Button != MouseButtons.Right )
        return false;

      return false;
    }

    /// <summary>
    /// Handles mouse wheel change.
    /// </summary>
    /// <returns>True if handled.</returns>
    public bool MouseWheelChange ( MouseEventArgs e )
    {
      // Warning: if you handle this event, Trackball won't get it..

      return false;
    }

    /// <summary>
    /// Handles keyboard key release.
    /// </summary>
    /// <returns>True if handled.</returns>
    public bool KeyHandle ( KeyEventArgs e )
    {
      return false;
    }
  }

  /// <summary>
  /// Data object containing all OpenGL-related values.
  /// </summary>
  public class OpenglState
  {
    /// <summary>
    /// Can we use shaders?
    /// </summary>
    public bool canShaders = false;

    /// <summary>
    /// Are we currently using shaders?
    /// </summary>
    public bool useShaders = false;

    /// <summary>
    /// Vertex array VBO (colors, normals, coords), index array VBO
    /// </summary>
    public uint[] VBOid = null;

    /// <summary>
    /// Global GLSL program repository.
    /// </summary>
    public Dictionary<string, GlProgramInfo> programs = new Dictionary<string, GlProgramInfo>();

    /// <summary>
    /// Current (active) GLSL program.
    /// </summary>
    public GlProgram activeProgram = null;

    /// <summary>
    /// Associated GLControl object.
    /// </summary>
    GLControl glC;

    /// <summary>
    /// Associated Form (camera handling) object.
    /// </summary>
    Form1 form;

    public bool useNormals     = false;
    public bool useTexture     = false;
    public bool useGlobalColor = false;

    /// <summary>
    /// Get data (rendering options) from the form.
    /// </summary>
    public void FormData ()
    {
      useNormals     = form.checkNormals.Checked;
      useTexture     = form.checkTexture.Checked;
      useGlobalColor = form.checkGlobalColor.Checked;
    }

    public OpenglState ( Form1 f, GLControl glc )
    {
      form = f;
      glC  = glc;
    }

    /// <summary>
    /// OpenGL init code.
    /// </summary>
    public void InitOpenGL ()
    {
      // log OpenGL info just for curiosity:
      GlInfo.LogGLProperties();

      // general OpenGL:
      glC.VSync = true;
      GL.ClearColor( Color.FromArgb( 30, 40, 90 ) );
      GL.Enable( EnableCap.DepthTest );
      GL.ShadeModel( ShadingModel.Flat );

      // VBO init:
      VBOid = new uint[ 2 ];           // one big buffer for vertex data, another buffer for tri/line indices
      GL.GenBuffers( 2, VBOid );
      GlInfo.LogError( "VBO init" );

      // shaders:
      canShaders = SetupShaders();

      // texture:
      GenerateTexture();
    }

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

    // Generated texture.
    const int TEX_SIZE = 128;
    const int TEX_CHECKER_SIZE = 8;
    static Vector3 colWhite = new Vector3( 0.85f, 0.75f, 0.15f );
    static Vector3 colBlack = new Vector3( 0.15f, 0.15f, 0.60f );
    static Vector3 colShade = new Vector3( 0.15f, 0.15f, 0.15f );

    /// <summary>
    /// Texture handle
    /// </summary>
    public int texName = 0;

    /// <summary>
    /// Generate the texture.
    /// </summary>
    public void GenerateTexture ()
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

    public void DestroyTexture ()
    {
      if ( texName != 0 )
      {
        GL.DeleteTexture( texName );
        texName = 0;
      }
    }

    public static int Align ( int address )
    {
      return ((address + 15) & -16);
    }

    // Appearance.
    public Vector3 globalAmbient = new Vector3(   0.2f,  0.2f,  0.2f );
    public Vector3 matAmbient    = new Vector3(   1.0f,  0.8f,  0.3f );
    public Vector3 matDiffuse    = new Vector3(   1.0f,  0.8f,  0.3f );
    public Vector3 matSpecular   = new Vector3(   0.8f,  0.8f,  0.8f );
    public float matShininess    = 100.0f;
    public Vector3 whiteLight    = new Vector3(   1.0f,  1.0f,  1.0f );
    public Vector3 lightPosition = new Vector3( -20.0f, 10.0f, 15.0f );
    public Vector3 eyePosition   = new Vector3(   0.0f,  0.0f, 10.0f );

    public void SetLightEye ( float size )
    {
      size *= 0.4f;
      lightPosition = new Vector3( -2.0f * size, size, 1.5f * size );
      eyePosition   = new Vector3(         0.0f, 0.0f,        size );
    }

    public Matrix4 GetModelView ()
    {
      return form.tb.ModelView;
    }

    public Matrix4 GetModelViewInv ()
    {
      return form.tb.ModelViewInv;
    }

    public Matrix4 GetProjection ()
    {
      return form.tb.Projection;
    }

    public Vector3 GetEyePosition ()
    {
      return form.tb.Eye;
    }

    // Attribute/vertex arrays.
    public void SetVertexAttrib ( bool on )
    {
      if ( activeProgram != null )
        if ( on )
          activeProgram.EnableVertexAttribArrays();
        else
          activeProgram.DisableVertexAttribArrays();
    }

    public void InitShaderRepository ()
    {
      programs.Clear();
      GlProgramInfo pi;

      // Default program.
      pi = new GlProgramInfo( "default", new GlShaderInfo[] {
        new GlShaderInfo( ShaderType.VertexShader,   "vertex.glsl",   "058marbles" ),
        new GlShaderInfo( ShaderType.FragmentShader, "fragment.glsl", "058marbles" ) } );
      programs[ pi.name ] = pi;

      // Put more programs here.
      // pi = new GlProgramInfo( ..
      //   ..
      // programs[ pi.name ] = pi;
    }

    public void DestroyResources ()
    {
      DestroyTexture();

      activeProgram = null;
      programs.Clear();

      if ( VBOid != null &&
           VBOid[ 0 ] != 0 )
      {
        GL.DeleteBuffers( 2, VBOid );
        VBOid = null;
      }
    }
  }
}
