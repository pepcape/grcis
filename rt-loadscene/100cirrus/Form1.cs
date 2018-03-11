using System;
using System.Windows.Forms;
using MathSupport;
using OpenglSupport;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace _100cirrus
{
  public partial class Form1 : Form
  {
    static readonly string rev = "$Rev$".Split( ' ' )[ 1 ];

    /// <summary>
    /// GLControl guard flag.
    /// </summary>
    bool loaded = false;

    /// <summary>
    /// Async screencast object instance, null if not screencasting.
    /// </summary>
    public static Snapshots screencast = null;

    /// <summary>
    /// Associated Trackball instance.
    /// </summary>
    Trackball tb = null;

    public Form1 ()
    {
      InitializeComponent();

      // Form params:
      string param;
      bool useTexture, globalColor, useNormals, usePtSize;
      Vector3 center;
      float diameter;
      InitParams( out param, out center, out diameter, out useTexture, out globalColor, out useNormals, out usePtSize );
      checkTexture.Checked = useTexture;
      checkGlobalColor.Checked = globalColor;
      checkNormals.Checked = useNormals;
      checkPointSize.Checked = usePtSize;
      textParam.Text = param ?? "";
      Text += " (rev: " + rev + ')';
      SetLightEye( center, diameter );

      // Trackball:
      tb = new Trackball( center, diameter );

      InitShaderRepository();
    }

    private void glControl1_Load ( object sender, EventArgs e )
    {
      InitOpenGL();
      InitSimulation();
      tb.GLsetupViewport( glControl1.Width, glControl1.Height );

      loaded = true;
      Application.Idle += new EventHandler( Application_Idle );
    }

    private void glControl1_Resize ( object sender, EventArgs e )
    {
      if ( !loaded ) return;

      tb.GLsetupViewport( glControl1.Width, glControl1.Height );
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

    private void buttonStart_Click ( object sender, EventArgs e )
    {
      PauseRestartSimulation();
    }

    private void buttonResetSim_Click ( object sender, EventArgs e )
    {
      ResetSimulation();
    }

    private void buttonUpdate_Click ( object sender, EventArgs e )
    {
      UpdateSimulation();
    }

    private void textParam_KeyPress ( object sender, System.Windows.Forms.KeyPressEventArgs e )
    {
      if ( e.KeyChar == (char)Keys.Enter )
      {
        e.Handled = true;
        UpdateSimulation();
      }
    }

    public static void StartStopScreencast ( bool start )
    {
      if ( start == (screencast != null) )
        return;

      if ( start )
      {
        screencast = new Snapshots( true );
        screencast.StartSaveThread();
      }
      else
      {
        screencast.StopSaveThread();
        screencast = null;
      }
    }

    private void Form1_FormClosing ( object sender, FormClosingEventArgs e )
    {
      if ( screencast != null )
        screencast.StopSaveThread();
      if ( VBOid != null &&
           VBOid[ 0 ] != 0 )
      {
        GL.DeleteBuffers( 2, VBOid );
        VBOid = null;
      }
    }

    private void glControl1_MouseDown ( object sender, MouseEventArgs e )
    {
      tb.MouseDown( e );
    }

    private void glControl1_MouseUp ( object sender, MouseEventArgs e )
    {
      tb.MouseUp( e );
    }

    private void glControl1_MouseMove ( object sender, MouseEventArgs e )
    {
      tb.MouseMove( e );
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
      tb.KeyUp( e );
    }

    private void buttonReset_Click ( object sender, EventArgs e )
    {
      tb.Reset();
    }
  }
}
