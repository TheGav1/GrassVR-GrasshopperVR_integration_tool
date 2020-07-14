using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//
using System;
using System.Text;
//using System.Numerics;//use unity vector3 non sys numeric

using System.Net;
using System.Net.Sockets;

using System.Threading;
using System.Linq;//ToList usage

/* Grasshopper Mesh transfert for Unity
    ---by TheGavi---

    UDP Reciver:

        ***base code by la1n***
        [url] https://forum.unity.com/threads/simple-udp-implementation-send-read-via-mono-c.15900/ [01/10/2019]
 
        references
        [url] http://msdn.microsoft.com/de-de/library/bb979228.aspx#ID0E3BAC [01/10/2019]

     Mesh Generator

        references
        [url] https://www.youtube.com/watch?v=eJEpeUH1EMg&list=PL3a153tnqpLbpOnKJ-jZ0Y1dWWRyXMNf_&index=14&t=138s [01/10/2019]

    Data converter
        stringvector by Jessespike
        [url] https://answers.unity.com/questions/1134997/string-to-vector3.html [01/10/2019]


    remuve characters https://stackoverflow.com/questions/7411438/remove-characters-from-c-sharp-string


*/

    

namespace GaviVR
{
    //Required component on the script element
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]

    public class MeshHandler : MonoBehaviour, IPolledObject
    {
        //list of private and public datas
        Thread recive;

        // Mesh require double side?
        public Boolean DoubleSideMesh = false;

        //UDP data
        public int receivePort = 5000/*, sendPort = 5001*/;
        private string serverIP="127.0.0.1";
        private IPEndPoint /*sendEndPoint,*/ receiveEndPoint;

        //message data
        private bool NewMesh = false;
        private string LastText;
        private List<string> data = new List<string>(); //conversion string2vector3 data save
        private string[] sdata = new string[4];//split data max dim = 4 for quad
        private bool quad = false;//is the mesh element quad bool value start as false
        
        Mesh mesh;

        //list data
        public List<Vector3> verticies = new List<Vector3>();
        List<int> triangles = new List<int>();
        public List<Vector3> VertSave = new List<Vector3>();
        List<int> TriangSave = new List<int>();
        //character remuver
        string[] ToRemuve = new string[] { "T", "Q", "{", "}", "\n" };

        //on Awake start UDP reader and connect the mesh to the meshfilter component
        private void Awake()
        {
            //generate mesh
            mesh = new Mesh();
            GetComponent<MeshFilter>().mesh = mesh;
            //start UDP reciver
            receiveEndPoint = new IPEndPoint(IPAddress.Parse(serverIP), receivePort);
            readerUdpClient();
        }

        // on Update the mesh is regenered if not empty
        void Update()
        {
            if(NewMesh)
            {
                UpdateMesh();
                NewMesh = false;
            }
        }//ok

        //close thread on quit
        void OnApplicationQuit()
        {
            stopTreads();
        }
        //UDP RECIVER CODE
        void readerUdpClient()
        {
            UdpClient readerClient = new UdpClient();
            IPEndPoint localEndpoint = new IPEndPoint(IPAddress.Parse(serverIP), receivePort);
            readerClient.Client.Bind(localEndpoint);          
            /******************THREAD START**********************************/
            recive = new Thread(() =>
            {
                while (true)
                {
                    string text = "";
                    Debug.Log("Awaiting data from server\n" + serverIP + " port:" + receivePort + "\n...");
                    //Console.WriteLine("Awaiting data from server\n" + serverIP + " port:" + receivePort + "\n...");
                    byte[] bytesReceived = readerClient.Receive(ref localEndpoint);
                    // decode UTF8-coded bytes to text format
                    text = Encoding.UTF8.GetString(bytesReceived);

                    //The above throws:     System.InvalidOperationException: 'You must call the Bind method before performing this operation'
                    Debug.Log("Received data from\n" + serverIP + " port:" + text);
                    //Console.WriteLine("Received data from\n" + serverIP + " port:" + text);
                    
                    //is a new message?
                    if(text!=LastText && text!=null && !NewMesh)
                    {
                        //convert data = Start convert rutine
                        //1 split at new line
                        data = text.Split('\n').ToList();
                        for (int i = 0; i < data.Count; i++)
                        {
                            //find if mesh (Triang-Quad)
                            if (data[i].StartsWith("T") || data[i].StartsWith("Q"))
                            {
                                //Debug.Log("T+Q" + i.ToString() + "\n");
                                if (data[i].StartsWith("Q"))
                                {
                                    quad = true;
                                }
                                foreach (var c in ToRemuve)
                                {
                                    data[i] = data[i].Replace(c, string.Empty);
                                }
                                //Console.WriteLine("Converted data check: " + i + "\n" + data[i]);

                                //Mesh string to list<int>
                                sdata = data[i].Split(';');
                                if (quad)//quad to triangle
                                {
                                    //first triangle
                                    triangles.Add(Int32.Parse(sdata[0]));
                                    triangles.Add(Int32.Parse(sdata[1]));
                                    triangles.Add(Int32.Parse(sdata[3]));
                                    //second triangle
                                    triangles.Add(Int32.Parse(sdata[1]));
                                    triangles.Add(Int32.Parse(sdata[2]));
                                    triangles.Add(Int32.Parse(sdata[3]));
                                    if (DoubleSideMesh == false)
                                    { //Debug.Log("Single sided mesh");
                                    }
                                    else
                                    {
                                        // store back side
                                        //first triangle
                                        triangles.Add(Int32.Parse(sdata[0]));
                                        triangles.Add(Int32.Parse(sdata[3]));
                                        triangles.Add(Int32.Parse(sdata[1]));
                                        //second triangle
                                        triangles.Add(Int32.Parse(sdata[1]));
                                        triangles.Add(Int32.Parse(sdata[3]));
                                        triangles.Add(Int32.Parse(sdata[2]));

                                        //Debug.Log("Double sided mesh");
                                    }
                                }
                                else//already triangle
                                {
                                    for (int c = 0; c < 3; c++)
                                    {
                                        triangles.Add(Int32.Parse(sdata[c]));
                                    }
                                    if (DoubleSideMesh == false)
                                    { //Debug.Log("Single sided mesh");
                                    }
                                    else
                                    {
                                        // store back side
                                        triangles.Add(Int32.Parse(sdata[0]));
                                        triangles.Add(Int32.Parse(sdata[2]));
                                        triangles.Add(Int32.Parse(sdata[1]));

                                        //Debug.Log("Double sided mesh");
                                    }
                                }
                                quad = false;
                            }//TRIANG + QUAD END
                             //Vertex or Normals (same procedure, change save name us used for normals)
                            else
                            {
                                Debug.Log("N+V" + i.ToString() + "\n");
                                foreach (var c in ToRemuve)
                                {
                                    data[i] = data[i].Replace(c, string.Empty);
                                }
                                //string to list<Vec3>
                                sdata = data[i].Split(',');

                                float xU, yU, zU;
                                // store as a Vector3
                                xU = float.Parse(sdata[1]);//xUnity=yRhino=1
                                yU = float.Parse(sdata[2]);//yUnity=zRhino=2
                                zU= -1*float.Parse(sdata[0]);//zUnity=-xRhino
                                verticies.Add(new Vector3(xU, yU, zU));                                    
                                   // x,y,z correction in store notes
                            }//end vertex                         
                        }//FOR END
                         //End convert rutine
                        SaveMeshData();
                        //clear data for loop check
                        verticies.Clear();
                        triangles.Clear();
                        data.Clear();
                        LastText = text;
                    }//if new message end
                    
                    //sleep time (ms)
                    Thread.Sleep(1);
                };//while end
            });
            /******************THREAD END**********************************/
            recive.Start();
        }
        //byte conversion for UDP message
        public byte[] toBytes(string text)
        {
            return Encoding.UTF8.GetBytes(text);
        }
        public string fromBytes(byte[] bytes)
        {
            return Encoding.UTF8.GetString(bytes);
        }
        // Stop treads
        private void stopTreads()
        {
            if (recive.IsAlive)
            {
                recive.Abort();
            }
        }
        private void UpdateMesh()
        {
                mesh.Clear();
                //to array conv for MeshGeneration in Unity
                mesh.vertices = VertSave.ToArray();
                mesh.triangles = TriangSave.ToArray();

            GetComponent<MeshFilter>().mesh = mesh;
        }
        private void SaveMeshData()
        {
            TriangSave.Clear();
            VertSave.Clear();
            //use add range and not list 1= list 2 (list 1 and 2 became the same if clear 1=> even 2 become empty)
            TriangSave.AddRange(triangles);
            VertSave.AddRange(verticies);
            NewMesh = true;
        }
         public void OnObjectSpawn()
        {

        }
    }
}
