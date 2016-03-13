//
//  DownstreamBridge.cs
//
//  Author:
//       Josh Harris <celant@celantinteractive.com>
//
//  Copyright (c) 2016 Celant
using Bolt.Proxy;
using LibMultiplicity;
using LibMultiplicity.Packets;
using LibMultiplicity.Packets.v1241;
using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;

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
                    if (packet.Length >= 3)
                    {
                        packet = TranslationManager.ProccessPacket(packet, TranslationType.Downstream);
                        using (MemoryStream ms = new MemoryStream(packet))
                        using (BinaryReader br = new BinaryReader(ms))
                        {
                            Console.WriteLine("[Bolt] [{0}] {1}", Thread.CurrentThread.Name, BitConverter.ToString(packet));
                            TerrariaPacket deserializedPacket = TerrariaPacket.Deserialize(br);
                            Console.WriteLine("[Bolt] [{0}] {1}", Thread.CurrentThread.Name, deserializedPacket);
                        }
                        ClientConnection.output.Write(packet, 0, packet.Length);
                    } else {
                        Disconnect disconnect = new Disconnect("Connection reset");
                        ClientConnection.output.Write(disconnect.ToArray(), 0, disconnect.ToArray().Length);
                    }
                } catch (EndOfStreamException e) {
                    Console.WriteLine("[Bolt] [{0}] {1}", Thread.CurrentThread.Name, e.Message);
                    ClientConnection.Destroy("Reached end of stream");
                    Interrupt();
                } catch (SocketException e) {
                    Console.WriteLine("[Bolt] [{0}] Error: {1}", Thread.CurrentThread.Name, e.Message);
                }
            }
        }
    }
}

