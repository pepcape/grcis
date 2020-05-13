using System;
using MathSupport;
using OpenTK;

namespace Scene3D
{
  public class Construction
  {
    #region Form initialization

    /// <summary>
    /// Optional data initialization.
    /// </summary>
    public static void InitParams ( out string param, out string name )
    {
      param = "";
      name  = "Josef Pelikán";
    }

    #endregion

    #region Instance data

    // !!! If you need any instance data, put them here..

    #endregion

    #region Construction

    public Construction ()
    {
      // !!! Any one-time initialization code goes here..
    }

    #endregion

    #region Mesh construction

    /// <summary>
    /// Construct a new Brep solid (preferebaly closed = regular one).
    /// </summary>
    /// <param name="scene">B-rep scene to be modified</param>
    /// <param name="m">Transform matrix (object-space to world-space)</param>
    /// <param name="param">Shape parameters if needed</param>
    /// <returns>Number of generated faces (0 in case of failure)</returns>
    public int AddMesh ( SceneBrep scene, Matrix4 m, string param )
    {
      // !!!{{ TODO: put your Mesh-construction code here

      // if there will be large number of new vertices, reserve space for them to save time:
      scene.Reserve ( 4 );

      return CreateTetrahedron ( scene, m, Vector3.Zero, 1.0f );

      // !!!}}
    }

    private int CreateTetrahedron ( SceneBrep scene, Matrix4 m, Vector3 center, float size )
    {
      int[] v = new int[4];
      float z = (float) ( size * Math.Sqrt ( 0.5 ) );

      Vector3 A = new Vector3 ( size, 0.0f, -z );
      Vector3 B = new Vector3 ( -size, 0.0f, -z );
      Vector3 C = new Vector3 ( 0.0f, size, z );
      Vector3 D = new Vector3 ( 0.0f, -size, z );

      // vertices:
      v [ 0 ] = scene.AddVertex ( Vector3.TransformPosition ( A, m ) );
      v [ 1 ] = scene.AddVertex ( Vector3.TransformPosition ( B, m ) );
      v [ 2 ] = scene.AddVertex ( Vector3.TransformPosition ( C, m ) );
      v [ 3 ] = scene.AddVertex ( Vector3.TransformPosition ( D, m ) );

      // normal vectors:
      scene.SetNormal ( v [ 0 ], Vector3.TransformVector ( A, m ).Normalized () );
      scene.SetNormal ( v [ 1 ], Vector3.TransformVector ( B, m ).Normalized () );
      scene.SetNormal ( v [ 2 ], Vector3.TransformVector ( C, m ).Normalized () );
      scene.SetNormal ( v [ 3 ], Vector3.TransformVector ( D, m ).Normalized () );

      // texture coordinates:
      scene.SetTxtCoord ( v [ 0 ], new Vector2 ( 1.0f, 0.0f ) );
      scene.SetTxtCoord ( v [ 1 ], new Vector2 ( 0.0f, 0.0f ) );
      scene.SetTxtCoord ( v [ 2 ], new Vector2 ( 1.0f, 1.0f ) );
      scene.SetTxtCoord ( v [ 3 ], new Vector2 ( 0.0f, 1.0f ) );

      // colors:
      ulong seed = (ulong) Math.Min ( ulong.MaxValue, ( m.Row3.LengthSquared * 10000.0f ) );
      seed = RandomStatic.numericRecipes ( seed );
      float r = ( seed & 255 ) / 255.0f;
      seed = RandomStatic.numericRecipes ( seed );
      float g = ( seed & 255 ) / 255.0f;
      seed = RandomStatic.numericRecipes ( seed );
      float b = ( seed & 255 ) / 255.0f;
      scene.SetColor ( v [ 0 ], new Vector3 ( r, g, b ) );
      r = Math.Min ( r + 0.2f, 1.0f );
      scene.SetColor ( v [ 1 ], new Vector3 ( r, g, b ) );
      g = Math.Min ( g + 0.2f, 1.0f );
      scene.SetColor ( v [ 2 ], new Vector3 ( r, g, b ) );
      b = Math.Min ( b + 0.2f, 1.0f );
      scene.SetColor ( v [ 3 ], new Vector3 ( r, g, b ) );

      // triangle faces:
      scene.AddTriangle ( v [ 0 ], v [ 1 ], v [ 2 ] );
      scene.AddTriangle ( v [ 2 ], v [ 1 ], v [ 3 ] );
      scene.AddTriangle ( v [ 1 ], v [ 0 ], v [ 3 ] );
      scene.AddTriangle ( v [ 2 ], v [ 3 ], v [ 0 ] );

      return 4;
    }

    #endregion
  }
}
