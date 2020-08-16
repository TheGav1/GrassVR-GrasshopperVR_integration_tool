using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//other libraries
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

/*Grasshopper Mesh transfert for Unity
    ---by TheGavi---

    UDP Reciver:

        ***base code by la1n***
        [url]https://forum.unity.com/threads/simple-udp-implementation-send-read-via-mono-c.15900/ [01/10/2019]
 
        references   
        [url]http://msdn.microsoft.com/de-de/library/bb979228.aspx#ID0E3BAC [01/10/2019]

     Mesh Generator
        
        references   
        [url]https://www.youtube.com/watch?v=eJEpeUH1EMg&list=PL3a153tnqpLbpOnKJ-jZ0Y1dWWRyXMNf_&index=14&t=138s [01/10/2019]

    Data converter
        stringvector by Jessespike
        [url]https://answers.unity.com/questions/1134997/string-to-vector3.html [01/10/2019]

  */

//Required component on the script element
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]

public class MeshGen : MonoBehaviour
{
    // read Threads
    Thread VertexTreadUdpR;
    Thread TrianglesTreadUdpR;

    // udpclient object
    UdpClient client;

    // port numbers
    private string IP = "127.0.0.1";
    public int portVerticies = 10000; //public to use the same code with just port modify for different object
    public int portTriangles = 10001; //public to use the same code with just port modify for different object

    // Mesh require double side?
    public Boolean DoubleSideMesh = false;

    // storage variable for the UDP packets
    private string[] lastReceivedPacket = new string[2];
    private string[] previousPacket = new string[2];
    private bool[] DataChanged = new bool[2];
    private int dataT =0, dataV=1;
    private string[] data; //conversion string2vector3 data save
    private string[] sArray = new string [3];//conversion

    /*mesh  ---array to use Unity command
      List to have easily incrementable data reciver
    */
    Mesh mesh;
    Vector3[] verticies = new Vector3[100];
    List<Vector3> vertList = new List<Vector3>();
    int[] triangles=new int[500];
    List<int> trianglesList = new List<int>();


    void Start()
    {
        // create a thread for reading UDP messages Vertecies (save data set 0)
        ThreadStart starterV = delegate { ReceiveData(portVerticies, dataV); };
        VertexTreadUdpR = new Thread(starterV);
        VertexTreadUdpR.IsBackground = true;
        VertexTreadUdpR.Start();

        // create a thread for reading UDP messages Triangles (save data set 1)
        ThreadStart starterT = delegate { ReceiveData(portTriangles, dataT); };
        TrianglesTreadUdpR = new Thread(starterT);
        TrianglesTreadUdpR.IsBackground = true;
        TrianglesTreadUdpR.Start();
        
        //mesh call
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        //Data converter
        DataConv(lastReceivedPacket);

        //mesh generation
        GenMesh();

    }


    void Update()
    {/*

        //code for creating the 4 boxes 

        //if the message has changed and the user is over a box with the mouse cursor
        // then modify the 4 boxes with the new coordiantes and scale
        if (DataChanged[0] || DataChanged[1])
        {
            hoverBoxHolder.gameObject.SetActive(true);
            parseDataToBoxes(lastReceivedPacket);
            DataChanged = { false , false };
        }
        //if user is not over a box the deactivate(hide) the 4 boxes
        if (!boxSendScript.mouseOverBox)
            hoverBoxHolder.gameObject.SetActive(false);

        */
    }

    // Unity Application Quit Function
    void OnApplicationQuit()
    {
        stopUdpR();
    }

    // Stop reading UDP messages
    private void stopUdpR()
    {
        if (VertexTreadUdpR.IsAlive)
        {
            VertexTreadUdpR.Abort();
        }
        if (TrianglesTreadUdpR.IsAlive)
        {
            TrianglesTreadUdpR.Abort();
        }
        client.Close();
    }

    // receive thread function
    private void ReceiveData(int port, int dataset)
    {
        client = new UdpClient(port);

        while (true)
        {
            try
            {
                // receive bytes

                IPEndPoint RecIP = new IPEndPoint(IPAddress.Parse(IP), port);
                byte[] data = client.Receive(ref RecIP);

                // decode UTF8-coded bytes to text format
                string text = Encoding.UTF8.GetString(data);

                // show received message
                print(">> " + text);

                // store new message as latest message
                previousPacket [dataset] = lastReceivedPacket [dataset];
                lastReceivedPacket [dataset] = text;
                if (lastReceivedPacket[dataset] != previousPacket[dataset])
                    DataChanged[dataset] = true;



            }
            catch (Exception err)
            {
                print(err.ToString());
            }
        }
    }

    //Mesh Generator & updaater
    private void GenMesh ()
    {
        mesh.Clear();
        mesh.vertices = verticies;
        mesh.triangles = triangles;
    }

    //Data conversion
    private void DataConv(string[] msg)
    {
        //clear data
        Array.Clear(verticies, 1, 2);
        Array.Clear(triangles, 1, 2);

        //data set Vertex=0
        data = msg[0].Split('}');
        for(int i=0; i<data.Length; i++)
        {
            // Remove the parentheses
            if (data[i].StartsWith("{"))
            {
                data[i] = data[i].Substring(1);
            }
            if (data[i].EndsWith("}"))
            {
                data[i] = data[i].Substring(data[i].Length - 2);
            }
            // split the items
            string[] sArray = data[i].Split(',');

            //single side mesh

           // store as a Vector3
                verticies[i] = new Vector3
                (
                    float.Parse(sArray[0]),
                    float.Parse(sArray[1]),
                    float.Parse(sArray[2])
                 );
            if (DoubleSideMesh == false)
            { Debug.Log("Single sided mesh"); }
            else
            {
                // store back side as a Vector3
                verticies[i] = new Vector3
                (
                    float.Parse(sArray[0]),
                    float.Parse(sArray[2]),
                    float.Parse(sArray[1])
                 );
                Debug.Log("Double sided mesh");
            }

            
            
        }
        Array.Clear(data, 1, 2);

        //data set Triangles=1
        data = msg[1].Split(',');
        for (int i = 0; i < data.Length; i++)
        {
            if (data[i].StartsWith("{"))
            {
                data[i] = data[i].Substring(1);
            }
            if (data[i].EndsWith("}"))
            {
                data[i] = data[i].Substring(data[i].Length - 2);
            }
            triangles[i] = Int32.Parse(data[i]);
        }
        Array.Clear(data, 1, 2);
    } 
}

/*
  
 try
        {
            string[] data = msg.Split(';');
            for (int i = 0; i < 4; i++)
            {

                string[] strScales = data[i].Split(',');
                string[] strCoords = data[i + 4].Split(',');

                boxes[i].position = new Vector3(float.Parse(strCoords[0]), float.Parse(strCoords[1]), float.Parse(strCoords[2]));
                boxes[i].localScale = new Vector3(float.Parse(strScales[0]), float.Parse(strScales[1]), float.Parse(strScales[2]));
            }
        }
        catch (Exception err)
        {
            print(err.ToString());
        }
        */