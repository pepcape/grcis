using System;
using System.Collections.Generic;
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

    private bool mousePressed;

    private List<Image> history;
    private int historyIndex;
    public byte historyCapacity = defaultHistoryCapacity;

    private const byte defaultHistoryCapacity = 5;
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

      history = new List<Image>( defaultHistoryCapacity );
      historyIndex = -1;
    }

    /// <summary>
    /// Zooms in to/out from middle of image; Useful when zooming by keys
    /// </summary>
    /// <param name="zoomIn">TRUE for zoom in; otherwise zoom out</param>
    /// <param name="modifierKeys">Currently pressed keys - used for detection of Shift key for faster zooming</param>
    public void ZoomToMiddle ( bool zoomIn, Keys modifierKeys )
    {
      PointF middle = new PointF
      {
        X = ( imageX * zoom + ( ( image.Width * zoom ) / 2f ) ),
        Y = ( imageY * zoom + ( ( image.Height * zoom ) / 2f ) )
      };

      ZoomToPosition ( zoomIn, middle, modifierKeys );
    }

    private const float zoomFactorNormal = 0.15f;
    private const float zoomFactorFast = 0.45f;
    /// <summary>
    /// Changes global variable zoom to indicate current zoom level of picture in main picture box; 
    /// Variable zoom can be equal 1 (no zoom), less than 1 (zoom out) or greater than 1 (zoom in)
    /// </summary>
    /// <param name="zoomIn">TRUE if zoom in is desired; FALSE if zoom out is desired</param>
    /// <param name="position">Position to zoom to/zoom out from - usually cursor position (relative to picturebox, not to image)
    /// or middle of picture in case of zoom by keys</param>
    /// <param name="modifierKeys">Currently pressed keys - used for detection of Shift key for faster zooming</param>
    public void ZoomToPosition ( bool zoomIn, PointF position, Keys modifierKeys )
    {
      float oldzoom    = zoom;
      float zoomFactor = zoomFactorNormal;

      if ( modifierKeys.HasFlag ( Keys.Shift ) ) // holding down the Shift key makes zoom in/out faster
        zoomFactor = zoomFactorFast;

      if ( zoomIn )
        zoom += zoomFactor;
      else
        zoom -= zoomFactor;

      ClampZoom ();

      float x = position.X;
      float y = position.Y;

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
    /// Converts absolute cursor location (in picture box) to relative location (based on position of actual picture; both translated and scaled)
    /// </summary>
    /// <param name="absoluteX">Absolute X position of cursor (technically relative to picture box)</param>
    /// <param name="absoluteY">Absolute Y position of cursor (technically relative to picture box)</param>
    /// <returns>Relative location of cursor in terms of actual rendered picture; (NaN, NaN) if relative position would be outside of picture</returns>
    public PointF GetRelativeCursorLocation ( int absoluteX, int absoluteY )
    {
      float X = ( absoluteX - ( imageX * zoom ) ) / zoom;
      float Y = ( absoluteY - ( imageY * zoom ) ) / zoom;

      if ( X < 0 || X >= image.Width || Y < 0 || Y >= image.Height )
        return new PointF ( float.NaN, float.NaN ); // cursor is outside of image
      else
        return new PointF ( X, Y );
    }

    /// <summary>
    /// Should be called from MouseDown event of PictureBox
    /// </summary>
    /// <param name="e">Needed to get pressed mouse button and cursor location</param>
    /// <param name="action">Action to do - parameters are int coordinates (X and Y) relative to image</param>
    /// <param name="condition">Condition which determines whether action should be invoked (usually right mouse button pressed etc.)</param>
    /// <param name="modifierKeys">ModifierKeys property</param>
    /// <param name="cursor">Cursor property passed by ref so it can be changed</param>
    public void MouseDownRegistration ( MouseEventArgs e, Action<int, int> action, bool condition, Keys modifierKeys, out Cursor cursor )
    {
      cursor = null;

      if ( condition )
      {
        PointF relative = GetRelativeCursorLocation ( e.X, e.Y );

        if ( !float.IsNaN ( relative.X ) )
          action ( (int) relative.X, (int) relative.Y );
      }

      if ( !modifierKeys.HasFlag ( Keys.Control ) && e.Button == MouseButtons.Left && !mousePressed ) //holding down CTRL key prevents panning
      {
        mousePressed = true;
        mouseDown = e.Location;
        startX = imageX;
        startY = imageY;
      }
      else
      {
        cursor = Cursors.Cross;
      }      
    }

    /// <summary>
    /// Should be called from MouseMove event of PictureBox
    /// </summary>
    /// <param name="e">Needed to get pressed mouse button and cursor location</param>
    /// <param name="action">Action to do - parameters are int coordinates (X and Y) relative to image</param>
    /// <param name="condition">Condition which determines whether action should be invoked (usually right mouse button pressed etc.)</param>
    /// <param name="modifierKeys">ModifierKeys property</param>
    /// <param name="cursor">Cursor property passed by ref so it can be changed</param>
    public void MouseMoveRegistration ( MouseEventArgs e, Action<int, int> action, bool condition, Keys modifierKeys, out Cursor cursor )
    {
      cursor = null;

      if ( condition )
      {
        PointF relative = GetRelativeCursorLocation ( e.X, e.Y );

        if ( !float.IsNaN ( relative.X ) && !mousePressed )
          action ( (int) relative.X, (int) relative.Y );
      }

      if ( mousePressed && e.Button == MouseButtons.Left )
      {
        cursor = Cursors.NoMove2D;

        float deltaX = e.Location.X - mouseDown.X;
        float deltaY = e.Location.Y - mouseDown.Y;

        imageX = startX + ( deltaX / zoom );
        imageY = startY + ( deltaY / zoom );

        pictureBox.Refresh ();
      }
    }

    /// <summary>
    /// Should be called from MouseUp event of PictureBox
    /// </summary>
    /// <param name="cursor">Cursor property passed by ref so it can be changed</param>
    public void MouseUpRegistration ( out Cursor cursor )
    {
      mousePressed = false;

      cursor = Cursors.Default;
    }

    /// <summary>
    /// Should be called from MouseWheel event of PictureBox
    /// </summary>
    /// <param name="e">Needed for mouse wheel delta value and cursor position</param>
    /// <param name="modifierKeys">ModifierKeys - needed for later Shift/Ctrl key check</param>
    public void MouseWheelRegistration ( MouseEventArgs e , Keys modifierKeys )
    {
      PointF relative = GetRelativeCursorLocation ( e.Location.X, e.Location.Y );

      if ( float.IsNaN ( relative.X ) ) // prevents scrolling if cursor is outside of image
        return;

      if ( e.Delta > 0 )
        ZoomToPosition ( true, e.Location, modifierKeys );
      else if ( e.Delta < 0 )
        ZoomToPosition ( false, e.Location, modifierKeys );
    }

    /// <summary>
    /// Should be called from KeyDown event of Form; 
    /// Catches +/PageUp for zoom in or -/PageDown for zoom out of image in picture box
    /// </summary>
    /// <param name="keys">Key which caused KeyDown event</param>
    /// <param name="modifierKeys">ModifierKeys - needed for later Shift/Ctrl key check</param>
    public void KeyDownRegistration ( Keys keys, Keys modifierKeys )
    {
      switch ( keys )
      {
        case Keys.Add:
        case Keys.PageUp:
          ZoomToMiddle ( true, modifierKeys );
          break;
        case Keys.Subtract:
        case Keys.PageDown:
          ZoomToMiddle ( false, modifierKeys );
          break;
      }
    }   

    /// <summary>
    /// Should be called from Paint event of PictureBox
    /// </summary>
    /// <param name="e">Needed to get Graphics class associated with PictureBox</param>
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
    /// Sets new image to PictueBox, clamps zoom and makes PictureBox to refresh (optionally, saves new iumage to history)
    /// </summary>
    /// <param name="newImage">New image to set for PictureBox</param>
    /// <param name="saveToHistory">TRUE to make this new image to save to history</param>
    public void SetNewImage ( Image newImage, bool saveToHistory )
    {
      if ( newImage == null )
        return;

      if ( saveToHistory )
      {
        if ( history.Count == historyCapacity && historyCapacity != 0)
          history.RemoveAt ( 0 );

        history.Add ( newImage );
        historyIndex = history.Count - 1;     
      }

      SetImage ( newImage );
    }

    /// <summary>
    /// Sets next image in history as active
    /// </summary>
    public void SetNextImageFromHistory ()
    {
      historyIndex++;

      SetImage ( history[historyIndex] );
    }

    /// <summary>
    /// Sets previous image from history as active
    /// </summary>
    public void SetPreviousImageFromHistory ()
    {
      historyIndex--;

      SetImage ( history [historyIndex] );
    }

    /// <summary>
    /// Next image availability
    /// </summary>
    /// <returns>TRUE if there is available next (newer than current one) image in history</returns>
    public bool NextImageAvailable ()
    {
      return historyIndex + 1 < history.Count;
    }

    /// <summary>
    /// Previous image availability
    /// </summary>
    /// <returns>TRUE if there is available previous (older than current one) image in history</returns>
    public bool PreviousImageAvailable ()
    {
      return historyIndex > 0;
    }

    /// <summary>
    /// Common set-image method for setting new image directly or from history
    /// Clamps zoom and makes PictureBox to refresh
    /// </summary>
    /// <param name="newImage">Image to set as active</param>
    private void SetImage ( Image newImage )
    {
      image = newImage;
      OutOfScreenFix ();
      ClampZoom ();
      pictureBox.Invalidate ();
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
    /// Prevents picture to be too small (minimum is absolute size of 20 pixels for width/height)
    /// </summary>
    /// <returns>Clamped zoom</returns>
    public void ClampZoom ()
    {
      float minZoomFactor = minimalAbsoluteSizeInPixels / Math.Min ( image.Width, image.Height );
      zoom = Math.Max ( zoom, minZoomFactor );
    }

    /// <summary>
    /// Resets image in picture box to 100% zoom and default position
    /// (left upper corner of image in left upper conrner of picture box)
    /// </summary>
    public void Reset ()
    {
      zoom = 1;

      imageX = 0;
      imageY = 0;

      pictureBox.Refresh ();

      setWindowTitleSuffix ( " Zoom: 100%" );
    }
  }
}
