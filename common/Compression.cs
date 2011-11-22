using System;
using System.Collections.Generic;
using System.Drawing;

// Common code for ray-based rendering.
namespace Compression
{
  #region Interfaces

  /// <summary>
  /// General entropy-codec with output written to the Stream
  /// </summary>
  public interface IEntropyCodec
  {
    /// <summary>
    /// Codec initialization
    /// </summary>
    /// <param name="output">open for output?</param>
    /// <returns>True if OK</returns>
    bool Open ( bool output );

    /// <summary>
    /// Maximal symbol value (0 to MaxSymbol)
    /// </summary>
    int MaxSymbol
    {
      get;
      set;
    }

    /// <summary>
    /// Codec flush.
    /// </summary>
    void Flush ();

    /// <summary>
    /// Codec close.
    /// </summary>
    void Close ();

    /// <summary>
    /// Current external (in symbols) codec position.
    /// </summary>
    long Position
    {
      get;
    }

    /// <summary>
    /// Current compressed position in bits.
    /// </summary>
    long Compressed
    {
      get;
    }

    /// <summary>
    /// Are they any input symbols (bits) available?
    /// </summary>
    bool Available
    {
      get;
    }

    /// <summary>
    /// Encodes the given symbol.
    /// </summary>
    /// <param name="symbol"></param>
    void Put ( int symbol );

    /// <summary>
    /// Encodes one subinterval from the given WheelOfFortune.
    /// </summary>
    /// <param name="s"></param>
    //void Put ( WheelOfFortune wheel, int s );

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
    //int Get ( WheelOfFortune wheel );

    /// <summary>
    /// Reads the given amount of "raw" bits (by-passes the entropy decoder).
    /// </summary>
    /// <param name="length">Number of bits to read</param>
    /// <returns>Read bits (or -1l if no bits are available)</returns>
    long GetBits ( int length );
  }

  #endregion
}
