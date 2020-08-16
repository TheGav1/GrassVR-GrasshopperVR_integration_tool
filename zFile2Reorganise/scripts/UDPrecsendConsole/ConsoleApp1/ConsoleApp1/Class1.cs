
using System.Collections.Generic;
using System.Text;

using System.Net;
using System.Net.Sockets;

using System.Threading;

namespace UDPTesting
{

    class UDPHandler
    {

        private int receivePort, sendPort;
        private string serverIP;
        private IPEndPoint sendEndPoint, receiveEndPoint;

        public UDPHandler(string serverIP, int receivePort, int sendPort)
        {
            this.serverIP = serverIP;
            this.receivePort = receivePort;
            this.sendPort = sendPort;
            this.sendEndPoint = new IPEndPoint(IPAddress.Parse(this.serverIP), this.sendPort);
            this.receiveEndPoint = new IPEndPoint(IPAddress.Parse(this.serverIP), this.receivePort);
            this.readerUdpClient();
            this.senderUdpClient();
        }

        void readerUdpClient()
        {
            UdpClient readerClient = new UdpClient();
            IPEndPoint localEndpoint = new IPEndPoint(IPAddress.Any, 3000);
            readerClient.Client.Bind(localEndpoint);          //Tried both Connect and Bind
            //readerClient.Connect(this.receiveEndPoint);
            Thread t = new Thread(() => {
                Console.WriteLine("Awaiting data from server...");
                var remoteEP = new IPEndPoint(IPAddress.Parse(this.serverIP), this.receivePort);
                byte[] bytesReceived = readerClient.Receive(ref remoteEP);
                // decode UTF8-coded bytes to text format
                string text = Encoding.UTF8.GetString(bytesReceived);

                //The above throws:     System.InvalidOperationException: 'You must call the Bind method before performing this operation'
                Console.WriteLine("Received data from " + text);
            });
            t.Start();
        }

        void senderUdpClient()
        {
            UdpClient senderClient = new UdpClient();
            senderClient.Connect(this.sendEndPoint);
            string sendString = "1;2;3";
            byte[] bytes = toBytes(sendString);
            Thread t = new Thread(() => {
                while (true)
                {
                    senderClient.Send(bytes, bytes.Length);
                    Thread.Sleep(1000);
                }
            });
            t.Start();
        }

        public byte[] toBytes(string text)
        {
            return Encoding.UTF8.GetBytes(text);
        }

        public string fromBytes(byte[] bytes)
        {
            return Encoding.UTF8.GetString(bytes);
        }

    }
}