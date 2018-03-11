using System;
using System.CodeDom.Compiler;
using Microsoft.CodeDom.Providers.DotNetCompilerPlatform;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using OpenTK;
using MathSupport;
using Utilities;
using Raster;

namespace _094
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
      project = "eval094";
      TextPersistence.Register( new EvalOptions(), 0 );

      RegisterMsgModes( "debug" );
    }

    public EvalOptions ()
    {
      // default values of structured members:
      baseDir = @".\input\";

      references.Add( @"System.dll" );
      references.Add( @"System.Core.dll" );
      references.Add( @"System.Linq.dll" );
      references.Add( @"System.Drawing.dll" );
      references.Add( typeof( Vector3d ).Assembly.Location );
      references.Add( typeof( Program ).Assembly.Location );
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

    public HashSet<string> references = new HashSet<string>();

    public HashSet<string> libraries = new HashSet<string>();

    public string compilerOptions = "/optimize+ /unsafe";

    public int imageWidth = 400;

    /// <summary>
    /// List of source files.
    /// </summary>
    public List<string> sourceFiles = new List<string>();

    public HashSet<string> bans = new HashSet<string>();

    public HashSet<string> best = new HashSet<string>();

    public bool bestOnly = false;

    public Dictionary<string, string[]> imageInfo = new Dictionary<string, string[]>();

    public bool imageLocal = false;

    /// <summary>
    /// Specific data cleanup.
    /// </summary>
    public override void Cleanup ()
    {
      base.Cleanup();

      sourceFiles.Clear();
      libraries.Clear();
      bans.Clear();
      best.Clear();
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
      //double newDouble = 0.0;

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
            Console.WriteLine( $"Warning: ignoring nonexistent image '{value}' ({FileLineNo()})" );
          break;

        case "imageInfo":
          {
            string[] info = Util.ParseList( value );
            if ( info.Length > 1 )
            {
              List<string> l = new List<string>( info );
              l.RemoveAt( 0 );
              imageInfo[ info[ 0 ] ] = l.ToArray();
            }
          }
          break;

        case "imageLocal":
          imageLocal = Util.positive( value );
          break;

        case "bestOnly":
          bestOnly = Util.positive( value );
          break;

        case "source":
          if ( File.Exists( value ) )
            sourceFiles.Add( value );
          else
          if ( File.Exists( baseDir + value ) )
            sourceFiles.Add( baseDir + value );
          else
            Console.WriteLine( $"Warning: ignoring nonexistent source '{value}' ({FileLineNo()})" );
          break;

        case "headerFile":
          if ( File.Exists( value ) )
            headerFile = value;
          else
            Console.WriteLine( $"Warning: ignoring nonexistent file '{value}' ({FileLineNo()})" );
          break;

        case "footerFile":
          if ( File.Exists( value ) )
            footerFile = value;
          else
            Console.WriteLine( $"Warning: ignoring nonexistent file '{value}' ({FileLineNo()})" );
          break;

        case "reference":
          references.Add( value );
          break;

        case "library":
          libraries.Add( value );
          break;

        case "ban":
          bans.Add( value );
          break;

        case "best":
          best.Add( value );
          break;

        case "imageWidth":
          if ( int.TryParse( value, out newInt ) &&
               newInt > 10 )
            imageWidth = newInt;
          break;

        case "compilerOptions":
          compilerOptions = value;
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

        case "source":
          sourceFiles.Clear();
          return true;

        case "library":
          libraries.Clear();
          return true;

        case "ban":
          bans.Clear();
          return true;

        case "best":
          best.Clear();
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
            if ( opt == "-x" &&
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
                Console.WriteLine( $"Warning: invalid option '{opt}'!" );
          }

      //--- do the job ---
      if ( !wasEvaluated )
        Evaluate();
    }

    static bool wasCompiled = false;
    static bool wasEvaluated = false;

    static Dictionary<string, Assembly> assemblies = new Dictionary<string, Assembly>();

    static Stopwatch sw = new Stopwatch();

    /// <summary>
    /// Statistics of computing-time in seconds.
    /// </summary>
    static Quantiles<double> qtime = new Quantiles<double>();

    static public void Evaluate ()
    {
      wasEvaluated = true;
      try
      {
        using ( TextWriter wri = new StreamWriter( Path.Combine( EvalOptions.options.outDir, EvalOptions.options.outputFileName ), false, Encoding.UTF8 ) )
        {
          string part;

          // compile all source files:
          if ( !wasCompiled )
          {
            Options.Log( null );
            CompileSources( EvalOptions.options.sourceFiles );
            wasCompiled = true;
          }
          int images = EvalOptions.options.inputFiles.Count;
          Console.WriteLine( Options.LogFormat( "Configuration - assemblies: {0}, bans: {1}, best: {2}, images: {3}, output: '{4}'",
                                                assemblies.Count, EvalOptions.options.bans.Count, EvalOptions.options.best.Count,
                                                images, EvalOptions.options.outputFileName ) );

          // HTML file header:
          if ( Util.ReadTextFile( EvalOptions.options.headerFile, out part ) )
            wri.Write( part );

          wri.WriteLine( "<table class=\"nb\">" );
          wri.WriteLine( "<tr><th>Name</th><th>Time</th><th>Image</th></tr>" );

          int ord = 0;
          foreach ( var imageFn in EvalOptions.options.inputFiles )
          {
            wri.WriteLine( "<tr><td>&nbsp;</td></tr>" );

            string relative = EvalOptions.options.imageLocal ?
                                Path.GetFileName( imageFn ) :
                                Util.MakeRelativePath( EvalOptions.options.outDir, imageFn );
            string relativeLDR = relative.Replace( ".hdr", ".jpg" );
            string[] desctiption;
            EvalOptions.options.imageInfo.TryGetValue( Path.GetFileName( imageFn ), out desctiption );
            wri.WriteLine( "<tr><td colspan=\"2\" class=\"b p r\">{0}</td>",
                           (desctiption == null) ? "&nbsp;" : string.Join( ", ", desctiption ) );
            wri.WriteLine( $"<td><a href=\"{relative}\"><img src=\"{relativeLDR}\" width=\"{EvalOptions.options.imageWidth}\" alt=\"input\" /></a></td>" );
            wri.WriteLine( "</tr>" );

            FloatImage image = RadianceHDRFormat.FromFile( imageFn );
            Options.LogFormatMode( "debug", "Input image '{0}':", imageFn );

            qtime.Clear();

            List<string> names = new List<string>( assemblies.Keys );
            names.Sort();
            string outBase = Path.Combine( EvalOptions.options.outDir, Path.GetFileNameWithoutExtension( imageFn ) );
            foreach ( var name in names )
              if ( !EvalOptions.options.bans.Contains( name ) &&
                   (!EvalOptions.options.bestOnly || EvalOptions.options.best.Contains( name )) )
                EvaluateSolution( name, assemblies[ name ], image, outBase, wri );

            wri.Write( string.Format( CultureInfo.InvariantCulture, "<tr><th class=\"l\">Time [s]</th><td class=\"p t r\">{0:f2}s</td>",
                                      qtime.Mean ) );
            wri.WriteLine( string.Format( CultureInfo.InvariantCulture, "<td class=\"p\">+-{0:f2}s, quartiles: {1:f2} - {2:f2} - {3:f2} - {4:f2} - {5:f2}</td></tr>",
                                          qtime.Variance, qtime.Min, qtime.Quartile( 1 ), qtime.Median, qtime.Quartile( 3 ), qtime.Max ) );

            Console.WriteLine( Options.LogFormat( "Finished image #{0}/{1} '{2}'", ++ord, images, relative ) );
          }
          wri.WriteLine( "</table>" );

          // HTML file footer:
          if ( Util.ReadTextFile( EvalOptions.options.footerFile, out part ) )
            wri.Write( part );
        }
      }
      catch ( IOException e )
      {
        Console.WriteLine( Options.LogFormat( "I/O error: {0}", e.Message ) );
      }
    }

    static void CompileSources ( IEnumerable<string> sources )
    {
      sw.Restart();

      CodeDomProvider P = new CSharpCodeProvider();
      CompilerParameters Opt = new CompilerParameters();
      Opt.GenerateInMemory = true;
      foreach ( string refName in EvalOptions.options.references )
        Opt.ReferencedAssemblies.Add( refName );
      Opt.IncludeDebugInformation = false;
      Opt.CompilerOptions = EvalOptions.options.compilerOptions;

      // Common optional library files ..
      string source;
      List<string> globalSources = new List<string>();
      foreach ( var libName in EvalOptions.options.libraries )
        if ( Util.ReadTextFile( libName, out source ) )
          globalSources.Add( source );
        else
          Options.LogFormat( "Error reading library source file '{0}', ignored.", libName );

      int files = 0;
      int errors = 0;

      foreach ( var fn in sources )
      {
        // read original source file ..
        if ( !Util.ReadTextFile( fn, out source ) )
        {
          Options.LogFormat( "Error reading source file '{0}', ignored.", fn );
          continue;
        }

        // .. determine the name ..
        int end = fn.LastIndexOf( ".cs" );
        if ( end < 1 )
        {
          Options.LogFormat( "Invalid format of source file name '{0}', ignored.", fn );
          continue;
        }

        string name = fn.Substring( 0, end );
        end = name.LastIndexOf( '.' );
        if ( end < 0 )
        {
          Options.LogFormat( "Invalid format of source file name '{0}', ignored.", fn );
          continue;
        }
        name = name.Substring( end + 1 );

        files++;

        // .. change its namespace ..
        source = source.Replace( "_094tonemapping", "tmap" + name );
        List<string> localSources = new List<string>( globalSources );
        localSources.Add( source );
        localSources.Add( "namespace tmap" + name + "{public class Form1{public static bool cont=true;}}\r\n" );

        // .. and finally compile it all:
        CompilerResults R = P.CompileAssemblyFromSource( Opt, localSources.ToArray() );
        if ( R.Errors.Count > 0 )
        {
          errors += R.Errors.Count;
          Options.LogFormat( "Source: {0}, name: {1}, errors:", fn, name );
          foreach ( var err in R.Errors )
            Options.Log( " " + err );
          continue;
        }

        Options.LogFormat( "Source: {0}, name: {1}, OK", fn, name );
        assemblies[ name ] = R.CompiledAssembly;
      }

      sw.Stop();
      Console.WriteLine( Options.LogFormat( "Compile finished, files: {0}, time: {1:f2}s, errors: {2}",
                                            files, sw.ElapsedMilliseconds * 0.001, errors ) );
    }

    static void EvaluateSolution ( string name, Assembly ass, FloatImage image, string outBase, TextWriter wri )
    {
      string name2 = null;
      string param = null;
      Bitmap ldr   = null;

      // memory cleanup and report:
      long memOccupied = GC.GetTotalMemory( true );
      Process procObj = Process.GetCurrentProcess();
      Options.LogFormatMode( "debug", $"Evaluating '{name}' [{(memOccupied >> 20)}M - {(procObj.PrivateMemorySize64 >> 20)}M - {(procObj.VirtualMemorySize64 >> 20)}M - {(procObj.WorkingSet64 >> 20)}M - {(procObj.PagedMemorySize64 >> 20)}M - {(procObj.PagedSystemMemorySize64 >> 10)}K - {(procObj.NonpagedSystemMemorySize64 >> 10)}K]" );
      string msg = null;

      // running the solution function:
      sw.Restart();
      try
      {
        // call #1: default params, name
        object[] arguments1 = new object[] { null, null };
        Type classType = ass.GetType( "tmap" + name + ".ToneMapping" );
        classType.GetMethod( "InitParams" ).Invoke( null, arguments1 );
        param = arguments1[ 0 ].ToString();
        name2 = arguments1[ 1 ].ToString();

        // call #2: the actual tone-mapping task
        object[] arguments2 = new object[] { image, null, param ?? "" };
        ldr = (Bitmap)classType.GetMethod( "ToneMap" ).Invoke( null, arguments2 );
      }
      catch ( Exception e )
      {
        msg = (e.InnerException ?? e).Message;
      }
      sw.Stop();

      // quantile statistics for elapsed time and color variance
      double elapsed = sw.ElapsedMilliseconds * 0.001;

      // report:
      if ( string.IsNullOrEmpty( name2 ) ||
           name2 == "pilot" )
        name2 = name;
      bool best = EvalOptions.options.best.Contains( name );
      wri.Write( string.Format( CultureInfo.InvariantCulture, "<tr><td class=\"t\">{0}{1}{2}</td><td class=\"p t r\">{3:f2}s</td>",
                                best ? "<b>" : "", name2, best ? "</b>" : "",
                                elapsed ) );

      if ( !string.IsNullOrEmpty( msg ) ||
           ldr == null )
      {
        Util.Log( $"Error: '{msg ?? "no image"}'" );
        wri.WriteLine( $"<td>Error: {msg ?? "no image"}</td>" );
        wri.WriteLine( "</tr>" );
        if ( ldr != null )
          ldr.Dispose();
        return;
      }

      // output LDR image:
      qtime.Add( elapsed );
      string ldrName = outBase + name + ".png";
      ldr.Save( ldrName, ImageFormat.Png );
      ldr.Dispose();

      wri.WriteLine( $"<td><img src=\"{Path.GetFileName(ldrName)}\" width=\"{EvalOptions.options.imageWidth}\" alt=\"{name}\" /></td>" );
      wri.WriteLine( "</tr>" );
    }
  }
}
