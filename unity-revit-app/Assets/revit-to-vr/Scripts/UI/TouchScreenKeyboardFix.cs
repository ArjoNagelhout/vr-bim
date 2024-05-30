using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.Serialization;

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

        private static GameObject _dummyObject;
        private static GameObject dummyObject
        {
            get
            {
                if (_dummyObject == null)
                {
                    _dummyObject = new GameObject("TouchScreenKeyboardFix_DummyObject");
                }
                
                return _dummyObject;
            }
        }

        private static event Action _stopAllListening;
        
        [FormerlySerializedAs("_triggerValue")] [SerializeField] private InputActionReference _trigger;

        private bool _enableKeyboardScheduled = false;
        private bool _listeningToTextChanges = false;

        private void Start()
        {
            _stopAllListening += StopListening; 
            
            _inputField = GetComponent<TMP_InputField>();
            Debug.Assert(_inputField != null);
            _inputField.onSelect.AddListener(ScheduleEnableTouchScreenKeyboard);
            _inputField.onDeselect.AddListener(OnDeselect);
            _trigger.action.Enable();
        }

        private void StopListening()
        {
            _listeningToTextChanges = false;
            _keyboard = null;
            UIConsole.Log("Stop Listening");
            
            DeselectAll();
        }

        private void OnDestroy()
        {
            _stopAllListening -= StopListening;
            _inputField.onSelect.RemoveListener(ScheduleEnableTouchScreenKeyboard);
            _inputField.onDeselect.RemoveListener(OnDeselect);
            Unschedule();
        }

        private void OnDeselect(string why)
        {
            UIConsole.Log("OnDeselect");
        }

        private void Unschedule()
        {
            if (_enableKeyboardScheduled)
            {
                _trigger.action.canceled -= OnTriggerCanceled;
                _enableKeyboardScheduled = false;
                UIConsole.Log("Unschedule");
            }
        }
        
        private void ScheduleEnableTouchScreenKeyboard(string text)
        {
            if (_listeningToTextChanges)
            {
                UIConsole.Log("Don't schedule");
                return;
            }
            
            _enableKeyboardScheduled = true;
            _trigger.action.canceled += OnTriggerCanceled;
            Debug.Assert(_trigger.action.enabled);
            UIConsole.Log("Scheduled");
        }

        private void Update()
        {
            ListenToKeyPresses();
        }

        private void OnTriggerCanceled(InputAction.CallbackContext context)
        {
            Debug.Assert(context.phase == InputActionPhase.Canceled);
            Debug.Assert(_enableKeyboardScheduled);
            
            // the provided string argument from the TMP_InputField.onSelect is probably the input field's text
            // but there is no documentation, as per usual, so we just grab the text
            // directly to be sure
            if (_keyboard != null)
            {
                _keyboard.active = false;
                _keyboard = null;
            }
            _stopAllListening?.Invoke();
            
            DeselectAll();
            
            _listeningToTextChanges = true;
            Unschedule();
            
            _keyboard = TouchScreenKeyboard.Open(_inputField.text, TouchScreenKeyboardType.Default);
            UIConsole.Log("Opened keyboard");
            DeselectAll();
        }

        private void DeselectAll()
        {
            EventSystem.current.SetSelectedGameObject(dummyObject); // we attempt to use dummy object because setting it to null doesn't work for some reason. Maybe not needed anymore
            _inputField.OnDeselect(new BaseEventData(EventSystem.current)); // needed because Unity and TextMeshPro are stupid
            UIConsole.Log("DeselectAll");
        }

        private void ListenToKeyPresses()
        {
            if (!_listeningToTextChanges)
            {
                return;
            }
            
            if (_keyboard == null)
            {
                StopListening();
                return;
            }

            if (!_keyboard.active)
            {
                StopListening();
                return;
            }

            _inputField.text = _keyboard.text;
        }
    }
}
