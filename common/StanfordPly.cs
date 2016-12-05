using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Text;
using OpenTK;

namespace Scene3D
{
  /// <summary>
  /// Stanford Triangle Format / Polygon File Format / PLY.
  /// </summary>
  /// <see cref="http://paulbourke.net/dataformats/ply/">File-format overview</see>
  public class StanfordPly
  {
    #region Constants

    /// <summary>
    /// The first line (including the ending character CR) is used as a 4-byte magic number.
    /// </summary>
    const string HEADER = "ply";

    /// <summary>
    /// Comments are introduced by this string.
    /// </summary>
    const string COMMENT = "comment";

    const string FORMAT_TEXT = "format ascii 1.0";

    const string FORMAT_BINARY_LE = "format binary_little_endian 1.0";

    const string FORMAT_BINARY_BE = "format binary_big_endian 1.0";

    const string ELEMENT = "element";

    const string VERTEX = "vertex";

    const string FACE = "face";

    const string PROPERTY = "property";

    const string NORMAL_X = "nx";

    const string NORMAL_Y = "ny";

    const string NORMAL_Z = "nz";

    const string TEXTURE_S = "s";

    const string TEXTURE_T = "t";

    const string COLOR_R = "red";

    const string COLOR_G = "green";

    const string COLOR_B = "blue";

    const string END_HEADER = "end_header";

    static string FORMAT_BINARY
    {
      get
      {
        return BitConverter.IsLittleEndian ? FORMAT_BINARY_LE : FORMAT_BINARY_BE;
      }
    }

    #endregion

    #region Instance data

    /// <summary>
    /// Change axis orientation in import/export.
    /// </summary>
    public bool Orientation { get; set; }

    public bool TextFormat { get; set; }

    public bool NativeNewLine { get; set; }

    public bool DoNormals { get; set; }

    public bool DoTxtCoords { get; set; }

    public bool DoColors { get; set; }

    #endregion

    #region Construction

    public StanfordPly ()
    {
      Orientation   = false;
      TextFormat    = true;
      NativeNewLine = true;
      DoNormals     = true;
      DoTxtCoords   = true;
      DoColors      = true;
    }

    #endregion

    #region File-format API

    /// <summary>
    /// Reads one 3D scene from a given file (containing text/binary variant of Stanford PLY format).
    /// Can read GZipped files.
    /// </summary>
    /// <param name="fileName">File-name (ending by .gz for gzipped file)</param>
    /// <param name="scene">Scene to be modified</param>
    /// <returns>Number of faces read</returns>
    public int ReadBrep ( string fileName, SceneBrep scene )
    {
      if ( fileName == null ||
           fileName.Length == 0 )
        return SceneBrep.NULL;

      StreamReader reader;
      if ( fileName.EndsWith( ".gz" ) )
        reader = new StreamReader( new GZipStream( new FileStream( fileName, FileMode.Open ), CompressionMode.Decompress ) );
      else
        reader = new StreamReader( new FileStream( fileName, FileMode.Open ) );
      int faces = ReadBrep( reader, scene );
      reader.Close();

      return faces;
    }

    /// <summary>
    /// Reads one 3D scene from a given stream (containing text/binary variant of Stanford PLY format).
    /// </summary>
    /// <param name="reader">Already open text reader</param>
    /// <param name="scene">Scene to be modified</param>
    /// <returns>Number of faces read</returns>
    public int ReadBrep ( StreamReader reader, SceneBrep scene )
    {
      if ( reader == null )
        return SceneBrep.NULL;

      Debug.Assert( scene != null );
      scene.Reset();
      return ReadBrep( reader, scene, Matrix4.Identity );
    }

    /// <summary>
    /// Reads one 3D scene from a given stream (containing text/binary variant of Stanford PLY format).
    /// </summary>
    /// <param name="reader">Already open text reader</param>
    /// <param name="scene">Scene to be modified</param>
    /// <param name="scene">Matrix for instancing</param>
    /// <returns>Number of faces read</returns>
    public int ReadBrep ( StreamReader reader, SceneBrep scene, Matrix4 m )
    {
      if ( reader == null )
        return SceneBrep.NULL;

      Debug.Assert( scene != null );
      int v0 = scene.Vertices;
      int lastVertex = v0 - 1;

      int faces = 0;

      // !!! TODO: not implemented yet !!!
#if false
      List<Vector2> txtCoords = new List<Vector2>( 256 );
      List<Vector3> normals = new List<Vector3>( 256 );
      int lastTxtCoord = -1;
      int lastNormal = -1;
      int[] f = new int[ 3 ];

      do
      {
        string line = reader.ReadLine();
        if ( line == null ) break;

        int commentPos = line.IndexOf( COMMENT );
        if ( commentPos >= 0 )
          line = line.Substring( 0, commentPos );

        string[] tokens = line.Split( DELIMITERS , StringSplitOptions.RemoveEmptyEntries );
        if ( tokens.Length < 1 ) continue;

        switch ( tokens[ 0 ] )
        {
          case VERTEX:
            if ( tokens.Length < 4 ) continue;

            Vector3 coord;
            if ( !float.TryParse( tokens[ 1 ], NumberStyles.Float, CultureInfo.InvariantCulture, out coord.X ) ||
                 !float.TryParse( tokens[ 2 ], NumberStyles.Float, CultureInfo.InvariantCulture, out coord.Y ) ||
                 !float.TryParse( tokens[ 3 ], NumberStyles.Float, CultureInfo.InvariantCulture, out coord.Z ) )
              continue;

            if ( MirrorConversion )
              coord.Z = -coord.Z;
            lastVertex = scene.AddVertex( Vector3.Transform( coord, m ) );
            break;

          case VERTEX_TEXTURE:
            if ( tokens.Length < 3 ) continue;

            Vector2 txtCoord;
            if ( !float.TryParse( tokens[1], NumberStyles.Float, CultureInfo.InvariantCulture, out txtCoord.X ) ||
                 !float.TryParse( tokens[2], NumberStyles.Float, CultureInfo.InvariantCulture, out txtCoord.Y ) )
              continue;

            txtCoords.Add( txtCoord );
            lastTxtCoord++;
            break;

          case VERTEX_NORMAL:
            if ( tokens.Length < 4 ) continue;

            Vector3 norm;
            if ( !float.TryParse( tokens[ 1 ], NumberStyles.Float, CultureInfo.InvariantCulture, out norm.X ) ||
                 !float.TryParse( tokens[ 2 ], NumberStyles.Float, CultureInfo.InvariantCulture, out norm.Y ) ||
                 !float.TryParse( tokens[ 3 ], NumberStyles.Float, CultureInfo.InvariantCulture, out norm.Z ) )
              continue;

            if ( MirrorConversion )
              norm.Z = -norm.Z;
            normals.Add( Vector3.TransformNormal( norm, m ) );
            lastNormal++;
            break;

          case FACE:
            if ( tokens.Length < 4 ) continue;
            int N = tokens.Length - 1;
            if ( f.Length < N )
              f = new int[ N ];
            int i;

            for ( i = 0; i < N; i++ )       // read indices for one vertex
            {
              string[] vt = tokens[ i + 1 ].Split( '/' );
              int ti, ni;
              ti = ni = 0;                  // 0 => value not present

              // 0 .. vertex coord index
              if ( !int.TryParse( vt[ 0 ], out f[ i ] ) ||
                   f[ i ] == 0 )
                break;

              if ( f[ i ] > 0 )
                f[ i ] = v0 + f[ i ] - 1;
              else
                f[ i ] = lastVertex + 1 - f[ i ];

              if ( vt.Length > 1 )
              {
                // 1 .. texture coord index (not yet)
                if ( !int.TryParse( vt[ 1 ], out ti ) ) ti = 0;

                if ( vt.Length > 2 )
                {
                  // 2 .. normal vector index
                  if ( !int.TryParse( vt[ 2 ], out ni ) ) ni = 0;
                }
              }

              // there was a texture coord..
              if ( ti != 0 )
              {
                if( ti > 0 )
                  ti--;
                else
                  ti = lastTxtCoord + 1 - ti;
                if ( ti >= 0 && ti < txtCoords.Count )
                  scene.SetTxtCoord( f[i], txtCoords[ti] );
              }

              // there was a normal..
              if ( ni != 0 )
              {
                if ( ni > 0 )
                  ni--;
                else
                  ni = lastNormal + 1 - ni;
                if ( ni >= 0 && ni < normals.Count )
                  scene.SetNormal( f[ i ], normals[ ni ] );
              }
            }

            N = i;
            for ( i = 1; i < N - 1; i++ )
            {
              scene.AddTriangle( f[ 0 ], f[ i ], f[ i + 1 ] );
              faces++;
            }

            break;
        }
      }
      while ( !reader.EndOfStream );
#endif

      return faces;
    }

    /// <summary>
    /// Writes the whole B-rep scene to a given text stream (uses text variant of Stanford PLY format).
    /// </summary>
    /// <param name="writer">Already open text writer</param>
    /// <param name="scene">Scene to write</param>
    public void WriteBrep ( StreamWriter writer, SceneBrep scene )
    {
      if ( scene == null ||
           scene.Triangles < 1 )
        return;

      Debug.Assert( TextFormat );
      if ( !NativeNewLine )
        writer.NewLine = "\r";     // CR only

      bool writeNormals   = DoNormals   && scene.HasNormals();
      bool writeTxtCoords = DoTxtCoords && scene.HasTxtCoords();
      bool writeColors    = DoColors    && scene.HasColors();

      writer.WriteLine( HEADER );
      writer.WriteLine( FORMAT_TEXT );

      // vertex-header:
      writer.WriteLine( "{0} {1} {2}", ELEMENT, VERTEX, scene.Vertices );
      writer.WriteLine( "{0} float x", PROPERTY );
      writer.WriteLine( "{0} float {1}", PROPERTY, Orientation ? 'z' : 'y' );
      writer.WriteLine( "{0} float {1}", PROPERTY, Orientation ? 'y' : 'z' );
      if ( writeNormals )
      {
        writer.WriteLine( "{0} float {1}", PROPERTY, NORMAL_X );
        writer.WriteLine( "{0} float {1}", PROPERTY, Orientation ? NORMAL_Z : NORMAL_Y );
        writer.WriteLine( "{0} float {1}", PROPERTY, Orientation ? NORMAL_Y : NORMAL_Z );
      }
      if ( writeTxtCoords )
      {
        writer.WriteLine( "{0} float {1}", PROPERTY, TEXTURE_S );
        writer.WriteLine( "{0} float {1}", PROPERTY, TEXTURE_T );
      }
      if ( writeColors )
      {
        writer.WriteLine( "{0} float {1}", PROPERTY, COLOR_R );
        writer.WriteLine( "{0} float {1}", PROPERTY, COLOR_G );
        writer.WriteLine( "{0} float {1}", PROPERTY, COLOR_B );
      }

      // face-header:
      writer.WriteLine( "{0} {1} {2}", ELEMENT, FACE, scene.Triangles );
      writer.WriteLine( "{0} list uchar int vertex_indices", PROPERTY );

      writer.WriteLine( END_HEADER );

      // vertex-data:
      int i;
      Vector3 v3;
      Vector2 v2;
      StringBuilder sb = new StringBuilder();
      for ( i = 0; i < scene.Vertices; i++ )
      {
        v3 = scene.GetVertex( i );
        sb.Clear();
        sb.AppendFormat( CultureInfo.InvariantCulture, "{0} {1} {2}", v3.X, v3.Y, v3.Z );
        if ( writeNormals )
        {
          v3 = scene.GetNormal( i );
          sb.AppendFormat( CultureInfo.InvariantCulture, " {0} {1} {2}", v3.X, v3.Y, v3.Z );
        }
        if ( writeTxtCoords )
        {
          v2 = scene.GetTxtCoord( i );
          sb.AppendFormat( CultureInfo.InvariantCulture, " {0} {1}", v2.X, v2.Y );
        }
        if ( writeColors )
        {
          v3 = scene.GetColor( i );
          sb.AppendFormat( CultureInfo.InvariantCulture, " {0} {1} {2}", v3.X, v3.Y, v3.Z );
        }
        writer.WriteLine( sb.ToString() );
      }

      // face-data:
      int A, B, C;
      for ( i = 0; i < scene.Triangles; i++ )
      {
        scene.GetTriangleVertices( i, out A, out B, out C );
        writer.WriteLine( "3 {0} {1} {2}", A, Orientation ? C : B, Orientation ? B : C );
      }
    }

#endregion
  }
}
