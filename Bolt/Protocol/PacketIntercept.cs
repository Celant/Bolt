//
//  PacketIntercept.cs
//
//  Author:
//       josh.harris <>
//
//  Copyright (c) 2017 ${CopyrightHolder}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Multiplicity.Packets;

namespace Bolt.Protocol
{
    public class PacketIntercept
    {
        public static List<PacketTypes> ServerPackets = new List<PacketTypes>()
        {
            PacketTypes.WorldInfo
        };

        public static List<PacketTypes> ClientPackets = new List<PacketTypes>()
        {
            
        };

        public static bool NeedsIntercepting(PacketTypes type, bool isInterceptingClient)
        {
            if (isInterceptingClient)
            {
                if (ClientPackets.Contains(type))
                    return true;
                else
                    return false;
            }
            else
            {
                if (ServerPackets.Contains(type))
                    return true;
                else
                    return false;
            }
        }

        public static void PerformIntercept(byte[] raw, Stream output, Stream input, bool isInterceptingClient)
        {
            using (MemoryStream ms = new MemoryStream(raw))
            using (BinaryReader br = new BinaryReader(ms))
            {
                TerrariaPacket deserializedPacket = TerrariaPacket.Deserialize(br);

                if (isInterceptingClient)
                {
                    
                }
                else
                {
                    switch (deserializedPacket.PacketType)
                    {
                        case PacketTypes.WorldInfo:
                            break;
                        default:
                            break;
                    }
                }

                SendData(deserializedPacket, raw, output);
            }

        }

        private static void SendData(TerrariaPacket deserializedPacket, byte[] raw, Stream output)
        {
            byte[] buffer = deserializedPacket.ToArray();
            if (deserializedPacket.PacketType != PacketTypes.LoadNetModule)
            {
                if (!buffer.SequenceEqual(raw))
                {
                    Console.WriteLine("[Bolt] [{0}] Multiplicity packet mismatch: {1} != {2}", Thread.CurrentThread.Name, buffer.Length, raw.Length);
                    Console.WriteLine("[Bolt] [{0}] client sent: {1}", Thread.CurrentThread.Name, BitConverter.ToString(raw));
                    Console.WriteLine("[Bolt] [{0}] multiplicity: {1}", Thread.CurrentThread.Name, BitConverter.ToString(buffer));

                    output.Write(raw, 0, raw.Length);
                }
                else
                {
                    Console.WriteLine("[Bolt] [{0}] client sent: {1}", Thread.CurrentThread.Name, deserializedPacket);

                    output.Write(buffer, 0, buffer.Length);
                }
            }
            else
            {
                output.Write(raw, 0, raw.Length);
            }
        }

        private static TerrariaPacket ModifyPacket(BinaryReader br, Stream output, Stream input)
        {
            TerrariaPacket packet = TerrariaPacket.Deserialize(br);
            return packet;
        }
    }
}
