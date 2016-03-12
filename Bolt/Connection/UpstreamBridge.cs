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
                    Console.WriteLine("Reached end of stream");
                    Console.WriteLine(e.Message);
                } catch (System.Exception e) {
                    Console.WriteLine(e.Message);
                }
            }
        }
    }
}

