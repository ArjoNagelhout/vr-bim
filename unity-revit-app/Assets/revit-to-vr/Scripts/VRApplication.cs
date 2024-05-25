using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using WebSocketSharp;
using System.Text.Json;
using revit_to_vr_common;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Rendering;

namespace RevitToVR
{
    public interface IMeshRepository
    {
        void AddMesh(VRBIM_MeshId meshId, Mesh mesh);

        void RemoveMesh(VRBIM_MeshId meshId);
    }
    
    public class VRApplication : MonoBehaviour
    {
        public string ipAddress;
        
        private MainServiceClient _mainServiceClient;
        private ClientDocument _clientDocument;
        private ClientDocumentRenderer _clientDocumentRenderer;
        private IMeshRepository _meshRepository;

        // the event we received, so that we can parse the binary data that is sent using a separate .Send() after the event
        private revit_to_vr_common.Event _cachedEvent;

        private void Start()
        {
            UIConsole.Log("Started VRApplication");
            _mainServiceClient = new MainServiceClient(ipAddress);
            _mainServiceClient.OnMessage += OnMessage;
        }

        private void OnDestroy()
        {
            _mainServiceClient.OnMessage -= OnMessage;
            _mainServiceClient.Disconnect();
            _mainServiceClient = null;
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

                revit_to_vr_common.Event e =
                    JsonSerializer.Deserialize<revit_to_vr_common.Event>(json, Configuration.jsonSerializerOptions);
                HandleEvent(e);
                _cachedEvent = e;
            }
            else if (args.IsBinary)
            {
                UIConsole.Log("VRApplication > OnMessage: Received binary with length: " + args.RawData.Length);
                // handle binary
                if (_cachedEvent == null)
                {
                    return;
                }

                UIConsole.Log("HandleBinary");
                HandleBinary(args.RawData);
                _cachedEvent = null;
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
            
            Debug.Assert(_clientDocument != null);
            _clientDocument.Apply(e);
        }

        // document opened information gets cached server side and sent on connection open
        // after this, a document changed event gets sent with all elements, this is not part of the DocumentOpenedEvent
        private void Handle(DocumentOpenedEvent e)
        {
            UIConsole.Log("Handle DocumentOpenedEvent");
            
            // create renderer
            _clientDocumentRenderer = new GameObject().AddComponent<ClientDocumentRenderer>();
            _clientDocumentRenderer.name = $"ClientDocumentRenderer ({e.documentGuid})";
            
            // set mesh repository
            _meshRepository = _clientDocumentRenderer;
            
            // create document
            _clientDocument = new ClientDocument();
            _clientDocument.Listener = _clientDocumentRenderer;
            
            // apply data
            _clientDocument.Apply(e);
        }

        private void Handle(DocumentClosedEvent e)
        {
            UIConsole.Log("Handle DocumentClosedEvent");
            
            _clientDocument.Apply(e);
            
            // destroy renderer
            Destroy(_clientDocumentRenderer);
            
            // clear mesh repository
            _meshRepository = null;
            
            // destroy document
            _clientDocument.Dispose();
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
            switch (_cachedEvent)
            {
                case SendMeshDataEvent sendMeshDataEvent:
                    HandleBinary(sendMeshDataEvent, buffer);
                    break;
            }
        }

        [System.Runtime.InteropServices.StructLayout(LayoutKind.Sequential, Size = 3 * 4 * 2)]
        public struct VRBIM_ReceivedVertexData
        {
            public VRBIM_Vector3 position;
            public VRBIM_Vector3 normal;
        }

        private unsafe void HandleBinary(SendMeshDataEvent e, byte[] buffer)
        {
            var descriptors = new NativeArray<VertexAttributeDescriptor>(2, Allocator.Temp);
            descriptors[0] = new VertexAttributeDescriptor()
            {
                attribute = VertexAttribute.Position,
                format = VertexAttributeFormat.Float32,
                dimension = 3
            };
            descriptors[1] = new VertexAttributeDescriptor()
            {
                attribute = VertexAttribute.Normal,
                format = VertexAttributeFormat.Float32,
                dimension = 3
            };

            int vertexCount = e.descriptor.vertexCount;
            int vector3SizeInBytes = 3 * 4; // 3 components * 4 bytes per float
            int vertexStrideInBytes = vector3SizeInBytes * 2; // 2 attributes (position and float)

            Mesh mesh = new Mesh();
            mesh.SetVertexBufferParams(vertexCount, descriptors);

            VRBIM_ReceivedVertexData[] verticesTyped = new VRBIM_ReceivedVertexData[vertexCount];

            Bounds bounds = new Bounds();
            for (int i = 0; i < vertexCount; i++)
            {
                VRBIM_Vector3 position = DataConversion.GetVector3FromBytes(buffer, i * vertexStrideInBytes);
                VRBIM_Vector3 normal = DataConversion.GetVector3FromBytes(buffer, i * vertexStrideInBytes + vector3SizeInBytes);
                
                // UIConsole.Log($"vertex at index: {i}: {pos.ToString()} (bytes: {BitConverter.ToString(buffer, i * vertexStrideInBytes, 4 * 3)})");

                bounds.Encapsulate(DataConversion.ToUnityVector3(position));
                verticesTyped[i] = new VRBIM_ReceivedVertexData()
                {
                    position = position,
                    normal = normal
                };
            }

            // count = amount of *vertices* to copy, not bytes
            mesh.SetVertexBufferData(verticesTyped, 0, 0, vertexCount, 0, MeshUpdateFlags.DontRecalculateBounds);

            // we need to set the indices
            int indexCount = e.descriptor.indexCount;
            UIConsole.Log($"indexCount: {indexCount}");
            mesh.SetIndexBufferParams(indexCount, IndexFormat.UInt32);

            int indexSizeInBytes = 4;
            byte[] indices = new byte[indexCount * indexSizeInBytes];

            int offset = vertexCount * vertexStrideInBytes;
            Buffer.BlockCopy(buffer, offset, indices, 0, indexCount * indexSizeInBytes);
            
            UInt32[] indicesTyped = new UInt32[indexCount];

            for (int i = 0; i < indexCount; i++)
            {
                UInt32 index = BitConverter.ToUInt32(indices, i * indexSizeInBytes);
                
                //UIConsole.Log($"index: {index}");
                indicesTyped[i] = index;
            }

            mesh.SetIndexBufferData(indicesTyped, 0, 0, indexCount, MeshUpdateFlags.DontRecalculateBounds);

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

            _meshRepository.AddMesh(e.descriptor.id, mesh);
        }
    }
}