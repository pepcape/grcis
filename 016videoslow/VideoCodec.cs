using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Text;
using System.IO;
using System.IO.Compression;
using Support;

namespace _016videoslow
{
  class VideoCodec
  {
    #region protected data

    protected const uint MAGIC = 0xfe4c128a;

    protected int frameWidth = 0;

    protected int frameHeight = 0;

    protected float framesPerSecond = 0.0f;

    #endregion

    #region constructor

    public VideoCodec ()
    {
    }

    #endregion

    #region Codec API

    public Stream EncodeHeader ( int width, int height, float fps, Stream outs )
    {
      frameWidth  = width;
      frameHeight = height;
      framesPerSecond = fps;

      if ( outs == null ) return null;

      DeflateStream ds = new BufferedDeflateStream( 16384, outs, CompressionMode.Compress, true );

      // !!!{{ TODO: add the header construction/encoding here

      // !!!}}

      return ds;
    }

    public void EncodeFrame ( int frameNo, Bitmap inp, Stream outs )
    {
      if ( inp  == null ||
           outs == null ) return;

      int width  = inp.Width;
      int height = inp.Height;
      if ( width  != frameWidth ||
           height != frameHeight ) return;

      // !!!{{ TODO: add the encoding code here

      for ( int y = 0; y < frameHeight; y++ )
        for ( int x = 0; x < frameWidth; x++ )
        {
          byte gr = (byte)(inp.GetPixel( x, y ).GetBrightness() * 255.0f);
          outs.WriteByte( gr );
        }

      // !!!}}
    }

    public Stream DecodeHeader ( Stream inps )
    {
      if ( inps == null ) return null;

      DeflateStream ds = new DeflateStream( inps, CompressionMode.Decompress, true );

      // !!!{{ TODO: add the header decoding here

      // !!!}}

      return ds;
    }

    public Bitmap DecodeFrame ( int frameNo, Stream inps )
    {
      if ( inps == null ) return null;

      // !!!{{ TODO: add the decoding code here

      Bitmap result = new Bitmap( frameWidth, frameHeight, System.Drawing.Imaging.PixelFormat.Format24bppRgb );

      for ( int y = 0; y < frameHeight; y++ )
        for ( int x = 0; x < frameWidth; x++ )
        {
          int gr = inps.ReadByte();
          result.SetPixel( x, y, Color.FromArgb( gr, gr, gr ) );
        }

      return result;

      // !!!}}
    }

    #endregion

  }

}
