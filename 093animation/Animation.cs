using System;
using System.Collections.Generic;
using System.Drawing;
using LineCanvas;
using MathSupport;
using Utilities;

namespace _093animation
{
  public class Animation
  {
    /// <summary>
    /// Form data initialization.
    /// </summary>
    /// <param name="name">Your first-name and last-name.</param>
    /// <param name="wid">Image width in pixels.</param>
    /// <param name="hei">Image height in pixels.</param>
    /// <param name="from">Animation start in seconds.</param>
    /// <param name="to">Animation end in seconds.</param>
    /// <param name="fps">Frames-per-seconds.</param>
    /// <param name="param">Optional text to initialize the form's text-field.</param>
    /// <param name="tooltip">Optional tooltip = param help.</param>
    public static void InitParams (out string name, out int wid, out int hei, out double from, out double to, out double fps, out string param, out string tooltip)
    {
      // {{

      // Put your name here.
      name = "Josef Pelikán";

      // Image size in pixels.
      wid = 640;
      hei = 480;

      // Animation.
      from = 0.0;
      to   = 10.0;
      fps  = 25.0;

      // Specific animation params.
      param = "width=1.0,anti=true,objects=100,hatches=12,prob=0.95";

      // Tooltip = help.
      tooltip = "width=<double>, anti[=<bool>], objects=<int>, hatches=<int>, prob=<double>";

      // }}
    }

    /// <summary>
    /// Global initialization. Called before each animation batch
    /// or single-frame computation.
    /// </summary>
    /// <param name="width">Width of the future canvas in pixels.</param>
    /// <param name="height">Height of the future canvas in pixels.</param>
    /// <param name="start">Start time (t0)</param>
    /// <param name="end">End time (for animation length normalization).</param>
    /// <param name="fps">Required fps.</param>
    /// <param name="param">Optional string parameter from the form.</param>
    public static void InitAnimation (int width, int height, double start, double end, double fps, string param)
    {
      // {{ TODO: put your init code here

      // }}
    }

    /// <summary>
    /// Draw single animation frame.
    /// Has to be re-entrant!
    /// </summary>
    /// <param name="c">Canvas to draw to.</param>
    /// <param name="time">Current time in seconds.</param>
    /// <param name="start">Start time (t0)</param>
    /// <param name="end">End time (for animation length normalization).</param>
    /// <param name="param">Optional string parameter from the form.</param>
    public static void DrawFrame (Canvas c, double time, double start, double end, string param)
    {
      // {{ TODO: put your drawing code here

      double timeNorm = Arith.Clamp((time - start) / (end - start), 0.0, 1.0);

      // input params:
      float penWidth = 1.0f;   // pen width
      bool antialias = false;  // use anti-aliasing?
      int objects    = 100;    // number of randomly generated objects (squares, stars, Brownian particles)
      int hatches    = 12;     // number of hatch-lines for the squares
      double prob    = 0.95;   // continue-probability for the Brownian motion simulator

      Dictionary<string, string> p = Util.ParseKeyValueList(param);
      if (p.Count > 0)
      {
        // with=<line-width>
        if (Util.TryParse(p, "width", ref penWidth))
        {
          if (penWidth < 0.0f)
            penWidth = 0.0f;
        }

        // anti[=<bool>]
        Util.TryParse(p, "anti", ref antialias);

        // squares=<number>
        if (Util.TryParse(p, "objects", ref objects) &&
            objects < 0)
          objects = 0;

        // hatches=<number>
        if (Util.TryParse(p, "hatches", ref hatches) &&
            hatches < 1)
          hatches = 1;

        // prob=<probability>
        if (Util.TryParse(p, "prob", ref prob) &&
            prob > 0.999)
          prob = 0.999;
      }

      int wq = c.Width / 4;
      int hq = c.Height / 4;
      int wh = wq + wq;
      int hh = hq + hq;
      int minh = Math.Min(wh, hh);
      double t;
      int i, j;
      double cx, cy, angle, x, y;

      c.Clear(Color.Black);

      // 1st quadrant - star.
      c.SetPenWidth(penWidth);
      c.SetAntiAlias(antialias);

      const int MAX_LINES = 30;
      for (i = 0, t = 0.0; i < MAX_LINES; i++, t += 1.0 / MAX_LINES)
      {
        c.SetColor(Color.FromArgb(i * 255 / MAX_LINES, 255, 255 - i * 255 / MAX_LINES)); // [0,255,255] -> [255,255,0]
        c.Line(t * wh, 0, wh - t * wh, hh);
      }
      for (i = 0, t = 0.0; i < MAX_LINES; i++, t += 1.0 / MAX_LINES)
      {
        c.SetColor(Color.FromArgb(255, 255 - i * 255 / MAX_LINES, i * 255 / MAX_LINES)); // [255,255,0] -> [255,0,255]
        c.Line(0, hh - t * hh, wh, t * hh);
      }

      // 2nd quadrant - random hatched squares.
      double size = minh / 10.0;
      double padding = size * Math.Sqrt(0.5);
      c.SetColor(Color.LemonChiffon);
      c.SetPenWidth(1.0f);
      Random r = new Random(12);

      for (i = 0; i < objects; i++)
      {
        do
          cx = r.NextDouble() * wh;
        while (cx < padding ||
               cx > wh - padding);

        c.SetAntiAlias(cx > wq);
        cx += wh;

        do
          cy = r.NextDouble() * hh;
        while (cy < padding ||
               cy > hh - padding);

        angle = (r.NextDouble() + timeNorm) * Math.PI;

        double dirx = Math.Sin(angle) * size * 0.5;
        double diry = Math.Cos(angle) * size * 0.5;
        cx -= dirx - diry;
        cy -= diry + dirx;
        double dx = -diry * 2.0 / hatches;
        double dy = dirx * 2.0 / hatches;
        double linx = dirx + dirx;
        double liny = diry + diry;

        for (j = 0; j++ < hatches; cx += dx, cy += dy)
          c.Line(cx, cy, cx + linx, cy + liny);
      }

      // 3rd quadrant - random stars.
      c.SetColor(Color.LightCoral);
      c.SetPenWidth(penWidth);
      size = minh / 16.0;
      padding = size;
      const int MAX_SIDES = 30;
      List<PointF> v = new List<PointF>(MAX_SIDES + 1);

      for (i = 0; i < objects; i++)
      {
        do
          cx = r.NextDouble() * wh;
        while (cx < padding ||
               cx > wh - padding);

        c.SetAntiAlias(cx > wq);

        do
          cy = r.NextDouble() * hh;
        while (cy < padding ||
               cy > hh - padding);

        cy += hh;

        int sides = r.Next(3, MAX_SIDES);
        double dAngle = Math.PI * 2.0 / sides;

        v.Clear();
        angle = 0.0;

        for (j = 0; j++ < sides; angle += dAngle)
        {
          double rad = size * (0.1 + 0.9 * r.NextDouble());
          x = cx + rad * Math.Sin(angle);
          y = cy + rad * Math.Cos(angle);
          v.Add(new PointF((float)x, (float)y));
        }
        v.Add(v[0]);
        c.PolyLine(v);
      }

      // 4th quadrant - Brownian motion.
      c.SetPenWidth(penWidth);
      c.SetAntiAlias(true);
      size = minh / 10.0;
      padding = size;

      for (i = 0; i < objects; i++)
      {
        do
          x = r.NextDouble() * wh;
        while (x < padding ||
               x > wh - padding);

        do
          y = r.NextDouble() * hh;
        while (y < padding ||
               y > hh - padding);

        c.SetColor(Color.FromArgb(127 + r.Next(0, 128),
                                  127 + r.Next(0, 128),
                                  127 + r.Next(0, 128)));

        for (j = 0; j++ < 1000;)
        {
          angle = r.NextDouble() * Math.PI * 2.0;
          double rad = size * r.NextDouble();
          cx = x + rad * Math.Sin(angle);
          cy = y + rad * Math.Cos(angle);
          if (cx < 0.0 || cx > wh ||
              cy < 0.0 || cy > hh)
            break;

          if (j < 70.0 * timeNorm)
            c.Line(x + wh, y + hh, cx + wh, cy + hh);

          x = cx;
          y = cy;
          if (r.NextDouble() > prob)
            break;
        }
      }

      // }}
    }
  }
}
