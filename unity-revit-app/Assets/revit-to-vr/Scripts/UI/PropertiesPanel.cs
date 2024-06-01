using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace RevitToVR
{
    public class PropertiesPanel : MonoBehaviour
    {
        private static PropertiesPanel _instance;

        public static PropertiesPanel Instance => _instance;

        [NonSerialized] public ClientDocumentRenderer ClientDocumentRenderer;

        [SerializeField] private GameObject errorMessage;
        [SerializeField] private TextMeshProUGUI errorMessageText;
        
        [SerializeField] private List<GameObject> toposolidSelected;

        private List<ElementRenderer> _cachedSelectedElements = new List<ElementRenderer>();
        private ToposolidRenderer _cachedToposolidRenderer;

        // where we should add the edit mode UI. 
        [SerializeField] private Transform _contentTransform;

        private GameObject _instantiatedEditModeUI;
        private bool _editModeActive = false;

        // returns the instantiated prefab, so that its data can be set, if required
        public GameObject StartEditMode(GameObject uiPrefab)
        {
            Debug.Assert(_instantiatedEditModeUI == null);
            _instantiatedEditModeUI = Instantiate(uiPrefab, _contentTransform, false);
            _editModeActive = true;
            UpdateState();
            return _instantiatedEditModeUI;
        }

        public void StopEditMode()
        {
            Debug.Assert(_editModeActive);
            Debug.Assert(_instantiatedEditModeUI != null);
            Destroy(_instantiatedEditModeUI);
            _editModeActive = false;
            UpdateState();
        }

        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(_instance);
            }
            
            _instance = this;
        }

        private void Start()
        {
            state = State.None;
        }

        private enum State
        {
            None,
            Toposolid,
            Multiple,
            Invalid
        }

        private State _state = State.None;
        private State state
        {
            get => _state;
            set
            {
                _state = value;
                UpdateState();
            }
        }

        public void OnSelectionChanged(List<long> selectedElementIds)
        {
            Debug.Assert(ClientDocumentRenderer != null);
            
            _cachedSelectedElements.Clear();
            _cachedToposolidRenderer = null;

            foreach (long id in selectedElementIds)
            {
                if (ClientDocumentRenderer.ElementRenderers.TryGetValue(id, out ElementRenderer value))
                {
                    _cachedSelectedElements.Add(value);                    
                }
                // we skip the elements that we don't have as element renderers
            }

            if (selectedElementIds.Count == 0)
            {
                state = State.None;
            }
            else if (selectedElementIds.Count > 1)
            {
                state = State.Multiple;
            }
            else
            {
                if (_cachedSelectedElements.Count == 1 && _cachedSelectedElements[0] is ToposolidRenderer)
                {
                    _cachedToposolidRenderer = _cachedSelectedElements[0] as ToposolidRenderer;
                    state = State.Toposolid;
                }
                else
                {
                    state = State.Invalid;
                }
            }
        }

        private void UpdateState()
        {
            if (_editModeActive)
            {
                errorMessage.SetActive(false);
                foreach (GameObject obj in toposolidSelected)
                {
                    obj.SetActive(false);
                }
                return;
            }
            
            errorMessage.SetActive(state != State.Toposolid);
            foreach (GameObject obj in toposolidSelected)
            {
                obj.SetActive(state == State.Toposolid);   
            }

            switch (state)
            {
                case State.Invalid:
                    errorMessageText.text = "Selected Element does not support Live Editing Functionality in VR yet.";
                    break;
                case State.Multiple:
                    errorMessageText.text = "Editing Multiple Elements at the same time is not supported yet.";
                    break;
                case State.None:
                    errorMessageText.text = "No Elements selected, please select an Element in the Document";
                    break;
            }
        }

        public void ToposolidEditSketch()
        {
            Debug.Assert(_cachedToposolidRenderer != null);
            _cachedToposolidRenderer.EditSketch();
        }

        public void ToposolidModifySubElements()
        {
            Debug.Assert(_cachedToposolidRenderer != null);
            _cachedToposolidRenderer.ModifySubElements();
        }
    }
}
