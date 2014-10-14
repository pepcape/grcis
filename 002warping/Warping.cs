using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace _002warping
{
  public partial class FormWarping : Form
  {
    /// <summary>
    /// Initialize the function-selection combo-box.
    /// </summary>
    protected void InitializeFunctions ()
    {
      functions = new List<IWarp>();

      // 0:
      functions.Add( new WarpMagniGlass() );
      comboFunction.Items.Add( "MagniGlass" );

      // 1
      functions.Add( new SpiralWarp() );
      comboFunction.Items.Add( "Spiral" );

      // 2:
      functions.Add( new InvSpiralWarp() );
      comboFunction.Items.Add( "InvSpiral" );

      // 3:
      // !!!{{ TODO: insert your own warping function here
      // !!!}}

      comboFunction.SelectedIndex = 0;
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
      output = new Bitmap( owidth, oheight, System.Drawing.Imaging.PixelFormat.Format24bppRgb );
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
