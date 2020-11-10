using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utilities;

namespace Modules
{
  using ScriptContext = Dictionary<string, object>;

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

    /// <summary>
    /// Script-context shared among all workers.
    /// </summary>
    public readonly ScriptContext context;

    public ImageContext (ScriptContext sc, int wid, int hei, int _x = 0, int _y = 0)
    {
      context = sc;
      width   = wid;
      height  = hei;
      x       = _x;
      y       = _y;
    }

  }

  /// <summary>
  /// All the information about raster-image to raster-image transformation.
  /// </summary>
  public class Formula
  {
    /// <summary>
    /// Script-context initialization from a text field 'Param'.
    /// </summary>
    /// <returns>Can be null.</returns>
    public delegate ScriptContext ScriptContextCreateDelegate (
      in Bitmap input,
      in string param);

    /// <summary>
    /// Pixel creation formula.
    /// </summary>
    /// <param name="ic">Image context (pixel position).</param>
    /// <param name="R">Output Red.</param>
    /// <param name="G">Output Green.</param>
    /// <param name="B">Output Blue.</param>
    public delegate void PixelCreateDelegate (
      in ImageContext ic,
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
      in ImageContext ic,
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
    /// Script-context created from a text field 'param'.
    /// Will be called only once for a picture update.
    /// </summary>
    public ScriptContextCreateDelegate contextCreate = (in Bitmap input, in string param) => null;

    /// <summary>
    /// Pixel creation function.
    /// Ignores Context size.
    /// Creates pixel color stored as RGB in three floats,
    /// pixel coordinates are provided in 'ic'.
    /// </summary>
    public PixelCreateDelegate pixelCreate = (in ImageContext ic, out float R, out float G, out float B) =>
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
    public PixelDelegate0 pixelTransform0 = (in ImageContext ic, ref float R, ref float G, ref float B) =>
      {
        return false;
      };
  }

  /// <summary>
  /// Global variables for formula scripts.
  /// </summary>
  public class Globals
  {
    /// <summary>
    /// Formula lambdas to define.
    /// </summary>
    public Formula formula;

    /// <summary>
    /// Optional text parameter (usually from form's 'Params:' field).
    /// </summary>
    public string param;

    /// <summary>
    /// Parameter map for passing values in/out of the script.
    /// </summary>
    public ScriptContext context;
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
      // Default behavior (fast mode, script name).
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

    protected string tooltip2 = "";

    /// <summary>
    /// Specific tooltip (script-dependent).
    /// </summary>
    public override string Tooltip2 => tooltip2;

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

    /// <summary>
    /// Returns an optional output message.
    /// Can return null.
    /// </summary>
    /// <param name="slot">Slot number from 0 to OutputSlots-1.</param>
    public override string GetOutputMessage (
      int slot = 0) => message;

    //==========================================
    //--- Formula parsed from CS-script file ---

    /// <summary>
    /// Defines implicit formulas if available.
    /// </summary>
    /// <returns>null if formulas should be read from a script.</returns>
    protected virtual Formula GetFormula ()
    {
      return null;
    }

    /// <summary>
    /// Current (active) formula definition.
    /// </summary>
    protected Formula formula = new Formula();

    /// <summary>
    /// Current CS-script file-name.
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
    /// File-name of the script. Can be empty for testing.
    /// </summary>
    protected string scriptFileName = "";

    /// <summary>
    /// Current script file content (to prevent redundant script compiling).
    /// </summary>
    protected string scriptSource = "";

    /// <summary>
    /// Returns the actual script file path if found, null otherwise.
    /// </summary>
    protected static string sourceLookup (
      string sourceFile)
    {
      if (File.Exists(sourceFile))
        return sourceFile;

      for (int i = 0; i++ < 2;)
      {
        sourceFile = Path.Combine("..", sourceFile);
        if (File.Exists(sourceFile))
          return sourceFile;
      }

      return null;
    }

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
        if (!Util.TryParse(p, "create", ref create))
          create = false;

        // wid=<int>
        if (Util.TryParse(p, "wid", ref width))
          width = Util.Clamp(width, 1, 16384);

        // hei=<int>
        if (Util.TryParse(p, "hei", ref height))
          height = Util.Clamp(height, 1, 16384);

        fast = true;
        // fast[=<bool>]
        Util.TryParse(p, "fast", ref fast);

        // slow[=<bool>]
        if (Util.TryParse(p, "slow", ref fast))
          fast = !fast;

        // script=<file-name>
        if (p.TryGetValue("script", out string fileName))
        {
          if (string.IsNullOrEmpty(fileName) ||
              string.IsNullOrEmpty(sourceLookup(fileName)))
          {
            message = $"Invalid file '{fileName}'";
            return;
          }

          scriptFileName = fileName;
        }
      }

      float coeff = 1.0f;

      // coeff=<float>
      if (Util.TryParse(p, "coeff", ref coeff))
        coeff = Util.Saturate(coeff);

      // Check the input image (only for non-create mode).
      if (!create)
      {
        if (inImage == null)
          return;

        width  = inImage.Width;
        height = inImage.Height;
      }

      // Actual file name (including the correct path).
      string scriptPath;

      // Trying internal formulas (for override classes).
      Formula newFormula = GetFormula();

      if (newFormula != null)
        formula = newFormula;

      else

      // Script compilation 'fileName' -> 'formula'.
      if (!string.IsNullOrEmpty(scriptFileName) &&
          !string.IsNullOrEmpty(scriptPath = sourceLookup(scriptFileName)))
      {
        string newSource = null;
        bool ok = true;
        message = "";

        // Measure the script compile+run time.
        Stopwatch sw = new Stopwatch();
        sw.Start();

        try
        {
          newSource = File.ReadAllText(scriptPath);
        }
        catch (IOException)
        {
          message = $"Warning: I/O error in script read: '{scriptFileName}'";
          newSource = null;
        }
        catch (UnauthorizedAccessException)
        {
          message = $"Warning: access error in scene read: '{scriptFileName}'";
          newSource = null;
        }

        if (!string.IsNullOrEmpty(newSource) &&
            newSource != scriptSource)
        {
          // Interpret the CS-script defining the scene:
          var assemblyNames = Assembly.GetExecutingAssembly().GetReferencedAssemblies();

          List<Assembly> assemblies = new List<Assembly>();
          assemblies.Add(Assembly.GetExecutingAssembly());
          foreach (var assemblyName in assemblyNames)
            assemblies.Add(Assembly.Load(assemblyName));

          // Standard usings = imports.
          List<string> imports = new List<string>
          {
            "System",
            "System.Diagnostics",
            "System.Drawing",
            "System.Collections.Generic",
            "OpenTK",
            "MathSupport",
            "Raster",
            "Utilities",
            "Modules"
          };

          // Reset values for the script.
          ScriptContext ctx = new ScriptContext();
          //!!! TODO: set 'ctx' values if needed ???
          formula = new Formula();

          // Global variables for the script.
          Globals globals = new Globals
          {
            formula = formula,
            param   = param,
            context = ctx,
          };

          // Compile & run the script.
          try
          {
            var task = CSharpScript.RunAsync(
            newSource,
            globals: globals,
            options: ScriptOptions.Default.WithReferences(assemblies).AddImports(imports));

            Task.WaitAll(task);
          }
          catch (CompilationErrorException e)
          {
            MessageBox.Show($"Error compiling script: {e.Message}, using defaults", "CSscript Error");
            ok = false;
          }
          catch (AggregateException e)
          {
            MessageBox.Show($"Error running script: {e.InnerException.Message}, using defaults", "CSscript Error");
            ok = false;
          }
        }

        if (ok)
        {
          long elapsed = sw.ElapsedMilliseconds;
          message = $"Script '{scriptFileName}' finished ok ({elapsed}ms)";
          scriptSource = newSource;
        }
        else
        {
          // Defaults.
          formula = new Formula();
          scriptSource = "";
        }
      }

      if (formula == null ||
          ( create && formula.pixelCreate == null) ||
          (!create && formula.pixelTransform0 == null))
      {
        message = $"Missing formula.pixelTransform0/pixelCreate() function ({fileName})";
        return;
      }

      // Transform the source image into output image.
      outImage = new Bitmap(width, height, PixelFormat.Format24bppRgb);

      // Shared script-context.
      ScriptContext sc = formula.contextCreate?.Invoke(inImage, param) ?? new ScriptContext();

      // Update specific tooltip2.
      if (!Util.TryParse(sc, "tooltip", ref tooltip2))
        tooltip2 = "";

      // Transform/create pixel data, 1:1 (context==0) mode only.

      // Create a new image.
      if (create)
      {
        if (fast)
        {
          // Fast memory-mapped code.
          BitmapData dataOut = outImage.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
          unsafe
          {
            int dO = Image.GetPixelFormatSize(PixelFormat.Format24bppRgb) / 8;

            void inner (int y)
            {
              // User break handling.
              if (UserBreak)
                return;

              ImageContext ic = new ImageContext(sc, width, height, 0, y);
              byte* optr = (byte*)dataOut.Scan0 + y * dataOut.Stride;

              for (int x = 0; x < width; x++)     // one output pixel
              {
                ic.x = x;
                formula.pixelCreate(ic, out float R, out float G, out float B);

                optr[0] = Convert.ToByte(255.0f * Util.Saturate(B));
                optr[1] = Convert.ToByte(255.0f * Util.Saturate(G));
                optr[2] = Convert.ToByte(255.0f * Util.Saturate(R));

                optr += dO;
              }
            }

            Parallel.For(0, height, inner);
          }

          outImage.UnlockBits(dataOut);
        }
        else
        {
          // Slow SetPixel code.
          ImageContext ic = new ImageContext(sc, width, height);

          for (int y = 0; y < height; y++)
          {
            // User break handling.
            if (UserBreak)
              break;

            ic.y = y;

            for (int x = 0; x < width; x++)
            {
              ic.x = x;
              formula.pixelCreate(ic, out float R, out float G, out float B);

              Color oColor = Color.FromArgb(
                Convert.ToInt32(255.0f * Util.Saturate(R)),
                Convert.ToInt32(255.0f * Util.Saturate(G)),
                Convert.ToInt32(255.0f * Util.Saturate(B)));

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
          int dI = Image.GetPixelFormatSize(iFormat) / 8;
          int dO = Image.GetPixelFormatSize(PixelFormat.Format24bppRgb) / 8;

          void inner (int y)
          {
            // User break handling.
            if (UserBreak)
              return;

            ImageContext ic = new ImageContext(sc, width, height, 0, y);
            byte* iptr = (byte*)dataIn.Scan0  + y * dataIn.Stride;
            byte* optr = (byte*)dataOut.Scan0 + y * dataOut.Stride;

            for (int x = 0; x < width; x++)     // one output pixel
            {
              float B = iptr[0] * (1.0f / 255.0f);
              float G = iptr[1] * (1.0f / 255.0f);
              float R = iptr[2] * (1.0f / 255.0f);

              ic.x = x;
              if (formula.pixelTransform0(ic, ref R, ref G, ref B))
              {
                optr[0] = Convert.ToByte(255.0f * Util.Saturate(B));
                optr[1] = Convert.ToByte(255.0f * Util.Saturate(G));
                optr[2] = Convert.ToByte(255.0f * Util.Saturate(R));
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

          Parallel.For(0, height, inner);
        }

        outImage.UnlockBits(dataOut);
        inImage.UnlockBits(dataIn);
      }
      else
      {
        // Slow GetPixel-SetPixel code.
        ImageContext ic = new ImageContext(sc, width, height);

        for (int y = 0; y < height; y++)
        {
          // User break handling.
          if (UserBreak)
            break;

          ic.y = y;

          for (int x = 0; x < width; x++)
          {
            Color color = inImage.GetPixel(x, y);
            float R = color.R * (1.0f / 255.0f);
            float G = color.G * (1.0f / 255.0f);
            float B = color.B * (1.0f / 255.0f);

            ic.x = x;
            if (formula.pixelTransform0(ic, ref R, ref G, ref B))
            {
              Color oColor = Color.FromArgb(
                Convert.ToInt32(255.0f * Util.Saturate(R)),
                Convert.ToInt32(255.0f * Util.Saturate(G)),
                Convert.ToInt32(255.0f * Util.Saturate(B)));

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
