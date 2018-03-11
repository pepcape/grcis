using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace _080warping
{
  public class Warping
  {
    /// <summary>
    /// Form data initialization.
    /// </summary>
    public static void InitForm ( out string author, out int grid )
    {
      author = "Josef Pelikán";
      grid = 6;
    }

    /// <summary>
    /// Tri-mesh vertices. They are shared among adjacent triangles.
    /// </summary>
    protected List<PointF> vertices = new List<PointF>();

    /// <summary>
    /// Triangles as vertex triples.
    /// </summary>
    protected List<int> triangles = new List<int>();

    /// <summary>
    /// Current mesh topology: number of columns.
    /// </summary>
    protected int columns;

    /// <summary>
    /// Current mesh topology: number of rows.
    /// </summary>
    protected int rows;

    /// <summary>
    /// Initialize the triangle mesh (to be uniform..).
    /// </summary>
    public void GenerateTriangleMesh ( int clmns, int rws )
    {
      columns = clmns;
      rows = rws;
      int v = (columns + 1) * (rows + 1);
      vertices.Clear();
      vertices.Capacity = v;
      int t = columns * rows * 6;
      triangles.Clear();
      triangles.Capacity = t;

      // vertices:
      float dc = 1.0f / columns;
      float dr = 1.0f / rows;
      float c, r;
      int ci, ri;
      for ( r = 0.0f, ri = 0; ri++ <= rows; r += dr )
        for ( c = 0.0f, ci = 0; ci++ <= columns; c += dc )
          vertices.Add( new PointF( c, r ) );

      // triangles (index-array):
      int i = 0;
      for ( ri = 0; ri++ < rows; i++ )
        for ( ci = 0; ci++ < columns; i++ )
        {
          // UL triangle:
          triangles.Add( i );
          triangles.Add( i + columns + 1 );
          triangles.Add( i + 1 );
          // BR triangle:
          triangles.Add( i + 1 );
          triangles.Add( i + columns + 1 );
          triangles.Add( i + columns + 2 );
        }
    }

    /// <summary>
    /// Color to paint the grid.
    /// </summary>
    protected Color gridColor = Color.Yellow;

    /// <summary>
    /// Gets or sets the grid's color.
    /// </summary>
    public Color GridColor
    {
      get { return gridColor; }
      set
      {
        gridColor = value;
      }
    }

    /// <summary>
    /// Draws the current triangle mesh.
    /// </summary>
    /// <param name="g">Graphic context to draw to.</param>
    /// <param name="width">Canvas width.</param>
    /// <param name="height">Canvas height.</param>
    public void DrawGrid ( Graphics g, float width, float height )
    {
      Pen pen = new Pen( gridColor );

      for ( int i = 0; i < triangles.Count; i += 3 )
      {
        PointF A = vertices[ triangles[ i ] ];
        A.X *= width;
        A.Y *= height;
        PointF B = vertices[ triangles[ i + 1 ] ];
        B.X *= width;
        B.Y *= height;
        PointF C = vertices[ triangles[ i + 2 ] ];
        C.X *= width;
        C.Y *= height;
        g.DrawLine( pen, A, B );
        g.DrawLine( pen, B, C );
        g.DrawLine( pen, C, A );
      }
    }

    /// <summary>
    /// Warps the given source image from the given source mesh to this (current) mesh.
    /// </summary>
    /// <param name="sourceImage">Source raster image.</param>
    /// <param name="sourceMesh">Source mesh in [0,1]x[0,1] coordinate space.</param>
    /// <returns>Target raster image.</returns>
    public Bitmap Warp ( Bitmap sourceImage, Warping sourceMesh )
    {
      int wid = sourceImage.Width;
      int hei = sourceImage.Height;
      Bitmap target = new Bitmap( wid, hei, System.Drawing.Imaging.PixelFormat.Format24bppRgb );

      // !!!{{ TODO: put your image-transformation code here

      int x, y;
      bool flip = (DateTime.Now.Second & 1) > 0;
      for ( y = 0; y < hei; y++ )
        for ( x = 0; x < wid; x++ )
          target.SetPixel( x, y, sourceImage.GetPixel( x, flip ? hei - 1 - y : y ) );

      // !!!}}

      return target;
    }

    /// <summary>
    /// This function is called after user left-clicks on the image plane.
    /// We need to define the nearest mesh vertex.
    /// </summary>
    public int NearestVertex ( Point p, float width, float height )
    {
      float x = p.X / width;
      float y = p.Y / height;

      int nearest = -1;
      float minDD = float.MaxValue;
      float dx, dy, dd;
      for ( int i = 0; i < vertices.Count; i++ )
      {
        dx = vertices[ i ].X - x;
        dy = vertices[ i ].Y - y;
        if ( (dd = dx * dx + dy * dy) < minDD )
        {
          minDD = dd;
          nearest = i;
        }
      }

      return nearest;
    }

    /// <summary>
    /// Finishing vertex dragging mode. The given vertex should be placed to the new
    /// required position.
    /// </summary>
    public void MoveVertex ( int i, Point newLocation, float width, float height )
    {
      if ( i < 0 ||
           i >= vertices.Count ) return;

      // !!! TODO: moving vertex outside the image region is prohibited!

      // !!! TODO: moving border vertex out of the border is prohibited!

      vertices[ i ] = new PointF( newLocation.X / width, newLocation.Y / height );
    }

    /// <summary>
    /// Optional keystroke handling function.
    /// </summary>
    public bool KeyPressed ( Keys key )
    {
      if ( key == Keys.Back )
      {
        // !!! TODO: undo the last vertex-move ???
        return true;
      }

      return false;
    }
  }
}
