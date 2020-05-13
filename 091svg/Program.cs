// Author: Josef Pelikan
//
// Instructions:
//  make changes in 'Customizable parameters' and 'Custom drawing' regions

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using OpenTK;
using Utilities;

namespace _091svg
{
  class Program
  {
    /// <summary>
    /// Defines program version (Rev#).
    /// </summary>
    static Program ()
    {
      Util.SetVersion( "$Rev: 405 $" );
    }

#region Customizable parameters

    /// <summary>
    /// Fractal type. 't' .. test, ..
    /// 'type=t'
    /// </summary>
    static string type = "t";

    /// <summary>
    /// Output SVG file-name.
    /// 'out=xxx.html' or 'out=xxx.svg'
    /// </summary>
    static string outputFileName = "out.html";

    /// <summary>
    /// Either HTML5 or SVG file-format.
    /// 'html=true'
    /// </summary>
    static bool generateHTML = true;

    /// <summary>
    /// Draw intermediate curves?
    /// </summary>
    static bool debug = false;

    /// <summary>
    /// Subdivision level.
    /// 'level=2'
    /// </summary>
    static int level = 2;

    /// <summary>
    /// Subdivision parameter.
    /// 'param=0.25'
    /// </summary>
    static double param = 0.25;

    /// <summary>
    /// List of vertex coordinates [x0,y0,x1,y1,..xN,yN].
    /// 'vert=[x0;y0;x1;y1;..xN;yN]'
    /// </summary>
    static List<double> vert = new List<double>( new double[] { 100.0, 100.0, 120.0, 400.0, 300.0, 450.0, 600.0, 390.0, 400.0, 120.0 } );

    /// <summary>
    /// Sets one specific config option.
    /// </summary>
    /// <param name="key">Option name ('type', 'out', 'html', ..).</param>
    /// <param name="value">New option value (any syntax/type but ',' inside).</param>
    /// <returns>True if the config option was ok.</returns>
    static bool setOption ( string key, string value )
    {
      if ( string.IsNullOrEmpty( key ) )
        return false;

      int newInt;
      double newDouble;

      switch ( key )
      {
        case "type":
          type = value;
          return true;

        case "out":
          outputFileName = value;
          return true;

        case "html":
          generateHTML = Util.positive( value );
          return true;

        case "debug":
          debug = Util.positive( value );
          return true;

        case "level":
          if ( int.TryParse( value, out newInt ) &&
               newInt >= 0 )
            level = newInt;
          return true;

        case "param":
          if ( double.TryParse( value, NumberStyles.Float, CultureInfo.InvariantCulture, out newDouble ) )
            param = newDouble;
          return true;

        case "vert":
          {
            List<double> list = Util.ParseDoubleList( value, ';' );
            if ( list != null &&
                 list.Count > 0 )
              vert = list;
          }
          return true;
      }

      return false;
    }

    static bool drawWasExecuted = false;

    /// <summary>
    /// Handles config-file command.
    /// </summary>
    /// <param name="command">Complete command line read from config.</param>
    /// <returns>True if handled.</returns>
    static bool handleCommand ( string command )
    {
      if ( command == "draw" )
      {
        Draw();
        drawWasExecuted = true;
        return true;
      }

      return false;
    }

#endregion

#region Command-line framework - no need to change

    static void parseDictionary ( Dictionary<string,string> d )
    {
      foreach ( var kvp in d )
        if ( !setOption( kvp.Key, kvp.Value ) )
          Console.WriteLine( "Warning: invalid option '{0}={1}'!", kvp.Key, kvp.Value );
    }

    /// <summary>
    /// Parsing of the text config-file. Key=value pairs, list of commands / input file-names, ..
    /// </summary>
    /// <param name="path">Config file-name.</param>
    static void parseConfig ( string path, bool initial =true )
    {
      string line;
      int lineNumber = 0;

      using ( StreamReader inp = new StreamReader( path ) )
      {
        do
        {
          if ( (line = inp.ReadLine()) != null )
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
            if ( !handleCommand( line ) )
              Console.WriteLine( "Warning: ignoring config-file line {0} ('{1}')", lineNumber, line );
            continue;
          }

          string key = line.Substring( 0, pos ).Trim();
          if ( key.Length < 1 )
          {
            Console.WriteLine( "Warning: ignoring config-file line {0} ('{1}')", lineNumber, line );
            continue;
          }

          string value = line.Substring( pos + 1 ).Trim();
          if ( !setOption( key, value ) )
            Console.WriteLine( "Warning: ignoring config-file line {0} ('{1}')", lineNumber, line );
        }
        while ( true );
      }

      Console.WriteLine( "Finished reading config-file '{0}' ({1}, {2}, {3}, {4})",
                         path, Util.ProgramVersion, Util.TargetFramework, Util.RunningFramework,
                         Util.FormatNowUtc() );
    }

    /// <summary>
    /// Main routine:
    /// </summary>
    /// <param name="args"></param>
    static void Main ( string[] args )
    {
      if ( args.Length < 1 )
        Console.WriteLine( "Warning: no command-line options, using default values!" );
      else
        for ( int i = 0; i < args.Length; i++ )
          if ( !string.IsNullOrEmpty( args[ i ] ) )
          {
            if ( args[ i ] == "-c" )     // process the config file
            {
              if ( ++i < args.Length &&
                   args[ i ] != null &&
                   args[ i ].Length > 0 )       // potentially valid config file-name
                if ( File.Exists( args[ i ] ) )
                {
                  string configFile = Path.GetFullPath( args[ i ] );
                  parseConfig( configFile );
                }
                else
                  Console.WriteLine( "Warning: config-file '{0}' doesn't exist!", args[ i ] );
            }
            else
              if ( args[ i ] == "-a" )   // additional key=value list
              {
                if ( ++i < args.Length &&
                     args[ i ] != null )
                {
                  Dictionary<string, string> p = Util.ParseKeyValueList( args[ i ] );
                  parseDictionary( p );
                }
              }
              else
                if ( args[ i ].Length > 1 &&
                     args[ i ][0] == '-' &&
                     i + 1 < args.Length )      // individual option
                {
                  if ( !setOption( args[ i ].Substring( 1 ), args[ i + 1 ] ) )
                    Console.WriteLine( "Warning: invalid option '{0} {1}'!", args[ i ], args[ i + 1 ] );
                  i++;
                }
                else
                  if ( !handleCommand( args[ i ] ) ) // command?
                    Console.WriteLine( "Warning: invalid command '{0}'!", args[ i ] );
            }

      if ( !drawWasExecuted )
        Draw();
    }

#endregion

#region Custom drawing

    /// <summary>
    /// One subdivision step - Chaikin algorithm.
    /// See http://www.cs.unc.edu/~dm/UNC/COMP258/LECTURES/Chaikins-Algorithm.pdf
    /// </summary>
    /// <param name="src">Source vertex-array.</param>
    /// <returns>Result vertex array.</returns>
    static List<Vector2d> Subdivide ( List<Vector2d> src )
    {
      List<Vector2d> result = new List<Vector2d>( src.Count * 2 );
      for ( int i = 0; i + 1 < src.Count; i++ )
      {
        Vector2d f = src[ i ] * (1.0 - param) + src[ i + 1 ] *        param;
        Vector2d s = src[ i ] *        param  + src[ i + 1 ] * (1.0 - param);
        result.Add( f );
        result.Add( s );
      }

      return result;
    }

    /// <summary>
    /// Writes one polyline in SVG format to the given output stream.
    /// </summary>
    /// <param name="wri">Opened output stream (must be left open).</param>
    /// <param name="workList">List of vertices.</param>
    /// <param name="x0">Origin - x-coord (will be subtracted from all x-coords).</param>
    /// <param name="y0">Origin - y-coord (will be subtracted from all y-coords)</param>
    /// <param name="color">Line color (default = black).</param>
    static void drawCurve ( StreamWriter wri, List<Vector2d> workList, double x0, double y0, string color ="#000" )
    {
      StringBuilder sb = new StringBuilder();
      sb.AppendFormat( CultureInfo.InvariantCulture, "M{0:f2},{1:f2}",
                       workList[ 0 ].X - x0, workList[ 0 ].Y - y0 );
      for ( int i = 1; i < workList.Count; i++ )
        sb.AppendFormat( CultureInfo.InvariantCulture, "L{0:f2},{1:f2}",
                         workList[ i ].X - x0, workList[ i ].Y - y0 );

      wri.WriteLine( "<path d=\"{0}\" stroke=\"{1}\" fill=\"none\"/>", sb.ToString(), color );
    }

    /// <summary>
    /// Draws vertices in SVG format to the given output stream.
    /// For debugging purposes.
    /// </summary>
    /// <param name="wri">Opened output stream (must be left open).</param>
    /// <param name="workList">List of vertices.</param>
    /// <param name="x0">Origin - x-coord (will be subtracted from all x-coords).</param>
    /// <param name="y0">Origin - y-coord (will be subtracted from all y-coords)</param>
    /// <param name="color">Vertex color (default = lime).</param>
    static void drawVertices ( StreamWriter wri, List<Vector2d> workList, double x0, double y0, string color ="lime" )
    {
      for ( int i = 0; i < workList.Count; i++ )
        wri.WriteLine( string.Format( CultureInfo.InvariantCulture, "<circle cx=\"{0:f2}\" cy=\"{1:f2}\" r=\"1\" fill=\"{2}\"/>",
                                      workList[ i ].X - x0, workList[ i ].Y - y0, color ) );
    }

    /// <summary>
    /// Generates a SVG output according to globally stored parameters (see setOption() function)..
    /// Must be able to be executed more than once.
    /// </summary>
    static void Draw ()
    {
      if ( type == "t" )
      {
        if ( vert.Count < 4 )
        {
          Console.WriteLine( "Error: not enough vertices to subdivide ({0})!", vert.Count );
          return;
        }

        List<Vector2d> workList = new List<Vector2d>( 2 + vert.Count / 2 );
        double minX = double.MaxValue;
        double maxX = double.MinValue;
        double minY = double.MaxValue;
        double maxY = double.MinValue;
        int i;

        workList.Add( new Vector2d( vert[ 0 ], vert[ 1 ] ) );
        for ( i = 0; i + 1 < vert.Count; i += 2 )
        {
          if ( vert[ i ] < minX ) minX = vert[ i ];
          if ( vert[ i ] > maxX ) maxX = vert[ i ];
          if ( vert[ i + 1 ] < minY ) minY = vert[ i + 1 ];
          if ( vert[ i + 1 ] > maxY ) maxY = vert[ i + 1 ];
          workList.Add( new Vector2d( vert[ i ], vert[ i + 1 ] ) );
        }
        workList.Add( new Vector2d( vert[ i - 2 ], vert[ i - 1 ] ) );

        // workList now contains input polygon with duplicated V[0] and V[N-1]
        // bounding box: [minX,minY]-[maxX,maxY]

        // SVG output:
        using ( StreamWriter wri = new StreamWriter( outputFileName ) )
        {
          double x0 = minX - 5.0;   // here a 'border' option should be used..
          double y0 = minY - 5.0;
          double width = maxX - minX + 10.0;
          double height = maxY - minY + 10.0;
          if ( generateHTML )
          {
            wri.WriteLine( "<!DOCTYPE html>" );
            wri.WriteLine( "<meta charset=\"utf-8\">" );
            wri.WriteLine( "<title>SVG test</title>" );
            wri.WriteLine( string.Format( CultureInfo.InvariantCulture, "<svg width=\"{0:f0}\" height=\"{1:f0}\">", width, height ) );
          }
          else
            wri.WriteLine( string.Format( CultureInfo.InvariantCulture, "<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"{0:f0}\" height=\"{1:f0}\">", width, height ) );

          if ( debug )
            drawCurve( wri, workList, x0, y0, string.Format( "#{0:X2}{0:X2}{0:X2}", Math.Min( 255, level * 20 ) ) );

          for ( int g = 0; g < level; g++ )
          {
            Console.WriteLine( "Subdividing {0}", g );
            workList = Subdivide( workList );
            if ( debug ||
                 g == level - 1 )
              drawCurve( wri, workList, x0, y0, string.Format( "#{0:X2}{0:X2}{0:X2}", Math.Min( 255, (level - 1 - g) * 20 ) ) );
          }

          if ( debug )
            drawVertices( wri, workList, x0, y0, "red" );

          wri.WriteLine( "</svg>" );
        }
      }
      else
        Console.WriteLine( "Error: unknown output type '{0}'!", type );
    }

#endregion
  }
}
