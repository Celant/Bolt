//
//  ClientConnection.cs
//
//  Author:
//       Josh Harris <celant@celantinteractive.com>
//
//  Copyright (c) 2016 Celant
using LibMultiplicity.Packets.v1241;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using TShockProxy.Protocol.Packets;

namespace TShockProxy.Connection
{
    public class ClientConnection : IConnection
    {
        public PlayerInfo playerInfo;
        private ServerConnection CurrentServer;

        public ClientConnection(Socket socket, PacketInputStream input, NetworkStream output, PlayerInfo playerInfo) : base(socket, input, output)
        {
            this.playerInfo = playerInfo;
            username = this.playerInfo.Name;
            Console.WriteLine("User {0} has connected");
        }

        public void Connect(IPEndPoint address) {
            try {
                ServerConnection NewServer = ServerConnection.connect(address);
                if (CurrentServer == null) {

                }
            } catch (Exception e) {
                Console.Error.WriteLine(e.Message);
            }
        }
    }

    public class UpstreamBridge : ProcessThread
    {
        public override void Run() {
            while (!Interrupted())
            {

            }
        }
    }
}

