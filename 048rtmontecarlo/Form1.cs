using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;
using Rendering;

namespace _048rtmontecarlo
{
  public partial class Form1 : Form
  {
    /// <summary>
    /// Current output raster image. Locked access.
    /// </summary>
    protected Bitmap outputImage = null;

    /// <summary>
    /// Scene to be rendered.
    /// </summary>
    protected IRayScene scene = null;

    /// <summary>
    /// The same order as items in the comboScenes.
    /// </summary>
    protected List<InitSceneDelegate> sceneInitFunctions = null;

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

      public override void Sync ( Object msg )
      {
        long now = f.sw.ElapsedMilliseconds;
        if ( now - lastSync < 6000L )
          return;

        lastSync = now;
        f.SetText( String.Format( "{0:f1}%:  {1:f1}s", 100.0f * Finished, 1.0e-3 * now ) );
        Bitmap b = (msg as Bitmap);
        if ( b != null )
          f.SetImage( (Bitmap)b.Clone() );
      }
    }

    /// <summary>
    /// Progress info / user break handling.
    /// </summary>
    protected RenderingProgress progress = null;

    /// <summary>
    /// Default behavior - create scene selected in the combo-box.
    /// </summary>
    protected IRayScene SceneByComboBox ()
    {
      DefaultRayScene sc = new DefaultRayScene();
      sceneInitFunctions[ selectedScene ]( sc );
      return sc;
    }

    /// <summary>
    /// [Re]-renders the whole image (in separate thread).
    /// </summary>
    private void RenderImage ()
    {
      Cursor.Current = Cursors.WaitCursor;

      int width   = panel1.Width;
      int height  = panel1.Height;
      outputImage = new Bitmap( width, height, System.Drawing.Imaging.PixelFormat.Format24bppRgb );

      if ( imf == null )
      {
        imf = getImageFunction();
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
        rend = getRenderer( imf );
      rend.Width  = width;
      rend.Height = height;
      rend.Adaptive = 16;
      rend.ProgressData = progress;
      progress.Reset();
      CSGInnerNode.ResetStatistics();

      lock ( sw )
      {
        sw.Reset();
        sw.Start();
      }

      rend.RenderRectangle( outputImage, 0, 0, width, height );

      long elapsed;
      lock ( sw )
      {
        sw.Stop();
        elapsed = sw.ElapsedMilliseconds;
      }

      SetText( String.Format( CultureInfo.InvariantCulture, "{0:f1}s  [ {1}x{2}, c{3:#,#}, i{4:#,#} ]",
                              1.0e-3 * elapsed, width, height, CSGInnerNode.countCalls, CSGInnerNode.countIntersections ) );
      SetImage( (Bitmap)outputImage.Clone() );

      Cursor.Current = Cursors.Default;

      StopRendering();
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
        {
          progress.Continue = false;
        }
        aThread.Join();
        aThread = null;

        // GUI stuff:
        buttonRender.Enabled = true;
        comboScene.Enabled = true;
        buttonSave.Enabled = true;
        buttonStop.Enabled = false;
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
        imf = getImageFunction();
        rend = null;
      }
      imf.Width  = panel1.Width;
      imf.Height = panel1.Height;
      RayTracing rt = imf as RayTracing;
      if ( rt != null )
      {
        rt.DoShadows = checkShadows.Checked;
        rt.DoReflections = checkReflections.Checked;
        rt.DoRefractions = checkRefractions.Checked;
      }

      double[] color = new double[ 3 ];
      long hash = imf.GetSample( x + 0.5, y + 0.5, color );
      labelSample.Text = String.Format( "Sample at [{0},{1}] = [{2:f},{3:f},{4:f}], {5}",
                                        x, y, color[ 0 ], color[ 1 ], color[ 2 ], hash );
    }

    public Form1 ()
    {
      InitializeComponent();
      progress = new RenderingProgress( this );
      String []tok = "$Rev$".Split( new char[] { ' ' } );
      Text += " (rev: " + tok[1] + ')';
      // Scenes combo-box
      InitializeScenes();
    }

    private void buttonRender_Click ( object sender, EventArgs e )
    {
      if ( aThread != null )
        return;

      buttonRender.Enabled = false;
      comboScene.Enabled = false;
      buttonSave.Enabled = false;
      buttonStop.Enabled = true;
      lock ( progress )
      {
        progress.Continue = true;
      }

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

      outputImage.Save( sfd.FileName, System.Drawing.Imaging.ImageFormat.Png );
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
