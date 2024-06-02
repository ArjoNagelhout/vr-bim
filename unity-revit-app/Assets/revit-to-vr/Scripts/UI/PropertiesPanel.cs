using System;
using System.Collections;
using System.Collections.Generic;
using revit_to_vr_common;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

        private long _cachedSelectedToposolidId = Configuration.invalidElementId;
        private ToposolidRenderer _cachedToposolidRenderer;

        // where we should add the edit mode UI. 
        [SerializeField] private Transform _contentTransform;

        private GameObject _instantiatedEditModeUI;
        private bool _editModeActive = false;

        [SerializeField] private Button modifySubElementsButton;

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

            _cachedToposolidRenderer = null;
            _cachedSelectedToposolidId = Configuration.invalidElementId;

            if (selectedElementIds.Count == 0)
            {
                state = State.None;
            }
            else if (selectedElementIds.Count > 1)
            {
                state = State.Multiple;
            }
            else if (selectedElementIds.Count == 1)
            {
                _cachedSelectedToposolidId = selectedElementIds[0];
                if (_cachedSelectedToposolidId != Configuration.invalidElementId &&
                    ClientDocumentRenderer.ElementRenderers.TryGetValue(_cachedSelectedToposolidId,
                        out ElementRenderer elementRenderer) &&
                    elementRenderer is ToposolidRenderer value)
                {
                    _cachedToposolidRenderer = value;
                    state = State.Toposolid;
                }
                else
                {
                    state = State.Invalid;
                }
            }
        }

        private void UpdateCachedToposolidRenderer()
        {
            if (_cachedToposolidRenderer == null)
            {
                // this can happen when the mesh has been updated, but the selection has not changed
                if (ClientDocumentRenderer.ElementRenderers.TryGetValue(_cachedSelectedToposolidId,
                        out ElementRenderer elementRenderer) && elementRenderer is ToposolidRenderer value)
                {
                    _cachedToposolidRenderer = value;
                }
                else
                {
                    Debug.Assert(false,
                        "selected Toposolid id doesn't exist in document, server should have sent a selection changed event");
                }
            }
        }
        
        private VRBIM_Toposolid GetSelectedToposolid()
        {
            Debug.Assert(_cachedSelectedToposolidId != Configuration.invalidElementId);
            UpdateCachedToposolidRenderer();
            return _cachedToposolidRenderer.toposolid;
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

            // set whether the modifySubElements button should be clickable
            if (state == State.Toposolid)
            {
                VRBIM_Toposolid toposolid = GetSelectedToposolid();
                modifySubElementsButton.interactable = !toposolid.IsSubdivision;
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
            UpdateCachedToposolidRenderer();
            Debug.Assert(_cachedToposolidRenderer != null);
            _cachedToposolidRenderer.EditSketch();
        }

        public void ToposolidModifySubElements()
        {
            UpdateCachedToposolidRenderer();
            Debug.Assert(_cachedToposolidRenderer != null);
            _cachedToposolidRenderer.ModifySubElements();
        }
    }
}