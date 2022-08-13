
using OpenTK;
using Rendering;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EduardHopfer
{
  struct Ray
  {
    public Vector3d Origin { get; set; }
    public Vector3d Dir    { get; set; }
    public double   T      { get; set; }
  }

  public enum ImplicitOperation
  {
    Intersection,
    Union,
    Difference
  }

  interface IImplicitOperation
  {
    // false -> choose left; true -> choose right
    bool Result (double leftSdfResult, double rightSdfResult);
  }

  public sealed class ImplicitIntersection : IImplicitOperation
  {
    public bool Result (double leftSdfResult, double rightSdfResult)
    {
      return rightSdfResult > leftSdfResult;
      //return Math.Max(leftSdfResult, rightSdfResult);
    }
  }

  // TODO: add blending
  public sealed class ImplicitUnion : IImplicitOperation
  {
    public bool Result (double leftSdfResult, double rightSdfResult)
    {
      return rightSdfResult < leftSdfResult;
      //return Math.Min(leftSdfResult, rightSdfResult);
    }
  }

  public sealed class ImplicitDifference : IImplicitOperation
  {
    public bool Result (double leftSdfResult, double rightSdfResult)
    {
      return (rightSdfResult * -1) > leftSdfResult;
      //return Math.Max(leftSdfResult, rightSdfResult * -1);
    }
  }

  public sealed class ImplicitInnerNode : DefaultSceneNode
  {
    private readonly IImplicitOperation bop;
    private          List<ISceneNode>   relevantChildren;
    private          List<double>       lastDistancesToBound;

    public ImplicitInnerNode ( ImplicitOperation op)
    {
      switch (op)
      {
        case ImplicitOperation.Intersection:
          bop = new ImplicitIntersection();
          break;
        case ImplicitOperation.Difference:
          bop = new ImplicitDifference();
          break;
        case ImplicitOperation.Union:
          bop = new ImplicitUnion();
          break;
        default:
          bop = new ImplicitUnion();
          break;
      }
    }

    public override void InsertChild (ISceneNode ch, Matrix4d toParent)
    {
      bool isImplicit = ch is DistanceField || ch is ImplicitInnerNode;
      if (!isImplicit)
      {
        throw new ArgumentException("ImplicitInnerNode can only have other implicits as children!");
      }

      base.InsertChild(ch, toParent);
    }

    public override LinkedList<Intersection> Intersect (Vector3d p0, Vector3d p1)
    {
      double positionOnRay = 0D;
      // reset state
      this.relevantChildren = this.children.ToList();
      this.lastDistancesToBound = Enumerable.Repeat(INFINITY_PLACEHOLDER, this.relevantChildren.Count).ToList();

      var result = new LinkedList<Intersection>();

      while (true)
      {
        var intersection = this.CompoundIntersection(p0, p1, positionOnRay);
        if (intersection is null)
        {
          break;
        }

        positionOnRay = intersection.T;
        //isInside = intersection.Enter;
        result.AddLast(intersection);
      }

      bool foundIntersection = result.Count > 1;
      return foundIntersection ? result : null;
    }

    private Intersection CompoundIntersection (Vector3d origin,
                                               Vector3d dir,
                                               double initialPosition = 0D)
    {
      double positionOnRay = initialPosition;
      double initialDistance = this.CompoundSDF(origin, dir, positionOnRay, out var chosen);
      bool isInside = initialDistance <= 0.0;
      bool invalid = Math.Abs(initialDistance) <= Intersection.RAY_EPSILON;
      // if (invalid)
      // {
      //   isInside = !isInside;
      //   positionOnRay += Intersection.RAY_EPSILON;
      // }
      if (invalid)
      {
        double nextD = this.CompoundSDF(origin, dir, positionOnRay + Intersection.RAY_EPSILON, out chosen);
        bool nextInside = nextD <= 0;
        if (isInside != nextInside)
        {
          // probably just a numeric error, avoid self intersection
          positionOnRay += Intersection.RAY_EPSILON;
          isInside = !isInside;
        }
      }

      while (this.relevantChildren.Count != 0)
      {
        double distance = this.CompoundSDF(origin, dir, positionOnRay, out chosen);

        bool crossedBoundary = (isInside && distance >= 0.0) || (!isInside && distance <= 0.0);
        if (crossedBoundary)
        {
          var originLocal = Vector3d.TransformPosition(origin, chosen.FromParent);
          var dirLocal = Vector3d.TransformVector(dir, chosen.FromParent);
          var rayLocal = originLocal + positionOnRay * dirLocal;

          var df = chosen as DistanceField;
          // chosen should always be a solid
          return new Intersection(df)
          {
            T = positionOnRay,
            Enter = !isInside,
            Front = !isInside,
            NormalLocal = df.CalculateNormal(rayLocal),
            CoordLocal = rayLocal
          };
        }

        this.CompoundBoundingBoxSDF(origin, dir, positionOnRay);

        double stepSize = Math.Max(Math.Abs(distance), Intersection.RAY_EPSILON); // Distance function is negative on the inside
        positionOnRay += stepSize;
      }

      return null;
    }

    private void CompoundBoundingBoxSDF (Vector3d origin, Vector3d dir, double t)
    {
      this.relevantChildren = this.relevantChildren ?? this.children.ToList();
      this.lastDistancesToBound = this.lastDistancesToBound ??
                                  Enumerable.Repeat(INFINITY_PLACEHOLDER, this.relevantChildren.Count).ToList();
      var toRemove = new HashSet<int>();

      for (int i = 0; i < this.relevantChildren.Count; i++)
      {
        var child = this.relevantChildren[i];
        var localOrigin = Vector3d.TransformPosition(origin, child.FromParent);
        var localDir = Vector3d.TransformVector(dir, child.FromParent);

        if (child is DistanceField df)
        {
          double distance = df.boundingSdf(localOrigin + t * localDir);
          double lastDistance = this.lastDistancesToBound[i];

          if (distance > 0.0 && lastDistance > 0.0 && distance > lastDistance)
          {
            toRemove.Add(i);
          }
          else
          {
            this.lastDistancesToBound[i] = distance;
          }
        }
        else if (child is ImplicitInnerNode inner)
        {
          inner.CompoundBoundingBoxSDF(localOrigin, localDir, t);
          if (inner.relevantChildren.Count == 0)
          {
            toRemove.Add(i);
          }
        }
      }

      this.relevantChildren = this.relevantChildren
        .Where((sn, inx) => !toRemove.Contains(inx))
        .ToList();
      this.lastDistancesToBound = this.lastDistancesToBound
        .Where((d, inx) => !toRemove.Contains(inx))
        .ToList();
    }

    private double CompoundSDF (Vector3d origin, Vector3d dir, double t, out ISceneNode chosen)
    {
      this.relevantChildren = this.relevantChildren ?? this.children.ToList();

      chosen = null;
      double result = 0D;
      bool first = true;

      foreach (var child in this.relevantChildren)
      {
        ISceneNode tmpChosen;
        double tmpResult;

        var localOrigin = Vector3d.TransformPosition(origin, child.FromParent);
        var localDir = Vector3d.TransformVector(dir, child.FromParent);

        if (child is ImplicitInnerNode inner)
        {
          tmpResult = inner.CompoundSDF(localOrigin, localDir, t, out tmpChosen);
        }
        else
        {
          tmpChosen = child;
          DistanceField leaf = child as DistanceField;
          tmpResult = leaf.sdf(localOrigin + t * localDir);
        }

        bool update = first || this.bop.Result(result, tmpResult);
        first = false;

        result = update ? tmpResult : result;
        chosen = update ? tmpChosen : chosen;
      }

      return result;
    }
  }
}
