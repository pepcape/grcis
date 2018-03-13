using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using OpenTK;
using Scene3D;
using Utilities;

namespace Rendering
{
  /// <summary>
  /// Delegate used for RT-scene initialization.
  /// </summary>
  public delegate void InitSceneDelegate ( IRayScene sc );

  /// <summary>
  /// More general RT-scene initialization, can pass user-provided string.
  /// </summary>
  public delegate void InitSceneParamDelegate ( IRayScene sc, string param );

  /// <summary>
  /// Delegate used for RT-scene initialization (variant with list of strings .. e.g. mesh names).
  /// </summary>
  public delegate long InitSceneStrDelegate ( IRayScene sc, string[] names );

  /// <summary>
  /// Some interesting scenes created mostly by MFF UK students.
  /// http://cgg.mff.cuni.cz/~pepca/gr/grcis/
  /// </summary>
  public class Scenes
  {
    /// <summary>
    /// Scene repository: sceneName -> {sceneDelegate | scriptFileContent}
    /// </summary>
    public static Dictionary<string, object> staticRepository;

    static Scenes ()
    {
      staticRepository = new Dictionary<string, object>();
      staticRepository[ "Five balls" ]           = new InitSceneDelegate( FiveBalls );
      staticRepository[ "Hedgehog in the cage" ] = new InitSceneDelegate( HedgehogInTheCage );
      staticRepository[ "Flags" ]                = new InitSceneDelegate( Flags );
      staticRepository[ "Sphere on the plane" ]  = new InitSceneDelegate( SpherePlane );
      staticRepository[ "Two spheres" ]          = new InitSceneParamDelegate( TwoSpheres );
      staticRepository[ "Sphere flake" ]         = new InitSceneParamDelegate( SphereFlake );
      staticRepository[ "Cubes" ]                = new InitSceneParamDelegate( Cubes );
      staticRepository[ "Cylinders" ]            = new InitSceneDelegate( Cylinders );
      staticRepository[ "Circus" ]               = new InitSceneDelegate( Circus );
      staticRepository[ "Toroids" ]              = new InitSceneDelegate( Toroids );
      staticRepository[ "Bezier" ]               = new InitSceneDelegate( Bezier );
    }

    /// <summary>
    /// Something very simple.
    /// </summary>
    public static IRayScene DefaultScene ( IRayScene sc =null )
    {
      if ( sc == null )
        sc = new DefaultRayScene();
      SpherePlane( sc );
      return sc;
    }

    /// <summary>
    /// Scene names (Str variant) how to appear in a combo-box.
    /// </summary>
    public static string[] NamesStr =
    {
      "Teapot",
      "Pitcher",
      "Buddha",
      "Toy-plane",
    };

    /// <summary>
    /// The init functions themselves (Str variant)..
    /// </summary>
    public static InitSceneStrDelegate[] InitFunctionsStr =
    {
      new InitSceneStrDelegate( TeapotObj ),
      new InitSceneStrDelegate( PitcherObj ),
      new InitSceneStrDelegate( BuddhaObj ),
      new InitSceneStrDelegate( PlaneObj ),
    };

    /// <summary>
    /// Simple scene containing five colored spheres.
    /// </summary>
    public static void FiveBalls ( IRayScene sc )
    {
      Debug.Assert( sc != null );

      // CSG scene:
      CSGInnerNode root = new CSGInnerNode( SetOperation.Union );
      root.SetAttribute( PropertyName.REFLECTANCE_MODEL, new PhongModel() );
      root.SetAttribute( PropertyName.MATERIAL, new PhongMaterial( new double[] { 0.5, 0.5, 0.5 }, 0.1, 0.6, 0.3, 16 ) );
      sc.Intersectable = root;

      // Background color:
      sc.BackgroundColor = new double[] { 0.0, 0.05, 0.05 };

      // Camera:
      sc.Camera = new StaticCamera( new Vector3d( 0.0, 0.0, -10.0 ),
                                    new Vector3d( 0.0, 0.0, 1.0 ),
                                    60.0 );

      // Light sources:
      sc.Sources = new LinkedList<ILightSource>();
      sc.Sources.Add( new AmbientLightSource( 0.8 ) );
      sc.Sources.Add( new PointLightSource( new Vector3d( -5.0, 3.0, -3.0 ), 1.0 ) );

      // --- NODE DEFINITIONS ----------------------------------------------------

      // sphere 1:
      Sphere s = new Sphere();
      s.SetAttribute( PropertyName.COLOR, new double[] { 1.0, 0.6, 0.0 } );
      root.InsertChild( s, Matrix4d.Identity );

      // sphere 2:
      s = new Sphere();
      s.SetAttribute( PropertyName.COLOR, new double[] { 0.2, 0.9, 0.5 } );
      root.InsertChild( s, Matrix4d.CreateTranslation( -2.2, 0.0, 0.0 ) );

      // sphere 3:
      s = new Sphere();
      s.SetAttribute( PropertyName.COLOR, new double[] { 0.1, 0.3, 1.0 } );
      root.InsertChild( s, Matrix4d.CreateTranslation( -4.4, 0.0, 0.0 ) );

      // sphere 4:
      s = new Sphere();
      s.SetAttribute( PropertyName.COLOR, new double[] { 1.0, 0.2, 0.2 } );
      root.InsertChild( s, Matrix4d.CreateTranslation( 2.2, 0.0, 0.0 ) );

      // sphere 5:
      s = new Sphere();
      s.SetAttribute( PropertyName.COLOR, new double[] { 0.1, 0.4, 0.0 } );
      s.SetAttribute( PropertyName.TEXTURE, new CheckerTexture( 80.0, 40.0, new double[] { 1.0, 0.8, 0.2 } ) );
      root.InsertChild( s, Matrix4d.CreateTranslation( 4.4, 0.0, 0.0 ) );
    }

    /// <summary>
    /// Hedgehog in the Cage - by Michal Wirth, (c) 2012
    /// </summary>
    public static void HedgehogInTheCage ( IRayScene sc )
    {
      Debug.Assert( sc != null );

      // CSG scene:
      CSGInnerNode root = new CSGInnerNode( SetOperation.Union );
      root.SetAttribute( PropertyName.REFLECTANCE_MODEL, new PhongModel() );
      root.SetAttribute( PropertyName.MATERIAL, new PhongMaterial( new double[] { 0.5, 0.5, 0.5 }, 0.2, 0.6, 0.2, 16 ) );
      sc.Intersectable = root;

      // Background color:
      sc.BackgroundColor = new double[] { 0.0, 0.05, 0.05 };

      // Camera:
      sc.Camera = new StaticCamera( new Vector3d( 0.0, 2.0, -7.0 ),
                                    new Vector3d( 0.0, -0.32, 1.0 ),
                                    40.0 );

      // Light sources:
      sc.Sources = new LinkedList<ILightSource>();
      sc.Sources.Add( new AmbientLightSource( 0.8 ) );
      sc.Sources.Add( new PointLightSource( new Vector3d( -8.0, 5.0, -3.0 ), 1.0 ) );

      // --- NODE DEFINITIONS ----------------------------------------------------

      // cage
      CSGInnerNode cage = new CSGInnerNode( SetOperation.Difference );
      cage.SetAttribute( PropertyName.COLOR, new double[] { 0.70, 0.93, 0.20 } );

      // cylinder1
      CSGInnerNode cylinder1 = new CSGInnerNode( SetOperation.Intersection );
      Sphere s = new Sphere();
      cylinder1.InsertChild( s, Matrix4d.Scale( 1.0, 1000.0, 1.0 ) );
      s = new Sphere();
      cylinder1.InsertChild( s, Matrix4d.Scale( 1000.0, 1.5, 1000.0 ) );
      cage.InsertChild( cylinder1, Matrix4d.Identity );

      // cylinder2
      CSGInnerNode cylinder2 = new CSGInnerNode( SetOperation.Intersection );
      s = new Sphere();
      cylinder2.InsertChild( s, Matrix4d.Scale( 1.0, 1000.0, 1.0 ) );
      s = new Sphere();
      cylinder2.InsertChild( s, Matrix4d.Scale( 1000.0, 1.5, 1000.0 ) );
      cage.InsertChild( cylinder2, Matrix4d.Scale( 0.9 ) );

      // holeUpDown
      Sphere holeUpDown = new Sphere();
      cage.InsertChild( holeUpDown, Matrix4d.Scale( 0.5, 1000.0, 0.5 ) );

      // hole1
      CSGInnerNode hole1 = new CSGInnerNode( SetOperation.Intersection );
      s = new Sphere();
      hole1.InsertChild( s, Matrix4d.Scale( 1000.0, 1.1, 1000.0 ) );
      s = new Sphere();
      hole1.InsertChild( s, Matrix4d.Scale( 0.4, 1000.0, 1000.0 ) );
      cage.InsertChild( hole1, Matrix4d.Identity );

      // hole2
      CSGInnerNode hole2 = new CSGInnerNode( SetOperation.Intersection );
      s = new Sphere();
      hole2.InsertChild( s, Matrix4d.Scale( 1000.0, 1.1, 1000.0 ) );
      s = new Sphere();
      hole2.InsertChild( s, Matrix4d.Scale( 0.4, 1000.0, 1000.0 ) );
      cage.InsertChild( hole2, Matrix4d.RotateY( Math.PI / 3 ) );

      // hole3
      CSGInnerNode hole3 = new CSGInnerNode( SetOperation.Intersection );
      s = new Sphere();
      hole3.InsertChild( s, Matrix4d.Scale( 1000.0, 1.1, 1000.0 ) );
      s = new Sphere();
      hole3.InsertChild( s, Matrix4d.Scale( 0.4, 1000.0, 1000.0 ) );
      cage.InsertChild( hole3, Matrix4d.RotateY( Math.PI / -3 ) );

      // hedgehog
      CSGInnerNode hedgehog = new CSGInnerNode( SetOperation.Union );
      hedgehog.SetAttribute( PropertyName.COLOR, new double[] { 0.4, 0.05, 0.05 } );

      s = new Sphere();
      hedgehog.InsertChild( s, Matrix4d.Scale( 0.48 ) );

      // spine1
      CSGInnerNode spine1 = new CSGInnerNode( SetOperation.Intersection );
      s = new Sphere();
      spine1.InsertChild( s, Matrix4d.Scale( 0.06, 1000.0, 0.06 ) );
      s = new Sphere();
      spine1.InsertChild( s, Matrix4d.Scale( 1.2 ) );
      hedgehog.InsertChild( spine1, Matrix4d.Identity );

      // spine2
      CSGInnerNode spine2 = new CSGInnerNode( SetOperation.Intersection );
      s = new Sphere();
      spine2.InsertChild( s, Matrix4d.Scale( 0.06, 1000.0, 0.06 ) );
      s = new Sphere();
      spine2.InsertChild( s, Matrix4d.Scale( 1.2 ) * Matrix4d.CreateTranslation( 0.0, 0.1, 0.0 ) );
      hedgehog.InsertChild( spine2, Matrix4d.RotateX( Math.PI / -3 ) );

      // spine3
      CSGInnerNode spine3 = new CSGInnerNode( SetOperation.Intersection );
      s = new Sphere();
      spine3.InsertChild( s, Matrix4d.Scale( 0.06, 1000.0, 0.06 ) );
      s = new Sphere();
      spine3.InsertChild( s, Matrix4d.Scale( 1.2 ) * Matrix4d.CreateTranslation( 0.0, -0.25, 0.0 ) );
      hedgehog.InsertChild( spine3, Matrix4d.RotateX( Math.PI / -3 ) * Matrix4d.RotateY( Math.PI * -0.4 ) );

      // spine4
      CSGInnerNode spine4 = new CSGInnerNode( SetOperation.Intersection );
      s = new Sphere();
      spine4.InsertChild( s, Matrix4d.Scale( 0.06, 1000.0, 0.06 ) );
      s = new Sphere();
      spine4.InsertChild( s, Matrix4d.Scale( 1.2 ) * Matrix4d.CreateTranslation( 0.0, 0.2, 0.0 ) );
      hedgehog.InsertChild( spine4, Matrix4d.RotateX( Math.PI / -3 ) * Matrix4d.RotateY( Math.PI * -0.8 ) );

      // spine5
      CSGInnerNode spine5 = new CSGInnerNode( SetOperation.Intersection );
      s = new Sphere();
      spine5.InsertChild( s, Matrix4d.Scale( 0.06, 1000.0, 0.06 ) );
      s = new Sphere();
      spine5.InsertChild( s, Matrix4d.Scale( 1.2 ) * Matrix4d.CreateTranslation( 0.0, -0.2, 0.0 ) );
      hedgehog.InsertChild( spine5, Matrix4d.RotateX( Math.PI / -3 ) * Matrix4d.RotateY( Math.PI * 0.4 ) );

      // spine6
      CSGInnerNode spine6 = new CSGInnerNode( SetOperation.Intersection );
      s = new Sphere();
      spine6.InsertChild( s, Matrix4d.Scale( 0.06, 1000.0, 0.06 ) );
      s = new Sphere();
      spine6.InsertChild( s, Matrix4d.Scale( 1.2 ) * Matrix4d.CreateTranslation( 0.0, -0.25, 0.0 ) );
      hedgehog.InsertChild( spine6, Matrix4d.RotateX( Math.PI / -3 ) * Matrix4d.RotateY( Math.PI * 0.8 ) );

      // all
      CSGInnerNode all = new CSGInnerNode( SetOperation.Union );
      all.InsertChild( cage, Matrix4d.RotateY( 0.25 ) );
      all.InsertChild( hedgehog, Matrix4d.Rotate( new Vector3d( 0.0, 1.0, 1.0 ), 0.1 ) * Matrix4d.CreateTranslation( 0.0, -0.1, 0.0 ) );

      root.InsertChild( all, Matrix4d.RotateZ( 0.1 ) );
    }

    /// <summary>
    /// Six national flags (in artistic style) - by Jakub Vlcek, (c) 2012
    /// </summary>
    public static void Flags ( IRayScene sc )
    {
      Debug.Assert( sc != null );

      // CSG scene:
      CSGInnerNode flags = new CSGInnerNode( SetOperation.Union );
      flags.SetAttribute( PropertyName.REFLECTANCE_MODEL, new PhongModel() );
      flags.SetAttribute( PropertyName.MATERIAL, new PhongMaterial( new double[] { 0.5, 0.5, 0.5 }, 0.2, 0.7, 0.1, 16 ) );
      sc.Intersectable = flags;
      Sphere s;
      Cube c;

      // Background color:
      sc.BackgroundColor = new double[] { 0.0, 0.05, 0.05 };

      // Camera:
      sc.Camera = new StaticCamera( new Vector3d( 0.0, 0.0, -10.0 ),
                                    new Vector3d( 0.0, 0.0, 1.0 ),
                                    60.0 );

      // Light sources:
      sc.Sources = new LinkedList<ILightSource>();
      sc.Sources.Add( new AmbientLightSource( 0.8 ) );
      sc.Sources.Add( new PointLightSource( new Vector3d( -5.0, 3.0, -3.0 ), 1.0 ) );
      sc.Sources.Add( new PointLightSource( new Vector3d( 5.0, 3.0, -3.0 ), 1.0 ) );

      // --- NODE DEFINITIONS ----------------------------------------------------

      // Latvian flag (intersection, difference and xor):
      CSGInnerNode latvia = new CSGInnerNode( SetOperation.Intersection );
      c = new Cube();
      c.SetAttribute( PropertyName.COLOR, new double[] { 0.3, 0.0, 0.0 } );
      latvia.InsertChild( c, Matrix4d.Scale( 4.0, 2.0, 0.4 ) * Matrix4d.CreateTranslation( -2.0, -1.0, 0.0 ) );
      CSGInnerNode latviaFlag = new CSGInnerNode( SetOperation.Xor );
      c = new Cube();
      c.SetAttribute( PropertyName.COLOR, new double[] { 0.3, 0.0, 0.0 } );
      latviaFlag.InsertChild( c, Matrix4d.CreateTranslation( -0.5, -0.5, -0.5 ) * Matrix4d.Scale( 4.0, 2.0, 2.0 ) );
      c = new Cube();
      c.SetAttribute( PropertyName.COLOR, new double[] { 1.0, 1.0, 1.0 } );
      latviaFlag.InsertChild( c, Matrix4d.CreateTranslation( -0.5, -0.5, -0.5 ) * Matrix4d.Scale( 4.2, 0.5, 0.5 ) );
      latvia.InsertChild( latviaFlag, Matrix4d.Identity );
      flags.InsertChild( latvia, Matrix4d.Scale( 0.7 ) * Matrix4d.CreateRotationX( Math.PI / 8 ) * Matrix4d.CreateTranslation( -3.5, 1.5, 0.0 ) );

      // Czech flag (difference):
      CSGInnerNode czech = new CSGInnerNode( SetOperation.Difference );
      s = new Sphere();
      s.SetAttribute( PropertyName.COLOR, new double[] { 0.2, 0.2, 0.2 } );
      czech.InsertChild( s, Matrix4d.Identity );
      s = new Sphere();
      s.SetAttribute( PropertyName.COLOR, new double[] { 1.0, 1.0, 1.0 } );
      czech.InsertChild( s, Matrix4d.CreateTranslation( 0.0, 0.8, -1.0 ) );
      s = new Sphere();
      s.SetAttribute( PropertyName.COLOR, new double[] { 1.0, 0.0, 0.0 } );
      czech.InsertChild( s, Matrix4d.CreateTranslation( 0.0, -0.8, -1.1 ) );
      s = new Sphere();
      s.SetAttribute( PropertyName.COLOR, new double[] { 0.0, 0.0, 1.0 } );
      czech.InsertChild( s, Matrix4d.CreateTranslation( -1.0, 0.0, -0.8 ) );
      flags.InsertChild( czech, Matrix4d.Scale( 1.6, 1.0, 1.0 ) * Matrix4d.CreateRotationY( Math.PI / 8 ) *
                                Matrix4d.CreateTranslation( 4.0, -1.5, 0.0 ) );

      // Croatian flag (union, intersection):
      CSGInnerNode croatianFlag = new CSGInnerNode( SetOperation.Union );
      CSGInnerNode croatianSign = new CSGInnerNode( SetOperation.Intersection );
      CSGInnerNode checkerBoard = new CSGInnerNode( SetOperation.Union );
      c = new Cube();
      c.SetAttribute( PropertyName.COLOR, new double[] { 1.0, 0.0, 0.0 } );
      c.SetAttribute( PropertyName.TEXTURE, new CheckerTexture( 10.0, 10.0, new double[] { 1.0, 1.0, 1.0 } ) );
      croatianSign.InsertChild( c, Matrix4d.CreateTranslation( -0.5, -0.5, -0.5 ) * Matrix4d.Scale( 2.0, 2.0, 0.4 ) );
      CSGInnerNode sign = new CSGInnerNode( SetOperation.Union );
      s = new Sphere();
      s.SetAttribute( PropertyName.COLOR, new double[] { 1.0, 1.0, 1.0 } );
      sign.InsertChild( s, Matrix4d.Identity );
      c = new Cube();
      c.SetAttribute( PropertyName.COLOR, new double[] { 1.0, 1.0, 1.0 } );
      sign.InsertChild( c, Matrix4d.Scale( 1.8 ) * Matrix4d.CreateTranslation( -0.9, 0.1, -0.9 ) );
      croatianSign.InsertChild( sign, Matrix4d.Scale( 0.5, 0.33, 0.5 ) * Matrix4d.CreateTranslation( 0.44, -0.5, 0.0 ) );
      c = new Cube();
      c.SetAttribute( PropertyName.COLOR, new double[] { 1.0, 0.0, 0.0 } );
      croatianFlag.InsertChild( c, Matrix4d.CreateTranslation( -0.5, -0.5, -0.5 ) * Matrix4d.Scale( 4.0, 0.6, 0.6 ) *
                                   Matrix4d.RotateX( Math.PI / 4 ) * Matrix4d.CreateTranslation( 0.5, 1.0, 1.0 ) );
      c = new Cube();
      c.SetAttribute( PropertyName.COLOR, new double[] { 1.0, 1.0, 1.0 } );
      croatianFlag.InsertChild( c, Matrix4d.CreateTranslation( -0.5, -0.5, -0.5 ) * Matrix4d.Scale( 4.0, 0.6, 0.6 ) *
                                   Matrix4d.RotateX( Math.PI / 4 ) * Matrix4d.CreateTranslation( 0.5, 0.3, 1.0 ) );
      c = new Cube();
      c.SetAttribute( PropertyName.COLOR, new double[] { 0.0, 0.0, 1.0 } );
      croatianFlag.InsertChild( c, Matrix4d.CreateTranslation( -0.5, -0.5, -0.5 ) * Matrix4d.Scale( 4.0, 0.6, 0.6 ) *
                                   Matrix4d.RotateX( Math.PI / 4 ) * Matrix4d.CreateTranslation( 0.5, -0.4, 1.0 ) );
      croatianFlag.InsertChild( croatianSign, Matrix4d.Scale( 0.8 ) * Matrix4d.CreateTranslation( 0.4, 0.5, 0.0 ) );
      flags.InsertChild( croatianFlag, Matrix4d.Scale( 0.8 ) * Matrix4d.CreateRotationY( Math.PI / 8 ) * Matrix4d.CreateTranslation( -0.4, 1.5, 1.0 ) );

      // Brazilian flag (union):
      CSGInnerNode brazilianFlag = new CSGInnerNode( SetOperation.Union );
      c = new Cube();
      c.SetAttribute( PropertyName.COLOR, new double[] { 0.0, 0.8, 0.0 } );
      brazilianFlag.InsertChild( c, Matrix4d.CreateTranslation( -0.5, -0.5, -0.5 ) * Matrix4d.Scale( 4.0, 2.0, 0.2 ) );
      c = new Cube();
      c.SetAttribute( PropertyName.COLOR, new double[] { 1.0, 1.0, 0.0 } );
      brazilianFlag.InsertChild( c, Matrix4d.CreateTranslation( -0.5, -0.5, -0.5 ) * Matrix4d.CreateRotationZ( Math.PI / 4 ) *
                                    Matrix4d.Scale( 2.0, 1.0, 0.4 ) );
      s = new Sphere();
      s.SetAttribute( PropertyName.COLOR, new double[] { 0.0, 0.0, 1.0 } );
      brazilianFlag.InsertChild( s, Matrix4d.Scale( 0.5 ) * Matrix4d.CreateTranslation( 0.0, 0.0, 0.0 ) );
      flags.InsertChild( brazilianFlag, Matrix4d.Scale( 0.9 ) * Matrix4d.RotateY( -Math.PI / 8 ) * Matrix4d.CreateTranslation( 0.0, -1.8, 1.0 ) );

      // Finnish flag (intersection and difference):
      CSGInnerNode finlandFlag = new CSGInnerNode( SetOperation.Difference );
      s = new Sphere();
      s.SetAttribute( PropertyName.COLOR, new double[] { 1.0, 1.0, 1.0 } );
      finlandFlag.InsertChild( s, Matrix4d.Scale( 2.0, 1.0, 0.5 ) * Matrix4d.CreateTranslation( 0.0, 0.0, 0.3 ) );
      c = new Cube();
      c.SetAttribute( PropertyName.COLOR, new double[] { 0.0, 0.0, 1.0 } );
      finlandFlag.InsertChild( c, Matrix4d.CreateTranslation( -0.5, -0.5, -0.5 ) * Matrix4d.Scale( 4.2, 0.5, 0.5 ) );
      c = new Cube();
      c.SetAttribute( PropertyName.COLOR, new double[] { 0.0, 0.0, 1.0 } );
      finlandFlag.InsertChild( c, Matrix4d.CreateTranslation( -0.5, -0.5, -0.5 ) * Matrix4d.Scale( 0.5, 4.2, 0.5 ) * Matrix4d.CreateTranslation( -0.5, 0.0, 0.0 ) );
      flags.InsertChild( finlandFlag, Matrix4d.Scale( 0.7 ) * Matrix4d.CreateTranslation( 3.5, 1.5, 0.0 ) );

      // Cuban flag (union and intersection):
      CSGInnerNode cubanFlag = new CSGInnerNode( SetOperation.Union );
      for ( int i = 0; i < 5; i++ )
      {
        c = new Cube();
        if ( i % 2 == 0 )
          c.SetAttribute( PropertyName.COLOR, new double[] { 0.0, 0.0, 1.0 } );
        else
          c.SetAttribute( PropertyName.COLOR, new double[] { 1.0, 1.0, 1.0 } );
        cubanFlag.InsertChild( c, Matrix4d.CreateTranslation( -0.5, -0.5, -0.5 ) * Matrix4d.Scale( 4.0, 0.4, 0.4 ) *
                                  Matrix4d.CreateTranslation( new Vector3d( 0.0, 0.0 - i * 0.4, 0.8 - i * 0.2 ) ) );
      }
      CSGInnerNode wedge = new CSGInnerNode( SetOperation.Intersection );
      c = new Cube();
      c.SetAttribute( PropertyName.COLOR, new double[] { 1.0, 0.0, 0.0 } );
      wedge.InsertChild( c, Matrix4d.CreateTranslation( -0.5, -0.5, -0.5 ) * Matrix4d.Scale( 2.0, 2.0, 2.0 ) *
                            Matrix4d.CreateRotationZ( Math.PI / 4 ) );
      c = new Cube();
      c.SetAttribute( PropertyName.COLOR, new double[] { 1.0, 0.0, 0.0 } );
      wedge.InsertChild( c, Matrix4d.Scale( 4.0 ) * Matrix4d.CreateTranslation( 0.0, -2.0, -2.0 ) );
      cubanFlag.InsertChild( wedge, Matrix4d.Scale( 0.7 ) * Matrix4d.CreateTranslation( -2.0001, -0.8, 0.4999 ) );
      flags.InsertChild( cubanFlag, Matrix4d.Scale( 0.7 ) * Matrix4d.RotateY( Math.PI / 8 ) * Matrix4d.CreateTranslation( -4.0, -1.0, 0.0 ) );
    }

    /// <summary>
    /// Infinite plane with a sphere on it
    /// </summary>
    public static void SpherePlane ( IRayScene sc )
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
      pl.SetAttribute( PropertyName.COLOR, new double[] { 0.5, 0.0, 0.0 } );
      pl.SetAttribute( PropertyName.TEXTURE, new CheckerTexture( 1.0, 1.0, new double[] { 1.0, 1.0, 1.0 } ) );
      root.InsertChild( pl, Matrix4d.RotateX( -MathHelper.PiOver2 ) * Matrix4d.CreateTranslation( 0.0, -1.0, 0.0 ) );
    }

    /// <summary>
    /// Infinite plane with two spheres on it (one of them is transparent)
    /// </summary>
    public static void TwoSpheres ( IRayScene sc, string param )
    {
      Debug.Assert( sc != null );

      // CSG scene:
      CSGInnerNode root = new CSGInnerNode( SetOperation.Union );
      root.SetAttribute( PropertyName.REFLECTANCE_MODEL, new PhongModel() );
      root.SetAttribute( PropertyName.MATERIAL, new PhongMaterial( new double[] { 1.0, 0.8, 0.1 }, 0.1, 0.6, 0.4, 128 ) );
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
      sc.Sources.Add( new PointLightSource( new Vector3d( -5.0, 4.0, -3.0 ), 1.2 ) );

      // --- NODE DEFINITIONS ----------------------------------------------------

      // Params dictionary:
      Dictionary<string, string> p = Util.ParseKeyValueList( param );

      // n = <index-of-refraction>
      double n = 1.6;
      Util.TryParse( p, "n", ref n );

      // Transparent sphere:
      Sphere s;
      s = new Sphere();
      PhongMaterial pm = new PhongMaterial( new double[] { 0.0, 0.2, 0.1 }, 0.05, 0.05, 0.1, 128 );
      pm.n = n;
      pm.Kt = 0.9;
      s.SetAttribute( PropertyName.MATERIAL, pm );
      root.InsertChild( s, Matrix4d.Identity );

      // Opaque sphere:
      s = new Sphere();
      root.InsertChild( s, Matrix4d.Scale( 1.2 ) * Matrix4d.CreateTranslation( 1.5, 0.2, 2.4 ) );

      // Infinite plane with checker:
      Plane pl = new Plane();
      pl.SetAttribute( PropertyName.COLOR, new double[] { 0.3, 0.0, 0.0 } );
      pl.SetAttribute( PropertyName.TEXTURE, new CheckerTexture( 0.6, 0.6, new double[] { 1.0, 1.0, 1.0 } ) );
      root.InsertChild( pl, Matrix4d.RotateX( -MathHelper.PiOver2 ) * Matrix4d.CreateTranslation( 0.0, -1.0, 0.0 ) );
    }

    public static ISceneNode flake ( int depth, double coef )
    {
      Matrix4d mat = Matrix4d.Scale( coef ) * Matrix4d.CreateTranslation( 0.0, 1.0 + coef, 0.0 );
      CSGInnerNode sf = new CSGInnerNode( SetOperation.Union );

      double a = Math.Atan2( 1.0, Math.Sqrt( 2.0 ) ) * 2.0;
      Matrix4d m1 = mat * Matrix4d.RotateX( a );
      Matrix4d m2 = mat * Matrix4d.RotateX( a + Math.PI );

      ISceneNode ch;

      Sphere s = new Sphere();
      sf.InsertChild( s, Matrix4d.Identity );

      if ( --depth > 0 )
      {
        ch = flake( depth, coef );
        sf.InsertChild( ch, mat );

        ch = flake( depth, coef );
        sf.InsertChild( ch, m1 );
        ch = flake( depth, coef );
        sf.InsertChild( ch, m2 );

        ch = flake( depth, coef );
        sf.InsertChild( ch, m1 * Matrix4d.RotateY( 2 * Math.PI / 3 ) );
        ch = flake( depth, coef );
        sf.InsertChild( ch, m2 * Matrix4d.RotateY( 2 * Math.PI / 3 ) );

        ch = flake( depth, coef );
        sf.InsertChild( ch, m1 * Matrix4d.RotateY( 4 * Math.PI / 3 ) );
        ch = flake( depth, coef );
        sf.InsertChild( ch, m2 * Matrix4d.RotateY( 4 * Math.PI / 3 ) );
      }

      return sf;
    }

    /// <summary>
    /// Infinite plane with limited transparent sphereflake on it
    /// </summary>
    public static void SphereFlake ( IRayScene sc, string param = null )
    {
      Debug.Assert( sc != null );

      // CSG scene:
      CSGInnerNode root = new CSGInnerNode( SetOperation.Union );
      root.SetAttribute( PropertyName.REFLECTANCE_MODEL, new PhongModel() );
      root.SetAttribute( PropertyName.MATERIAL, new PhongMaterial( new double[] { 1.0, 0.8, 0.1 }, 0.1, 0.6, 0.4, 128 ) );
      sc.Intersectable = root;

      // Background color:
      sc.BackgroundColor = new double[] { 0.0, 0.05, 0.07 };

      // Camera:
      sc.Camera = new StaticCamera( new Vector3d( 1.6, 2.2, -7.0 ),
                                    new Vector3d( -0.12, -0.3, 1.0 ),
                                    50.0 );

      // Light sources:
      sc.Sources = new LinkedList<ILightSource>();
      sc.Sources.Add( new AmbientLightSource( 0.8 ) );
      sc.Sources.Add( new PointLightSource( new Vector3d( -5.0, 4.0, -3.0 ), 1.2 ) );

      // --- NODE DEFINITIONS ----------------------------------------------------

      // Params dictionary:
      Dictionary<string, string> p = Util.ParseKeyValueList( param );

      // depth = <depth>
      int depth = 5;
      Util.TryParse( p, "depth", ref depth );

      // n = <index-of-refraction>
      double n = 1.6;
      Util.TryParse( p, "n", ref n );

      // coef = <scale-coef>
      double coef = 0.4;
      Util.TryParse( p, "coef", ref coef );

      // mat = {mirror|glass|diffuse}
      PhongMaterial pm = new PhongMaterial( new double[] { 1.0, 0.6, 0.1 }, 0.1, 0.85, 0.05, 16 );
      string mat;
      if ( p.TryGetValue( "mat", out mat ) )
        switch ( mat )
        {
          case "mirror":
            pm = new PhongMaterial( new double[] { 1.0, 1.0, 0.8 }, 0.0, 0.1, 0.9, 128 );
            break;

          case "glass":
            pm = new PhongMaterial( new double[] { 0.0, 0.2, 0.1 }, 0.05, 0.05, 0.1, 128 );
            pm.n = n;
            pm.Kt = 0.9;
            break;
        }

      // Sphere flake - recursive definition:
      ISceneNode sf = flake( depth, coef );
      sf.SetAttribute( PropertyName.MATERIAL, pm );
      root.InsertChild( sf, Matrix4d.Identity );

      // Infinite plane with checker:
      Plane pl = new Plane();
      pl.SetAttribute( PropertyName.COLOR, new double[] { 0.3, 0.0, 0.0 } );
      pl.SetAttribute( PropertyName.TEXTURE, new CheckerTexture( 0.6, 0.6, new double[] { 1.0, 1.0, 1.0 } ) );
      root.InsertChild( pl, Matrix4d.RotateX( -MathHelper.PiOver2 ) * Matrix4d.CreateTranslation( 0.0, -1.0, 0.0 ) );
    }

    /// <summary>
    /// Test scene for cubes
    /// </summary>
    public static void Cubes ( IRayScene sc, string param )
    {
      Debug.Assert( sc != null );

      // CSG scene:
      CSGInnerNode root = new CSGInnerNode( SetOperation.Union );
      root.SetAttribute( PropertyName.REFLECTANCE_MODEL, new PhongModel() );
      root.SetAttribute( PropertyName.MATERIAL, new PhongMaterial( new double[] { 1.0, 0.6, 0.1 }, 0.1, 0.8, 0.2, 16 ) );
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

      // Params dictionary:
      Dictionary<string, string> p = Util.ParseKeyValueList( param );

      // n = <index-of-refraction>
      double n = 1.6;
      Util.TryParse( p, "n", ref n );

      // mat = {mirror|glass|diffuse}
      PhongMaterial pm = new PhongMaterial( new double[] { 1.0, 0.6, 0.1 }, 0.1, 0.8, 0.2, 16 );
      string mat;
      if ( p.TryGetValue( "mat", out mat ) )
        switch ( mat )
        {
          case "mirror":
            pm = new PhongMaterial( new double[] { 1.0, 1.0, 0.8 }, 0.0, 0.1, 0.9, 128 );
            break;

          case "glass":
            pm = new PhongMaterial( new double[] { 0.0, 0.2, 0.1 }, 0.05, 0.05, 0.1, 128 );
            pm.n = n;
            pm.Kt = 0.9;
            break;
        }

      // Base plane
      Plane pl = new Plane();
      pl.SetAttribute( PropertyName.COLOR, new double[] { 0.6, 0.0, 0.0 } );
      pl.SetAttribute( PropertyName.TEXTURE, new CheckerTexture( 0.5, 0.5, new double[] { 1.0, 1.0, 1.0 } ) );
      root.InsertChild( pl, Matrix4d.RotateX( -MathHelper.PiOver2 ) * Matrix4d.CreateTranslation( 0.0, -1.0, 0.0 ) );

      // Cubes
      Cube c;
      // front row:
      c = new Cube();
      root.InsertChild( c, Matrix4d.RotateY( 0.6 ) * Matrix4d.CreateTranslation( -3.5, -0.8, 0.0 ) );
      c.SetAttribute( PropertyName.MATERIAL, pm );
      c = new Cube();
      root.InsertChild( c, Matrix4d.RotateY( 1.2 ) * Matrix4d.CreateTranslation( -1.5, -0.8, 0.0 ) );
      c.SetAttribute( PropertyName.MATERIAL, pm );
      c = new Cube();
      root.InsertChild( c, Matrix4d.RotateY( 1.8 ) * Matrix4d.CreateTranslation( 0.5, -0.8, 0.0 ) );
      c.SetAttribute( PropertyName.MATERIAL, pm );
      c = new Cube();
      root.InsertChild( c, Matrix4d.RotateY( 2.4 ) * Matrix4d.CreateTranslation( 2.5, -0.8, 0.0 ) );
      c.SetAttribute( PropertyName.MATERIAL, pm );
      c = new Cube();
      root.InsertChild( c, Matrix4d.RotateY( 3.0 ) * Matrix4d.CreateTranslation( 4.5, -0.8, 0.0 ) );
      c.SetAttribute( PropertyName.MATERIAL, pm );
      // back row:
      c = new Cube();
      root.InsertChild( c, Matrix4d.RotateX( 3.5 ) * Matrix4d.CreateTranslation( -4.0, 1.0, 2.0 ) );
      c.SetAttribute( PropertyName.MATERIAL, pm );
      c = new Cube();
      root.InsertChild( c, Matrix4d.RotateX( 3.0 ) * Matrix4d.CreateTranslation( -2.5, 1.0, 2.0 ) );
      c.SetAttribute( PropertyName.MATERIAL, pm );
      c = new Cube();
      root.InsertChild( c, Matrix4d.RotateX( 2.5 ) * Matrix4d.CreateTranslation( -1.0, 1.0, 2.0 ) );
      c.SetAttribute( PropertyName.MATERIAL, pm );
      c = new Cube();
      root.InsertChild( c, Matrix4d.RotateX( 2.0 ) * Matrix4d.CreateTranslation( 0.5, 1.0, 2.0 ) );
      c.SetAttribute( PropertyName.MATERIAL, pm );
      c = new Cube();
      root.InsertChild( c, Matrix4d.RotateX( 1.5 ) * Matrix4d.CreateTranslation( 2.0, 1.0, 2.0 ) );
      c.SetAttribute( PropertyName.MATERIAL, pm );
      c = new Cube();
      root.InsertChild( c, Matrix4d.RotateX( 1.0 ) * Matrix4d.CreateTranslation( 3.5, 1.0, 2.0 ) );
      c.SetAttribute( PropertyName.MATERIAL, pm );
      c = new Cube();
      root.InsertChild( c, Matrix4d.RotateX( 0.5 ) * Matrix4d.CreateTranslation( 5.0, 1.0, 2.0 ) );
      c.SetAttribute( PropertyName.MATERIAL, pm );
    }

    /// <summary>
    /// Test scene for cylinders
    /// </summary>
    public static void Cylinders ( IRayScene sc )
    {
      Debug.Assert( sc != null );

      // CSG scene:
      CSGInnerNode root = new CSGInnerNode( SetOperation.Union );
      root.SetAttribute( PropertyName.REFLECTANCE_MODEL, new PhongModel() );
      root.SetAttribute( PropertyName.MATERIAL, new PhongMaterial( new double[] { 0.6, 0.0, 0.0 }, 0.15, 0.8, 0.15, 16 ) );
      sc.Intersectable = root;

      // Background color:
      sc.BackgroundColor = new double[] { 0.0, 0.05, 0.07 };

      // Camera:
      sc.Camera = new StaticCamera( new Vector3d( 0.7, 3.0, -10.0 ),
                                    new Vector3d( 0.0, -0.2, 1.0 ),
                                    50.0 );

      // Light sources:
      sc.Sources = new LinkedList<ILightSource>();
      sc.Sources.Add( new AmbientLightSource( 1.0 ) );
      sc.Sources.Add( new PointLightSource( new Vector3d( -5.0, 3.0, -3.0 ), 1.6 ) );

      // --- NODE DEFINITIONS ----------------------------------------------------

      // Base plane
      Plane pl = new Plane();
      pl.SetAttribute( PropertyName.COLOR, new double[] { 0.0, 0.2, 0.0 } );
      pl.SetAttribute( PropertyName.TEXTURE, new CheckerTexture( 0.5, 0.5, new double[] { 1.0, 1.0, 1.0 } ) );
      root.InsertChild( pl, Matrix4d.RotateX( -MathHelper.PiOver2 ) * Matrix4d.CreateTranslation( 0.0, -1.0, 0.0 ) );

      // Cylinders
      Cylinder c = new Cylinder();
      root.InsertChild( c, Matrix4d.RotateX( MathHelper.PiOver2 ) * Matrix4d.CreateTranslation( -2.1, 0.0, 1.0 ) );
      c = new Cylinder();
      c.SetAttribute( PropertyName.COLOR, new double[] { 0.2, 0.0, 0.7 } );
      c.SetAttribute( PropertyName.TEXTURE, new CheckerTexture( 12.0, 1.0, new double[] { 0.0, 0.0, 0.3 } ) );
      root.InsertChild( c, Matrix4d.RotateY( -0.4 ) * Matrix4d.CreateTranslation( 1.0, 0.0, 1.0 ) );
      c = new Cylinder( 0.0, 100.0 );
      c.SetAttribute( PropertyName.COLOR, new double[] { 0.1, 0.7, 0.0 } );
      root.InsertChild( c, Matrix4d.RotateY( 0.2 ) * Matrix4d.CreateTranslation( 5.0, 0.3, 4.0 ) );
      c = new Cylinder( -0.5, 0.5 );
      c.SetAttribute( PropertyName.COLOR, new double[] { 0.8, 0.6, 0.0 } );
      root.InsertChild( c, Matrix4d.Scale( 2.0 ) * Matrix4d.RotateX( 1.2 ) * Matrix4d.CreateTranslation( 2.0, 1.8, 16.0 ) );
    }

    /// <summary>
    /// Test scene by Adam Hanka (c) 2012
    /// </summary>
    public static void Circus ( IRayScene sc )
    {
      Debug.Assert( sc != null );

      // CSG scene:
      CSGInnerNode root = new CSGInnerNode( SetOperation.Union );
      root.SetAttribute( PropertyName.REFLECTANCE_MODEL, new PhongModel() );
      root.SetAttribute( PropertyName.MATERIAL, new PhongMaterial( new double[] { 0.5, 0.5, 0.5 }, 0.2, 0.8, 0.1, 16 ) );
      sc.Intersectable = root;

      // Background color:
      sc.BackgroundColor = new double[] { 0.5, 0.7, 0.6 };

      // Camera:
      sc.Camera = new StaticCamera( new Vector3d( 0.0, 0.0, -11.0 ),
                                    new Vector3d( 0.0, 0.0, 1.0 ),
                                    60.0 );

      // Light sources:
      sc.Sources = new LinkedList<ILightSource>();
      sc.Sources.Add( new AmbientLightSource( 1.0 ) );
      sc.Sources.Add( new PointLightSource( new Vector3d( -5.0, 3.0, -3.0 ), 1.2 ) );

      // --- NODE DEFINITIONS ----------------------------------------------------

      // Vlevo nahore: hlava panacka - vyuziva implementace Xor na rty, vnitrek ust je tvoren pomoci Intersection, jinak je zalozena na Union a Difference
      root.InsertChild( head(), Matrix4d.CreateTranslation( -6, 1.5, 0 ) * Matrix4d.RotateY( Math.PI / 10 ) * Matrix4d.Scale( 0.7 ) );

      // Uprostred nahode: testovani funkcnosti difference pro sest kouli
      root.InsertChild( test6Spheres( SetOperation.Difference ), Matrix4d.CreateTranslation( 0, 1.5, 0 ) * Matrix4d.Scale( 0.7 ) );

      // Vpravo nahore: testovani operace Xor tak, ze se funkce test6Spheres vola jednou s Union, jednou s Xor a vysledky se odectou
      root.InsertChild( testXor2(), Matrix4d.CreateTranslation( 6, 1.5, 0 ) * Matrix4d.RotateX( Math.PI / 3 ) * Matrix4d.RotateY( Math.PI / 4 ) * Matrix4d.Scale( 0.6 ) );

      // Vlevo dole: test xoru s Cylindry - opet jako Union - Xor = vysledek
      root.InsertChild( testWithCylindersXor(), Matrix4d.CreateTranslation( -5, -2.5, 0 ) * Matrix4d.Scale( 0.7 ) );

      // Uprostred dole: prosty Xor valcu
      root.InsertChild( testWithCylinders( SetOperation.Xor ), Matrix4d.CreateTranslation( 0, -2.5, 0 ) * Matrix4d.Scale( 0.7 ) );

      // Vpravo dole: testovani Intersection - prosty prusecik dvou kouli
      root.InsertChild( test2Spheres( SetOperation.Intersection ), Matrix4d.CreateTranslation( 4, -1.5, 0 ) );
    }

    public static CSGInnerNode head ()
    {
      CSGInnerNode root = new CSGInnerNode( SetOperation.Union );

      double[] skinColor = new double[] { 1, .8, .65 };
      double[] whiteColor = new double[] { 1, 1, 1 };
      double[] blackColor = new double[] { 0, 0, 0 };
      double[] blueColor = new double[] { 0, 0, 1 };
      double[] redColor = new double[] { 1, 0, 0 };

      CSGInnerNode hlava = new CSGInnerNode( SetOperation.Difference );
      root.InsertChild( hlava, Matrix4d.Identity );

      Sphere s = new Sphere();
      s.SetAttribute( PropertyName.COLOR, skinColor );
      hlava.InsertChild( s, Matrix4d.Scale( 2.3 ) );

      // klobouk
      CSGInnerNode klobouk = new CSGInnerNode( SetOperation.Union );

      Cylinder klb = new Cylinder( -0.5, 0.5 );
      klb.SetAttribute( PropertyName.COLOR, blackColor );
      klobouk.InsertChild( klb, Matrix4d.Scale( 2, 2, 1.4 ) * Matrix4d.RotateX( Math.PI / 2 ) * Matrix4d.CreateTranslation( 0, 0.5, 0 ) );

      Cylinder ksilt = new Cylinder( -0.5, 0.5 );
      ksilt.SetAttribute( PropertyName.COLOR, blackColor );
      klobouk.InsertChild( ksilt, Matrix4d.Scale( 3, 3, .1 ) * Matrix4d.RotateX( Math.PI / 2 ) );

      root.InsertChild( klobouk, Matrix4d.CreateTranslation( 0, 1.25, 0 ) );

      // levy ocni dulek
      s = new Sphere();
      s.SetAttribute( PropertyName.COLOR, skinColor );
      hlava.InsertChild( s, Matrix4d.Scale( 0.7 ) * Matrix4d.CreateTranslation( -0.7, .3, -1.8 ) );

      // pravy ocni dulek
      s = new Sphere();
      s.SetAttribute( PropertyName.COLOR, skinColor );
      hlava.InsertChild( s, Matrix4d.Scale( 0.7 ) * Matrix4d.CreateTranslation( 0.7, .3, -1.8 ) );

      // prave oko
      CSGInnerNode oko = new CSGInnerNode( SetOperation.Union );
      root.InsertChild( oko, Matrix4d.CreateTranslation( 0.70, 0.3, -0.5 ) );

      Sphere bulva = new Sphere();
      bulva.SetAttribute( PropertyName.COLOR, whiteColor );
      oko.InsertChild( bulva, Matrix4d.Scale( 1.3 ) );

      Sphere cocka = new Sphere();
      cocka.SetAttribute( PropertyName.COLOR, blueColor );
      oko.InsertChild( cocka, Matrix4d.Scale( 0.3 ) * Matrix4d.CreateTranslation( 0, 0, -1.2 ) );

      // leve oko
      oko = new CSGInnerNode( SetOperation.Union );
      root.InsertChild( oko, Matrix4d.CreateTranslation( -0.70, 0.3, -0.5 ) );

      bulva = new Sphere();
      bulva.SetAttribute( PropertyName.COLOR, whiteColor );
      oko.InsertChild( bulva, Matrix4d.Scale( 1.3 ) );

      cocka = new Sphere();
      cocka.SetAttribute( PropertyName.COLOR, blueColor );
      oko.InsertChild( cocka, Matrix4d.Scale( 0.3 ) * Matrix4d.CreateTranslation( 0, 0, -1.2 ) );

      // nos
      Cylinder nos = new Cylinder( -0.5, 0.5 );
      nos.SetAttribute( PropertyName.COLOR, skinColor );
      root.InsertChild( nos, Matrix4d.Scale( 0.2, 0.2, 0.6 ) * Matrix4d.CreateTranslation( 0, 0, -2.65 ) );

      // usta
      CSGInnerNode usta = new CSGInnerNode( SetOperation.Xor );

      Cylinder horniRet = new Cylinder( -0.5, 0.5 );
      horniRet.SetAttribute( PropertyName.COLOR, redColor );
      usta.InsertChild( horniRet, Matrix4d.CreateTranslation( 0, -0.3, 0 ) );

      Cylinder dolniRet = new Cylinder( -0.5, 0.5 );
      dolniRet.SetAttribute( PropertyName.COLOR, redColor );
      usta.InsertChild( dolniRet, Matrix4d.CreateTranslation( 0, 0.3, 0 ) );

      root.InsertChild( usta, Matrix4d.Scale( 0.7, 0.3, 1 ) * Matrix4d.CreateTranslation( 0, -0.7, -1.85 ) );

      // vnitrek ust
      CSGInnerNode vnitrekUst = new CSGInnerNode( SetOperation.Intersection );
      horniRet = new Cylinder( -0.5, 0.5 );
      horniRet.SetAttribute( PropertyName.COLOR, blackColor );
      vnitrekUst.InsertChild( horniRet, Matrix4d.CreateTranslation( 0, -0.3, 0 ) );

      dolniRet = new Cylinder( -0.5, 0.5 );
      dolniRet.SetAttribute( PropertyName.COLOR, blackColor );
      vnitrekUst.InsertChild( dolniRet, Matrix4d.CreateTranslation( 0, 0.3, 0 ) );

      usta.InsertChild( vnitrekUst, Matrix4d.CreateTranslation( 0, 0, 0.1 ) );

      return root;
    }

    public static CSGInnerNode testWithCylindersXor ()
    {
      CSGInnerNode root = new CSGInnerNode( SetOperation.Difference );
      root.InsertChild( testWithCylinders( SetOperation.Union ), Matrix4d.Identity );
      root.InsertChild( testWithCylinders( SetOperation.Xor ), Matrix4d.Identity );

      return root;
    }

    public static CSGInnerNode testWithCylinders ( SetOperation op )
    {
      // scene:
      CSGInnerNode root = new CSGInnerNode( op );
      int num = 5;

      for ( int i = 0; i < num; ++i )
      {
        double j = i;
        Cylinder c = new Cylinder( -0.5, 0.5 );
        c.SetAttribute( PropertyName.COLOR, new double[] { j / num, 1.0 - j / num, 0 } );
        root.InsertChild( c, Matrix4d.RotateX( Math.PI / 4 ) * Matrix4d.CreateTranslation( -1 + 0.6 * i, 0, 0 ) );
      }

      return root;
    }

    public static CSGInnerNode test2Spheres ( SetOperation op )
    {
      // scene:
      CSGInnerNode root = new CSGInnerNode( op );

      Sphere s = new Sphere();
      s.SetAttribute( PropertyName.COLOR, new double[] { 1, 0.3, 1 } );
      root.InsertChild( s, Matrix4d.CreateTranslation( -0.3, 0.0, 0 ) );

      s = new Sphere();
      s.SetAttribute( PropertyName.COLOR, new double[] { 1, 0.3, 0 } );
      root.InsertChild( s, Matrix4d.CreateTranslation( 0.2, 0, -0.2 ) );

      return root;
    }

    public static CSGInnerNode test6Spheres ( SetOperation op )
    {
      // scene:
      CSGInnerNode root = new CSGInnerNode( op );

      Sphere s = new Sphere();
      s.SetAttribute( PropertyName.COLOR, new double[] { 1, 0.3, 1 } );
      root.InsertChild( s, Matrix4d.Scale( 2 ) );

      s = new Sphere();
      s.SetAttribute( PropertyName.COLOR, new double[] { 0.5, 0.8, 0.2 } );
      root.InsertChild( s, Matrix4d.CreateTranslation( 0, 0, -2 ) );

      s = new Sphere();
      s.SetAttribute( PropertyName.COLOR, new double[] { 1, 0.3, 0 } );
      root.InsertChild( s, Matrix4d.CreateTranslation( 0, 1, -0.5 ) );

      s = new Sphere();
      s.SetAttribute( PropertyName.COLOR, new double[] { 1, 0.3, 0 } );
      root.InsertChild( s, Matrix4d.CreateTranslation( 1, 0, -0.5 ) );

      s = new Sphere();
      s.SetAttribute( PropertyName.COLOR, new double[] { 1, 0.3, 0 } );
      root.InsertChild( s, Matrix4d.CreateTranslation( 0, -1, -0.5 ) );

      s = new Sphere();
      s.SetAttribute( PropertyName.COLOR, new double[] { 1, 0.3, 0 } );
      root.InsertChild( s, Matrix4d.CreateTranslation( -1, 0, -0.5 ) );

      return root;
    }

    public static CSGInnerNode testXor2 ()
    {
      // testovani probiha podle vzoru (a union b) - (a xor b) = (a and b)
      // takze udelame levou stranu rovnice a to nam da intersection

      CSGInnerNode root = new CSGInnerNode( SetOperation.Difference );

      root.InsertChild( test6Spheres( SetOperation.Union ), Matrix4d.Identity );
      root.InsertChild( test6Spheres( SetOperation.Xor ), Matrix4d.Identity );

      return root;
    }

    /// <summary>
    /// Test scene for Toruses by Jan Navratil, (c) 2012
    /// </summary>
    public static void Toroids ( IRayScene sc )
    {
      Debug.Assert( sc != null );

      // CSG scene:
      CSGInnerNode root = new CSGInnerNode( SetOperation.Union );
      root.SetAttribute( PropertyName.REFLECTANCE_MODEL, new PhongModel() );
      root.SetAttribute( PropertyName.MATERIAL, new PhongMaterial( new double[] { 0.5, 0.5, 0.5 }, 0.2, 0.7, 0.2, 16 ) );
      sc.Intersectable = root;

      // Background color:
      sc.BackgroundColor = new double[] { 0.0, 0.15, 0.2 };

      // Camera:
      sc.Camera = new StaticCamera( new Vector3d( 0.0, 0.0, -10.0 ),
                                    new Vector3d( 0.0, -0.04, 1.0 ),
                                    60.0 );

      // Light sources:
      sc.Sources = new LinkedList<ILightSource>();
      sc.Sources.Add( new AmbientLightSource( 1.8 ) );
      sc.Sources.Add( new PointLightSource( new Vector3d( -5.0, 3.0, -3.0 ), 2.0 ) );

      // --- NODE DEFINITIONS ----------------------------------------------------

      Sphere s;
      Torus t;

      t = new Torus( 2.0, 1.0 );
      t.SetAttribute( PropertyName.COLOR, new double[] { 0, 0, 0.7 } );
      t.SetAttribute( PropertyName.TEXTURE, new CheckerTexture( 50, 20, new double[] { 0.9, 0.9, 0 } ) );
      root.InsertChild( t, Matrix4d.Scale( 0.8 ) * Matrix4d.CreateRotationX( 0.9 ) * Matrix4d.CreateTranslation( 2.7, 0.6, 0.0 ) );

      t = new Torus( 1.0, 1.5 );
      t.SetAttribute( PropertyName.COLOR, new double[] { 1, 0, 0 } );
      t.SetAttribute( PropertyName.TEXTURE, new CheckerTexture( 40, 40, new double[] { 0.8, 1.0, 1.0 } ) );

      CSGInnerNode donut = new CSGInnerNode( SetOperation.Difference );
      donut.InsertChild( t, Matrix4d.Identity );
      s = new Sphere();
      s.SetAttribute( PropertyName.COLOR, new double[] { 0.5, 0.5, 0.5 } );
      s.SetAttribute( PropertyName.TEXTURE, new CheckerTexture( 100, 100, new double[] { 1, 0.5, 0 } ) );
      donut.InsertChild( s, Matrix4d.Scale( 3 ) * Matrix4d.CreateTranslation( 0, -3, 0 ) );
      root.InsertChild( donut, Matrix4d.CreateRotationX( 0.8 ) * Matrix4d.CreateTranslation( -2.5, 0.0, 0.0 ) );

      int number = 14;
      for ( int i = 0; i < number; i++ )
      {
        t = new Torus( 0.2, 0.01 + 0.185 * i / number );
        t.SetAttribute( PropertyName.COLOR, new double[] { 1, 1, 1 } );
        root.InsertChild( t, Matrix4d.CreateRotationX( 0.5 * i ) * Matrix4d.CreateTranslation( 10.0 * (1.0 * i / number) - 4.7, -2.5, 0 ) );
      }
    }

    /// <summary>
    /// Test scene for BezierSurface.
    /// </summary>
    public static void Bezier ( IRayScene sc )
    {
      Debug.Assert( sc != null );

      // CSG scene:
      CSGInnerNode root = new CSGInnerNode( SetOperation.Union );
      root.SetAttribute( PropertyName.REFLECTANCE_MODEL, new PhongModel() );
      root.SetAttribute( PropertyName.MATERIAL, new PhongMaterial( new double[] { 1.0, 0.8, 0.1 }, 0.1, 0.5, 0.5, 64 ) );
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

      // Bezier patch (not yet):
      BezierSurface b = new BezierSurface( 1, 2, new double[] {
        0.0, 0.0, 3.0,  // row 0
        1.0, 0.0, 3.0,
        2.0, 0.0, 3.0,
        3.0, 0.0, 3.0,
        4.0, 0.0, 3.0,
        5.0, 0.0, 3.0,
        6.0, 0.0, 3.0,
        0.0, 0.0, 2.0,  // row 1
        1.0, 0.0, 2.0,
        2.0, 3.0, 2.0,
        3.0, 3.0, 2.0,
        4.0, 3.0, 2.0,
        5.0, 0.0, 2.0,
        6.0, 0.0, 2.0,
        0.0, 0.0, 1.0,  // row 2
        1.0, 0.0, 1.0,
        2.0, 0.0, 1.0,
        3.0, 1.5, 1.0,
        4.0, 3.0, 1.0,
        5.0, 0.0, 1.0,
        6.0, 0.0, 1.0,
        0.0, 0.0, 0.0,  // row 3
        1.0, 0.0, 0.0,
        2.0, 0.0, 0.0,
        3.0, 0.0, 0.0,
        4.0, 0.0, 0.0,
        5.0, 0.0, 0.0,
        6.0, 0.0, 0.0,
        } );
      b.SetAttribute( PropertyName.TEXTURE, new CheckerTexture( 10.5, 12.0, new double[] { 0.0, 0.0, 0.1 } ) );
      root.InsertChild( b, Matrix4d.RotateY( -0.4 ) * Matrix4d.CreateTranslation( -1.1, -0.9, 0.0 ) );

      // Cylinders for reflections..
      Cylinder c = new Cylinder();
      c.SetAttribute( PropertyName.MATERIAL, new PhongMaterial( new double[] { 0.0, 0.6, 0.0 }, 0.2, 0.6, 0.3, 8 ) );
      root.InsertChild( c, Matrix4d.Scale( 0.15 ) * Matrix4d.RotateX( MathHelper.PiOver2 ) * Matrix4d.CreateTranslation( -0.4, 0.0, 0.0 ) );
      c = new Cylinder();
      c.SetAttribute( PropertyName.MATERIAL, new PhongMaterial( new double[] { 0.8, 0.2, 0.0 }, 0.2, 0.6, 0.3, 8 ) );
      root.InsertChild( c, Matrix4d.Scale( 0.2 ) * Matrix4d.RotateX( MathHelper.PiOver2 ) * Matrix4d.CreateTranslation( -1.9, 0.0, 3.0 ) );

      // Infinite plane with checker:
      Plane pl = new Plane();
      pl.SetAttribute( PropertyName.COLOR, new double[] { 0.3, 0.0, 0.0 } );
      pl.SetAttribute( PropertyName.TEXTURE, new CheckerTexture( 0.6, 0.6, new double[] { 1.0, 1.0, 1.0 } ) );
      root.InsertChild( pl, Matrix4d.RotateX( -MathHelper.PiOver2 ) * Matrix4d.CreateTranslation( 0.0, -1.0, 0.0 ) );
    }

    /// <summary>
    /// Find files in the current directory and three more levels up the file-system tree.
    /// Suitable for data/* files accessible from xxx/bin/Release/xxx.exe
    /// </summary>
    /// <param name="names"></param>
    /// <returns></returns>
    public static string[] SmartFindFiles ( string[] names )
    {
      if ( names == null || names.Length == 0 )
        return null;

      int len = names.Length;
      string[] result = new string[ len ];
      for ( int i = 0; i < len; i++ )
        if ( names[ i ] == null || names[ i ].Length == 0 )
          result[ i ] = "";
        else
          if ( names[ i ].Contains( @"\" ) )
          result[ i ] = names[ i ];
        else
          try
          {
            string[] search = Directory.GetFiles( ".", names[ i ], SearchOption.AllDirectories );
            if ( search.Length > 0 )
            {
              result[ i ] = search[ 0 ];
              continue;
            }
            search = Directory.GetFiles( "..", names[ i ], SearchOption.AllDirectories );
            if ( search.Length > 0 )
            {
              result[ i ] = search[ 0 ];
              continue;
            }
            search = Directory.GetFiles( @"..\..", names[ i ], SearchOption.AllDirectories );
            if ( search.Length > 0 )
            {
              result[ i ] = search[ 0 ];
              continue;
            }
            search = Directory.GetFiles( @"..\..\..", names[ i ], SearchOption.AllDirectories );
            if ( search.Length > 0 )
            {
              result[ i ] = search[ 0 ];
              continue;
            }
          }
          catch ( IOException )
          {
          }
          catch ( UnauthorizedAccessException )
          {
          }

      return result;
    }

    /// <summary>
    /// Generic OBJ-loading scene definition routine.
    /// </summary>
    /// <param name="dir">Viewing direction of a camera.</param>
    /// <param name="FoVy">Field of View in degrees.</param>
    /// <param name="correction">Value less than 1.0 brings camera nearer.</param>
    /// <param name="name">OBJ file-name.</param>
    /// <param name="names">Substitute file-name[s].</param>
    /// <returns>Number of triangles.</returns>
    protected static long SceneObj ( IRayScene sc, Vector3d dir, double FoVy, double correction, string name, string[] names, double[] surfaceColor )
    {
      Debug.Assert( sc != null );

      Vector3 center = Vector3.Zero;   // center of the mesh
      dir.Normalize();                 // normalized viewing vector of the camera
      float diameter = 2.0f;           // default scene diameter
      int faces = 0;

      // CSG scene:
      CSGInnerNode root = new CSGInnerNode( SetOperation.Union );

      // OBJ file to read:
      if ( names.Length == 0 ||
           names[ 0 ].Length == 0 )
        names = new string[] { name };

      string[] paths = Scenes.SmartFindFiles( names );
      if ( paths[ 0 ] == null || paths[ 0 ].Length == 0 )
      {
        for ( int i = 0; i < names.Length; i++ )
          if ( names[ i ].Length > 0 )
            names[ i ] += ".gz";
        paths = Scenes.SmartFindFiles( names );
      }
      if ( paths[ 0 ] == null || paths[ 0 ].Length == 0 )
        root.InsertChild( new Sphere(), Matrix4d.Identity );
      else
      {
        // B-rep scene construction:
        WavefrontObj objReader = new WavefrontObj();
        objReader.MirrorConversion = false;
        SceneBrep brep = new SceneBrep();
        faces = objReader.ReadBrep( paths[ 0 ], brep );
        brep.BuildCornerTable();
        diameter = brep.GetDiameter( out center );
        TriangleMesh m = new TriangleMesh( brep );
        root.InsertChild( m, Matrix4d.Identity );
      }

      root.SetAttribute( PropertyName.REFLECTANCE_MODEL, new PhongModel() );
      root.SetAttribute( PropertyName.MATERIAL, new PhongMaterial( surfaceColor, 0.2, 0.5, 0.4, 32 ) );
      root.SetAttribute( PropertyName.COLOR, surfaceColor );
      sc.Intersectable = root;

      // Background color:
      sc.BackgroundColor = new double[] { 0.0, 0.05, 0.07 };

      // Camera:
      double dist = (0.5 * diameter * correction) / Math.Tan( MathHelper.DegreesToRadians( (float)(0.5 * FoVy) ) );
      Vector3d cam = (Vector3d)center - dist * dir;
      sc.Camera = new StaticCamera( cam, dir, FoVy );

      // Light sources:
      sc.Sources = new LinkedList<ILightSource>();
      sc.Sources.Add( new AmbientLightSource( 0.8 ) );
      Vector3d lightDir = Vector3d.TransformVector( dir, Matrix4d.CreateRotationY( -2.0 ) );
      lightDir = Vector3d.TransformVector( lightDir, Matrix4d.CreateRotationZ( -0.8 ) );
      sc.Sources.Add( new PointLightSource( (Vector3d)center + diameter * lightDir, 1.0 ) );

      return faces;
    }

    /// <summary>
    /// Scene containing one instance of Utah Teapot read from the OBJ file.
    /// </summary>
    public static long TeapotObj ( IRayScene sc, string[] names )
    {
      return SceneObj( sc, new Vector3d( 0.1, -0.3, 0.9 ), 50.0, 0.9, "teapot.obj", names, new double[] { 1.0, 0.6, 0.0 } );
    }

    /// <summary>
    /// Scene containing one instance of a pitcher read from the OBJ file.
    /// </summary>
    public static long PitcherObj ( IRayScene sc, string[] names )
    {
      return SceneObj( sc, new Vector3d( 0.6, -0.3, 0.5 ), 60.0, 1.1, "pitcher.obj", names, new double[] { 0.3, 0.6, 0.0 } );
    }

    /// <summary>
    /// Scene containing one instance of the Buddha read from the OBJ file.
    /// </summary>
    public static long BuddhaObj ( IRayScene sc, string[] names )
    {
      return SceneObj( sc, new Vector3d( 0.4, -0.3, -0.9 ), 70.0, 1.1, "buddha.obj", names, new double[] { 1.0, 0.4, 0.0 } );
    }

    /// <summary>
    /// Scene containing one instance of toy-plane read from the OBJ file.
    /// </summary>
    public static long PlaneObj ( IRayScene sc, string[] names )
    {
      return SceneObj( sc, new Vector3d( -0.7, -0.25, 0.7 ), 70.0, 0.9, "toyplane.obj", names, new double[] { 0.2, 0.8, 0.0 } );
    }
  }
}
