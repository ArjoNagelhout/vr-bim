using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace RevitToVR
{
    // The TMP_InputField is interacted with using an XR Ray Interactor
    // Unity had the brilliant idea to make this select the input field on
    // trigger *down*. This is incredibly stupid, as Meta on their turn had the brilliant
    // idea to dismiss the keyboard whenever the trigger is down. Not just when you press it down the first time,
    // but whether it is down at all.
    //
    // This immediately returns focus to our VR application, which receives a trigger down event and
    // so the cycle repeats. 
    // This results in indefinite rapid opening and closing of the keyboard. 
    //
    // The poor man's remedy is to open the keyboard while having the thumbstick moved into a direction,
    // as for some godawful reason, that disallows the trigger to dismiss the keyboard. 
    // 
    // However, because we want the user to not have to deal with this level of bullshit,
    // we'll write the dumbest fix ever, that simply waits until the trigger is *up*, and only
    // then opens the keyboard. 
    [RequireComponent(typeof(TMP_InputField))]
    public class TouchScreenKeyboardFix : MonoBehaviour
    {
        private TouchScreenKeyboard _keyboard;
        private TMP_InputField _inputField;
        [SerializeField] private InputActionReference _triggerValue;

        private bool _enableKeyboardScheduled = false;

        private void Start()
        {
            _inputField = GetComponent<TMP_InputField>();
            Debug.Assert(_inputField != null);
            _inputField.onSelect.AddListener(ScheduleEnableTouchScreenKeyboard);
            _triggerValue.action.Enable();
            
            UIConsole.Log("Started TouchScreenKeyboardFix");
        }

        private void OnDestroy()
        {
            _inputField.onSelect.RemoveListener(ScheduleEnableTouchScreenKeyboard);
            Unschedule();
        }

        private void Unschedule()
        {
            if (_enableKeyboardScheduled)
            {
                _triggerValue.action.performed -= OnTriggerValuePerformed;
                _triggerValue.action.canceled -= OnTriggerValueCanceled;
                _enableKeyboardScheduled = false;
            }
        }
        
        private void ScheduleEnableTouchScreenKeyboard(string text)
        {
            _enableKeyboardScheduled = true;
            _triggerValue.action.performed += OnTriggerValuePerformed;
            _triggerValue.action.canceled += OnTriggerValueCanceled;
            Debug.Assert(_triggerValue.action.enabled);
            UIConsole.Log("Scheduled keyboard fix");
        }

        private void OnTriggerValueCanceled(InputAction.CallbackContext context)
        {
            Debug.Assert(context.phase == InputActionPhase.Canceled);
            UIConsole.Log("OnTriggerValueCanceled");
            
            if (!_enableKeyboardScheduled)
            {
                return;
            }
            
            // the provided string argument from the TMP_InputField.onSelect is probably the input field's text
            // but there is no documentation, as per usual, so we just grab the text
            // directly to be sure
            if (_keyboard != null)
            {
                _keyboard.active = false;
                _keyboard = null;
            }
            _keyboard = TouchScreenKeyboard.Open(_inputField.text, TouchScreenKeyboardType.Default);
            UIConsole.Log("Opened keyboard");
            Unschedule();
        }

        private void OnTriggerValuePerformed(InputAction.CallbackContext context)
        {
            Debug.Assert(context.phase == InputActionPhase.Performed);
            UIConsole.Log("OnTriggerValuePerformed");

            float axisValue = context.ReadValue<float>();
            UIConsole.Log($"axis value: {axisValue.ToString()}");

            if (axisValue != 0.0f)
            {
                return;
            }
            
            
        }
    }
}
