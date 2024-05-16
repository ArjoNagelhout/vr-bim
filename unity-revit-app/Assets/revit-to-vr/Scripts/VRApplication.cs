using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using System.Text.Json;
using revit_to_vr_common;

namespace RevitToVR
{
    public class VRApplication : MonoBehaviour
    {
        private MainServiceClient mainServiceClient_;
        private Database database_;
        
        private void Start()
        {
            database_ = new Database();
            UIConsole.Log("Started VRApplication");
            mainServiceClient_ = new MainServiceClient();
            mainServiceClient_.OnMessage += OnMessage;
        }

        private void OnDestroy()
        {
            mainServiceClient_.OnMessage -= OnMessage;
            mainServiceClient_.Disconnect();
            mainServiceClient_ = null;
        }

        // server to client communication
        private void OnMessage(object sender, MessageEventArgs args)
        {
            if (args.IsText)
            {
                // handle text
                string json = args.Data;

                revit_to_vr_common.Event e = JsonSerializer.Deserialize<revit_to_vr_common.Event>(json);
                HandleEvent(e);
            }
            else if (args.IsBinary)
            {
                // handle binary
                byte[] data = args.RawData;
            }
        }

        private void HandleEvent(revit_to_vr_common.Event @event)
        {
            switch (@event)
            {
                case DocumentChangedEvent documentChangedEvent:
                    Handle(documentChangedEvent);
                    break;
                case DocumentOpenedEvent documentOpenedEvent:
                    Handle(documentOpenedEvent);
                    break;
                case DocumentClosedEvent documentClosedEvent:
                    Handle(documentClosedEvent);
                    break;
                case SelectionChangedEvent selectionChangedEvent:
                    Handle(selectionChangedEvent);
                    break;
            }
        }

        private void Handle(DocumentChangedEvent e)
        {
            UIConsole.Log("Handle DocumentChangedEvent");
        }

        private void Handle(DocumentOpenedEvent e)
        {
            UIConsole.Log("Handle DocumentOpenedEvent");
        }

        private void Handle(DocumentClosedEvent e)
        {
            UIConsole.Log("Handle DocumentClosedEvent");
        }

        private void Handle(SelectionChangedEvent e)
        {
            UIConsole.Log("Handle SelectionChangedEvent");
        }
    }
}
