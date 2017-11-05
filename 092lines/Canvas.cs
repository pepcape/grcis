using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;

namespace LineCanvas
{
  public class Canvas
  {
    #region Status&Support

    protected Bitmap bmp = null;

    protected Graphics gr = null;

    protected Color currColor = Color.White;

    protected float currPenWidth = 1.0f;

    protected Pen currPen = null;

    protected bool currAntiAlias = false;

    protected void InitializeBitmap ()
    {
      if ( gr != null )
      {
        gr.Dispose();
        gr = null;
      }
      if ( bmp != null )
      {
        bmp.Dispose();
        bmp = null;
      }
      bmp = new Bitmap( Width, Height, PixelFormat.Format24bppRgb );
      gr = Graphics.FromImage( bmp );
      gr.SmoothingMode = currAntiAlias ? SmoothingMode.AntiAlias : SmoothingMode.None;
    }

    #endregion

    /// <summary>
    /// Canvas width in pixels.
    /// </summary>
    public int Width
    {
      get;
      set;
    }

    /// <summary>
    /// Canvas height in pixels.
    /// </summary>
    public int Height
    {
      get;
      set;
    }

    /// <summary>
    /// Initializes the new canvas to the given resolution, black background.
    /// </summary>
    /// <param name="width">Initial image width.</param>
    /// <param name="height">Initial image height.</param>
    public Canvas ( int width, int height )
    {
      Width  = width;
      Height = height;
      InitializeBitmap();

      if ( currPen != null )
        currPen.Dispose();
      currPen = new Pen( Color.White, currPenWidth );
    }

    /// <summary>
    /// Clears the image using the given color.
    /// </summary>
    /// <param name="bg">Background color.</param>
    public void Clear ( Color bg )
    {
      if ( bmp.Width  != Width ||
           bmp.Height != Height )
        InitializeBitmap();

      gr.Clear( bg );
    }

    /// <summary>
    /// Sets antialiasing-mode for all consequent drawing.
    /// </summary>
    /// <param name="aa">Use anti-aliasing?</param>
    public void SetAntiAlias ( bool aa )
    {
      if ( currAntiAlias == aa ) return;
      currAntiAlias = aa;
      gr.SmoothingMode = currAntiAlias ? SmoothingMode.AntiAlias : SmoothingMode.None;
    }

    /// <summary>
    /// Sets current drawing color, used both for disks and circles.
    /// </summary>
    /// <param name="col">New drawing color.</param>
    public void SetColor ( Color col )
    {
      if ( currColor == col ) return;
      currColor = col;
      currPen.Color = col;
    }

    /// <summary>
    /// Sets current pen width.
    /// </summary>
    /// <param name="width">New pen width in pixels.</param>
    public void SetPenWidth ( float width )
    {
      if ( currPenWidth == width ) return;
      currPen.Width = currPenWidth = width;
    }

    /// <summary>
    /// Draws the line using the current color.
    /// </summary>
    public void Line ( float x1, float y1, float x2, float y2 )
    {
      gr.DrawLine( currPen, x1, y1, x2, y2 );
    }

    /// <summary>
    /// Draws the line using the current color.
    /// </summary>
    public void Line ( double x1, double y1, double x2, double y2 )
    {
      gr.DrawLine( currPen, (float)x1, (float)y1, (float)x2, (float)y2 );
    }

    /// <summary>
    /// Draws the poly-line using the current color.
    /// </summary>
    public void PolyLine ( PointF[] arr )
    {
      gr.DrawLines( currPen, arr );
    }

    /// <summary>
    /// Draws the poly-line using the current color.
    /// </summary>
    public void PolyLine ( IEnumerable<PointF> arr )
    {
      gr.DrawLines( currPen, arr.ToArray() );
    }

    /// <summary>
    /// Reinitializes the canvas, returns result in the Bitmap form.
    /// </summary>
    /// <returns>Result Bitmap.</returns>
    public Bitmap Finish ()
    {
      gr.Dispose();
      gr = null;
      Bitmap result = bmp;
      bmp = null;
      InitializeBitmap();

      return result;
    }
  }
}
