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
using System.Threading;

namespace _012animation
{
  public partial class Form1 : Form
  {
    protected Bitmap outputImage = null;

    volatile protected Thread aThread = null;

    volatile protected bool cont = true;

    delegate void SetImageCallback ();

    private void SetImage ()
    {
      pictureBox1.Image = outputImage;
      pictureBox1.Invalidate();
    }

    delegate void SetTextCallback ( string text );

    private void SetText ( string text )
    {
      labelElapsed.Text = text;
    }

    delegate void EndAnimationCallback ();

    private void EndAnimation ()
    {
      buttonStart.Enabled = true;
      buttonStop.Enabled  = false;
      aThread = null;
    }

    public Form1 ()
    {
      InitializeComponent();
    }

    public void RenderAnimation ()
    {
      int maxFrame = (int)numericFrames.Value;
      int width    = (int)numericXres.Value;
      int height   = (int)numericYres.Value;

      Animation anim = new Animation();
      SetImageCallback si = new SetImageCallback( SetImage );
      SetTextCallback  st = new SetTextCallback( SetText );

      for ( int i = 0; i < maxFrame; i++ )
      {
        if ( !cont ) return;

        outputImage = anim.RenderFrame( width, height, i, maxFrame );
        if ( checkShow.Checked )
          Invoke( si );
        Invoke( st, new object[] { String.Format( "Frame: {0}", i ) } );
        string fileName = String.Format( "out{0:0000}.png", i );
        outputImage.Save( fileName, System.Drawing.Imaging.ImageFormat.Png );
      }

      EndAnimationCallback ea = new EndAnimationCallback( EndAnimation );
      Invoke( ea );
    }

    private void buttonStart_Click ( object sender, EventArgs e )
    {
      if ( aThread != null ) return;

      buttonStart.Enabled = false;
      buttonStop.Enabled  = true;
      cont = true;

      aThread = new Thread( new ThreadStart( this.RenderAnimation ) );
      aThread.Start();
    }

    private void buttonStop_Click ( object sender, EventArgs e )
    {
      if ( aThread == null ) return;
      cont = false;

      aThread.Join();

      EndAnimation();
    }
  }
}
