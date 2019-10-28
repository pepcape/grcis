using System.Drawing;
using System.Windows.Forms;

namespace Modules
{
  public partial class HistogramForm : Form
  {
    /// <summary>
    /// Back-buffer. Resized to current client-size of the form.
    /// </summary>
    protected Bitmap backBuffer = null;

    /// <summary>
    /// Associated raster module (to be notified under various conditions).
    /// </summary>
    protected IRasterModule module;

    public void SetResult (Bitmap result)
    {
      backBuffer?.Dispose();
      backBuffer = result;
      Invalidate();
    }

    public HistogramForm (IRasterModule hModule)
    {
      module = hModule;

      InitializeComponent();
      Show();
    }

    private void HistogramForm_FormClosed (object sender, FormClosedEventArgs e)
    {
      backBuffer?.Dispose();
      backBuffer = null;

      module?.OnGuiWindowClose();
    }

    private void HistogramForm_Paint (object sender, PaintEventArgs e)
    {
      if (backBuffer == null)
        e.Graphics.Clear(Color.White);
      else
        e.Graphics.DrawImageUnscaled(backBuffer, 0, 0);
    }

    private void HistogramForm_Resize (object sender, System.EventArgs e)
    {
      if (module == null)
        return;

      if (backBuffer == null ||
          backBuffer.Width  != ClientSize.Width ||
          backBuffer.Height != ClientSize.Height)
      {
        // This is the correct way to ask the module for recomputation..
        module.UpdateRequest?.Invoke(module);
      }
    }
  }
}
