//
//  ConnectionThread.cs
//
//  Author:
//       Josh Harris <celant@celantinteractive.com>
//
//  Copyright (c) 2016 Celant
using DotNetty.Buffers;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using TShockProxy.Proxy;

namespace TShockProxy.Connection
{
    public class ConnectionThread
    {
        public Socket socket;

        private static ManualResetEvent ConnectionAccepted = new ManualResetEvent(false);

        public ConnectionThread(IPEndPoint address)
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(address);
            socket.Listen(100);
            Console.WriteLine("Succesfully bound to {0}", address);
        }

        public void Run()
        {
            while (TShockProxy.Instance.IsRunning)
            {
                try {
                    ConnectionAccepted.Reset();

                    Console.WriteLine("Waiting for a connection...");
                    socket.BeginAccept(new AsyncCallback(AcceptCallback), socket);

                    ConnectionAccepted.WaitOne();
                } catch (Exception e) {
                    Console.WriteLine(e.Message);
                }
            }
        }

        public static void AcceptCallback(IAsyncResult ar)
        {
            ConnectionAccepted.Set();

            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

            InitialHandler player = new InitialHandler(handler);
            bool result = ThreadPool.QueueUserWorkItem(new WaitCallback(o => player.Run()));
            Console.WriteLine("{0} has connected", handler.RemoteEndPoint.ToString());
            //TShockProxy.Instance.Players.Add("Player1", player);
            if (!result)
            {
                Console.WriteLine("Failed to queue socket processor");
            }
        }
    }
}

