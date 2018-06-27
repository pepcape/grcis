using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace RenderClient
{
	class Program
	{
	  static void Main ( string[] args )
	  {
	    //RenderClient.ParseAddress ();

	    RenderClient.ConnectToServer ();
	  }
	}

  static class RenderClient
  {
    private static IPAddress ipAdr;
    private static int port = 5000;

    public static void ConnectToServer()
    {
      TcpListener localServer = new TcpListener ( IPAddress.Loopback, port );

      NetworkStream stream;

      do
      {
				TcpClient client = localServer.AcceptTcpClient ();

        stream = client.GetStream ();
			} while ( stream == null );

      //TODO: Do something with stream
		}
  }
}