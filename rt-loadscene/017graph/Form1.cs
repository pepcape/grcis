using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.Windows.Forms;
using Scene3D;

namespace _017graph
{
  public partial class Form1 : Form
  {
    static readonly string rev = "$Rev$".Split( ' ' )[ 1 ];

    protected Bitmap outputImage = null;

    public Form1 ()
    {
      InitializeComponent();
      Text += " (rev: " + rev + ')';
    }

    private void setImage ( ref Bitmap bakImage, Bitmap newImage )
    {
      pictureBox1.Image = newImage;
      if ( bakImage != null )
        bakImage.Dispose();
      bakImage = newImage;
    }

    private void redraw ()
    {
      Cursor.Current = Cursors.WaitCursor;

      int width   = panel1.Width;
      int height  = panel1.Height;
      Bitmap newImage = new Bitmap( width, height, PixelFormat.Format24bppRgb );

      FunctionsR2ToR fun = new FunctionsR2ToR();
      fun.Variant = (int)numericVariant.Value;
      double centerx = 0.5 * (fun.MinX + fun.MaxX);
      double centerz = 0.5 * (fun.MinZ + fun.MaxZ);
      double coef = (double)numericParam.Value;
      fun.MinX = centerx - coef * (centerx - fun.MinX);
      fun.MaxX = centerx + coef * (fun.MaxX - centerx);
      fun.MinZ = centerz - coef * (centerz - fun.MinZ);
      fun.MaxZ = centerz + coef * (fun.MaxZ - centerz);

      GraphRendering renderer = new GraphRendering();
      renderer.Perspective = checkPerspective.Checked;
      renderer.Azimuth     = (double)numericAzimuth.Value;
      renderer.Elevation   = (double)numericElevation.Value;
      renderer.ViewVolume  = 30.0;
      renderer.Distance    = 10.0;
      renderer.Rows        = (int)numericRows.Value;
      renderer.Columns     = (int)numericColumns.Value;

      Stopwatch sw = new Stopwatch();
      sw.Start();

      renderer.Render( newImage, fun );

      sw.Stop();
      labelElapsed.Text = string.Format( CultureInfo.InvariantCulture, "Elapsed: {0:f2}s",
                                         1.0e-3 * sw.ElapsedMilliseconds );

      setImage( ref outputImage, newImage );

      Cursor.Current = Cursors.Default;
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
  }
}
