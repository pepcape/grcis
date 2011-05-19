using System;
using System.Collections.Generic;
using System.Windows.Forms;
using MathSupport;
using OpenTK;
using Rendering;
using Scene3D;

namespace _028rtmesh
{
  public partial class Form1 : Form
  {
    /// <summary>
    /// Initialize ray-scene and image function (good enough for single samples).
    /// </summary>
    private IImageFunction getImageFunction ()
    {
      // default constructor of the RayScene .. custom scene
      scene = new RayScene( brepScene );
      return new RayTracing( scene );
    }

    /// <summary>
    /// Initialize image synthesizer (responsible for raster image computation).
    /// The 'imf' member has been already initialized..
    /// </summary>
    private IRenderer getRenderer ()
    {
      SimpleImageSynthesizer sis = new SimpleImageSynthesizer();
      sis.ImageFunction = imf;
      return sis;
    }
  }
}

namespace Rendering
{
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
      Camera = new StaticCamera( new Vector3d( 0.0, 0.0, -30.0 ),
                                 new Vector3d( 0.0, 0.0, 1.0 ), 60.0 );

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

    public TriangleMesh ( SceneBrep m )
    {
      mesh = m;

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

      LinkedList<Intersection> result = null;
      Intersection i;

      for ( int id = 0; id < mesh.Triangles; id++ )
      {
        Vector3 a, b, c;
        mesh.GetTriangleVertices( id, out a, out b, out c );
        Vector2d uv;
        Vector3d aa, bb, cc;
        aa.X = a.X; aa.Y = a.Y; aa.Z = a.Z;
        bb.X = b.X; bb.Y = b.Y; bb.Z = b.Z;
        cc.X = c.X; cc.Y = c.Y; cc.Z = c.Z;
        double t = Geometry.RayTriangleIntersection( p0, p1, ref aa, ref bb, ref cc, out uv );
        if ( Double.IsInfinity( t ) ) continue;

        if ( result == null )
          result = new LinkedList<Intersection>();

        // Compile the 1st Intersection instance:
        i = new Intersection( this );
        i.T = t;
        i.Enter =
        i.Front = true;
        i.CoordLocal.X = p0.X + i.T * p1.X;
        i.CoordLocal.Y = p0.Y + i.T * p1.Y;
        i.CoordLocal.Z = p0.Z + i.T * p1.Z;

        // Tmp data object
        TmpData tmp = new TmpData();
        tmp.face    = id;
        tmp.uv      = uv;
        Vector3 ba  = b - a;    // only the constant shading so far..
        Vector3 ca  = c - a;
        Vector3.Cross( ref ba, ref ca, out tmp.normal );
        i.SolidData = tmp;

        // Compile the 2nd Intersection instance:
        i = new Intersection( this );
        i.T = t + 1.0e-4;
        i.Enter =
        i.Front = false;
        i.CoordLocal.X = p0.X + i.T * p1.X;
        i.CoordLocal.Y = p0.Y + i.T * p1.Y;
        i.CoordLocal.Z = p0.Z + i.T * p1.Z;

        // Tmp data object
        TmpData tmp2 = new TmpData();
        tmp2.face    = id;
        tmp2.uv      = uv;
        tmp2.normal  = -tmp.normal;
        i.SolidData  = tmp2;

        result.AddLast( i );
      }

      if ( result != null )   // sort the result list
      {
      }

      return result;

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
        Vector3d tu, tv;
        Vector3d normal;
        normal.X = tmp.normal.X; normal.Y = tmp.normal.Y; normal.Z = tmp.normal.Z;
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
