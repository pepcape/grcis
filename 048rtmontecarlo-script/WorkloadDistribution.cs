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

    private List<NetworkWorker> networkWorkers;

    private Thread[] pool;

    public Thread mainThread;

    public int totalNumberOfAssignments;

    public int finishedAssignments;

    // width and height of one block of pixels (rendered at one thread at the time); 32 seems to be optimal; should be power of 2 and smaller than 8
    public const int assignmentSize = 32;

    public Progress progressData;

    public int assignmentRoundsFinished = 0;
    public int assignmentRoundsTotal;

    public Bitmap    bitmap;
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
      instance = this;

      finishedAssignments = 0;

      this.bitmap   = bitmap;
      this.scene    = scene;
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
    public void StartThreads ( object threads )
    {
      pool = new Thread[(int) threads];


      for ( int i = 0; i < (int) threads; i++ )
      {
        Thread newThread = new Thread ( Consume );
        newThread.Priority = ThreadPriority.AboveNormal;
        pool [ i ]         = newThread;
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
    private void Consume ()
    {
      MT.InitThreadData ();

      while ( finishedAssignments < totalNumberOfAssignments )
      {
        Assignment newAssignment;
        availableAssignments.TryDequeue ( out newAssignment );

        if ( !Master.instance.progressData.Continue )
          return;

        if ( newAssignment == null )
        {
          continue;
        }

        float[] colorArray = newAssignment.Render ();
        BitmapMerger ( colorArray, newAssignment.x1, newAssignment.y1, newAssignment.x2 + 1, newAssignment.y2 + 1 );
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

          Assignment newAssignment = new Assignment ( localX,
                                                      localY,
                                                      localX + assignmentSize - 1,
                                                      localY + assignmentSize - 1,
                                                      bitmap.Width, 
                                                      bitmap.Height );
          availableAssignments.Enqueue ( newAssignment );
        }
      }


      totalNumberOfAssignments = availableAssignments.Count;
      assignmentRoundsTotal    = totalNumberOfAssignments * 4;
    }

    /// <summary>
    /// Goes through all clients from RenderClientsForm and assigns a NetworkWorker to each of them
    /// </summary>
    public void AssignNetworkWorkerToStream ()
    {
      foreach ( Client client in RenderClientsForm.instance.clients )
      {
        NetworkWorker newWorker = new NetworkWorker ( client.address );
        networkWorkers = new List<NetworkWorker> ();

        if ( !newWorker.ConnectToClient () ) // removes NetworkWorker instance in case of failure to connect to the client
        {
          newWorker = null;
        }
        else
        {
          networkWorkers.Add ( newWorker );
          newWorker.SendNecessaryObjects ();
        }
      }
    }

    /// <summary>
    /// Adds colors represented in newBitmap array to main bitmap
    /// </summary>
    /// <param name="newBitmap">Float values (for later HDR support) representing pixel color values</param>
    public void BitmapMerger ( float[] newBitmap, int x1, int y1, int x2, int y2 ) //TODO: Change to unsafe?
    {
      lock ( bitmap )
      {
        int arrayPosition = 0;

        for ( int y = y1; y < y2; y++ )
        {
          for ( int x = x1; x < x2; x++ )
          {
            if ( !float.IsInfinity ( newBitmap[arrayPosition] ) )
            {
              Color color = Color.FromArgb ( (int) newBitmap [ arrayPosition ],
                                             (int) newBitmap [ arrayPosition + 1 ],
                                             (int) newBitmap [ arrayPosition + 2 ] );
              bitmap.SetPixel ( x, y, color );
            }

            arrayPosition += 3;
          }
        }
      }
    }

    /// <summary>
    /// Goes through NetworkStreams of all clients and checks if there is rendered image pending in them
    /// Plus it takes care of that data via ReceiveRenderedImage method
    /// </summary>
    public void RenderedImageReceiver ()
    {
      while ( finishedAssignments < totalNumberOfAssignments )
      {
        foreach ( NetworkWorker worker in networkWorkers )
        {
          worker.ReceiveRenderedImage ();
        }
      }
    }
  }


  /// <summary>
  /// Takes care of network communication with with 1 render client
  /// </summary>
  class NetworkWorker
  {
    private       IPAddress  ipAdr;
    private       IPEndPoint endPoint;
    private const int        port = 5000;

    private TcpClient     client;
    private NetworkStream stream;

    public NetworkWorker ( IPAddress ipAdr )
    {
      this.ipAdr = ipAdr;
    }

    /// <summary>
    /// Establishes NetworkStream with desired client
    /// </summary>
    /// <returns>True if connection was succesfull, False otherwise</returns>
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

      return true;
      //TODO: Do something with stream
    }

    /// <summary>
    /// For DEBUG only
    /// </summary>
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

    /// <summary>
    /// Sends all objects which are necessary to render scene to client
    /// IRayScene - scene representation like solids, textures, lights, camera, ...
    /// IRenderer - renderer itself including IImageFunction; needed for RenderPixel method
    /// </summary>
    public void SendNecessaryObjects ()
    {
      SendObject<IRayScene> ( Master.instance.scene );
      WaitForConfirmation ();
      SendObject<IRenderer> ( Master.instance.renderer );
      WaitForConfirmation ();
    }

    /// <summary>
    /// For DEBUG purposes and to remove some problems with NetworkStream
    /// Waits until byte "1" is not received
    /// </summary>
    private void WaitForConfirmation ()
    {
      byte receivedData;

      do
      {
        receivedData = (byte) stream.ReadByte ();
      } while ( receivedData != 1 );
    }

    /// <summary>
    /// Serializes and sends object through NetworkStream
    /// </summary>
    /// <typeparam name="T">Type of object - must be marked as [Serializable], as well as all his recursive dependencies</typeparam>
    /// <param name="objectToSend">Instance of actual object to send</param>
    private void SendObject<T> ( T objectToSend )
    {
      BinaryFormatter formatter    = new BinaryFormatter ();
      MemoryStream    memoryStream = new MemoryStream ();

      formatter.Serialize ( memoryStream, objectToSend );

      byte[] dataBuffer = memoryStream.ToArray ();

      client.SendBufferSize = dataBuffer.Length;

      stream.Write ( dataBuffer, 0, dataBuffer.Length );
    }

    /// <summary>
    /// Receives object from NetworkStream and deserializes it
    /// </summary>
    /// <typeparam name="T">Type of object - must be marked as [Serializable], as well as all his recursive dependencies</typeparam>
    /// <returns>Instance of actual received object</returns>
    public T ReceiveObject<T> ()
    {
      byte[] dataBuffer = new byte[client.ReceiveBufferSize];

      stream.Read ( dataBuffer, 0, dataBuffer.Length );

      MemoryStream    memoryStream = new MemoryStream ( dataBuffer );
      BinaryFormatter formatter    = new BinaryFormatter ();
      memoryStream.Position = 0;

      T receivedObject = (T) formatter.Deserialize ( memoryStream );

      return receivedObject;
    }

    private const int bufferSize = ( Master.assignmentSize * Master.assignmentSize + 2) * sizeof ( float );
    /// <summary>
    /// Checks whether there is any data in NetworkStream to read
    /// If so, it reads it as - expected format is array of floats
    ///   - first 2 floats represents x1 and y1 coordinates - position in main bitmap;
    ///   - rest of array are colors of rendered bitmap - 3 floats (RGB values) per pixel;
    ///   - stored per lines from left upper corner (coordinates position)
    /// </summary>
    public void ReceiveRenderedImage ()
    {
      if ( stream.DataAvailable )
      {
        byte[] buffer = new byte[bufferSize];
        int[] coordinates = new int[2];

        stream.Read ( buffer, 0, bufferSize );

        float[] floatBuffer = new float[Master.assignmentSize * Master.assignmentSize];
        Buffer.BlockCopy ( buffer, 2 * sizeof ( float ), floatBuffer, 0, floatBuffer.Length );
        Buffer.BlockCopy ( buffer, 0, coordinates, 0, coordinates.Length );

        Master.instance.BitmapMerger ( floatBuffer, 
                                       coordinates [ 0 ], 
                                       coordinates [ 1 ],
                                       coordinates [ 0 ] + Master.assignmentSize,
                                       coordinates [ 1 ] + Master.assignmentSize );

        Master.instance.finishedAssignments++;
        AskForNewAssignment();
      }
    }    

    private void AskForNewAssignment ()
    {
      throw new NotImplementedException ();
    }
  }


  /// <summary>
  /// Represents 1 render work ( = rectangle of pixels to render at specific stride)
  /// </summary>
  [Serializable]
  public class Assignment
  {
    internal int x1, y1, x2, y2;

    public int stride; // Density of 'n' means that only each 'n'-th pixel is rendered (for sake of dynamic rendering)

    private readonly int bitmapWidth, bitmapHeight;

    public Assignment ( int x1, int y1, int x2, int y2, int bitmapWidth, int bitmapHeight )
    {
      this.x1 = x1;
      this.y1 = y1;
      this.x2 = x2;
      this.y2 = y2;
      this.bitmapWidth = bitmapWidth;
      this.bitmapHeight = bitmapHeight;

      // stride values: 8 > 4 > 2 > 1; initially always 8
      // decreases at the end of rendering of current assignment and therefore makes another render of this assignment more detailed
      stride = 8;   
    }

    /// <summary>
    /// Main render method
    /// Directly writes pixel colors to the main bitmap after rendering them
    /// </summary>
    public float[] Render ()
    {
      float[] returnArray = new float[Master.assignmentSize * Master.assignmentSize * 3];

      for ( int y = y1; y <= y2; y += stride )
      {
        for ( int x = x1; x <= x2; x += stride )
        {
          if ( stride != 8 && ( y % ( stride << 1 ) == 0 ) && ( x % ( stride << 1 ) == 0 ) ) // prevents rendering of already rendered pixels
          {
            returnArray[PositionInArray ( x, y )]     = float.PositiveInfinity;
            returnArray[PositionInArray ( x, y ) + 1] = float.PositiveInfinity;
            returnArray[PositionInArray ( x, y ) + 2] = float.PositiveInfinity;
            continue;
          }

          if ( x >= bitmapWidth || y >= bitmapHeight )
          {
            continue;
          }

          double[] color = new double[3];

          Master.instance.renderer.RenderPixel ( x, y, color ); // called at desired IRenderer; gets pixel color

          if ( stride == 1 )
          {
            returnArray [ PositionInArray ( x, y ) ] = (float) ( color [ 0 ] * 255.0 );
            returnArray [ PositionInArray ( x, y ) + 1 ] = (float) ( color [ 1 ] * 255.0 );
            returnArray [ PositionInArray ( x, y ) + 2 ] = (float) ( color [ 2 ] * 255.0 );
          }
          else
          {
            for ( int j = y; j < y + stride; j++ )
            {
              if ( j < bitmapHeight )
              {
                for ( int i = x; i < x + stride; i++ )
                {
                  if ( i < bitmapWidth )
                  {
                    // actual set of pixel color to main bitmap
                    returnArray [ PositionInArray ( i, j ) ] = (float) ( color [ 0 ] * 255.0 );
                    returnArray [ PositionInArray ( i, j ) + 1 ] = (float) ( color [ 1 ] * 255.0 );
                    returnArray [ PositionInArray ( i, j ) + 2 ] = (float) ( color [ 2 ] * 255.0 );
                  }
                }
              }
            }
          }


          lock ( Master.instance.progressData )
          {
            // test whether rendering should end (Stop button pressed) 
            if ( !Master.instance.progressData.Continue )
              return returnArray;

            // synchronization of bitmap with PictureBox in Form and update of progress (percentage of done work)
            if ( Master.instance.mainThread == Thread.CurrentThread )
            {
              Master.instance.progressData.Finished =
                Master.instance.assignmentRoundsFinished / (float) Master.instance.assignmentRoundsTotal;
              Master.instance.progressData.Sync ( Master.instance.bitmap );
            }
          }
        }
      }


      if ( stride == 1 )
      {
        Master.instance.finishedAssignments++;
        Master.instance.assignmentRoundsFinished++;
      }
      else
      {
        stride = stride >> 1; // stride values: 8 > 4 > 2 > 1
        Master.instance.assignmentRoundsFinished++;
        Master.instance.availableAssignments.Enqueue ( this );
      }

      return returnArray;
    }

    /// <summary>
    /// Computes position of coordinates (bitmap) into one-dimensional array of floats (with 3 floats (RGB channels) per pixel)
    /// </summary>
    /// <param name="x">X coordinate in bitmap</param>
    /// <param name="y">Y coordinate in bitmap</param>
    /// <returns>Position in array of floats</returns>
    private int PositionInArray ( int x, int y )
    {
      return ( ( y - y1 ) * Master.assignmentSize + ( x - x1 ) ) * 3;
    }
  }
}