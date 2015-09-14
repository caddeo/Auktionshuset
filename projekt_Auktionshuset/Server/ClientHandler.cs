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

        // serverens runtime status
        private bool _running; 

        public ClientHandler(Socket clientSocket, Broadcaster broadcaster)
        {
            this._clientSocket = clientSocket;
            this._broadcaster = broadcaster;

            /* Udskriver IP'en */
            string IP = clientSocket.RemoteEndPoint.ToString();
            IPAddress ip = IPAddress.Parse(IP.Substring(0, IP.Length - 5));

            Console.WriteLine(IP+" connected");
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
                // Brug Trim() for at sikre at det er "RAW" input
                // Indtil der bliver inputtet "slut" så kør
                if (input.Trim().ToLower() == "quit")
                {
                    _running = false;
                }

                if (input.Trim().ToLower() == "count")
                {
                    _broadcaster.Broadcast(_broadcaster.BroadcastClientCount());
                }

                _broadcaster.Broadcast("CLIENT: " + input);
            }
            // hvis den er null
            else
            {
                _running = false;
            }
        }

        public void Broadcast(string message)
        {
            Send(message);
        }
    }
}
