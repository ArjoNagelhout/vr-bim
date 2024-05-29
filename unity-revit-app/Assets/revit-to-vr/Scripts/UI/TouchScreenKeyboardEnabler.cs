using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace RevitToVR
{
    [RequireComponent(typeof(TMP_InputField))]
    public class TouchScreenKeyboardEnabler : MonoBehaviour
    {
        private TouchScreenKeyboard _keyboard;
        private TMP_InputField _inputField;

        private void Start()
        {
            _inputField = GetComponent<TMP_InputField>();
            Debug.Assert(_inputField != null);
            _inputField.onSelect.AddListener(EnableTouchScreenKeyboard);
        }

        private void OnDestroy()
        {
            _inputField.onSelect.RemoveListener(EnableTouchScreenKeyboard);
        }

        public void EnableTouchScreenKeyboard(string text)
        {
            // the provided string argument is probably the input field's text
            // but there is no documentation, as per usual, so we just grab the text
            // directly to be sure
            _keyboard = TouchScreenKeyboard.Open(_inputField.text);
        }
    }
}
