using Raster;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utilities;

namespace Modules
{
  /// <summary>
  /// Notification handler for async operations.
  /// </summary>
  /// <param name="module"></param>
  public delegate void NotifyHandler (IRasterModule module);

  public interface IRasterModule
  {
    /// <summary>
    /// Author's full name.
    /// </summary>
    string Author { get; }

    /// <summary>
    /// Name of the module (short enough to fit inside a list-boxes, etc.).
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Tooltip for Param (text parameters).
    /// </summary>
    string Tooltip { get; }

    /// <summary>
    /// Dynamic tooltip (can depend on the actual Param: field).
    /// </summary>
    string Tooltip2 { get; }

    /// <summary>
    /// Current 'Param' string is stored in the module.
    /// Set reasonable initial value.
    /// </summary>
    string Param { get; set; }

    /// <summary>
    /// Optional mouse handler: button down
    /// </summary>
    MouseEventHandler MouseDown { get; }

    /// <summary>
    /// Optional mouse handler: button up
    /// </summary>
    MouseEventHandler MouseUp { get; }

    /// <summary>
    /// Optional mouse handler: pointer move
    /// </summary>
    MouseEventHandler MouseMove { get; }

    /// <summary>
    /// Optional mouse handler: wheel turn
    /// </summary>
    MouseEventHandler MouseWheel { get; }

    /// <summary>
    /// Optional keyboard handler
    /// </summary>
    KeyEventHandler KeyDown { get; }

    // Mandatory plain module-instance constructor should look like
    // class Module : IRasterModule
    // {
    //   public Module ()
    //   {
    //     ...
    //   }
    //   ...
    // }

    /// <summary>
    /// Usually read-only, optionally writable (client is defining number of inputs).
    /// </summary>
    int InputSlots { get; set; }

    /// <summary>
    /// Assigns an input raster image to the given slot.
    /// Doesn't start computation (see #Update for this).
    /// </summary>
    /// <param name="inputImage">Input raster image (can be null).</param>
    /// <param name="slot">Slot number from 0 to InputSlots-1.</param>
    void SetInput (
      Bitmap inputImage,
      int slot = 0);

    /// <summary>
    /// Assigns an input HDR image to the given slot.
    /// Doesn't start computation (see #Update for this).
    /// </summary>
    /// <param name="inputImage">Input HDR image (can be null).</param>
    /// <param name="slot">Slot number from 0 to InputSlots-1.</param>
    void SetInput (
      FloatImage inputImage,
      int slot = 0);

    /// <summary>
    /// Recompute the output image[s] according to input image[s].
    /// Blocking (synchronous) function.
    /// #GetOutput() functions can be called after that.
    /// </summary>
    void Update ();

    /// <summary>
    /// Asynchronous recomputing of the output image[s].
    /// </summary>
    /// <param name="notify">Optional notification handler.</param>
    void UpdateAsync (
      NotifyHandler notify = null
      );

    /// <summary>
    /// PixelUpdate() is called after every user interaction.
    /// </summary>
    bool HasPixelUpdate { get; }

    /// <summary>
    /// Optional action performed at the given pixel.
    /// Blocking (synchronous) function.
    /// Logically equivalent to Update() but with potential local effect.
    /// </summary>
    /// <param name="x">Horizontal image coordinate in pixels.</param>
    /// <param name="y">Vertical image coordinate in pixels.</param>
    void PixelUpdate (
      int x,
      int y);

    /// <summary>
    /// Async action performed at the given pixel.
    /// Logically equivalent to UpdateAsync() but with potential local effect.
    /// </summary>
    /// <param name="x">Horizontal image coordinate in pixels.</param>
    /// <param name="y">Vertical image coordinate in pixels.</param>
    /// <param name="notify">Optional notification handler.</param>
    void PixelUpdateAsync (
      int x,
      int y,
      NotifyHandler notify = null);

    /// <summary>
    /// Usually read-only, sometimes writable (client is defining number of outputs).
    /// </summary>
    int OutputSlots { get; set; }

    /// <summary>
    /// Returns an output raster image.
    /// Can return null.
    /// </summary>
    /// <param name="slot">Slot number from 0 to OutputSlots-1.</param>
    Bitmap GetOutput (
      int slot = 0);

    /// <summary>
    /// Returns an optional output message.
    /// Can return null.
    /// </summary>
    /// <param name="slot">Slot number from 0 to OutputSlots-1.</param>
    string GetOutputMessage (
      int slot = 0);

    /// <summary>
    /// Returns an output HDR image.
    /// Can return null.
    /// </summary>
    /// <param name="slot">Slot number from 0 to OutputSlots-1.</param>
    FloatImage GetOutputHDR (
      int slot = 0);

    /// <summary>
    /// Returns true if there is an active GUI window associted with this module.
    /// You can open/close GUI window using the setter.
    /// </summary>
    bool GuiWindow { get; set; }

    /// <summary>
    /// Notification (GUI -> module): GUI window has been closed.
    /// </summary>
    void OnGuiWindowClose ();

    /// <summary>
    /// Notification (GUI -> module): GUI window changed its values (sync GUI -> module is needed).
    /// </summary>
    void OnGuiWindowChanged ();

    /// <summary>
    /// The module is asking to be recomputed by its owner (module -> main).
    /// </summary>
    NotifyHandler UpdateRequest { get; set; }

    /// <summary>
    /// The module is changing the Param string (module -> main).
    /// </summary>
    NotifyHandler ParamUpdated { get; set; }

    /// <summary>
    /// The module is asking to be deactivated (module -> main).
    /// </summary>
    NotifyHandler DeactivateRequest { get; set; }
  }

  public abstract class DefaultRasterModule : IRasterModule
  {
    /// <summary>
    /// Author's full name.
    /// Mandatory override.
    /// </summary>
    public abstract string Author { get; }

    /// <summary>
    /// Name of the module (short enough to fit inside a list-boxes, etc.).
    /// Mandatory override.
    /// </summary>
    public abstract string Name { get; }

    /// <summary>
    /// Tooltip for Param (text parameters).
    /// </summary>
    public virtual string Tooltip => "-- no params --";

    /// <summary>
    /// Dynamic tooltip (can depend on the actual Param: field).
    /// </summary>
    public virtual string Tooltip2 => "";

    /// <summary>
    /// Not-null while an async update (UpdateAsync() or PixelUpdateAsync()) is running.
    /// Guarded by lock (this).
    /// </summary>
    protected Task task;

    /// <summary>
    /// True if 'param' has to be parsed in the next Update*() call.
    /// </summary>
    protected bool paramDirty = true;

    /// <summary>
    /// Current module parameters.
    /// Set reasonable initial value in a derived class.
    /// </summary>
    protected string param = "";

    /// <summary>
    /// Setter logic defines the 'paramDirty' value.
    /// </summary>
    public virtual string Param
    {
      get => param;
      set
      {
        if (value != param)
        {
          param = value;
          paramDirty = true;
        }
      }
    }

    /// <summary>
    /// Optional mouse handler: button down
    /// </summary>
    public virtual MouseEventHandler MouseDown => null;

    /// <summary>
    /// Optional mouse handler: button up
    /// </summary>
    public virtual MouseEventHandler MouseUp => null;

    /// <summary>
    /// Optional mouse handler: pointer move
    /// </summary>
    public virtual MouseEventHandler MouseMove => null;

    /// <summary>
    /// Optional mouse handler: wheel turn
    /// </summary>
    public virtual MouseEventHandler MouseWheel => null;

    /// <summary>
    /// Optional keyboard handler
    /// </summary>
    public virtual KeyEventHandler KeyDown => null;

    /// <summary>
    /// Usually read-only, optionally writable (client is defining number of inputs).
    /// </summary>
    public virtual int InputSlots { get; set; } = 1;

    /// <summary>
    /// Assigns an input raster image to the given slot.
    /// Doesn't start computation (see #Update for this).
    /// </summary>
    /// <param name="inputImage">Input raster image (can be null).</param>
    /// <param name="slot">Slot number from 0 to InputSlots-1.</param>
    public virtual void SetInput (
      Bitmap inputImage,
      int slot = 0)
    {}

    /// <summary>
    /// Assigns an input HDR image to the given slot.
    /// Doesn't start computation (see #Update for this).
    /// </summary>
    /// <param name="inputImage">Input HDR image (can be null).</param>
    /// <param name="slot">Slot number from 0 to InputSlots-1.</param>
    public virtual void SetInput (
      FloatImage inputImage,
      int slot = 0)
    {}

    /// <summary>
    /// Default naive implementation of the user-break.
    /// </summary>
    public static volatile bool UserBreak = false;

    /// <summary>
    /// Recompute the output image[s] according to input image[s].
    /// Blocking (synchronous) function.
    /// #GetOutput() functions can be called after that.
    /// </summary>
    public virtual void Update ()
    {}

    /// <summary>
    /// Asynchronous recomputing of the output image[s].
    /// </summary>
    /// <param name="notify">Optional notification handler.</param>
    public virtual void UpdateAsync (
      NotifyHandler notify = null)
    {
      lock (this)
      {
        if (task == null)
          task = Task.Factory.StartNew(() => Update());

        task.ContinueWith((Task tt) =>
        {
          notify?.Invoke(this);

          lock (this)
            task = null;
        });
      }
    }

    /// <summary>
    /// PixelUpdate() is called after every user interaction.
    /// </summary>
    public virtual bool HasPixelUpdate => false;

    /// <summary>
    /// Optional action performed at the given pixel.
    /// Blocking (synchronous) function.
    /// Logically equivalent to Update() but with potential local effect.
    /// </summary>
    /// <param name="x">Horizontal image coordinate in pixels.</param>
    /// <param name="y">Vertical image coordinate in pixels.</param>
    public virtual void PixelUpdate (
      int x,
      int y)
    {}

    /// <summary>
    /// Async action performed at the given pixel.
    /// Logically equivalent to UpdateAsync() but with potential local effect.
    /// </summary>
    /// <param name="x">Horizontal image coordinate in pixels.</param>
    /// <param name="y">Vertical image coordinate in pixels.</param>
    /// <param name="notify">Optional notification handler.</param>
    public virtual void PixelUpdateAsync (
      int x,
      int y,
      NotifyHandler notify = null)
    {
      lock (this)
      {
        if (task == null)
          task = Task.Factory.StartNew(() => PixelUpdate(x, y));

        task.ContinueWith((Task tt) =>
        {
          notify?.Invoke(this);

          lock (this)
            task = null;
        });
      }
    }

    /// <summary>
    /// Usually read-only, sometimes writable (client is defining number of outputs).
    /// </summary>
    public virtual int OutputSlots { get; set; } = 1;

    /// <summary>
    /// Returns an output raster image.
    /// Can return null.
    /// </summary>
    /// <param name="slot">Slot number from 0 to OutputSlots-1.</param>
    public virtual Bitmap GetOutput (
      int slot = 0) => null;

    /// <summary>
    /// Returns an optional output message.
    /// Can return null.
    /// </summary>
    /// <param name="slot">Slot number from 0 to OutputSlots-1.</param>
    public virtual string GetOutputMessage (
      int slot = 0) => null;

    /// <summary>
    /// Returns an output HDR image.
    /// Can return null.
    /// </summary>
    /// <param name="slot">Slot number from 0 to OutputSlots-1.</param>
    public virtual FloatImage GetOutputHDR (
      int slot = 0) => null;

    /// <summary>
    /// Returns true if there is an active GUI window associted with this module.
    /// You can open/close GUI window using the setter.
    /// </summary>
    public virtual bool GuiWindow { get; set; } = false;

    /// <summary>
    /// Notification (GUI -> module): GUI window has been closed.
    /// </summary>
    public virtual void OnGuiWindowClose ()
    {}

    /// <summary>
    /// Notification (GUI -> module): GUI window changed its values (sync GUI -> module is needed).
    /// </summary>
    public virtual void OnGuiWindowChanged ()
    {}

    /// <summary>
    /// The module is asking to be recomputed by its owner (module -> main).
    /// </summary>
    public virtual NotifyHandler UpdateRequest { get; set; } = null;

    /// <summary>
    /// The module is changing the Param string (module -> main).
    /// </summary>
    public virtual NotifyHandler ParamUpdated { get; set; } = null;

    /// <summary>
    /// The module is asking to be deactivated (module -> main).
    /// </summary>
    public virtual NotifyHandler DeactivateRequest { get; set; } = null;
  }

  public class ModuleRegistry
  {
    /// <summary>
    /// Module.Author + Name => Type.
    /// </summary>
    internal static Dictionary<string, Type> reg = null;

    /// <summary>
    /// Support constant.
    /// </summary>
    internal static readonly Type[] voidArgs = new Type[0];

    /// <summary>
    /// Decorated module name for humans (e.g. used in list-boxes)
    /// </summary>
    /// <param name="rm"></param>
    /// <returns></returns>
    public static string DecoratedModuleName (
      IRasterModule rm) => rm.Author + '-' + rm.Name;

    /// <summary>
    /// Register a new module.
    /// </summary>
    /// <param name="onlyDefault">True for modules inherited from 'DefaultRasterModule'.</param>
    public static void RegisterModules (
      bool onlyDefault = true)
    {
      reg = new Dictionary<string, Type>();
      IEnumerable<Type> compatible = onlyDefault
        ? TypeLoader.GetTypesWithInterface<DefaultRasterModule>()
        : TypeLoader.GetTypesWithInterface<IRasterModule>();

      foreach (Type t in compatible)
      {
        var constructor = t.GetConstructor(voidArgs);
        if (constructor != null)
        {
          if (constructor.Invoke(voidArgs) is IRasterModule rm)
            reg[DecoratedModuleName(rm)] = t;
        }
      }
    }

    public static IRasterModule CreateModule (string name)
    {
      if (reg == null)
        RegisterModules(false);

      if (reg.ContainsKey(name))
      {
        var constructor = reg[name].GetConstructor(voidArgs);
        if (constructor != null)
        {
          if (constructor.Invoke(voidArgs) is IRasterModule rm)
            return rm;
        }
      }

      return null;
    }

    public static ICollection<string> RegisteredModuleNames ()
    {
      if (reg == null)
        RegisterModules(false);

      return reg.Keys;
    }
  }
}
