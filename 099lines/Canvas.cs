using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace LineCanvas
{
  public class Canvas
  {
    #region Status&Support

    public const int DIRECTION_RIGHT = -1;
    public const int DIRECTION_LEFT  = -2;
    public const int DIRECTION_KEEP  = -3;

    public const int DIRECTION_NORTH =  0;
    public const int DIRECTION_WEST  =  1;
    public const int DIRECTION_SOUTH =  2;
    public const int DIRECTION_EAST  =  3;

    protected Bitmap bmp = null;

    protected Graphics gr = null;

    protected Color currColor = Color.White;

    protected float currPenWidth = 1.0f;

    protected Pen currPen = null;

    protected bool currAntiAlias = false;

    protected float currX = 0.0f;

    protected float currY = 0.0f;

    protected int currDir = DIRECTION_EAST;

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
    /// Sets the current direction, absolutely or relatively.
    /// </summary>
    private void direction ( int dir )
    {
      switch ( dir )
      {
        case DIRECTION_KEEP:
        default:
          break;

        case DIRECTION_NORTH:
        case DIRECTION_SOUTH:
        case DIRECTION_WEST:
        case DIRECTION_EAST:
          currDir = dir;
          break;

        case DIRECTION_LEFT:
          if ( ++currDir > DIRECTION_EAST )
            currDir = DIRECTION_NORTH;
          break;

        case DIRECTION_RIGHT:
          if ( --currDir < DIRECTION_NORTH )
            currDir = DIRECTION_EAST;
          break;
      }
    }

    /// <summary>
    /// Turns the pen to the left.
    /// </summary>
    public void Left ()
    {
      direction( DIRECTION_LEFT );
    }

    /// <summary>
    /// Turns the pen to the right.
    /// </summary>
    public void Right ()
    {
      direction( DIRECTION_RIGHT );
    }

    /// <summary>
    /// Moves the current position (absolutely) and optionally changes the current direction.
    /// </summary>
    public void MoveTo ( float x, float y, int dir =DIRECTION_KEEP )
    {
      currX = x;
      currY = y;
      direction( dir );
    }

    /// <summary>
    /// Moves the current position (relatively) and optionally changes the current direction.
    /// </summary>
    public void MoveRel ( float dx, float dy, int dir =DIRECTION_KEEP )
    {
      currX += dx;
      currY += dy;
      direction( dir );
    }

    /// <summary>
    /// Current X-coordinate (horizontal).
    /// </summary>
    public float CurrentX
    {
      get
      {
        return currX;
      }
    }

    /// <summary>
    /// Current Y-coordinate (vertical).
    /// </summary>
    public float CurrentY
    {
      get
      {
        return currY;
      }
    }

    /// <summary>
    /// Draws the horizontal line (absolute).
    /// </summary>
    public void HLineTo ( float x )
    {
      gr.DrawLine( currPen, currX, currY, x, currY );
      currX = x;
    }

    /// <summary>
    /// Draws the horizontal line (relative).
    /// </summary>
    public void HLineRel ( float dx )
    {
      gr.DrawLine( currPen, currX, currY, currX + dx, currY );
      currX += dx;
    }

    /// <summary>
    /// Draws the vertical line (absolute).
    /// </summary>
    public void VLineTo ( float y )
    {
      gr.DrawLine( currPen, currX, currY, currX, y );
      currY = y;
    }

    /// <summary>
    /// Draws the vertical line (relative).
    /// </summary>
    public void VLineRel ( float dy )
    {
      gr.DrawLine( currPen, currX, currY, currX, currY + dy );
      currY += dy;
    }

    /// <summary>
    /// Draws the forward line.
    /// </summary>
    public void Draw ( float d )
    {
      float oldX = currX;
      float oldY = currY;
      Skip( d );
      gr.DrawLine( currPen, oldX, oldY, currX, currY );
    }

    /// <summary>
    /// Skips the given distance.
    /// </summary>
    public void Skip ( float d )
    {
      switch ( currDir )
      {
        case DIRECTION_NORTH:
          currY -= d;
          break;

        case DIRECTION_SOUTH:
          currY += d;
          break;

        case DIRECTION_EAST:
          currX += d;
          break;

        case DIRECTION_WEST:
          currX -= d;
          break;
      }
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
