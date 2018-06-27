using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Rendering
{
	class WorkloadDistribution
	{

	}


  class Master
  {
    public static Master instance; // singleton

  }


  class LocalWorker: Worker
	{

  }

  class NetworkWorker : Worker
	{
		public IPAddress ipAdr;
		private IPEndPoint endPoint;
    private static int port = 5000;


    public bool CreateEndPoint ()
    {
      if ( ipAdr == null)
      {
				return false;
      }
      else
      {
				endPoint = new IPEndPoint ( ipAdr, port );
				return true;
			}     
    }

	  public bool ConnectToClient ()
	  {
	    TcpClient client = new TcpClient ();
	    client.Connect ( endPoint );

	    NetworkStream stream = client.GetStream ();


			return true;
      //TODO: Do something with stream
	  }
	}

  abstract class Worker: IWorker
  {
    delegate void currentWork ( Bitmap image, int x1, int y1, int x2, int y2 );

		public bool isWorking;
	}

	interface IWorker
  {

  }
}
