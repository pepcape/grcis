using System;
using System.Collections.Generic;
using MathSupport;
using OpenTK;
using Utilities;

namespace Scene3D
{
  public class Construction
  {
    #region Form initialization

    /// <summary>
    /// Optional form-data initialization.
    /// </summary>
    /// <param name="name">Return your full name.</param>
    /// <param name="param">Optional text to initialize the form's text-field.</param>
    /// <param name="tooltip">Optional tooltip = param help.</param>
    public static void InitParams (out string name, out string param, out string tooltip)
    {
      name    = "Josef Pelikán";
      param   = "r=1,seg=1000,kx=1.0,dx=1/2pi,ky=3/2,dy=1/2pi,kz=5/3,dz=pi";
      tooltip = "r=<radius>, seg=<segments>, kx,dx,ky,dy,kz,dz .. frequencies and phase shifts,\ne.g. coordinate x = r*cos(kx*t+dx)";
    }

    #endregion

    #region Instance data

    // !!! If you need any instance data, put them here..

    private float radius = 1.0f;
    private double kx = 1.0;
    private double dx = 0.0;
    private double ky = 1.0;
    private double dy = 0.0;
    private double kz = 1.0;
    private double dz = 0.0;
    private int segments = 1000;
    private double maxT = 2.0 * Math.PI;

    private void parseParams (string param)
    {
      // Defaults.
      radius   = 1.0f;
      kx       = 1.0;
      dx       = 0.0;
      ky       = 1.0;
      dy       = 0.0;
      kz       = 1.0;
      dz       = 0.0;
      segments = 1000;

      Dictionary<string, string> p = Util.ParseKeyValueList(param);
      if (p.Count > 0)
      {
        // r=<double>
        Util.TryParse(p, "r", ref radius);

        // seg=<int>
        if (Util.TryParse(p, "seg", ref segments) &&
            segments < 10)
          segments = 10;

        // kx,dx,ky,dy,kz,dz .. frequencies and phase shifts.
        Util.TryParseRational(p, "kx", ref kx);
        Util.TryParseRational(p, "dx", ref dx);
        Util.TryParseRational(p, "ky", ref ky);
        Util.TryParseRational(p, "dy", ref dy);
        Util.TryParseRational(p, "kz", ref kz);
        Util.TryParseRational(p, "dz", ref dz);

        // ... you can add more parameters here ...
      }

      // Estimate of upper bound for 't'.
      maxT = 2.0 * Math.PI / Arith.GCD(Arith.GCD(kx, ky), kz);
    }

    #endregion

    public Construction ()
    {
      // {{

      // }}
    }

    #region Mesh construction

    /// <summary>
    /// Construct a new Brep solid (preferebaly closed = regular one).
    /// </summary>
    /// <param name="scene">B-rep scene to be modified</param>
    /// <param name="m">Transform matrix (object-space to world-space)</param>
    /// <param name="param">Shape parameters if needed</param>
    /// <returns>Number of generated faces (0 in case of failure)</returns>
    public int AddMesh (SceneBrep scene, Matrix4 m, string param)
    {
      // {{ TODO: put your Mesh-construction code here

      parseParams(param);

      // If there will be large number of new vertices, reserve space for them to save time.
      scene.Reserve(segments + 1);

      double t = 0.0;
      double dt = maxT / segments;
      double s = 0.0;       // for both texture coordinate & color ramp
      double ds = 1.0 / segments;

      int vPrev = 0;
      Vector3 A;
      for (int i = 0; i <= segments; i++)
      {
        // New vertex's coordinates.
        A.X = (float)(radius * Math.Cos(kx * t + dx));
        A.Y = (float)(radius * Math.Cos(ky * t + dy));
        A.Z = (float)(radius * Math.Cos(kz * t + dz));

        // New vertex.
        int v = scene.AddVertex(Vector3.TransformPosition(A, m));

        // Vertex attributes.
        scene.SetTxtCoord(v, new Vector2((float)s, (float)s));
        System.Drawing.Color c = Raster.Draw.ColorRamp(0.5 *(s + 1.0));
        scene.SetColor(v, new Vector3(c.R / 255.0f, c.G / 255.0f, c.B / 255.0f));

        // New line?
        if (i > 0)
          scene.AddLine(vPrev, v);

        // Next vertex.
        t += dt;
        s += ds;
        vPrev = v;
      }

      // Thick line (for rendering).
      scene.LineWidth = 3.0f;

      return segments;

      // }}
    }

    #endregion
  }
}
