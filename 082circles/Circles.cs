// Author: Josef Pelikan

using System;
using System.Drawing;
using System.Globalization;
using CircleCanvas;

namespace _082circles
{
  public class Circles
  {
    /// <summary>
    /// Optional data initialization.
    /// </summary>
    public static void InitParams ( out int wid, out int hei, out string param )
    {
      wid   = 800;
      hei   = 520;
      param = "1.0";
    }

    /// <summary>
    /// Draw the image into the initialized Canvas object.
    /// </summary>
    /// <param name="c">Canvas ready for your drawing.</param>
    /// <param name="param">Optional string parameter from the form.</param>
    public static void Draw ( Canvas c, string param )
    {
      // !!!{{ TODO: put your drawing code here

      int wq = c.Width  / 4;
      int hq = c.Height / 4;
      int minq = Math.Min( wq, hq );
      double t;
      int i, j;

      c.Clear( Color.Black );

      // example of passing numerical value through string param:
      float w = 1.0f;
      if ( float.TryParse( param, NumberStyles.Number, CultureInfo.InvariantCulture, out w ) )
        c.SetPenWidth( w );

      // 1st quadrant - anti-aliased disks..
      c.SetAntiAlias( true );
      const int MAX_DISK = 30;
      for ( i = 0, t = 0.0; i < MAX_DISK; i++, t += 0.65 )
      {
        float r = 5.0f + i * (minq * 0.7f - 5.0f) / MAX_DISK;
        c.SetColor( Color.FromArgb( (i * 255) / MAX_DISK, 255, 255 - (i * 255) / MAX_DISK ) );
        c.FillDisk( (float)(wq + r * Math.Sin( t )), (float)(hq + r * Math.Cos( t )), (float)(r * 0.3) );
      }

      // 2nd quadrant - anti-aliased circles..
      const int MAX_CIRCLES = 60;
      c.SetColor( Color.LemonChiffon );
      for ( i = 0; i < MAX_CIRCLES; i++ )
      {
        t = i / (double)MAX_CIRCLES;
        c.DrawCircle( (float)(c.Width - t * (wq + wq)), (float)(t * (hq + hq)), (float)(t * 0.9 * minq) );
      }

      // 4th quadrant - jaggy circles..
      c.SetColor( Color.LightCoral );
      c.SetAntiAlias( false );
      for ( i = 0; i < MAX_CIRCLES; i++ )
      {
        t = i / (double)MAX_CIRCLES;
        c.DrawCircle( (float)(c.Width - t * (wq + wq)), (float)(c.Height - t * (hq + hq)), (float)(t * 0.9 * minq) );
      }

      // 3rd quadrant - jaggy disks..
      const int DISKS = 12;
      for ( j = 0; j < DISKS; j++ )
        for ( i = 0; i < DISKS; i++ )
        {
          c.SetColor( ((i ^ j) & 1) == 0 ? Color.White : Color.Blue );
          c.FillDisk( (float)(wq + (i - DISKS / 2) * (wq * 1.8f / DISKS)),
                      (float)(3 * hq + (j - DISKS / 2) * (hq * 1.7f / DISKS)),
                      (((i ^ j) & 15) + 1.0f)/ DISKS * minq * 0.08f  );
        }

      // !!!}}
    }
  }
}
