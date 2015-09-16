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
            serverHandler.RecieveDescriptionEvent += OnRecieveDescriptionEvent;
            serverHandler.RecieveEstimatedEvent += OnRecieveEstimatedEvent;
            serverHandler.WriteToSocket("CONNECTED", "");
        }
        private void OnRecieveNewBidderEvent(string bidder)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.Invoke(new ServerHandler.RecieveEventType(OnRecieveNewBidEvent), bidder);
                return;
            }
            Console.WriteLine("Auktion:" + bidder);
            ListBoxAuctionLog.Items.Add(bidder);

        }
        private void OnRecieveNewBidEvent(string bid)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.Invoke(new ServerHandler.RecieveEventType(OnRecieveNewBidEvent), bid);
                return;
            }
            TextBlockCurrentPrice.Dispatcher.Invoke(new ServerHandler.RecieveEventType(OnRecieveNewBidEvent), bid);
            Console.WriteLine("GUI:" + bid);
        }

        private void OnRecieveDescriptionEvent(string desc)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.Invoke(new ServerHandler.RecieveEventType(OnRecieveDescriptionEvent), desc);
                return;
            }
            TextBlockDescription.Dispatcher.Invoke(new ServerHandler.RecieveEventType(OnRecieveDescriptionEvent), desc);
            Console.WriteLine("GUI:" + desc);
        }
        private void OnRecieveEstimatedEvent(string price)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.Invoke(new ServerHandler.RecieveEventType(OnRecieveEstimatedEvent), price);
                return;
            }
            TextBlockDescription.Dispatcher.Invoke(new ServerHandler.RecieveEventType(OnRecieveDescriptionEvent), price);
            Console.WriteLine("GUI:" + price);
        }

        private void ButtonSend_Click(object sender, RoutedEventArgs e)
        {
            serverHandler.WriteToSocket("BID", TextboxUserInput.Text);
        }

        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            serverHandler.WriteToSocket("DISCONNECT", "");
        }
    }
}
