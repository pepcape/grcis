using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.IO;
using System.Diagnostics;
using OpenTK;

namespace Rendering
{
  /// <summary>
  /// Ray-casting rendering (ray-tracing w/o all secondary rays).
  /// </summary>
  public class RayCasting : IImageFunction
  {
    /// <summary>
    /// Domain width.
    /// </summary>
    public double Width
    {
      get { return scene.Camera.Width; }
      set { scene.Camera.Width = value; }
    }

    /// <summary>
    /// Domain height.
    /// </summary>
    public double Height
    {
      get { return scene.Camera.Height; }
      set { scene.Camera.Height = value; }
    }

    protected IRayScene scene;

    public RayCasting ( IRayScene sc )
    {
      scene = sc;
    }

    /// <summary>
    /// Computes one image sample.
    /// </summary>
    /// <param name="x">Horizontal coordinate.</param>
    /// <param name="y">Vertical coordinate.</param>
    /// <param name="color">Computed sample color.</param>
    /// <returns>Hash-value used for adaptive subsampling.</returns>
    public long GetSample ( double x, double y, double[] color )
    {
      return 0L;
    }

  }

}
