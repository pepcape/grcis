using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
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
