using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using WebSocketSharp;
using System.Text.Json;
using revit_to_vr_common;
using Unity.Collections;
using UnityEngine.Rendering;

namespace RevitToVR
{
    public interface IMeshRepository
    {
        void AddMesh(VRBIM_MeshId meshId, Mesh mesh);

        void RemoveMesh(VRBIM_MeshId meshId);
    }

    public class LocalClientConfiguration
    {
        private static string documentScaleKey = "DOCUMENT_SCALE";
        private static string handleScaleKey = "HANDLE_SCALE";
        
        private float _documentScale = PlayerPrefs.GetFloat(documentScaleKey);
        public float DocumentScale
        {
            get => _documentScale;
            set
            {
                _documentScale = value;
                PlayerPrefs.SetFloat(documentScaleKey, _documentScale);
                onDocumentScaleChanged?.Invoke(_documentScale);
            }
        }
        public event Action<float> onDocumentScaleChanged;

        private float _handleScale = PlayerPrefs.GetFloat(handleScaleKey);

        public float HandleScale
        {
            get => _handleScale;
            set
            {
                _handleScale = value;
                PlayerPrefs.SetFloat(handleScaleKey, _handleScale);
                onHandleScaleChanged?.Invoke(_handleScale);
            }
        }

        public event Action<float> onHandleScaleChanged;
    }
    
    public class VRApplication : MonoBehaviour
    {
        private static VRApplication _instance;
        public static VRApplication instance => _instance;

        // events for the UI
        public Action onOpen;
        public Action onClose;
        public Action onMessage;

        // configuration to use for the connection
        public ClientConfiguration clientConfiguration;
        
        private MainServiceClient _mainServiceClient;
        private ClientDocument _clientDocument;
        private ClientDocumentRenderer _clientDocumentRenderer;
        private IMeshRepository _meshRepository;
        private EditModeState _editModeState;

        // configuration to use locally (not communicated to the server)
        public LocalClientConfiguration localClientConfiguration = null;
        
        // the event we received, so that we can parse the binary data that is sent using a separate .Send() after the event
        private ServerEvent _cachedEvent;

        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(_instance);
            }

            _instance = this;
            localClientConfiguration = new LocalClientConfiguration();
        }
        
        private void Start()
        {
            Application.targetFrameRate = 30;
            
            CreateEditModeState();
            
            UIConsole.Log("Started VRApplication");
        }
        public void RequestConnect(string ipAddress)
        {
            UIConsole.Log("OnRequestConnect");
            
            _mainServiceClient = new MainServiceClient(ipAddress);
            _mainServiceClient.OnMessage += OnMessage;
            _mainServiceClient.OnOpen += OnOpen;
            _mainServiceClient.OnClose += OnClose;
            _mainServiceClient.Connect();
        }

        public void RequestDisconnect()
        {
            // todo
            Handle(new DocumentClosedEvent());
            Disconnect();
            OnClose();
            UIConsole.Clear();
        }

        private void CreateEditModeState()
        {
            _editModeState = new GameObject().AddComponent<EditModeState>();
            _editModeState.name = "EditModeState";
        }

        private void OnDestroy()
        {
            Disconnect();
        }

        private void Disconnect()
        {
            if (_mainServiceClient != null)
            {
                _mainServiceClient.OnMessage -= OnMessage;
                _mainServiceClient.OnOpen -= OnOpen;
                _mainServiceClient.OnClose -= OnClose;
                _mainServiceClient.Disconnect();
                _mainServiceClient = null;
            }
        }

        private void OnOpen()
        {
            // called when the connection to the server is opened by the MainServiceClient
            
            // send configuration
            SendConfigurationDataAndStartListening();
            
            onOpen?.Invoke();
        }

        private void OnClose()
        {
            // called when the connection to the server is closed by the MainServiceClient
            onClose?.Invoke();
        }

        // server to client communication
        private void OnMessage(object sender, MessageEventArgs args)
        {
            onMessage?.Invoke();
            
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

                //UIConsole.Log("VRApplication > OnMessage: Received json: " + json);

                JsonSerializerOptions options = Configuration.jsonSerializerOptions;
                revit_to_vr_common.ServerEvent e =
                    JsonSerializer.Deserialize<revit_to_vr_common.ServerEvent>(json, options);
                
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

        public void SendConfigurationDataAndStartListening()
        {
            _mainServiceClient.SendJson(new SendClientConfigurationEvent()
            {
                clientConfiguration = clientConfiguration
            });
            _mainServiceClient.SendJson(new StartListeningToEvents());
        }

        private void HandleEvent(ServerEvent @event)
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
                case StartedEditMode startedEditMode:
                    Handle(startedEditMode);
                    break;
                case StoppedEditMode stoppedEditMode:
                    Handle(stoppedEditMode);
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

            // ugly, but suffices for now, we don't want to open the document twice
            if (_clientDocument != null)
            {
                Handle(new DocumentClosedEvent());
            }
            
            // create renderer
            _clientDocumentRenderer = new GameObject().AddComponent<ClientDocumentRenderer>();
            _clientDocumentRenderer.name = $"ClientDocumentRenderer ({e.documentGuid})";
            
            // todo: move, this is hacky
            PropertiesPanel.Instance.ClientDocumentRenderer = _clientDocumentRenderer;
            
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
            Destroy(_clientDocumentRenderer.gameObject);
            
            // clear mesh repository
            _meshRepository = null;
            
            // destroy document
            _clientDocument.Dispose();
            _clientDocument = null;
        }

        private void Handle(SelectionChangedEvent e)
        {
            UIConsole.Log("Handle SelectionChangedEvent");
            _clientDocument.Apply(e);
            
            // move this somewhere else
            PropertiesPanel.Instance.OnSelectionChanged(e.selectedElementIds);
        }

        private void Handle(SendMeshDataEvent e)
        {
            UIConsole.Log("Handle SendMeshDataEvent");
            // don't need to do anything here, because we already cached the json event and use it
            // afterward in the HandleBinary function
        }
        
        // edit mode
        private void Handle(StartedEditMode e)
        {
            UIConsole.Log("Handle StartedEditMode event");
            _editModeState.Apply(e);
        }

        private void Handle(StoppedEditMode e)
        {
            UIConsole.Log("Handle StoppedEditMode event");
            _editModeState.Apply(e);
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

        private void HandleBinary(SendMeshDataEvent e, byte[] buffer)
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
            VRBIM_MeshId id = e.descriptor.id;
            mesh.name = id.IsTemporary ? $"temporary mesh: {id.temporaryId.ToString()}" : $"mesh: {id.id.ToString()}";
            mesh.SetSubMesh(0, subMeshDescriptor, MeshUpdateFlags.DontRecalculateBounds);

            // mesh.RecalculateBounds();

            mesh.UploadMeshData(false);
            Debug.Assert(mesh.isReadable);

            _meshRepository.AddMesh(e.descriptor.id, mesh);
        }
    }
}