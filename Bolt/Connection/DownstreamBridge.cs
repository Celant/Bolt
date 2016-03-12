//
//  DownstreamBridge.cs
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
    public class DownstreamBridge : ProcessThread
    {
        protected ClientConnection ClientConnection;
        protected NATManager TranslationManager;

        public DownstreamBridge(ClientConnection parent)
        {
            this.ClientConnection = parent;
            this.TranslationManager = ClientConnection.TranslationManager;
        }

        public override void Run() {
            while (!Interrupted())
            {
                try {
                    byte[] packet = ClientConnection.CurrentServer.input.readPacket();
                    packet = TranslationManager.ProccessPacket(packet, TranslationType.Downstream);
                    ClientConnection.output.Write(packet, 0, packet.Length);
                } catch (EndOfStreamException e) {
                    Console.WriteLine("[Bolt] [Downstream] Reached end of stream");
                    ClientConnection.Destroy(e.Message);
                } catch (IOException e) {
                    Console.WriteLine("[Bolt] Failed on Downstream Bridge. Error: {0}", e.Message);
                    ClientConnection.Destroy(e.Message);
                }
            }
        }
    }
}

