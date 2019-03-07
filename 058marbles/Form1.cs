using System;
using System.Threading;
using System.Windows.Forms;
using MathSupport;
using OpenglSupport;
using OpenTK;
using Utilities;

namespace _058marbles
{
  public partial class Form1 : Form
  {
    static readonly string rev = Util.SetVersion( "$Rev$" );

    /// <summary>
    /// Scene center point.
    /// </summary>
    protected Vector3 center = Vector3.Zero;

    /// <summary>
    /// Scene diameter.
    /// </summary>
    protected float diameter = 30.0f;

    float near = 0.1f;
    float far  = 100.0f;

    /// <summary>
    /// GLControl guard flag.
    /// </summary>
    bool loaded = false;

    /// <summary>
    /// Associated Trackball instance.
    /// </summary>
    public Trackball tb = null;

    /// <summary>
    /// Cached tooltip string.
    /// </summary>
    string tooltip;

    /// <summary>
    /// Shared global ToolTip instance.
    /// </summary>
    ToolTip tt = new ToolTip();

    /// <summary>
    /// OpenGL state.
    /// </summary>
    OpenglState OGL;

    /// <summary>
    /// Simulation worker-thread.
    /// </summary>
    Thread simThread = null;

    /// <summary>
    /// Simulation-control flag.
    /// </summary>
    volatile bool simulate = false;

    /// <summary>
    /// Async screencast object instance, null if not screencasting.
    /// </summary>
    public static Snapshots screencast = null;

    public Form1 ()
    {
      InitializeComponent();

      string param;
      string name;
      bool useTexture, globalColor, useNormals, useWireframe, useMT;
      InitParams( out name, out param, out tooltip,
                  out center, out diameter,
                  out useTexture, out globalColor, out useNormals, out useWireframe, out useMT );
      checkTexture.Checked     = useTexture;
      checkGlobalColor.Checked = globalColor;
      checkNormals.Checked     = useNormals;
      checkWireframe.Checked   = useWireframe;
      checkMultithread.Checked = useMT;
      textParam.Text           = param ?? "";
      Text += " (" + rev + ") '" + name + '\'';

      // Trackball.
      tb = new Trackball( center, diameter );
      tb.Button = MouseButtons.Left;

      OGL = new OpenglState( this, glControl1 );
      OGL.InitShaderRepository();
    }

    private void glControl1_Load ( object sender, EventArgs e )
    {
      OGL.InitOpenGL();

      // Simulation scene and related stuff:
      InitSimulation( textParam.Text );

      tb.GLsetupViewport( glControl1.Width, glControl1.Height, near, far );

      loaded = true;
      Application.Idle += new EventHandler( Application_Idle );

      // Start the simulation thread?
      StartSimulation();
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

    private void Form1_FormClosing ( object sender, FormClosingEventArgs e )
    {
      if ( screencast != null )
        screencast.StopSaveThread();

      StopSimulation();

      if ( OGL != null )
      {
        OGL.DestroyResources();
        OGL = null;
      }
    }

    void StartSimulation ()
    {
      if ( simThread != null ||
           !checkMultithread.Checked ||
           Environment.ProcessorCount < 2 )
        return;

      simulate = true;
      simThread = new Thread( new ThreadStart( SimulationLoop ) );
      simThread.Priority = ThreadPriority.AboveNormal;
      simThread.Start();
    }

    void StopSimulation ()
    {
      simulate = false;
      if ( simThread == null )
        return;

      simThread.Join();
      simThread = null;
    }

    /// <summary>
    /// Infinite simulation loop.
    /// </summary>
    void SimulationLoop ()
    {
      while ( simulate )
        Simulate();
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

    private void buttonResetSim_Click ( object sender, EventArgs e )
    {
      StopSimulation();

      ResetSimulation( textParam.Text );

      StartSimulation();
    }

    private void buttonStart_Click ( object sender, EventArgs e )
    {
      PauseRestartSimulation();
    }

    private void buttonUpdate_Click ( object sender, EventArgs e )
    {
      UpdateSimulation( textParam.Text );
    }

    private void textParam_KeyPress ( object sender, System.Windows.Forms.KeyPressEventArgs e )
    {
      if ( e.KeyChar == (char)Keys.Enter )
      {
        e.Handled = true;
        UpdateSimulation( textParam.Text );
      }
    }

    private void checkVsync_CheckedChanged ( object sender, EventArgs e )
    {
      glControl1.VSync = checkVsync.Checked;
    }

    private void checkMultithread_CheckedChanged ( object sender, EventArgs e )
    {
      if ( checkMultithread.Checked )
        StartSimulation();
      else
        StopSimulation();
    }

    private void buttonReset_Click ( object sender, EventArgs e )
    {
      tb.Reset();
    }
  }
}
