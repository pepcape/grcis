// Author: Josef Pelikan

using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace _002warping
{
  public class FormSupport
  {
    /// <summary>
    /// Initialize the function-selection combo-box.
    /// </summary>
    public static void InitializeFunctions ( FormWarping form )
    {
      form.functions = new List<IWarp>();

      // 0:
      form.functions.Add( new WarpMagniGlass() );
      form.ComboFunction.Items.Add( "MagniGlass" );

      // 1
      form.functions.Add( new WarpSpiral() );
      form.ComboFunction.Items.Add( "Spiral" );

      // 2:
      form.functions.Add( new WarpInvSpiral() );
      form.ComboFunction.Items.Add( "InvSpiral" );

      // 3:
      // !!!{{ TODO: insert your own warping function here
      // !!!}}

      form.ComboFunction.SelectedIndex = 0;
    }
  }

  public class Warping
  {
    public static void WarpImage ( Bitmap input, out Bitmap output, IWarp warp )
    {
      // !!!{{ TODO: write your own warping algorithm here

      int width  = input.Width;
      int height = input.Height;
      int owidth, oheight;
      warp.OutputSize( width, height, out owidth, out oheight );
      output = new Bitmap( owidth, oheight, PixelFormat.Format24bppRgb );
      for ( int y = 0; y < oheight; y++ )
        for ( int x = 0; x < owidth; x++ )
        {
          double xd, yd;
          warp.FInv( (double)x, (double)y, out xd, out yd );
          if ( xd < 0.0 ) xd = 0.0;
          if ( xd > width - 1.0 ) xd = width - 1.0;
          if ( yd < 0.0 ) yd = 0.0;
          if ( yd > height - 1.0 ) yd = height - 1.0;
          Color col = input.GetPixel( (int)xd, (int)yd );
          output.SetPixel( x, y, col );
        }

      // !!!}}
    }
  }
}
