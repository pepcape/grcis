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

      Pen p = new Pen( Color.FromArgb( 255, 255, 20 ), 2.0f );

      double tim = currentFrame / (totalFrames - 1.0);
      int wid = (int)(tim * maxWid);
      int hei = (int)(tim * maxHei);
      gr.DrawEllipse( p, x0 - wid, y0 - hei, wid + wid, hei + hei );

      return result;

      // !!!}}
    }

  }
}
