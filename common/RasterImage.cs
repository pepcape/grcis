using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using MathSupport;

namespace Raster
{
  /// <summary>
  /// Multi-channel float raster image.
  /// Can compute mirrored borders.
  /// </summary>
  public class FloatImage
  {
    /// <summary>
    /// Raw image data with optional border.
    /// </summary>
    protected float[] data;

    /// <summary>
    /// Raw data array (use Scan0 and Stride).
    /// </summary>
    public float[] Data
    {
      get
      {
        return data;
      }
    }

    protected int width;

    /// <summary>
    /// Image width in pixels.
    /// </summary>
    public int Width
    {
      get
      {
        return width;
      }
    }

    protected int height;

    /// <summary>
    /// Image height in pixels.
    /// </summary>
    public int Height
    {
      get
      {
        return height;
      }
    }

    protected int channels;

    /// <summary>
    /// Number of image channels (number of float numbers per pixel).
    /// </summary>
    public int Channels
    {
      get
      {
        return channels;
      }
    }

    /// <summary>
    /// Internal image border allocated around the whole image.
    /// </summary>
    protected int border;

    /// <summary>
    /// Array stride in floats (indices).
    /// </summary>
    protected int stride;

    protected int widChannels;

    /// <summary>
    /// Origin of the image itself (skipping over the optional borders).
    /// </summary>
    protected int origin;

    /// <summary>
    /// Offset of the upper-left image corner from the array origin (in array indices).
    /// </summary>
    public int Scan0
    {
      get
      {
        return origin;
      }
    }

    /// <summary>
    /// Image stride in array indices.
    /// </summary>
    public int Stride
    {
      get
      {
        return stride;
      }
    }

    protected void setAccelerators ()
    {
      stride = channels * (width + 2 * border);
      origin = border   * (stride + channels);
    }

    protected void init ( int wid, int hei, int ch =1, int bor =0, float[] d =null )
    {
      border   = Math.Max( bor, 0 );
      width    = wid;
      height   = hei;
      channels = Math.Max( ch, 1 );

      widChannels = width * channels;
      if ( border > Math.Min( width, height ) )
        border = Math.Min( width, height );

      // Support (accelerator) values:
      setAccelerators();

      // The data array itself:
      data = d ?? new float[ stride * (height + 2 * border) ];
    }

    /// <summary>
    /// Create a HDR image of required dimensions.
    /// </summary>
    /// <param name="wid">Image width in pixels.</param>
    /// <param name="hei">Image height in pixels.</param>
    /// <param name="ch">Number of channels.</param>
    /// <param name="bor">Border width in pixels.</param>
    public FloatImage ( int wid, int hei, int ch =1, int bor =0 )
    {
      init( wid, hei, ch, bor );
    }

    public FloatImage ( FloatImage from )
    {
      if ( from == null )
        init( 1, 1, 1, 0 );
      else
        init( from.Width, from.Height, from.channels, from.border, (float[])from.data.Clone() );
    }

    /// <summary>
    /// Create a HDR image from provided LDR Bitmap.
    /// </summary>
    /// <param name="bmp">Input LDR image.</param>
    /// <param name="bor">Border width in pixels.</param>
    public unsafe FloatImage ( Bitmap bmp, int bor =0 )
    {
      int ch = 3;
      if ( bmp.PixelFormat == PixelFormat.Format16bppGrayScale )
        ch = 1;

      int wid = bmp.Width;
      int hei = bmp.Height;
      init( wid, hei, ch, bor );

      const float inv255 = 1.0f / 255.0f;
      int i, j;
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

      if ( border > 0 )
        ComputeBorder();
    }

    public FloatImage GrayImage ( bool inv =false, double gamma =0.0 )
    {
      FloatImage ni = new FloatImage( width, height, 1, border );
      int i;
      int len = (width + 2 * border) * (height + 2 * border);
      double val;
      unsafe
      {
        fixed ( float* pii = data )
        fixed ( float* poi = ni.data )
        {
          float* pi = pii;
          float* po = poi;
          if ( channels >= 3 )
            for ( i = 0; i++ < len; pi += channels )
            {
              val = (pi[ 0 ] * Draw.RED_WEIGHT + pi[ 1 ] * Draw.GREEN_WEIGHT + pi[ 2 ] * Draw.BLUE_WEIGHT) / Draw.WEIGHT_TOTAL;
              if ( inv ) val = 1.0 - val;
              if ( gamma > 0.0 )
                val = Math.Pow( val, gamma );
              *po++ = (float)val;
            }
          else
            for ( i = 0; i++ < len; pi += channels )
            {
              val = *pi;
              if ( inv ) val = 1.0 - val;
              if ( gamma > 0.0 )
                val = Math.Pow( val, gamma );
              *po++ = (float)val;
            }
        }
      }
      return ni;
    }

    /// <summary>
    /// Sets the required border size (exactly).
    /// Does not compute the border pixels yet!
    /// </summary>
    /// <param name="newBorder">Required border size in pixels. Zero value means 'no border'.</param>
    public void SetBorder ( int newBorder )
    {
      if ( newBorder < 0 ||
           newBorder == border )
        return;

      int newStride = channels * (width + 2 * newBorder);
      int newOrigin = newBorder * (newStride + channels);
      float[] newData = new float[ newStride * (height + 2 * newBorder) ];
      int len = width * channels;

      unsafe
      {
        fixed ( float* pii = data )
        fixed ( float* poi = newData )
        {
          for ( int y = 0; y < width; y++ )
          {
            float* pi = pii + origin    + y * stride;
            float* po = poi + newOrigin + y * newStride;
            int n = len;
            while ( n-- > 0 )
              *po++ = *pi++;
          }
        }
      }

      data   = newData;
      border = newBorder;
      stride = newStride;
      origin = newOrigin;
    }

    /// <summary>
    /// Asserts minimal border value.
    /// Computes actual border pixels.
    /// </summary>
    /// <param name="minBorder">Minimal border size needed.</param>
    public void AssertMinBorder ( int minBorder )
    {
      if ( border >= minBorder )
        return;

      SetBorder( minBorder );
      ComputeBorder();
    }

    /// <summary>
    /// Computes the image border.
    /// </summary>
    /// <param name="type">Border type, ignored (only mirror border is implemented).</param>
    protected unsafe void ComputeBorder ( int type =0 )
    {
      if ( border == 0 )
        return;

      int j;
      fixed ( float* ptr = data )
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

    const int SIZEOF_FLOAT = sizeof( float );

    /// <summary>
    /// Reads one HDR color pixel.
    /// </summary>
    /// <param name="x">X-coordinate.</param>
    /// <param name="y">Y-coordinate.</param>
    /// <param name="pix">Pre-allocated output array.</param>
    /// <returns>True if Ok.</returns>
    public bool GetPixel ( int x, int y, float[] pix )
    {
      if ( x < 0 || x >= width ||
           y < 0 || y >= height ||
           pix == null ||
           pix.Length < channels )
        return false;

      Buffer.BlockCopy( data, SIZEOF_FLOAT * (origin + x * channels + y * stride), pix, 0, SIZEOF_FLOAT * channels );
      return true;
    }

    /// <summary>
    /// Reads one grayscale pixel.
    /// </summary>
    /// <param name="x">X-coordinate.</param>
    /// <param name="y">Y-coordinate.</param>
    /// <returns>Gray value or -1.0f if out of range.</returns>
    public float GetGray ( int x, int y )
    {
      if ( x < 0 || x >= width ||
           y < 0 || y >= height )
        return -1.0f;

      int i = origin + x * channels + y * stride;
      if ( channels >= 3 )
        return (data[ i ] * Draw.RED_WEIGHT + data[ i + 1 ] * Draw.GREEN_WEIGHT + data[ i + 2 ] * Draw.BLUE_WEIGHT) / Draw.WEIGHT_TOTAL;

      return data[ i ];
    }

    /// <summary>
    /// Sets the required pixel to a new value.
    /// </summary>
    /// <param name="x">X-coordinate.</param>
    /// <param name="y">Y-coordinate.</param>
    /// <param name="pix">New pixel value.</param>
    public void PutPixel ( int x, int y, float[] pix )
    {
      if ( x < 0 || x >= width ||
           y < 0 || y >= height ||
           pix == null ||
           pix.Length < channels )
        return;

      Buffer.BlockCopy( pix, 0, data, SIZEOF_FLOAT * (origin + x * channels + y * stride), SIZEOF_FLOAT * channels );
    }

    /// <summary>
    /// Sets the gray-value pixel.
    /// </summary>
    /// <param name="x">X-coordinate.</param>
    /// <param name="y">Y-coordinate.</param>
    /// <param name="val">New pixel value.</param>
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
    public unsafe float MAD ( FloatImage b )
    {
      if ( b.Width != Width ||
           b.Height != Height ||
           b.Channels != Channels )
        return 2.0f;

      double sum = 0.0;
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
          for ( i = 0; i++ < widChannels; )
            sum += Math.Abs( *pa++ - *pb++ );
        }
      }

      return ((float)(sum / (height * widChannels)));
    }

    /// <summary>
    /// Mean Absolute Difference of two images, one channel only.
    /// </summary>
    /// <param name="b">The other image.</param>
    /// <param name="ch">Channel number.</param>
    /// <returns>Sum of absolute pixel differences divided by number of pixels.</returns>
    public unsafe float MAD ( FloatImage b, int ch )
    {
      if ( b.Width != Width ||
           b.Height != Height ||
           ch >= Channels ||
           ch >= b.Channels )
        return 2.0f;

      double sum = 0.0;
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

      return ((float)(sum / (height * width)));
    }

    /// <summary>
    /// Image blur - Gaussian or uniform.
    /// </summary>
    /// <param name="gauss">Use Gaussian filter? (3x3 window)</param>
    public unsafe void Blur ( bool gauss =false )
    {
      if ( border < 1 )
        return;

      float[] ndata = new float[ data.Length ];

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

          if ( gauss )
            for ( i = 0; i++ < widChannels; pa++ )
            {
              float sum =        pa[ -stride - 1 ] + 2.0f * pa[ -stride ] +        pa[ -stride + 1 ] +
                          2.0f * pa[ -1 ]          + 4.0f * pa[ 0 ]       + 2.0f * pa[ 1 ] +
                                 pa[ stride - 1 ]  + 2.0f * pa[ stride ]  +        pa[ stride + 1 ];
              *pn++ = sum / 16.0f;
            }
          else
            for ( i = 0; i++ < widChannels; pa++ )
            {
              float sum = pa[ -stride - 1 ] + pa[ -stride ] + pa[ -stride + 1 ] +
                          pa[ -1 ]          + pa[ 0 ]       + pa[ 1 ] +
                          pa[ stride - 1 ]  + pa[ stride ]  + pa[ stride + 1 ];
              *pn++ = sum / 9.0f;
            }
        }
      }

      data = ndata;
      ComputeBorder();
    }

    /// <summary>
    /// Image resize by an integral subsample factor.
    /// </summary>
    /// <param name="factor">Subsample factor.</param>
    public unsafe void Resize ( int factor )
    {
      if ( factor < 2 )
        return;

      int newWidth  = width / factor;
      int newHeight = height / factor;

      if ( newWidth  < 1 ||
           newHeight < 1 )
        return;

      int newStride      = channels * (newWidth + 2 * border);
      int newOrigin      = border   * (newStride + channels);
      float area = factor * factor;

      // The data array itself:
      float[] ndata = new float[ newStride * (newHeight + 2 * border) ];

      fixed ( float* ad = data )
      fixed ( float* nd = ndata )
      {
        float* ln = nd + newOrigin;
        int i, j, ii, jj, ch;
        for ( j = 0; j < newHeight; j++, ln += newStride )
        {
          float* pn = ln;

          for ( i = 0; i < newWidth; i++ )
            for ( ch = 0; ch < channels; ch++ )
            {
              float* la = ad + origin + factor * (j * stride + i * channels) + ch;

              float sum = 0.0f;
              for ( jj = 0; jj++ < factor; la += stride )
              {
                float* pa = la;
                for ( ii = 0; ii++ < factor; pa += channels )
                  sum += *pa;
              }
              *pn++ = sum / area;
            }
        }
      }

      init( newWidth, newHeight, channels, border, ndata );
      ComputeBorder();
    }

    /// <summary>
    /// Computes image range (min and max pixel values).
    /// </summary>
    /// <param name="minY">Output (nonzero) min value.</param>
    /// <param name="maxY">Output max value.</param>
    public unsafe void Contrast ( out double minY, out double maxY )
    {
      float minf = float.MaxValue;
      float maxf = float.MinValue;

      fixed ( float* id = data )
      {
        float* iptr;

        for ( int y = 0; y < height; y++ )
        {
          iptr = id + origin + y * stride;
          for ( int x = 0; x++ < width; )
          {
            float Y = 0.0f;
            for ( int ch = 0; ch++ < channels; )
              Y = Math.Max( Y, *iptr++ );

            if ( Y > float.Epsilon )
            {
              if ( Y < minf ) minf = Y;
              if ( Y > maxf ) maxf = Y;
            }
          }
        }
      }

      minY = minf;
      maxY = maxf;
    }

    /// <summary>
    /// Cummulative distribution function in discrete form.
    /// Support data structure for efficient sampling.
    /// If allocated, it should have 'image resolution' + 1 values.
    /// </summary>
    protected double[] cdf = null;

    /// <summary>
    /// Prepare support cdf array.
    /// </summary>
    /// <param name="ch">Optional channel index.</param>
    public void PrepareCdf ( int ch =0 )
    {
      ch = Arith.Clamp( ch, 0, Channels - 1 );
      int cdfRes = width * height + 1;
      cdf = new double[ cdfRes ];
      double acc = 0.0;
      int oi = 1;
      for ( int y = 0; y < height; y++ )
      {
        int ii = Scan0 + y * Stride + ch;
        for ( int x = 0; x++ < width; ii += Channels )
        {
          acc += data[ ii ];
          cdf[ oi++ ] = acc;
        }
      }
      double k = 1.0 / acc;
      for ( oi = 1; oi < cdfRes; )
        cdf[ oi++ ] *= k;
    }

    /// <summary>
    /// CDF-based sampling.
    /// </summary>
    /// <param name="x">Output horizontal coordinate from the [0.0,width) range.</param>
    /// <param name="y">Output vertical coordinate from the [0.0,height) range.</param>
    /// <param name="random">[0,1] uniform random value.</param>
    /// <param name="rnd">Optional random generator instance. If provided, internal randomization is possible.</param>
    public void GetSample ( out double x, out double y, double random, RandomJames rnd =null )
    {
      if ( cdf == null )
        PrepareCdf();

      // CDF-based importance sampling:
      int ia = 0;
      int ib = width * height;
      double a = 0.0;
      double b = 1.0;
      do
      {
        int ip = (ia + ib) >> 1;
        double p = cdf[ ip ];
        if ( p < random )
        {
          ia = ip;
          a = p;
        }
        else
        {
          ib = ip;
          b = p;
        }
      }
      while ( ia + 1 < ib );

      y = ia / width;
      x = ia % width;

      if ( rnd != null )
      {
        x += rnd.UniformNumber();
        y += rnd.UniformNumber();
      }
      else
      {
        x += 0.5;
        y += 0.5;
      }
    }

    /// <summary>
    /// Computes simple exposure from HDR to LDR format. Optional gamma pre-compensation
    /// </summary>
    /// <param name="result">Optional pre-allocated Bitmap.</param>
    /// <param name="exp">Exposure coefficient.</param>
    /// <param name="gamma">Optional target gammma (if zero, no pre-compensation is done).</param>
    /// <returns>Output LDR Bitbap.</returns>
    public unsafe Bitmap Exposure ( Bitmap result, double exp, double gamma =0.0 )
    {
      if ( channels < 3 )
        return null;

      if ( result == null ||
           result.Width != width ||
           result.Height != height ||
           result.PixelFormat != PixelFormat.Format24bppRgb )
        result = new Bitmap( width, height, PixelFormat.Format24bppRgb );

      // gamma pre-compensation?
      double g = 0.0;
      if ( gamma > 0.001 &&
           Math.Abs( gamma - 1.0 ) > 0.001 )
        g = 1.0 / gamma;

      BitmapData dataOut = result.LockBits( new Rectangle( 0, 0, width, height ), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb );
      fixed ( float* id = data )
      {
        float* iptr;
        byte* optr;
        int dO = Image.GetPixelFormatSize( PixelFormat.Format24bppRgb ) / 8;    // BGR

        for ( int y = 0; y < height; y++ )
        {
          iptr = id + origin + y * stride;
          optr = (byte*)dataOut.Scan0 + y * dataOut.Stride;
          if ( g > 0.0 )
            for ( int x = 0; x++ < width; iptr += channels, optr += dO )
            {
              optr[ 2 ] = (byte)Arith.Clamp( 255.999 * Math.Pow( iptr[ 0 ] * exp, g ), 0.0, 255.0 );
              optr[ 1 ] = (byte)Arith.Clamp( 255.999 * Math.Pow( iptr[ 1 ] * exp, g ), 0.0, 255.0 );
              optr[ 0 ] = (byte)Arith.Clamp( 255.999 * Math.Pow( iptr[ 2 ] * exp, g ), 0.0, 255.0 );
            }
          else
            for ( int x = 0; x++ < width; iptr += channels, optr += dO )
            {
              optr[ 2 ] = (byte)Arith.Clamp( 255.999 * iptr[ 0 ] * exp, 0.0, 255.0 );
              optr[ 1 ] = (byte)Arith.Clamp( 255.999 * iptr[ 1 ] * exp, 0.0, 255.0 );
              optr[ 0 ] = (byte)Arith.Clamp( 255.999 * iptr[ 2 ] * exp, 0.0, 255.0 );
            }
        }
      }
      result.UnlockBits( dataOut );

      return result;
    }
  }

  /// <summary>
  /// Radiance HDR (PIC) file-format.
  /// Can read/write RLE-encoded HDR files.
  /// </summary>
  public class RadianceHDRFormat
  {
    class HDRInstance
    {
      //==========================================================================
      //  Data:

      /** Binary file header. */
      static readonly byte[] HEADER = new byte[]
      {
        // Radiance header
        (byte)'#', (byte)'?', (byte)'R', (byte)'A',
        (byte)'D', (byte)'I', (byte)'A', (byte)'N',
        (byte)'C', (byte)'E'
      };

      /** RGBe scanline, byte order: <tt>[ Red, Green, Blue, Exp ]</tt>. */
      protected byte[] scanline = null;

      /** Minimum scanline length for encoding. */
      const int MIN_ELEN = 8;

      /** Maximum scanline length for encoding. */
      const int MAX_ELEN = 0x7fff;

      /** Minimum run length. */
      const int MIN_RUN = 4;

      //--------------------------------------------------------------------------
      //  Support routines:

      /** Asserts minimum length of the {@link #scanline} array (in pixels). */
      protected void assertScanline ( int width )
      {
        int len = width << 2;
        if ( scanline != null &&
             scanline.Length >= len ) return;

        scanline = new byte[ len ];
      }

      /**
       * Reads data from the old binary format into the {@link #scanline} array.
       *
       * @param  start Starting index into the {@link #scanline} array (not pixel index!).
       * @param  len Length to read (in pixels).
       * @param  iss Binary stream to read from.
       * @param  unget First byte read before calling this method (or <tt>-1</tt> if none was).
       */
      protected void oldReadScanline ( int start, int len, Stream iss, int unget )
      {
        int repShift = 0;                     // LSB/MSB RLE repeat attribute
        int i;
        int readBunch = (unget < 0) ? 4 : 3;
        if ( unget >= 0 )
          scanline[ start++ ] = (byte)unget;

        while ( len-- > 0 )                   // read one quadruple
        {
          iss.Read( scanline, start, readBunch );
          start += readBunch;

          if ( scanline[ start - 2 ] == 1 &&
               scanline[ start - 3 ] == 1 &&
               scanline[ start - 4 ] == 1 )       // RLE packet
          {
            start -= 4;
            i = scanline[ start + 3 ] << repShift;
            // number of pixels to repeat
            len -= (i - 1);                 // one item was counted before
            i <<= 2;                       // number of bytes

            while ( i-- > 0 )
            {
              scanline[ start ] = scanline[ start - 4 ];
              start++;
            }

            repShift += 8;                    // next RLE packet would contain MSB repeat count
          }

          else                                // regular pixel
            repShift = 0;

          readBunch = 4;
        }
      }

      /** Reads data in new RLE format (band-ordered) into the {@link #scanline} array. */
      protected void readScanline ( int len, Stream iss )
      {
        assertScanline( len );

        if ( len < MIN_ELEN ||
             len > MAX_ELEN )                 // use old scanline format
        {
          oldReadScanline( 0, len, iss, -1 );
          return;
        }

        int b = iss.ReadByte();
        if ( b != 2 )                         // use old scanline format
        {
          oldReadScanline( 0, len, iss, b );
          return;
        }

        // check scanline header - should be [ 2, 2, hi(len), lo(len) ]:
        scanline[ 0 ] = (byte)b;
        iss.Read( scanline, 1, 3 );

        if ( scanline[ 1 ] != 2 ||
             (scanline[ 2 ] & 0x80) > 0 )       // header mismatch => use the old format
        {
          oldReadScanline( 4, len - 1, iss, -1 );
          return;
        }

        int readLen =   (scanline[ 3 ] & 0xff) +
                       ((scanline[ 2 ] & 0x7f) << 8);
        if ( readLen != len )                 // header lenght mismatch => error
          throw new IOException( "Error in HDR scanline" );

        // read regular scanline data:
        int band;                             // band number (0 for Red, .. 3 for Exp)
        int rep;                              // repeat value (for RLE)
        for ( band = 0; band < 4; band++ )
        {
          b = band;                           // index into scanline array
          int i = 0;
          while ( i < len )                   // read one component
          {
            rep = iss.ReadByte();
            if ( rep > 128 )                  // RLE packet
            {
              rep &= 0x7f;
              byte val = (byte)iss.ReadByte();
              while ( rep-- > 0 )
              {
                scanline[ b ] = val;
                b += 4;
                i++;
              }
            }
            else                              // raw packet
              while ( rep-- > 0 )
              {
                scanline[ b ] = (byte)iss.ReadByte();
                b += 4;
                i++;
              }
          }
        }
      }

      protected void writeScanline ( int len, Stream oss )
      {
        if ( len < MIN_ELEN ||
             len > MAX_ELEN )                 // use old scanline format
        {
          oss.Write( scanline, 0, len << 2 );
          return;
        }

        oss.WriteByte( 2 );
        oss.WriteByte( 2 );
        oss.WriteByte( (byte)(len >> 8) );
        oss.WriteByte( (byte)(len & 0x7f) );

        // put 4 components separately:
        len <<= 2;
        int b, band;
        for ( band = 0; band < 4; band++ )
        {
          b = band;                           // index into scanline array
          do                                  // encode at least one packet: [raw] [run]
          {
            // looking for RLE packet starting at 'b'
            int run = 0;
            int brun = b + 4;
            while ( brun < len )
            {
              if ( scanline[ brun ] == scanline[ brun - 4 ] )
              {
                if ( ++run >= MIN_RUN - 1 ) break; // found the run!
              }
              else
                run = 0;

              brun += 4;
            }

            brun -= (run << 2);               // start of the run
            int rawLen = (brun - b) >> 2;

            // now write out raw packets:
            while ( rawLen > 0 )
            {
              int rl = Math.Min( 128, rawLen );

              // write raw packet of size 'rl' starting at 'b':
              oss.WriteByte( (byte)rl );
              rawLen -= rl;
              do
              {
                oss.WriteByte( scanline[ b ] );
                b += 4;
              }
              while ( --rl > 0 );
            }

            if ( run >= MIN_RUN - 1 )           // write one run
            {
              brun = scanline[ b ];             // value to repeat
              run = 1;
              while ( (b += 4) < len &&
                      scanline[ b ] == brun &&
                      ++run < 127 ) ;
              if ( run == 127 ) b += 4;

              oss.WriteByte( (byte)(run + 128) );
              oss.WriteByte( (byte)brun );
            }
          }
          while ( b < len );
        }
      }

      /**
       * Read character token delimited by an white-space.
       * Skips initial white-spaces.
       */
      public static string readToken ( Stream iss )
      {
        int ch;
        do
          ch = iss.ReadByte();
        while ( ch >= 0 && char.IsWhiteSpace( (char)ch ) );

        if ( ch < 0 ) return null;

        StringBuilder sb = new StringBuilder();
        while ( ch >= 0 && !char.IsWhiteSpace( (char)ch ) )
        {
          sb.Append( (char)ch );
          ch = iss.ReadByte();
        }

        return sb.ToString();
      }

      /**
       * Common load code.
       *
       * @param  stream Opened input bit-stream.
       */
      public FloatImage commonLoad ( Stream stream )
      {
        if ( stream == null )
          throw new FileNotFoundException();

        // decode HDR file into raster image:
        // "#?RADIANCE ... 0x0a, 0x0a {+|-}Y <height> {+|-}X <width>"
        bool bottomUp = false;
        bool leftToRight = true;
        int width  = 0;
        int height = 0;

        int last;
        int act = stream.ReadByte();
        do
        {
          last = act;
          act = stream.ReadByte();
        }
        while ( act >= 0 && (last != 0x0a || act != 0x0a) );

        if ( act < 0 )
          throw new IOException( "Error in HDR header" );

        string axis1 = readToken( stream );
        int dim1;
        int.TryParse( readToken( stream ), out dim1 );
        string axis2 = readToken( stream );
        int dim2;
        int.TryParse( readToken( stream ), out dim2 );

        if ( axis1 == null || axis1.Length != 2 ||
             axis1 == null || axis1.Length != 2 )
          throw new IOException( "Error in HDR header" );

        if ( char.ToLower( axis1[ 1 ] ) == 'x' )
        {
          width = dim1;
          leftToRight = (axis1[ 0 ] == '+');
        }
        else
        if ( char.ToLower( axis1[ 1 ] ) == 'y' )
        {
          height = dim1;
          bottomUp = (axis1[ 0 ] == '+');
        }
        else
          throw new IOException( "Error in HDR header" );

        if ( char.ToLower( axis2[ 1 ] ) == 'x' )
        {
          width = dim2;
          leftToRight = (axis2[ 0 ] == '+');
        }
        else
        if ( char.ToLower( axis2[ 1 ] ) == 'y' )
        {
          height = dim2;
          bottomUp = (axis2[ 0 ] == '+');
        }
        else
          throw new IOException( "Error in HDR header" );

        if ( width <= 0 ||
             height <= 0 )
          throw new IOException( "Error in HDR header" );

        // init memory bitmap ..
        FloatImage im = new FloatImage( width, height, 3 );

        // .. and finally read the binary data into it:
        int x, y, dy, i;
        float[] pix = new float[ 3 ];
        if ( bottomUp )
        {
          y = height - 1;
          dy = -1;
        }
        else
        {
          y = 0;
          dy = 1;
        }

        while ( y >= 0 && y < height )        // read one scanline
        {
          readScanline( width, stream );

          // convert RGBe pixels into float[3] RGB:
          if ( leftToRight )
            for ( x = i = 0; x < width; x++, i += 4 )
            {
              Arith.RGBeToRGB( scanline, i, pix, 0 );
              im.PutPixel( x, y, pix );
            }
          else
            for ( x = width, i = 0; --x >= 0; i += 4 )
            {
              Arith.RGBeToRGB( scanline, i, pix, 0 );
              im.PutPixel( x, y, pix );
            }

          y += dy;
        }

        return im;
      }

      /**
       * Common save code.
       *
       * @param  stream Opened output bit-stream.
       * @param  im Floating-point raster image.
       */
      public void commonSave ( Stream stream, FloatImage im )
      {
        if ( stream == null )
          throw new FileNotFoundException();

        // write HDR header:
        int width  = im.Width;
        int height = im.Height;

        // "#?RADIANCE ... 0x0a, 0x0a -Y <height> +X <width>"
        StringBuilder sb = new StringBuilder( "#?RADIANCE\nFORMAT=32-bit_rle_rgbe\n\n-Y " );
        sb.Append( height ).Append( " +X " ).Append( width ).Append( '\n' );

        int x, y, i;
        foreach ( char c in sb.ToString() )
          stream.WriteByte( (byte)c );

        // write the bitmap:
        float[] pix = new float[ 3 ];
        assertScanline( width );

        for ( y = 0; y < height; y++ )        // encode one scanline
        {
          // read the scanline & convert it to RGBe format:
          for ( x = i = 0; x < width; x++, i += 4 )
          {
            im.GetPixel( x, y, pix );
            Arith.RGBToRGBe( scanline, i, pix[ 0 ], pix[ 1 ], pix[ 2 ] );
          }

          writeScanline( width, stream );
        }
      }
    }

    /// <summary>
    /// Read HDR image from the given disk file.
    /// </summary>
    /// <returns>Result or 'null' in case of failure.</returns>
    public static FloatImage FromFile ( string fileName )
    {
      try
      {
        using ( FileStream fs = new FileStream( fileName, FileMode.Open ) )
        {
          HDRInstance hi = new HDRInstance();
          return hi.commonLoad( fs );
        }
      }
      catch ( IOException )
      {
        return null;
      }
    }
  }
}
