﻿//
//  IConnection.cs
//
//  Author:
//       Josh Harris <celant@celantinteractive.com>
//
//  Copyright (c) 2016 Celant
using DotNetty.Buffers;
using LibMultiplicity;
using LibMultiplicity.Packets.v1241;
using LibMultiplicity.Packets.v1302;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using Bolt.Protocol;

namespace Bolt.Connection
{
    public class GenericConnection
    {
        public Socket socket;
        public PacketInputStream input;
        public Stream output;
        public string username;

        public GenericConnection(Socket socket, PacketInputStream input, NetworkStream output) {
            this.socket = socket;
            this.input = input;
            this.output = output;
        }

        public void Disconnect(string reason) {
            
            Disconnect DisconnectPacket = new LibMultiplicity.Packets.v1241.Disconnect(reason);

            try {
                socket.Send(DisconnectPacket.ToArray());
            } catch (SocketException e) {
                //Console.Error.WriteLine(e.Message);
            } finally {
                try {
                    socket.Shutdown(SocketShutdown.Both);
                    socket.Close();
                } catch (SocketException e) {
                    //Console.Error.WriteLine(e.Message);
                }
            }
        }
    }
}
