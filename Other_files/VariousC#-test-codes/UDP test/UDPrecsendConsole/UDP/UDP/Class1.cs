using System;
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
        private int i=1;

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
            IPEndPoint localEndpoint = new IPEndPoint(IPAddress.Parse(this.serverIP), this.receivePort);
            readerClient.Client.Bind(localEndpoint);          //Tried both Connect and Bind
            //readerClient.Connect(this.receiveEndPoint);
            Thread t = new Thread(() => {
                while(i!=0)
                {
                    string text = "";
                    Console.WriteLine("Awaiting data from server\n"+serverIP+" port:"+receivePort+"\n...");
                    byte[] bytesReceived = readerClient.Receive(ref localEndpoint);
                    // decode UTF8-coded bytes to text format
                    text = Encoding.UTF8.GetString(bytesReceived);

                    //The above throws:     System.InvalidOperationException: 'You must call the Bind method before performing this operation'
                    Console.WriteLine("Received data from\n" + text);

                    //convert data
                    //1 split \n

                    //T1 triangle T

                    //T2 Parentesis

                    //T3 string to list<int>

                    //other1 data not T

                    //other2 Parentesis

                    //other2 string to list<Vec3>

                        //x,y,z correction



                    Console.WriteLine("Received data from\n" + text);
                    i = Convert.ToInt32(Console.ReadLine());
                }
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