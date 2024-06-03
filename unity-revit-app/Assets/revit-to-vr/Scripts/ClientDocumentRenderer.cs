using System;
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
        [NonSerialized]
        public Dictionary<long, ElementRenderer> ElementRenderers = new Dictionary<long, ElementRenderer>();

        private IMeshRepository _meshRepositoryImplementation;

        // register listeners for mesh data events
        public void RegisterMeshDataEventListener(VRBIM_MeshId meshId, IMeshDataEventListener listener)
        {
            Debug.Assert(!_meshDataEventListeners.ContainsKey(meshId));
            _meshDataEventListeners.Add(meshId, listener);

            // if the mesh already exists, directly call the listener
            if (_meshes.TryGetValue(meshId, out Mesh mesh))
            {
                listener.OnMeshAdded(mesh);
            }
        }

        public void UnregisterMeshDataEventListener(VRBIM_MeshId meshId, IMeshDataEventListener listener)
        {
            Debug.Assert(_meshDataEventListeners.ContainsKey(meshId));
            Debug.Assert(_meshDataEventListeners[meshId] == listener);
            _meshDataEventListeners.Remove(meshId);

            if (_meshes.ContainsKey(meshId))
            {
                _meshes.Remove(meshId);
            }
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

        // register listening to the document scale changes
        private void Start()
        {
            LocalClientConfiguration config = VRApplication.instance.localClientConfiguration;
            config.onDocumentScaleChanged += OnDocumentScaleChanged;
            OnDocumentScaleChanged(config.DocumentScale, config.DocumentLogScale);
        }

        private void OnDestroy()
        {
            VRApplication.instance.localClientConfiguration.onDocumentScaleChanged -= OnDocumentScaleChanged;
        }

        private void OnDocumentScaleChanged(float scale, float logScale)
        {
            transform.localScale = new Vector3(logScale, logScale, logScale);
        }

        // IClientDocumentListener implementation

        void IClientDocumentListener.OnOpen()
        {
            // elements do not need to be created here, that should be handled using ElementAdded
        }

        void IClientDocumentListener.ElementAdded(long elementId, VRBIM_Element element)
        {
            switch (element)
            {
                case VRBIM_Toposolid:
                    AddElementRenderer<ToposolidRenderer>(elementId, element);
                    break;
                default:
                    AddElementRenderer<ElementRenderer>(elementId, element);
                    break;
            }
        }

        private void AddElementRenderer<T>(long elementId, VRBIM_Element element) where T : ElementRenderer
        {
            Debug.Assert(!ElementRenderers.ContainsKey(elementId));
            // instantiate element renderer
            GameObject elementRendererObject =
                Instantiate(UnityAssetProvider.instance.elementRendererPrefab, transform, false);
            //elementRendererObject.transform.SetParent(transform, false);
            T elementRenderer = elementRendererObject.AddComponent<T>();
            elementRenderer.Initialize(this, element);
            ElementRenderers.Add(elementId, elementRenderer);
        }

        void IClientDocumentListener.ElementRemoved(long elementId)
        {
            Debug.Assert(ElementRenderers.ContainsKey(elementId));
            Destroy(ElementRenderers[elementId].gameObject);
            ElementRenderers.Remove(elementId);
        }

        void IClientDocumentListener.OnClose()
        {
            // destroy all created renderers
            foreach (KeyValuePair<long, ElementRenderer> entry in ElementRenderers)
            {
                Destroy(entry.Value.gameObject);
            }

            ElementRenderers.Clear();
        }

        void IClientDocumentListener.ElementSelected(long elementId)
        {
            if (ElementRenderers.TryGetValue(elementId, out ElementRenderer elementRenderer))
            {
                (elementRenderer as IElementSelectionChangedListener).OnSelect();
            }
        }

        void IClientDocumentListener.ElementDeselected(long elementId)
        {
            if (ElementRenderers.TryGetValue(elementId, out ElementRenderer elementRenderer))
            {
                (elementRenderer as IElementSelectionChangedListener).OnDeselect();
            }
        }
    }
}