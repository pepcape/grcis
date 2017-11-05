using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System;
using MathSupport;

namespace _053rectangles
{
  public class Canvas
  {
    #region Status&Support

    protected Bitmap bmp;

    protected Graphics gr;

    protected Color currColor = Color.White;

    protected float currPenWidth = 1.0f;

    protected Pen currPen;

    protected SolidBrush currBrush;

    protected GraphicsPath path = new GraphicsPath();

    protected RectangleF rect = new RectangleF();

    protected bool currAntiAlias = false;

    protected void InitializeBitmap ()
    {
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
      currPen = new Pen( Color.White, currPenWidth );
      currBrush = new SolidBrush( Color.White );
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
    /// Sets current drawing color, used both for filled and outlined rectangles.
    /// </summary>
    /// <param name="col">New drawing color.</param>
    public void SetColor ( Color col )
    {
      if ( currColor == col ) return;
      currColor = col;
      currPen.Color = col;
      currBrush.Color = col;
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
    /// Fills interior of the given rectangle.
    /// </summary>
    /// <param name="cx">X-coordinate of the rectangle center.</param>
    /// <param name="cy">Y-coordinate of the rectangle center.</param>
    /// <param name="w">Horizontal size of the rectangle.</param>
    /// <param name="h">Vertical size of the rectangle.</param>
    public void FillRectangle ( float cx, float cy, float w, float h )
    {
      gr.FillRectangle( currBrush, cx - 0.5f * w, cy - 0.5f * h, w, h );
    }

    /// <summary>
    /// Draws the given rectangle using the current color.
    /// </summary>
    /// <param name="cx">X-coordinate of the rectangle center.</param>
    /// <param name="cy">Y-coordinate of the rectangle center.</param>
    /// <param name="w">Horizontal size of the rectangle.</param>
    /// <param name="h">Vertical size of the rectangle.</param>
    public void DrawRectangle ( float cx, float cy, float w, float h )
    {
      gr.DrawRectangle( currPen, cx - 0.5f * w, cy - 0.5f * h, w, h );
    }

    protected void TiltedRectangle ( float cx, float cy, float w, float h, float angle )
    {
      double da = Arith.DegreeToRadian( angle );
      double sina = Math.Sin( da );
      double cosa = Math.Cos( da );
      double dw = 0.5 * w;
      double dh = 0.5 * h;
      double dax =  dw * cosa + dh * sina;
      double day = -dw * sina + dh * cosa;
      double dbx = -dw * cosa + dh * sina;
      double dby =  dw * sina + dh * cosa; // A, B, -A, -B
      path.Reset();
      path.AddLine( (float)(cx + dax), (float)(cy + day), (float)(cx + dbx), (float)(cy + dby) );
      path.AddLine( (float)(cx - dax), (float)(cy - day), (float)(cx - dbx), (float)(cy - dby) );
      path.AddLine( (float)(cx + dax), (float)(cy + day), (float)(cx + dbx), (float)(cy + dby) );
    }

    /// <summary>
    /// Fills interior of the given tilted rectangle.
    /// </summary>
    /// <param name="cx">X-coordinate of the rectangle center.</param>
    /// <param name="cy">Y-coordinate of the rectangle center.</param>
    /// <param name="w">Horizontal size of the rectangle.</param>
    /// <param name="h">Vertical size of the rectangle.</param>
    /// <param name="angle">Rectangle tilt.</param>
    public void FillRectangle ( float cx, float cy, float w, float h, float angle )
    {
      TiltedRectangle( cx, cy, w, h, angle );
      gr.FillPath( currBrush, path );
    }

    /// <summary>
    /// Draws the given tilted rectangle using the current color.
    /// </summary>
    /// <param name="cx">X-coordinate of the rectangle center.</param>
    /// <param name="cy">Y-coordinate of the rectangle center.</param>
    /// <param name="w">Horizontal size of the rectangle.</param>
    /// <param name="h">Vertical size of the rectangle.</param>
    /// <param name="angle">Rectangle tilt.</param>
    public void DrawRectangle ( float cx, float cy, float w, float h, float angle )
    {
      TiltedRectangle( cx, cy, w, h, angle );
      gr.DrawPath( currPen, path );
    }

    /// <summary>
    /// Reinitializes the canvas, returns result in the Bitmap form.
    /// </summary>
    /// <returns>Result Bitmap.</returns>
    public Bitmap Finish ()
    {
      gr.Dispose();
      Bitmap result = bmp;
      InitializeBitmap();
      return result;
    }
  }
}
