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
    byte[] bytes = new byte[sizeof ( int )];

    int receivedSize = stream.Read ( bytes, 0, bytes.Length );

    if ( receivedSize != bytes.Length )
    {
      return -1;
    }

    return BitConverter.ToInt32 ( bytes, 0 );
  }
}