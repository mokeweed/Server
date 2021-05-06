﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace Server
{
    class Program
    {
        public static List<string> msg_Queue = new List<string>();
        public static List<Thread> cli_Conns = new List<Thread>();
        public static List<TCPChat> cli_List = new List<TCPChat>();

        private const int tcp_Port = 1337;

        static void Main(string[] args)
        {
            Boolean done = false;
            Mutex mutex = new Mutex();

            IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, tcp_Port);

            Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            listener.Bind(localEndPoint);
            listener.Listen(10);

            Console.WriteLine("Server started. Listening on port {0}", tcp_Port);

            QueryReader reader = new QueryReader(msg_Queue, cli_List, mutex);
            Thread t = new Thread(reader.Run);
            t.Start();

            while (done == false)
            {
                Socket handler = listener.Accept();
                Console.WriteLine("A client had connected!");
                mutex.WaitOne();
                cli_List.Add(new TCPChat(handler, msg_Queue, mutex));
                cli_Conns.Add(new Thread(cli_List[cli_List.Count - 1].Run));
                cli_Conns[cli_Conns.Count - 1].Start();
                mutex.ReleaseMutex();
            }

        }
    }
}