using OpenTK;
using Rendering;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace EduardHopfer
{
  public sealed class DistanceField : DefaultSceneNode, ISolid
  {
    private const   double                                MIN_STEP = Intersection.RAY_EPSILON;
    public readonly ImplicitCommon.SignedDistanceFunction sdf;
    public readonly ImplicitCommon.SignedDistanceFunction boundingSdf = ImplicitCommon.Box;

    public DistanceField (ImplicitCommon.SignedDistanceFunction sdf)
    {
      this.sdf = sdf;
    }

    private Intersection ImplicitIntersection (Vector3d origin,
                                               Vector3d dir,
                                               double initialPosition = 0,
                                               bool? isInsideOr = null)
    {
      double positionOnRay = initialPosition;
      var ray = origin + positionOnRay * dir;

      bool isInside = isInsideOr ?? this.sdf(ray) <= 0.0;
      bool isInsideBoundingBox = boundingSdf(ray) <= 0.0;

      double distToBoundAbs = Math.Abs(boundingSdf(ray));

      while (true)
      {
        double distance = this.sdf(ray);

        bool crossedBoundary = (isInside && distance >= 0.0) || (!isInside && distance <= 0.0);
        if (crossedBoundary)
        {
          // Intersection
          return new Intersection(this)
          {
            T = positionOnRay,
            Enter = !isInside,
            Front = !isInside,
            NormalLocal = this.CalculateNormal(ray),
            CoordLocal = ray,
            SolidData = new ImplicitData
            {
              Distance = distance,
              Weights = Enumerable.Repeat(new WeightedSurface()
              {
                Solid = this,
                Weight = 1.0,
              }, 1),
            },
          };
        }

        double lastDistToBound = distToBoundAbs;
        double distToBound = boundingSdf(ray);
        distToBoundAbs = Math.Abs(distToBound);

        bool lastInsideBoundingBox = isInsideBoundingBox;
        isInsideBoundingBox = distToBound <= 0.0;


        //bool leavingBoundingSphere = lastInsideBoundingBox && !isInsideBoundingBox;
        bool goingAwayFromObject = !isInsideBoundingBox && !lastInsideBoundingBox && distToBoundAbs > lastDistToBound;
        if (/*leavingBoundingSphere ||*/ goingAwayFromObject)
        {
          // No further intersections will happen...
          return null;
        }

        double stepSize = Math.Max(Math.Abs(distance), MIN_STEP); // Distance function is negative on the inside
        positionOnRay += stepSize;
        ray = origin + positionOnRay * dir;
      }
    }

    public override LinkedList<Intersection> Intersect (Vector3d p0, Vector3d p1)
    {
      double positionOnRay = 0D;

      // We don't know where the ray origin is yet
      // this won't work with very thin objects, but then again
      // this maybe isn't a big optimization at all.
      // We only save 1 evaluation of the SDF
      bool? isInside = null;
      var result = new LinkedList<Intersection>();

      while (true)
      {
        var intersection = this.ImplicitIntersection(p0, p1, positionOnRay, isInside);
        if (intersection is null)
        {
          break;
        }

        positionOnRay = intersection.T;
        //isInside = intersection.Enter;
        result.AddLast(intersection);
      }

      // TODO: What is going on?? why does it suck when there is only a single intersection?
      bool foundIntersection = result.Count > 1;
      return foundIntersection ? result : null;
    }

    // This implementation only evaluates the SDF 4 times,
    // as opposed to 6 needed for classic gradient approximation.
    // source: https://iquilezles.org/articles/normalsSDF/
    public Vector3d CalculateNormal (Vector3d p)
    {
      const double h = 0.0001;
      Vector2d k = new Vector2d(1.0,-1.0);
      Vector3d xyy = new Vector3d(k.X, k.Y, k.Y);
      Vector3d yyx = new Vector3d(k.Y, k.Y, k.X);
      Vector3d yxy = new Vector3d(k.Y, k.X, k.Y);
      Vector3d xxx = new Vector3d(k.X, k.X, k.X);

      return (xyy * this.sdf(p + xyy * h) +
        yyx * this.sdf(p + yyx * h) +
        yxy * this.sdf(p + yxy * h) +
        xxx * this.sdf(p + xxx * h)).Normalized();
    }

    // TODO: does this transform automatically?
    public void GetBoundingBox (out Vector3d corner1, out Vector3d corner2)
    {
      corner1 = new Vector3d(-1, -1, -1);
      corner2 = new Vector3d( 1,  1,  1);
    }
  }
}
