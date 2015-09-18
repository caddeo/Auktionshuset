using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    class Auction
    {
        public string Name { get { return _name; } }
        public string Description { get { return _description; } }
        public double EstimatedPrice { get { return _estimatedprice; } }
        public double CurrentPrice { get { return _currentprice; } }
        public int TimeLeft { get { return _timeLeft; } }
        public Client HighestBidder { get { return _highestbidder; } }

        // thread locks
        private Object _bidLock = new Object();
        private Object _timeLock = new Object();

        // name, desc, estimatedprice, currentprice, timeleft, highestbider
        private string _name;
        private string _description;
        private double _estimatedprice;
        private double _currentprice;
        private int _timeLeft;
        private Client _highestbidder;

        public System.Timers.Timer Timer;
        public Auction(string name, string description, double estimatedPrice, double currentPrice,
            Client highestBidder)
        {
            this._name = name;
            this._description = description;
            this._estimatedprice = estimatedPrice;
            this._currentprice = currentPrice;
            this._highestbidder = highestBidder;
            this._timeLeft = 17000;

            // Timer
            this.Timer = new System.Timers.Timer();
            this.Timer.Interval = 1000;

        }

        public void SetHighestBid(Client client, double bid)
        {
            lock (_bidLock)
            {
                _highestbidder = client;
                _currentprice = bid;
            }
        }

        public void SetTimeLeft(int time)
        {
            lock (_timeLock)
            {
                _timeLeft = time;
            }
        }
    }
}
