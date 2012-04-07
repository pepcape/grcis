using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;

namespace _012animation
{
  public class Animation
  {
    public Animation ()
    {
    }

    public Bitmap RenderFrame ( int width, int height, int currentFrame, int totalFrames )
    {
      // !!!{{ TODO: put your frame-rendering code here

      Bitmap result = new Bitmap( width, height, System.Drawing.Imaging.PixelFormat.Format24bppRgb );
      Graphics gr = Graphics.FromImage( result );

      int x0 = width / 2;
      int y0 = height / 2;
      int maxWid = x0 - 2;
      int maxHei = y0 - 2;

      Pen p1 = new Pen( Color.FromArgb( 255, 255,  20 ), 1.0f );
      Pen p2 = new Pen( Color.FromArgb(  60, 120, 255 ), 2.0f );

      double tim = currentFrame / (totalFrames - 1.0);
      int wid = (int)(tim * maxWid);
      int hei = (int)(tim * maxHei);
      gr.DrawEllipse( p2, x0 - hei, y0 - wid, hei + hei, wid + wid );
      gr.DrawEllipse( p1, x0 - wid, y0 - hei, wid + wid, hei + hei );

      return result;

      // !!!}}
    }

  }
}
