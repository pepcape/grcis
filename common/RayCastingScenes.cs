using System;
using System.Collections.Generic;
using System.Diagnostics;
using OpenTK;

namespace Rendering
{
  /// <summary>
  /// Some interesting scenes created mostly by MFF UK students.
  /// http://cgg.mff.cuni.cz/~pepca/lectures/npgr004.current.html
  /// </summary>
  public class Scenes
  {
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
  }
}
