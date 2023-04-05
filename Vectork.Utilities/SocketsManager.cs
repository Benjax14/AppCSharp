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
	public class SocketsManager
	{
		private Socket _ListeningSocket;
		private List<Socket> _Sockets;
		private bool _Verbose;
		private Thread _ReaderThread;
		private bool _KeepReading;

		private EndPoint _LocalEndPoint;
		private EndPoint _RemoteEndPoint;
		
		private string _Ip;
		private int _Port;
		private bool _Listen;


		public event EventHandler<string> DataReceived;

		public SocketsManager(string ipAddress, int port, bool listen, bool verbose = false)
		{
			_Verbose = verbose;
			_Ip = ipAddress;
			_Port = port;
			_Listen = listen;
			_Sockets = new List<Socket>();

		}

		/// <summary>
		/// Acepta un socket remoto cuando este socket quedó en modo Listen
		/// </summary>
		/// <param name="result">Datos de la conexion entrante</param>
		private void AcceptRemoteSocket(IAsyncResult result)
		{
			try
			{
				if (_Verbose)
					Console.WriteLine(" Un socket remoto se ha conectado a este socket listen");

				var socket = (Socket)result.AsyncState;
				socket = socket.EndAccept(result);
				_Sockets.Add(socket);
				BeginRead();
			}
			catch (Exception e)
			{
				Console.WriteLine(" Error al aceptar Socket Remoto." + e);
			}
		}

		/// <summary>
		/// Conectar
		/// </summary>
		public bool Connect()
		{
			try
			{
				Destroy();

				if (_Listen)
				{
					_LocalEndPoint = GetIpEndPoint(_Ip,_Port);
					

					var socket = new Socket(_LocalEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
					socket.Bind(_LocalEndPoint);

					_ListeningSocket = socket;
					_ListeningSocket.Listen(100);
					_ListeningSocket.BeginAccept(new AsyncCallback(AcceptRemoteSocket), _ListeningSocket);
					if (_Verbose) Console.WriteLine("Ha quedado Listen");
				}
				else
				{
					_LocalEndPoint = GetIpEndPoint("localhost",0);
					_RemoteEndPoint = GetIpEndPoint(_Ip,_Port);

					var socket = new Socket(_LocalEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
					socket.Bind(_LocalEndPoint);
					socket.Connect(_RemoteEndPoint);
					
					if (_Verbose) Console.WriteLine("--- Socket Conectado!!!");
					_Sockets.Add(socket);
					BeginRead();
				}

			}
			catch (Exception e)
			{
				Console.WriteLine("--- Error al conectar socket"+e);
				Destroy();
				return false;
			}
			return true;
		}



		/// <summary>
		/// Envia el objeto serializado
		/// </summary>
		/// <param name="o">Objeto que implementa IByteSerialize</param>
		public void Send(string o)
		{
			byte[] sendBytes;

			using (var stream1 = new MemoryStream())
			using (var writer = new BinaryWriter(stream1))
			{
				writer.Write(o);
				sendBytes = stream1.GetBuffer();
			}

			Send(sendBytes);
		}

		/// <summary>
		/// Envia un array de bytes
		/// </summary>
		/// <param name="data">Array de bytes</param>
		private void Send(byte[] data)
		{
			try
			{
				
				// Agregamos el tamaño de datos al inicio de lo que enviaremos
				// El tamaño no incluye los 4 bytes usados para escribir el tamaño de datos.
				var dataSize = data.Length;
				var dataSizeBytes = BitConverter.GetBytes(dataSize);
				var dataWithSize = new byte[sizeof(int) + dataSize]; // Esto enviaremos
				// Copiamos el tamaño al inicio de lo que enviaremos
				Buffer.BlockCopy(dataSizeBytes, 0, dataWithSize, 0, dataSizeBytes.Length);
				// Luego copiamos los datos
				Buffer.BlockCopy(data, 0, dataWithSize, dataSizeBytes.Length, dataSize);
				data = dataWithSize;
				

				foreach (var socket in _Sockets)
				{
					if (!socket.Connected)
					{
						Console.WriteLine("Socket es null o no esta conectado ");
						return;
					}	
					socket.Send(data);

					//if (_Settings.Verbose) Error("  Datos enviados correctamente. IP: " + _Settings.RemoteEndPoint.IP + " -- Puerto : " + _Settings.RemoteEndPoint.Port + " -- ");

					//if (_Verbose)
					//{
					//	var remoteIpEndPoint = socket.RemoteEndPoint as IPEndPoint;
					//	var localIpEndPoint = socket.LocalEndPoint as IPEndPoint;

					//	if (remoteIpEndPoint != null)
					//	{
					//		Error($"  Datos enviados correctamente. IP: { remoteIpEndPoint.Address}  -- Puerto : {remoteIpEndPoint.Port} -- Bytes {data.Length} -- ");	
					//	}

						
					//}
				}
			}
			catch (Exception e)
			{
				Console.WriteLine("  Error al enviar datos "+ e);
			}
		}
	


		public void BeginRead()
		{
			if (_ReaderThread == null || !_ReaderThread.IsAlive)
			{
				_ReaderThread = new Thread(new ThreadStart(this.ReadProcess));
				_ReaderThread.IsBackground = true;
				_ReaderThread.Name = "Socket Server Read Thread";
				_ReaderThread.Priority = ThreadPriority.Normal;
				_ReaderThread.Start();
			}

		}

		private void ReadProcess()
		{
			try
			{

				if (_Verbose)
					Console.WriteLine("Comienza lectura de datos");

				_KeepReading = true;

				var socket = _Sockets.First();

				// Tamaño de los datos que llegaran en cada paquete de datos enviados
				// por defecto es el tamaño de la serializacion a menos que el tamaño venga en el mismo paquete de datos
				int incomingSize = 0;
				//if (_Settings.Mode == NetworkTransferMode.Both) SerilizationSizeTransfered = true;
				while (_KeepReading)
				{

					// Primero leemos el tamaño de los datos que llegaran
					var sizeBuffer = new byte[sizeof(int)];
					ReadBytes(sizeBuffer, socket);
					incomingSize = BitConverter.ToInt32(sizeBuffer, 0);


					var dataBuffer = new byte[incomingSize];
					ReadBytes(dataBuffer, socket);
					RaiseDataReceived(dataBuffer);

				}
			}
			catch (Exception e)
			{
				Destroy();
				Console.WriteLine("  Error lectura de dato" + e);
			}
		}



		/// <summary>
		/// Lee buffer.Size bytes entrantes desde un socket.
		/// </summary>
		/// <param name="buffer">El buffer</param>
		/// <param name="socket">El socket</param>
		/// <param name="verbose">Si emitir mensajes de logger</param>
		private void ReadBytes(byte[] buffer, Socket socket)
		{
			int offset = 0;
			while (offset < buffer.Length & _KeepReading)
			{
				var numberOfBytesRead = socket.Receive(buffer, offset, buffer.Length - offset, 0);
				offset += numberOfBytesRead;
			}

			if (offset == buffer.Length)
			{
				if (_Verbose) Console.WriteLine("se terminaron de leer " + buffer.Length + " bytes  ");
			}
			else
				Console.WriteLine($"Se leyeron ({offset}) mas datos de los esperados {buffer.Length}");


		}

		private void RaiseDataReceived(byte[] data)
		{
			var handler = DataReceived;

			if (handler != null)
			{
				var text = "";
				try {
					
					using(var stream2 = new MemoryStream(data))
					using(var reader = new BinaryReader(stream2))
					{
						text = reader.ReadString();
					}
				}
				catch (Exception e)
				{
					Console.WriteLine(e);
				}

				handler(this, text);
			}
		}

		/// <summary>
		/// Destruye las instancias de sockets (no destruye este objeto, se puede volver a conectar)
		/// </summary>
		public void Destroy()
		{
			StopRead();

			if (_ListeningSocket != null)
			{
				if (_ListeningSocket.Connected)
				{
					_ListeningSocket.Shutdown(SocketShutdown.Both);
					_ListeningSocket.Disconnect(false);
					_ListeningSocket = null;
				}
			}

			foreach (var socket in _Sockets)
			{
				if (socket.Connected)
				{
					socket.Shutdown(SocketShutdown.Both);
					socket.Disconnect(false);
					Console.WriteLine("--Socket Desconectado");

				}
			}
			_Sockets.Clear();
		}


		public void StopRead()
		{
			_KeepReading = false;
		}

		private IPEndPoint GetIpEndPoint(string ip, int port)
		{
			IPAddress ipAddress = null;

			if (ip.ToLowerInvariant() == "localhost")
			{
				var ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
				ipAddress = ipHostInfo.AddressList.FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetwork);

				if (ipAddress == null)
					throw new Exception("Sin IP para localhost");

			}
			else
			{
				if (!IPAddress.TryParse(ip, out ipAddress))
					throw new Exception($"La dirección IP no es válida : {ip}");
			}

			return new IPEndPoint(ipAddress, port);
		}
	}
}
