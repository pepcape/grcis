using System;
using System.Collections.Generic;
using System.Drawing;

namespace Utilities
{
  public interface IRasterModule : ICloneable
  {
    string Author
    {
      get;
    }

    string Name
    {
      get;
    }

    /// <summary>
    /// Tooltip for Param (text parameters).
    /// </summary>
    string Tooltip
    {
      get;
    }

    /// <summary>
    /// Param string was changed in the client/caller.
    /// </summary>
    void UpdateParam (string par);

    /// <summary>
    /// Activates/creates a new window.
    /// Resets the input window.
    /// </summary>
    void InitWindow ();

    /// <summary>
    /// The input image was changed in the client/caller.
    /// </summary>
    void InputImage (Bitmap inputImage);

    /// <summary>
    /// Action performed at the given pixel.
    /// Non-mandatory.
    /// </summary>
    void Action (int x, int y);

    /// <summary>
    /// Returns current output image.
    /// Can return null if the module doesn't compute an output image.
    /// </summary>
    Bitmap OutputImage ();
  }

  public abstract class DefaultRasterModule
  {
    // Author & Name are mandatory.

    public virtual string Tooltip => "-- no param --";

    /// <summary>
    /// Param string was changed in the client/caller.
    /// Default behavior: do nothing.
    /// </summary>
    public virtual void UpdateParam (string par)
    {}

    /// <summary>
    /// Activates/creates a new window.
    /// Resets the input window.
    /// Default behavior: no module window.
    /// </summary>
    public virtual void InitWindow ()
    {}

    // InputImage() is mandatory.

    /// <summary>
    /// Action performed at the given pixel.
    /// Non-mandatory.
    /// Default behavior: no action.
    /// </summary>
    public virtual void Action (int x, int y)
    {}

    /// <summary>
    /// Returns current output image.
    /// Can return null if the module doesn't compute an output image.
    /// </summary>
    public virtual Bitmap OutputImage () => null;
  }

  public class ModuleRegistry
  {
    /// <summary>
    /// Module.Name => prototype class.
    /// </summary>
    internal static Dictionary<string, IRasterModule> reg = new Dictionary<string, IRasterModule>();

    public static void Register (IRasterModule m)
    {
      reg[m.Name] = m;
    }

    public static IRasterModule CreateModule (string name)
    {
      return reg.ContainsKey(name) ? (IRasterModule)reg[name].Clone() : null;
    }
  }
}
