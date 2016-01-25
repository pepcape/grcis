using System;
using System.Threading;
using System.Windows.Forms;
using OpenglSupport;
using OpenTK;

namespace _058marbles
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
    protected float diameter = 30.0f;

    /// <summary>
    /// GLControl guard flag.
    /// </summary>
    bool loaded = false;

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
      bool useTexture, globalColor, useNormals, useWireframe, useMT;
      InitParams( out param, out center, out diameter, out useTexture, out globalColor, out useNormals, out useWireframe, out useMT );
      checkTexture.Checked = useTexture;
      checkGlobalColor.Checked = globalColor;
      checkNormals.Checked = useNormals;
      checkWireframe.Checked = useWireframe;
      checkMultithread.Checked = useMT;
      textParam.Text = param ?? "";
      Text += " (rev: " + rev + ')';

      OGL = new OpenglState( this, glControl1 );
      OGL.InitShaderRepository();
    }

    void StopSimulation ()
    {
      simulate = false;
      if ( simThread == null )
        return;

      simThread.Join();
      simThread = null;
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

    /// <summary>
    /// Infinite simulation loop.
    /// </summary>
    void SimulationLoop ()
    {
      while ( simulate )
        Simulate();
    }

    private void buttonResetSim_Click ( object sender, EventArgs e )
    {
      StopSimulation();

      ResetSimulation();

      StartSimulation();
    }

    private void glControl1_Load ( object sender, EventArgs e )
    {
      OGL.InitOpenGL();

      // Simulation scene and related stuff:
      InitSimulation();

      SetupViewport();

      loaded = true;
      Application.Idle += new EventHandler( Application_Idle );

      // Start the simulation thread?
      StartSimulation();
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

    private void buttonStart_Click ( object sender, EventArgs e )
    {
      PauseRestartSimulation();
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

    private void checkVsync_CheckedChanged ( object sender, EventArgs e )
    {
      glControl1.VSync = checkVsync.Checked;
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
      StopSimulation();
    }

    private void checkMultithread_CheckedChanged ( object sender, EventArgs e )
    {
      if ( checkMultithread.Checked )
        StartSimulation();
      else
        StopSimulation();
    }
  }
}
