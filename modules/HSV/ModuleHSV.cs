using MathSupport;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using Utilities;

namespace Modules
{
  public class ModuleHSV : DefaultRasterModule
  {
    /// <summary>
    /// Mandatory plain constructor.
    /// </summary>
    public ModuleHSV ()
    {}

    /// <summary>
    /// Author's full name.
    /// </summary>
    public override string Author => "PelikanJosef";

    /// <summary>
    /// Name of the module (short enough to fit inside a list-boxes, etc.).
    /// </summary>
    public override string Name => "HSV";

    /// <summary>
    /// Tooltip for Param (text parameters).
    /// </summary>
    public override string Tooltip => "[dH=<double>][,mulS=<double>][,mulV=<double>][,gamma=<double>]\n... dH is absolute, mS, mV, dGamma relative";

    /// <summary>
    /// Default cell size (width x height).
    /// </summary>
    protected string param = "mulS=1.2,gamma=1.1";

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
    public override int OutputSlots => 1;

    /// <summary>
    /// Input raster image.
    /// </summary>
    protected Bitmap inImage = null;

    /// <summary>
    /// Output raster image.
    /// </summary>
    protected Bitmap outImage = null;

    /// <summary>
    /// Recompute the image.
    /// </summary>
    protected void recompute ()
    {
      if (inImage == null)
        return;

      double dH = 0.0;
      double mS = 1.0;
      double mV = 1.0;
      double gamma = 1.0;

      // 'param' parsing.
      Dictionary<string, string> p = Util.ParseKeyValueList(param);
      if (p.Count > 0)
      {
        // dH=<double> [+- number in degrees]
        Util.TryParse(p, "dH", ref dH);

        // mulS=<double> [relative number .. multiplicator]
        Util.TryParse(p, "mulS", ref mS);

        // mulV=<double> [relative number .. multiplicator]
        Util.TryParse(p, "mulV", ref mV);

        // gamma=<double> [gamma correction .. exponent]
        if (Util.TryParse(p, "gamma", ref gamma))
        {
          // <= 0.0 || 1.0.. nothing
          if (gamma < 0.001)
            gamma = 1.0;
          else
            gamma = 1.0 / gamma;
        }
      }

      int wid = inImage.Width;
      int hei = inImage.Height;

      // Output image must be true-color.
      outImage = new Bitmap(wid, hei, PixelFormat.Format24bppRgb);

      // Convert pixel data:
      int x, y;

#if !FAST

      // Slow GetPixel-SetPixel code.
      for (y = 0; y < hei; y++)
      {
        // !!! TODO: Interrupt handling.
        for (x = 0; x < wid; x++)
        {
          Color ic = inImage.GetPixel(x, y);

          // Conversion to HSV.
          double H, S, V;
          Arith.ColorToHSV(ic, out H, out S, out V);
          // 0 <= H <= 360, 0 <= S <= 1, 0 <= V <= 1

          // HSV transform.
          H = Util.Clamp(H + dH, 0.0, 360.0);
          S = Util.Clamp(S * mS, 0.0, 1.0);
          V = Util.Clamp(V * mV, 0.0, 1.0);

          // Conversion back to RGB.
          double R, G, B;
          Arith.HSVToRGB(H, S, V, out R, out G, out B);
          // [R,G,B] is from [0.0, 1.0]^3

          // Optional gamma correction.
          if (gamma != 1.0)
          {
            // Gamma-correction.
            R = Math.Pow(R, gamma);
            G = Math.Pow(G, gamma);
            B = Math.Pow(B, gamma);
          }

          Color oc = Color.FromArgb(
            Convert.ToInt32(Util.Clamp(R * 255.0, 0.0, 255.0)),
            Convert.ToInt32(Util.Clamp(G * 255.0, 0.0, 255.0)),
            Convert.ToInt32(Util.Clamp(B * 255.0, 0.0, 255.0)));

          outImage.SetPixel(x, y, oc);
        }
      }

#else

      // Fast memory-mapped code.
      PixelFormat iFormat = inImage.PixelFormat;
      if (!PixelFormat.Format24bppRgb.Equals(iFormat) &&
          !PixelFormat.Format32bppArgb.Equals(iFormat) &&
          !PixelFormat.Format32bppPArgb.Equals(iFormat) &&
          !PixelFormat.Format32bppRgb.Equals(iFormat))
        iFormat = PixelFormat.Format24bppRgb;

      int x0, y0;
      int x1, y1;
      BitmapData dataIn  = inImage.LockBits(new Rectangle(0, 0, wid, hei), ImageLockMode.ReadOnly, iFormat);
      BitmapData dataOut = outImage.LockBits(new Rectangle(0, 0, wid, hei), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
      unsafe
      {
        byte* iptr, optr;
        byte r, g, b;
        int sR, sG, sB;
        int pixels;
        int dI = Image.GetPixelFormatSize(iFormat) / 8;
        int dO = Image.GetPixelFormatSize(PixelFormat.Format24bppRgb) / 8;

        for (y0 = 0; y0 < hei; y0 += cellH)
        {
          // !!! TODO: Interrupt handling.

          for (x0 = 0; x0 < wid; x0 += cellW)     // one output cell
          {
            sR = sG = sB = 0;
            x1 = Math.Min(x0 + cellW, wid);
            y1 = Math.Min(y0 + cellH, hei);

            for (y = y0; y < y1; y++)
            {
              iptr = (byte*)dataIn.Scan0 + y * dataIn.Stride + x0 * dI;
              for (x = x0; x < x1; x++, iptr += dI)
              {
                sB += iptr[0];
                sG += iptr[1];
                sR += iptr[2];
              }
            }

            pixels = (x1 - x0) * (y1 - y0);
            r = (byte)((sR + pixels / 2) / pixels);
            g = (byte)((sG + pixels / 2) / pixels);
            b = (byte)((sB + pixels / 2) / pixels);

            for (y = y0; y < y1; y++)
            {
              optr = (byte*)dataOut.Scan0 + y * dataOut.Stride + x0 * dO;
              for (x = x0; x < x1; x++, optr += dO)
              {
                optr[0] = b;
                optr[1] = g;
                optr[2] = r;
              }
            }
          }
        }
      }

      outImage.UnlockBits(dataOut);
      inImage.UnlockBits(dataIn);

#endif
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
      inImage = inputImage;
    }

    /// <summary>
    /// Recompute the output image[s] according to input image[s].
    /// Blocking (synchronous) function.
    /// #GetOutput() functions can be called after that.
    /// </summary>
    public override void Update () => recompute();

    /// <summary>
    /// Returns an output raster image.
    /// Can return null.
    /// </summary>
    /// <param name="slot">Slot number from 0 to OutputSlots-1.</param>
    public override Bitmap GetOutput (
      int slot = 0) => outImage;
  }
}
