// Author: Josef Pelikan

using System;
using System.Drawing;
using System.IO;
using Compression;

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

    public IEntropyCodec EncodeHeader ( int width, int height, float fps, Stream outs )
    {
      frameWidth      = width;
      frameHeight     = height;
      framesPerSecond = fps;

      if ( outs == null ) return null;

      IEntropyCodec c = new DeflateCodec();
      c.MaxSymbol = 255;
      if ( c.MaxSymbol < 255 )
        throw new Exception( "Unappropriate codec used (alphabet too small)!" );

      c.BinaryStream = outs;
      c.Open( true );

      // !!!{{ TODO: add the header construction/encoding here

      // video header: [ MAGIC, width, height, fps ]
      c.PutBits( MAGIC, 32 );
      c.PutBits( width, 16 );
      c.PutBits( height, 16 );

      int fpsInt = (int)(100.0f * fps);
      c.PutBits( fpsInt, 16 );

      // !!!}}

      return c;
    }

    public void EncodeFrame ( int frameNo, Bitmap inp, IEntropyCodec c )
    {
      if ( inp  == null ||
           c == null ) return;

      int width  = inp.Width;
      int height = inp.Height;
      if ( width  != frameWidth ||
           height != frameHeight ) return;

      // !!!{{ TODO: add the encoding code here

      // frame header: [ MAGIC_FRAME, frameNo ]
      c.PutBits( MAGIC_FRAME, 32 );
      c.PutBits( frameNo, 16 );

      for ( int y = 0; y < frameHeight; y++ )
        for ( int x = 0; x < frameWidth; x++ )
        {
          byte gr = (byte)(inp.GetPixel( x, y ).GetBrightness() * 255.0f);
          c.Put( gr );
        }

      // !!!}}
    }

    public IEntropyCodec DecodeHeader ( Stream inps )
    {
      if ( inps == null ) return null;

      IEntropyCodec c = new DeflateCodec();
      c.BinaryStream = inps;
      c.Open( false );

      // !!!{{ TODO: add the header decoding here

      // Check the global header:
      uint magic = (uint)c.GetBits( 32 );
      if ( magic != MAGIC ) return null;

      frameWidth  = (int)c.GetBits( 16 );
      frameHeight = (int)c.GetBits( 16 );

      if ( frameWidth < 1 || frameHeight < 1 )
        return null;

      framesPerSecond = (int)c.GetBits( 16 );

      // !!!}}

      return c;
    }

    public Bitmap DecodeFrame ( int frameNo, IEntropyCodec c )
    {
      if ( c == null ) return null;

      // !!!{{ TODO: add the decoding code here

      // Check the frame header:
      uint magic = (uint)c.GetBits( 32 );
      if ( magic != MAGIC_FRAME ) return null;

      int frNo = (int)c.GetBits( 16 );
      if ( frNo != frameNo ) return null;

      Bitmap result = new Bitmap( frameWidth, frameHeight, System.Drawing.Imaging.PixelFormat.Format24bppRgb );

      for ( int y = 0; y < frameHeight; y++ )
        for ( int x = 0; x < frameWidth; x++ )
        {
          int gr = c.Get();
          result.SetPixel( x, y, Color.FromArgb( gr, gr, gr ) );
        }

      return result;

      // !!!}}
    }

    #endregion

  }

}
