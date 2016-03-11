//
//  DownstreamBridge.cs
//
//  Author:
//       Josh Harris <celant@celantinteractive.com>
//
//  Copyright (c) 2016 Celant
using System;
using System.IO;

namespace Bolt.Connection
{
    public class DownstreamBridge : ProcessThread
    {
        
        protected ClientConnection clientConnection;
        protected int remotePlayerId;
        protected int localPlayerId;

        public DownstreamBridge(ClientConnection parent)
        {
            this.clientConnection = parent;
        }

        public override void Run() {
            while (!Interrupted())
            {
                try {
                    byte[] packet = clientConnection.CurrentServer.input.readPacket();
                    clientConnection.output.Write(packet, 0, packet.Length);
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

