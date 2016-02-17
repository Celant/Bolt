//
//  PacketWrapper.cs
//
//  Author:
//       Josh Harris <celant@celantinteractive.com>
//
//  Copyright (c) 2016 Celant
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Bolt.Connection;
using Bolt.Proxy;

namespace Bolt
{
	class Bolt
	{
        public static Bolt Instance;

        public volatile bool IsRunning;

        public List<ClientConnection> Players = new List<ClientConnection>();

        private ConnectionThread Listener;

        private Thread ListenerThread;

		static void Main(string[] args)
		{
            Instance = new Bolt();
            Console.WriteLine("Bolt by George has been initialised");
            Console.WriteLine("Starting up networking server");

            Instance.Start();

            while (Instance.IsRunning)
            {
                string line = Console.ReadLine();
                if (line != null)
                {
                    if (line.Equals("stop"))
                    {
                        Instance.Stop();
                    }
                }
            }
		}

        public void Start() {
            IsRunning = true;

            IPAddress Address = IPAddress.Parse("0.0.0.0");
            IPEndPoint LocalEndpoint = new IPEndPoint(Address, 7777);

            Listener = new ConnectionThread(LocalEndpoint);
            ListenerThread = new Thread(Listener.Run);

            ListenerThread.Start();
        }

        public void Stop() {
            IsRunning = false;

            Console.WriteLine("Closing network ports");
            Thread.Sleep(500);
            Listener.socket.Close();
            ListenerThread.Join();
        }
	}
}