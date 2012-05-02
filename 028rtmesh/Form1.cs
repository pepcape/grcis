using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using GuiSupport;
using MathSupport;
using Rendering;
using Scene3D;

namespace _028rtmesh
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
    /// Image width in pixels, 0 for default value (according to panel size).
    /// </summary>
    protected int ImageWidth = 0;

    /// <summary>
    /// Image height in pixels, 0 for default value (according to panel size).
    /// </summary>
    protected int ImageHeight = 0;

    /// <summary>
    /// Global instance of a random generator.
    /// </summary>
    private static RandomJames rnd = new RandomJames();

    /// <summary>
    /// B-rep scene read from the .OBJ file.
    /// </summary>
    protected SceneBrep brepScene = new SceneBrep();

    /// <summary>
    /// Redraws the whole image.
    /// </summary>
    private void redraw ()
    {
      Cursor.Current = Cursors.WaitCursor;

      // determine output image size:
      int width = ImageWidth;
      if ( width <= 0 ) width = panel1.Width;
      int height = ImageHeight;
      if ( height <= 0 ) height = panel1.Height;
      outputImage = new Bitmap( width, height, System.Drawing.Imaging.PixelFormat.Format24bppRgb );

      if ( imf == null )
        imf = getImageFunction();
      imf.Width  = width;
      imf.Height = height;

      if ( rend == null )
        rend = getRenderer();
      rend.Width  = width;
      rend.Height = height;
      CSGInnerNode.ResetStatistics();

      Stopwatch sw = new Stopwatch();
      sw.Start();

      rend.RenderRectangle( outputImage, 0, 0, width, height, rnd );

      sw.Stop();
      labelElapsed.Text = String.Format( CultureInfo.InvariantCulture, "{0:f1}s  [ {1}x{2}, r{3:#,#}k, i{4:#,#}k, bb{5:#,#}k, t{6:#,#}k ]",
                                         1.0e-3 * sw.ElapsedMilliseconds, width, height,
                                         (Intersection.countRays + 500L) / 1000L,
                                         (Intersection.countIntersections + 500L) / 1000L,
                                         (CSGInnerNode.countBoundingBoxes + 500L) / 1000L,
                                         (CSGInnerNode.countTriangles + 500L) / 1000L );
      pictureBox1.Image = outputImage;

      Cursor.Current = Cursors.Default;
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

      // determine output image size:
      int width = ImageWidth;
      if ( width <= 0 ) width = panel1.Width;
      int height = ImageHeight;
      if ( height <= 0 ) height = panel1.Height;
      imf.Width = width;
      imf.Height = height;
      double[] color = new double[ 3 ];
      long hash = imf.GetSample( x, y, color );
      labelSample.Text = String.Format( "Sample at [{0},{1}] = [{2:f},{3:f},{4:f}], {5}",
                                        x, y, color[ 0 ], color[ 1 ], color[ 2 ], hash );
    }

    public Form1 ()
    {
      InitializeComponent();
      String[] tok = "$Rev$".Split( new char[] { ' ' } );
      Text += " (rev: " + tok[ 1 ] + ')';
    }

    private void buttonRes_Click ( object sender, EventArgs e )
    {
      FormResolution form = new FormResolution( ImageWidth, ImageHeight );
      if ( form.ShowDialog() == DialogResult.OK )
      {
        ImageWidth = form.ImageWidth;
        ImageHeight = form.ImageHeight;
        buttonRes.Text = String.Format( "{0} x {1}", ImageWidth, ImageHeight );
      }
    }

    private void buttonRedraw_Click ( object sender, EventArgs e )
    {
      redraw();
    }

    private void buttonSave_Click ( object sender, EventArgs e )
    {
      if ( outputImage == null ) return;

      SaveFileDialog sfd = new SaveFileDialog();
      sfd.Title = "Save PNG file";
      sfd.Filter = "PNG Files|*.png";
      sfd.AddExtension = true;
      sfd.FileName = "";
      if ( sfd.ShowDialog() != DialogResult.OK )
        return;

      outputImage.Save( sfd.FileName, System.Drawing.Imaging.ImageFormat.Png );
    }

    private void pictureBox1_MouseDown ( object sender, MouseEventArgs e )
    {
      if ( e.Button == MouseButtons.Left )
        singleSample( e.X, e.Y );
    }

    private void pictureBox1_MouseMove ( object sender, MouseEventArgs e )
    {
      if ( e.Button == MouseButtons.Left )
        singleSample( e.X, e.Y );
    }

    private void buttonLoad_Click ( object sender, EventArgs e )
    {
      OpenFileDialog ofd = new OpenFileDialog();

      ofd.Title = "Open Scene File";
      ofd.Filter = "Wavefront OBJ Files|*.obj" +
          "|All scene types|*.obj";

      ofd.FilterIndex = 1;
      ofd.FileName = "";
      if ( ofd.ShowDialog() != DialogResult.OK )
        return;

      WavefrontObj objReader = new WavefrontObj();
      objReader.MirrorConversion = false;
      StreamReader reader = new StreamReader( new FileStream( ofd.FileName, FileMode.Open ) );
      int faces = objReader.ReadBrep( reader, brepScene );
      reader.Close();
      brepScene.BuildCornerTable();
      labelSample.Text = String.Format( "{0} faces", faces );

      imf = null;  // reset the scene object (to be sure)
      rend = null; // reset the renderer as well..
    }
  }
}
