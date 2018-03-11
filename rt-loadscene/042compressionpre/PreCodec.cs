// Author: Josef Pelikan

using System;
using System.Drawing;
using System.IO;
using Compression;
using Raster;

namespace _042compressionpre
{
  class PreCodec
  {
    #region protected data

    protected const uint MAGIC = 0xfee1600d;

    #endregion

    #region constructor

    public PreCodec ()
    {
    }

    #endregion

    #region Codec API

    public void EncodeImage ( Bitmap inp, Stream outs )
    {
      if ( inp  == null ||
           outs == null ) return;

      int width  = inp.Width;
      int height = inp.Height;
      if ( width < 1 || height < 1 ) return;

      // !!!{{ TODO: add the encoding code here

      IEntropyCodec c = new DeflateCodec();
      c.MaxSymbol = 255;
      if ( c.MaxSymbol < 255 )
        throw new Exception( "Unappropriate codec used (alphabet too small)!" );

      c.BinaryStream = outs;
      c.Open( true );

      // file header: [ MAGIC, width, height ]
      c.PutBits( MAGIC, 32 );
      c.PutBits( width, 16 );
      c.PutBits( height, 16 );

      for ( int y = 0; y < height; y++ )
      {
        for ( int x = 0; x < width; x++ )
        {
          Color col = inp.GetPixel( x, y );
          int gr = Draw.RgbToGray( col.R, col.G, col.B );
          c.Put( gr & 0xff );
        }
      }

      c.Close();

      // !!!}}
    }

    public Bitmap DecodeImage ( Stream inps )
    {
      if ( inps == null ) return null;

      // !!!{{ TODO: add the decoding code here

      IEntropyCodec c = new DeflateCodec();
      c.BinaryStream = inps;
      c.Open( false );

      uint magic = (uint)c.GetBits( 32 );
      if ( magic != MAGIC ) return null;

      int width, height;
      width  = (int)c.GetBits( 16 );
      height = (int)c.GetBits( 16 );

      if ( width < 1 || height < 1 )
        return null;

      Bitmap result = new Bitmap( width, height, System.Drawing.Imaging.PixelFormat.Format24bppRgb );

      for ( int y = 0; y < height; y++ )
        for ( int x = 0; x < width; x++ )
        {
          int gr = c.Get();
          if ( gr < 0 ) goto fin;
          result.SetPixel( x, y, Color.FromArgb( gr, gr, gr ) );
        }

      fin:
      c.Close();

      return result;

      // !!!}}
    }

    #endregion
  }
}
