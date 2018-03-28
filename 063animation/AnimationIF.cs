using System;
using System.Diagnostics;
using Rendering;
using MathSupport;

namespace _063animation
{
  public class FormSupport
  {
    /// <summary>
    /// Initialize animation parameters.
    /// </summary>
    public static void InitializeParams ( out string name )
    {
      name = "Josef Pelikán";

      Form1 f = Form1.singleton;

      // single frame:
      f.ImageWidth = 640;
      f.ImageHeight = 480;
      f.numericSupersampling.Value = 4;

      // animation:
      f.numFrom.Value = (decimal)0.0;
      f.numTo.Value = (decimal)10.0;
      f.numFps.Value = (decimal)25.0;
    }

    /// <summary>
    /// Initialize (optional) animation data.
    /// </summary>
    public static object getData ()
    {
      return new AnimationData();
    }

    /// <summary>
    /// Initialize image function.
    /// </summary>
    public static IImageFunction getImageFunction ( object data )
    {
      return new Animation( data );
    }

    /// <summary>
    /// Initialize image synthesizer (responsible for raster image computation).
    /// </summary>
    public static IRenderer getRenderer ( IImageFunction imf )
    {
      Form1 f = Form1.singleton;

      if ( f.superSampling > 1 )
      {
        SupersamplingImageSynthesizer sis = new SupersamplingImageSynthesizer();
        sis.ImageFunction = imf;
        sis.Supersampling = f.superSampling;
        sis.Jittering = 1.0;
        return sis;
      }
      SimpleImageSynthesizer s = new SimpleImageSynthesizer();
      s.ImageFunction = imf;
      return s;
    }
  }

  /// <summary>
  /// Pilot implementation of time-dependent image function.
  /// </summary>
  public class Animation : IImageFunction, ITimeDependent
  {
    /// <summary>
    /// Starting (minimal) time in seconds.
    /// </summary>
    public double Start
    {
      get;
      set;
    }

    /// <summary>
    /// Ending (maximal) time in seconds.
    /// </summary>
    public double End
    {
      get;
      set;
    }

    protected double time;

    protected double frequency = 100.0;

    protected double width = 1.0;

    protected double height = 1.0;

    protected double center;

    protected double mul;

    protected void setup ()
    {
      center = 0.5 * width;
      mul = frequency / width;
    }

    /// <summary>
    /// Looks around to an infinite checkerboard.
    /// One complete turn for the whole time interval.
    /// </summary>
    protected virtual void setTime ( double newTime )
    {
      Debug.Assert( Start != End );

      time = newTime;

      // change the view azimuth:
      double pr = (time - Start) / (End - Start);
      frequency = 100.0 + pr * 9900.0;
      setup();
    }

    /// <summary>
    /// Current time in seconds.
    /// </summary>
    public double Time
    {
      get
      {
        return time;
      }
      set
      {
        setTime( value );
      }
    }

    /// <summary>
    /// Domain width.
    /// </summary>
    public double Width
    {
      get
      {
        return width;
      }
      set
      {
        width = value;
        setup();
      }
    }

    /// <summary>
    /// Domain height.
    /// </summary>
    public double Height
    {
      get
      {
        return height;
      }
      set
      {
        height = value;
        setup();
      }
    }

    protected object data = null;

    protected double[] bg = new double[] { 0.0, 0.0, 0.0 };
    protected double[] fg = new double[] { 1.0, 1.0, 1.0 };

    /// <summary>
    /// Computes one image sample. Simple variant, w/o an integration support.
    /// </summary>
    /// <param name="x">Horizontal coordinate.</param>
    /// <param name="y">Vertical coordinate.</param>
    /// <param name="color">Computed sample color.</param>
    /// <returns>Hash-value used for adaptive subsampling.</returns>
    public virtual long GetSample ( double x, double y, double[] color )
    {
      return GetSample( x, y, 0, 0, null, color );
    }

    /// <summary>
    /// Computes one image sample. Internal integration support.
    /// </summary>
    /// <param name="x">Horizontal coordinate.</param>
    /// <param name="y">Vertical coordinate.</param>
    /// <param name="rank">Rank of this sample, 0 <= rank < total (for integration).</param>
    /// <param name="total">Total number of samples (for integration).</param>
    /// <param name="rnd">Global (per-thread) instance of the random generator.</param>
    /// <param name="color">Computed sample color.</param>
    /// <returns>Hash-value used for adaptive subsampling.</returns>
    public virtual long GetSample ( double x, double y, int rank, int total, RandomJames rnd, double[] color )
    {
      long ord = 0L;
      if ( !Geometry.IsZero( y ) )
        ord = (long)(Math.Round( mul * (x - center) / y ) + Math.Round( frequency / y ));
#if false
      double u = 0.0, v = 0.0;
      if ( !Geometry.IsZero( y ) )
      {
        u   = mul * (x - center) / y;
        v   = frequency / y;
        ord = (long)(Math.Round( u + v ) + Math.Round( u - v ));
      }
#endif
      Array.Copy( (ord & 1L) == 0 ? fg : bg, color, fg.Length );
      return ord;
    }

    /// <summary>
    /// Clone all the time-dependent components, share the others.
    /// </summary>
    /// <returns></returns>
    public virtual object Clone ()
    {
      ITimeDependent datatd = data as ITimeDependent;
      Animation c = new Animation( (datatd == null) ? data : datatd.Clone() );
      c.Start  = Start;
      c.End    = End;
      c.Time   = Time;
      c.Width  = width;
      c.Height = height;
      return c;
    }

    public Animation ( object d =null )
    {
      data   = d;
      Start  = 0.0;
      End    = 10.0;
      time   = 0.0;
      Width  = 1.0;
      Height = 1.0;
    }
  }

  /// <summary>
  /// Animation data (optional).
  /// </summary>
  public class AnimationData    // : ITimeDependent
  {
    public AnimationData ()
    {
      // !!!{{ TODO: initialize the animation data

      // !!!}}
    }
  }
}
