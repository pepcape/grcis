using System;
using System.Collections.Generic;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace _041lsystems
{
  #region Camera

  public class Camera
  {
    public Camera ()
    {
      Reset();
    }

    public void Reset ()
    {
      mEye = new Vector3d( 0, -30, 5 );
      mCenter = new Vector3d( 0, 0, 5 );
      mUp = new Vector3d( 0, 0, 1 );
      Vector3d tmp = mEye - mCenter;
      tmp.Normalize();
      mRight = Vector3d.Cross( mUp, tmp );
    }

    public void Zoom ( double aZoom )
    {
      if ( aZoom < 0.2 || aZoom > 6 )
        return;

      Vector3d tmp = mEye - mCenter;
      double dst = tmp.Length;
      tmp.Normalize();
      dst *= aZoom;
      mEye = tmp * dst + mCenter;
    }

    public void Orbit ( double aYaw, double aPitch )
    {
      Matrix4d m1 = Matrix4d.CreateFromAxisAngle( new Vector3d( 0.0, 0.0, 1.0 ) /* mUp */, aYaw );
      Matrix4d m2 = Matrix4d.CreateFromAxisAngle( mRight, aPitch );
      Matrix4d m = m1 * m2;
      Matrix4d mI = Matrix4d.Transpose( m );

      Vector3d tmp = mEye - mCenter;
      double dst = tmp.Length;
      tmp.Normalize();

      mUp = Vector3d.Transform( mUp, mI );
      mUp.Normalize();
      tmp = Vector3d.Transform( tmp, mI );
      tmp.Normalize();
      mRight = Vector3d.Cross( mUp, tmp );
      mEye = tmp * dst + mCenter;
    }

    public void SetupGLView ()
    {
      GL.MatrixMode( MatrixMode.Modelview );
      Matrix4d lookAt = Matrix4d.LookAt( mEye, mCenter, mUp );
      GL.LoadMatrix( ref lookAt );
    }

    Vector3d mCenter;
    Vector3d mEye;
    Vector3d mUp;
    Vector3d mRight;
  }

  #endregion

  public class VisualisationParameters : ICloneable
  {
    public double angle
    {
      get;
      set;
    }

    public double length
    {
      get;
      set;
    }
    public double shortage
    {
      get;
      set;
    }

    public double radius
    {
      get;
      set;
    }

    public double shrinkage
    {
      get;
      set;
    }

    object ICloneable.Clone ()
    {
      return this.Clone();
    }

    public VisualisationParameters Clone ()
    {
      return (VisualisationParameters)this.MemberwiseClone();
    }
  };

  public class LSystemRenderer
  {
    private Camera mCamera;

    public void OrbitCamera ( double aYaw, double aPitch )
    {
      mCamera.Orbit( aYaw, aPitch );
    }

    public void Zoom ( double aZoom )
    {
      mCamera.Zoom( aZoom );
    }

    public void ResetCamera ()
    {
      mCamera.Reset();
    }

    public LSystemRenderer ()
    {
      mCamera = new Camera();
    }

    public void Render ( string aLSInstance, int aGeneration, VisualisationParameters aParameters )
    {
      #region View setup ...

      GL.Clear( ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit );
      mCamera.SetupGLView();
      GL.Light( LightName.Light0, LightParameter.Position, new Vector4( -50.0f, -50.0f, 150.0f, 1.0f ) );
      GL.Light( LightName.Light1, LightParameter.Diffuse, new Color4( 1.0f, 1.0f, 1.0f, 1.0f ) );

      #endregion

      GL.MatrixMode( MatrixMode.Modelview );

      #region Render ground ...

      GL.Color3( Color.Green );
      GL.Begin( PrimitiveType.Quads );

      GL.Normal3( 0.0f, 0.0f, 1.0f );
      GL.Vertex3( -10.0f, -10.0f, 0.0f );
      GL.Vertex3( 10.0f, -10.0f, 0.0f );
      GL.Vertex3( 10.0f, 10.0f, 0.0f );
      GL.Vertex3( -10.0f, 10.0f, 0.0f );

      GL.End();

      #endregion

      if ( aLSInstance == null || aParameters == null ) // exit if nothing to Render
        return;

      // Initialize parameter stack
      Stack<VisualisationParameters> parameterStack = new Stack<VisualisationParameters>();
      VisualisationParameters currentParameters = aParameters.Clone();

      GL.Color3( Color.Khaki );
      foreach ( char v in aLSInstance )
      {
        switch ( v )
        {
          case 'F':
            ForwardStep( currentParameters.length, currentParameters.radius, currentParameters.radius * currentParameters.shrinkage );
            currentParameters.length *= currentParameters.shortage;
            currentParameters.radius *= currentParameters.shrinkage;
            break;

          case '+':
            GL.Rotate( currentParameters.angle, 0.0f, 1.0f, 0.0f );
            break;

          case '-':
            GL.Rotate( -currentParameters.angle, 0.0f, 1.0f, 0.0f );
            break;

          case '/':
            GL.Rotate( 90.0f, 0.0f, 0.0f, 1.0f );
            break;

          case '\\':
            GL.Rotate( -90.0f, 0.0f, 0.0f, 1.0f );
            break;

          case '[':               // Save current state to stack
            GL.PushMatrix();
            parameterStack.Push( currentParameters.Clone() );
            break;

          case ']':               // Restore state from stack
            GL.PopMatrix();
            currentParameters = parameterStack.Pop();
            break;
        }
      }
    }

    #region Helper methods

    private void ForwardStep ( double aStepSize, double aRadius0, double aRadius1 )
    {
      GL.Begin( PrimitiveType.Quads );

      GL.Normal3( 0.0f, 0.0f, -1.0f );
      GL.Vertex3( -aRadius0, -aRadius0, 0.0f );
      GL.Vertex3( aRadius0, -aRadius0, 0.0f );
      GL.Vertex3( aRadius0, aRadius0, 0.0f );
      GL.Vertex3( -aRadius0, aRadius0, 0.0f );

      GL.Normal3( 0.0f, 0.0f, 1.0f );
      GL.Vertex3( -aRadius1, -aRadius1, aStepSize );
      GL.Vertex3( aRadius1, -aRadius1, aStepSize );
      GL.Vertex3( aRadius1, aRadius1, aStepSize );
      GL.Vertex3( -aRadius1, aRadius1, aStepSize );

      GL.Normal3( 0.0f, -1.0f, 0.0f );
      GL.Vertex3( -aRadius0, -aRadius0, 0.0f );
      GL.Vertex3( aRadius0, -aRadius0, 0.0f );
      GL.Vertex3( aRadius1, -aRadius1, aStepSize );
      GL.Vertex3( -aRadius1, -aRadius1, aStepSize );

      GL.Normal3( 1.0f, 0.0f, 0.0f );
      GL.Vertex3( aRadius0, -aRadius0, 0.0f );
      GL.Vertex3( aRadius0, aRadius0, 0.0f );
      GL.Vertex3( aRadius1, aRadius1, aStepSize );
      GL.Vertex3( aRadius1, -aRadius1, aStepSize );

      GL.Normal3( 0.0f, 1.0f, 0.0f );
      GL.Vertex3( aRadius0, aRadius0, 0.0f );
      GL.Vertex3( -aRadius0, aRadius0, 0.0f );
      GL.Vertex3( -aRadius1, aRadius1, aStepSize );
      GL.Vertex3( aRadius1, aRadius1, aStepSize );

      GL.Normal3( -1.0f, 0.0f, 0.0f );
      GL.Vertex3( -aRadius0, aRadius0, 0.0f );
      GL.Vertex3( -aRadius0, -aRadius0, 0.0f );
      GL.Vertex3( -aRadius1, -aRadius1, aStepSize );
      GL.Vertex3( -aRadius1, aRadius1, aStepSize );

      GL.End();

      GL.Translate( 0.0f, 0.0f, aStepSize );
    }

    #endregion
  }
}
