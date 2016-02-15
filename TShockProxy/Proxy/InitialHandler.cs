//
//  ProxiedPlayer.cs
//
//  Author:
//       Josh Harris <celant@celantinteractive.com>
//
//  Copyright (c) 2016 Celant
using DotNetty.Buffers;
using LibMultiplicity;
using LibMultiplicity.Packets.v1241;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using TShockProxy.Protocol.Packets;

namespace TShockProxy.Proxy
{
    public class InitialHandler
    {
        private Socket Socket;
        private PacketInputStream input;
        private NetworkStream output;

        public InitialHandler(Socket socket)
        {
            this.Socket = socket;
            input = new PacketInputStream(new NetworkStream(socket));
            output = new NetworkStream(socket, true);
        }

        public void Run()
        {
            while (TShockProxy.Instance.IsRunning && Socket.Connected)
            {
                byte[] packetBuf = input.readPacket();
                
                using (MemoryStream ms = new MemoryStream(packetBuf))
                using (BinaryReader br = new BinaryReader(ms))
                {
                    TerrariaPacket packet = TerrariaPacket.Deserialize(br);
                    Console.WriteLine(packet);
                    ProcessPacket(packet);
                }
                
                
            }
        }

        public void ProcessPacket(TerrariaPacket packet)
        {
            switch (packet.ID)
            {
                case 0x01:
                    Disconnect kickPacket = new Disconnect("Server is full.");
                    byte[] buffer = kickPacket.ToArray();
                    output.Write(buffer, 0, buffer.Length);
                    break;
                default:
                    break;
            }
        }
    }
}

