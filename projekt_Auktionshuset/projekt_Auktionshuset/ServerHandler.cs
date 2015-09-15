using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading;

namespace projekt_Auktionshuset
{
    class ServerHandler
    {
        public delegate void RecieveEventType(string Response);
        public event RecieveEventType RecieveNewBidderEvent;
        public event RecieveEventType RecieveDescriptionEvent;
        public event RecieveEventType RecieveNewHighestEvent;
        public event RecieveEventType RecieveEstimatedEvent;
        private string _servername;
        private int port;

        private TcpClient serverSocket;
        private NetworkStream netStream;
        private StreamWriter writer;
        private StreamReader reader;
        private Thread readerThread;

        public ServerHandler(string servername, int port)
        {
            this._servername = servername;
            this.port = port;
        }
        public void Open()
        {
            serverSocket = new TcpClient("127.0.0.1", 12000);
            netStream = serverSocket.GetStream();
            writer = new StreamWriter(netStream);
            reader = new StreamReader(netStream);
            readerThread = new Thread(ReaderThreadMethod);
            readerThread.Start();
        }
        public void Close()
        {
            writer.Close();
            reader.Close();
            netStream.Close();
            serverSocket.Close();
            serverSocket = null;
        }

        public void WriteToSocket(string command, string bid)
        {
            writer.WriteLine(command);
            writer.Flush();
            writer.WriteLine(bid);
            writer.Flush();

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
                return reader.ReadLine();
            }
            catch
            {
                return null;
            }
        }
        public string RecieveMessage()
        {
            string msg = ReadLineFromSocket();
            return msg;
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
                }
            }
        }
    }
}
