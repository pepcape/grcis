using System.Collections.Generic;
using System.Windows.Forms;
using OpenTK;
using Rendering;
using System;
using MathSupport;
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
      // !!!{{ TODO: add your actual intersection code here

      double OD;
      Vector3d.Dot( ref p0, ref p1, out OD );
      double DD;
      Vector3d.Dot( ref p1, ref p1, out DD );
      double OO;
      Vector3d.Dot( ref p0, ref p0, out OO );
      double d = OD * OD + DD * (1.0 - OO); // discriminant
      if ( d <= 0.0 )
        return null;           // no intersections

      d = Math.Sqrt( d );

      // there will be two intersections: (-OD - d) / DD, (-OD + d) / DD
      LinkedList<Intersection> result = new LinkedList<Intersection>();
      Intersection i;

      // first intersection (-OD - d) / DD:
      i = new Intersection( this );
      i.T = (-OD - d) / DD;
      i.Enter =
      i.Front = true;
      i.CoordLocal.X = p0.X + i.T * p1.X;
      i.CoordLocal.Y = p0.Y + i.T * p1.Y;
      i.CoordLocal.Z = p0.Z + i.T * p1.Z;
      result.AddLast( i );

      // second intersection (-OD + d) / DD:
      i = new Intersection( this );
      i.T = (-OD + d) / DD;
      i.Enter =
      i.Front = false;
      i.CoordLocal.X = p0.X + i.T * p1.X;
      i.CoordLocal.Y = p0.Y + i.T * p1.Y;
      i.CoordLocal.Z = p0.Z + i.T * p1.Z;
      result.AddLast( i );

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
      Vector3d tu, tv;
      Geometry.GetAxes( ref inter.CoordLocal, out tu, out tv );
      tu = Vector3d.TransformVector( tu, inter.LocalToWorld );
      tv = Vector3d.TransformVector( tv, inter.LocalToWorld );
      Vector3d.Cross( ref tu, ref tv, out inter.Normal );

      // 2D texture coordinates:
      double r = Math.Sqrt( inter.CoordLocal.X * inter.CoordLocal.X + inter.CoordLocal.Y * inter.CoordLocal.Y );
      inter.TextureCoord.X = Geometry.IsZero( r ) ? 0.0 : (Math.Atan2( inter.CoordLocal.Y, inter.CoordLocal.X ) / (2.0 * Math.PI) + 0.5);
      inter.TextureCoord.Y = Math.Atan2( r, inter.CoordLocal.Z ) / Math.PI;

      // !!!}}
    }
  }

}
