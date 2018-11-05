using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Rendering
{
  class PanAndZoomSupport
  {
    private const int border = 50;
    private const float minimalAbsoluteSizeInPixels = 20;

    private readonly PictureBox pictureBox;
    private float zoom;
    
    public Image image;
    private readonly Action<string> setWindowTitleSuffix;

    private PointF mouseDown;

    private float imageX;
    private float imageY;   
    private float startX;
    private float startY;

    /// <summary>
    /// Default constructor
    /// </summary>
    /// <param name="pictureBox">Picturebox to apply to zoom and pan</param>
    /// <param name="image">Image of picturebox</param>
    /// <param name="setWindowTitleSuffix">Delegate to method which displays current zoom factor</param>
    public PanAndZoomSupport ( PictureBox pictureBox,
                               Image image,
                               Action<string> setWindowTitleSuffix )
    {
      this.pictureBox = pictureBox;
      this.image = image;
      this.setWindowTitleSuffix = setWindowTitleSuffix;

      zoom = 1;
    }

    /// <summary>
    /// Zooms in to/out from middle of image; Useful when zooming by keys
    /// </summary>
    /// <param name="zoomIn"></param>
    /// <param name="modifierKeys">Currently pressed keys - used for detection of Shift key for faster zooming</param>
    public void zoomToMiddle ( bool zoomIn, Keys modifierKeys )
    {
      PointF middle = new PointF ();
      middle.X = ( ( imageX + image.Width ) * zoom ) / 2f;
      middle.Y = ( ( imageY + image.Height ) * zoom ) / 2f;

      zoomToPosition ( zoomIn, middle, modifierKeys );
    }

    /// <summary>
    /// Changes global variable zoom to indicate current zoom level of picture in main picture box
    /// Variable zoom can be equal 1 (no zoom), less than 1 (zoom out) or greater than 1 (zoom in)
    /// </summary>
    /// <param name="zoomIn">TRUE if zoom in is desired; FALSE if zoom out is desired</param>
    /// <param name="zoomToPosition">Position to zoom to/zoom out from - usually cursor position or middle of picture in case of zoom by keys</param>
    /// <param name="modifierKeys">Currently pressed keys - used for detection of Shift key for faster zooming</param>
    public void zoomToPosition ( bool zoomIn, PointF zoomToPosition, Keys modifierKeys )
    {
      float oldzoom    = zoom;
      float zoomFactor = 0.15F;

      if ( modifierKeys.HasFlag ( Keys.Shift ) ) // holding down the Shift key makes zoom in/out faster
        zoomFactor = 0.45F;

      if ( zoomIn )
        zoom += zoomFactor;
      else
        zoom -= zoomFactor;

      ClampZoom ();

      float x = zoomToPosition.X - pictureBox.Location.X;
      float y = zoomToPosition.Y - pictureBox.Location.Y;

      float oldImageX = x / oldzoom;
      float oldImageY = y / oldzoom;

      float newImageX = x / zoom;
      float newImageY = y / zoom;

      imageX = newImageX - oldImageX + imageX;
      imageY = newImageY - oldImageY + imageY;

      pictureBox.Refresh ();

      setWindowTitleSuffix ( $" Zoom: {(int) ( zoom * 100 )}%" );
    }

    /// <summary>
    /// Prevents picture to be too small (minimum is absolute size of 20 pixels for width/height)
    /// </summary>
    /// <returns>Clamped zoom</returns>
    public void ClampZoom ()
    {
      float minZoomFactor = minimalAbsoluteSizeInPixels / Math.Min ( image.Width, image.Height );
      zoom = Math.Max ( zoom, minZoomFactor );
    }

    /// <summary>
    /// Prevents panning of image outside of picture box
    /// There will always be small amount of pixels (variable border) visible at the edge
    /// </summary>
    public void OutOfScreenFix ()
    {
      float absoluteX = imageX * zoom;
      float absoluteY = imageY * zoom;

      float width  = image.Width * zoom;
      float height = image.Height * zoom;

      if ( absoluteX > pictureBox.Width - border )
        imageX = ( pictureBox.Width - border ) / zoom;

      if ( absoluteY > pictureBox.Height - border )
        imageY = ( pictureBox.Height - border ) / zoom;

      if ( absoluteX + width < border )
        imageX = ( border - width ) / zoom;

      if ( absoluteY + height < border )
        imageY = ( border - height ) / zoom;
    }

    /// <summary>
    /// Resets position and zoom factor of picture
    /// </summary>
    public void Reset ()
    {
      zoom = 1;

      imageX = 0;
      imageY = 0;

      pictureBox.Refresh ();

      setWindowTitleSuffix ( $" Zoom: {(int) ( zoom * 100 )}%" );
    }

    /// <summary>
    /// Converts absolute cursor location (in picture box) to relative location (based on position of actual picture; both translated and scaled)
    /// </summary>
    /// <param name="absoluteX">Absolute X position of cursor (technically relative to picture box)</param>
    /// <param name="absoluteY">Absolute Y position of cursor (technically relative to picture box)</param>
    /// <returns>Relative location of cursor in terms of actual rendered picture; (-1, -1) if relative position would be outside of picture</returns>
    public PointF getRelativeCursorLocation ( int absoluteX, int absoluteY )
    {
      float X = ( absoluteX - ( imageX * zoom ) ) / zoom;
      float Y = ( absoluteY - ( imageY * zoom ) ) / zoom;

      if ( X < 0 || X > image.Width || Y < 0 || Y > image.Height )
        return new PointF ( -1, -1 ); // cursor is outside of image
      else
        return new PointF ( X, Y );
    }

    /// <summary>
    /// Should be called from MouseMove event
    /// </summary>
    /// <param name="mousePosNow">Current cursor position</param>
    public void MouseMove ( Point mousePosNow )
    {     
      float deltaX = mousePosNow.X - mouseDown.X;
      float deltaY = mousePosNow.Y - mouseDown.Y;

      imageX = startX + ( deltaX / zoom );
      imageY = startY + ( deltaY / zoom );

      pictureBox.Refresh ();
    }

    /// <summary>
    /// Should be called from MouseDown event (when correct mouse button is pressed)
    /// </summary>
    /// <param name="mousePosNow">Current cursor position</param>
    public void MouseDown ( Point mousePosNow )
    {
      mouseDown = mousePosNow;
      startX = imageX;
      startY = imageY;
    }

    /// <summary>
    /// Should be called from Paint event of PictureBox
    /// </summary>
    /// <param name="e"></param>
    public void Paint ( PaintEventArgs e )
    {
      if ( image == null )
        return;

      e.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
      e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
      e.Graphics.SmoothingMode = SmoothingMode.None;

      OutOfScreenFix ();

      e.Graphics.ScaleTransform ( zoom, zoom );
      e.Graphics.DrawImage ( image, imageX, imageY );
    }

    /// <summary>
    /// Sets new image to PictueBox, clamps zoom and makes PictureBox to refresh
    /// </summary>
    /// <param name="newImage">New image to set for PictureBox</param>
    public void SetNewImage ( Bitmap newImage )
    {
      image = newImage;
      ClampZoom ();
      pictureBox.Invalidate ();
    }
  }
}
