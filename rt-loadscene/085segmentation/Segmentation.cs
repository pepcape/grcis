// Author: Josef Pelikan

using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using MathSupport;
using Raster;

namespace _085segmentation
{
  public class Segmentation
  {
    /// <summary>
    /// Image mask - input for segmentation algorithm.
    /// </summary>
    protected Bitmap mask = null;

    /// <summary>
    /// Transparent color index.
    /// </summary>
    public const int COL_BACKGROUND = 0;

    /// <summary>
    /// Exterior's color index.
    /// </summary>
    public const int COL_EXTERIOR = 1;


    /// <summary>
    /// Interior's color index.
    /// </summary>
    public const int COL_INTERIOR = 2;

    /// <summary>
    /// First unused color index.
    /// </summary>
    public const int COL_OTHER = 3;

    /// <summary>
    /// Get/set of the image mask.
    /// </summary>
    public Bitmap Mask
    {
      get
      {
        return mask;
      }

      set
      {
        if ( value.Width  == wid &&
             value.Height == hei )
        {
          InitMask();

          int depth = Image.GetPixelFormatSize( PixelFormat.Format8bppIndexed ) / 8;
          int x, y;
          BitmapData dataOut = mask.LockBits( new Rectangle( 0, 0, wid, hei ), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed );
          unsafe
          {
            for ( y = 0; y < hei; y++ )
            {
              byte* mptr = (byte*)dataOut.Scan0 + y * dataOut.Stride;

              for ( x = 0; x < wid; x++, mptr += depth )
              {
                Color col = value.GetPixel( x, y );
                if ( col.R > 128 )
                  *mptr = COL_INTERIOR;
                else
                  if ( col.B > 128 )
                    *mptr = COL_EXTERIOR;
                  else
                    *mptr = COL_BACKGROUND;
              }
            }
          }
          mask.UnlockBits( dataOut );
        }
      }
    }

    /// <summary>
    /// Working image width in pixels.
    /// </summary>
    int wid = 0;

    /// <summary>
    /// Working image height in pixels.
    /// </summary>
    int hei = 0;

    /// <summary>
    /// X-position of last mouse position with button down.
    /// </summary>
    int lastx = -1;

    /// <summary>
    /// Y-position of last mouse position with button down.
    /// </summary>
    int lasty = -1;

    /// <summary>
    /// Trace diameter in pixels.
    /// </summary>
    double traceSize = 4.0;

    /// <summary>
    /// Get/set current trace-size.
    /// </summary>
    public double TraceSize
    {
      get
      {
        return traceSize;
      }
      set
      {
        traceSize = Arith.Clamp( value, 1.0, 5.0 );
      }
    }

    /// <summary>
    /// [Re-]initialize the input mask.
    /// </summary>
    void InitMask ()
    {
      if ( mask != null )
        mask.Dispose();

      mask = new Bitmap( wid, hei, PixelFormat.Format8bppIndexed );

      // set the palette (transparent background, blue surround, red object):
      ColorPalette pal = mask.Palette;
      pal.Entries[ COL_BACKGROUND ] = Color.FromArgb( 0, 0, 0, 0 );
      pal.Entries[ COL_EXTERIOR ]   = Color.Blue;
      pal.Entries[ COL_INTERIOR ]   = Color.Red;

      for ( int i = COL_OTHER; i < 256; )
        pal.Entries[ i++ ] = Color.Black;

      mask.Palette = pal;
    }

    /// <summary>
    /// Initialize the segmented image.
    /// </summary>
    /// <returns>Initial target image.</returns>
    public Bitmap InitImage ( Bitmap sourceImage )
    {
      Debug.Assert( sourceImage != null );

      wid = sourceImage.Width;
      hei = sourceImage.Height;

      InitMask();

      // initial target image:
      Bitmap target = new Bitmap( wid, hei, PixelFormat.Format1bppIndexed );

      // set the B/W palette (0 .. white, 1 .. black):
      ColorPalette pal = target.Palette;
      pal.Entries[ 0 ] = Color.White;
      pal.Entries[ 1 ] = Color.Black;
      target.Palette = pal;

      lastx =
      lasty = -1;

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

    const int MAX_RADIUS = 2;

    /// <summary>
    /// Draw one trace point (bullet-shaped).
    /// </summary>
    /// <param name="interior">True for object interior, false for exterior.</param>
    public void DrawTrace ( int x, int y, bool interior )
    {
      Debug.Assert( mask != null );

      int xmin = Arith.Clamp( x - MAX_RADIUS, 0, wid - 1 );
      int xmax = Arith.Clamp( x + MAX_RADIUS + 1, 0, wid );
      int ymin = Arith.Clamp( y - MAX_RADIUS, 0, hei - 1 );
      int ymax = Arith.Clamp( y + MAX_RADIUS + 1, 0, hei );
      if ( xmin >= xmax ||
           ymin >= ymax )
        return;

      byte col = (byte)(interior ? COL_INTERIOR : COL_EXTERIOR);
      int wWid = xmax - xmin;
      int wHei = ymax - ymin;
      int x0 = x - xmin;
      int y0 = y - ymin;
      int depth = Image.GetPixelFormatSize( PixelFormat.Format8bppIndexed ) / 8;

      BitmapData dataOut = mask.LockBits( new Rectangle( xmin, ymin, wWid, wHei ), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed );
      unsafe
      {
        // SetPixel( x0, y0, col ):
        if ( x0 >= 0 && x0 < wWid &&
             y0 >= 0 && y0 < wHei )
          *((byte*)dataOut.Scan0 + y0 * dataOut.Stride + x0 * depth) = col;

        if ( traceSize > 1.0 )
          for ( int pi = 1; pi < Draw.squares.Length && Draw.squares[ pi ] <= traceSize; pi++ )
          {
            int pix = x0 + Draw.penPixels[ pi ].Item1;
            int piy = y0 + Draw.penPixels[ pi ].Item2;
            if ( pix < 0 || pix >= wWid ||
                 piy < 0 || piy >= wHei )
              continue;

            // SetPixel( pix, piy, col ):
            *((byte*)dataOut.Scan0 + piy * dataOut.Stride + pix * depth) = col;
          }
      }
      mask.UnlockBits( dataOut );
    }

    public bool MouseDown ( int x, int y, MouseButtons button )
    {
      if ( mask == null )
        return false;

      DrawTrace( x, y, button == MouseButtons.Left );
      lastx = x;
      lasty = y;

      return true;
    }

    public bool MouseUp ( int x, int y, MouseButtons button )
    {
      lastx = lasty = -1;
      if ( mask == null )
        return false;

      return false;
    }

    public bool MouseMove ( int x, int y, MouseButtons button )
    {
      if ( mask == null ||
           button == MouseButtons.None ||
           lastx < 0 )
      {
        lastx = lasty = -1;
        return false;
      }

      bool interior = (button == MouseButtons.Left);
      DrawTrace( x, y, interior );

      int maxd = Math.Max( Math.Abs( lastx - x ), Math.Abs( lasty - y ) );
      int maxdh = maxd / 2;
      if ( lastx >= 0 &&
           maxd > 1 )
        for ( int i = 1; i < maxd; i++ )
          DrawTrace( (i * x + (maxd - i) * lastx + maxdh) / maxd, (i * y + (maxd - i) * lasty + maxdh) / maxd, interior );

      lastx = x;
      lasty = y;

      return true;
    }

    /// <summary>
    /// Optional keystroke handling function.
    /// </summary>
    public bool KeyPressed ( Keys key )
    {
      if ( key == Keys.Back )
      {
        // reset the mask as an example:
        InitMask();
        return true;
      }

      return false;
    }

    /// <summary>
    /// Segments the given source image using the 'clasImage' mask.
    /// </summary>
    /// <param name="sourceImage">Source raster image.</param>
    /// <param name="whiteExterior">True for white exterior (color=0) of the output image.</param>
    /// <returns>Target raster image.</returns>
    public Bitmap DoSegmentation ( Bitmap sourceImage, bool whiteExterior =true )
    {
      Debug.Assert( sourceImage != null );
      Debug.Assert( sourceImage.Width == wid && sourceImage.Height == hei );
      Debug.Assert( mask != null );
      Debug.Assert( mask.Width == wid && mask.Height == hei );

      Bitmap target = new Bitmap( wid, hei, PixelFormat.Format1bppIndexed );

      // set the B/W palette (0 .. white/exterior, 1 .. black/interior):
      ColorPalette pal = target.Palette;
      pal.Entries[ whiteExterior ? 0 : 1 ] = Color.White;
      pal.Entries[ whiteExterior ? 1 : 0 ] = Color.Black;
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
      BitmapData dataIn = sourceImage.LockBits( new Rectangle( 0, 0, wid, hei ), ImageLockMode.ReadOnly, iFormat );
      BitmapData dataOut = target.LockBits( new Rectangle( 0, 0, wid, hei ), ImageLockMode.WriteOnly, PixelFormat.Format1bppIndexed );
      unsafe
      {
        byte* optr, iptr;
        int buffer;   // bit buffer (8 pixels in one byte)
        int dI = Image.GetPixelFormatSize( iFormat ) / 8;

        for ( y = 0; y < hei; y++ )
        {
          iptr = (byte*)dataIn.Scan0 + y * dataIn.Stride;
          optr = (byte*)dataOut.Scan0 + y * dataOut.Stride;
          buffer = 0;

          for ( x = 0; x < wid; iptr += dI )
          {
            int gray = Draw.RgbToGray( iptr[ 2 ], iptr[ 1 ], iptr[ 0 ] );
            bool InMask = mask.GetPixel( x, y ).R > 0;

            // set/clear one bit (output pixel):
            buffer += buffer;
            if ( gray < 128 || InMask )
              buffer++;

            // check the output buffer:
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
  }
}
