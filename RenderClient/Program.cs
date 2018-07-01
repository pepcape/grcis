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


  static class RenderClient
  {
    private static IPAddress     ipAdr;
    private static int           port = 5000;
    private static NetworkStream stream;
    private static TcpClient     client;

    private static IRayScene scene;
    private static IRenderer renderer;

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

    public static T ReceiveObject<T> ()
    {
      byte[] dataBuffer = new byte[client.ReceiveBufferSize];

      stream.Read ( dataBuffer, 0, dataBuffer.Length );

      MemoryStream    memoryStream = new MemoryStream ( dataBuffer );
      BinaryFormatter formatter    = new BinaryFormatter ();
      memoryStream.Position = 0;

      T receivedObject = (T) formatter.Deserialize ( memoryStream );

      return receivedObject;
    }

    public static void TestSend ()
    {
      byte[] bytes = Encoding.ASCII.GetBytes ( "This is test message." );

      client.SendBufferSize = bytes.Length;

      stream.Write ( bytes, 0, bytes.Length );

      stream.Close ();
      client.Close ();
    }

    public static void ReceiveNecessaryObjects ()
    {
      //scene = ReceiveObject<IRayScene> ();
      renderer = ReceiveObject<IRenderer> ();

      while ( true ) { }
    }
  }
}