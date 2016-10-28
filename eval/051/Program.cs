using System;
using System.Collections.Generic;
using System.IO;
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
      // default members:
    }

    public static void Touch ()
    {
      if ( options == null )
        Util.Log( "EvalOptions not initialized!" );
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
            string opt = args[ i ];
            if ( opt.Equals( "-x" ) &&
                 i + 1 < args.Length )     // sample custom command-line option..
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

      // !!! TODO: do the job !!!
      Options.Log( "debug", "Log(debug) test" );
    }
  }
}
