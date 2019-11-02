using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using LineCanvas;
using Utilities;

namespace _092lines
{
  public partial class Form1 : Form
  {
    static readonly string rev = Util.SetVersion("$Rev$");

    /// <summary>
    /// Current computed image.
    /// </summary>
    private Bitmap outputImage = null;

    /// <summary>
    /// Param string tooltip = help.
    /// </summary>
    private string tooltip = "";

    /// <summary>
    /// Shared ToolTip instance.
    /// </summary>
    private ToolTip tt = new ToolTip();

    public Form1 ()
    {
      InitializeComponent();

      // Custom init.
      int width, height;
      string param, name;
      Lines.InitParams(out name, out width, out height, out param, out tooltip);
      numericXres.Value = Math.Max(width, 10);
      numericYres.Value = Math.Max(height, 10);
      textParam.Text = param ?? "";

      Text += " (" + rev + ") '" + name + '\'';
    }

    private void setImage (ref Bitmap bakImage, Bitmap newImage)
    {
      pictureBox1.Image = newImage;
      bakImage?.Dispose();
      bakImage = newImage;
    }

    private void redraw ()
    {
      int width  = (int)numericXres.Value;
      int height = (int)numericYres.Value;

      buttonSave.Enabled = false;

      Stopwatch sw = new Stopwatch();
      sw.Start();

      Canvas c = new Canvas(width, height);
      Lines.Draw(c, textParam.Text);
      Bitmap newImage = c.Finish();

      sw.Stop();
      float elapsed = 0.001f * sw.ElapsedMilliseconds;

      labelElapsed.Text = string.Format(CultureInfo.InvariantCulture, "Elapsed: {0:f3}s", elapsed);
      setImage(ref outputImage, newImage);

      buttonSave.Enabled = true;
    }

    private void buttonRedraw_Click (object sender, EventArgs e)
    {
      redraw();
    }

    private void buttonSave_Click (object sender, EventArgs e)
    {
      if (outputImage == null)
        return;

      SaveFileDialog sfd = new SaveFileDialog();
      sfd.Title = "Save PNG file";
      sfd.Filter = "PNG Files|*.png";
      sfd.AddExtension = true;
      sfd.FileName = "";

      if (sfd.ShowDialog() != DialogResult.OK)
        return;

      outputImage.Save(sfd.FileName, System.Drawing.Imaging.ImageFormat.Png);
    }

    private void labelElapsed_MouseHover (object sender, EventArgs e)
    {
      tt.Show(Util.TargetFramework + " (" + Util.RunningFramework + ')',
              (IWin32Window)sender, 10, -25, 2000);
    }

    private void textParam_MouseHover (object sender, EventArgs e)
    {
      tt.Show(tooltip, (IWin32Window)sender, 10, -25, 2000);
    }

    private void textParam_KeyPress (object sender, KeyPressEventArgs e)
    {
      if (e.KeyChar == (char)Keys.Enter)
      {
        e.Handled = true;
        redraw();
      }
    }
  }
}
