using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using MathSupport;

namespace Rendering
{
  [Serializable]
  public class RenderingProgress : Progress
  {
    protected long lastSync = 0L;

    [NonSerialized]
    public IRenderProgressForm form;

    public RenderingProgress(IRenderProgressForm form)
    {
      this.form = form;
    }

    public void Reset ()
    {
      lastSync = 0L;
    }

    public override void Sync (object msg)
    {
      long now = form.GetStopwatch().ElapsedMilliseconds;

      if (now - lastSync < SyncInterval)
        return;

      lastSync = now;
      form.SetText(string.Format(CultureInfo.InvariantCulture, "{0:f1}%:  {1:f1}s",
                                 100.0f * Finished, 1.0e-3 * now));
      Bitmap b = msg as Bitmap;
      if (b != null)
      {
        Bitmap nb;
        lock (b)
          nb = new Bitmap(b);
        form.SetImage(nb);
      }
    }
  }

  public interface IRenderProgressForm
  {
    Stopwatch GetStopwatch ();
    void SetImage (Bitmap bitmap);
    void SetText (string text);
  }
}
