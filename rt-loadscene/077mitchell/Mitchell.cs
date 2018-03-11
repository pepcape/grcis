// Author: Josef Pelikan

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using OpenTK;
using Utilities;

namespace _077mitchell
{
  public partial class Form1 : Form
  {
    /// <summary>
    /// Initialize form parameters.
    /// </summary>
    private void InitializeParams ()
    {
      comboSampling.SelectedIndex = comboSampling.Items.IndexOf( "Random" );
      comboDensity.SelectedIndex  = comboDensity.Items.IndexOf( DefaultPdf.PDF_UNIFORM );
      densityFile                 = "";
      numericSamples.Value        = 1024;
      numericSeed.Value           = 12;
      numericResolution.Value     = 512;
      textParams.Text             = "k=6";
    }
  }

  /// <summary>
  /// Sampling method registry.
  /// </summary>
  public abstract partial class DefaultSampling
  {
    static DefaultSampling ()
    {
      Register( new RandomSampling() );
      Register( new RandomDensitySampling() );
      Register( new MitchellSampling() );
      Register( new MitchellDensitySampling() );
    }
  }

  /// <summary>
  /// Mitchell sampling by dart throwing.
  /// Number of candidates is defined by the parameter 'k'
  /// </summary>
  public class MitchellSampling : DefaultSampling, ISampling
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

      // sampling parameters:
      int K = 5;
      bool toroid = true;
      Dictionary<string, string> p = Util.ParseKeyValueList( param );

      // d = <distance>
      Util.TryParse( p, "k", ref K );
      if ( K < 1 ) K = 1;

      // toroid = {true|false}
      Util.TryParse( p, "toroid", ref toroid );

      hash = 0L;
      pointPointCounter = pointBoxCounter = heapCounter = 0L;

      set.samples.Clear();
      for ( int i = 0; i < count; i++ )
      {
        // generate one sample:
        double bestX = 0.0;
        double bestY = 0.0;   // best candidate so far
        double bestDD = 0.0;  // square distance of the best candidate
        int bestCandidate = 0;

        int candidates = i * K;
        int candidate = 0;
        do
        {
          // one candidate:
          double x = rnd.UniformNumber();
          double y = rnd.UniformNumber();

          // compute its distance to the current sample-set:
          double DD = 4.0;   // current maximal distance squared (will decrease)
          double D = 2.0;    // current maximal distance

          if ( i == 0 ) break;
          bool checkX = toroid && (x < D || x > 1.0 - D);
          bool checkY = toroid && (y < D || y > 1.0 - D);

          foreach ( var s in set.samples )
          {
            bool dirty = false;    // need recompute D, checkX, checkY

            pointPointCounter++;
            double DDx = (x - s.X) * (x - s.X);
            double DDy = (y - s.Y) * (y - s.Y);

            // plain distance:
            if ( DDx + DDy < DD )
            {
              DD = DDx + DDy;
              dirty = true;
            }

            // toroid:
            if ( checkX )
            {
              double save = DDx;

              pointPointCounter++;
              DDx = (x - 1.0 - s.X) * (x - 1.0 - s.X);
              if ( DDx + DDy < DD )
              {
                DD = DDx + DDy;
                dirty = true;
              }

              pointPointCounter++;
              DDx = (x + 1.0 - s.X) * (x + 1.0 - s.X);
              if ( DDx + DDy < DD )
              {
                DD = DDx + DDy;
                dirty = true;
              }

              DDx = save;
            }

            if ( checkY )
            {
              pointPointCounter++;
              DDy = (y - 1.0 - s.Y) * (y - 1.0 - s.Y);
              if ( DDx + DDy < DD )
              {
                DD = DDx + DDy;
                dirty = true;
              }

              pointPointCounter++;
              DDy = (y + 1.0 - s.Y) * (y + 1.0 - s.Y);
              if ( DDx + DDy < DD )
              {
                DD = DDx + DDy;
                dirty = true;
              }
            }

            if ( DD <= bestDD ) break;

            if ( dirty )
            {
              D = Math.Sqrt( DD );
              checkX = toroid && (x < D || x > 1.0 - D);
              checkY = toroid && (y < D || y > 1.0 - D);
            }
          }

          // DD is the farthest distance squared
          if ( DD > bestDD )
          {
            // we have the better candidate:
            bestDD = DD;
            bestX = x;
            bestY = y;
            bestCandidate = candidate;
          }
        }
        while ( candidate++ < candidates );

        hash = hash * 348937L + bestCandidate * 8999L + 4831L;
        set.samples.Add( new Vector2d( bestX, bestY ) );

        // User break check:
        if ( userBreak ) break;
      }

      return set.samples.Count;
    }

    /// <summary>
    /// Sampling class identifier.
    /// </summary>
    public string Name
    {
      get { return "Mitchell"; }
    }
  }

  /// <summary>
  /// Density-controlled Mitchell sampling by dart throwing.
  /// Number of candidates is defined by the parameter 'k'.
  /// </summary>
  public class MitchellDensitySampling : DefaultSampling, ISampling
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

      // sampling parameters:
      int K = 5;
      bool toroid = true;
      if ( param != null )
      {
        string astr;
        Dictionary<string, string> p = Util.ParseKeyValueList( param );

        // d = <distance>
        if ( p.TryGetValue( "k", out astr ) )
        {
          int.TryParse( astr, out K );
          p.Remove( "k" );
        }
        if ( K < 1 ) K = 1;

        // toroid = {true|false}
        if ( p.TryGetValue( "toroid", out astr ) )
        {
          toroid = Util.positive( astr );
          p.Remove( "toroid" );
        }
      }

      hash = 0L;
      pointPointCounter = pointBoxCounter = heapCounter = 0L;

      set.samples.Clear();
      if ( Density == null ) return 0;

      for ( int i = 0; i < count; i++ )
      {
        // generate one sample:
        double bestX = 0.0;
        double bestY = 0.0;    // best candidate so far
        double bestDD = -1.0;  // square distance of the best candidate
        int bestCandidate = 0;

        int candidates = i * K;
        int candidate = 0;
        do
        {
          // one candidate:
          double x, y, density;
          do
          {
            Density.GetSample( out x, out y, rnd.UniformNumber(), rnd );
            // density at the sample point:
            density = Density.Pdf( x, y );
          }
          while ( density < 1e-4 );

          // compute its distance to the current sample-set:
          double DD = double.MaxValue;   // current maximal distance squared (will decrease)

          if ( i == 0 )
          {
            bestX = x;
            bestY = y;
            break;
          }

          foreach ( var s in set.samples )
          {
            pointPointCounter++;
            double DDx = (x - s.X) * (x - s.X);
            double DDy = (y - s.Y ) * (y - s.Y);

            // plain distance:
            if ( DDx + DDy < DD )
              DD = DDx + DDy;

            // toroid:
            if ( toroid )
            {
              double save = DDx;

              pointPointCounter++;
              DDx = (x - 1.0 - s.X) * (x - 1.0 - s.X);
              if ( DDx + DDy < DD )
                DD = DDx + DDy;

              pointPointCounter++;
              DDx = (x + 1.0 - s.X) * (x + 1.0 - s.X);
              if ( DDx + DDy < DD )
                DD = DDx + DDy;

              DDx = save;

              pointPointCounter++;
              DDy = (y - 1.0 - s.Y) * (y - 1.0 - s.Y);
              if ( DDx + DDy < DD )
                DD = DDx + DDy;

              pointPointCounter++;
              DDy = (y + 1.0 - s.Y) * (y + 1.0 - s.Y);
              if ( DDx + DDy < DD )
                DD = DDx + DDy;
            }

            if ( DD * density <= bestDD ) break;
          }

          // DD is the farthest distance squared
          if ( DD * density > bestDD )
          {
            // we have the better candidate:
            bestDD = DD * density;
            bestX = x;
            bestY = y;
            bestCandidate = candidate;
          }
        }
        while ( candidate++ < candidates );

        hash = hash * 348937L + bestCandidate * 8999L + 4831L;
        set.samples.Add( new Vector2d( bestX, bestY ) );

        // User break check:
        if ( userBreak ) break;
      }

      return set.samples.Count;
    }

    /// <summary>
    /// Sampling class identifier.
    /// </summary>
    public string Name
    {
      get { return "Mitchell density"; }
    }
  }
}
