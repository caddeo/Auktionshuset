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
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace projekt_Auktionshuset
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<string> AuctionMessages;
        ServerHandler serverHandler;
        public MainWindow()
        {
            InitializeComponent();
            AuctionMessages = new List<string>();
            ListBoxAuctionLog.ItemsSource = AuctionMessages;
            serverHandler = new ServerHandler("127.0.0.1", 12000);
            serverHandler.Open();
            serverHandler.RecieveNewBidderEvent += OnRecieveNewBidderEvent;
            serverHandler.RecieveNewHighestEvent += OnRecieveNewBidEvent;
            serverHandler.RecieveDescriptionEvent += OnRecieveDescriptionEvent;
            serverHandler.RecieveEstimatedEvent += OnRecieveEstimatedEvent;
            serverHandler.RecieveMessageEvent += OnRecieveMessageEvent;
            serverHandler.RecieveDisconnectEvent += OnRecieveDisconnectEvent;
            serverHandler.RecieveNameEvent += OnRecieveNameEvent;
            serverHandler.WriteToSocket("CONNECTED", "");
            TextboxUserInput.Focus();
        }
        private void OnRecieveNewBidderEvent(string bidder)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(new ServerHandler.RecieveEventType(OnRecieveNewBidderEvent), bidder);
                return;
            }
            AuctionMessages.Add("Nyt bud: " + bidder);
            ListBoxAuctionLog.ScrollIntoView(ListBoxAuctionLog.Items[ListBoxAuctionLog.Items.Count - 1]);
        }
        private void OnRecieveNewBidEvent(string bid)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(new ServerHandler.RecieveEventType(OnRecieveNewBidEvent), bid);
                return;
            }
            TextBlockCurrentPrice.Text = "Nuværende bud: " + bid;
        }

        private void OnRecieveDescriptionEvent(string desc)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(new ServerHandler.RecieveEventType(OnRecieveDescriptionEvent), desc);
                return;
            }
            TextBlockDescription.Text = "Beskrivelse: \n" + desc;
        }
        private void OnRecieveEstimatedEvent(string price)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(new ServerHandler.RecieveEventType(OnRecieveEstimatedEvent), price);
                return;
            }
            TextBlockEstimatedPrice.Text = "Estimeret pris: " + price;
        }

        private void OnRecieveMessageEvent(string text)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(new ServerHandler.RecieveEventType(OnRecieveMessageEvent), text);
                return;
            }
            AuctionMessages.Add("Auktion besked: " + text);
            ListBoxAuctionLog.ScrollIntoView(ListBoxAuctionLog.Items[ListBoxAuctionLog.Items.Count - 1]);
        }
        private void OnRecieveNameEvent(string name)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(new ServerHandler.RecieveEventType(OnRecieveNameEvent), name);
                return;
            }
            LabelAuctionName.Content = ("Auktionnavn: " + name);
        }

        private void ButtonSend_Click(object sender, RoutedEventArgs e)
        {
            serverHandler.WriteToSocket("BID", TextboxUserInput.Text);
            TextboxUserInput.Clear();
        }

        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void MainWindow_OnClosed(object sender, EventArgs e)
        {
            serverHandler.Close();
            Close();
        }

        private void TextboxUserInput_KeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.Enter) {
                serverHandler.WriteToSocket("BID", TextboxUserInput.Text);
                TextboxUserInput.Clear();
            }
        }

        private void TextboxUserInput_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }
        private static bool IsTextAllowed(string text)
        {
            Regex regex = new Regex("[^0-9.-]+");
            return !regex.IsMatch(text);
        }
        private void OnRecieveDisconnectEvent(string text)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(new ServerHandler.RecieveEventType(OnRecieveDisconnectEvent), text);
                return;
            }
            serverHandler.Close();
        }
    }
}
