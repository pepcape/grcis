using System.Collections.Generic;
using System.Windows.Forms;
using OpenTK;
using Rendering;

namespace _019raytracing
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
      return new RayTracing( scene );
    }

    /// <summary>
    /// Initialize image synthesizer (responsible for raster image computation).
    /// </summary>
    private IRenderer getRenderer ( IImageFunction imf )
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
    /// Constructs a ray-rendering scene.
    /// </summary>
    public RayScene ()
    {
      //Scenes.FiveBalls( this );
      //Scenes.HedgehogInTheCage( this );
      Scenes.Flags( this );
    }
  }
}
