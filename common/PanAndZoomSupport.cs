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

    /// <summary>
    /// Copy (prevention of image.Dispose() outside of this class) of the image.
    /// </summary>
    private Image image = null;

    public Image CurrentImage () => image;

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

    // Memory consumption!
    private const byte defaultHistoryCapacity = 5;

    /// <summary>
    /// Which mouse button is used for panning?
    /// </summary>
    public MouseButtons Button
    {
      get; set;
    } = MouseButtons.Left;

    /// <summary>
    /// Default constructor
    /// </summary>
    /// <param name="pictureBox">PictureBox to apply to zoom and pan.</param>
    /// <param name="im">Image for pictureBox.</param>
    /// <param name="setWindowTitleSuffix">Delegate to method which displays current zoom factor.</param>
    public PanAndZoomSupport (
      PictureBox pictureBox,
      Image im,
      Action<string> setWindowTitleSuffix)
    {
      this.pictureBox = pictureBox;
      setImage(im);
      this.setWindowTitleSuffix = setWindowTitleSuffix;

      zoom = 1;

      history = new List<Image>(defaultHistoryCapacity);
      historyIndex = -1;
    }

    /// <summary>
    /// Prevents picture to be too small (minimum is absolute size of 20 pixels for width/height)
    /// </summary>
    /// <returns>Clamped zoom</returns>
    private void ClampZoom ()
    {
      if (image == null)
        return;

      float minZoomFactor = minimalAbsoluteSizeInPixels / Math.Min(image.Width, image.Height);
      zoom = Math.Max(zoom, minZoomFactor);
    }

    /// <summary>
    /// Zooms in to/out from middle of image; Useful when zooming by keys
    /// </summary>
    /// <param name="zoomIn">Negative for zoom out, positive for zoom in</param>
    /// <param name="modifierKeys">Currently pressed keys - used for detection of Shift key for faster zooming</param>
    public void ZoomToMiddle (
      int zoomIn,
      Keys modifierKeys)
    {
      if (image == null)
        return;

      PointF middle = new PointF
      {
        X =  imageX * zoom + 0.5f * image.Width  * zoom,
        Y =  imageY * zoom + 0.5f * image.Height * zoom 
      };

      ZoomToPosition(zoomIn, middle, modifierKeys);
    }

    private const float zoomFactorNormal = 0.1f;

    private const float zoomFactorFast = 0.3f;

    /// <summary>
    /// Changes global variable zoom to indicate current zoom level of picture in main picture box.
    /// </summary>
    /// <param name="newZoom">Negative for zoom out, positive for zoom in</param>
    /// <param name="position">Position to zoom to/zoom out from - usually cursor position (relative to picturebox, not to image)
    /// or middle of picture in case of zoom by keys</param>
    public void UpdateZoom (
      float newZoom,
      PointF position)
    {
      float oldzoom = zoom;
      zoom = newZoom;

      ClampZoom();

      float x = position.X;
      float y = position.Y;

      float oldImageX = x / oldzoom;
      float oldImageY = y / oldzoom;

      float newImageX = x / zoom;
      float newImageY = y / zoom;

      imageX = newImageX - oldImageX + imageX;
      imageY = newImageY - oldImageY + imageY;

      pictureBox.Refresh();

      setWindowTitleSuffix($" Zoom: {(int)(zoom * 100)}%");
    }

    public void UpdateZoomToMiddle (
      float newZoom = 1.0f)
    {
      if (image == null)
        return;

      PointF middle = new PointF
      {
        X =  imageX * zoom + 0.5f * image.Width  * zoom,
        Y =  imageY * zoom + 0.5f * image.Height * zoom
      };

      UpdateZoom(newZoom, middle);
    }

    /// <summary>
    /// Changes global variable zoom to indicate current zoom level of picture in main picture box.
    /// </summary>
    /// <param name="zoomIn">Negative for zoom out, positive for zoom in</param>
    /// <param name="position">Position to zoom to/zoom out from - usually cursor position (relative to picturebox, not to image)
    /// or middle of picture in case of zoom by keys</param>
    /// <param name="modifierKeys">Currently pressed keys - used for detection of Shift key for faster zooming</param>
    public void ZoomToPosition (
      int zoomIn,
      PointF position,
      Keys modifierKeys)
    {
      float newZoom = zoom;

      if (zoomIn != 0)
      {
        float zoomFactor = zoomFactorNormal;

        if (modifierKeys.HasFlag(Keys.Shift)) // holding down the Shift key makes zoom in/out faster
          zoomFactor = zoomFactorFast;

        newZoom += zoomIn > 0
          ?  zoomFactor
          : -zoomFactor;
      }

      UpdateZoom(newZoom, position);
    }

    /// <summary>
    /// Converts absolute cursor location (in picture box) to relative location (based on position of actual picture; both translated and scaled)
    /// </summary>
    /// <param name="absoluteX">Absolute X position of cursor (technically relative to picture box)</param>
    /// <param name="absoluteY">Absolute Y position of cursor (technically relative to picture box)</param>
    /// <returns>Relative location of cursor in terms of actual rendered picture; (NaN, NaN) if relative position would be outside of picture</returns>
    public PointF GetRelativeCursorLocation (
      int absoluteX,
      int absoluteY)
    {
      float X = (absoluteX -  imageX * zoom) / zoom;
      float Y = (absoluteY -  imageY * zoom) / zoom;

      if (image == null ||
          X < 0 || X >= image.Width ||
          Y < 0 || Y >= image.Height)
        return new PointF(float.NaN, float.NaN); // cursor is outside of image
      else
        return new PointF(X, Y);
    }

    /// <summary>
    /// Should be called from MouseDown event of PictureBox
    /// </summary>
    /// <param name="e">Needed to get pressed mouse button and cursor location</param>
    /// <param name="action">Action to do - parameters are int coordinates (X and Y) relative to image</param>
    /// <param name="condition">Condition which determines whether action should be invoked (usually right mouse button pressed etc.)</param>
    /// <param name="modifierKeys">ModifierKeys property</param>
    /// <param name="cursor">Cursor property passed by ref so it can be changed</param>
    public void OnMouseDown (
      MouseEventArgs e,
      Action<int, int> action,
      bool condition,
      Keys modifierKeys,
      out Cursor cursor)
    {
      cursor = null;

      if (condition)
      {
        PointF relative = GetRelativeCursorLocation(e.X, e.Y);

        if (action != null &&
            !float.IsNaN(relative.X))
          action((int)relative.X, (int)relative.Y);
      }

      // Holding down CTRL key prevents panning.
      if (!modifierKeys.HasFlag(Keys.Control) &&
          e.Button == Button &&
          !mousePressed)
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
    public void OnMouseMove (
      MouseEventArgs e,
      Action<int, int> action,
      bool condition,
      Keys modifierKeys,
      out Cursor cursor)
    {
      cursor = null;

      if (condition)
      {
        PointF relative = GetRelativeCursorLocation(e.X, e.Y);

        if (action != null &&
            !float.IsNaN(relative.X) &&
            !mousePressed)
          action((int)relative.X, (int)relative.Y);
      }

      if (mousePressed &&
          e.Button == Button)
      {
        cursor = Cursors.NoMove2D;

        float deltaX = e.Location.X - mouseDown.X;
        float deltaY = e.Location.Y - mouseDown.Y;

        imageX = startX + deltaX / zoom;
        imageY = startY + deltaY / zoom;

        pictureBox.Refresh();
      }
    }

    /// <summary>
    /// Should be called from MouseUp event of PictureBox
    /// </summary>
    /// <param name="cursor">Cursor property passed by ref so it can be changed</param>
    public void OnMouseUp (
      out Cursor cursor)
    {
      mousePressed = false;

      cursor = Cursors.Default;
    }

    /// <summary>
    /// Should be called from MouseWheel event of PictureBox
    /// </summary>
    /// <param name="e">Needed for mouse wheel delta value and cursor position</param>
    /// <param name="modifierKeys">ModifierKeys - needed for later Shift/Ctrl key check</param>
    public void OnMouseWheel (
      MouseEventArgs e,
      Keys modifierKeys)
    {
      PointF relative = GetRelativeCursorLocation(e.Location.X, e.Location.Y);

      if (float.IsNaN(relative.X)) // prevents scrolling if cursor is outside of image
        return;

      ZoomToPosition(e.Delta, e.Location, modifierKeys);
    }

    /// <summary>
    /// Should be called from KeyDown event of Form; 
    /// Catches +/PageUp for zoom in or -/PageDown for zoom out of image in picture box
    /// </summary>
    /// <param name="keys">Key which caused KeyDown event</param>
    /// <param name="modifierKeys">ModifierKeys - needed for later Shift/Ctrl key check</param>
    public void OnKeyDown (
      Keys keys,
      Keys modifierKeys)
    {
      switch (keys)
      {
        case Keys.Add:
        case Keys.PageUp:
          ZoomToMiddle(1, modifierKeys);
          break;

        case Keys.Subtract:
        case Keys.PageDown:
          ZoomToMiddle(-1, modifierKeys);
          break;
      }
    }

    /// <summary>
    /// Should be called from Paint event of PictureBox
    /// </summary>
    /// <param name="e">Needed to get Graphics class associated with PictureBox</param>
    public void OnPaint (
      PaintEventArgs e)
    {
      if (image == null)
        return;

      e.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
      e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
      e.Graphics.SmoothingMode = SmoothingMode.None;

      OutOfScreenFix();

      e.Graphics.ScaleTransform(zoom, zoom);
      e.Graphics.DrawImage(image, imageX, imageY);
    }

    /// <summary>
    /// Sets new image to PictueBox, clamps zoom and makes PictureBox to refresh (optionally, saves new iumage to history)
    /// </summary>
    /// <param name="newImage">New image to set for PictureBox.</param>
    /// <param name="saveToHistory">TRUE to make this new image to save to history.</param>
    public void SetNewImage (
      Image newImage,
      bool saveToHistory = false)
    {
      if (newImage != image)
        setImage(newImage);
      // Now 'image' contains copy of the new image..

      if (saveToHistory)
      {
        if (history.Count == historyCapacity &&
            historyCapacity != 0)
          history.RemoveAt(0);

        history.Add(image);
        historyIndex = history.Count - 1;
      }

      selectImage(image);
    }

    /// <summary>
    /// Sets next image in history as active
    /// </summary>
    public void SetNextImageFromHistory ()
    {
      historyIndex++;

      selectImage(history[historyIndex]);
    }

    /// <summary>
    /// Sets previous image from history as active
    /// </summary>
    public void SetPreviousImageFromHistory ()
    {
      historyIndex--;

      selectImage(history[historyIndex]);
    }

    /// <summary>
    /// Next image availability
    /// </summary>
    /// <returns>TRUE if there is available next (newer than current one) image in history</returns>
    public bool NextImageAvailable => historyIndex + 1 < history.Count;

    /// <summary>
    /// Previous image availability
    /// </summary>
    /// <returns>TRUE if there is available previous (older than current one) image in history</returns>
    public bool PreviousImageAvailable => historyIndex > 0;

    private void setImage (Image newImage)
    {
      image?.Dispose();
      Bitmap bmp = (Bitmap)newImage.Clone();
      bmp.SetResolution(96.0f, 96.0f);
      image = bmp;
    }

    /// <summary>
    /// Common function for selecting image from history.
    /// Clamps zoom and makes PictureBox to refresh
    /// </summary>
    /// <param name="newImage">Image to set as active</param>
    private void selectImage (Image newImage)
    {
      image = newImage;

      OutOfScreenFix();
      ClampZoom();
      pictureBox.Invalidate();
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

      if (absoluteX > pictureBox.Width - border)
        imageX = (pictureBox.Width - border) / zoom;

      if (absoluteY > pictureBox.Height - border)
        imageY = (pictureBox.Height - border) / zoom;

      if (absoluteX + width < border)
        imageX = (border - width) / zoom;

      if (absoluteY + height < border)
        imageY = (border - height) / zoom;
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

      pictureBox.Refresh();

      setWindowTitleSuffix(" Zoom: 100%");
    }
  }
}
