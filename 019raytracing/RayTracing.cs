using System.Collections.Generic;
using System.Diagnostics;
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
      // feel free to replace it..
      scene = SceneByComboBox();
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
      sceneInitFunctions.Add( new InitSceneDelegate( CustomScene.MyScene ) );
      comboScene.Items.Add( "Sphere" );

      // .. and set your favorite scene here:
      comboScene.SelectedIndex = 0;
    }
  }
}

namespace Rendering
{
  /// <summary>
  /// Custom scene for ray-based rendering.
  /// </summary>
  public class CustomScene
  {
    public static void MyScene ( IRayScene sc )
    {
      Debug.Assert( sc != null );

      // CSG scene:
      CSGInnerNode root = new CSGInnerNode( SetOperation.Union );
      root.SetAttribute( PropertyName.REFLECTANCE_MODEL, new PhongModel() );
      root.SetAttribute( PropertyName.MATERIAL, new PhongMaterial( new double[] { 1.0, 0.6, 0.1 }, 0.1, 0.6, 0.4, 16 ) );
      sc.Intersectable = root;

      // Background color:
      sc.BackgroundColor = new double[] { 0.0, 0.05, 0.07 };

      // Camera:
      sc.Camera = new StaticCamera( new Vector3d( 0.0, 0.0, -4.0 ),
                                    new Vector3d( 0.0, 0.0, 1.0 ),
                                    50.0 );

      // Light sources:
      sc.Sources = new LinkedList<ILightSource>();
      sc.Sources.Add( new AmbientLightSource( 0.8 ) );
      sc.Sources.Add( new PointLightSource( new Vector3d( -5.0, 3.0, -3.0 ), 1.0 ) );

      // --- NODE DEFINITIONS ----------------------------------------------------

      // Sole sphere:
      Sphere s = new Sphere();
      root.InsertChild( s, Matrix4d.Identity );
    }
  }
}
