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
using TShockProxy.Connection;
using TShockProxy.Proxy;

namespace TShockProxy
{
	class TShockProxy
	{
        public static TShockProxy Instance;

        public volatile bool IsRunning;

        public List<ProxiedPlayer> Players = new List<ProxiedPlayer>();

        private ConnectionThread Listener;

        private Thread ListenerThread;

		static void Main(string[] args)
		{
            Instance = new TShockProxy();
            Console.WriteLine("TShockProxy by George has been initialised");
            Console.WriteLine("Binding network ports");

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