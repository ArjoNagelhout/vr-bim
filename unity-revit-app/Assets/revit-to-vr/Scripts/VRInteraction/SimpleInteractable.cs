using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace RevitToVR
{
    public interface ISimpleInteractableListener
    {
        // on move events (if we want to enable moving the interactable)
    }
    
    // automatically sets materials
    [RequireComponent(typeof(MeshRenderer))]
    public class SimpleInteractable : XRBaseInteractable
    {
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
            UpdateMaterial();
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

        protected override void OnHoverEntered(HoverEnterEventArgs args)
        {
            base.OnHoverEntered(args);
            hovered = true;
        }

        protected override void OnHoverExited(HoverExitEventArgs args)
        {
            base.OnHoverExited(args);
            hovered = false;
        }

        protected override void OnSelectEntered(SelectEnterEventArgs args)
        {
            base.OnSelectEntered(args);
            selected = true;
        }

        protected override void OnSelectExited(SelectExitEventArgs args)
        {
            base.OnSelectExited(args);
            selected = false;
        }
    }
}
