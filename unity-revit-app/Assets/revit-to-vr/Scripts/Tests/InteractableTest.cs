using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace RevitToVR
{
    [RequireComponent(typeof(MeshRenderer))]
    public class InteractableTest : XRBaseInteractable
    {
        private MeshRenderer _meshRenderer;

        private bool _cachedSelected = false;
        private bool cachedSelected
        {
            get => _cachedSelected;
            set
            {
                _cachedSelected = value;
                UpdateAppearance();
            }
        }
        
        private bool _cachedHovered = false;
        private bool cachedHovered
        {
            get => _cachedHovered;
            set
            {
                _cachedHovered = value;
                UpdateAppearance();
            }
        }

        private void UpdateAppearance()
        {
            if (cachedSelected)
            {
                _meshRenderer.material = UnityAssetProvider.instance.defaultMaterials.selected;
            }
            else if (cachedHovered)
            {
                _meshRenderer.material = UnityAssetProvider.instance.defaultMaterials.hovered;
            }
            else
            {
                _meshRenderer.material = UnityAssetProvider.instance.defaultMaterials.normal;
            }
        }
        
        private void Start()
        {
            _meshRenderer = GetComponent<MeshRenderer>();
        }

        protected override void OnHoverEntered(HoverEnterEventArgs args)
        {
            base.OnHoverEntered(args);
            cachedHovered = true;
        }

        protected override void OnHoverExited(HoverExitEventArgs args)
        {
            base.OnHoverExited(args);
            cachedHovered = false;
        }

        protected override void OnSelectEntered(SelectEnterEventArgs args)
        {
            base.OnSelectEntered(args);
            cachedSelected = true;
        }

        protected override void OnSelectExited(SelectExitEventArgs args)
        {
            base.OnSelectExited(args);
            cachedSelected = false;
        }
    }
}
