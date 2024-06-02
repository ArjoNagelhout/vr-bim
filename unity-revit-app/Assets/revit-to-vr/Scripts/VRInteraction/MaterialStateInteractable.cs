using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace RevitToVR
{
    public interface IMaterialStateInteractableListener
    {
        public void OnHoverEntered();

        public void OnHoverExited();

        public void OnSelectEntered();

        public void OnSelectExited();

    }
    
    // automatically sets materials
    [RequireComponent(typeof(MeshRenderer))]
    public class MaterialStateInteractable : MonoBehaviour
    {
        private XRBaseInteractable _interactable;

        public IMaterialStateInteractableListener Listener;
        
        private StateMaterials _materials = new StateMaterials();
        public StateMaterials Materials
        {
            get => _materials;
            set
            {
                _materials = value;
                UpdateMaterial();
            }
        }

        private void UpdateMaterial()
        {
            if (selected)
            {
                meshRenderer.material = Materials.selected;
            }
            else if (hovered)
            {
                meshRenderer.material = Materials.hovered;
            }
            else
            {
                meshRenderer.material = Materials.normal;
            }
        }

        private void Start()
        {
            _interactable = GetComponent<XRBaseInteractable>();
            RegisterInteractableEvents();
            UpdateMaterial();
        }

        private void OnDestroy()
        {
            UnregisterInteractableEvents();
        }

        private void RegisterInteractableEvents()
        {
            _interactable.hoverEntered.AddListener(OnHoverEntered);
            _interactable.hoverExited.AddListener(OnHoverExited);
            _interactable.selectEntered.AddListener(OnSelectEntered);
            _interactable.selectExited.AddListener(OnSelectExited);
        }

        private void UnregisterInteractableEvents()
        {
            _interactable.hoverEntered.RemoveListener(OnHoverEntered);
            _interactable.hoverExited.RemoveListener(OnHoverExited);
            _interactable.selectEntered.RemoveListener(OnSelectEntered);
            _interactable.selectExited.RemoveListener(OnSelectExited);
        }

        private MeshRenderer _meshRenderer = null;
        private MeshRenderer meshRenderer
        {
            get
            {
                if (_meshRenderer == null)
                {
                    _meshRenderer = GetComponent<MeshRenderer>();
                    Debug.Assert(_meshRenderer != null);
                }
                return _meshRenderer;
            }
        }
        
        private bool _selected = false;
        private bool selected
        {
            get => _selected;
            set
            {
                _selected = value;
                UpdateMaterial();
            }
        }

        private bool _hovered = false;

        private bool hovered
        {
            get => _hovered;
            set
            {
                _hovered = value;
                UpdateMaterial();
            }
        }

        private void OnHoverEntered(HoverEnterEventArgs args)
        {
            hovered = true;
            Listener?.OnHoverEntered();
        }

        private void OnHoverExited(HoverExitEventArgs args)
        {
            hovered = false;
            Listener?.OnHoverExited();
        }

        private void OnSelectEntered(SelectEnterEventArgs args)
        {
            selected = true;
            Listener?.OnSelectEntered();
        }

        private void OnSelectExited(SelectExitEventArgs args)
        {
            selected = false;
            Listener?.OnSelectExited();
        }
    }
}
