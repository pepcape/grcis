using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Rendering;
using System.Threading;

namespace _019raytracing
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
    /// Global stopwatch for rendering thread. Lock the access.
    /// </summary>
    protected Stopwatch sw = new Stopwatch();

    /// <summary>
    /// Rendering thread.
    /// </summary>
    protected Thread aThread = null;

    /// <summary>
    /// Progress info / user break handling.
    /// </summary>
    protected Progress progress = new Progress();

    /// <summary>
    /// [Re]-renders the whole image (in separate thread).
    /// </summary>
    private void RenderImage ()
    {
      Cursor.Current = Cursors.WaitCursor;

      int width   = panel1.Width;
      int height  = panel1.Height;
      Bitmap newImage = new Bitmap( width, height, System.Drawing.Imaging.PixelFormat.Format24bppRgb );

      if ( imf == null )
        imf = getImageFunction();
      imf.Width  = width;
      imf.Height = height;

      if ( rend == null )
        rend = getRenderer( imf );
      rend.Width  = width;
      rend.Height = height;
      rend.Adaptive = 1;
      rend.ProgressData = progress;

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

      SetText( String.Format( "Elapsed: {0:f}s", 1.0e-3 * elapsed ) );
      SetImage( (Bitmap)newImage.Clone() );

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
        pictureBox1.Image = outputImage = newImage;
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

      if ( buttonRedraw.InvokeRequired )
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
        buttonRedraw.Enabled = true;
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
        imf = getImageFunction();

      imf.Width  = panel1.Width;
      imf.Height = panel1.Height;
      double[] color = new double[ 3 ];
      long hash = imf.GetSample( x, y, color );
      labelSample.Text = String.Format( "Sample at [{0},{1}] = [{2:f},{3:f},{4:f}], {5}",
                                        x, y, color[ 0 ], color[ 1 ], color[ 2 ], hash );
    }

    public Form1 ()
    {
      InitializeComponent();
      String []tok = "$Rev$".Split( new char[] { ' ' } );
      Text += " (rev: " + tok[1] + ')';
    }

    private void buttonRedraw_Click ( object sender, EventArgs e )
    {
      if ( aThread != null )
        return;

      buttonRedraw.Enabled = false;
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
