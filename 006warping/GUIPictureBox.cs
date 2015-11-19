// Author: Josef Pelikan

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace _006warping
{
  [Designer( "System.Windows.Forms.Design.PictureBoxDesigner, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a" )]
  public partial class GUIPictureBox : PictureBox
  {
    #region data

    /// <summary>
    /// Input image.
    /// </summary>
    private Bitmap input;

    #endregion

    #region warping feature settings

    /// <summary>
    ///  Color to paint the features.
    /// </summary>
    protected Color featuresColor = Color.LightGreen;

    /// <summary>
    /// Gets or sets the features' color.
    /// </summary>
    public Color FeaturesColor
    {
      get { return featuresColor; }
      set
      {
        featuresColor = value;
        // !!! TODO: redraw the current features?
      }
    }

    #endregion

    #region set/get pictures

    /// <summary>
    /// Sets a new input picture.
    /// </summary>
    /// <param name="newInput">New input picture</param>
    public void SetPicture ( Bitmap newInput )
    {
      input = newInput;
      if ( newInput != null )
        Image = (Image)newInput.Clone();
      else
        Image = null;
    }

    /// <summary>
    /// Gets the current output picture.
    /// </summary>
    public Bitmap GetPicture ()
    {
      return (Bitmap)Image;
    }

    #endregion

    #region display results

    protected override void OnPaint ( PaintEventArgs e )
    {
      base.OnPaint( e );
      // !!! TODO: custom drawing!
    }

    #endregion

    #region mouse events

    /// <summary>
    /// Stores position of mouse when button was pressed.
    /// </summary>
    protected Point mouseDownPoint;

    /// <summary>
    /// Indicates wheter the right mouse button is down.
    /// </summary>
    protected bool rightButtonDown = false;

    protected override void OnMouseDown ( MouseEventArgs e )
    {
      base.OnMouseDown( e );
      mouseDownPoint = e.Location;
      if ( e.Button == MouseButtons.Right )
        rightButtonDown = true;
    }

    protected override void OnMouseUp ( MouseEventArgs e )
    {
      base.OnMouseUp( e );

      if ( input == null )
        return;

      if ( e.Button == MouseButtons.Right )
        rightButtonDown = false;

      if ( e.Location == mouseDownPoint )
        return;

      UseWaitCursor = true;

      // choose action accodring to the mouse button
      if ( e.Button == MouseButtons.Left )
      {
        // !!! TODO: add new feature?
        Bitmap bmp = (Bitmap)Image;
        bmp.SetPixel( mouseDownPoint.X, mouseDownPoint.Y, featuresColor );
        bmp.SetPixel( e.Location.X, e.Location.Y, featuresColor );
        Invalidate( new Rectangle( Math.Min( mouseDownPoint.X, e.Location.X ),
                                   Math.Min( mouseDownPoint.Y, e.Location.Y ),
                                   Math.Abs( mouseDownPoint.X - e.Location.X ) + 1,
                                   Math.Abs( mouseDownPoint.Y - e.Location.Y ) + 1 ) );
        UseWaitCursor = false;
        return;
      }

      if ( e.Button == MouseButtons.Right )
      {
        // !!! TODO: shift the last feature?
        Invalidate( new Rectangle( Math.Min( mouseDownPoint.X, e.Location.X ),
                                   Math.Min( mouseDownPoint.Y, e.Location.Y ),
                                   Math.Abs( mouseDownPoint.X - e.Location.X ) + 1,
                                   Math.Abs( mouseDownPoint.Y - e.Location.Y ) + 1 ) );
      }
      UseWaitCursor = false;
   }

    protected override void OnMouseMove ( MouseEventArgs e )
    {
      base.OnMouseMove( e );

      if ( rightButtonDown )
        return;

      // !!! TODO: active contour?
      UseWaitCursor = true;
      Invalidate( new Rectangle( Math.Min( mouseDownPoint.X, e.Location.X ),
                                 Math.Min( mouseDownPoint.Y, e.Location.Y ),
                                 Math.Abs( mouseDownPoint.X - e.Location.X ) + 1,
                                 Math.Abs( mouseDownPoint.Y - e.Location.Y ) + 1 ) );
      UseWaitCursor = false;
    }

    #endregion

    #region keyboard events

    protected override void OnKeyDown ( KeyEventArgs e )
    {
      base.OnKeyDown( e );
      KeyPressed( e.KeyCode );
    }

    public void KeyPressed ( Keys key )
    {
      if ( key == Keys.Back )
      {
        // !!! TODO: remove the last feature?
      }

      if ( key == Keys.Delete )
      {
        // !!! TODO: remove the current feature?
      }
    }

    #endregion
  }
}
