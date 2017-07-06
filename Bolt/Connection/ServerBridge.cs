//
//  DownstreamBridge.cs
//
//  Author:
//       Josh Harris <celant@celantinteractive.com>
//
//  Copyright (c) 2016 Celant
using Bolt.Proxy;
using Multiplicity.Packets;
using Multiplicity.Packets.Models;
using System;
using System.IO;
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
                    byte[] packet = ClientConnection.CurrentServer.input.readPacket();
                    if (packet.Length >= 3)
                    {
                        using (MemoryStream ms = new MemoryStream(packet))
                        using (BinaryReader br = new BinaryReader(ms))
                        {
                            TerrariaPacket deserializedPacket = TerrariaPacket.Deserialize(br);
                            Console.WriteLine ("[Bolt] [{0}] Received from server: {1}", Thread.CurrentThread.Name, deserializedPacket);
                            Console.WriteLine ("[Bolt] [{0}] Sent to client: {1}", Thread.CurrentThread.Name, deserializedPacket);
                            ClientConnection.output.Write (packet, 0, packet.Length);
                        }
                    }
                    else
                    {
                        NetworkText disconnectReason = new NetworkText () {
                            TextMode = 0,
                            Text = "Connection reset"
                        };
                        Disconnect disconnectPacket = new Disconnect ();
                        disconnectPacket.Reason = disconnectReason;
                        ClientConnection.output.Write(disconnectPacket.ToArray(), 0, disconnectPacket.ToArray().Length);
                    }
                }
                catch (EndOfStreamException e) {
                    Console.WriteLine("[Bolt] [{0}] {1}", Thread.CurrentThread.Name, e.Message);
                    ClientConnection.Destroy("Reached end of stream");
                    Interrupt();
                }
                catch (SocketException e) {
                    Console.WriteLine("[Bolt] [{0}] Error: {1}", Thread.CurrentThread.Name, e.Message);
                }
                catch (IOException e)
                {
                    Console.WriteLine("[Bolt] [{0}] Error: {1}", Thread.CurrentThread.Name, e.Message);
                }
            }
        }
    }
}

