using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace _002warping
{
  public partial class FormWarping : Form
  {
    protected Image inputImage = null;
    protected WarpMagniGlass warp = new WarpMagniGlass();

    public FormWarping ()
    {
      InitializeComponent();
    }

    private void buttonInput_Click ( object sender, EventArgs e )
    {
      OpenFileDialog ofd = new OpenFileDialog();

      ofd.Title = "Open Image File";
      ofd.Filter = "Bitmap Files|*.bmp" +
          "|Gif Files|*.gif" + 
          "|JPEG Files|*.jpg" +
          "|PNG Files|*.png" + 
          "|TIFF Files|*.tif" +
          "|All image types|*.bmp;*.gif;*.jpg;*.png;*.tif";

      ofd.FilterIndex = 6;
      ofd.FileName = "";
      ofd.ShowDialog();

      if ( ofd.FileName == "" )
        return;

      inputImage = Image.FromFile( ofd.FileName );

      rewarp();
    }

    private void rewarp ()
    {
      if ( inputImage == null ) return;

      Bitmap ibmp = (Bitmap)inputImage;
      Bitmap bmp;
      Warping.WarpImage( ibmp, out bmp, warp );
      outputPictureBox.Image = bmp;
    }

    private void numericFactor_ValueChanged ( object sender, EventArgs e )
    {
      double fact = (double)numericFactor.Value;
      warp.Factor = fact;

      rewarp();
    }

    private void buttonSave_Click ( object sender, EventArgs e )
    {
      if ( inputImage == null ) return;

      SaveFileDialog sfd = new SaveFileDialog();
      sfd.Title = "Save PNG file";
      sfd.Filter = "PNG Files|*.png";
      sfd.AddExtension = true;
      sfd.FileName = "";
      sfd.ShowDialog();

      if ( sfd.FileName == "" )
        return;

      outputPictureBox.Image.Save( sfd.FileName, System.Drawing.Imaging.ImageFormat.Png );
    }
  }
}
