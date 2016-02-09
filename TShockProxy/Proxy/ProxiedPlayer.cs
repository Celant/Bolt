//
//  ProxiedPlayer.cs
//
//  Author:
//       Josh Harris <celant@celantinteractive.com>
//
//  Copyright (c) 2016 Celant
using DotNetty.Buffers;
using System;
using System.Net;
using System.Net.Sockets;
using TShockProxy.Protocol.Packets;

namespace TShockProxy.Proxy
{
    public class ProxiedPlayer
    {
        private Socket Socket;

        public ProxiedPlayer(Socket socket)
        {
            this.Socket = socket;
        }

        public void Run()
        {
            while (TShockProxy.Instance.IsRunning && Socket.Connected)
            {
                IByteBuffer buffer = Unpooled.Buffer();
                byte[] byteArray = new byte[1024];

                int length = Socket.Receive(byteArray);
                buffer.WriteBytes(byteArray, 0, length);

                if (buffer.ReadableBytes >= 3)
                {
                    BasePacket packet = new BasePacket();
                    packet.Read(buffer);

                    Console.WriteLine(packet.PacketID);

                    ProcessPacket(packet);
                }
            }
        }

        public void ProcessPacket(BasePacket packet)
        {
            switch (packet.PacketID)
            {
                case 0x01:
                    Packet2Kick KickPacket = new Packet2Kick("Server is full.");
                    IByteBuffer buffer = Unpooled.Buffer();
                    KickPacket.Write(buffer);
                    byte[] Data = new byte[buffer.ReadableBytes];
                    buffer.ReadBytes(Data);
                    int sent = Socket.Send(Data);
                    break;
                default:
                    break;
            }
        }
    }
}

