// Alpha-channel experiments

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using Utilities;

namespace _084filter
{
  class Filter
  {
    /// <summary>
    /// Optional data initialization.
    /// </summary>
    public static void InitParams ( out string param, out string name )
    {
      param = "mode=b,showbg=[255;255;255]";
      name = "Josef Pelikán";
    }

    /// <summary>
    /// Recompute the output image.
    /// </summary>
    /// <param name="input">Input image.</param>
    /// <param name="param">Optional string parameter (its content and format is entierely up to you).</param>
    public static Bitmap Recompute ( Bitmap input, string param )
    {
      if ( input == null )
        return null;

      // custom parameters from the text-field:
      Dictionary<string, string> p = Util.ParseKeyValueList( param );
      Color bg  = Color.White;
      Color fg1 = Color.Black;
      Color fg2 = Color.FromArgb( 48, 112, 64 );

      int mode = 0; // default = modify alpha
      float acoeff = 1.0f;
      float tolerance = 4.0f;

      if ( p.Count > 0 )
      {
        List<int> col = null;

        // mode = { a, b }
        string command;
        if ( p.TryGetValue( "mode", out command ) )
          switch ( command )
          {
            case "b":
              mode = 1;
              break;
            default:
              mode = 0;
              break;
          }

        // bg = [r;g;b]
        if ( Util.TryParse( p, "bg", ref col, ';' ) &&
             col.Count >= 3 )
          bg = Color.FromArgb( Util.Clamp( col[ 0 ], 0, 255 ),
                               Util.Clamp( col[ 1 ], 0, 255 ),
                               Util.Clamp( col[ 2 ], 0, 255 ) );

        // fg = [r;g;b]
        if ( Util.TryParse( p, "fg", ref col, ';' ) &&
             col.Count >= 3 )
          fg1 = Color.FromArgb( Util.Clamp( col[ 0 ], 0, 255 ),
                                Util.Clamp( col[ 1 ], 0, 255 ),
                                Util.Clamp( col[ 2 ], 0, 255 ) );

        // fg2 = [r;g;b]
        if ( Util.TryParse( p, "fg2", ref col, ';' ) &&
             col.Count >= 3 )
          fg2 = Color.FromArgb( Util.Clamp( col[ 0 ], 0, 255 ),
                                Util.Clamp( col[ 1 ], 0, 255 ),
                                Util.Clamp( col[ 2 ], 0, 255 ) );

        // showbg = [r;g;b]
        if ( Util.TryParse( p, "showbg", ref col, ';' ) &&
             col.Count >= 3 )
          Form1.imageBoxBackground = Color.FromArgb( Util.Clamp( col[ 0 ], 0, 255 ),
                                                     Util.Clamp( col[ 1 ], 0, 255 ),
                                                     Util.Clamp( col[ 2 ], 0, 255 ) );

        // alpha = <alpha-coeff>
        if ( Util.TryParse( p, "alpha", ref acoeff ) )
          acoeff = Math.Max( 0.0f, acoeff );

        // tolerance = <float>
        if ( Util.TryParse( p, "tolerance", ref tolerance ) )
          tolerance = Math.Max( 0.0f, tolerance );
      }

      // keeping image size
      int wid = input.Width;
      int hei = input.Height;

      // output image in 32bit RGBA format
      Bitmap output = new Bitmap( wid, hei, PixelFormat.Format32bppArgb );

      // convert pixel data:
      int x, y, a;
      float dR, dG, dB, RR, GG, BB;
      Color ic, oc;
      float[] fb1 = new float[ 3 ];
      float[] fb2 = new float[ 3 ];
      float fbNN1 = 1.0f;
      float fbNN2 = 1.0f;
      float t0 = 0.0f;
      float tolerance1 = 0.0f;
      float tolerance2 = 0.0f;

      if ( mode == 1 )
      {
        fb1[ 0 ] = fg1.R - bg.R;
        fb1[ 1 ] = fg1.G - bg.G;
        fb1[ 2 ] = fg1.B - bg.B;
        fbNN1 = Math.Max( 1.0f, fb1[ 0 ] * fb1[ 0 ] + fb1[ 1 ] * fb1[ 1 ] + fb1[ 2 ] * fb1[ 2 ] );
        tolerance1 = tolerance / (float)Math.Sqrt( fbNN1 );
        fb2[ 0 ] = fg2.R - bg.R;
        fb2[ 1 ] = fg2.G - bg.G;
        fb2[ 2 ] = fg2.B - bg.B;
        fbNN2 = Math.Max( 1.0f, fb2[ 0 ] * fb2[ 0 ] + fb2[ 1 ] * fb2[ 1 ] + fb2[ 2 ] * fb2[ 2 ] );
        tolerance2 = tolerance / (float)Math.Sqrt( fbNN2 );
        tolerance *= tolerance;
      }

      // slow GetPixel-SetPixel code:
      for ( y = 0; y < hei; y++ )
      {
        if ( !Form1.cont ) break;

        switch ( mode )
        {
          case 1:      // 1 .. create alpha from bg/fg
            for ( x = 0; x < wid; x++ )
            {
              ic = input.GetPixel( x, y );

              // trying the 1st foreground color (fg1):
              dR = ic.R - (float)bg.R;
              dG = ic.G - (float)bg.G;
              dB = ic.B - (float)bg.B;
              t0 = (dR * fb1[ 0 ] + dG * fb1[ 1 ] + dB * fb1[ 2 ]) / fbNN1;
              RR = (bg.R + t0 * fb1[ 0 ]) - ic.R;
              GG = (bg.G + t0 * fb1[ 1 ]) - ic.G;
              BB = (bg.B + t0 * fb1[ 2 ]) - ic.B;

              if ( RR * RR + GG * GG + BB * BB < tolerance &&
                   t0 > -tolerance1 &&
                   t0 < 1.0f + tolerance1 )     // fg1 foreground match
                if ( t0 <= 0.0f )
                  oc = Color.FromArgb( 0, 0, 0, 0 );
                else
                {
                  t0 *= ic.A / 255.0f;
                  oc = Color.FromArgb( (int)Math.Min( 255.0f * t0, 255.0f ), fg1.R, fg1.G, fg1.B );
                }
              else
              {
                // trying the 2nd foreground color (fg2):
                t0 = (dR * fb2[ 0 ] + dG * fb2[ 1 ] + dB * fb2[ 2 ]) / fbNN2;
                RR = (bg.R + t0 * fb2[ 0 ]) - ic.R;
                GG = (bg.G + t0 * fb2[ 1 ]) - ic.G;
                BB = (bg.B + t0 * fb2[ 2 ]) - ic.B;
                if ( RR * RR + GG * GG + BB * BB < tolerance &&
                     t0 > -tolerance2 &&
                     t0 < 1.0f + tolerance2 )   // fg2 foreground match
                  if ( t0 <= 0.0f )
                    oc = Color.FromArgb( 0, 0, 0, 0 );
                  else
                  {
                    t0 *= ic.A / 255.0f;
                    oc = Color.FromArgb( (int)Math.Min( 255.0f * t0, 255.0f ), fg2.R, fg2.G, fg2.B );
                  }
                else
                  oc = Color.FromArgb( ic.A, ic.R, ic.G, ic.B );
              }

              output.SetPixel( x, y, oc );
            }
            break;

          default:     // 0 .. modify alpha
            for ( x = 0; x < wid; x++ )
            {
              ic = input.GetPixel( x, y );

              a = Util.Clamp( (int)(ic.A * acoeff), 0, 255 );
              oc = Color.FromArgb( a, ic.R, ic.G, ic.B );

              output.SetPixel( x, y, oc );
            }
            break;
        }
      }

      return output;
    }
  }
}
