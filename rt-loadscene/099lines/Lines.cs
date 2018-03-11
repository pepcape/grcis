using System;
using System.Collections.Generic;
using System.Drawing;
using LineCanvas;
using Utilities;

namespace _099lines
{
  public class Lines
  {
    /// <summary>
    /// Data initialization.
    /// </summary>
    public static void InitParams ( out int wid, out int hei, out string param, out string name )
    {
      wid   = 800;
      hei   = 520;
      param = "width=1.0,anti=true,objects=200,prob=0.95";
      name  = "Josef Pelikán";
    }

    /// <summary>
    /// Draw the image into the initialized Canvas object.
    /// </summary>
    /// <param name="c">Canvas ready for your drawing.</param>
    /// <param name="param">Optional string parameter from the form.</param>
    public static void Draw ( Canvas c, string param )
    {
      // !!!{{ TODO: put your drawing code here

      // input params:
      float penWidth = 1.0f;  // pen width
      bool antialias = true;  // use anti-aliasing?
      bool explicitAntialias = false;
      int objects = 200;      // number of randomly generated objects (crosses, lines, Brownian particles)
      double prob = 0.95;     // continue-probability for the Brownian motion simulator

      Dictionary<string, string> p = Util.ParseKeyValueList( param );
      if ( p.Count > 0 )
      {
        // with=<line-width>
        if ( Util.TryParse( p, "width", ref penWidth ) )
        {
          if ( penWidth < 0.0f )
            penWidth = 0.0f;
        }

        // anti=<bool>
        if ( Util.TryParse( p, "anti", ref antialias ) )
          explicitAntialias = true;

        // objects=<number>
        if ( Util.TryParse( p, "objects", ref objects ) &&
             objects < 0 )
          objects = 0;

        // prob=<probability>
        if ( Util.TryParse( p, "prob", ref prob ) &&
             prob > 0.999 )
          prob = 0.999;
      }

      int wq = c.Width / 4;
      int hq = c.Height / 4;
      int wh = wq + wq;
      int hh = hq + hq;
      int minh = Math.Min( wh, hh );
      int i, j, dir;
      float t, cx, cy, x, y;

      c.Clear( Color.Black );

      // 1st quadrant - V and H stripes
      c.SetPenWidth( penWidth );
      c.SetAntiAlias( explicitAntialias && antialias );

      const int MAX_LINES = 30;
      for ( i = 0, t = 0.0f; i < MAX_LINES; i++, t += 1.0f / MAX_LINES )
      {
        c.SetColor( Color.FromArgb( (i * 255) / MAX_LINES, 255, 255 - (i * 255) / MAX_LINES ) ); // [0,255,255] -> [255,255,0]
        c.MoveTo( t * wh, 0 );
        c.VLineTo( hh );
      }
      for ( i = 0, t = 0.0f; i < MAX_LINES; i++, t += 1.0f / MAX_LINES )
      {
        c.SetColor( Color.FromArgb( 255, 255 - (i * 255) / MAX_LINES, (i * 255) / MAX_LINES ) ); // [255,255,0] -> [255,0,255]
        c.MoveTo( 0, hh - t * hh );
        c.HLineTo( t * wh );
      }

      // 2nd quadrant - random isolated segments
      float size = minh / 10.0f;
      float padding = size;
      c.SetColor( Color.LemonChiffon );
      c.SetPenWidth( 1.0f );
      Random r = new Random( 12 );

      for ( i = 0; i < objects; i++ )
      {
        do
          cx = (float)(r.NextDouble() * wh);
        while ( cx < padding ||
                cx > wh - padding );
        c.SetAntiAlias( cx > wq );
        cx += wh;

        do
          cy = (float)(r.NextDouble() * hh);
        while ( cy < padding ||
                cy > hh - padding );

        dir = r.Next() % 4;

        c.MoveTo( cx, cy, dir );
        c.Draw( size );
      }

      // 3rd quadrant - random crosses
      c.SetColor( Color.LightCoral );
      c.SetPenWidth( penWidth );
      padding = minh / 16.0f;

      for ( i = 0; i < objects; i++ )
      {
        do
          cx = (float)(r.NextDouble() * wh);
        while ( cx < padding ||
                cx > wh - padding );
        c.SetAntiAlias( cx > wq );

        do
          cy = (float)(r.NextDouble() * hh);
        while ( cy < padding ||
                cy > hh - padding );
        cy += hh;

        dir = r.Next() % 4;
        size = (float)(r.NextDouble() * padding * 0.5f);
        c.MoveTo( cx, cy, dir );

        c.Draw( size );
        c.Draw( size );
        c.Left();
        c.Draw( size );
        c.Right();
        c.Draw( size );
        c.Right();
        c.Draw( size );
        c.Left();
        c.Draw( size );
        c.Right();
        c.Draw( size );
        c.Right();
        c.Draw( size );
        c.Left();
        c.Draw( size );
        c.Right();
        c.Draw( size );
        c.Right();
        c.Draw( size );
        c.Left();
        c.Draw( size );
        c.Draw( size );
        c.Right();
        c.Draw( size );
      }

      // 4th quadrant - "Brownian" motion
      c.SetPenWidth( penWidth );
      c.SetAntiAlias( true );
      size = minh / 10.0f;
      padding = size;

      for ( i = 0; i < objects; i++ )
      {
        do
          x = (float)(r.NextDouble() * wh);
        while ( x < padding ||
                x > wh - padding );

        do
          y = (float)r.NextDouble() * hh;
        while ( y < padding ||
                y > hh - padding );

        c.SetColor( Color.FromArgb( 127 + r.Next( 0, 128 ),
                                    127 + r.Next( 0, 128 ),
                                    127 + r.Next( 0, 128 ) ) );

        dir = r.Next() % 4;
        c.MoveTo( x + wh, y + hh, dir );

        for ( j = 0; j++ < 1000; )
        {
          float d = (float)(size * r.NextDouble());
          c.Draw( d );

          if ( (r.Next() & 1) > 0 )
            c.Left();
          else
            c.Right();

          if ( c.CurrentX < wh || c.CurrentX > c.Width ||
               c.CurrentY < hh || c.CurrentY > c.Height ||
               r.NextDouble() > prob )
              break;
        }
      }

      // !!!}}
    }
  }
}
