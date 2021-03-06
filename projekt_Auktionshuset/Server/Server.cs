﻿using System;
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
        private bool _running;

        private string _servername;
        private int _port;


        public Server(int port, string ip)
        {
            _servername = ip;
            _port = port;
        }

        private void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            int timeleft = _auction.TimeLeft;
            Console.WriteLine("Time left : "+timeleft);

            switch (_auction.TimeLeft)
            {
                // switcher ved hvor lang tid der er tilbage
                case 17000:     // 18 sek
                    break;
                case 8000:  // 8
                    _broadcaster.Broadcast("MESSAGE");
                    _broadcaster.Broadcast("Første!");
                    Console.WriteLine("Første!");
                    break;
                case 3000:  // 3
                    _broadcaster.Broadcast("MESSAGE");
                    _broadcaster.Broadcast("Anden!");
                    Console.WriteLine("Anden!");
                    break;
                case 0:     // 0
                    _auction.Timer.Stop();
                    _broadcaster.Broadcast("MESSAGE");
                    _broadcaster.Broadcast("Tredje! Solgt til " + _auction.HighestBidder.Name + " for " + _auction.CurrentPrice);
                    Console.WriteLine("Solgt til IP " + _auction.HighestBidder.IPAddress + " (" + _auction.HighestBidder.Name + ") for " + _auction.CurrentPrice);
                    break;
            }

            _auction.SetTimeLeft(timeleft - 1000);
        }

        private void HandleInput()
        {
            while (_running)
            {
                Console.WriteLine("Server klar til input");
                string input = Console.ReadLine();

                try
                {
                    switch (input?.Trim().ToUpper())
                    {
                        case "NEW":
                            if (_auction == null)
                            {
                                bool inputrunning = true;

                                while (inputrunning)
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

                                    ConnectClients();
                                    _broadcaster.SetAuction(auction);

                                    _auction.Timer.Elapsed += OnTimedEvent;
                                    inputrunning = false;
                                }
                            }
                            else
                            {
                                Console.WriteLine("Auktion allerede i gang");
                            }
                            break;
                        case "STOP":
                            StopAuction();
                            break;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("HandleInput() " + e.Message);
                }
            }
        }

        // sender "NEWAUCTION" til klient så de opretter forbindelse
        private void ConnectClients()
        {
            _broadcaster.Broadcast("NEWAUCTION");
            _broadcaster.Broadcast("MESSAGE");
            _broadcaster.Broadcast("Ny auktion sættes i gang");
        }

        private void StopAuction()
        {
            _auction.Timer.Stop();
            _auction = null;
            _broadcaster.SetAuction(null);

            _broadcaster.Broadcast("MESSAGE");
            _broadcaster.Broadcast("Auktion stoppet af serveren");
        }


        public void Run()
        {
            _broadcaster = new Broadcaster();

            IPAddress ip = IPAddress.Parse(_servername);
            TcpListener listener = new TcpListener(ip, _port);

            /* Gør så at serveren kan skrive input*/
            Thread serverInputThread = new Thread(HandleInput);
            serverInputThread.Start();

            _running = true;

            listener.Start();

            Console.WriteLine("Skriv \'new\' for at starte en ny auktion.");

            while (_running)
            {
                System.Console.WriteLine("Server klar til bruger");
                /* En socket forbinder*/
                Socket clientSocket = listener.AcceptSocket();

                /* Lav en ny client handler til forbindelsen */
                ClientHandler handler = new ClientHandler(clientSocket, _broadcaster);
                handler.SetAuction(_auction);

                /* Start det i en ny tråd */
                Thread clientThread = new Thread(handler.RunClient);

                /* Start trådene */
                clientThread.Start();
            }
        }
    }
}
