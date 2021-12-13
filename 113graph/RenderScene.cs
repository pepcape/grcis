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
  /// Container for rendering style and appearance/material attributes.
  /// </summary>
  public class RenderingStyle
  {
    /// <summary>
    /// Draw two-sided triangles? (if on, you don't need to worry about face orientation,
    /// but it is a good habit to keep all faces regularly oriented)
    /// </summary>
    public bool twoSided = true;

    /// <summary>
    /// Use wire-frame style? (no fill)
    /// </summary>
    public bool wireframe = false;

    /// <summary>
    /// Use color texture? (we are limited to single color texture in this example)
    /// </summary>
    public bool texture = false;

    /// <summary>
    /// Use lighting? (light source lits the surface at vertices or at every pixel)
    /// </summary>
    public bool lighting = false;

    /// <summary>
    /// Use smooth interpolation from vertices to pixels? (color interpolation or normal interpolation if 'phong' is on)
    /// </summary>
    public bool smooth = false;

    /// <summary>
    /// Use Phong-style interpolation? (relevant only if both 'lighting' and 'smooth' are on)
    /// </summary>
    public bool phong = false;

    /// <summary>
    /// Use ambient component in lighting? (Ka)
    /// </summary>
    public bool useAmbient = true;

    /// <summary>
    /// Use diffuse component in lighting? (Kd)
    /// </summary>
    public bool useDiffuse = true;

    /// <summary>
    /// Use specular component in lighting? (Ks)
    /// </summary>
    public bool useSpecular = true;

    /// <summary>
    /// Use global color instead of vertex-defined color?
    /// </summary>
    public bool useGlobalColor = false;

    /// <summary>
    /// Color of the ambient (Ka) lighting component. [R, G, B]
    /// </summary>
    public Vector3 globalAmbient = new Vector3(  0.2f,  0.2f,  0.2f);

    /// <summary>
    /// Ambient color contribution (Ka * C). [R, G, B]
    /// </summary>
    public Vector3 matAmbient = new Vector3(  0.8f,  0.6f,  0.2f);

    /// <summary>
    /// Diffuse reflection contribution (Kd * C). [R, G, B]
    /// </summary>
    public Vector3 matDiffuse = new Vector3(  0.8f,  0.6f,  0.2f);

    /// <summary>
    /// Specular reflection contribution (Ks * C_l). [R, G, B]
    /// </summary>
    public Vector3 matSpecular = new Vector3(  0.8f,  0.8f,  0.8f);

    /// <summary>
    /// Specular exponent = shineness.
    /// </summary>
    public float   matShininess = 100.0f;

    /// <summary>
    /// Light source color. [R, G, B]
    /// </summary>
    public Vector3 whiteLight = new Vector3(  1.0f,  1.0f,  1.0f);

    /// <summary>
    /// Light source position in the world coordinate space. [x, y, z]
    /// </summary>
    public Vector3 lightPosition = new Vector3(-20.0f, 10.0f, 10.0f);

    /// <summary>
    /// Light source direction in world space. [x, y, z]
    /// </summary>
    public Vector3 lightDirection = new Vector3(-2, 1, 1);

    /// <summary>
    /// Set light-source coordinate in the world-space.
    /// </summary>
    /// <param name="size">Relative size (based on the scene size).</param>
    /// <param name="light">Light source direction (default=[-2,1,1],viewer=[0,0,1]).</param>
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

    /// <summary
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
          lastFps = 0.5 * lastFps + 0.5 * (frameCounter     * 1.0e7 / (now - lastFpsTime));
          lastPps = 0.5 * lastPps + 0.5 * (primitiveCounter * 1.0e7 / (now - lastFpsTime));
          lastFpsTime = now;
          frameCounter = 0;
          primitiveCounter = 0L;

          if (lastPps < 5.0e5)
            labelFps.Text = string.Format(CultureInfo.InvariantCulture, "Fps: {0:f1}, Tps: {1:f0}k",
                                          lastFps, (lastPps * 1.0e-3));
          else
            labelFps.Text = string.Format(CultureInfo.InvariantCulture, "Fps: {0:f1}, Tps: {1:f1}m",
                                          lastFps, (lastPps * 1.0e-6));
        }

        // Pointing.
        if (pointOrigin != null &&
            pointDirty)
        {
          Vector3d p0 = new Vector3d(pointOrigin.Value.X, pointOrigin.Value.Y, pointOrigin.Value.Z);
          Vector3d p1 = new Vector3d(pointTarget.X,       pointTarget.Y,       pointTarget.Z      ) - p0;
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
    /// Update rendering style ('style') from the application form.
    /// </summary>
    private void UpdateStyle ()
    {
      style.twoSided       = checkTwosided.Checked;
      style.wireframe      = checkWireframe.Checked;
      style.texture        = checkTexture.Checked;
      style.useGlobalColor = checkGlobalColor.Checked;
      style.lighting       = checkLighting.Checked;
      style.useAmbient     = checkAmbient.Checked;
      style.useDiffuse     = checkDiffuse.Checked;
      style.useSpecular    = checkSpecular.Checked;
      style.smooth         = checkSmooth.Checked;
      style.phong          = checkPhong.Checked;
    }

    /// <summary>
    /// Render one frame.
    /// </summary>
    private void Render ()
    {
      if (!loaded)
        return;

      frameCounter++;

      // Update rendering style from the form.
      UpdateStyle();

      GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
      //GL.ShadeModel(style.smooth ? ShadingModel.Smooth : ShadingModel.Flat);
      GL.PolygonMode(style.twoSided  ? MaterialFace.FrontAndBack : MaterialFace.Front,
                     style.wireframe ? PolygonMode.Line : PolygonMode.Fill);
      if (style.twoSided)
        GL.Disable(EnableCap.CullFace);
      else
        GL.Enable(EnableCap.CullFace);

      tb.GLsetCamera();

      if (gr != null)
        gr.RenderScene(tb, style, ref primitiveCounter);

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
    /// Global rendering-style object.
    /// </summary>
    RenderingStyle style = new RenderingStyle();

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
      if (Util.TryParse(p, "fov", ref fov))
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

      // Shading: light source direction => position.
      if (Geometry.TryParse(p, "light", ref style.lightDirection))
      {
        if (style.lightDirection.Length < 1.0e-3f)
          style.lightDirection = new Vector3(-2, 1, 1);
        style.SetLight(tb.Eye.Z * 0.5f, ref style.lightDirection);
      }

      // Shading: global material color.
      if (Geometry.TryParse(p, "color", ref style.matAmbient))
        style.matDiffuse = style.matAmbient;

      // Shading: global ambient coeff.
      fov = 0.2f;
      if (Util.TryParse(p, "Ka", ref fov))
        style.globalAmbient.X =
        style.globalAmbient.Y =
        style.globalAmbient.Z = Util.Clamp(fov, 0.0f, 1.0f);

      // Shading: shininess.
      if (!Util.TryParse(p, "shininess", ref style.matShininess))
        style.matShininess = Arith.Clamp(style.matShininess, 1.0f, 1.0e4f);
    }
  }
}
