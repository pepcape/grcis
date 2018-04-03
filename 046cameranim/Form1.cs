using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using GuiSupport;
using MathSupport;
using Rendering;
using System.Globalization;
using System.Drawing.Imaging;

namespace _046cameranim
{
  public partial class Form1 : Form
  {
    static readonly string rev = "$Rev$".Split( ' ' )[ 1 ];

    /// <summary>
    /// Output raster image.
    /// </summary>
    protected Bitmap outputImage = null;

    /// <summary>
    /// Main animation-rendering thread.
    /// </summary>
    protected Thread aThread = null;

    /// <summary>
    /// Progress info / user break handling.
    /// Used also as input lock for MT animation computation.
    /// </summary>
    protected Progress progress = new Progress();

    /// <summary>
    /// Global prototype of a scene.
    /// Working threads should clone it before setting specific times to it.
    /// </summary>
    protected IRayScene scene = null;

    /// <summary>
    /// Image width in pixels, 0 for default value (according to panel size).
    /// </summary>
    protected int ImageWidth = 640;

    /// <summary>
    /// Image height in pixels, 0 for default value (according to panel size).
    /// </summary>
    protected int ImageHeight = 480;

    private void setImage ( ref Bitmap bakImage, Bitmap newImage )
    {
      pictureBox1.Image = newImage;
      if ( bakImage != null )
        bakImage.Dispose();
      bakImage = newImage;
    }

    /// <summary>
    /// Redraws the whole image.
    /// </summary>
    private void RenderImage ()
    {
      Cursor.Current = Cursors.WaitCursor;

      buttonRender.Enabled = false;
      buttonRenderAnim.Enabled = false;
      buttonRes.Enabled = false;

      width = ImageWidth;
      if ( width <= 0 ) width = panel1.Width;
      height = ImageHeight;
      if ( height <= 0 ) height = panel1.Height;
      superSampling = (int)numericSupersampling.Value;
      Bitmap newImage = new Bitmap( width, height, PixelFormat.Format24bppRgb );

      if ( scene == null )
        scene = FormSupport.getScene();                 // scene prototype

      IImageFunction imf = FormSupport.getImageFunction( scene );
      imf.Width  = width;
      imf.Height = height;

      IRenderer rend = FormSupport.getRenderer( imf, superSampling );
      rend.Width  = width;
      rend.Height = height;
      rend.Adaptive = 0;
      rend.ProgressData = progress;
      progress.Continue = true;

      // animation:
      ITimeDependent sc = scene as ITimeDependent;
      if ( sc != null )
        sc.Time = (double)numTime.Value;
      MT.InitThreadData();

      Stopwatch sw = new Stopwatch();
      sw.Start();

      rend.RenderRectangle( newImage, 0, 0, width, height );

      sw.Stop();
      labelElapsed.Text = string.Format( CultureInfo.InvariantCulture, "Elapsed: {0:f1}s",
                                         1.0e-3 * sw.ElapsedMilliseconds );

      setImage( ref outputImage, newImage );

      buttonRender.Enabled = true;
      buttonRenderAnim.Enabled = true;
      buttonRes.Enabled = true;

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
        setImage( ref outputImage, newImage );
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
        buttonRes.Enabled = true;
        buttonStop.Enabled = false;
      }
    }

    public Form1 ()
    {
      InitializeComponent();
      Text += " (rev: " + rev + ')';
      buttonRes.Text = FormResolution.GetLabel( ref ImageWidth, ref ImageHeight );
    }

    private void buttonRes_Click ( object sender, EventArgs e )
    {
      FormResolution form = new FormResolution( ImageWidth, ImageHeight );
      if ( form.ShowDialog() == DialogResult.OK )
      {
        ImageWidth = form.ImageWidth;
        ImageHeight = form.ImageHeight;
        buttonRes.Text = FormResolution.GetLabel( ref ImageWidth, ref ImageHeight );
      }
    }

    private void buttonRender_Click ( object sender, EventArgs e )
    {
      RenderImage();
    }

    private void buttonStop_Click ( object sender, EventArgs e )
    {
      StopAnimation();
    }

    private void Form1_FormClosing ( object sender, FormClosingEventArgs e )
    {
      StopAnimation();
    }

    //============================================================
    //===      Animation rendering using multiple threads      ===
    //============================================================

    //============================================================
    //   Constant data:

    /// <summary>
    /// Frame width in pixels.
    /// </summary>
    protected int width;

    /// <summary>
    /// Frame height in pixels.
    /// </summary>
    protected int height;

    /// <summary>
    /// Time of the last frame.
    /// </summary>
    protected double end;

    /// <summary>
    /// Time delta.
    /// </summary>
    protected double dt;

    /// <summary>
    /// Supersampling factor.
    /// </summary>
    protected int superSampling;

    //============================================================
    //   Variable data ("progress" is used as "input data lock"):

    /// <summary>
    /// Frame number to compute.
    /// </summary>
    protected volatile int frameNumber;

    /// <summary>
    /// Frame time to compute.
    /// </summary>
    protected double time;

    /// <summary>
    /// One computed animation frame.
    /// </summary>
    public class Result
    {
      public Bitmap image;
      public int frameNumber;
    }

    /// <summary>
    /// Semaphore guarding the output queue.
    /// </summary>
    protected Semaphore sem = null;

    /// <summary>
    /// Output queue.
    /// </summary>
    protected Queue<Result> queue = null;

    protected void initQueue ()
    {
      if ( queue == null )
        queue = new Queue<Result>();
      else
      {
        while ( queue.Count > 0 )
        {
          Result r = queue.Dequeue();
          r.image.Dispose();
        }
      }
    }

    /// <summary>
    /// Animation rendering prolog: prepare all the global (uniform) values, start the main thread.
    /// </summary>
    private void buttonRenderAnim_Click ( object sender, EventArgs e )
    {
      if ( aThread != null )
        return;

      buttonRenderAnim.Enabled = false;
      buttonRender.Enabled = false;
      buttonRes.Enabled = false;
      buttonStop.Enabled = true;
      lock ( progress )
      {
        progress.Continue = true;
      }

      // Global animation properties (it's safe to access GUI components here):
      time = (double)numFrom.Value;
      end = (double)numTo.Value;
      if ( end <= time )
        end = time + 1.0;
      double fps = (double)numFps.Value;
      dt = (fps > 0.0) ? 1.0 / fps : 25.0;
      end += 0.5 * dt;
      frameNumber = 0;

      width = ImageWidth;
      if ( width <= 0 ) width = panel1.Width;
      height = ImageHeight;
      if ( height <= 0 ) height = panel1.Height;
      superSampling = (int)numericSupersampling.Value;

      if ( scene == null )
        scene = FormSupport.getScene();                 // scene prototype

      // Start main rendering thread:
      aThread = new Thread( new ThreadStart( this.RenderAnimation ) );
      aThread.Start();
    }

    /// <summary>
    /// Main animation rendering thread.
    /// Initializes worker threads and collects the results.
    /// </summary>
    protected void RenderAnimation ()
    {
      Cursor.Current = Cursors.WaitCursor;

      int threads = Environment.ProcessorCount;
      initQueue();
      sem = new Semaphore( 0, 10 * threads );

      // pool of working threads:
      Thread[] pool = new Thread[ threads ];
      int t;
      for ( t = 0; t < threads; t++ )
      {
        pool[ t ] = new Thread( new ThreadStart( this.RenderWorker ) );
        pool[ t ].Start();
      }

      // loop for collection of computed frames:
      int frames = 0;
      int lastDisplayedFrame = -1;
      const long DISPLAY_GAP = 10000L;
      long lastDisplayedTime = -DISPLAY_GAP;
      Stopwatch sw = new Stopwatch();
      sw.Start();

      while ( true )
      {
        sem.WaitOne();                      // wait until a frame is finished

        lock ( progress )                   // regular finish, escape, user break?
        {
          if ( !progress.Continue ||
               time >= end &&
               frames >= frameNumber )
            break;
        }

        // there could be a frame to process:
        Result r;
        lock ( queue )
        {
          if ( queue.Count == 0 )
            continue;
          r = queue.Dequeue();
        }

        // GUI progress indication:
        double seconds = 1.0e-3 * sw.ElapsedMilliseconds;
        double fps = ++frames / seconds;
        SetText( string.Format( CultureInfo.InvariantCulture, "Frames (mt{0}): {1}  ({2:f0} s, {3:f2} fps)",
                                threads, frames, seconds, fps ) );
        if ( r.frameNumber > lastDisplayedFrame &&
             sw.ElapsedMilliseconds > lastDisplayedTime + DISPLAY_GAP )
        {
          lastDisplayedFrame = r.frameNumber;
          lastDisplayedTime = sw.ElapsedMilliseconds;
          SetImage( (Bitmap)r.image.Clone() );
        }

        // save the image file:
        string fileName = string.Format( "out{0:0000}.png", r.frameNumber );
        r.image.Save( fileName, System.Drawing.Imaging.ImageFormat.Png );
        r.image.Dispose();
      }

      for ( t = 0; t < threads; t++ )
      {
        pool[ t ].Join();
        pool[ t ] = null;
      }

      Cursor.Current = Cursors.Default;

      StopAnimation();
    }

    /// <summary>
    /// Worker thread (picks up individual frames and renders them one by one).
    /// </summary>
    protected void RenderWorker ()
    {
      // thread-specific data:
      ITimeDependent mySceneTD = scene as ITimeDependent;
      IRayScene myScene = (mySceneTD == null) ? scene : (IRayScene)mySceneTD.Clone();
      mySceneTD = myScene as ITimeDependent;
      MT.InitThreadData();

      IImageFunction imf = FormSupport.getImageFunction( myScene );
      imf.Width = width;
      imf.Height = height;

      IRenderer rend = FormSupport.getRenderer( imf, superSampling );
      rend.Width = width;
      rend.Height = height;
      rend.Adaptive = 0;                    // turn off adaptive bitmap synthesis completely (interactive preview not needed)
      rend.ProgressData = progress;

      // worker loop:
      while ( true )
      {
        double myTime;
        int myFrameNumber;

        lock ( progress )
        {
          if ( !progress.Continue ||
               time > end )
          {
            sem.Release();                  // chance for the main animation thread to give up as well..
            return;
          }

          // got a frame to compute:
          myTime = time;
          time += dt;
          myFrameNumber = frameNumber++;
        }

        // set up the new result record:
        Result r = new Result();
        r.image = new Bitmap( width, height, System.Drawing.Imaging.PixelFormat.Format24bppRgb );
        r.frameNumber = myFrameNumber;

        // set specific time to my scene:
        if ( mySceneTD != null )
          mySceneTD.Time = myTime;

        // render the whole frame:
        rend.RenderRectangle( r.image, 0, 0, width, height );

        // ... and put the result into the output queue:
        lock ( queue )
        {
          queue.Enqueue( r );
        }
        sem.Release();                      // notify the main animation thread
      }
    }
  }
}
