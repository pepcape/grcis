using System;
using System.Collections.Generic;
using System.Drawing;
using Utilities;

namespace Modules
{
  /// <summary>
  /// ModuleFormulaInternal class - template for internal image filter
  /// defined pixel-by-pixel using lambda functions.
  /// </summary>
  public class ModuleFormulaInternal : ModuleFormula
  {
    /// <summary>
    /// Mandatory plain constructor.
    /// </summary>
    public ModuleFormulaInternal ()
    {
      param = "fast";
    }

    /// <summary>
    /// Author's full name (SurnameFirstname).
    /// </summary>
    public override string Author => "PelikanJosef";

    /// <summary>
    /// Name of the module (short enough to fit inside a list-boxes, etc.).
    /// </summary>
    public override string Name => "FormuleInternal";

    /// <summary>
    /// Tooltip for Param (text parameters).
    /// </summary>
    public override string Tooltip =>
      "fast/slow .. fast/slow bitmap access\r" +
      "create .. new image\r" +
      "wid=<width>, hei=<height>";

    //====================================================
    //--- Formula defined directly in this source file ---

    /// <summary>
    /// Defines implicit formulas if available.
    /// </summary>
    /// <returns>null if formulas sould be read from a script.</returns>
    protected override Formula GetFormula ()
    {
      Formula f = new Formula();

      // Text params -> script context.
      // Any global pre-processing is allowed here.
      f.contextCreate = (in Bitmap input, in string param) =>
      {
        if (string.IsNullOrEmpty(param))
          return null;

        Dictionary<string, string> p = Util.ParseKeyValueList(param);

        float coeff = 1.0f;

        // coeff=<float>
        if (Util.TryParse(p, "coeff", ref coeff))
          coeff = Util.Saturate(coeff);

        float freq = 12.0f;

        // freq=<float>
        if (Util.TryParse(p, "freq", ref freq))
          freq = Util.Clamp(freq, 0.01f, 1000.0f);

        Dictionary<string, object> sc = new Dictionary<string, object>();
        sc["coeff"] = coeff;
        sc["freq"] = freq;
        sc["tooltip"] = "coeff=<float> .. swap coefficient (0.0 - no swap, 1.0 - complete swap)\r" +
                        "freq=<float> .. density frequency for image generation (default=12)";

        return sc;
      };

      // R <-> B channel swap with weights.
      f.pixelTransform0 = (
        in ImageContext ic,
        ref float R,
        ref float G,
        ref float B) =>
      {
        float coeff = 0.0f;
        Util.TryParse(ic.context, "coeff", ref coeff);

        float r = Util.Saturate(R * (1.0f - coeff) + B * coeff);
        float b = Util.Saturate(R * coeff          + B * (1.0f - coeff));
        R = r;
        B = b;

        // Output color was modified.
        return true;
      };

      // Test create function: sinc(r^2)
      f.pixelCreate = (
        in ImageContext ic,
        out float R,
        out float G,
        out float B) =>
      {
        // [x, y] in {0, 1]
        double x = ic.x / (double)Math.Max(1, ic.width  - 1);
        double y = ic.y / (double)Math.Max(1, ic.height - 1);

        // I need uniform scale (x-scale == y-scale) with origin at the image center.
        if (ic.width > ic.height)
        {
          // Landscape.
          x -= 0.5;
          y = ic.height * (y - 0.5) / ic.width;
        }
        else
        {
          // Portrait.
          x = ic.width * (x - 0.5) / ic.height;
          y -= 0.5;
        }

        // Custom scales.
        float freq = 12.0f;
        Util.TryParse(ic.context, "freq", ref freq);

        x *= freq;
        y *= freq;

        // Periodic function of r^2.
        double rr = x * x + y * y;
        bool odd = ((int)Math.Round(rr) & 1) > 0;

        // Simple color palette (yellow, blue).
        R = odd ? 0.0f : 1.0f;
        G = R;
        B = 1.0f - R;
      };

      return f;
    }
  }
}
