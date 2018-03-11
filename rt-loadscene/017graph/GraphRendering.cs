// Author: Josef Pelikan

using System;
using System.Drawing;
using OpenTK;

namespace Scene3D
{
  /// <summary>
  /// Graph rendering.
  /// </summary>
  public class GraphRendering
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
    /// Number of quad columns (x-direction).
    /// </summary>
    public int Columns { get; set; }

    /// <summary>
    /// Number of quad rows (z-direction).
    /// </summary>
    public int Rows { get; set; }

    #endregion

    #region Construction

    /// <summary>
    /// Sets up default viewing parameters.
    /// </summary>
    public GraphRendering ()
    {
      Perspective = false;
      Azimuth     = 30.0;
      Elevation   = 20.0;
      Distance    = 10.0;
      ViewVolume  = 60.0;
      Columns     = 12;
      Rows        = 12;
    }

    #endregion

    #region Rendering support

    protected Matrix4 compound;

    protected IFunctionR2ToR f;

    protected void transform ( float x, float z, out float xf, out float yf )
    {
      Vector4 A = new Vector4( x, (float)f.f( x, z ), z, 1.0f );
      A = Vector4.Transform( A, compound );
      xf = A.X / A.W;
      yf = A.Y / A.W;
    }

    protected void prepareProjection ( Bitmap output, IFunctionR2ToR funct )
    {
      f = funct;
      Vector3 center;
      center.X = 0.5f * (float)(f.MaxX + f.MinX);
      center.Z = 0.5f * (float)(f.MaxZ + f.MinZ);
      center.Y = (float)( 0.5 * f.f( center.X, center.Z ) );

      float diameter = (float)((f.MaxX - f.MinX) + (f.MaxZ - f.MinZ));
      if ( Distance < diameter ) Distance = diameter;

      // and the rest of projection matrix goes here:
      int width = output.Width;
      int height = output.Height;
      float aspect = width / (float)height;
      double az = Azimuth / 180.0 * Math.PI;
      double el = Elevation / 180.0 * Math.PI;

      Vector3 eye = new Vector3( (float)(center.X + Distance * Math.Sin( az ) * Math.Cos( el )),
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

      compound = Matrix4.Mult( modelView, proj );
      Matrix4 viewport = Geometry.SetViewport( 0, 0, width, height );
      compound = Matrix4.Mult( compound, viewport );
    }

    #endregion

    #region Rendering API

    public void Render ( Bitmap output, IFunctionR2ToR funct )
    {
      if ( output == null ||
           funct  == null ) return;

      prepareProjection( output, funct );

      // !!!{{ TODO: add the graph rendering code here

      Graphics gr = Graphics.FromImage( output );
      Pen pen = new Pen( Color.FromArgb( 255, 255, 80 ), 1.0f );

      float x, dx, z, dz;
      float ax, ay, bx, by;

      dx = (float)(funct.MaxX - funct.MinX) / Columns;
      dz = (float)(funct.MaxZ - funct.MinZ) / Rows;

      z = (float)funct.MinZ;
      x = (float)funct.MinX;
      for ( int i = 1; i < Columns; i++, x += dx )
      {
        transform( x,      z, out ax, out ay );
        transform( x + dx, z, out bx, out by );
        gr.DrawLine( pen, ax, ay, bx, by );
      }

      for ( int j = 1; j < Rows; j++, z += dz )
      {
        x = (float)funct.MinX;
        transform( x, z,      out ax, out ay );
        transform( x, z + dz, out bx, out by );
        gr.DrawLine( pen, ax, ay, bx, by );

        for ( int i = 1; i < Columns; i++, x += dx )
        {
          transform( x,      z + dz, out ax, out ay );
          transform( x + dx, z + dz, out bx, out by );
          gr.DrawLine( pen, ax, ay, bx, by );
          transform( x + dx, z,      out ax, out ay );
          gr.DrawLine( pen, ax, ay, bx, by );
        }
      }

      // !!!}}

    }

    #endregion
  }
}
