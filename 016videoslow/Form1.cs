using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;

namespace _016videoslow
{
  public partial class Form1 : Form
  {
    protected Bitmap inputImage = null;

    protected Bitmap outputImage = null;

    public Form1 ()
    {
      InitializeComponent();
    }

    private void buttonEncode_Click ( object sender, EventArgs e )
    {
      VideoCodec vc = new VideoCodec();

      FileStream fs = new FileStream( "videocode.bin", FileMode.Create );
      Stream s;

      string fn = String.Format( textInputMask.Text, 0 );
      if ( File.Exists( fn ) )
      {
        inputImage = (Bitmap)Image.FromFile( fn );
        s = vc.EncodeHeader( inputImage.Width, inputImage.Height, 10, fs );
        int i = 0;
        do
        {
          vc.EncodeFrame( i, inputImage, s );
          fn = String.Format( textInputMask.Text, ++i );
          if ( !File.Exists( fn ) ) break;
          inputImage = (Bitmap)Image.FromFile( fn );
        } while ( true );

        s.Close();
      }
    }

    private void buttonDecode_Click ( object sender, EventArgs e )
    {
      VideoCodec vc = new VideoCodec();
    }
  }
}
