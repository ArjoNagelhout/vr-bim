using System;
using System.Collections.Generic;
using revit_to_vr_common;
using UnityEngine;

namespace RevitToVR
{
    // we simply destroy and re-add elements when they're changed, so we don't need
    // to handle changes. This obviously isn't the correct way to handle this, but
    // for demonstration purposes it suffices.  
    public class ElementRenderer : MonoBehaviour
    {
        private ClientDocumentRenderer _documentRenderer;
        
        private VRBIM_Element _element;

        private List<GeometryObjectRenderer> _geometryObjectRenderers = new List<GeometryObjectRenderer>();

        // can't use constructors
        public void Initialize(ClientDocumentRenderer documentRenderer, VRBIM_Element element)
        {
            _documentRenderer = documentRenderer;

            name = $"{element.name} ({element.elementId})";

            _element = element;
            
            // create GeometryObjectRenderer for each geometry object
            if (_element.geometries != null)
            {
                foreach (VRBIM_Geometry geometry in _element.geometries)
                {
                    switch (geometry)
                    {
                        case VRBIM_Solid:
                            AddGeometry<SolidRenderer>(geometry);
                            break;
                    }
                }                
            }
        }

        private void AddGeometry<T>(VRBIM_Geometry geometry) where T : GeometryObjectRenderer
        {
            GameObject o = new GameObject();
            o.name = $"Geometry: {typeof(T).Name}";
            o.transform.SetParent(transform, false);
            SolidRenderer r = o.AddComponent<SolidRenderer>();
            r.Initialize(_documentRenderer, geometry);
            _geometryObjectRenderers.Add(r);
        }

        private void OnDestroy()
        {
            foreach (GeometryObjectRenderer geometryObjectRenderer in _geometryObjectRenderers)
            {
                Destroy(geometryObjectRenderer.gameObject);
            }
        }
    }
}