﻿//
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

        public List<ClientConnection> Players = new List<ClientConnection> ();

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
                    switch (line)
                    {
                        case "stop":
                            Stop();
                            break;
                        case "connect":
                            SwitchServer();
                            break;
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

            Console.WriteLine("[Bolt] Shutting down");
            Thread.Sleep(500);

            for (int i = Instance.Players.Count - 1; i >= 0; i--)
            {
                Instance.Players[i].Destroy("Server is shutting down");
                Instance.Players.RemoveAt(i);
            }

            Instance.Listener.socket.Close();
            Instance.ListenerThread.Interrupt();
            Instance.ListenerThread.Join();

            Environment.Exit(0);
        }

        public static void SwitchServer() {
            for (int i = 0; i < Instance.Players.Count; i++)
            {
                Instance.Players[i].Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 7877));
            }
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