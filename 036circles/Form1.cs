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

namespace _036circles
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
      if ( labelImageName.InvokeRequired )
      {
        SetTextCallback st = new SetTextCallback( SetText );
        BeginInvoke( st, new object[] { text } );
      }
      else
        labelImageName.Text = text;
    }

    delegate void StopAnimationCallback ();

    protected void StopAnimation ()
    {
      if ( aThread == null ) return;

      if ( buttonRedraw.InvokeRequired )
      {
        StopAnimationCallback ea = new StopAnimationCallback( StopAnimation );
        BeginInvoke( ea );
      }
      else
      {
        // actually stop the animation:
        cont = false;
        aThread.Join();
        aThread = null;

        // GUI stuff:
        buttonRedraw.Enabled = true;
        buttonLoadImage.Enabled = false;
      }
    }

    public Form1 ()
    {
      InitializeComponent();
    }

    public void RenderAnimation ()
    {
      int maxFrame = (int)numericParam.Value;
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

      StopAnimation();
    }

    private void buttonRedraw_Click ( object sender, EventArgs e )
    {
      if ( aThread != null ) return;

      buttonRedraw.Enabled = false;
      buttonLoadImage.Enabled  = true;
      cont = true;

      aThread = new Thread( new ThreadStart( this.RenderAnimation ) );
      aThread.Start();
    }

    private void buttonLoadImage_Click ( object sender, EventArgs e )
    {
      StopAnimation();
    }
  }
}
