using System;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
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
    MemoryStream    memoryStream = new MemoryStream ();

    formatter.Serialize ( memoryStream, objectToSend );

    byte[] dataBuffer = memoryStream.ToArray ();

    client.SendBufferSize = dataBuffer.Length;

    stream.Write ( dataBuffer, 0, dataBuffer.Length );

    Console.WriteLine ( "Data for {0} serialized and sent to server.", typeof ( T ).Name );
  }

  /// <summary>
  /// Receives object from NetworkStream and deserializes it
  /// </summary>
  /// <typeparam name="T">Type of object - must be marked as [Serializable], as well as all his recursive dependencies</typeparam>
  /// <returns>Instance of actual received object</returns>
  public static T ReceiveObject<T> ( TcpClient client, NetworkStream stream )
  {
    byte[] dataBuffer = new byte[client.ReceiveBufferSize];

    stream.Read ( dataBuffer, 0, dataBuffer.Length );

    MemoryStream    memoryStream = new MemoryStream ( dataBuffer );
    BinaryFormatter formatter    = new BinaryFormatter ();
    memoryStream.Position = 0;

    T receivedObject = (T) formatter.Deserialize ( memoryStream );

    Console.WriteLine ( "Data for {0} received and deserialized.", typeof ( T ).Name );

    return receivedObject;
  }

  /// <summary>
  /// For DEBUG only
  /// </summary>
  public static void TestSend ( TcpClient client, NetworkStream stream )
  {
    byte[] bytes = Encoding.ASCII.GetBytes ( "This is test message." );

    client.SendBufferSize = bytes.Length;

    stream.Write ( bytes, 0, bytes.Length );

    stream.Close ();
    client.Close ();
  }

  /// <summary>
  /// For DEBUG purposes and to remove some problems with NetworkStream
  /// Simply sends byte "1"
  /// </summary>
  public static void SendConfirmation ( NetworkStream stream )
  {
    stream.WriteByte ( 1 );
  }

  /// <summary>
  /// For DEBUG purposes and to remove some problems with NetworkStream
  /// Waits until byte "1" is not received
  /// </summary>
  public static void WaitForConfirmation ( NetworkStream stream )
  {
    byte receivedData;

    do
    {
      receivedData = (byte) stream.ReadByte ();
    } while ( receivedData != 1 );
  }
}