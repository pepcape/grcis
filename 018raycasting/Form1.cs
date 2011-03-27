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
using Rendering;

namespace _018raycasting
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

      SimpleImageSynthesizer sis = new SimpleImageSynthesizer();
      sis.Width  = width;
      sis.Height = height;

      // default constructor of the RayScene .. custom scene
      RayScene scene = new RayScene();
      IImageFunction imf = new RayCasting( scene );
      imf.Width  = width;
      imf.Height = height;
      sis.ImageFunction = imf;

      Stopwatch sw = new Stopwatch();
      sw.Start();

      sis.RenderRectangle( outputImage, 0, 0, width, height );

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
