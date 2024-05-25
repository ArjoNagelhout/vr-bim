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

using System.Diagnostics;

using WebSocketSharp;
using WebSocketSharp.Server;
using System.Windows.Interop;
using Autodesk.Revit.DB.Events;
using revit_to_vr_common;
using static revit_to_vr_plugin.DataConversion;
using System.Text.Json;
using System.Windows.Markup;

namespace revit_to_vr_plugin
{
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
        public DockablePaneCreator()
        {
        }

        FrameworkElement IFrameworkElementCreator.CreateFrameworkElement()
        {
            return new RevitToVRDockablePane();
        }
    }

    public class DockablePaneProvider : IDockablePaneProvider
    {
        private DockablePaneCreator creator;

        public DockablePaneProvider()
        {
            creator = new DockablePaneCreator();
        }

        void IDockablePaneProvider.SetupDockablePane(DockablePaneProviderData data)
        {
            data.FrameworkElement = null;
            data.FrameworkElementCreator = creator;
        }
    }
    
    public class SentGeometryObjectData
    {
        public long geometryObjectId; // non-view specific
        public int hashCode; // hash code to see whether we need to sent the updated geometry
    }

    // contains information about which document is currently opened etc.
    public class ApplicationState
    {
        public Document openedDocument;
    }

    // reset on each connection with the client
    public class ClientState
    {
        public Dictionary<long, SentGeometryObjectData> sentGeometry = new Dictionary<long, SentGeometryObjectData>();
    }

    // this is the entry point for the application
    // the application sends data on an idle event, as we can't directly access the UIApplication and the documents
    // from the UIControlledApplication. 
    public class Application : IExternalApplication
    {
        private static Application instance_;
        public static Application Instance => instance_;

        // public properties        
        public Server server = new Server();

        // private properties
        
        private DockablePaneProvider paneProvider;
        private uint connectionCount = 0;

        // state
        ApplicationState applicationState = new ApplicationState(); // server side state information
        ClientState clientState = new ClientState();

        // methods
        public Application()
        {
            Debug.Assert(instance_ == null);
            instance_ = this;
            paneProvider = new DockablePaneProvider();
            UIConsole.Log("RevitToVR application started");
        }

        Result IExternalApplication.OnStartup(UIControlledApplication uiApp)
        {
            ControlledApplication app = uiApp.ControlledApplication;

            // register events
            uiApp.SelectionChanged += OnSelectionChanged;

            app.DocumentChanged += OnDocumentChanged;
            app.DocumentClosed += OnDocumentClosed;
            app.DocumentCreated += OnDocumentCreated;
            app.DocumentOpened += OnDocumentOpened;

            // add UI elements
            uiApp.CreateRibbonTab(Constants.tabName);
            uiApp.CreateRibbonPanel(Constants.tabName, Constants.sessionPanelName);
            uiApp.RegisterDockablePane(new DockablePaneId(Guid.NewGuid()), Constants.dockablePaneName, paneProvider);

            return Result.Succeeded;
        }

        Result IExternalApplication.OnShutdown(UIControlledApplication uiApp)
        {
            ControlledApplication app = uiApp.ControlledApplication;

            // unregister events
            uiApp.SelectionChanged -= OnSelectionChanged;

            app.DocumentChanged -= OnDocumentChanged;
            app.DocumentClosed -= OnDocumentClosed;
            app.DocumentCreated -= OnDocumentCreated;
            app.DocumentOpened -= OnDocumentOpened;

            return Result.Succeeded;
        }

        void OnSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            UIConsole.Log("OnSelectionChanged");

            ISet<ElementId> elements = args.GetSelectedElements();

            // send event
            SelectionChangedEvent e = new SelectionChangedEvent();
            MainService.SendJson(e);
        }

        void OnDocumentChanged(object sender, DocumentChangedEventArgs args)
        {
            UIConsole.Log("OnDocumentChanged");

            DocumentChangedEvent e = new DocumentChangedEvent()
            {
                deletedElementIds = new List<long>(),
                changedElements = new Dictionary<long, VRBIM_Element>()
            };

            Queue<MeshDataToSend> toSend = new Queue<MeshDataToSend>();

            try
            {
                Document document = args.GetDocument();
                ICollection<ElementId> added = args.GetAddedElementIds();
                ICollection<ElementId> deleted = args.GetDeletedElementIds();
                ICollection<ElementId> modified = args.GetModifiedElementIds();

                foreach (ElementId elementId in deleted)
                {
                    e.deletedElementIds.Add(elementId.Value);
                }

                // we don't care if an element is added or changed, it needs to be updated anyway
                // so on client-side we delete the old data stored for this element id
                IEnumerable<ElementId> changed = modified.Union(added); 
                foreach (ElementId elementId in changed)
                {
                    Element element = document.GetElement(elementId);
                    UIConsole.Log(elementId.ToString());
                    
                    VRBIM_Element targetElement = DataConversion.Convert(element, clientState, toSend);
                    if (targetElement != null)
                    {
                        e.changedElements.Add(elementId.Value, targetElement);
                    }
                }
            }
            catch (Exception exception)
            {
                UIConsole.Log("Error: " + exception.Message);
            }


            // send event
            MainService.SendJson(e);

            string json = JsonSerializer.Serialize(e, Configuration.jsonSerializerOptions);

            // send mesh data
            foreach (MeshDataToSend meshData in toSend)
            {
                SendMeshDataEvent sendMeshDataEvent = new SendMeshDataEvent()
                {
                    descriptor = meshData.descriptor
                };

                MainService.SendJson(sendMeshDataEvent);
                MainService.SendBytes(meshData.data);
            }
            toSend.Clear();
        }

        void OnDocumentClosed(object sender, DocumentClosedEventArgs args)
        {
            UIConsole.Log("OnDocumentClosed");
            int documentId = args.DocumentId;
            applicationState.openedDocument = null;

            // send event
            DocumentClosedEvent e = new DocumentClosedEvent();
            MainService.SendJson(e);
        }

        void OnDocumentCreated(object sender, DocumentCreatedEventArgs args)
        {
            UIConsole.Log("OnDocumentCreated");
            Document document = args.Document;
            applicationState.openedDocument = document;

            // send event
            SendDocumentOpenedEvent();
        }

        void OnDocumentOpened(object sender, DocumentOpenedEventArgs args)
        {
            UIConsole.Log("OnDocumentOpened");
            Document document = args.Document;
            applicationState.openedDocument = document;
            
            // send event
            SendDocumentOpenedEvent();
        }

        void SendDocumentOpenedEvent()
        {
            Document d = applicationState.openedDocument;
            Debug.Assert(d != null);
            DocumentOpenedEvent e = new DocumentOpenedEvent()
            {
                documentGuid = d.CreationGUID
            };
            MainService.SendJson(e);
        }

        // called by MainService

        public void OnClientSentMessage(MessageEventArgs args)
        {
            if (args.IsText)
            {
                // handle text, should be json

            }
            else if (args.IsBinary)
            {
                // handle binary data
            }
        }

        public void OnClientConnected()
        {
            connectionCount++;
            UIConsole.Log("Application > OnClientConnected");

            // reset state (e.g. which GeometryObjects have already been sent)
            clientState = new ClientState();

            // send document opened event if a document is already opened
            if (applicationState.openedDocument != null)
            {
                SendDocumentOpenedEvent();

                // todo: send DocumentChangedEvent with all elements in the document that were changed
            }
        }

        public void OnClientDisconnected()
        {
            connectionCount--;
            UIConsole.Log("Application > OnClientDisconnected");
        }
    }
}