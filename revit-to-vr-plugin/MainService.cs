using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using revit_to_vr_common;
using System.Text.Json;
using WebSocketSharp;
using WebSocketSharp.Server;
using System.Diagnostics;

namespace revit_to_vr_plugin
{
    public class MainService : WebSocketBehavior
    {
        private static MainService instance_;
        public static MainService Instance => instance_;

        public MainService()
        {
            instance_ = this;
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            UIConsole.Log("MainService > OnMessage > " + e.Data);
            Application.Instance.OnClientSentMessage(e);
        }

        protected override void OnClose(CloseEventArgs e)
        {
            UIConsole.Log("MainService > OnClose");
            Application.Instance.OnClientDisconnected();
        }

        protected override void OnOpen()
        {
            UIConsole.Log("MainService > OnOpen");
            Application.Instance.OnClientConnected();
            Send("You got connected, amazing");
        }

        public static void SendJsonAsync<T>(T data, Action<bool> onComplete)
        {
            string json = JsonSerializer.Serialize(data);
            if (Instance != null)
            {
                Instance.Send(json);
                UIConsole.Log("MainService > OnSendJson: " + json);
            }
            else
            {
                UIConsole.Log("No Client connected, so no need to send json");
            }
        }
    }

    // server is responsible for sending events and data to the
    public class Server
    {
        // properties
        WebSocketServer server;

        // methods
        public Server()
        {
            string uri = Configuration.uri;
            server = new WebSocketServer(uri);
            server.AddWebSocketService<MainService>(Configuration.mainPath);
            server.Start();
            UIConsole.Log(string.Format("Started WebSocketServer at {0} with service MainService at {1}", uri, Configuration.mainPath));
        }

        ~Server()
        {
            // Disposable would be cleaner, but for now this suffices (https://stackoverflow.com/questions/13988334/difference-between-destructor-dispose-and-finalize-method)
            server.Stop();
        }
    }
}
