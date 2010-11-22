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

namespace _012animation
{
  public partial class Form1 : Form
  {
    protected Bitmap outputImage = null;

    public Form1 ()
    {
      InitializeComponent();
    }

    private void buttonStart_Click ( object sender, EventArgs e )
    {
      int maxFrame = (int)numericFrames.Value;
      int width    = (int)numericXres.Value;
      int height   = (int)numericYres.Value;
      bool show    = checkShow.Checked;

      Cursor.Current = Cursors.WaitCursor;

      Animation anim = new Animation();

      for ( int i = 0; i < maxFrame; i++ )
      {
        outputImage = anim.RenderFrame( width, height, i, maxFrame );
        if ( show )
          pictureBox1.Image = outputImage;
        labelElapsed.Text = String.Format( "Frame: {0}", i );
        string fileName = String.Format( "out{0:0000}.png", i );
        outputImage.Save( fileName, System.Drawing.Imaging.ImageFormat.Png );
      }

      Cursor.Current = Cursors.Default;
    }

    private void buttonStop_Click ( object sender, EventArgs e )
    {
    }
  }
}
