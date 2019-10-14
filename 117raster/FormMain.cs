using _117raster.Properties;
using Modules;
using Rendering;
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
    private Bitmap inputImage = null;

    /// <summary>
    /// Optional output image for 'input->output' template.
    /// </summary>
    private Bitmap outputImage = null;

    /// <summary>
    /// Image currently displayed on the form.
    /// </summary>
    private Bitmap displayedImage = null;

    /// <summary>
    /// Window title prefix (basic app title).
    /// </summary>
    private string titlePrefix = "";

    /// <summary>
    /// Middle part of the window title (file-name etc.).
    /// </summary>
    private string titleMiddle = "";

    private void SetWindowTitleSuffix (string suffix) => Text = string.IsNullOrEmpty(suffix)
      ? titlePrefix + titleMiddle
      : titlePrefix + titleMiddle + ' ' + suffix;

    /// <summary>
    /// Object responsible for interactive pan-and-zoom (right mouse button, mouse wheel).
    /// </summary>
    private readonly PanAndZoomSupport panAndZoom;

    /// <summary>
    /// Param string tooltip = help.
    /// </summary>
    string tooltip = "-- no tooltip yet --";

    /// <summary>
    /// Shared ToolTip instance.
    /// </summary>
    ToolTip tt = new ToolTip();

    /// <summary>
    /// Previously activated modules.
    /// </summary>
    Dictionary <string, IRasterModule> modules = new Dictionary<string, IRasterModule>();

    /// <summary>
    /// Current active module.
    /// </summary>
    IRasterModule currModule = null;

    public FormMain ()
    {
      InitializeComponent();

      // Touch all modules and force their registration.
      // !!! TODO: automatic registration ???
      new ModuleGlobalHistogram();

      titlePrefix = Text += " (" + rev + ")";
      SetWindowTitleSuffix(" Zoom: 100%");

      // Placeholder image for PictureBox.
      displayedImage = Resources.InitialImage;

      // Default PaZ button = Right.
      panAndZoom = new PanAndZoomSupport(pictureBoxMain, displayedImage, SetWindowTitleSuffix)
      {
        Button = MouseButtons.Right
      };
      panAndZoom.UpdateZoomToMiddle(0.6f);

      // Empty input/output image.
      inputImage  = Resources.InitialImage;
      outputImage = Resources.EmptyImage;

      // Modules registry => combo-box.
      foreach (string key in ModuleRegistry.RegisteredModuleNames())
        comboBoxModule.Items.Add(key);
      comboBoxModule.SelectedIndex = 0;
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

    /// <summary>
    /// Sets displayed image.
    /// </summary>
    protected void SetImage (Bitmap newImage)
    {
      if (pictureBoxMain.InvokeRequired)
      {
        SetImageCallback si = new SetImageCallback(SetImage);
        BeginInvoke(si, new object[] { newImage });
      }
      else
      {
        displayedImage = newImage;
        panAndZoom.SetNewImage(displayedImage);
        pictureBoxMain.BackColor = imageBoxBackground;
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
      if (displayedImage == null)
        return;

      x = Util.Clamp(x, 0, displayedImage.Width - 1);
      y = Util.Clamp(y, 0, displayedImage.Height - 1);

      Color c = displayedImage.GetPixel(x, y);
      StringBuilder sb = new StringBuilder();
      sb.AppendFormat(" image[{0},{1}] = ", x, y);
      if (displayedImage.PixelFormat == PixelFormat.Format32bppArgb ||
          displayedImage.PixelFormat == PixelFormat.Format64bppArgb ||
          displayedImage.PixelFormat == PixelFormat.Format16bppArgb1555)
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

      setImage(ref inputImage, inp);
      setImage(ref outputImage, Resources.EmptyImage);

      titleMiddle = " [" + fn + ']';

      recompute();

      return true;
    }

    private void displayImage ()
    {
      if (checkBoxResult.Checked)
      {
        if (outputImage != null)
          SetImage(outputImage);
      }
      else
      {
        if (inputImage != null)
          SetImage(inputImage);
      }
    }

    private void recompute (
      string param = null)
    {
      Bitmap oi = null;

      if (currModule != null &&
          inputImage != null)
      {
        // Set input.
        currModule.SetInput(inputImage);
        if (param != null)
          currModule.Param = param;

        // Recompute.
        currModule.Update();

        // Update result image.
        oi = currModule.GetOutput(0);
      }

      setImage(ref outputImage, oi ?? Resources.EmptyImage);

      // Display input or output image.
      displayImage();
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
        recompute(textBoxParam.Text);
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

    private void pictureBoxMain_Paint (object sender, PaintEventArgs e)
    {
      panAndZoom.OnPaint(e);
    }

    private void pictureBoxMain_MouseDown (object sender, MouseEventArgs e)
    {
      pictureBoxMain.Focus();

      panAndZoom.OnMouseDown(e, imageProbe, e.Button == MouseButtons.Left, ModifierKeys, out Cursor cursor);

      if (cursor != null)
        Cursor = cursor;
    }

    private void pictureBoxMain_MouseUp (object sender, MouseEventArgs e)
    {
      panAndZoom.OnMouseUp(out Cursor cursor);

      Cursor = cursor;
    }

    private void pictureBoxMain_MouseMove (object sender, MouseEventArgs e)
    {
      panAndZoom.OnMouseMove(e, imageProbe, e.Button == MouseButtons.Left, ModifierKeys, out Cursor cursor);

      if (cursor != null)
        Cursor = cursor;
    }

    /// <summary>
    /// Catches mouse wheel movement for zoom in/out of image in picture box
    /// </summary>
    /// <param name="sender">Not needed</param>
    /// <param name="e">Needed for mouse wheel delta value and cursor location</param>
    private void pictureBoxMain_MouseWheel (object sender, MouseEventArgs e)
    {
      panAndZoom.OnMouseWheel(e, ModifierKeys);
    }

    private void pictureBoxMain_PreviewKeyDown (object sender, PreviewKeyDownEventArgs e)
    {
      if (e.KeyCode == Keys.R)
        panAndZoom.Reset();

      panAndZoom.OnKeyDown(e.KeyCode, ModifierKeys);
    }

    private void FormMain_Load (object sender, EventArgs e)
    {
      pictureBoxMain.SizeMode = PictureBoxSizeMode.Zoom;
      pictureBoxMain.MouseWheel += new MouseEventHandler(pictureBoxMain_MouseWheel);
      KeyPreview = true;
    }

    private void buttonModule_Click (object sender, EventArgs e)
    {
      int selectedModule = comboBoxModule.SelectedIndex;
      string moduleName = (string)comboBoxModule.Items[selectedModule];

      currModule = modules.ContainsKey(moduleName)
        ? modules[moduleName]
        : modules[moduleName] = ModuleRegistry.CreateModule(moduleName);

      textBoxParam.Text = currModule.Param;
      tooltip = currModule.Tooltip;
      currModule.GuiWindow = true;
      if (inputImage != null)
        currModule.SetInput(inputImage);
    }

    private void buttonRecompute_Click (object sender, EventArgs e)
    {
      recompute(textBoxParam.Text);
    }
  }
}
