using System.Collections.Generic;
using System.Windows.Forms;
using Rendering;

namespace _046cameranim
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
  public class RayScene : DefaultRayScene, ITimeDependent
  {
    /// <summary>
    /// Starting (minimal) time in seconds.
    /// </summary>
    public double Start
    {
      get;
      set;
    }

    /// <summary>
    /// Ending (maximal) time in seconds.
    /// </summary>
    public double End
    {
      get;
      set;
    }

    /// <summary>
    /// Current time in seconds.
    /// </summary>
    public double Time
    {
      get;
      set;
    }

    /// <summary>
    /// Creates default ray-rendering scene.
    /// </summary>
    public RayScene ()
    {
      Scenes.FiveBalls( this );
      //Scenes.HedgehogInTheCage( this );
      //Scenes.Flags( this );

      // !!!{{ TODO: put your camera setup code here

      //Camera = new MyAnimationCamera( ... );

      // !!!}}

      // animation support:
      Start = 0.0;
      End   = 10.0;
      Time  = 0.0;
    }
  }
}
