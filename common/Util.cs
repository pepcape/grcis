using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.Versioning;
using System.Text;

namespace Utilities
{
  /// <summary>
  /// Assorted utilities.
  /// </summary>
  class Util
  {
    /// <summary>
    /// One day in seconds.
    /// </summary>
    public const long ONE_DAY = 24 * 3600L;

    /// <summary>
    /// Positive value (1, yes, y, true, ano, a)
    /// </summary>
    public static bool positive ( string val )
    {
      if ( val == null ) return false;

      int i;
      if ( int.TryParse( val, out i ) )
        return (i > 0);

      char answ = char.ToLower( val[ 0 ] );
      return (answ == 'y' || answ == 'a' || answ == 't');
    }

    /// <summary>
    /// Strictly positive value (1, yes, true, ano)
    /// </summary>
    public static bool positiveStrict ( string val )
    {
      if ( val == null )
        return false;

      val = val.ToLower();
      return (val == "1" || val == "yes" || val == "true" || val == "ano");
    }

    /// <summary>
    /// Yes/no from a boolean.
    /// </summary>
    public static string YesNo ( bool b )
    {
      return b ? "yes" : "no";
    }

    public static double percent ( double count, double total )
    {
      return (100.0 * count) / Math.Max( total, 1.0 );
    }

    public static double percent ( long count, long total )
    {
      return (100.0 * count) / Math.Max( total, 1L );
    }

    public static double percent ( int count, int total )
    {
      return (100.0 * count) / Math.Max( total, 1 );
    }

    /// <summary>
    /// Checks validity of the given IP address.
    /// </summary>
    /// <param name="ip">IP address as a string.</param>
    /// <returns>True if the IP-format is OK.</returns>
    public static bool IsIpAddress ( string ip )
    {
      int len = ip.Length;
      if ( len < 7 || !char.IsDigit( ip, 0 ) )
        return false;
      int i = 1;

      for ( int j = 0; j++ < 3; )
      {
        while ( i < len && char.IsDigit( ip, i ) )
          i++;
        if ( i >= len - 1 || ip[ i ] != '.' || !char.IsDigit( ip, i + 1 ) )
          return false;
        i += 2;
      }

      while ( i < len && char.IsDigit( ip, i ) )
        i++;
      return (i == len);
    }

    /// <summary>
    /// Returns numerical prefix of the given string.
    /// </summary>
    /// <param name="val">Non-null string.</param>
    /// <returns>Non-null string (decimal number).</returns>
    public static string NumberPrefix ( string val )
    {
      int len = val.Length;
      int i = 0;
      while ( i < len && char.IsDigit( val, i ) )
        i++;

      return (i < len) ? val.Substring( 0, i ) : val;
    }

    /// <summary>
    /// Prints time value
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    public static string FormatTime ( long time )
    {
      long seconds = time % 60L;
      time /= 60L;
      long minutes = time % 60L;
      time /= 60L;
      long hours = time % 24L;
      time /= 24L;
      return( time == 0L ?
              (hours == 0L ? string.Format( "{0}:{1:d2}", minutes, seconds ) : string.Format( "{0}:{1:d2}:{2:d2}", hours, minutes, seconds ) ) :
              string.Format( "{0}:{1:d2}:{2:d2}:{3:d2}", time, hours, minutes, seconds ) );
    }

    /// <summary>
    /// POSIX time-stamp to DateTime conversion
    /// </summary>
    public static DateTime UnixTimeStampToDateTime ( long unixTimeStamp )
    {
      // POSIX timestamp is seconds past epoch
      DateTime epoch = new DateTime( 1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc );
      return epoch.AddSeconds( unixTimeStamp ).ToLocalTime();
    }

    /// <summary>
    /// POSIX DateTime to time-stamp conversion
    /// </summary>
    public static long DateTimeToUnixTimeStamp ( DateTime dt )
    {
      // POSIX timestamp is seconds past epoch
      DateTime epoch = new DateTime( 1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc );
      return (long)dt.ToUniversalTime().Subtract( epoch ).TotalSeconds;
    }

    /// <summary>
    /// Returns time relative to current local time.
    /// </summary>
    /// <param name="rel">Time-delta in seconds.</param>
    /// <returns>POSIX time-stamp format of the result.</returns>
    public static long RelativeTime ( long rel )
    {
      DateTime now = DateTime.Now.AddSeconds( rel );
      return Util.DateTimeToUnixTimeStamp( now );
    }

    /// <summary>
    /// Parse relative time (w, d, h, m suffixes are allowed).
    /// </summary>
    /// <param name="value">Input value.</param>
    /// <param name="time">Output value (left unchanged if not valid input).</param>
    /// <returns>True if changed.</returns>
    public static bool ParseRelativeTime ( string value, ref long time )
    {
      char suff = 's';
      int len = value.Length;
      if ( char.IsLetter( value, len - 1 ) )
      {
        suff = char.ToLower( value[ len - 1 ] );
        value = value.Substring( 0, len - 1 );
      }

      long newLong;
      if ( !long.TryParse( value, out newLong ) )
        return false;

      switch ( suff )
      {
        case 'w':
          newLong *= 7L * Util.ONE_DAY;
          break;
        case 'd':
          newLong *= Util.ONE_DAY;
          break;
        case 'h':
          newLong *= 3600L;
          break;
        case 'm':
          newLong *= 60L;
          break;
      }
      time = newLong;
      return true;
    }

    /// <summary>
    /// Remove diacritics from a string.
    /// </summary>
    public static string RemoveDiacritics ( string stIn )
    {
      if ( stIn == null || stIn.Length == 0 )
        return "";

      string stFormD = stIn.Normalize( NormalizationForm.FormD );
      StringBuilder sb = new StringBuilder();

      for ( int ich = 0; ich < stFormD.Length; ich++ )
      {
        UnicodeCategory uc = CharUnicodeInfo.GetUnicodeCategory( stFormD[ ich ] );
        if ( uc != UnicodeCategory.NonSpacingMark )
          sb.Append( stFormD[ ich ] );
      }

      return sb.ToString().Normalize( NormalizationForm.FormC );
    }

    /// <summary>
    /// Looks for the required file in many locations.
    /// </summary>
    /// <param name="fileName">Simple file-name (w/o directories).</param>
    /// <param name="hint">Optional hint directory.</param>
    /// <returns>File-name or null if not found.</returns>
    public static string FindSourceFile ( string fileName, string hint =null )
    {
      if ( fileName == null ||
           fileName.Length == 0 )
        return null;

      // 1. local folder
      if ( File.Exists( fileName ) )
        return fileName;

      // 2. parent folders
      string fn = "../" + fileName;
      if ( File.Exists( fn ) )
        return fn;

      fn = "../" + fn;
      if ( File.Exists( fn ) )
        return fn;

      fn = "../" + fn;
      if ( File.Exists( fn ) )
        return fn;

      if ( hint != null ||
           hint.Length == 0 )
        return null;

      // 3. hint folder tries
      fn = "../" + hint + '/' + fileName;
      if ( File.Exists( fn ) )
        return fn;

      fn = "../" + fn;
      if ( File.Exists( fn ) )
        return fn;

      fn = "../" + fn;
      if ( File.Exists( fn ) )
        return fn;

      return null;
    }

    /// <summary>
    /// Separator for the config-file lists.
    /// </summary>
    public const char COMMA = ',';

    public static string[] ParseList ( string value )
    {
      string[] list = value.Split( COMMA );
      if ( list.Length < 1 ) return null;

      for ( int i = 0; i < list.Length; i++ )
        list[ i ] = list[ i ].Trim();

      return list;
    }

    /// <summary>
    /// Parses integer value from the dictionary.
    /// </summary>
    /// <returns>True if everything went well.</returns>
    public static bool TryParse ( Dictionary<string, string> rec, string key, ref int val )
    {
      string sval;
      return ( rec.TryGetValue( key, out sval ) &&
               int.TryParse( sval, out val ) );
    }

    /// <summary>
    /// Parses long value from the dictionary.
    /// </summary>
    /// <returns>True if everything went well.</returns>
    public static bool TryParse ( Dictionary<string, string> rec, string key, ref long val )
    {
      string sval;
      return ( rec.TryGetValue( key, out sval ) &&
               long.TryParse( sval, out val ) );
    }

    /// <summary>
    /// Parses float value from the dictionary.
    /// </summary>
    /// <returns>True if everything went well.</returns>
    public static bool TryParse ( Dictionary<string, string> rec, string key, ref float val )
    {
      string sval;
      return ( rec.TryGetValue( key, out sval ) &&
               float.TryParse( sval, NumberStyles.Number, CultureInfo.InvariantCulture, out val ) );
    }

    /// <summary>
    /// Parses double value from the dictionary.
    /// </summary>
    /// <returns>True if everything went well.</returns>
    public static bool TryParse ( Dictionary<string, string> rec, string key, ref double val )
    {
      string sval;
      return ( rec.TryGetValue( key, out sval ) &&
               double.TryParse( sval, NumberStyles.Number, CultureInfo.InvariantCulture, out val ) );
    }

    /// <summary>
    /// Parses boolean value from the dictionary.
    /// </summary>
    /// <returns>True if everything went well.</returns>
    public static bool TryParse ( Dictionary<string, string> rec, string key, ref bool val )
    {
      string sval;
      if ( !rec.TryGetValue( key, out sval ) )
        return false;

      val = positive( sval );
      return true;
    }

    /// <summary>
    /// Returns first defined value (for the given keys) or null.
    /// </summary>
    public static string FirstDefined ( Dictionary<string, string> rec, string[] keys )
    {
      string val;
      foreach ( var key in keys )
        if ( rec.TryGetValue( key, out val ) )
          return val;
      return null;
    }

    /// <summary>
    /// Returns first defined nonempty value (for the given keys) or null.
    /// </summary>
    public static string FirstDefinedNonempty ( Dictionary<string, string> rec, string[] keys )
    {
      string val;
      foreach ( var key in keys )
        if ( rec.TryGetValue( key, out val ) &&
             val.Length > 0 )
          return val;
      return null;
    }

    /// <summary>
    /// Returns first defined nonempty value only from letters (for the given keys) or null.
    /// </summary>
    public static string FirstDefinedLetters ( Dictionary<string, string> rec, string[] keys )
    {
      string val;
      foreach ( var key in keys )
        if ( rec.TryGetValue( key, out val ) )
        {
          int len = val.Length;
          int i = 0;
          while ( i < len && char.IsLetter( val, i ) ) i++;
          if ( i > 0 )
            return (i == len) ? val : val.Substring( 0, i );
        }
      return null;
    }

    /// <summary>
    /// Returns first defined integer value (for the given keys) or null.
    /// </summary>
    public static string FirstDefinedInteger ( Dictionary<string, string> rec, string[] keys )
    {
      string val;
      foreach ( var key in keys )
        if ( rec.TryGetValue( key, out val ) )
        {
          val = Util.NumberPrefix( val );
          if ( val.Length > 0 )
            return val;
        }
      return null;
    }

    public static KeyValuePair<string, string> Pair<T> ( string key, T val )
    {
      return new KeyValuePair<string, string>( key, val.ToString() );
    }

    /// <summary>
    /// Splits the given string into parts using '|' as separator,
    /// handles "\|" sequences correctly..
    /// </summary>
    /// <param name="str">String to be split.</param>
    /// <param name="start">Starting index.</param>
    /// <returns>List of non-null strings.</returns>
    public static List<string> BarSplit ( string str, int start )
    {
      int len = str.Length;
      if ( start > len )
        return null;

      List<string> res = new List<string>();
      if ( start == len )
      {
        res.Add( "" );
        return res;
      }

      StringBuilder sb = null;
      string seg;
      do
      {
        int pos = str.IndexOf( '|', start );

        if ( pos < 0 )
        {
          seg = str.Substring( start );
          if ( sb != null )
          {
            sb.Append( seg );
            res.Add( sb.ToString() );
            sb = null;
          }
          else
            res.Add( seg );
          break;
        }

        if ( pos == start )
        {
          if ( sb != null )
          {
            res.Add( sb.ToString() );
            sb = null;
          }
          else
            res.Add( "" );
        }
        else
          if ( str[ pos - 1 ] == '\\' )
          {                               // escaped | => keep it
            if ( sb == null )
              sb = new StringBuilder();
            sb.Append( str.Substring( start, pos - 1 - start ) ).Append( '|' );
          }
          else
          {                               // regular | => separator
            seg = str.Substring( start, pos - start );
            if ( sb != null )
            {
              sb.Append( seg );
              res.Add( sb.ToString() );
              sb = null;
            }
            else
              res.Add( seg );
          }
        start = pos + 1;
      }
      while ( true );

      return res;
    }

    /// <summary>
    /// Converts a &-separated list into a sequence of key=value tuples.
    /// </summary>
    /// <param name="list">Result sequence, already initialized (and possibly nonempty).</param>
    /// <param name="prefix">Key-prefix.</param>
    /// <param name="str">String to parse.</param>
    /// <param name="start">Where to start..</param>
    /// <param name="separator">Optional specification of the separator character.</param>
    /// <param name="keepEmpty">Keep empty values?</param>
    public static void ParseList ( List<KeyValuePair<string, string>> list, string prefix, string str, int start, bool keepEmpty =false, char separator ='&' )
    {
      int len = str.Length;
      while ( start < len )
      {
        int end = str.IndexOf( separator, start );
        if ( end == start )
        {
          start++;
          continue;
        }
        if ( end < 0 ) end = len;
        int eq = str.IndexOf( '=', start );
        if ( eq != start )
        {
          if ( eq < 0 || eq > end )  // only key (tag) is present, assume empty value..
            eq = end;
          string value = (eq < end - 1) ? str.Substring( eq + 1, end - eq - 1 ) : "";
          if ( keepEmpty || value.Length > 0 )
          {
            string key = str.Substring( start, eq - start );
            list.Add( Pair( prefix + key, value ) );
          }
        }
        start = end + 1;
      }
    }

    /// <summary>
    /// Converts a comma-separated list into a dictionary of [key,value] tuples.
    /// </summary>
    /// <param name="str">String to parse.</param>
    /// <param name="separator">Optional specification of the separator character.</param>
    public static Dictionary<string,string> ParseKeyValueList ( string str, char separator =',' )
    {
      Dictionary<string, string> result = new Dictionary<string, string>();
      if ( str == null )
        return result;

      int len = str.Length;
      int start = 0;
      while ( start < len )
      {
        int end = str.IndexOf( separator, start );
        if ( end == start )
        {
          start++;
          continue;
        }
        if ( end < 0 ) end = len;
        int eq = str.IndexOf( '=', start );
        if ( eq != start )
        {
          if ( eq < 0 || eq > end )  // only key (tag) is present, assume empty value..
            eq = end;
          string value = (eq < end - 1) ? str.Substring( eq + 1, end - eq - 1 ) : "";
          string key = str.Substring( start, eq - start );
          result[ key.Trim() ] = value.Trim();
        }
        start = end + 1;
      }

      return result;
    }

    public static int OccupiedString ( string s )
    {
      if ( s == null ||
           String.IsInterned( s ) != null )
        return 4;

      int occ = 8 + 4 + 2 + 2 * s.Length;
      return ((occ + 3) & -4);
    }

    public static void StringStat ( string s, long[] result )
    {
      if ( s == null || result == null || result.Length < 4 ) return;

      int len = s.Length;
      result[ 0 ]++;
      result[ 1 ] += len;
      if ( String.IsInterned( s ) != null )
      {
        result[ 2 ]++;
        result[ 3 ] += len;
      }
    }

    public static void StringStat ( IEnumerable<string> ss, long[] result )
    {
      if ( ss == null || result == null || result.Length < 4 ) return;

      foreach ( string s in ss )
        StringStat( s, result );
    }

    public static string kmg ( long n )
    {
      if ( n < 8192L ) return n.ToString();
      n >>= 10;
      if ( n < 8192L ) return string.Format( "{0}K", n );
      n >>= 10;
      if ( n < 8192L ) return string.Format( "{0}M", n );
      return string.Format( "{0}G", n >> 10 );
    }

    public static string logFileName = "log.txt";

    static bool firstLog = true;

    /// <summary>
    /// Logging (log message is always appended to the log-file).
    /// </summary>
    /// <param name="msg">The explicit message.</param>
    public static string Log ( string msg )
    {
      try
      {
        lock ( logFileName )
          using ( StreamWriter log = new StreamWriter( logFileName, true ) )
          {
            if ( firstLog )
              log.WriteLine();
            log.WriteLine( string.Format( "{0}: {1}", DateTime.UtcNow.ToString( "yyyy-MM-dd HH:mm:ss" ), msg ) );
          }
      }
      catch ( IOException e )
      {
        Console.WriteLine( "Log - unhandled IOException: " + e.Message );
        Console.WriteLine( "Stack: " + e.StackTrace );
      }
      firstLog = false;

      return msg;
    }

    /// <summary>
    /// Logging (log message is always appended to the log-file).
    /// </summary>
    /// <param name="fmt">Format string.</param>
    /// <param name="pars">Values to substitute.</param>
    public static string LogFormat ( string fmt, params object[] pars )
    {
      fmt = string.Format( CultureInfo.InvariantCulture, fmt, pars );
      return Log( fmt );
    }

    public static void GetFrameworkVersions ( out string targetFramework, out string runningFramework )
    {
      // .NET CLR version this program was build against:
      Assembly assembly = Assembly.GetExecutingAssembly();
      targetFramework = "Unknown";
      object[] targetFrameworkAttributes = assembly.GetCustomAttributes( typeof( System.Runtime.Versioning.TargetFrameworkAttribute ), true );
      if ( targetFrameworkAttributes.Length > 0 )
      {
        TargetFrameworkAttribute targetFrameworkAttribute = targetFrameworkAttributes[ 0 ] as TargetFrameworkAttribute;
        if ( targetFrameworkAttribute != null )
          targetFramework = targetFrameworkAttribute.FrameworkDisplayName;
      }

      // currently running framework version:
      // System.Runtime.InteropServices.RuntimeEnvironment.GetSystemVersion()
      runningFramework = Environment.Version.ToString();
    }
  }

  /// <summary>
  /// Class for computing ETF (Estimated Time of Finish).
  /// </summary>
  public class ETF
  {
    float lastTime;
    float lastFinished;
    float total;

    public ETF ()
    {
      lastTime = 0.0f;
      lastFinished = 0.0f;
      total = 0.0f;
    }

    /// <summary>
    /// 60% of current speed, 40% of global state
    /// </summary>
    /// <param name="time">Currently spent time.</param>
    /// <param name="finished">Currently finished part.</param>
    /// <param name="etf">Estimated time to finish.</param>
    /// <returns>Total estimate</returns>
    public float Estimate ( float time, float finished, out float etf )
    {
      if ( finished <= float.Epsilon )
        return etf = 0.0f;

      float total0 =  time / finished;
      if ( total == 0.0f )
        total = total0;
      float total1 = (finished == lastFinished) ? total : (time - lastTime) / (finished - lastFinished);
      total = 0.5f * (total + total1);
      lastTime = time;
      lastFinished = finished;
      float tot = (total0 * 0.4f + total * 0.6f);
      etf = (1.0f - finished) * tot;
      return tot;
    }
  }

  /// <summary>
  /// Reverse variant of any IComparable<T>
  /// </summary>
  /// <typeparam name="T">Type to compare.</typeparam>
  public sealed class ReverseComparer<T> : IComparer<T> where T : IComparable<T>
  {
    public int Compare ( T x, T y )
    {
      if ( x == null )        // handle nulls according to MSDN
        return (y == null) ? 0 : 1;

      if ( y == null )        // y null but x not, so x > y, but reversing to -1!
        return -1;

      // if neither arg is null, pass on to CompareTo in reverse order.
      return y.CompareTo( x );
    }
  }

  /// <summary>
  /// Simple histogram. Each unique object has its own counter.
  /// </summary>
  /// <typeparam name="T">Any type usable as a key in a Dictionary.</typeparam>
  public class Histogram<T> : Dictionary<T, long>
  {
    public long Inc ( T key )
    {
      long count = 0L;
      if ( TryGetValue( key, out count ) )
        this[ key ] = count + 1;
      else
        Add( key, 1L );
      return count;
    }

    /// <summary>
    /// Print the histogram (sorted by frequency or key).
    /// </summary>
    /// <param name="sb">Output to write to.</param>
    /// <param name="comp">Comparison object or null for sorting by frequency.</param>
    /// <param name="limit">Lines limit (0 means "no limits")</param>
    public void Print ( StringBuilder sb, IComparer<T> comp =null, int limit =0 )
    {
      if ( limit == 0 )
        limit = Count;

      if ( comp == null )       // sorted by frequency
      {
        SortedSet<long> freq = new SortedSet<long>( Values, new ReverseComparer<long>() );
        HashSet<T> done = new HashSet<T>();
        foreach ( var val in freq )
          foreach ( var key in Keys )
            if ( this[ key ] == val && !done.Contains( key ) )
            {
              sb.AppendFormat( "{0,20}: {1,10}{2}", key.ToString(), val, Environment.NewLine );
              if ( --limit <= 0 )
                break;
              done.Add( key );
            }
      }
      else                      // sorted by key
      {
        SortedDictionary<T, long> sorted = new SortedDictionary<T, long>( this, comp );
        foreach ( var kvp in sorted )
        {
          sb.AppendFormat( "{0,20}: {1,10}{2}", kvp.Key.ToString(), kvp.Value, Environment.NewLine );
          if ( --limit <= 0 )
            break;
        }
      }
    }

    /// <summary>
    /// Print the histogram (sorted by frequency or key).
    /// </summary>
    /// <param name="o">Output to write to.</param>
    /// <param name="comp">Comparison object or null for sorting by frequency.</param>
    /// <param name="limit">Lines limit (0 means "no limits")</param>
    public void Print ( TextWriter o, IComparer<T> comp =null, int limit =0 )
    {
      StringBuilder sb = new StringBuilder();
      Print( sb, comp, limit );
      o.Write( sb.ToString() );
    }
  }

  /// <summary>
  /// Int-keyed histogram with arbitrary bin size.
  /// </summary>
  public class HistogramInt: Histogram<int>
  {
    public int BinSize
    {
      get;
      set;
    }

    public HistogramInt ( int binSize =1 )
    {
      BinSize = binSize;
    }

    public new long Inc ( int key )
    {
      key /= BinSize;
      long count = 1L;
      if ( TryGetValue( key, out count ) )
        this[ key ] = ++count;
      else
        Add( key, 1L );
      return count;
    }

    public new void Print ( TextWriter o, IComparer<int> comp, int limit =0 )
    {
      SortedDictionary<int, long> sorted = new SortedDictionary<int, long>( this, comp );
      if ( limit == 0 ) limit = sorted.Count;
      foreach ( var kvp in sorted )
      {
        o.WriteLine( "{0,20}: {1,10}", kvp.Key * BinSize, kvp.Value );
        if ( --limit <= 0 ) break;
      }
    }

    public void PrintHtml ( StringBuilder sb, IComparer<int> comp, int limit =0 )
    {
      SortedDictionary<int, long> sorted = new SortedDictionary<int, long>( this, comp );
      if ( limit == 0 ) limit = sorted.Count;
      foreach ( var kvp in sorted )
      {
        sb.AppendFormat( " <tr><td align=\"right\">{0}</td><td align=\"right\">{1}</td></tr>\r\n", kvp.Key * BinSize, kvp.Value );
        if ( --limit <= 0 ) break;
      }
    }
  }

  /// <summary>
  /// Long-keyed histogram with arbitrary bin size.
  /// </summary>
  public class HistogramLong : Histogram<long>
  {
    public long BinSize
    {
      get;
      set;
    }

    public HistogramLong ( long binSize =1 )
    {
      BinSize = binSize;
    }

    public new long Inc ( long key )
    {
      key /= BinSize;
      long count = 1L;
      if ( TryGetValue( key, out count ) )
        this[ key ] = ++count;
      else
        Add( key, 1L );
      return count;
    }

    public new void Print ( TextWriter o, IComparer<long> comp, int limit =0 )
    {
      SortedDictionary<long, long> sorted = new SortedDictionary<long, long>( this, comp );
      if ( limit == 0 ) limit = sorted.Count;
      foreach ( var kvp in sorted )
      {
        o.WriteLine( "{0,20}: {1,10}", kvp.Key * BinSize, kvp.Value );
        if ( --limit <= 0 ) break;
      }
    }
  }
}
