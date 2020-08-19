using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;

using System.Net;
using System.Net.Sockets;

using System.Threading;
using System.Linq;//ToList usage

//remuve characters https://stackoverflow.com/questions/7411438/remove-characters-from-c-sharp-string


namespace UDPTesting
{

    class UDPHandler
    {
        // Mesh require double side?
        public Boolean DoubleSideMesh = false;

        private int receivePort, sendPort;
        private string serverIP;
        private IPEndPoint sendEndPoint, receiveEndPoint;

        private List<string> data = new List<string>(); //conversion string2vector3 data save
        private string[] sdata = new string[4];//split data max dim = 4 for quad
        private bool quad = false;//is the mesh element quad bool value start as false

        //list data
        List<Vector3> verticies = new List<Vector3>();
        List<int> triangles = new List<int>();

        //character remuver
        string[] ToRemove = new string[] { "T", "Q", "{", "}", "\n"};

        public UDPHandler(string serverIP, int receivePort, int sendPort)
        {
            this.serverIP = serverIP;
            this.receivePort = receivePort;
            this.sendPort = sendPort;
            this.sendEndPoint = new IPEndPoint(IPAddress.Parse(this.serverIP), this.sendPort);
            this.receiveEndPoint = new IPEndPoint(IPAddress.Parse(this.serverIP), this.receivePort);
            //this.readerUdpClient();
            this.senderUdpClient();
            
        }

        void readerUdpClient()
        {
            UdpClient readerClient = new UdpClient();
            IPEndPoint localEndpoint = new IPEndPoint(IPAddress.Parse(this.serverIP), this.receivePort);
            readerClient.Client.Bind(localEndpoint);          //Tried both Connect and Bind
            //readerClient.Connect(this.receiveEndPoint);
            Thread recive = new Thread(() =>
            {
                while (true)
                {
                    string text = "";
                    Console.WriteLine("Awaiting data from server\n" + serverIP + " port:" + receivePort + "\n...");
                    byte[] bytesReceived = readerClient.Receive(ref localEndpoint);
                    // decode UTF8-coded bytes to text format
                    text = Encoding.UTF8.GetString(bytesReceived);

                    //The above throws:     System.InvalidOperationException: 'You must call the Bind method before performing this operation'
                    Console.WriteLine("Received data from\n" + serverIP + " port:" + text);

                    //convert data = Start convert rutine
                    //1 split \n
                    data = text.Split('\n').ToList();
                    //find if mesh (Triang-Quad)


                    for (int i = 0; i < data.Count; i++)
                    {

                        if (data[i].StartsWith("T") || data[i].StartsWith("Q"))
                        {
                            Console.WriteLine("T+Q" + i.ToString() + "\n");
                            if (data[i].StartsWith("Q"))
                            {
                                quad = true;
                            }
                            foreach (var c in ToRemove)
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
                                { Console.WriteLine("Single sided mesh"); }
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

                                    Console.WriteLine("Double sided mesh");
                                }
                            }
                            else//already triangle
                            {
                                for (int c = 0; c < 3; c++)
                                {
                                    triangles.Add(Int32.Parse(sdata[c]));
                                }
                                if (DoubleSideMesh == false)
                                { Console.WriteLine("Single sided mesh"); }
                                else
                                {
                                 // store back side
                                    triangles.Add(Int32.Parse(sdata[0]));
                                    triangles.Add(Int32.Parse(sdata[2]));
                                    triangles.Add(Int32.Parse(sdata[1]));

                                    Console.WriteLine("Double sided mesh");
                                }
                            }
                            quad = false;
                        }//TRIANG + QUAD END
                        //Vertex or Normals (same procedure, change save name us used for normals)
                        else
                        {
                            Console.WriteLine("N+V" + i.ToString() + "\n");
                            foreach (var c in ToRemove)
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

                    //PRINT DATA vertex+triang
                    Console.WriteLine("Vertex data:\n");
                    verticies.ForEach(b => Console.WriteLine(b.ToString()));
                    Console.WriteLine("\n\n");
                    Console.WriteLine("Triangles data:\n");
                    triangles.ForEach(b => Console.WriteLine(b));
                    Console.WriteLine("\n\n");
                    //clear data for loop check
                    verticies.Clear();
                    triangles.Clear();
                    //clear data to restart while
                    data.Clear();
                    //sleep time (ms)
                    Thread.Sleep(1);
                };//while end
            });//tread definition end
        recive.Start();
        }

        void senderUdpClient()
        {
            UdpClient senderClient = new UdpClient();
            senderClient.Connect(this.sendEndPoint);
            string sendString;// = "Del\n"+"{1}\n";
            byte[] bytes;// = toBytes(sendString);
            int i = 0;
            Thread send = new Thread(() => {
                while (true)
                {
                    i++;
                    Console.WriteLine(i);
                    string Line =Console.ReadLine();
/*                    i = i+1;
                    Console.WriteLine(i.ToString(), "/n");
                    if (i>20)
                    {
                        string aa = "3;2;1";
                        byte[] bt = toBytes(aa);
                        senderClient.Send(bt, bt.Length);
                    }
                    else
                    {
*/                        //senderClient.Send(bytes, bytes.Length);
 //                   }
                    switch (Line)
                    {
                        case "Add":
                            sendString = "Add\n"+"{1,2,3}\n";
                            bytes = toBytes(sendString);
                            senderClient.Send(bytes, bytes.Length);
                            break;
                        case "Del":
                            sendString = "Del\n" + "{1}\n";
                            bytes = toBytes(sendString);
                            senderClient.Send(bytes, bytes.Length);
                            break;
                        default:
                            break;

                    }
                    Thread.Sleep(5);
                    
                }
            });
            send.Start();
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
