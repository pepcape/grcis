// Author: Jan Dupej, Josef Pelikan

using System.Collections.Generic;
using MathSupport;
using System;

namespace _029flow
{
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

    List<Particle> particles = null;

    int activeParticles;

    long totalSpawned;

    double ppt;

    List<Wall> walls = null;

    public RandomJames rnd = null;

    /*
    static float pDot[5][5] = { {0.0625f, 0.125f, 0.25f, 0.125f, 0.0625f} ,
                              {0.125f, 0.25f, 0.5f, 0.25f, 0.125f} ,
                              {0.25f, 0.5f, 1.0f, 0.5f, 0.25f} ,
                              {0.125f, 0.25f, 0.5f, 0.25f, 0.125f} ,
                              {0.0625f, 0.125f, 0.25f, 0.125f, 0.0625f} };
    */

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
          fAttractive = AttractiveMax * ( 1.0 - (fR-AttractiveMaxRadius) / (AttractiveRange-AttractiveMaxRadius) );
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

      totalSpawned += nCount;

      int nCountToDo = nCount;

      double fJitterRange = 1.0 / nCountToDo;

      foreach ( var par in particles )
        if ( nCountToDo <= 0 ) break;
        else
          if ( !par.active )
          {
            par.active = true;
            par.x = 0.0001;
            par.y = 0.1 * rnd.UniformNumber() + 0.45;
            //par.y = fJitterRange * rnd.UniformNumber() + ( nCountToDo - 1.0 ) * fJitterRange;
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
      for ( int i = 0; i < particles.Count - 1; i++ )
        if ( particles[i].active )
        {
          double x1 = particles[i].x;
          double y1 = particles[i].y;

          for ( int j = i+1; j < particles.Count; j++ )
            if ( particles[j].active )
            {
              double x2 = particles[j].x;
              double y2 = particles[j].y;

              if( (x2 + RepulsiveRange > x1) && (x2 - RepulsiveRange < x1) &&
                ( (y2 + RepulsiveRange > y1) && (y2 - RepulsiveRange < y1) )
              {           
                double dx = x2 - x1;
                double dy = y2 - y1;

                double r = Math.Sqrt( dx * dx + dy * dy );

                Normalize2DR( ref dx, ref dy, r );

                double f = CalcInteraction( r );
                double msq = ParticleMass * ParticleMass;

                particles[i].fx += msq * dx * f;
                particles[i].fy += msq * dy * f;

                particles[j].fx -= msq * dx * f;
                particles[j].fy -= msq * dy * f;
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

          if ( (particles[ i ].x < 0.0) || (particles[ i ].x > 3.0) ||
               (particles[ i ].y <= 0.0) || (particles[ i ].y >= 1.0) )
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
      int nWall = -1; // collided wall
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
        if ( (tt >= 0.0) && (tt < t) && (tt < 1.0) )
        {
          // pt in rect
          double xx = p.x + tt * dx;
          double yy = p.y + tt * dy;

          if ( (xx < w.x0) && (xx < w.x1) ) continue;
          if ( (xx > w.x0) && (xx > w.x1) ) continue;

          if ( (yy < w.y0) && (yy < w.y1) ) continue;
          if ( (yy > w.y0) && (yy > w.y1) ) continue;

          // valid
          t = tt;
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

    void InitWalls ( int nMaxWalls )
    {
      walls = new List<Wall>( nMaxWalls );
    }

    void DeInitWalls ()
    {
      walls = null;
    }

    //--- Public methods ---

    public FluidSimulator ( RandomJames _rnd =null )
    {
      particles = null;
      activeParticles = 0;
      walls = null;
      totalSpawned = 0L;
      rnd = _rnd;
      if ( rnd == null )
      {
        rnd = new RandomJames();
        rnd.Randomize();
      }
    }

    public void Init ( int nParticles, double fPPT, int nMaxWalls =16 )
    {
      ppt = fPPT;
      InitParticles( nParticles );
      InitWalls( nMaxWalls );
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

    public long GetTotalSpawned ()
    {
      return totalSpawned;
    }

    public void GatherPressure ( float[][] pBuffer )
    {
      int hei = pBuffer.Length;
      int wid = pBuffer[0].Length;

      foreach ( var par in particles )
        if ( par.active )
        {
          int ix = Arith.Clamp( (int)(par.x * wid), 0, wid - 1 );
          int iy = Arith.Clamp( (int)(par.y * hei), 0, hei - 1 );
          pBuffer[ iy ][ ix ] += 1.0f;
        }
    }

    public void GatherVelocity ( float[][] pX, float[][] pY, int[][] pSamps )
    {
      int hei = pX.Length;
      int wid = pY[ 0 ].Length;
      if ( pY.Length < hei ||
           pY[ 0 ].Length < wid ||
           pSamps.Length < hei ||
           pSamps[ 0 ].Length < wid )
        return;

      foreach ( var par in particles )
        if ( par.active )
        {
          int ix = Arith.Clamp( (int)(par.x * wid), 0, wid - 1 );
          int iy = Arith.Clamp( (int)(par.y * hei), 0, hei - 1 );
          pX[ iy ][ ix ] += (float)par.vx;
          pX[ iy ][ ix ] += (float)par.vy;
          pSamps[ iy ][ ix ]++;
        }
    }

    public void GatherDensity ( float[][] pBuffer )
    {
      int hei = pBuffer.Length;
      int wid = pBuffer[ 0 ].Length;
      int hei2 = hei / 2;
      int wid2 = wid / 2;

      foreach ( var par in particles )
        if ( par.active )
        {
          int ix = Arith.Clamp( (int)(par.x * wid2), 0, wid2 - 1 );
          int iy = Arith.Clamp( (int)(par.y * hei2), 0, hei2 - 1 );
          ix += ix;
          iy += iy;
          pBuffer[ iy ][ ix ] += 1.0f;
          pBuffer[ iy ][ ix + 1 ] += 1.0f;
          pBuffer[ iy + 1 ][ ix ] += 1.0f;
          pBuffer[ iy + 1 ][ ix + 1 ] += 1.0f;
        }
    }
  }
}
