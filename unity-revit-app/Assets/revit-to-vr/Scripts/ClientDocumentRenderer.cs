using System.Collections.Generic;
using revit_to_vr_common;
using UnityEngine;

namespace RevitToVR
{
    public interface IMeshDataEventListener
    {
        public void OnMeshAdded(Mesh mesh);

        public void OnMeshRemoved();
    }
    
    public class ClientDocumentRenderer : MonoBehaviour, IClientDocumentListener, IMeshRepository
    {
        // meshes
        private Dictionary<VRBIM_MeshId, Mesh> _meshes = new Dictionary<VRBIM_MeshId, Mesh>();
        private Dictionary<VRBIM_MeshId, IMeshDataEventListener> _meshDataEventListeners =
            new Dictionary<VRBIM_MeshId, IMeshDataEventListener>();

        // elements
        private Dictionary<long, ElementRenderer> _elementRenderers = new Dictionary<long, ElementRenderer>();
        private IMeshRepository _meshRepositoryImplementation;
        
        // register listeners for mesh data events
        public void RegisterMeshDataEventListener(VRBIM_MeshId meshId, IMeshDataEventListener listener)
        {
            Debug.Assert(!_meshDataEventListeners.ContainsKey(meshId));
            _meshDataEventListeners.Add(meshId, listener);
        }

        public void UnregisterMeshDataEventListener(VRBIM_MeshId meshId, IMeshDataEventListener listener)
        {
            Debug.Assert(_meshDataEventListeners.ContainsKey(meshId));
            Debug.Assert(_meshDataEventListeners[meshId] == listener);
            _meshDataEventListeners.Remove(meshId);
        }
        
        // IMeshRepository implementation

        public void AddMesh(VRBIM_MeshId meshId, Mesh mesh)
        {
            // try to remove mesh if needed
            RemoveMesh(meshId);

            _meshes.Add(meshId, mesh);
            
            // make sure that anyone is listening for this mesh id
            if (_meshDataEventListeners.TryGetValue(meshId, out IMeshDataEventListener listener))
            {
                listener.OnMeshAdded(mesh);                
            }
        }

        public void RemoveMesh(VRBIM_MeshId meshId)
        {
            if (_meshes.ContainsKey(meshId))
            {
                _meshes.Remove(meshId);

                if (_meshDataEventListeners.TryGetValue(meshId, out IMeshDataEventListener listener))
                {
                    listener.OnMeshRemoved();
                }
            }
        }

        // IClientDocumentListener implementation

        void IClientDocumentListener.OnOpen()
        {
            // elements do not need to be created here, that should be handled using ElementAdded
        }

        void IClientDocumentListener.ElementAdded(long elementId, VRBIM_Element element)
        {
            Debug.Assert(!_elementRenderers.ContainsKey(elementId));
            // instantiate element renderer
            GameObject elementRendererObject = new GameObject();
            elementRendererObject.transform.SetParent(transform);
            ElementRenderer elementRenderer = elementRendererObject.AddComponent<ElementRenderer>();
            elementRenderer.Initialize(this, element);
            _elementRenderers.Add(elementId, elementRenderer);
        }

        void IClientDocumentListener.ElementRemoved(long elementId)
        {
            Debug.Assert(_elementRenderers.ContainsKey(elementId));
            Destroy(_elementRenderers[elementId].gameObject);
            _elementRenderers.Remove(elementId);
        }

        void IClientDocumentListener.OnClose()
        {
            // destroy all created renderers
            foreach (KeyValuePair<long, ElementRenderer> entry in _elementRenderers)
            {
                Destroy(entry.Value.gameObject);
            }
        }
    }
}