using System;
using System.Net;

namespace TShockProxy.Connection
{
	public interface IConnection
	{
		IPEndPoint getAddress();

		void SendPacket(byte[] packet);
	}
}

