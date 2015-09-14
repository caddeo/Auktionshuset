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

            var IP = IPAddress.Parse(ip);
            TcpListener listener = new TcpListener(IP, port);

            running = true;

            listener.Start();
            
            System.Console.WriteLine("Server klar");

            while (running)
            {
                /* En socket forbinder*/
                Socket clientSocket = listener.AcceptSocket();

                /* Lav en ny client handler til forbindelsen */
                ClientHandler handler = new ClientHandler(clientSocket, _broadcaster);

                /* Start det i en ny tråd */
                Thread clientThread = new Thread(handler.RunClient);
                clientThread.Start();
            }
        }
    }
}
