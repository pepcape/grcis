using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using OpenTK;

namespace _025contours
{
  public partial class Form1 : Form
  {
    /// <summary>
    /// Output raster image.
    /// </summary>
    protected Bitmap outputImage = null;

    /// <summary>
    /// Implicit function delegate.
    /// </summary>
    protected Func<double,double,double> f = null;

    /// <summary>
    /// Array of thresholds.
    /// </summary>
    protected List<double> thr = new List<double>();

    /// <summary>
    /// Available functions.
    /// </summary>
    protected List<Func<double, double, double>> functions;

    /// <summary>
    /// Position of the coordinate system origin in Bitmap coordinates.
    /// </summary>
    protected Vector2d origin = new Vector2d( 200.0, 200.0 );

    /// <summary>
    /// Scaling factor from Bitmap coordinates to function argument space.
    /// </summary>
    protected double scale = 1.0;

    /// <summary>
    /// Threshold drift (from the standard threshold set).
    /// </summary>
    protected double valueDrift = 0.0;

    private void resetScale ()
    {
      origin.X = panel1.Width / 2;
      origin.Y = panel1.Height / 2;
      scale = 1.0;
      valueDrift = 0.0;
    }

    /// <summary>
    /// Redraws the whole image.
    /// </summary>
    private void redraw ()
    {
      Cursor.Current = Cursors.WaitCursor;

      int width   = panel1.Width;
      int height  = panel1.Height;
      if ( outputImage == null ||
           outputImage.Width != width ||
           outputImage.Height != height )
        outputImage = new Bitmap( width, height, System.Drawing.Imaging.PixelFormat.Format24bppRgb );

      Stopwatch sw = new Stopwatch();
      sw.Start();

      ComputeContours( outputImage );

      sw.Stop();
      labelElapsed.Text = String.Format( "Elapsed: {0:f}s", 1.0e-3 * sw.ElapsedMilliseconds );

      pictureBox1.Image = outputImage;

      Cursor.Current = Cursors.Default;
    }

    /// <summary>
    /// Computes function at the single point..
    /// </summary>
    /// <param name="x">X-coordinate inside the raster image.</param>
    /// <param name="y">Y-coordinate inside the raster image.</param>
    private void singleSample ( int x, int y )
    {
      if ( f != null )
      {
        double dx = (x - origin.X) * scale;
        double dy = (y - origin.Y) * scale;
        double val = f( dx, dy );
        labelSample.Text = String.Format( "Sample at [{0},{1}], [{2:f},{3:f}] = {4:f}",
                                          x, y, dx, dy, val );
      }
    }

    public Form1 ()
    {
      InitializeComponent();
      InitializeFunctions();
    }

    protected bool dragging = false;
    protected bool dragged = false;

    protected int lastX, lastY;

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
      if ( e.Button == MouseButtons.Left ||
           e.Button == MouseButtons.Right )
      {
        singleSample( e.X, e.Y );
        dragging = true;
        dragged = false;
        lastX = e.X;
        lastY = e.Y;
      }
    }

    private void pictureBox1_MouseMove ( object sender, MouseEventArgs e )
    {
      if ( !dragging ) return;

      if ( e.Button == MouseButtons.Left )
      {
        origin.X += e.X - lastX;
        origin.Y += e.Y - lastY;
        lastX = e.X;
        lastY = e.Y;
        dragged = true;
      }

      if ( e.Button == MouseButtons.Right )
      {
        scale *= Math.Exp( 0.01 * (e.Y - lastY) );
        valueDrift += 0.02 * (e.X - lastX);
        lastX = e.X;
        lastY = e.Y;
        dragged = true;
      }
    }

    private void pictureBox1_MouseUp ( object sender, MouseEventArgs e )
    {
      if ( dragging )
      {
        dragging = false;
        if ( dragged )
          redraw();
      }
    }

    private void buttonReset_Click ( object sender, EventArgs e )
    {
      resetScale();
      redraw();
    }

    private void comboFunction_SelectedIndexChanged ( object sender, EventArgs e )
    {
      f = functions[ comboFunction.SelectedIndex ];
      redraw();
    }
  }
}
