using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Reflection;

namespace projekt_Auktionshuset
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ServerHandler serverHandler;
        public MainWindow()
        {
            InitializeComponent();
            serverHandler = new ServerHandler("127.0.0.1", 12000);
            serverHandler.Open();
            serverHandler.RecieveNewBidderEvent += OnRecieveNewBidderEvent;
            serverHandler.RecieveNewHighestEvent += OnRecieveNewBidEvent;
        }
        private void OnRecieveNewBidderEvent(string bidder)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.Invoke(new ServerHandler.RecieveEventType(OnRecieveNewBidEvent), bidder);
                return;
            }
            Console.WriteLine("GUI:" + bidder);
            ListBoxAuctionLog.Items.Add(bidder);

        }
        private void OnRecieveNewBidEvent(string bid)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.Invoke(new ServerHandler.RecieveEventType(OnRecieveNewBidEvent), bid);
                return;
            }
            string hej = hej;
            DataGridAuctionInfo.Columns[0].Dispatcher.Invoke(new ServerHandler.RecieveEventType(OnRecieveNewBidEvent), bid);
            Console.WriteLine("GUI:" + bid);
            textBlockRecieve.Text += bid;
        }

    }
}
