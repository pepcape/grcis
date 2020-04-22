using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
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

    static readonly char[] DELIMITERS = {' ', '\t'};

    #endregion

    #region Instance data

    public bool MirrorConversion { get; set; }

    public bool TextureUpsideDown { get; set; }

    #endregion

    #region Construction

    public WavefrontObj ()
    {
      MirrorConversion = false;
      TextureUpsideDown = false;
    }

    #endregion

    #region File-format API

    /// <summary>
    /// Reads one 3D scene from a given file (containing text variant of Wavefront OBJ format).
    /// Can read GZipped files.
    /// </summary>
    /// <param name="fileName">File-name (ending by .gz for gzipped file)</param>
    /// <param name="scene">Scene to be modified</param>
    /// <returns>Number of faces read</returns>
    public int ReadBrep (string fileName, SceneBrep scene)
    {
      if (fileName == null ||
          fileName.Length == 0)
        return SceneBrep.NULL;

      StreamReader reader;
      if (fileName.EndsWith(".gz"))
        reader = new StreamReader(new GZipStream(new FileStream(fileName, FileMode.Open),
                                                 CompressionMode.Decompress));
      else
        reader = new StreamReader(new FileStream(fileName, FileMode.Open));
      int faces = ReadBrep(reader, scene);
      reader.Close();

      return faces;
    }

    /// <summary>
    /// Reads one 3D scene from a given stream (containing text variant of Wavefront OBJ format).
    /// </summary>
    /// <param name="reader">Already open text reader</param>
    /// <param name="scene">Scene to be modified</param>
    /// <returns>Number of faces read</returns>
    public int ReadBrep (StreamReader reader, SceneBrep scene)
    {
      if (reader == null)
        return SceneBrep.NULL;

      Debug.Assert(scene != null);
      scene.Reset();
      return ReadBrep(reader, scene, Matrix4.Identity);
    }

    /// <summary>
    /// Reads one 3D scene from a given stream (containing text variant of Wavefront OBJ format).
    /// </summary>
    /// <param name="reader">Already open text reader</param>
    /// <param name="scene">Scene to be modified</param>
    /// <param name="scene">Matrix for instancing</param>
    /// <returns>Number of faces read</returns>
    public int ReadBrep (StreamReader reader, SceneBrep scene, Matrix4 m)
    {
      if (reader == null)
        return SceneBrep.NULL;

      Debug.Assert(scene != null);
      int v0 = scene.Vertices;
      int lastVertex = v0 - 1;

      int faces = 0;

      List<Vector2> txtCoords    = new List<Vector2>(256);
      List<Vector3> normals      = new List<Vector3>(256);
      int           lastTxtCoord = -1;
      int           lastNormal   = -1;
      int[]         f            = new int[3];

      do
      {
        string line = reader.ReadLine();
        if (line == null)
          break;

        int commentPos = line.IndexOf(COMMENT);
        if (commentPos >= 0)
          line = line.Substring(0, commentPos);

        string[] tokens = line.Split(DELIMITERS, StringSplitOptions.RemoveEmptyEntries);
        if (tokens.Length < 1)
          continue;

        switch (tokens[0])
        {
          case VERTEX:
            if (tokens.Length < 4)
              continue;

            Vector3 coord;
            if (!float.TryParse(tokens[1], NumberStyles.Float, CultureInfo.InvariantCulture, out coord.X) ||
                !float.TryParse(tokens[2], NumberStyles.Float, CultureInfo.InvariantCulture, out coord.Y) ||
                !float.TryParse(tokens[3], NumberStyles.Float, CultureInfo.InvariantCulture, out coord.Z))
              continue;

            if (MirrorConversion)
              coord.Z = -coord.Z;
            lastVertex = scene.AddVertex(Vector3.TransformPosition(coord, m));
            break;

          case VERTEX_TEXTURE:
            if (tokens.Length < 3)
              continue;

            Vector2 txtCoord;
            if (!float.TryParse(tokens[1], NumberStyles.Float, CultureInfo.InvariantCulture, out txtCoord.X) ||
                !float.TryParse(tokens[2], NumberStyles.Float, CultureInfo.InvariantCulture, out txtCoord.Y))
              continue;

            if (TextureUpsideDown)
              txtCoord.Y = 1.0f - txtCoord.Y;

            txtCoords.Add(txtCoord);
            lastTxtCoord++;
            break;

          case VERTEX_NORMAL:
            if (tokens.Length < 4)
              continue;

            Vector3 norm;
            if (!float.TryParse(tokens[1], NumberStyles.Float, CultureInfo.InvariantCulture, out norm.X) ||
                !float.TryParse(tokens[2], NumberStyles.Float, CultureInfo.InvariantCulture, out norm.Y) ||
                !float.TryParse(tokens[3], NumberStyles.Float, CultureInfo.InvariantCulture, out norm.Z))
              continue;

            if (MirrorConversion)
              norm.Z = -norm.Z;
            normals.Add(Vector3.TransformNormal(norm, m));
            lastNormal++;
            break;

          case FACE:
            if (tokens.Length < 4)
              continue;
            int N = tokens.Length - 1;
            if (f.Length < N)
              f = new int[N];
            int i;

            for (i = 0; i < N; i++) // read indices for one vertex
            {
              string[] vt = tokens[i + 1].Split('/');
              int      ti, ni;
              ti = ni = 0; // 0 => value not present

              // 0 .. vertex coord index
              if (!int.TryParse(vt[0], out f[i]) ||
                   f[i] == 0)
                break;

              if (f[i] > 0)
                f[i] = v0 + f[i] - 1;
              else
                f[i] = lastVertex + 1 - f[i];

              if (vt.Length > 1)
              {
                // 1 .. texture coord index (not yet)
                if (!int.TryParse(vt[1], out ti))
                  ti = 0;

                if (vt.Length > 2)
                {
                  // 2 .. normal vector index
                  if (!int.TryParse(vt[2], out ni))
                    ni = 0;
                }
              }

              // there was a texture coord..
              if (ti != 0)
              {
                if (ti > 0)
                  ti--;
                else
                  ti = lastTxtCoord + 1 - ti;
                if (ti >= 0 && ti < txtCoords.Count)
                  scene.SetTxtCoord(f[i], txtCoords[ti]);
              }

              // there was a normal..
              if (ni != 0)
              {
                if (ni > 0)
                  ni--;
                else
                  ni = lastNormal + 1 - ni;
                if (ni >= 0 && ni < normals.Count)
                  scene.SetNormal(f[i], normals[ni]);
              }
            }

            N = i;
            for (i = 1; i < N - 1; i++)
            {
              scene.AddTriangle(f[0], f[i], f[i + 1]);
              faces++;
            }

            break;
        }
      } while (!reader.EndOfStream);

      return faces;
    }

    /// <summary>
    /// Writes the whole B-rep scene to a given text stream (uses text variant of Wavefront OBJ format).
    /// </summary>
    /// <param name="writer">Already open text writer</param>
    /// <param name="scene">Scene to write</param>
    public void WriteBrep (StreamWriter writer, SceneBrep scene)
    {
      if (scene == null || scene.Triangles < 1)
        return;

      int i;
      for (i = 0; i < scene.Vertices; i++)
      {
        Vector3 v = scene.GetVertex(i);
        writer.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0} {1} {2} {3}",
                                       new object[] { VERTEX, v.X, v.Y, v.Z }));
      }

      bool hasNormals = scene.Normals > 0;
      if (hasNormals)
        for (i = 0; i < scene.Vertices; i++)
        {
          Vector3 n = scene.GetNormal(i);
          writer.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0} {1} {2} {3}",
                                         new object[] { VERTEX_NORMAL, n.X, n.Y, n.Z }));
        }

      for (i = 0; i < scene.Triangles; i++)
      {
        int A, B, C;
        scene.GetTriangleVertices(i, out A, out B, out C);
        A++;
        B++;
        C++;
        if (hasNormals)
          writer.WriteLine("{0} {1}//{1} {2}//{2} {3}//{3}", FACE, A, B, C);
        else
          writer.WriteLine("{0} {1} {2} {3}", FACE, A, B, C);
      }
    }

    #endregion
  }
}
