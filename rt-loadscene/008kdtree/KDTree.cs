// Author: Josef Pelikan

using System;
using System.Collections.Generic;

namespace _008kdtree
{
  /// <summary>
  /// Simple structure holding 2D line segment (end-point coordinates as floats).
  /// </summary>
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

    /// <summary>
    /// Copy of initial segment array (you might not need it).
    /// </summary>
    protected List<Segment2D> segs;

    #endregion

    #region constructor

    public KDTree ()
    {
      segs = null;
    }

    #endregion

    #region stat data

    /// <summary>
    /// Counter for ray-segment intersections.
    /// </summary>
    protected long segmentCounter = 0L;

    /// <summary>
    /// Counter for ray-rectangle intersections.
    /// </summary>
    protected long boxCounter = 0L;

    /// <summary>
    /// Counter for (log N-complex) heap operations.
    /// </summary>
    protected long heapCounter = 0L;

    #endregion

    #region KD-tree API

    /// <summary>
    /// Initialize the data structure (KD-tree) here.
    /// All old data should be destroyed (forgotten).
    /// </summary>
    /// <param name="segments">Array (cloud) of 2D line segments.</param>
    public void BuildTree ( List<Segment2D> segments )
    {
      // !!!{{ TODO: add the KD-tree bulding code here

      segs = segments;

      // !!!}}
    }

    /// <summary>
    /// Find at most k intersections with the given ray.
    /// Results are returned as sorted array if ID-s (indices into the original segment array).
    /// </summary>
    /// <param name="ox">Ray origin - x-coordinate.</param>
    /// <param name="oy">Ray origin - y-coordinate.</param>
    /// <param name="dx">Ray direction - x-coordinate.</param>
    /// <param name="dy">Ray direction - y-coordinate.</param>
    /// <param name="k">Maximum result size (only the k closest intersections are requested).</param>
    /// <param name="result">Actual number of segments returned (should be less or equal to k).</param>
    /// <returns></returns>
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
        result[ i ] = res.Values[ i ];

      return n;

      // !!!}}
    }

    /// <summary>
    /// Request for current data-structure statistics
    /// </summary>
    /// <param name="segmentHits">Number of tests (ray vs. segment) performed.</param>
    /// <param name="boxHits">Number of tests (ray vs. rectangle) performed.</param>
    /// <param name="heapOperations">Number of heap-operations (of log N - complexity) performed.</param>
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

    /// <summary>
    /// Ray vs. line segment intersection in 2D.
    /// </summary>
    /// <param name="ox">Ray origin - x-coordinate.</param>
    /// <param name="oy">Ray origin - y-coordinate.</param>
    /// <param name="dx">Ray direction - x-coordinate.</param>
    /// <param name="dy">Ray direction - y-coordinate.</param>
    /// <param name="ax">A endpoint - x-coordinate.</param>
    /// <param name="ay">A endpoint - y-coordinate.</param>
    /// <param name="bx">B endpoint - x-coordinate.</param>
    /// <param name="by">B endpoint - y-coordinate.</param>
    /// <returns>Parameter coordinate of the intersection or Double.NegativeInfinity if none exists.</returns>
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
