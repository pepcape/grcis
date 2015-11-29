using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace _082circles
{
  public partial class Form1 : Form
  {
    static readonly string rev = "$Rev$".Split( ' ' )[ 1 ];

    /// <summary>
    /// Current computed image.
    /// </summary>
    protected Bitmap outputImage = null;

    public Form1 ()
    {
      InitializeComponent();

      // custom init:
      int width, height;
      string param;
      Circles.InitParams( out width, out height, out param );
      numericXres.Value = Math.Max( width, 10 );
      numericYres.Value = Math.Max( height, 10 );
      textParam.Text = param ?? "";

      Text += " (rev: " + rev + ')';
    }

    private void setImage ( ref Bitmap bakImage, Bitmap newImage )
    {
      pictureBox1.Image = newImage;
      if ( bakImage != null )
        bakImage.Dispose();
      bakImage = newImage;
    }

    private void buttonRedraw_Click ( object sender, EventArgs e )
    {
      int width  = (int)numericXres.Value;
      int height = (int)numericYres.Value;

      buttonSave.Enabled = false;

      Stopwatch sw = new Stopwatch();
      sw.Start();

      Canvas c = new Canvas( width, height );
      Circles.Draw( c, textParam.Text );
      Bitmap newImage = c.Finish();

      sw.Stop();
      float elapsed = 0.001f * sw.ElapsedMilliseconds;

      labelElapsed.Text  = string.Format( CultureInfo.InvariantCulture, "Elapsed: {0:f3}s", elapsed );
      setImage( ref outputImage, newImage );

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
