using System;
using System.Collections.Generic;
using System.Diagnostics;
using MathSupport;
using OpenTK;
using Utilities;

namespace Rendering
{
  public class FormSupport
  {
    /// <summary>
    /// Prepare form data (e.g. combo-box with available scenes).
    /// </summary>
    public static void InitializeScenes (string[] args, out string name)
    {
      name = "Josef Pelikán";

      Form1 f = Form1.singleton;

      // 1. default scenes from RayCastingScenes
      f.sceneRepository = new Dictionary<string, object>(Scenes.staticRepository);

      // 2. scenes read from the command-line:
      int rd = Scripts.ReadFromConfig(args, f.sceneRepository);
      if (rd > 0)
        f.SetText($"{rd} scene scripts found");
      else
        f.SetText($"No scene scripts found, check your -dir/-scene cmdline argument..");

      // 3. static 'Test scene' (in this source file)
      f.sceneRepository["Test scene"] = new InitSceneParamDelegate(CustomScene.TestScene);

      // 4. fill the combo-box
      foreach (string key in f.sceneRepository.Keys)
        f.ComboScene.Items.Add(key);

      // .. and set your favorite scene here:
      f.ComboScene.SelectedIndex = f.ComboScene.Items.IndexOf("Test scene");

      // default image parameters?
      f.ImageWidth = 640;
      f.ImageHeight = 480;
      f.NumericSupersampling.Value = 4;
      f.CheckMultithreading.Checked = true;
      f.TextParam.Text = "sampling=adapt1";
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
      string param)
    {
      // 'superSampling' not used here but it could be if IRenderer is created here.
      // 'param'         - ditto -
      // ActualWidth & ActualHeight can be modified by this call!
      return Form1.singleton.SceneByComboBox(
        out imf,
        out rend,
        ref width,
        ref height,
        ref superSampling);
    }

    /// <summary>
    /// Initialize ray-scene and image function (good enough for simple samples).
    /// Called only in case the scene-script didn't define it.
    /// </summary>
    public static IImageFunction getImageFunction (
      IRayScene scene,
      string param)
    {
      return new RayTracing(scene);
    }

    /// <summary>
    /// Initialize image synthesizer (responsible for raster image computation).
    /// </summary>
    public static IRenderer getRenderer (int superSampling, double jittering, string param)
    {
      Dictionary<string, string> p = Util.ParseKeyValueList(param);

      string isType;
      IRenderer r = null;
      if (p.TryGetValue("sampling", out isType) &&
           superSampling > 1)
        switch (isType)
        {
          case "adapt1":
            double threshold = 0.004;
            Util.TryParse(p, "threshold", ref threshold);
            r = new AdaptiveSupersamplingJR(threshold)
            {
              Supersampling = superSampling,
              Jittering = jittering
            };
            break;

          case "adapt2":
            r = new AdaptiveSupersampling
            {
              Supersampling = superSampling,
              Jittering = jittering
            };
            break;
        }

      if (r == null)
      {
        r = new SupersamplingImageSynthesizer
        {
          Supersampling = superSampling,
          Jittering = jittering
        };
      }

      return r;
    }
  }
}

namespace Rendering
{
  /// <summary>
  /// Super-samples only pixels/pixel parts which actually need it!
  /// </summary>
  [Serializable]
  public class AdaptiveSupersampling : SupersamplingImageSynthesizer
  {
    public AdaptiveSupersampling ()
      : base (16)
    {}

    /// <summary>
    /// Renders the single pixel of an image (using required super-sampling).
    /// </summary>
    /// <param name="x">Horizontal coordinate.</param>
    /// <param name="y">Vertical coordinate.</param>
    /// <param name="color">Computed pixel color.</param>
    public override void RenderPixel (int x, int y, double[] color)
    {
      Debug.Assert(color != null);
      Debug.Assert(MT.rnd != null);

      // !!!{{ TODO: this is exactly the code inherited from static sampling - make it adaptive!

      int bands = color.Length;
      int b;
      Array.Clear(color, 0, bands);
      double[] tmp = new double[bands];

      int    i, j;
      double step      = 1.0 / superXY;
      double amplitude = Jittering * step;
      double origin    = 0.5 * (step - amplitude);
      double x0, y0;
      MT.StartPixel(x, y, Supersampling);

      for (j = 0, y0 = y + origin; j++ < superXY; y0 += step)
        for (i = 0, x0 = x + origin; i++ < superXY; x0 += step)
        {
          ImageFunction.GetSample(x0 + amplitude * MT.rnd.UniformNumber(),
                                  y0 + amplitude * MT.rnd.UniformNumber(),
                                  tmp);
          MT.NextSample();
          Util.ColorAdd(tmp, color);
        }

      double mul = step / superXY;

      // Gamma-encoding?
      if (Gamma > 0.001)
      {
        // Gamma-encoding and clamping.
        double g = 1.0 / Gamma;
        for (b = 0; b < bands; b++)
          color[b] = Arith.Clamp(Math.Pow(color[b] * mul, g), 0.0, 1.0);
      }
      else
        // No gamma, no clamping (for HDRI).
        Util.ColorMul(mul, color);

      // !!!}}
    }
  }

  /// <summary>
  /// Custom scene for adaptive super-sampling (derived from "Sphere on the Plane").
  /// </summary>
  public class CustomScene
  {
    public static void TestScene (IRayScene sc, string param)
    {
      Debug.Assert(sc != null);

      // CSG scene:
      CSGInnerNode root = new CSGInnerNode(SetOperation.Union);

      root.SetAttribute(
        PropertyName.REFLECTANCE_MODEL,
        new PhongModel());

      root.SetAttribute(
        PropertyName.MATERIAL,
        new PhongMaterial(new double[] { 1.0, 0.8, 0.1 }, 0.1, 0.6, 0.4, 128));

      sc.Intersectable = root;

      // Background color:
      sc.BackgroundColor = new double[] { 0.0, 0.05, 0.07 };

      // Camera:
      sc.Camera = new StaticCamera(
        new Vector3d(0.7, 0.5, -5.0),
        new Vector3d(0.0, -0.18, 1.0),
        50.0);

      // Light sources:
      sc.Sources = new LinkedList<ILightSource>();
      sc.Sources.Add(new AmbientLightSource(0.8));
      sc.Sources.Add(new PointLightSource(new Vector3d(-5.0, 4.0, -3.0), 1.2));

      /*
      // horizontal stick source:
      //RectangleLightSource rls = new RectangleLightSource( new Vector3d( -5.0, 4.0, -6.0 ),
      //                                                     new Vector3d( 0.0, 0.0, 6.0 ),
      //                                                     new Vector3d( 0.0, 0.1, 0.0 ), 2.2 );
      // vertical stick source:
      RectangleLightSource rls = new RectangleLightSource( new Vector3d( -5.0, 1.0, -3.0 ),
                                                           new Vector3d( 0.0, 0.0, 0.1 ),
                                                           new Vector3d( 0.0, 6.0, 0.0 ), 2.2 );
      // rectangle source:
      //RectangleLightSource rls = new RectangleLightSource( new Vector3d( -5.0, 1.0, -6.0 ),
      //                                                     new Vector3d( 0.0, 0.0, 6.0 ),
      //                                                     new Vector3d( 0.0, 6.0, 0.0 ), 2.2 );
      rls.Dim = new double[] { 1.0, 0.04, 0.0 };
      sc.Sources.Add( rls );
      */

      // --- NODE DEFINITIONS ----------------------------------------------------

      // Sphere:
      Sphere s = new Sphere ();

      root.InsertChild(s, Matrix4d.Identity);

      // Infinite plane with checker:
      Plane pl = new Plane ();

      pl.SetAttribute(
        PropertyName.COLOR,
        new double[] { 0.3, 0.0, 0.0 });

      pl.SetAttribute(
        PropertyName.TEXTURE,
        new CheckerTexture(0.6, 0.6, new double[] { 1.0, 1.0, 1.0 }));

      root.InsertChild(pl, Matrix4d.RotateX(-MathHelper.PiOver2) * Matrix4d.CreateTranslation(0.0, -1.0, 0.0));
    }
  }
}
