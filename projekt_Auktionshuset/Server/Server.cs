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
        private Aktion _aktion;
        private bool running;

        public Server(int port, string ip)
        {
            _broadcaster = new Broadcaster();

            var IP = IPAddress.Parse(ip);
            TcpListener listener = new TcpListener(IP, port);

            Console.WriteLine("Server klar til input");

            while (!running)
            {
                HandleInput();
            }

            listener.Start();

            System.Console.WriteLine("Server klar til bruger");

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

        public void HandleInput()
        {
            string input = Console.ReadLine();

            if (input?.Trim().ToLower() == "new")
            {
                // name,  description, estimatedPrice, currentPrice, highestBidder

                try
                {
                    string name = Console.ReadLine();
                    string description = Console.ReadLine();
                    double estimatedprice = double.Parse(Console.ReadLine());
                    double currentprice = double.Parse(Console.ReadLine());

                    Aktion aktion = new Aktion(name, description, estimatedprice, currentprice, null);

                    _aktion = aktion;

                    running = true;
                }
                catch(Exception exception)
                {
                    Console.WriteLine(exception.Message);
                }
            }
        }
    }
}
