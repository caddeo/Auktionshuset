﻿using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading;

namespace projekt_Auktionshuset
{
    class ServerHandler
    {
        public delegate void RecieveEventType(string response);
        public event RecieveEventType RecieveNewBidderEvent;
        public event RecieveEventType RecieveDescriptionEvent;
        public event RecieveEventType RecieveNewHighestEvent;
        public event RecieveEventType RecieveEstimatedEvent;
        public event RecieveEventType RecieveMessageEvent;
        private string _servername;
        private int _port;

        private TcpClient _serverSocket;
        private NetworkStream _netStream;
        private StreamWriter _writer;
        private StreamReader _reader;
        private Thread _readerThread;

        public ServerHandler(string servername, int port)
        {
            this._servername = servername;
            this._port = port;
        }
        public void Open()
        {
            _serverSocket = new TcpClient("127.0.0.1", 12000);
            _netStream = _serverSocket.GetStream();
            _writer = new StreamWriter(_netStream);
            _reader = new StreamReader(_netStream);
            _readerThread = new Thread(ReaderThreadMethod);
            _readerThread.Start();
        }
        public void Close()
        {
            _writer.Close();
            _reader.Close();
            _netStream.Close();
            _serverSocket.Close();
            _serverSocket = null;
        }

        public void WriteToSocket(string command, string bid)
        {
            _writer.WriteLine(command);
            _writer.Flush();
            _writer.WriteLine(bid);
            _writer.Flush();

            // en alternativ måde

            //if (Regex.IsMatch(bid, @"[a-zA-Z]"))
            //{
            //    writer.WriteLine("COMMAND");
            //    writer.Flush();
            //    writer.WriteLine(bid);
            //    writer.Flush();
            //}
            //else
            //{
            //    writer.Write(bid);
            //    writer.Flush();
            //}
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
            while (true)
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
                        String text = ReadLineFromSocket();
                        if (RecieveMessageEvent != null)
                            RecieveMessageEvent(text);
                        break;
                }
            }
        }
    }
}
