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

        // TODO:
        // listen skal i controller også
        // tilmeld og afmeld skal fra clienthanler til controller
        private List<ClientHandler> _clients;

        public List<Client> Clients; 

        public Broadcaster()
        {
            this._clients = new List<ClientHandler>();
            this.Clients = new List<Client>();
        }

        /* Subscribe fra listen */
        public void Subscribe(ClientHandler client)
        {
            _clients.Add(client);
        }

        /* Unsubscribe fra listen */
        public void Unsubscribe(ClientHandler client)
        {
            _clients.Remove(client);
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
