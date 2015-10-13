// Author: Josef Pelikan

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace _080warping
{
  public class Warping
  {
    /// <summary>
    /// Tri-mesh vertices. They are shared between adjacent triangles.
    /// </summary>
    protected List<PointF> vertices = new List<PointF>();

    /// <summary>
    /// Triangles as vertex triples.
    /// </summary>
    protected List<int> triangles = new List<int>();

    protected int columns;

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
      //Bitmap target = new Bitmap( sourceImage.Width, sourceImage.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb );
      Bitmap target = new Bitmap( sourceImage );

      return target;
    }

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

    public void MoveVertex ( int i, Point newLocation, float width, float height )
    {
      if ( i < 0 ||
           i >= vertices.Count ) return;

      // !!! TODO: moving border vertices is prohibited!

      vertices[ i ] = new PointF( newLocation.X / width, newLocation.Y / height );
    }

    public void KeyPressed ( Keys key )
    {
      if ( key == Keys.Back )
      {
        // !!! TODO: undo the last vertex-move ???
      }
    }
  }
}
