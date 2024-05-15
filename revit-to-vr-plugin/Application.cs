﻿using System;
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

            Debug.WriteLine("Startup of Application, amazing");

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
            ISet<ElementId> elements = args.GetSelectedElements();
        }

        void OnDocumentChanged(object sender, DocumentChangedEventArgs args)
        {
            Document document = args.GetDocument();
            ICollection<ElementId> added = args.GetAddedElementIds();
            ICollection<ElementId> deleted = args.GetDeletedElementIds();
            ICollection<ElementId> modified = args.GetModifiedElementIds();

            Element e = document.GetElement(added.First());
            GeometryElement geometry = e.get_Geometry(new Options()
            {
                DetailLevel = ViewDetailLevel.Coarse,
                ComputeReferences = true,
                IncludeNonVisibleObjects = true
            });
            Material material = geometry.MaterialElement;
            
            BoundingBoxXYZ bounds = geometry.GetBoundingBox();
            
            foreach (GeometryObject obj in geometry)
            {
                // handle all cases that the geometry could be
                if (obj is Solid)
                {
                    Solid solid = obj as Solid;
                    FaceArray faces = solid.Faces;
                    foreach (Face face in faces)
                    {
                        Mesh mesh = face.Triangulate(1);
                        IList<XYZ> vertices = mesh.Vertices;
                        IList<XYZ> normals = mesh.GetNormals();
                    }
                }
                else if (obj is Mesh)
                {
                    Mesh mesh = obj as Mesh;
                    
                }
                else if (obj is GeometryInstance)
                {
                    GeometryInstance instance = obj as GeometryInstance;
                }
                else if (obj is Curve)
                {
                    Curve curve = obj as Curve;
                }
                else if (obj is Autodesk.Revit.DB.Point)
                {
                    Autodesk.Revit.DB.Point point = obj as Autodesk.Revit.DB.Point;
                }
                else if (obj is PolyLine)
                {
                    PolyLine polyLine = obj as PolyLine;
                }
            }
        }

        void OnDocumentClosed(object sender, DocumentClosedEventArgs args)
        {
            int documentId = args.DocumentId;
        }

        void OnDocumentCreated(object sender, DocumentCreatedEventArgs args)
        {
            Document document = args.Document;
        }

        void OnDocumentOpened(object sender, DocumentOpenedEventArgs args)
        {
            Document document = args.Document;
            Guid id = document.CreationGUID;
        }
    }
}