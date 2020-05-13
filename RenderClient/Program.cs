using System;
using System.Collections.Concurrent;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Open.Nat;
using Rendering;

namespace RenderClient
{
  class Program
  {
    static void Main ( string[] args )
    {
      Console.SetWindowSize ( Math.Min ( 85, Console.LargestWindowWidth ),
                              Math.Min ( 50, Console.LargestWindowHeight ) );

      Console.Title = "Render Client";

      RenderClient.Start ();
    }
  }

  /// <summary>
  /// Takes care of connection and communication with server
  /// </summary>
  internal static class RenderClient
  {
    private const  int           port = 5411; // randomly chosen port - should be above 5000 and with bit of luck, it will not interfere with any other prorgam
    private static NetworkStream stream;
    private static TcpClient     client;
    private static bool          finished = false;  // indication for Consume method that rendering of everything is finished and there will be no more assignments

    private static IRayScene scene;
    private static IRenderer renderer;

    private static readonly int threadCount;

    private static readonly object consoleLock = new object();

    static RenderClient ()
    {
#if DEBUG
      threadCount = 1;
#else
      threadCount = Environment.ProcessorCount;
#endif
    }

    /// <summary>
    /// TcpListener is used even though this is in fact a client, not a server - this is done so that the real server can connect to the client
    /// (and not otherwise as usually)
    /// </summary>
    private static void ConnectToServer ()
    {
      //NetworkSupport.NecessaryNetworkSettings ( port, Protocol.Tcp, "RenderClient (TCP)");  //sets port-forwarding

      if ( stream != null ) // connection already exists
        return;

      TcpListener localServer = new TcpListener ( IPAddress.Any, port );
      localServer.Start ( 1 );

      do // loop to accept connection from render server (which in this case acts as "client" in TCP/IP terminology)
      {
        client = localServer.AcceptTcpClient ();

        stream = client.GetStream ();
      } while ( stream == null );

      client.ReceiveBufferSize = 1024 * 1024; // needed just in case - large portions of data are expected to be transfered at the same time (one rendered assignment is 50kB)
      client.SendBufferSize = 1024 * 1024;
    }


    private static bool resetAssignmentAlreadyReceived = false;
    /// <summary>
    /// Main start of RenderClient
    /// The only method needed to be called from outside
    /// </summary>
    public static void Start ()
    {
      while ( true )  // this loop is exited by Environment.Exit ( 1 ) in EndOfRenderClientWork ()
      {
        try
        {
          //displays external and internal IP addresses for easier connection establishment
          Console.ForegroundColor = ConsoleColor.White;
          Console.WriteLine ( "Your external IP address is: " + NetworkSupport.GetExternalIPAddress () );
          Console.WriteLine ( "Your local IP address is: " + NetworkSupport.GetLocalIPAddress () + '\n' );

          Console.WriteLine ( @"Waiting for remote server to connect to this client..." );

          ConnectToServer ();

          Console.WriteLine ( @"Client succesfully connected." );
        }
        catch ( SocketException ) // thrown in case of multiple clients being launched on the same computer
        {
          Console.ForegroundColor = ConsoleColor.Red;
          Console.WriteLine ( "\nYou can not start more than one client on the same computer." );

          Console.ForegroundColor = ConsoleColor.White;
          Console.WriteLine ( "\nPress any key to exit this RenderClient...\n" );

          Console.ReadKey ();
          return;
        }

        finished = false;

        ExchangeNecessaryInfo ();

        Thread receiver = new Thread ( ReceiveAssignments );
        receiver.Name = "AssignmentReceiver";
        receiver.Priority = ThreadPriority.BelowNormal;
        receiver.Start ();

        ClientMaster.singleton.StartThreads ( threadCount );

        EndOfRenderClientWork ();
      }
    }

    /// <summary>
    /// Takes care of end of RenderClient work
    /// Offers user to either exit or wait for new work from server
    /// </summary>
    /// <returns>True if RenderClient should be reset; False if RenderClient should exit</returns>
    private static void EndOfRenderClientWork ()
    {
      CancellationTokenSource source = new CancellationTokenSource();
      CancellationToken token = source.Token;

      Task exit = new Task( () =>
      {
        Console.WriteLine ( "Press ESC to exit or wait for new render work from server." );
        while ( Console.ReadKey ().Key != ConsoleKey.Escape ) ;
        Environment.Exit ( 1 );
      }, token);

      exit.Start ();


      WaitForResetAssignment ();

      source.Cancel ();

      Console.Clear ();
    }


    /// <summary>
    /// Receives all objects which are necessary from render server
    /// IRayScene - scene representation like solids, textures, lights, camera, ...
    /// IRenderer - renderer itself including IImageFunction; needed for RenderPixel method
    /// Sends number of threads available at client to render
    /// </summary>
    private static void ExchangeNecessaryInfo ()
    {
      if ( resetAssignmentAlreadyReceived ) // if reset assignment was received in WaitForResetAssignment, skip it's receival here
      {
        resetAssignmentAlreadyReceived = false;
      }
      else
      {
        SetAssembliesNames ();

        Assignment assignment = NetworkSupport.ReceiveObject<Assignment> ( stream );

        if ( assignment.type != Assignment.AssignmentType.Reset )
        {
          throw new Exception ( $"Received assignment with {assignment.type}. {Assignment.AssignmentType.Reset} expected." );
        }
      }

      scene = NetworkSupport.ReceiveObject<IRayScene> ( stream );
      Console.ForegroundColor = ConsoleColor.Green;
      Console.WriteLine ( "\nData for {0} received and deserialized.", typeof ( IRayScene ).Name );


      renderer = NetworkSupport.ReceiveObject<IRenderer> ( stream );
      Console.ForegroundColor = ConsoleColor.Green;
      Console.WriteLine ( "Data for {0} received and deserialized.\n", typeof ( IRenderer ).Name );

      NetworkSupport.SendInt ( stream, threadCount );

      ClientMaster.singleton = new ClientMaster ( null, scene, renderer );
    }


    /// <summary>
    /// Thread in infinite loop accepting new assignments
    /// Loop is ended by receiving Ending Assignment
    /// </summary>
    private static void ReceiveAssignments ()
    {
      while ( true )
      {
        try
        {
          if (stream.DataAvailable)
          {
            Assignment newAssignment = NetworkSupport.ReceiveObject<Assignment>(stream);

            if (newAssignment.type == Assignment.AssignmentType.Ending) // Ending assignment signals end of rendering
            {
              finished = true; // signal for threads in Consume method

              lock (consoleLock)
              {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\nRendering on server finished or no more assignments are expected to be received.\n");
              }

              return;
            }

            lock (consoleLock)
            {
              Console.ForegroundColor = ConsoleColor.Green;
              Console.WriteLine(@"Data for assignment [{0}, {1}, {2}, {3}] received and deserialized.", newAssignment.x1, newAssignment.y1, newAssignment.x2, newAssignment.y2);
            }

            ClientMaster.singleton.availableAssignments.Enqueue(newAssignment); // adds new assignment to the queue; it is later taken from there by thread in Consume method
          }
                }
        catch ( ObjectDisposedException )
        {
          ConnectionLost ();
        }
      }
    }


    /// <summary>
    /// Used by other thread to wait for receival of special Reset-type assignment
    /// </summary>
    private static void WaitForResetAssignment ()
    {
      stream = null;
      ConnectToServer ();

      if ( stream == null )
      {
        return;
      }

      while ( true )
      {
        if ( stream.DataAvailable )
        {
          SetAssembliesNames ();

          Assignment newAssignment = NetworkSupport.ReceiveObject<Assignment> ( stream );

          if ( newAssignment.type == Assignment.AssignmentType.Reset ) // Reset assignment signals that RenderClient should expect new render work
          {
            resetAssignmentAlreadyReceived = true;

            return;
          }
        }
      }
    }

    /// <summary>
    /// Sets sender and receiver assemblies in NetworkSupport - needed for correct serialization/deserialization
    /// </summary>
    private static void SetAssembliesNames ()
    {
      string senderAssembly = NetworkSupport.ReceiveString ( stream );
      NetworkSupport.SendString ( Assembly.GetExecutingAssembly ().GetName ().Name, stream );
      NetworkSupport.SetAssemblyNames ( senderAssembly, Assembly.GetExecutingAssembly ().GetName ().Name );
    }


    private const int bufferSize = ( Master.assignmentSize * Master.assignmentSize * 3 + 2 ) * sizeof ( float );
    /// <summary>
    /// Encodes and sends rendered image
    /// Format:
    ///   - array of floats where first 2 floats are coordinates x1 and y1 (left upper corner in main bitmap)
    ///   - rest of the floats are colors of pixels per rows (3 floats per 1 pixel - RGB channels)
    /// </summary>
    private static void SendRenderedImage ( float[] colorBuffer, int x1, int y1 )
    {
      float[] coordinates    = { x1, y1 };
      float[] combinedBuffer = new float[coordinates.Length + colorBuffer.Length];
      coordinates.CopyTo ( combinedBuffer, 0 );                  // copy coordinates to the first 2 floats in combinedBuffer
      colorBuffer.CopyTo ( combinedBuffer, coordinates.Length ); // copy colors to the rest of combinedBuffer

      byte[] sendBuffer = new byte[combinedBuffer.Length * sizeof ( float )];

      Buffer.BlockCopy ( combinedBuffer, 0, sendBuffer, 0, sendBuffer.Length );

      lock ( stream )
      {
        try
        {
          stream.Write ( sendBuffer, 0, bufferSize );
        }
        catch ( IOException )
        {
          ConnectionLost ();
        }
      }
    }

    /// <summary>
    /// Notifies user about lost connection and exits application
    /// </summary>
    private static void ConnectionLost ()
    {
      client.Close ();

      lock ( consoleLock )
      {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine ( "\nConnection to the server was lost.\n" );

        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine ( "Press any key to exit this RenderClient...\n" );
      }

      Console.ReadKey ();

      Environment.Exit ( 0 );
    }


    /// <summary>
    /// Takes care of distribution of work between local threads
    /// </summary>
    private class ClientMaster: Master
    {
      public new static ClientMaster singleton;

      private Thread[] pool;

      private static PointCloud dummy; // do not use; dummy object to pass to base constructor

      public ClientMaster ( Bitmap bitmap, IRayScene scene, IRenderer renderer ) : base ( bitmap, scene, renderer, null, 1, false, ref dummy )
      {
        availableAssignments = new ConcurrentQueue<Assignment> ();

        this.scene = scene;
        this.renderer = renderer;
      }

      /// <summary>
      /// Creates threadpool and starts all threads on Consume method
      /// </summary>
      /// <param name="threads">Number of threads to be used for rendering</param>
      public void StartThreads ( int threads )
      {
        pool = new Thread[threads];

        for ( int i = 0; i < threads; i++ )
        {
          Thread newThread = new Thread ( Consume );
          newThread.Name = "RenderThread" + i;
          pool[i]            = newThread;
          newThread.Start ();
        }

        mainRenderThread = pool[0];

        for ( int i = 0; i < threads; i++ )
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
      private new void Consume ()
      {
        MT.InitThreadData ();

        while ( !finished || !availableAssignments.IsEmpty )
        {
          availableAssignments.TryDequeue ( out Assignment newAssignment );

          if ( newAssignment == null ) // TryDequeue was not succesfull
            continue;

          lock ( consoleLock )
          {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine ( @"Start of rendering of assignment [{0}, {1}, {2}, {3}]", newAssignment.x1, newAssignment.y1, newAssignment.x2, newAssignment.y2 );
          }

          float[] colorArray = newAssignment.Render ( true, renderer );

          lock ( consoleLock )
          {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine ( @"Rendering of assignment [{0}, {1}, {2}, {3}] finished. Sending result to server.", newAssignment.x1, newAssignment.y1, newAssignment.x2, newAssignment.y2 );
          }

          SendRenderedImage ( colorArray, newAssignment.x1, newAssignment.y1 );

          lock ( consoleLock )
          {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine ( @"Result of assignment [{0}, {1}, {2}, {3}] sent.", newAssignment.x1, newAssignment.y1, newAssignment.x2, newAssignment.y2 );
          }
        }
      }

      private new void AssignNetworkWorkerToStream ()
      {
        throw new NotSupportedException (); // exception thrown because this method should not be used in context of ClientMaster
      }

      private new void InitializeAssignments ( Bitmap bitmap, IRayScene scene, IRenderer renderer )
      {
        throw new NotSupportedException (); // exception thrown because this method should not be used in context of ClientMaster
      }
    }
  }
}