//
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
    public class UpstreamBridge : ProcessThread
    {
        protected ClientConnection conn;

        public UpstreamBridge(ClientConnection parent)
        {
            this.conn = parent;
        }

        public override void Run() {
            while (!Interrupted())
            {
                try {
                    byte[] packet = conn.input.readPacket();
                    if (packet.Length >= 3)
                    {
                        using (MemoryStream ms = new MemoryStream(packet))
                        using (BinaryReader br = new BinaryReader(ms))
                        {
                            TerrariaPacket deserializedPacket = TerrariaPacket.Deserialize(br);
                            Console.WriteLine("[Bolt] [{0}] {1}", Thread.CurrentThread.Name, deserializedPacket);
                        }

                        conn.CurrentServer.output.Write(packet, 0, packet.Length);
                    }
                } catch (EndOfStreamException e) {
                    Console.WriteLine("[Bolt] [{0}] {1}", Thread.CurrentThread.Name, e.Message);
                    conn.Destroy("Reached end of stream");
                    Interrupt();
                } catch (SocketException e) {
                    Console.WriteLine("[Bolt] [{0}] Error: {1}", Thread.CurrentThread.Name, e.Message);
                }
            }
        }
    }
}

