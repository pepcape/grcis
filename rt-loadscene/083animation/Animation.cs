// Author: Josef Pelikan

using System;
using System.Drawing;
using MathSupport;
using CircleCanvas;

namespace _083animation
{
  public class Animation
  {
    /// <summary>
    /// Initialize form parameters.
    /// </summary>
    public static void InitParams ( out int wid, out int hei, out double from, out double to, out double fps )
    {
      // single frame:
      wid = 640;
      hei = 480;

      // animation:
      from =  0.0;
      to   = 10.0;
      fps  = 25.0;
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
    public static void InitAnimation ( int width, int height, double start, double end, double fps )
    {
      // !!!{{ TODO: put your init code here

      // !!!}}
    }

    /// <summary>
    /// Draw single animation frame.
    /// </summary>
    /// <param name="c">Canvas to draw to.</param>
    /// <param name="time">Current time in seconds.</param>
    /// <param name="start">Start time (t0)</param>
    /// <param name="end">End time (for animation length normalization).</param>
    public static void DrawFrame ( Canvas c, double time, double start, double end )
    {
      // !!!{{ TODO: put your drawing code here

      int wq = c.Width  / 4;
      int hq = c.Height / 4;
      int minq = Math.Min( wq, hq );
      double t;
      int i, j;

      float dx = wq * 4.0f * Arith.Clamp( (float)( (time - start)/(end - start) ), 0.0f, 1.0f );

      c.Clear( Color.Black );
      c.SetPenWidth( 1.0f );

      // 1st quadrant - anti-aliased disks..
      c.SetAntiAlias( true );
      const int MAX_DISK = 30;
      for ( i = 0, t = 0.0; i < MAX_DISK; i++, t += 0.65 )
      {
        float r = 5.0f + i * (minq * 0.7f - 5.0f) / MAX_DISK;
        c.SetColor( Color.FromArgb( (i * 255) / MAX_DISK, 255, 255 - (i * 255) / MAX_DISK ) );
        c.FillDisk( dx + (float)(wq + r * Math.Sin( t )), (float)(hq + r * Math.Cos( t )), (float)(r * 0.3) );
      }

      // 2nd quadrant - anti-aliased circles..
      const int MAX_CIRCLES = 60;
      c.SetColor( Color.LemonChiffon );
      for ( i = 0; i < MAX_CIRCLES; i++ )
      {
        t = i / (double)MAX_CIRCLES;
        c.DrawCircle( dx + (float)(c.Width - t * (wq + wq)), (float)(t * (hq + hq)), (float)(t * 0.9 * minq) );
      }

      // 4th quadrant - jaggy circles..
      c.SetColor( Color.LightCoral );
      c.SetAntiAlias( false );
      for ( i = 0; i < MAX_CIRCLES; i++ )
      {
        t = i / (double)MAX_CIRCLES;
        c.DrawCircle( dx + (float)(c.Width - t * (wq + wq)), (float)(c.Height - t * (hq + hq)), (float)(t * 0.9 * minq) );
      }

      // 3rd quadrant - jaggy disks..
      const int DISKS = 12;
      for ( j = 0; j < DISKS; j++ )
        for ( i = 0; i < DISKS; i++ )
        {
          c.SetColor( ((i ^ j) & 1) == 0 ? Color.White : Color.Blue );
          c.FillDisk( dx + (float)(wq + (i - DISKS / 2) * (wq * 1.8f / DISKS)),
                      (float)(3 * hq + (j - DISKS / 2) * (hq * 1.7f / DISKS)),
                      (((i ^ j) & 15) + 1.0f) / DISKS * minq * 0.08f );
        }

      // !!!}}
    }
  }
}
