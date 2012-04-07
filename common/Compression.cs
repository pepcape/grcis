using System.IO;

// Common code (interfaces mostly) for data compression.
namespace Compression
{
  #region Interfaces

  /// <summary>
  /// General entropy-codec with output written to a Stream.
  /// </summary>
  public interface IEntropyCodec
  {
    /// <summary>
    /// Current stream used for binary (encoded) I/O.
    /// </summary>
    Stream BinaryStream
    {
      set;
      get;
    }

    /// <summary>
    /// Codec initialization, sets default context (0).
    /// </summary>
    /// <param name="output">Open for output?</param>
    /// <returns>True if OK</returns>
    bool Open ( bool output );

    /// <summary>
    /// Current context. "Contexts" are relatively independent codec instances,
    /// they only share common output buffer, so they are capable of fast and seamless
    /// inter-switching.
    /// If a codec doesn't implement contexts, you cannot change the default one (0).
    /// </summary>
    int Context
    {
      get;
      set;
    }

    /// <summary>
    /// Maximal symbol value (0 to MaxSymbol)
    /// </summary>
    int MaxSymbol
    {
      get;
      set;
    }

    /// <summary>
    /// Codec flush (current context).
    /// Finish the work and empty the output bit-buffer as well.
    /// </summary>
    void Flush ();

    /// <summary>
    /// Codec close.
    /// </summary>
    void Close ();

    /// <summary>
    /// Returns current external (in symbols) codec position.
    /// </summary>
    long Position ();

    /// <summary>
    /// Returns current compressed position in bits.
    /// </summary>
    long Compressed ();

    /// <summary>
    /// Check if they are any input symbols (bits) available.
    /// </summary>
    bool Available ();

    /// <summary>
    /// Encodes the given symbol.
    /// </summary>
    /// <param name="symbol"></param>
    void Put ( int symbol );

    /// <summary>
    /// Encodes one subinterval from the given WheelOfFortune.
    /// </summary>
    /// <param name="s"></param>
    void Put ( IWheelOfFortune wheel, int s );

    /// <summary>
    /// Writes the give "raw" bits (by-passes the entropy encoder).
    /// </summary>
    /// <param name="bits">bits to be written (MSB first)</param>
    /// <param name="length">Number of bits to write</param>
    void PutBits ( long bits, int length );

    /// <summary>
    /// Decodes one symbol.
    /// </summary>
    /// <returns>The decoded symbol or -1 in case of error</returns>
    int Get ();

    /// <summary>
    /// Decodes one subinterval from the given WheelOfFortune.
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    int Get ( IWheelOfFortune wheel );

    /// <summary>
    /// Reads the given amount of "raw" bits (by-passes the entropy decoder).
    /// </summary>
    /// <param name="length">Number of bits to read</param>
    /// <returns>Read bits (or -1L if no bits are available)</returns>
    long GetBits ( int length );
  }

  /// <summary>
  /// Abstract interface for the "Wheel of Fortune" - an ordered array of
  /// frequencies used for entropy encoding.
  /// </summary>
  public interface IWheelOfFortune
  {
    /// <summary>
    /// Total number of symbols.
    /// Altering the alphabet size keeps frequencies, only truncates the list of symbols.
    /// </summary>
    int Symbols
    {
      get;
      set;
    }

    /// <summary>
    /// Sets all the frequencies at once.
    /// </summary>
    /// <param name="freq">Array of new frequencies</param>
    void Set ( long[] freq );

    /// <summary>
    /// Sets frequency of an individual symbol.
    /// </summary>
    /// <param name="sym">Symbol to change</param>
    /// <param name="freq">The new frequency</param>
    void Set ( int sym, long freq );

    /// <summary>
    /// Returns the frequency sum of all symbols.
    /// Same as Left( Symbols() ) but might be more effective.
    /// </summary>
    /// <returns></returns>
    long Total ();

    /// <summary>
    /// Returns the start of the symbol's cummulative frequency interval
    /// (the frequency sum of all preceding symbols).
    /// </summary>
    /// <param name="sym">Symbol code</param>
    /// <returns>The sum of all preceding symbols' frequencies</returns>
    long Left ( int sym );

    /// <summary>
    /// Frequency of the given symbol.
    /// Has to be equal to "Left(sym+1)-Left(sym)"
    /// </summary>
    /// <param name="sym">Symbol code</param>
    /// <returns>The frequency of the symbol</returns>
    long Get ( int sym );

    /// <summary>
    /// Converts a frequency sum to the symbol code.
    /// </summary>
    /// <param name="sum">Frequency sum</param>
    /// <returns>The symbol which has sum in it's cummulative frequency interval</returns>
    int Convert ( long sum );
  }

  /// <summary>
  /// Abstract histogram interface - IWheelOfFortune with incremental frequency
  /// update capability and decimation mechanism.
  /// </summary>
  public interface IEntropyHistogram : IWheelOfFortune
  {
    /// <summary>
    /// Histogram [re-]initialization.
    /// Destroys all previously collected statistics.
    /// </summary>
    void Init ();

    /// <summary>
    /// Current context. If you set a new one, default
    /// values will be used to initialize it.
    /// </summary>
    int Context
    {
      get;
      set;
    }

    /// <summary>
    /// Default alphabet size for new contexts.
    /// </summary>
    int DefaultSymbols
    {
      get;
      set;
    }

    /// <summary>
    /// Frequency sum limit for the current context.
    /// </summary>
    long SumLimit
    {
      get;
      set;
    }

    /// <summary>
    /// Default frequency sum limit for new contexts.
    /// </summary>
    long DefaultSumLimit
    {
      get;
      set;
    }

    /// <summary>
    /// Should we keep non-zero frequencies all the time?
    /// (current context)
    /// </summary>
    bool NonZero
    {
      get;
      set;
    }

    /// <summary>
    /// Default NonZero flag for new contexts.
    /// </summary>
    bool DefaultNonZero
    {
      get;
      set;
    }

    /// <summary>
    /// Divides each frequency in the current context by 2.
    /// Called automatically when the total frequency sum exceeds the limit.
    /// </summary>
    void Decimate ();

    /// <summary>
    /// Increments a single symbol's frequency by one.
    /// </summary>
    /// <param name="sym">Symbol code</param>
    void Inc ( int sym );

    /// <summary>
    /// Increments a single symbol's frequency.
    /// </summary>
    /// <param name="sym">Symbol code</param>
    /// <param name="incr">Increment</param>
    void Inc ( int sym, long incr );

    /// <summary>
    /// Increments frequencies of all symbols in the current context.
    /// </summary>
    /// <param name="incr">Array of frequency increments</param>
    void Inc ( long[] incr );
  }

  #endregion
}
