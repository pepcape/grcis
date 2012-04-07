using System.Collections.Generic;
using System.Drawing;
using System.Text;

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

        public static string Process ( Bitmap src, int width, int height )
        {
            // !!!{{ TODO: replace this with your own bitmap -> ASCII conversion code

            if ( src == null || width <= 0 || height <= 0 )
                return "";

            float widthBmp  = src.Width;
            float heightBmp = src.Height;

            const char MAX_LEVEL = '#';
            const char MIN_LEVEL = ' ';

            string dest = "";

            for ( int y = 0; y < height; y++ )
            {
                float fYBmp = y * heightBmp / height;

                for ( int x = 0; x < width; x++ )
                {
                    float fXBmp = x * widthBmp / width;

                    Color c = src.GetPixel( (int)fXBmp, (int)fYBmp );

                    // luma = Y = 0.2126*R + 0.7152*G + 0.0722*B
                    // using only integer multiplications and bit shift
                    int luma = (54 * (int)c.R + 183 * (int)c.G + 19 * (int)c.B) >> 8;

                    dest += (luma < 128) ? MAX_LEVEL : MIN_LEVEL;
                }

                dest += "\r\n";
            }

            // !!!}}

            return dest;
        }
    }
}
