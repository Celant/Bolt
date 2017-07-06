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
using System.Runtime.InteropServices;
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

        public Dictionary<ClientConnection, byte> Players = new Dictionary<ClientConnection, byte> ();

        private ConnectionThread Listener;

        private Thread ListenerThread;

		static void Main(string[] args)
		{
            Instance = new Bolt();
            Console.WriteLine("Bolt v0.1 beginning initialisation");

            Console.CancelKeyPress += delegate
			{
			    Stop();
			};

            Instance.Start();

            while (Instance.IsRunning)
            {
                string line = Console.ReadLine();
                if (line != null)
                {
                    if (line.Equals("stop"))
                    {
                        Stop();
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

        public static void Stop() {
            Instance.IsRunning = false;

            Console.WriteLine("Closing network ports");
            Thread.Sleep(500);

            foreach (KeyValuePair<ClientConnection, byte> pair in Instance.Players)
            {
                pair.Key.Destroy("Server is shutting down");
            }

            Instance.Listener.socket.Close();
            Instance.ListenerThread.Interrupt();
            Instance.ListenerThread.Join();
        }

        static bool ConsoleEventCallback(int eventType)
        {
            if (eventType == 2)
            {
                Stop();
            }
            return false;
        }
	}
}