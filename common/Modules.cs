using System;
using System.Collections.Generic;
using System.Drawing;

namespace Utilities
{
  public interface IModule : ICloneable
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
    /// Param string was changed in the client/caller.
    /// </summary>
    /// <param name="par"></param>
    void UpdateParam (string par);

    /// <summary>
    /// Activates/creates a new window.
    /// </summary>
    void InitWindow ();

    /// <summary>
    /// The input image was changed in the client/caller.
    /// </summary>
    /// <param name="inputImage"></param>
    void InputImage (Bitmap inputImage);

    /// <summary>
    /// Returns current output image.
    /// </summary>
    /// <returns></returns>
    Bitmap OutputImage ();
  }

  public class ModuleRegistry
  {
    /// <summary>
    /// Module.Name => prototype class.
    /// </summary>
    internal static Dictionary<string, IModule> reg = new Dictionary<string, IModule>();

    public static void Register (IModule m)
    {
      reg[m.Name] = m;
    }

    public static IModule CreateModule (string name)
    {
      return reg.ContainsKey(name) ? (IModule)reg[name].Clone() : null;
    }
  }
}
