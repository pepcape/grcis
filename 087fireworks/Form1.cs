using System;
using System.Windows.Forms;
using MathSupport;
using OpenglSupport;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Utilities;

namespace _087fireworks
{
  public partial class Form1 : Form
  {
    static readonly string rev = Util.SetVersion("$Rev$");

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

    /// <summary>
    /// Default scene center.
    /// </summary>
    Vector3 center;

    /// <summary>
    /// Default scene diameter.
    /// </summary>
    float diameter;

    string tooltip;
    ToolTip tt = new ToolTip();

    public Form1 ()
    {
      InitializeComponent();

      // Form params:
      string param;
      bool useTexture, globalColor, useNormals, usePtSize;
      string name;
      MouseButtons trackballButton;
      InitParams(out param, out tooltip, out name, out trackballButton,
                 out center, out diameter, out useTexture, out globalColor, out useNormals, out usePtSize);
      checkTexture.Checked = useTexture;
      checkGlobalColor.Checked = globalColor;
      checkNormals.Checked = useNormals;
      checkPointSize.Checked = usePtSize;
      textParam.Text = param ?? "";
      Text += " (" + rev + ") '" + name + '\'';
      SetLightEye(center, diameter);

      // Trackball:
      tb = new Trackball(center, diameter);
      tb.Button = trackballButton;

      InitShaderRepository();
    }

    private void glControl1_Load (object sender, EventArgs e)
    {
      InitOpenGL();
      InitSimulation();
      tb.GLsetupViewport(glControl1.Width, glControl1.Height);

      loaded = true;
      Application.Idle += new EventHandler(Application_Idle);
    }

    private void glControl1_Resize (object sender, EventArgs e)
    {
      if (!loaded)
        return;

      tb.GLsetupViewport(glControl1.Width, glControl1.Height);
      glControl1.Invalidate();
    }

    private void glControl1_Paint (object sender, PaintEventArgs e)
    {
      Render();
    }

    private void checkVsync_CheckedChanged (object sender, EventArgs e)
    {
      glControl1.VSync = checkVsync.Checked;
    }

    private void buttonStart_Click (object sender, EventArgs e)
    {
      PauseRestartSimulation();
    }

    private void buttonResetSim_Click (object sender, EventArgs e)
    {
      ResetSimulation();
    }

    private void buttonUpdate_Click (object sender, EventArgs e)
    {
      UpdateSimulation();
    }

    private void textParam_KeyPress (object sender, System.Windows.Forms.KeyPressEventArgs e)
    {
      if (e.KeyChar == (char)Keys.Enter)
      {
        e.Handled = true;
        UpdateSimulation();
      }
    }

    public static void StartStopScreencast (bool start)
    {
      if (start == (screencast != null))
        return;

      if (start)
      {
        screencast = new Snapshots(true);
        screencast.StartSaveThread();
      }
      else
      {
        screencast.StopSaveThread();
        screencast = null;
      }
    }

    private void Form1_FormClosing (object sender, FormClosingEventArgs e)
    {
      if (screencast != null)
        screencast.StopSaveThread();

      if (VBOid != null &&
          VBOid[0] != 0)
      {
        GL.DeleteBuffers(2, VBOid);
        VBOid = null;
      }
    }

    private void glControl1_MouseDown (object sender, MouseEventArgs e)
    {
      if (!tb.MouseDown(e) &&
          fw != null)
        fw.MouseButtonDown(e);
    }

    private void glControl1_MouseUp (object sender, MouseEventArgs e)
    {
      if (!tb.MouseUp(e) &&
          fw != null)
        fw.MouseButtonUp(e);
    }

    private void glControl1_MouseMove (object sender, MouseEventArgs e)
    {
      if (!tb.MouseMove(e) &&
          fw != null)
        fw.MousePointerMove(e);
    }

    private void glControl1_MouseWheel (object sender, MouseEventArgs e)
    {
      tb.MouseWheel(e);
    }

    private void glControl1_KeyDown (object sender, KeyEventArgs e)
    {
      tb.KeyDown(e);
    }

    private void glControl1_KeyUp (object sender, KeyEventArgs e)
    {
      if (!tb.KeyUp(e) &&
          fw != null)
        fw.KeyHandle(e);
    }

    private void buttonReset_Click (object sender, EventArgs e)
    {
      tb.Center   = center;
      tb.Diameter = diameter;
      tb.Reset();
    }

    private void labelStat_MouseHover (object sender, EventArgs e)
    {
      tt.Show(Util.TargetFramework + " (" + Util.RunningFramework + "), OpenTK " + Util.AssemblyVersion(typeof(Vector3)),
              (IWin32Window)sender, 10, -25, 4000);
    }

    private void textParam_MouseHover (object sender, EventArgs e)
    {
      tt.Show(tooltip, (IWin32Window)sender, 10, -25, 4000);
    }

    private void glControl1_PreviewKeyDown (object sender, PreviewKeyDownEventArgs e)
    {
      if (e.KeyCode == Keys.Up ||
          e.KeyCode == Keys.Down ||
          e.KeyCode == Keys.Left ||
          e.KeyCode == Keys.Right)
        e.IsInputKey = true;
    }
  }
}
