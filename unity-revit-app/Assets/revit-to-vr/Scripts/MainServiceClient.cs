using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.Json;
using revit_to_vr_common;
using UnityEngine;
using WebSocketSharp;

namespace RevitToVR
{
    // receives data changes
    public class MainServiceClient
    {
        private static MainServiceClient _instance;
        public static MainServiceClient instance => _instance;
        
        private WebSocket socket;
        private string uri;

        public delegate void OnMessageAction(object sender, MessageEventArgs args);

        public OnMessageAction OnMessage;

        public Action OnOpen;

        public Action OnClose;
        
        public MainServiceClient(string ipAddress)
        {
            Debug.Assert(_instance == null);
            _instance = this;
            
            uri = Configuration.protocolPrefix + ipAddress + Configuration.mainPath;
            socket = new WebSocket(uri);
            socket.WaitTime = TimeSpan.FromSeconds(5);

            socket.OnOpen += OnOpenInternal;
            socket.OnClose += OnCloseInternal;
            socket.OnMessage += OnMessageInternal;
        }

        public void Connect()
        {
            UIConsole.Log("Connecting to MainService with uri: " + uri);
            socket.Connect();
        }

        public void Disconnect()
        {
            UIConsole.Log("Disconnected");
            socket.OnOpen -= OnOpenInternal;
            socket.OnClose -= OnCloseInternal;
            socket.OnMessage -= OnMessageInternal;
            socket.Close();
            socket = null;
        }
        
        ~MainServiceClient()
        {
            Disconnect();
        }

        private void OnOpenInternal(object sender, EventArgs args)
        {
            UIConsole.Log("MainServiceClient > OnOpen");
            // socket.Send("Connected from Unity!");
            OnOpen?.Invoke();
        }

        private void OnCloseInternal(object sender, CloseEventArgs args)
        {
            UIConsole.Log("MainServiceClient > OnClose, reason: " + args.Reason);
            OnClose?.Invoke();
        }

        private void OnMessageInternal(object sender, MessageEventArgs args)
        {
            MainThreadDispatcher.Instance.Enqueue(() =>
            {
                //UIConsole.Log("MainServiceClient > OnMessage: " + args.Data);
                OnMessage?.Invoke(sender, args);                
            });
        }
        
        // sending data from client to server
        // dry makes it so it doesn't actually send it to the client, but does serialize
        public void SendJson<T>(T data, bool dry = false)
        {
            string json = JsonSerializer.Serialize(data, Configuration.jsonSerializerOptions);

            // to check why deserialization might fail:
            //try
            //{
            //    T test = JsonSerializer.Deserialize<T>(json, Configuration.jsonSerializerOptions);
            //}
            //catch (Exception e)
            //{
            //    UIConsole.Log($"Deserialization failed: {e.Message}");
            //}
    
            SendJsonInternal(json, dry);
        }

        private void SendJsonInternal(string json, bool dry)
        {
            if (socket != null)
            {
                if (!dry)
                {
                    socket.Send(json);
                }
                //UIConsole.Log("MainServiceClient > SendJson: " + json);
            }
            else
            {
                //UIConsole.Log("json (not sent): " + json);
            }
        }
    }
}