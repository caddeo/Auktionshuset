using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class ClientHandler
    {

        private Socket _clientSocket;
        private NetworkStream _netStream;
        private StreamWriter _writer;
        private StreamReader _reader;

        private Broadcaster _broadcaster;

        private Client client;
        private Auction _auction;

        // serverens runtime status
        private bool _running; 

        public ClientHandler(Socket clientSocket, Broadcaster broadcaster)
        {
            this._clientSocket = clientSocket;
            this._broadcaster = broadcaster;

            _auction = null;

            /* Udskriver IP'en */
            string clientIp = clientSocket.RemoteEndPoint.ToString();
            IPAddress ip = IPAddress.Parse(clientIp.Substring(0, clientIp.Length - 5));

            client = new Client(clientIp.Substring(0, clientIp.Length - 5));
            _broadcaster.Clients.Add(client);

            Console.WriteLine(clientIp +" connected");
        }

        public void RunClient()
        {
            try
            {
                using (this._netStream = new NetworkStream(_clientSocket))
                using (this._writer = new StreamWriter(_netStream))
                using (this._reader = new StreamReader(_netStream))
                {
                    Run();
                }
            }
            catch (Exception e)
            {
                // Skriv en fejlmelding til serveren
                Console.WriteLine("RunClient() " + e.Message);
            }

            /* Disconnect */
            this._netStream.Close();
            this._writer.Close();
            this._reader.Close();

            this._clientSocket.Shutdown(SocketShutdown.Both);
            this._clientSocket.Close();
        }

        private void Send(string message)
        {
            _writer.WriteLine(message);
            _writer.Flush();
        }
        
        /* Spørger om input fra clienten */
        private string Recieve()
        {
            try
            {
                return _reader.ReadLine();
            }
            /* Kan bruges til at undgå bruger input fejl */
            /* if(e is Exception...) */
            catch(Exception e)
            {
                /* De forskellige exceptions kan catches*/
                /* retunere fejl beskeden */
                return "Recieve() "+ e.Message;
            }
        }

        private void Run()
        {
            try
            {
                Broadcast("Server Klar til input");
                _running = true;

                /* Subscribe broadcaster */
                _broadcaster.BroadcastMessage += this.Broadcast;
                _broadcaster.Subscribe(this);

                /* Imens den er true så kør*/
                while (_running)
                {
                    /* Imens den kører så handle user input*/
                    HandleInput();
                }
            }
            catch (Exception e)
            {
                //if (e is NullReferenceException)
                //{
                //    // ignore
                //}

                Console.WriteLine("Run() " + e.Message);
            }
            finally
            {
                /* Unsubscribe broadcaster */
                _broadcaster.BroadcastMessage -= this.Broadcast;
                _broadcaster.Unsubscribe(this);
            }
        }



        private void HandleInput() 
        {
            // Behandling af input fra klient
            string input = Recieve();

            // Tjekker om input er null (den er typisk null når man tryker kryds)
            if (input != null)
            {
                /*if (input.Trim().ToLower() == "usercount")

                {
                    _broadcaster.Broadcast(_broadcaster.BroadcastClientCount());
                }*/

                switch (input.Trim().ToUpper())
                {
                    case "CONNECTED":
                        Send("DESCRIPTION");
                        Send(_auction.Description);
                        Send("ESTIMATEDPRICE");
                        Send(_auction.CurrentPrice.ToString());
                        Send("CURRENTPRICE");
                        Send(_auction.CurrentPrice.ToString());
                        break;
                    case "BID":
                        string clientBidInput = Recieve();
                        HandleBid(clientBidInput);
                        break;
                    case "DISCONNECT":
                        _running = false;
                        break;
                }

                //_broadcaster.Broadcast("CLIENT: " + input);
            }

            // hvis den er null
            else
            {
                _running = false;
            }
        }

        private void HandleBid(string bid)
        {
            try
            {
                double clientBid = double.Parse(bid);

                if (clientBid > _auction.CurrentPrice)
                {
                    // bud er ok
                    client.SetCurrentBid(clientBid);
                    _auction.SetHighestBid(client, clientBid);

                    _broadcaster.Broadcast("NEWBIDDER");
                    _broadcaster.Broadcast(client.Name + " er den nye højeste bydende (" + clientBid + " kr.)");
                    _broadcaster.Broadcast("NEWHIGHEST");
                    _broadcaster.Broadcast(clientBid.ToString());
                }
                else
                {
                    // bud er ikke ok
                    Send("Dit bud er ikke over "+_auction.CurrentPrice+" kr!");
                }
            }
            catch(Exception e) 
            {
                Console.WriteLine("HandleBid() "+e.Message);
            }

        }

        public void SetAuction(Auction auction)
        {
            _auction = auction;
        }

        public void Broadcast(string message)
        {
            Send(message);
        }
    }
}
