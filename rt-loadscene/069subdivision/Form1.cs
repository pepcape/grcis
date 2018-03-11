using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace _069subdivision
{
  public partial class Form1 : Form
  {
    protected Image outputImage = null;

    public Form1 ()
    {
      InitializeComponent();
      string[] tok = "$Rev$".Split( ' ' );
      Text += " (rev: " + tok[ 1 ] + ')';
    }

    private void buttonRedraw_Click ( object sender, EventArgs e )
    {
      int width  = (int)numericXres.Value;
      int height = (int)numericYres.Value;

      Bitmap output = new Bitmap( width, height, System.Drawing.Imaging.PixelFormat.Format24bppRgb );

      Stopwatch sw = new Stopwatch();
      sw.Start();

      Subdivision.TestImage( output, textParam.Text );

      sw.Stop();
      float elapsed = 1.0e-3f * sw.ElapsedMilliseconds;

      labelElapsed.Text = string.Format( CultureInfo.InvariantCulture, "Elapsed: {0:f3}s", elapsed );

      pictureBox1.Image = outputImage = output;
      buttonSave.Enabled = true;
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
