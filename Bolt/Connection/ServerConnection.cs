//
//  ServerConnection.cs
//
//  Author:
//       Josh Harris <celant@celantinteractive.com>
//
//  Copyright (c) 2016 Celant
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
    public class ServerConnection : IConnection
    {
        public ServerConnection(Socket socket, PacketInputStream input, NetworkStream output) : base(socket, input, output)
        {
        }

        // , LibMultiplicity.Packets.v1302.PlayerInfo playerInfo, LibMultiplicity.Packets.v1241.PlayerHP playerHp, LibMultiplicity.Packets.v1241.PlayerMana playerMana
        public static ServerConnection connect(IPEndPoint address, LoginQueue loginQueue)
        {
            Console.WriteLine("[Bolt] Opening connection to target server");
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(address);

            PacketInputStream input = new PacketInputStream(new NetworkStream(socket));
            NetworkStream output = new NetworkStream(socket, true);

            byte[] buffer;

            ConnectRequest connRequest = new ConnectRequest("Terraria156"); 
            buffer = connRequest.ToArray();
            output.Write(buffer, 0, buffer.Length);
            Console.WriteLine(connRequest);

            buffer = input.readPacket();
            using (MemoryStream ms = new MemoryStream(buffer))
            using (BinaryReader br = new BinaryReader(ms))
            {
                TerrariaPacket packet = TerrariaPacket.Deserialize(br);
                Console.WriteLine(packet);
                if (packet.ID != 0x03)
                {
                    throw new ProtocolViolationException("Connection was refused to the target server");
                }
                Console.WriteLine("[Bolt] Target server accepted connection");
            }

            buffer = loginQueue.playerInfo.ToArray();
            Console.WriteLine(loginQueue.playerInfo);
            output.Write(buffer, 0, buffer.Length);

            buffer = loginQueue.playerHP.ToArray();
            Console.WriteLine(loginQueue.playerHP);
            output.Write(buffer, 0, buffer.Length);

            buffer = loginQueue.playerMana.ToArray();
            Console.WriteLine(loginQueue.playerMana);
            output.Write(buffer, 0, buffer.Length);

            buffer = loginQueue.playerBuffs.ToArray();
            Console.WriteLine(loginQueue.playerBuffs);
            output.Write(buffer, 0, buffer.Length);

            buffer = new byte[0];

            foreach (PlayerInventorySlot slot in loginQueue.playerSlot)
            {
                byte[] partbuffer = slot.ToArray();
                Array.Resize(ref buffer, buffer.Length + partbuffer.Length);
                Array.Copy(partbuffer, 0, buffer, buffer.Length - partbuffer.Length, partbuffer.Length);
                Console.WriteLine(slot);
                output.Write(buffer, 0, buffer.Length);
            }

            return new ServerConnection(socket, input, output);
        }
    }
}

