using System;
using System.Collections.Generic;
using revit_to_vr_common;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace RevitToVR
{
    public interface ISelectHoveredStateChangedListener
    {
        public void OnSelectHoveredStateChanged(bool hovered, bool selected);
    }
    
    // we simply destroy and re-add elements when they're changed, so we don't need
    // to handle changes. This obviously isn't the correct way to handle this, but
    // for demonstration purposes it suffices.  
    public class ElementRenderer : MonoBehaviour, IElementSelectionChangedListener, IElementVRInteractableListener
    {
        protected ClientDocumentRenderer _documentRenderer;
        
        protected VRBIM_Element _element;

        private List<GeometryObjectRenderer> _geometryObjectRenderers = new List<GeometryObjectRenderer>();

        private ElementVRInteractable _interactable;

        // not ground truth, but used for updating the mesh renderers in the GeometryObjects
        private bool _cachedSelected = false;

        private bool cachedSelected
        {
            get => _cachedSelected;
            set
            {
                _cachedSelected = value;
                OnSelectHoveredStateChanged();
            }
        }

        private bool _cachedHovered = false;

        private bool cachedHovered
        {
            get => _cachedHovered;
            set
            {
                _cachedHovered = value;
                OnSelectHoveredStateChanged();
            }
        }

        private void OnSelectHoveredStateChanged()
        {
            foreach (GeometryObjectRenderer obj in _geometryObjectRenderers)
            {
                obj.OnSelectHoveredStateChanged(cachedHovered, cachedSelected);
            }
        }

        // can't use constructors
        public void Initialize(ClientDocumentRenderer documentRenderer, VRBIM_Element element)
        {
            _documentRenderer = documentRenderer;
            _element = element;
            
            name = $"{_element.name} ({_element.elementId})";
            
            // add interactable, register events
            
            // _interactable.colliders.Clear();
            
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
                        case VRBIM_Curve:
                            AddGeometry<CurveRenderer>(geometry);
                            break;
                    }
                }                
            }
            
            _interactable = gameObject.AddComponent<ElementVRInteractable>();
            _interactable.listener = this;
            
            OnInitialize();
        }

        protected virtual void OnInitialize()
        {
            
        }

        private void AddGeometry<T>(VRBIM_Geometry geometry) where T : GeometryObjectRenderer
        {
            GameObject o = new GameObject();
            o.name = $"Geometry: {typeof(T).Name}";
            o.transform.SetParent(transform, false);
            T r = o.AddComponent<T>();
            r.Initialize(_documentRenderer, geometry);
            _geometryObjectRenderers.Add(r);
            //_interactable.colliders.Add(r.GetComponent<MeshCollider>());
        }

        protected virtual void OnDestroy()
        {
            // deregister interactable
            _interactable.listener = null;
            Destroy(_interactable);
            
            foreach (GeometryObjectRenderer geometryObjectRenderer in _geometryObjectRenderers)
            {
                Destroy(geometryObjectRenderer.gameObject);
            }
        }

        public void InteractableOnHoverEnter()
        {
            cachedHovered = true;
        }

        public void InteractableOnHoverExit()
        {
            cachedHovered = false;
        }

        // client to server
        public void InteractableOnSelect()
        {
            UIConsole.Log("InteractableOnSelect called");
            // todo: add the other SelectElementType types when holding a shift or control modifier key.  
            MainServiceClient.instance.SendJson(new SelectElementClientEvent()
            {
                selectedElementIds = new List<long>()
                {
                    _element.elementId
                },
                type = SelectElementType.New
            });
        }
        
        // these events are received from the server:
        public virtual void OnSelect()
        {
            cachedSelected = true;
        }

        public virtual void OnDeselect()
        {
            cachedSelected = false;
        }
    }
}