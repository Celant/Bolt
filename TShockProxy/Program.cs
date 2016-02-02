using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace TShockProxy
{
	class MainClass
	{
		static void Main(string[] args)
		{
			try
			{
				int port = 50000;
				IPAddress address = IPAddress.Loopback;
				AsyncService service = new AsyncService(port, address);
				service.Run();
				Console.ReadLine();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				Console.ReadLine();
			}
		}
	}

	public class AsyncService
	{
		private IPAddress ipAddress;
		private int port;

		public AsyncService(int port, IPAddress ipAddress)
		{
			this.port = port;
			this.ipAddress = ipAddress;
			if (this.ipAddress == null) {
				throw new Exception ("No IPv4 address for server");
			}
		}

		public async void Run()
		{
			TcpListener listener = new TcpListener(this.ipAddress, this.port);
			listener.Start();
			Console.Write ("Array Min and Avg service is now running");
			Console.WriteLine(" on " + this.ipAddress + ":" + this.port);
			Console.WriteLine("Hit <enter> to stop service\n");
			while (true) {
				try {
					TcpClient tcpClient = await listener.AcceptTcpClientAsync();
					Task t = Process(tcpClient);
					await t;
				}
					catch (Exception ex) {
					Console.WriteLine(ex.Message);
				}
			}
		}

		private async Task Process(TcpClient tcpClient)
		{
			string clientEndPoint = tcpClient.Client.RemoteEndPoint.ToString();
			Console.WriteLine("Received connection request from " + clientEndPoint);
			try {
				NetworkStream networkStream = tcpClient.GetStream();
				StreamReader reader = new StreamReader(networkStream);
				StreamWriter writer = new StreamWriter(networkStream);
				writer.AutoFlush = true;

				while (true) {
					string request = await reader.ReadLineAsync();
					if (request != null) {
						Console.WriteLine("Received service request: " + request);
						string response = Response(request);
						Console.WriteLine("Computed response is: " + response + "\n");
						await writer.WriteLineAsync(response);
					} else {
						break; // Client closed connection
					}
				}
				tcpClient.Close();
			}
			catch (Exception ex) {
				Console.WriteLine(ex.Message);
				if (tcpClient.Connected) {
					tcpClient.Close();
				}
			}
		}

		private static string Response(string request) {
			return "";
		}

		private static double Average(double[] vals) {
			return 0;
		}

		private static double Minimum(double[] vals) {
			return 0;
		}
	}
}