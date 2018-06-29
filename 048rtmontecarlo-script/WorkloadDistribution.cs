using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Rendering
{
	class WorkloadDistribution
	{

	}


  class Master
  {
    public static Master instance; // singleton

    public ConcurrentQueue<Assignment> availableAssignments;

    public int totalNumberOfAssignments;

    public int finishedAssignments;

    public const int assignmentSize = 64;

    public Master ( Bitmap bitmap, IRayScene scene, IRenderer renderer )
    {
			instance = this;
      finishedAssignments = 0;

      InitializeAssignmnts ( bitmap, scene, renderer );
    }

    public void StartThreads( bool multithreading )
    {     
			int threads = multithreading ? Environment.ProcessorCount : 1;

      for ( int i = 0; i < threads; i++ )
      {
        Thread newThread = new Thread ( Consume );
        newThread.Start ();
      }

      /*if ( threads > 1 )
      {
        availableThreads = new ConcurrentQueue<Thread>();

				for ( int i = 0; i < threads; i++ )
				{
				  availableThreads.Enqueue ( new Thread ( Assignment.Render ) );
				}
      }
      else
      {
        //TODO: Add code for singlethread case
      }*/

      
    }

		private void Consume()
    {
      while ( finishedAssignments < totalNumberOfAssignments )
      {
				Assignment newAssignment;
        availableAssignments.TryDequeue ( out newAssignment );

        newAssignment.Render ();
      }
		}

    public void InitializeAssignmnts ( Bitmap bitmap, IRayScene scene, IRenderer renderer )
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
    }

    /*public static void Render ( object assignment )
    {
      ( (Assignment) assignment ).Render ();
    }*/

    public void Render ()
    {
      for ( int y = y1; y < y2; y += density )
      {
        for ( int x = x1; x < x2; x += density )
        {
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
                if ( j < bitmap.Height )
                {
									for ( int i = x; i < x + density; i++ )
									{
									  if ( i < bitmap.Width )
									  {
											bitmap.SetPixel ( i, j, c );
										}
									}
								}							
							}            
            }
          }
        }
			}


      if ( density == 1)
      {
        Master.instance.finishedAssignments++;
      }
      else
      {
				density = density >> 1; // density values: 8 > 4 > 2 > 1
        Master.instance.availableAssignments.Enqueue ( this );
			}
		}
	}
}
