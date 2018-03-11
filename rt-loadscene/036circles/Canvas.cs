using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace _036circles
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
    /// Sets current drawing color, used both for disks and circles.
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
    /// Fills the given disk with the current color.
    /// </summary>
    /// <param name="x">X-coordinate of the disk center.</param>
    /// <param name="y">Y-coordinate of the disk center.</param>
    /// <param name="r">Radius of the disk.</param>
    public void FillDisk ( float x, float y, float r )
    {
      gr.FillEllipse( currBrush, x - r, y - r, r + r, r + r );
    }

    /// <summary>
    /// Draws the circle using the current color.
    /// </summary>
    /// <param name="x">X-coordinate of the circle center.</param>
    /// <param name="y">Y-coordinate of the circle center.</param>
    /// <param name="r">Radius of the circle.</param>
    public void DrawCircle ( float x, float y, float r )
    {
      gr.DrawEllipse( currPen, x - r, y - r, r + r, r + r );
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
