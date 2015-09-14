using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Broadcaster
    {
        public delegate void BroadcastEvent(string message);

        public event BroadcastEvent BroadcastMessage;


        private List<ClientHandler> _clients;

        public Broadcaster()
        {
            this._clients = new List<ClientHandler>();
        }

        /* Subscribe fra listen */
        public void Subscribe(ClientHandler client)
        {
            _clients.Add(client);
            Console.WriteLine("DEBUG. Subscribe()");
        }

        /* Unsubscribe fra listen */
        public void Unsubscribe(ClientHandler client)
        {
            _clients.Remove(client);
            Console.WriteLine("DEBUG. Unsubscribe()");
        }

        /* Broadcast til clienter på listen */
        public void Broadcast(string message)
        {
            if (this.BroadcastMessage != null)
            {
                BroadcastMessage(message);
            }
        }
    }
}
