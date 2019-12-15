using _117raster.Properties;
using Modules;
using Rendering;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.Text;
using System.Windows.Forms;
using Utilities;

namespace _117raster
{
  public partial class FormMain : Form
  {
    static readonly string rev = Util.SetVersion("$Rev$");

    /// <summary>
    /// Input image read from disk / drag-and-dropped.
    /// null if no input was defined yet (InitialImage is displayed instead).
    /// </summary>
    private Bitmap inputImage = null;

    /// <summary>
    /// Input image's file-name, ends with "*" if it has been computed.
    /// </summary>
    private string inputImageFileName = "";

    /// <summary>
    /// Optional output image for 'input->output' template.
    /// null if there is no output (EmptyImage is displayed instead).
    /// </summary>
    private Bitmap outputImage = null;

    /// <summary>
    /// Window title prefix (basic app title).
    /// </summary>
    private string titlePrefix = "";

    /// <summary>
    /// Middle part of the window title (file-name etc.).
    /// </summary>
    private string titleMiddle = "";

    private void setWindowTitleSuffix (string suffix) => Text = string.IsNullOrEmpty(suffix)
      ? titlePrefix + titleMiddle
      : titlePrefix + titleMiddle + ' ' + suffix;

    /// <summary>
    ///  Enables the 'Save image', 'Set as input' and 'Clear input' buttons.
    /// </summary>
    private void setSaveButton ()
    {
      buttonSaveImage.Enabled = checkBoxResult.Checked
        ? outputImage != null
        : inputImage  != null;

      buttonSetInput.Enabled = checkBoxResult.Checked &&
                               outputImage != null;

      buttonClear.Enabled = inputImage != null;
    }

    /// <summary>
    /// Object responsible for interactive pan-and-zoom (right mouse button, mouse wheel).
    /// </summary>
    private readonly PanAndZoomSupport panAndZoom;

    /// <summary>
    /// Param string tooltip = help.
    /// </summary>
    private string tooltip = "-- no tooltip yet --";

    /// <summary>
    /// Shared ToolTip instance.
    /// </summary>
    private ToolTip tt = new ToolTip();

    /// <summary>
    /// Previously activated modules.
    /// </summary>
    private Dictionary <string, IRasterModule> modules = new Dictionary<string, IRasterModule>();

    /// <summary>
    /// Current active module.
    /// </summary>
    private IRasterModule currModule = null;

    public FormMain ()
    {
      InitializeComponent();

      titlePrefix = Text += " (" + rev + ")";
      setWindowTitleSuffix(" Zoom: 100%");

      // Default PaZ button = Right.
      panAndZoom = new PanAndZoomSupport(pictureBoxMain, Resources.InitialImage, setWindowTitleSuffix)
      {
        Button = MouseButtons.Right
      };
      panAndZoom.UpdateZoomToMiddle();

      // Modules registry => combo-box.
      foreach (string key in ModuleRegistry.RegisteredModuleNames())
        comboBoxModule.Items.Add(key);
      comboBoxModule.SelectedIndex = 0;
    }

    private static void setImage (ref Bitmap bakImage, Bitmap newImage)
    {
      if (newImage != bakImage)
      {
        bakImage?.Dispose();
        bakImage = newImage;
      }
    }

    /// <summary>
    /// Sets GUI elements according to 'running' state.
    /// Must be called inside lock (runLock).
    /// </summary>
    private void setRecomputeGui ()
    {
      // Set GUI elements for computing/non-computing mode.
      comboBoxModule.Enabled =
      checkBoxResult.Enabled =
      buttonLoadImage.Enabled =
      buttonModule.Enabled =
      buttonRecompute.Enabled =
      buttonSetInput.Enabled =
      buttonClear.Enabled =
      buttonShowGUI.Enabled =
      buttonSaveImage.Enabled =
        running == null;

      buttonBreak.Enabled =
        running != null;

      if (inputImage == null)
        buttonClear.Enabled = false;
    }

    delegate void SetImageCallback (Bitmap newImage);

    /// <summary>
    /// Sets new output image (async support).
    /// Not used yet.
    /// </summary>
    private void SetImage (Bitmap newImage)
    {
      if (pictureBoxMain.InvokeRequired)
      {
        SetImageCallback si = new SetImageCallback(SetImage);
        BeginInvoke(si, new object[] { newImage });
      }
      else
        lock (runLock)
        {
          // Finishing phase after a module has recomputed the output image.
          setImage(ref outputImage, newImage);
          checkBoxResult.Checked = newImage != null;
          setSaveButton();

          // Display input or output image.
          displayImage();

          // !!! TODO: this value could be set to Color.White or ColorBlack
          //           for transparent images. Reflect it in API ???
          pictureBoxMain.BackColor = BackColor;

          // Reset the 'running' item.
          running = null;
          setRecomputeGui();
        }
    }

    delegate void SetTextCallback (string text);

    /// <summary>
    /// Sets label text (async support).
    /// </summary>
    /// <param name="text"></param>
    private void SetText (string text)
    {
      if (labelStatus.InvokeRequired)
      {
        SetTextCallback st = new SetTextCallback(SetText);
        BeginInvoke(st, new object[] { text });
      }
      else
        labelStatus.Text = text;
    }

    /// <summary>
    /// Displays pixel coordinates & color in the status line.
    /// </summary>
    private void imageProbe (int x, int y)
    {
      lock (runLock)
      {
        if (running != null)
          return;

        Bitmap image = checkBoxResult.Checked
        ? outputImage
        : inputImage;

        if (image == null)
          return;

        if ((ModifierKeys & Keys.Shift) > 0)
        {
          // Current image info.
          string suffix = checkBoxResult.Checked ? "*" : "";
          SetText($"{inputImageFileName}{suffix} [{image.Width}x{image.Height}]");

          return;
        }

        // Original "image probe": coordinates and color of the picked pixel.
        x = Util.Clamp(x, 0, image.Width - 1);
        y = Util.Clamp(y, 0, image.Height - 1);

        Color c = image.GetPixel(x, y);
        StringBuilder sb = new StringBuilder();
        sb.AppendFormat(" image[{0},{1}] = ", x, y);
        if (image.PixelFormat == PixelFormat.Format32bppArgb ||
            image.PixelFormat == PixelFormat.Format64bppArgb ||
            image.PixelFormat == PixelFormat.Format16bppArgb1555)
          sb.AppendFormat("[{0},{1},{2},{3}] = #{0:X02}{1:X02}{2:X02}{3:X02}", c.R, c.G, c.B, c.A);
        else
          sb.AppendFormat("[{0},{1},{2}] = #{0:X02}{1:X02}{2:X02}", c.R, c.G, c.B);

        SetText(sb.ToString());
      }
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

      inputImageFileName = fn;
      lock (runLock)
      {
        setImage(ref inputImage, inp);
        setImage(ref outputImage, null);

        setSaveButton();
        titleMiddle = " [" + fn + ']';
        SetText($"{fn} [{inp.Width}x{inp.Height}]");
      }

      recompute(false);

      return true;
    }

    private void displayImage ()
    {
      if (checkBoxResult.Checked)
        panAndZoom.SetNewImage(outputImage ?? Resources.EmptyImage);
      else
        panAndZoom.SetNewImage(inputImage ?? Resources.InitialImage);
    }

    class RecomputeTask
    {
      /// <summary>
      /// I'll need to access the form data.
      /// </summary>
      FormMain form;

      /// <summary>
      /// Associated module (to be sure of it's lifespan).
      /// </summary>
      IRasterModule module;

      /// <summary>
      /// Elapsed stop-watch.
      /// </summary>
      Stopwatch sw;

      /// <summary>
      /// Keep references to FormMain and the current module (to be sure).
      /// </summary>
      public RecomputeTask (FormMain f, IRasterModule m)
      {
        form   = f;
        module = m;
      }

      /// <summary>
      /// Starts the module.Update()/PixelUpdate() + support.
      /// </summary>
      public void Start (Bitmap inImage, string newParam, int x = -1, int y = -1)
      {
        // Set input.
        if (module.InputSlots > 0)
          module.SetInput(inImage, 0);

        // Update Param string?
        if (newParam != null)
          module.Param = newParam;

        // Show the optional GUI for the module.
        // !!! TODO: needs check ???
        module.GuiWindow = true;

        // Reset the break system.
        DefaultRasterModule.UserBreak = false;

        // Measure the recompute time.
        sw = new Stopwatch();
        sw.Start();

        // Recompute.
        if (x >= 0 &&
            y >= 0)
          module.PixelUpdateAsync(x, y, Finish);
        else
          module.UpdateAsync(Finish);
      }

      /// <summary>
      /// Finishing procedure.
      /// </summary>
      public void Finish (IRasterModule m)
      {
        // Called as UpdateAsync() finish routine.

        // Elapsed time in milliseconds.
        sw.Stop();
        long elapsed = sw.ElapsedMilliseconds;

        // Optional output message.
        string message = DefaultRasterModule.UserBreak ? "User break!" : module.GetOutputMessage(0);
        form?.SetText(string.Format(CultureInfo.InvariantCulture, "Elapsed: {0:0.000}s{1}",
                      0.001 * elapsed,
                      string.IsNullOrEmpty(message) ? "" : $", {message}"));
        DefaultRasterModule.UserBreak = false;

        // Gui is already visible (if applicable). See Start()
        // !!! TODO: needs check ???
        //module.GuiWindow = true;

        // Update result image.
        Bitmap oi = module.GetOutput(0);

        // Finish the job on the Form side (and discard this RecomputeTask eventually).
        form?.SetImage(oi);
      }
    }

    /// <summary>
    /// Current recomputation or null if nothing is running.
    /// </summary>
    private RecomputeTask running = null;

    /// <summary>
    /// Lock for 'running', 'currModule', 'inputImage', 'outputImage', ...
    /// </summary>
    private object runLock = new object();

    /// <summary>
    /// MT-safe currentModule.
    /// </summary>
    private IRasterModule currModuleSafe ()
    {
      lock (runLock)
        return currModule;
    }

    /// <summary>
    /// Recompute initiated in the FormMain. Optional update of module's param from the textBox.
    /// </summary>
    /// <param name="setParam">If true, module's Param will be modified.</param>
    private void recompute (bool setParam)
    {
      lock (runLock)
      {
        if (running != null)
          return;

        if (currModule == null)
        {
          // Nothing to compute => set outputImage to null.

          setImage(ref outputImage, null);
          checkBoxResult.Checked = false;
          setSaveButton();

          // Display input image.
          displayImage();

          return;
        }

        running = new RecomputeTask(this, currModule);
        setRecomputeGui();
        running.Start(inputImage, setParam ? textBoxParam.Text : null);
      }
    }

    /// <summary>
    /// Called every time left mouse button is pressed WHILE Ctrl key is down
    /// and the current module has positive 'HasPixelUpdate'.
    /// </summary>
    /// <param name="x">Recomputed image coordinate in pixels.</param>
    /// <param name="y">Recomputed image coordinate in pixels.</param>
    private void recomputePixel (
      int x,
      int y)
    {
      lock (runLock)
      {
        if (running != null)
          return;

        if (currModule == null)
        {
          // Nothing to compute => set outputImage to null.

          setImage(ref outputImage, null);
          checkBoxResult.Checked = false;
          setSaveButton();

          // Display input image.
          displayImage();

          return;
        }

        running = new RecomputeTask(this, currModule);
        setRecomputeGui();
        running.Start(inputImage, textBoxParam.Text, x, y);
      }
    }

    private void buttonSaveImage_Click (object sender, EventArgs e)
    {
      Bitmap oi;
      lock (runLock)
        if (running != null ||
            (oi = outputImage) == null)
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
          oi.Save(sfd.FileName, ImageFormat.Png);
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
      if (running != null)
      {
        DefaultRasterModule.UserBreak = true;
        System.Threading.Thread.Sleep(50);
      }
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
        recompute(true);
      }
    }

    private void textBoxParam_MouseHover (object sender, EventArgs e)
    {
      tt.Show(tooltip, (IWin32Window)sender,
              10, -30 - 15 * Util.CharsInString(tooltip, '\n'), 2000);
    }

    private void labelStatus_MouseHover (object sender, EventArgs e)
    {
      tt.Show(Util.TargetFramework + " (" + Util.RunningFramework + ')', (IWin32Window)sender,
              10, -30, 2000);
    }

    private void pictureBoxMain_Paint (object sender, PaintEventArgs e)
    {
      panAndZoom.OnPaint(e);
    }

    /// <summary>
    /// Transforms a mouse event using 'pan-and-zoom' coordinate transform.
    /// </summary>
    private MouseEventArgs transformMouseEvent (
      MouseEventArgs e)
    {
      PointF relative = panAndZoom.GetRelativeCursorLocation(e.X, e.Y);

      return new MouseEventArgs(
          e.Button,
          e.Clicks,
          (int)Math.Round(relative.X),
          (int)Math.Round(relative.Y),
          e.Delta);
    }

    private void pictureBoxMain_MouseDown (object sender, MouseEventArgs e)
    {
      pictureBoxMain.Focus();

      // MT-safe currentModule.
      IRasterModule cm = currModuleSafe();

      bool isCtrl = (ModifierKeys & Keys.Control) > 0;
      bool isModule = cm != null;
      bool hasPixelUpdate = isModule && cm.HasPixelUpdate;
      bool leftButton = e.Button == MouseButtons.Left;
      Cursor cursor;

      if (hasPixelUpdate &&
          isCtrl)
        panAndZoom.OnMouseDown(e, recomputePixel, leftButton,
          ModifierKeys, out cursor);
      else
        panAndZoom.OnMouseDown(e, imageProbe, leftButton,
          ModifierKeys, out cursor);

      // Warning for Ctrl+Mouse without currModule.HasPixelUpdate.
      if (isCtrl &&
          isModule &&
          !hasPixelUpdate)
        SetText($"Current module '{ModuleRegistry.DecoratedModuleName(cm)}' has no PixelUpdate()");

      // Module's mouse handling.
      if (isModule &&
          cm.MouseDown != null)
        cm.MouseDown(sender, transformMouseEvent(e));

      if (cursor != null)
        Cursor = cursor;
    }

    private void pictureBoxMain_MouseUp (object sender, MouseEventArgs e)
    {
      panAndZoom.OnMouseUp(out Cursor cursor);

      // MT-safe currentModule.
      IRasterModule cm = currModuleSafe();

      // Module's mouse handling.
      if (cm != null &&
          cm.MouseUp != null)
        cm.MouseUp(sender, transformMouseEvent(e));

      Cursor = cursor;
    }

    private void pictureBoxMain_MouseMove (object sender, MouseEventArgs e)
    {
      panAndZoom.OnMouseMove(e, imageProbe, e.Button == MouseButtons.Left, ModifierKeys, out Cursor cursor);

      // MT-safe currentModule.
      IRasterModule cm = currModuleSafe();

      // Module's mouse handling.
      if (cm != null &&
          cm.MouseMove != null)
        cm.MouseMove(sender, transformMouseEvent(e));

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

      // MT-safe currentModule.
      IRasterModule cm = currModuleSafe();

      // Module's mouse handling.
      if (cm != null &&
          cm.MouseWheel != null)
        cm.MouseWheel(sender, transformMouseEvent(e));
    }

    private void pictureBoxMain_PreviewKeyDown (object sender, PreviewKeyDownEventArgs e)
    {
      panAndZoom.OnKeyDown(e.KeyCode, ModifierKeys);

      // MT-safe currentModule.
      IRasterModule cm = currModuleSafe();

      // Module's key handling.
      if (cm != null &&
          cm.KeyDown != null)
        cm.KeyDown(sender, new KeyEventArgs(e.KeyData));
    }

    private void FormMain_Load (object sender, EventArgs e)
    {
      pictureBoxMain.SizeMode = PictureBoxSizeMode.Zoom;
      pictureBoxMain.MouseWheel += new MouseEventHandler(pictureBoxMain_MouseWheel);
      KeyPreview = true;
    }

    private void deactivateModule (string moduleName)
    {
      lock (runLock)
      {
        // Deactivate the current module.
        currModule.GuiWindow = false;
        modules.Remove(moduleName);
        currModule = null;
      }

      // Form elements.
      textBoxParam.Text = "";
      tooltip = "";
      buttonModule.Text = "Activate module";
      buttonRecompute.Enabled = false;
      buttonBreak.Enabled = false;
      buttonShowGUI.Enabled = false;
    }

    private void buttonModule_Click (object sender, EventArgs e)
    {
      int selectedModule = comboBoxModule.SelectedIndex;
      string moduleName = (string)comboBoxModule.Items[selectedModule];

      // Is the selected module activated?
      bool activated = modules.ContainsKey(moduleName);

      if (activated)
      {
        // Deactivate the current module.
        deactivateModule(moduleName);
      }
      else
        lock (runLock)
        {
          // Activate the module.
          currModule = modules[moduleName] = ModuleRegistry.CreateModule(moduleName);

          // Notifications module -> main.
          currModule.UpdateRequest = recomputeRequest;
          currModule.ParamUpdated = updateParamFromModule;
          currModule.DeactivateRequest = deactivateRequest;

          // Form elements.
          textBoxParam.Text = currModule.Param;
          tooltip = currModule.Tooltip;
          buttonModule.Text = "Deactivate module";
          buttonShowGUI.Enabled = true;

          currModule.GuiWindow = true;
          if (currModule.InputSlots > 0 &&
              inputImage != null)
            currModule.SetInput(inputImage);

          buttonRecompute.Enabled = true;
          buttonBreak.Enabled = false;
        }
    }

    private void buttonShowGUI_Click (object sender, EventArgs e)
    {
      // MT-safe currentModule.
      IRasterModule cm = currModuleSafe();

      if (cm != null)
        cm.GuiWindow = true;
    }

    private void recomputeRequest (IRasterModule module)
    {
      if (module == currModuleSafe())
        recompute(false);
    }

    private void updateParamFromModule (IRasterModule module)
    {
      // MT-safe currentModule.
      IRasterModule cm = currModuleSafe();

      if (module == cm)
        textBoxParam.Text = cm.Param;
    }

    private void deactivateRequest (IRasterModule module)
    {
      if (module == currModuleSafe())
      {
        int selectedModule = comboBoxModule.SelectedIndex;
        string moduleName = (string)comboBoxModule.Items[selectedModule];
        deactivateModule(moduleName);
      }
    }

    private void buttonRecompute_Click (object sender, EventArgs e)
    {
      recompute(true);
    }

    private void checkBoxResult_CheckedChanged (object sender, EventArgs e)
    {
      lock (runLock)
      {
        displayImage();
        setSaveButton();
      }
    }

    private void comboBoxModule_SelectedIndexChanged (object sender, EventArgs e)
    {
      int selectedModule = comboBoxModule.SelectedIndex;
      string moduleName = (string)comboBoxModule.Items[selectedModule];

      lock (runLock)
      {
        // Backup data of the old module.
        if (currModule != null)
          currModule.Param = textBoxParam.Text;

        // Is the new selected module activated?
        bool activated = modules.ContainsKey(moduleName);

        if (activated)
        {
          // Switch data for the new module.
          currModule = modules[moduleName];

          // Form.
          textBoxParam.Text = currModule.Param;
          tooltip = currModule.Tooltip;
          buttonModule.Text = "Deactivate module";

          currModule.GuiWindow = true;
          if (currModule.InputSlots > 0 &&
              inputImage != null)
            currModule.SetInput(inputImage, 0);
        }
        else
        {
          currModule = null;

          // Form.
          textBoxParam.Text = "";
          tooltip = "";
          buttonModule.Text = "Activate module";
        }

        buttonRecompute.Enabled = activated;
        buttonBreak.Enabled = false;
      }
    }

    private void buttonZoomReset_Click (object sender, EventArgs e)
    {
      panAndZoom.Reset();
    }

    private void buttonSetInput_Click (object sender, EventArgs e)
    {
      if (outputImage == null)
        return;

      inputImageFileName += "*";
      setImage(ref inputImage, outputImage);
      outputImage = null;

      setSaveButton();
      titleMiddle = " [" + inputImageFileName + ']';
      SetText($"{inputImageFileName} [{inputImage.Width}x{inputImage.Height}]");
    }

    private void buttonBreak_Click (object sender, EventArgs e)
    {
      DefaultRasterModule.UserBreak = true;
    }

    private void buttonClear_Click (object sender, EventArgs e)
    {
      if (inputImage == null)
        return;

      inputImageFileName = "";
      lock (runLock)
      {
        setImage(ref inputImage, null);

        displayImage();
        setSaveButton();
        titleMiddle = "";
        SetText("Input cleared.");
      }
    }
  }
}
