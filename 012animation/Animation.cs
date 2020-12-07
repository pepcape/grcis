using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using MathSupport;

namespace _012animation
{
  public class Animation
  {
    /// <summary>
    /// Initialize form parameters.
    /// </summary>
    public static void InitParams (out string name, out int wid, out int hei, out double from, out double to, out double fps)
    {
      // Author.
      name = "Josef Pelikán";

      // Single frame.
      wid = 640;
      hei = 480;

      // Animation.
      from = 0.0;
      to   = 10.0;
      fps  = 25.0;
    }

    /// <summary>
    /// Global initialization. Called before each animation batch
    /// or single-frame computation.
    /// </summary>
    /// <param name="width">Width of animation frames in pixels.</param>
    /// <param name="height">Height of animation frames in pixels.</param>
    /// <param name="start">Start time (t0)</param>
    /// <param name="end">End time (for animation length normalization).</param>
    /// <param name="fps">Required fps.</param>
    public static void InitAnimation (int width, int height, double start, double end, double fps)
    {
      // !!!{{ TODO: put your init code here

      // !!!}}
    }

    /// <summary>
    /// Draw single animation frame.
    /// </summary>
    /// <param name="width">Required frame width in pixels.</param>
    /// <param name="height">Required frame height in pixels.</param>
    /// <param name="time">Current time in seconds.</param>
    /// <param name="start">Start time (t0)</param>
    /// <param name="end">End time (for animation length normalization).</param>
    public static Bitmap RenderFrame (int width, int height, double time, double start, double end)
    {
      // !!!{{ TODO: put your frame-rendering code here

      Bitmap result = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format24bppRgb );
      Graphics gr = Graphics.FromImage(result);

      int x0 = width / 2;
      int y0 = height / 2;
      int maxWid = x0 - 2;
      int maxHei = y0 - 2;

      Pen p1 = new Pen(Color.FromArgb(255, 255,  20), 1.0f);
      Pen p2 = new Pen(Color.FromArgb( 60, 120, 255), 2.0f);

      double tim = (time - start) / (end - start);
      int wid = (int)(tim * maxWid);
      int hei = (int)(tim * maxHei);
      gr.DrawEllipse(p2, x0 - hei, y0 - wid, hei + hei, wid + wid);
      gr.DrawEllipse(p1, x0 - wid, y0 - hei, wid + wid, hei + hei);

      gr.Dispose();

      return result;

      // !!!}}
    }
  }
}
