using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace RevitToVR
{
    public class Constraint
    {
        public bool enableXMovement = true;
        public bool enableYMovement = true;
        public bool enableZMovement = true;
    }
    
    public class ElementVRInteractable : XRBaseInteractable
    {
        [SerializeField] private MeshRenderer _meshRenderer; 
        
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

        private void UpdateMaterial()
        {
            Debug.Assert(_meshRenderer != null);
            Material material = null;
            if (selected)
            {
                material = UnityAssetProvider.instance.defaultMaterials.selected;
            }
            else if (hovered)
            {
                material = UnityAssetProvider.instance.defaultMaterials.hovered;
            }
            else
            {
                material = UnityAssetProvider.instance.defaultMaterials.normal;
            }

            _meshRenderer.sharedMaterial = material;
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