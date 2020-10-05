using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Utilities;

namespace Modules
{
  public class ImageContext
  {
    /// <summary>
    /// Horizontal image resolution = number of columns.
    /// </summary>
    public int width;

    /// <summary>
    /// Vertical image resolution = number of rows.
    /// </summary>
    public int height;

    /// <summary>
    /// Horizontal pixel coordinate = column (left-to-right).
    /// </summary>
    public int x;

    /// <summary>
    /// Vertical pixel coordinate = row (top-to-bottom).
    /// </summary>
    public int y;

    public ImageContext (int wid, int hei, int _x = 0, int _y = 0)
    {
      width = wid;
      height = hei;
      x = _x;
      y = _y;
    }

  }

  /// <summary>
  /// All the information about raster-image to raster-image transformation.
  /// </summary>
  public class Formula
  {
    /// <summary>
    /// Pixel transformation formula w/o context.
    /// </summary>
    /// <param name="ic">Image context (pixel position)</param>
    /// <param name="R">Input/output Red.</param>
    /// <param name="G">Input/output Green.</param>
    /// <param name="B">Input/output Blue.</param>
    public delegate void PixelDelegate0 (
      ImageContext ic,
      ref float R,
      ref float G,
      ref float B);

    /// <summary>
    /// If 0, every output pixel depends only on its source pixel.
    /// Values greater than zero are used to define context radius in pixels.
    /// </summary>
    protected int context = 0;

    /// <summary>
    /// If 0, every output pixel depends only on its source pixel.
    /// Values greater than zero are used to define context radius in pixels.
    /// </summary>
    public int Context
    {
      get => context;
      set => context = Math.Max(0, value);
    }

    /// <summary>
    /// Context-free pixel transform function.
    /// Used only if Context == 0.
    /// </summary>
    public PixelDelegate0 pixelTransform0 = (ImageContext ic, ref float R, ref float G, ref float B) => {};

  }

  /// <summary>
  /// RasterModule class.
  /// </summary>
  public class ModuleFormula : DefaultRasterModule
  {
    /// <summary>
    /// Mandatory plain constructor.
    /// </summary>
    public ModuleFormula ()
    {
      // Default cell size (width x height).
      param = "fast,script=Contrast.cs";
    }

    /// <summary>
    /// Author's full name.
    /// </summary>
    public override string Author => "PelikanJosef";

    /// <summary>
    /// Name of the module (short enough to fit inside a list-boxes, etc.).
    /// </summary>
    public override string Name => "Formula";

    /// <summary>
    /// Tooltip for Param (text parameters).
    /// </summary>
    public override string Tooltip => "fast .. fast bitmap access\rscript=<filename> .. CS-script";

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
    /// Output message (script compile/run errors).
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
    /// Returns an output raster image.
    /// Can return null.
    /// </summary>
    /// <param name="slot">Slot number from 0 to OutputSlots-1.</param>
    public override Bitmap GetOutput (
      int slot = 0) => outImage;

    //==========================================
    //--- Formula parsed from CS-script file ---

    /// <summary>
    /// Current (active) formula definition.
    /// </summary>
    protected Formula formula = new Formula();

    /// <summary>
    /// Cuurent CS-script file-name.
    /// </summary>
    protected string fileName;

    /// <summary>
    /// Fast computing (using unsafe direct bitmap access).
    /// </summary>
    protected bool fast = true;

    /// <summary>
    /// Recompute the output image[s] according to input image[s].
    /// Blocking (synchronous) function.
    /// #GetOutput() functions can be called after that.
    /// </summary>
    public override void Update ()
    {
      if (inImage == null)
        return;

      // Used for parameter lookup both here and in the script.
      Dictionary<string, string> p = Util.ParseKeyValueList(param);

      // Smart (lazy) parameter parsing hinted by 'paramDirty'
      if (paramDirty)
      {
        // fast[=<bool>]
        Util.TryParse(p, "fast", ref fast);

        // script=<file-name>
        if (p.TryGetValue("stript", out string fileName))
        {
          //!!! TODO: better source-file lookup !!!
          if (string.IsNullOrEmpty(fileName) ||
              !File.Exists(fileName))
          {
            message = $"Invalid file '{fileName}'";
            return;
          }
        }
      }

      // Script compilation 'fileName' -> 'formula'.
      //!!! TODO: not yet !!!

      if (formula == null ||
          formula.pixelTransform0 == null)
      {
        message = $"Missing formula.pixelTransform0() function ({fileName})";
        return;
      }

      // Transform the source image into output image.
      int wid  = inImage.Width;
      int hei  = inImage.Height;
      outImage = new Bitmap(wid, hei, PixelFormat.Format24bppRgb);

      // Transform pixel data, 1:1 (context==0) mode only.
      int x, y;
      float R, G, B;

      if (fast)
      {
        // Fast memory-mapped code.
        PixelFormat iFormat = inImage.PixelFormat;
        if (!PixelFormat.Format24bppRgb.Equals(iFormat) &&
            !PixelFormat.Format32bppArgb.Equals(iFormat) &&
            !PixelFormat.Format32bppPArgb.Equals(iFormat) &&
            !PixelFormat.Format32bppRgb.Equals(iFormat))
          iFormat = PixelFormat.Format24bppRgb;

        BitmapData dataIn  = inImage.LockBits(new Rectangle(0, 0, wid, hei), ImageLockMode.ReadOnly, iFormat);
        BitmapData dataOut = outImage.LockBits(new Rectangle(0, 0, wid, hei), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
        unsafe
        {
          byte* iptr, optr;
          int dI = Image.GetPixelFormatSize(iFormat) / 8;
          int dO = Image.GetPixelFormatSize(PixelFormat.Format24bppRgb) / 8;

          for (y = 0; y < hei; y++)       // one scanline
          {
            // User break handling.
            if (UserBreak)
              break;

            ImageContext ic = new ImageContext(wid, hei, 0, y);
            iptr = (byte*)dataIn.Scan0 +  y * dataIn.Stride;
            optr = (byte*)dataOut.Scan0 + y * dataOut.Stride;

            for (x = 0; x < wid; x++)     // one output pixel
            {
              B = iptr[0] * (1.0f / 255.0f);
              G = iptr[1] * (1.0f / 255.0f);
              R = iptr[2] * (1.0f / 255.0f);

              ic.x = x;
              formula.pixelTransform0(ic, ref R, ref G, ref B);

              optr[0] = (byte)Util.Clamp((int)Math.Round(B * 255.0f), 0, 255);
              optr[1] = (byte)Util.Clamp((int)Math.Round(G * 255.0f), 0, 255);
              optr[2] = (byte)Util.Clamp((int)Math.Round(R * 255.0f), 0, 255);

              iptr += dI;
              optr += dO;
            }
          }
        }

        outImage.UnlockBits(dataOut);
        inImage.UnlockBits(dataIn);
      }
      else
      {
        // Slow GetPixel-SetPixel code.
        ImageContext ic = new ImageContext(wid, hei);

        for (y = 0; y < hei; y++)
        {
          // User break handling.
          if (UserBreak)
            break;

          ic.y = y;

          for (x = 0; x < wid; x++)
          {
            Color color = inImage.GetPixel(x, y);
            R = color.R * (1.0f / 255.0f);
            G = color.G * (1.0f / 255.0f);
            B = color.B * (1.0f / 255.0f);

            ic.x = x;
            formula.pixelTransform0(ic, ref R, ref G, ref B);

            Color oColor = Color.FromArgb(
              Util.Clamp((int)Math.Round(R * 255.0f), 0, 255),
              Util.Clamp((int)Math.Round(G * 255.0f), 0, 255),
              Util.Clamp((int)Math.Round(B * 255.0f), 0, 255));
            outImage.SetPixel(x, y, oColor);
          }
        }
      }
    }
  }
}
