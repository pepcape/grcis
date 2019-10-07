using System;
using System.Collections.Generic;
using System.Drawing;

namespace Modules
{
  public interface IRasterModule : ICloneable
  {
    string Author { get; }

    string Name { get; }

    /// <summary>
    /// Tooltip for Param (text parameters).
    /// </summary>
    string Tooltip { get; }

    /// <summary>
    /// Param string was changed in the client/caller.
    /// </summary>
    void UpdateParam (string par);

    /// <summary>
    /// Activates/creates a new GUI window.
    /// Resets the input window.
    /// </summary>
    /// <param name="moduleManager">Reference to the module manager.</param>
    void InitWindow (IRasterModuleManager moduleManager);

    /// <summary>
    /// Called after an associated GUI window (the last of associated GUI windows) is closed.
    /// </summary>
    void OnWindowClose ();

    /// <summary>
    /// Returns true if there is at least one active GUI window associted with this module.
    /// </summary>
    bool HasActiveWindow { get; }

    /// <summary>
    /// The input image was changed in the client/caller.
    /// </summary>
    void InputImage (Bitmap inputImage);

    /// <summary>
    /// Action performed at the given pixel.
    /// Non-mandatory.
    /// </summary>
    void PixelAction (int x, int y);

    /// <summary>
    /// Returns current output image.
    /// Can return null if the module doesn't compute an output image.
    /// </summary>
    Bitmap OutputImage ();
  }

  public abstract class DefaultRasterModule : IRasterModule
  {
    public abstract string Author { get; }

    public abstract string Name { get; }

    public virtual string Tooltip => "-- no params --";

    /// <summary>
    /// Param string was changed in the client/caller.
    /// Default behavior: do nothing.
    /// </summary>
    public virtual void UpdateParam (string par)
    {}

    /// <summary>
    /// Activates/creates a new GUI window.
    /// Resets the input image.
    /// Default behavior: windowless module.
    /// </summary>
    /// <param name="moduleManager">Reference to the module manager.</param>
    public virtual void InitWindow (IRasterModuleManager moduleManager)
    {}

    /// <summary>
    /// Called after an associated window (the last of associated windows) is closed.
    /// Default behavior: nothing.
    /// </summary>
    public virtual void OnWindowClose ()
    {}

    /// <summary>
    /// Returns true if there is at least one active GUI window associted with this module.
    /// Default behavior: windowless module.
    /// </summary>
    public virtual bool HasActiveWindow => false;

    // InputImage() is mandatory.

    /// <summary>
    /// Action performed at the given pixel.
    /// Non-mandatory.
    /// Default behavior: no action.
    /// </summary>
    public virtual void PixelAction (int x, int y)
    {}

    /// <summary>
    /// Returns current output image.
    /// Can return null if the module doesn't compute an output image.
    /// </summary>
    public virtual Bitmap OutputImage () => null;

    /// <summary>
    /// The input image was changed in the client/caller.
    /// </summary>
    public abstract void InputImage (Bitmap inputImage);

    public abstract object Clone ();
  }

  public interface IRasterModuleManager
  {
    /// <summary>
    /// [Re-]initializes raster module with the given name.
    /// </summary>
    /// <param name="moduleName"></param>
    void InitModule (string moduleName);

    /// <summary>
    /// [Re-]initializes GUI window of the given module.
    /// </summary>
    void InitWindow (string moduleName);

    /// <summary>
    /// Called after an associated window (the last of associated windows) of the given module is closed.
    /// Default behavior: nothing.
    /// </summary>
    void OnWindowClose (string moduleName);
  }

  public class ModuleRegistry
  {
    /// <summary>
    /// Module.Name => prototype class.
    /// </summary>
    internal static Dictionary<string, IRasterModule> reg = new Dictionary<string, IRasterModule>();

    /// <summary>
    /// Register a new module.
    /// </summary>
    /// <param name="m">Instance of a new module.</param>
    public static void Register (IRasterModule m)
    {
      reg[m.Name] = m;
    }

    public static IRasterModule CreateModule (string name)
    {
      return reg.ContainsKey(name) ? (IRasterModule)reg[name].Clone() : null;
    }

    public static ICollection<string> RegisteredModuleNames ()
    {
      return reg.Keys;
    }
  }
}
