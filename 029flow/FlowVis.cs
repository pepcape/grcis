// Author: Jan Dupej, Josef Pelikan

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Threading;
using MathSupport;
using Raster;
using Utilities;

namespace _029flow
{
  public class FlowVisualization
  {
    public Form1.SimulationProgress progress = null;

    public Form1 form;

    public FlowVisualization ( Form1 f )
    {
      form = f;
    }

    /// <summary>
    /// Initialize the simulator object.
    /// </summary>
    public FluidSimulator getSimulator ( int ord )
    {
      DateTime now = DateTime.UtcNow;
      FluidSimulator s = new FluidSimulator( progress, new RandomJames( now.Ticks * (ord + 1) ) );
      return s;
    }

    /// <summary>
    /// The same order as items in the comboScenes.
    /// </summary>
    List<InitWorldDelegate> worldInitFunctions = null;

    /// <summary>
    /// Prepare combo-box of available scenes.
    /// </summary>
    public void InitializeScenes ()
    {
      worldInitFunctions = new List<InitWorldDelegate>( Worlds.InitFunctions );

      // 1. default worlds from Worlds
      foreach ( string name in Worlds.Names )
        form.ComboScene.Items.Add( name );

      // 2. eventually add custom worlds
      //worldInitFunctions.Add( new InitWorldDelegate( xxx ) );
      //form.comboScene.Items.Add( "xxx" );

      // .. and set your favorite scene here:
      form.ComboScene.SelectedIndex = form.ComboScene.Items.IndexOf( "Roof" );

      // default visualization parameters?
      form.ImageWidth  = 600;
      form.ImageHeight = 200;
      form.CheckMultithreading.Checked = true;
    }

    /// <summary>
    /// Buffer dimensions.
    /// </summary>
    int width = 100;
    int height = 100;

    int oldWidth = 100;
    int oldHeight = 100;

    public int Width
    {
      get
      {
        return width;
      }

      set
      {
        oldWidth = width;
        width = value;
      }
    }

    public int Height
    {
      get
      {
        return width;
      }

      set
      {
        oldHeight = height;
        height = value;
      }
    }

    public bool UseMultithreading = true;

    /// <summary>
    /// If true the simulation must be initialized first..
    /// </summary>
    public volatile bool dirty = true;

    /// <summary>
    /// Buffer for particle density.
    /// </summary>
    int[ , ] cell = null;

    /// <summary>
    /// Buffers for velocity components / sum of total square velocity.
    /// </summary>
    double[ , ] vx, vy, power;

    /// <summary>
    /// Extremal values for visualizations.
    /// </summary>
    double maxV2N, maxV;

    /// <summary>
    /// Total simulation time in seconds.
    /// </summary>
    double SimTime = 0.0;

    /// <summary>
    /// Total Spawned particles in all workers.
    /// </summary>
    long TotalSpawned = 0L;

    /// <summary>
    /// Current working array of simulators (one for each working thread).
    /// </summary>
    List<FluidSimulator> sims = null;

    public string WorldName;

    public volatile int SelectedWorld = 0;

    /// <summary>
    /// Global stopwatch for rendering thread. Locked access.
    /// </summary>
    public Stopwatch sw = new Stopwatch();

    /// <summary>
    /// Worker-thread-specific data.
    /// </summary>
    public class WorkerThreadInit
    {
      /// <summary>
      /// Fluid simulator instance.
      /// </summary>
      public FluidSimulator sim;

      public int nPart;

      public double ppt;

      public double dt;

      public double vart;

      public WorkerThreadInit ( FluidSimulator s, int n, double pptake, double deltat, double variancet )
      {
        sim = s;
        nPart = n;
        ppt = pptake;
        dt = deltat;
        vart = variancet;
      }
    }

    /// <summary>
    /// Routine of one worker-thread.
    /// Collect arrays and rendering progress are the only two shared objects.
    /// </summary>
    /// <param name="spec">Thread-specific data (worker-thread-selector).</param>
    private void SimulationWorker ( Object spec )
    {
      WorkerThreadInit init = spec as WorkerThreadInit;
      if ( init != null )
      {
        init.sim.Init( init.nPart, init.ppt );

        // infinite simulation loop:
        do
        {
          double deltaT = init.dt + init.vart * (init.sim.rnd.UniformNumber() - 1.0);
          init.sim.Tick( deltaT );
          init.sim.SimTime += deltaT;
          lock ( progress )
            if ( !progress.Continue ) break;
          init.sim.GatherBuffers();
        }
        while ( true );
      }
    }

    /// <summary>
    /// Runs the simulation (in separate thread[s]).
    /// </summary>
    public void RunSimulation ()
    {
      // allocate & init simulator array:
      int threads = Math.Max( 1, Environment.ProcessorCount - 1 );
      if ( !UseMultithreading ) threads = 1;
      sims = new List<FluidSimulator>( threads );
      int t;
      for ( t = 0; t < threads; t++ )
      {
        FluidSimulator fs = getSimulator( t );
        worldInitFunctions[ SelectedWorld ]( fs );
        sims.Add( fs );
      }
      int origW = width;
      int origH = height;
      foreach ( var sim in sims )
      {
        width = origW;
        height = origH;
        sim.SetPresentationSize( ref width, ref height );
        sim.InitBuffers();
      }

      // output presentation image:
      if ( form.outputImage != null )
        form.outputImage.Dispose();
      form.outputImage = new Bitmap( width, height, PixelFormat.Format24bppRgb );
      SyncObject so = new SyncObject();
      so.bmp = form.outputImage;

      if ( dirty ||
           oldWidth != width ||
           oldHeight != height )
      {
        SimTime = 0.0;
        TotalSpawned = 0L;
        cell = new int[ height, width ];
        vx = new double[ height, width ];
        vy = new double[ height, width ];
        power = new double[ height, width ];
        dirty = false;
      }
      else
      {
        sims[ 0 ].SimTime = SimTime;
        sims[ 0 ].TotalSpawned = TotalSpawned;
        System.Array.Copy( cell, sims[ 0 ].cell, width * height );
        System.Array.Copy( vx, sims[ 0 ].vx, width * height );
        System.Array.Copy( vy, sims[ 0 ].vy, width * height );
        System.Array.Copy( power, sims[ 0 ].power, width * height );
      }

      // progress & timer:
      progress.SyncInterval = ((width * (long)height) > (2L << 20)) ? 30000L : 10000L;
      progress.Reset();
      lock ( sw )
      {
        sw.Reset();
        sw.Start();
      }

      // run the simulators:
      Thread[] pool = new Thread[ threads ];
      for ( t = 0; t < threads; t++ )
        pool[ t ] = new Thread( new ParameterizedThreadStart( this.SimulationWorker ) );
      for ( t = threads; --t >= 0; )
        pool[ t ].Start( new WorkerThreadInit( sims[ t ], 8000, 4500.0f, 0.004, 0.004 ) );

      do
      {
        Thread.Sleep( 2000 );

        bool velocity = false;
        bool pressure = false;
        lock ( progress )
        {
          if ( !progress.Continue ) break;
          if ( !progress.NeedsSync() ) continue;

          velocity = progress.velocity;
          pressure = progress.pressure;
        }

        // 1. collect data from all workers:
        int x, y;
        lock ( cell )
        {
          lock ( sims[ 0 ].cell )
          {
            TotalSpawned = sims[ 0 ].TotalSpawned;
            SimTime = sims[ 0 ].SimTime;
            System.Array.Copy( sims[ 0 ].cell, cell, width * height );
            System.Array.Copy( sims[ 0 ].vx, vx, width * height );
            System.Array.Copy( sims[ 0 ].vy, vy, width * height );
            System.Array.Copy( sims[ 0 ].power, power, width * height );
          }
          for ( t = 1; t < threads; t++ )
            lock ( sims[ t ].cell )
            {
              TotalSpawned += sims[ t ].TotalSpawned;
              SimTime += sims[ t ].SimTime;
              for ( y = 0; y < height; y++ )
                for ( x = 0; x < width; x++ )
                {
                  cell[ y, x ] += sims[ t ].cell[ y, x ];
                  vx[ y, x ] += sims[ t ].vx[ y, x ];
                  vy[ y, x ] += sims[ t ].vy[ y, x ];
                  power[ y, x ] += sims[ t ].power[ y, x ];
                }
            }
        }
        so.simTime = SimTime;
        so.totalSpawned = TotalSpawned;

        // 2. visualizations - default (pressure/velocity) and/or custom
        if ( progress.velocity ||
             progress.pressure ||
             progress.custom )
        {
          maxV2N = 0.001;
          double V2N;
          maxV = 0.0;

          int n;
          for ( y = 2; y < height - 2; y++ )    // avoid borders..
            for ( x = 2; x < width - 2; x++ )
              if ( (n = cell[ y, x ]) > 0 )
              {
                if ( (V2N = n * power[ y, x ]) > maxV2N )
                  maxV2N = V2N;
                double mvx = Math.Abs( vx[ y, x ] / n );
                double mvy = Math.Abs( vy[ y, x ] / n );
                if ( mvx > maxV )
                  maxV = mvx;
                if ( mvy > maxV )
                  maxV = mvy;
              }

          int origin = 0;
          if ( progress.velocity ) origin += height;
          if ( progress.pressure ) origin += height;
          if ( progress.custom ) origin += height;

          if ( origin > 0 && origin != form.outputImage.Height )
          {
            if ( form.outputImage != null )
              form.outputImage.Dispose();
            so.bmp = form.outputImage = new Bitmap( width, origin, PixelFormat.Format24bppRgb );
          }

          origin = 0;
          // individual visualizations - velocity
          if ( progress.velocity )
          {
            VisualizeVelocity( so, 0, origin );
            origin += height;
          }

          // individual visualizations - pressure
          if ( progress.pressure )
          {
            VisualizePressure( so, 0, origin );
            origin += height;
          }

          // individual visualizations - custom routine
          if ( progress.custom )
          {
            VisualizeCustom( so, 0, origin, progress.param );
            origin += height;
          }

          progress.Sync( so );
        }
      }
      while ( true );

      // wait for the simulator threads:
      for ( t = 0; t < threads; t++ )
      {
        pool[ t ].Join();
        pool[ t ] = null;
      }

      long elapsed;
      lock ( sw )
      {
        sw.Stop();
        elapsed = sw.ElapsedMilliseconds;
      }

      string msg = string.Format( CultureInfo.InvariantCulture,
                                  "{0:f1}s  [ {1}x{2}, mt{3}, sim{4:f1}s, spawned{5} ]",
                                  1.0e-3 * elapsed, width, height, threads,
                                  SimTime, Util.kmg( TotalSpawned ) );
      form.SetText( msg );
      Console.WriteLine( "Simulation finished: " + msg );
      form.SetImage( (Bitmap)form.outputImage.Clone() );

      form.StopSimulation();
    }

    public void SaveResults ( string fileName )
    {
      if ( cell == null )
        return;

      lock ( cell )
        using ( StreamWriter wri = new StreamWriter( fileName ) )
        {
          wri.WriteLine( "\"world\";\"sim-time\";\"spawned\";\"width\";\"height\"" );
          wri.WriteLine( string.Format( CultureInfo.InvariantCulture, "\"{0}\";{1:f1};{2};{3};{4}",
                                        WorldName, SimTime, TotalSpawned, width, height ) );
          wri.WriteLine( "\"x\";\"y\";\"particles\";\"mean vx\";\"mean vy\";\"rms v\"" );
          double scale = 1.0 / sims[ 0 ].scalexy;
          for ( int y = 0; y < height; y++ )
            for ( int x = 0; x < width; x++ )
            {
              int n = cell[ y, x ];
              int denom = Math.Max( n, 1 );
              wri.WriteLine( string.Format( CultureInfo.InvariantCulture, "{0:f5};{1:f5};{2};{3:f6};{4:f6};{5:f6}",
                                            x * scale, y * scale, n, vx[ y, x ] / denom, vy[ y, x ] / denom,
                                            Math.Sqrt( power[ y, x ] / denom ) ) );
            }
        }
    }

    public bool LoadResults ( string fileName )
    {
      using ( StreamReader rea = fileName.EndsWith( ".gz" ) ?
              new StreamReader( new GZipStream( new FileStream( fileName, FileMode.Open ), CompressionMode.Decompress ) ) :
              new StreamReader( new FileStream( fileName, FileMode.Open ) ) )
      {
        string line;
        do
          line = rea.ReadLine();
        while ( line != null &&
                line != "\"world\";\"sim-time\";\"spawned\";\"width\";\"height\"" );
        if ( line == null ) return false;
        line = rea.ReadLine();
        if ( line == null ) return false;
        string[] token = line.Split( ';' );
        if ( token == null ||
             token.Length < 5 )
          return false;
        if ( token[ 0 ].Length < 3 ) return false;
        if ( rea.ReadLine() == null )
          return false;
        WorldName = token[ 0 ].Trim( '\"' );

        double newSimTime;
        long newTotalSpawned;
        int newWidth;
        int newHeight;
        if ( !double.TryParse( token[ 1 ], NumberStyles.Float, CultureInfo.InvariantCulture, out newSimTime ) ||
             !long.TryParse( token[ 2 ], out newTotalSpawned ) ||
             !int.TryParse( token[ 3 ], out newWidth ) ||
             !int.TryParse( token[ 4 ], out newHeight ) )
          return false;

        SimTime = newSimTime;
        TotalSpawned = newTotalSpawned;
        width = newWidth;
        height = newHeight;

        cell = new int[ height, width ];
        vx = new double[ height, width ];
        vy = new double[ height, width ];
        power = new double[ height, width ];

        int x, y;
        for ( y = 0; y < height; y++ )
          for ( x = 0; x < width; x++ )
          {
            line = rea.ReadLine();
            if ( line == null ||
                 (token = line.Split( ';' )) == null ||
                 token.Length < 6 )
            {
              y = height + 1;
              break;
            }
            int newCell;
            double newVx, newVy, newPower;
            if ( !int.TryParse( token[ 2 ], out newCell ) ||
                 !double.TryParse( token[ 3 ], NumberStyles.Float, CultureInfo.InvariantCulture, out newVx ) ||
                 !double.TryParse( token[ 4 ], NumberStyles.Float, CultureInfo.InvariantCulture, out newVy ) ||
                 !double.TryParse( token[ 5 ], NumberStyles.Float, CultureInfo.InvariantCulture, out newPower ) )
            {
              y = height + 1;
              break;
            }
            cell[ y, x ] = newCell;
            vx[ y, x ] = newVx * newCell;
            vy[ y, x ] = newVy * newCell;
            power[ y, x ] = newPower * newPower * newCell;
          }

        dirty = (y >= height + 1);
      }
      return true;
    }

    public string Probe ( int x, int y )
    {
      int cellLoc = 0;
      double vxLoc = 0.0, vyLoc = 0.0, powerLoc = 0.0;
      y = y % height;

      if ( cell == null ) return "";
      lock ( cell )
      {
        if ( y < 0 || y >= cell.GetLength( 0 ) ||
             x < 0 || x >= cell.GetLength( 1 ) )
          return "";

        cellLoc = cell[ y, x ];
        vxLoc = vx[ y, x ];
        vyLoc = vy[ y, x ];
        powerLoc = power[ y, x ];
      }

      // local state of the field: cellLoc, vxLoc, vyLoc, powerLoc
      double RMSpower = 0.0, Mvx = 0.0, Mvy = 0.0;
      if ( cellLoc > 0 )
      {
        RMSpower = Math.Sqrt( powerLoc / cellLoc );
        Mvx = vxLoc / cellLoc;
        Mvy = vyLoc / cellLoc;
      }

      // show results:
      return string.Format( CultureInfo.InvariantCulture, "State[{0},{1}]: particles={2}({3}), Mv=[{4:f4},{5:f4}], RMSpower={6:f4}",
                            x, y, cellLoc, Util.kmg( TotalSpawned ), Mvx, Mvy, RMSpower );
    }

    /// <summary>
    /// Specimen - velocity visualization.
    /// </summary>
    /// <param name="so">so.bmp is the target Bitmap.</param>
    /// <param name="x0">Visualization origin - x.</param>
    /// <param name="y0">Visualization origin - y.</param>
    public void VisualizeVelocity ( SyncObject so, int x0, int y0 )
    {
      int r, g, b, num;
      int x, y;
      Color col;
      double vMul = 128.0 / maxV;

      for ( y = 0; y < height; y++ )
        for ( x = 0; x < width; x++ )
        {
          if ( (num = cell[ y, x ]) < 1 )
            col = Color.FromArgb( 0, 0, 128 );
          else
          {
            r = (int)(128 + vMul * vx[ y, x ] / num);
            g = (int)(128 + vMul * vy[ y, x ] / num);
            b = 0;
            col = Color.FromArgb( Arith.Clamp( r, 0, 255 ),
                                  Arith.Clamp( g, 0, 255 ),
                                  Arith.Clamp( b, 0, 255 ) );
          }
          so.bmp.SetPixel( x0 + x, y0 + y, col );
        }
    }

    /// <summary>
    /// Specimen - pressure visualization.
    /// </summary>
    /// <param name="so">so.bmp is the target Bitmap.</param>
    /// <param name="x0">Visualization origin - x.</param>
    /// <param name="y0">Visualization origin - y.</param>
    public void VisualizePressure ( SyncObject so, int x0, int y0 )
    {
      int num, x, y;
      Color col;
      double pressMul = 2.0 / Math.Sqrt( maxV2N );

      for ( y = 0; y < height; y++ )
        for ( x = 0; x < width; x++ )
        {
          if ( (num = cell[ y, x ]) < 1 )
            col = Color.FromArgb( 0, 0, 128 );
          else
            col = Draw.ColorRamp( pressMul * Math.Sqrt( num * power[ y, x ] ) );
          so.bmp.SetPixel( x0 + x, y0 + y, col );
        }
    }

    /// <summary>
    /// Custom visualization.
    /// </summary>
    /// <param name="so">so.bmp is the target Bitmap.</param>
    /// <param name="x0">Visualization origin - x.</param>
    /// <param name="y0">Visualization origin - y.</param>
    /// <param name="param">Optional text parameter[s].</param>
    public void VisualizeCustom ( SyncObject so, int x0, int y0, string param )
    {
      // !!!{{ TODO: put your visualization code here

      int x, y;
      for ( y = 0; y < height; y++ )
        for ( x = 0; x < width; x++ )
        {
          so.bmp.SetPixel( x0 + x, y0 + y, Draw.ColorRamp( (double)x / width ) );
        }

      // !!!}}
    }
  }
}
