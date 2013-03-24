using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using MathSupport;
using OpenTK;
using Rendering;

namespace _049distributedrt
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
      return new DistributedRayTracing( scene );
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
      SupersamplingImageSynthesizer sis = new SupersamplingImageSynthesizer();
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

      // default image parameters?
      ImageWidth  = 800;
      ImageHeight = 540;
      numericSupersampling.Value = 16;
      checkMultithreading.Checked = true;
    }
  }
}

namespace Rendering
{
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
      root.SetAttribute( PropertyName.MATERIAL, new PhongMaterial( new double[] { 1.0, 0.8, 0.1 }, 0.1, 0.6, 0.4, 128 ) );
      sc.Intersectable = root;

      // Background color:
      sc.BackgroundColor = new double[] { 0.0, 0.05, 0.07 };

      // Camera:
      sc.Camera = new StaticCamera( new Vector3d( 0.7, 0.5, -5.0 ),
                                    new Vector3d( 0.0, -0.28, 1.0 ),
                                    50.0 );

      // Light sources:
      sc.Sources = new LinkedList<ILightSource>();
      sc.Sources.Add( new AmbientLightSource( 0.8 ) );
      //sc.Sources.Add( new PointLightSource( new Vector3d( -5.0, 4.0, -3.0 ), 1.2 ) );

      /* */
      // horizontal stick source:
      //RectangleLightSource rls = new RectangleLightSource( new Vector3d( -5.0, 4.0, -6.0 ),
      //                                                     new Vector3d( 0.0, 0.0, 6.0 ),
      //                                                     new Vector3d( 0.0, 0.1, 0.0 ), 1.8 );
      // vertical stick source:
      RectangleLightSource rls = new RectangleLightSource( new Vector3d( -5.0, 1.0, -3.0 ),
                                                           new Vector3d( 0.0, 0.0, 0.1 ),
                                                           new Vector3d( 0.0, 6.0, 0.0 ), 1.8 );
      // rectangle source:
      //RectangleLightSource rls = new RectangleLightSource( new Vector3d( -5.0, 1.0, -6.0 ),
      //                                                     new Vector3d( 0.0, 0.0, 6.0 ),
      //                                                     new Vector3d( 0.0, 6.0, 0.0 ), 1.8 );
      rls.Dim = new double[] { 1.0, 0.05, 0.0 };
      sc.Sources.Add( rls );
      /* */

      // --- NODE DEFINITIONS ----------------------------------------------------

      // Sphere:
      Sphere s = new Sphere();
      root.InsertChild( s, Matrix4d.Identity );

      // Infinite plane with checker:
      Plane pl = new Plane();
      pl.SetAttribute( PropertyName.COLOR, new double[] { 0.3, 0.0, 0.0 } );
      pl.SetAttribute( PropertyName.TEXTURE, new CheckerTexture( 0.6, 0.6, new double[] { 1.0, 1.0, 1.0 } ) );
      root.SetAttribute( PropertyName.MATERIAL, new PhongMaterial( new double[] { 1.0, 0.8, 0.1 }, 0.1, 0.3, 0.7, 16 ) );
      root.InsertChild( pl, Matrix4d.RotateX( -MathHelper.PiOver2 ) * Matrix4d.CreateTranslation( 0.0, -1.0, 0.0 ) );
    }
  }

  /// <summary>
  /// Distributed Ray-tracing.
  /// Feel free to override the shade() function.
  /// </summary>
  public class DistributedRayTracing : RayTracing
  {
    public DistributedRayTracing ( IRayScene sc ) : base( sc )
    {
    }

    /// <summary>
    /// Recursive shading function - computes color contribution of the given ray (shot from the
    /// origin 'p0' into direction vector 'p1''). Recursion is stopped
    /// by a hybrid method: 'importance' and 'level' are checked.
    /// Internal integration support.
    /// </summary>
    /// <param name="level">Current recursion depth.</param>
    /// <param name="importance">Importance of the current ray.</param>
    /// <param name="p0">Ray origin.</param>
    /// <param name="p1">Ray direction vector.</param>
    /// <param name="rank">Rank of this sample, 0 <= rank < total (for integration).</param>
    /// <param name="total">Total number of samples (for integration).</param>
    /// <param name="rnd">Global (per-thread) instance of the random generator.</param>
    /// <param name="color">Result color.</param>
    /// <returns>Hash-value (ray sub-signature) used for adaptive subsampling.</returns>
    protected override long shade ( int level, double importance, ref Vector3d p0, ref Vector3d p1,
                                    int rank, int total, RandomJames rnd, double[] color )
    {
      int bands = color.Length;
      LinkedList<Intersection> intersections = scene.Intersectable.Intersect( p0, p1 );
      Intersection.countRays++;
      Intersection i = Intersection.FirstIntersection( intersections, ref p1 );
      int b;

      if ( i == null )          // no intersection -> background color
      {
        Array.Copy( scene.BackgroundColor, color, bands );
        return 1L;
      }

      // there was at least one intersection
      i.Complete();

      // hash code for adaptive supersampling:
      long hash = i.Solid.GetHashCode();

      // apply all the textures fist..
      if ( i.Textures != null )
        foreach ( ITexture tex in i.Textures )
          hash = hash * HASH_TEXTURE + tex.Apply( i, rank, total, rnd );

      p1 = -p1;   // viewing vector
      p1.Normalize();

      if ( scene.Sources == null || scene.Sources.Count < 1 )
        // no light sources at all
        Array.Copy( i.SurfaceColor, color, bands );
      else
      {
        // apply the reflectance model for each source
        i.Material = (IMaterial)i.Material.Clone();
        i.Material.Color = i.SurfaceColor;
        Array.Clear( color, 0, bands );

        foreach ( ILightSource source in scene.Sources )
        {
          Vector3d dir;
          double[] intensity = source.GetIntensity( i, rank, total, rnd, out dir );
          if ( intensity != null )
          {
            if ( DoShadows && !dir.Equals( Vector3d.Zero ) )
            {
              intersections = scene.Intersectable.Intersect( i.CoordWorld, dir );
              Intersection.countRays++;
              Intersection si = Intersection.FirstIntersection( intersections, ref dir );
              // Better shadow testing: intersection between 0.0 and 1.0 kills the lighting
              if ( si != null && !si.Far( 1.0, ref dir ) )
                continue;
            }

            double[] reflection = i.ReflectanceModel.ColorReflection( i, dir, p1, ReflectionComponent.ALL );
            if ( reflection != null )
            {
              for ( b = 0; b < bands; b++ )
                color[ b ] += intensity[ b ] * reflection[ b ];
              hash = hash * HASH_LIGHT + source.GetHashCode();
            }
          }
        }
      }

      // check the recursion depth:
      if ( level++ >= MaxLevel ||
           !DoReflections && !DoRefractions )
        return hash;                        // no further recursion

      Vector3d r;
      double maxK;
      double[] comp = new double[ bands ];
      double newImportance;

      if ( DoReflections )
      {
        // !!!{{ TODO: cast the reflected ray to a bit random direction (variance should be based on Phong material's exponent H)..
        Geometry.SpecularReflection( ref i.Normal, ref p1, out r );
        double[] ks = i.ReflectanceModel.ColorReflection( i, p1, r, ReflectionComponent.SPECULAR_REFLECTION );
        // !!!}}

        if ( ks != null )
        {
          maxK = ks[ 0 ];
          for ( b = 1; b < bands; b++ )
            if ( ks[ b ] > maxK )
              maxK = ks[ b ];

          newImportance = importance * maxK;
          if ( newImportance >= MinImportance ) // do compute the reflected ray
          {
            hash += HASH_REFLECT * shade( level, newImportance, ref i.CoordWorld, ref r, rank, total, rnd, comp );
            for ( b = 0; b < bands; b++ )
              color[ b ] += ks[ b ] * comp[ b ];
          }
        }
      }

      if ( DoRefractions )                  // trying to shoot a refracted ray..
      {
        maxK = i.Material.Kt;               // simple solution, no influence of reflectance model yet
        newImportance = importance * maxK;
        if ( newImportance < MinImportance )
          return hash;

        // refracted ray:
        if ( (r = Geometry.SpecularRefraction( i.Normal, i.Material.n, p1 )) == null )
          return hash;

        // !!!{{ TODO: tweak the refracted ray as well?
        // !!!}}

        hash += HASH_REFRACT * shade( level, newImportance, ref i.CoordWorld, ref r, rank, total, rnd, comp );
        for ( b = 0; b < bands; b++ )
          color[ b ] += maxK * comp[ b ];
      }

      return hash;
    }
  }
}
