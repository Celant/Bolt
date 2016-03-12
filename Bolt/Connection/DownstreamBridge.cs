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
                    Console.WriteLine("Reached end of stream");
                    Console.WriteLine(e.Message);
                } catch (System.Exception e) {
                    Console.WriteLine(e.Message);
                }
            }
        }
    }
}

