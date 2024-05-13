using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.UI.Events;
using Autodesk.Revit.DB.Architecture;
using System.Windows;

using System.Net.Sockets;
using System.Net;

using WebSocketSharp;
using WebSocketSharp.Server;

namespace revit_to_vr_plugin
{
    // we need to send the data over the network to the Unity Runtime, installed on the
    // Meta Quest Pro. We could do this via the following methods:
    // 
    // - http server, this would be hard to set up bi-directional data, as it would require the Client to poll whether changes have made
    // - websockets
    // - RPC
    //
    // Let's use websockets
    //
    // We build the VR application with the IP address of the Server and the secret API authentication key. 

    // resources:
    // https://thebuildingcoder.typepad.com/blog/2013/08/determining-absolutely-all-visible-elements.html
    // https://thebuildingcoder.typepad.com/blog/about-the-author.html#5.4
    // https://github.com/imAliAsad/VisualStudioRevitTemplate/tree/master

    // https://help.autodesk.com/view/RVT/2022/ENU/?guid=Revit_API_Revit_API_Developers_Guide_Advanced_Topics_Events_Database_Events_DocumentChanged_event_html
    // Revit DocumentChanged -> Read only
    // We need to listen to Database events, UIControlledApplication is about UI events. 
    // "Use IUpdate Framework to make changes to the Database"
    // how do we listen to currently open documents?
    // that should be the minimum...
    // I don't want to just get the ActiveUIDocument

    // ControlledApplication does not have access to documents...
    // https://www.revitapidocs.com/2024/35859972-2407-3910-cb07-bbb337e307e6.htm
    // so we need Application, that does have document events
    //
    // Turns out an IExternalApplication can edit the UI, but not the document

    // registration of addins:
    // https://help.autodesk.com/view/RVT/2024/ENU/?guid=Revit_API_Revit_API_Developers_Guide_Introduction_Getting_Started_Using_the_Autodesk_Revit_API_Registration_of_add_ins_html

    // todo:
    // - [ ] create websocket connection / websocket server that can be started and stopped from UI (send dummy data)
    // - [ ] receive data in Unity client
    // - [ ] send geometry over websocket on change to Unity. 

    public static class Constants
    {
        public static string tabName = "RevitToVR";
        public static string sessionPanelName = "Session";
        public static string dockablePaneName = "RevitToVR";
    }

    // WPF is used for defining the UI. 
    // a XAML file defines the hierarchy and data binding, and we create the class here (see DockablePane.xaml and DockablePane.xaml.cs)
    public class DockablePaneCreator : IFrameworkElementCreator
    {
        FrameworkElement IFrameworkElementCreator.CreateFrameworkElement()
        {
            return new RevitToVRDockablePane();
        }
    }

    public class DockablePaneProvider : IDockablePaneProvider
    {
        private DockablePaneCreator creator = new DockablePaneCreator();

        void IDockablePaneProvider.SetupDockablePane(DockablePaneProviderData data)
        {
            data.FrameworkElement = null;
            data.FrameworkElementCreator = creator;
        }
    }

    public class Application : IExternalApplication
    {
        // properties
        private DockablePaneProvider pane = new DockablePaneProvider();
        private EventHandler<SelectionChangedEventArgs> onSelectionChanged;
        private RevitToVRServer server = new RevitToVRServer();

        // methods
        public Application()
        {
            onSelectionChanged = OnSelectionChanged;
        }

        Result IExternalApplication.OnStartup(UIControlledApplication application)
        {
            // add UI elements
            application.SelectionChanged += onSelectionChanged;
            application.CreateRibbonTab(Constants.tabName);
            application.CreateRibbonPanel(Constants.tabName, Constants.sessionPanelName);
            application.RegisterDockablePane(new DockablePaneId(Guid.NewGuid()), Constants.dockablePaneName, pane);
            
            return Result.Succeeded;
        }

        Result IExternalApplication.OnShutdown(UIControlledApplication application)
        {
            application.SelectionChanged -= onSelectionChanged;
            return Result.Succeeded;
        }

        void OnSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            ISet<ElementId> elements = args.GetSelectedElements();
            
        }
    }

    // websockets
    // https://developer.mozilla.org/en-US/docs/Web/API/WebSockets_API/Writing_WebSocket_server
    // and https://developer.mozilla.org/en-US/docs/Web/API/WebSockets_API/Writing_WebSocket_servers

    // should be done asynchronously as creating the RevitToVRServer should not be blocking. 
    // the Revit API is single threaded. 
    // maybe use a main thread dispatcher? Or some event system?

    // we need to listen to incoming TCP connections, as WebSockets are implemented on top of TCP.
    // which port?

    // todo: read RFC 6455 (WebSockets protocol standardized by IETF (Internet Engineering Task Force))
    // use: https://github.com/sta/websocket-sharp,
    // because implementing the protocol ourselves is silly. 

    public class TestWebSocket : WebSocketBehavior
    {
        protected override void OnMessage(MessageEventArgs e)
        {
            var msg = e.Data == "Test"
                      ? "Received test"
                      : "Nope, that was something else.";

            TaskDialog dialog = new TaskDialog(string.Format("We got a message: {0}", msg));
            dialog.Show();

            Send(msg);
        }
    }

    public class RevitToVRServer
    {
        // properties


        // methods
        public RevitToVRServer()
        {
            // todo: change to non-blocking
            //Execute();
            ExecuteWebsocket();
        }

        private void ExecuteWebsocket()
        {
            WebSocketServer server = new WebSocketServer("ws://127.0.0.1");

            server.AddWebSocketService<TestWebSocket>("/Test");
            server.Start();
            while (true)
            {

            }
            server.Stop();
        }

        private void Execute()
        {
            int port = 80;
            IPAddress address = IPAddress.Parse("127.0.0.1"); // localhost (loopback ip)
            TcpListener server = new TcpListener(address, port);
            server.Start();
            Console.WriteLine("Server started on {0}:{1}, waiting for client to connect", address, port);
            
            TcpClient client = server.AcceptTcpClient(); // blocking call, TcpClient is a thin wrapper on top of Socket
            Console.WriteLine("A client connected!", address, port);

            NetworkStream stream = client.GetStream();

            while (true)
            {
                if (!stream.DataAvailable)
                {
                    continue;
                }

                byte[] bytes = new byte[client.Available];
                stream.Read(bytes, 0, bytes.Length);

                Console.WriteLine();

                TaskDialog dialog = new TaskDialog(string.Format("Read {0} bytes", bytes.Length));
                dialog.Show();

                // this works :)
                // read 706 bytes. 
            }

        }
    }
}