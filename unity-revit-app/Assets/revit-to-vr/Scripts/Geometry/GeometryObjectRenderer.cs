using revit_to_vr_common;
using UnityEngine;

namespace RevitToVR
{
    // this is to avoid having to create a full mapping system for materials 
    public enum GeometryObjectMaterial
    {
        Generic, // Generic - 1000mm
        Path, // Path - 350mm Asphalt
        Grassland, // Grassland - 1200mm
        Other // when the name is not matched
    }
    
    public class GeometryObjectRenderer : MonoBehaviour, ISelectHoveredStateChangedListener
    {
        protected ClientDocumentRenderer _documentRenderer;

        protected VRBIM_Geometry _geometry;

        protected GeometryObjectMaterial _material;

        public void Initialize(ClientDocumentRenderer documentRenderer, VRBIM_Geometry geometry, GeometryObjectMaterial material)
        {
            _documentRenderer = documentRenderer;
            _geometry = geometry;
            _material = material;
            OnInitialize();
        }

        protected virtual void OnInitialize()
        {
            
        }
        
        protected virtual void OnDestroy()
        {
            
        }

        public virtual void OnSelectHoveredStateChanged(bool hovered, bool selected)
        {
            
        }
    }
}