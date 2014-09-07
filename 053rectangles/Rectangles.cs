using System;
using System.Drawing;
using System.Globalization;
using MathSupport;

namespace _053rectangles
{
  public class Rectangles
  {
    public static void Draw ( Canvas c, Bitmap input, string param )
    {
      // !!!{{ TODO: put your drawing code here

      int wq = c.Width  / 4;
      int hq = c.Height / 4;
      int minq = Math.Min( wq, hq );
      double t;
      int i, j;

      c.Clear( Color.Black );

      // example of passing numerical value through string param:
      double w = 1.0;
      if ( Double.TryParse( param, NumberStyles.Float, CultureInfo.InvariantCulture, out w ) )
        c.SetPenWidth( (float)w );

      // 1st quadrant - anti-aliased filled squares..
      c.SetAntiAlias( true );
      const int MAX_FILLED = 32;
      for ( i = 0, t = 0.0; i < MAX_FILLED; i++, t += 0.65 )
      {
        float r = 2.0f + i * (minq - 5.0f) / MAX_FILLED;
        c.SetColor( Color.FromArgb( (i * 255) / MAX_FILLED, 240, 255 - (i * 255) / MAX_FILLED ) );
        c.FillRectangle( (float)(wq + r * Math.Sin( t )),
                         (float)(hq + r * Math.Cos( t )),
                         (float)(r * 0.3),
                         (float)(r * 0.3),
                         (float)Arith.RadianToDegree( t ) );
      }

      // 2nd quadrant - anti-aliased squares..
      const int MAX_DRAWN = 60;
      c.SetColor( Color.LemonChiffon );
      for ( i = 0; i < MAX_DRAWN; i++ )
      {
        t = i / (double)MAX_DRAWN;
        c.DrawRectangle( (float)(c.Width - t * 0.9 * (wq + wq)),
                         (float)(t * (hq + hq)),
                         (float)(t * minq),
                         (float)(t * minq),
                         i * 360.0f / MAX_DRAWN );
      }

      // 4th quadrant - jaggy squares..
      c.SetColor( Color.LightCoral );
      c.SetAntiAlias( false );
      for ( i = 0; i < MAX_DRAWN; i++ )
      {
        t = i / (double)MAX_DRAWN;
        c.DrawRectangle( (float)(c.Width - t * 0.9 * (wq + wq)),
                         (float)(c.Height - t * (hq + hq)),
                         (float)(t * minq),
                         (float)(t * minq),
                         i * 360.0f / MAX_DRAWN );
      }

      // 3rd quadrant - jaggy filled squares..
      const int MAX_MATRIX = 12;
      for ( j = 0; j < MAX_MATRIX; j++ )
        for ( i = 0; i < MAX_MATRIX; i++ )
        {
          c.SetColor( ((i ^ j) & 1) == 0 ? Color.White : Color.Blue );
          c.FillRectangle( (float)(wq + (i - MAX_MATRIX / 2) * (wq * 1.8f / MAX_MATRIX)),
                           (float)(3 * hq + (j - MAX_MATRIX / 2) * (hq * 1.7f / MAX_MATRIX)),
                           (((i ^ j) & 15) + 1.0f) / MAX_MATRIX * minq * 0.08f,
                           (((i ^ j) & 15) + 1.0f) / MAX_MATRIX * minq * 0.08f,
                           (i + j) * 360.0f / MAX_MATRIX );
        }

      // !!!}}
    }
  }
}
