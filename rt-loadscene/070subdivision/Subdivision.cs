using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics;

namespace Scene3D
{
  public class Subdivision
  {
    #region Instance data

    // !!! If you need any instance data, put them here..

    /// <summary>
    /// Working copy of the triangle-mesh. The one actual used for subdivision..
    /// </summary>
    public SceneBrep result;

    #endregion

    #region Construction

    public Subdivision ()
    {
      // !!! Any one-time initialization code goes here..
    }

    #endregion

    #region Control mesh construction

    /// <summary>
    /// Construct a new control mesh for subdivision (formally the Brep solid..).
    /// </summary>
    /// <param name="scene">B-rep scene to be modified</param>
    /// <param name="m">Transform matrix (object-space to world-space)</param>
    /// <param name="time">Current time in seconds</param>
    /// <param name="param">Shape parameters if needed</param>
    public void ControlMesh ( SceneBrep scene, Matrix4 m, float time, string param )
    {
      // !!!{{ TODO: put your Control-mesh-construction code here

      List<int> upper = new List<int>();
      upper.Add( scene.AddVertex( Vector3.TransformPosition( new Vector3( 0, 1, 0 ), m ) ) );
      upper.Add( scene.AddVertex( Vector3.TransformPosition( new Vector3( 1, 1, 0 ), m ) ) );
      upper.Add( scene.AddVertex( Vector3.TransformPosition( new Vector3( 1, 1, 1 ), m ) ) );
      upper.Add( scene.AddVertex( Vector3.TransformPosition( new Vector3( 0, 1, 1 ), m ) ) );

      List<int> lower = new List<int>();
      lower.Add( scene.AddVertex( Vector3.TransformPosition( new Vector3( 0, 0, 0 ), m ) ) );
      lower.Add( scene.AddVertex( Vector3.TransformPosition( new Vector3( 1, 0, 0 ), m ) ) );
      lower.Add( scene.AddVertex( Vector3.TransformPosition( new Vector3( 1, 0, 1 ), m ) ) );
      lower.Add( scene.AddVertex( Vector3.TransformPosition( new Vector3( 0, 0, 1 ), m ) ) );

      // Lower face
      scene.AddTriangle( lower[ 0 ], lower[ 1 ], lower[ 2 ] );
      scene.AddTriangle( lower[ 2 ], lower[ 3 ], lower[ 0 ] );

      // Upper face
      scene.AddTriangle( upper[ 2 ], upper[ 1 ], upper[ 0 ] );
      scene.AddTriangle( upper[ 0 ], upper[ 3 ], upper[ 2 ] );

      // Side faces
      for ( int i = 0; i < upper.Count; i++ )
      {
        int j = i < (upper.Count - 1) ? i + 1 : 0;
        scene.AddTriangle( upper[ i ], upper[ j ], lower[ i ] );
        scene.AddTriangle( lower[ i ], upper[ j ], lower[ j ] );
      }

      // !!!}}
    }

    #endregion

    #region Loop subdivision

    /// <summary>
    /// Do Loop subdivision of the input control triangle mesh.
    /// </summary>
    /// <param name="scene">Input scene (must not be changed).</param>
    /// <param name="epsilon">Reference error toleration (size of undivided triangle).</param>
    /// <param name="normals">Generate normals? (optional)</param>
    /// <param name="colors">Generate colors? (optional)</param>
    /// <param name="txtcoord">Generate texture coordinates? (optional)</param>
    /// <param name="time">Current time for animation (optional)</param>
    /// <param name="param">Optional additional parameters.</param>
    /// <returns>Number of generated points.</returns>
    public int Subdivide ( SceneBrep scene, float epsilon, bool normals, bool colors, bool txtcoord, float time, string param )
    {
      result = scene.Clone();

      // !!!{{ TODO: put your Loop subdivision code here

      // pilot: do one (trivial) division
      int triangles = result.Triangles;
      int tr;
      for ( tr = 0; tr < triangles; tr++ )
      {
        int A, B, C;
        result.GetTriangleVertices( tr, out A, out B, out C );
        Vector3 vA = result.GetVertex( A );
        Vector3 vB = result.GetVertex( B );
        Vector3 vC = result.GetVertex( C );

        Vector3 vA2 = (vB + vC) * 0.5f;
        Vector3 vB2 = (vA + vC) * 0.5f;
        Vector3 vC2 = (vA + vB) * 0.5f;
        int A2 = result.AddVertex( vA2 );
        int B2 = result.AddVertex( vB2 );
        int C2 = result.AddVertex( vC2 );

        result.AddTriangle( B2, C2, A2 );
        result.AddTriangle( B2, A2, C );
        result.AddTriangle( C2, B, A2 );
        result.SetTriangleVertices( tr, A, C2, B2 );
      }

      //result.BuildCornerTable();

      return result.Vertices;

      // !!!}}
    }

    /// <summary>
    /// Computes vertex array size (VBO) in bytes.
    /// </summary>
    /// <param name="normals">Use normal vectors?</param>
    /// <param name="colors">Use vertex colors?</param>
    /// <param name="txtcoord">Use texture coordinates?</param>
    /// <returns>Buffer size in bytes</returns>
    public int VertexBufferSize ( bool normals, bool colors, bool txtcoord )
    {
      if ( result == null ) return 0;

      int size = result.Vertices * 3 * sizeof( float );
      if ( txtcoord )
        size += result.Vertices * 2 * sizeof( float );
      if ( colors )
        size += result.Vertices * 3 * sizeof( float );
      if ( normals )
        size += result.Vertices * 3 * sizeof( float );

      return size;
    }

    /// <summary>
    /// Fill vertex data into the provided memory array (VBO after MapBuffer).
    /// </summary>
    /// <param name="ptr">Memory pointer</param>
    /// <param name="normals">Use normal vectors?</param>
    /// <param name="colors">Use vertex colors?</param>
    /// <param name="txtcoord">Use texture coordinates?</param>
    /// <returns>Stride (vertex size) in bytes</returns>
    public unsafe int FillVertexBuffer ( float* ptr, bool normals, bool colors, bool txtcoord )
    {
      // !!!{{ TODO: put your buffer-fill code here

      if ( result == null ) return 0;

      // Trivial solution: use all the working-scene vertices..
      // Not the case => rewrite the code!
      return result.FillVertexBuffer( ptr, true, txtcoord, colors, normals );

      // !!!}}
    }

    #endregion
  }
}
