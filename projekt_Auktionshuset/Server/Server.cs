using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    class Server
    {

        private Broadcaster _broadcaster;

        private bool running;

        public Server(int port, string ip)
        {
            _broadcaster = new Broadcaster();

            IPAddress IP = IPAddress.Parse(ip);
            TcpListener listener = new TcpListener(IP, port);

            running = true;

            listener.Start();

            while (running)
            {
                System.Console.WriteLine("Server klar");
                Socket clientSocket = listener.AcceptSocket();

                System.Console.WriteLine("Connection");
                ClientHandler handler = new ClientHandler(clientSocket, _broadcaster);

                Thread clientThread = new Thread(handler.RunClient);
                clientThread.Start();
            }

            
        }


    }
}
