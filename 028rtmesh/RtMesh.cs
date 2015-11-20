// Author: Josef Pelikan

using System;
using System.Collections.Generic;
using MathSupport;
using OpenTK;
using Scene3D;

namespace Rendering
{
  public class FormSupport
  {
    /// <summary>
    /// Initialize ray-scene and image function (good enough for single samples).
    /// </summary>
    public static IImageFunction getImageFunction ( out IRayScene scene, SceneBrep brepScene )
    {
      // default constructor of the RayScene .. custom scene
      scene = new RayScene( brepScene );
      return new RayTracing( scene );
    }

    /// <summary>
    /// Initialize image synthesizer (responsible for raster image computation).
    /// The 'imf' member has been already initialized..
    /// </summary>
    public static IRenderer getRenderer ( IImageFunction imf )
    {
      SimpleImageSynthesizer sis = new SimpleImageSynthesizer();
      sis.ImageFunction = imf;
      return sis;
    }
  }

  /// <summary>
  /// Test scene for ray-based rendering.
  /// </summary>
  public class RayScene : IRayScene
  {
    public IIntersectable Intersectable
    {
      get;
      set;
    }

    public double[] BackgroundColor
    {
      get;
      set;
    }

    public ICamera Camera
    {
      get;
      set;
    }

    public ICollection<ILightSource> Sources
    {
      get;
      set;
    }

    /// <summary>
    /// Creates default ray-rendering scene.
    /// </summary>
    public RayScene ( SceneBrep mesh )
    {
      // scene:
      TriangleMesh root = new TriangleMesh( mesh );
      root.SetAttribute( PropertyName.REFLECTANCE_MODEL, new PhongModel() );
      root.SetAttribute( PropertyName.MATERIAL, new PhongMaterial( new double[] { 0.5, 0.5, 0.5 }, 0.2, 0.5, 0.4, 12 ) );
      root.SetAttribute( PropertyName.COLOR, new double[] { 1.0, 0.6, 0.0 } );
      Intersectable = root;

      // background color:
      BackgroundColor = new double[] { 0.0, 0.15, 0.2 };

      // camera:
      Camera = new StaticCamera( new Vector3d( 0.5, 2.0, -5.0 ),
                                 new Vector3d( 0.1, -0.2, 0.9 ), 60.0 );

      // light sources:
      Sources = new LinkedList<ILightSource>();
      Sources.Add( new AmbientLightSource( 0.8 ) );
      Sources.Add( new PointLightSource( new Vector3d( -20.0, 12.0, -12.0 ), 1.0 ) );
    }
  }

  /// <summary>
  /// Triangle mesh able to compute ray-intersection and normal vector.
  /// </summary>
  public class TriangleMesh : DefaultSceneNode, ISolid
  {
    protected class TmpData
    {
      /// <summary>
      /// Face id (id of the intersected triangle).
      /// </summary>
      public int face;

      /// <summary>
      /// Barycentric coordinates in the intersected triangle.
      /// </summary>
      public Vector2d uv;

      // Vertex ids.
      //int va, vb, vc;

      /// <summary>
      /// Normal vector in B-rep coordinates.
      /// </summary>
      public Vector3 normal;
    }

    /// <summary>
    /// Original mesh object (triangles in a Corner table).
    /// </summary>
    protected SceneBrep mesh;

    /// <summary>
    /// Shell mode: surface is considered as a thin shell (double-sided).
    /// </summary>
    public bool ShellMode
    {
      get;
      set;
    }

    /// <summary>
    /// Smooth mode: smooth interpolation of surface normals (a la Phong shading).
    /// </summary>
    public bool Smooth
    {
      get;
      set;
    }

    public TriangleMesh ( SceneBrep m )
    {
      mesh = m;
      ShellMode = false;
      Smooth = true;

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
      if ( inter.SolidData is TmpData )
      {
        TmpData tmp = (TmpData)inter.SolidData;

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
