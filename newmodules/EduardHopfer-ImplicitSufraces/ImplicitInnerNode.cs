
using OpenTK;
using Rendering;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace EduardHopfer
{
  class Ray
  {
    public Vector3d Origin { get; set; }
    public Vector3d Dir    { get; set; }
  }

  struct NodeDistance
  {
    public ISceneNode Node     { get; set; }
    public double LastDistance { get; set; }
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
    bool Result (ref double leftSdfResult, ref double rightSdfResult);
  }

  public sealed class ImplicitIntersection : IImplicitOperation
  {
    public bool Result (ref double leftSdfResult, ref double rightSdfResult)
    {
      return rightSdfResult > leftSdfResult;
      //return Math.Max(leftSdfResult, rightSdfResult);
    }
  }

  // TODO: add blending
  public sealed class ImplicitUnion : IImplicitOperation
  {
    public bool Result (ref double leftSdfResult, ref double rightSdfResult)
    {
      return rightSdfResult < leftSdfResult;
      //return Math.Min(leftSdfResult, rightSdfResult);
    }
  }

  public sealed class ImplicitDifference : IImplicitOperation
  {
    public bool Result (ref double leftSdfResult, ref double rightSdfResult)
    {
      rightSdfResult *= -1; // invert the shape
      return rightSdfResult > leftSdfResult;
      //return Math.Max(leftSdfResult, rightSdfResult * -1);
    }
  }


  public sealed class ImplicitInnerNode : DefaultSceneNode
  {
    private static   double                                         correction = Intersection.RAY_EPSILON2;
    private readonly IImplicitOperation                             bop;
    //private          List<ISceneNode>                               relevantChildren;
    //private          List<double>                                   lastDistancesToBound;
    private ConcurrentDictionary<Ray, IList<double>> boundingBoxDistances;

    private List<ISceneNode> _lChildren = null;

    public List<ISceneNode> ListChildren
    {
      get
      {
        if (this._lChildren == null)
        {
          this._lChildren = this.children.ToList();
        }

        return this._lChildren;
      }
    }

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

      int concurrencyLevel = Environment.ProcessorCount * 2;
      boundingBoxDistances = new ConcurrentDictionary<Ray, IList<double>>(concurrencyLevel, concurrencyLevel);
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
      var ray = this.InitializeRayData(p0, p1);

      // TODO: is this transform needed?
      // var localPos = Vector3d.TransformPosition(p0, FromParent);
      // var localDir = Vector3d.TransformVector(p1, FromParent);

      var result = new LinkedList<Intersection>();

      while (true)
      {
        var intersection = this.CompoundIntersection(ray, positionOnRay);
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

    private Intersection CompoundIntersection (Ray ray,
                                               double initialPosition = 0D)
    {
      double positionOnRay = initialPosition;
      double initialDistance = this.CompoundSDF(ray, positionOnRay, out var chosen);
      bool isInside = initialDistance <= 0.0;

      while (this.IntersectionPossible(ray, positionOnRay))
      {
        double distance = this.CompoundSDF(ray, positionOnRay, out chosen);

        const double treshold = 0.0;
        bool crossedBoundary = (isInside && distance >= treshold) || (!isInside && distance <= -treshold);
        if (crossedBoundary)
        {
          var originLocal = Vector3d.TransformPosition(ray.Origin, chosen.FromParent);
          var dirLocal = Vector3d.TransformVector(ray.Dir, chosen.FromParent);
          var rayLocal = originLocal + positionOnRay * dirLocal;

          var df = chosen as DistanceField;
          // chosen should always be a solid
          return new Intersection(df)
          {
            T = positionOnRay,
            Enter = !isInside,
            Front = !isInside,
            // TODO: is this right? it works but shouldn't the normal point the other way?
            NormalLocal = this.CalculateNormal(ray, positionOnRay),
            CoordLocal = rayLocal,
            SolidData = new ImplicitData {distance = distance},
          };
        }

        double stepSize = Math.Max(Math.Abs(distance), Intersection.RAY_EPSILON); // Distance function is negative on the inside
        positionOnRay += stepSize;
      }

      return null;
    }

    private bool IntersectionPossible (Ray ray, double t)
    {
      var lastDistances = this.boundingBoxDistances.GetOrAdd(ray,
                                                          _ => Enumerable.Repeat(INFINITY_PLACEHOLDER, this.children.Count).ToList());
      for (int i = 0; i < this.ListChildren.Count; i++)
      {
        var child = this.ListChildren[i];
        var localOrigin = Vector3d.TransformPosition(ray.Origin, child.FromParent);
        var localDir = Vector3d.TransformVector(ray.Dir, child.FromParent);

        switch (child)
        {
          case DistanceField df:
          {
            double distance = df.boundingSdf(localOrigin + t * localDir);
            double lastDistance = lastDistances[i];

            if (distance < 0.0 || lastDistance < 0.0 || distance < lastDistance)
            {
              return true;
            }

            lastDistances[i] = distance;
            break;
          }
          case ImplicitInnerNode inner:
          {
            var rayLocal = new Ray() {Origin = localOrigin, Dir = localDir,};
            if (inner.IntersectionPossible(rayLocal, t))
            {
              return true;
            }

            break;
          }
          default:
            throw new Exception("bad node type");
        }
      }

      return false;
    }

    private double CompoundSDF (Ray ray, double t, out ISceneNode chosen, Vector3d? offset = null)
    {
      chosen = null;
      double result = 0D;
      bool first = true;
      Vector3d _offset = offset ?? Vector3d.Zero;

      foreach (var child in this.ListChildren)
      {
        ISceneNode tmpChosen;
        double tmpResult;

        var localOrigin = Vector3d.TransformPosition(ray.Origin, child.FromParent);
        var localDir = Vector3d.TransformVector(ray.Dir, child.FromParent);

        if (child is ImplicitInnerNode inner)
        {
          var localRay = new Ray() {Origin = localOrigin, Dir = localDir,};
          tmpResult = inner.CompoundSDF(localRay, t, out tmpChosen, offset);
        }
        else
        {
          tmpChosen = child;
          DistanceField leaf = child as DistanceField;
          tmpResult = leaf.sdf(localOrigin + t * localDir + _offset);
        }

        bool update = first || this.bop.Result(ref result, ref tmpResult);
        first = false;

        result = update ? tmpResult : result;
        chosen = update ? tmpChosen : chosen;
      }

      return result;
    }

    private Vector3d CalculateNormal (Ray ray, double positionOnRay)
    {
      const double h = 0.0001;
      Vector2d k = new Vector2d(1.0,-1.0);
      Vector3d xyy = new Vector3d(k.X, k.Y, k.Y);
      Vector3d yyx = new Vector3d(k.Y, k.Y, k.X);
      Vector3d yxy = new Vector3d(k.Y, k.X, k.Y);
      Vector3d xxx = new Vector3d(k.X, k.X, k.X);

      return xyy * this.CompoundSDF(ray, positionOnRay, out _, offset: xyy * h) +
             yyx * this.CompoundSDF(ray, positionOnRay, out _, offset: yyx * h) +
             yxy * this.CompoundSDF(ray, positionOnRay, out _, offset: yxy * h) +
             xxx * this.CompoundSDF(ray, positionOnRay, out _, offset: xxx * h);
    }

    private Ray InitializeRayData (Vector3d origin, Vector3d dir)
    {
      var rayKey = new Ray {Origin = origin, Dir = dir,};
      var rayData = Enumerable.Repeat(INFINITY_PLACEHOLDER, this.children.Count).ToList();

      // add or overwrite the data in the dictionary
      this.boundingBoxDistances.AddOrUpdate(rayKey, _ => rayData, (_, __) => rayData);
      return rayKey;
    }
  }
}
