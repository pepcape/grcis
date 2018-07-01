using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MathSupport;
using _048rtmontecarlo;

namespace Rendering
{
  /// <summary>
	/// Takes care of distribution of rendering work between local threads and remote/network render clients
	/// </summary>
  class Master
  {
    public static Master instance; // singleton

    public ConcurrentQueue<Assignment> availableAssignments;

    private Thread[] pool;

		public Thread mainThread;

    public int totalNumberOfAssignments;

    public int finishedAssignments;

		// width and height of one block of pixels (rendered at one thread at the time); 32 seems to be optimal; should be power of 2 and smaller than 8
		public const int assignmentSize = 32;

		public Progress progressData;

    public int assignmentRoundsFinished = 0;
    public int assignmentRoundsTotal;

    public Bitmap bitmap;
    public IRayScene scene;
    public IRenderer renderer;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="bitmap">Main bitmap - used also in PictureBox in Form1</param>
		/// <param name="scene">Scene to render</param>
		/// <param name="renderer">Rendered to use for RenderPixel method</param>
		public Master ( Bitmap bitmap, IRayScene scene, IRenderer renderer )
    {
      finishedAssignments = 0;

			this.bitmap = bitmap;
			this.scene = scene;
			this.renderer = renderer;

			InitializeAssignments ( bitmap, scene, renderer );


			if ( RenderClientsForm.instance == null )
      {
				RenderClientsForm.instance = new RenderClientsForm ();
			}
		}

    /// <summary>
		/// Creates threadpool and starts all threads on Consume method
		/// </summary>
		/// <param name="threads">Number of threads - is expected to be int</param>
    public void StartThreads( object threads )
    {
      pool = new Thread[(int)threads];


      for ( int i = 0; i < (int)threads; i++ )
      {
				Thread newThread = new Thread ( Consume );
        newThread.Priority = ThreadPriority.AboveNormal;
        pool [ i ] = newThread;
        newThread.Start ();
      }

      mainThread = pool [ 0 ];

      AssignNetworkWorkerToStream ();

			for ( int i = 0; i < (int) threads; i++ )
			{
			  pool [ i ].Join ();
			  pool [ i ] = null;
			}
		}

		/// <summary>
		/// Consumer-producer based multithreading work distribution
		/// Each thread waits for a new Assignment to be added to availableAssignments queue
		/// Most of the time is number of items in availableAssignments expected to be several times larger than number of threads
		/// </summary>
		private void Consume()
    {
      MT.InitThreadData ();

			while ( finishedAssignments < totalNumberOfAssignments )
      {
				Assignment newAssignment;
        availableAssignments.TryDequeue ( out newAssignment );

        if ( !Master.instance.progressData.Continue )
          return;

        newAssignment?.Render ();
      }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="bitmap">Main bitmap - used also in PictureBox in Form1</param>
		/// <param name="scene">Scene to render</param>
		/// <param name="renderer">Rendered to use for RenderPixel method</param>
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

          Assignment newAssignment = new Assignment ( bitmap, scene, renderer, localX, localY, localX + assignmentSize - 1, localY + assignmentSize - 1 );
          availableAssignments.Enqueue ( newAssignment );
        }
      }


      totalNumberOfAssignments = availableAssignments.Count;
      assignmentRoundsTotal = totalNumberOfAssignments * 4;
    }

    /// <summary>
		/// Goes through all clients from RenderClientsForm and assigns a NetworkWorker to each of them
		/// </summary>
    public void AssignNetworkWorkerToStream ()
    {
      foreach ( Client client in RenderClientsForm.instance.clients )
      {
        NetworkWorker newWorker = new NetworkWorker ( client.address );

        if ( !newWorker.ConnectToClient () )  // removes NetworkWorker instance in case of failure to connect to the client
        {
					newWorker = null;
        }
      }
    }

    /// <summary>
		/// Adds colors represented in newBitmap array to main bitmap
		/// </summary>
		/// <param name="newBitmap">Float values (for later HDR support) representing pixel color values</param>
    public void BitmapMerger ( float[] newBitmap, int x1, int y1, int x2, int y2 )  //TODO: Change to unsafe?
    {     
      lock ( bitmap )
      {
        int arrayPosition = 0;

				for ( int y = y1; y < y2; y++ )
        {
          for ( int x = x1; x < x2; x++ )
          {
            Color color = Color.FromArgb ( (int) newBitmap [ arrayPosition ], 
                                           (int) newBitmap [ arrayPosition + 1 ],
                                           (int) newBitmap [ arrayPosition + 2 ] );
            bitmap.SetPixel ( x, y, color );

            arrayPosition += 3;
          }
        }
      }
    }
  }

  /// <summary>
	/// Takes care of network communication with with 1 render client
	/// </summary>
  class NetworkWorker
	{
		private IPAddress ipAdr;
		private IPEndPoint endPoint;
    private const int port = 5000;

	  private TcpClient client;
	  private NetworkStream stream;

		public NetworkWorker ( IPAddress ipAdr )
	  {
			this.ipAdr = ipAdr;
		}

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
	    if ( endPoint == null )
	    {
				endPoint = new IPEndPoint ( ipAdr, port );
			}

	    client = new TcpClient ();
	    try
	    {
	      client.Connect ( endPoint );
			}
	    catch ( SocketException e )
	    {
				return false;
	    }
	    
	    stream = client.GetStream ();

	    SendNecessaryObjects ();


			while ( true )
	    {
	      
	    }

			return true;
      //TODO: Do something with stream
	  }


	  public void TestReceive ()
	  {
	    while ( true )
	    {
	      byte[] buffer = new byte[client.ReceiveBufferSize];

	      stream.Read ( buffer, 0, buffer.Length );

	      string message = Encoding.UTF8.GetString ( buffer );

	      Debug.WriteLine ( message );

	      if ( message.Contains ( "." ) )
	      {
	        break;
	      }
	    }
		}

	  public void SendNecessaryObjects ()
	  {
	    //SendObject<IRayScene> ( Master.instance.scene );
	    SendObject<IRenderer> ( Master.instance.renderer );
		}

	  private void SendObject<T> (T objectToSend)
	  {
	    BinaryFormatter formatter = new BinaryFormatter ();
	    MemoryStream memoryStream = new MemoryStream ();

	    formatter.Serialize ( memoryStream, objectToSend );

	    byte[] sceneData = memoryStream.ToArray ();

	    client.SendBufferSize = sceneData.Length;

	    stream.Write ( sceneData, 0, sceneData.Length );
	  }
	}

	/// <summary>
	/// Represents 1 render work ( = rectangle of pixels to render at specific density)
	/// </summary>
	public class Assignment
  {
		internal Bitmap bitmap;
    internal IRayScene scene;
    internal int x1, y1, x2, y2;

    public int density; // Density of 'n' means that only each 'n'-th pixel is rendered (for sake of dynamic rendering)

    public IRenderer renderer;

    private readonly int bitmapWidth, bitmapHeight;

		public Assignment ( Bitmap bitmap, IRayScene scene, IRenderer renderer, int x1, int y1, int x2, int y2 )
    {
			this.bitmap = bitmap;
			this.scene = scene;
			this.renderer = renderer;
			this.x1 = x1;
			this.y1 = y1;
			this.x2 = x2;
			this.y2 = y2;

			// density values: 8 > 4 > 2 > 1; initially always 8
			// decreases at the end of rendering of current assignment and therefore makes another render of this assignment more detailed
      density = 8;

			bitmapWidth = bitmap.Width;
      bitmapHeight = bitmap.Height;
    }

    /// <summary>
		/// Main render method
		/// Directly writes pixel colors to the main bitmap after rendering them
		/// </summary>
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

          renderer.RenderPixel (x, y, color); // called at desired IRenderer; gets pixel color

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
									    bitmap.SetPixel ( i, j, c );  // actual set of pixel color to main bitmap
										}
									}
								}							
							}            
            }
          }

          
					lock ( Master.instance.progressData )
					{
						// test whether rendering should end (Stop button pressed) 
					  if ( !Master.instance.progressData.Continue )
					    return;

						// synchronization of bitmap with PictureBox in Form and update of progress (percentage of done work)
						if ( Master.instance.mainThread == Thread.CurrentThread )
						{
						  Master.instance.progressData.Finished = Master.instance.assignmentRoundsFinished / (float) Master.instance.assignmentRoundsTotal;
						  Master.instance.progressData.Sync ( bitmap );
						}					
					}
				}
			}


      if ( density == 1 )
      {
        Master.instance.finishedAssignments++;
        Master.instance.assignmentRoundsFinished ++;
			}
      else
      {
				density = density >> 1; // density values: 8 > 4 > 2 > 1
        Master.instance.assignmentRoundsFinished ++;
				Master.instance.availableAssignments.Enqueue ( this );
			}
		}
	}
}