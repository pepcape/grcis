using System;
using System.Collections.Generic;
using System.Diagnostics;
using MathSupport;
using OpenTK;
using Rendering;
using Scene3D;

namespace _050rtmesh
{
  public class FormSupport
  {
    /// <summary>
    /// Prepare form data (e.g. combo-box with available scenes).
    /// </summary>
    public static void InitializeScenes ( out string name )
    {
      name = "Josef Pelikán";

      Form1 f = Form1.singleton;
      f.sceneInitFunctions = new List<InitSceneStrDelegate>( Scenes.InitFunctionsStr );

      // 1. default scenes from RayCastingScenes
      foreach ( string n in Scenes.NamesStr )
        f.ComboScene.Items.Add( n );

      // 2. eventually add custom scenes
      f.sceneInitFunctions.Add( new InitSceneStrDelegate( CustomScene.TestScene ) );
      f.ComboScene.Items.Add( "Test scene" );

      // .. and set your favorite scene here:
      f.ComboScene.SelectedIndex = f.ComboScene.Items.IndexOf( "Test scene" );

      // default image parameters?
      f.ImageWidth = 400;
      f.ImageHeight = 300;
      f.NumericSupersampling.Value = 1;
      f.CheckJitter.Checked = false;
      f.CheckMultithreading.Checked = true;
    }

    /// <summary>
    /// Initialize the ray-scene.
    /// </summary>
    public static IRayScene getScene ()
    {
      return Form1.singleton.SceneByComboBox();
    }

    /// <summary>
    /// Initialize ray-scene and image function (good enough for simple samples).
    /// </summary>
    public static IImageFunction getImageFunction ( IRayScene scene )
    {
      return new RayTracing( scene );
    }

    /// <summary>
    /// Initialize image synthesizer (responsible for raster image computation).
    /// </summary>
    public static IRenderer getRenderer ( IImageFunction imf )
    {
      SupersamplingImageSynthesizer sis = new SupersamplingImageSynthesizer();
      sis.ImageFunction = imf;
      return sis;
    }
  }
}

namespace Rendering
{
  /// <summary>
  /// Custom scene for mesh rendering (derived from "Teapot").
  /// </summary>
  public class CustomScene
  {
    public static long TestScene ( IRayScene sc, string[] names )
    {
      Debug.Assert( sc != null );

      Vector3 center = Vector3.Zero;   // center of the mesh
      Vector3d dir   = new Vector3d( 0.1, -0.3, 0.9 );
      dir.Normalize();                 // normalized viewing vector of the camera
      float diameter = 2.0f;           // default scene diameter
      double FoVy = 60.0;              // Field of View in degrees
      int faces = 0;

      // CSG scene:
      CSGInnerNode root = new CSGInnerNode( SetOperation.Union );

      // OBJ file to read:
      if ( names.Length == 0 ||
           names[ 0 ].Length == 0 )
        names = new string[] { "teapot.obj" };

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
        TriangleMesh m = new FastTriangleMesh( brep );
        root.InsertChild( m, Matrix4d.Identity );
      }

      root.SetAttribute( PropertyName.REFLECTANCE_MODEL, new PhongModel() );
      root.SetAttribute( PropertyName.MATERIAL, new PhongMaterial( new double[] { 0.5, 0.5, 0.5 }, 0.2, 0.5, 0.4, 12 ) );
      root.SetAttribute( PropertyName.COLOR, new double[] { 1.0, 0.6, 0.0 } );
      sc.Intersectable = root;

      // Background color:
      sc.BackgroundColor = new double[] { 0.0, 0.05, 0.07 };

      // Camera:
      double dist = (0.6 * diameter) / Math.Tan( MathHelper.DegreesToRadians( (float)(0.5 * FoVy) ) );
      Vector3d cam = (Vector3d)center - dist * dir;
      sc.Camera = new StaticCamera( cam, dir, FoVy );

      // Light sources:
      sc.Sources = new LinkedList<ILightSource>();
      sc.Sources.Add( new AmbientLightSource( 0.8 ) );
      sc.Sources.Add( new PointLightSource( new Vector3d( -20.0, 12.0, -12.0 ), 1.0 ) );

      return faces;
    }
  }

  /// <summary>
  /// Accelerated triangle mesh able to compute ray-intersection and normal vector.
  /// </summary>
  public class FastTriangleMesh : TriangleMesh
  {
    public FastTriangleMesh ( SceneBrep m )
      : base( m )
    {
      // !!!{{ TODO: prepare acceleration structure for the mesh

      // !!!}}
    }

    /// <summary>
    /// Computes the complete intersection of the given ray with the object.
    /// </summary>
    /// <param name="p0">Ray origin.</param>
    /// <param name="p1">Ray direction vector.</param>
    /// <returns>Sorted list of intersection records.</returns>
    public override LinkedList<Intersection> Intersect ( Vector3d p0, Vector3d p1 )
    {
      // !!!{{ TODO: add your accelerated intersection code here

      if ( mesh == null || mesh.Triangles < 1 ) return null;

      List<Intersection> result = null;
      Intersection i;

      for ( int id = 0; id < mesh.Triangles; id++ )
      {
        Vector3 a, b, c;
        mesh.GetTriangleVertices( id, out a, out b, out c );
        Vector2d uv;
        CSGInnerNode.countTriangles++;
        double t = Geometry.RayTriangleIntersection( ref p0, ref p1, ref a, ref b, ref c, out uv );
        if ( Double.IsInfinity( t ) ) continue;

        if ( result == null )
          result = new List<Intersection>();

        // Compile the 1st Intersection instance:
        i = new Intersection( this );
        i.T = t;
        i.Enter =
        i.Front = true;
        i.CoordLocal = p0 + i.T * p1;

        // Tmp data object
        TmpData tmp = new TmpData();
        tmp.face    = id;
        tmp.uv      = uv;
        Vector3 ba  = b - a;    // temporary value for flat shading
        Vector3 ca  = c - a;
        Vector3.Cross( ref ba, ref ca, out tmp.normal );
        i.SolidData = tmp;

        result.Add( i );

        if ( !ShellMode ) continue;

        // Compile the 2nd Intersection instance:
        i = new Intersection( this );
        i.T = t + 1.0e-4;
        i.Enter =
        i.Front = false;
        i.CoordLocal = p0 + i.T * p1;

        // Tmp data object
        TmpData tmp2 = new TmpData();
        tmp2.face    = id;
        tmp2.uv      = uv;
        tmp2.normal  = -tmp.normal;
        i.SolidData  = tmp2;

        result.Add( i );
      }

      if ( result == null )
        return null;

      // Finalizing the result: sort the result list
      result.Sort();
      return new LinkedList<Intersection>( result );

      // !!!}}
    }

    /// <summary>
    /// Complete all relevant items in the given Intersection object.
    /// </summary>
    /// <param name="inter">Intersection instance to complete.</param>
    public override void CompleteIntersection ( Intersection inter )
    {
      // !!!{{ TODO: add your actual completion code here

      // normal vector:
      TmpData tmp = inter.SolidData as TmpData;
      if ( tmp != null )
      {
        if ( Smooth && mesh.Normals > 0 )   // smooth interpolation of normal vectors
        {
          int v1, v2, v3;
          mesh.GetTriangleVertices( tmp.face, out v1, out v2, out v3 );
          tmp.normal  = mesh.GetNormal( v1 ) * (float)(1.0 - tmp.uv.X - tmp.uv.Y);
          tmp.normal += mesh.GetNormal( v2 ) * (float)tmp.uv.X;
          tmp.normal += mesh.GetNormal( v3 ) * (float)tmp.uv.Y;
        }

        Vector3d tu, tv;
        Vector3d normal = (Vector3d)tmp.normal;
        Geometry.GetAxes( ref normal, out tu, out tv );
        tu = Vector3d.TransformVector( tu, inter.LocalToWorld );
        tv = Vector3d.TransformVector( tv, inter.LocalToWorld );
        Vector3d.Cross( ref tu, ref tv, out inter.Normal );
      }

      // 2D texture coordinates (not yet):
      inter.TextureCoord.X =
      inter.TextureCoord.Y = 0.0;

      // !!!}}
    }
  }
}
