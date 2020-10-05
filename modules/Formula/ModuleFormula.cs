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
      width  = wid;
      height = hei;
      x      = _x;
      y      = _y;
    }

  }

  /// <summary>
  /// All the information about raster-image to raster-image transformation.
  /// </summary>
  public class Formula
  {
    /// <summary>
    /// Pixel creation formula.
    /// </summary>
    /// <param name="ic">Image context (pixel position).</param>
    /// <param name="R">Output Red.</param>
    /// <param name="G">Output Green.</param>
    /// <param name="B">Output Blue.</param>
    public delegate void PixelCreateDelegate (
      ImageContext ic,
      out float R,
      out float G,
      out float B);

    /// <summary>
    /// Pixel transformation formula w/o context.
    /// </summary>
    /// <param name="ic">Image context (pixel position).</param>
    /// <param name="R">Input/output Red.</param>
    /// <param name="G">Input/output Green.</param>
    /// <param name="B">Input/output Blue.</param>
    /// <returns>True if the pixel was changed.</returns>
    public delegate bool PixelDelegate0 (
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
    /// Pixel creation function.
    /// Ignores Context size.
    /// Creates pixel color stored as RGB in three floats,
    /// pixel coordinates are provided in 'ic'.
    /// </summary>
    public PixelCreateDelegate pixelCreate = (ImageContext ic, out float R, out float G, out float B) =>
      {
        R = ic.x / (float)Math.Max(1, ic.width  - 1);
        B = ic.y / (float)Math.Max(1, ic.height - 1);
        G = 0.5f * (R + B);
      };

    /// <summary>
    /// Context-free pixel transform function.
    /// Used only if Context == 0.
    /// Recomputes pixel color stored as RGB in three floats,
    /// input color needs not to be changed.
    /// </summary>
    public PixelDelegate0 pixelTransform0 = (ImageContext ic, ref float R, ref float G, ref float B) =>
      {
        return false;
      };

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
    public override string Tooltip =>
      "fast/slow .. fast/slow bitmap access\r" +
      "create .. new image\r" +
      "wid=<width>, hei=<height>\r" +
      "script=<filename> .. CS-script";

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
    /// If true, 'pixelCreate' is called and input image is ignored.
    /// </summary>
    protected bool create = false;

    /// <summary>
    /// Image width in pixels for the 'create' mode.
    /// </summary>
    protected int width = 256;

    /// <summary>
    /// Image height in pixels for the 'create' mode.
    /// </summary>
    protected int height = 256;

    /// <summary>
    /// Recompute the output image[s] according to input image[s].
    /// Blocking (synchronous) function.
    /// #GetOutput() functions can be called after that.
    /// </summary>
    public override void Update ()
    {
      // Used for parameter lookup both here and in the script.
      Dictionary<string, string> p = Util.ParseKeyValueList(param);

      // Smart (lazy) parameter parsing hinted by 'paramDirty'
      if (paramDirty)
      {
        // create[=<bool>]
        Util.TryParse(p, "create", ref create);

        // wid=<int>
        if (Util.TryParse(p, "wid", ref width))
          width = Util.Clamp(width, 1, 16384);

        // hei=<int>
        if (Util.TryParse(p, "hei", ref height))
          height = Util.Clamp(height, 1, 16384);

        // fast[=<bool>]
        Util.TryParse(p, "fast", ref fast);

        // slow[=<bool>]
        if (Util.TryParse(p, "slow", ref fast))
          fast = !fast;

        // script=<file-name>
        if (p.TryGetValue("script", out string fileName))
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

      // Check the input image (only for non-create mode).
      if (!create)
      {
        if (inImage == null)
          return;

        width  = inImage.Width;
        height = inImage.Height;
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
      outImage = new Bitmap(width, height, PixelFormat.Format24bppRgb);

      // Transform/create pixel data, 1:1 (context==0) mode only.
      int x, y;
      float R, G, B;

      // Create a new image.
      if (create)
      {
        if (fast)
        {
          // Fast memory-mapped code.
          BitmapData dataOut = outImage.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
          unsafe
          {
            byte* optr;
            int dO = Image.GetPixelFormatSize(PixelFormat.Format24bppRgb) / 8;

            for (y = 0; y < height; y++)       // one scanline
            {
              // User break handling.
              if (UserBreak)
                break;

              ImageContext ic = new ImageContext(width, height, 0, y);
              optr = (byte*)dataOut.Scan0 + y * dataOut.Stride;

              for (x = 0; x < width; x++)     // one output pixel
              {
                ic.x = x;
                formula.pixelCreate(ic, out R, out G, out B);

                optr[0] = (byte)Util.Clamp((int)Math.Round(B * 255.0f), 0, 255);
                optr[1] = (byte)Util.Clamp((int)Math.Round(G * 255.0f), 0, 255);
                optr[2] = (byte)Util.Clamp((int)Math.Round(R * 255.0f), 0, 255);

                optr += dO;
              }
            }
          }

          outImage.UnlockBits(dataOut);
        }
        else
        {
          // Slow GetPixel-SetPixel code.
          ImageContext ic = new ImageContext(width, height);

          for (y = 0; y < height; y++)
          {
            // User break handling.
            if (UserBreak)
              break;

            ic.y = y;

            for (x = 0; x < width; x++)
            {
              ic.x = x;
              formula.pixelCreate(ic, out R, out G, out B);

              Color oColor = Color.FromArgb(
                Util.Clamp((int)Math.Round(R * 255.0f), 0, 255),
                Util.Clamp((int)Math.Round(G * 255.0f), 0, 255),
                Util.Clamp((int)Math.Round(B * 255.0f), 0, 255));

              outImage.SetPixel(x, y, oColor);
            }
          }
        }
        return;
      }

      // Transform the 'inImage'.
      if (fast)
      {
        // Fast memory-mapped code.
        PixelFormat iFormat = inImage.PixelFormat;
        if (!PixelFormat.Format24bppRgb.Equals(iFormat) &&
            !PixelFormat.Format32bppArgb.Equals(iFormat) &&
            !PixelFormat.Format32bppPArgb.Equals(iFormat) &&
            !PixelFormat.Format32bppRgb.Equals(iFormat))
          iFormat = PixelFormat.Format24bppRgb;

        BitmapData dataIn  = inImage.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, iFormat);
        BitmapData dataOut = outImage.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
        unsafe
        {
          byte* iptr, optr;
          int dI = Image.GetPixelFormatSize(iFormat) / 8;
          int dO = Image.GetPixelFormatSize(PixelFormat.Format24bppRgb) / 8;

          for (y = 0; y < height; y++)       // one scanline
          {
            // User break handling.
            if (UserBreak)
              break;

            ImageContext ic = new ImageContext(width, height, 0, y);
            iptr = (byte*)dataIn.Scan0 +  y * dataIn.Stride;
            optr = (byte*)dataOut.Scan0 + y * dataOut.Stride;

            for (x = 0; x < width; x++)     // one output pixel
            {
              B = iptr[0] * (1.0f / 255.0f);
              G = iptr[1] * (1.0f / 255.0f);
              R = iptr[2] * (1.0f / 255.0f);

              ic.x = x;
              if (formula.pixelTransform0(ic, ref R, ref G, ref B))
              {
                optr[0] = (byte)Util.Clamp((int)Math.Round(B * 255.0f), 0, 255);
                optr[1] = (byte)Util.Clamp((int)Math.Round(G * 255.0f), 0, 255);
                optr[2] = (byte)Util.Clamp((int)Math.Round(R * 255.0f), 0, 255);
              }
              else
              {
                optr[0] = iptr[0];
                optr[1] = iptr[1];
                optr[2] = iptr[2];
              }

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
        ImageContext ic = new ImageContext(width, height);

        for (y = 0; y < height; y++)
        {
          // User break handling.
          if (UserBreak)
            break;

          ic.y = y;

          for (x = 0; x < width; x++)
          {
            Color color = inImage.GetPixel(x, y);
            R = color.R * (1.0f / 255.0f);
            G = color.G * (1.0f / 255.0f);
            B = color.B * (1.0f / 255.0f);

            ic.x = x;
            if (formula.pixelTransform0(ic, ref R, ref G, ref B))
            {
              Color oColor = Color.FromArgb(
                Util.Clamp((int)Math.Round(R * 255.0f), 0, 255),
                Util.Clamp((int)Math.Round(G * 255.0f), 0, 255),
                Util.Clamp((int)Math.Round(B * 255.0f), 0, 255));

              outImage.SetPixel(x, y, oColor);
            }
            else
              outImage.SetPixel(x, y, color);
          }
        }
      }
    }
  }
}
