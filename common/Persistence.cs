using System.Collections.Generic;
using System.IO;
using System.Text;
using System;

namespace Utilities
{
  /// <summary>
  /// General fast object persistence in simple text files.
  /// Record prolog (single record): <code>|ClassName|Version|Type</code>
  /// Array prolog: <code>|ClassName|Version|Type|[size]</code>
  /// Array separator: <code>||</code>
  /// </summary>
  public interface ITextPersistent
  {
    /// <summary>
    /// Writes (some) information about the object instance to the given stream.
    /// </summary>
    /// <param name="wri">Stream to write to.</param>
    /// <param name="separator">Separator character.</param>
    /// <param name="embedded">Embedded format? (brief format w/o object header)</param>
    /// <param name="type"></param>
    /// <returns>True if the record was written</returns>
    bool TextWrite ( TextWriter wri, char separator, bool embedded =false, int type =0 );

    /// <summary>
    /// Writes the array prolog to the given stream.
    /// </summary>
    /// <param name="wri">Stream to write to.</param>
    /// <param name="separator">Separator character.</param>
    /// <param name="type"></param>
    void TextWriteArrayProlog ( TextWriter wri, char separator, int type =0 );

    /// <summary>
    /// Creates a new object instance and reads (some) information to it.
    /// Object header (ClassName|version|type) was already read - see method arguments.
    /// </summary>
    /// <param name="rea">Stream to read from.</param>
    /// <param name="separator">Separator character.</param>
    /// <param name="firstLine">In: already read 1st line of the object, out: one line past the object data.</param>
    /// <param name="version">Object version (read from common array header).</param>
    /// <param name="type">Type of the data record (non-mandatory, read from common array header).</param>
    /// <returns></returns>
    ITextPersistent TextRead ( TextReader rea, char separator, ref string firstLine, int version, int type =0 );

    /// <summary>
    /// String id of the class.
    /// </summary>
    string PersName
    { get; }

    /// <summary>
    /// Approx. size of occupied memory in bytes.
    /// </summary>
    int Occupied
    { get; }

    /// <summary>
    /// Retrieves the string-statistics of the object.
    /// </summary>
    /// <param name="result">[0] .. number of strings, [1] .. total string length, [2] .. number of interned strings, [3] .. total length of interned strings</param>
    void StringStatistics ( long[] result );
  }

  /// <summary>
  /// Persistence manager.
  /// </summary>
  public class TextPersistence
  {
    /// <summary>
    /// Set of tags.
    /// </summary>
    public static Dictionary<string, ITextPersistent> Registry = new Dictionary<string, ITextPersistent>();

    public static void Register ( ITextPersistent mother, int version )
    {
      Registry.Add( mother.PersName, mother );   // version is ignored so far
    }

    /// <summary>
    /// Returns a "mother" instance for deserialization.
    /// </summary>
    /// <param name="name">Class name for persistance.</param>
    /// <returns>Mother class or null.</returns>
    public static ITextPersistent GetMother ( string name )
    {
      ITextPersistent result;
      if ( Registry.TryGetValue( name, out result ) )
        return result;
      return null;
    }

    public static ITextPersistent GetMother ( Type T )
    {
      foreach ( var m in Registry.Values )
        if ( m.GetType().Equals( T ) )
          return m;
      return null;
    }

    public static ITextPersistent ObjectRead ( TextReader rea, char separator, ref string firstLine )
    {
      if ( firstLine == null ) firstLine = rea.ReadLine();
      int version, type;
      if ( firstLine.Length < 6 ) return null;

      string[] tokens = firstLine.Split( separator );
      if ( tokens.Length < 4 ) return null;

      ITextPersistent mother = GetMother( tokens[ 1 ] );
      if ( mother == null ||
           !int.TryParse( tokens[ 2 ], out version ) ||
           !int.TryParse( tokens[ 3 ], out type ) )
        return null;
      firstLine = null;

      return mother.TextRead( rea, separator, ref firstLine, version, type );
    }

    /// <summary>
    /// |class|version|type|[size]
    /// </summary>
    public static List<ITextPersistent> ArrayRead ( TextReader rea, char separator, ref string firstLine, HashSet<string> banned =null )
    {
      if ( firstLine == null ) firstLine = rea.ReadLine();
      int version, type;
      if ( firstLine.Length < 3 ) return null;
      string[] tokens = firstLine.Split( separator );
      if ( tokens.Length < 5 ) return null;

      if ( banned != null && banned.Contains( tokens[ 1 ] ) )
        return null;

      ITextPersistent mother = GetMother( tokens[ 1 ] );
      if ( mother == null ||
           !int.TryParse( tokens[ 2 ], out version ) ||
           !int.TryParse( tokens[ 3 ], out type ) )
        return null;

      List<ITextPersistent> l = new List<ITextPersistent>();
      do
      {
        firstLine = rea.ReadLine();
        if ( firstLine == null ) break;
        if ( firstLine.Length == 2 && firstLine[ 0 ] == separator && firstLine[ 1 ] == separator )
        {
          firstLine = null;
          break;
        }
        ITextPersistent elem = mother.TextRead( rea, separator, ref firstLine, version, type );
        if ( elem == null ) break;
        l.Add( elem );
      } while ( true );

      return l;
    }

    public static void ArrayWriteProlog ( TextWriter wri, char separator, ITextPersistent classInstance, int version, int type )
    {
      wri.WriteLine( "{0}{1}{0}{2}{0}{3}{0}", separator, classInstance.PersName, version, type );
    }

    public static void ArrayWriteEpilog ( TextWriter wri, char separator )
    {
      wri.WriteLine( "{0}{0}", separator );
    }

    /// <summary>
    /// Filename format string for SYT files (1st format int parameter is expected).
    /// </summary>
    public static string PathFormatString ( string initialPath )
    {
      return PathInsert( initialPath, "{0:000}" );
    }

    static string PathInsert ( string initialPath, string inset )
    {
      string[] pathParts = initialPath.Split( '.' );
      inset = "." + inset + '.';

      if ( pathParts.Length < 2 )
        return initialPath + inset + "syt";

      if ( pathParts.Length < 3 )
        return (pathParts[ 0 ] + inset + pathParts[ 1 ]);

      StringBuilder sb = new StringBuilder( pathParts[ 0 ] );
      int i = 1;
      while ( i < pathParts.Length - 2 )
        sb.Append( pathParts[ i++ ] );
      sb.Append( inset ).Append( pathParts[ i ] );

      return sb.ToString();
    }

    /// <summary>
    /// File-mask for deleting all previous files.
    /// </summary>
    public static string PathDelete ( string initialPath )
    {
      return PathInsert( initialPath, "???" );
    }
  }
}
