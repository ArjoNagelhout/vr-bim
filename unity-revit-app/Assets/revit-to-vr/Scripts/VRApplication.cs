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

        public struct VRBIM_Vertex
        {
            public Vector3 position;
        }

        private void HandleBinary(SendMeshDataEvent e, byte[] buffer)
        {
            var descriptors = new NativeArray<VertexAttributeDescriptor>(1, Allocator.Temp);
            descriptors[0] = new VertexAttributeDescriptor()
            {
                attribute = VertexAttribute.Position,
                format = VertexAttributeFormat.Float32,
                dimension = 3
            };
            // descriptors[1] = new VertexAttributeDescriptor()
            // {
            //     attribute = VertexAttribute.Normal,
            //     format = VertexAttributeFormat.Float32,
            //     dimension = 3
            // };

            int vertexCount = e.descriptor.vertexCount;
            int vector3SizeInBytes = 3 * 4; // 3 components * 4 bytes per float
            int vertexStrideInBytes = vector3SizeInBytes; //* 2; // 2 attributes (position and float)
            
            Mesh mesh = new Mesh();
            mesh.SetVertexBufferParams(vertexCount, descriptors);
            
            Bounds bounds = new Bounds();
            for (int i = 0; i < vertexCount; i++)
            {
                float x = BitConverter.ToSingle(buffer, i * vertexStrideInBytes);
                float y = BitConverter.ToSingle(buffer, i * vertexStrideInBytes + vector3SizeInBytes);
                float z = BitConverter.ToSingle(buffer, i * vertexStrideInBytes + vector3SizeInBytes * 2);
                Vector3 pos = new Vector3(x, y, z);
                UIConsole.Log($"vertex at index: {i}: {pos.ToString()}");
                bounds.Encapsulate(pos); // apparently this is required because Unity crashes otherwise.
            }
            
            // count = amount of *vertices* to copy, not bytes
            mesh.SetVertexBufferData(buffer, 0, 0, vertexCount, 0, MeshUpdateFlags.DontRecalculateBounds);
            
            // we need to set the indices
            int indexCount = e.descriptor.indexCount;
            UIConsole.Log($"indexCount: {indexCount}");
            mesh.SetIndexBufferParams(indexCount, IndexFormat.UInt32);
            
            int indexSizeInBytes = 4;
            byte[] indices = new byte[indexCount * indexSizeInBytes];

            int offset = vertexCount * vertexStrideInBytes;
            Buffer.BlockCopy(buffer, offset, indices, 0, indexCount * indexSizeInBytes);

            bool firstOk = false;
            
            for (int i = 0; i < indexCount; i++)
            {
                UInt32 index = BitConverter.ToUInt32(indices, i * indexSizeInBytes);

                if (!firstOk && index > 0 && index < vertexCount)
                {
                    UIConsole.Log($"First ok index index: {i}");
                    firstOk = true;
                }
                
                UIConsole.Log($"index: {index}");
            }
            
            mesh.SetIndexBufferData(indices, 0, 0, indexCount, MeshUpdateFlags.DontRecalculateBounds);
            
            // we also need to set a submesh, otherwise it won't show any triangles
            mesh.subMeshCount = 1;
            var subMeshDescriptor = new SubMeshDescriptor()
            {   
                topology = MeshTopology.Triangles,
                baseVertex = 0,
                indexStart = 0,
                indexCount = indexCount,
                vertexCount = vertexCount,
                firstVertex = 0, 
                bounds = bounds
            };

            mesh.bounds = bounds;
            
            mesh.SetSubMesh(0, subMeshDescriptor, MeshUpdateFlags.DontRecalculateBounds);
            
            // mesh.RecalculateBounds();
            
            mesh.UploadMeshData(true);
            
            MeshDataRepository.Instance.AddMesh(e.descriptor.id, mesh);
        }
    }
}
