// Author: Josef Pelikan

using System;
using System.Collections.Generic;
using System.Drawing;
using OpenTK;
using Raster;

namespace _025contours
{
  public class FormSupport
  {
    /// <summary>
    /// Initialize the function-selection combo-box.
    /// </summary>
    public static void InitializeFunctions ( Form1 form )
    {
      form.functions = new List<Func<double, double, double>>();

      // 0:
      form.functions.Add( ( double x, double y ) => Math.Sin( 0.1 * x ) + Math.Cos( 0.1 * y ) );
      form.ComboFunction.Items.Add( "Waves 0" );
      // 1:
      form.functions.Add( ( double x, double y ) =>
      {
        double r = 0.1 * Math.Sqrt( x * x + y * y );
        return (r <= Double.Epsilon) ? 10.0 : (10.0 * Math.Sin( r ) / r);
      } );
      form.ComboFunction.Items.Add( "Drop 0" );

      form.ComboFunction.SelectedIndex = 0;
      form.f = form.functions[ 0 ];

      // threshold set
      for ( double d = -10.0; d <= 10.0; d += 0.2 )
        form.thr.Add( d );
    }

  }

  public class Contours
  {
    /// <summary>
    /// Draw contour field to the given Bitmap
    /// using class members: origin, scale, valueDrift, thr
    /// </summary>
    public static void ComputeContours ( Form1 form, Bitmap image )
    {
      if ( form.f == null ) return;

      // !!!{{ TODO: write your own contours drawing code here

      int width  = image.Width;
      int height = image.Height;
      Vector2d orig = form.origin;

      for ( int y = 0; y < height; y++ )
      {
        double dy = (y - orig.Y) * form.scale;
        for ( int x = 0; x < width; x++ )
        {
          double dx = (x - orig.X) * form.scale;
          double val = form.f( dx, dy ) + form.valueDrift;
          image.SetPixel( x, y, Draw.ColorRamp( val * 0.1 + 0.5 ) );
        }
      }

      // !!!}}
    }
  }
}
