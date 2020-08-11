using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Threading;
using System.Text;

using System.Net;
using System.Net.Sockets;

namespace GaviVR
{
    public class AnchorHandlerMain : MonoBehaviour, IPolledObject
    {
        //define force data structure
        public struct Anchor
        {
            public float Strength;
            public Vector3 Pt;
            public Vector3 EndPt;
        }

        //define variables
        private List<Anchor> a = new List<Anchor>();

        //UDP data
        public int receivePort = 7000, sendPort = 7001;//force F (6xxx)
        private string serverIP = "127.0.0.1";
        private IPEndPoint sendEndPoint, receiveEndPoint;
        //Threads
        Thread send, recive;

        //Pool
        ObjectPooler objectPooler;

        //at awake start UDP connection
        void Awake()
        {
            //start UDP
            receiveEndPoint = new IPEndPoint(IPAddress.Parse(serverIP), receivePort);
            sendEndPoint = new IPEndPoint(IPAddress.Parse(serverIP), sendPort);
            UdpClient();
            //generate pool of Force object
            objectPooler = ObjectPooler.Instance;
        }

        // Update is called once per frame
        void Update()
        {
            //check force pool vs force list
        }

        //close thread on quit
        void OnApplicationQuit()
        {
            stopTreads();
        }

        //UDP handler
        void UdpClient()
        {
            //RECIVE
            UdpClient readerClient = new UdpClient();
            readerClient.Client.Bind(receiveEndPoint);
            ////////////////////////////TreadStart\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
            recive = new Thread(() => {
                Debug.Log("Awaiting data from server...");
                byte[] bytesReceived = readerClient.Receive(ref receiveEndPoint);
                // decode UTF8-coded bytes to text format
                string text = Encoding.UTF8.GetString(bytesReceived);

                //The above throws:     System.InvalidOperationException: 'You must call the Bind method before performing this operation'
                Debug.Log("Received data from " + text);
                while (true)
                {


                    Thread.Sleep(5);
                }
            });

            //SEND
            UdpClient senderClient = new UdpClient();
            senderClient.Connect(this.sendEndPoint);
            string sendString = "1;2;3";
            byte[] bytes2Send = toBytes(sendString);
            ////////////////////////////TreadStart\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
            send = new Thread(() => {
                while (true)
                {

                    senderClient.Send(bytes2Send, bytes2Send.Length);

                    Thread.Sleep(5);
                }
            });

            send.Start();
            recive.Start();
        }
        // Stop treads
        private void stopTreads()
        {
            if (recive.IsAlive)
            {
                recive.Abort();
            }
            if (send.IsAlive)
            {
                send.Abort();
            }
        }
        public byte[] toBytes(string text)
        {
            return Encoding.UTF8.GetBytes(text);
        }

        public string fromBytes(byte[] bytes)
        {
            return Encoding.UTF8.GetString(bytes);
        }
        //action to set on spawn of forces
        public void OnObjectSpawn()
        {

        }
    }
}
