using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using MathSupport;
using OpenTK;
using Raster;
using Utilities;

namespace _077mitchell
{
  /// <summary>
  /// 2D sample set.
  /// </summary>
  public class SampleSet
  {
    /// <summary>
    /// Unsorted set of samples in the [0,1]x[0,1] domain.
    /// </summary>
    public List<Vector2d> samples = new List<Vector2d>();

    /// <summary>
    /// Sampling class's name.
    /// </summary>
    public string samplingSource = null;

    /// <summary>
    /// Textual parameters used in generating this sample set.
    /// </summary>
    public string samplingParams = null;

    /// <summary>
    /// Random seed used in generating this sample set.
    /// See RandomJames.Reset(long)
    /// </summary>
    public long seed = 0L;

    public void Clear ()
    {
      samples.Clear();
    }

    public Bitmap ExportImage ( int resolution )
    {
      if ( samples == null )
        return null;

      Color backgroundColor = Color.White;
      Color sampleColor = Color.Black;
      Bitmap image = new Bitmap( resolution, resolution, PixelFormat.Format24bppRgb );
      using ( Graphics g = Graphics.FromImage( image ) )
        g.Clear( backgroundColor );

      foreach ( var v in samples )
      {
        int x = Arith.Clamp( (int)Math.Floor( v.X * resolution ), 0, resolution - 1 );
        int y = Arith.Clamp( (int)Math.Floor( v.Y * resolution ), 0, resolution - 1 );
        Draw.Dot( image, x, y, 1.0, sampleColor );
      }

      return image;
    }

    public bool TextWrite ( StreamWriter wri, char separator )
    {
      wri.WriteLine( "{0}{1}{0}{2}{0}{3}", separator, PersName, 0, 0 );
      wri.WriteLine( "{1}{0}{2}{0}{3}",
                     separator, samplingSource ?? "", seed, samples.Count );
      wri.WriteLine( samplingParams ?? "" );

      foreach ( var v in samples )
        wri.WriteLine( string.Format( CultureInfo.InvariantCulture, "{1}{0}{2}", separator, v.X, v.Y ) );

      return true;
    }

    /// <summary>
    /// Name of the class for persistence.
    /// </summary>
    public string PersName
    {
      get { return "samples"; }
    }

    public SampleSet ()
    {
    }
  }

  /// <summary>
  /// Sampling template.
  /// </summary>
  public interface ISampling
  {
    /// <summary>
    /// Probability density object.
    /// </summary>
    IPdf Density
    { get; set; }

    /// <summary>
    /// Hash value for result checking.
    /// </summary>
    long Hash
    { get; set; }

    /// <summary>
    /// Statistics: point vs. point distance,
    /// point vs. box distance,
    /// heap operations.
    /// </summary>
    long[] Stat
    { get; }

    /// <summary>
    /// Initial random seed (0L for Randomize).
    /// </summary>
    /// <returns>Actual used random seed (returns result of Randomize()).</returns>
    long SetSeed ( long seed );

    /// <summary>
    /// Generate 'count' samples in the [0,1]x[0,1] domain.
    /// </summary>
    /// <param name="set">Output object.</param>
    /// <param name="count">Desired number of samples</param>
    /// <param name="param">Optional textual parameter set.</param>
    /// <returns>Actual number of generated samples.</returns>
    int GenerateSampleSet ( SampleSet set, int count, string param =null );

    /// <summary>
    /// Returns all the decoration lines in one PathGeometry.
    /// </summary>
    /// <param name="x0">Rectangle origin (X-coord).</param>
    /// <param name="y0">Rectangle origin (Y-coord).</param>
    /// <param name="size">Rectangle size.</param>
    /// <param name="sset">SampleSet to be decorated.</param>
    System.Windows.Media.PathGeometry GetDecorations ( double x0, double y0, double size, SampleSet sset );

    /// <summary>
    /// Asynchronous user break.
    /// </summary>
    bool Break
    { set; }

    /// <summary>
    /// Sampling class identifier.
    /// </summary>
    string Name
    { get; }
  }

  /// <summary>
  /// Probability density function.
  /// Domain: [0,1] x [0,1]
  /// </summary>
  public interface IPdf
  {
    /// <summary>
    /// Direct value of the (unnormalized) density function.
    /// </summary>
    double Pdf ( double x, double y );

    /// <summary>
    /// CDF-based sampling.
    /// </summary>
    /// <param name="random">[0,1] uniform random value.</param>
    /// <param name="rnd">Optional random generator instance. If provided, internal randomization is possible.</param>
    void GetSample ( out double x, out double y, double random, RandomJames rnd =null );
  }

  /// <summary>
  /// Base sampling class, sampling method registry.
  /// </summary>
  public abstract partial class DefaultSampling
  {
    /// <summary>
    /// Dedicated instance of random generator.
    /// </summary>
    protected RandomJames rnd = new RandomJames();

    /// <summary>
    /// Hash value for result checking.
    /// </summary>
    protected long hash = 0L;

    /// <summary>
    /// Hash value for result checking.
    /// </summary>
    public long Hash
    {
      get { return hash; }
      set { hash = value; }
    }

    protected long pointPointCounter = 0L;

    protected long pointBoxCounter = 0L;

    protected long heapCounter = 0L;

    /// <summary>
    /// Statistics: point vs. point distance,
    /// point vs. box distance,
    /// heap operations.
    /// </summary>
    public long[] Stat
    {
      get
      {
        return new long[] { pointPointCounter, pointBoxCounter, heapCounter };
      }
    }

    /// <summary>
    /// Random generator init.
    /// </summary>
    /// <param name="seed">Random seed or 0L for Randomize().</param>
    /// <returns></returns>
    public long SetSeed ( long seed )
    {
      if ( seed <= 0L )
        seed = rnd.Randomize();
      else
        rnd.Reset( seed );

      return seed;
    }

    /// <summary>
    /// User break flag.
    /// </summary>
    protected volatile bool userBreak = false;

    /// <summary>
    /// User break indicator property.
    /// </summary>
    public bool Break
    {
      set
      {
        userBreak = value;
      }
    }

    /// <summary>
    /// Density property.
    /// </summary>
    public IPdf Density { get; set; }

    /// <summary>
    /// Global Decorations tweaking => draw grid instead..
    /// </summary>
    public static bool tweakGrid = false;

    /// <summary>
    /// Global Decorations tweaking: grid resolution
    /// </summary>
    public static int gridResolution = 10;

    /// <summary>
    /// Returns all the decoration lines in one PathGeometry.
    /// </summary>
    public virtual System.Windows.Media.PathGeometry GetDecorations ( double x0, double y0, double size, SampleSet sset )
    {
      return null;
    }

    /// <summary>
    /// Sampling class repository.
    /// </summary>
    public static Dictionary<string, ISampling> Samplings = new Dictionary<string, ISampling>();

    public static void Register ( ISampling s )
    {
      Samplings[ s.Name ] = s;
    }
  }

  /// <summary>
  /// Common code for PDF implementations.
  /// </summary>
  public abstract partial class DefaultPdf
  {
    public const string PDF_UNIFORM = "uniform";
    public const string PDF_IMAGE   = "image file";
    public const string PDF_RAMP    = "Gray ramp";
    public const string PDF_COSRR   = "cos(r^2)";
    public const string PDF_SINCR   = "sinc(r)";
    public const string PDF_SINCOS  = "sin(x)+cos(y)";

    /// <summary>
    /// Rectangular resolution of the raster structures.
    /// </summary>
    public int resolution = 128;

    /// <summary>
    /// Row/column width in the [0,1] space.
    /// </summary>
    public double pixel = 1.0 / 128;

    /// <summary>
    /// Cummulative distribution function in discrete form.
    /// Support data structure for efficient sampling.
    /// If allocated, it should have resolution^2+1 values.
    /// </summary>
    protected double[] cdf = null;

    protected int cdfRes = 0;

    /// <summary>
    /// Prepare support cdf array.
    /// </summary>
    protected void CollectCdf ()
    {
      cdfRes = resolution * resolution + 1;
      cdf = new double[ cdfRes ];
      double acc = 0.0;
      double x, y;
      int i = 1;
      for ( x = 0.5 * pixel; x < 1.0; x += pixel )
        for ( y = 0.5 * pixel; y < 1.0; y += pixel )
        {
          acc += Pdf( x, y );
          cdf[ i++ ] = acc;
        }
      double k = 1.0 / acc;
      for ( i = 1; i < cdfRes; )
        cdf[ i++ ] *= k;
    }

    protected void SetResolution ( int res )
    {
      resolution = res;
      pixel = 1.0 / resolution;
    }

    /// <summary>
    /// [0,1] to raster coordinate conversion.
    /// </summary>
    protected int intCoord ( double xy )
    {
      int ixy = (int)Math.Floor( xy * resolution );
      return Arith.Clamp( ixy, 0, resolution - 1 );
    }

    /// <summary>
    /// Direct value of the (unnormalized) density function.
    /// </summary>
    public virtual double Pdf ( double x, double y )
    {
      return 0.0;
    }

    /// <summary>
    /// CDF-based sampling.
    /// </summary>
    /// <param name="random">[0,1] uniform random value.</param>
    /// <param name="rnd">Optional random generator instance. If provided, internal randomization is possible.</param>
    public virtual void GetSample ( out double x, out double y, double random, RandomJames rnd =null )
    {
      if ( cdf == null )
        CollectCdf();

      // CDF-based importance sampling:
      int ia = 0;
      int ib = cdfRes - 1;
      double a = 0.0;
      double b = 1.0;
      do
      {
        int ip = (ia + ib) >> 1;
        double p = cdf[ ip ];
        if ( p < random )
        {
          ia = ip;
          a = p;
        }
        else
        {
          ib = ip;
          b = p;
        }
      }
      while ( ia + 1 < ib );

      int ix = ia / resolution;
      int iy = ia % resolution;

      x = (ix + (rnd == null ? 0.5 : rnd.UniformNumber())) * pixel;
      y = (iy + (rnd == null ? 0.5 : rnd.UniformNumber())) * pixel;
    }
  }

  /// <summary>
  /// Pdf based on raster image.
  /// </summary>
  public class RasterPdf : DefaultPdf, IPdf
  {
    /// <summary>
    /// Optional image file-name.
    /// </summary>
    public string fileName = null;

    /// <summary>
    /// Gray-converted image data. Ranging from 0.0f to 1.0f.
    /// </summary>
    public float[ , ] data = null;

    /// <summary>
    /// Sets PDF data from a raster image.
    /// </summary>
    public bool SetImage ( Bitmap image, bool negative )
    {
      if ( image == null )
        return false;

      // Rectangle:
      SetResolution( Math.Min( image.Width, image.Height ) );

      // Convert pixels to gray:
      data = new float[ resolution, resolution ];
      for ( int x = 0; x < resolution; x++ )
        for ( int y = 0; y < resolution; y++ )
        {
          data[ x, y ] = Draw.ColorToGray( image.GetPixel( x, y ) );
          if ( negative )
            data[ x, y ] = 1.0f - data[ x, y ];
        }

      return true;
    }

    /// <summary>
    /// The given image will define the distribution.
    /// Only UL rectangular part NxN will be used.
    /// </summary>
    /// <param name="fn">File-name of the image.</param>
    public bool LoadImage ( string fn, bool negative )
    {
      fileName = fn;
      Bitmap image;
      try
      {
        image = new Bitmap( Bitmap.FromFile( fileName ) );
      }
      catch ( Exception e )
      {
        Util.Log( e.Message );
        return false;
      }

      return SetImage( image, negative );
    }

    public RasterPdf ( string fn, bool neg =true )
    {
      LoadImage( fn, neg );
    }

    public RasterPdf ( int res )
    {
      SetResolution( res );
      data = new float[ resolution, resolution ];
    }

    /// <summary>
    /// Direct value of the (unnormalized) density function.
    /// </summary>
    public override double Pdf ( double x, double y )
    {
      Debug.Assert( data != null );

      int ix = intCoord( x );
      int iy = intCoord( y );
      return data[ ix, iy ];
    }
  }

  /// <summary>
  /// Set of standard test functions..
  /// </summary>
  public class DensityFunction : DefaultPdf, IPdf
  {
    /// <summary>
    /// Functions:
    /// 0 .. gray ramp
    /// 1 .. cos(r^2)
    /// 2 .. sinc(r)
    /// 3 .. sin(x)+cos(y)
    /// default .. constant density
    /// </summary>
    public int variant = 0;

    public DensityFunction ( int vari =0, int res =128 )
    {
      variant = vari;
      SetResolution( res );
    }

    /// <summary>
    /// Direct value of the (unnormalized) density function.
    /// </summary>
    public override double Pdf ( double x, double y )
    {
      if ( x < 0.0 )
        x = -x;
      if ( x > 1.0 )
        x = 2.0 - x;
      if ( y < 0.0 )
        y = -y;
      if ( y > 1.0 )
        y = 2.0 - y;

      double r;
      switch ( variant )
      {
        case 0:
          if ( y < 0.4 )
            return x;
          if ( y < 0.7 )
            return 0.5 * x;
          else
            return 0.5 * (1.0 + x);

        case 1:
          r = x * x + y * y;
          return( 0.5 * (1.0 + Math.Cos( 20.0 * r )) );

        case 2:
          r = 30.0 * Math.Sqrt( (x - 0.5) * (x - 0.5) + (y - 0.5) * (y - 0.5) );
          return( 0.2 + 0.8 * ((r < double.Epsilon) ? 1.0 : Math.Sin( r ) / r) );

        case 3:
          return( 0.25 * (2.0 + Math.Sin( x * 15.0 ) + Math.Cos( y * 15.0 )) );

        default:
          return 1.0;
      }
    }
  }
}
