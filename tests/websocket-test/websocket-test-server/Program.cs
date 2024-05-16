using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace websocket_test_server
{
    public class TestServer : WebSocketBehavior
    {
        protected override void OnMessage(MessageEventArgs e)
        {
            Console.WriteLine("OnMessage from client: {0}", e.Data);
            var msg = e.Data == "This message is sent by the client"
                      ? "Received client response"
                      : "Got some other message";

            Send(msg);
        }

        protected override void OnClose(CloseEventArgs e)
        {
            base.OnClose(e);
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("---------- SERVER -----------");

            // Server
            string uri = "ws://192.168.0.100"; // ip address of the Parallels Windows VM. 
            var server = new WebSocketServer(uri);
            
            server.AddWebSocketService<TestServer>("/Test");
            server.Start();
            Console.WriteLine("Started WebSocketServer at {0}", uri);
            Console.ReadKey(true);
            server.Stop();
        }
    }
}
