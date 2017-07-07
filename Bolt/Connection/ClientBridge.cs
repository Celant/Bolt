﻿//
//  UpstreamBridge.cs
//
//  Author:
//       Josh Harris <celant@celantinteractive.com>
//
//  Copyright (c) 2016 Celant
using Bolt.Proxy;
using Multiplicity.Packets;
using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace Bolt.Connection
{
    public class ClientBridge : ProcessThread
    {
        protected ClientConnection conn;

        public ClientBridge(ClientConnection parent)
        {
            this.conn = parent;
        }

        public override void Run() {
            while (!Interrupted())
            {
                try
                {
                    byte[] raw = conn.input.readPacket();
                    byte[] packet2 = new byte[raw.Length];
                    raw.CopyTo(packet2, 0);

                    if (packet2.Length >= 3)
                    {
                        using (MemoryStream ms = new MemoryStream(packet2))
                        using (BinaryReader br = new BinaryReader(ms))
                        {
                            TerrariaPacket deserializedPacket = TerrariaPacket.Deserialize(br);
                            byte[] buffer = deserializedPacket.ToArray();

                            if (deserializedPacket.PacketType != PacketTypes.LoadNetModule)
                            {
                                if (buffer.Length != packet2.Length)
                                {
                                    Console.WriteLine("[Bolt] [{0}] Multiplicity length mismatch: {1} != {2}", Thread.CurrentThread.Name, buffer.Length, packet2.Length);
                                    Console.WriteLine("[Bolt] [{0}] client sent: {1}", Thread.CurrentThread.Name, BitConverter.ToString(packet2));
                                    Console.WriteLine("[Bolt] [{0}] multiplicity: {1}", Thread.CurrentThread.Name, BitConverter.ToString(buffer));
                                    conn.CurrentServer.output.Write(buffer, 0, buffer.Length);
                                    continue;
                                }
                            }
                            else
                            {
                                conn.CurrentServer.output.Write(raw, 0, raw.Length);
                                continue;
                            }
                        }
                    }

                }
                catch (EndOfStreamException e)
                {
                    Console.WriteLine("[Bolt] [{0}] {1}", Thread.CurrentThread.Name, e.Message);
                    conn.Destroy("Reached end of stream");
                    Interrupt();
                }
                catch (SocketException e)
                {
                    Console.WriteLine("[Bolt] [{0}] Error: {1}", Thread.CurrentThread.Name, e.Message);
                    conn.Destroy(e.Message);
                    Interrupt();
                }
                catch (IOException e)
                {
                    Console.WriteLine("[Bolt] [{0}] Error: {1}", Thread.CurrentThread.Name, e.Message);
                    conn.Destroy(e.Message);
                    Interrupt();
                }
            }
        }
    }
}

