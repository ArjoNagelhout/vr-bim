using revit_to_vr_common;
using UnityEngine;

namespace RevitToVR
{
    public class GeometryObjectRenderer : MonoBehaviour
    {
        private ClientDocumentRenderer.OnMeshAdded _onMeshAdded;
        private ClientDocumentRenderer.OnMeshRemoved _onMeshRemoved;

        private ClientDocumentRenderer _documentRenderer;

        protected VRBIM_Geometry _geometry;
        
        private void Awake()
        {
            _onMeshAdded = OnMeshAdded;
            _onMeshRemoved = OnMeshRemoved;
        }

        public void Initialize(ClientDocumentRenderer documentRenderer, VRBIM_Geometry geometry)
        {
            _documentRenderer = documentRenderer;
            _documentRenderer.onMeshAdded += _onMeshAdded;
            _documentRenderer.onMeshRemoved += _onMeshRemoved;

            _geometry = geometry;
        }
        
        private void OnDestroy()
        {
            _documentRenderer.onMeshAdded -= _onMeshAdded;
            _documentRenderer.onMeshRemoved -= _onMeshRemoved;
        }
        
        protected virtual void OnMeshAdded(VRBIM_MeshId meshId, Mesh mesh)
        {
            // check if this geometry object uses this mesh id
        }

        protected virtual void OnMeshRemoved(VRBIM_MeshId meshId)
        {
            // remove reference to mesh (otherwise we'd keep garbage around)
        }
    }
}