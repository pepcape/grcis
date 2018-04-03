// Author: Josef Pelikan

using System.Diagnostics;
using OpenTK;
using Rendering;

namespace _046cameranim
{
  public class FormSupport
  {
    /// <summary>
    /// Initialize the ray-scene.
    /// </summary>
    public static IRayScene getScene ()
    {
      return new RayScene();
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
    public static IRenderer getRenderer ( IImageFunction imf, int superSampling )
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
  }
}

namespace Rendering
{
  /// <summary>
  /// Pilot implementation of time-dependent camera.
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

      // !!!{{ TODO: put your camera time-dependency code here

      //time = Arith.Clamp( newTime, Start, End );
      time = newTime;    // Here Start & End define a periodicity, not bounds!

      // change the camera position:
      double angle = MathHelper.TwoPi * (time - Start) / (End - Start);
      Vector3d radial = Vector3d.TransformVector( center0 - lookAt, Matrix4d.CreateRotationY( -angle ) );
      center = lookAt + radial;
      direction = -radial;
      direction.Normalize();
      prepare();

      // !!!}}
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

  /// <summary>
  /// Animated Ray-scene.
  /// </summary>
  public class RayScene : AnimatedRayScene
  {
    /// <summary>
    /// Creates default ray-rendering scene.
    /// </summary>
    public RayScene ()
    {
      // !!!{{ TODO: put your scene and camera setup code here

      Scenes.FiveBalls( this );
      AnimatedCamera cam = new AnimatedCamera( new Vector3d( 0.0, 0.0, 1.0 ),
                                               new Vector3d( 0.0, 0.0, -9.0 ),
                                               60.0 );

      //Scenes.Bezier( this );
      //AnimatedCamera cam = new AnimatedCamera( new Vector3d( 0.7, -1.0, 3.0 ),
      //                                         new Vector3d( 0.7, 0.5, -5.0 ),
      //                                         55.0 );

      Camera = cam;
      cam.End = 5.0;   // one complete turn takes 5.0 seconds
          End = 5.0;

      // !!!}}
    }
  }
}
