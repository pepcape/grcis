using System.Collections.Generic;
using System.IO;
using Utilities;

namespace Compression
{
  public class BitStream
  {
    protected Stream stream = null;

    protected int buffer;

    protected int bufLen = 0;

    public void Flush ()
    {
      if ( stream == null ||
           !stream.CanWrite )
        return;

      if ( bufLen > 0 )
      {
        stream.WriteByte( (byte)(buffer << (8 - bufLen)) );
        bufLen = 0;
      }

      stream.Flush();
    }

    public void Close ()
    {
      Flush();
      if ( stream != null )
      {
        stream.Close();
        stream = null;
      }
    }

    public void Open ( Stream s )
    {
      Flush();
      stream = s;
      bufLen = 0;
    }

    public BitStream ( Stream s = null )
    {
      Open( s );
    }

    public long ReadBits ( int bits )
    {
      if ( stream == null ||
           !stream.CanRead )
        return 0L;

      long result = 0L;
      while ( bits-- > 0 )
      {
        // read one bit:
        if ( bufLen == 0 )
        {
          buffer = stream.ReadByte();
          bufLen = 8;
        }
        result += result + ((buffer & 0x80) > 0 ? 1 : 0);
        bufLen--;
      }
      return result;
    }

    public void WriteBits ( long value, int bits )
    {
      if ( bits <= 0 ||
           stream == null ||
           !stream.CanWrite )
        return;

      long mask = 1L << (bits - 1);
      while ( mask > 0 )
      {
        // write one bit:
        if ( bufLen == 8 )
        {
          stream.WriteByte( (byte)buffer );
          bufLen = 0;
          buffer = 0;
        }
        buffer += buffer + ((value & mask) > 0 ? 1 : 0);
        bufLen++;
        mask >>= 1;
      }
    }
  }

  /// <summary>
  /// Adaptive Huffman codec.
  /// Original author: Ondrej Starek <stareko@seznam.cz> (JaGrLib)
  /// </summary>
  public class HuffmanCodec : IEntropyCodec
  {
    /// <summary>
    /// Huffman tree node.
    /// </summary>
    protected class HuffNode
    {
      /// <summary>
      /// Node word.
      /// </summary>
      public int character;

      /// <summary>
      /// Summary frequency of the subtree.
      /// </summary>
      public long freq;

      /// <summary>
      /// Tree vertical links.
      /// </summary>
      public HuffNode left;
      public HuffNode right;
      public HuffNode parent;

      /// <summary>
      /// Sibling (horizontal) links.
      /// </summary>
      public HuffNode next;
      public HuffNode prev;

      public HuffNode ( int ch )
      {
        character = ch;
        freq = 1L;
        // all other data is implicitly initialized to 0 (null)
      }
    }

    /// <summary>
    /// Huffman tree (used as EntropyCodec-context).
    /// </summary>
    protected class HuffTree
    {
      /// <summary>
      /// Total number of code-words (including {@link #NYT}-escape).
      /// </summary>
      private int wordCount;

      /// <summary>
      /// Not-Yet-Transmitted escape code.
      /// </summary>
      private int NYT;

      /// <summary>
      /// Special value used in inner nodes.
      /// </summary>
      private int innode;

      /// <summary>
      /// Code-word size in bits.
      /// </summary>
      private int codeSize;

      /// <summary>
      /// Array of tree leaves.
      /// </summary>
      private HuffNode[] leaves;

      /// <summary>
      /// Root of the current Huffman tree.
      /// </summary>
      private HuffNode root;

      public HuffTree ( int maxSymbol )
      {
        init( maxSymbol );
      }

      public void init ( int maxSymbol )
      {
        if ( wordCount != maxSymbol + 2 )
        {
          wordCount = maxSymbol + 2;        // including NYT
          NYT = maxSymbol + 1;
          innode = maxSymbol + 2;

          // compute code-word size:
          codeSize = 1;
          int size = 2;
          while ( size < NYT )
          {
            size += size;
            codeSize++;
          }

          // array of leaves:
          leaves = new HuffNode[ wordCount ];
        }
        else
          for ( int i = 0; i < wordCount; )
            leaves[ i ] = null;

        // root item:
        root = new HuffNode( NYT );
        leaves[ NYT ] = root;
      }

      public int getMaxSymbol ()
      {
        return (wordCount - 2);
      }

      private void printTreeInner ( HuffNode node )
      {
        if ( node == null ) return;

        printTreeInner( node.left );
        printTreeInner( node.right );
      }

      public void printTree ()
      {
        printTreeInner( root );
      }

      private void splitNYT ( int character )
      {
        HuffNode n1, n2;

        n1 = new HuffNode( NYT );
        n2 = new HuffNode( character );
        leaves[ NYT ].character = innode;
        leaves[ NYT ].freq = 1;

        // siblings:
        n1.next = n2;
        n2.next = leaves[ NYT ];
        n2.prev = n1;
        leaves[ NYT ].prev = n2;

        // tree links:
        n1.parent = leaves[ NYT ];
        n2.parent = leaves[ NYT ];
        leaves[ NYT ].left = n1;
        leaves[ NYT ].right = n2;

        // update leaf node:
        leaves[ NYT ] = n1;
        leaves[ character ] = n2;
      }

      private HuffNode swapLast ( HuffNode node )
      {
        HuffNode n;

        // find the last node with the same frequency..
        n = node;
        while ( n.next != null &&
                n.next != root &&
                n.next.freq == node.freq )
          n = n.next;

        // it's me and my parent => dont swap
        if ( n == node ) return node;

        // swap data:
        int d = node.character;
        node.character = n.character;
        n.character = d;
        long f = node.freq;
        node.freq = n.freq;
        n.freq = f;

        // swap children:
        HuffNode h;
        h = node.left;
        node.left = n.left;
        n.left = h;
        h = node.right;
        node.right = n.right;
        n.right = h;
        if ( node.character == innode )
        {
          node.left.parent = node;
          node.right.parent = node;
        }
        if ( n.character == innode )
        {
          n.left.parent = n;
          n.right.parent = n;
        }

        // update list of leaves:
        if ( node.character != innode )
          leaves[ node.character ] = node;
        if ( n.character != innode )
          leaves[ n.character ] = n;

        return n;
      }

      private void rearrangeTree ( HuffNode node )
      {
        while ( node != root )
        {
          node = swapLast( node );
          node.freq++;
          node = node.parent;
        }
        node.freq++;
      }

      public int getChar ( BitStream s )
      {
        HuffNode n = root;
        long bit;
        int ch;

        // find the leaf node:
        while ( n.character == innode )
        {
          bit = s.ReadBits( 1 );
          n = ((bit & 1) != 0) ? n.right : n.left;
        }

        if ( n.character == NYT )
        {
          // if the leaf is NYT, read the next character:
          ch = (int)s.ReadBits( codeSize );
          splitNYT( ch );
          n = leaves[ ch ].parent;
        }
        else
          ch = n.character;

        // update the tree:
        rearrangeTree( n );

        return ch;
      }

      private void writeCode ( HuffNode node, BitStream s )
      {
        if ( node == root ) return;

        writeCode( node.parent, s );
        s.WriteBits( (node.parent.left == node) ? 0 : 1, 1 );
      }

      public void writeChar ( BitStream s, int character )
      {
        if ( character < 0 ||
             character >= NYT )
        {
          Util.Log( "HuffmanCodec - symbol (" + character + ") out of range (" + (NYT - 1) + ')' );
          character = 0;
        }

        // write the code
        if ( leaves[ character ] == null )
        {
          // write NYT and character:
          writeCode( leaves[ NYT ], s );
          s.WriteBits( character, codeSize );

          // insert char into the tree:
          splitNYT( character );
          rearrangeTree( leaves[ character ].parent );
        }
        else
        {
          writeCode( leaves[ character ], s );
          rearrangeTree( leaves[ character ] );
        }
      }
    }

    protected bool openedEncode;
    protected bool openedDecode;

    protected Stream binaryStream = null;
    protected BitStream bitStream = new BitStream();

    /// <summary>
    /// Set of Huffman contexts.
    /// </summary>
    protected Dictionary<int, HuffTree> contexts = new Dictionary<int, HuffTree>();

    /// <summary>
    /// Id of the current context.
    /// </summary>
    protected int currentContext = int.MinValue;

    /// <summary>
    /// Current stream used for binary (encoded) I/O.
    /// </summary>
    public Stream BinaryStream
    {
      set
      {
        if ( binaryStream != null ) Close();
        binaryStream = value;
        bitStream.Open( binaryStream );
        position = 0L;
      }
      get
      {
        return binaryStream;
      }
    }

    /// <summary>
    /// Codec initialization, sets default context (0).
    /// </summary>
    /// <param name="output">Open for output?</param>
    /// <returns>True if OK</returns>
    public bool Open ( bool output )
    {
      if ( binaryStream == null ) return false;

      if ( openedEncode || openedDecode )
        Close();

      if ( output && !binaryStream.CanWrite ||
           !output && !binaryStream.CanRead )
        return false;

      openedEncode = output;
      openedDecode = !output;
      bitStream.Open( binaryStream );
      position = 0L;
      initContexts( 0 );

      return true;
    }

    /// <summary>
    /// Current context (accelerator).
    /// </summary>
    protected HuffTree context;

    /// <summary>
    /// Max-symbol used only for creating of new contexts!
    /// </summary>
    protected int maxSymbol = 255;

    /// <summary>
    /// Current position in original (logical) stream.
    /// </summary>
    protected long position = 0;

    /// <summary>
    /// Discards all contexts.
    /// </summary>
    protected void clearContexts ()
    {
      contexts = new Dictionary<int, HuffTree>( 1 );
      currentContext = int.MinValue;
      context = null;
    }

    /// <summary>
    /// [Re-]initializes context set, creates the given solo context.
    /// </summary>
    protected void initContexts ( int ctx )
    {
      clearContexts();
      setContextInner( ctx );
    }

    private int setContextInner ( int ctxId )
    {
      if ( ctxId == currentContext ) return currentContext;

      int oldId = currentContext;

      HuffTree hf;
      if ( !contexts.TryGetValue( ctxId, out hf ) )
      {
        hf = new HuffTree( maxSymbol );
        contexts[ ctxId ] = hf;
      }
      currentContext = ctxId;
      context = hf;

      return oldId;
    }

    public HuffmanCodec ()
    {
      clearContexts();
    }

    /// <summary>
    /// Are they any input symbols (bits) available?
    /// </summary>
    public bool Available ()
    {
      return (openedDecode &&
               binaryStream != null &&
               binaryStream.Length > binaryStream.Position);
    }

    public void Close ()
    {
      if ( bitStream == null )
        return;

      bitStream.Close();
      binaryStream = null;
      position = 0L;
      clearContexts();
      openedDecode =
      openedEncode = false;
    }

    public long Compressed ()
    {
      // !!! TODO: compute exact binary position
      if ( binaryStream == null )
        return 0L;

      return binaryStream.Position;
    }

    public void Flush ()
    {
      if ( openedEncode )
        bitStream.Flush();
    }

    public int Get ()
    {
      if ( binaryStream == null ||
           !openedDecode )
        throw new IOException( "HuffmanCodec is not opened for reading!" );

      position++;
      return context.getChar( bitStream );
    }

    public int Get ( IWheelOfFortune wheel )
    {
      return Get();
    }

    public long GetBits ( int length )
    {
      if ( binaryStream == null ||
           !openedDecode )
        throw new IOException( "HuffmanCodec is not opened for reading!" );

      return bitStream.ReadBits( length );
    }

    public int MaxSymbol
    {
      set
      {
        maxSymbol = value;
        if ( context != null )
          context.init( value );
      }
      get
      {
        if ( binaryStream == null )
          return -1;

        if ( context != null )
          return context.getMaxSymbol();

        return maxSymbol;
      }
    }

    public long Position ()
    {
      return position;
    }

    public void Put ( int symbol )
    {
      if ( binaryStream == null ||
           !openedEncode )
        throw new IOException ( "HuffmanCodec is not opened for writting!" );

      position++;
      context.writeChar( bitStream, symbol );
    }

    public void Put ( IWheelOfFortune wheel, int s )
    {
      Put( s );
    }

    public void PutBits ( long bits, int length )
    {
      bitStream.WriteBits( bits, length );
    }

    public int Context
    {
      get
      {
        return currentContext;
      }
      set
      {
        setContextInner( value );
      }
    }
  }
}
