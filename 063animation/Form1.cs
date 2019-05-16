using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using MathSupport;
using Rendering;
using System.Globalization;
using GuiSupport;
using Utilities;

namespace _063animation
{
  public partial class Form1 : Form
  {
    static readonly string rev = Util.SetVersion( "$Rev$" );

    public static Form1 singleton = null;

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
    public int ImageWidth = 640;

    /// <summary>
    /// Image height in pixels, 0 for default value (according to panel size).
    /// </summary>
    public int ImageHeight = 480;

    private void SetGui (bool render)
    {
      IImageFunction imf = FormSupport.getImageFunction( textParam.Text, data );
      bool canAnimate = (imf != null) &&
                        (imf is ITimeDependent);

      buttonRenderAnim.Enabled = render && canAnimate;
      buttonRender.Enabled     = render;
      buttonRes.Enabled        = render;
      buttonStop.Enabled       = !render;

      numTime.Enabled          = canAnimate;
      numFrom.Enabled          = canAnimate;
      numTo.Enabled            = canAnimate;
      numFps.Enabled           = canAnimate;
      label1.Enabled           = canAnimate;
      label2.Enabled           = canAnimate;
      label3.Enabled           = canAnimate;
      label4.Enabled           = canAnimate;
    }

    /// <summary>
    /// Redraws the whole image.
    /// </summary>
    private void RenderImage ()
    {
      Cursor.Current = Cursors.WaitCursor;

      SetGui( false );

      width = ImageWidth;
      if ( width <= 0 ) width = panel1.Width;
      height = ImageHeight;
      if ( height <= 0 ) height = panel1.Height;
      superSampling = (int)numericSupersampling.Value;
      Bitmap im = new Bitmap( width, height, System.Drawing.Imaging.PixelFormat.Format24bppRgb );
      MT.InitThreadData();

      if ( data == null )
        data = FormSupport.getData( textParam.Text );                 // animation data

      IImageFunction imf = FormSupport.getImageFunction( textParam.Text, data );
      imf.Width  = width;
      imf.Height = height;

      IRenderer rend = FormSupport.getRenderer( textParam.Text, imf );
      rend.Width  = width;
      rend.Height = height;
      rend.Adaptive = 0;
      rend.ProgressData = progress;
      progress.Continue = true;

      // animation:
      ITimeDependent imftd = imf as ITimeDependent;
      if ( imftd != null )
        imftd.Time = (double)numTime.Value;

      Stopwatch sw = new Stopwatch();
      sw.Start();

      rend.RenderRectangle( im, 0, 0, width, height );

      sw.Stop();
      labelElapsed.Text = string.Format( CultureInfo.InvariantCulture, "Elapsed: {0:f1}s", 1.0e-3 * sw.ElapsedMilliseconds );

      SetImage( (Bitmap)im.Clone() );

      string fileName = Util.FileNameString( textParam.Text ) + ".png";
      im.Save( fileName, System.Drawing.Imaging.ImageFormat.Png );
      im.Dispose();

      SetGui( true );

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
        SetGui( true );
      }
    }

    public Form1 ()
    {
      singleton = this;
      InitializeComponent();

      // Init rendering params:
      string name;
      FormSupport.InitializeParams( out name );
      Text += " (rev: " + rev + ") '" + name + '\'';

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

    private void Form1_Load ( object sender, EventArgs e )
    {
      SetGui( true );
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
    public int superSampling;

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
    /// Output file-name prefix.
    /// </summary>
    protected string prefix = "out";

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

      SetGui( false );
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

      // param string:
      Dictionary<string, string> p = Util.ParseKeyValueList( textParam.Text );
      if ( p.Count > 0 )
      {
        // output file-name prefix:
        if ( !p.TryGetValue( "prefix", out prefix ) )
          prefix = "out";
      }

      if ( data == null )
        data = FormSupport.getData( textParam.Text );               // animation data

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
      IImageFunction imf = FormSupport.getImageFunction( textParam.Text, data );
      if ( imf == null ||
          !(imf is ITimeDependent) )
        return;

      Cursor.Current = Cursors.WaitCursor;

      int threads = Environment.ProcessorCount;
      initQueue();
      sem = new Semaphore( 0, 10 * threads );

      // pool of working threads:
      Thread[] pool = new Thread[ threads ];
      int t;
      for ( t = 0; t < threads; t++ )
      {
        pool[ t ] = new Thread( new ThreadStart( RenderWorker ) );
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
        string fileName = string.Format( "{0}{1:0000}.png", prefix, r.frameNumber );
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
      ITimeDependent datatd = data as ITimeDependent;
      object myData = (datatd == null) ? data : datatd.Clone();

      MT.InitThreadData();

      IImageFunction imf = FormSupport.getImageFunction( textParam.Text, myData );
      imf.Width  = width;
      imf.Height = height;
      ITimeDependent imftd = imf as ITimeDependent;

      IRenderer rend = FormSupport.getRenderer( textParam.Text, imf );
      rend.Width  = width;
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

        // set specific time to my image function:
        if ( imftd != null )
          imftd.Time = myTime;

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
