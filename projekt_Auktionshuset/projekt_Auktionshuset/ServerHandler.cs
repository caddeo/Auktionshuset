using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Documents;

namespace projekt_Auktionshuset
{
    class ServerHandler
    {
        public delegate void RecieveEventType(string response);

        public event RecieveEventType RecieveNameEvent;
        public event RecieveEventType RecieveNewBidderEvent;
        public event RecieveEventType RecieveDescriptionEvent;
        public event RecieveEventType RecieveNewHighestEvent;
        public event RecieveEventType RecieveEstimatedEvent;
        public event RecieveEventType RecieveMessageEvent;
        public event RecieveEventType RecieveDisconnectEvent;

        private string _servername;
        private int _port;

        private bool _clientRunning;

        private TcpClient _serverSocket;
        private NetworkStream _netStream;
        private StreamWriter _writer;
        private StreamReader _reader;
        private Thread _readerThread;
        public ServerHandler(string servername, int port)
        {
            _servername = servername;
            _port = port;
        }
        public void Open()
        {
            _serverSocket = new TcpClient(_servername, _port);
            _netStream = _serverSocket.GetStream();
            _writer = new StreamWriter(_netStream);
            _reader = new StreamReader(_netStream);
            _readerThread = new Thread(ReaderThreadMethod);
            _readerThread.Start();
            _clientRunning = true;
        }
        public void Close()
        {
            if (_serverSocket.Connected.Equals(true) && _serverSocket != null)
            {
                WriteToSocket("DISCONNECT", "");
                _writer.Close();
                _reader.Close();
                _netStream.Close();
                _serverSocket.Close();
                _serverSocket = null;
            }
            _clientRunning = false;
        }
        public void WriteToSocket(string command, string bid)
        {
            _writer.WriteLine(command);
            _writer.Flush();
            _writer.WriteLine(bid);
            _writer.Flush();
        }
        private string ReadLineFromSocket()
        {
            try
            {
                return _reader.ReadLine();
            }
            catch
            {
                return null;
            }
        }
        public string RecieveMessage()
        {
            string message = ReadLineFromSocket();
            return message;
        }
        private void ReaderThreadMethod()
        {
            while (_clientRunning == true)
            {
                string command = ReadLineFromSocket();
                Console.WriteLine(command);
                switch (command)
                {
                    case "NEWBIDDER":
                        string newBidder = ReadLineFromSocket();
                        if (RecieveNewBidderEvent != null)
                            RecieveNewBidderEvent(newBidder);
                        break;
                    case "NEWHIGHEST":
                        string bid = ReadLineFromSocket();
                        if (RecieveNewHighestEvent != null)
                            RecieveNewHighestEvent(bid);
                        break;
                    case "DESCRIPTION":
                        string name = ReadLineFromSocket();
                        if (RecieveNameEvent != null)
                            RecieveNameEvent(name);
                        string desc = ReadLineFromSocket();
                        if (RecieveDescriptionEvent != null)
                            RecieveDescriptionEvent(desc);
                        string estimatePrice = ReadLineFromSocket();
                        if (RecieveEstimatedEvent != null)
                            RecieveEstimatedEvent(estimatePrice);
                        string currentPrice = ReadLineFromSocket();
                        if (RecieveNewHighestEvent != null)
                            RecieveNewHighestEvent(currentPrice);
                        break;
                    case "MESSAGE":
                        string text = ReadLineFromSocket();
                        if (RecieveMessageEvent != null)
                            RecieveMessageEvent(text);
                        break;
                    case "DISCONNECT":
                        string disconnect = "Disconnect";
                        if (RecieveDisconnectEvent != null)
                            RecieveDisconnectEvent(disconnect);
                        break;
                    case "NEWAUCTION":
                        WriteToSocket("CONNECTED", "");
                        break;
                }
            }
        }
    }
}
