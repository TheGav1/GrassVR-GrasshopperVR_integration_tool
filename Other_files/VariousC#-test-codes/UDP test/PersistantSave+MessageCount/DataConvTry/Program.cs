using System;
using System.Threading;
using System.Collections.Generic;

namespace PersistantSave
{
    //only read, for send use data conv
    class Program
    {

        static void Main(string[] args)
        {
            string serverIP = "127.0.0.1";
            int receivePort = 5000;
            UDPHandler handler = new UDPHandler(serverIP, receivePort);

        }


    }

}
