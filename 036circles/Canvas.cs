using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;

namespace _036circles
{
  public class Canvas
  {
    /// <summary>
    /// Canvas width in pixels.
    /// </summary>
    public int Width
    {
      get; set;
    }

    /// <summary>
    /// Canvas height in pixels.
    /// </summary>
    public int Height
    {
      get; set;
    }

    protected Bitmap bmp;

    protected Graphics gr;

    protected Color currColor = Color.White;

    protected Pen currPen;

    protected SolidBrush currBrush;

    protected bool currAntiAlias = false;

    public Canvas ()
    {
      Width = 800;
      Height = 600;
      bmp = new Bitmap( Width, Height, PixelFormat.Format24bppRgb );
      gr = Graphics.FromImage( bmp );
      currPen = new Pen( Color.White );
      currBrush = new SolidBrush( Color.White );
    }

    public void Clear ( Color bg )
    {
      if ( bmp.Width  != Width ||
           bmp.Height != Height )
      {
        bmp = new Bitmap( Width, Height, PixelFormat.Format24bppRgb );
        gr = Graphics.FromImage( bmp );
        gr.SmoothingMode = currAntiAlias;
      }
      gr.Clear( bg );
    }

    public void SetAntiAlias ( bool aa )
    {
      if ( currAntiAlias == aa ) return;
      currAntiAlias = aa;
      gr.SmoothingMode = currAntiAlias;
    }

    public void SetColor ( Color col )
    {
      if ( currColor.Equals( col ) ) return;
      currColor = col;
      currPen.Color = col;
      currBrush.Color = col;
    }

    public void FillDisk ( float x, float y, float r )
    {
      gr.FillEllipse( currBrush, x - r, y - r, r + r, r + r );
    }

    public void DrawCircle ( float x, float y, float r )
    {
      gr.DrawEllipse( currPen, x - r, y - r, r + r, r + r );
    }

    public Bitmap Finish ()
    {
      gr.Dispose();
      Bitmap result = bmp;
      bmp = new Bitmap( Width, Height, PixelFormat.Format24bppRgb );
      gr = Graphics.FromImage( bmp );
      gr.SmoothingMode = currAntiAlias;
      return result;
    }

  }
}
