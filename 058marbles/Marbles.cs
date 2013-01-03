using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace _058marbles
{
  /// <summary>
  /// Rendering data passed from MarblesWorld to MarblesRenderer
  /// </summary>
  public class MarblesRenderData
  {
    // !!!{{ TODO: modify/add data

    public List<Vector3> centers;

    public List<float> radii;

    public List<Color> colors;

    // !!!}}
  }

  /// <summary>
  /// Renderer: can interpret MarblesRenderData and converts them into
  /// OpenGL commands..
  /// </summary>
  public class MarblesRenderer
  {
    /// <summary>
    /// Number of marbles (triangles) VBOs are allocated for..
    /// </summary>
    int lastMarbles = -1;

    public void Render ( MarblesRenderData data )
    {
      // !!!{{ TODO: modify the rendering code!

      int marbles = data.radii.Count;
      int stride = sizeof( float ) * 6;   // float[3] color, position
      int triangles = marbles;
      IntPtr videoMemoryPtr;

      if ( marbles != lastMarbles )
      {
        // relocate the buffers:
        GL.EnableClientState( ArrayCap.VertexArray );
        GL.EnableClientState( ArrayCap.ColorArray );

        // vertex array: [ color coord ]
        GL.BindBuffer( BufferTarget.ArrayBuffer, Form1.VBOid[ 0 ] );
        int vertexBufferSize = (marbles * 3) * stride;  // TODO: marble should be drawn as a SPHERE, not a triangle..
        GL.BufferData( BufferTarget.ArrayBuffer, (IntPtr)vertexBufferSize, IntPtr.Zero, BufferUsageHint.DynamicDraw );
        GL.BindBuffer( BufferTarget.ArrayBuffer, 0 );

        // index array:
        GL.BindBuffer( BufferTarget.ElementArrayBuffer, Form1.VBOid[ 1 ] );
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

      // refill vertex buffer:
      GL.BindBuffer( BufferTarget.ArrayBuffer, Form1.VBOid[ 0 ] );
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
          *ptr++ = r;
          *ptr++ = g;
          *ptr++ = b;
          *ptr++ = data.centers[ i ].X;
          *ptr++ = data.centers[ i ].Y;
          *ptr++ = data.centers[ i ].Z;
          *ptr++ = r;
          *ptr++ = g;
          *ptr++ = b;
          *ptr++ = data.centers[ i ].X + rad;
          *ptr++ = data.centers[ i ].Y;
          *ptr++ = data.centers[ i ].Z;
          *ptr++ = r;
          *ptr++ = g;
          *ptr++ = b;
          *ptr++ = data.centers[ i ].X;
          *ptr++ = data.centers[ i ].Y + rad;
          *ptr++ = data.centers[ i ].Z;
        }
        GL.UnmapBuffer( BufferTarget.ArrayBuffer );
      }

      // index buffer
      GL.BindBuffer( BufferTarget.ElementArrayBuffer, Form1.VBOid[ 1 ] );

      // render the scene:
      GL.ColorPointer( 3, ColorPointerType.Float, stride, IntPtr.Zero );
      GL.VertexPointer( 3, VertexPointerType.Float, stride, (IntPtr)(Vector3.SizeInBytes) );

      Form1.triangleCounter += triangles;
      GL.DrawElements( BeginMode.Triangles, triangles * 3, DrawElementsType.UnsignedInt, IntPtr.Zero );

      // !!!}}
    }
  }

  /// <summary>
  /// Simulation class. Holds internal state of the virtual world,
  /// generated rendering data, etc.
  /// </summary>
  public class MarblesWorld
  {
    // !!!{{ TODO: modify/add data

    List<Vector3> centers;

    List<float> radii;

    List<Vector3> velocities;

    List<Color> colors;

    // !!!}}

    /// <summary>
    /// [Re-]initialization of the world.
    /// </summary>
    /// <param name="param">String parameter from user, e.g.: number of marbles.</param>
    public void Init ( string param )
    {
      int marbles = 0;
      int.TryParse( param, out marbles );
      if ( marbles < 12 ) marbles = 12;
      Random rnd = new Random( 144 );

      centers = new List<Vector3>( marbles );
      radii = new List<float>( marbles );
      velocities = new List<Vector3>( marbles );
      colors = new List<Color>( marbles );

      for ( int i = 0; i < marbles; i++ )
      {
        centers.Add( new Vector3( -10.0f + 20.0f * (float)rnd.NextDouble(),
                                  -10.0f + 20.0f * (float)rnd.NextDouble(),
                                  -10.0f + 20.0f * (float)rnd.NextDouble() ) );

        radii.Add( 0.4f + 0.6f * (float)rnd.NextDouble() );

        velocities.Add( new Vector3( -2.0f + 4.0f * (float)rnd.NextDouble(),
                                     -2.0f + 4.0f * (float)rnd.NextDouble(),
                                     -2.0f + 4.0f * (float)rnd.NextDouble() ) );

        colors.Add( Color.FromArgb( rnd.Next( 255 ), rnd.Next( 255 ), rnd.Next( 255 ) ) );

        // !!!{{ TODO: more initialization?
        // !!!}}
      }
    }

    public MarblesRenderData Simulate ( float dTime )
    {
      // !!!{{ TODO: put your own simulation code here

      // Simulate (update) the world:
      for ( int i = 0; i < centers.Count; i++ )
      {
        centers[ i ] += velocities[ i ] * dTime;
        Vector3 vel;
        if ( Math.Abs( centers[ i ].X ) >= 10.0f )
        {
          vel = velocities[ i ];
          vel.X = -vel.X;
          velocities[ i ] = vel;
        }
        if ( Math.Abs( centers[ i ].Y ) >= 10.0f )
        {
          vel = velocities[ i ];
          vel.Y = -vel.Y;
          velocities[ i ] = vel;
        }
        if ( Math.Abs( centers[ i ].Z ) >= 10.0f )
        {
          vel = velocities[ i ];
          vel.Z = -vel.Z;
          velocities[ i ] = vel;
        }
      }

      // Copy the new data into a new MarblesRenderData instance:
      MarblesRenderData d = new MarblesRenderData();
      d.centers = new List<Vector3>( centers );
      d.radii = new List<float>( radii );
      d.colors = new List<Color>( colors );
      return d;

      // !!!}}
    }
  }

  public partial class Form1
  {
    static volatile public MarblesRenderData data = null;

    /// <summary>
    /// Initialized in glControl1_Load()
    /// </summary>
    static volatile public MarblesWorld world = null;

    /// <summary>
    /// Initialized in glControl1_Load()
    /// </summary>
    static public MarblesRenderer renderer = null;

    /// <summary>
    /// Time of the last frame in seconds (for camera movement).
    /// </summary>
    double lastFrameTime = DateTime.Now.Ticks * 1.0e-7;

    /// <summary>
    /// Time of the last simulation in seconds.
    /// </summary>
    double lastSimTime = DateTime.Now.Ticks * 1.0e-7;

    /// <summary>
    /// Function called whenever the main application is idle..
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Application_Idle ( object sender, EventArgs e )
    {
      while ( glControl1.IsIdle )
      {
        // World simulation:
        double timeInSeconds = DateTime.Now.Ticks * 1.0e-7;
        float dTime = (float)(timeInSeconds - lastSimTime);
        lastSimTime = timeInSeconds;
        if ( world != null )
          data = world.Simulate( dTime );

        // Force redraw:
        glControl1.Invalidate();
        Thread.Sleep( 5 );

        long now = DateTime.Now.Ticks;
        if ( now - lastFpsTime > 10000000 )      // more than 1 sec
        {
          double fps = frameCounter * 1.0e7 / (now - lastFpsTime);
          double tps = triangleCounter * 1.0e7 / (now - lastFpsTime);
          lastFpsTime = now;
          frameCounter = 0;
          triangleCounter = 0L;
          if ( tps < 5.0e5 )
            labelFps.Text = String.Format( "Fps: {0:0.0}, Tps: {1:0}k", fps, (tps * 1.0e-3) );
          else
            labelFps.Text = String.Format( "Fps: {0:0.0}, Tps: {1:0.0}m", fps, (tps * 1.0e-6) );
        }
      }
    }

    /// <summary>
    /// Rendering of one frame, all the stuff.
    /// </summary>
    private void Render ()
    {
      if ( !loaded ) return;

      frameCounter++;
      GL.Clear( ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit );
      GL.ShadeModel( checkSmooth.Checked ? ShadingModel.Smooth : ShadingModel.Flat );
      GL.PolygonMode( MaterialFace.FrontAndBack, checkWireframe.Checked ? PolygonMode.Line : PolygonMode.Fill );

      double timeInSeconds = DateTime.Now.Ticks * 1.0e-7;
      float dTime = (float)(timeInSeconds - lastFrameTime);
      lastFrameTime = timeInSeconds;

      // Camera animation:
      SetCamera( checkCamera.Checked ? dTime : 0.0f );

      // Scene rendering:
      RenderScene();

      glControl1.SwapBuffers();
    }

    /// <summary>
    /// OpenGL rendering code itself (separated for clarity).
    /// </summary>
    private void RenderScene ()
    {
      // Simulation scene rendering:
      MarblesRenderData rdata = data;
      if ( rdata != null && renderer != null )
      {
        renderer.Render( rdata );
      }
      else                              // color cube (JB)
      {
        GL.Begin( BeginMode.Quads );

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

        triangleCounter += 12;
      }
    }
  }
}
