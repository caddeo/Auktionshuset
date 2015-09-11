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

        /* Tilmeld fra listen */
        public void Tilmeld(ClientHandler client)
        {
            _clients.Add(client);
        }

        /* Afmeld fra listen */
        public void Afmeld(ClientHandler client)
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
