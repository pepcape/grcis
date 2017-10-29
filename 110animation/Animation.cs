using System;
using System.Collections.Generic;
using System.Drawing;
using LineCanvas;
using MathSupport;
using Utilities;

namespace _110animation
{
  public class Animation
  {
    /// <summary>
    /// Initialize form parameters.
    /// </summary>
    public static void InitParams ( out int wid, out int hei, out double from, out double to, out double fps, out string param, out string name )
    {
      // single frame:
      wid = 640;
      hei = 480;

      // animation:
      from =  0.0;
      to   = 10.0;
      fps  = 25.0;

      // specific animation params:
      param = "width=1.0,anti=true,objects=200,prob=0.95";

      // put your name here:
      name = "Josef Pelikán";
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
    public static void InitAnimation ( int width, int height, double start, double end, double fps, string param )
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
    /// <param name="param">Optional string parameter from the form.</param>
    public static void DrawFrame ( Canvas c, double time, double start, double end, string param )
    {
      // !!!{{ TODO: put your drawing code here

      float timeNorm = (float)Arith.Clamp( (time - start) / (end - start), 0.0, 1.0 );

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
      Random r = new Random( 12 );

      const int MAX_LINES = 30;
      for ( i = 0, t = 0.0f; i < MAX_LINES; i++, t += 1.0f / MAX_LINES )
      {
        c.SetColor( Color.FromArgb( (i * 255) / MAX_LINES, 255, 255 - (i * 255) / MAX_LINES ) ); // [0,255,255] -> [255,255,0]
        float center = (float)(hh * r.NextDouble());
        float min = center - timeNorm * hh;
        float max = center + timeNorm * hh;
        c.MoveTo( t * wh, Math.Max( 0.0f, min ) );
        c.VLineTo( Math.Min ( hh, max ) );
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
        // animated gap:
        c.Draw( timeNorm * 0.8f * size );
        c.Skip( 0.2f * size );
        c.Draw( (1.0f - timeNorm) * 0.8f * size );
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
        size = (float)((0.1 + timeNorm * r.NextDouble()) * padding * 0.5f);
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

          if ( j < 70.0 * timeNorm )
            c.Draw( d );
          else
            c.Skip( d );

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
