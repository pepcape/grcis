// Author: Josef Pelikan

using System;
using System.Drawing;
using OpenTK;
using Raster;
using System.Collections.Generic;

namespace _069subdivision
{
  public class Subdivision
  {
    /// <summary>
    /// Separator for string parameter.
    /// </summary>
    static readonly char COMMA = ',';

    static double ChaikinCoef = 0.25;

    public static void SetParam ( string name, int value )
    {
    }

    public static void SetParam ( string name, double value )
    {
      if ( name.Equals( "coef", StringComparison.InvariantCultureIgnoreCase ) )
        ChaikinCoef = value;
    }

    public static void SetParam ( string name, bool value )
    {
    }

    /// <summary>
    /// Draw one subdivision curve.
    /// </summary>
    /// <param name="output">Target bitmap.</param>
    /// <param name="P">Array of control points.</param>
    /// <param name="col">Drawing color.</param>
    public static void DrawCurve ( Bitmap output, List<Vector2d> P, Color col )
    {
      // !!!{{ TODO: write your own subdivision curve rasterization code here

      int i;
      for ( i = 0; i < P.Count - 1; i++ )
        Draw.Line( output, (int)Math.Round( P[ i ].X ), (int)Math.Round( P[ i ].Y ), (int)Math.Round( P[ i + 1 ].X ), (int)Math.Round( P[ i + 1 ].Y ), col );

      // !!!}}
    }

    /// <summary>
    /// Draw the test image into pre-allocated bitmap.
    /// </summary>
    /// <param name="output">Bitmap to fill.</param>
    /// <param name="param">Optional parameter string.</param>
    public static void TestImage ( Bitmap output, string param )
    {
      // !!!{{ TODO: write your own test-image drawing here

      int width  = output.Width;
      int height = output.Height;

      List<Vector2d> P = new List<Vector2d>();
      P.Add( new Vector2d( width * 0.05, height * 0.06 ) );
      P.Add( new Vector2d( width * 0.45, height * 0.16 ) );
      P.Add( new Vector2d( width * 0.37, height * 0.86 ) );
      P.Add( new Vector2d( width * 0.07, height * 0.86 ) );
      P.Add( new Vector2d( width * 0.05, height * 0.06 ) );

      DrawCurve( output, P, Color.White );

      P.Clear();
      P.Add( new Vector2d( width * 0.55, height * 0.76 ) );
      P.Add( new Vector2d( width * 0.55, height * 0.76 ) );
      P.Add( new Vector2d( width * 0.55, height * 0.08 ) );
      P.Add( new Vector2d( width * 0.75, height * 0.42 ) );
      P.Add( new Vector2d( width * 0.95, height * 0.08 ) );
      P.Add( new Vector2d( width * 0.95, height * 0.76 ) );
      P.Add( new Vector2d( width * 0.95, height * 0.76 ) );

      DrawCurve( output, P, Color.Yellow );

      // !!!}}
    }
  }
}
