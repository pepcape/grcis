using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using GuiSupport;
using MathSupport;
using Rendering;
using Utilities;

namespace _050rtmesh
{
  public partial class Form1 : Form
  {
    static readonly string rev = Util.SetVersion( "$Rev$" );

    public static Form1 singleton = null;

    /// <summary>
    /// Current output raster image. Locked access.
    /// </summary>
    protected Bitmap outputImage = null;

    /// <summary>
    /// The same order as items in the comboScenes.
    /// </summary>
    public List<InitSceneStrDelegate> sceneInitFunctions = null;

    /// <summary>
    /// Index of the current (selected) scene.
    /// </summary>
    protected volatile int selectedScene = 0;

    /// <summary>
    /// Explicit user-defined OBJ file-name.
    /// </summary>
    protected string objFileName = "";

    /// <summary>
    /// Ray-based renderer in form of image function.
    /// </summary>
    protected IImageFunction imf = null;

    /// <summary>
    /// Image synthesizer used to compute raster images.
    /// </summary>
    protected IRenderer rend = null;

    /// <summary>
    /// Image width in pixels, 0 for default value (according to panel size).
    /// </summary>
    public int ImageWidth = 0;

    /// <summary>
    /// Image height in pixels, 0 for default value (according to panel size).
    /// </summary>
    public int ImageHeight = 0;

    /// <summary>
    /// Global instance of a random generator.
    /// </summary>
    public static RandomJames rnd = new RandomJames();

    /// <summary>
    /// Global stopwatch for rendering thread. Locked access.
    /// </summary>
    protected Stopwatch sw = new Stopwatch();

    /// <summary>
    /// Rendering master thread.
    /// </summary>
    protected Thread aThread = null;

    protected class RenderingProgress : Progress
    {
      protected Form1 f;

      protected long lastSync = 0L;

      public RenderingProgress ( Form1 _f )
      {
        f = _f;
      }

      public void Reset ()
      {
        lastSync = 0L;
      }

      public override void Sync ( Object msg )
      {
        long now = f.sw.ElapsedMilliseconds;
        if ( now - lastSync < SyncInterval )
          return;

        lastSync = now;
        f.SetText( string.Format( CultureInfo.InvariantCulture, "{0:f1}%:  {1:f1}s",
                                  100.0f * Finished, 1.0e-3 * now ) );
        Bitmap b = msg as Bitmap;
        if ( b != null )
        {
          Bitmap nb;
          lock ( b )
            nb = (Bitmap)b.Clone();
          f.SetImage( nb );
        }
      }
    }

    /// <summary>
    /// Progress info / user break handling.
    /// </summary>
    protected RenderingProgress progress = null;

    /// <summary>
    /// Default behavior - create scene selected in the combo-box.
    /// </summary>
    public IRayScene SceneByComboBox ()
    {
      DefaultRayScene sc = new DefaultRayScene();
      long faces = sceneInitFunctions[ selectedScene ]( sc, new string[] { objFileName } );
      SetText( "Faces: " + faces );
      CSGInnerNode.countFaces = faces;
      return sc;
    }

    public ComboBox ComboScene
    {
      get { return comboScene; }
    }

    public NumericUpDown NumericSupersampling
    {
      get { return numericSupersampling; }
    }

    public CheckBox CheckJitter
    {
      get { return checkJitter; }
    }

    public CheckBox CheckMultithreading
    {
      get { return checkMultithreading; }
    }

    private void setImage ( ref Bitmap bakImage, Bitmap newImage )
    {
      pictureBox1.Image = newImage;
      pictureBox1.Invalidate();

      if ( bakImage != null )
        bakImage.Dispose();
      bakImage = newImage;
    }

    /// <summary>
    /// Worker-thread-specific data.
    /// </summary>
    protected class WorkerThreadInit
    {
      /// <summary>
      /// Worker id (for debugging purposes only).
      /// </summary>
      public int id;

      /// <summary>
      /// Worker-selector predicate.
      /// </summary>
      public ThreadSelector sel;

      public Bitmap image;

      public int width;
      public int height;

      public WorkerThreadInit ( Bitmap im, int wid, int hei, int threadNo, int threads )
      {
        image  = im;
        id     = threadNo;
        width  = wid;
        height = hei;
        sel    = ( n ) => (n % threads) == threadNo;
      }
    }

    /// <summary>
    /// Routine of one worker-thread.
    /// Result image and rendering progress are the only two shared objects.
    /// </summary>
    /// <param name="spec">Thread-specific data (worker-thread-selector).</param>
    private void RenderWorker ( Object spec )
    {
      WorkerThreadInit init = spec as WorkerThreadInit;
      if ( init != null )
      {
        MT.InitThreadData();
        rend.RenderRectangle( init.image, 0, 0, init.width, init.height,
                              init.sel );
      }
    }

    /// <summary>
    /// [Re]-renders the whole image (in separate thread).
    /// </summary>
    private void RenderImage ()
    {
      Cursor.Current = Cursors.WaitCursor;

      // determine output image size:
      int width = ImageWidth;
      if ( width <= 0 ) width = panel1.Width;
      int height = ImageHeight;
      if ( height <= 0 ) height = panel1.Height;

      Bitmap newImage = new Bitmap( width, height, PixelFormat.Format24bppRgb );

      if ( imf == null )
      {
        imf = FormSupport.getImageFunction( FormSupport.getScene() );
        rend = null;
      }
      imf.Width  = width;
      imf.Height = height;
      RayTracing rt = imf as RayTracing;
      if ( rt != null )
      {
        rt.DoShadows = checkShadows.Checked;
        rt.DoReflections = checkReflections.Checked;
        rt.DoRefractions = checkRefractions.Checked;
      }

      if ( rend == null )
        rend = FormSupport.getRenderer( imf );
      rend.Width  = width;
      rend.Height = height;
      rend.Adaptive = 8;
      rend.ProgressData = progress;

      SupersamplingImageSynthesizer ss = rend as SupersamplingImageSynthesizer;
      if ( ss != null )
      {
        ss.Supersampling = (int)numericSupersampling.Value;
        ss.Jittering = checkJitter.Checked ? 1.0 : 0.0;
      }
      progress.SyncInterval = ((width * (long)height) > (2L << 20)) ? 30000L : 10000L;
      progress.Reset();
      CSGInnerNode.ResetStatistics();

      lock ( sw )
      {
        sw.Reset();
        sw.Start();
      }

      if ( checkMultithreading.Checked && Environment.ProcessorCount > 1 )
      {
        Thread[] pool = new Thread[ Environment.ProcessorCount ];
        int t;
        for ( t = 0; t < pool.Length; t++ )
          pool[ t ] = new Thread( new ParameterizedThreadStart( this.RenderWorker ) );
        for ( t = pool.Length; --t >= 0; )
          pool[ t ].Start( new WorkerThreadInit( newImage, width, height, t, pool.Length ) );

        for ( t = 0; t < pool.Length; t++ )
        {
          pool[ t ].Join();
          pool[ t ] = null;
        }
      }
      else
      {
        MT.InitThreadData();
        rend.RenderRectangle( newImage, 0, 0, width, height );
      }

      long elapsed;
      lock ( sw )
      {
        sw.Stop();
        elapsed = sw.ElapsedMilliseconds;
      }

      string msg = string.Format( CultureInfo.InvariantCulture,
                                  "{0:f1}s  [ {1}x{2}, f{3:#,#}, mt{4}, r{5:#,#}k, i{6:#,#}k, bb{7:#,#}k, t{8:#,#}k ]",
                                  1.0e-3 * elapsed, width, height, CSGInnerNode.countFaces,
                                  checkMultithreading.Checked ? Environment.ProcessorCount : 1,
                                  (Intersection.countRays + 500L) / 1000L,
                                  (Intersection.countIntersections + 500L) / 1000L,
                                  (CSGInnerNode.countBoundingBoxes + 500L) / 1000L,
                                  (CSGInnerNode.countTriangles + 500L) / 1000L );
      SetText( msg );
      Console.WriteLine( "Rendering finished: " + msg );
      SetImage( newImage );

      Cursor.Current = Cursors.Default;

      StopRendering();
    }

    void SetGUI ( bool enable )
    {
      numericSupersampling.Enabled =
      checkJitter.Enabled =
      checkShadows.Enabled =
      checkReflections.Enabled =
      checkReflections.Enabled =
      checkRefractions.Enabled =
      checkMultithreading.Enabled =
      buttonRender.Enabled =
      comboScene.Enabled =
      buttonRes.Enabled =
      buttonSave.Enabled = enable;
      buttonStop.Enabled = !enable;
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
        setImage( ref outputImage, newImage );
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

    delegate void StopRenderingCallback ();

    protected void StopRendering ()
    {
      if ( aThread == null )
        return;

      if ( buttonRender.InvokeRequired )
      {
        StopRenderingCallback ea = new StopRenderingCallback( StopRendering );
        BeginInvoke( ea );
      }
      else
      {
        // actually stop the rendering:
        lock ( progress )
          progress.Continue = false;
        aThread.Join();
        aThread = null;

        // GUI stuff:
        SetGUI( true );
      }
    }

    /// <summary>
    /// Shoots single primary ray only..
    /// </summary>
    /// <param name="x">X-coordinate inside the raster image.</param>
    /// <param name="y">Y-coordinate inside the raster image.</param>
    private void singleSample ( int x, int y )
    {
      if ( imf == null )
      {
        imf = FormSupport.getImageFunction( FormSupport.getScene() );
        rend = null;
      }

      // determine output image size:
      int width = ImageWidth;
      if ( width <= 0 ) width = panel1.Width;
      int height = ImageHeight;
      if ( height <= 0 ) height = panel1.Height;
      imf.Width = width;
      imf.Height = height;

      RayTracing rt = imf as RayTracing;
      if ( rt != null )
      {
        rt.DoShadows = checkShadows.Checked;
        rt.DoReflections = checkReflections.Checked;
        rt.DoRefractions = checkRefractions.Checked;
      }

      double[] color = new double[ 3 ];
      long hash = imf.GetSample( x + 0.5, y + 0.5, color );
      labelSample.Text = string.Format( CultureInfo.InvariantCulture, "Sample at [{0},{1}] = [{2:f},{3:f},{4:f}], {5:X}",
                                        x, y, color[ 0 ], color[ 1 ], color[ 2 ], hash );
    }

    public Form1 ()
    {
      singleton = this;
      InitializeComponent();
      progress = new RenderingProgress( this );

      // Init scenes etc.
      string name;
      FormSupport.InitializeScenes( out name );
      Text += " (rev: " + rev + ") '" + name + '\'';

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
      if ( aThread != null )
        return;

      SetGUI( false );
      lock ( progress )
        progress.Continue = true;

      SetText( "Wait a moment.." );
      aThread = new Thread( new ThreadStart( this.RenderImage ) );
      aThread.Start();
    }

    private void buttonSave_Click ( object sender, EventArgs e )
    {
      if ( outputImage == null ||
           aThread != null ) return;

      SaveFileDialog sfd = new SaveFileDialog();
      sfd.Title = "Save PNG file";
      sfd.Filter = "PNG Files|*.png";
      sfd.AddExtension = true;
      sfd.FileName = "";
      if ( sfd.ShowDialog() != DialogResult.OK )
        return;

      outputImage.Save( sfd.FileName, ImageFormat.Png );
    }

    private void buttonObj_Click ( object sender, EventArgs e )
    {
      if ( aThread != null ) return;

      imf = null;
      rend = null;
      OpenFileDialog ofd = new OpenFileDialog();

      ofd.Title = "Open Scene File";
      ofd.Filter = "Wavefront OBJ Files|*.obj;*.obj.gz" +
                   "|All scene types|*.obj";

      ofd.FilterIndex = 1;
      ofd.FileName = "";
      if ( ofd.ShowDialog() != DialogResult.OK )
      {
        buttonObj.Text = "Default OBJ";
        objFileName = "";
        return;
      }

      objFileName = ofd.FileName;
      buttonObj.Text = Path.GetFileName( objFileName );
    }

    private void buttonStop_Click ( object sender, EventArgs e )
    {
      StopRendering();
    }

    private void comboScene_SelectedIndexChanged ( object sender, EventArgs e )
    {
      selectedScene = comboScene.SelectedIndex;
      imf = null;
      rend = null;
    }

    private void pictureBox1_MouseDown ( object sender, MouseEventArgs e )
    {
      if ( aThread == null && e.Button == MouseButtons.Left )
        singleSample( e.X, e.Y );
    }

    private void pictureBox1_MouseMove ( object sender, MouseEventArgs e )
    {
      if ( aThread == null && e.Button == MouseButtons.Left )
        singleSample( e.X, e.Y );
    }

    private void Form1_FormClosing ( object sender, FormClosingEventArgs e )
    {
      StopRendering();
    }
  }
}
