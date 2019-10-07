using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utilities;

namespace _117raster
{
  public partial class FormMain : Form
  {
    static readonly string rev = Util.SetVersion("$Rev$");

    /// <summary>
    /// Input image read from disk / drag-and-dropped.
    /// </summary>
    protected Bitmap inputImage = null;

    /// <summary>
    /// Optional output image for 'input->output' template.
    /// </summary>
    protected Bitmap outputImage = null;

    /// <summary>
    /// Window title prefix.
    /// </summary>
    protected string titlePrefix = null;

    /// <summary>
    /// Param string tooltip = help.
    /// </summary>
    string tooltip = "-- no tooltip yet --";

    /// <summary>
    /// Shared ToolTip instance.
    /// </summary>
    ToolTip tt = new ToolTip();

    public FormMain ()
    {
      InitializeComponent();

      titlePrefix = Text += " (" + rev + ")";
    }

    private static void setImage (ref Bitmap bakImage, Bitmap newImage)
    {
      bakImage?.Dispose();
      bakImage = newImage;
    }

    /// <summary>
    /// Picture-box background color (for alpha-images).
    /// </summary>
    public static Color imageBoxBackground = Color.White;

    delegate void SetImageCallback (Bitmap newImage);

    protected void SetImage (Bitmap newImage)
    {
      if (pictureBoxMain.InvokeRequired)
      {
        SetImageCallback si = new SetImageCallback(SetImage);
        BeginInvoke(si, new object[] { newImage });
      }
      else
      {
        pictureBoxMain.Image = newImage;
        pictureBoxMain.BackColor = imageBoxBackground;
        setImage(ref outputImage, newImage);
      }
    }

    delegate void SetTextCallback (string text);

    protected void SetText (string text)
    {
      if (labelStatus.InvokeRequired)
      {
        SetTextCallback st = new SetTextCallback(SetText);
        BeginInvoke(st, new object[] { text });
      }
      else
        labelStatus.Text = text;
    }

    private void imageProbe (int x, int y)
    {
      if (outputImage == null)
        return;

      x = Util.Clamp(x, 0, outputImage.Width - 1);
      y = Util.Clamp(y, 0, outputImage.Height - 1);

      Color c = outputImage.GetPixel(x, y);
      StringBuilder sb = new StringBuilder();
      sb.AppendFormat(" image[{0},{1}] = ", x, y);
      if (outputImage.PixelFormat == PixelFormat.Format32bppArgb ||
          outputImage.PixelFormat == PixelFormat.Format64bppArgb ||
          outputImage.PixelFormat == PixelFormat.Format16bppArgb1555)
        sb.AppendFormat("[{0},{1},{2},{3}] = #{0:X02}{1:X02}{2:X02}{3:X02}", c.R, c.G, c.B, c.A);
      else
        sb.AppendFormat("[{0},{1},{2}] = #{0:X02}{1:X02}{2:X02}", c.R, c.G, c.B);

      SetText(sb.ToString());
    }

    private void buttonLoadImage_Click (object sender, EventArgs e)
    {
      using (OpenFileDialog ofd = new OpenFileDialog()
      {
        Title = "Open Image File",
        Filter = "Bitmap Files|*.bmp" +
          "|Gif Files|*.gif" +
          "|JPEG Files|*.jpg" +
          "|PNG Files|*.png" +
          "|TIFF Files|*.tif" +
          "|All image types|*.bmp;*.gif;*.jpg;*.png;*.tif",
        FilterIndex = 6,
        FileName = ""
      })
      {
        if (ofd.ShowDialog() == DialogResult.OK)
          newImage(ofd.FileName);
      }
    }

    /// <summary>
    /// Sets the new image defined by its file-name. Does all the checks.
    /// </summary>
    /// <returns>True if succeeded.</returns>
    private bool newImage (string fn)
    {
      Bitmap inp;
      try
      {
        inp = (Bitmap)Image.FromFile(fn);
      }
      catch (Exception)
      {
        return false;
      }

      if (inp == null)
        return false;

      pictureBoxMain.Image = null;
      setImage(ref inputImage, inp);
      setImage(ref outputImage, null);

      //recompute();
      SetImage(inputImage);

      Text = titlePrefix + ' ' + fn;

      return true;
    }

    private void buttonSaveImage_Click (object sender, EventArgs e)
    {
      if (outputImage == null)
        return;

      using (SaveFileDialog sfd = new SaveFileDialog
      {
        Title = "Save PNG file",
        Filter = "PNG Files|*.png",
        AddExtension = true,
        FileName = ""
      })
      {
        if (sfd.ShowDialog() == DialogResult.OK)
          outputImage.Save(sfd.FileName, ImageFormat.Png);
      }
    }

    private void FormMain_DragDrop (object sender, DragEventArgs e)
    {
      string[] strFiles = (string[])e.Data.GetData(DataFormats.FileDrop);
      newImage(strFiles[0]);
    }

    private void FormMain_DragEnter (object sender, DragEventArgs e)
    {
      if (e.Data.GetDataPresent(DataFormats.FileDrop))
        e.Effect = DragDropEffects.Copy;
    }

    private void FormMain_FormClosing (object sender, FormClosingEventArgs e)
    {
      // !!! TODO: stop computing, etc.
    }

    private void FormMain_FormClosed (object sender, FormClosedEventArgs e)
    {
      inputImage?.Dispose();
      outputImage?.Dispose();
      tt.Dispose();
    }

    private void textBoxParam_KeyPress (object sender, KeyPressEventArgs e)
    {
      if (e.KeyChar == (char)Keys.Enter)
      {
        e.Handled = true;
        // recompute();
      }
    }

    private void textBoxParam_MouseHover (object sender, EventArgs e)
    {
      tt.Show(tooltip, (IWin32Window)sender, 10, -25, 2000);
    }

    private void labelStatus_MouseHover (object sender, EventArgs e)
    {
      tt.Show(Util.TargetFramework + " (" + Util.RunningFramework + ')',
              (IWin32Window)sender, 10, -25, 2000);
    }

    private void pictureBoxMain_MouseDown (object sender, MouseEventArgs e)
    {
      if (e.Button == MouseButtons.Left)
        imageProbe(e.X, e.Y);
    }

    private void pictureBoxMain_MouseMove (object sender, MouseEventArgs e)
    {
      if (e.Button == MouseButtons.Left)
        imageProbe(e.X, e.Y);
    }
  }
}
