using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;

namespace websocket_test_client
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("---------- CLIENT -----------");
            string uri = "ws://10.211.55.3/Test"; // ip address of the Parallels Windows VM. 
            // Client
            using (var socket = new WebSocket(uri))
            {
                socket.OnMessage += (sender, e) =>
                          Console.WriteLine("Test WebSocket Server sent data: " + e.Data);

                socket.Connect();
                Console.WriteLine("Connected WebSocket client to server at {0}", uri);

                socket.Send("This message is sent by the client");
                Console.ReadKey(true);
            }
        }
    }
}
