using System;
using System.IO;
using System.IO.Compression;

namespace Support
{
  public class BufferedDeflateStream : DeflateStream
  {
    internal const int DEFAULT_BUFFER_SIZE = 4096;

    private readonly byte[] buffer;
    private readonly int bSize;
    private int bPtr;

    public BufferedDeflateStream ( Stream stream, CompressionMode mode ) : this( DEFAULT_BUFFER_SIZE, stream, mode )
    {}

    public BufferedDeflateStream ( Stream stream, CompressionMode mode, bool leaveOpen ) : this( DEFAULT_BUFFER_SIZE, stream, mode, leaveOpen )
    {}

    public BufferedDeflateStream ( int bufferSize, Stream stream, CompressionMode mode ) : base( stream, mode )
    {
      buffer = new byte[ bufferSize ];
      bSize  = bufferSize;
      bPtr   = 0;
    }

    public BufferedDeflateStream ( int bufferSize, Stream stream, CompressionMode mode, bool leaveOpen ) : base( stream, mode, leaveOpen )
    {
      buffer = new byte[ bufferSize ];
      bSize  = bufferSize;
      bPtr   = 0;
    }

    public override void WriteByte ( byte value )
    {
      if ( bPtr >= bSize )
      {
        base.Write( buffer, 0, bSize );
        bPtr = 0;
      }
      buffer[ bPtr++ ] = value;
    }

    public override void Flush ()
    {
      if ( bPtr > 0 )
      {
        base.Write( buffer, 0, bPtr );
        bPtr = 0;
      }
      base.Flush();
    }

    public override void Write ( byte[] array, int offset, int count )
    {
      if ( count < 1 ) return;

      while ( bPtr + count > bSize )
      {
        int copyCount = bSize - bPtr;
        if ( copyCount > 0 )
          Array.Copy( array, offset, buffer, bPtr, copyCount );
        base.Write( buffer, 0, bSize );
        bPtr    = 0;
        offset += copyCount;
        count  -= copyCount;
      }

      if ( count > 0 )
      {
        Array.Copy( array, offset, buffer, bPtr, count );
        bPtr += count;
      }
    }

  }
}
