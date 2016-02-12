//
//  ServerConnection.cs
//
//  Author:
//       Josh Harris <celant@celantinteractive.com>
//
//  Copyright (c) 2016 Celant
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using TShockProxy.Protocol.Packets;

namespace TShockProxy.Connection
{
    public class ServerConnection : IConnection
    {
        public ServerConnection(Socket socket, PacketInputStream input, NetworkStream output) : base(socket, input, output)
        {
        }

        public static ServerConnection connect(IPEndPoint address)
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(address);

            PacketInputStream input = new PacketInputStream(new NetworkStream(socket));
            NetworkStream output = new NetworkStream(socket, true);

            return new ServerConnection(socket, input, output);
        }
    }
}

