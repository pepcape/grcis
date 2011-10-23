using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Threading;

namespace _035plasma
{
  public class Simulation
  {
    public Simulation ( int width, int height )
    {
      Width  = width;
      Height = height;
      Frame  = 0;
    }

    /// <summary>
    /// Width of the visualization bitmap in pixels.
    /// </summary>
    public int Width
    {
      get;
      set;
    }

    /// <summary>
    /// Height of the visualization bitmap in pixels.
    /// </summary>
    public int Height
    {
      get;
      set;
    }

    /// <summary>
    /// Frame number starting from 0.
    /// </summary>
    public long Frame
    {
      get;
      set;
    }

    /// <summary>
    /// Simulation reset.
    /// </summary>
    public void Reset ()
    {
      Frame = 0;
    }

    /// <summary>
    /// Simulation of single timestep.
    /// </summary>
    /// <returns>Visualization bitmap.</returns>
    public Bitmap Simulate ()
    {
      // !!!{{ TODO: put your simulation code here

      Bitmap result = new Bitmap( Width, Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb );
      Graphics gr = Graphics.FromImage( result );

      int x0 = Width / 2;
      int y0 = Height / 2;
      int maxWid = x0 - 2;
      int maxHei = y0 - 2;

      Pen p1 = new Pen( Color.FromArgb( 255, 255,  20 ), 1.0f );
      Pen p2 = new Pen( Color.FromArgb(  60, 120, 255 ), 2.0f );

      double tim = 0.5 + 0.5 * Math.Sin( Frame * 0.01 );
      int wid = (int)(tim * maxWid);
      int hei = (int)(tim * maxHei);
      gr.DrawEllipse( p2, x0 - hei, y0 - wid, hei + hei, wid + wid );
      gr.DrawEllipse( p1, x0 - wid, y0 - hei, wid + wid, hei + hei );

      Frame++;
      Thread.Sleep( 2 );

      return result;

      // !!!}}
    }

  }
}
