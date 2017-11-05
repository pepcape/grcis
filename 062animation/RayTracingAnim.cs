// Author: Josef Pelikan

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using OpenTK;
using Rendering;

namespace _062animation
{
  public partial class Form1 : Form
  {
    /// <summary>
    /// Initialize the ray-scene.
    /// </summary>
    private IRayScene getScene ()
    {
      return new AnimatedScene();
    }

    /// <summary>
    /// Initialize ray-scene and image function (good enough for simple samples).
    /// </summary>
    private IImageFunction getImageFunction ( IRayScene scene )
    {
      return new RayTracing( scene );
    }

    /// <summary>
    /// Initialize image synthesizer (responsible for raster image computation).
    /// </summary>
    private IRenderer getRenderer ( IImageFunction imf )
    {
      if ( superSampling > 1 )
      {
        SupersamplingImageSynthesizer sis = new SupersamplingImageSynthesizer();
        sis.ImageFunction = imf;
        sis.Supersampling = superSampling;
        sis.Jittering = 1.0;
        return sis;
      }
      SimpleImageSynthesizer s = new SimpleImageSynthesizer();
      s.ImageFunction = imf;
      return s;
    }

    /// <summary>
    /// Initialize rendering parameters.
    /// </summary>
    private void InitializeParams ()
    {
      // single frame:
      ImageWidth  = 320;
      ImageHeight = 180;
      numericSupersampling.Value = 1;

      // animation:
      numFrom.Value = (decimal)0.0;
      numTo.Value   = (decimal)20.0;
      numFps.Value  = (decimal)25.0;
    }
  }
}

namespace Rendering
{
  /// <summary>
  /// Pilot implementation of time-dependent camera.
  /// It simply goes round the central point (look-at point).
  /// </summary>
  public class AnimatedCamera : StaticCamera, ITimeDependent
  {
    /// <summary>
    /// Starting (minimal) time in seconds.
    /// </summary>
    public double Start
    {
      get;
      set;
    }

    /// <summary>
    /// Ending (maximal) time in seconds.
    /// </summary>
    public double End
    {
      get;
      set;
    }

    protected double time;

    /// <summary>
    /// Goes round the central point (lookAt).
    /// One complete turn for the whole time interval.
    /// </summary>
    protected virtual void setTime ( double newTime )
    {
      Debug.Assert( Start != End );

      time = newTime;    // Here Start & End define a periodicity, not bounds!

      // change the camera position:
      double angle = MathHelper.TwoPi * (time - Start) / (End - Start);
      Vector3d radial = Vector3d.TransformVector( center0 - lookAt, Matrix4d.CreateRotationY( -angle ) );
      center = lookAt + radial;
      direction = -radial;
      direction.Normalize();
      prepare();
    }

    /// <summary>
    /// Current time in seconds.
    /// </summary>
    public double Time
    {
      get
      {
        return time;
      }
      set
      {
        setTime( value );
      }
    }

    /// <summary>
    /// Central point (to look at).
    /// </summary>
    protected Vector3d lookAt;

    /// <summary>
    /// Center for time == Start;
    /// </summary>
    protected Vector3d center0;

    /// <summary>
    /// Clone all the time-dependent components, share the others.
    /// </summary>
    /// <returns></returns>
    public virtual object Clone ()
    {
      AnimatedCamera c = new AnimatedCamera( lookAt, center0, MathHelper.RadiansToDegrees( (float)hAngle ) );
      c.Start = Start;
      c.End   = End;
      c.Time  = Time;
      return c;
    }

    public AnimatedCamera ( Vector3d lookat, Vector3d cen, double ang )
      : base( cen, lookat - cen, ang )
    {
      lookAt  = lookat;
      center0 = cen;
      Start   = 0.0;
      End     = 10.0;
      time    = 0.0;
    }
  }

  // !!!{{ TODO: put your new time-dependent scene-construction classes here

  // !!!}}

  /// <summary>
  /// Animated Ray-scene.
  /// </summary>
  public class AnimatedScene : AnimatedRayScene
  {
    /// <summary>
    /// Creates default ray-rendering scene.
    /// </summary>
    public AnimatedScene ()
    {
      // !!!{{ TODO: .. and use your time-dependent objects to construct the scene

      // This code is based on Scenes.TwoSpheres():

      // CSG scene:
      CSGInnerNode root = new CSGInnerNode( SetOperation.Union );
      root.SetAttribute( PropertyName.REFLECTANCE_MODEL, new PhongModel() );
      root.SetAttribute( PropertyName.MATERIAL, new PhongMaterial( new double[] { 1.0, 0.8, 0.1 }, 0.1, 0.6, 0.4, 128 ) );
      Intersectable = root;

      // Background color:
      BackgroundColor = new double[] { 0.0, 0.05, 0.07 };

      // Camera:
      AnimatedCamera cam = new AnimatedCamera( new Vector3d( 0.7, -0.4,  0.0 ),
                                               new Vector3d( 0.7,  0.8, -6.0 ),
                                               50.0 );
      cam.End =             // one complete turn takes 20.0 seconds
          End = 20.0;
      Camera  = cam;

      //Camera = new StaticCamera( new Vector3d( 0.7,  0.5, -5.0 ),
      //                           new Vector3d( 0.0, -0.18, 1.0 ),
      //                           50.0 );

      // Light sources:
      Sources = new LinkedList<ILightSource>();
      Sources.Add( new AmbientLightSource( 0.8 ) );
      Sources.Add( new PointLightSource( new Vector3d( -5.0, 4.0, -3.0 ), 1.2 ) );

      // --- NODE DEFINITIONS ----------------------------------------------------

      // Transparent sphere:
      Sphere s;
      s = new Sphere();
      PhongMaterial pm = new PhongMaterial( new double[] { 0.0, 0.2, 0.1 }, 0.03, 0.03, 0.08, 128 );
      pm.n  = 1.6;
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

      // !!!}}
    }
  }
}
