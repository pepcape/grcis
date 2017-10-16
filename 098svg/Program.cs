using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using MathSupport;
using OpenTK;
using Utilities;

namespace _098svg
{
  public class CmdOptions : Options
  {
    /// <summary>
    /// Put your name here.
    /// </summary>
    public string name = "Josef Pelikán";

    /// <summary>
    /// Singleton instance.
    /// </summary>
    public static new CmdOptions options = (CmdOptions)(Options.options = new CmdOptions());

    public override void StringStatistics ( long[] result )
    {
      if ( result == null || result.Length < 4 )
        return;

      Util.StringStat( commands, result );
    }

    static CmdOptions ()
    {
      project = "svg098";
      TextPersistence.Register( new CmdOptions(), 0 );

      RegisterMsgModes( "debug" );
    }

    public CmdOptions ()
    {
      // default values of structured members:
      baseDir = @"./";
    }

    public static void Touch ()
    {
      if ( options == null )
        Util.Log( "CmdOptions not initialized!" );
    }

    //--- project-specific options ---

    /// <summary>
    /// Output directory with trailing dir separator.
    /// </summary>
    public string outDir = @"./";

    /// <summary>
    /// Number of maze columns (horizontal size in cells).
    /// </summary>
    public int columns = 12;

    /// <summary>
    /// Number of maze rows (vertical size in cells).
    /// </summary>
    public int rows = 8;

    /// <summary>
    /// Difficulty coefficient (optional).
    /// </summary>
    public double difficulty = 1.0;

    /// <summary>
    /// Maze width in SVG units (for SVG header).
    /// </summary>
    public double width = 600.0;

    /// <summary>
    /// Maze height in SVG units (for SVG header).
    /// </summary>
    public double height = 400.0;

    /// <summary>
    /// RandomJames generator seed, 0 for randomize.
    /// </summary>
    public long seed = 0L;

    /// <summary>
    /// Generate HTML5 file? (else - direct SVG format)
    /// </summary>
    public bool html = false;

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
      long newLong;
      double newDouble = 0.0;

      switch ( key )
      {
        case "outDir":
          outDir = value;
          break;

        case "name":
          name = value;
          break;

        case "columns":
          if ( int.TryParse( value, out newInt ) &&
               newInt > 0 )
            columns = newInt;
          break;

        case "rows":
          if ( int.TryParse( value, out newInt ) &&
               newInt > 0 )
            rows = newInt;
          break;

        case "difficulty":
          if ( double.TryParse( value, NumberStyles.Float, CultureInfo.InvariantCulture, out newDouble ) &&
               newDouble > 0.0 )
            difficulty = newDouble;
          break;

        case "width":
          if ( double.TryParse( value, NumberStyles.Float, CultureInfo.InvariantCulture, out newDouble ) &&
               newDouble > 0 )
            width = newDouble;
          break;

        case "height":
          if ( double.TryParse( value, NumberStyles.Float, CultureInfo.InvariantCulture, out newDouble ) &&
               newDouble > 0 )
            height = newDouble;
          break;

        case "seed":
          if ( long.TryParse( value, out newLong ) &&
               newLong >= 0L )
            seed = newLong;
          break;

        case "html":
          html = Util.positive( value );
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
        case "seed":
          seed = 0L;
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
        case "generate":
          Program.Generate();
          return true;
      }

      return false;
    }
  }

  class Program
  {
    /// <summary>
    /// The 'generate' command was executed at least once..
    /// </summary>
    static bool wasGenerated = false;

    static void Main ( string[] args )
    {
      CmdOptions.Touch();

      if ( args.Length < 1 )
        Console.WriteLine( "Warning: no command-line options, using default values!" );
      else
        for ( int i = 0; i < args.Length; i++ )
          if ( !string.IsNullOrEmpty( args[ i ] ) )
          {
            string opt = args[ i ];
            if ( !CmdOptions.options.ParseOption( args, ref i ) )
              Console.WriteLine( $"Warning: invalid option '{opt}'!" );
          }

      if ( !wasGenerated )
        Generate();
    }

    /// <summary>
    /// Writes one polyline in SVG format to the given output stream.
    /// </summary>
    /// <param name="wri">Opened output stream (must be left open).</param>
    /// <param name="workList">List of vertices.</param>
    /// <param name="x0">Origin - x-coord (will be subtracted from all x-coords).</param>
    /// <param name="y0">Origin - y-coord (will be subtracted from all y-coords)</param>
    /// <param name="color">Line color (default = black).</param>
    static void drawCurve ( StreamWriter wri, List<Vector2> workList, double x0, double y0, string color = "#000" )
    {
      StringBuilder sb = new StringBuilder();
      sb.AppendFormat( CultureInfo.InvariantCulture, "M{0:f2},{1:f2}",
                       workList[ 0 ].X - x0, workList[ 0 ].Y - y0 );
      for ( int i = 1; i < workList.Count; i++ )
        sb.AppendFormat( CultureInfo.InvariantCulture, "L{0:f2},{1:f2}",
                         workList[ i ].X - x0, workList[ i ].Y - y0 );

      wri.WriteLine( "<path d=\"{0}\" stroke=\"{1}\" fill=\"none\"/>", sb.ToString(), color );
    }

    static public void Generate ()
    {
      wasGenerated = true;

      // !!!{{ TODO - generate and draw maze in SVG format

      string fileName = CmdOptions.options.outputFileName;
      if ( string.IsNullOrEmpty( fileName ) )
        fileName = CmdOptions.options.html ? "out.html" : "out.svg";
      string outFn = Path.Combine( CmdOptions.options.outDir, fileName );

      // SVG output:
      using ( StreamWriter wri = new StreamWriter( outFn ) )
      {
        if ( CmdOptions.options.html )
        {
          wri.WriteLine( "<!DOCTYPE html>" );
          wri.WriteLine( "<meta charset=\"utf-8\">" );
          wri.WriteLine( $"<title>SVG test ({CmdOptions.options.name})</title>" );
          wri.WriteLine( string.Format( CultureInfo.InvariantCulture, "<svg width=\"{0:f0}\" height=\"{1:f0}\">",
                                        CmdOptions.options.width, CmdOptions.options.height ) );
        }
        else
          wri.WriteLine( string.Format( CultureInfo.InvariantCulture, "<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"{0:f0}\" height=\"{1:f0}\">",
                                        CmdOptions.options.width, CmdOptions.options.height ) );

        List<Vector2> workList = new List<Vector2>();
        RandomJames rnd = new RandomJames();
        if ( CmdOptions.options.seed > 0L )
          rnd.Reset( CmdOptions.options.seed );
        else
          rnd.Randomize();

        for ( int i = 0; i < CmdOptions.options.columns; i++ )
          workList.Add( new Vector2( rnd.RandomFloat( 0.0f, (float)CmdOptions.options.width ),
                                     rnd.RandomFloat( 0.0f, (float)CmdOptions.options.height ) ) );

        drawCurve( wri, workList, 0, 0, string.Format( "#{0:X2}{0:X2}{0:X2}", 0 ) );

        wri.WriteLine( "</svg>" );

        // !!!}}
      }
    }
  }
}
