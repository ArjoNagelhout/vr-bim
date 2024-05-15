using System;
using System.Collections;
using System.Collections.Generic;
using revit_to_vr_common;
using WebSocketSharp;

namespace RevitToVR
{
    public class MainServiceClient
    {
        private WebSocket socket;

        public MainServiceClient()
        {
            string uri = Configuration.uri + Configuration.mainPath;

            socket = new WebSocket(uri);
            socket.OnMessage += (sender, e) => UIConsole.Log("MainService (Server) sent: " + e.Data);
            socket.WaitTime = TimeSpan.FromSeconds(1);
            socket.Connect();
            UIConsole.Log("Connected to MainServiceClient with uri: " + uri);
            
            socket.Send("Connected from Unity!");
        }

        ~MainServiceClient()
        {
        }
    }
}