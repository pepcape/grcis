using System.Diagnostics;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Scene3D;

namespace _040morph3d
{
  public class Camera
  {
    #region Camera interface

    public Camera ()
    {
      Reset();
    }

    public void Reset ()
    {
      mEye = new Vector3d( 0, -15, 0 );
      mCenter = new Vector3d( 0, 0, 0 );
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
      Matrix4d m1 = Matrix4d.CreateFromAxisAngle( mUp, aYaw );
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

    #endregion
  }

  class Morph
  {
    public Morph ()
    {
    }

    public void AssignObjects ( SceneBrep aFirst, SceneBrep aSecond )
    {
      mFirstObject  = aFirst;
      mSecondObject = aSecond;
      if ( aFirst == null || aSecond == null || aFirst.Vertices != aSecond.Vertices )
      {
        mInterpolatedObject = null;
        return;
      }
      mInterpolatedObject = mFirstObject.Clone();

      // !!!{{ TODO: put your initialization code here

      // !!!}}
    }

    public void Interpolate ( double alpha )
    {
      if ( mFirstObject == null || mSecondObject == null )
        mInterpolatedObject = null;

      if ( mInterpolatedObject == null )
        return;

      // !!!{{ TODO: put your interpolation code here

      for ( int i = 0; i < mInterpolatedObject.Vertices; ++i )
      {
        Vector3 v1 = mFirstObject.GetVertex( i );
        Vector3 v2 = mSecondObject.GetVertex( i );
        mInterpolatedObject.SetVertex( i, (float)(1.0 - alpha) * v1 + (float)alpha * v2 );
      }

      if ( mFirstObject.HasNormals() && mSecondObject.HasNormals() )
        for ( int i = 0; i < mInterpolatedObject.Normals; ++i )
        {
          Vector3 n1 = mFirstObject.GetNormal( i );
          Vector3 n2 = mSecondObject.GetNormal( i );
          mInterpolatedObject.SetNormal( i, Vector3.Normalize( (float)(1.0 - alpha) * n1 + (float)alpha * n2 ) );
        }

      // !!!}}
    }

    public SceneBrep InterpolatedObject
    {
      get
      {
        return mInterpolatedObject;
      }
    }

    private SceneBrep mInterpolatedObject;
    private SceneBrep mFirstObject;
    private SceneBrep mSecondObject;
  }
}
