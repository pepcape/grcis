// Author: Josef Pelikan

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing.Imaging;
using Raster;
using MathSupport;

namespace _085segmentation
{
  public class Segmentation
  {
    /// <summary>
    /// Image mask - input for segmentation algorithm.
    /// </summary>
    protected Bitmap mask = null;

    public const int COL_BACKGROUND = 0;

    public const int COL_EXTERIOR = 1;

    public const int COL_INTERIOR = 2;

    public const int COL_OTHER = 3;

    /// <summary>
    /// Initialize the segmented image.
    /// </summary>
    /// <returns>Initial target image.</returns>
    public Bitmap InitImage ( Bitmap sourceImage )
    {
      Debug.Assert( sourceImage != null );

      if ( mask != null )
        mask.Dispose();

      mask = new Bitmap( sourceImage.Width, sourceImage.Height, PixelFormat.Format8bppIndexed );

      // set the palette (transparent background, blue surround, red object):
      ColorPalette pal = mask.Palette;
      pal.Entries[ COL_BACKGROUND ] = Color.FromArgb( 0, 0, 0, 0 );
      pal.Entries[ COL_EXTERIOR ]   = Color.Blue;
      pal.Entries[ COL_INTERIOR ]   = Color.Red;

      for ( int i = COL_OTHER; i < 256; )
        pal.Entries[ i++ ] = Color.Black;

      mask.Palette = pal;

      // initial target image:
      Bitmap target = new Bitmap( sourceImage.Width, sourceImage.Height, PixelFormat.Format1bppIndexed );

      // set the B/W palette (0 .. white, 1 .. black):
      pal = target.Palette;
      pal.Entries[ 0 ] = Color.White;
      pal.Entries[ 1 ] = Color.Black;
      target.Palette = pal;

      return target;
    }

    /// <summary>
    /// Draws overlay over the given source image.
    /// </summary>
    /// <param name="g">Graphic context to draw to.</param>
    public void DrawOverlay ( Graphics g )
    {
      Debug.Assert( g != null );
      Debug.Assert( mask != null );

      g.DrawImageUnscaled( mask, 0, 0 );
    }

    /// <summary>
    /// Segments the given source image using the 'clasImage' mask.
    /// </summary>
    /// <param name="sourceImage">Source raster image.</param>
    /// <returns>Target raster image.</returns>
    public Bitmap DoSegmentation ( Bitmap sourceImage )
    {
      Debug.Assert( sourceImage != null );
      Debug.Assert( mask != null );

      int wid = sourceImage.Width;
      int hei = sourceImage.Height;
      Debug.Assert( mask.Width == wid && mask.Height == hei );
      Bitmap target = new Bitmap( wid, hei, PixelFormat.Format1bppIndexed );

      // set the B/W palette (0 .. white, 1 .. black):
      ColorPalette pal = target.Palette;
      pal.Entries[ 0 ] = Color.White;
      pal.Entries[ 1 ] = Color.Black;
      target.Palette = pal;

      // !!!{{ TODO: put your image-segmentation code here

      int x, y;

      PixelFormat iFormat = sourceImage.PixelFormat;
      if ( !PixelFormat.Format24bppRgb.Equals( iFormat ) &&
           !PixelFormat.Format32bppArgb.Equals( iFormat ) &&
           !PixelFormat.Format32bppPArgb.Equals( iFormat ) &&
           !PixelFormat.Format32bppRgb.Equals( iFormat ) )
        iFormat = PixelFormat.Format24bppRgb;

      // Segmentation (fast memory-mapped code):
      BitmapData dataIn  = sourceImage.LockBits( new Rectangle( 0, 0, wid, hei ), ImageLockMode.ReadOnly, iFormat );
      BitmapData dataOut = target.LockBits( new Rectangle( 0, 0, wid, hei ), ImageLockMode.WriteOnly, PixelFormat.Format1bppIndexed );
      unsafe
      {
        byte* optr, iptr;
        int buffer;   // bit buffer (8 pixels in one byte)
        int dI = Image.GetPixelFormatSize( iFormat ) / 8;

        for ( y = 0; y < hei; y++ )
        {
          iptr = (byte*)dataIn.Scan0  + y * dataIn.Stride;
          optr = (byte*)dataOut.Scan0 + y * dataOut.Stride;
          buffer = 0;

          for ( x = 0; x < wid; iptr += dI )
          {
            int gray = Draw.RgbToGray( iptr[ 2 ], iptr[ 1 ], iptr[ 0 ] );

            buffer += buffer;
            if ( gray < 128 ) buffer++;

            if ( (++x & 7) == 0 )
            {
              *optr++ = (byte)buffer;
              buffer = 0;
            }
          }

          // finish the last byte of the scanline:
          if ( (x & 7) != 0 )
          {
            while ( (x++ & 7) != 0 )
              buffer += buffer;
            *optr = (byte)buffer;
          }
        }
      }
      target.UnlockBits( dataOut );
      sourceImage.UnlockBits( dataIn );

      // !!!}}

      return target;
    }

    const int RADIUS = 3;

    public void DrawTrace ( int x, int y, bool interior )
    {
      Debug.Assert( mask != null );

      int wid = mask.Width;
      int hei = mask.Height;
      int xmin = Arith.Clamp( x - RADIUS, 0, wid - 1 );
      int xmax = Arith.Clamp( x + RADIUS, 0, wid - 1 );
      int ymin = Arith.Clamp( y - RADIUS, 0, hei - 1 );
      int ymax = Arith.Clamp( y + RADIUS, 0, hei - 1 );
      if ( xmin > xmax ||
           ymin > ymax )
        return;

      int xi, yi;
      byte col = (byte)(interior ? COL_INTERIOR : COL_EXTERIOR);

      BitmapData dataOut = mask.LockBits( new Rectangle( 0, 0, wid, hei ), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed );
      unsafe
      {
        byte* optr;
        int dO = Image.GetPixelFormatSize( PixelFormat.Format8bppIndexed ) / 8;

        for ( yi = ymin; yi <= ymax; yi++ )
        {
          optr = (byte*)dataOut.Scan0 + yi * dataOut.Stride + xmin * dO;
          for ( xi = xmin; xi++ <= xmax; optr += dO )
            optr[ 0 ] = col;
        }
      }
      mask.UnlockBits( dataOut );
    }

    public bool MouseDown ( int x, int y, MouseButtons button )
    {
      if ( mask == null )
        return false;

      DrawTrace( x, y, button == MouseButtons.Left );
      return true;
    }

    public bool MouseUp ( int x, int y, MouseButtons button )
    {
      if ( mask == null )
        return false;

      return false;
    }

    public bool MouseMove ( int x, int y, MouseButtons button )
    {
      if ( mask == null ||
           button == MouseButtons.None )
        return false;

      DrawTrace( x, y, button == MouseButtons.Left );
      return true;
    }

    /// <summary>
    /// Optional keystroke handling function.
    /// </summary>
    public bool KeyPressed ( Keys key )
    {
      if ( key == Keys.Back )
      {
        // !!! TODO: reset the mask, undo.. ???
      }

      return false;
    }
  }
}
