using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;
using Rendering;

namespace RenderClient
{
  class Program
  {
    static void Main ( string[] args )
    {
      RenderClient.ConnectToServer ();
      RenderClient.ReceiveNecessaryObjects ();

      //TODO: Do something with stream
    }
  }

  /// <summary>
  /// Takes care of connection and communication with server
  /// </summary>
  static class RenderClient
  {
    private static IPAddress     ipAdr;
    private static int           port = 5000;
    private static NetworkStream stream;
    private static TcpClient     client;

    private static IRayScene scene;
    private static IRenderer renderer;

    /// <summary>
    /// TcpListener is used even though this is in fact a client, not a server - this is done so that the real server can connect to the client
    /// (and not otherwise as usually)
    /// </summary>
    public static void ConnectToServer ()
    {
      Console.WriteLine ( "Waiting for remote server to connect to this client..." );

      TcpListener localServer = new TcpListener ( IPAddress.Loopback, port );
      localServer.Start ();

      do
      {
        client = localServer.AcceptTcpClient ();

        stream = client.GetStream ();
      } while ( stream == null );


      Console.WriteLine ( "Client succesfully connected." );
    }

    /// <summary>
    /// Serializes and sends object through NetworkStream
    /// </summary>
    /// <typeparam name="T">Type of object - must be marked as [Serializable], as well as all his recursive dependencies</typeparam>
    /// <param name="objectToSend">Instance of actual object to send</param>
    private static void SendObject<T> ( T objectToSend )
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
    public static T ReceiveObject<T> ()
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
    public static void TestSend ()
    {
      byte[] bytes = Encoding.ASCII.GetBytes ( "This is test message." );

      client.SendBufferSize = bytes.Length;

      stream.Write ( bytes, 0, bytes.Length );

      stream.Close ();
      client.Close ();
    }

    /// <summary>
    /// Receives all objects which are necessary from render server
    /// IRayScene - scene representation like solids, textures, lights, camera, ...
    /// IRenderer - renderer itself including IImageFunction; needed for RenderPixel method
    /// </summary>
    public static void ReceiveNecessaryObjects ()
    {
      scene = ReceiveObject<IRayScene> ();
      SendConfirmation ();
      renderer = ReceiveObject<IRenderer> ();
      SendConfirmation ();

      Console.ReadLine ();  // For Debug Only
    }

    /// <summary>
    /// For DEBUG purposes and to remove some problems with NetworkStream
    /// Simply sends byte "1"
    /// </summary>
    private static void SendConfirmation ()
    {
      stream.WriteByte ( 1 );
    }
  }
}