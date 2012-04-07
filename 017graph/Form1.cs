using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using OpenTK;
using Scene3D;

namespace _017graph
{
  public partial class Form1 : Form
  {
    protected Bitmap outputImage = null;

    public Form1 ()
    {
      InitializeComponent();
    }

    private void redraw ()
    {
      Cursor.Current = Cursors.WaitCursor;

      int width   = panel1.Width;
      int height  = panel1.Height;
      outputImage = new Bitmap( width, height, System.Drawing.Imaging.PixelFormat.Format24bppRgb );

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

      renderer.Render( outputImage, fun );

      sw.Stop();
      labelElapsed.Text = String.Format( "Elapsed: {0:f}s", 1.0e-3 * sw.ElapsedMilliseconds );

      pictureBox1.Image = outputImage;

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

      outputImage.Save( sfd.FileName, System.Drawing.Imaging.ImageFormat.Png );
    }
  }
}
