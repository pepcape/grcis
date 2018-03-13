// Author: Josef Pelikan

namespace Rendering
{
  public class FormSupport
  {
    /// <summary>
    /// Initialize ray-scene and image function (good enough for single samples).
    /// </summary>
    public static IImageFunction getImageFunction ( out IRayScene scene )
    {
      // default constructor of the RayScene .. custom scene
      scene = new RayScene();
      return new RayCasting( scene );
    }

    /// <summary>
    /// Initialize image synthesizer (responsible for raster image computation).
    /// </summary>
    public static IRenderer getRenderer ( IImageFunction imf )
    {
      SimpleImageSynthesizer sis = new SimpleImageSynthesizer();
      sis.ImageFunction = imf;
      return sis;
    }
  }

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
      //Scenes.HedgehogInTheCage( this );
      //Scenes.Flags( this );
    }
  }
}
