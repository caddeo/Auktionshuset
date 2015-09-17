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
            this._name = GenerateName(ipaddress);
            this._currentbid = 0;
        }

        // split en ip op og giv et navn til clienten
        public string GenerateName(string ip)
        {
            string[] seperators = { ".", ":" };
            string[] name = ip.Split(seperators, StringSplitOptions.RemoveEmptyEntries);

            return name[name.Length - 2];
        }

        public void SetCurrentBid(double currentbid)
        {
            this._currentbid = currentbid;
        }

    }
}
