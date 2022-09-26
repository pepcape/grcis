
using OpenTK;
using Rendering;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace EduardHopfer
{
  public sealed class ImplicitInnerNode : DefaultSceneNode
  {
    private readonly IImplicitOperation                       bop;
    private          ConcurrentDictionary<Ray, IList<double>> boundingBoxDistances;

    private List<ISceneNode> _lChildren = null;

    private List<ISceneNode> ListChildren
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

    public ImplicitInnerNode (IImplicitOperation op)
    {
      this.bop = op;

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

      this.boundingBoxDistances.TryRemove(ray, out _);
      bool foundIntersection = result.Count > 1;
      return foundIntersection ? result : null;
    }

    private Intersection CompoundIntersection (Ray ray,
                                               double initialPosition = 0D)
    {
      double positionOnRay = initialPosition;

      var initialResult = this.CompoundSDF(ray, positionOnRay);
      bool isInside = initialResult.Distance <= 0.0;

      while (this.IntersectionPossible(ray, positionOnRay))
      {
        var result = this.CompoundSDF(ray, positionOnRay);

        double distance = result.Distance;
        DistanceField chosen = result.Chosen;

        const double threshold = 0.0;
        bool crossedBoundary = (isInside && distance >= threshold) || (!isInside && distance <= -threshold);
        if (crossedBoundary)
        {
          var originLocal = Vector3d.TransformPosition(ray.Origin, chosen.FromParent);
          var dirLocal = Vector3d.TransformVector(ray.Dir, chosen.FromParent);
          var rayLocal = originLocal + positionOnRay * dirLocal;

          // chosen should always be a solid
          return new Intersection(chosen)
          {
            T = positionOnRay,
            Enter = !isInside,
            Front = !isInside,
            // TODO: is this right? it works but shouldn't the normal point the other way?
            NormalLocal = this.CalculateNormal(ray, positionOnRay),
            CoordLocal = rayLocal,
            SolidData = new ImplicitData
            {
              Distance = distance,
              Weights = result.Weights,
            },
          };
        }

        // Distance function is negative on the inside
        double stepSize = Math.Max(Math.Abs(distance), ImplicitCommon.MIN_STEP);
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

    private SDFResult CompoundSDF (Ray ray, double t, Vector3d? offset = null)
    {
      Vector3d _offset = offset ?? Vector3d.Zero;

      double distToScene = 0D;
      DistanceField chosen = null;
      var childWeights = Enumerable.Empty<WeightedSurface>();

      for (int i = 0; i < this.ListChildren.Count; i++)
      {
        bool first = i == 0;
        bool second = i == 1;
        var child = this.ListChildren[i];

        var tmpWeights = Enumerable.Empty<WeightedSurface>();
        DistanceField tmpChosen;

        double distToChild;

        var localOrigin = Vector3d.TransformPosition(ray.Origin, child.FromParent);
        var localDir = Vector3d.TransformVector(ray.Dir, child.FromParent);

        if (child is ImplicitInnerNode inner)
        {
          var localRay = new Ray()
          {
            Origin = localOrigin,
            Dir = localDir,
          };

          var innerResult = inner.CompoundSDF(localRay, t, offset);

          // Use the inner distance field instead of the inner node
          tmpChosen = innerResult.Chosen;
          distToChild = innerResult.Distance;
          tmpWeights = innerResult.Weights;
        }
        else
        {
          DistanceField leaf = child as DistanceField;
          Debug.Assert(leaf != null);

          distToChild = leaf.sdf(localOrigin + t * localDir + _offset);
          tmpChosen = leaf;

          double childWeight = this.bop.GetWeightRight(0.0, distToChild);
          tmpWeights = Enumerable.Repeat(new WeightedSurface()
          {
            Solid = leaf,
            Weight = childWeight,
          }, 1);
        }

        double resultLocal = distToScene; // not really needed but I like it better
        bool update = first;
        if (!update)
        {
          this.bop.Transform(ref resultLocal, ref distToChild);
          update = this.bop.ChooseRight(resultLocal, distToChild);
        }

        childWeights = childWeights.Concat(tmpWeights);
        // the first weight is 1.0 which sucks dick
        if (update)
        {
          distToScene = distToChild;

          chosen = tmpChosen;
        }
        else
        {
          distToScene = resultLocal;
        }
      }

      return new SDFResult()
      {
        Weights = childWeights,
        Distance = distToScene,
        Chosen = chosen,
      };
    }

    private Vector3d CalculateNormal (Ray ray, double positionOnRay)
    {
      const double h = 0.0001; // 0.0001
      Vector2d k = new Vector2d(1.0,-1.0);
      Vector3d xyy = new Vector3d(k.X, k.Y, k.Y);
      Vector3d yyx = new Vector3d(k.Y, k.Y, k.X);
      Vector3d yxy = new Vector3d(k.Y, k.X, k.Y);
      Vector3d xxx = new Vector3d(k.X, k.X, k.X);

      double sdf (Ray r, double t, Vector3d? offset = null)
      {
        var sdfResult = this.CompoundSDF(r, t, offset);
        return sdfResult.Distance;
      }

      return xyy * sdf(ray, positionOnRay, offset: xyy * h) +
             yyx * sdf(ray, positionOnRay, offset: yyx * h) +
             yxy * sdf(ray, positionOnRay, offset: yxy * h) +
             xxx * sdf(ray, positionOnRay, offset: xxx * h);
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
