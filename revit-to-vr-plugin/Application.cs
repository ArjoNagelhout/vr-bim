using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using Autodesk.Revit.UI.Selection;
using System.Windows;

using System.Diagnostics;

using WebSocketSharp;
using Autodesk.Revit.DB.Events;
using revit_to_vr_common;
using static revit_to_vr_plugin.DataConversion;
using System.Text.Json;
using Autodesk.Revit.Creation;
using Document = Autodesk.Revit.DB.Document;

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

    // contains information about which document is currently opened etc.
    public class ApplicationState
    {
        public Document openedDocument;
        public List<long> selectedElementIds = new List<long>();
        public EditMode editMode;

        // UIDocument exposes functionality for for example changing the selection state in Revit,
        // so we need it as well in addition to the Document
        public UIControlledApplication cachedUIControlledApplication;
        public bool registeredIdlingEvent = false;
        public UIApplication uiApplication;
        public UIDocument activeUIDocument;
    }

    // reset on each connection with the client
    public class ClientState
    {
        public ClientConfiguration clientConfiguration = new ClientConfiguration(); // default client configuration
        public bool wantsToReceiveEvents = false;
    }

    // this is the entry point for the application
    // the application sends data on an idle event, as we can't directly access the UIApplication and the documents
    // from the UIControlledApplication. 
    public class Application : IExternalApplication
    {
        private static Application instance_;
        public static Application Instance => instance_;

        private ClientEvent cachedEvent;

        // public properties        
        public Server server = new Server();

        // private properties
        
        private DockablePaneProvider paneProvider;

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

        private void RegisterIdlingEvent()
        {
            if (applicationState.registeredIdlingEvent)
            {
                return;
            }
            UIControlledApplication app = applicationState.cachedUIControlledApplication;
            Debug.Assert(app != null);
            app.Idling += OnIdling;
            applicationState.registeredIdlingEvent = true;
        }

        private void UnregisterIdlingEvent()
        {
            if (!applicationState.registeredIdlingEvent)
            {
                return;
            }
            UIControlledApplication app = applicationState.cachedUIControlledApplication;
            Debug.Assert(app != null);
            app.Idling -= OnIdling;
            applicationState.registeredIdlingEvent = false;
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

            // temp event for retrieving the UIApplication
            applicationState.cachedUIControlledApplication = uiApp;

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

        // used for setting the UIApplication... gotta love the Revit API
        private void OnIdling(object sender, IdlingEventArgs args)
        {
            Debug.Assert(sender is UIApplication);
            UIControlledApplication uiControlledApp = applicationState.cachedUIControlledApplication;
            Debug.Assert(uiControlledApp != null);

            UIApplication uiApp = sender as UIApplication;
            applicationState.uiApplication = uiApp;

            if (uiApp.ActiveUIDocument == null)
            {
                return;
            }

            applicationState.activeUIDocument = uiApp.ActiveUIDocument;
            UnregisterIdlingEvent();
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            UIConsole.Log("OnSelectionChanged");

            Debug.Assert(applicationState.openedDocument.CreationGUID == args.GetDocument().CreationGUID);

            ISet<ElementId> elements = args.GetSelectedElements();

            List<long> selectedElementIds = new List<long>(elements.Count);
            foreach (ElementId id in elements)
            {
                selectedElementIds.Add(id.Value);
            }

            // set cached selected element ids so that when the client connects
            // we can serve which elements are selected (we can't retrieve the selection easily another way on demand)
            applicationState.selectedElementIds = selectedElementIds;
            SendSelectionChangedEvent();
        }

        private void SendSelectionChangedEvent()
        {
            SelectionChangedEvent e = new SelectionChangedEvent()
            {
                selectedElementIds = applicationState.selectedElementIds
            };
            SendEventIfDesired(e);
        }

        private void SendDocumentChangedEvent(IEnumerable<ElementId> changedElementIds, IEnumerable<ElementId> deletedElementIds)
        {
            changedElementIds = FilterElements(changedElementIds);
            // we don't filter deletedElementIds, because that creates a FilteredElementCollector, which checks for the Elements in the database,
            // but because it has just been deleted, it doesn't exist, and thus would set deletedElementIds to an empty enumerable. 

            DocumentChangedEvent e = new DocumentChangedEvent()
            {
                deletedElementIds = new List<long>(),
                changedElements = new Dictionary<long, VRBIM_Element>()
            };

            foreach (ElementId deletedElement in deletedElementIds)
            {
                e.deletedElementIds.Add(deletedElement.Value);
            }

            Queue<MeshDataToSend> toSend = new Queue<MeshDataToSend>();

            try
            {
                foreach (ElementId elementId in changedElementIds)
                {
                    Element element = applicationState.openedDocument.GetElement(elementId);
                    VRBIM_Element targetElement = DataConversion.Convert(element, clientState, toSend, clientState.clientConfiguration);
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
            SendEventIfDesired(e);

            string json = JsonSerializer.Serialize(e, Configuration.jsonSerializerOptions);

            // send mesh data
            foreach (MeshDataToSend meshData in toSend)
            {
                SendMeshDataEvent sendMeshDataEvent = new SendMeshDataEvent()
                {
                    descriptor = meshData.descriptor
                };

                SendEventIfDesired(sendMeshDataEvent);
                MainService.SendBytes(meshData.data);
            }
            toSend.Clear();
        }

        private void OnDocumentChanged(object sender, DocumentChangedEventArgs args)
        {
            UIConsole.Log("OnDocumentChanged");

            // we don't support editing multiple documents at the same time
            Document document = args.GetDocument();
            Debug.Assert(document.CreationGUID == applicationState.openedDocument.CreationGUID);

            ICollection<ElementId> added = args.GetAddedElementIds();
            ICollection<ElementId> deleted = args.GetDeletedElementIds();
            ICollection<ElementId> modified = args.GetModifiedElementIds();

            // we don't care if an element is added or changed, it needs to be updated anyway
            // so on client-side we delete the old data stored for this element id
            IEnumerable<ElementId> changed = modified.Union(added);

            SendDocumentChangedEvent(changed, deleted);
        }

        private void OnDocumentClosed(object sender, DocumentClosedEventArgs args)
        {
            UIConsole.Log("OnDocumentClosed");
            int documentId = args.DocumentId;
            applicationState.openedDocument = null;

            // send event
            DocumentClosedEvent e = new DocumentClosedEvent();
            SendEventIfDesired(e);

            // unregister
            UnregisterIdlingEvent();
        }

        private void OnDocumentCreated(object sender, DocumentCreatedEventArgs args)
        {
            UIConsole.Log("OnDocumentCreated");
            Document document = args.Document;
            applicationState.openedDocument = document;

            // send event
            SendDocumentOpenedEvent();

            // register
            RegisterIdlingEvent();
        }

        private void OnDocumentOpened(object sender, DocumentOpenedEventArgs args)
        {
            UIConsole.Log("OnDocumentOpened");
            Document document = args.Document;
            applicationState.openedDocument = document;

            // send event
            SendDocumentOpenedEvent();
            
            // register
            RegisterIdlingEvent();
        }

        private void SendDocumentOpenedEvent()
        {
            Document d = applicationState.openedDocument;
            Debug.Assert(d != null);
            DocumentOpenedEvent e = new DocumentOpenedEvent()
            {
                documentGuid = d.CreationGUID
            };
            SendEventIfDesired(e);

            // todo: send DocumentChangedEvent with all elements in the document that were changed
            SendAllElements();

            SendSelectionChangedEvent();
        }

        // called by MainService

        public void OnClientSentMessage(MessageEventArgs args)
        {
            if (args.IsText)
            {
                // handle text, should be json
                // handle text
                string json = args.Data;
                // validate json
                if (string.IsNullOrWhiteSpace(json))
                {
                    return;
                }

                try
                {
                    using var _ = JsonDocument.Parse(json);
                }
                catch
                {
                    return;
                }

                UIConsole.Log("Application > OnMessage: Received json: " + json);

                JsonSerializerOptions options = Configuration.jsonSerializerOptions;
                ClientEvent e = JsonSerializer.Deserialize<ClientEvent>(json, options);

                HandleEvent(e);
                cachedEvent = e;
            }
            else if (args.IsBinary)
            {
                // handle binary data
            }
        }

        private void SendEventIfDesired(ServerEvent e)
        {
            MainService.SendJson(e, !clientState.wantsToReceiveEvents);
        }

        private void HandleEvent(ClientEvent e)
        {
            switch (e)
            {
                case SendClientConfigurationEvent sendClientConfigurationEvent:
                    HandleSendClientConfigurationEvent(sendClientConfigurationEvent);
                    break;
                case StartListeningToEvents startListeningToEvents:
                    HandleStartListeningToEvents(startListeningToEvents);
                    break;
                case StopListeningToEvents stopListeningToEvents:
                    HandleStopListeningToEvents(stopListeningToEvents);
                    break;
                case StartEditMode startEditMode:
                    HandleStartEditMode(startEditMode);
                    break;
                case StopEditMode stopEditMode:
                    HandleStopEditMode(stopEditMode);
                    break;
                case SelectElementClientEvent selectElement:
                    HandleSelectElement(selectElement);
                    break;
            }
        }

        private void HandleSendClientConfigurationEvent(SendClientConfigurationEvent e)
        {
            clientState.clientConfiguration = e.clientConfiguration;
        }

        private void HandleStartListeningToEvents(StartListeningToEvents e)
        {
            clientState.wantsToReceiveEvents = true;

            // on first start listening, make sure we catch up with everything that happened in the document
            // before we started listening:

            // send document opened event if a document is already opened
            if (applicationState.openedDocument != null)
            {
                SendDocumentOpenedEvent();
            }
        }
        
        private void HandleStopListeningToEvents(StopListeningToEvents e)
        {
            clientState.wantsToReceiveEvents = false;
        }

        // handle edit modes

        private void HandleStartEditMode(StartEditMode e)
        {
            // stop current mode if needed
            if (applicationState.editMode != null)
            {
                SendStopEditMode();
            }

            EditMode editMode = null;
            switch (e.data)
            {
                case ToposolidEditSketchEditModeData toposolidEditSketch:
                    editMode = new ToposolidEditSketchEditMode();
                    break;
                case ToposolidModifySubElementsEditModeData toposolidModifySubElements:
                    editMode = new ToposolidModifySubElementsEditMode();
                    break;
            }
            Debug.Assert(editMode != null);
            editMode.editModeData = e.data;
            editMode.StartEditMode();

            MainService.SendJson(new StartedEditMode() { populatedEditModeData = editMode.editModeData });
        }

        private void HandleStopEditMode(StopEditMode e)
        {
            EditMode editMode = applicationState.editMode;
            Debug.Assert(editMode != null && editMode.editModeData.GetType() == e.data.GetType());
            SendStopEditMode();
        }

        private void SendStopEditMode()
        {
            EditMode editMode = applicationState.editMode;
            Debug.Assert(editMode != null);
            editMode.StopEditMode();
            MainService.SendJson(new StoppedEditMode() { stoppedEditModeData = editMode.editModeData });
            editMode = null;
        }

        private void HandleSelectElement(SelectElementClientEvent e)
        {
            Debug.Assert(applicationState.activeUIDocument != null);

            // update selection inside Revit
            Selection selection = applicationState.activeUIDocument.Selection;
            HashSet<long> ids = selection
                .GetElementIds()
                .Select((elementId) => elementId.Value)
                .ToHashSet();

            HashSet<long> providedIds = e.selectedElementIds.ToHashSet();

            switch (e.type)
            {
                case SelectElementType.New:
                    // create new selection of the given element ids
                    ids = providedIds;
                    break;
                case SelectElementType.Add:
                    // add the provided ids to the current selection
                    ids.UnionWith(providedIds);
                    break;
                case SelectElementType.Remove:
                    ids.ExceptWith(providedIds);
                    break;
            }

            selection.SetElementIds(ids.Select((value) => new ElementId(value)).ToList());

            // this will automatically call the OnSelectionChanged event, so we
            // don't have to do anything additional

        }

        public void OnClientConnected()
        {
            UIConsole.Log("Application > OnClientConnected");

            // reset state (e.g. which GeometryObjects have already been sent)
            clientState = new ClientState();
        }

        public void OnClientDisconnected()
        {
            UIConsole.Log("Application > OnClientDisconnected");
        }

        private void SendAllElements()
        {
            Debug.Assert(applicationState.openedDocument != null);

            FilteredElementCollector elements = new FilteredElementCollector(applicationState.openedDocument);
            elements = elements.WhereElementIsNotElementType(); // inverts, so we collect all

            SendDocumentChangedEvent(elements.ToElementIds(), new List<ElementId>());
        }

        // this is the filter of what elements we want to collect. Should be changed into
        // a configuration 
        // and it's inefficient to create the collector twice, but works for now. 
        private IEnumerable<ElementId> FilterElements(IEnumerable<ElementId> elementIds)
        {
            if (elementIds.Count() == 0)
            {
                return elementIds;
            }

            // we can filter on category via ElementCategoryFilter
            // we can filter on type via ElementClassFilter

            // the following commented out code can be used to get *all* elements

            //FilteredElementCollector elements = new FilteredElementCollector(applicationState.openedDocument);
            //elements = elements.WhereElementIsNotElementType();
            
            // for our test, we want to only send the subset of elements
            // that have been implemented, such as the Toposolid. 
            FilteredElementCollector elements = new FilteredElementCollector(applicationState.openedDocument, elementIds.ToList());

            elements = elements.OfClass(typeof(Toposolid));
            return elements.ToElementIds();
        }
    }
}