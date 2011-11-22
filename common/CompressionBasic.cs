using System.IO;

// Simple implementation classes of compression interfaces.
namespace Compression
{
  /// <summary>
  /// Codec based on DeflateStream.
  /// </summary>
  public class DeflateCodec : IEntropyCodec
  {
    protected const int DEFAULT_BUFFER_SIZE = 16384;
    protected readonly byte[] buffer;
    protected int bPtr;
    protected bool openedEncode;
    protected bool openedDecode;

    public DeflateCodec ( int bufferSize )
    {
      buffer = new byte[ bufferSize ];
      bPtr = 0;
      bitStream = null;
      position = 0L;
      openedDecode =
      openedEncode = false;
    }

    public DeflateCodec ()
      : this( DEFAULT_BUFFER_SIZE )
    {
    }

    protected Stream bitStream;

    /// <summary>
    /// Current stream used for binary (encoded) I/O.
    /// </summary>
    public Stream BitStream
    {
      set
      {
        if ( bitStream != null ) Close();
        bitStream = value;
      }
      get
      {
        return bitStream;
      }
    }

    /// <summary>
    /// Codec initialization, sets default context (0).
    /// </summary>
    /// <param name="output">Open for output?</param>
    /// <returns>True if OK</returns>
    public bool Open ( bool output )
    {
      if ( bitStream == null ) return false;

      if ( openedEncode || openedDecode )
        Close();

      if ( output && !bitStream.CanWrite ||
           !output && !bitStream.CanRead )
        return false;

      openedEncode = output;
      openedDecode = !output;
      bPtr = 0;
      position = 0;
      return true;
    }

    /// <summary>
    /// Current context. "Contexts" are relatively independent codec instances,
    /// they only share common output buffer, so they are capable of fast and seamless
    /// inter-switching.
    /// If a codec doesn't implement contexts, you cannot change the default one (0).
    /// </summary>
    public int Context
    {
      get
      {
        return 0;
      }
      set
      {
      }
    }

    /// <summary>
    /// Maximal symbol value (0 to MaxSymbol)
    /// </summary>
    public int MaxSymbol
    {
      get
      {
        return 255;
      }
      set
      {
      }
    }

    /// <summary>
    /// Codec flush (current context).
    /// Finish the work and empty the output bit-buffer as well.
    /// </summary>
    public void Flush ()
    {
      if ( bPtr > 0 )
      {
        bitStream.Write( buffer, 0, bPtr );
        bPtr = 0;
      }
    }

    /// <summary>
    /// Codec close.
    /// </summary>
    public void Close ()
    {
      Flush();
      openedDecode =
      openedEncode = false;
    }

    protected long position;

    /// <summary>
    /// Returns current external (in symbols) codec position.
    /// </summary>
    public long Position ()
    {
      return position;
    }

    /// <summary>
    /// Returns current compressed position in bits.
    /// </summary>
    public long Compressed ()
    {
      return 0L;
    }

    /// <summary>
    /// Check if they are any input symbols (bits) available.
    /// </summary>
    public bool Available ()
    {
      return openedDecode;
    }

    /// <summary>
    /// Encodes the given symbol.
    /// </summary>
    /// <param name="symbol">Symbol code</param>
    public void Put ( int symbol )
    {
      if ( !openedEncode ) return;

      if ( bPtr >= buffer.Length )
      {
        bitStream.Write( buffer, 0, bPtr );
        bPtr = 0;
      }
      buffer[ bPtr++ ] = (byte)(symbol & 0xff);
      position++;
    }

    /// <summary>
    /// Encodes one subinterval from the given WheelOfFortune.
    /// </summary>
    /// <param name="wheel"></param>
    /// <param name="s">Symbol code</param>
    public void Put ( IWheelOfFortune wheel, int s )
    {
      Put( s );
    }

    /// <summary>
    /// Writes the give "raw" bits (by-passes the entropy encoder).
    /// </summary>
    /// <param name="bits">bits to be written (MSB first)</param>
    /// <param name="length">Number of bits to write</param>
    public void PutBits ( long bits, int length )
    {
      // !!! TODO !!!
    }

    /// <summary>
    /// Decodes one symbol.
    /// </summary>
    /// <returns>The decoded symbol or -1 in case of error</returns>
    public int Get ()
    {
      if ( !openedDecode ) return -1;

      position++;
      return bitStream.ReadByte();
    }

    /// <summary>
    /// Decodes one subinterval from the given WheelOfFortune.
    /// </summary>
    /// <param name="t"></param>
    /// <returns>The decoded symbol or -1 in case of error</returns>
    public int Get ( IWheelOfFortune wheel )
    {
      return Get();
    }

    /// <summary>
    /// Reads the given amount of "raw" bits (by-passes the entropy decoder).
    /// </summary>
    /// <param name="length">Number of bits to read</param>
    /// <returns>Read bits (or -1L if no bits are available)</returns>
    public long GetBits ( int length )
    {
      // !!! TODO !!!
      return 0L;
    }
  }

  /// <summary>
  /// Simple roulette-wheel: set of intervals with equal size (i.e. equal probability).
  /// </summary>
  public class StraightRoulette : IWheelOfFortune
  {
    /// <summary>
    /// Number of symbols.
    /// </summary>
    public int Symbols
    {
      get;
      set;
    }

    /// <summary>
    /// This command is ignored.
    /// </summary>
    public void Set ( long[] freq )
    {
    }

    /// <summary>
    /// This command is ignored.
    /// </summary>
    public void Set ( int sym, long freq )
    {
    }

    /// <summary>
    /// Returns the frequency sum of all symbols.
    /// Same as Left( Symbols() ) but might be more effective.
    /// </summary>
    /// <returns></returns>
    public long Total ()
    {
      return Symbols;
    }

    /// <summary>
    /// Returns the start of the symbol's cummulative frequency interval
    /// (the frequency sum of all preceding symbols).
    /// </summary>
    /// <param name="sym">Symbol code</param>
    /// <returns>The sum of all preceding symbols' frequencies</returns>
    public long Left ( int sym )
    {
      return sym;
    }

    /// <summary>
    /// Frequency of the given symbol.
    /// Has to be equal to "Left(sym+1)-Left(sym)"
    /// </summary>
    /// <param name="sym">Symbol code</param>
    /// <returns>The frequency of the symbol</returns>
    public long Get ( int sym )
    {
      return 1L;
    }

    /// <summary>
    /// Converts a frequency sum to the symbol code.
    /// </summary>
    /// <param name="sum">Frequency sum</param>
    /// <returns>The symbol which has sum in it's cummulative frequency interval</returns>
    public int Convert ( long sum )
    {
      return (int)sum;
    }
  }
}
