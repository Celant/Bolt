//
//  UpstreamBridge.cs
//
//  Author:
//       Josh Harris <celant@celantinteractive.com>
//
//  Copyright (c) 2016 Celant
using Bolt.Proxy;
using System;
using System.IO;
using System.Net.Sockets;

namespace Bolt.Connection
{
    public class UpstreamBridge : ProcessThread
    {
        protected ClientConnection conn;
        protected NATManager TranslationManager;

        public UpstreamBridge(ClientConnection parent)
        {
            this.conn = parent;
            this.TranslationManager = conn.TranslationManager;
        }

        public override void Run() {
            while (!Interrupted())
            {
                try {
                    byte[] packet = conn.input.readPacket();
                    packet = TranslationManager.ProccessPacket(packet, TranslationType.Upstream);
                    conn.CurrentServer.output.Write(packet, 0, packet.Length);
                } catch (EndOfStreamException e) {
                    Console.WriteLine("[Bolt] [Upstream] {0}", e.Message);
                    conn.Destroy("Reached end of stream");
                    Interrupt();
                } catch (SocketException e) {
                    Console.WriteLine("[Bolt] Failed on Upstream Bridge. Error: {0}", e.Message);
                }
            }
        }
    }
}

