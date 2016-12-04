using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;
using GuiSupport;

namespace _075animation
{
  public partial class Form1 : Form
  {
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
    /// Global animation data (ITimeDependent or constant).
    /// Working threads should clone it before setting specific times to it.
    /// </summary>
    protected object data = null;

    /// <summary>
    /// Image width in pixels, 0 for default value (according to panel size).
    /// </summary>
    protected int ImageWidth = 640;

    /// <summary>
    /// Image height in pixels, 0 for default value (according to panel size).
    /// </summary>
    protected int ImageHeight = 480;

    private void EnableGUI ( bool compute )
    {
      buttonRenderAnim.Enabled =
      buttonRender.Enabled     =
      numFrom.Enabled          =
      numTo.Enabled            =
      numFps.Enabled           =
      numTime.Enabled          =
      buttonRes.Enabled        = compute;
      buttonStop.Enabled       = !compute;
    }

    /// <summary>
    /// Redraws the whole image.
    /// </summary>
    private void RenderImage ()
    {
      Cursor.Current = Cursors.WaitCursor;

      EnableGUI( false );

      width = ImageWidth;
      if ( width <= 0 ) width = panel1.Width;
      height = ImageHeight;
      if ( height <= 0 ) height = panel1.Height;
      Animation.InitAnimation( width, height );

      Stopwatch sw = new Stopwatch();
      sw.Start();

      Canvas c = new Canvas( width, height );

      Animation.DrawFrame( c, (double)numTime.Value, (double)numFrom.Value, (double)numTo.Value );

      if ( outputImage != null )
        outputImage.Dispose();
      outputImage = c.Finish();
      sw.Stop();

      labelElapsed.Text = string.Format( CultureInfo.InvariantCulture, "Elapsed: {0:f1}s", 1.0e-3 * sw.ElapsedMilliseconds );

      pictureBox1.Image = outputImage;

      EnableGUI( true );

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
        if ( pictureBox1.Image != null )
        {
          pictureBox1.Image.Dispose();
          pictureBox1.Image = null;
        }
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

        // Dispose unwritten queued results:
        initQueue( 0 );
        semQueue = null;
        semResults = null;

        // GUI stuff:
        EnableGUI( true );
      }
    }

    public Form1 ()
    {
      InitializeComponent();
      string []tok = "$Rev$".Split( ' ' );
      Text += " (rev: " + tok[1] + ')';

      // Init rendering params:
      double from, to, fps;
      Animation.InitParams( out ImageWidth, out ImageHeight, out from, out to, out fps );
      numFrom.Value = (decimal)from;
      numTo.Value   = (decimal)to;
      numFps.Value  = (decimal)fps;

      buttonRes.Text = FormResolution.GetLabel( ref ImageWidth, ref ImageHeight );
    }

    private void buttonRes_Click ( object sender, EventArgs e )
    {
      FormResolution form = new FormResolution( ImageWidth, ImageHeight );
      if ( form.ShowDialog() == DialogResult.OK )
      {
        ImageWidth  = form.ImageWidth;
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
    /// Time of the first frame.
    /// </summary>
    protected double start;

    /// <summary>
    /// Time of the last frame.
    /// </summary>
    protected double end;

    /// <summary>
    /// Time delta.
    /// </summary>
    protected double dt;

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
    /// Total number of frames.
    /// </summary>
    protected int totalFrames;

    /// <summary>
    /// One computed animation frame.
    /// </summary>
    public class Result : IDisposable
    {
      public Bitmap image;
      public int frameNumber;

      public void Dispose ()
      {
        if ( image != null )
        {
          image.Dispose();
          image = null;
        }
      }
    }

    /// <summary>
    /// Semaphore guarding the output queue.
    /// Signaled if there are results ready..
    /// </summary>
    protected Semaphore semResults = null;

    /// <summary>
    /// Semaphore for the maximum queue capacity.
    /// Signaled if there is empty space in the queue.
    /// </summary>
    protected Semaphore semQueue = null;

    /// <summary>
    /// Output queue.
    /// </summary>
    protected Queue<Result> queue = null;

    /// <summary>
    /// Maximum allowed queue size.
    /// </summary>
    protected int queueSize = 1;

    protected void initQueue ( int initSize )
    {
      if ( queue == null )
        queue = new Queue<Result>( initSize );
      else
      {
        while ( queue.Count > 0 )
          queue.Dequeue().Dispose();
      }
    }

    /// <summary>
    /// Animation rendering prolog: prepare all the global (uniform) values, start the main thread.
    /// </summary>
    private void buttonRenderAnim_Click ( object sender, EventArgs e )
    {
      if ( aThread != null )
        return;

      EnableGUI( false );
      lock ( progress )
      {
        progress.Continue = true;
      }

      // Global animation properties (it's safe to access GUI components here):
      start = time = (double)numFrom.Value;
      end = (double)numTo.Value;
      if ( end <= time )
        end = time + 1.0;
      double fps = (double)numFps.Value;
      dt = (fps > 0.0) ? 1.0 / fps : 25.0;
      end += 0.5 * dt;
      frameNumber = 0;
      totalFrames = (int)( (end - time) / dt );

      width = ImageWidth;
      if ( width <= 0 ) width = panel1.Width;
      height = ImageHeight;
      if ( height <= 0 ) height = panel1.Height;
      Animation.InitAnimation( width, height );

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
      queueSize = threads + 2;                                // intended queue capacity
      initQueue( queueSize );                                 // queue is prepared for the capacity
      semResults = new Semaphore( 0, 2 * queueSize );         // no results are ready
      semQueue   = new Semaphore( queueSize, 2 * queueSize ); // the whole queue capacity is ready
      Stopwatch sw = new Stopwatch();
      sw.Start();

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

      while ( true )
      {
        semResults.WaitOne();               // wait until a frame is finished

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
          semQueue.Release();
        }

        // GUI progress indication:
        SetText( string.Format( CultureInfo.InvariantCulture, "Frames (mt{0}): {1}  ({2:f1}s)",
                                threads, ++frames, 1.0e-3 * sw.ElapsedMilliseconds ) );
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
        r.Dispose();
      }

      // letting all the workers finish their work:
      semQueue.Release( threads );
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
      Canvas c = new Canvas( width, height );

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
            semResults.Release();                  // chance for the main animation thread to give up as well..
            return;
          }

          // got a frame to compute:
          myTime = time;
          time += dt;
          myFrameNumber = frameNumber++;
        }

        // set up the new result record:
        Result r = new Result();
        r.frameNumber = myFrameNumber;

        Animation.DrawFrame( c, myTime, start, end );
        r.image = c.Finish();

        // ... and put the result into the output queue:
        semQueue.WaitOne();                        // wait for a space in the result queue
        lock ( queue )
          queue.Enqueue( r );

        lock ( progress )
        {
          semResults.Release();                    // notify the main animation thread
          if ( !progress.Continue )
            return;
        }
      }
    }
  }

  /// <summary>
  /// Data class keeping info about current progress of a computation.
  /// </summary>
  public class Progress
  {
    /// <summary>
    /// Relative amount of work finished so far (0.0f to 1.0f).
    /// </summary>
    public float Finished
    {
      get;
      set;
    }

    /// <summary>
    /// Optional message. Any string.
    /// </summary>
    public string Message
    {
      get;
      set;
    }

    /// <summary>
    /// Continue in an associated computation.
    /// </summary>
    public bool Continue
    {
      get;
      set;
    }

    /// <summary>
    /// Sync interval in milliseconds.
    /// </summary>
    public long SyncInterval
    {
      get;
      set;
    }

    /// <summary>
    /// Any message from computing unit to the GUI main.
    /// </summary>
    public virtual void Sync ( Object msg )
    {
    }

    /// <summary>
    /// Set all the harmless values.
    /// </summary>
    public Progress ()
    {
      Finished = 0.0f;
      Message = "";
      Continue = true;
      SyncInterval = 8000L;
    }
  }
}
