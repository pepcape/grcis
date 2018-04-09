using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;
using GuiSupport;
using MathSupport;
using Rendering;
using Utilities;

namespace _019raytracing
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
    /// Scenes for the listbox.
    /// </summary>
    public Dictionary<string, object> sceneRepository = null;

    /// <summary>
    /// Index of the current (selected) scene.
    /// </summary>
    protected volatile int selectedScene = 0;

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
    /// Global stopwatch for rendering thread. Locked access.
    /// </summary>
    protected Stopwatch sw = new Stopwatch();

    /// <summary>
    /// Rendering thread.
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

      public override void Sync ( object msg )
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
      string sceneName = (string)comboScene.Items[ selectedScene ];

      object initFunction;
      InitSceneDelegate isd = null;
      InitSceneParamDelegate ispd = null;
      sceneRepository.TryGetValue( sceneName, out initFunction );
      isd = initFunction as InitSceneDelegate;
      ispd = initFunction as InitSceneParamDelegate;
      if ( isd == null &&
           ispd == null )
        return Scenes.DefaultScene( sc );

      if ( isd != null )
        isd( sc );
      else
        ispd?.Invoke( sc, textParam.Text );

      SetText( $"Rendering '{sceneName}'.." );
      return sc;
    }

    public ComboBox ComboScene
    {
      get { return comboScene; }
    }

    public TextBox TextParam
    {
      get { return textParam; }
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

      if ( rend == null )
        rend = FormSupport.getRenderer( imf );
      rend.Width  = width;
      rend.Height = height;
      rend.Adaptive = 8;
      rend.ProgressData = progress;

      progress.SyncInterval = 5000L;
      progress.Reset();
      CSGInnerNode.ResetStatistics();
      MT.InitThreadData();

      lock ( sw )
      {
        sw.Reset();
        sw.Start();
      }

      rend.RenderRectangle( newImage, 0, 0, width, height );

      long elapsed;
      lock ( sw )
      {
        sw.Stop();
        elapsed = sw.ElapsedMilliseconds;
      }

      string msg = string.Format( CultureInfo.InvariantCulture, "{0:f1}s  [ {1}x{2}, r{3:#,#}k, i{4:#,#}k, bb{5:#,#}k, t{6:#,#}k ]",
                                  1.0e-3 * elapsed, width, height,
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
      buttonRender.Enabled =
      comboScene.Enabled =
      textParam.Enabled =
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
      double[] color = new double[ 3 ];
      long hash = imf.GetSample( x + 0.5, y + 0.5, color );
      labelSample.Text = string.Format( CultureInfo.InvariantCulture, "Sample at [{0},{1}] = [{2:f},{3:f},{4:f}], {5:X}",
                                        x, y, color[ 0 ], color[ 1 ], color[ 2 ], hash );
    }

    public Form1 ( string[] args )
    {
      singleton = this;
      InitializeComponent();
      progress = new RenderingProgress( this );

      // Init scenes etc.
      string name;
      FormSupport.InitializeScenes( args, out name );
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

    private void buttonStop_Click ( object sender, EventArgs e )
    {
      StopRendering();
    }

    private void comboScene_SelectedIndexChanged ( object sender, EventArgs e )
    {
      selectedScene = comboScene.SelectedIndex;
      imf = null;
    }

    private void textParam_TextChanged ( object sender, EventArgs e )
    {
      imf = null;
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
