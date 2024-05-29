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

    public interface IElementVRInteractableListener
    {
        public void InteractableOnHoverEnter();
        public void InteractableOnHoverExit();
        public void InteractableOnSelect();
    }
    
    public class ElementVRInteractable : XRBaseInteractable
    {
        public IElementVRInteractableListener listener;
        
        protected override void OnHoverEntered(HoverEnterEventArgs args)
        {
            base.OnHoverEntered(args);
            listener?.InteractableOnHoverEnter();
        }

        protected override void OnHoverExited(HoverExitEventArgs args)
        {
            base.OnHoverExited(args);
            listener?.InteractableOnHoverExit();
        }

        protected override void OnSelectEntered(SelectEnterEventArgs args)
        {
            base.OnSelectEntered(args);
            listener?.InteractableOnSelect();
        }

        protected override void OnSelectExited(SelectExitEventArgs args)
        {
            base.OnSelectExited(args);
            // we don't care about this event for selecting elements
        }
    }
}