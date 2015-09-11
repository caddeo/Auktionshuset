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
        private string servername;
        private int port;

        private TcpClient serverSocket;
        private NetworkStream netStream;
        private StreamWriter writer;
        private StreamReader reader;
        private Thread readerThread;

        public ServerHandler(string servername, int port)
        {
            this.servername = servername;
            this.port = port;
        }
        public void Open()
        {
            serverSocket = new TcpClient(servername, port);
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
        private void WriteToSocket(string bid)
        {
            if (Regex.IsMatch(bid, @"[a-zA-Z]"))
            {
                writer.WriteLine("COMMAND");
                writer.Flush();
                writer.WriteLine(bid);
                writer.Flush();
            }
            else
            {
                writer.Write(bid);
                writer.Flush();
            }
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
                string cmdmsg = ReadLineFromSocket();
                Console.WriteLine(cmdmsg);
                switch (cmdmsg)
                {
                    case "NEWBIDDER":
                        string newBidder = ReadLineFromSocket();
                        if (RecieveNewBidEvent != null)
                            RecieveNewBidEvent(newBidder);
                        break;
                    //case "NEWHIGHEST":
                    //    string msg = ReadLineFromSocket();

                    //    if (RecieveNewHighestEvent != null)
                    //        RecieveNewHighestEvent(msg);
                        break;
                    case "DESCRIPTION":
                        if (RecieveDescriptionEvent != null)
                            RecieveDescriptionEvent(cmdmsg);
                        break;
                }
            }
        }
    }
}
