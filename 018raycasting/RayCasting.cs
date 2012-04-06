using System.Collections.Generic;
using System.Windows.Forms;
using OpenTK;
using Rendering;

namespace _018raycasting
{
  public partial class Form1 : Form
  {
    /// <summary>
    /// Initialize ray-scene and image function (good enough for single samples).
    /// </summary>
    private IImageFunction getImageFunction ()
    {
      // default constructor of the RayScene .. custom scene
      scene = new RayScene();
      return new RayCasting( scene );
    }

    /// <summary>
    /// Initialize image synthesizer (responsible for raster image computation).
    /// The 'imf' member has been already initialized..
    /// </summary>
    private IRenderer getRenderer ()
    {
      SimpleImageSynthesizer sis = new SimpleImageSynthesizer();
      sis.ImageFunction = imf;
      return sis;
    }
  }
}

namespace Rendering
{
  /// <summary>
  /// Test scene for ray-based rendering.
  /// </summary>
  public class RayScene : DefaultRayScene
  {
    /// <summary>
    /// Constructs ray-rendering scene.
    /// </summary>
    public RayScene ()
    {
      Scenes.FiveBalls( this );
    }
  }
}
