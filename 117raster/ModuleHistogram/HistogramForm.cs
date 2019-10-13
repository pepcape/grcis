using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;

namespace Modules
{
  public partial class HistogramForm : Form
  {
    /// <summary>
    /// Back-buffer. Resized to current client-size of the form.
    /// </summary>
    protected Bitmap backBuffer = null;

    protected ModuleGlobalHistogram histogramModule;

    public void SetResult (Bitmap result)
    {
      backBuffer = result;
      Invalidate();
    }

    public HistogramForm (ModuleGlobalHistogram hModule)
    {
      histogramModule = hModule;

      InitializeComponent();
    }

    private void HistogramForm_FormClosed (object sender, FormClosedEventArgs e)
    {
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
      if (backBuffer == null ||
          backBuffer.Width != ClientSize.Width ||
          backBuffer.Height != ClientSize.Height)
      {
        histogramModule.Update();
      }
    }
  }
}
