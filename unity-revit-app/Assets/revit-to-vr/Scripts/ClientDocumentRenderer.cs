using System.Collections.Generic;
using revit_to_vr_common;
using UnityEngine;

namespace RevitToVR
{
    public class ClientDocumentRenderer : MonoBehaviour, IClientDocumentListener, IMeshRepository
    {
        // events

        public delegate void OnMeshRemoved(VRBIM_MeshId meshId);

        public delegate void OnMeshAdded(VRBIM_MeshId meshId, Mesh mesh);

        public event OnMeshAdded onMeshAdded;

        public event OnMeshRemoved onMeshRemoved;

        // meshes
        private Dictionary<VRBIM_MeshId, Mesh> _meshes = new Dictionary<VRBIM_MeshId, Mesh>();

        // elements
        private Dictionary<long, ElementRenderer> _elementRenderers = new Dictionary<long, ElementRenderer>();
        private IMeshRepository _meshRepositoryImplementation;

        // initialize
        
        // IMeshRepository implementation

        public void AddMesh(VRBIM_MeshId meshId, Mesh mesh)
        {
            // try to remove mesh if needed
            RemoveMesh(meshId);

            _meshes.Add(meshId, mesh);
            onMeshAdded?.Invoke(meshId, mesh);
        }

        public void RemoveMesh(VRBIM_MeshId meshId)
        {
            if (_meshes.ContainsKey(meshId))
            {
                _meshes.Remove(meshId);
                onMeshRemoved?.Invoke(meshId);
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