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
                try {
                    byte[] packet = conn.input.readPacket();
                    //conn.CurrentServer.output.Write (packet, 0, packet.Length);

                    if (packet.Length >= 3)
                    {
                        using (MemoryStream ms = new MemoryStream(packet))
                        using (BinaryReader br = new BinaryReader(ms))
                        {
                            TerrariaPacket deserializedPacket = TerrariaPacket.Deserialize(br);
                            Console.WriteLine ("[Bolt] [{0}] Received from client: {1}", Thread.CurrentThread.Name, deserializedPacket);
                            Console.WriteLine ("[Bolt] [{0}] Sent to server: {1}", Thread.CurrentThread.Name, deserializedPacket);
                            conn.CurrentServer.output.Write (deserializedPacket.ToArray(), 0, packet.Length);
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

