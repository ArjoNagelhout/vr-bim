using System;
using System.Collections;
using System.Collections.Generic;
using revit_to_vr_common;
using UnityEngine;

namespace RevitToVR
{
    public class SlabShapeVertex : MonoBehaviour
    {
        private SimpleInteractable _interactable;
        
        private VRBIM_SlabShapeVertex _data;

        private LocalClientConfiguration _config;

        public VRBIM_SlabShapeVertex Data
        {
            get => _data;
            set
            {
                _data = value;
                OnDataChanged();
            }
        }

        private void OnDataChanged()
        {
            // update the transform and render data of this vertex
            _cachedPosition = DataConversion.ToUnityVector3(_data.position);
            UpdatePosition();
        }

        private Vector3 _cachedPosition = Vector3.zero;
        private float _cachedDocumentScale = 1.0f;

        private void UpdatePosition()
        {
            transform.localPosition = _cachedPosition * _cachedDocumentScale;
        }

        private void Start()
        {
            _config = VRApplication.instance.localClientConfiguration;
            _config.onDocumentScaleChanged += OnDocumentScaleChanged;
            _config.onHandleScaleChanged += OnHandleScaleChanged;
            OnDocumentScaleChanged(_config.DocumentScale);
            OnHandleScaleChanged(_config.HandleScale);

            _interactable = GetComponent<SimpleInteractable>();
            Debug.Assert(_interactable != null);
            _interactable.Materials = UnityAssetProvider.instance.slabShapeVertexMaterials;
        }

        private void OnDestroy()
        {
            _config.onDocumentScaleChanged -= OnDocumentScaleChanged;
            _config.onHandleScaleChanged -= OnHandleScaleChanged;
        }

        private void OnDocumentScaleChanged(float scale)
        {
            _cachedDocumentScale = scale;
            UpdatePosition();
        }

        private void OnHandleScaleChanged(float scale)
        {
            transform.localScale = new Vector3(scale, scale, scale);
        }
    }
}
