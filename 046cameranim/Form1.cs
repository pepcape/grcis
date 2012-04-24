using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using MathSupport;
using Rendering;

namespace _046cameranim
{
  public partial class Form1 : Form
  {
    /// <summary>
    /// Output raster image.
    /// </summary>
    protected Bitmap outputImage = null;

    /// <summary>
    /// Scene to be rendered.
    /// </summary>
    protected IRayScene scene = null;

    /// <summary>
    /// Ray-based renderer in form of image function.
    /// </summary>
    protected IImageFunction imf = null;

    /// <summary>
    /// Image synthesizer used to compute raster images.
    /// </summary>
    protected IRenderer rend = null;

    /// <summary>
    /// Rendering thread.
    /// </summary>
    protected Thread aThread = null;

    /// <summary>
    /// Progress info / user break handling.
    /// </summary>
    protected Progress progress = new Progress();

    /// <summary>
    /// Redraws the whole image.
    /// </summary>
    private void RenderImage ()
    {
      Cursor.Current = Cursors.WaitCursor;
      buttonRender.Enabled = false;
      buttonRenderAnim.Enabled = false;

      int width   = panel1.Width;
      int height  = panel1.Height;
      outputImage = new Bitmap( width, height, System.Drawing.Imaging.PixelFormat.Format24bppRgb );

      if ( imf == null )
        imf = getImageFunction();
      imf.Width  = width;
      imf.Height = height;

      if ( rend == null )
        rend = getRenderer( imf );
      rend.Width  = width;
      rend.Height = height;
      rend.Adaptive = 0;
      rend.ProgressData = progress;
      progress.Continue = true;

      if ( rnd == null )
        rnd = new RandomJames();

      // animation:
      ITimeDependent sc = scene as ITimeDependent;
      if ( sc != null )
        sc.Time = (double)numTime.Value;

      Stopwatch sw = new Stopwatch();
      sw.Start();

      rend.RenderRectangle( outputImage, 0, 0, width, height, rnd );

      sw.Stop();
      labelElapsed.Text = String.Format( "Elapsed: {0:f1}s", 1.0e-3 * sw.ElapsedMilliseconds );

      pictureBox1.Image = outputImage;

      buttonRender.Enabled = true;
      buttonRenderAnim.Enabled = true;
      Cursor.Current = Cursors.Default;
    }

    delegate void SetImageCallback ( Bitmap newImage );

    protected void SetImage ( Bitmap newImage )
    {
      if ( pictureBox1.InvokeRequired )
      {
        SetImageCallback si = new SetImageCallback( SetImage );
        BeginInvoke( si, new object[] { newImage } );
      }
      else
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

    delegate void StopAnimationCallback ();

    protected void StopAnimation ()
    {
      if ( aThread == null ) return;

      if ( buttonRenderAnim.InvokeRequired )
      {
        StopAnimationCallback ea = new StopAnimationCallback( StopAnimation );
        BeginInvoke( ea );
      }
      else
      {
        // actually stop the animation:
        lock ( progress )
        {
          progress.Continue = false;
        }
        aThread.Join();
        aThread = null;

        // GUI stuff:
        buttonRenderAnim.Enabled = true;
        buttonRender.Enabled = true;
        buttonStop.Enabled = false;
      }
    }

    public Form1 ()
    {
      InitializeComponent();
      String []tok = "$Rev$".Split( new char[] { ' ' } );
      Text += " (rev: " + tok[1] + ')';
    }

    private void buttonRender_Click ( object sender, EventArgs e )
    {
      RenderImage();
    }

    private void buttonRenderAnim_Click ( object sender, EventArgs e )
    {
      if ( aThread != null ) return;

      buttonRenderAnim.Enabled = false;
      buttonRender.Enabled = false;
      buttonStop.Enabled = true;
      lock ( progress )
      {
        progress.Continue = true;
      }

      aThread = new Thread( new ThreadStart( this.RenderAnimation ) );
      aThread.Start();
    }

    private void buttonStop_Click ( object sender, EventArgs e )
    {
      StopAnimation();
    }

    private static RandomJames rnd = null;

    private Bitmap RenderFrame ( int width, int height, double time )
    {
      Bitmap result = new Bitmap( width, height, System.Drawing.Imaging.PixelFormat.Format24bppRgb );

      if ( imf == null )
        imf = getImageFunction();
      imf.Width = width;
      imf.Height = height;

      if ( rend == null )
        rend = getRenderer( imf );
      rend.Width = width;
      rend.Height = height;
      rend.Adaptive = 0;        // turn off adaptive bitmap synthesis completely (interactive preview not needed)
      rend.ProgressData = progress;

      if ( rnd == null )
        rnd = new RandomJames();

      // animation:
      ITimeDependent sc = scene as ITimeDependent;
      if ( sc != null )
        sc.Time = time;

      rend.RenderRectangle( result, 0, 0, width, height, rnd );

      return result;
    }

    public void RenderAnimation ()
    {
      double time = (double)numFrom.Value;
      double end  = (double)numTo.Value;
      if ( end <= time ) end = time + 1.0;
      double fps = (double)numFps.Value;
      double dt = (fps > 0.0) ? 1.0 / fps : 25.0;
      int width = panel1.Width;
      int height = panel1.Height;

      Cursor.Current = Cursors.WaitCursor;

      Stopwatch sw = new Stopwatch();

      for ( int fr = 0; time < end; fr++, time += dt )
      {
        lock ( progress )
        {
          if ( !progress.Continue ) break;
        }
        sw.Start();
        Bitmap newImage = RenderFrame( width, height, time );
        SetImage( (Bitmap)newImage.Clone() );
        sw.Stop();

        SetText( String.Format( "Frame: {0}, elapsed: {1:f1}s", fr, 1.0e-3 * sw.ElapsedMilliseconds ) );
        string fileName = String.Format( "out{0:0000}.png", fr );
        newImage.Save( fileName, System.Drawing.Imaging.ImageFormat.Png );
      }

      Cursor.Current = Cursors.Default;

      StopAnimation();
    }

    private void Form1_FormClosing ( object sender, FormClosingEventArgs e )
    {
      StopAnimation();
    }
  }
}
