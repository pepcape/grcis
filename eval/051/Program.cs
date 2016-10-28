using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Utilities;

namespace _051
{
  public class EvalOptions : Options
  {
    /// <summary>
    /// Singleton instance.
    /// </summary>
    public static new EvalOptions options = (EvalOptions)(Options.options = new EvalOptions());

    public override void StringStatistics ( long[] result )
    {
      if ( result == null || result.Length < 4 )
        return;

      Util.StringStat( commands, result );
    }

    static EvalOptions ()
    {
      project = "eval051";
      TextPersistence.Register( new EvalOptions(), 0 );

      RegisterMsgModes( "debug" );
    }

    public EvalOptions ()
    {
      // default values of structured members:
      baseDir = @".\input\";
    }

    public static void Touch ()
    {
      if ( options == null )
        Util.Log( "EvalOptions not initialized!" );
    }

    //--- project-specific options ---

    public string outDir = @".\output\";

    public string headerFile = @".\output\header-standalone.html";

    public string footerFile = @".\output\footer-standalone.html";

    /// <summary>
    /// Specific data cleanup.
    /// </summary>
    public override void Cleanup ()
    {
      base.Cleanup();
    }

    /// <summary>
    /// Parse additional keys.
    /// </summary>
    /// <param name="key">Key string (non-empty, trimmed).</param>
    /// <param name="value">Value string (non-null, trimmed).</param>
    /// <returns>True if recognized.</returns>
    public override bool AdditionalKey ( string key, string value, string line )
    {
      if ( base.AdditionalKey( key, value, line ) )
        return true;

      int newInt = 0;
      long newLong = 0L;
      float newFloat = 0.0f;

      switch ( key )
      {
        case "outDir":
          outDir = value;
          break;

        case "image":
          if ( File.Exists( value ) )
            inputFiles.Add( value );
          else
          if ( File.Exists( baseDir + value ) )
            inputFiles.Add( baseDir + value );
          else
            Console.WriteLine( "Warning: ignoring nonexistent image '{0}' ({1})", value, FileLineNo() );
          break;

        case "headerFile":
          if ( File.Exists( value ) )
            headerFile = value;
          else
            Console.WriteLine( "Warning: ignoring nonexistent file '{0}' ({1})", value, FileLineNo() );
          break;

        case "footerFile":
          if ( File.Exists( value ) )
            footerFile = value;
          else
            Console.WriteLine( "Warning: ignoring nonexistent file '{0}' ({1})", value, FileLineNo() );
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
    public override bool HandleEmptyValue ( string key )
    {
      switch ( key )
      {
        case "image":
          inputFiles.Clear();
          return true;
      }

      return false;
    }

    /// <summary>
    /// How to handle the non-key-value config line?
    /// </summary>
    /// <param name="line">The nonempty config line.</param>
    /// <returns>True if config line was handled.</returns>
    public override bool HandleCommand ( string line )
    {
      switch ( line )
      {
        case "eval":
          Program.Evaluate();
          return true;
      }

      return false;
    }
  }

  class Program
  {
    /// <summary>
    /// Defines program version (Rev#).
    /// </summary>
    static Program ()
    {
      Util.SetVersion( "$Rev$" );
    }

    static void Main ( string[] args )
    {
      EvalOptions.Touch();

      if ( args.Length < 1 )
        Console.WriteLine( "Warning: no command-line options, using default values!" );
      else
        for ( int i = 0; i < args.Length; i++ )
          if ( !string.IsNullOrEmpty( args[ i ] ) )
          {
            // sample custom command-line option:
            string opt = args[ i ];
            if ( opt.Equals( "-x" ) &&
                 i + 1 < args.Length )
            {
              if ( !string.IsNullOrEmpty( args[ ++i ] ) ) // potentially valid file-name
              {
                string xFile = Path.GetFullPath( args[ i ] );
                // do_anything_with( xFile );
              }
              else
                Console.WriteLine( "Warning: invalid x-file '{0}'!", args[ i ] ?? "-" );
            }
            else
              if ( !EvalOptions.options.ParseOption( args, ref i ) )
                Console.WriteLine( "Warning: invalid option '{0}'!", opt );
          }

      //--- do the job ---
      Options.Log( "debug", "Log(debug) test" );
      if ( !wasEvaluated )
        Evaluate();
    }

    static bool wasEvaluated = false;

    static public void Evaluate ()
    {
      wasEvaluated = true;
      using ( TextWriter wri = new StreamWriter( Path.Combine( EvalOptions.options.outDir, EvalOptions.options.outputFileName ), false, Encoding.UTF8 ) )
      {
        string part;

        // HTML file header:
        if ( Util.ReadTextFile( EvalOptions.options.headerFile, out part ) )
          wri.Write( part );

        foreach ( var image in EvalOptions.options.inputFiles )
        {
          string relative = Util.MakeRelativePath( EvalOptions.options.outDir, image );
          wri.WriteLine( "<p>" );
          wri.WriteLine( "<img src=\"{0}\" width=\"400\"/>", relative );
          wri.WriteLine( "</p>" );
        }

        // HTML file footer:
        if ( Util.ReadTextFile( EvalOptions.options.footerFile, out part ) )
          wri.Write( part );
      }
    }
  }
}
