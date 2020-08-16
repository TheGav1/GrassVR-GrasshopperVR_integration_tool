using System;
using System.Threading;

namespace UDPTesting
{

    class Program
    {

        static void Main(string[] args)
        {
            string serverIP = "127.0.0.1";
            int sendPort = 5001;
            int receivePort = 5000;
            UDPHandler handler = new UDPHandler(serverIP, receivePort, sendPort);
        }

    }

}
