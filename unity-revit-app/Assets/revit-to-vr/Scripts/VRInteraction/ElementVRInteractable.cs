using System;
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
    
    public class ElementVRInteractable : TeleportationArea
    {
        public IElementVRInteractableListener listener;

        private void Start()
        {
            
        }

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
            if (!VRApplication.instance.IsTeleporting())
            {
                listener?.InteractableOnSelect();
            }
            base.OnSelectEntered(args);
        }

        protected override void OnSelectExited(SelectExitEventArgs args)
        {
            // we need to do something here to make sure the teleportation thing doesn't get the event
            // we need to check whether the teleportation was activated. How do we do that?
            // LocomotionSystem?

            // the locomotion 
            // this does not work, as the base.OnSelectExited() is the method that calls
            // QueueTeleportationRequest, which adds it to the LocomotionSystem on BeginLocomotion() in the update
            // we should just get the currently active key. 
            // it's inside the ActionBasedControllerManager

            if (!VRApplication.instance.IsTeleporting())
            {
                return;
            }
            
            base.OnSelectExited(args);
            // we don't care about this event for selecting elements
        }
    }
}