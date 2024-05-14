using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;

namespace RevitToVR
{
    public class RevitToVR : MonoBehaviour
    {
        private void Start()
        {
            Debug.Log("Started RevitToVR Session");
            //SendMessage();
        }

        private void OnDestroy()
        {
            Debug.Log("Stopped RevitToVR Session");
        }

        private void Update()
        {
        }

        private void SendMessage()
        {
            using (WebSocket socket = new WebSocket("ws://127.0.0.1/Test"))
            {
                //socket.OnMessage += (sender, e) => Debug.Log(string.Format("Received from server: {0}", e.Data));
                socket.Connect();
                Debug.Log(socket.ReadyState);
                //socket.Send("Test");
            }
        }
    }
}