using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using revit_to_vr_common;
using System.Text.Json;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace revit_to_vr_plugin
{
    internal class MainService : WebSocketBehavior
    {
        

        protected override void OnMessage(MessageEventArgs e)
        {
            
        }

        protected override void OnClose(CloseEventArgs e)
        {
            
        }

        protected override void OnOpen()
        {
            
        }
    }

    // server is responsible for sending events and data to the
    internal class Server
    {
        // properties
        WebSocketServer server;

        // methods
        public Server()
        {
            server = new WebSocketServer(Configuration.uri);
            server.AddWebSocketService<MainService>(Configuration.mainPath);
            server.Start();
        }

        ~Server()
        {
            // Disposable would be cleaner, but for now this suffices (https://stackoverflow.com/questions/13988334/difference-between-destructor-dispose-and-finalize-method)
            server.Stop();
        }

        public void SendJson<T>(T data)
        {
            string json = JsonSerializer.Serialize(data);
            
        }
    }
}
