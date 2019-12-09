using System;
using System.Collections.Generic;
using MathSupport;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Utilities;

namespace _086shader
{
  public class AnimatedCamera : DefaultDynamicCamera
  {
    /// <summary>
    /// Optional form-data initialization.
    /// </summary>
    /// <param name="name">Return your full name.</param>
    /// <param name="param">Optional text to initialize the form's text-field.</param>
    /// <param name="tooltip">Optional tooltip = param help.</param>
    public static void InitParams (out string name, out string param, out string tooltip)
    {
      // {{

      name    = "Josef Pelikán";
      param   = "period=10.0, rad=2.0";
      tooltip = "period=<cam period in seconds>, rad=<cam radius in scene diameters>";

      // }}
    }

    /// <summary>
    /// Called after form's Param field is changed.
    /// </summary>
    /// <param name="param">String parameters from the form.</param>
    /// <param name="cameraFile">Optional file-name of your custom camera definition (camera script?).</param>
    public override void Update (string param, string cameraFile)
    {
      // {{ Put your parameter-parsing code here

      Dictionary<string, string> p = Util.ParseKeyValueList(param);
      if (p.Count > 0)
      {
        // period=<double>
        double v = 1.0;
        if (Util.TryParse(p, "period", ref v))
          MaxTime = Math.Max(v, 0.1);

        // rad=<double>
        if (Util.TryParse(p, "rad", ref v))
          radius = (float)Math.Max(v, 0.01);

        // ... you can add more parameters here ...
      }

      Time = Util.Clamp(Time, MinTime, MaxTime);

      // Put your camera-definition-file parsing here.

      // }}
    }

    /// <summary>
    /// Radius of camera trajectory.
    /// </summary>
    float radius = 1.0f;

    /// <param name="param">String parameters from the form.</param>
    /// <param name="cameraFile">Optional file-name of your custom camera definition (camera script?).</param>
    public AnimatedCamera (string param, string cameraFile = "")
    {
      // {{ Put your camera initialization code here

      Update(param, cameraFile);

      // }}
    }

    Matrix4 perspectiveProjection;

    /// <summary>
    /// Returns Projection matrix. Must be implemented.
    /// </summary>
    public override Matrix4 Projection => perspectiveProjection;

    /// <summary>
    /// Called every time a viewport is changed.
    /// It is possible to ignore some arguments in case of scripted camera.
    /// </summary>
    public override void GLsetupViewport (int width, int height, float near = 0.01f, float far = 1000.0f)
    {
      // 1. set ViewPort transform:
      GL.Viewport(0, 0, width, height);

      // 2. set projection matrix
      perspectiveProjection = Matrix4.CreatePerspectiveFieldOfView(Fov, width / (float)height, near, far);
      GLsetProjection();
    }

    /// <summary>
    /// I'm using internal ModelView matrix computation.
    /// </summary>
    Matrix4 computeModelView ()
    {
      double t = (Time - MinTime) * 2 * Math.PI / (MaxTime - MinTime);
      double r = radius * 0.5 * Diameter;
      Vector3 eye = Center + new Vector3((float)(Math.Sin(t) * r),
                                         (float)(Math.Sin(t + 1.0) * r * 0.2),
                                         (float)(Math.Cos(t) * r) );
      return Matrix4.LookAt(eye, Center, Vector3.UnitY);
    }

    /// <summary>
    /// Crucial property = is called in every frame.
    /// </summary>
    public override Matrix4 ModelView => computeModelView();

    /// <summary>
    /// Crucial property = is called in every frame.
    /// </summary>
    public override Matrix4 ModelViewInv => computeModelView().Inverted();
  }
}
