using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using MathSupport;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Utilities;

namespace _113graph
{
  public partial class Form1 : Form
  {
    static readonly string rev = Util.SetVersion( "$Rev$" );

    /// <summary>    /// <summary>
    /// Scene center point.
    /// </summary>
    Vector3 center = Vector3.Zero;

    /// <summary>
    /// Scene diameter.
    /// </summary>
    float diameter = 4.0f;

    float near = 0.1f;
    float far = 5.0f;

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

    string tooltip;
    ToolTip tt = new ToolTip();

    /// <summary>
    /// Graph object.
    /// </summary>
    Graph gr = null;

    long lastFpsTime = 0L;
    int frameCounter = 0;
    long primitiveCounter = 0L;
    double lastFps = 0.0;
    double lastPps = 0.0;
    
    public Form1 ()
    {
      InitializeComponent();

      string param;
      string name;
      string expr;
      MouseButtons trackballButton;
      Graph.InitParams( out param, out tooltip, out expr, out name, out trackballButton );
      textParam.Text = param ?? "";
      textExpression.Text = expr;
      Text += " (" + rev + ") '" + name + '\'';

      // Trackball:
      tb = new Trackball( center, diameter );
      tb.Button = trackballButton;

      // Graph object:
      gr = new Graph();
    }

    private void glControl1_Load ( object sender, EventArgs e )
    {
      gr.InitOpenGL( glControl1 );
      gr.InitSimulation( textParam.Text, textExpression.Text );
      tb.GLsetupViewport( glControl1.Width, glControl1.Height, near, far );

      loaded = true;
      Application.Idle += new EventHandler( Application_Idle );
    }

    private void glControl1_Resize ( object sender, EventArgs e )
    {
      if ( !loaded ) return;

      SetupViewport( false );
      glControl1.Invalidate();
    }

    private void glControl1_Paint ( object sender, PaintEventArgs e )
    {
      Render();
    }

    private void control_PreviewKeyDown ( object sender, PreviewKeyDownEventArgs e )
    {
      if ( e.KeyCode == Keys.Up ||
           e.KeyCode == Keys.Down ||
           e.KeyCode == Keys.Left ||
           e.KeyCode == Keys.Right )
        e.IsInputKey = true;
    }

    /// <summary>
    /// Unproject support
    /// </summary>
    Vector3 screenToWorld ( int x, int y, float z = 0.0f )
    {
      Matrix4 modelViewMatrix, projectionMatrix;
      GL.GetFloat( GetPName.ModelviewMatrix, out modelViewMatrix );
      GL.GetFloat( GetPName.ProjectionMatrix, out projectionMatrix );

      return Geometry.UnProject( ref projectionMatrix, ref modelViewMatrix, glControl1.Width, glControl1.Height, x, glControl1.Height - y, z );
    }

    private void glControl1_MouseDown ( object sender, MouseEventArgs e )
    {
      if ( !tb.MouseDown( e ) )
        if ( checkDebug.Checked )
        {
          // pointing to the scene:
          pointOrigin = screenToWorld( e.X, e.Y, 0.0f );
          pointTarget = screenToWorld( e.X, e.Y, 1.0f );
          pointDirty = true;
        }
        else
          MouseButtonDown( e );
    }

    private void glControl1_MouseUp ( object sender, MouseEventArgs e )
    {
      if ( !tb.MouseUp( e ) )
        MouseButtonUp( e );
    }

    private void glControl1_MouseMove ( object sender, MouseEventArgs e )
    {
      if ( !tb.MouseMove( e ) )
        MousePointerMove( e );
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
            frustumFrame.Add( screenToWorld( 0, 0, N ) );
            frustumFrame.Add( screenToWorld( R, 0, N ) );
            frustumFrame.Add( screenToWorld( 0, B, N ) );
            frustumFrame.Add( screenToWorld( R, B, N ) );
            frustumFrame.Add( screenToWorld( 0, 0, F ) );
            frustumFrame.Add( screenToWorld( R, 0, F ) );
            frustumFrame.Add( screenToWorld( 0, B, F ) );
            frustumFrame.Add( screenToWorld( R, B, F ) );
          }
        }
        else
          KeyHandle( e );
    }

    /// <summary>
    /// Prepare VBO content and upload it to the GPU.
    /// </summary>
    private void PrepareDataBuffers ()
    {
      if ( useVBO &&
           scene != null &&
           scene.Triangles > 0 )
      {
        GL.EnableClientState( ArrayCap.VertexArray );
        if ( scene.HasNormals() )
          GL.EnableClientState( ArrayCap.NormalArray );
        if ( scene.HasColors() )
          GL.EnableClientState( ArrayCap.ColorArray );

        // Vertex array: [color] [normal] coord
        GL.BindBuffer( BufferTarget.ArrayBuffer, VBOid[ 0 ] );
        int vertexBufferSize = scene.VertexBufferSize( true, false, true, true );
        GL.BufferData( BufferTarget.ArrayBuffer, (IntPtr)vertexBufferSize, IntPtr.Zero, BufferUsageHint.StaticDraw );
        IntPtr videoMemoryPtr = GL.MapBuffer( BufferTarget.ArrayBuffer, BufferAccess.WriteOnly );
        unsafe
        {
          stride = scene.FillVertexBuffer( (float*)videoMemoryPtr.ToPointer(), true, false, true, true );
        }
        GL.UnmapBuffer( BufferTarget.ArrayBuffer );
        GL.BindBuffer( BufferTarget.ArrayBuffer, 0 );

        // Index buffer
        GL.BindBuffer( BufferTarget.ElementArrayBuffer, VBOid[ 1 ] );
        GL.BufferData( BufferTarget.ElementArrayBuffer, (IntPtr)(scene.Triangles * 3 * sizeof( uint )), IntPtr.Zero, BufferUsageHint.StaticDraw );
        videoMemoryPtr = GL.MapBuffer( BufferTarget.ElementArrayBuffer, BufferAccess.WriteOnly );
        unsafe
        {
          scene.FillIndexBuffer( (uint*)videoMemoryPtr.ToPointer() );
        }
        GL.UnmapBuffer( BufferTarget.ElementArrayBuffer );
        GL.BindBuffer( BufferTarget.ElementArrayBuffer, 0 );
      }
      else
      {
        GL.DisableClientState( ArrayCap.VertexArray );
        GL.DisableClientState( ArrayCap.NormalArray );
        GL.DisableClientState( ArrayCap.ColorArray );

        if ( useVBO )
        {
          GL.BindBuffer( BufferTarget.ArrayBuffer, VBOid[ 0 ] );
          GL.BufferData( BufferTarget.ArrayBuffer, (IntPtr)0, IntPtr.Zero, BufferUsageHint.StaticDraw );
          GL.BindBuffer( BufferTarget.ArrayBuffer, 0 );
          GL.BindBuffer( BufferTarget.ElementArrayBuffer, VBOid[ 1 ] );
          GL.BufferData( BufferTarget.ElementArrayBuffer, (IntPtr)0, IntPtr.Zero, BufferUsageHint.StaticDraw );
          GL.BindBuffer( BufferTarget.ElementArrayBuffer, 0 );
        }
      }
    }
  }
}
