using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using MathSupport;
using _048rtmontecarlo;

namespace Rendering
{
  class Master
  {
    public static Master instance; // singleton

    public ConcurrentQueue<Assignment> availableAssignments;

    private Thread[] pool;

		public Thread mainThread;

    public int totalNumberOfAssignments;

    public int finishedAssignments;

    public const int assignmentSize = 64;

    public object locker = new object ();

    public Progress progressData;

    public int densityCount = 0;
    public int densityCountMax;

		public Master ( Bitmap bitmap, IRayScene scene, IRenderer renderer )
    {
			//instance = this;
      finishedAssignments = 0;

      InitializeAssignments ( bitmap, scene, renderer );
    }

    public void StartThreads( object threads )
    {
      pool = new Thread[(int)threads];


      for ( int i = 0; i < (int)threads; i++ )
      {
				Thread newThread = new Thread ( Consume );
        pool [ i ] = newThread;
        newThread.Start ();
      }

      mainThread = pool [ 0 ];

			for ( int i = 0; i < (int) threads; i++ )
			{
			  pool [ i ].Join ();
			  pool [ i ] = null;
			}
		}

		private void Consume()
    {
      MT.InitThreadData ();

			while ( finishedAssignments < totalNumberOfAssignments )
      {
				Assignment newAssignment;
        availableAssignments.TryDequeue ( out newAssignment );

        if ( !Master.instance.progressData.Continue )
          return;

				if ( newAssignment != null )
        {
					newAssignment.Render ();
				}      
      }
		}

    public void InitializeAssignments ( Bitmap bitmap, IRayScene scene, IRenderer renderer )
    {
      availableAssignments = new ConcurrentQueue<Assignment> ();

      int numberOfAssignmentsOnWidth = bitmap.Width % assignmentSize == 0
        ? bitmap.Width / assignmentSize
        : bitmap.Width / assignmentSize + 1;

      int numberOfAssignmentsOnHeight = bitmap.Height % assignmentSize == 0
        ? bitmap.Height / assignmentSize
        : bitmap.Height / assignmentSize + 1;


			for ( int y = 0; y < numberOfAssignmentsOnHeight; y++ )
      {
        for ( int x = 0; x < numberOfAssignmentsOnWidth; x++ )
        {
          int localX = x * assignmentSize;
          int localY = y * assignmentSize;

          Assignment newAssignment = new Assignment (bitmap, scene, renderer, localX, localY, localX + assignmentSize - 1, localY + assignmentSize - 1);
          availableAssignments.Enqueue ( newAssignment );
        }
      }


      totalNumberOfAssignments = availableAssignments.Count;
      densityCountMax = totalNumberOfAssignments * 8;
    }
	}


  class LocalWorker: Worker
	{
	  protected override void Render ()
	  {
      
	  }
	}

  class NetworkWorker : Worker
	{
		public IPAddress ipAdr;
		private IPEndPoint endPoint;
    private static int port = 5000;


    public bool CreateEndPoint ()
    {
      if ( ipAdr == null)
      {
				return false;
      }
      else
      {
				endPoint = new IPEndPoint ( ipAdr, port );
				return true;
			}     
    }

	  public bool ConnectToClient ()
	  {
	    TcpClient client = new TcpClient ();
	    client.Connect ( endPoint );

	    NetworkStream stream = client.GetStream ();


			return true;
      //TODO: Do something with stream
	  }

	  protected override void Render ()
	  {

	  }
	}

  abstract class Worker
  {
		public Assignment currentAssignemnt;

		public bool isWorking;

    public static void Render ( object worker )
    {
      ( (Worker) worker ).Render ();
    }

    protected abstract void Render ();

  }

  public class Assignment
  {
		internal Bitmap bitmap;
    internal IRayScene scene;
    internal int x1, y1, x2, y2;

    public int density;

    public IRenderer renderer;

    private int bitmapWidth, bitmapHeight;

    private static object locker = new object();

		public Assignment ( Bitmap bitmap, IRayScene scene, IRenderer renderer, int x1, int y1, int x2, int y2 )
    {
			this.bitmap = bitmap;
			this.scene = scene;
			this.renderer = renderer;
			this.x1 = x1;
			this.y1 = y1;
			this.x2 = x2;
			this.y2 = y2;

      density = 8;

      bitmapWidth = bitmap.Width;
      bitmapHeight = bitmap.Height;
    }

    public void Render ()
    {    
      for ( int y = y1; y <= y2; y += density )
      {
        for ( int x = x1; x <= x2; x += density )
        {
          if ( density != 8 && ( y % ( density << 1 ) == 0 ) && ( x % ( density << 1 ) == 0 ) ) // prevents rendering of already rendered pixels
          {
            continue;
          }

          if ( x >= bitmapWidth || y >= bitmapHeight )
          {
						continue;
          }

          double[] color = new double[3];

          renderer.RenderPixel (x, y, color);

          Color c = Color.FromArgb( (int)(color[ 0 ] * 255.0),
                                    (int)(color[ 1 ] * 255.0),
                                    (int)(color[ 2 ] * 255.0) );

					lock ( bitmap )
          {
            if ( density == 1 )
            {
              bitmap.SetPixel ( x, y, c );         
            }
            else
            {
              for ( int j = y; j < y + density; j++ )
              {
                if ( j < bitmapHeight )
                {
									for ( int i = x; i < x + density; i++ )
									{
									  if ( i < bitmapWidth )
									  {
									    bitmap.SetPixel ( i, j, c );								
										}
									}
								}							
							}            
            }
          }

          lock ( Master.instance.progressData )
          {
            if ( !Master.instance.progressData.Continue )
              return;

						if ( Master.instance.mainThread == Thread.CurrentThread )
						{
						  Master.instance.progressData.Finished = Master.instance.densityCount / (float) Master.instance.densityCountMax;
						  Master.instance.progressData.Sync ( bitmap );
						}					
					}
				}
			}


      if ( density == 1 )
      {
        Master.instance.finishedAssignments++;
        Master.instance.densityCount += density;
			}
      else
      {
				density = density >> 1; // density values: 8 > 4 > 2 > 1
        Master.instance.densityCount += density;
				Master.instance.availableAssignments.Enqueue ( this );
			}
		}
	}
}
