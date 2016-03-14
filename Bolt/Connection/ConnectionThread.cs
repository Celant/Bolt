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
using Bolt.Proxy;

namespace Bolt.Connection
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
            while (Bolt.Instance.IsRunning)
            {
                try {
                    //ConnectionAccepted.Reset();

                    Console.WriteLine("Waiting for a connection...");
                    //socket.BeginAccept(new AsyncCallback(AcceptCallback), socket);

                    Socket acceptedSocket = socket.Accept();

                    AcceptCallback(acceptedSocket);

                    //ConnectionAccepted.WaitOne();
                } catch (System.Exception e) {
                    Console.WriteLine(e.Message);
                }
            }
        }

        public static void AcceptCallback(Socket socket)
        {
            Socket handler = socket;

            InitialHandler player = new InitialHandler(handler);
           // bool result = ThreadPool.QueueUserWorkItem(new WaitCallback(o => player.Run()));

            Thread t = new Thread(() => player.Run());
            t.IsBackground = true;
            t.Start();

            Console.WriteLine("{0} has connected", handler.RemoteEndPoint.ToString());
            //TShockProxy.Instance.Players.Add("Player1", player);
            //if (!result)
            //{
            //    Console.WriteLine("Failed to queue socket processor");
            //}
        }
    }
}

