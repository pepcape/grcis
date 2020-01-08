using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using OpenTK;

namespace Scene3D
{
  using Edge = KeyValuePair<int, int>;

  /// <summary>
  /// B-rep 3D scene with associated corner-table (Jarek Rossignac).
  /// </summary>
  public partial class SceneBrep : ICloneable
  {
    #region Constants

    /// <summary>
    /// Invalid handle (for vertices, trinagles, corners..).
    /// </summary>
    public const int NULL = -1;

    #endregion

    #region Scene data

    /// <summary>
    /// Array of vertex coordinates (float[3]).
    /// </summary>
    protected List<Vector3> geometry = null;

    /// <summary>
    /// Array of normal vectors (non mandatory).
    /// </summary>
    protected List<Vector3> normals = null;

    /// <summary>
    /// Array of vertex colors (non mandatory).
    /// </summary>
    protected List<Vector3> colors = null;

    /// <summary>
    /// Array of 2D texture coordinates (non mandatory).
    /// </summary>
    protected List<Vector2> txtCoords = null;

    /// <summary>
    /// Vertex pointer (handle) for each triangle corner.
    /// Or vertex of the line.
    /// 'vertexPtr' must never be null (see Reset()).
    /// </summary>
    protected List<int> vertexPtr = null;

    /// <summary>
    /// Opposite corner pointer (handle) for each triangle corner.
    /// Valid only for topological scene (triangles are connected).
    /// </summary>
    protected List<int> oppositePtr = null;

    /// <summary>
    /// If true, lines are used instead of triangles.
    /// Valid after 1st triangle or line was inserted into the scene.
    /// </summary>
    protected bool lines = false;

    /// <summary>
    /// Returns true if the scene is (or can be) composed of lines.
    /// </summary>
    public bool HasLines => lines || vertexPtr.Count == 0;

    /// <summary>
    /// Returns true if the scene is (or can be) composed of triangles.
    /// </summary>
    public bool HasTriangles => !lines || vertexPtr.Count == 0;

    public int statEdges  = 0;
    public int statShared = 0;

    #endregion

    #region Construction

    public SceneBrep ()
    {
      Reset();
    }

    #endregion

    object ICloneable.Clone ()
    {
      return Clone();
    }

    public SceneBrep Clone ()
    {
      SceneBrep tmp = new SceneBrep();
      tmp.geometry = new List<Vector3>(geometry);
      if (normals != null)     tmp.normals = new List<Vector3>(normals);
      if (colors != null)      tmp.colors = new List<Vector3>(colors);
      if (txtCoords != null)   tmp.txtCoords = new List<Vector2>(txtCoords);
      if (vertexPtr != null)   tmp.vertexPtr = new List<int>(vertexPtr);
      if (oppositePtr != null) tmp.oppositePtr = new List<int>(oppositePtr);
      tmp.lines = lines;
      tmp.LineWidth = LineWidth;

      tmp.BuildCornerTable();
      return tmp;
    }

    #region B-rep API

    /// <summary>
    /// [Re]initializes the scene data.
    /// </summary>
    public void Reset ()
    {
      geometry    = new List<Vector3>(256);
      normals     = null;
      colors      = null;
      txtCoords   = null;
      vertexPtr   = new List<int>(256);
      oppositePtr = null;
      lines       = false;
      LineWidth   = 2.0f;
    }

    /// <summary>
    /// Reserve space for additional vertices.
    /// </summary>
    /// <param name="additionalVertices">Number of new vertices going to be inserted.</param>
    public void Reserve (int additionalVertices)
    {
      if (additionalVertices < 0)
        return;

      int newReserve = geometry.Count + additionalVertices;
      geometry.Capacity = newReserve;
      if (normals != null)   normals.Capacity = newReserve;
      if (colors != null)    colors.Capacity = newReserve;
      if (txtCoords != null) txtCoords.Capacity = newReserve;

      newReserve = vertexPtr.Count + additionalVertices * 3;
      vertexPtr.Capacity = newReserve;
      if (oppositePtr != null) oppositePtr.Capacity = newReserve;
    }

    /// <summary>
    /// Current number of vertices in the scene.
    /// </summary>
    public int Vertices => (geometry == null) ? 0 : geometry.Count;

    /// <summary>
    /// Current number of normal vectors in the scene (should be 0 or the same as Vertices).
    /// </summary>
    public int Normals => (normals == null) ? 0 : normals.Count;

    public bool HasNormals ()
    {
      return normals != null;
    }

    public int NormalBytes ()
    {
      return (normals != null) ? 3 * sizeof(float) : 0;
    }

    /// <summary>
    /// Current number of vertex colors in the scene (should be 0 or the same as Vertices).
    /// </summary>
    public int Colors => (colors == null) ? 0 : colors.Count;

    public bool HasColors ()
    {
      return colors != null;
    }

    public int ColorBytes ()
    {
      return (colors != null) ? 3 * sizeof(float) : 0;
    }

    /// <summary>
    /// Current number of texture coordinates in the scene (should be 0 or the same as Vertices).
    /// </summary>
    public int TxtCoords => (txtCoords == null) ? 0 : txtCoords.Count;

    public bool HasTxtCoords ()
    {
      return txtCoords != null;
    }

    public int TxtCoordsBytes ()
    {
      return (txtCoords != null) ? 2 * sizeof(float) : 0;
    }

    /// <summary>
    /// Current number of triangles in the scene.
    /// Zero if lines are used.
    /// </summary>
    public int Triangles
    {
      get
      {
        if (HasLines)
          return 0;

        Debug.Assert(vertexPtr.Count % 3 == 0, "Invalid V[] size");
        return vertexPtr.Count / 3;
      }
    }

    /// <summary>
    /// Current number of Lines.
    /// Zero if triangles are used.
    /// </summary>
    public int Lines
    {
      get
      {
        if (HasTriangles)
          return 0;

        Debug.Assert(vertexPtr.Count % 2 == 0, "Invalid V[] size");
        return vertexPtr.Count / 2;
      }
    }

    /// <summary>
    /// Line-width for rendering.
    /// </summary>
    public float LineWidth { get; set; } = 2.0f;

    /// <summary>
    /// Current number of corners in the scene (# of triangles times three).
    /// </summary>
    public int Corners => HasLines ? 0 : vertexPtr.Count;

    /// <summary>
    /// Add a new vertex defined by its 3D coordinate.
    /// </summary>
    /// <param name="v">Vertex coordinate in the object space</param>
    /// <returns>Vertex handle</returns>
    public int AddVertex (Vector3 v)
    {
      Debug.Assert(geometry != null);

      int handle = geometry.Count;
      geometry.Add(v);

      if (normals != null)
      {
        Debug.Assert(normals.Count == handle, "Invalid N[] size");
        normals.Add(Vector3.UnitY);
      }

      if (colors != null)
      {
        Debug.Assert(colors.Count == handle, "Invalid C[] size");
        colors.Add(Vector3.One);
      }

      if (txtCoords != null)
      {
        Debug.Assert(txtCoords.Count == handle, "Invalid T[] size");
        txtCoords.Add(Vector2.Zero);
      }

      return handle;
    }

    /// <summary>
    /// Returns object-space coordinates of the given vertex.
    /// </summary>
    /// <param name="v">Vertex handle</param>
    /// <returns>Object-space coordinates</returns>
    public Vector3 GetVertex (int v)
    {
      Debug.Assert(geometry != null, "Invalid G[]");
      Debug.Assert(0 <= v && v < geometry.Count, "Invalid vertex handle");

      return geometry[v];
    }

    public void SetVertex (int v, Vector3 pos)
    {
      Debug.Assert(geometry != null, "Invalid G[]");
      Debug.Assert(0 <= v && v < geometry.Count, "Invalid vertex handle");

      geometry[v] = pos;
    }

    /// <summary>
    /// Assigns a normal vector to an existing vertex
    /// </summary>
    /// <param name="v">Vertex handle</param>
    /// <param name="normal">New normal vector</param>
    public void SetNormal (int v, Vector3 normal)
    {
      Debug.Assert(geometry != null, "Invalid G[]");
      Debug.Assert(0 <= v && v < geometry.Count, "Invalid vertex handle");

      if (normals == null)
      {
        normals = new List<Vector3>(geometry.Count);
        for (int i = 0; i < geometry.Count; i++)
          normals.Add(Vector3.UnitX);
      }

      normals[v] = normal;
    }

    /// <summary>
    /// Returns normal vector of the given vertex.
    /// </summary>
    /// <param name="v">Vertex handle</param>
    /// <returns>Normal vector</returns>
    public Vector3 GetNormal (int v)
    {
      Debug.Assert(normals != null, "Invalid N[]");
      Debug.Assert(0 <= v && v < normals.Count, "Invalid vertex handle");

      return normals[v];
    }

    /// <summary>
    /// Assigns a color to an existing vertex
    /// </summary>
    /// <param name="v">Vertex handle</param>
    /// <param name="color">New vertex color</param>
    public void SetColor (int v, Vector3 color)
    {
      Debug.Assert(geometry != null, "Invalid G[]");
      Debug.Assert(0 <= v && v < geometry.Count, "Invalid vertex handle");

      if (colors == null)
      {
        colors = new List<Vector3>(geometry.Count);
        for (int i = 0; i < geometry.Count; i++)
          colors.Add(Vector3.One);
      }

      colors[v] = color;
    }

    /// <summary>
    /// Returns color of the given vertex.
    /// </summary>
    /// <param name="v">Vertex handle</param>
    /// <returns>Vertex color</returns>
    public Vector3 GetColor (int v)
    {
      Debug.Assert(colors != null, "Invalid C[]");
      Debug.Assert(0 <= v && v < colors.Count, "Invalid vertex handle");

      return colors[v];
    }

    /// <summary>
    /// Assigns a texture coordinate to an existing vertex
    /// </summary>
    /// <param name="v">Vertex handle</param>
    /// <param name="txt">New texture coordinate</param>
    public void SetTxtCoord (int v, Vector2 txt)
    {
      Debug.Assert(geometry != null, "Invalid G[]");
      Debug.Assert(0 <= v && v < geometry.Count, "Invalid vertex handle");

      if (txtCoords == null)
      {
        txtCoords = new List<Vector2>(geometry.Count);
        for (int i = 0; i < geometry.Count; i++)
          txtCoords.Add(Vector2.Zero);
      }

      txtCoords[v] = txt;
    }

    /// <summary>
    /// Returns texture coordinate of the given vertex.
    /// </summary>
    /// <param name="v">Vertex handle</param>
    /// <returns>Texture coordinate</returns>
    public Vector2 GetTxtCoord (int v)
    {
      Debug.Assert(txtCoords != null, "Invalid T[]");
      Debug.Assert(0 <= v && v < txtCoords.Count, "Invalid vertex handle");

      return txtCoords[v];
    }

    /// <summary>
    /// Adds a new triangle face defined by its vertices.
    /// </summary>
    /// <param name="v1">Handle of the 1st vertex</param>
    /// <param name="v2">Handle of the 2nd vertex</param>
    /// <param name="v3">Handle of the 3rd vertex</param>
    /// <returns>Triangle handle</returns>
    public int AddTriangle (int v1, int v2, int v3)
    {
      Debug.Assert(HasTriangles, "Triangles are mixed with lines");
      Debug.Assert(geometry != null, "Invalid G[] size");
      Debug.Assert(geometry.Count > v1 &&
                   geometry.Count > v2 &&
                   geometry.Count > v3, "Invalid vertex handle");
      Debug.Assert(vertexPtr != null && (vertexPtr.Count % 3 == 0),
                   "Invalid corner-table (V[] size)");

      if (!HasTriangles)
        return NULL;

      int handle1 = vertexPtr.Count;
      vertexPtr.Add(v1);
      vertexPtr.Add(v2);
      vertexPtr.Add(v3);

      if (oppositePtr != null)
      {
        Debug.Assert(oppositePtr.Count == handle1, "Invalid O[] size");
        oppositePtr.Add(NULL);
        oppositePtr.Add(NULL);
        oppositePtr.Add(NULL);
      }

      return handle1 / 3;
    }

    /// <summary>
    /// Add a new line.
    /// Lines must not be mixed with triangles.
    /// </summary>
    /// <param name="v1">Handle of the 1st vertex</param>
    /// <param name="v2">Handle of the 2nd vertex</param>
    /// <returns>Line handle</returns>
    public int AddLine (int v1, int v2)
    {
      Debug.Assert(HasLines, "Triangles are mixed with lines");
      Debug.Assert(vertexPtr.Count % 2 == 0,
                   "Invalid V[] size");
      Debug.Assert(geometry != null, "Invalid G[] size");
      Debug.Assert(geometry.Count > v1 &&
                   geometry.Count > v2, "Invalid vertex handle");

      if (!HasLines)
        return NULL;

      lines = true;
      int handle1 = vertexPtr.Count;
      vertexPtr.Add(v1);
      vertexPtr.Add(v2);

      return handle1 / 2;
    }

    /// <summary>
    /// Returns vertex handles of the given triangle.
    /// </summary>
    /// <param name="tr">Triangle handle</param>
    /// <param name="v1">Variable to receive the 1st vertex handle</param>
    /// <param name="v2">Variable to receive the 2nd vertex handle</param>
    /// <param name="v3">Variable to receive the 3rd vertex handle</param>
    public void GetTriangleVertices (int tr, out int v1, out int v2, out int v3)
    {
      Debug.Assert(HasTriangles, "This scene is composed of lines");
      Debug.Assert(geometry != null, "Invalid G[] size");
      tr *= 3;
      Debug.Assert(0 <= tr && tr + 2 < vertexPtr.Count,
                   "Invalid triangle handle");

      v1 = vertexPtr[tr];
      v2 = vertexPtr[tr + 1];
      v3 = vertexPtr[tr + 2];
    }

    /// <summary>
    /// Returns vertex handles of the given line.
    /// </summary>
    /// <param name="li">Line handle</param>
    /// <param name="v1">Variable to receive the 1st vertex handle</param>
    /// <param name="v2">Variable to receive the 2nd vertex handle</param>
    public void GetLineVertices (int li, out int v1, out int v2)
    {
      Debug.Assert(HasLines, "This scene is composed of triangles");
      Debug.Assert(geometry != null, "Invalid G[] size");
      li *= 2;
      Debug.Assert(0 <= li && li + 1 < vertexPtr.Count,
                   "Invalid line handle");

      v1 = vertexPtr[li];
      v2 = vertexPtr[li + 1];
    }

    /// <summary>
    /// Changes vertices of an existing triangle.
    /// </summary>
    /// <param name="v1">New handle of the 1st vertex</param>
    /// <param name="v2">New handle of the 2nd vertex</param>
    /// <param name="v3">New handle of the 3rd vertex</param>
    /// <returns>Triangle handle</returns>
    public void SetTriangleVertices (int tr, int v1, int v2, int v3)
    {
      Debug.Assert(HasTriangles, "This scene is composed of lines");
      Debug.Assert(geometry != null, "Invalid G[] size");
      tr *= 3;
      Debug.Assert(0 <= tr && tr + 2 < vertexPtr.Count,
                   "Invalid triangle handle");
      Debug.Assert(geometry.Count > v1 &&
                   geometry.Count > v2 &&
                   geometry.Count > v3, "Invalid vertex handle");

      vertexPtr[tr]     = v1;
      vertexPtr[tr + 1] = v2;
      vertexPtr[tr + 2] = v3;
    }

    /// <summary>
    /// Returns vertex coordinates of the given triangle.
    /// </summary>
    /// <param name="tr">Triangle handle</param>
    /// <param name="v1">Variable to receive the 1st vertex coordinates</param>
    /// <param name="v2">Variable to receive the 2nd vertex coordinates</param>
    /// <param name="v3">Variable to receive the 3rd vertex coordinates</param>
    public void GetTriangleVertices (int tr, out Vector3 v1, out Vector3 v2, out Vector3 v3)
    {
      Debug.Assert(HasTriangles, "This scene is composed of lines");
      Debug.Assert(geometry != null, "Invalid G[] size");
      tr *= 3;
      Debug.Assert(0 <= tr && tr + 2 < vertexPtr.Count,
                   "Invalid triangle handle");

      int h1 = vertexPtr[tr];
      int h2 = vertexPtr[tr + 1];
      int h3 = vertexPtr[tr + 2];
      v1 = (h1 < 0 || h1 >= geometry.Count) ? Vector3.Zero : geometry[h1];
      v2 = (h2 < 0 || h2 >= geometry.Count) ? Vector3.Zero : geometry[h2];
      v3 = (h3 < 0 || h3 >= geometry.Count) ? Vector3.Zero : geometry[h3];
    }

    /// <summary>
    /// Returns vertex coordinates of the given triangle.
    /// </summary>
    /// <param name="tr">Triangle handle</param>
    /// <param name="v1">Variable to receive the 1st vertex coordinates</param>
    /// <param name="v2">Variable to receive the 2nd vertex coordinates</param>
    /// <param name="v3">Variable to receive the 3rd vertex coordinates</param>
    public void GetTriangleVertices (int tr, out Vector4 v1, out Vector4 v2, out Vector4 v3)
    {
      Debug.Assert(HasTriangles, "This scene is composed of lines");
      Debug.Assert(geometry != null, "Invalid G[] size");
      tr *= 3;
      Debug.Assert(0 <= tr && tr + 2 < vertexPtr.Count,
                   "Invalid triangle handle");

      int h1 = vertexPtr[tr];
      int h2 = vertexPtr[tr + 1];
      int h3 = vertexPtr[tr + 2];
      v1 = new Vector4((h1 < 0 || h1 >= geometry.Count) ? Vector3.Zero : geometry[h1], 1.0f);
      v2 = new Vector4((h2 < 0 || h2 >= geometry.Count) ? Vector3.Zero : geometry[h2], 1.0f);
      v3 = new Vector4((h3 < 0 || h3 >= geometry.Count) ? Vector3.Zero : geometry[h3], 1.0f);
    }

    /// <summary>
    /// Updates bounding box coordinates (AABB) for the given triangle.
    /// </summary>
    /// <param name="tr">Triangle handle</param>
    /// <param name="min">Minimum-vertex of the AABB</param>
    /// <param name="max">Maximum-vertex of the AABB</param>
    public void TriangleBoundingBox (int tr, ref Vector3 min, ref Vector3 max)
    {
      Vector3 a, b, c;
      GetTriangleVertices(tr, out a, out b, out c);

      if (a.X < min.X) min.X = a.X;
      if (a.X > max.X) max.X = a.X;
      if (a.Y < min.Y) min.Y = a.Y;
      if (a.Y > max.Y) max.Y = a.Y;
      if (a.Z < min.Z) min.Z = a.Z;
      if (a.Z > max.Z) max.Z = a.Z;

      if (b.X < min.X) min.X = b.X;
      if (b.X > max.X) max.X = b.X;
      if (b.Y < min.Y) min.Y = b.Y;
      if (b.Y > max.Y) max.Y = b.Y;
      if (b.Z < min.Z) min.Z = b.Z;
      if (b.Z > max.Z) max.Z = b.Z;

      if (c.X < min.X) min.X = c.X;
      if (c.X > max.X) max.X = c.X;
      if (c.Y < min.Y) min.Y = c.Y;
      if (c.Y > max.Y) max.Y = c.Y;
      if (c.Z < min.Z) min.Z = c.Z;
      if (c.Z > max.Z) max.Z = c.Z;
    }

    /// <summary>
    /// Computes vertex array size (VBO) in bytes.
    /// </summary>
    /// <param name="vertices">Use vertex coordinates?</param>
    /// <param name="txt">Use texture coordinates?</param>
    /// <param name="col">Use vertex colors?</param>
    /// <param name="norm">Use normal vectors?</param>
    /// <returns>Buffer size in bytes</returns>
    public int VertexBufferSize (bool vertices, bool txt, bool col, bool norm)
    {
      Debug.Assert(geometry != null, "Invalid G[]");

      int size = 0;
      if (vertices)
        size += Vertices * 3 * sizeof(float);
      if (txt && TxtCoords > 0)
        size += Vertices * 2 * sizeof(float);
      if (col && Colors > 0)
        size += Vertices * 3 * sizeof(float);
      if (norm && Normals > 0)
        size += Vertices * 3 * sizeof(float);

      return size;
    }

    /// <summary>
    /// Fill vertex data into the provided memory array (VBO after MapBuffer).
    /// </summary>
    /// <param name="ptr">Memory pointer</param>
    /// <param name="vertices">Use vertex coordinates?</param>
    /// <param name="txt">Use texture coordinates?</param>
    /// <param name="col">Use vertex colors?</param>
    /// <param name="norm">Use normal vectors?</param>
    /// <returns>Stride (vertex size) in bytes</returns>
    public unsafe int FillVertexBuffer (float* ptr, bool vertices, bool txt, bool col, bool norm)
    {
      if (geometry == null)
        return 0;

      if (txt && TxtCoords < Vertices)
        txt = false;

      if (col && Colors < Vertices)
        col = false;

      if (norm && Normals < Vertices)
        norm = false;

      int i;
      for (i = 0; i < Vertices; i++)
      {
        // GL_T2F_C3F_N3F_V3F

        if (txt)
        {
          *ptr++ = txtCoords[i].X;
          *ptr++ = txtCoords[i].Y;
        }

        if (col)
        {
          *ptr++ = colors[i].X;
          *ptr++ = colors[i].Y;
          *ptr++ = colors[i].Z;
        }

        if (norm)
        {
          *ptr++ = normals[i].X;
          *ptr++ = normals[i].Y;
          *ptr++ = normals[i].Z;
        }

        if (vertices)
        {
          *ptr++ = geometry[i].X;
          *ptr++ = geometry[i].Y;
          *ptr++ = geometry[i].Z;
        }
      }

      return sizeof(float) * ((txt ? 2 : 0) + (col ? 3 : 0) + (norm ? 3 : 0) + (vertices ? 3 : 0));
    }

    /// <summary>
    /// Fills index data into provided memory array (VBO after MapBuffer).
    /// </summary>
    /// <param name="ptr">Memory pointer</param>
    public unsafe void FillIndexBuffer (uint* ptr)
    {
      if (vertexPtr == null)
        return;

      foreach (int i in vertexPtr)
        *ptr++ = (uint)i;
    }

    /// <summary>
    /// Computes center point and diameter of the scene.
    /// </summary>
    /// <param name="center">Center point</param>
    /// <returns>Diameter</returns>
    public float GetDiameter (out Vector3 center)
    {
      if (Vertices < 2)
      {
        center = (Vertices == 1) ? GetVertex(0) : Vector3.Zero;
        return 4.0f;
      }

      // center of the object = point to look at:
      double cx   = 0.0;
      double cy   = 0.0;
      double cz   = 0.0;
      float  minx = float.MaxValue;
      float  miny = float.MaxValue;
      float  minz = float.MaxValue;
      float  maxx = float.MinValue;
      float  maxy = float.MinValue;
      float  maxz = float.MinValue;
      int    i;

      for (i = 0; i < Vertices; i++)
      {
        Vector3 vi = GetVertex ( i );
        cx += vi.X;
        cy += vi.Y;
        cz += vi.Z;
        if (vi.X < minx) minx = vi.X;
        if (vi.Y < miny) miny = vi.Y;
        if (vi.Z < minz) minz = vi.Z;
        if (vi.X > maxx) maxx = vi.X;
        if (vi.Y > maxy) maxy = vi.Y;
        if (vi.Z > maxz) maxz = vi.Z;
      }

      center = new Vector3((float)(cx / Vertices),
                           (float)(cy / Vertices),
                           (float)(cz / Vertices));
      return (float)Math.Sqrt((maxx - minx) * (maxx - minx) +
                              (maxy - miny) * (maxy - miny) +
                              (maxz - minz) * (maxz - minz));
    }

    /// <summary>
    /// Generate random vertex colors.
    /// </summary>
    /// <param name="seed">Random seed</param>
    public void GenerateColors (int seed)
    {
      Random rnd = new Random(seed);

      if (colors == null)
        colors = new List<Vector3>(geometry.Count);

      while (Colors < Vertices)
        colors.Add(new Vector3((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble()));
    }

    /// <summary>
    /// Recompute all normals by averaging incident triangles' normals.
    /// Corner table must be valid (BuildCornerTable()).
    /// </summary>
    public void ComputeNormals ()
    {
      if (!HasTriangles)
        return;

      normals = new List<Vector3>(new Vector3[geometry.Count]);
      int[] n = new int[geometry.Count];
      int   ai, bi, ci;

      for (int i = 0; i < vertexPtr.Count; i += 3) // process one triangle
      {
        Vector3 A = geometry[ai = cVertex(i)];
        Vector3 c = geometry[bi = cVertex(i + 1)] - A;
        Vector3 b = geometry[ci = cVertex(i + 2)] - A;
        A = Vector3.Cross(c, b).Normalized();
        normals[ai] += A;
        n[ai]++;
        normals[bi] += A;
        n[bi]++;
        normals[ci] += A;
        n[ci]++;
      }

      // average the normals:
      for (int i = 0; i < geometry.Count; i++)
        if (n[i] > 0)
        {
          normals[i] /= n[i];
          normals[i].Normalize();
        }
    }

    #endregion

    #region Corner-table API

    /// <summary>
    /// [Re]builds the mesh topology (corner-table should be consistent after this call).
    /// </summary>
    public void BuildCornerTable ()
    {
      if (geometry  == null || geometry.Count  < 1 ||
          vertexPtr == null || vertexPtr.Count < 1)
      {
        Reset();
        return;
      }

      if (!HasTriangles)
      {
        oppositePtr = null;
        return;
      }

      int n = vertexPtr.Count;
      oppositePtr = new List<int>(n);
      for (int i = 0; i < n; i++)
        oppositePtr.Add(NULL);
      Dictionary<Edge, int> edges = new Dictionary<Edge, int> ();

      statEdges = statShared = 0;
      for (int i = 0; i < n; i++) // process one corner
      {
        int cmin = cVertex(cPrev(i));
        int cmax = cVertex(cNext(i));
        if (cmin < 0 || cmax < 0)
          continue;

        if (cmin > cmax)
        {
          int tmp = cmin;
          cmin = cmax;
          cmax = tmp;
        }

        Edge edge = new Edge(cmin, cmax);
        if (edges.ContainsKey(edge))
        {
          int other = edges[edge];
          Debug.Assert(oppositePtr[other] == NULL);
          oppositePtr[other] = i;
          oppositePtr[i] = other;
          edges.Remove(edge);
          statShared++;
        }
        else
        {
          edges.Add(edge, i);
          statEdges++;
        }
      }
    }

    /// <summary>
    /// Returns triangle handle of the given corner
    /// </summary>
    /// <param name="c">Corner handle</param>
    /// <returns>Triangle handle</returns>
    public static int cTriangle (int c)
    {
      return c / 3;
    }

    /// <summary>
    /// Returns handle of the 1st corner of the given triangle
    /// </summary>
    /// <param name="tr">Triangle handle</param>
    /// <returns>Corner handle</returns>
    public static int tCorner (int tr)
    {
      return tr * 3;
    }

    /// <summary>
    /// Returns the next corner inside the same triangle
    /// </summary>
    /// <param name="c">Corner handle</param>
    /// <returns>Handle of the next corner</returns>
    public static int cNext (int c)
    {
      return (c % 3 == 2) ? c - 2 : c + 1;
    }

    /// <summary>
    /// Returns the previous corner inside the same triangle
    /// </summary>
    /// <param name="c">Corner handle</param>
    /// <returns>Handle of the previous corner</returns>
    public static int cPrev (int c)
    {
      return (c % 3 == 0) ? c + 2 : c - 1;
    }

    /// <summary>
    /// Returns vertex handle of the given corner
    /// </summary>
    /// <param name="c">Corner handle</param>
    /// <returns>Associated vertex's handle</returns>
    public int cVertex (int c)
    {
      if (c < 0)
        return NULL;

      Debug.Assert(c < vertexPtr.Count, "Invalid corner handle");

      return vertexPtr[c];
    }

    /// <summary>
    /// Returns opposite corner to the given corner
    /// </summary>
    /// <param name="c">Corner handle</param>
    /// <returns>Handle of the opposite corner</returns>
    public int cOpposite (int c)
    {
      if (c < 0)
        return NULL;

      Debug.Assert(oppositePtr != null, "Invalid O[] array");
      Debug.Assert(c < oppositePtr.Count, "Invalid corner handle");

      return oppositePtr[c];
    }

    /// <summary>
    /// Returns the "right" corner from the given corner
    /// </summary>
    /// <param name="c">Corner handle</param>
    /// <returns>Corner handle of the "right" triangle</returns>
    public int cRight (int c)
    {
      return cOpposite(cNext(c));
    }

    /// <summary>
    /// Returns the "left" corner from the given corner
    /// </summary>
    /// <param name="c">Corner handle</param>
    /// <returns>Corner handle of the "left" triangle</returns>
    public int cLeft (int c)
    {
      return cOpposite(cPrev(c));
    }

    /// <summary>
    /// Checks consistency of the Corner-table.
    /// Based on code of Karel Hrkal, 2014.
    /// </summary>
    /// <param name="errors">Optional output stream for detailed error messages</param>
    /// <param name="thorough">Do thorough checks? (might be too strict and too memory intensive in many cases)</param>
    /// <returns>Number of errors/inconsistencies (0 if everything is Ok)</returns>
    public int CheckCornerTable (StreamWriter errors, bool thorough = false)
    {
      if (!HasTriangles)
        return 0;

      if (errors == null)
      {
        errors = new StreamWriter(Console.OpenStandardOutput());
        errors.AutoFlush = true;
        Console.SetOut(errors);
      }

      int errCount = 0;
      Action<string> log = ( s ) =>
      {
        errCount++;
        errors.WriteLine(s);
      };

      // 1. check trivial things such as in 1 triangle, all corners and all vertexes are disjont,
      //    cNext and cPevious, etc
      for (int i = 0; i < Corners; i++)
      {
        int r1 = cNext(i);
        int r2 = cNext(r1);
        int r3 = cNext(r2);

        if (i == r1 || r1 == r2 || r2 == i || i != r3)
          log("cNext not working properly for corner " + i);

        if (i != cPrev(r1) || r1 != cPrev(r2) || r2 != cPrev(i))
          log("cPrev not working properly for corner " + i);

        int v0 = cVertex(i);
        int v1 = cVertex(r1);
        int v2 = cVertex(r2);
        if (v0 == v1 || v1 == v2 || v2 == v0)
          log("Duplicate vertex in triangle with corner " + i);
      }

      // 2. check corner <-> opposite validity
      for (int i = 0; i < Corners; i++)
      {
        int other = cOpposite(i);
        if (other != NULL)
          if (cOpposite(other) != i)
            log("Corner " + i + " has an opposite " + other + " but not vice versa!");
          else
          {
            // while we are at it, check if 2 corners are linked as opposite
            // they also have same neighbour vertexes
            int a, b, c, d;
            a = cVertex(cNext(i));
            b = cVertex(cPrev(i));
            c = cVertex(cNext(other));
            d = cVertex(cPrev(other));
            bool correct     = (a == d) && (b == c);
            bool semiCorrect = (a == c) && (b == d);

            if (!correct)
              if (semiCorrect)
                // this is the case where the triangles indeed have same neighbours,
                // but one is facing the other way than the other (and that is at least suspicious)
                //  makes sense only for one-sided faces, disable this otherwise
                log("Opposite corners " + i + " and " + other +
                    " have the same neighbour, but are facing opposite directions!");
              else
                log("Opposite corners " + i + " and " + other + " does not have the same neighbours!");
          }
      }

      if (thorough)
      {
        // 3. now let's check that the cRight works properly
        //    we will use cPrev(cOpposite(cPrev()))
        //    also, this whole test assumes that neighbour triangles are facing the same way
        //    if triangles have both faces (front and back) visible, this test doesn't make much sence
        int[] temp = new int[Triangles + 1]; // corners will be saved here

        for (int i = 0; i < Corners; i++)
        {
          temp[0] = i;
          for (int j = 0; j < Triangles; j++)
          {
            int right = cOpposite(cPrev(temp[j]));
            if (right != NULL)
            {
              right = cPrev(right);
              temp[j + 1] = right;
            }

            if (right == i || right == NULL)
            {
              // test vertex equality
              for (int k = 0; k < j - 1; k++)
                if (cVertex(temp[k]) != cVertex(temp[k + 1]))
                  log("Traversing right corners from " + i + " resolved into differrent vertices at " + temp[k]);

              break;
            }

            for (int k = 0; k <= j; k++)
              if (temp[k] == right)
              {
                log("Starting in corner " + i + " we went right into corner " + right +
                      " twice before returning to " + i);
                j = Triangles;
                break;
              }
          }
        }

        // 4. finally check if 2 triangles share the 2 vertices, then they have properly set opposite corners
        //    moreover, at most 2 triangles can share an edge (2-manifold)
        int[,] arr = new int[Vertices, Vertices];
        // for edge i<j, at position [i,j] is which corner is first found opposite corner to this edge
        // at position [j,i] is how many edges in triangles are there

        for (int i = 0; i < Corners; i++)
        {
          int a = cVertex(cNext(i));
          int b = cVertex(cPrev(i));
          // ensure a < b
          if (a > b)
          {
            int tmp = a;
            a = b;
            b = tmp;
          }

          if (arr[b, a] == 0)
          {
            // for the 1st time at this edge
            arr[a, b] = i;
            arr[b, a] = 1;
          }
          else if (arr[b, a] == 1)
          {
            // for the 2nd time at this edge
            if (cOpposite(i) != arr[a, b])
              log("Corners " + i + " and " + arr[a, b] +
                  " have the same opposite side, but are not linked together!");
            arr[b, a] = 2;
          }
          else
            // broken 2-manifold
            log("Corner " + i + " has an opposide side thas was already used at least twice, 2-manifold is broken!");
        }
      }

      return errCount;
    }

    #endregion
  }
}
