using System;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
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
    /// Receives all objects which are necessary from render server
    /// IRayScene - scene representation like solids, textures, lights, camera, ...
    /// IRenderer - renderer itself including IImageFunction; needed for RenderPixel method
    /// </summary>
    public static void ReceiveNecessaryObjects ()
    {
      scene = NetworkSupport.ReceiveObject<IRayScene> ( client, stream );
      Console.WriteLine ( "Data for {0} received and deserialized.", typeof ( IRayScene ).Name );
      NetworkSupport.SendConfirmation ( stream );

      renderer = NetworkSupport.ReceiveObject<IRenderer> ( client, stream );
      Console.WriteLine ( "Data for {0} received and deserialized.", typeof ( IRenderer ).Name );
      NetworkSupport.SendConfirmation ( stream );

      Console.ReadLine ();  // For Debug Only
    }
  }
}