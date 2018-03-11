using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Compression;
using Raster;
using Utilities;

namespace _066histogram
{
  public class Compress
  {
    public static long PCM ( Bitmap input, string param )
    {
      // Text parameters:
      Dictionary<string, string> p = Util.ParseKeyValueList( param );
      bool BW = false;
      Util.TryParse( p, "bw", ref BW );

      int width  = input.Width;
      int height = input.Height;
      if ( width < 1 || height < 1 )
        return 0L;

      // Encoding, only dry-run in memory.
      // Backend: adaptive Huffman encoder.
      MemoryStream ms = new MemoryStream();
      HuffmanCodec huff = new HuffmanCodec();
      huff.BinaryStream = ms;
      huff.Open( true );

      if ( BW )
      {
        int buffer = 0;
        int bufLen = 0;
        for ( int y = 0; y < height; y++ )
        {
          for ( int x = 0; x < width; x++ )
          {
            int gr = (input.GetPixel( x, y ).GetBrightness() < 0.5f) ? 0 : 1;
            buffer += buffer + gr;
            if ( ++bufLen == 8 )
            {
              huff.Put( buffer & 0xff );
              buffer = 0;
              bufLen = 0;
            }
          }

          // end of scanline
          if ( bufLen > 0 )
          {
            buffer <<= 8 - bufLen;
            huff.Put( buffer & 0xff );
            buffer = 0;
            bufLen = 0;
          }
        }
      }
      else
        for ( int y = 0; y < height; y++ )
          for ( int x = 0; x < width; x++ )
          {
            Color c = input.GetPixel( x, y );
            int gr = Draw.RgbToGray( c.R, c.G, c.B );
            huff.Put( gr );
          }

      huff.Flush();
      long result = ms.Length;
      huff.Close();

      return result;
    }

    public static long DPCM ( Bitmap input, string param )
    {
      // Text parameters:
      Dictionary<string, string> p = Util.ParseKeyValueList( param );
      bool predict = false;
      Util.TryParse( p, "predict", ref predict );

      int width = input.Width;
      int height = input.Height;
      if ( width < 1 || height < 1 )
        return 0L;

      // Encoding, only dry-run in memory.
      // Backend: adaptive Huffman encoder.
      MemoryStream ms = new MemoryStream();
      HuffmanCodec huff = new HuffmanCodec();
      huff.BinaryStream = ms;
      huff.MaxSymbol = 510;      // -255 to +255
      huff.Open( true );

      // Predictive gray-pixel encoding:
      int previous = 0;
      for ( int y = 0; y < height; y++ )
        for ( int x = 0; x < width; x++ )
        {
          Color c = input.GetPixel( x, y );
          int gr = Draw.RgbToGray( c.R, c.G, c.B );
          if ( predict )
          {
            huff.Put( gr - previous + 255 );
            previous = gr;
          }
          else
            huff.Put( gr );
        }

      huff.Flush();
      long result = ms.Length;
      huff.Close();

      return result;
    }
  }
}
