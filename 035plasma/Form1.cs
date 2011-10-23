using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using Scene3D;

namespace _035plasma
{
  public partial class Form1 : Form
  {
    /// <summary>
    /// Global thread variable. Valid while the simulation is in progress..
    /// </summary>
    protected Thread aThread = null;

    /// <summary>
    /// Global simulation object. Re-entrant simulation of one frame.
    /// </summary>
    protected Simulation sim = null;

    /// <summary>
    /// Break-variable. Has to be checked on regular basis.
    /// </summary>
    volatile protected bool cont = true;

    protected FpsMeter fps = new FpsMeter();

    delegate void SetImageCallback ( Bitmap newImage );

    protected void SetImage ( Bitmap newImage )
    {
      if ( pictureBox1.InvokeRequired )
      {
        SetImageCallback si = new SetImageCallback( SetImage );
        BeginInvoke( si, new object[] { newImage } );
      }
      else
        if ( true )
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

    delegate void EndSimulationCallback ();

    protected void EndSimulation ()
    {
      if ( buttonStart.InvokeRequired )
      {
        EndSimulationCallback ea = new EndSimulationCallback( EndSimulation );
        BeginInvoke( ea );
      }
      else
      {
        buttonStart.Enabled = true;
        buttonReset.Enabled = true;
        buttonStop.Enabled = false;
        aThread = null;
      }
    }

    public Form1 ()
    {
      InitializeComponent();
    }

    public void RenderSimulation ()
    {
      int width  = (int)numericXres.Value;
      int height = (int)numericYres.Value;

      if ( sim == null || sim.Width != width || sim.Height != height )
        sim = new Simulation( width, height );

      fps.Start();
      float fp = 0.0f;

      while ( cont )
      {
        Bitmap newImage = sim.Simulate();
        SetImage( newImage );

        float newFp = fps.Frame();
        if ( sim.Frame % 32 == 0 ) fp = newFp;
        SetText( String.Format( "Frame: {0} (FPS = {1:0.0})", sim.Frame, fp ) );

        //string fileName = String.Format( "out{0:0000}.png", i );
        //newImage.Save( fileName, System.Drawing.Imaging.ImageFormat.Png );
      }

      fps.Stop();
    }

    private void buttonStart_Click ( object sender, EventArgs e )
    {
      if ( aThread != null ) return;

      buttonStart.Enabled = false;
      buttonReset.Enabled = false;
      buttonStop.Enabled  = true;
      cont = true;

      aThread = new Thread( new ThreadStart( this.RenderSimulation ) );
      aThread.Start();
    }

    private void buttonStop_Click ( object sender, EventArgs e )
    {
      if ( aThread == null ) return;

      cont = false;
      aThread.Join();

      EndSimulation();
    }

    private void buttonReset_Click ( object sender, EventArgs e )
    {
      if ( sim == null || aThread != null )
        return;

      sim.Reset();
      SetText( "Frame: 0" );
    }
  }
}
