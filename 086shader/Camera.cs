using System;
using System.Collections.Generic;
using MathSupport;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Utilities;

namespace _086shader
{
  public class AnimatedCamera : DefaultRealtimeCamera
  {
    /// <summary>
    /// Optional form-data initialization.
    /// </summary>
    /// <param name="name">Return your full name.</param>
    /// <param name="param">Optional text to initialize the form's text-field.</param>
    /// <param name="tooltip">Optional tooltip = param help.</param>
    public static void InitParams ( out string name, out string param, out string tooltip )
    {
      // {{

      name    = "Josef Pelikán";
      param   = "period=5.0, rad=2.0";
      tooltip = "period=<cam period in seconds>, rad=<cam radius in scene diameters>";

      // }}
    }

    public override void Update ( string param, string cameraFile )
    {
      // {{ Put your parameter-parsing code here

      Dictionary<string, string> p = Util.ParseKeyValueList( param );
      if ( p.Count > 0 )
      {
        // period=<double>
        double v = 1.0;
        if ( Util.TryParse( p, "period", ref v ) )
          MaxTime = Math.Max( v, 0.1 );

        // rad=<double>
        if ( Util.TryParse( p, "rad", ref v ) )
          radius = (float)Math.Max( v, 0.01 );

        // ... you can add more parameters here ...
      }

      Time = Util.Clamp( Time, MinTime, MaxTime );

      // Put your camera-definition-file parsing here.

      // }}
    }

    /// <summary>
    /// Radius of camera trajectory.
    /// </summary>
    float radius = 1.0f;

    public override void Reset ()
    {
      Time = MinTime;
    }

    public AnimatedCamera ( string param, string cameraFile = "" )
    {
      // {{ Put your camera initialization code here

      Update( param, cameraFile );

      // }}
    }

    Matrix4 perspectiveProjection;

    public override Matrix4 Projection
    {
      get
      {
        return perspectiveProjection;
      }
    }

    /// <summary>
    /// Sets up a projective viewport
    /// </summary>
    public override void GLsetupViewport ( int width, int height, float near = 0.01f, float far = 1000.0f )
    {
      // 1. set ViewPort transform:
      GL.Viewport( 0, 0, width, height );

      // 2. set projection matrix
      perspectiveProjection = Matrix4.CreatePerspectiveFieldOfView( Fov, width / (float)height, near, far );
      GLsetProjection();
    }

    Matrix4 computeMV ()
    {
      double t = (Time - MinTime) * 2 * Math.PI / (MaxTime - MinTime);
      double r = radius * 0.5 * Diameter;
      Vector3 eye = Center + new Vector3( (float)(Math.Sin( t ) * r), 0.0f, (float)(Math.Cos( t ) * r) );
      return Matrix4.LookAt( eye, Center, Vector3.UnitY );
    }

    public override Matrix4 ModelView
    {
      get
      {
        return computeMV();
      }
    }

    public override Matrix4 ModelViewInv
    {
      get
      {
        return computeMV().Inverted();
      }
    }
  }
}
