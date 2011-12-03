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

    /// <summary>
    /// Available warping functions.
    /// </summary>
    protected List<IWarp> functions;

    public FormWarping ()
    {
      InitializeComponent();
      InitializeFunctions();
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
      if ( ofd.ShowDialog() != DialogResult.OK )
        return;

      Image inp = Image.FromFile( ofd.FileName );
      inputImage = new Bitmap( inp );
      inp.Dispose();

      pictureBox1.Image = inputImage;
    }

    private void rewarp ()
    {
      if ( inputImage == null ) return;

      IWarp warp = functions[ comboFunction.SelectedIndex ];
      double fact = (double)numericFactor.Value;
      warp.Factor = fact;

      Bitmap ibmp = (Bitmap)inputImage;
      Bitmap bmp;
      Warping.WarpImage( ibmp, out bmp, warp );
      pictureBox1.Image = bmp;
    }

    private void buttonRedraw_Click ( object sender, EventArgs e )
    {
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

      pictureBox1.Image.Save( sfd.FileName, System.Drawing.Imaging.ImageFormat.Png );
    }
  }
}
