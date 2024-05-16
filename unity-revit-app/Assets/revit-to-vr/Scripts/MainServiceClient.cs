using System;
using System.Collections;
using System.Collections.Generic;
using revit_to_vr_common;
using WebSocketSharp;

namespace RevitToVR
{
    // receives data changes
    public class MainServiceClient
    {
        private WebSocket socket;
        private bool connected = false;
        private string uri = Configuration.uri + Configuration.mainPath;

        public delegate void OnMessageAction(object sender, MessageEventArgs args);

        public OnMessageAction OnMessage;
        
        public MainServiceClient()
        {
            socket = new WebSocket(uri);
            socket.WaitTime = TimeSpan.FromSeconds(1);
            socket.OnMessage += (sender, e) => UIConsole.Log("MainService (Server) sent: " + e.Data);
            
            socket.ConnectAsync();
            UIConsole.Log("Connecting to MainServiceClient with uri: " + uri);

            socket.OnOpen += OnOpen;
            socket.OnClose += OnClose;
            socket.OnMessage += OnMessageInternal;
        }

        public void Disconnect()
        {
            UIConsole.Log("Disconnected");
            socket.OnOpen -= OnOpen;
            socket.OnClose -= OnClose;
            socket.OnMessage -= OnMessageInternal;
            socket.Close();
            socket = null;
        }
        
        ~MainServiceClient()
        {
            Disconnect();
        }

        void OnOpen(object sender, EventArgs args)
        {
            UIConsole.Log("MainServiceClient > OnOpen");
            
            socket.Send("Connected from Unity!");
        }

        void OnClose(object sender, CloseEventArgs args)
        {
            UIConsole.Log("MainServiceClient > OnClose, reason: " + args.Reason);
        }

        void OnMessageInternal(object sender, MessageEventArgs args)
        {
            UIConsole.Log("MainServiceClient > OnMessage: " + args.Data);
            OnMessage?.Invoke(sender, args);
        }
    }
}