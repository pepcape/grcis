using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using MathSupport;
using Utilities;

namespace Rendering
{
  /// <summary>
  /// Takes care of distribution of rendering work between local threads and remote/network render clients
  /// </summary>
  public class Master
  {
    public static Master singleton;

    public ConcurrentQueue<Assignment> availableAssignments;

    public List<NetworkWorker> networkWorkers;

    private Thread[] pool;

    public Thread mainRenderThread;

    public int totalNumberOfAssignments;

    public int finishedAssignments;

    // Square size of one block of pixels (rendered at one thread at the time)
    // 64 seems to be optimal; should be power of 2 and larger than 8 (= stride)
    public const int assignmentSize = 64;

    public Progress progressData;

    public int assignmentRoundsFinished;
    public int assignmentRoundsTotal;

    public Bitmap           bitmap;
    public IRayScene[]      scenes;
    public IImageFunction[] imageFunctions;
    public IRenderer[]      renderers;
    public double           time;

    private readonly IEnumerable<Client> clientsCollection;

    public PointCloud pointCloud;

    /// <summary>
    /// Constructor which takes also care of initializing assignments
    /// </summary>
    /// <param name="bitmap">Main bitmap - used also in PictureBox in Form1</param>
    /// <param name="scene">Scene to render</param>
    /// <param name="imageFunction">Current IImageFunction (if applicable)</param>
    /// <param name="renderer">Rendered to use for RenderPixel method</param>
    /// <param name="time">Time in seconds for still image from an animation</param>
    /// <param name="clientsCollection">Collection of Clients to get their IP addresses - names are only for user</param>
    /// <param name="threads">Number of threads to be used for rendering</param>
    /// <param name="newPointCloud">Bool whether new point cloud should be created</param>
    /// <param name="pointCloudStorage">External storage of point cloud data</param>
    public Master (
      Bitmap bitmap,
      IRayScene scene,
      IImageFunction imageFunction,
      IRenderer renderer,
      double time,
      IEnumerable<Client> clientsCollection,
      int threads,
      bool newPointCloud,
      ref PointCloud pointCloudStorage)
    {
      finishedAssignments = 0;

      scenes                 = new IRayScene[] { scene };
      imageFunctions         = new IImageFunction[] { imageFunction };
      renderers              = new IRenderer[] { renderer };
      this.time              = time;
      this.bitmap            = bitmap;
      this.clientsCollection = clientsCollection;
      MT.threads             = threads;

      Assignment.assignmentSize = assignmentSize;

      if (newPointCloud && !MT.pointCloudSavingInProgress)
      {
        pointCloud = new PointCloud(threads);
        pointCloudStorage = pointCloud;
      }
      else
      {
        pointCloud = pointCloudStorage;
      }

      singleton = this;
    }

    /// <summary>
    /// Creates threadpool and starts all threads on Consume method
    /// Thread which calls this method will take care of preparing assignments and receiving rendered images from RenderClients meanwhile
    /// </summary>
    public void RunThreads ()
    {
      pool = new Thread[MT.threads];

      AssignNetworkWorkerToStream();

      WaitHandle[] waitHandles = new WaitHandle[MT.threads];

      // Multiply animated instances.
      int i;

      // Scene definitions.
      if (scenes[0] is ITimeDependent scenea)
      {
        scenea.Time = time;
        scenes = new IRayScene[MT.threads];
        for (i = 0; i < MT.threads; i++)
          scenes[i] = i == 0 ? (IRayScene)scenea : (IRayScene)scenea.Clone();
      }

      // Image functions.
      if (imageFunctions[0] is ITimeDependent imfa)
      {
        imfa.Time = time;
        imageFunctions = new IImageFunction[MT.threads];
        for (i = 0; i < MT.threads; i++)
          imageFunctions[i] = i == 0 ? (IImageFunction)imfa : (IImageFunction)imfa.Clone();
      }

      // Renderers.
      if (renderers[0] is ITimeDependent renda)
      {
        renda.Time = time;
        renderers = new IRenderer[MT.threads];
        for (i = 0; i < MT.threads; i++)
          renderers[i] = i == 0 ? (IRenderer)renda : (IRenderer)renda.Clone();
      }

      for (i = 0; i < MT.threads; i++)
      {
        EventWaitHandle handle = new EventWaitHandle(false, EventResetMode.ManualReset);

        // Thread-specific instances: thread's id.
        int tid = i;

        Thread newThread = new Thread(() =>
        {
          // Set TLS.
          MT.threadID = tid;
          MT.InitThreadData();
          MT.SetRendering(
            scenes[Math.Min(tid, scenes.Length - 1)],
            imageFunctions[Math.Min(tid, imageFunctions.Length - 1)],
            renderers[Math.Min(tid, renderers.Length - 1)]);

          Consume();

          // Signal finishing the work.
          handle.Set();
        });

        newThread.Name = "RenderThread #" + i;
        pool[i] = newThread;
        newThread.Start();

        waitHandles[i] = handle;
      }

      mainRenderThread = pool[0];

      Thread imageReceiver = new Thread(RenderedImageReceiver);
      imageReceiver.Name = "ImageReceiver";
      imageReceiver.Start();

      WaitHandle.WaitAll(waitHandles);

      if (networkWorkers?.Count > 0)
      {
        foreach (NetworkWorker worker in networkWorkers) // sends ending assignment to all clients
        {
          worker.SendSpecialAssignment(Assignment.AssignmentType.Ending);
        }
      }

      // Reset the pool-thread.
      pool = null;
    }

    /// <summary>
    /// Consumer-producer based multithreading work distribution
    /// Each thread waits for a new Assignment to be added to availableAssignments queue
    /// Most of the time is number of items in availableAssignments expected to be several times larger than number of threads
    /// </summary>
    protected void Consume ()
    {
      while (!availableAssignments.IsEmpty ||
             finishedAssignments < totalNumberOfAssignments - MT.threads ||
             NetworkWorker.assignmentsAtClients > 0)
      {
        availableAssignments.TryDequeue(out Assignment newAssignment);

        if (!progressData.Continue) // test whether rendering should end (Stop button pressed)
          return;

        if (newAssignment == null) // TryDequeue was not successful
          continue;

        float[] colorArray = newAssignment.Render(false, progressData);
        BitmapMerger(
          colorArray,
          newAssignment.x1, newAssignment.y1,
          newAssignment.x2 + 1, newAssignment.y2 + 1);

        if (newAssignment.stride == 1)
        {
          finishedAssignments++;
          assignmentRoundsFinished++;
        }
        else
        {
          newAssignment.stride >>= 1; // stride values: 8 > 4 > 2 > 1
          assignmentRoundsFinished++;
          availableAssignments.Enqueue(newAssignment);
        }
      }
    }

    /// <summary>
    /// Creates new assignments based on width and heigh of bitmap and assignmentSize
    /// </summary>
    /// <param name="bitmap">Main bitmap - used also in PictureBox in Form1</param>
    public void InitializeAssignments (
      Bitmap bitmap)
    {
      availableAssignments = new ConcurrentQueue<Assignment>();

      int numberOfAssignmentsOnWidth = bitmap.Width % assignmentSize == 0
        ? bitmap.Width / assignmentSize
        : bitmap.Width / assignmentSize + 1;

      int numberOfAssignmentsOnHeight = bitmap.Height % assignmentSize == 0
        ? bitmap.Height / assignmentSize
        : bitmap.Height / assignmentSize + 1;

      for (int y = 0; y < numberOfAssignmentsOnHeight; y++)
      {
        for (int x = 0; x < numberOfAssignmentsOnWidth; x++)
        {
          int localX = x * assignmentSize;
          int localY = y * assignmentSize;

          Assignment newAssignment = new Assignment(localX,
                                                    localY,
                                                    localX + assignmentSize - 1,
                                                    localY + assignmentSize - 1,
                                                    bitmap.Width,
                                                    bitmap.Height);
          availableAssignments.Enqueue(newAssignment);
        }
      }

      totalNumberOfAssignments = availableAssignments.Count;

      // number of assignments * number of renderings of each assignments (depends on stride)
      assignmentRoundsTotal = totalNumberOfAssignments * (int)(Math.Log(Assignment.startingStride, 2) + 1);
    }

    /// <summary>
    /// Goes through all clients from RenderClientsForm and assigns a NetworkWorker to each of them
    /// </summary>
    public void AssignNetworkWorkerToStream ()
    {
      if (clientsCollection == null)
        return;

      networkWorkers = new List<NetworkWorker>();

      foreach (Client client in clientsCollection)
      {
        NetworkWorker newWorker = new NetworkWorker(client.address);

        if (newWorker.ConnectToClient())
        {
          try
          {
            newWorker.ExchangeNecessaryInfo();
          }
          catch (IOException) //thrown usually in case when stream is closed while exchanging necessary data
          {
            continue;
          }

          networkWorkers.Add(newWorker);

          for (int i = 0; i < newWorker.threadCountAtClient + 2; i++) //to each client is sent enough assignments to fill all threads + 2 for reserve
          {
            newWorker.TryToGetNewAssignment();
          }
        }
      }
    }

    /// <summary>
    /// Adds colors represented in colorBuffer array to main bitmap
    /// </summary>
    /// <param name="colorBuffer">Float values (possible to be used for HDR) representing pixel color values</param>
    /// <param name="x1">X coordinate of left upper corner in main picture</param>
    /// <param name="y1">Y coordinate of left upper corner</param>
    /// <param name="x2">X coordinate of right lower corner</param>
    /// <param name="y2">Y coordinate of right lower corner</param>
    public void BitmapMerger (float[] colorBuffer, int x1, int y1, int x2, int y2)
    {
      lock (bitmap)
      {
        int arrayPosition = 0;

        for (int y = y1; y < Math.Min(y2, bitmap.Height); y++)
        {
          for (int x = x1; x < x2; x++)
          {
            // Positive infinity indicates an already computed pixel.
            if (!float.IsInfinity(colorBuffer[arrayPosition]) &&
                x < bitmap.Width)
            {
              Color color = Color.FromArgb(Util.Clamp((int)colorBuffer[arrayPosition    ], 0, 255),
                                           Util.Clamp((int)colorBuffer[arrayPosition + 1], 0, 255),
                                           Util.Clamp((int)colorBuffer[arrayPosition + 2], 0, 255));
              bitmap.SetPixel(x, y, color);
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
      if (networkWorkers == null ||
          networkWorkers.Count == 0)
        return;

      foreach (NetworkWorker t in networkWorkers)
      {
        t?.ReceiveRenderedImageAsync();
      }
    }
  }

  /// <summary>
  /// Takes care of network communication with with one render client.
  /// </summary>
  public class NetworkWorker
  {
    private readonly IPAddress  ipAdr;
    private          IPEndPoint endPoint;
    private const    int        port = 5411;

    public TcpClient     client;
    public NetworkStream stream;

    public int threadCountAtClient;
    public static int assignmentsAtClients;

    private readonly List<Assignment> unfinishedAssignments = new List<Assignment>();

    public CancellationTokenSource imageReceivalCancelSource;
    private readonly CancellationToken imageReceivalCancelToken;

    public NetworkWorker (IPAddress ipAdr)
    {
      this.ipAdr = ipAdr;

      imageReceivalCancelSource = new CancellationTokenSource();
      imageReceivalCancelToken = imageReceivalCancelSource.Token;
    }

    /// <summary>
    /// Establishes NetworkStream with desired client
    /// </summary>
    /// <returns>True if connection was succesfull, False otherwise</returns>
    public bool ConnectToClient ()
    {
      if (ipAdr == null ||
          ipAdr.ToString() == "0.0.0.0")
        return false;

      if (endPoint == null)
        endPoint = new IPEndPoint(ipAdr, port);

      client = new TcpClient();

      try
      {
        client.Connect(endPoint);
      }
      catch (SocketException)
      {
        return false;
      }

      stream = client.GetStream();

      // Needed just in case - large portions of data are expected to be transfered at the same time (one rendered assignment is 50kB)
      client.ReceiveBufferSize = 1024 * 1024;
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
      // Set assemblies - needed for correct serialization/deserialization
      NetworkSupport.SendString(Assembly.GetExecutingAssembly().GetName().Name, stream);
      string targetAssembly = NetworkSupport.ReceiveString(stream);
      NetworkSupport.SetAssemblyNames(Assembly.GetExecutingAssembly().GetName().Name, targetAssembly);

      NetworkSupport.SendObject(new Assignment(Assignment.AssignmentType.Reset), stream);

      NetworkSupport.SendObject(Master.singleton.scenes[0], stream);

      NetworkSupport.SendObject(Master.singleton.renderers[0], stream);

      threadCountAtClient = NetworkSupport.ReceiveInt(stream);
    }

    private const int bufferSize = (Master.assignmentSize * Master.assignmentSize * 3 + 2) * sizeof(float);

    /// <summary>
    /// Asynchronous image receiver uses NetworkStream.ReadAsync
    /// Recursively called at the end
    /// If so, it reads it as - expected format is array of floats
    ///   - first 2 floats represents x1 and y1 coordinates - position in main bitmap;
    ///   - rest of array are colors of rendered bitmap - 3 floats (RGB values) per pixel;
    ///   - stored per lines from left upper corner (coordinates position)
    /// </summary>
    public async void ReceiveRenderedImageAsync ()
    {
      while (Master.singleton.finishedAssignments < Master.singleton.totalNumberOfAssignments &&
             Master.singleton.progressData.Continue)
      {
        if (!NetworkSupport.IsConnected(client))
        {
          LostConnection();
          return;
        }

        byte[] receiveBuffer = new byte[bufferSize];

        try
        {
          await stream.ReadAsync(receiveBuffer, 0, bufferSize, imageReceivalCancelToken);
        }
        catch (IOException)
        {
          LostConnection ();
          return;
        }

        // Uses parts of receiveBuffer - separates and converts data to coordinates
        // and floats representing colors of pixels
        float[] coordinates = new float[2];
        float[] floatBuffer = new float[Master.assignmentSize * Master.assignmentSize * 3];
        Buffer.BlockCopy(receiveBuffer, 0, coordinates, 0, 2 * sizeof(float));
        Buffer.BlockCopy(receiveBuffer, 2 * sizeof(float), floatBuffer, 0, floatBuffer.Length * sizeof(float));

        Master.singleton.BitmapMerger(
          floatBuffer,
          (int)coordinates[0],
          (int)coordinates[1],
          (int)coordinates[0] + Master.assignmentSize,
          (int)coordinates[1] + Master.assignmentSize);

        Master.singleton.finishedAssignments++;

        // Takes care of increasing assignmentRoundsFinished by the amount of finished
        // rendering rounds on RenderClient.
        lock (unfinishedAssignments)
        {
          Assignment currentAssignment = unfinishedAssignments.Find(a => a.x1 == (int)coordinates[0] && a.y1 == (int)coordinates[1]);
          int roundsFinished = (int)Math.Log(currentAssignment.stride, 2) + 1; // stride goes from 8 > 4 > 2 > 1 (1 step = 1 rendering round)
          Master.singleton.assignmentRoundsFinished += roundsFinished;

          assignmentsAtClients--;

          RemoveAssignmentFromUnfinishedAssignments((int)coordinates[0], (int)coordinates[1]);
        }

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
      ResetUnfinishedAssignments();
      Master.singleton.networkWorkers.Remove(this);
      client.Close();
    }

    /// <summary>
    /// Removes assignments identified by its coordinates of left upper corner (x1 and y1) from unfinishedAssignments list
    /// </summary>
    /// <param name="x">Compared to value of x1 in assignment</param>
    /// <param name="y">Compared to value of y1 in assignment</param>
    private void RemoveAssignmentFromUnfinishedAssignments (int x, int y)
    {
      for (int i = 0; i < unfinishedAssignments.Count; i++)
      {
        if (unfinishedAssignments[i].x1 == x &&
            unfinishedAssignments[i].y1 == y)  // assignments are uniquely indentified by coordinates of left upper corner
        {
          unfinishedAssignments.RemoveAt(i);
          return;
        }
      }

      unfinishedAssignments.Clear();
    }

    /// <summary>
    /// Loops until it gets a new assignment and sends it to the RenderClient (or all assignments have been rendered)
    /// </summary>
    public void TryToGetNewAssignment ()
    {
      while (Master.singleton.finishedAssignments < Master.singleton.totalNumberOfAssignments - assignmentsAtClients)
      {
        if (!NetworkSupport.IsConnected(client))
        {
          LostConnection();
          return;
        }

        Master.singleton.availableAssignments.TryDequeue(out Assignment newAssignment);

        if (!Master.singleton.progressData.Continue) // test whether rendering should end (Stop button pressed)
          return;

        if (newAssignment == null) // TryDequeue was not succesfull
          continue;

        lock (stream)
        {
          NetworkSupport.SendObject(newAssignment, stream);
          assignmentsAtClients++;
          unfinishedAssignments.Add(newAssignment);
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
      foreach (Assignment unfinishedAssignment in unfinishedAssignments)
      {
        Master.singleton.availableAssignments.Enqueue(unfinishedAssignment);
        assignmentsAtClients--;
      }
    }

    /// <summary>
    /// Sends special Assignment which can signal to client that rendering stopped or that client should be reset and wait for more work
    /// </summary>
    public void SendSpecialAssignment (Assignment.AssignmentType assignmentType)
    {
      Assignment newAssignment = new Assignment(assignmentType);

      lock (stream)
      {
        NetworkSupport.SendObject(newAssignment, stream);
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

    // Global assignment size.
    public static int assignmentSize;

    // Only one pixel from a stride x stride square is rendered (progressive rendering)
    public int stride;

    private readonly int bitmapWidth, bitmapHeight;

    public AssignmentType type;

    public const int startingStride = 8;

    /// <summary>
    /// Main constructor for standard assignments.
    /// </summary>
    public Assignment (int x1, int y1, int x2, int y2, int width, int height)
    {
      this.x1 = x1;
      this.y1 = y1;
      this.x2 = x2;
      this.y2 = y2;
      bitmapWidth  = width;
      bitmapHeight = height;
      type = AssignmentType.Standard;

      // Stride values: 8 > 4 > 2 > 1; initially always 8
      // Stride decreases at the end of rendering of current assignment and therefore makes
      // another render of this assignment more detailed.
      stride = startingStride;
    }

    /// <summary>
    /// Constructor used for special assignments with AssignmentType other than Standard
    /// Special assignments are used to indicate end of rendering, request to reset render client...
    /// Coordinates, dimensions and stride are irrelevant in this case - they are set to -1 for error checking
    /// </summary>
    public Assignment (AssignmentType type)
    {
      x1           =
      y1           =
      x2           =
      y2           =
      bitmapWidth  =
      bitmapHeight =
      stride       = -1;

      this.type = type;
    }

    /// <summary>
    /// Main render method
    /// </summary>
    /// <param name="renderEverything">True if you want to ignore stride and just render everything at once (removes dynamic rendering effect; distributed network rendering)</param>
    /// <param name="progressData">Used for sync of bitmap with main PictureBox</param>
    /// <returns>Float array which represents colors of pixels (3 floats per pixel - RGB channel)</returns>
    public float[] Render (
      bool renderEverything,
      Progress progressData = null)
    {
      // !!! TODO: change this for non-RGB rendering !!!
      float[] returnArray = new float[assignmentSize * assignmentSize * 3];

      int previousStride = stride;

      if (renderEverything)
      {
        stride = 1;

        if (previousStride == stride)
          renderEverything = false;
      }

      for (int y = y1; y <= y2; y += stride)
        for (int x = x1; x <= x2; x += stride)
        {
          // !!! TODO: change this for non-RGB rendering !!!
          double[] color      = new double[3];
          float[]  floatColor = new float[3];

          // Removes the need to make assignments of different sizes to accommodate
          // bitmaps with sides indivisible by assignment size.
          if (y >= bitmapHeight || x >= bitmapWidth)
            break;

          // Prevents rendering of already rendered pixels.
          if (stride == 8 ||
              (y % (stride << 1) != 0) ||
              (x % (stride << 1) != 0) ||
              renderEverything )
          {
            // IRenderer does the job = computes the pixel color.
            MT.renderer.RenderPixel(x, y, color);

            floatColor[0] = (float)color[0];
            floatColor[1] = (float)color[1];
            floatColor[2] = (float)color[2];
          }
          else
          {
            // Positive infinity is used to signal BitmapMerger that color for this pixel
            // is already present in main bitmap and is final (therefore no need for change)
            floatColor[0] = float.PositiveInfinity;
            floatColor[1] = float.PositiveInfinity;
            floatColor[2] = float.PositiveInfinity;
          }

          if (stride == 1)
          {
            // Apply color to single pixel only.
            returnArray[PositionInArray(x, y)]     = floatColor[0] * 255;
            returnArray[PositionInArray(x, y) + 1] = floatColor[1] * 255;
            returnArray[PositionInArray(x, y) + 2] = floatColor[2] * 255;
          }
          else
          {
            // Apply constant color to multiple neighbour pixels:
            // square starting at the current pixel with side = stride.
            for (int j = y; j < y + stride && j < bitmapHeight; j++)
              for (int i = x; i < x + stride && i < bitmapWidth; i++)
              {
                returnArray[PositionInArray(i, j)]     = floatColor[0] * 255;
                returnArray[PositionInArray(i, j) + 1] = floatColor[1] * 255;
                returnArray[PositionInArray(i, j) + 2] = floatColor[2] * 255;
              }
          }

          // 'progressData' is not used for distributed network rendering
          // - null value used in rendering in RenderClients.
          if (progressData != null)
          {
            lock (Master.singleton.progressData)
            {
              // Test whether rendering should end (Stop button pressed).
              if (!Master.singleton.progressData.Continue)
                return returnArray;

              // Synchronization of bitmap with PictureBox in Form and
              // update of progress (percentage of done work).
              if (Master.singleton.mainRenderThread == Thread.CurrentThread)
              {
                Master.singleton.progressData.Finished = Master.singleton.assignmentRoundsFinished / (float)Master.singleton.assignmentRoundsTotal;
                Master.singleton.progressData.Sync(Master.singleton.bitmap);
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
    private int PositionInArray (int x, int y)
    {
      return ((y - y1) * assignmentSize + (x - x1)) * 3;
    }

    /// <summary>
    /// Standard - normal assignment with valid coordinates, dimensions and stride
    /// Ending - special assignment which tells to RenderClient that rendering is dome/no more work should be expected
    /// Reset - special asssignment which tells to RenderClient that it should reset and expect new render work
    /// </summary>
    public enum AssignmentType
    {
      Standard,       // Computing pixel colors
      Ending,         // Epilogue ?
      Reset           // Initial assignment sent to a network worker
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
      get => GetIPAddress();
      set => CheckAndSetIPAddress(value);
    }

    private void CheckAndSetIPAddress (string value)
    {
      value = value.Trim();

      if (value.ToLower() == "localhost" ||
          value.ToLower() == "local" ||
          value.ToLower() == "l")
      {
        address = IPAddress.Parse("127.0.0.1");
        return;
      }

      bool isValidIP = IPAddress.TryParse(value, out address);

      if (!isValidIP)
      {
        address = IPAddress.Parse("0.0.0.0");
      }
    }

    private string GetIPAddress ()
    {
      if (address == null)
        return "";

      if (address.ToString () == "0.0.0.0")
        return "Invalid IP Address!";

      return address.ToString();
    }
  }
}
