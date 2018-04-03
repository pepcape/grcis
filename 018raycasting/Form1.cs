using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.Windows.Forms;
using MathSupport;
using Rendering;
using Utilities;

namespace _018raycasting
{
  public partial class Form1 : Form
  {
    static readonly string rev = Util.SetVersion( "$Rev$" );

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
    private void redraw ()
    {
      Cursor.Current = Cursors.WaitCursor;

      int width   = panel1.Width;
      int height  = panel1.Height;

      Bitmap newImage = new Bitmap( width, height, PixelFormat.Format24bppRgb );

      if ( imf == null )
        imf = FormSupport.getImageFunction( out scene );
      imf.Width  = width;
      imf.Height = height;

      if ( rend == null )
        rend = FormSupport.getRenderer( imf );
      rend.Width  = width;
      rend.Height = height;
      rend.Adaptive = 0;

      MT.InitThreadData();
      Stopwatch sw = new Stopwatch();
      sw.Start();

      rend.RenderRectangle( newImage, 0, 0, width, height );

      sw.Stop();
      labelElapsed.Text = string.Format( CultureInfo.InvariantCulture, "Elapsed: {0:f2}s",
                                         1.0e-3 * sw.ElapsedMilliseconds );

      setImage( ref outputImage, newImage );

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
        imf = FormSupport.getImageFunction( out scene );

      imf.Width  = panel1.Width;
      imf.Height = panel1.Height;
      double[] color = new double[ 3 ];
      long hash = imf.GetSample( x, y, color );
      labelSample.Text = string.Format( CultureInfo.InvariantCulture, "Sample at [{0},{1}] = [{2:f},{3:f},{4:f}], {5:X}",
                                        x, y, color[ 0 ], color[ 1 ], color[ 2 ], hash );
    }

    public Form1 ()
    {
      InitializeComponent();
      Text += " (rev: " + rev + ')';
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

      outputImage.Save( sfd.FileName, ImageFormat.Png );
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
  }
}
