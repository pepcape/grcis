using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using Rendering;

namespace RenderClient
{
	class Program
	{
	  static void Main ( string[] args )
	  {





			// RenderClient.ConnectToServer ();

			//TODO: Do something with stream
		}
	}

  static class RenderClient
  {
    private static IPAddress ipAdr;
    private static int port = 5000;
    private static NetworkStream stream;

		public static void ConnectToServer()
    {
			Console.WriteLine ( "Waiting for remote server to connect to this client..." ); 

      TcpListener localServer = new TcpListener ( IPAddress.Loopback, port );

      do
      {
				TcpClient client = localServer.AcceptTcpClient ();

        stream = client.GetStream ();
			} while ( stream == null );


			Console.WriteLine ( "Client succesfully connected." );     
		}
  }
}