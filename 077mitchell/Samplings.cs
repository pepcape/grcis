using System;
using System.Collections.Generic;
using System.Diagnostics;
using OpenTK;
using Utilities;

namespace _077mitchell
{
  /// <summary>
  /// Random (independent) sampling.
  /// </summary>
  public class RandomSampling : DefaultSampling, ISampling
  {
    /// <summary>
    /// Generate 'count' samples in the [0,1]x[0,1] domain.
    /// </summary>
    /// <param name="set">Output object.</param>
    /// <param name="count">Desired number of samples</param>
    /// <param name="param">Optional textual parameter set.</param>
    /// <returns>Actual number of generated samples.</returns>
    public int GenerateSampleSet ( SampleSet set, int count, string param =null )
    {
      Debug.Assert( set != null );
      if ( count < 0 ) count = 0;

      set.samples.Clear();
      for ( int i = 0; i++ < count; )
      {
        double x = rnd.UniformNumber();
        double y = rnd.UniformNumber();
        set.samples.Add( new Vector2d( x, y ) );

        // User break check:
        if ( (i & 0xffff) == 0 && userBreak ) break;
      }

      return count;
    }

    /// <summary>
    /// Sampling class identifier.
    /// </summary>
    public string Name
    {
      get { return "Random"; }
    }
  }

  /// <summary>
  /// Random importance sampling.
  /// </summary>
  public class RandomDensitySampling : DefaultSampling, ISampling
  {
    /// <summary>
    /// Generate 'count' samples in the [0,1]x[0,1] domain.
    /// </summary>
    /// <param name="set">Output object.</param>
    /// <param name="count">Desired number of samples</param>
    /// <param name="param">Optional textual parameter set.</param>
    /// <returns>Actual number of generated samples.</returns>
    public int GenerateSampleSet ( SampleSet set, int count, string param =null )
    {
      Debug.Assert( set != null );
      if ( count < 0 ) count = 0;

      set.samples.Clear();
      if ( Density == null ) return 0;

      for ( int i = 0; i++ < count; )
      {
        double x, y;
        Density.GetSample( out x, out y, rnd.UniformNumber(), rnd );
        set.samples.Add( new Vector2d( x, y ) );

        // User break check:
        if ( (i & 0xffff) == 0 && userBreak ) break;
      }

      return count;
    }

    /// <summary>
    /// Sampling class identifier.
    /// </summary>
    public string Name
    {
      get { return "Random density"; }
    }
  }
}
