using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Threading;
using System.Text;

using System.Net;
using System.Net.Sockets;

namespace GaviVR
{
    public class ForceHandlerMain : MonoBehaviour, IPolledObject
    {
        //define force data structure
        public struct Force
        {
            public float Intensity;
            public Vector3 Pt;
            public Vector3 Dir;
            private bool isGrass;
        }


        //define variables
        private List<Force> f = new List<Force>();
        private Force ForceField;
        private List<GameObject> ForceObjects;
        private GameObject FieldObjct;
        //character remuver
        string[] ToRemuve = new string[] { "Force", "Field", "{", "}", "\n" };
        string[] FunctionTag = new string[] { "Force.Add", "Force.Edit", "Force.Remove", "Field.Add", "Field.Edit", "Field.Remove" };


        //UDP data
        public int receivePort = 6000, sendPort = 6001;//force F (6xxx)
        private string serverIP = "127.0.0.1";
        private IPEndPoint sendEndPoint, receiveEndPoint;
        private UdpClient senderClient = new UdpClient();
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
            senderClient.Connect(this.sendEndPoint);
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
                string lastMessage = null;
                while (true)
                {
                    string text = null;
                    byte[] bytesReceived = readerClient.Receive(ref receiveEndPoint);
                    // decode UTF8-coded bytes to text format
                    text = Encoding.UTF8.GetString(bytesReceived);
                    //Debug.Log("Received data from " + text);//to test data format
                    if (text!=lastMessage)
                    {

                    }

                    Thread.Sleep(5);
                }
            });

            //SEND
            
            
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
            //set data
        }
        //string[] FunctionTag = new string[] { "Force.Add", "Force.Edit", "Force.Remove", "Field.Add", "Field.Edit", "Field.Remove" };
        public void AddForce(Vector3 position, float intensity=1)
        {
            int c = ForceObjects.Count;
            GameObject Force = objectPooler.SpawnFromPool(ToRemuve[0], position);
            //FunctionTag[0]
            string sendData = FunctionTag[0]+"\n";
            ForceObjects.Add(Force);
            Force.GetComponent<ForceData>().index = c;
            Force.GetComponent<ForceData>().intensity = intensity;
        }
        public void EditForce()
        {
            //FunctionTag[1]
        }
        public void RemoveForce(GameObject Force)
        {
            int index = Force.GetComponent<ForceData>().index;
            objectPooler.poolDictionary[ToRemuve[0]].Enqueue(Force);
            Force.SetActive(false);

            //FunctionTag[2]
            string Comand = FunctionTag[2] + "(" + index.ToString() + ")";
        }
        public void AddField()
        {
            //FunctionTag[3]
        }
        public void EditField()
        {
            
            //FunctionTag[4]
        }
        public void RemoveField()
        {
            //FunctionTag[5]
        }
        public void SendData(string sendString)
        {
            byte[] bytes2Send = toBytes(sendString);
            senderClient.Send(bytes2Send, bytes2Send.Length);
        }
    }
}

