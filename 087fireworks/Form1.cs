using System;
using System.Windows.Forms;
using OpenglSupport;
using OpenTK;

namespace _087fireworks
{
  public partial class Form1 : Form
  {
    static readonly string rev = "$Rev$".Split( ' ' )[ 1 ];

    /// <summary>
    /// Scene center point.
    /// </summary>
    protected Vector3 center = Vector3.Zero;

    /// <summary>
    /// Scene diameter.
    /// </summary>
    protected float diameter = 6.0f;

    /// <summary>
    /// GLControl guard flag.
    /// </summary>
    bool loaded = false;

    /// <summary>
    /// Async screencast object instance, null if not screencasting.
    /// </summary>
    public static Snapshots screencast = null;

    public Form1 ()
    {
      InitializeComponent();

      string param;
      bool useTexture, globalColor, useNormals, usePtSize;
      InitParams( out param, out center, out diameter, out useTexture, out globalColor, out useNormals, out usePtSize );
      checkTexture.Checked = useTexture;
      checkGlobalColor.Checked = globalColor;
      checkNormals.Checked = useNormals;
      checkPointSize.Checked = usePtSize;
      textParam.Text = param ?? "";
      Text += " (rev: " + rev + ')';

      InitShaderRepository();
    }

    private void glControl1_Load ( object sender, EventArgs e )
    {
      InitOpenGL();
      InitSimulation();
      SetupViewport();

      loaded = true;
      Application.Idle += new EventHandler( Application_Idle );
    }

    private void glControl1_Resize ( object sender, EventArgs e )
    {
      if ( !loaded ) return;

      SetupViewport();
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
    }
  }
}
