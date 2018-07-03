using System;
using System.Collections.Concurrent;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Rendering;

namespace RenderClient
{
  class Program
  {
    static void Main ( string[] args )
    {
      Console.SetWindowSize ( Math.Min ( 85, Console.LargestWindowWidth ),
                              Math.Min ( 50, Console.LargestWindowHeight ) );

      try
      {
        RenderClient.ConnectToServer ();
      }
      catch ( SocketException e )
      {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine ( "\nYou can not start more than one client on one computer." );

        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine ( "\nPress any key to exit this RenderClient...\n" );
        Console.ReadLine ();

        Console.ReadLine ();
        return;
      }
      
      RenderClient.ReceiveNecessaryObjects ();

      Thread receiver = new Thread ( RenderClient.ReceiveAssignments );
      receiver.Name = "AssignmentReceiver";
      receiver.Priority = ThreadPriority.BelowNormal;
      receiver.Start ();

      RenderClient.ClientMaster.instance.StartThreads ( 1 ); //TODO: change to "Environment.ProcessorCount"

      Console.ForegroundColor = ConsoleColor.White;
      Console.WriteLine ( "\nPress any key to exit this RenderClient...\n" );
      Console.ReadLine ();
    }
  }

  /// <summary>
  /// Takes care of connection and communication with server
  /// </summary>
  static class RenderClient
  {
    private const  int           port = 5000;
    private static NetworkStream stream;
    private static TcpClient     client;
    private static bool          finished = false;

    private static IRayScene scene;
    private static IRenderer renderer;

    /// <summary>
    /// TcpListener is used even though this is in fact a client, not a server - this is done so that the real server can connect to the client
    /// (and not otherwise as usually)
    /// </summary>
    public static void ConnectToServer ()
    {
      Console.ForegroundColor = ConsoleColor.White;
      Console.WriteLine ( @"Waiting for remote server to connect to this client..." );

      TcpListener localServer = new TcpListener ( IPAddress.Loopback, port );
      localServer.Start ();

      do
      {
        client = localServer.AcceptTcpClient ();

        stream = client.GetStream ();
      } while ( stream == null );

      client.ReceiveBufferSize = 1024 * 1024 * 32;
      client.SendBufferSize = 1024 * 1024 * 32;
      //client.NoDelay = true;

      Console.WriteLine ( @"Client succesfully connected." );
    }

    /// <summary>
    /// Receives all objects which are necessary from render server
    /// IRayScene - scene representation like solids, textures, lights, camera, ...
    /// IRenderer - renderer itself including IImageFunction; needed for RenderPixel method
    /// </summary>
    public static void ReceiveNecessaryObjects ()
    {     
      scene = NetworkSupport.ReceiveObject<IRayScene> ( client, stream );
      Console.ForegroundColor = ConsoleColor.Green;
      Console.WriteLine ( "\nData for {0} received and deserialized.", typeof ( IRayScene ).Name );


      renderer = NetworkSupport.ReceiveObject<IRenderer> ( client, stream );
      Console.ForegroundColor = ConsoleColor.Green;
      Console.WriteLine ( "Data for {0} received and deserialized.\n", typeof ( IRenderer ).Name );

      ClientMaster.instance = new ClientMaster ( null, scene, renderer );
    }

    /// <summary>
    /// Thread in infinite loop accepting new assignments
    /// </summary>
    public static void ReceiveAssignments ()
    {
      while ( true )  //TODO: properly end
      {
        if ( stream.DataAvailable )
        {
          Assignment newAssignment = NetworkSupport.ReceiveObject<Assignment> ( client, stream );         

          if ( newAssignment.x1 == -1 ) // Ending assignment represented by -1 in x1 variable signals end of rendering
          {
            finished = true;

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine ( "\nRendering on server finished or no more assignments are expected to be received.\n" );

            return;
          }

          Console.ForegroundColor = ConsoleColor.Green;
          Console.WriteLine ( @"Data for assignment [{0}, {1}, {2}, {3}] received and deserialized.", newAssignment.x1, newAssignment.y1, newAssignment.x2, newAssignment.y2 );

          ClientMaster.instance.availableAssignments.Enqueue ( newAssignment );
        }
      }
    }

    private const int bufferSize = ( Master.assignmentSize * Master.assignmentSize * 3 + 2 ) * sizeof ( float );
    /// <summary>
    /// Encodes and sends rendered image
    /// Format:
    ///   - array of floats where first 2 floats are coordinates x1 and y1 (left upper corner in main bitmap)
    ///   - rest of the floats are colors of pixels per rows (3 floats per 1 pixel - RGB channels)
    /// </summary>
    public static void SendRenderedImage ( float[] colorBuffer, int x1, int y1 )
    {
      float[] coordinates    = { x1, y1 };
      float[] combinedBuffer = new float[coordinates.Length + colorBuffer.Length];
      coordinates.CopyTo ( combinedBuffer, 0 );                  // copy coordinates to the first 2 floats in combinedBuffer
      colorBuffer.CopyTo ( combinedBuffer, coordinates.Length ); // copy colors to the rest of combinedBuffer

      byte[] sendBuffer = new byte[combinedBuffer.Length * sizeof ( float )];

      Buffer.BlockCopy ( combinedBuffer, 0, sendBuffer, 0, sendBuffer.Length );

      lock ( stream )
      {
        stream.Write ( sendBuffer, 0, bufferSize );
      }
    }


    public class ClientMaster: Master
    {
      public new static ClientMaster instance; //singleton

      private Thread[] pool;

      public ClientMaster ( Bitmap bitmap, IRayScene scene, IRenderer renderer ) : base ( bitmap, scene, renderer )
      {
        availableAssignments = new ConcurrentQueue<Assignment> ();

        this.scene = scene;
        this.renderer = renderer;
      }

      /// <summary>
      /// Creates threadpool and starts all threads on Consume method
      /// </summary>
      /// <param name="threads">Number of threads to be used for rendering</param>
      public new void StartThreads ( int threads )
      {
        pool = new Thread[threads];

        for ( int i = 0; i < threads; i++ )
        {
          Thread newThread = new Thread ( Consume );
          newThread.Name = "RenderThread" + i;
          newThread.Priority = ThreadPriority.AboveNormal;
          pool[i]            = newThread;
          newThread.Start ();
        }

        mainThread = pool[0];

        for ( int i = 0; i < (int) threads; i++ )
        {
          pool[i].Join ();
          pool[i] = null;
        }
      }

      /// <summary>
      /// Consumer-producer based multithreading work distribution
      /// Each thread waits for a new Assignment to be added to availableAssignments queue
      /// Most of the time is number of items in availableAssignments expected to be several times larger than number of threads
      /// </summary>
      protected new void Consume ()
      {
        MT.InitThreadData ();

        while ( !finished || !availableAssignments.IsEmpty )
        {
          Assignment newAssignment;
          availableAssignments.TryDequeue ( out newAssignment );

          if ( newAssignment == null ) // TryDequeue was not succesfull
            continue;

          Console.ForegroundColor = ConsoleColor.DarkYellow;
          Console.WriteLine ( @"Start of rendering of assignment [{0}, {1}, {2}, {3}]", newAssignment.x1, newAssignment.y1, newAssignment.x2, newAssignment.y2 );

          float[] colorArray = newAssignment.Render ( true, renderer );

          Console.ForegroundColor = ConsoleColor.Yellow;
          Console.WriteLine ( @"Rendering of assignment [{0}, {1}, {2}, {3}] finished. Sending result to server.", newAssignment.x1, newAssignment.y1, newAssignment.x2, newAssignment.y2 );


          SendRenderedImage ( colorArray, newAssignment.x1, newAssignment.y1 );

          Console.ForegroundColor = ConsoleColor.Yellow;
          Console.WriteLine ( @"Result of assignment [{0}, {1}, {2}, {3}] sent.", newAssignment.x1, newAssignment.y1, newAssignment.x2, newAssignment.y2 );
        }
      }

      public new void AssignNetworkWorkerToStream () { }  // initially left blank
      public new void InitializeAssignments ( Bitmap bitmap, IRayScene scene, IRenderer renderer ) { } // initially left blank
    }    
  }
}