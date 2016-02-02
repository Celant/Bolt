using System;

namespace TShockProxy.Connection
{
	public interface IServer : IConnection
	{
		//string GetInfo();

		void SendData(byte[] data);
	}
}

