using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using OpenTK;
using Scene3D;

namespace _013scene
{
  public partial class Form1 : Form
  {
    protected SceneBrep scene = new SceneBrep();

    protected Bitmap outputImage = null;

    public Form1 ()
    {
      InitializeComponent();
    }

    private void buttonOpen_Click ( object sender, EventArgs e )
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
      objReader.MirrorConversion = true;
      StreamReader reader    = new StreamReader( new FileStream( ofd.FileName, FileMode.Open ) );
      int faces = objReader.ReadBrep( reader, scene );

      labelFaces.Text = String.Format( "{0} faces", faces );
      redraw();
    }

    private void redraw ()
    {
      if ( scene == null ) return;

      Cursor.Current = Cursors.WaitCursor;

      int width  = panel1.Width;
      int height = panel1.Height;
      outputImage = new Bitmap( width, height, System.Drawing.Imaging.PixelFormat.Format24bppRgb );

      Wireframe renderer = new Wireframe();
      renderer.Perspective = false;
      renderer.Azimuth     = (double)numericAzimuth.Value;
      renderer.Elevation   = 20.0;
      renderer.ViewVolume  =  8.0;
      renderer.Distance    = 30.0;
      renderer.Render( outputImage, scene );

      pictureBox1.Image = outputImage;

      Cursor.Current = Cursors.Default;
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

    private void buttonRedraw_Click ( object sender, EventArgs e )
    {
      redraw();
    }
  }
}
