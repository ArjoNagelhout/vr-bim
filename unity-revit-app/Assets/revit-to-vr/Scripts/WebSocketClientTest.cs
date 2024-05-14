using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using WebSocketSharp;

namespace RevitToVR
{
    public class WebSocketClientTest : MonoBehaviour
    {
        public string serverIpAddress;
        public string message;
        
        private WebSocket socket;
        
        // Start is called before the first frame update
        void Start()
        {
            Debug.Log("---------- CLIENT -----------");
            string uri = "ws://" + serverIpAddress + "/Test"; // ip address of the Parallels Windows VM. 
// Client
            socket = new WebSocket(uri);
            socket.OnMessage += (sender, e) =>
                Debug.Log("Test WebSocket Server sent data: " + e.Data);

            socket.Connect();
            Debug.Log(string.Format("Connected WebSocket client to server at {0}", uri));

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
