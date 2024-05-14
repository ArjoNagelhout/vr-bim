using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

using WebSocketSharp;

namespace RevitToVR
{
    public class WebSocketClientTest2 : MonoBehaviour
    {
        public string serverIpAddress;
        public string message;
        
        private WebSocket socket;

        public TextMeshProUGUI text;
        
        // Start is called before the first frame update
        void Log(string m)
        {
            text.text += m + "\n";
        }
        
        void Start()
        {
            Log("---------- CLIENT -----------");
            string uri = "ws://" + serverIpAddress + "/Test"; // ip address of the Parallels Windows VM. 
// Client
            socket = new WebSocket(uri);
            socket.OnMessage += (sender, e) =>
                Log("Test WebSocket Server sent data: " + e.Data);

            socket.Connect();
            Log(string.Format("Connected WebSocket client to server at {0}", uri));

            socket.Send("This message is sent by the client");
            socket.Send("This is from Unity. ");
            socket.Send(message);
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}