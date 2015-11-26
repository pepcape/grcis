// Author: Josef Pelikan

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace _080warping
{
  [Designer( "System.Windows.Forms.Design.PictureBoxDesigner, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f12d50a3a" )]
  public partial class GUIPictureBox : PictureBox
  {
    #region Data

    /// <summary>
    /// Associated form.
    /// </summary>
    private Form1 form;

    /// <summary>
    /// Associated warping object (triangle mesh).
    /// </summary>
    public Warping warp;

    protected float width;

    protected float height;

    #endregion

    #region Picture getters/setters

    /// <summary>
    /// Inits a new original picture.
    /// </summary>
    /// <param name="newOriginal">New original picture</param>
    public void InitPicture ( Form1 f, Bitmap newOriginal, int columns, int rows )
    {
      form = f;
      Image = (Bitmap)newOriginal.Clone();
      width = Image.Width;
      height = Image.Height;

      warp = new Warping();
      warp.GenerateTriangleMesh( columns, rows );
      Invalidate();
    }

    public void SetPicture ( Bitmap newImage )
    {
      Image = newImage;
    }

    /// <summary>
    /// Gets the current picture.
    /// </summary>
    public Bitmap GetPicture ()
    {
      return (Bitmap)Image;
    }

    #endregion

    #region Display results

    protected override void OnPaint ( PaintEventArgs e )
    {
      base.OnPaint( e );
      if ( Image == null ||
           warp == null )
        return;

      // tri-mesh drawing:
      warp.DrawGrid( e.Graphics, width, height );
    }

    #endregion

    #region Mouse events

    /// <summary>
    /// Stores position of mouse when button was pressed.
    /// </summary>
    protected int mouseDownIndex = -1;

    protected override void OnMouseDown ( MouseEventArgs e )
    {
      base.OnMouseDown( e );
      if ( warp != null )
        mouseDownIndex = warp.NearestVertex( e.Location, width, height );

      Focus();
    }

    protected override void OnMouseUp ( MouseEventArgs e )
    {
      base.OnMouseUp( e );

      if ( mouseDownIndex < 0 ||
           warp == null )
        return;

      warp.MoveVertex( mouseDownIndex, e.Location, width, height );
      mouseDownIndex = -1;

      form.Recompute();
   }

    protected override void OnMouseMove ( MouseEventArgs e )
    {
      base.OnMouseMove( e );

      if ( mouseDownIndex < 0 ||
           warp == null )
        return;

      warp.MoveVertex( mouseDownIndex, e.Location, width, height );
      Invalidate();
    }

    #endregion

    #region Keyboard events

    protected override void OnKeyDown ( KeyEventArgs e )
    {
      base.OnKeyDown( e );

      if ( warp != null &&
           warp.KeyPressed( e.KeyCode ) )
        form.Recompute();
    }

    #endregion
  }
}
