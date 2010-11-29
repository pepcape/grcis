using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Globalization;
using OpenTK;

namespace Scene3D
{
  /// <summary>
  /// Wavefront OBJ file-format.
  /// </summary>
  public class WavefrontObj
  {
    #region Constants

    /// <summary>
    /// Comments are introduced by this character.
    /// </summary>
    const char COMMENT = '#';

    const string VERTEX = "v";

    const string VERTEX_NORMAL = "vn";

    const string VERTEX_TEXTURE = "vt";

    const string FACE = "f";

    #endregion

    #region Instance data

    public bool MirrorConversion { get; set; }

    #endregion

    #region Construction

    public WavefrontObj ()
    {
      MirrorConversion = false;
    }

    #endregion

    #region File-format API

    public int ReadBrep ( StreamReader reader, SceneBrep scene )
    {
      if ( reader == null ) return SceneBrep.NULL;

      Debug.Assert( scene != null );
      scene.Reset();
      int faces = 0;
      List<Vector3> normals = new List<Vector3>( 256 );
      int[] f = new int[ 3 ];

      do
      {
        string line = reader.ReadLine();
        if ( line == null ) break;

        int commentPos = line.IndexOf( COMMENT );
        if ( commentPos >= 0 )
          line = line.Substring( 0, commentPos );

        string[] tokens = line.Split( ' ' );

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
            scene.AddVertex( coord );
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
            normals.Add( norm );
            break;

          case FACE:
            if ( tokens.Length < 4 ) continue;
            int i;

            for ( i = 0; i < 3; i++ )
            {
              string[] vt = tokens[ i + 1 ].Split( '/' );
              int ti, ni;
              ti = ni = SceneBrep.NULL;
              // 0 .. vertex coord index
              if ( !int.TryParse( vt[ 0 ], out f[ i ] ) ) break;
              f[ i ]--;
              if ( vt.Length >= 2 )
              {
                // 1 .. texture coord index (not yet)
                int.TryParse( vt[ 1 ], out ti );
                ti--;
                if ( vt.Length >= 3 )
                {
                  // 2 .. normal vector index
                  int.TryParse( vt[ 2 ], out ni );
                  ni--;
                }
              }
              if ( ni >= 0 && ni < normals.Count )
                scene.SetNormal( f[ i ], normals[ ni ] );
            }

            if ( i >= 3 )
            {
              scene.AddTriangle( f[ 0 ], f[ 1 ], f[ 2 ] );
              faces++;
            }

            break;
        }
      }
      while ( !reader.EndOfStream );

      return faces;
    }

    #endregion

  }
}
