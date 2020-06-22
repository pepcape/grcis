using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utilities;

namespace Rendering
{
  using ScriptContext = Dictionary<string, object>;

  /// <summary>
  /// Delegate used for GUI messaging.
  /// </summary>
  public delegate void StringDelegate (string msg);

  /// <summary>
  /// CSscripting support functions.
  /// </summary>
  public class Scripts
  {
    /// <summary>
    /// Reads additional scenes defined in a command-line arguments.
    /// </summary>
    /// <param name="args">Command-line arguments.</param>
    /// <param name="repo">Existing scene repository to be modified (sceneName -&gt; sceneDelegate | scriptFileName).</param>
    /// <param name="selectedKey">Scene name to be selected (or null).</param>
    /// <returns>How many new scenes were found.</returns>
    public static int ReadFromConfig (
      string[] args,
      ScriptContext repo,
      out string selectedKey)
    {
      // -nodefault               // use it as the first option!
      // [-scene] <sceneFile>     // can be used multiple times
      // -mask <scenePath-mask>   // can be used multiple times
      // -dir <directory>         // can be used multiple times
      // more options to add? (super-sampling factor, output image file-name, output resolution, rendering flags, ..)

      int count = 0;
      selectedKey = null;
      for (int i = 0; i < args.Length; i++)
      {
        if (string.IsNullOrEmpty(args[i]))
          continue;

        string fileName = null;   // file-name to add
        string dir      = null;   // directory to add ('mask' is used)
        string mask     = null;

        if (args[i][0] != '-')
        {
          // <sceneFile.cs>
          if (File.Exists(args[i]))
            fileName = Path.GetFullPath(args[i]);
        }
        else
        {
          string opt = args[i].Substring(1);
          if (opt == "nodefault")
          {
            // -nodefault
            repo.Clear();
          }
          else
          if (opt == "scene" && i + 1 < args.Length)
          {
            // -scene <sceneFile.cs>
            if (File.Exists(args[++i]))
              fileName = Path.GetFullPath(args[i]);
          }
          else
          if (opt == "dir" && i + 1 < args.Length)
          {
            // -dir <directory>
            if (Directory.Exists(args[++i]))
            {
              dir = Path.GetFullPath(args[i]);
              mask = "*.cs";
            }
          }
          else
          if (opt == "mask" && i + 1 < args.Length)
          {
            // -mask <scenePath-mask>
            dir = Path.GetFullPath(args[++i]);
            mask = Path.GetFileName(dir);
            dir = Path.GetDirectoryName(dir);
          }

          // Here new commands could be handled..
          // else if (opt == 'xxx' ..
        }

        if (!string.IsNullOrEmpty(dir) ||
            !string.IsNullOrEmpty(mask))
        {
          // Valid dir & mask.
          try
          {
            string[] search = Directory.GetFiles(dir, mask);
            foreach (string fn in search)
            {
              string path = Path.GetFullPath(fn);
              if (File.Exists(path))
              {
                string key = Path.GetFileName(path);
                if (key.EndsWith(".cs"))
                  key = key.Substring(0, key.Length - 3);

                repo["* " + key] = path;
                count++;
              }
            }
          }
          catch (IOException)
          {
            Console.WriteLine($"Warning: I/O error in dir/mask command: '{dir}'/'{mask}'");
          }
          catch (UnauthorizedAccessException)
          {
            Console.WriteLine($"Warning: access error in dir/mask command: '{dir}'/'{mask}'");
          }

          continue;
        }

        if (!string.IsNullOrEmpty(fileName))
        {
          // Single scene file.
          try
          {
            string path = Path.GetFullPath(fileName);
            if (File.Exists(path))
            {
              string key = Path.GetFileName(path);
              if (key.EndsWith(".cs"))
                key = key.Substring(0, key.Length - 3);

              // The last single file will be selected by default.
              selectedKey = "* " + key;
              repo[selectedKey] = path;
              count++;
            }
          }
          catch (IOException)
          {
            Console.WriteLine($"Warning: I/O error in scene command: '{fileName}'");
          }
          catch (UnauthorizedAccessException)
          {
            Console.WriteLine($"Warning: access error in scene command: '{fileName}'");
          }
        }
      }

      return count;
    }

    /// <summary>
    /// Global variables for the script.
    /// </summary>
    public class Globals
    {
      /// <summary>
      /// Scene object to be filled.
      /// </summary>
      public IRayScene scene;

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
    /// Global script run count.
    /// </summary>
    protected static int count = 0;

    public static bool SceneIsDefined (in ScriptContext ctx)
    {
      return ctx.TryGetValue(PropertyName.CTX_SCENE, out object o) &&
             (o is IRayScene scene) &&
             scene.Intersectable != null;
    }

    public static void SceneReset (in ScriptContext ctx)
    {
      if (ctx.TryGetValue(PropertyName.CTX_SCENE, out object os) &&
          os is IRayScene sc0)
        sc0.Intersectable = null;
    }

    public static void SetScene (
      in ScriptContext ctx,
      IRayScene sc = null)
    {
      ctx[PropertyName.CTX_SCENE] = sc;
    }

    /// <summary>
    /// Initializes the RT-script context before each individual call of 'SceneFromObject'.
    /// </summary>
    /// <param name="ctx">Pre-allocated context map.</param>
    /// <param name="name">Readable short scene name.</param>
    /// <param name="width">Optional output image width in pixels.</param>
    /// <param name="height">Optional output image height in pixels.</param>
    /// <param name="superSampling">Optional super-sampling coefficient.</param>
    /// <param name="minTime">Optional animation start time.</param>
    /// <param name="maxTime">Optional animation finish time.</param>
    /// <param name="fps">Optional animation 'frames-per-second'.</param>
    /// <returns>True if SceneFromObject() should be called.</returns>
    public static bool ContextInit (
      in ScriptContext ctx,
      in string name = "noname",
      in int width = 640,
      in int height = 480,
      in int superSampling = 0,
      in double minTime = 0.0,
      in double maxTime = 10.0,
      in double fps = 25.0)
    {
      Debug.Assert(ctx != null);

      bool hasScene = ctx.ContainsKey(PropertyName.CTX_SCENE) &&
                      ctx[PropertyName.CTX_SCENE] is IRayScene;

#if LOGGING
      Util.Log($"ContextInit(thr={MT.threadID}): preproc={ctx.Count < 2}, scene={hasScene}, defined={SceneIsDefined(ctx)}");
#endif

      // Preprocessing.
      if (ctx.Count < 2)
        ctx[PropertyName.CTX_PREPROCESSING] = true;
      else
        ctx.Remove(PropertyName.CTX_PREPROCESSING);

      // Scene.
      if (!hasScene)
        ctx[PropertyName.CTX_SCENE] = new DefaultRayScene();

      // Scene name.
      ctx[PropertyName.CTX_SCENE_NAME] = name;

      // Resolution.
      ctx[PropertyName.CTX_WIDTH]  = width;
      ctx[PropertyName.CTX_HEIGHT] = height;

      // SuperSampling.
      ctx[PropertyName.CTX_SUPERSAMPLING] = superSampling;

      // Start.
      ctx[PropertyName.CTX_START_ANIM] = minTime;

      // End.
      ctx[PropertyName.CTX_END_ANIM] = maxTime;

      // Fps.
      ctx[PropertyName.CTX_FPS] = fps;

      // Scene definition needed?
      return !SceneIsDefined(ctx);
    }

    /// <summary>
    /// Retrieves standarda data frome the context after calling 'SceneFromObject'.
    /// </summary>
    /// <param name="ctx">Input context map (after running a script).</param>
    /// <param name="imf">IImageFunction implementation if specified in the script.</param>
    /// <param name="rend">IRenderer if specified in the script.</param>
    /// <param name="tooltip">Tool-tip string if defined.</param>
    /// <param name="width">Output image width in pixels.</param>
    /// <param name="height">Output image height in pixels.</param>
    /// <param name="superSampling">Super-sampling coefficient.</param>
    /// <param name="minTime">Animation start time.</param>
    /// <param name="maxTime">Animation finish time.</param>
    /// <param name="fps">Animation fps (frames per second).</param>
    /// <returns>Scene defined in the context.</returns>
    public static IRayScene ContextMining (
      in ScriptContext ctx,
      out IImageFunction imf,
      out IRenderer rend,
      out string tooltip,
      ref int width,
      ref int height,
      ref int superSampling,
      ref double minTime,
      ref double maxTime,
      ref double fps)
    {
      Debug.Assert(ctx != null);

      imf     = null;
      rend    = null;
      tooltip = "";

      // IImageFunction.
      bool hasImf = ctx.TryGetValue(PropertyName.CTX_ALGORITHM, out object o1) &&
                    (imf = o1 as IImageFunction) != null;
      // IRenderer.
      bool hasRend = ctx.TryGetValue(PropertyName.CTX_SYNTHESIZER, out o1) &&
                     (rend = o1 as IRenderer) != null;

#if LOGGING
      Util.Log($"ContextMining(thr={MT.threadID}): imf={hasImf}, rend={hasRend}, defined={SceneIsDefined(ctx)}");
#endif

      // Scene.
      if (!ctx.TryGetValue(PropertyName.CTX_SCENE, out object o) ||
          !(o is IRayScene scene))
        return null;

      // Tooltip.
      if (ctx.TryGetValue(PropertyName.CTX_TOOLTIP, out o1) &&
          o1 is string)
        tooltip = o1 as string;

      // Resolution.
      Util.TryParse(ctx, PropertyName.CTX_WIDTH,  ref width);
      Util.TryParse(ctx, PropertyName.CTX_HEIGHT, ref height);

      // Super-sampling.
      Util.TryParse(ctx, PropertyName.CTX_SUPERSAMPLING, ref superSampling);

      // Start.
      Util.TryParse(ctx, PropertyName.CTX_START_ANIM, ref minTime);

      // End.
      Util.TryParse(ctx, PropertyName.CTX_END_ANIM, ref maxTime);

      // End.
      Util.TryParse(ctx, PropertyName.CTX_FPS, ref fps);

      return scene;
    }

    /// <summary>
    /// Reset the scene object.
    /// </summary>
    public static void SceneInit (IRayScene sc)
    {
      sc.Animator      = null;
      sc.Background    = new DefaultBackground(sc);
      sc.Camera        = null;
      sc.Intersectable = null;
      sc.Sources       = null;
    }

    /// <summary>
    /// Compute a scene based on general description object 'definition' (one of delegate functions or CSscript file-name).
    /// </summary>
    /// <param name="ctx">Context map containing data for the script.</param>
    /// <param name="definition">Script file-name (string) or scene definition function.</param>
    /// <param name="par">Text parameter (from form's text field..).</param>
    /// <param name="defaultScene">Fallback scene definition function.</param>
    /// <param name="message">Message callback function.</param>
    public static void SceneFromObject (
      in ScriptContext ctx,
      in object definition,
      in string par,
      in InitSceneDelegate defaultScene,
      in StringDelegate message = null)
    {
      Debug.Assert(ctx != null);

      bool hasScene = ctx.TryGetValue(PropertyName.CTX_SCENE, out object o) &&
                      o is IRayScene;

#if LOGGING
      Util.Log($"SceneFromObject(thr={MT.threadID}): scene={hasScene}, script={definition as string ?? "---"}");
#endif

      // Scene.
      IRayScene sc;
      if (hasScene)
        sc = o as IRayScene;
      else
        ctx[PropertyName.CTX_SCENE] = sc = new DefaultRayScene();

      // Scene name.
      string name = "noname";
      Util.TryParse(ctx, PropertyName.CTX_SCENE_NAME, ref name);

      // Script file-name.
      string scriptFileName = definition as string;
      if (scriptFileName != null)
        ctx[PropertyName.CTX_SCRIPT_PATH] = scriptFileName;

      string scriptSource = null;

      if (!string.IsNullOrEmpty(scriptFileName) &&
          File.Exists(scriptFileName))
      {
        try
        {
          scriptSource = File.ReadAllText(scriptFileName);
        }
        catch (IOException)
        {
          Console.WriteLine($"Warning: I/O error in scene read: '{scriptFileName}'");
          scriptSource = null;
        }
        catch (UnauthorizedAccessException)
        {
          Console.WriteLine($"Warning: access error in scene read: '{scriptFileName}'");
          scriptSource = null;
        }

        if (!string.IsNullOrEmpty(scriptSource))
        {
          message?.Invoke($"Running scene script '{name}' ({++count})..");

          // interpret the CS-script defining the scene:
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
            "System.Collections.Generic",
            "OpenTK",
            "MathSupport",
            "Rendering",
            "Utilities"
          };

          // Global variables for the script.
          Globals globals = new Globals
          {
            scene     = sc,
            param     = par,
            context   = ctx,
          };

          bool ok = true;
          try
          {
            var task = CSharpScript.RunAsync(
              scriptSource,
              globals: globals,
              options: ScriptOptions.Default.WithReferences(assemblies).AddImports(imports));

            Task.WaitAll(task);
          }
          catch (CompilationErrorException e)
          {
            MessageBox.Show($"Error compiling scene script: {e.Message}, using default scene", "CSscript Error");
            ok = false;
          }
          catch (AggregateException e)
          {
            MessageBox.Show($"Error running scene script: {e.InnerException.Message}, using default scene", "CSscript Error");
            ok = false;
          }

          if (ok)
          {
            // Done.
            message?.Invoke($"Script '{name}' finished ok, rendering..");
            return;
          }
        }

        message?.Invoke("Using default scene..");
        SceneInit(sc);
        defaultScene(sc);
        return;
      }

      // Script file doesn't exist => use delegate function instead.
      if (definition is InitSceneDelegate isd)
        isd(sc);
      else
        if (definition is InitSceneParamDelegate ispd)
          ispd(sc, par);

      message?.Invoke($"Rendering '{name}' ({++count})..");
      return;
    }
  }
}
