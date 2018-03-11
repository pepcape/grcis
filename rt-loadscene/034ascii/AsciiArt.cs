// Author: Jan Dupej, Josef Pelikan

using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Raster;

namespace _034ascii
{
  public class AsciiArt
  {
    public static Font GetFont ()
    {
      // !!!{{ TODO: if you need a font different from the default one, change this..

      return null;
      //return new Font( "Lucida Console", 6.0f );

      // !!!}}
    }

    /// <summary>
    /// Converts the given input bitmap into an ASCCI art string.
    /// </summary>
    /// <param name="src">Source image.</param>
    /// <param name="width">Required output width in characters.</param>
    /// <param name="height">Required output height in characters.</param>
    /// <param name="param">Textual parameter.</param>
    /// <returns>String (height x width ASCII table)</returns>
    public static string Process ( Bitmap src, int width, int height, string param )
    {
      // !!!{{ TODO: replace this with your own bitmap -> ASCII conversion code

      if ( src == null || width <= 0 || height <= 0 )
        return "";

      float widthBmp  = src.Width;
      float heightBmp = src.Height;

      const char MAX_LEVEL = '#';
      const char MIN_LEVEL = ' ';

      StringBuilder sb = new StringBuilder();

      for ( int y = 0; y < height; y++ )
      {
        float fYBmp = y * heightBmp / height;

        for ( int x = 0; x < width; x++ )
        {
          float fXBmp = x * widthBmp / width;

          Color c = src.GetPixel( (int)fXBmp, (int)fYBmp );

          int luma = Draw.RgbToGray( c.R, c.G, c.B );

          // Alternative (luma): Y = 0.2126 * R + 0.7152 * G + 0.0722 * B
          //int luma = (54 * (int)c.R + 183 * (int)c.G + 19 * (int)c.B) >> 8;

          sb.Append( (luma < 128) ? MAX_LEVEL : MIN_LEVEL );
        }

        sb.Append( "\r\n" );
      }

      // !!!}}

      return sb.ToString();
    }
  }
}
