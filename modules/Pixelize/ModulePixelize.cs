using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Modules
{
  public class ModulePixelize : DefaultRasterModule
  {
    /// <summary>
    /// Mandatory plain constructor.
    /// </summary>
    public ModulePixelize ()
    {
      // Default cell size (width x height).
      param = "12,8";
    }

    /// <summary>
    /// Author's full name.
    /// </summary>
    public override string Author => "PelikanJosef";

    /// <summary>
    /// Name of the module (short enough to fit inside a list-boxes, etc.).
    /// </summary>
    public override string Name => "Pixelize";

    /// <summary>
    /// Tooltip for Param (text parameters).
    /// </summary>
    public override string Tooltip => "<boxw>[,<boxh>] .. box size in pixels";

    /// <summary>
    /// Separator for string parameter.
    /// </summary>
    static readonly char COMMA = ',';

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
      if (inImage == null)
        return;

      // We are not using 'paramDirty', so the 'Param' string has to be parsed every time.
      // Text parameter = cell size.
      int wid = inImage.Width;
      int hei = inImage.Height;
      int cellW = 8;
      int cellH = 8;
      if (param.Length > 0)
      {
        string[] size = param.Split(COMMA);
        if (size.Length > 0)
        {
          if (!int.TryParse(size[0], out cellW) ||
              cellW < 1)
            cellW = 1;

          cellH = cellW;
          if (size.Length > 1)
          {
            if (!int.TryParse(size[1], out cellH))
              cellH = cellW;
            if (cellH < 1)
              cellH = 1;
          }
        }
      }

      outImage = new Bitmap(wid, hei, PixelFormat.Format24bppRgb);

      // convert pixel data:
      int x, y;

#if SLOW

      // slow GetPixel-SetPixel code:
      for (y = 0; y < hei; y++)
      {
        // User break handling.
        if (UserBreak)
          break;

        for (x = 0; x < wid; x++)
        {
          Color ic = inImage.GetPixel(x, y);
          Color oc = Color.FromArgb(255 - ic.R, 255 - ic.G, 255 - ic.B);
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
          // User break handling.
          if (UserBreak)
            break;

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
    /// Returns an output raster image.
    /// Can return null.
    /// </summary>
    /// <param name="slot">Slot number from 0 to OutputSlots-1.</param>
    public override Bitmap GetOutput (
      int slot = 0) => outImage;
  }
}
