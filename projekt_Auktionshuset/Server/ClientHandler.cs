using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        public ClientHandler(Socket clientSocket, Broadcaster broadcaster)
        {
            this._clientSocket = clientSocket;
            this._broadcaster = broadcaster;
        }

        public void RunClient()
        {
            using (this._netStream = new NetworkStream(_clientSocket))
            using (this._writer = new StreamWriter(_netStream))
            using(this._reader = new StreamReader(_netStream))
            {
                Run();
            }
            this._clientSocket.Shutdown(SocketShutdown.Both);
            this._clientSocket.Close();
        }

        private void Send(string message)
        {
            _writer.WriteLine(message);
        }
        
        /* Spørger om input fra clienten */
        private string Recieve()
        {
            try
            {
                return _reader.ReadLine();
            }
            catch(Exception e)
            {
                /* De forskellige exceptions kan catches*/
                /* retunere fejl beskeden */
                return e.Message;
            }
        }

        private void Run()
        {
            try
            {
                bool running = true;

                while (running)
                {
                    Send("TEST Server klar");

                    /* Imens den kører - broadcast besked */
                    _broadcaster.BroadcastMessage += this.Broadcast;

                    string input = Recieve();

                    if (!HandleInput(input))
                    {
                        running = false;
                    }
                }
            }
            catch (Exception e)
            {
                /* Send en besked med exceptionen */
                Send(e.Message);
            }
            
            /* Hvis den stopper - så unsubscribe */
            _broadcaster.BroadcastMessage -= this.Broadcast;

        }

        private bool HandleInput(string rawinput)
        {
            string input = rawinput.Trim().ToLower();
            
            /* f.eks. så kan der laves 
                if(input == null)
                {
                return false
                }

                for at undgå noget bestemt input
            */
            
            _broadcaster.Broadcast(input);
            return true;
        }

        public void Broadcast(string message)
        {
            Send(message);
        }
    }
}
