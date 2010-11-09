using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Text;

namespace _008kdtree
{

  public struct Segment2D
  {
    public float x1, y1, x2, y2;

    public Segment2D ( float _x1, float _y1, float _x2, float _y2 )
    {
      x1 = _x1; y1 = _y1;
      x2 = _x2; y2 = _y2;
    }
  }

  class KDTree
  {
    #region protected data

    protected List<Segment2D> segs;

    #endregion

    #region constructor

    public KDTree ()
    {
      segs = null;
    }

    #endregion

    #region stat data

    protected long segmentCounter = 0L;

    protected long boxCounter = 0L;

    protected long heapCounter = 0L;

    #endregion

    #region KD-tree API

    public void BuildTree ( List<Segment2D> segments )
    {
      // !!!{{ TODO: add the KD-tree bulding code here

      segs = segments;

      // !!!}}
    }

    public int RayIntersect ( float ox, float oy, float dx, float dy, int k, int[] result )
    {
      // !!!{{ TODO: add the intersection code here

      if ( segs == null || k <= 0 ) return 0;

      SortedList<double, int> res = new SortedList<double, int>();
      for ( int i = 0; i < segs.Count; i++ )
      {
        Segment2D seg = segs[ i ];
        double dist = RaySegment2D( ox, oy, dx, dy, seg.x1, seg.y1, seg.x2, seg.y2 );
        segmentCounter++;
        if ( dist >= 0.0 )
          res.Add( dist, i );
      }

      int n = Math.Min( k, res.Count );
      for ( int i = 0; i < n; i++ )
        result[ i ] = res.ElementAt( i ).Value;

      return n;

      // !!!}}
    }

    public void GetStatistics ( out long segmentHits, out long boxHits, out long heapOperations )
    {
      // !!!{{ TODO: return requested statistics here

      segmentHits = segmentCounter;
      segmentCounter = 0L;

      boxHits = boxCounter;
      boxCounter = 0L;

      heapOperations = heapCounter;
      heapCounter = 0L;

      // !!!}}
    }

    #endregion

    #region support

    public static double RaySegment2D ( double ox, double oy, double dx, double dy,
                                        double ax, double ay, double bx, double by )
    {
      double nx  = ay - by;
      double ny  = bx - ax;
      double den = nx * dx + ny * dy;
      if ( den > -2.0 * Double.Epsilon &&
           den <  2.0 * Double.Epsilon ) return Double.NegativeInfinity;

      double t   = (nx * (ax - ox) + ny * (ay - oy)) / den;
      double resol;

      if ( Math.Abs( ny ) > Math.Abs( nx ) )
      {                                     // use X coordinate
        resol = ox + t * dx;
        if ( resol < Math.Min( ax, bx ) ||
             resol > Math.Max( ax, bx ) ) return Double.NegativeInfinity;
      }
      else
      {                                     // use Y coordinate
        resol = oy + t * dy;
        if ( resol < Math.Min( ay, by ) ||
             resol > Math.Max( ay, by ) ) return Double.NegativeInfinity;
      }

      return t;
    }

    #endregion
  }

}
