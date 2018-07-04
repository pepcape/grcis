using System;
using System.Net.Sockets;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

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
    MemoryStream    memoryStream = new MemoryStream ();

    formatter.Serialize ( memoryStream, objectToSend );

    byte[] dataBuffer = memoryStream.ToArray ();

    SendSize ( stream, dataBuffer.Length );

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
    int leftToReceive     = ReceiveSize ( stream );

    byte[] receiveBuffer = new byte[leftToReceive];

    while ( leftToReceive > 0 ) // Loop until enough data is received
    {
      int latestReceivedSize = stream.Read ( receiveBuffer, totalReceivedSize, leftToReceive );
      leftToReceive     -= latestReceivedSize;
      totalReceivedSize += latestReceivedSize;
    }

    MemoryStream    memoryStream = new MemoryStream ( receiveBuffer );
    BinaryFormatter formatter    = new BinaryFormatter ();
    memoryStream.Position = 0;

    T receivedObject = (T) formatter.Deserialize ( memoryStream );

    return receivedObject;
  }

  /// <summary>
  /// Sends message (always 4 bytes) notifying about size of the next message
  /// </summary>
  /// <param name="stream">NetworkStream to use</param>
  /// <param name="size">Int size of the next message (in bytes)</param>
  public static void SendSize ( NetworkStream stream, int size )
  {
    byte[] bytes = BitConverter.GetBytes ( size );

    stream.Write ( bytes, 0, bytes.Length );
  }

  /// <summary>
  /// Reads message (always 4 bytes) and returns size of the next message
  /// </summary>
  /// <param name="stream">NetworkStream to use</param>
  /// <returns>Int size of the next message (in bytes)</returns>
  public static int ReceiveSize ( NetworkStream stream )
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
}