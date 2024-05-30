using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RevitToVR
{
    public class TestInputInteraction : MonoBehaviour
    {
        [SerializeField] private InputActionReference _inputActionReference;

        private void Start()
        {
            _inputActionReference.action.started += OnInputActionStarted; 
            _inputActionReference.action.performed += OnInputActionPerformed;
            _inputActionReference.action.canceled += OnInputActionCanceled;
            _inputActionReference.action.Enable();
        }

        private void OnDestroy()
        {
            _inputActionReference.action.started -= OnInputActionStarted; 
            _inputActionReference.action.performed -= OnInputActionPerformed;
            _inputActionReference.action.canceled -= OnInputActionCanceled;
        }

        private void OnInputActionStarted(InputAction.CallbackContext context)
        {
            Debug.Assert(context.phase == InputActionPhase.Started);
            UIConsole.Log("Started");
        }

        private void OnInputActionCanceled(InputAction.CallbackContext context)
        {
            Debug.Assert(context.phase == InputActionPhase.Canceled);
            UIConsole.Log("Canceled");
        }
        
        private void OnInputActionPerformed(InputAction.CallbackContext context)
        {
            Debug.Assert(context.phase == InputActionPhase.Performed);
            UIConsole.Log("Performed");
        }
    }
}
