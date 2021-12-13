#define USE_INVALIDATE

using System;
using System.Collections.Generic;
using System.Globalization;
using MathSupport;
using OpenglSupport;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Utilities;

namespace _113graph
{
  /// <summary>
  /// Container for appearance/material attributes.
  /// </summary>
  public class Appearance
  {
    public Vector3 globalAmbient = new Vector3(  0.2f,  0.2f,  0.2f);
    public Vector3 matAmbient    = new Vector3(  0.8f,  0.6f,  0.2f);
    public Vector3 matDiffuse    = new Vector3(  0.8f,  0.6f,  0.2f);
    public Vector3 matSpecular   = new Vector3(  0.8f,  0.8f,  0.8f);
    public float   matShininess  = 100.0f;
    public Vector3 whiteLight    = new Vector3(  1.0f,  1.0f,  1.0f);
    public Vector3 lightPosition = new Vector3(-20.0f, 10.0f, 10.0f);

    /// <summary>
    /// Light source direction.
    /// </summary>
    public Vector3 light         = new Vector3(-2, 1, 1);

    public bool    useGlobalColor = false;

    /// <summary>
    /// Set light-source coordinate in the world-space.
    /// </summary>
    /// <param name="size">Relative size (based on the scene size).</param>
    /// <param name="light">Relative light position (default=[-2,1,1],viewer=[0,0,1]).</param>
    public void SetLight (float size, ref Vector3 light)
    {
      lightPosition = 2.0f * size * light;
    }
  }

  /// <summary>
  /// Rendering part of the form, see Form1.cs for system stuff.
  /// </summary>
  public partial class Form1
  {
    /// <summary>
    /// Point in the 3D scene pointed out by an user, or null.
    /// </summary>
    Vector3? spot = null;

    /// <summary>
    /// Is 'pointOrigi'n non-processed (new)?
    /// </summary>
    bool pointDirty = false;

    /// <summary>
    /// Clicked point on the screen (z = 0.0)
    /// </summary>
    Vector3? pointOrigin = null;

    /// <summary>
    /// Target point on the screen (z = 1.0).
    /// </summary>
    Vector3 pointTarget;

    /// <summary>
    /// Eye position at the time of user click.
    /// </summary>
    Vector3 eye;

    /// <summary>
    /// Frustum vertices, 0 or 8 vertices
    /// </summary>
    List<Vector3> frustumFrame = new List<Vector3>();

    long lastFpsTime = 0L;
    int frameCounter = 0;
    long primitiveCounter = 0L;
    double lastFps = 0.0;
    double lastPps = 0.0;

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
        gr.RenderScene(tb, appearance, ref primitiveCounter);
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
      GL.Vertex3(gr.center);
      GL.Vertex3(gr.center + new Vector3(0.5f, 0.0f, 0.0f) * gr.diameter);

      GL.Color3(0.0f, 1.0f, 0.0f);
      GL.Vertex3(gr.center);
      GL.Vertex3(gr.center + new Vector3(0.0f, 0.5f, 0.0f) * gr.diameter);

      GL.Color3(0.2f, 0.2f, 1.0f);
      GL.Vertex3(gr.center);
      GL.Vertex3(gr.center + new Vector3(0.0f, 0.0f, 0.5f) * gr.diameter);

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

    /// <summary>
    /// Global appearance params object.
    /// </summary>
    Appearance appearance = new Appearance();

    /// <summary>
    /// Update rendering/trackball parameters.
    /// </summary>
    /// <param name="param">User-provided parameter string.</param>
    public void UpdateRenderingParams (Dictionary<string, string> p)
    {
      // Input params.
      if (p.Count == 0)
        return;

      // Trackball: zoom limits.
      float minZoom = tb.MinZoom;
      float maxZoom = tb.MaxZoom;
      if (Util.TryParse(p, "minZoom", ref minZoom))
        tb.MinZoom = Math.Max(1.0e-4f, maxZoom);

      if (Util.TryParse(p, "maxZoom", ref maxZoom))
        tb.MaxZoom = Arith.Clamp(maxZoom, minZoom, 1.0e6f);

      // Trackball: zoom.
      float zoom = tb.Zoom;
      if (Util.TryParse(p, "zoom", ref zoom))
        tb.Zoom = Arith.Clamp(zoom, tb.MinZoom, tb.MaxZoom);

      // Rendering: perspective/orthographic projection.
      bool perspective = tb.UsePerspective;
      if (Util.TryParse(p, "perspective", ref perspective))
        tb.UsePerspective = perspective;

      // Rendering: vertical field-of-view.
      float fov = tb.Fov;
      if (!Util.TryParse(p, "fov", ref fov))
      {
        fov = (float)Arith.DegreeToRadian(Arith.Clamp(fov, 0.001f, 170.0f));
        if (fov != tb.Fov)
        {
          tb.Fov = fov;
          tb.GLsetupViewport(
            glControl1.Width, glControl1.Height,
            gr.near,
            gr.far);
        }
      }

      // Rendering: background color.
      Vector3 col = Vector3.Zero;
      if (Geometry.TryParse(p, "backgr", ref col))
        GL.ClearColor(col.X, col.Y, col.Z, 1.0f);

      // Rendering: line width.
      fov = 1.0f;
      if (Util.TryParse(p, "line", ref fov))
        GL.LineWidth(Math.Max(0.0f, fov));

      // Shading: relative light position.
      if (Geometry.TryParse(p, "light", ref appearance.light))
      {
        if (appearance.light.Length < 1.0e-3f)
          appearance.light = new Vector3(-2, 1, 1);
        appearance.SetLight(tb.Eye.Z * 0.5f, ref appearance.light);
      }

      // Shading: global material color.
      if (Geometry.TryParse(p, "color", ref appearance.matAmbient))
        appearance.matDiffuse = appearance.matAmbient;

      // Shading: global ambient coeff.
      fov = 0.2f;
      if (Util.TryParse(p, "Ka", ref fov))
        appearance.globalAmbient.X =
        appearance.globalAmbient.Y =
        appearance.globalAmbient.Z = Util.Clamp(fov, 0.0f, 1.0f);

      // Shading: shininess.
      if (!Util.TryParse(p, "shininess", ref appearance.matShininess))
        appearance.matShininess = Arith.Clamp(appearance.matShininess, 1.0f, 1.0e4f);
    }
  }
}
