using System;
using System.Windows.Forms;
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
    protected float diameter = 4.0f;

    /// <summary>
    /// GLControl guard flag.
    /// </summary>
    bool loaded = false;

    public Form1 ()
    {
      InitializeComponent();

      string param;
      Fireworks.InitParams( out param, out center, out diameter );
      textParam.Text = param ?? "";
      Text += " (rev: " + rev + ')';

      InitShaderRepository();
    }

    private void glControl1_Load ( object sender, EventArgs e )
    {
      InitOpenGL();
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
    }

    private void buttonResetSim_Click ( object sender, EventArgs e )
    {
      ResetCamera();
    }
  }
}
