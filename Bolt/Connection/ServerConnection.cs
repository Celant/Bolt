//
//  ServerConnection.cs
//
//  Author:
//       Josh Harris <celant@celantinteractive.com>
//
//  Copyright (c) 2016 Celant
using Multiplicity.Packets;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using Bolt.Protocol;
using Bolt.Exception;

namespace Bolt.Connection
{
    public class ServerConnection : GenericConnection
    {
        public byte ServerPlayerID;

        public ServerConnection(Socket socket, PacketInputStream input, NetworkStream output, byte serverPlayerID) : base(socket, input, output)
        {
            this.ServerPlayerID = serverPlayerID;
        }

        public static ServerConnection connect(IPEndPoint address)
        {
            try {
                Console.WriteLine("[Bolt] Opening connection to target server");
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(address);

                PacketInputStream input = new PacketInputStream(new NetworkStream(socket));
                NetworkStream output = new NetworkStream(socket, true);

                byte[] buffer;

                byte serverPlayerID;

                ConnectRequest connRequest = new ConnectRequest("Terraria156"); 
                buffer = connRequest.ToArray();
                output.Write(buffer, 0, buffer.Length);
                Console.WriteLine(connRequest);

                buffer = input.readPacket();
                using (MemoryStream ms = new MemoryStream(buffer))
                using (BinaryReader br = new BinaryReader(ms))
                {
                    TerrariaPacket packet = TerrariaPacket.Deserialize(br);
                    if (packet.ID != 0x03)
                    {
                        throw new ProtocolViolationException("Connection was refused to the target server");
                    }
                    ContinueConnecting continueConnecting = packet as ContinueConnecting;
                    serverPlayerID = continueConnecting.PlayerID;

                    Console.WriteLine("[Bolt] Target server accepted connection as Player {0}", serverPlayerID);
                }

                return new ServerConnection(socket, input, output, serverPlayerID);
            } catch (SocketException e) {
                Console.WriteLine(e);
                throw new KickException("[Bolt] Failed to connect to target server");
            }
        }
    }
}

