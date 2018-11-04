using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;
using GuiSupport;
using MathSupport;
using Utilities;

namespace Rendering
{
  public partial class Form1 : Form, IRenderProgressForm
  {
    static readonly string rev = Util.SetVersion ( "$Rev$" );

    public static Form1 singleton = null;

    /// <summary>
    /// Current output raster image. Locked access.
    /// </summary>
    internal Bitmap outputImage = null;

    /// <summary>
    /// Scenes for the listbox: sceneName -> {sceneDelegate | scriptFileName}
    /// </summary>
    public Dictionary<string, object> sceneRepository = null;

    public Scripts MyScenes;

    /// <summary>
    /// Index of the current (selected) scene.
    /// </summary>
    protected volatile int selectedScene = 0;

    /// <summary>
    /// If positive, new scene & image-function & renderer has to be created..
    /// </summary>
    protected bool dirty = true;

    /// <summary>
    /// Ray-based renderer in form of image function.
    /// For single sample computing only.
    /// </summary>
    protected IImageFunction imfs = null;

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
    public static RandomJames rnd = new RandomJames ();

    /// <summary>
    /// Global stopwatch for rendering thread. Locked access.
    /// </summary>
    public Stopwatch sw = new Stopwatch ();

    /// <summary>
    /// Rendering master thread.
    /// </summary>
    protected Thread aThread = null;

    /// <summary>
    /// Progress info / user break handling.
    /// </summary>
    protected RenderingProgress progress = null;

    protected string winTitle;

    public Form1 ( string[] args )
    {
      singleton = this;
      InitializeComponent ();
      progress = new RenderingProgress ( this );

      // Init scenes etc.
      string name;
      FormSupport.InitializeScenes ( args, out name );     

      Text += " (" + rev + ") '" + name + '\'';
      winTitle = Text;
      setWindowTitleSuffix ( $" Zoom: {(int) ( zoom * 100 )}%" );

      SetOptions ( args );
      buttonRes.Text = FormResolution.GetLabel ( ref ImageWidth, ref ImageHeight );

      try
      {
        image = Image.FromFile ( "Logo-CGG.png" );
      }
      catch ( Exception )
      {
        // ignored
      }

      if ( AdvancedTools.singleton == null )
        AdvancedTools.singleton = new AdvancedTools ();

      AdvancedTools.singleton.Initialize ();
      AdvancedTools.singleton.SetNewDimensions ( ImageWidth, ImageHeight ); //makes all maps to initialize again
    }

    /// <summary>
    /// Default behavior - create scene selected in the combo-box.
    /// Can handle InitSceneDelegate, InitSceneParamDelegate or CSscript file-name
    /// </summary>
    public IRayScene SceneByComboBox ()
    {
      string sceneName = (string) comboScene.Items [ selectedScene ];

      object definition;
      if ( sceneRepository.TryGetValue ( sceneName, out definition ) )
        return Scripts.SceneFromObject ( sceneName, definition, textParam.Text, str => SetText ( str ) );

      // fallback to a default scene;
      return Scenes.DefaultScene ();
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

    public TextBox TextParam
    {
      get { return textParam; }
    }

    private void setImage ( ref Bitmap bakImage, Bitmap newImage )
    {
      image = newImage;
      zoom = ClampZoom ();
      pictureBox1.Invalidate ();

      bakImage?.Dispose ();

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

      /// <summary>
      /// Animated scene object or null if not animated.
      /// </summary>
      public ITimeDependent scene;

      /// <summary>
      /// Animated image function or null if not animated.
      /// </summary>
      public ITimeDependent imfunc;

      /// <summary>
      /// Actual rendering object.
      /// </summary>
      public IRenderer rend;

      public int width;
      public int height;

      public WorkerThreadInit ( IRenderer r, ITimeDependent sc, ITimeDependent imf, Bitmap im, int wid, int hei,
                                int threadNo, int threads )
      {
        scene = sc;
        imfunc = imf;
        rend = r;
        image = im;
        id = threadNo;
        width = wid;
        height = hei;
        sel = ( n ) => ( n % threads ) == threadNo;
      }
    }


    /// <summary>
    /// Routine of one worker-thread.
    /// Result image and rendering progress are the only two shared objects.
    /// </summary>
    /// <param name="spec">Thread-specific data (worker-thread-selector).</param>
    private void RenderWorker ( object spec )
    {
      WorkerThreadInit init = spec as WorkerThreadInit;
      if ( init != null )
      {
        MT.InitThreadData ();
        init.rend.RenderRectangle ( init.image, 0, 0, init.width, init.height,
                                    init.sel );
      }
    }

    private IImageFunction getImageFunction ( IRayScene sc, int width, int height )
    {
      IImageFunction imf = FormSupport.getImageFunction ( sc, TextParam.Text );
      imf.Width = width;
      imf.Height = height;

      RayTracing rt = imf as RayTracing;
      if ( rt != null )
      {
        rt.DoShadows = checkShadows.Checked;
        rt.DoReflections = checkReflections.Checked;
        rt.DoRefractions = checkRefractions.Checked;
      }

      return imf;
    }

    private IRenderer getRenderer ( IImageFunction imf, int width, int height )
    {
      IRenderer rend = FormSupport.getRenderer ( imf, (int) numericSupersampling.Value, checkJitter.Checked ? 1.0 : 0.0,
                                                 TextParam.Text );
      rend.Width = width;
      rend.Height = height;
      rend.Adaptive = 8;
      rend.ProgressData = progress;

      return rend;
    }

    /// <summary>
    /// [Re]-renders the whole image (in separate thread).
    /// </summary>
    private void RenderImage ()
    {
      Cursor.Current = Cursors.WaitCursor;

      // determine output image size:
      int width = ImageWidth;
      if ( width <= 0 )
        width = panel1.Width;

      int height = ImageHeight;
      if ( height <= 0 )
        height = panel1.Height;

      Bitmap newImage = new Bitmap ( width, height, PixelFormat.Format24bppRgb );

      int threads = checkMultithreading.Checked ? Environment.ProcessorCount : 1;
      int t; // thread ordinal number

      WorkerThreadInit[] wti = new WorkerThreadInit[threads];

      // separate renderer, image function and the scene for each thread (safety precaution)
      for ( t = 0; t < threads; t++ )
      {
        IRayScene      sc  = FormSupport.getScene ();
        IImageFunction imf = getImageFunction ( sc, width, height );
        IRenderer      r   = getRenderer ( imf, width, height );
        wti[t] = new WorkerThreadInit ( r, sc as ITimeDependent, imf as ITimeDependent, newImage, width, height, t,
                                           threads );
      }

      progress.SyncInterval = ( ( width * (long) height ) > ( 2L << 20 ) ) ? 3000L : 1000L;
      progress.Reset ();
      CSGInnerNode.ResetStatistics ();

      lock ( sw )
        sw.Restart ();

      if ( threads > 1 )
      {
        Thread[] pool = new Thread[threads];
        for ( t = 0; t < threads; t++ )
          pool[t] = new Thread ( new ParameterizedThreadStart ( RenderWorker ) );
        for ( t = threads; --t >= 0; )
          pool[t].Start ( wti[t] );

        for ( t = 0; t < threads; t++ )
        {
          pool[t].Join ();
          pool[t] = null;
        }
      }
      else
      {
        MT.InitThreadData ();
        wti[0].rend.RenderRectangle ( newImage, 0, 0, width, height );
      }

      long elapsed;
      lock ( sw )
      {
        sw.Stop ();
        elapsed = sw.ElapsedMilliseconds;
      }

      string msg = string.Format ( CultureInfo.InvariantCulture,
                                   "{0:f1}s  [ {1}x{2}, mt{3}, r{4:#,#}k, i{5:#,#}k, bb{6:#,#}k, t{7:#,#}k ]",
                                   1.0e-3 * elapsed, width, height, threads,
                                   ( Intersection.countRays + 500L ) / 1000L,
                                   ( Intersection.countIntersections + 500L ) / 1000L,
                                   ( CSGInnerNode.countBoundingBoxes + 500L ) / 1000L,
                                   ( CSGInnerNode.countTriangles + 500L ) / 1000L );
      SetText ( msg );
      Console.WriteLine ( "Rendering finished: " + msg );
      SetImage ( newImage );

      Cursor.Current = Cursors.Default;

      StopRendering ();
    }

    private void RenderImage2 ()
    {
      Cursor.Current = Cursors.WaitCursor;

      // determine output image size:
      int width = ImageWidth;
      if ( width <= 0 )
        width = panel1.Width;

      int height = ImageHeight;
      if ( height <= 0 )
        height = panel1.Height;

      Bitmap newImage = new Bitmap ( width, height, PixelFormat.Format24bppRgb );

      int threads = checkMultithreading.Checked ? Environment.ProcessorCount : 1;

      IRayScene      sc  = FormSupport.getScene ();
      IImageFunction imf = getImageFunction ( sc, width, height );
      IRenderer      r   = getRenderer ( imf, width, height );

      Master.singleton = new Master ( newImage, sc, r, RenderClientsForm.instance?.clients, threads );
      Master.singleton.progressData = progress;
      Master.singleton.InitializeAssignments ( newImage, sc, r );

      progress.SyncInterval = ( ( width * (long) height ) > ( 2L << 20 ) ) ? 3000L : 1000L;
      progress.Reset ();
      CSGInnerNode.ResetStatistics ();

      lock ( sw )
        sw.Restart ();

      Master.singleton.StartThreads ();
      /*
      ThreadStart ts              = delegate { Master.instance.StartThreads ( threads > 4 ? threads - 2 : threads ); };
      Thread      newRenderThread = new Thread ( ts );
      newRenderThread.Start ();

      newRenderThread.Join ();*/

      long elapsed;
      lock ( sw )
      {
        sw.Stop ();
        elapsed = sw.ElapsedMilliseconds;
      }

      string msg = string.Format ( CultureInfo.InvariantCulture,
                                   "{0:f1}s  [ {1}x{2}, mt{3}, r{4:#,#}k, i{5:#,#}k, bb{6:#,#}k, t{7:#,#}k ]",
                                   1.0e-3 * elapsed, width, height, threads,
                                   ( Intersection.countRays + 500L ) / 1000L,
                                   ( Intersection.countIntersections + 500L ) / 1000L,
                                   ( CSGInnerNode.countBoundingBoxes + 500L ) / 1000L,
                                   ( CSGInnerNode.countTriangles + 500L ) / 1000L );
      SetText ( msg );
      Console.WriteLine ( "Rendering finished: " + msg );
      SetImage ( newImage );

      Cursor.Current = Cursors.Default;

      StopRendering ();
    }

    private void SetGUI ( bool enable )
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
      textParam.Enabled =
      buttonRes.Enabled =
      pointCloudCheckBox.Enabled =
      collectDataCheckBox.Enabled =
      SavePointCloudButton.Enabled =
      buttonSave.Enabled = enable;

      buttonStop.Enabled = !enable;
    }

    delegate void SetImageCallback ( Bitmap newImage );

    public Stopwatch GetStopwatch ()
    {
      return sw;
    }

    public void SetImage ( Bitmap newImage )
    {
      if ( pictureBox1.InvokeRequired )
      {
        SetImageCallback si = new SetImageCallback ( SetImage );
        BeginInvoke ( si, new object[] { newImage } );
      }
      else
        setImage ( ref outputImage, newImage );
    }

    delegate void SetTextCallback ( string text );

    public void SetText ( string text )
    {
      if ( labelElapsed.InvokeRequired )
      {
        SetTextCallback st = new SetTextCallback ( SetText );
        BeginInvoke ( st, new object[] { text } );
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
        StopRenderingCallback ea = new StopRenderingCallback ( StopRendering );
        BeginInvoke ( ea );
      }
      else
      {
        // actually stop the rendering:
        lock ( progress )
          progress.Continue = false;
        aThread.Join ();
        aThread = null;

        // GUI stuff:
        SetGUI ( true );

        AdvancedToolsForm.instance?.RenderButtonsEnabled ( true );
        MT.renderingInProgress = false;
        MT.sceneRendered = true;

        if ( Master.singleton.pointCloud == null || Master.singleton.pointCloud.IsCloudEmpty )
          SavePointCloudButton.Enabled = false;
      }
    }

    /// <summary>
    /// Shoots single primary ray only..
    /// </summary>
    /// <param name="x">X-coordinate inside the raster image.</param>
    /// <param name="y">Y-coordinate inside the raster image.</param>
    private void singleSample ( int x, int y )
    {
      MT.singleRayTracing = true;
      RayVisualizer.singleton?.Reset ();

      // determine output image size:
      int width                 = ImageWidth;
      if ( width <= 0 ) width = panel1.Width;
      int height                = ImageHeight;
      if ( height <= 0 ) height = panel1.Height;

      if ( dirty || imfs == null )
      {
        imfs = getImageFunction ( FormSupport.getScene (), width, height );
        dirty = false;
      }

      double[] color = new double[3];
      long     hash  = imfs.GetSample ( x + 0.5, y + 0.5, color );
      labelSample.Text = string.Format ( CultureInfo.InvariantCulture,
                                         "Sample at [{0},{1}] = [{2:f},{3:f},{4:f}], {5:X}",
                                         x, y, color[0], color[1], color[2], hash );

      MT.singleRayTracing = false;
    }  

    private void SetOptions ( string[] args )
    {
      foreach ( var opt in args )
        if ( !string.IsNullOrEmpty ( opt ) &&
             opt[0] == '-' &&
             opt.Contains ( "=" ) )
        {
          string[] opts = opt.Split ( '=' );
          if ( opts.Length > 1 )
            switch ( opts[0] )
            {
              case "jittering":
                checkJitter.Checked = Util.positive ( opts[1] );
                break;

              case "shadows":
                checkShadows.Checked = Util.positive ( opts[1] );
                break;

              case "reflections":
                checkReflections.Checked = Util.positive ( opts[1] );
                break;

              case "refractions":
                checkRefractions.Checked = Util.positive ( opts[1] );
                break;

              case "multi-threading":
                checkMultithreading.Checked = Util.positive ( opts[1] );
                break;

              case "supersampling":
              {
                int v;
                if ( int.TryParse ( opts[1], out v ) )
                  numericSupersampling.Text = v.ToString ();
              }
              break;
            }
        }
    }

    private void buttonRes_Click ( object sender, EventArgs e )
    {
      FormResolution form = new FormResolution ( ImageWidth, ImageHeight );
      if ( form.ShowDialog () == DialogResult.OK )
      {
        ImageWidth = form.ImageWidth;
        ImageHeight = form.ImageHeight;
        buttonRes.Text = FormResolution.GetLabel ( ref ImageWidth, ref ImageHeight );
      }
    }

    private void buttonRender_Click ( object sender, EventArgs e )
    {
      AdvancedToolsForm.instance?.SetNewDimensions ( ImageWidth, ImageHeight );

      AdvancedTools.singleton.SetNewDimensions ( ImageWidth, ImageHeight );
      AdvancedTools.singleton.NewRenderInitialization ();

      if ( aThread != null )
        return;

      // GUI stuff:
      SetGUI ( false );     

      AdvancedToolsForm.instance?.RenderButtonsEnabled ( false );
      MT.renderingInProgress = true;
      Statistics.Reset ();


      lock ( progress )
        progress.Continue = true;

      SetText ( "Wait a moment.." );
      aThread = new Thread ( new ThreadStart ( this.RenderImage2 ) );
      aThread.Start ();
    }

    private void buttonSave_Click ( object sender, EventArgs e )
    {
      if ( outputImage == null ||
           aThread != null ) return;

      SaveFileDialog sfd = new SaveFileDialog ();
      sfd.Title = "Save PNG file";
      sfd.Filter = "PNG Files|*.png";
      sfd.AddExtension = true;
      sfd.FileName = "RenderResult";
      if ( sfd.ShowDialog () != DialogResult.OK )
        return;

      outputImage.Save ( sfd.FileName, ImageFormat.Png );
    }

    private void buttonStop_Click ( object sender, EventArgs e )
    {
      StopRendering ();
    }

    private void comboScene_SelectedIndexChanged ( object sender, EventArgs e )
    {
      selectedScene = comboScene.SelectedIndex;
      dirty = true;
    }

    private void textParam_TextChanged ( object sender, EventArgs e )
    {
      dirty = true;
    }
    private void AdvancedToolsButton_Click ( object sender, EventArgs e )
    {
      if ( AdvancedToolsForm.instance != null )
      {
        AdvancedToolsForm.instance.Activate ();

        return; //only one instance of Form1 can exist at the time
      }

      AdvancedToolsForm advancedToolsForm = new AdvancedToolsForm ();
      advancedToolsForm.Show ();
    }

    private void RayVisualiserButton_Click ( object sender, EventArgs e )
    {
      if ( RayVisualizerForm.instance != null )
      {
        RayVisualizerForm.instance.Activate ();

        return; //only one instance of RayVisualizerForm can exist at the time
      }

      Cursor.Current = Cursors.WaitCursor;

      RayVisualizerForm rayVisualizerForm = new RayVisualizerForm ();
      rayVisualizerForm.Show ();
    }

    private void addRenderClientToolStripMenuItem_Click ( object sender, EventArgs e )
    {
      if ( RenderClientsForm.instance != null )
      {
        RenderClientsForm.instance.Show ();

        RenderClientsForm.instance.Activate ();

        return; //only one instance of renderClientsForm can exist at the time
      }

      RenderClientsForm renderClientsForm = new RenderClientsForm ();
      renderClientsForm.Show ();
    }

    private Image image;
    private Point mouseDown;
    private int startX;
    private int startY;
    private int imageX;
    private int imageY;

    private bool mousePressed;
    private float zoom = 1;

    /// <summary>
    /// Handles calling singleSample for RayVisualizer and picture box image pan control
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void pictureBox1_MouseDown ( object sender, MouseEventArgs e )
    {
      if ( aThread == null && e.Button == MouseButtons.Left && MT.sceneRendered && !MT.renderingInProgress )
      {
        PointF relative = getRelativeCursorLocation (e.X, e.Y);

        if ( relative.X >= 0 )
          singleSample ( (int) relative.X, (int) relative.Y );
      }

      if ( !ModifierKeys.HasFlag ( Keys.Control ) && e.Button == MouseButtons.Left && !mousePressed ) //holding down CTRL key prevents panning
      {
        mousePressed = true;
        mouseDown    = e.Location;
        startX       = imageX;
        startY       = imageY;
      }
      else
      {
        Cursor = Cursors.Cross;
      }
    }

    /// <summary>
    /// Handles calling singleSample for RayVisualizer and picture box image pan control
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void pictureBox1_MouseMove ( object sender, MouseEventArgs e )
    {
      if ( aThread == null && e.Button == MouseButtons.Left && MT.sceneRendered && !MT.renderingInProgress )
      {
        PointF relative = getRelativeCursorLocation (e.X, e.Y);

        if ( relative.X >= 0 && !mousePressed )
          singleSample ( (int) relative.X, (int) relative.Y );
      }

      if ( mousePressed && e.Button == MouseButtons.Left )
      {
        Cursor = Cursors.NoMove2D;

        Point mousePosNow = e.Location;

        int deltaX = mousePosNow.X - mouseDown.X;
        int deltaY = mousePosNow.Y - mouseDown.Y;

        imageX = (int) ( startX + ( deltaX / zoom ) );
        imageY = (int) ( startY + ( deltaY / zoom ) );

        pictureBox1.Refresh ();
      }
    }

    private void pictureBox1_MouseUp ( object sender, MouseEventArgs e )
    {
      mousePressed = false;

      Cursor = Cursors.Default;
    }

    /// <summary>
    /// Catches mouse wheel movement for zoom in/out of image in picture box
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void pictureBox1_MouseWheel ( object sender, MouseEventArgs e )
    {     
      if ( e.Delta > 0 )
        zoomPictureBox ( true, e.Location );
      else if ( e.Delta < 0 )
        zoomPictureBox ( false, e.Location );
    }

    /// <summary>
    /// Sets necessary stuff at form load
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Form2_Load ( object sender, EventArgs e )
    {
      pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
      pictureBox1.MouseWheel += new MouseEventHandler ( pictureBox1_MouseWheel );
      KeyPreview = true;
    }

    private void Form2_FormClosing ( object sender, FormClosingEventArgs e )
    {
      StopRendering ();
    }

    /// <summary>
    /// Catches +/PageUp for zoom in or -/PageDown for zoom out of image in picture box
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Form2_KeyDown ( object sender, KeyEventArgs e )
    {
      Point middle = new Point ();
      middle.X = (int) ( ( ( imageX + image.Width ) * zoom ) / 2f );
      middle.Y = (int) ( ( ( imageY + image.Height ) * zoom ) / 2f );

      if ( e.KeyCode == Keys.Add || e.KeyCode == Keys.PageUp )
        zoomPictureBox ( true, middle );
      else if ( e.KeyCode == Keys.Subtract || e.KeyCode == Keys.PageDown )
        zoomPictureBox ( false, middle );
    }

    /// <summary>
    /// Converts absolute cursor location (in picture box) to relative location (based on position of actual picture; both translated and scaled)
    /// </summary>
    /// <param name="absoluteX">Absolute X position of cursor (technically relative to picture box)</param>
    /// <param name="absoluteY">Absolute Y position of cursor (technically relative to picture box)</param>
    /// <returns>Relative location of cursor in terms of actual rendered picture; (-1, -1) if relative position would be outside of picture</returns>
    private PointF getRelativeCursorLocation ( int absoluteX, int absoluteY )
    {
      float X = (absoluteX - ( imageX * zoom )) / zoom;
      float Y = (absoluteY - ( imageY * zoom )) / zoom;

      if ( X < 0 || X > image.Width || Y < 0 || Y > image.Height )
        return new PointF ( -1, -1 ); // cursor is outside of picture
      else
        return new PointF ( X, Y );
    }

    /// <summary>
    /// Changes global variable zoom to indicate current zoom level of picture in main picture box
    /// Variable zoom can be equal 1 (no zoom), less than 1 (zoom out) or greater than 1 (zoom in)
    /// </summary>
    /// <param name="zoomIn">TRUE if zoom in is desired; FALSE if zoom out is desired</param>
    /// <param name="zoomToPosition">Position to zoom to/zoom out from - usually cursor position or middle of picture in case of zoom by keys</param>
    private void zoomPictureBox ( bool zoomIn, Point zoomToPosition )
    {
      float oldzoom = zoom;
      float zoomFactor = 0.15F;

      if ( ModifierKeys.HasFlag ( Keys.Shift ) ) // holding down the Shift key makes zoom in/out faster
        zoomFactor = 0.45F;

      if ( zoomIn )
        zoom += zoomFactor;
      else
        zoom -= zoomFactor;

      zoom = ClampZoom ();

      int x = zoomToPosition.X - pictureBox1.Location.X;
      int y = zoomToPosition.Y - pictureBox1.Location.Y;

      int oldImageX = (int) ( x / oldzoom );
      int oldImageY = (int) ( y / oldzoom );

      int newImageX = (int) ( x / zoom );
      int newImageY = (int) ( y / zoom );

      imageX = newImageX - oldImageX + imageX;
      imageY = newImageY - oldImageY + imageY;

      pictureBox1.Refresh ();

      setWindowTitleSuffix ($" Zoom: {(int)(zoom * 100)}%");
    }

    private const float minimalAbsoluteSizeInPixels = 20;

    /// <summary>
    /// Prevents picture to be too small (minimum is absolute size of 20 pixels for width/height)
    /// </summary>
    /// <returns>Clamped zoom</returns>
    private float ClampZoom ()
    {
      float minZoomFactor = minimalAbsoluteSizeInPixels / Math.Min ( image.Width, image.Height );
      return Math.Max ( zoom, minZoomFactor );
    }

    /// <summary>
    /// Called every time main picture box is needed to be re-painted
    /// Used for re-painting after request for zoom in/out or pan
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void pictureBox1_Paint ( object sender, PaintEventArgs e )
    {
      if ( image == null )
        return;

      e.Graphics.PixelOffsetMode   = PixelOffsetMode.Half;
      e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
      e.Graphics.SmoothingMode     = SmoothingMode.None;

      OutOfScreenFix ();

      e.Graphics.ScaleTransform ( zoom, zoom );
      e.Graphics.DrawImage ( image, imageX, imageY );
    }

    private const int border = 50;

    /// <summary>
    /// Prevents panning of image outside of picture box
    /// There will always be small amount of pixels (variable border) visible at the edge
    /// </summary>
    private void OutOfScreenFix ()
    {
      float absoluteX = imageX * zoom;
      float absoluteY = imageY * zoom;

      float width = image.Width * zoom;
      float height = image.Height * zoom;

      if ( absoluteX > pictureBox1.Width - border )
        imageX = (int) ( ( pictureBox1.Width - border ) / zoom );

      if ( absoluteY > pictureBox1.Height - border )
        imageY = (int) ( ( pictureBox1.Height - border ) / zoom );

      if ( absoluteX + width < border )
        imageX = (int) ( ( border - width ) / zoom );

      if ( absoluteY + height < border )
        imageY = (int) ( ( border - height ) / zoom );
    }

    /// <summary>
    /// Opens SaveFileDialog and calls method SaveToPLYFile in PointCloud class
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void SavePointCloudButton_Click ( object sender, EventArgs e )
    {
      SaveFileDialog sfd = new SaveFileDialog ();
      sfd.Title        = @"Save PLY file";
      sfd.Filter       = @"PLY Files|*.ply";
      sfd.AddExtension = true;
      sfd.FileName     = "PointCloud";
      if ( sfd.ShowDialog () != DialogResult.OK )
        return;

      Master.singleton?.pointCloud?.SaveToPLYFile ( sfd.FileName );
    }

    /// <summary>
    /// Displays bubble notification (in system tray or Notification Center) with desired title, text and diration
    /// </summary>
    /// <param name="title">Title of notification</param>
    /// <param name="text">Body of notification</param>
    /// <param name="duration">Duration of notification in milliseconds</param>
    public void Notification ( string title, string text, int duration )
    {
      if ( text == null || title == null )
        return;

      notificationIcon.Icon = SystemIcons.Information;

      notificationIcon.Visible = true;
      notificationIcon.BalloonTipTitle = title;
      notificationIcon.BalloonTipText = text;
      notificationIcon.ShowBalloonTip ( duration );
      notificationIcon.Visible = false;
    }

    /// <summary>
    /// Adds suffix to default text in Form>text property (title text in upper panel, between icon and minimize and close buttons)
    /// </summary>
    /// <param name="suffix"></param>
    void setWindowTitleSuffix ( string suffix )
    {
      if ( string.IsNullOrEmpty ( suffix ) )
        Text = winTitle;
      else
        Text = winTitle + ' ' + suffix;
    }

    /// <summary>
    /// Resets image in picture box to 100% zoom and default position
    /// (left upper corner of image in left upper conrner of picture box)
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ResetButton_Click ( object sender, EventArgs e )
    {
      zoom = 1;
      startX = 0;
      startY = 0;

      imageX = 0;
      imageY = 0;

      pictureBox1.Refresh ();

      setWindowTitleSuffix ( $" Zoom: {(int) ( zoom * 100 )}%" );
    }
  }
}
