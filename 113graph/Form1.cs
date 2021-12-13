#define USE_INVALIDATE

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using MathSupport;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Utilities;

namespace _113graph
{
  public partial class Form1 : Form
  {
    static readonly string rev = Util.SetVersion("$Rev: 978 $");

    public static Form1 form = null;

    /// <summary>    /// <summary>
    /// Scene center point.
    /// </summary>
    Vector3 center = Vector3.Zero;

    /// <summary>
    /// Scene diameter.
    /// </summary>
    float diameter = 4.0f;

    float near =  0.1f;

    float far  = 20.0f;

    Vector3 light = new Vector3(-2, 1, 1);

    /// <summary>
    /// Point in the 3D scene pointed out by an user, or null.
    /// </summary>
    Vector3? spot = null;

    Vector3? pointOrigin = null;
    Vector3 pointTarget;
    Vector3 eye;

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
    /// False until the 'Regenerate' button is pressed for the first time..
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
    Graph gr = null;

    long lastFpsTime = 0L;
    int frameCounter = 0;
    long primitiveCounter = 0L;
    double lastFps = 0.0;
    double lastPps = 0.0;

    public Form1 ()
    {
      InitializeComponent();
      form = this;

      string param;
      string name;
      string expr;
      MouseButtons trackballButton;
      Graph.InitParams(out param, out tooltip, out expr, out name, out trackballButton);
      textParam.Text = param ?? "";
      textExpression.Text = expr;
      Text += " (" + rev + ") '" + name + '\'';

      // Trackball.
      tb = new Trackball(center, diameter);
      tb.Button = trackballButton;

      // Graph object.
      gr = new Graph();
    }

    private void glControl1_Load (object sender, EventArgs e)
    {
      gr.InitOpenGL(glControl1);
      gr.InitSimulation(textParam.Text, textExpression.Text);
      tb.GLsetupViewport(glControl1.Width, glControl1.Height, near, far);

      loaded = true;
      Application.Idle += new EventHandler(Application_Idle);
    }

    private void glControl1_Resize (object sender, EventArgs e)
    {
      if (!loaded)
        return;

      tb.GLsetupViewport(glControl1.Width, glControl1.Height, near, far);
      glControl1.Invalidate();
    }

    /// <summary>
    /// Function called whenever the main application is idle..
    /// It actually contains the redraw-loop.
    /// </summary>
    void Application_Idle (object sender, EventArgs e)
    {
      while (glControl1.IsIdle)
      {
#if USE_INVALIDATE
        glControl1.Invalidate();
#else
        glControl1.MakeCurrent();
        Render();
#endif

        long now = DateTime.Now.Ticks;
        if (now - lastFpsTime > 8000000)      // more than 0.8 sec
        {
          lastFps = 0.5 * lastFps + 0.5 * (frameCounter * 1.0e7 / (now - lastFpsTime));
          lastPps = 0.5 * lastPps + 0.5 * (primitiveCounter * 1.0e7 / (now - lastFpsTime));
          lastFpsTime = now;
          frameCounter = 0;
          primitiveCounter = 0L;

          if (lastPps < 5.0e5)
            labelFps.Text = string.Format(CultureInfo.InvariantCulture, "Fps: {0:f1}, Pps: {1:f0}k",
                                          lastFps, (lastPps * 1.0e-3));
          else
            labelFps.Text = string.Format(CultureInfo.InvariantCulture, "Fps: {0:f1}, Pps: {1:f1}m",
                                          lastFps, (lastPps * 1.0e-6));
        }

        // Pointing.
        if (pointOrigin != null &&
            pointDirty)
        {
          Vector3d p0 = new Vector3d( pointOrigin.Value.X, pointOrigin.Value.Y, pointOrigin.Value.Z );
          Vector3d p1 = new Vector3d( pointTarget.X,       pointTarget.Y,       pointTarget.Z ) - p0;
          double nearest = double.PositiveInfinity;

          if (gr != null)
            gr.Intersect(ref p0, ref p1, ref nearest);

          if (double.IsInfinity(nearest))
            spot = null;
          else
            spot = new Vector3((float)(p0.X + nearest * p1.X),
                               (float)(p0.Y + nearest * p1.Y),
                               (float)(p0.Z + nearest * p1.Z));
          pointDirty = false;
        }
      }
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

    void SetModelGeometry (Vector3 cen, float diam, float n, float f)
    {
    }

    /// <summary>
    /// Render one frame.
    /// </summary>
    private void Render ()
    {
      if (!loaded)
        return;

      frameCounter++;

      GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
      GL.ShadeModel(checkSmooth.Checked ? ShadingModel.Smooth : ShadingModel.Flat);
      GL.PolygonMode(checkTwosided.Checked ? MaterialFace.FrontAndBack : MaterialFace.Front,
                     checkWireframe.Checked ? PolygonMode.Line : PolygonMode.Fill);
      if (checkTwosided.Checked)
        GL.Disable(EnableCap.CullFace);
      else
        GL.Enable(EnableCap.CullFace);

      tb.GLsetCamera();
      if (gr != null)
        gr.RenderScene(ref primitiveCounter);
      Decorate();

      glControl1.SwapBuffers();
    }

    private void Decorate ()
    {
      if (!checkDebug.Checked)
        return;

      // Support: axes
      float origWidth = GL.GetFloat(GetPName.LineWidth);
      float origPoint = GL.GetFloat(GetPName.PointSize);

      // Axes.
      GL.LineWidth(2.0f);
      GL.Begin(PrimitiveType.Lines);

      GL.Color3(1.0f, 0.1f, 0.1f);
      GL.Vertex3(center);
      GL.Vertex3(center + new Vector3(0.5f, 0.0f, 0.0f) * diameter);

      GL.Color3(0.0f, 1.0f, 0.0f);
      GL.Vertex3(center);
      GL.Vertex3(center + new Vector3(0.0f, 0.5f, 0.0f) * diameter);

      GL.Color3(0.2f, 0.2f, 1.0f);
      GL.Vertex3(center);
      GL.Vertex3(center + new Vector3(0.0f, 0.0f, 0.5f) * diameter);

      GL.End();

      // Support: pointing
      if (pointOrigin != null)
      {
        GL.Begin(PrimitiveType.Lines);
        GL.Color3(1.0f, 1.0f, 0.0f);
        GL.Vertex3(pointOrigin.Value);
        GL.Vertex3(pointTarget);
        GL.Color3(1.0f, 0.0f, 0.0f);
        GL.Vertex3(pointOrigin.Value);
        GL.Vertex3(eye);
        GL.End();

        GL.PointSize(4.0f);
        GL.Begin(PrimitiveType.Points);
        GL.Color3(1.0f, 0.0f, 0.0f);
        GL.Vertex3(pointOrigin.Value);
        GL.Color3(0.0f, 1.0f, 0.2f);
        GL.Vertex3(pointTarget);
        GL.Color3(1.0f, 1.0f, 1.0f);
        if (spot != null)
          GL.Vertex3(spot.Value);
        GL.Vertex3(eye);
        GL.End();
      }

      // Support: frustum
      if (frustumFrame.Count >= 8)
      {
        GL.LineWidth(2.0f);
        GL.Begin(PrimitiveType.Lines);

        GL.Color3(1.0f, 0.0f, 0.0f);
        GL.Vertex3(frustumFrame[0]);
        GL.Vertex3(frustumFrame[1]);
        GL.Vertex3(frustumFrame[1]);
        GL.Vertex3(frustumFrame[3]);
        GL.Vertex3(frustumFrame[3]);
        GL.Vertex3(frustumFrame[2]);
        GL.Vertex3(frustumFrame[2]);
        GL.Vertex3(frustumFrame[0]);

        GL.Color3(1.0f, 1.0f, 1.0f);
        GL.Vertex3(frustumFrame[0]);
        GL.Vertex3(frustumFrame[4]);
        GL.Vertex3(frustumFrame[1]);
        GL.Vertex3(frustumFrame[5]);
        GL.Vertex3(frustumFrame[2]);
        GL.Vertex3(frustumFrame[6]);
        GL.Vertex3(frustumFrame[3]);
        GL.Vertex3(frustumFrame[7]);

        GL.Color3(0.0f, 1.0f, 0.0f);
        GL.Vertex3(frustumFrame[4]);
        GL.Vertex3(frustumFrame[5]);
        GL.Vertex3(frustumFrame[5]);
        GL.Vertex3(frustumFrame[7]);
        GL.Vertex3(frustumFrame[7]);
        GL.Vertex3(frustumFrame[6]);
        GL.Vertex3(frustumFrame[6]);
        GL.Vertex3(frustumFrame[4]);

        GL.End();
      }

      GL.LineWidth(origWidth);
      GL.PointSize(origPoint);
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
      GL.GetFloat(GetPName.ModelviewMatrix, out modelViewMatrix);
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
      string err = gr.RegenerateGraph( param, expr );
      Cursor.Current = Cursors.Default;

      if (!string.IsNullOrEmpty(err))
      {
        SetStatus("Err: " + err);
        return;
      }

      // Update model dimensions (Trackball):
      tb.Center = center = gr.center;
      tb.Diameter = diameter = gr.diameter;
      if (gr.near != near ||
          gr.far  != far)
      {
        near = gr.near;
        far = gr.far;
        tb.GLsetupViewport(glControl1.Width, glControl1.Height, near, far);
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
      tt.Show(tooltip, (IWin32Window)sender, 10, -25, 4000);
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

    private void Form1_FormClosing (object sender, FormClosingEventArgs e)
    {
      if (gr != null)
        gr.Destroy();
    }
  }
}
