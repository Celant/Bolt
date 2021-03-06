﻿//
//  IConnection.cs
//
//  Author:
//       Josh Harris <celant@celantinteractive.com>
//
//  Copyright (c) 2016 Celant
using Multiplicity.Packets;
using Multiplicity.Packets.Models;
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

            NetworkText disconnectReason = new NetworkText () {
                TextMode = 0,
                Text = reason
            };
            Disconnect disconnectPacket = new Disconnect ();
            disconnectPacket.Reason = disconnectReason;

            try {
                socket.Send(disconnectPacket.ToArray());
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

