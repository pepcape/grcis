using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System;
using MathSupport;

namespace _075animation
{
  public class Canvas
  {
    #region Status&Support

    protected Bitmap bmp = null;

    protected Graphics gr = null;

    protected Color currColor = Color.White;

    protected SolidBrush currBrush = null;

    protected GraphicsPath path = new GraphicsPath();

    protected RectangleF rect = new RectangleF();

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
      if ( currBrush != null )
        currBrush.Dispose();
      currBrush = new SolidBrush( Color.White );
    }

    /// <summary>
    /// Clears the image using the given color.
    /// </summary>
    /// <param name="bg">Background color.</param>
    public void Clear ( Color bg )
    {
      if ( bmp.Width != Width ||
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
      currBrush.Color = col;
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

    public void FillPolygon ( float[] c )
    {
      path.Reset();
      int i;
      for ( i = 0; i < c.Length - 3; i += 2 )
        path.AddLine( c[ i ], c[ i + 1 ], c[ i + 2 ], c[ i + 3 ] );
      gr.FillPath( currBrush, path );
    }

    /// <summary>
    /// Reinitializes the canvas, returns result in the Bitmap form.
    /// </summary>
    /// <returns>Result Bitmap.</returns>
    public Bitmap Finish ()
    {
      Bitmap result = bmp;
      bmp = null;
      InitializeBitmap();

      return result;
    }
  }
}
