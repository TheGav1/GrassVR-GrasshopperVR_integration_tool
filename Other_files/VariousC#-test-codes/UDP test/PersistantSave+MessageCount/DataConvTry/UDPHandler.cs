using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;

using System.Net;
using System.Net.Sockets;

using System.Threading;
using System.Linq;//ToList usage

//remuve characters https://stackoverflow.com/questions/7411438/remove-characters-from-c-sharp-string


namespace PersistantSave
{
    class UDPHandler
    {
        //count
        int MessageN = 0;

        // Mesh require double side?
        public Boolean DoubleSideMesh = false;

        private int receivePort;
        private string serverIP;
        private IPEndPoint receiveEndPoint;
        private bool NewMesh = false;
        private string LastText;

        private List<string> data = new List<string>(); //conversion string2vector3 data save
        private string[] sdata = new string[4];//split data max dim = 4 for quad
        private bool quad = false;//is the mesh element quad bool value start as false

        //list data
        List<Vector3> verticies = new List<Vector3>();
        List<int> triangles = new List<int>();
        //Save list
        public List<Vector3> VertSave = new List<Vector3>();
        public List<int> TriangSave = new List<int>();

        Thread recive, DataState;

        //character remuver
        string[] ToRemuve = new string[] { "T", "Q", "{", "}", "\n"};

        public UDPHandler(string serverIP, int receivePort)
        {
            this.serverIP = serverIP;
            this.receivePort = receivePort;
            this.receiveEndPoint = new IPEndPoint(IPAddress.Parse(this.serverIP), this.receivePort);
            this.readerUdpClient();
            //Start thread for data print (5ms if new mesh)
            this.Data();
        }

        void readerUdpClient()
        {
            Console.WriteLine("NewMesh = "+NewMesh);
            Console.WriteLine("IsDouble = " + DoubleSideMesh);
            Console.WriteLine("Message = " + MessageN);
            UdpClient readerClient = new UdpClient();
            IPEndPoint localEndpoint = new IPEndPoint(IPAddress.Parse(serverIP), receivePort);
            readerClient.Client.Bind(localEndpoint);          //Tried both Connect and Bind
            //readerClient.Connect(this.receiveEndPoint);
            /******************THREAD START**********************************/
            recive = new Thread(() =>
            {
                //Debug.Log("Awaiting data from server\n" + serverIP + " port:" + receivePort + "\n...");
                Console.WriteLine("Awaiting data from server\n" + serverIP + " port:" + receivePort + "\n...");
                while (true)
                {
                    string text = "";
                    byte[] bytesReceived = readerClient.Receive(ref localEndpoint);
                    // decode UTF8-coded bytes to text format
                    text = Encoding.UTF8.GetString(bytesReceived);

                    //The above throws:     System.InvalidOperationException: 'You must call the Bind method before performing this operation'
                    if (MessageN==0)
                    {
                        //Debug.Log("Received data from\n" + serverIP + " port:" + text);
                        Console.WriteLine("Received data\n" + text);
                    }
                    //is a new message?
                    if (text != LastText && text != null && !NewMesh)
                    {
                        MessageN++;
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
                                //Console.WriteLine("N+V" + i.ToString() + "\n");
                                foreach (var c in ToRemuve)
                                {
                                    data[i] = data[i].Replace(c, string.Empty);
                                }
                                //string to list<Vec3>
                                sdata = data[i].Split(',');

                                // store as a Vector3
                                verticies.Add(new Vector3(
                                    float.Parse(sdata[0]),//0=xRhino=xUnity
                                    float.Parse(sdata[2]),//2=zRhino=yUnity
                                    float.Parse(sdata[1])));//1=yRhino=zUnity
                                                            // x,y,z correction in store notes
                            }//end vertex                         
                        }//FOR END
                         //End convert rutine
                        Console.WriteLine("ready to save \n");
                        SaveMeshData();
                        //clear data for loop check
                        verticies.Clear();
                        triangles.Clear();
                        data.Clear();
                        LastText = text;
                        Console.WriteLine(LastText == text);
                    }//if new message end
                    else
                    { Console.WriteLine("same message"); Thread.Sleep(1000); }
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

        private void SaveMeshData()
        {
            TriangSave.Clear();
            VertSave.Clear();
            Console.WriteLine("Data save" + MessageN + "\n");
            //use add range and not list 1= list 2 (list 1 and 2 became the same if clear 1=> even 2 become empty)
            TriangSave.AddRange(triangles);
            VertSave.AddRange(verticies);
            NewMesh = true;
            
        }
        void Data()
        {
            Console.WriteLine("NewMesh = " + NewMesh);
            Console.WriteLine("IsDouble = " + DoubleSideMesh);
            Console.WriteLine("Message = " + MessageN);
            DataState = new Thread(() =>
            {
                while (true)
                {
                    if (NewMesh == true)
                    {
                        Console.WriteLine("Message number: \n" + MessageN.ToString() + "\n");
                        Console.WriteLine("Triangles: \n");
                        Vector3[] VerticiesArr = VertSave.ToArray();
                        int[] TrianglesArr = TriangSave.ToArray();
                        for (int i =0; i< TrianglesArr.Length;i++)
                            Console.WriteLine(TrianglesArr[i].ToString());
                        Console.WriteLine("Verticies: \n");
                        for (int i = 0; i < VerticiesArr.Length; i++)
                            Console.WriteLine(VerticiesArr[i].ToString());
                        NewMesh = false;
                    }
                }
                
            });

            DataState.Start();
        }

    }
}
