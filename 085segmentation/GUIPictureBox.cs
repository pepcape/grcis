// Author: Josef Pelikan

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace _085segmentation
{
  [Designer( "System.Windows.Forms.Design.PictureBoxDesigner, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b0314f7f12d50a3a" )]
  public partial class GUIPictureBox : PictureBox
  {
    #region Data

    /// <summary>
    /// Associated form.
    /// </summary>
    private Form1 form;

    /// <summary>
    /// Associated segmentation object.
    /// </summary>
    public Segmentation segm;

    #endregion

    #region Picture getters/setters

    /// <summary>
    /// Inits a new original picture.
    /// </summary>
    /// <param name="newOriginal">New original picture</param>
    /// <returns>Initial result image.</returns>
    public Bitmap InitPicture ( Form1 f, Bitmap newOriginal )
    {
      form = f;
      Image = newOriginal;

      segm = new Segmentation();
      Bitmap result = segm.InitImage( newOriginal );
      Invalidate();

      return result;
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
           segm == null )
        return;

      // draw custom overlay:
      segm.DrawOverlay( e.Graphics );
    }

    #endregion

    #region Mouse events

    protected override void OnMouseDown ( MouseEventArgs e )
    {
      base.OnMouseDown( e );
      if ( segm != null &&
           segm.MouseDown( e.X, e.Y, e.Button ) )
        Invalidate();

      Focus();
    }

    protected override void OnMouseUp ( MouseEventArgs e )
    {
      base.OnMouseUp( e );

      if ( segm != null &&
           segm.MouseUp( e.X, e.Y, e.Button ) )
        Invalidate();
   }

    protected override void OnMouseMove ( MouseEventArgs e )
    {
      base.OnMouseMove( e );

      if ( segm != null &&
           segm.MouseMove( e.X, e.Y, e.Button ) )
        Invalidate();
    }

    #endregion

    #region Keyboard events

    protected override void OnKeyDown ( KeyEventArgs e )
    {
      base.OnKeyDown( e );

      if ( segm != null &&
           segm.KeyPressed( e.KeyCode ) )
        Invalidate();
    }

    #endregion
  }
}
