using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Client
    {
        public string IPAddress { get { return _ipaddress; } }
        public string Name { get { return _name; } }
        public double CurrentBid { get { return _currentbid; } }

        private string _ipaddress;
        private string _name;
        private double _currentbid;

        public Client(string ipaddress)
        {
            this._ipaddress = ipaddress;
            _name = ipaddress.Substring(ipaddress.Length-3, 3);
            _currentbid = 0;
        }

        public void SetCurrentBid(double currentbid)
        {
            _currentbid = currentbid;
        }
    }
}
