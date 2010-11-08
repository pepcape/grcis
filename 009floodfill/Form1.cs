using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace _009floodfill
{
  public partial class Form1 : Form
  {
    public Form1 ()
    {
      InitializeComponent();
    }

    private void redraw ()
    {
      Cursor.Current = Cursors.WaitCursor;

      Bitmap output = new Bitmap( 512, 512, System.Drawing.Imaging.PixelFormat.Format24bppRgb );
      int size = (int)numericSize.Value;

      pictureBox1.Image = output;

      Cursor.Current = Cursors.Default;
    }

    private void buttonSave_Click ( object sender, EventArgs e )
    {
      SaveFileDialog sfd = new SaveFileDialog();
      sfd.Title = "Save PNG file";
      sfd.Filter = "PNG Files|*.png";
      sfd.AddExtension = true;
      sfd.FileName = "";
      if ( sfd.ShowDialog() != DialogResult.OK )
        return;

      pictureBox1.Image.Save( sfd.FileName, System.Drawing.Imaging.ImageFormat.Png );
    }

    private void buttonRedraw_Click ( object sender, EventArgs e )
    {
      redraw();
    }
  }
}
