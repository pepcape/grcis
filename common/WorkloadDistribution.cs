using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using MathSupport;

namespace Rendering
{
  /// <summary>
  /// Takes care of distribution of rendering work between local threads and remote/network render clients
  /// </summary>
  public class Master
  {
    public static Master instance; // singleton

    public ConcurrentQueue<Assignment> availableAssignments;

    public List<NetworkWorker> networkWorkers;

    private Thread[] pool;

    public Thread mainRenderThread;

    public int totalNumberOfAssignments;

    public int finishedAssignments;

    // width and height of one block of pixels (rendered at one thread at the time); 64 seems to be optimal; should be power of 2 and larger than 8
    public const int assignmentSize = 64;

    public Progress progressData;

    public int assignmentRoundsFinished;
    public int assignmentRoundsTotal;

    public Bitmap    bitmap;
    public IRayScene scene;
    public IRenderer renderer;

    private IEnumerable<Client> clientsCollection;

    /// <summary>
    /// Constructor which takes also care of initializing assignments
    /// </summary>
    /// <param name="bitmap">Main bitmap - used also in PictureBox in Form1</param>
    /// <param name="scene">Scene to render</param>
    /// <param name="renderer">Rendered to use for RenderPixel method</param>
    /// <param name="clientsCollection">Collection of Clients to get their IP addresses - names are only for user</param>
    public Master ( Bitmap bitmap, IRayScene scene, IRenderer renderer, IEnumerable<Client> clientsCollection )
    {
      finishedAssignments = 0;
   
      this.scene = scene;
      this.renderer = renderer;
      this.bitmap = bitmap;
      this.clientsCollection = clientsCollection;

      Assignment.assignmentSize = assignmentSize;
    }

    /// <summary>
    /// Creates threadpool and starts all threads on Consume method
    /// Thread which calls this method will take care of preparing assignments and receiving rendered images from RenderClients meanwhile
    /// </summary>
    /// <param name="threads">Number of threads to be used for rendering</param>
    public void StartThreads ( int threads )
    {
      pool = new Thread[threads];

      AssignNetworkWorkerToStream ();

      for ( int i = 0; i < threads; i++ )
      {
        Thread newThread = new Thread ( Consume );
        newThread.Name = "RenderThread #" + i;
        pool [ i ] = newThread;
        newThread.Start ();
      }

      mainRenderThread = pool [ 0 ];

      RenderedImageReceiver ();

      for ( int i = 0; i < threads; i++ )
      {
        pool [ i ].Join ();
        pool [ i ] = null;
      }

      
      if ( networkWorkers?.Count > 0 )
      {
        foreach ( NetworkWorker worker in networkWorkers ) // properly closes connections (and also sockets and streams) to all clients
        {
          worker.SendSpecialAssignment ( Assignment.AssignmentType.Ending );
          worker.client.Close ();
        }
      }   
    }

    /// <summary>
    /// Consumer-producer based multithreading work distribution
    /// Each thread waits for a new Assignment to be added to availableAssignments queue
    /// Most of the time is number of items in availableAssignments expected to be several times larger than number of threads
    /// </summary>
    protected void Consume ()
    {
      MT.InitThreadData ();

      while ( finishedAssignments < totalNumberOfAssignments )
      {
        Assignment newAssignment;
        availableAssignments.TryDequeue ( out newAssignment );

        if ( !progressData.Continue ) // test whether rendering should end (Stop button pressed) 
          return;

        if ( newAssignment == null ) // TryDequeue was not successful
          continue;

        float[] colorArray = newAssignment.Render ( false, renderer, progressData );
        BitmapMerger ( colorArray, newAssignment.x1, newAssignment.y1, newAssignment.x2 + 1, newAssignment.y2 + 1 );

        if ( newAssignment.stride == 1 )
        {
          finishedAssignments++;
          assignmentRoundsFinished++;
        }
        else
        {
          newAssignment.stride = newAssignment.stride >> 1; // stride values: 8 > 4 > 2 > 1
          assignmentRoundsFinished++;
          availableAssignments.Enqueue ( newAssignment );
        }
      }
    }

    /// <summary>
    /// Creates new assignments based on width and heigh of bitmap and assignmentSize
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
      if ( clientsCollection == null )
      {
        return;
      }

      networkWorkers = new List<NetworkWorker> ();

      foreach ( Client client in clientsCollection )
      {
        NetworkWorker newWorker = new NetworkWorker ( client.address );        

        if ( newWorker.ConnectToClient () )
        {
          networkWorkers.Add ( newWorker );

          newWorker.ExchangeNecessaryInfo ();

          for ( int i = 0; i < newWorker.threadCountAtClient; i++ )
          {
            newWorker.TryToGetNewAssignment ();
          }          

          //newWorker.SendEndingAssignment ();
        }
      }
    }

    /// <summary>
    /// Adds colors represented in colorBuffer array to main bitmap
    /// </summary>
    /// <param name="colorBuffer">Float values (possible to be used for HDR) representing pixel color values</param>
    public void BitmapMerger ( float[] colorBuffer, int x1, int y1, int x2, int y2 )
    {
      lock ( bitmap )
      {
        int arrayPosition = 0;

        for ( int y = y1; y < Math.Min ( y2, bitmap.Height ); y++ )
        {
          for ( int x = x1; x < x2; x++ )
          {
            if ( !float.IsInfinity ( colorBuffer [ arrayPosition ] ) && x < bitmap.Width )  // positive infinity is indicator that color for this pixel is already present in bitmap and is final
            {
              Color color = Color.FromArgb ( Math.Min ( (int) colorBuffer [ arrayPosition ], 255 ),
                                             Math.Min ( (int) colorBuffer [ arrayPosition + 1 ], 255 ),
                                             Math.Min ( (int) colorBuffer [ arrayPosition + 2 ], 255 ) );
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
      if ( networkWorkers == null || networkWorkers.Count == 0 )
      {
        return;
      }

      while ( finishedAssignments < totalNumberOfAssignments && progressData.Continue )
      {
        for ( int i = 0; i < networkWorkers.Count; i++ )
        {
          networkWorkers[i]?.ReceiveRenderedImage ();
        }
      }
    }
  }


  /// <summary>
  /// Takes care of network communication with with 1 render client
  /// </summary>
  public class NetworkWorker
  {
    private readonly IPAddress  ipAdr;
    private          IPEndPoint endPoint;
    private const    int        port = 5000;

    public TcpClient     client;
    public NetworkStream stream;

    public int threadCountAtClient;
    private static int assignmentsAtClients;

    private List<Assignment> unfinishedAssignments = new List<Assignment>();

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
      if ( ipAdr == null || ipAdr.ToString() == "0.0.0.0")
        return false;

      if ( endPoint == null )
        endPoint = new IPEndPoint ( ipAdr, port );

      client = new TcpClient ();

      try
      {
        client.Connect ( endPoint );
      }
      catch ( SocketException )
      {
        return false;
      }

      stream = client.GetStream ();

      client.ReceiveBufferSize = 1024 * 1024;  // needed just in case - large portions of data are expected to be transfered at the same time (one rendered assignment is 50kB)
      client.SendBufferSize    = 1024 * 1024;

      return true;
    }


    /// <summary>
    /// Sends all objects which are necessary to render scene to client
    /// Assignment - special type of assignment (reset) to indicate (new) start of work
    /// IRayScene - scene representation like solids, textures, lights, camera, ...
    /// IRenderer - renderer itself including IImageFunction; needed for RenderPixel method
    /// Receives number of threads available to render at RenderClient
    /// </summary>
    public void ExchangeNecessaryInfo ()
    {
      NetworkSupport.SetAssemblyNames ( Assembly.GetExecutingAssembly().GetName().Name, "RenderClient" );

      NetworkSupport.SendObject<Assignment> ( new Assignment ( Assignment.AssignmentType.Reset ), stream );

      NetworkSupport.SendObject<IRayScene> ( Master.instance.scene, stream );

      NetworkSupport.SendObject<IRenderer> ( Master.instance.renderer, stream );

      threadCountAtClient = NetworkSupport.ReceiveInt ( stream );
    }


    private const int bufferSize = ( Master.assignmentSize * Master.assignmentSize * 3 + 2) * sizeof ( float );
    /// <summary>
    /// Checks whether there is any data in NetworkStream to read
    /// If so, it reads it as - expected format is array of floats
    ///   - first 2 floats represents x1 and y1 coordinates - position in main bitmap;
    ///   - rest of array are colors of rendered bitmap - 3 floats (RGB values) per pixel;
    ///   - stored per lines from left upper corner (coordinates position)
    /// </summary>
    public void ReceiveRenderedImage ()
    {
      if ( !NetworkSupport.IsConnected ( client ) )
      {
        LostConnection ();        
        return;
      }

      if ( stream.DataAvailable )
      {
        byte [] tmp = new byte[1];

        stream.Write ( tmp, 0, 0 );

        int totalReceivedSize = 0;
        int leftToReceive = bufferSize;

        byte[] receiveBuffer = new byte[bufferSize];        

        while ( leftToReceive > 0 )  // Loop until enough data is received
        {
          int latestReceivedSize = stream.Read ( receiveBuffer, totalReceivedSize, leftToReceive );
          leftToReceive -= latestReceivedSize;
          totalReceivedSize += latestReceivedSize;
        }

        // Use parts of receiveBuffer - separate and convert data to coordinates and floats representing colors of pixels
        float[] coordinates = new float[2];
        float[] floatBuffer = new float[Master.assignmentSize * Master.assignmentSize * 3];
        Buffer.BlockCopy ( receiveBuffer, 0, coordinates, 0, 2 * sizeof ( float ) );
        Buffer.BlockCopy ( receiveBuffer, 2 * sizeof ( float ), floatBuffer, 0, floatBuffer.Length * sizeof ( float ) );

        Master.instance.BitmapMerger ( floatBuffer, 
                                       (int) coordinates [ 0 ],
                                       (int) coordinates [ 1 ],
                                       (int) coordinates [ 0 ] + Master.assignmentSize,
                                       (int) coordinates [ 1 ] + Master.assignmentSize );

        Master.instance.finishedAssignments++;
        assignmentsAtClients--;

        RemoveAssignmentFromUnfinishedAssignments ( (int) coordinates[0], (int) coordinates[1] );

        TryToGetNewAssignment();
      }
    }

    /// <summary>
    /// Takes care of flushing unfinished assignments back to availableAssignments queue in Master,
    /// removes this NetworkWorker from networkWorkers list and properly closes TCP connection
    /// Useful in case of lost connection to the client
    /// </summary>
    private void LostConnection ()
    {
      ResetUnfinishedAssignments ();
      Master.instance.networkWorkers.Remove ( this );
      client.Close ();
    }

    /// <summary>
    /// Removes assignments identified by its coordinates of left upper corner (x1 and y1) from unfinishedAssignments list
    /// </summary>
    /// <param name="x">Compared to value of x1 in assignment</param>
    /// <param name="y">Compared to value of y1 in assignment</param>
    private void RemoveAssignmentFromUnfinishedAssignments ( int x, int y )
    {
      for ( int i = 0; i < unfinishedAssignments.Count; i++ )
      {
        if ( unfinishedAssignments[i].x1 == x && unfinishedAssignments[i].y1 == y ) // assignments are uniquely indentified by coordinates of left upper corner
        {
          unfinishedAssignments.RemoveAt ( i );
          return;
        }
      }

      unfinishedAssignments.Clear ();
    }

    /// <summary>
    /// Loops until it gets a new assignment and sends it to the RenderClient (or all assignments have been rendered)
    /// </summary>
    public void TryToGetNewAssignment ()
    {      
      Assignment newAssignment = null;

      while ( Master.instance.finishedAssignments < Master.instance.totalNumberOfAssignments - assignmentsAtClients )
      {
        if ( !NetworkSupport.IsConnected ( client ) )
        {
          LostConnection ();
          return;
        }         

        Master.instance.availableAssignments.TryDequeue ( out newAssignment );

        if ( !Master.instance.progressData.Continue ) // test whether rendering should end (Stop button pressed) 
          return;

        if ( newAssignment == null ) // TryDequeue was not succesfull
          continue;

        lock ( stream )
        {
          NetworkSupport.SendObject<Assignment> ( newAssignment, stream );
          assignmentsAtClients++;
          unfinishedAssignments.Add ( newAssignment );
        }
        
        break;
      }
    }

    /// <summary>
    /// Moves all unfinished assignments cached in local unfinishedAssignments list to the main availableAssignments queue
    /// Useful in case of lost connection to the client
    /// </summary>
    private void ResetUnfinishedAssignments ()
    {
      foreach ( Assignment unfinishedAssignment in unfinishedAssignments )
      {
        Master.instance.availableAssignments.Enqueue ( unfinishedAssignment );
      }     
    }

    /// <summary>
    /// Sends special Assignment which can signal to client that rendering stopped or that client should be reset and wait for more work 
    /// </summary>
    public void SendSpecialAssignment ( Assignment.AssignmentType assignmentType )
    {
      Assignment newAssignment = new Assignment ( assignmentType );

      lock ( stream )
      {
        NetworkSupport.SendObject<Assignment> ( newAssignment, stream );
      }
    }
  }


  /// <summary>
  /// Represents 1 render work (= square of pixels to render at specific stride)
  /// </summary>
  [Serializable]
  public class Assignment
  {
    public int x1, y1, x2, y2;

    public static int assignmentSize;

    public int stride; // stride of 'n' means that only each 'n'-th pixel is rendered (for sake of dynamic rendering)

    private readonly int bitmapWidth, bitmapHeight;

    public AssignmentType type;

    /// <summary>
    /// Main constructor for standard assignments
    /// </summary>
    public Assignment ( int x1, int y1, int x2, int y2, int bitmapWidth, int bitmapHeight)
    {
      this.x1 = x1;
      this.y1 = y1;
      this.x2 = x2;
      this.y2 = y2;
      this.bitmapWidth = bitmapWidth;
      this.bitmapHeight = bitmapHeight;
      this.type = AssignmentType.Standard;

      // stride values: 8 > 4 > 2 > 1; initially always 8
      // decreases at the end of rendering of current assignment and therefore makes another render of this assignment more detailed
      stride = 8;   
    }

    /// <summary>
    /// Constructor used for special assignments with AssignmentType other than Standard
    /// Special assignments are used to indicate end of rendering, request to reset render client, ...
    /// Coordinates, dimensions and stride are irrelevant in this case - they are set to -1 for error checking
    /// </summary>
    /// <param name="type"></param>
    public Assignment ( AssignmentType type )
    {
      x1 = y1 = x2 = y2 = bitmapWidth = bitmapHeight = stride = -1;

      this.type = type;
    }

    /// <summary>
    /// Main render method
    /// </summary>
    /// <param name="renderEverything">True if you want to ignore stride and just render everything at once (removes dynamic rendering effect; distributed network rendering)</param>
    /// <param name="renderer">IRenderer which will be used for RenderPixel method</param>
    /// <param name="progressData">Used for sync of bitmap with main PictureBox</param>
    /// <returns>Float array which represents colors of pixels (3 floats per pixel - RGB channel)</returns>
    public float[] Render ( bool renderEverything, IRenderer renderer, Progress progressData = null )
    {
      float[] returnArray = new float[assignmentSize * assignmentSize * 3];

      int previousStride = stride;

      if ( renderEverything )
      {
        stride = 1;

        if ( previousStride == stride )
        {
          renderEverything = false;
        }
      }

      for ( int y = y1; y <= y2; y += stride )
      {
        for ( int x = x1; x <= x2; x += stride )
        {
          double[] color      = new double[3];
          float[]  floatColor = new float[3];

          // removes the need to make assignments of different sizes to accommodate bitmaps with sides indivisible by assignment size
          if ( y >= bitmapHeight || x >= bitmapWidth )
            break;       

          // prevents rendering of already rendered pixels
          if ( ( stride == 8 ||
                 ( y % ( stride << 1 ) != 0 ) ||
                 ( x % ( stride << 1 ) != 0 ) )
             ||
               ( renderEverything &&
                 ( ( y % ( previousStride << 1 ) != 0 ) ||
                   ( x % ( previousStride << 1 ) != 0 ) ) ) )
          {
            renderer.RenderPixel ( x, y, color ); // called at desired IRenderer; gets pixel color

            floatColor [ 0 ] = (float) color [ 0 ];
            floatColor [ 1 ] = (float) color [ 1 ];
            floatColor [ 2 ] = (float) color [ 2 ];
          }
          else
          {
            // positive infinity is used to signal BitmapMerger that color for this pixel is already present in main bitmap and is final (therefore no need for change)
            floatColor [ 0 ] = float.PositiveInfinity;  
            floatColor [ 1 ] = float.PositiveInfinity;
            floatColor [ 2 ] = float.PositiveInfinity;
          }

          
          if ( stride == 1 )  // apply color only to one pixel
          {
            returnArray [ PositionInArray ( x, y ) ]     = floatColor [ 0 ] * 255;
            returnArray [ PositionInArray ( x, y ) + 1 ] = floatColor [ 1 ] * 255;
            returnArray [ PositionInArray ( x, y ) + 2 ] = floatColor [ 2 ] * 255;
          }
          else // apply same color to multiple neighbour pixels (rectangle with current pixel in top left and length of side equal to stride value)
          {
            for ( int j = y; j < y + stride; j++ )
            {
              if ( j < bitmapHeight )
              {
                for ( int i = x; i < x + stride; i++ )
                {
                  if ( i < bitmapWidth )
                  {
                    returnArray [ PositionInArray ( i, j ) ]     = floatColor[0] * 255;
                    returnArray [ PositionInArray ( i, j ) + 1 ] = floatColor[1] * 255;
                    returnArray [ PositionInArray ( i, j ) + 2 ] = floatColor[2] * 255;
                  }
                }
              }
            }
          }


          if ( progressData != null ) // progressData is not used for distributed network rendering - null value used in rendering in RenderClients
          {
            lock ( Master.instance.progressData )
            {
              // test whether rendering should end (Stop button pressed) 
              if ( !Master.instance.progressData.Continue )
                return returnArray;

              // synchronization of bitmap with PictureBox in Form and update of progress (percentage of done work)
              if ( Master.instance.mainRenderThread == Thread.CurrentThread )
              {
                Master.instance.progressData.Finished = Master.instance.assignmentRoundsFinished / (float) Master.instance.assignmentRoundsTotal;
                Master.instance.progressData.Sync ( Master.instance.bitmap );
              }
            }
          }         
        }
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
      return ( ( y - y1 ) * assignmentSize + ( x - x1 ) ) * 3;
    }

    /// <summary>
    /// Standard - normal assignment with valid coordinates, dimensions and stride
    /// Ending - special assignment which tells to RenderClient that rendering is dome/no more work should be expected
    /// Reset - special asssignment which tells to RenderClient that it should reset and expect new render work
    /// </summary>
    public enum AssignmentType
    {
      Standard, Ending, Reset
    }
  }

  /// <summary>
  /// Represents one render client - name and IPaddress got from clientsDataGrid
  /// </summary>
  public class Client
  {
		public  IPAddress address;

		public string Name { get; set; }

		public string AddressString // string representation for the need of text in clientsDataGrid
    {
      get => GetIPAddress ();
      set => CheckAndSetIPAddress ( value );
    }

    private void CheckAndSetIPAddress ( string value )
    {
      value = value.Trim ();

      if ( value.ToLower() == "localhost" || value.ToLower() == "local")
      {
        address = IPAddress.Parse ( "127.0.0.1" );
        return;
      }

      bool isValidIP = IPAddress.TryParse ( value, out address );

      if ( !isValidIP )
      {
        address = IPAddress.Parse ( "0.0.0.0" );       
      }
    }

    private string GetIPAddress ()
    {
      if ( address == null )
      {
        return "";
      }

      if ( address.ToString () == "0.0.0.0" )
      {
        return "Invalid IP Address!";
      }
      else
      {
        return address.ToString ();
      }
    }
  }
}