//
//  ProxiedPlayer.cs
//
//  Author:
//       Josh Harris <celant@celantinteractive.com>
//
//  Copyright (c) 2016 Celant
using Multiplicity.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.IO;
using System.Net;
using System.Net.Sockets;
using Bolt.Connection;
using Bolt.Protocol;

namespace Bolt.Proxy
{
    public class InitialHandler
    {
        private Socket socket;
        private PacketInputStream input;
        private NetworkStream output;

        protected LoginQueue loginQueue = new LoginQueue();

        public InitialHandler(Socket socket)
        {
            this.socket = socket;
            input = new PacketInputStream(new NetworkStream(socket));
            output = new NetworkStream(socket, true);
        }

        public void Run()
        {
            while (Bolt.Instance.IsRunning && socket.Connected)
            {
                byte[] packetBuf = input.readPacket();
                if (packetBuf.Length <= 0)
                {
                    return;
                }
                
                using (MemoryStream ms = new MemoryStream(packetBuf))
                using (BinaryReader br = new BinaryReader(ms))
                {
                    TerrariaPacket packet = TerrariaPacket.Deserialize(br);
                    Console.WriteLine(packet);
                    if (ProcessPacket(packet))
                    {
                        break;
                    }
                }
            }
        }

        public bool ProcessPacket(TerrariaPacket packet)
        {
            bool handled = false;
            switch (packet.ID)
            {
                case 0x01:
	                ClientConnection clientCon = new ClientConnection (socket, input, output);
	                clientCon.Register (0);
	                clientCon.Connect (new IPEndPoint (IPAddress.Parse ("127.0.0.1"), 7977));
	                handled = true;
                    break;
                default:
                    break;
            }

            return handled;
        }
    }
}

