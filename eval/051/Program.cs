using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using MathSupport;
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

      references.Add( @"System.dll" );
      references.Add( @"System.Core.dll" );
      references.Add( @"System.Linq.dll" );
      references.Add( @"System.Drawing.dll" );
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

    public double minV = 0.2;

    public double maxV = 0.8;

    public double minS =  0.1;

    /// <summary>
    /// List of source files.
    /// </summary>
    public List<string> sourceFiles = new List<string>();

    public HashSet<string> bans = new HashSet<string>();

    public HashSet<string> best = new HashSet<string>();

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
      double newDouble = 0.0;

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

        case "source":
          if ( File.Exists( value ) )
            sourceFiles.Add( value );
          else
          if ( File.Exists( baseDir + value ) )
            sourceFiles.Add( baseDir + value );
          else
            Console.WriteLine( "Warning: ignoring nonexistent source '{0}' ({1})", value, FileLineNo() );
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

        case "minV":
          if ( double.TryParse( value, NumberStyles.Float, CultureInfo.InvariantCulture, out newDouble ) )
            minV = newDouble;
          break;

        case "maxV":
          if ( double.TryParse( value, NumberStyles.Float, CultureInfo.InvariantCulture, out newDouble ) )
            maxV = newDouble;
          break;

        case "minS":
          if ( double.TryParse( value, NumberStyles.Float, CultureInfo.InvariantCulture, out newDouble ) )
            minS = newDouble;
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
      if ( !wasEvaluated )
        Evaluate();
    }

    static bool wasCompiled = false;
    static bool wasEvaluated = false;

    static Dictionary<string, Assembly> assemblies = new Dictionary<string, Assembly>();

    static Stopwatch sw = new Stopwatch();

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

          wri.WriteLine( "<table>" );
          wri.WriteLine( "<tr><th>Name</th><th>Time</th><th>Image / colors</th></tr>" );

          int ord = 0;
          foreach ( var imageFn in EvalOptions.options.inputFiles )
          {
            wri.WriteLine( "<tr><td>&nbsp;</td></tr>" );

            string relative = Util.MakeRelativePath( EvalOptions.options.outDir, imageFn );
            wri.WriteLine( "<tr><td>&nbsp;</td><td>&nbsp;</td>" );
            wri.WriteLine( "<td><img src=\"{0}\" width=\"{1}\"/></td>",
                           relative, EvalOptions.options.imageWidth );
            wri.WriteLine( "</tr>" );

            Bitmap image = (Bitmap)Image.FromFile( imageFn );

            List<string> names = new List<string>( assemblies.Keys );
            names.Sort();
            foreach ( var name in names )
              if ( !EvalOptions.options.bans.Contains( name ) )
                EvaluateSolution( name, assemblies[ name ], image, wri );

            image.Dispose();

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

      CodeDomProvider P = CodeDomProvider.CreateProvider( "C#" );
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
        source = source.Replace( "_051colormap", "cmap" + name );
        List<string> localSources = new List<string>( globalSources );
        localSources.Add( source );

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

    static void EvaluateSolution ( string name, Assembly ass, Bitmap image, TextWriter wri )
    {
      Color[] colors = null;
      object[] arguments = new object[] { image, 10, colors };

      sw.Restart();
      ass.GetType( "cmap" + name + ".Colormap" ).GetMethod( "Generate" ).Invoke( null, arguments );
      sw.Stop();

      colors = arguments[ 2 ] as Color[];
      double minS = EvalOptions.options.minS;
      double minV = EvalOptions.options.minV;
      double maxV = EvalOptions.options.maxV;

      // report:
      if ( colors != null )
      {
        bool best = EvalOptions.options.best.Contains( name );
        wri.Write( string.Format( CultureInfo.InvariantCulture, "<tr><td class=\"t\">{0}{1}{2}</td><td class=\"t r\">{3:f2}s</td>",
                                  best ? "<b>" : "", name, best ? "</b>" : "", sw.ElapsedMilliseconds * 0.001 ) );

        // color ordering:
        Array.Sort( colors, ( a, b ) =>
        {
          double aH, aS, aV;
          Arith.ColorToHSV( a, out aH, out aS, out aV );
          double aMin = Math.Min( Math.Min( a.R, a.G ), a.B ) / 255.0;

          int aSeg;
          double aM;
          if ( aV < minV )
          {
            aSeg = 0;
            aM = aV;
          }
          else
          if ( aMin > maxV ||
               aS < minS )
          {
            aSeg = 1;
            aM = aMin;
          }
          else
          {
            aSeg = 2;
            aM = aH;
          }

          double bH, bS, bV;
          Arith.ColorToHSV( b, out bH, out bS, out bV );
          double bMin = Math.Min( Math.Min( b.R, b.G ), b.B ) / 255.0;

          int bSeg;
          double bM;
          if ( bV < minV )
          {
            bSeg = 0;
            bM = bV;
          }
          else
          if ( bMin > maxV ||
               bS < minS )
          {
            bSeg = 1;
            bM = bMin;
          }
          else
          {
            bSeg = 2;
            bM = bH;
          }

          if ( aSeg == bSeg )
            return aM.CompareTo( bM );

          return aSeg.CompareTo( bSeg );
        } );

        // SVG color visualization:
        int width = EvalOptions.options.imageWidth;
        int widthBin = width / 10;
        int height = 50;
        int border = 2;
        wri.WriteLine( "<td><svg width=\"{0}\" height=\"{1}\">", width, height );
        int x = 0;
        foreach ( var col in colors )
        {
          string rgb = string.Format( "#{0:X2}{1:X2}{2:X2}", col.R, col.G, col.B );
          wri.WriteLine( "<rect x=\"{0}\" y=\"{1}\" width=\"{2}\" height=\"{3}\" fill=\"{4}\" />",
                         x + border, border, widthBin - 2 * border, height - 2 * border - 14, rgb );
          wri.WriteLine( "<text x=\"{0}\" y=\"{1}\" class=\"rgb\" text-anchor=\"middle\">{2}</text>",
                         x + widthBin / 2, height - border, rgb );
          x += widthBin;
        }
        wri.WriteLine( "</svg></td>" );
        wri.WriteLine( "</tr>" );
      }
    }
  }
}
