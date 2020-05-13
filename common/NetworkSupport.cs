using System;
using System.Net.Sockets;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using Open.Nat;
using Rendering;


public static class NetworkSupport
{
  /// <summary>
  /// Serializes and sends object through NetworkStream
  /// </summary>
  /// <typeparam name="T">Type of object - must be marked as [Serializable], as well as all his recursive dependencies</typeparam>
  ///   /// <param name="stream">Network stream to send data</param>
  /// <param name="objectToSend">Instance of actual object to send</param>
  public static void SendObject<T> ( T objectToSend, NetworkStream stream )
  {
	  MemoryStream memoryStream = new MemoryStream ();
    BinaryFormatter formatter = new BinaryFormatter ();

	  if ( customBinder != null )
		  formatter.Binder = customBinder;   

    formatter.Serialize ( memoryStream, objectToSend );

    SendBytes ( memoryStream.ToArray (), stream );
  }


  /// <summary>
  /// Receives object from NetworkStream and deserializes it
  /// </summary>
  /// <typeparam name="T">Type of object - must be marked as [Serializable], as well as all his recursive dependencies</typeparam>
  /// <param name="stream">Network stream to receive data</param>
  /// <returns>Instance of actual received object</returns>
  public static T ReceiveObject<T> ( NetworkStream stream )
  {
    byte[] receiveBuffer = ReceiveByteArray ( stream );

    MemoryStream memoryStream = new MemoryStream ( receiveBuffer );
	  BinaryFormatter formatter    = new BinaryFormatter ();

	  if ( customBinder != null )
		  formatter.Binder = customBinder;
			
    memoryStream.Position = 0;    		

	  T receivedObject = (T)formatter.Deserialize ( memoryStream );

    return receivedObject;
  }


	private static CustomBinder customBinder = null;

  /// <summary>
  /// Used to avoid problems with serialization in one assembly and deserialization in another one
  /// </summary>
  /// <param name="senderAssembly">Assembly name of sender</param>
  /// <param name="targetAssembly">Assembly name of receiver</param>
  public static void SetAssemblyNames ( string senderAssembly, string targetAssembly )
  {
	  customBinder = new CustomBinder ( senderAssembly, targetAssembly );
  }


  /// <summary>
  /// Sends message containing an int (4 bytes)
  /// Used for notifying the other side about size of the next message (to prepare buffers)
  /// </summary>
  /// <param name="stream">NetworkStream to use</param>
  /// <param name="size">Int to send</param>
  public static void SendInt ( NetworkStream stream, int size )
  {
    byte[] bytes = BitConverter.GetBytes ( size );

    stream.Write ( bytes, 0, bytes.Length );
  }


  /// <summary>
  /// Reads 4 bytes from stream and returns them as int
  /// Used for receiving info from the other side about size of the next message (to prepare buffers)
  /// </summary>
  /// <param name="stream">NetworkStream to use</param>
  /// <returns>Int value represented by 4 bytes from NetworkStream</returns>
  public static int ReceiveInt ( NetworkStream stream )
  {
    int totalReceivedSize = 0;
    int leftToReceive     = sizeof ( int );

    byte[] receiveBuffer = new byte[leftToReceive];

    while ( leftToReceive > 0 ) // Loop until enough data is received
    {
      int latestReceivedSize = stream.Read ( receiveBuffer, totalReceivedSize, leftToReceive );
      leftToReceive     -= latestReceivedSize;
      totalReceivedSize += latestReceivedSize;
    }

    return BitConverter.ToInt32 ( receiveBuffer, 0 );
  }


  /// <summary>
  /// Notifies the receiver about size of byte array (as single int) and sends that array to stream
  /// </summary>
  /// <param name="bytes">Data to send</param>
  /// <param name="stream">NetworkStream to send data</param>
  public static void SendBytes ( byte[] bytes, NetworkStream stream )
  {
    SendInt ( stream, bytes.Length );

    stream.Write ( bytes, 0, bytes.Length );
  }


  /// <summary>
  /// Receives info about size of byte array which will be received (as single int) and receives it from stream
  /// </summary>
  /// <param name="stream">NetworkStream to receive data</param>
  /// <returns></returns>
  public static byte[] ReceiveByteArray ( NetworkStream stream )
  {
    int totalReceivedSize = 0;
    int leftToReceive     = ReceiveInt ( stream );

    byte[] receiveBuffer = new byte[leftToReceive];

    while ( leftToReceive > 0 ) // Loop until enough data is received
    {
      int latestReceivedSize = stream.Read ( receiveBuffer, totalReceivedSize, leftToReceive );
      leftToReceive     -= latestReceivedSize;
      totalReceivedSize += latestReceivedSize;
    }

    return receiveBuffer;
  }


  /// <summary>
  /// Encodes string in ASCII and sends it over network stream
  /// </summary>
  /// <param name="str">String to send (if null, nothing is sent)</param>
  /// <param name="stream">NetworkStream to send data</param>
  public static void SendString ( string str, NetworkStream stream )
  {
    if ( str != null )
    {
      SendBytes ( Encoding.ASCII.GetBytes ( str ), stream );
    }    
  }

  /// <summary>
  /// Receives string from network stream and decodes it
  /// </summary>
  /// <param name="stream">NetworkStream to send data</param>
  /// <returns>Received string</returns>
  public static string ReceiveString ( NetworkStream stream )
  {
    byte[] bytes = ReceiveByteArray ( stream );

    return Encoding.ASCII.GetString ( bytes );
  }


  /// <summary>
  /// Determines whether TCP Client is still connected
  /// Works similar to ping
  /// Sends non-blocking, empty message with no need to actively read it
  /// </summary>
  /// <param name="client">TcpClient used to get underlying Socket</param>
  /// <returns></returns>
  public static bool IsConnected ( TcpClient client )
  {
    Socket socket = client.Client;

    bool blockingState = socket.Blocking; // preserves current blocking state

    try
    {
      byte [] tmp = new byte[1];

      socket.Blocking = false;
      socket.Send ( tmp, 0, 0 );

      return true;
    }
    catch ( SocketException e )
    {
      // 10035 == WSAEWOULDBLOCK (this error is thrown when non-blocking socket cannot be completed immediatelly; does not mean it did not arrive)
      if ( e.NativeErrorCode.Equals ( 10035 ) )
        return true;
      else
        return false;
    }
    finally
    {
      socket.Blocking = blockingState;
    }
  }


	/// <summary>
  /// Takes care of port forwarding
  /// </summary>
  /// <param name="port">Port to forward</param>
  /// <param name="protocol">Protocol to use for forwarding</param>
  /// <param name="description">Description of forwarding</param>
	public static async void NecessaryNetworkSettings ( int port, Protocol protocol, string description )
	{
    NatDiscoverer discoverer = new NatDiscoverer();
		CancellationTokenSource cts = new CancellationTokenSource ( 10000 );
		NatDevice device = await discoverer.DiscoverDeviceAsync ( PortMapper.Upnp, cts );

		await device.CreatePortMapAsync ( new Mapping ( protocol, port, port, description ) );
  }

	/// <summary>
  /// Simple getter for external IP address
  /// </summary>
  /// <returns>Returns external IP address</returns>
	public static IPAddress GetExternalIPAddress ()
	{
		string externalIP = new WebClient ().DownloadString ( "http://icanhazip.com" );

		IPAddress.TryParse ( externalIP.TrimEnd(), out IPAddress address );

    return address;
	}

	/// <summary>
	/// Simple getter for local IP address
	/// </summary>
	/// <returns>Returns local IP address</returns>
  public static IPAddress GetLocalIPAddress ()
	{
		string localIP;

		using ( Socket socket = new Socket ( AddressFamily.InterNetwork, SocketType.Dgram, 0 ) )
		{
			socket.Connect ( "8.8.8.8", 65530 );	//8.8.8.8 is one of the Google's DNS servers

			IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
			localIP = endPoint.Address.ToString ();
		}

		IPAddress.TryParse ( localIP, out IPAddress address );

		return address;
	}


	/// <summary>
  /// Custom Binder needed for serialization/deserialization between different assemblies
  /// </summary>
  sealed class CustomBinder : SerializationBinder
	{
		public CustomBinder ( string senderAssembly, string targetAssembly )
		{
			this.senderAssembly = senderAssembly;
      this.targetAssembly = targetAssembly;			
		}

		private string senderAssembly;
    private string targetAssembly;		

    /// <summary>
    /// Overrides BindToType in order to change the assembly name. Called by BinaryFormatter.
    /// </summary>
    /// <param name="assemblyName"></param>
    /// <param name="typeName"></param>
    /// <returns></returns>
    public override Type BindToType ( string assemblyName, string typeName )
		{
			string currentAssembly = Assembly.GetExecutingAssembly().FullName;

			string typeToGet = $"{typeName}, {currentAssembly}";

			if ( typeToGet.Contains ( "`" ) )  //char ` is used in full type name of generic classes - they need special treatment when changing their assembly name 
			{
				string temp = typeof ( LinkedList<ILightSource> ).AssemblyQualifiedName;
        string systemAssembly = temp.Substring (temp.IndexOf(", System, Version=") + 2);

        typeToGet = $"{typeName}, {systemAssembly}";

				typeToGet = typeToGet.Replace ( senderAssembly, targetAssembly);
			}			

      Type typeToDeserialize = Type.GetType ( typeToGet );

			return typeToDeserialize;
		}


    /// <summary>
    /// Overrides BindToName in order to change the assembly name. Setting assembly name to null does nothing. Called by BinaryFormatter.
    /// </summary>
    /// <param name="serializedType"></param>
    /// <param name="assemblyName"></param>
    /// <param name="typeName"></param>
    public override void BindToName ( Type serializedType, out string assemblyName, out string typeName )
		{
			if ( serializedType.Assembly.ToString() == senderAssembly )
			{
				assemblyName = targetAssembly;
      }
			else
			{
				assemblyName = null;
      }

			typeName = serializedType.FullName;
		}
	}
}