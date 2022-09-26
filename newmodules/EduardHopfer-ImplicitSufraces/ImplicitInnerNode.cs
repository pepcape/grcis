using OpenTK;
using Rendering;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EduardHopfer
{
  public sealed class ImplicitInnerNode : DefaultSceneNode, ITimeDependent
  {
    private readonly IImplicitOperation bop;
    private          IList<double>      boundingBoxDistances;
    private          List<ISceneNode>   _lChildren = null;

    // For convenience
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

      boundingBoxDistances = new List<double>();
    }

    public override void InsertChild (ISceneNode ch, Matrix4d toParent)
    {
      bool isImplicit = ch is DistanceField || ch is ImplicitInnerNode;
      // For AnimatedCSGInnerNode mainly
      bool hasSingleImplicitChild = ch.Children.Count == 1 &&
                                    (ch.Children.First() is DistanceField || ch.Children.First() is ImplicitInnerNode);

      if (!isImplicit && !hasSingleImplicitChild)
      {
        throw new ArgumentException("ImplicitInnerNode can only have other implicits as children!");
      }

      base.InsertChild(ch, toParent);
    }

    public override LinkedList<Intersection> Intersect (Vector3d p0, Vector3d p1)
    {
      double positionOnRay = 0D;
      this.boundingBoxDistances = Enumerable.Repeat(INFINITY_PLACEHOLDER, this.children.Count).ToList();
      var ray = new Ray {Origin = p0, Dir = p1,};

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

    // Sphere tracing implementation
    // Intersection is found when the sign of the scene SDF changes
    // TODO: go back to traditional implementation with epsilon based intersections
    //       That way we can get glow and soft shadows for free
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
      //var lastDistances = this.boundingBoxDistances;
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
            double lastDistance = this.boundingBoxDistances[i];

            if (distance < 0.0 || lastDistance < 0.0 || distance < lastDistance)
            {
              return true;
            }

            this.boundingBoxDistances[i] = distance;
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
            // TODO: move into a function and allow ImplicitInnerNode as well
            var next = child.Children.Single();
            var nextLeaf = next as DistanceField;
            double d = nextLeaf.boundingSdf(localOrigin + t * localDir);
            double ld = this.boundingBoxDistances[i];

            if (d < 0.0 || ld < 0.0 || d < ld)
            {
              return true;
            }

            this.boundingBoxDistances[i] = d;
            break;
        }
      }

      return false;
    }

    // Compute the SDF of the whole scene by applying the CSG operation
    // on the child SDFs
    private SDFResult CompoundSDF (Ray ray, double t, Vector3d? offset = null)
    {
      Vector3d _offset = offset ?? Vector3d.Zero;

      double distToScene = 0D;
      DistanceField chosen = null;
      var childWeights = Enumerable.Empty<WeightedSurface>();

      for (int i = 0; i < this.ListChildren.Count; i++)
      {
        bool first = i == 0;
        var child = this.ListChildren[i];

        var tmpWeights = Enumerable.Empty<WeightedSurface>();
        DistanceField tmpChosen;

        double distToChild;

        var localOrigin = Vector3d.TransformPosition(ray.Origin, child.FromParent);
        var localDir = Vector3d.TransformVector(ray.Dir, child.FromParent);

        switch (child)
        {
          case ImplicitInnerNode inner:
            var localRay = new Ray()
            {
              Origin = localOrigin,
              Dir = localDir,
            };

            var innerCopy = inner.Clone() as ImplicitInnerNode;
            var innerResult = innerCopy.CompoundSDF(localRay, t, offset);

            // Use the inner distance field instead of the inner node
            tmpChosen = innerResult.Chosen;
            distToChild = innerResult.Distance;
            tmpWeights = innerResult.Weights;
            break;
          case DistanceField leaf:
            distToChild = leaf.sdf(localOrigin + t * localDir + _offset);
            tmpChosen = leaf;

            double childWeight = this.bop.GetWeightRight(0.0, distToChild);
            tmpWeights = Enumerable.Repeat(new WeightedSurface()
            {
              Solid = leaf,
              Weight = childWeight,
            }, 1);
            break;
          default:
            var nextChild = child.Children.Single();
            var nextLeaf = nextChild as DistanceField;
            nextLeaf.FromParent = child.FromParent;
            nextLeaf.ToParent = child.ToParent;

            distToChild = nextLeaf.sdf(localOrigin + t * localDir + _offset);
            tmpChosen = nextLeaf;

            double cWeight = this.bop.GetWeightRight(0.0, distToChild);
            tmpWeights = Enumerable.Repeat(new WeightedSurface()
            {
              Solid = nextLeaf,
              Weight = cWeight,
            }, 1);
            break;
        }

        double resultLocal = distToScene; // not really needed but I like it better
        bool update = first;
        if (!update)
        {
          this.bop.Transform(ref resultLocal, ref distToChild);
          update = this.bop.ChooseRight(resultLocal, distToChild);
        }

        childWeights = childWeights.Concat(tmpWeights);

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

    // Normal vector approximation using the CompoundSDF
    // see DistanceField.CalculateNormal
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

    public object Clone ()
    {
      ImplicitInnerNode clone = new ImplicitInnerNode(this.bop);
      ShareCloneAttributes(clone);
      ShareCloneChildren(clone);
      clone.Time = time;
      return clone;
    }

    public double Start { get; set; }
    public double End   { get; set; }

    private double time { get; set; }

    /// <summary>
    /// Propagates time to descendants.
    /// </summary>
    private void setTime (double newTime)
    {
      time = newTime;

      // set time in relevant children:
      foreach (ISceneNode child in children)
        if (child is ITimeDependent cha)
          cha.Time = newTime;
    }

    /// <summary>
    /// Current time in seconds.
    /// </summary>
    public double Time
    {
      get => time;
      set => setTime(value);
    }
  }
}
