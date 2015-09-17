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
        private Auction _auction;
        private bool running;

        public Server(int port, string ip)
        {
            _broadcaster = new Broadcaster();

            var IP = IPAddress.Parse(ip);
            TcpListener listener = new TcpListener(IP, port);

            Console.WriteLine("Server klar til input");
            while (!running)
            {
                Console.WriteLine("Skriv \'new\' for at starte en ny auktion.");
                HandleInput();
            }

            listener.Start();

            while (running)
            {
                System.Console.WriteLine("Server klar til bruger");
                /* En socket forbinder*/
                Socket clientSocket = listener.AcceptSocket();

                /* Lav en ny client handler til forbindelsen */
                ClientHandler handler = new ClientHandler(clientSocket, _broadcaster);
                handler.SetAuction(_auction);

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
                    Console.WriteLine("Auction's navn:");
                    string name = Console.ReadLine();

                    Console.WriteLine("Auction's beskrivelse:");
                    string description = Console.ReadLine();

                    Console.WriteLine("Auction's estimerede pris:");
                    double estimatedprice = double.Parse(Console.ReadLine());

                    Console.WriteLine("Auction's pris:");
                    double currentprice = double.Parse(Console.ReadLine());

                    Auction auction = new Auction(name, description, estimatedprice, currentprice, null);

                    _auction = auction;

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
