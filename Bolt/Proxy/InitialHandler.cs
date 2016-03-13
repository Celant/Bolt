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
using LibMultiplicity.Packets.v1302;
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
            byte[] buffer;
            bool handled = false;
            switch (packet.ID)
            {
                case 0x01:
                    var playerid = Array.FindIndex(Bolt.Instance.Players, i => i == null);
                    ContinueConnecting continuePacket = new ContinueConnecting(0);
                    continuePacket.PlayerID = (byte) playerid;
                    buffer = continuePacket.ToArray();
                    output.Write(buffer, playerid, buffer.Length);
                    break;
                case 0x04:
                    LibMultiplicity.Packets.v1302.PlayerInfo playerInfo = packet as LibMultiplicity.Packets.v1302.PlayerInfo;
                    loginQueue.playerInfo = playerInfo;
                    Console.WriteLine("{0} is connecting to slot {1}", playerInfo.Name, playerInfo.PlayerID);
                    break;
                case 0x10:
                    loginQueue.playerHP = packet as LibMultiplicity.Packets.v1241.PlayerHP;
                    break;
                case 0x2a:
                    loginQueue.playerMana = packet as LibMultiplicity.Packets.v1241.PlayerMana;
                    break;
                case 0x32:
                    loginQueue.playerBuffs = packet as LibMultiplicity.Packets.v1241.PlayerBuffs;
                    break;
                case 0x05:
                    loginQueue.playerSlot.Add(packet as LibMultiplicity.Packets.v1241.PlayerInventorySlot);
                    break;
                case 0x06:
                    /*
                    Console.WriteLine("Received packet 0x06, dumping login queue");
                    Console.WriteLine("-----------------------------------------");
                    Console.WriteLine("                Begin dump               ");
                    Console.WriteLine("-----------------------------------------");
                    Console.WriteLine(loginQueue.playerInfo);
                    Console.WriteLine(loginQueue.playerHP);
                    Console.WriteLine(loginQueue.playerMana);
                    Console.WriteLine(loginQueue.playerBuffs);
                    foreach (PlayerInventorySlot slot in loginQueue.playerSlot)
                    {
                        Console.WriteLine(slot);
                    }
                    Console.WriteLine("-----------------------------------------");
                    Console.WriteLine("                 End dump                ");
                    Console.WriteLine("-----------------------------------------");
                    */

                    NATManager translationManager = new NATManager(loginQueue.playerInfo.PlayerID);
                    ClientConnection clientCon = new ClientConnection(socket, input, output, loginQueue, translationManager);
                    clientCon.Register(loginQueue.playerInfo.PlayerID);
                    clientCon.Connect(new IPEndPoint(IPAddress.Parse("85.236.105.4"), 7977));
                    handled = true;
                    break;
                default:
                    break;
            }

            return handled;
        }
    }
}

