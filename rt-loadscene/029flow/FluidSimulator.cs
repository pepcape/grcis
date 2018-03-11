// Author: Jan Dupej, Josef Pelikan

using System.Collections.Generic;
using MathSupport;
using System;

namespace _029flow
{
  /// <summary>
  /// Delegate used for simulation-world initialization.
  /// </summary>
  public delegate void InitWorldDelegate ( FluidSimulator sim );

  public class FluidSimulator
  {
    const double ParticleMass = 0.02;

    const double RepulsiveMax = 500000.0;
    const double RepulsiveRange = 0.05;//075;

    const double AttractiveMax = 0.0;
    const double AttractiveMaxRadius = 0.075;
    const double AttractiveRange = 0.1;

    const double InitialPressureMax = 100.0;
    const double InitialPressureRange = 0.05;

    const double ViscosityAtt = 0.9995;

    const double BounceEfficiency = 0.4;

    const double Epsilon = 1e-10;
    const double FrictionEpsilon = 0.001;

    public class Particle
    {
      /// <summary>
      /// Particle position.
      /// </summary>
      public double x, y;

      /// <summary>
      /// Velocity vector.
      /// </summary>
      public double vx, vy;

      /// <summary>
      /// Total force.
      /// </summary>
      public double fx, fy;

      /// <summary>
      /// Mass.
      /// </summary>
      public double m;

      /// <summary>
      /// Is this particle active?
      /// </summary>
      public bool active = false;
    }

    List<Particle> particles = null;

    int activeParticles;

    public long TotalSpawned;

    double ppt;

    public class Wall
    {
      /// <summary>
      /// Wall end-points.
      /// </summary>
      public double x0, y0, x1, y1;

      /// <summary>
      /// Analytic expression (a,b) normalized.
      /// </summary>
      public double a, b, c;
    }

    List<Wall> walls = null;

    /// <summary>
    /// Simulation field bounds.
    /// </summary>
    public double xMin, xMax, yMin, yMax;

    public void SetBounds ( double xMi, double xMa, double yMi, double yMa )
    {
      xMin = xMi;
      xMax = xMa;
      yMin = yMi;
      yMax = yMa;
    }

    /// <summary>
    /// Presentation bitmap width in pixels.
    /// </summary>
    public int width = 0;

    /// <summary>
    /// Presentation bitmap height in pixels.
    /// </summary>
    public int height = 0;

    public double scalexy = 1.0;

    /// <summary>
    /// Set presentation bitmap size, recalculates aspect ratio acording to simulation field bounds.
    /// </summary>
    public void SetPresentationSize ( ref int wid, ref int hei )
    {
      double scalex = wid / (xMax - xMin);
      double scaley = hei / (yMax - yMin);
      if ( scalex < scaley )
      {
        scalexy = scalex;
        hei = (int)((yMax - yMin) * scalexy);
      }
      else
      {
        scalexy = scaley;
        wid = (int)((xMax - xMin) * scalexy);
      }
      width  = wid;
      height = hei;
    }

    /// <summary>
    /// Total simulation time in seconds.
    /// </summary>
    public double SimTime = 0.0;

    /// <summary>
    /// Buffer for particle density. Used for buffer locking too!
    /// </summary>
    public int[,] cell;

    /// <summary>
    /// Buffers for velocity components / sum of total square velocity.
    /// </summary>
    public double[,] vx, vy, power;

    /// <summary>
    /// Dedicated random generator (each simulation thread should have its own instance).
    /// </summary>
    public RandomJames rnd = null;

    /// <summary>
    /// Progress object for user break.
    /// </summary>
    public Progress progress = null;

    //--- Inlines ---

    double CalcInteraction ( double fR )
    {
      double fRepulsive = 0.0;
      double fAttractive = 0.0;

      if ( fR < RepulsiveRange )
        fRepulsive = RepulsiveMax * (1.0 - fR / RepulsiveRange);

#if USE_ATTRACTIVE
      if ( fR < AttractiveMaxRadius )
        fAttractive = AttractiveMax * fR / AttractiveMaxRadius;
      else
        if ( fR < AttractiveRange )
          fAttractive = AttractiveMax * ( 1.0 - (fR - AttractiveMaxRadius) / (AttractiveRange - AttractiveMaxRadius) );
#endif

      return fAttractive - fRepulsive;
    }

    /// <summary>
    /// This actually causes the fluid to flow
    /// </summary>
    double InitialFluidPressure ( double fRange )
    {
      if ( fRange < 0.0 )
        return InitialPressureMax;

      if ( fRange < InitialPressureRange )                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                    
        return ( InitialPressureMax * (1.0 - fRange / InitialPressureRange) );

      return 0.0;
    }

    void Normalize2DR ( ref double x, ref double y, double r )
    {
      if ( Math.Abs( r ) > Epsilon )
      {
        x /= r;
        y /= r;
      }
    }

    void Normalize2D ( ref double x, ref double y )
    {
      double r = Math.Sqrt( x * x + y * y );

      if ( Math.Abs( r ) > Epsilon )
      {
        x /= r;
        y /= r;
      }
    }

    void InitParticles ( int nParticles )
    {
      particles = new List<Particle>( nParticles );
      for ( int i = 0; i < nParticles; i++ )
        particles.Add( new Particle() );
      activeParticles = 0;
    }

    int ActivateParticles ( int nMax )
    {
      int nCount = Math.Min( nMax, particles.Count - activeParticles );

      if ( nCount < 1 )
        return 0;

      TotalSpawned += nCount;

      int nCountToDo = nCount;

      double fJitterRange = 0.1 / nCountToDo;

      foreach ( var par in particles )
        if ( nCountToDo <= 0 ) break;
        else
          if ( !par.active )
          {
            par.active = true;
            par.x = xMin + 0.0001;
            //par.y = yMin + 0.1 * rnd.UniformNumber() + 0.45;
            par.y = 0.45 + fJitterRange * (rnd.UniformNumber() + nCountToDo - 1.0 );
            par.fx = 0.0;
            par.fy = 0.0;
            par.m = ParticleMass;
            par.vx = 0.0;
            par.vy = 0.0;
            
            nCountToDo--;
          }

      activeParticles += nCount;

      return nCount;
    }

    void InitParticleAccel ()
    {
      foreach ( var par in particles )
        if ( par.active )
        {
          par.fx = InitialFluidPressure( par.x );
          par.fy = 0.0;
        }
    }

    void CalcParticleAccel ()
    {
      double msq = ParticleMass * ParticleMass;
      for ( int i = 0; i < particles.Count - 1; i++ )
        if ( particles[i].active )
        {
          double x1 = particles[i].x;
          double y1 = particles[i].y;
          double x1pl = x1 + RepulsiveRange;
          double x1mi = x1 - RepulsiveRange;
          double y1pl = y1 + RepulsiveRange;
          double y1mi = y1 - RepulsiveRange;

          for ( int j = i+1; j < particles.Count; j++ )
            if ( particles[j].active )
            {
              double x2 = particles[j].x;
              double y2 = particles[j].y;

              if ( x2 > x1mi && x2 < x1pl &&
                   y2 > y1mi && y2 < y1pl )
              {           
                double dx = x2 - x1;
                double dy = y2 - y1;

                double r = Math.Sqrt( dx * dx + dy * dy );

                Normalize2DR( ref dx, ref dy, r );

                double f = CalcInteraction( r ) * msq;

                particles[i].fx += dx * f;
                particles[i].fy += dy * f;

                particles[j].fx -= dx * f;
                particles[j].fy -= dy * f;
              }
            }
        }
    }

    void CalcParticleVelocity ( double fTick )
    {
      for ( int i = 0; i < particles.Count; i++ )
        if ( particles[ i ].active )
        {
          particles[ i ].vx += fTick * particles[ i ].fx;
          particles[ i ].vy += fTick * particles[ i ].fy;

          particles[ i ].vx *= ViscosityAtt;
          particles[ i ].vy *= ViscosityAtt;
        }
    }

    void UpdateParticlePosition ( double fTick )
    {
      for ( int i = 0; i < particles.Count; i++ )
        if ( particles[ i ].active )
        {
          MoveBounceParticle( i, fTick * particles[ i ].vx, fTick * particles[ i ].vy );
          //particles[i].x += fTick * particles[i].vx;
          //particles[i].y += fTick * particles[i].vy;

          if ( particles[ i ].x < xMin || particles[ i ].x > xMax ||
               particles[ i ].y < yMin || particles[ i ].y > yMax )
          {
            activeParticles--;
            particles[ i ].active = false;
          }
        }
    }

    void MoveBounceParticle ( int nIndex, double dx, double dy )
    {
      // assumes nIndex is in range and particle is active
      Particle p = particles[ nIndex ];

      // collide the closest wall (if applicable)
      double t = 100.0; // assume no collision yet
      int nWall = -1;   // collided wall
      Wall w;

      for ( int i = 0; i < walls.Count; i++ )
      {
        w = walls[ i ];

        // calculate the numerator separately, 
        double num = w.a * dx + w.b * dy;

        // protect!
        if ( Math.Abs( num ) < Epsilon )
          continue;

        double tt = -(w.a * p.x + w.b * p.y + w.c) / num;

        // if this wall is closer, remember
        if ( tt >= 0.0 && tt < t && tt < 1.0 )
        {
          // pt in rect
          double xx = p.x + tt * dx;
          double yy = p.y + tt * dy;

          if ( xx < w.x0 && xx < w.x1 ) continue;
          if ( xx > w.x0 && xx > w.x1 ) continue;

          if ( yy < w.y0 && yy < w.y1 ) continue;
          if ( yy > w.y0 && yy > w.y1 ) continue;

          // valid
          t     = tt;
          nWall = i;
        }
      }

      // no collision recorded
      if ( nWall < 0 )
      {
        // just move the particle
        p.x += dx;
        p.y += dy;
        return;
      }

      // --- we indeed have a collision, deal with it honorably

      // The Wall
      w = walls[ nWall ];

      // calculate the "c" of the analytical expression of the impact normal
      double xt = p.x + t * dx;
      double yt = p.y + t * dy;
      double cn = -(w.b * xt - w.a * yt);

      double s = -(w.b * p.x - w.a * p.y + cn);

      // calculate the reflected position
      double xr = p.x + w.b * s * 2.0;
      double yr = p.y - w.a * s * 2.0;

      // the particle will only go the rest of the way
      double tr = 1.0 - t;

      // put the particle in its place
      p.x = (xr - xt) * tr + xt;
      p.y = (yr - yt) * tr + yt;

      // see how fast the particle has been flying
      double v = Math.Sqrt( p.vx * p.vx + p.vy * p.vy );

      // normalize the new velocity vector
      double vx = xr - xt;
      double vy = yr - yt;
      Normalize2D( ref vx, ref vy );

      // calculate a new velocity vector for the feisty litte thing
      p.vx = vx * v * BounceEfficiency;
      p.vy = vy * v * BounceEfficiency;
    }

    void InitWalls ()
    {
      if ( walls == null )
        walls = new List<Wall>();
      else
        walls.Clear();
    }

    //--- Public methods ---

    public FluidSimulator ( Progress prog, RandomJames _rnd =null )
    {
      // particles:
      particles = null;
      activeParticles = 0;
      TotalSpawned = 0L;

      // boundaries:
      walls = null;
      xMin = 0.0;
      xMax = 3.0;
      yMin = 0.0;
      yMax = 1.0;
      InitWalls();

      progress = prog;

      // random generator:
      rnd = _rnd;
      if ( rnd == null )
      {
        rnd = new RandomJames();
        rnd.Randomize();
      }
    }

    public void InitBuffers ()
    {
      cell  = new int[ height, width ];
      vx    = new double[ height, width ];
      vy    = new double[ height, width ];
      power = new double[ height, width ];
    }

    public void Init ( int nParticles, double fPPT )
    {
      ppt = fPPT;
      InitParticles( nParticles );
    }

    public void DeInit ()
    {
      particles = null;
      activeParticles = 0;
      ppt = 0.0;
    }

    public void Tick ( double dT )
    {
      ActivateParticles( (int)(dT * ppt) );
      InitParticleAccel();
      CalcParticleAccel();
      CalcParticleVelocity( dT );
      UpdateParticlePosition( dT );
    }

    public bool AddWall ( double fX0, double fY0, double fX1, double fY1 )
    {
      Wall w = new Wall();

      w.x0 = fX0;
      w.y0 = fY0;
      w.x1 = fX1;
      w.y1 = fY1;

      // calculate the analytic expression of the thing, might come in handy...
      w.a = fY1 - fY0;
      w.b = fX0 - fX1;
      Normalize2D( ref w.a, ref w.b );
      w.c = -w.a * fX0 - w.b * fY0;

      walls.Add( w );

      return true;
    }

    public bool RemoveAllWalls ()
    {
      walls.Clear();

      return true;
    }

    public bool GetParticle ( int nIndex, out double pX, out double pY )
    {
      if ( nIndex >= particles.Count ||
           !particles[ nIndex ].active )
      {
        pX = pY = 0.0;
        return false;
      }

      pX = particles[ nIndex ].x;
      pY = particles[ nIndex ].y;
      return true;
    }

    public bool GetParticleVelocity ( int nIndex, out double vX, out double vY )
    {
      if ( nIndex >= particles.Count ||
           !particles[ nIndex ].active )
      {
        vX = vY = 0.0;
        return false;
      }

      vX = particles[ nIndex ].vx;
      vY = particles[ nIndex ].vy;
      return true;
    }

    public int GetActive ()
    {
      return activeParticles;
    }

    public void GatherBuffers ()
    {
      lock ( cell )
        foreach ( var par in particles )
          if ( par.active )
          {
            int ix = Arith.Clamp( (int)Math.Floor( (par.x - xMin) * scalexy ), 0, width - 1 );
            int iy = Arith.Clamp( (int)Math.Floor( (par.y - yMin) * scalexy ), 0, height - 1 );

            cell[ iy, ix ]++;
            vx[ iy, ix ] += par.vx;
            vy[ iy, ix ] += par.vy;
            double pow = par.vx * par.vx + par.vy * par.vy;
            power[ iy, ix ] += pow;
          }
    }
  }

  public class Worlds
  {
    /// <summary>
    /// Worlds names how to appear in a combo-box.
    /// </summary>
    public static string[] Names =
    {
      "Roof",
      "Rectangle",
      "Maze",
    };

    /// <summary>
    /// The init functions themselves..
    /// </summary>
    public static InitWorldDelegate[] InitFunctions =
    {
      new InitWorldDelegate( Roof ),
      new InitWorldDelegate( Rectangle ),
      new InitWorldDelegate( Maze ),
    };

    /// <summary>
    /// Simple world containing two segments.
    /// </summary>
    public static void Roof ( FluidSimulator sim )
    {
      sim.SetBounds( 0.0, 3.0, 0.0, 1.0 );

      sim.RemoveAllWalls();
      sim.AddWall( -0.01, 0.0, 3.0, 0.0 );
      sim.AddWall( -0.01, 1.0, 3.0, 1.0 );
      sim.AddWall(  0.0,  0.0, 0.0, 1.0 );

      sim.AddWall(  0.5,  0.5, 0.9, 0.15 );
      sim.AddWall(  0.5,  0.5, 0.9, 0.85 );
    }

    /// <summary>
    /// Simple world containing one rectangle.
    /// </summary>
    public static void Rectangle ( FluidSimulator sim )
    {
      sim.SetBounds( 0.0, 3.0, 0.0, 1.0 );

      sim.RemoveAllWalls();
      sim.AddWall( -0.01, 0.0, 3.0, 0.0 );
      sim.AddWall( -0.01, 1.0, 3.0, 1.0 );
      sim.AddWall( 0.0, 0.0, 0.0, 1.0 );

      sim.AddWall( 0.5, 0.2, 0.5, 0.8 );
      sim.AddWall( 0.5, 0.2, 1.1, 0.2 );
      sim.AddWall( 0.5, 0.8, 1.1, 0.8 );
      sim.AddWall( 1.1, 0.2, 1.1, 0.8 );
    }

    /// <summary>
    /// Simple world with a maze.
    /// </summary>
    public static void Maze ( FluidSimulator sim )
    {
      sim.SetBounds( 0.0, 3.0, 0.0, 1.0 );

      sim.RemoveAllWalls();
      sim.AddWall( -0.01, 0.0, 3.0, 0.0 );
      sim.AddWall( -0.01, 1.0, 3.0, 1.0 );
      sim.AddWall( 0.0, 0.0, 0.0, 1.0 );

      sim.AddWall( 0.4, 0.0, 0.4, 0.8 );
      sim.AddWall( 0.6, 0.2, 0.6, 1.0 );
      sim.AddWall( 0.8, 0.0, 0.8, 0.8 );
      sim.AddWall( 1.0, 0.2, 1.0, 1.0 );
      sim.AddWall( 1.2, 0.0, 1.2, 0.8 );
      sim.AddWall( 1.4, 0.2, 1.4, 1.0 );
    }
  }
}
