using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using MathSupport;
using OpenTK;
using Rendering;

namespace _048rtmontecarlo
{
  public partial class Form1 : Form
  {
    /// <summary>
    /// Initialize the ray-scene.
    /// </summary>
    private IRayScene getScene ()
    {
      return SceneByComboBox();
    }

    /// <summary>
    /// Initialize ray-scene and image function (good enough for simple samples).
    /// </summary>
    private IImageFunction getImageFunction ( IRayScene scene )
    {
      return new RayTracing( scene );
    }

    /// <summary>
    /// Global instance of a random generator.
    /// </summary>
    private static RandomJames rnd = new RandomJames();

    /// <summary>
    /// Initialize image synthesizer (responsible for raster image computation).
    /// </summary>
    private IRenderer getRenderer ( IImageFunction imf )
    {
      AdaptiveSupersamplingImageSynthesizer sis = new AdaptiveSupersamplingImageSynthesizer();
      sis.ImageFunction = imf;
      return sis;
    }

    /// <summary>
    /// Prepare combo-box of available scenes.
    /// </summary>
    private void InitializeScenes ()
    {
      sceneInitFunctions = new List<InitSceneDelegate>( Scenes.InitFunctions );

      // 1. default scenes from RayCastingScenes
      foreach ( string name in Scenes.Names )
        comboScene.Items.Add( name );

      // 2. eventually add custom scenes
      sceneInitFunctions.Add( new InitSceneDelegate( CustomScene.TestScene ) );
      comboScene.Items.Add( "Test scene" );

      // .. and set your favorite scene here:
      comboScene.SelectedIndex = comboScene.Items.IndexOf( "Test scene" );
    }
  }
}

namespace Rendering
{
  /// <summary>
  /// Super-samples only pixels which actually need it!
  /// </summary>
  public class AdaptiveSupersamplingImageSynthesizer : SupersamplingImageSynthesizer
  {
    public AdaptiveSupersamplingImageSynthesizer ()
      : base( 16 )
    {
    }

    /// <summary>
    /// Renders the single pixel of an image (using required super-sampling).
    /// </summary>
    /// <param name="x">Horizontal coordinate.</param>
    /// <param name="y">Vertical coordinate.</param>
    /// <param name="color">Computed pixel color.</param>
    /// <param name="rnd">Shared random generator.</param>
    public override void RenderPixel ( int x, int y, double[] color, RandomJames rnd )
    {
      Debug.Assert( color != null );
      Debug.Assert( rnd != null );

      // !!!{{ TODO: this is exactly the code inherited from static sampling - make it adaptive!

      int bands = color.Length;
      int b;
      Array.Clear( color, 0, bands );
      double[] tmp = new double[ bands ];

      int i, j, ord;
      double step = 1.0 / superXY;
      double amplitude = Jittering * step;
      double origin = 0.5 * (step - amplitude);
      double x0, y0;
      for ( j = ord = 0, y0 = y + origin; j++ < superXY; y0 += step )
        for ( i = 0, x0 = x + origin; i++ < superXY; x0 += step )
        {
          ImageFunction.GetSample( x0 + amplitude * rnd.UniformNumber(),
                                   y0 + amplitude * rnd.UniformNumber(),
                                   ord++, Supersampling, tmp );
          for ( b = 0; b < bands; b++ )
            color[ b ] += tmp[ b ];
        }

      double mul = step / superXY;
      if ( Gamma > 0.001 )
      {                                     // gamma-encoding and clamping
        double g = 1.0 / Gamma;
        for ( b = 0; b < bands; b++ )
          color[ b ] = Arith.Clamp( Math.Pow( color[ b ] * mul, g ), 0.0, 1.0 );
      }
      else                                  // no gamma, no clamping (for HDRI)
        for ( b = 0; b < bands; b++ )
          color[ b ] *= mul;

      // !!!}}
    }
  }

  /// <summary>
  /// Custom scene for adaptive super-sampling (derived from "Sphere on the Plane").
  /// </summary>
  public class CustomScene
  {
    public static void TestScene ( IRayScene sc )
    {
      Debug.Assert( sc != null );

      // CSG scene:
      CSGInnerNode root = new CSGInnerNode( SetOperation.Union );
      root.SetAttribute( PropertyName.REFLECTANCE_MODEL, new PhongModel() );
      root.SetAttribute( PropertyName.MATERIAL, new PhongMaterial( new double[] { 1.0, 0.8, 0.1 }, 0.1, 0.6, 0.4, 16 ) );
      sc.Intersectable = root;

      // Background color:
      sc.BackgroundColor = new double[] { 0.0, 0.05, 0.07 };

      // Camera:
      sc.Camera = new StaticCamera( new Vector3d( 0.7, 0.5, -5.0 ),
                                    new Vector3d( 0.0, -0.18, 1.0 ),
                                    50.0 );

      // Light sources:
      sc.Sources = new LinkedList<ILightSource>();
      sc.Sources.Add( new AmbientLightSource( 0.8 ) );
      sc.Sources.Add( new PointLightSource( new Vector3d( -5.0, 3.0, -3.0 ), 1.0 ) );

      // --- NODE DEFINITIONS ----------------------------------------------------

      // Sphere:
      Sphere s = new Sphere();
      root.InsertChild( s, Matrix4d.Identity );

      // Infinite plane with checker:
      Plane pl = new Plane();
      pl.SetAttribute( PropertyName.COLOR, new double[] { 0.3, 0.0, 0.0 } );
      pl.SetAttribute( PropertyName.TEXTURE, new CheckerTexture( 0.6, 0.6, new double[] { 1.0, 1.0, 1.0 } ) );
      root.InsertChild( pl, Matrix4d.RotateX( -MathHelper.PiOver2 ) * Matrix4d.CreateTranslation( 0.0, -1.0, 0.0 ) );
    }
  }
}
