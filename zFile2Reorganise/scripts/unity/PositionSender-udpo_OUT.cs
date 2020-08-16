using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//other libraries
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

/*Grasshopper Mesh transfert for Unity
    ---by TheGavi---

    UDP Sender:

        ***base code by la1n***
        [url]https://forum.unity.com/threads/simple-udp-implementation-send-read-via-mono-c.15900/ [01/10/2019]
 
        references   
        [url]http://msdn.microsoft.com/de-de/library/bb979228.aspx#ID0E3BAC [01/10/2019]

  */

public class UDPSend : MonoBehaviour
{
    private static int localPort;

    // Host location
    private string IP = "127.0.0.1";
    public int port = 8051;

    // connection name
    IPEndPoint remoteEndPoint;
    UdpClient client;

    // Message initialization
    string strMessage = "";


    // call it from shell (as program)
    private static void Main()
    {
        UDPSend sendObj = new UDPSend();
        sendObj.Init();

        // testing via console
        // sendObj.inputFromConsole();

        // as server sending endless
        sendObj.SendEndless(" endless infos \n");

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // OnGUI
    void OnGUI()
    {
        Rect rectObj = new Rect(40, 380, 200, 400);
        GUIStyle style = new GUIStyle
        {
            alignment = TextAnchor.UpperLeft
        };
        GUI.Box(rectObj, "# UDPSend-Data\n127.0.0.1 " + port + " #\n"
                    + "shell> nc -lu 127.0.0.1  " + port + " \n"
                , style);

        // ------------------------
        // send it
        // ------------------------
        strMessage = GUI.TextField(new Rect(40, 420, 140, 20), strMessage);
        if (GUI.Button(new Rect(190, 420, 40, 20), "send"))
        {
            SendString(strMessage + "\n");
        }
    }

    // UDP Start
    public void Init()
    {
        // Endpunkt definieren, von dem die Nachrichten gesendet werden.
        print("UDPSend.Start()");

        // ----------------------------
        // Senden
        // ----------------------------
        remoteEndPoint = new IPEndPoint(IPAddress.Parse(IP), port);
        client = new UdpClient();

        // status
        print("Sending to " + IP + " : " + port);
        print("Testing: nc -lu " + IP + " : " + port);

    }

    // inputFromConsole
    private void InputFromConsole()
    {
        try
        {
            string text;
            do
            {
                text = Console.ReadLine();

                // Send the text to the remote client
                if (text != "")
                {

                    // Encoding data with UTF8 encoding into binary format
                    byte[] data = Encoding.UTF8.GetBytes(text);

                    // Send the text to the remote client
                    client.Send(data, data.Length, remoteEndPoint);
                }
            } while (text != "");
        }
        catch (Exception err)
        {
            print(err.ToString());
        }

    }

    // sendData
    private void SendString(string message)
    {
        try
        {
            //if (message != "")
            //{

            // Encoding data with UTF8 encoding into binary format
            byte[] data = Encoding.UTF8.GetBytes(message);

            // Send the text to the remote client
            client.Send(data, data.Length, remoteEndPoint);
            //}
        }
        catch (Exception err)
        {
            print(err.ToString());
        }
    }


    // endless test
    private void SendEndless(string testStr)
    {
        do
        {
            SendString(testStr);


        }
        while (true);

    }
}
//end shutdown to do