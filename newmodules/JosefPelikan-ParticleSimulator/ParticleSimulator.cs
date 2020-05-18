using System;
using System.Collections.Generic;
using System.Diagnostics;
using MathSupport;
using OpenTK;
using Rendering;
using Utilities;

namespace JosefPelikan
{
  /// <summary>
  /// Algorithm able to simulate set of point particles.
  /// </summary>
  [Serializable]
  public class ParticleSimulator
  {
    /// <summary>
    /// Associated animator.
    /// </summary>
    protected ITimeDependentProperty animator;

    /// <summary>
    /// Starting time of the animation (double.MinValue for default =
    /// time get from an associated animator).
    /// </summary>
    protected double startTime;

    /// <summary>
    /// Animation finish time (double.MaxValue for default =
    /// time get from an associated animator).
    /// </summary>
    protected double endTime;

    /// <summary>
    /// Time granularity in seconds.
    /// </summary>
    protected double deltaT;

    /// <summary>
    /// List of Properties - result of simulation.
    /// </summary>
    protected Dictionary<string, object> properties;

    public ParticleSimulator (
      in ITimeDependentProperty anim = null,
      in double dT = 0.2,
      in double minTime = double.MinValue,
      in double maxTime = double.MaxValue)
    {
      SetAnimator(anim);
      SetTimes(dT, minTime, maxTime);
    }

    public void SetAnimator (
      in ITimeDependentProperty anim = null)
    {
      if (anim != null)
        animator = anim;
      else
        animator = MT.scene?.Animator as ITimeDependentProperty;
    }

    public void SetTimes (
      in double dT = 0.2,
      in double minTime = double.MinValue,
      in double maxTime = double.MaxValue)
    {
      startTime = minTime;
      endTime   = maxTime;
      deltaT    = dT;
      if (animator != null)
      {
        startTime = Math.Max(startTime, animator.Start);
        endTime   = Math.Min(endTime,   animator.End);
      }
    }

    public const string PARTICLES  = "N";         // int
    public const string CENTER     = "Center";    // Vector3
    public const string RADIUS     = "Radius";    // Vector3, float
    public const string POS_NAME   = "PosName";   // string
    public const string COLOR_NAME = "ColorName"; // string
    public const string MIN_SIZE   = "MinSize";   // float
    public const string MAX_SIZE   = "MaxSize";   // float
    public const string MIN_COLOR  = "MinColor";  // float
    public const string MAX_COLOR  = "MaxColor";  // float

    public virtual void Simulate (
      in bool cyclic,
      in Dictionary<string, object> parameters = null,
      RandomJames rnd = null,
      in long seed = -1L)
    {
      // Random generator, optional seed for determinism.
      if (seed >= 0L)
        if (rnd == null)
          rnd = new RandomJames(seed);
        else
          rnd.Reset(seed);
      else
        if (rnd != null)
          rnd = MT.rnd;

      // Starting with a new set of properties.
      properties = new Dictionary<string, object>();

      // Retrieve some simulation parameters.

      // Simulation properties.
      // Number of particles.
      int n = 100;
      Util.TryParse(parameters, PARTICLES, ref n);
      if (n < 1)
        n = 1;

      // Center of field.
      Vector3 center = Vector3.Zero;
      Geometry.TryParse(parameters, CENTER, ref center);

      // Size/radius of the field.
      Vector3 size = new Vector3( 1.0f, 1.0f, 1.0f );
      if (!Geometry.TryParse(parameters, RADIUS, ref size))
      {
        // Trying float size (radius).
        float radius = 1.0f;
        Util.TryParse(parameters, RADIUS, ref radius);
        if (radius < 0.0f)
          radius = 0.0f;
        size = new Vector3(radius, radius, radius);
      }

      // Particle size.
      float minSize = 0.2f;
      Util.TryParse(parameters, MIN_SIZE, ref minSize);
      float maxSize = 1.0f;
      Util.TryParse(parameters, MAX_SIZE, ref maxSize);

      // Particle color.
      float minColor = 0.2f;
      Util.TryParse(parameters, MIN_COLOR, ref minColor);
      float maxColor = 1.0f;
      Util.TryParse(parameters, MAX_COLOR, ref maxColor);

      // Simulation.
      int knots = (int)((endTime - startTime) / deltaT);
      if (!cyclic) knots += 2;
      List<Vector4[]> posList = new List<Vector4[]>(knots);
      List<Vector3[]> colList = new List<Vector3[]>(knots);

      for (int i = 0; i < knots; i++)
        if (!cyclic && i == 1)
        {
          posList.Add(posList[0]);
          colList.Add(colList[0]);
        }
        else
        if (!cyclic && i == knots - 1)
        {
          posList.Add(posList[knots - 2]);
          colList.Add(colList[knots - 2]);
        }
        else
        {
          // Create regular random rows (positions, colors).
          Vector4[] pos = new Vector4[n];
          Vector3[] col = new Vector3[n];
          for (int j = 0; j < n; j++)
          {
            pos[j] = new Vector4(
              (float)rnd.Normal(center.X, size.X),
              (float)rnd.Normal(center.Y, size.Y),
              (float)rnd.Normal(center.Z, size.Z),
              rnd.RandomFloat(minSize, maxSize));
            col[j] = new Vector3(
              rnd.RandomFloat(minColor, maxColor),
              rnd.RandomFloat(minColor, maxColor),
              rnd.RandomFloat(minColor, maxColor));
          }
          posList.Add(pos);
          colList.Add(col);
        }

      // Property names.
      string posName = "posName";
      string colName = "colName";
      Util.TryParse(parameters, POS_NAME,   ref posName);
      Util.TryParse(parameters, COLOR_NAME, ref colName);

      // Create the Properties.
      PropertyAnimator.Property<Vector4[]> ppos = new PropertyAnimator.Property<Vector4[]>()
      {
        name    = posName,
        tStart  = startTime,
        tEnd    = endTime,
        tPeriod = endTime - startTime,
        data    = posList,
        cyclic  = cyclic,
        style   = cyclic
                    ? PropertyAnimator.InterpolationStyle.Cyclic
                    : PropertyAnimator.InterpolationStyle.Once
      };
      PropertyAnimator.Property<Vector3[]> pcol = new PropertyAnimator.Property<Vector3[]>()
      {
        name    = colName,
        tStart  = startTime,
        tEnd    = endTime,
        tPeriod = endTime - startTime,
        data    = colList,
        cyclic  = cyclic,
        style   = cyclic
                    ? PropertyAnimator.InterpolationStyle.Cyclic
                    : PropertyAnimator.InterpolationStyle.Once
      };

      properties[posName] = ppos;
      properties[colName] = pcol;
    }

    public virtual PropertyAnimator.Property<T> GetProperty<T> (
      in string name)
    {
      if (!properties.TryGetValue(name, out object pr) ||
          !(pr is PropertyAnimator.Property<T> prop))
        return null;

      return prop;
    }
  }
}
