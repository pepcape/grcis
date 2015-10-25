// Author: Josef Pelikan

using System;
using System.Drawing;
using System.Threading;
using MathSupport;

namespace _075animation
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
    public static void InitAnimation ( int width, int height )
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

      // 1st quadrant - anti-aliased filled squares..
      c.SetAntiAlias( true );
      const int MAX_FILLED = 32;
      for ( i = 0, t = 0.0; i < MAX_FILLED; i++, t += 0.65 )
      {
        float r = 2.0f + i * (minq - 5.0f) / MAX_FILLED;
        c.SetColor( Color.FromArgb( (i * 255) / MAX_FILLED, 240, 255 - (i * 255) / MAX_FILLED ) );
        c.FillRectangle( dx + (float)(wq + r * Math.Sin( t )),
                         (float)(hq + r * Math.Cos( t )),
                         (float)(r * 0.3),
                         (float)(r * 0.3),
                         (float)Arith.RadianToDegree( t ) );
      }

      // 2nd quadrant - star-shaped polygon
      const int MAX_CORNERS = 32;
      float[] v = new float[ MAX_CORNERS * 4 ];
      double dt = Math.PI / MAX_CORNERS;
      double r1 = minq * 0.9;
      double r2 = minq * 0.4;
      for ( i = 0, t = 0.0; i < MAX_CORNERS; i++ )
      {
        v[ 4 * i     ] = dx + (float)(wq * 3 + r1 * Math.Sin( t ));
        v[ 4 * i + 1 ] =      (float)(hq     + r1 * Math.Cos( t ));
        t += dt;
        v[ 4 * i + 2 ] = dx + (float)(wq * 3 + r2 * Math.Sin( t ));
        v[ 4 * i + 3 ] =      (float)(hq     + r2 * Math.Cos( t ));
        t += dt;
      }
      c.SetColor( Color.LightSalmon );
      c.FillPolygon( v );

      // 3rd quadrant - jaggy filled squares..
      c.SetAntiAlias( false );
      const int MAX_MATRIX = 12;
      for ( j = 0; j < MAX_MATRIX; j++ )
        for ( i = 0; i < MAX_MATRIX; i++ )
        {
          c.SetColor( ((i ^ j) & 1) == 0 ? Color.White : Color.Blue );
          c.FillRectangle( dx + (float)(wq + (i - MAX_MATRIX / 2) * (wq * 1.8f / MAX_MATRIX)),
                           (float)(3 * hq + (j - MAX_MATRIX / 2) * (hq * 1.7f / MAX_MATRIX)),
                           (((i ^ j) & 15) + 1.0f) / MAX_MATRIX * minq * 0.08f,
                           (((i ^ j) & 15) + 1.0f) / MAX_MATRIX * minq * 0.08f,
                           (i + j) * 360.0f / MAX_MATRIX );
        }

      // !!!}}
    }
  }
}
