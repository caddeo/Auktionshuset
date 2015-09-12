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
            try
            {
                using (this._netStream = new NetworkStream(_clientSocket))
                using (this._writer = new StreamWriter(_netStream))
                using (this._reader = new StreamReader(_netStream))
                {
                    Run();
                }
            }
            catch
            {
                // ignored
                // TODO: 
                // client disconnect handling here
            }

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
                Broadcast("Server Klar til input");

                /* Subscribe broadcaster */
                _broadcaster.BroadcastMessage += this.Broadcast; 

                /* Find på en bedre løsning */
                while (HandleInput()) ; 
            }
            catch(Exception e)
            {
                Send(e.Message);
            }
            finally
            {
                /* Subscribe broadcaster */
                _broadcaster.BroadcastMessage -= this.Broadcast;
            }
        }

        private bool HandleInput() 
        {
            // Behandling af input fra klient
            string input = Recieve();

            if (input == null)
            {
                return false;
            }

            _broadcaster.Broadcast(input);

            return true;
        }

        public void Broadcast(string message)
        {
            Send(message);
        }
    }
}
