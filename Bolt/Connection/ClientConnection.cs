//
//  ClientConnection.cs
//
//  Author:
//       Josh Harris <celant@celantinteractive.com>
//
//  Copyright (c) 2016 Celant
using LibMultiplicity.Packets.v1241;
using LibMultiplicity.Packets.v1302;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Bolt.Protocol;
using Bolt.Exception;

namespace Bolt.Connection
{
    public class ClientConnection : GenericConnection
    {
        public LoginQueue loginQueue;
        public ServerConnection CurrentServer;

        private UpstreamBridge upstreamBridge;
        private Thread upstreamBridgeThread;

        private DownstreamBridge downstreamBridge;
        private Thread downstreamBridgeThread;

        public ClientConnection(Socket socket, PacketInputStream input, NetworkStream output, LoginQueue loginQueue) : base(socket, input, output)
        {
            this.loginQueue = loginQueue;
            username = this.loginQueue.playerInfo.Name;
            Console.WriteLine("User {0} has connected to slot {1}", username, loginQueue.playerInfo.PlayerID);
        }

        public void Connect(IPEndPoint address) {
            try {
                ServerConnection NewServer = ServerConnection.connect(address, loginQueue);
                if (CurrentServer == null) {
                    upstreamBridge = new UpstreamBridge(this);
                    upstreamBridgeThread = new Thread(upstreamBridge.Run);
                    upstreamBridgeThread.Start();
                }
                if (downstreamBridge != null) {
                    downstreamBridge.Interrupt();
                    downstreamBridgeThread.Join();
                }
                downstreamBridge = new DownstreamBridge(this);
                downstreamBridgeThread = new Thread(downstreamBridge.Run);
                CurrentServer = NewServer;
                downstreamBridgeThread.Start();

                ContinueConnecting2 continueConnecting2 = new ContinueConnecting2();
                Console.WriteLine(continueConnecting2);
                byte[] buf = continueConnecting2.ToArray();
                Console.WriteLine(continueConnecting2.ID);
                CurrentServer.output.Write(buf, 0, buf.Length);
            } catch (KickException e) {
                Disconnect disconnectPacket = new Disconnect(e.Message);
                byte[] buffer = disconnectPacket.ToArray();
                output.Write(buffer, 0, buffer.Length);
            }
        }

        public void Register() {
            Bolt.Instance.Players.Add(this);
        }

        private void Destroy(string reason) {
            if (Bolt.Instance.IsRunning)
            {
                Bolt.Instance.Players.Remove(this);
            }
            if (upstreamBridge != null)
            {
                upstreamBridge.Interrupt();
                upstreamBridgeThread.Join();
            }
            if (downstreamBridge != null)
            {
                downstreamBridge.Interrupt();
                downstreamBridgeThread.Join();
            }
            Disconnect(reason);
            if (CurrentServer != null)
            {
                CurrentServer.Disconnect("Quitting");
            }
        }
    }

    public class UpstreamBridge : ProcessThread
    {
        protected ClientConnection clientConnection;

        public UpstreamBridge(ClientConnection parent)
        {
            this.clientConnection = parent;
        }

        public override void Run() {
            while (!Interrupted())
            {
                try {
                    byte[] packet = clientConnection.input.readPacket();
                    clientConnection.CurrentServer.output.Write(packet, 0, packet.Length);
                } catch (EndOfStreamException e) {
                    Console.WriteLine("Reached end of stream");
                    Console.WriteLine(e.Message);
                } catch (System.Exception e) {
                    Console.WriteLine(e.Message);
                }
            }
        }
    }

    public class DownstreamBridge : ProcessThread
    {
        protected ClientConnection clientConnection;

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

