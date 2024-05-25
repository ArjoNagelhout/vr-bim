using revit_to_vr_common;
using UnityEngine;

namespace RevitToVR
{
    public class GeometryObjectRenderer : MonoBehaviour
    {
        protected ClientDocumentRenderer _documentRenderer;

        protected VRBIM_Geometry _geometry;

        public void Initialize(ClientDocumentRenderer documentRenderer, VRBIM_Geometry geometry)
        {
            _documentRenderer = documentRenderer;
            _geometry = geometry;
            OnInitialize();
        }

        protected virtual void OnInitialize()
        {
            
        }
        
        protected virtual void OnDestroy()
        {
            
        }
    }
}