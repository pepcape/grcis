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
    protected Thread aThread = null;

    volatile protected bool cont = true;

    delegate void SetImageCallback ( Bitmap newImage );

    protected void SetImage ( Bitmap newImage )
    {
      if ( pictureBox1.InvokeRequired )
      {
        SetImageCallback si = new SetImageCallback( SetImage );
        BeginInvoke( si, new object[] { newImage } );
      }
      else
        if ( checkShow.Checked )
        {
          pictureBox1.Image = newImage;
          pictureBox1.Invalidate();
        }
    }

    delegate void SetTextCallback ( string text );

    protected void SetText ( string text )
    {
      if ( labelElapsed.InvokeRequired )
      {
        SetTextCallback st = new SetTextCallback( SetText );
        BeginInvoke( st, new object[] { text } );
      }
      else
        labelElapsed.Text = text;
    }

    delegate void EndAnimationCallback ();

    protected void EndAnimation ()
    {
      if ( buttonStart.InvokeRequired )
      {
        EndAnimationCallback ea = new EndAnimationCallback( EndAnimation );
        BeginInvoke( ea );
      }
      else
      {
        buttonStart.Enabled = true;
        buttonStop.Enabled = false;
        aThread = null;
      }
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

      for ( int i = 0; i < maxFrame; i++ )
      {
        if ( !cont ) return;

        Bitmap newImage = anim.RenderFrame( width, height, i, maxFrame );
        SetImage( (Bitmap)newImage.Clone() );
        SetText( String.Format( "Frame: {0}", i ) );
        string fileName = String.Format( "out{0:0000}.png", i );
        newImage.Save( fileName, System.Drawing.Imaging.ImageFormat.Png );
      }

      EndAnimation();
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
