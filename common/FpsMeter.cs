using System.Collections.Generic;
using System.Diagnostics;

namespace Scene3D
{
  /// <summary>
  /// FPS meter based on Stopwatch measurements.
  /// </summary>
  public class FpsMeter
  {
    const int QUEUE = 128;

    protected Stopwatch sw;

    protected List<long> frames;

    protected int framesPtr;

    protected bool full;

    public FpsMeter ()
    {
      frames = new List<long>( QUEUE );
      framesPtr = 0;
      full = false;
      sw = new Stopwatch();
    }

    public void Start ()
    {
      frames = new List<long>( QUEUE );
      framesPtr = 0;
      full = false;
      sw.Stop();
      sw.Reset();
      sw.Start();
    }

    public float Frame ( int history )
    {
      // 1. time-queue update
      long now = sw.ElapsedMilliseconds;
      int recent = framesPtr++;
      if ( full )
        frames[ recent ] = now;
      else
        frames.Add( now );
      if ( framesPtr >= QUEUE )
      {
        framesPtr = 0;
        full = true;
      }

      // 2. FPS calculation (preparation)
      if ( history < 1 )
        history = 1;
      if ( history >= QUEUE )
        history = QUEUE - 1;
      if ( !full )
        if ( recent == 0 )
          return 0.0f;
        else
          if ( history > recent )
            history = recent;
      int oldest = recent - history;
      if ( oldest < 0 )
        oldest += QUEUE;

      // 3. FPS formula
      return (history * 1000.0f) / (frames[ recent ] - frames[ oldest ]);
    }

    public float Frame ()
    {
      return Frame( QUEUE );
    }

    public void Stop ()
    {
      sw.Stop();
    }
  }
}
