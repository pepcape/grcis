using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Raster;

namespace _025contours
{
  public partial class Form1 : Form
  {
    /// <summary>
    /// Initialize the function-selection combo-box.
    /// </summary>
    protected void InitializeFunctions ()
    {
      functions = new List<Func<double, double, double>>();

      // 0:
      functions.Add( ( double x, double y ) => Math.Sin( 0.1 * x ) + Math.Cos( 0.1 * y ) );
      comboFunction.Items.Add( "Waves 0" );
      // 1:
      functions.Add( ( double x, double y ) =>
      {
        double r = 0.1 * Math.Sqrt( x * x + y * y );
        return (r <= Double.Epsilon) ? 10.0 : (10.0 * Math.Sin( r ) / r);
      } );
      comboFunction.Items.Add( "Drop 0" );

      comboFunction.SelectedIndex = 0;
      f = functions[ 0 ];

      // threshold set
      for ( double d = -10.0; d <= 10.0; d += 0.2 )
        thr.Add( d );
    }

    /// <summary>
    /// Draw contour field to the given Bitmap
    /// using class members: origin, scale, valueDrift, thr
    /// </summary>
    protected void ComputeContours ( Bitmap image )
    {
      if ( f == null ) return;

      // !!!{{ TODO: write your own contours drawing code here

      int width  = image.Width;
      int height = image.Height;

      for ( int y = 0; y < height; y++ )
      {
        double dy = (y - origin.Y) * scale;
        for ( int x = 0; x < width; x++ )
        {
          double dx = (x - origin.X) * scale;
          double val = f( dx, dy ) + valueDrift;
          image.SetPixel( x, y, Draw.ColorRamp( val * 0.1 + 0.5 ) );
        }
      }

      // !!!}}
    }
  }
}
