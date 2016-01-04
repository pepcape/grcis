using System;
using System.Drawing;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;

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

    public Form1 ()
    {
      InitializeComponent();

      string param;
      bool useTexture, globalColor, useNormals, useWireframe;
      InitParams( out param, out center, out diameter, out useTexture, out globalColor, out useNormals, out useWireframe );
      checkTexture.Checked = useTexture;
      checkGlobalColor.Checked = globalColor;
      checkNormals.Checked = useNormals;
      checkWireframe.Checked = useWireframe;
      textParam.Text = param ?? "";
      Text += " (rev: " + rev + ')';

      OGL = new OpenglState( this, glControl1 );
      OGL.InitShaderRepository();
    }

    private void buttonResetSim_Click ( object sender, EventArgs e )
    {
      // TODO: stop the simulation thread?

      ResetSimulation();

      // TODO: start the simulation thread?
    }

    private void glControl1_Load ( object sender, EventArgs e )
    {
      OGL.InitOpenGL();

      // Simulation scene and related stuff:
      InitSimulation();

      SetupViewport();

      loaded = true;
      Application.Idle += new EventHandler( Application_Idle );

      // TODO: start the simulation thread?
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
  }
}
