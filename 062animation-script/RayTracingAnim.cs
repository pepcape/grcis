using OpenTK;
using Rendering;
using System.Collections.Generic;
using System.Diagnostics;

namespace _062animation
{
  public class FormSupport
  {
    /// <summary>
    /// Initialize rendering parameters.
    /// </summary>
    public static void InitializeParams (
      string[] args,
      out string name)
    {
      name = "Josef Pelikán";

      Form1 f = Form1.singleton;

      // Scene script (empty string for default scene).
      f.sceneFileName = (args != null && args.Length > 0) ? args[0] : "../data/rtscenes/AnimatedScene.cs";

      // Param string.
      f.textParam.Text = "";

      // Single frame.
      f.ImageWidth  = 320;
      f.ImageHeight = 180;
      f.numericSupersampling.Value = 1;

      // Animation.
      f.numFrom.Value = (decimal)0.0;
      f.numTo.Value   = (decimal)20.0;
      f.numFps.Value  = (decimal)25.0;
    }

    /// <summary>
    /// Initialize the ray-scene.
    /// </summary>
    public static IRayScene getScene (
      out IImageFunction imf,
      out IRenderer rend,
      ref int width,
      ref int height,
      ref int superSampling,
      ref double minTime,
      ref double maxTime,
      ref double fps,
      string param,
      in double? time = null)
    {
      IRayScene sc = Form1.singleton.SceneFromScript(
        out imf,
        out rend,
        ref width,
        ref height,
        ref superSampling,
        ref minTime,
        ref maxTime,
        ref fps,
        time);      // 'param' will be fetched from the Form

      if (sc != null)
        return sc;

      sc = new AnimatedRayScene();
      return AnimatedScene.Init(sc, param);
    }

    /// <summary>
    /// Initialize ray-scene and image function (good enough for simple samples).
    /// Called only if IImageFunction was not defined in the animation script.
    /// </summary>
    public static IImageFunction getImageFunction ()
    {
      return new RayTracing();
    }

    /// <summary>
    /// Initialize image synthesizer (responsible for raster image computation).
    /// </summary>
    public static IRenderer getRenderer (
      int superSampling)
    {
      if (superSampling > 1)
        return new SupersamplingImageSynthesizer
        {
          Supersampling = superSampling,
          Jittering     = 1.0
        };

      return new SimpleImageSynthesizer();
    }
  }
}

namespace Rendering
{
  /// <summary>
  /// Animated Ray-scene.
  /// </summary>
  public class AnimatedScene
  {
    /// <summary>
    /// Creates default ray-rendering scene.
    /// </summary>
    public static IRayScene Init (IRayScene sc, string param)
    {
      // !!!{{ TODO: .. and use your time-dependent objects to construct the scene

      // This code is based on Scenes.TwoSpheres():

      // CSG scene:
      CSGInnerNode root = new CSGInnerNode(SetOperation.Union);
      root.SetAttribute(PropertyName.REFLECTANCE_MODEL, new PhongModel());
      root.SetAttribute(PropertyName.MATERIAL, new PhongMaterial(new double[] {1.0, 0.8, 0.1}, 0.1, 0.6, 0.4, 128));
      sc.Intersectable = root;

      // Background color:
      sc.BackgroundColor = new double[] {0.0, 0.05, 0.07};

      // Camera:
      AnimatedCamera cam = new AnimatedCamera(new Vector3d(0.7, -0.4,  0.0),
                                              new Vector3d(0.7,  0.8, -6.0),
                                              50.0 );
      cam.End = 20.0;            // one complete turn takes 20.0 seconds
      if (sc is AnimatedRayScene asc)
        asc.End = 20.0;
      sc.Camera  = cam;

      // Light sources:
      sc.Sources = new LinkedList<ILightSource>();
      sc.Sources.Add( new AmbientLightSource(0.8));
      sc.Sources.Add( new PointLightSource(new Vector3d(-5.0, 4.0, -3.0), 1.2));

      // --- NODE DEFINITIONS ----------------------------------------------------

      // Transparent sphere:
      Sphere s;
      s = new Sphere();
      PhongMaterial pm = new PhongMaterial(new double[] {0.0, 0.2, 0.1}, 0.03, 0.03, 0.08, 128);
      pm.n  = 1.6;
      pm.Kt = 0.9;
      s.SetAttribute(PropertyName.MATERIAL, pm);
      root.InsertChild(s, Matrix4d.Identity);

      // Opaque sphere:
      s = new Sphere();
      root.InsertChild(s, Matrix4d.Scale(1.2) * Matrix4d.CreateTranslation(1.5, 0.2, 2.4));

      // Infinite plane with checker:
      Plane pl = new Plane();
      pl.SetAttribute(PropertyName.COLOR, new double[] {0.3, 0.0, 0.0});
      pl.SetAttribute(PropertyName.TEXTURE, new CheckerTexture(0.6, 0.6, new double[] {1.0, 1.0, 1.0}));
      root.InsertChild(pl, Matrix4d.RotateX(-MathHelper.PiOver2) * Matrix4d.CreateTranslation(0.0, -1.0, 0.0));

      // !!!}}

      return sc;
    }
  }
}
