using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WebSocketSharp;
using System.Text.Json;
using revit_to_vr_common;
using Unity.Collections;
using UnityEngine.Rendering;

namespace RevitToVR
{
    public class VRApplication : MonoBehaviour
    {
        public string ipAddress;
        
        private MainServiceClient mainServiceClient_;
        private Database database_;

        // the event we received, so that we can parse the binary data that is sent using a separate .Send() after the event
        private revit_to_vr_common.Event cachedEvent_;
        
        private void Start()
        {
            database_ = new Database();
            UIConsole.Log("Started VRApplication");
            mainServiceClient_ = new MainServiceClient(ipAddress);
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
                UIConsole.Log("VRApplication > OnMessage: Received json: " + json);

                revit_to_vr_common.Event e = JsonSerializer.Deserialize<revit_to_vr_common.Event>(json, Configuration.jsonSerializerOptions);
                HandleEvent(e);
                cachedEvent_ = e;
            }
            else if (args.IsBinary)
            {
                UIConsole.Log("VRApplication > OnMessage: Received binary with length: " + args.RawData.Length);
                // handle binary
                if (cachedEvent_ == null)
                {
                    return;
                }
                
                UIConsole.Log("HandleBinary");
                HandleBinary(args.RawData);
                cachedEvent_ = null;
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
                case SendMeshDataEvent sendMeshDataEvent:
                    Handle(sendMeshDataEvent);
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

        private void Handle(SendMeshDataEvent e)
        {
            UIConsole.Log("Handle SendMeshDataEvent");
        }

        private void HandleBinary(byte[] buffer)
        {
            switch (cachedEvent_)
            {
                case SendMeshDataEvent sendMeshDataEvent:
                    HandleBinary(sendMeshDataEvent, buffer);
                    break;
            }
        }

        private void HandleBinary(SendMeshDataEvent e, byte[] buffer)
        {
            var descriptors = new NativeArray<VertexAttributeDescriptor>(2, Allocator.Temp);
            descriptors[0] = new VertexAttributeDescriptor()
            {
                attribute = VertexAttribute.Position,
                format = VertexAttributeFormat.Float32,
                dimension = 4
            };
            descriptors[1] = new VertexAttributeDescriptor()
            {
                attribute = VertexAttribute.Normal,
                format = VertexAttributeFormat.Float32,
                dimension = 4
            };

            int vertexCount = e.descriptor.vertexCount;
            
            Mesh mesh = new Mesh();
            mesh.SetVertexBufferParams(vertexCount, descriptors);
            mesh.SetVertexBufferData(buffer, 0, 0, vertexCount, 0, MeshUpdateFlags.Default);
            
            MeshDataRepository.Instance.AddMesh(e.descriptor.id, mesh);
        }
    }
}
