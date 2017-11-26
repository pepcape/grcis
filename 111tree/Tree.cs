using System;
using System.Collections.Generic;
using OpenTK;

namespace _111tree
{
  class Tree
  {
    /// <summary>
    /// Data initialization.
    /// </summary>
    public static void InitParams ( out int setSize, out int querySize, out int K, out int seed, out string name )
    {
      setSize   = 100000;
      querySize =    100;
      K         =     24;
      seed      =     12;

      name = "Josef Pelikán";
    }

    #region protected data

    /// <summary>
    /// Copy of initial point array (you might not need it).
    /// </summary>
    protected List<Vector2> pts;

    #endregion

    #region constructor

    public Tree ()
    {
      pts = null;
    }

    #endregion

    #region stat data

    /// <summary>
    /// Counter for center-point distance.
    /// </summary>
    protected long pointCounter = 0L;

    /// <summary>
    /// Counter for center-rectangle distance.
    /// </summary>
    protected long boxCounter = 0L;

    /// <summary>
    /// Counter for (log N-complex) heap operations.
    /// </summary>
    protected long heapCounter = 0L;

    #endregion

    #region tree API

    /// <summary>
    /// Initialize the data structure (KD-tree, R-tree) here.
    /// All old data should be destroyed (forgotten).
    /// </summary>
    /// <param name="points">Array (cloud) of 2D points.</param>
    public void BuildTree ( List<Vector2> points )
    {
      // !!!{{ TODO: add the tree bulding code here

      pts = points;

      // !!!}}
    }

    /// <summary>
    /// Find at most k nearest points.
    /// Results are returned as sorted array if ID-s (indices into the original point array).
    /// </summary>
    /// <param name="cx">Search center - x-coordinate.</param>
    /// <param name="cy">Search center - y-coordinate.</param>
    /// <param name="k">Maximum result size (only the k closest points are requested).</param>
    /// <param name="result">Pre-allocated array to hold result-point IDs.</param>
    /// <returns>Actual number of points returned (should be less or equal to k).</returns>
    public int FindNearest ( double cx, double cy, int k, int[] result )
    {
      // !!!{{ TODO: add the nearest lookup code here

      if ( pts == null ||
           k <= 0 ) return 0;

      SortedList<double, int> res = new SortedList<double, int>();
      for ( int i = 0; i < pts.Count; i++ )
      {
        Vector2 pt = pts[ i ];
        double dist = (pt.X - cx) * (pt.X - cx) + (pt.Y - cy) * (pt.Y - cy);
        pointCounter++;
        res.Add( dist, i );
        if ( res.Count > k )
          res.RemoveAt( k );
      }

      int n = Math.Min( k, res.Count );
      for ( int i = 0; i < n; i++ )
        result[ i ] = res.Values[ i ];

      return n;

      // !!!}}
    }

    /// <summary>
    /// Request for current data-structure statistics.
    /// </summary>
    /// <param name="pointHits">Number of point-tests (point-point distance) performed.</param>
    /// <param name="boxHits">Number of box-tests (point-box distance) performed.</param>
    /// <param name="heapOperations">Number of heap-operations (of log N - complexity) performed.</param>
    public void GetStatistics ( out long pointHits, out long boxHits, out long heapOperations )
    {
      // !!!{{ TODO: return requested statistics here

      pointHits = pointCounter;
      pointCounter = 0L;

      boxHits = boxCounter;
      boxCounter = 0L;

      heapOperations = heapCounter;
      heapCounter = 0L;

      // !!!}}
    }

    #endregion

    #region support

    /// <summary>
    /// Point-box distance in 2D.
    /// </summary>
    /// <param name="cx">Point - x-coordinate.</param>
    /// <param name="cy">Point - y-coordinate.</param>
    /// <param name="minx">Box ul corner - x-coordinate.</param>
    /// <param name="miny">Box ul corner - y-coordinate.</param>
    /// <param name="maxx">Box lr corner - x-coordinate.</param>
    /// <param name="maxy">Box lr corner - y-coordinate.</param>
    /// <returns>Squared point-box distance (0.0 inside the box).</returns>
    public static double PointBoxDistanceSquared ( double cx, double cy,
                                                   double minx, double miny, double maxx, double maxy )
    {
      double sum2 = 0.0;

      if ( cx < minx )
        sum2 += (minx - cx) * (minx - cx);
      else
      if ( cx > maxx )
        sum2 += (cx - maxx) * (cx - maxx);

      if ( cy < miny )
        sum2 += (miny - cy) * (miny - cy);
      else
      if ( cy > maxy )
        sum2 += (cy - maxy) * (cy - maxy);

      return sum2;
    }

    #endregion
  }
}
