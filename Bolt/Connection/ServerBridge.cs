//
//  DownstreamBridge.cs
//
//  Author:
//       Josh Harris <celant@celantinteractive.com>
//
//  Copyright (c) 2016 Celant
using Bolt.Protocol;
using Bolt.Proxy;
using Multiplicity.Packets;
using System;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading;

namespace Bolt.Connection
{
    public class ServerBridge : ProcessThread
    {
        protected ClientConnection ClientConnection;

        public ServerBridge(ClientConnection parent)
        {
            this.ClientConnection = parent;
        }

        public override void Run() {
            while (!Interrupted())
            {
                try
                {
                    byte[] raw = ClientConnection.CurrentServer.input.readPacket();
                    byte[] packet2 = new byte[raw.Length];
                    raw.CopyTo(packet2, 0);

                    if (packet2.Length >= 3)
                    {
                        PacketIntercept.PerformIntercept(raw, ClientConnection.output, ClientConnection.CurrentServer.output, false);
                    }
                }
                catch (EndOfStreamException e) {
                    Console.WriteLine("[Bolt] [{0}] {1}", Thread.CurrentThread.Name, e.Message);
                    ClientConnection.Destroy("Reached end of stream");
                    Interrupt();
                }
                catch (SocketException e) {
                    Console.WriteLine("[Bolt] [{0}] Error: {1}", Thread.CurrentThread.Name, e.Message);
                    ClientConnection.Destroy(e.Message);
                    Interrupt();
                }
                catch (IOException e)
                {
                    Console.WriteLine("[Bolt] [{0}] Error: {1}", Thread.CurrentThread.Name, e.Message);
                    ClientConnection.Destroy(e.Message);
                    Interrupt();
                }
            }
        }
    }
}

