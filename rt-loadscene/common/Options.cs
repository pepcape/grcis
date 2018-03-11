using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Utilities
{
  /// <summary>
  /// Global program options - abstract / support class.
  /// </summary>
  public abstract class Options : ITextPersistent
  {
    public const string PERS_NAME = "options";

    /// <summary>
    /// Singleton instance.
    /// </summary>
    public static Options options = null;

    /// <summary>
    /// Start time of the application.
    /// </summary>
    public static DateTime startMain = DateTime.UtcNow;

    /// <summary>
    /// Project name (used in SYT headers, SYT file-names, etc.)
    /// </summary>
    public static string project = null;

    /// <summary>
    /// Global ITextPersistent save path.
    /// </summary>
    public static string SavePath
    {
      get
      {
        return project + ".syt";
      }
    }

    /// <summary>
    /// Normalizes the directory path, appends dir-separator to it.
    /// </summary>
    public static string NormalizeDir ( string path )
    {
      return path.TrimEnd( '/', '\\' ) + Path.DirectorySeparatorChar;
    }

    /// <summary>
    /// Combines the given directory name with the standard file mask.
    /// In case of empty/null input dir returns null as well.
    /// </summary>
    public static string CombineSavePath ( string path )
    {
      if ( string.IsNullOrEmpty( path ) )
        return null;

      return NormalizeDir( path ) + SavePath;
    }

    /// <summary>
    /// Registry of all known messaging-modes (for typo checks).
    /// </summary>
    public static HashSet<string> knownMsgModes = new HashSet<string>();

    /// <summary>
    /// Registers (recognized) message modes.
    /// </summary>
    /// <param name="modes">List of all recognized modes to be added.</param>

    public static void RegisterMsgModes ( params string[] modes )
    {
      if ( modes == null ||
           modes.Length == 0 )
        return;

      foreach ( var mode in modes )
        knownMsgModes.Add( string.Intern( mode ) );
    }

    /// <summary>
    /// Messaging modes currently turned on.
    /// </summary>
    public HashSet<string> msgModes = new HashSet<string>();

    public HashSet<string> PushMsgModes ()
    {
      return new HashSet<string>( msgModes );
    }

    public void PopMsgModes ( HashSet<string> modes )
    {
      msgModes.Clear();
      msgModes.UnionWith( modes );
    }

    /// <summary>
    /// [Re]sets current message-mode.
    /// </summary>
    /// <param name="modes">Modes separated by ',', leading '+' for adding a new modes, '-' for removing modes.</param>
    /// <returns>True if message modes were altered.</returns>
    public bool SetMsgMode ( string modes )
    {
      if ( modes == null ||
           (modes = modes.Trim()).Length == 0 )
      {
        msgModes.Clear();
        return true;
      }

      bool add = true;
      bool change = false;

      if ( modes[ 0 ] == '+' ||
           modes[ 0 ] == '-' )
      {
        add = (modes[ 0 ] == '+');
        modes = modes.Substring( 1 );
      }
      else
      {
        msgModes.Clear();
        change = true;
      }

      string[] tokens = Util.ParseList( modes );
      if ( tokens == null )
        return change;

      foreach ( var mode in tokens )
      {
        bool localAddRemove = (mode[ 0 ] == '+' || mode[ 0 ] == '-');
        string modeNetto = localAddRemove ? mode.Substring( 1 ) : mode;

        if ( modeNetto != "verbose" &&
             !knownMsgModes.Contains( modeNetto ) )
          Util.LogFormat( "Unknown message mode: '{0}' from '{1}'", modeNetto, modes );

        if ( (localAddRemove && mode[ 0 ] == '+') ||
             (!localAddRemove && add) )
        {
          if ( msgModes.Add( string.Intern( modeNetto ) ) )
            change = true;
        }
        else
        {
          if ( msgModes.Remove( modeNetto ) )
            change = true;
        }
      }

      return change;
    }

    /// <summary>
    /// Returns if the given message-mode is set.
    /// </summary>

    public bool SingleMsgMode ( string mode )
    {
      if ( msgModes.Contains( mode ) )
        return true;

      if ( !knownMsgModes.Contains( mode ) )
        Util.LogFormat( "Unknown message mode: '{0}'", mode );

      return false;
    }

    /// <summary>
    /// Returns true if any of given message-mode[s] is[are] set.
    /// Uses logical OR.
    /// </summary>
    /// <param name="modes">One or more modes (comma-separated).</param>
    public bool MsgMode ( string modes )
    {
      if ( string.IsNullOrEmpty( modes ) )
        return true;

      if ( modes.IndexOf( ',' ) < 0 )
        return SingleMsgMode( modes );

      string[] tokens = Util.ParseList( modes );
      if ( tokens == null )
        return true;

      foreach ( var mode in tokens )
        if ( SingleMsgMode( mode ) )
          return true;

      return false;
    }

    /// <summary>
    /// Number of Touch() calls before next memory-cleanup will be performed..
    /// </summary>
    public int memoryCheckPeriod = 500;

    /// <summary>
    /// Output file-name.
    /// </summary>
    public string outputFileName = "";

    /// <summary>
    /// Command-line has priority over config file.
    /// </summary>
    public bool priorityOutputFileName = false;

    /// <summary>
    /// Log file-name.
    /// </summary>
    public string logFileName = "log.txt";

    /// <summary>
    /// Command-line has priority over config file.
    /// </summary>
    public bool priorityLogFileName = false;

    /// <summary>
    /// Base directory to read input files from.
    /// </summary>
    public string baseDir = "";

    /// <summary>
    /// Ordered list of input file-names
    /// </summary>
    public List<string> inputFiles = new List<string>();

    /// <summary>
    /// Format for the absolute dates..
    /// </summary>
    public const string DATE_FORMAT_STRING = "yyyy'-'MM'-'dd";

    /// <summary>
    /// List of commands to execute.
    /// Won't be saved in the SYT file!
    /// </summary>
    public List<string> commands = new List<string>();

    /// <summary>
    /// Add the nonempty string to the command sequence.
    /// </summary>
    /// <returns>True if implemented (handled).</returns>
    public bool AddCommand ( string command )
    {
      commands.Add( command );
      return true;
    }

    /// <summary>
    /// Append a new line to the alert-message.
    /// Messages are completed and sent using SendEmails().
    /// </summary>
    public virtual void EmailAlert ( string msg )
    {
    }

    /// <summary>
    /// Append a new line to the info-message.
    /// Messages are completed and sent using SendEmails().
    /// </summary>
    public virtual void EmailInfo ( string msg )
    {
    }

    /// <summary>
    /// Sends accumulated alert and info messages.
    /// </summary>
    public virtual void SendEmails ()
    {
    }

    /// <summary>
    /// Backup file-name mask. log-adreq.txt -> log-adreq.001.txt
    /// {0} .. base file-name (w/o extension)
    /// {1} .. backup number (1, 2, ...)
    /// {2} .. extension including leading dot
    /// </summary>
    public string backupLogFileMask = "{0}.{1:0000}{2}";

    /// <summary>
    /// Maximum log-file size in bytes.
    /// </summary>
    public long backupLogMaxSize = 5L * 1024 * 1024;

    /// <summary>
    /// Retrieves log-file stream including too-large file handling.
    /// </summary>
    public TextWriter GetLogFile ( string origFileName, long coeff =1L )
    {
      StreamWriter stream = new StreamWriter( origFileName, true );
      if ( stream.BaseStream.Position <= backupLogMaxSize * coeff ) return stream;
      stream.Close();

      // determine the 1st free backup log file-name:
      int i = 1;
      string baseFileName = Path.GetFileNameWithoutExtension( origFileName );
      string extension = Path.GetExtension( origFileName );
      string currFileName;
      do
      {
        currFileName = string.Format( backupLogFileMask, baseFileName, i, extension );
        if ( !File.Exists( currFileName ) &&
             !File.Exists( currFileName + ".gz" ) ) break;
        i++;
      }
      while ( true );

      // rename current log-file to the 1st available backup name:
      File.Move( origFileName, currFileName );

      return new StreamWriter( origFileName, false );
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

    /// <summary>
    /// Logging (log message is appended to the log-file).
    /// </summary>
    /// <param name="modes">Message mode[s].</param>
    /// <param name="fmt">Format string.</param>
    /// <param name="pars">Values to substitute.</param>
    public static string LogFormatMode ( string modes, string fmt, params object[] pars )
    {
      if ( !options.MsgMode( modes ) )
        return null;

      fmt = string.Format( CultureInfo.InvariantCulture, fmt, pars );
      return Log( fmt );
    }

    /// <summary>
    /// Logging (log message is appended to the log-file).
    /// </summary>
    /// <param name="modes">Message mode[s].</param>
    /// <param name="msg">The explicit message.</param>
    public static string Log ( string modes, string msg )
    {
      if ( !options.MsgMode( modes ) )
        return null;

      return Log( msg );
    }

    /// <summary>
    /// Logging (log message is always appended to the log-file).
    /// </summary>
    /// <param name="msg">The explicit message.</param>
    public static string Log ( string msg )
    {
      try
      {
        lock ( options.logFileName )
          using ( TextWriter log = options.GetLogFile( options.logFileName ) )
          {
            if ( msg == null )
              log.WriteLine();
            else
              log.WriteLine( string.Format( "{0}: {1}", Util.FormatNowUtc(), msg ) );
            //Util.Touch( ++MemstatCounter >= Options.options.memoryCheckPeriod, log );
          }
      }
      catch ( IOException e )
      {
        Console.WriteLine( "Log - unhandled IOException: " + e.Message );
        Console.WriteLine( "Stack: " + e.StackTrace );
      }
      return msg;
    }

    /// <summary>
    /// Name of the class for persistence.
    /// </summary>
    public string PersName
    {
      get { return PERS_NAME; }
    }

    /// <summary>
    /// Occupied memory in bytes.
    /// </summary>
    public int Occupied
    {
      get { return 0; }
    }

    public virtual void StringStatistics ( long[] result )
    {
      if ( result == null || result.Length < 4 )
        return;
    }

    /// <summary>
    /// Data statistics / check.
    /// Output is written in HTML format to the provided StringBuilder.
    /// </summary>
    public virtual void DataStatisticsHTML ( StringBuilder sb, string className =null )
    {
      string classAttr = (className == null) ? "" : (" class=\"" + className + '\"');

      // string statistics:
      long[] optionSS = new long[ 4 ];
      StringStatistics( optionSS );

      // string statistics:
      sb.AppendFormat( "<p{0}><b>Globals:</b> interned strings: {1} (len {2}), noninterned strings: {3} (len {4})\r\n",
                       classAttr, optionSS[ 2 ], optionSS[ 3 ], (optionSS[ 0 ] - optionSS[ 2 ]), (optionSS[ 1 ] - optionSS[ 3 ]) );
      sb.Append( "</p>\r\n" );
    }

    public Options ()
    {
    }

    /// <summary>
    /// Config file-name (read from command-line).
    /// </summary>
    public string configFile = "";

    /// <summary>
    /// Writes the array prolog to the given stream.
    /// Fixing version type to "0".
    /// </summary>
    public void TextWriteArrayProlog ( TextWriter wri, char separator, int type =0 )
    {
      TextPersistence.ArrayWriteProlog( wri, separator, this, 0, type );
    }

    /// <summary>
    /// type = 0 .. the only type so far
    /// <code>options|version|type</code>
    /// version: 0
    /// </summary>
    public virtual bool TextWrite ( TextWriter wri, char separator, bool embedded =false, int type =0 )
    {
      if ( !embedded )
        wri.WriteLine( "{0}{1}{0}{2}{0}{3}", separator, PersName, 0, type );

      wri.WriteLine( "msgMode="      + string.Join( ",", msgModes ) );
      wri.WriteLine( "memoryCheck="  + memoryCheckPeriod );
      if ( outputFileName.Length > 0 )
        wri.WriteLine( "output=" + outputFileName );
      wri.WriteLine( "log="          + logFileName );
      wri.WriteLine( "backupMask="   + backupLogFileMask );
      wri.WriteLine( "backupMaxSize=" + backupLogMaxSize );
      wri.WriteLine( "base="         + baseDir );

      return true;
    }

    /// <summary>
    /// Read options similar to the config-file read.
    /// Does not execure PreConfig() and PostConfig().
    /// </summary>
    protected void TextReadCore ( TextReader rea, char separator, ref string firstLine, int version, int type =0 )
    {
      InitRead();
      lineNumber = 3;

      do
      {
        if ( firstLine == null &&
             (firstLine = ReadFromBuffer()) == null &&
             (firstLine = rea.ReadLine()) != null )
          lineNumber++;

        if ( firstLine == null )
          break;

        firstLine = firstLine.Trim();
        if ( firstLine.Length == 0 || firstLine[ 0 ] == '#' )
        {
          firstLine = null;
          continue;
        }

        // key = value line?
        int pos = firstLine.IndexOf( '=' );
        if ( pos < 0 )
        {
          if ( firstLine[ 0 ] == separator )
            break;

          if ( !HandleCommand( firstLine ) )
            Console.WriteLine( "Warning: ignoring SYT config line: {0} ({1})", FileLineNo(), firstLine );

          firstLine = null;
          continue;
        }

        string key = firstLine.Substring( 0, pos ).Trim();
        if ( key.Length < 1 )
        {
          Console.WriteLine( "Warning: ignoring SYT config line: {0} ({1})", FileLineNo(), firstLine );
          firstLine = null;
          continue;
        }

        string value = firstLine.Substring( pos + 1 );
        int checkPos = 0;
        while ( checkPos < value.Length )
        {
          pos = value.IndexOf( '#', checkPos );
          if ( pos < checkPos ) break;
          if ( pos == checkPos || value[ pos - 1 ] != '\\' )
          {
            value = value.Substring( 0, pos );
            break;
          }
          checkPos = pos + 1;
        }
        value = value.Trim();

        if ( (value.Length > 0 ||
              !HandleEmptyValue( key )) &&
             !AdditionalKey( key, value, firstLine ) )
          Console.WriteLine( Util.LogFormat( "Warning: unknown key '{0}' in SYT config {1} ({2})", key, FileLineNo(), firstLine ) );

        firstLine = null;
      }
      while ( true );
    }

    public string FileLineNo ()
    {
      StringBuilder sb = new StringBuilder( "'" );
      sb.Append( (includeFiles.Count > 0) ? includeFiles.Peek() : path ).Append( "':" ).Append( lineNumber );
      return sb.ToString();
    }

    /// <summary>
    /// Standard ITextPersistent compliant read from a (SYT) text stream.
    /// </summary>
    public virtual ITextPersistent TextRead ( TextReader rea, char separator, ref string firstLine, int version, int type =0 )
    {
      options.PreConfig();

      options.TextReadCore( rea, separator, ref firstLine, version, type );

      options.PostConfig();

      return options;
    }

    public delegate bool StringPredicate ( string name );

#region General config-file parsing

    /// <summary>
    /// Global data cleanup.
    /// </summary>
    public virtual void Cleanup ()
    {
      commands.Clear();
      inputFiles.Clear();
      baseDir = ".";
    }

    /// <summary>
    /// Current saving/loading root file-name.
    /// </summary>
    public string path = "";

    /// <summary>
    /// Line-number in the current file.
    /// </summary>
    public int lineNumber;

    /// <summary>
    /// Processing of options from sampling params..
    /// </summary>
    /// <param name="dict">Key-value dictionary.</param>
    public void ParseDictionary ( Dictionary<string, string> dict )
    {
      foreach ( var kvp in dict )
        AdditionalKey( kvp.Key, kvp.Value, "-" );
    }

    /// <summary>
    /// Process one "config-like" line.
    /// For loops, includes, .. etc .. are not allowed!
    /// </summary>
    /// <returns>True in case of anything reasonable.</returns>
    public virtual bool ProcessSimpleLine ( string line )
    {
      // encapsulation by quotes:
      if ( line.Length > 1 &&
           (line[ 0 ] == '\"' || line[ 0 ] == '\'') &&
           line[ line.Length - 1 ] == line[ 0 ] )
        line = line.Substring( 1, line.Length - 2 );

      int pos = line.IndexOf( '#' );
      if ( pos >= 0 )
        line = line.Substring( 0, pos );
      line = line.Trim();

      // comment check:
      if ( line.Length == 0 ||
           line[ 0 ] == '|' )
        return false;

      // simple command:
      pos = line.IndexOf( '=' );
      if ( pos < 0 )          // command name only
        return HandleCommand( line );

      string key = line.Substring( 0, pos ).Trim();
      if ( key.Length < 1 )
      {
        Console.WriteLine( "Warning: ignoring invalid option line: '{0}'", line );
        return false;
      }

      string value = line.Substring( pos + 1 );
      value = value.Trim();
      if ( value.Length == 0 &&
           HandleEmptyValue( key ) )
        return true;

      if ( !AdditionalKey( key, value, line ) )
      {
        Console.WriteLine( "Warning: unknown key '{0}' in option line: {1} ({2})", key, FileLineNo(), line );
        return false;
      }

      return true;
    }

    /// <summary>
    /// Parsing of the text config-file. Key=value pairs, list of commands / input file-names, ..
    /// </summary>
    /// <param name="path">Config file-name.</param>
    /// <param name="initial">Do PreConfig() initialization first?</param>
    public void ParseConfig ( string path, bool initial =true )
    {
      if ( initial )
        PreConfig();

      string line;
      InitRead();  // init the include mechanism (include, for loops..)

      using ( StreamReader inp = new StreamReader( path ) )
      {
        this.path = path;

        do
        {
          if ( (line = ReadFromBuffer()) == null &&
               (line = inp.ReadLine()) != null )
            lineNumber++;

          if ( line == null )
            break;

          int pos = line.IndexOf( '#' );
          if ( pos >= 0 )
            line = line.Substring( 0, pos );
          line = line.Trim();

          if ( line.Length == 0 ||
               line[ 0 ] == '|' )
            continue;

          // key = value line?
          pos = line.IndexOf( '=' );
          if ( pos < 0 )          // only the input-file name
          {
            if ( !HandleCommand( line ) )
              Console.WriteLine( "Warning: ignoring config-file {0} ({1})", FileLineNo(), line );
            continue;
          }

          string key = line.Substring( 0, pos ).Trim();
          if ( key.Length < 1 )
          {
            Console.WriteLine( "Warning: ignoring config-file {0} ({1})", FileLineNo(), line );
            continue;
          }

          string value = line.Substring( pos + 1 ).Trim();
          if ( value.Length == 0 &&
               HandleEmptyValue( key ) )
            continue;

          switch ( key )
          {
            case "for":
              {
                string[] tokens = Util.ParseList( value );
                if ( tokens.Length >= 4 )
                {
                  int from = 0;
                  int step = 1;
                  int to = 12;
                  if ( !int.TryParse( tokens[ 0 ], out from ) ||
                       !int.TryParse( tokens[ 1 ], out step ) ||
                       !int.TryParse( tokens[ 2 ], out to ) )
                  {
                    Console.WriteLine( "Warning: number error in 'for' statement in config-file {0} ({1})", FileLineNo(), line );
                    break;
                  }
                  string template = tokens[ 3 ];
                  for ( int i = 4; i < tokens.Length; i++ )
                    template += "," + tokens[ i ];

                  // generate the lines:
                  if ( to >= from )
                  {
                    int forLines = 0;
                    for ( int i = to; i >= from; i -= step )
                    {
                      buffer.Push( string.Format( template, i ) );
                      forLines++;
                    }

                    includeFiles.Push( "for(" + template + ')' );
                    includeLines.Push( forLines );
                    parentLine.Push( lineNumber );
                    lineNumber = 0;
                  }
                }
                else
                  Console.WriteLine( "Warning: error in 'for' statement in config-file line: '{0}'", line );
                break;
              }

            case "foreach":
              {
                string[] tokens = Util.ParseList( value );
                if ( tokens.Length >= 2 )
                {
                  int i = tokens.Length - 1;
                  string template = tokens[ i ];

                  // generate the lines:
                  while ( --i >= 0 )
                    buffer.Push( string.Format( template, tokens[ i ] ) );

                  includeFiles.Push( "foreach(" + template + ')' );
                  includeLines.Push( tokens.Length - 1 );
                  parentLine.Push( lineNumber );
                  lineNumber = 0;
                }
                else
                  Console.WriteLine( "Warning: error in 'foreach' statement in config-file line: '{0}'", line );
                break;
              }

            case "forfiles":
              {
                string[] tokens = Util.ParseList( value );
                if ( tokens.Length >= 2 )
                {
                  int i = tokens.Length - 1;
                  string template = tokens[ i ];
                  int files = 0;

                  // generate the lines:
                  while ( --i >= 0 )
                  {
                    try
                    {
                      string dir = Path.GetDirectoryName( tokens[ i ] );
                      string mask = Path.GetFileName( tokens[ i ] );
                      string[] search = Directory.GetFiles( dir, mask );
                      int len = search.Length;
                      while ( --len >= 0 )
                      {
                        buffer.Push( string.Format( template, search[ len ] ) );
                        files++;
                      }
                    }
                    catch ( IOException )
                    {
                      Console.WriteLine( "Warning: I/O error in 'foreach' statement for token: '{0}'", tokens[ i ] );
                    }
                    catch ( UnauthorizedAccessException )
                    {
                      Console.WriteLine( "Warning: access error in 'foreach' statement for token: '{0}'", tokens[ i ] );
                    }
                  }

                  includeFiles.Push( "forfiles(" + template + ')' );
                  includeLines.Push( files );
                  parentLine.Push( lineNumber );
                  lineNumber = 0;
                }
                else
                  Console.WriteLine( "Warning: error in 'forfiles' statement in config-file line: '{0}'", line );
                break;
              }

            default:
              if ( !AdditionalKey( key, value, line ) )
                Console.WriteLine( "Warning: unknown key '{0}' in config-file {1} ({2})", key, FileLineNo(), line );
              break;
          }
        }
        while ( true );
      }

      PostConfig();

      Console.WriteLine( "Finished reading config-file '{0}' ({1}, {2}, {3}, {4})",
                         path, Util.ProgramVersion, Util.TargetFramework, Util.RunningFramework,
                         Util.FormatNowUtc() );
    }

    /// <summary>
    /// Parse single option from the command-line.
    /// </summary>
    /// <param name="args">Global argument-list.</param>
    /// <param name="i">Index to the argument-list.</param>
    /// <returns>True if the option was recognized and handled ok.</returns>
    public virtual bool ParseOption ( string[] args, ref int i )
    {
      if ( i >= args.Length ||
           string.IsNullOrEmpty( args[ i ] ) )
        return false;

      // <command> or <config-file>

      if ( args[ i ][ 0 ] != '-' )
      {
        if ( File.Exists( args[ i ] ) )
        {
          configFile = Path.GetFullPath( args[ i ] );
          ParseConfig( configFile );
          return true;
        }
        return HandleCommand( args[ i ] );
      }

      string opt = args[ i ].Substring( 1 );

      // -c <config-file>

      if ( opt == "c" &&
           i + 1 < args.Length )
      {
        if ( File.Exists( args[ ++i ] ) )
        {
          configFile = Path.GetFullPath( args[ i ] );
          ParseConfig( configFile );
        }
        else
          Console.WriteLine( "Warning: config-file '{0}' doesn't exist!", args[ i ] );

        return true;
      }

      // -o <output-file>

      if ( opt == "o" &&
           i + 1 < args.Length )       // priority output file-name
      {
        if ( !string.IsNullOrEmpty( args[ ++i ] ) )    // potentially valid output file-name
        {
          outputFileName = args[ i ];
          priorityOutputFileName = true;
        }

        return true;
      }

      // -l <log-file>

      if ( opt == "l" &&
           i + 1 < args.Length )       // log file-name
      {
        if ( !string.IsNullOrEmpty( args[ ++i ] ) )    // potentially valid log file-name
        {
          logFileName = args[ i ];
          priorityLogFileName = true;
        }

        return true;
      }

      // -<command>

      int pos = opt.IndexOf( '=' );
      if ( pos < 0 )
        return HandleCommand( opt );

      string key = opt.Substring( 0, pos ).Trim();
      if ( key.Length < 1 )
        return false;

      // -<option>=

      string value = opt.Substring( pos + 1 ).Trim();
      if ( value.Length == 0 )
        return HandleEmptyValue( key );

      // -<option>=<value>

      return AdditionalKey( key, value, args[ i ] );
    }

    /// <summary>
    /// Config-file read prefix.
    /// </summary>
    public virtual void PreConfig ()
    {
      Cleanup();
    }

    /// <summary>
    /// Config-file read postfix.
    /// </summary>
    public virtual void PostConfig ()
    {
    }

    /// <summary>
    /// Parse additional keys.
    /// </summary>
    /// <param name="key">Key string (non-empty, trimmed).</param>
    /// <param name="value">Value string (non-null, trimmed).</param>
    /// <returns>True if recognized.</returns>
    public virtual bool AdditionalKey ( string key, string value, string line )
    {
      int newInt   = 0;
      long newLong = 0L;

      switch ( key )
      {
        case "verbose":
          msgModes.Add( "verbose" );
          return true;

        case "verbose2":
          msgModes.UnionWith( knownMsgModes );
          return true;

        case "msgMode":
          SetMsgMode( value );
          break;

        case "memoryCheck":
          if ( int.TryParse( value, out newInt ) && newInt > 0 )
            memoryCheckPeriod = newInt;
          break;

        case "output":
          if ( !priorityOutputFileName )
            outputFileName = value;
          break;

        case "log":
          if ( !priorityLogFileName )
            logFileName = value;
          break;

        case "base":
          if ( Directory.Exists( value = NormalizeDir( value ) ) )
            baseDir = value;
          else
            Console.WriteLine( "Warning: ignoring nonexistent base directory: '{0}'", value );
          break;

        case "backupMask":
          backupLogFileMask = value;
          break;

        case "backupMaxSize":
          if ( long.TryParse( value, out newLong ) && newLong > 0 )
            backupLogMaxSize = newLong;
          break;

        case "include":
          IncludeFile( GetRelativePath( value ) );
          break;

        default:
          return false;
      }

      return true;
    }

    /// <summary>
    /// How to handle the "key=" config line?
    /// </summary>
    /// <returns>True if config line was handled.</returns>
    public virtual bool HandleEmptyValue ( string key )
    {
      Console.WriteLine( Util.LogFormat( "Warning: ignoring empty value for the key: '{0}' ({1})", key, FileLineNo() ) );
      return false;
    }

    /// <summary>
    /// How to handle the non-key-value config line?
    /// </summary>
    /// <param name="line">The nonempty config line.</param>
    /// <returns>True if config line was handled.</returns>
    public virtual bool HandleCommand ( string line )
    {
      return AddCommand( line );
    }

    /// <summary>
    /// Stack of lines to be executed..
    /// </summary>
    protected Stack<string> buffer = new Stack<string>();     // for the "include", "codebookFile", .. commands

    /// <summary>
    /// Stack of files being parsed. TOS = current file.
    /// </summary>
    protected Stack<string> includeFiles = new Stack<string>();

    /// <summary>
    /// Line numbers of parsed files. TOS = current file.
    /// </summary>
    protected Stack<int> includeLines = new Stack<int>();

    /// <summary>
    /// Line number in the parent file.
    /// Line number of the 'include=' or 'for=' command.
    /// </summary>
    protected Stack<int> parentLine = new Stack<int>();

    /// <summary>
    /// Init the file-include mechanism.
    /// </summary>
    protected void InitRead ()
    {
      buffer.Clear();
      includeFiles.Clear();
      includeLines.Clear();
      parentLine.Clear();
      lineNumber = 0;
    }

    /// <summary>
    /// Reads the next line from the buffer (include, for, ..)
    /// </summary>
    protected string ReadFromBuffer ()
    {
      while ( includeLines.Count > 0 &&
              lineNumber >= includeLines.Peek() )
      {
        // returns to the previous level[s]
        includeFiles.Pop();
        includeLines.Pop();
        lineNumber = parentLine.Pop();
      }

      if ( buffer.Count == 0 )
        return null;

      lineNumber++;
      return buffer.Pop();
    }

    protected string GetRelativePath ( string rel )
    {
      return Path.GetFullPath( Path.Combine( Path.GetDirectoryName( (includeFiles.Count > 0) ? includeFiles.Peek() : path ), rel ) );
    }

    /// <summary>
    /// Include the given file (reads its lines into the "buffer" stack).
    /// </summary>
    /// <param name="fileName">Fully-qualified file name.</param>
    protected void IncludeFile ( string fileName )
    {
      if ( !File.Exists( fileName ) )
      {
        Console.WriteLine( Util.LogFormat( "Warning: ignoring nonexistent include file: '{0}' ({1})", fileName, FileLineNo() ) );
        return;
      }
      Stack<string> include = new Stack<string>();
      string line;
      using ( StreamReader incl = new StreamReader( fileName ) )
        do
        {
          line = incl.ReadLine();
          if ( line == null )
            break;
          include.Push( line );
        }
        while ( true );

      if ( include.Count > 0 )
      {
        includeFiles.Push( fileName );
        includeLines.Push( include.Count );
        parentLine.Push( lineNumber );
        do
          buffer.Push( include.Pop() );
        while ( include.Count > 0 );
        lineNumber = 0;
      }
    }

#endregion
  }
}
