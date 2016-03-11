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
using Bolt.Proxy;

namespace Bolt.Connection
{
    public class ClientConnection : GenericConnection
    {
        public LoginQueue loginQueue;
        public ServerConnection CurrentServer;

        public NATManager TranslationManager;

        private UpstreamBridge upstreamBridge;
        private Thread upstreamBridgeThread;

        private DownstreamBridge downstreamBridge;
        private Thread downstreamBridgeThread;

        public ClientConnection(Socket socket, PacketInputStream input, NetworkStream output, LoginQueue loginQueue, NATManager translationManager)
            : base(socket, input, output)
        {
            this.loginQueue = loginQueue;
            username = this.loginQueue.playerInfo.Name;
            this.TranslationManager = translationManager;
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

        public void Register(int slot) {
            Bolt.Instance.Players[slot] = this;
        }

        private void Destroy(string reason) {
            if (Bolt.Instance.IsRunning)
            {
                Bolt.Instance.Players[0];
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
}

