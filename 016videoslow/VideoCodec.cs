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

    protected const uint MAGIC_FRAME = 0x1212fba1;

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
      frameWidth      = width;
      frameHeight     = height;
      framesPerSecond = fps;

      if ( outs == null ) return null;

      DeflateStream ds = new BufferedDeflateStream( 16384, outs, CompressionMode.Compress, true );

      // !!!{{ TODO: add the header construction/encoding here

      // video header: [ MAGIC, width, height, fps ]
      ds.WriteByte( (byte)((MAGIC >> 24) & 0xff) );
      ds.WriteByte( (byte)((MAGIC >> 16) & 0xff) );
      ds.WriteByte( (byte)((MAGIC >>  8) & 0xff) );
      ds.WriteByte( (byte)(MAGIC         & 0xff) );

      ds.WriteByte( (byte)((width >> 8) & 0xff) );
      ds.WriteByte( (byte)(width        & 0xff) );

      ds.WriteByte( (byte)((height >> 8) & 0xff) );
      ds.WriteByte( (byte)(height        & 0xff) );

      int fpsInt = (int)(100.0f * fps);
      ds.WriteByte( (byte)((fpsInt >> 8) & 0xff) );
      ds.WriteByte( (byte)(fpsInt        & 0xff) );

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

      // frame header: [ MAGIC_FRAME, frameNo ]
      outs.WriteByte( (byte)((MAGIC_FRAME >> 24) & 0xff) );
      outs.WriteByte( (byte)((MAGIC_FRAME >> 16) & 0xff) );
      outs.WriteByte( (byte)((MAGIC_FRAME >>  8) & 0xff) );
      outs.WriteByte( (byte)(MAGIC_FRAME         & 0xff) );

      outs.WriteByte( (byte)((frameNo >> 8) & 0xff) );
      outs.WriteByte( (byte)(frameNo        & 0xff) );

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

      // Check the global header:
      int buffer;
      buffer = ds.ReadByte();
      if ( buffer < 0 || buffer != ((MAGIC >> 24) & 0xff) ) return null;
      buffer = ds.ReadByte();
      if ( buffer < 0 || buffer != ((MAGIC >> 16) & 0xff) ) return null;
      buffer = ds.ReadByte();
      if ( buffer < 0 || buffer != ((MAGIC >>  8) & 0xff) ) return null;
      buffer = ds.ReadByte();
      if ( buffer < 0 || buffer != (MAGIC         & 0xff) ) return null;

      frameWidth = ds.ReadByte();
      if ( frameWidth < 0 ) return null;
      buffer = ds.ReadByte();
      if ( buffer < 0 ) return null;
      frameWidth = (frameWidth << 8) + buffer;
      frameHeight = ds.ReadByte();
      if ( frameHeight < 0 ) return null;
      buffer = ds.ReadByte();
      if ( buffer < 0 ) return null;
      frameHeight = (frameHeight << 8) + buffer;

      int fpsInt = ds.ReadByte();
      if ( fpsInt < 0 ) return null;
      buffer = ds.ReadByte();
      if ( buffer < 0 ) return null;
      framesPerSecond = ((fpsInt << 8) + buffer) * 0.01f;

      // !!!}}

      return ds;
    }

    public Bitmap DecodeFrame ( int frameNo, Stream inps )
    {
      if ( inps == null ) return null;

      // !!!{{ TODO: add the decoding code here

      // Check the frame header:
      int buffer;
      buffer = inps.ReadByte();
      if ( buffer < 0 || buffer != ((MAGIC_FRAME >> 24) & 0xff) ) return null;
      buffer = inps.ReadByte();
      if ( buffer < 0 || buffer != ((MAGIC_FRAME >> 16) & 0xff) ) return null;
      buffer = inps.ReadByte();
      if ( buffer < 0 || buffer != ((MAGIC_FRAME >>  8) & 0xff) ) return null;
      buffer = inps.ReadByte();
      if ( buffer < 0 || buffer != (MAGIC_FRAME         & 0xff) ) return null;

      int frNo = inps.ReadByte();
      if ( frNo < 0 ) return null;
      buffer = inps.ReadByte();
      if ( buffer < 0 ) return null;
      if ( ((frNo << 8) + buffer) != frameNo ) return null;

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
