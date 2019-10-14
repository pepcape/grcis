using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using Utilities;

namespace Modules
{
  public class ModuleGlobalHistogram : DefaultRasterModule
  {
    /// <summary>
    /// Mandatory plain constructor.
    /// </summary>
    public ModuleGlobalHistogram ()
    {}

    /// <summary>
    /// Author's full name.
    /// </summary>
    public override string Author => "00pilot";

    /// <summary>
    /// Name of the module (short enough to fit inside a list-boxes, etc.).
    /// </summary>
    public override string Name => "GlobalHistogram";

    /// <summary>
    /// Tooltip for Param (text parameters).
    /// </summary>
    public override string Tooltip => "{ red | green | blue | gray} [sort] [alt]";

    /// <summary>
    /// Default mode - gray.
    /// </summary>
    protected string param = "gray";

    /// <summary>
    /// Current 'Param' string is stored in the module.
    /// Set reasonable initial value.
    /// </summary>
    public override string Param
    {
      get => param;
      set
      {
        if (value != param)
        {
          param = value;
          dirty = true;     // to recompute histogram table

          recompute();
        }
      }
    }

    /// <summary>
    /// Usually read-only, optionally writable (client is defining number of inputs).
    /// </summary>
    public override int InputSlots => 1;

    /// <summary>
    /// Usually read-only, optionally writable (client is defining number of outputs).
    /// </summary>
    public override int OutputSlots => 0;

    /// <summary>
    /// Input raster image.
    /// </summary>
    protected Bitmap inImage = null;

    /// <summary>
    /// True if the histogram needs recomputation.
    /// </summary>
    protected bool dirty = true;

    /// <summary>
    /// Active histogram view window.
    /// </summary>
    protected HistogramForm hForm = null;

    /// <summary>
    /// Interior (visualization) of the hForm.
    /// </summary>
    protected Bitmap hImage = null;

    protected void recompute ()
    {
      if (hForm   != null &&
          inImage != null)
      {
        hImage = new Bitmap(hForm.ClientSize.Width, hForm.ClientSize.Height, PixelFormat.Format24bppRgb);
        if (dirty)
        {
          ImageHistogram.ComputeHistogram(inImage, Param);
          dirty = false;
        }
        ImageHistogram.DrawHistogram(hImage);
        hForm.SetResult(hImage);
      }
    }

    /// <summary>
    /// Returns true if there is an active GUI window associted with this module.
    /// Open/close GUI window using the setter.
    /// </summary>
    public override bool GuiWindow
    {
      get => hForm != null;
      set
      {
        if (value)
        {
          // Show GUI window.
          if (hForm == null)
          {
            hForm = new HistogramForm(this);
            hForm.Show();
          }

          recompute();
        }
        else
        {
          hForm.Hide();
          hForm = null;
        }
      }
    }

    /// <summary>
    /// Assigns an input raster image to the given slot.
    /// Doesn't start computation (see #Update for this).
    /// </summary>
    /// <param name="inputImage">Input raster image (can be null).</param>
    /// <param name="slot">Slot number from 0 to InputSlots-1.</param>
    public override void SetInput (
      Bitmap inputImage,
      int slot = 0)
    {
      dirty   = inImage != inputImage;     // to recompute histogram table
      inImage = inputImage;

      recompute();
    }

    /// <summary>
    /// Recompute the output image[s] according to input image[s].
    /// Blocking (synchronous) function.
    /// #GetOutput() functions can be called after that.
    /// </summary>
    public override void Update ()
    {
      recompute();
    }

    /// <summary>
    /// PixelUpdate() is called after every user interaction.
    /// </summary>
    public override bool HasPixelUpdate => true;

    /// <summary>
    /// Optional action performed at the given pixel.
    /// Blocking (synchronous) function.
    /// Logically equivalent to Update() but with potential local effect.
    /// </summary>
    public override void PixelUpdate (
      int x,
      int y)
    {
      recompute();
    }

    /// <summary>
    /// Notification: form window has closed.
    /// </summary>
    public void OnFormClose ()
    {
      hForm = null;
    }
  }
}
