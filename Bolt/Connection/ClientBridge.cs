﻿//
//  UpstreamBridge.cs
//
//  Author:
//       Josh Harris <celant@celantinteractive.com>
//
//  Copyright (c) 2016 Celant
using Bolt.Proxy;
using Bolt.Protocol;
using Multiplicity.Packets;
using System;
using System.IO;
using System.Linq;
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
                        PacketIntercept.PerformIntercept(raw, conn.CurrentServer.output, conn.output, true);

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

