using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace Vectork.Utilities
{
	public class SocketsServer : SocketsManager
	{


		public SocketsServer(string ipAddress, int port, bool verbose = false) : base(ipAddress,port,true,verbose)
		{

			
		}
	}
}
