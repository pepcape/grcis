using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using Utilities;

namespace Modules
{
  public class ModuleGlobalHistogram : DefaultRasterModule
  {
    static ModuleGlobalHistogram ()
    {
      ModuleRegistry.Register(new ModuleGlobalHistogram());
    }

    public override string Author { get; } = "0Pilot";

    public override string Name { get; } = "GlobalHistogram";

    /// <summary>
    /// Keywords can be separated by colons.
    /// </summary>
    public override string Tooltip => "{ red | green | blue | gray} [, sort] [, alt]";

    /// <summary>
    /// Current Param value.
    /// </summary>
    protected string currParam = "";

    public void Recompute ()
    {
      if (hForm != null &&
          inImage != null)
      {
        hImage = new Bitmap(hForm.ClientSize.Width, hForm.ClientSize.Height, PixelFormat.Format24bppRgb);
        ImageHistogram.ComputeHistogram(inImage, currParam);
        ImageHistogram.DrawHistogram(hImage);
        hForm.SetResult(hImage);
      }
    }

    /// <summary>
    /// Param string was changed in the client/caller.
    /// </summary>
    public override void UpdateParam (string par)
    {
      if (par != currParam)
      {
        currParam = par;

        Recompute();
      }
    }

    /// <summary>
    /// Input raster image.
    /// </summary>
    protected Bitmap inImage = null;

    /// <summary>
    /// Active histogram view window.
    /// </summary>
    protected HistogramForm hForm = null;

    /// <summary>
    /// Interior (visualization) of the hForm.
    /// </summary>
    protected Bitmap hImage = null;

    /// <summary>
    /// Activates/creates a new window.
    /// Resets the input window.
    /// </summary>
    /// <param name="moduleManager">Reference to the module manager.</param>
    public override void InitWindow (IRasterModuleManager moduleManager)
    {
      if (hForm == null)
      {
        hForm = new HistogramForm(this);
        hForm.Show();
      }

      Recompute();
    }

    /// <summary>
    /// Called after an associated window (the last of associated windows) is closed.
    /// </summary>
    public override void OnWindowClose ()
    {
      if (hForm != null)
      {
        hForm.Hide();
        hForm = null;
      }
    }

    /// <summary>
    /// Returns true if there is at least one active GUI window associted with this module.
    /// </summary>
    public override bool HasActiveWindow => hForm != null;

    /// <summary>
    /// The input image was changed in the client/caller.
    /// </summary>
    public override void InputImage (Bitmap inputImage)
    {
      inImage = inputImage;

      Recompute();
    }

    public override object Clone ()
    {
      ModuleGlobalHistogram me = this;
      return new ModuleGlobalHistogram()
      {
        inImage = me.inImage,
        currParam = me.currParam
      };
    }
  }
}
