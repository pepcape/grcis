using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;

namespace Raster
{
  public class FloatImage
  {
    /// <summary>
    /// Image data, with optional border.
    /// </summary>
    protected float[] data;

    protected int width;

    public int Width
    {
      get
      {
        return width;
      }
    }

    protected int height;

    public int Height
    {
      get
      {
        return height;
      }
    }

    protected int border;

    protected int stride;

    protected int origin;

    public FloatImage ( int wid, int hei, int bor =0 )
    {
      border = Math.Max( bor, 0 );
      width = wid;
      height = hei;
      if ( border > Math.Min( width, height ) )
        border = Math.Min( width, height );
      stride = width + 2 * border;
      origin = border * (stride + 1);
      int alloc = stride * (height + 2 * border);
      data = new float[ alloc ];
    }

    /// <summary>
    /// Sets the image border.
    /// </summary>
    /// <param name="type">Border type, ignored (only mirror border is implemented).</param>
    public void ComputeBorder ( int type =0 )
    {
      if ( border == 0 )
        return;

      unsafe
      {
        fixed ( float *ptr = data )
        {
          // upper border:
          float* inside = ptr + origin;
          float* outside = inside - stride;
          float* pi;
          float* po;
          int b, i;
          for ( b = 0; b < border; b++ )
          {
            inside += stride;
            for ( i = 0, pi = inside, po = outside; i < width; i++ )
              *po++ = *pi++;
            outside -= stride;
          }

          // lower border:
          inside = ptr + origin + stride * (height - 1);
          outside = inside + stride;
          for ( b = 0; b < border; b++ )
          {
            inside -= stride;
            for ( i = 0, pi = inside, po = outside; i < width; i++ )
              *po++ = *pi++;
            outside += stride;
          }

          // left border:
          inside = ptr + border;
          outside = inside - 1;
          for ( b = 0; b < border; b++ )
          {
            inside++;
            for ( i = height + 2 * border, pi = inside, po = outside; i-- > 0; )
            {
              *po = *pi;
              po += stride;
              pi += stride;
            }
            outside--;
          }

          // right border:
          outside = ptr + border + width;
          inside = outside - 1;
          for ( b = 0; b < border; b++ )
          {
            inside--;
            for ( i = height + 2 * border, pi = inside, po = outside; i-- > 0; )
            {
              *po = *pi;
              po += stride;
              pi += stride;
            }
            outside++;
          }
        }
      }
    }

    /// <summary>
    /// Mean Absolute Difference of two images.
    /// </summary>
    /// <param name="b">The other image.</param>
    /// <returns>Sum of absolute pixel differences divided by number of pixels.</returns>
    public float MAD ( FloatImage b )
    {
      if ( b.Width  != Width ||
           b.Height != Height )
        return 2.0f;

      double sum = 0.0;
      unsafe
      {
        fixed ( float* ad = data )
        fixed ( float* bd = b.data )
        {
          float* la = ad + origin;
          float* lb = bd + b.origin;
          int i, j;
          for ( j = 0; j++ < height; la += stride, lb += b.stride )
          {
            float* pa = la;
            float* pb = lb;
            for ( i = 0; i++ < width;  )
              sum += Math.Abs( *pa++ - *pb++ );
          }
        }
      }
      return( (float)(sum / (width * height)) );
    }

    /// <summary>
    /// Uniform image blur.
    /// </summary>
    public void Blur ()
    {
      if ( border < 1 )
        return;

      float[] ndata = new float[ data.Length ];
      unsafe
      {
        fixed ( float* ad = data )
        fixed ( float* nd = ndata )
        {
          float* la = ad + origin;
          float* ln = nd + origin;
          int i, j;
          for ( j = 0; j++ < height; la += stride, ln += stride )
          {
            float* pa = la;
            float* pn = ln;
            for ( i = 0; i++ < width; pa++ )
            {
              float sum  = pa[ -stride - 1 ] + pa[ -stride ] + pa[ -stride + 1 ] +
                           pa[ -1 ]          + pa[ 0 ]       + pa[ 1 ] +
                           pa[ stride - 1 ]  + pa[ stride ]  + pa[ stride + 1 ];
              *pn++ = sum / 9.0f;
            }
          }
        }
      }

      data = ndata;
      ComputeBorder();
    }

    /// <summary>
    /// Decomposition of the input Bitmap into three band FloatImage-s.
    /// </summary>
    /// <param name="bmp">Input image.</param>
    /// <param name="border">Burder size in pixels.</param>
    /// <param name="red">Output red component.</param>
    /// <param name="green">Output green component.</param>
    /// <param name="blue">Output blue component.</param>
    public static void BitmapBands ( Bitmap bmp, int border, out FloatImage red, out FloatImage green, out FloatImage blue )
    {
      int wid = bmp.Width;
      int hei = bmp.Height;
      red   = new FloatImage( wid, hei, border );
      green = new FloatImage( wid, hei, border );
      blue  = new FloatImage( wid, hei, border );
      const float inv255 = 1.0f / 255.0f;

      unsafe
      {
        fixed ( float *rd = red.data )
        fixed ( float *gd = green.data )
        fixed ( float *bd = blue.data )
        {
          float *lr = rd + red.origin;
          float *lg = gd + green.origin;
          float *lb = bd + blue.origin;
          int i, j;
          for ( j = 0; j < hei; j++, lr += red.stride, lg += green.stride, lb += blue.stride )
          {
            float* pr = lr;
            float* pg = lg;
            float* pb = lb;
            for ( i = 0; i < wid; i++ )
            {
              Color c = bmp.GetPixel( i, j );
              *pr++ = c.R * inv255;
              *pg++ = c.G * inv255;
              *pb++ = c.B * inv255;
            }
          }
        }
      }

      if ( border > 0 )
      {
        red.ComputeBorder();
        green.ComputeBorder();
        blue.ComputeBorder();
      }
    }
  }
}
