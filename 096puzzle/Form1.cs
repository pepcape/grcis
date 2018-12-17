using System;
using System.Collections.Generic;
using System.Windows.Forms;
using MathSupport;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Utilities;

namespace _096puzzle
{
  public partial class Form1 : Form
  {
    static readonly string rev = Util.SetVersion( "$Rev$" );

    /// <summary>
    /// Scene center point.
    /// </summary>
    Vector3 center = Vector3.Zero;

    /// <summary>
    /// Scene diameter.
    /// </summary>
    float diameter = 4.0f;

    float near = 0.1f;
    float far  = 5.0f;

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

    /// <summary>
    /// Cached tooltip string.
    /// </summary>
    string tooltip;

    /// <summary>
    /// Shared global ToolTip instance.
    /// </summary>
    ToolTip tt = new ToolTip();

    public Form1 ()
    {
      InitializeComponent();

      string param;
      string name;
      InitParams( out name, out param, out tooltip, out center, out diameter );
      textParam.Text = param ?? "";
      Text += " (" + rev + ") '" + name + '\'';

      // Trackball.
      tb = new Trackball( center, diameter );
      tb.Button = MouseButtons.Left;
    }

    private void glControl1_Load ( object sender, EventArgs e )
    {
      InitOpenGL();

      InitSimulation( textParam.Text );

      tb.GLsetupViewport( glControl1.Width, glControl1.Height, near, far );

      loaded = true;
      Application.Idle += new EventHandler( Application_Idle );
    }

    private void glControl1_Resize ( object sender, EventArgs e )
    {
      if ( !loaded ) return;

      tb.GLsetupViewport( glControl1.Width, glControl1.Height, near, far );
      glControl1.Invalidate();
    }

    private void glControl1_Paint ( object sender, PaintEventArgs e )
    {
      Render();
    }

    private void checkVsync_CheckedChanged ( object sender, EventArgs e )
    {
      glControl1.VSync = checkVsync.Checked;
    }

    private void textParam_KeyPress ( object sender, System.Windows.Forms.KeyPressEventArgs e )
    {
      if ( e.KeyChar == (char)Keys.Enter )
      {
        e.Handled = true;
        UpdateSimulation();
      }
    }

    private void Form1_FormClosing ( object sender, FormClosingEventArgs e )
    {
      DestroyTexture( ref texName );

      if ( VBOid != null &&
           VBOid[ 0 ] != 0 )
      {
        GL.DeleteBuffers( 2, VBOid );
        VBOid = null;
      }
    }

    /// <summary>
    /// Unproject support.
    /// </summary>
    Vector3 screenToWorld ( int x, int y, float z =0.0f )
    {
      Matrix4 modelViewMatrix, projectionMatrix;
      GL.GetFloat( GetPName.ModelviewMatrix,  out modelViewMatrix );
      GL.GetFloat( GetPName.ProjectionMatrix, out projectionMatrix );

      return Geometry.UnProject( ref projectionMatrix, ref modelViewMatrix, glControl1.Width, glControl1.Height, x, glControl1.Height - y, z );
    }

    private void glControl1_MouseDown ( object sender, MouseEventArgs e )
    {
      if ( !MouseButtonDown( e ) )
        tb.MouseDown( e );

      if ( checkDebug.Checked &&
           e.Button != tb.Button )
      {
        // Pointing to the scene.
        pointOrigin = screenToWorld( e.X, e.Y, 0.0f );
        pointTarget = screenToWorld( e.X, e.Y, 1.0f );
        pointDirty  = true;
      }
    }

    private void glControl1_MouseUp ( object sender, MouseEventArgs e )
    {
      if ( !MouseButtonUp( e ) )
        tb.MouseUp( e );
    }

    private void glControl1_MouseMove ( object sender, MouseEventArgs e )
    {
      if ( !MousePointerMove( e ) )
        tb.MouseMove( e );
    }

    private void glControl1_MouseWheel ( object sender, MouseEventArgs e )
    {
      if ( !MouseWheelChange(e) )
        tb.MouseWheel( e );
    }

    private void glControl1_KeyDown ( object sender, KeyEventArgs e )
    {
      tb.KeyDown( e );
    }

    private void glControl1_KeyUp ( object sender, KeyEventArgs e )
    {
      // Simulation has the priority.
      if ( KeyHandle( e ) )
        return;

      // Trackball is next.
      if ( !tb.KeyUp( e ) )
        // 'F' for frustum visualization.
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
    }

    private void buttonResetCam_Click ( object sender, EventArgs e )
    {
      tb.Reset();
    }

    private void labelStatus_MouseHover ( object sender, EventArgs e )
    {
      tt.Show( Util.TargetFramework + " (" + Util.RunningFramework + "), OpenTK " + Util.AssemblyVersion( typeof( Vector3 ) ),
               (IWin32Window)sender, 10, -25, 4000 );
    }

    private void buttonStart_Click ( object sender, EventArgs e )
    {
      PauseRestartSimulation();
    }

    private void buttonResetSim_Click ( object sender, EventArgs e )
    {
      ResetSimulation( textParam.Text );
    }

    private void buttonUpdate_Click ( object sender, EventArgs e )
    {
      UpdateSimulation();
    }

    private void textParam_MouseHover ( object sender, EventArgs e )
    {
      tt.Show( tooltip, (IWin32Window)sender, 10, -25, 4000 );
    }
  }
}
