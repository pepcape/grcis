using System;
using System.Net.Sockets;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using Rendering;


public static class NetworkSupport
{
  /// <summary>
  /// Serializes and sends object through NetworkStream
  /// </summary>
  /// <typeparam name="T">Type of object - must be marked as [Serializable], as well as all his recursive dependencies</typeparam>
  /// <param name="objectToSend">Instance of actual object to send</param>
  public static void SendObject<T> ( T objectToSend, TcpClient client, NetworkStream stream )
  {
	  MemoryStream memoryStream = new MemoryStream ();
    BinaryFormatter formatter    = new BinaryFormatter ();

	  if ( customBinder != null )
		  formatter.Binder = customBinder;   

    formatter.Serialize ( memoryStream, objectToSend );

    byte[] dataBuffer = memoryStream.ToArray ();

    SendInt ( stream, dataBuffer.Length );

    stream.Write ( dataBuffer, 0, dataBuffer.Length );
  }

  /// <summary>
  /// Receives object from NetworkStream and deserializes it
  /// </summary>
  /// <typeparam name="T">Type of object - must be marked as [Serializable], as well as all his recursive dependencies</typeparam>
  /// <returns>Instance of actual received object</returns>
  public static T ReceiveObject<T> ( TcpClient client, NetworkStream stream )
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

    MemoryStream    memoryStream = new MemoryStream ( receiveBuffer );
	  BinaryFormatter formatter    = new BinaryFormatter ();

	  if ( customBinder != null )
		  formatter.Binder = customBinder;
	  

    memoryStream.Position = 0;    

	  T receivedObject = (T)formatter.Deserialize ( memoryStream );

    return receivedObject;
  }

	private static CustomBinder customBinder = null;

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

			string typeToGet = String.Format ( "{0}, {1}", typeName, currentAssembly );

			if ( typeToGet.Contains ( "`" ) )  //char ` is used in full type name of generic classes - they need special treatment when changing their assembly name 
			{
				string temp = typeof ( LinkedList<ILightSource> ).AssemblyQualifiedName;
        string systemAssembly = temp.Substring (temp.IndexOf(", System, Version=") + 2);

        typeToGet = String.Format ( "{0}, {1}", typeName, systemAssembly );

				typeToGet = typeToGet.Replace ("048rtmontecarlo-script", "RenderClient");
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