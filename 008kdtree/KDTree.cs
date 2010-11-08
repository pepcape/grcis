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
  }

  class KDTree
  {
    #region stat data

    protected static long segmentCounter = 0L;

    protected static long boxCounter = 0L;

    protected static long heapCounter = 0L;

    #endregion

    #region KD-tree API

    public static void BuildTree ( List<Segment2D> segments )
    {
      // !!!{{ TODO: add the KD-tree bulding code here


      // !!!}}
    }

    public static int[] RayIntersect ( float ox, float oy, float dx, float dy, int k )
    {
      // !!!{{ TODO: add the intersection code here

      return null;

      // !!!}}
    }

    public static void GetStatistics ( ref long segmentHits, ref long boxHits, ref long heapOperations )
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
  }

}
