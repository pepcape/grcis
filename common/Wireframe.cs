using System;
using System.Drawing;
using OpenTK;

namespace Scene3D
{
  public class Geometry
  {
    public static Matrix4 SetViewport ( int x0, int y0, int width, int height )
    {
      Matrix4 tr = Matrix4.CreateTranslation( 1.0f, 1.0f, 1.0f );
      tr = Matrix4.Mult( tr, Matrix4.CreateScale( 0.5f * width, -0.5f * height, 0.5f ) );
      return Matrix4.Mult( tr, Matrix4.CreateTranslation( x0, y0 + height, 0.0f ) );
    }
  }

  /// <summary>
  /// Wireframe rendering of a 3D scene.
  /// </summary>
  public class Wireframe
  {
    #region Instance data

    /// <summary>
    /// Use perspective projection?
    /// </summary>
    public bool Perspective { get; set; }

    /// <summary>
    /// View vector: azimuth angle in degrees.
    /// </summary>
    public double Azimuth { get; set; }

    /// <summary>
    /// View vector: elevation angle in degrees.
    /// </summary>
    public double Elevation { get; set; }

    /// <summary>
    /// Camera distance (for perspective camera only).
    /// </summary>
    public double Distance { get; set; }

    /// <summary>
    /// Viewing volume (ortho: horizontal size, perspective: horizontal view angle in degrees).
    /// </summary>
    public double ViewVolume { get; set; }

    /// <summary>
    /// Draw normal vectors?
    /// </summary>
    public bool DrawNormals { get; set; }

    #endregion

    #region Construction

    /// <summary>
    /// Sets up default viewing parameters.
    /// </summary>
    public Wireframe ()
    {
      Perspective = false;
      Azimuth     = 30.0;
      Elevation   = 20.0;
      Distance    = 10.0;
      ViewVolume  = 60.0;
      DrawNormals = false;
    }

    #endregion

    #region Rendering API

    public void Render ( Bitmap output, SceneBrep scene )
    {
      if ( output == null ||
           scene  == null ) return;

      Vector3 center;
      float diameter = scene.GetDiameter( out center );
      if ( Distance < diameter ) Distance = diameter;

      // and the rest of projection matrix goes here:
      int width    = output.Width;
      int height   = output.Height;
      float aspect = width / (float)height;
      double az    = Azimuth / 180.0 * Math.PI;
      double el    = Elevation / 180.0 * Math.PI;

      Vector3 eye   = new Vector3( (float)(center.X + Distance * Math.Sin( az ) * Math.Cos( el )),
                                   (float)(center.Y + Distance * Math.Sin( el )),
                                   (float)(center.Z + Distance * Math.Cos( az ) * Math.Cos( el )) );
      Matrix4 modelView = Matrix4.LookAt( eye, center, Vector3.UnitY );
      Matrix4 proj;

      if ( Perspective )
      {
        float vv = (float)(2.0 * Math.Atan2( diameter * 0.5, Distance ));
        proj = Matrix4.CreatePerspectiveFieldOfView( vv, aspect, 1.0f, 50.0f );
      }
      else
      {
        float vHalf = diameter * 0.52f;
        proj = Matrix4.CreateOrthographicOffCenter( -vHalf, vHalf,
                                                    -vHalf / aspect, vHalf / aspect,
                                                    1.0f, 50.0f );
      }

      Matrix4 compound = Matrix4.Mult( modelView, proj );
      Matrix4 viewport = Geometry.SetViewport( 0, 0, width, height );
      compound = Matrix4.Mult( compound, viewport );

      // wireframe rendering:
      Graphics gr = Graphics.FromImage( output );
      Pen pen = new Pen( Color.FromArgb( 255, 255, 80 ), 1.0f );
      int n = scene.Triangles;
      for ( int i = 0; i < n; i++ )
      {
        Vector4 A, B, C;
        scene.GetTriangleVertices( i, out A, out B, out C );
        A = Vector4.Transform( A, compound );
        B = Vector4.Transform( B, compound );
        C = Vector4.Transform( C, compound );
        Vector2 a = new Vector2( A.X / A.W, A.Y / A.W );
        Vector2 b = new Vector2( B.X / B.W, B.Y / B.W );
        Vector2 c = new Vector2( C.X / C.W, C.Y / C.W );
        gr.DrawLine( pen, a.X, a.Y, b.X, b.Y );
        gr.DrawLine( pen, b.X, b.Y, c.X, c.Y );
        gr.DrawLine( pen, c.X, c.Y, a.X, a.Y );
      }

      if ( DrawNormals && scene.Normals > 0 )
      {
        pen = new Pen( Color.FromArgb( 255, 80, 80 ), 1.0f );
        n = scene.Vertices;
        for ( int i = 0; i < n; i++ )
        {
          Vector4 V = new Vector4( scene.GetVertex( i ), 1.0f );
          Vector3 N = scene.GetNormal( i );
          N.Normalize();
          N *= diameter * 0.03f;
          Vector4 W = V + new Vector4( N );
          V = Vector4.Transform( V, compound );
          W = Vector4.Transform( W, compound );
          Vector2 v = new Vector2( V.X / V.W, V.Y / V.W );
          Vector2 w = new Vector2( W.X / W.W, W.Y / W.W );
          gr.DrawLine( pen, v.X, v.Y, w.X, w.Y );
        }
      }
    }

    #endregion

  }
}
