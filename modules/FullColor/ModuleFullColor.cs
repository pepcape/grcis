using Raster;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using Utilities;

namespace Modules
{
  public class ModuleFullColor : DefaultRasterModule
  {
    /// <summary>
    /// Mandatory plain constructor.
    /// </summary>
    public ModuleFullColor ()
    {
      // Default cell size (wid x hei).
      param = "wid=4096,hei=4096";
    }

    /// <summary>
    /// Author's full name.
    /// </summary>
    public override string Author => "00pilot";

    /// <summary>
    /// Name of the module (short enough to fit inside a list-boxes, etc.).
    /// </summary>
    public override string Name => "FullColor";

    /// <summary>
    /// Tooltip for Param (text parameters).
    /// </summary>
    public override string Tooltip => "[wid=<width>][,hei=<height>][,slow][,ignore-input][,no-check]";

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
    /// Output message (color check).
    /// </summary>
    protected string message;

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
    public override void Update ()
    {
      // Input image is optional.
      // Starts a new computation.
      UserBreak = false;

      // Default values.
      int wid = 4096;
      int hei = 4096;
      bool fast = true;
      bool ignoreInput = false;
      bool check = true;

      // We are not using 'paramDirty', so the 'Param' string has to be parsed every time.
      Dictionary<string, string> p = Util.ParseKeyValueList(param);
      if (p.Count > 0)
      {
        // wid=<int> [image width in pixels]
        if (Util.TryParse(p, "wid", ref wid))
          wid = Math.Max(1, wid);

        // hei=<int> [image height in pixels]
        if (Util.TryParse(p, "hei", ref hei))
          hei = Math.Max(1, wid);

        // slow ... use Bitmap.SetPixel()
        fast = !p.ContainsKey("slow");

        // ignore-input ... ignore input image even if it is present
        ignoreInput = p.ContainsKey("ignore-input");

        // no-check ... disable color check at the end
        check = !p.ContainsKey("no-check");
      }

      outImage = new Bitmap(wid, hei, PixelFormat.Format24bppRgb);

      // Generate full-color image.
      int xo, yo;
      byte ro, go, bo;

      if (!ignoreInput &&
          inImage != null)
      {
        // Input image is present => use it.

        // Convert pixel data (fast memory-mapped code).
        PixelFormat iFormat = inImage.PixelFormat;
        if (!PixelFormat.Format24bppRgb.Equals(iFormat) &&
            !PixelFormat.Format32bppArgb.Equals(iFormat) &&
            !PixelFormat.Format32bppPArgb.Equals(iFormat) &&
            !PixelFormat.Format32bppRgb.Equals(iFormat))
          iFormat = PixelFormat.Format24bppRgb;

        int width  = inImage.Width;
        int height = inImage.Height;
        int xi, yi;
        BitmapData dataIn  = inImage.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, iFormat);
        BitmapData dataOut = outImage.LockBits(new Rectangle( 0, 0, wid, hei ), ImageLockMode.WriteOnly, outImage.PixelFormat);
        unsafe
        {
          byte* iptr, optr;
          byte ri, gi, bi;
          int dI = Image.GetPixelFormatSize(iFormat) / 8;               // pixel size in bytes
          int dO = Image.GetPixelFormatSize(outImage.PixelFormat) / 8;  // pixel size in bytes

          yi = 0;
          for (yo = 0; yo < hei; yo++)
          {
            // User break handling.
            if (UserBreak)
              break;

            iptr = (byte*)dataIn.Scan0  + yi * dataIn.Stride;
            optr = (byte*)dataOut.Scan0 + yo * dataOut.Stride;

            xi = 0;
            for (xo = 0; xo < wid; xo++)
            {
              // read input colors
              bi = iptr[0];
              gi = iptr[1];
              ri = iptr[2];

              // !!! TODO: do anything with the colors
              bo = bi;
              go = (byte)((gi + xo) & 0xFF);
              ro = (byte)((ri + yo) & 0xFF);

              // write output colors
              optr[0] = bo;
              optr[1] = go;
              optr[2] = ro;

              iptr += dI;
              optr += dO;
              if (++xi >= width)
              {
                xi = 0;
                iptr = (byte*)dataIn.Scan0 + yi * dataIn.Stride;
              }
            }

            if (++yi >= height)
              yi = 0;
          }
        }
        outImage.UnlockBits(dataOut);
        inImage.UnlockBits(dataIn);
      }
      else
      {
        // No input => generate constant full-color image.

        int col;
        if (fast)
        {
          // Generate pixel data (fast memory-mapped code).

          BitmapData dataOut = outImage.LockBits(new Rectangle(0, 0, wid, hei), ImageLockMode.WriteOnly, outImage.PixelFormat);
          unsafe
          {
            byte* optr;
            int dO = Image.GetPixelFormatSize(outImage.PixelFormat) / 8;  // pixel size in bytes

            col = 0;
            for (yo = 0; yo < hei; yo++)
            {
              // User break handling.
              if (UserBreak)
                break;

              optr = (byte*)dataOut.Scan0 + yo * dataOut.Stride;

              for (xo = 0; xo < wid; xo++, col++)
              {
                // !!! TODO: do anything with the input color
                bo = (byte)((col >> 16) & 0xFF);
                go = (byte)((col >> 8) & 0xFF);
                ro = (byte)(col & 0xFF);

                // write output colors
                optr[0] = bo;
                optr[1] = go;
                optr[2] = ro;

                optr += dO;
              }
            }
          }
          outImage.UnlockBits(dataOut);
        }
        else
        {
          // Generate pixel data (slow mode).

          col = 0;
          for (yo = 0; yo < hei; yo++)
          {
            // User break handling.
            if (UserBreak)
              break;

            for (xo = 0; xo < wid; xo++, col++)
            {
              // !!! TODO: do anything with the input color
              bo = (byte)((col >> 16) & 0xFF);
              go = (byte)((col >> 8) & 0xFF);
              ro = (byte)(col & 0xFF);

              // write output colors
              outImage.SetPixel(xo, yo, Color.FromArgb(ro, go, bo));
            }
          }
        }
      }

      // Output message.
      if (check &&
          !UserBreak)
      {
        long colors = Draw.ColorNumber(outImage);
        message = colors == (1 << 24) ? "Colors: 16M, Ok" : $"Colors: {colors}, Fail";
      }
      else
        message = null;
    }

    /// <summary>
    /// Returns an output raster image.
    /// Can return null.
    /// </summary>
    /// <param name="slot">Slot number from 0 to OutputSlots-1.</param>
    public override Bitmap GetOutput (
      int slot = 0) => outImage;

    /// <summary>
    /// Returns an optional output message.
    /// Can return null.
    /// </summary>
    /// <param name="slot">Slot number from 0 to OutputSlots-1.</param>
    public override string GetOutputMessage (
      int slot = 0) => message;
  }
}
