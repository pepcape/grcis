using System.Drawing;
using MathSupport;

namespace _052colortransform
{
  class Transform
  {
    /// <summary>
    /// Separator for string parameter.
    /// </summary>
    static readonly char[] COMMA = new char[] { ',' };

    public static void TransformImage ( Bitmap input, out Bitmap output, string param )
    {
      // !!!{{ TODO: write your own color transformation code here

      // Text parameter = color shift
      int dR = 0;
      int dG = 0;
      int dB = 0;
      if ( param.Length > 0 )
      {
        string[] delta = param.Split( COMMA );
        if ( delta.Length >= 3 )
        {
          int.TryParse( delta[ 0 ], out dR );
          int.TryParse( delta[ 1 ], out dG );
          int.TryParse( delta[ 2 ], out dB );
        }
      }

      int width  = input.Width;
      int height = input.Height;
      output = new Bitmap( width, height, System.Drawing.Imaging.PixelFormat.Format24bppRgb );
      for ( int y = 0; y < height; y++ )
        for ( int x = 0; x < width; x++ )
        {
          Color col = input.GetPixel( x, y );
          //double H, S, V;
          //Arith.ColorToHSV( col, out H, out S, out V );
          col = Color.FromArgb( Arith.Clamp( col.R + dR, 0, 255 ),
                                Arith.Clamp( col.G + dG, 0, 255 ),
                                Arith.Clamp( col.B + dB, 0, 255 ) );
          //col = Arith.HSVToColor( H, S * 2.0, V );
          output.SetPixel( x, y, col );
        }

      // !!!}}
    }
  }
}
