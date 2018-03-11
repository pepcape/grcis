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
    static readonly string rev = "$Rev$".Split( ' ' )[ 1 ];

    protected Bitmap inputImage = null;
    protected Bitmap outputImage = null;

    /// <summary>
    /// Available warping functions.
    /// </summary>
    public List<IWarp> functions;

    public ComboBox ComboFunction
    {
      get { return comboFunction; }
    }

    public FormWarping ()
    {
      InitializeComponent();
      Text += " (rev: " + rev + ')';

      FormSupport.InitializeFunctions( this );
    }

    private void setImage ( ref Bitmap bakImage, Bitmap newImage )
    {
      pictureBox1.Image = newImage;
      if ( bakImage != null )
        bakImage.Dispose();
      bakImage = newImage;
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

      setImage( ref inputImage, (Bitmap)Image.FromFile( ofd.FileName ) );
    }

    private void rewarp ()
    {
      if ( inputImage == null ) return;

      IWarp warp = functions[ comboFunction.SelectedIndex ];
      double fact = (double)numericFactor.Value;
      warp.Factor = fact;

      Bitmap newImage;
      Warping.WarpImage( inputImage, out newImage, warp );

      setImage( ref outputImage, newImage );
    }

    private void buttonRedraw_Click ( object sender, EventArgs e )
    {
      rewarp();
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
