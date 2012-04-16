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
      sceneInitFunctions.Add( new InitSceneDelegate( CustomScene.TestScene ) );
      comboScene.Items.Add( "Test scene" );

      // .. and set your favorite scene here:
      comboScene.SelectedIndex = comboScene.Items.IndexOf( "Cubes" );
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
    public static void TestScene ( IRayScene sc )
    {
      Debug.Assert( sc != null );

      // CSG scene:
      CSGInnerNode root = new CSGInnerNode( SetOperation.Union );
      root.SetAttribute( PropertyName.REFLECTANCE_MODEL, new PhongModel() );
      root.SetAttribute( PropertyName.MATERIAL, new PhongMaterial( new double[] { 0.7, 0.1, 0.0 }, 0.1, 0.8, 0.2, 16 ) );
      sc.Intersectable = root;

      // Background color:
      sc.BackgroundColor = new double[] { 0.0, 0.05, 0.07 };

      // Camera:
      sc.Camera = new StaticCamera( new Vector3d( 0.7, 3.0, -10.0 ),
                                    new Vector3d( 0.0, -0.3, 1.0 ),
                                    50.0 );

      // Light sources:
      sc.Sources = new LinkedList<ILightSource>();
      sc.Sources.Add( new AmbientLightSource( 0.8 ) );
      sc.Sources.Add( new PointLightSource( new Vector3d( -5.0, 3.0, -3.0 ), 1.0 ) );

      // --- NODE DEFINITIONS ----------------------------------------------------

      // Base plane
      Plane pl = new Plane();
      pl.SetAttribute( PropertyName.COLOR, new double[] { 0.1, 0.4, 0.0 } );
      pl.SetAttribute( PropertyName.TEXTURE, new CheckerTexture( 0.5, 0.5, new double[] { 1.0, 1.0, 1.0 } ) );
      root.InsertChild( pl, Matrix4d.RotateX( -MathHelper.PiOver2 ) * Matrix4d.CreateTranslation( 0.0, -1.0, 0.0 ) );

      // Cubes
      Cube c;
      // front row:
      c = new Cube();
      root.InsertChild( c, Matrix4d.RotateY( 0.6 ) * Matrix4d.CreateTranslation( -3.5, -0.8, 0.0 ) );
      c = new Cube();
      root.InsertChild( c, Matrix4d.RotateY( 1.2 ) * Matrix4d.CreateTranslation( -1.5, -0.8, 0.0 ) );
      c = new Cube();
      root.InsertChild( c, Matrix4d.RotateY( 1.8 ) * Matrix4d.CreateTranslation( 0.5, -0.8, 0.0 ) );
      c = new Cube();
      root.InsertChild( c, Matrix4d.RotateY( 2.4 ) * Matrix4d.CreateTranslation( 2.5, -0.8, 0.0 ) );
      c = new Cube();
      root.InsertChild( c, Matrix4d.RotateY( 3.0 ) * Matrix4d.CreateTranslation( 4.5, -0.8, 0.0 ) );
      // back row:
      c = new Cube();
      root.InsertChild( c, Matrix4d.RotateX( 3.5 ) * Matrix4d.CreateTranslation( -4.0, 1.0, 2.0 ) );
      c = new Cube();
      root.InsertChild( c, Matrix4d.RotateX( 3.0 ) * Matrix4d.CreateTranslation( -2.5, 1.0, 2.0 ) );
      c = new Cube();
      root.InsertChild( c, Matrix4d.RotateX( 2.5 ) * Matrix4d.CreateTranslation( -1.0, 1.0, 2.0 ) );
      c = new Cube();
      root.InsertChild( c, Matrix4d.RotateX( 2.0 ) * Matrix4d.CreateTranslation( 0.5, 1.0, 2.0 ) );
      c = new Cube();
      root.InsertChild( c, Matrix4d.RotateX( 1.5 ) * Matrix4d.CreateTranslation( 2.0, 1.0, 2.0 ) );
      c = new Cube();
      root.InsertChild( c, Matrix4d.RotateX( 1.0 ) * Matrix4d.CreateTranslation( 3.5, 1.0, 2.0 ) );
      c = new Cube();
      root.InsertChild( c, Matrix4d.RotateX( 0.5 ) * Matrix4d.CreateTranslation( 5.0, 1.0, 2.0 ) );
    }
  }
}
