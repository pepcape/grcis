using System.Collections.Generic;
using OpenTK;

namespace Rendering
{
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
    public RayScene ()
    {
      // scene:
      InnerNode root = new InnerNode( SetOperation.Union );
      Intersectable = root;
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
      s.SetAttribute( PropertyName.COLOR, new double[] { 0.2, 0.6, 0.0 } );
      s.SetAttribute( PropertyName.TEXTURE, new CheckerTexture( 20.0, 20.0, new double[] { 1.0, 0.8, 0.2 } ) );
      root.InsertChild( s, Matrix4d.CreateTranslation( 4.4, 0.0, 0.0 ) );

      // background color:
      BackgroundColor = new double[] { 0.0, 0.1, 0.2 };

      // camera:
      Camera = new StaticCamera( new Vector3d( 0.0, 0.0, -10.0 ),
                                 new Vector3d( 0.0, 0.0, 1.0 ), 60.0 );

      // light sources:
      Sources = new LinkedList<ILightSource>();
      Sources.Add( new PointLightSource( new Vector3d( -10.0, 8.0, 3.0 ), 1.0 ) );
    }
  }
}
