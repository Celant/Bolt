//
//  ClientConnection.cs
//
//  Author:
//       Josh Harris <celant@celantinteractive.com>
//
//  Copyright (c) 2016 Celant
using Multiplicity.Packets;
using Multiplicity.Packets.Models;
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
        public ServerConnection CurrentServer;

        private ClientBridge upstreamBridge;
        private Thread upstreamBridgeThread;

        private ServerBridge downstreamBridge;
        private Thread downstreamBridgeThread;

        public ClientConnection (Socket socket, PacketInputStream input, NetworkStream output)
            : base (socket, input, output)
        {
            Console.WriteLine ("[Bolt] [ClientConnection] Received join request from player");
        }

        private void Start(IPEndPoint address)
        {
            try
            {
                ServerConnection Server = ServerConnection.connect(address);
                CurrentServer = Server;
                upstreamBridge = new ClientBridge(this);
                upstreamBridgeThread = new Thread(upstreamBridge.Run);
                upstreamBridgeThread.Name = "UpstreamBridge-" + Bolt.Instance.Players.IndexOf(this);
                upstreamBridgeThread.Start();

                downstreamBridge = new ServerBridge(this);
                downstreamBridgeThread = new Thread(downstreamBridge.Run);
                downstreamBridgeThread.Name = "DownstreamBridge-" + Bolt.Instance.Players.IndexOf(this);
                downstreamBridgeThread.Start();

                ContinueConnecting continueConnecting = new ContinueConnecting()
                {
                    PlayerID = CurrentServer.ServerPlayerID
                };

                Console.WriteLine("[Bolt] [ClientConnection] Sending to client: " + continueConnecting);
                byte[] buf = continueConnecting.ToArray();

                output.Write(buf, 0, buf.Length);

            }
            catch (KickException e)
            {
                NetworkText disconnectReason = new NetworkText()
                {
                    TextMode = 0,
                    Text = e.Message
                };
                Disconnect disconnectPacket = new Disconnect();
                disconnectPacket.Reason = disconnectReason;

                byte[] buffer = disconnectPacket.ToArray();
                output.Write(buffer, 0, buffer.Length);
            }
        }

        public void Connect (IPEndPoint address)
        {
            
            try
            {
                if (CurrentServer == null)
                {
                    Start(address);
                    return;
                }

                upstreamBridge.Interrupt();
                upstreamBridgeThread.Join();
                downstreamBridge.Interrupt();
                downstreamBridgeThread.Join();

                ServerConnection NewServer = ServerConnection.connect (address);

                CurrentServer = NewServer;

                upstreamBridge = new ClientBridge (this);
                upstreamBridgeThread = new Thread (upstreamBridge.Run);
                upstreamBridgeThread.Name = "UpstreamBridge-" + Bolt.Instance.Players.IndexOf(this);
                upstreamBridgeThread.Start();

                downstreamBridge = new ServerBridge(this);
                downstreamBridgeThread = new Thread(downstreamBridge.Run);
                downstreamBridgeThread.Name = "DownstreamBridge-" + Bolt.Instance.Players.IndexOf(this);
                downstreamBridgeThread.Start();

                ContinueConnecting continueConnecting = new ContinueConnecting () {
                    PlayerID = CurrentServer.ServerPlayerID
                };

                TeleportationPotion teleportationPotion = new TeleportationPotion();

                Console.WriteLine ("[Bolt] [ClientConnection] Sending to client: " + continueConnecting);
                byte [] buf = continueConnecting.ToArray ();

                output.Write(buf, 0, buf.Length);

            } 
            catch (KickException e)
            {
                NetworkText disconnectReason = new NetworkText ()  {
                    TextMode = 0,
                    Text = e.Message
                };
                Disconnect disconnectPacket = new Disconnect ();
                disconnectPacket.Reason = disconnectReason;

                byte [] buffer = disconnectPacket.ToArray ();
                output.Write (buffer, 0, buffer.Length);
            }
        }

        public void Register (byte slot)
        {
            Bolt.Instance.Players.Add (this);
        }

        public void Destroy (string reason)
        {
            Disconnect(reason);
            if (CurrentServer != null)
            {
                CurrentServer.Disconnect(reason);
            }

            upstreamBridge.Interrupt ();
            upstreamBridgeThread.Join ();
            downstreamBridge.Interrupt ();
            downstreamBridgeThread.Join ();

            Console.WriteLine("[Bolt] [ClientConnection] Dropped player {0}: {1}", CurrentServer.ServerPlayerID, reason);
        }
    }
}

