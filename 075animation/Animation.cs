// Author: Josef Pelikan

using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using MathSupport;

namespace _075animation
{
  public partial class Form1 : Form
  {
    /// <summary>
    /// Initialize animation parameters.
    /// </summary>
    private void InitializeParams ()
    {
      // single frame:
      ImageWidth  = 640;
      ImageHeight = 480;

      // animation:
      numFrom.Value = (decimal)0.0;
      numTo.Value   = (decimal)10.0;
      numFps.Value  = (decimal)25.0;
    }
  }

  public class Animation
  {
    public static void DrawFrame ( Canvas c, double time )
    {
      // !!!{{ TODO: put your drawing code here

      int wq = c.Width  / 4;
      int hq = c.Height / 4;
      int minq = Math.Min( wq, hq );
      double t;
      int i, j;

      float dx = Math.Min( (float)(wq * time), 3.0f * wq );

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

      // 3rd quadrant - jaggy filled squares..
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
