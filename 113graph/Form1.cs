using System;
using System.Collections.Generic;
using System.Windows.Forms;
using MathSupport;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Utilities;

namespace _113graph
{
  public partial class Form1 : Form
  {
    static readonly string rev = Util.SetVersion("$Rev: 980 $");

    public static Form1 form = null;

    /// <summary>
    /// GLControl guard flag.
    /// </summary>
    bool loaded = false;

    /// <summary>
    /// False until the 'Regenerate' button is pressed for the first time.
    /// </summary>
    public bool drawGraph = false;

    /// <summary>
    /// Associated Trackball instance.
    /// </summary>
    Trackball tb = null;

    string tooltip;
    ToolTip tt = new ToolTip();

    /// <summary>
    /// Graph object.
    /// </summary>
    Graph gr = new Graph();

    public Form1 ()
    {
      InitializeComponent();
      form = this;

      string param;
      string userTooltip;
      string name;
      string expr;
      MouseButtons trackballButton;
      Graph.InitParams(out param, out userTooltip, out expr, out name, out trackballButton);
      textParam.Text = param ?? "";
      tooltip = "minZoom=<float>,maxZoom=<float>,zoom=<float>,perspective=<bool>,fov=<degrees>,backgr=<color>,light=<vector3>,color=<color>,Ka=<float>,shininess=<float>";
      if (!string.IsNullOrEmpty(userTooltip))
        tooltip = tooltip + '\r' + userTooltip;
      textExpression.Text = expr;
      Text += " (" + rev + ") '" + name + '\'';

      // Trackball.
      tb = new Trackball(gr.center, gr.diameter);
      tb.Button = trackballButton;
    }

    /// <summary>
    /// [near, far] cached. Indicates whether the values were changed.
    /// </summary>
    private float near =  0.1f;
    private float far  = 20.0f;

    /// <summary>
    /// Cold-start of the application.
    /// </summary>
    private void glControl1_Load (object sender, EventArgs e)
    {
      gr.InitOpenGL(glControl1);
      gr.InitSimulation(textParam.Text, textExpression.Text);
      tb.GLsetupViewport(
        glControl1.Width, glControl1.Height,
        near = gr.near,
        far = gr.far);

      loaded = true;
      Application.Idle += new EventHandler(Application_Idle);
    }

    /// <summary>
    /// Warm-restart of the rendering system (resize canvas).
    /// </summary>
    private void glControl1_Resize (object sender, EventArgs e)
    {
      if (!loaded)
        return;

      tb.GLsetupViewport(
        glControl1.Width, glControl1.Height,
        near = gr.near,
        far = gr.far);
      glControl1.Invalidate();
    }

    delegate void SetTextCallback (string text);

    public void SetStatus (string text)
    {
      if (labelStatus.InvokeRequired)
      {
        SetTextCallback st = new SetTextCallback(SetStatus);
        BeginInvoke(st, new object[]{text});
      }
      else
        labelStatus.Text = text;
    }

    private void glControl1_Paint (object sender, PaintEventArgs e)
    {
      Render();
    }

    private void control_PreviewKeyDown (object sender, PreviewKeyDownEventArgs e)
    {
      if (e.KeyCode == Keys.Up ||
          e.KeyCode == Keys.Down ||
          e.KeyCode == Keys.Left ||
          e.KeyCode == Keys.Right)
        e.IsInputKey = true;
    }

    /// <summary>
    /// Unproject support
    /// </summary>
    Vector3 screenToWorld (int x, int y, float z = 0.0f)
    {
      Matrix4 modelViewMatrix, projectionMatrix;
      GL.GetFloat(GetPName.ModelviewMatrix,  out modelViewMatrix);
      GL.GetFloat(GetPName.ProjectionMatrix, out projectionMatrix);

      return Geometry.UnProject(ref projectionMatrix, ref modelViewMatrix, glControl1.Width, glControl1.Height, x, glControl1.Height - y, z);
    }

    private void glControl1_MouseDown (object sender, MouseEventArgs e)
    {
      if (!tb.MouseDown(e) &&
          (gr == null || !gr.MouseButtonDown(e)))
      {
        // Pointing to the scene.
        pointOrigin = screenToWorld(e.X, e.Y, 0.0f);
        pointTarget = screenToWorld(e.X, e.Y, 1.0f);
        eye = tb.Eye;
        pointDirty = true;
      }
    }

    private void glControl1_MouseUp (object sender, MouseEventArgs e)
    {
      if (!tb.MouseUp(e) &&
          gr != null)
        gr.MouseButtonUp(e);
    }

    private void glControl1_MouseMove (object sender, MouseEventArgs e)
    {
      if (!tb.MouseMove(e) &&
          gr != null)
        gr.MousePointerMove(e);
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
          (gr == null || !gr.KeyHandle(e)))
        if (e.KeyCode == Keys.F)
        {
          e.Handled = true;
          if (frustumFrame.Count > 0)
            frustumFrame.Clear();
          else
          {
            float N = 0.0f;
            float F = 1.0f;
            int R = glControl1.Width  - 1;
            int B = glControl1.Height - 1;
            frustumFrame.Add(screenToWorld(0, 0, N));
            frustumFrame.Add(screenToWorld(R, 0, N));
            frustumFrame.Add(screenToWorld(0, B, N));
            frustumFrame.Add(screenToWorld(R, B, N));
            frustumFrame.Add(screenToWorld(0, 0, F));
            frustumFrame.Add(screenToWorld(R, 0, F));
            frustumFrame.Add(screenToWorld(0, B, F));
            frustumFrame.Add(screenToWorld(R, B, F));
          }
        }
        else
        if (e.KeyCode == Keys.R)
        {
          e.Handled = true;
          tb.Reset();
        }
    }

    private void checkVsync_CheckedChanged (object sender, EventArgs e)
    {
      glControl1.VSync = checkVsync.Checked;
    }

    private void Regenerate ()
    {
      if (gr == null)
        return;

      drawGraph = true;
      Cursor.Current = Cursors.WaitCursor;
      string expr = textExpression.Text;
      string param = textParam.Text;

      // Re-generate the graph..
      string err = gr.RegenerateGraph(param, expr);
      Cursor.Current = Cursors.Default;

      if (!string.IsNullOrEmpty(err))
      {
        SetStatus("Err: " + err);
        return;
      }

      // Update model dimensions (Trackball):
      tb.Center   = gr.center;
      tb.Diameter = gr.diameter;
      if (gr.near != near ||
          gr.far  != far)
      {
        // Change viewport only when you need to...
        tb.GLsetupViewport(
          glControl1.Width, glControl1.Height,
          near = gr.near,
          far = gr.far);
      }

      glControl1.Invalidate();
    }

    private void textParam_KeyPress (object sender, System.Windows.Forms.KeyPressEventArgs e)
    {
      if (e.KeyChar == (char)Keys.Enter)
      {
        e.Handled = true;
        Regenerate();
      }
    }

    private void textExpression_KeyPress (object sender, System.Windows.Forms.KeyPressEventArgs e)
    {
      if (e.KeyChar == (char)Keys.Enter)
      {
        e.Handled = true;
        Regenerate();
      }
    }

    private void textParam_MouseHover (object sender, EventArgs e)
    {
      tt.Show(tooltip, (IWin32Window)sender,
        10, -24 - 15 * Util.EolnsInString(tooltip), 4000);
    }

    private void textExpression_MouseHover (object sender, EventArgs e)
    {
      tt.Show("Enter expression f(x,y) or f(x,z)", (IWin32Window)sender, 10, -25, 4000);
    }

    private void labelStatus_MouseHover (object sender, EventArgs e)
    {
      tt.Show(labelStatus.Text, (IWin32Window)sender, 10, -25, 4000);
    }

    private void labelFps_MouseHover (object sender, EventArgs e)
    {
      tt.Show(Util.TargetFramework + " (" + Util.RunningFramework + "), OpenTK " + Util.AssemblyVersion(typeof(Vector3)),
              (IWin32Window)sender, 10, -25, 4000);
    }

    private void buttonRegenerate_Click (object sender, EventArgs e)
    {
      Regenerate();
    }

    private void buttonReset_Click (object sender, EventArgs e)
    {
      tb.Center   = gr?.center ?? Vector3.Zero;
      tb.Diameter = gr?.diameter ?? 5.0f;
      tb.Reset();
    }

    private void Form1_FormClosing (object sender, FormClosingEventArgs e)
    {
      if (gr != null)
        gr.Destroy();
    }
  }
}
