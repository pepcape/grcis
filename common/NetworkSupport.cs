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
    BinaryFormatter formatter    = new BinaryFormatter ();

		//XmlSerializer formatter = new XmlSerializer(typeof(T));

		//IExtendedXmlSerializer formatter = new ConfigurationContainer ().EnableImplicitTyping ().EnableReferences ().Create ();

		formatter.Binder = new PreMergeToMergedDeserializationBinder ();

	  //formatter.AssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple;

		MemoryStream memoryStream = new MemoryStream ();

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

		//XmlSerializer formatter = new XmlSerializer(typeof(T));
	  //IExtendedXmlSerializer formatter = new ConfigurationContainer().Create();


		memoryStream.Position = 0;

    formatter.Binder = new PreMergeToMergedDeserializationBinder ();

	  T receivedObject = (T)formatter.Deserialize ( memoryStream );

    return receivedObject;
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

	sealed class PreMergeToMergedDeserializationBinder : SerializationBinder
	{
		public override Type BindToType ( string assemblyName, string typeName )
		{
			string currentAssembly = Assembly.GetExecutingAssembly().FullName;

			string typeToGet = String.Format ( "{0}, {1}", typeName, currentAssembly );

			if ( typeToGet.Contains("LinkedList") )
			{
				if ( typeToGet.Contains ( "ILight" ) )
				{
					typeToGet = typeof ( LinkedList<ILightSource> ).AssemblyQualifiedName;
        }
				if ( typeToGet.Contains ( "CSGInnerNode" ) )
				{
					typeToGet = typeof ( LinkedList<CSGInnerNode> ).AssemblyQualifiedName;
				}
				if ( typeToGet.Contains ( "ISceneNode" ) )
				{
					typeToGet = typeof ( LinkedList<ISceneNode> ).AssemblyQualifiedName;
				}
      }

			if ( typeToGet.Contains ( "Generic.Dictionary" ) )
			{
				typeToGet = typeof ( Dictionary<string, object> ).AssemblyQualifiedName;
      }

      // Get the type using the typeName and assemblyName
      Type typeToDeserialize = Type.GetType ( typeToGet );

			return typeToDeserialize;
		}


    /// <summary>
    /// override BindToName in order to strip the assembly name. Setting assembly name to null does nothing.
    /// </summary>
    /// <param name="serializedType"></param>
    /// <param name="assemblyName"></param>
    /// <param name="typeName"></param>
    public override void BindToName ( Type serializedType, out string assemblyName, out string typeName )
		{
			if ( serializedType.Assembly.ToString() == "048rtmontercarlo-script" )
			{
				assemblyName = "RenderClient";
      }
			else
			{
				assemblyName = null;
      }

			typeName = serializedType.FullName;
		}
	}
}