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
                        //ClientConnection.output.Write (packet, 0, packet.Length);

                        Console.WriteLine("[Bolt] [{0}] Received from server: {1}", Thread.CurrentThread.Name, BitConverter.ToString(packet));
                        Console.WriteLine("[Bolt] [{0}] Received from server len: {1}", Thread.CurrentThread.Name, packet.Length);
                        using (MemoryStream ms = new MemoryStream(packet))
                        using (BinaryReader br = new BinaryReader(ms))
                        {
                            TerrariaPacket deserializedPacket = TerrariaPacket.Deserialize(br);
                            Console.WriteLine ("[Bolt] [{0}] Received from server: {1}", Thread.CurrentThread.Name, deserializedPacket);
                            Console.WriteLine("[Bolt] [{0}] Sent to client: {1}", Thread.CurrentThread.Name, deserializedPacket);
                            byte[] buffer = deserializedPacket.ToArray();
                            Console.WriteLine("[Bolt] [{0}] Sent to client: {1}", Thread.CurrentThread.Name, BitConverter.ToString(buffer));
                            Console.WriteLine("[Bolt] [{0}] Sent to client len: {1}", Thread.CurrentThread.Name, buffer.Length);
                            ClientConnection.output.Write(buffer, 0, buffer.Length);
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
                        byte[] buffer = disconnectPacket.ToArray();
                        ClientConnection.output.Write(buffer, 0, buffer.Length);
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

