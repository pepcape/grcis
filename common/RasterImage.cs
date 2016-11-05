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

    protected int channels;

    public int Channels
    {
      get
      {
        return channels;
      }
    }

    protected int border;

    /// <summary>
    /// Array stride in floats (indexes).
    /// </summary>
    protected int stride;

    protected int widChannels;

    /// <summary>
    /// Origin of the image itself (skipping over the optional borders).
    /// </summary>
    protected int origin;

    protected void init ( int wid, int hei, int ch = 1, int bor =0 )
    {
      border = Math.Max( bor, 0 );
      width = wid;
      height = hei;
      channels = ch;
      widChannels = width * channels;
      if ( border > Math.Min( width, height ) )
        border = Math.Min( width, height );
      stride = channels * (width + 2 * border);
      origin = border * (stride + channels);
      int alloc = stride * (height + 2 * border);
      data = new float[ alloc ];
    }

    public FloatImage ( int wid, int hei, int ch =1, int bor =0 )
    {
      init( wid, hei, ch, bor );
    }

    public FloatImage ( Bitmap bmp, int bor =0 )
    {
      int ch = 3;
      if ( bmp.PixelFormat == PixelFormat.Format16bppGrayScale )
        ch = 1;

      int wid = bmp.Width;
      int hei = bmp.Height;
      init( wid, hei, ch, bor );

      const float inv255 = 1.0f / 255.0f;
      int i, j;
      unsafe
      {
        fixed ( float* dt = data )
        {
          float* ptr = dt + origin;
          if ( ch == 1 )
            for ( j = 0; j < hei; j++, ptr += stride )
            {
              float* p = ptr;
              for ( i = 0; i < wid; i++ )
              {
                Color c = bmp.GetPixel( i, j );
                *p++ = c.R * inv255;
              }
            }
          else
            for ( j = 0; j < hei; j++, ptr += stride )
            {
              float* p = ptr;
              for ( i = 0; i < wid; i++ )
              {
                Color c = bmp.GetPixel( i, j );
                *p++ = c.R * inv255;
                *p++ = c.G * inv255;
                *p++ = c.B * inv255;
              }
            }
        }
      }

      if ( border > 0 )
        ComputeBorder();
    }

    /// <summary>
    /// Sets the image border.
    /// </summary>
    /// <param name="type">Border type, ignored (only mirror border is implemented).</param>
    protected void ComputeBorder ( int type =0 )
    {
      if ( border == 0 )
        return;

      int j;
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
            for ( i = 0, pi = inside, po = outside; i < widChannels; i++ )
              *po++ = *pi++;
            outside -= stride;
          }

          // lower border:
          inside = ptr + origin + stride * (height - 1);
          outside = inside + stride;
          for ( b = 0; b < border; b++ )
          {
            inside -= stride;
            for ( i = 0, pi = inside, po = outside; i < widChannels; i++ )
              *po++ = *pi++;
            outside += stride;
          }

          // left border:
          inside = ptr + border * channels;
          outside = inside - channels;
          for ( b = 0; b < border; b++ )
          {
            inside += channels;
            for ( i = height + 2 * border, pi = inside, po = outside; i-- > 0; )
            {
              for ( j = 0; j++ < channels; )
                *po++ = *pi++;
              po += stride - channels;
              pi += stride - channels;
            }
            outside -= channels;
          }

          // right border:
          outside = ptr + (border + width) * channels;
          inside = outside - channels;
          for ( b = 0; b < border; b++ )
          {
            inside -= channels;
            for ( i = height + 2 * border, pi = inside, po = outside; i-- > 0; )
            {
              for ( j = 0; j++ < channels; )
                *po++ = *pi++;
              po += stride - channels;
              pi += stride - channels;
            }
            outside += channels;
          }
        }
      }
    }

    public bool GetPixel ( int x, int y, float[] pix )
    {
      if ( x < 0 || x >= width  ||
           y < 0 || y >= height ||
           pix == null ||
           pix.Length < channels )
        return false;

      Buffer.BlockCopy( data, origin + x * channels + y * stride, pix, 0, channels );
      return true;
    }

    public float GetGray ( int x, int y )
    {
      if ( x < 0 || x >= width ||
           y < 0 || y >= height )
        return 0.0f;

      int i = origin + x * channels + y * stride;
      if ( channels >= 3 )
        return (data[ i ] * Draw.RED_WEIGHT + data[ i + 1 ] * Draw.GREEN_WEIGHT + data[ i + 2 ] * Draw.BLUE_WEIGHT) / Draw.WEIGHT_TOTAL;

      return data[ i ];
    }

    public void PutPixel ( int x, int y, float[] pix )
    {
      if ( x < 0 || x >= width ||
           y < 0 || y >= height ||
           pix == null ||
           pix.Length < channels )
        return;

      Buffer.BlockCopy( pix, 0, data, origin + x * channels + y * stride, channels );
    }

    public void PutPixel ( int x, int y, float val )
    {
      if ( x < 0 || x >= width ||
           y < 0 || y >= height )
        return;

      int i = origin + x * channels + y * stride;
      for ( int ch = 0; ch++ < channels; )
        data[ i++ ] = val;
    }

    /// <summary>
    /// Mean Absolute Difference of two images.
    /// </summary>
    /// <param name="b">The other image.</param>
    /// <returns>Sum of absolute pixel differences divided by number of pixels.</returns>
    public float MAD ( FloatImage b )
    {
      if ( b.Width  != Width ||
           b.Height != Height ||
           b.Channels != Channels )
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
            for ( i = 0; i++ < widChannels;  )
              sum += Math.Abs( *pa++ - *pb++ );
          }
        }
      }
      return( (float)(sum / (height * widChannels)) );
    }

    /// <summary>
    /// Mean Absolute Difference of two images, one channel only.
    /// </summary>
    /// <param name="b">The other image.</param>
    /// <param name="ch">Channel number.</param>
    /// <returns>Sum of absolute pixel differences divided by number of pixels.</returns>
    public float MAD ( FloatImage b, int ch )
    {
      if ( b.Width  != Width ||
           b.Height != Height ||
           ch >= Channels ||
           ch >= b.Channels )
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
            for ( i = 0; i++ < width; pa += channels, pb += channels )
              sum += Math.Abs( pa[ ch ] - pb[ ch ] );
          }
        }
      }
      return ((float)(sum / (height * width)));
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
            for ( i = 0; i++ < widChannels; pa++ )
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
  }
}
