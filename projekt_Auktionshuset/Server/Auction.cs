using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Auction
    {
        public string Name { get { return _name; } }
        public string Description { get { return _description; } }
        public double EstimatedPrice { get { return _estimatedprice; } }
        public double CurrentPrice { get { return _currentprice; } }
        public double TimeLeft { get { return _timeleft; } }
        public Client HighestBidder { get { return _highestbidder; } }
        // is running?

        // name, desc, estimatedprice, currentprice, timeleft, highestbider
        private string _name;
        private string _description;
        private double _estimatedprice;
        private double _currentprice;
        private double _timeleft;
        private Client _highestbidder;

        public Auction(string name, string description, double estimatedPrice, double currentPrice,
            Client highestBidder)
        {
            this._name = name;
            this._description = description;
            this._estimatedprice = estimatedPrice;
            this._currentprice = currentPrice;
            this._timeleft = 11.0; // 11 == ikke begyndt
            this._highestbidder = highestBidder;
        }

        public void SetHighestBid(Client client, double bid)
        {
            _highestbidder = client;
            _currentprice = bid;
        }
    }
}
