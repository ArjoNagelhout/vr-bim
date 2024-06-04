using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RevitToVR
{
    // only needs a reference to the ClientDocumentRenderer,
    // as we want to set its scale. 
    public class DocumentPanel : MonoBehaviour
    {
        private LocalClientConfiguration _config;

        [SerializeField] private TextMeshProUGUI documentScaleText;

        [SerializeField] private Slider slider;

        private void Start()
        {
            Register();
        }
        
        private void OnEnable()
        {
            Register();
        }

        private void Register()
        {
            if (_config == null)
            {
                _config = VRApplication.instance.localClientConfiguration;
            }
            
            _config.onDocumentScaleChanged += OnDocumentScaleChanged;
            OnDocumentScaleChanged(_config.DocumentScale, _config.DocumentLogScale);
            slider.value = _config.DocumentScale;
        }

        private void OnDisable()
        {
            _config.onDocumentScaleChanged -= OnDocumentScaleChanged;
        }

        // slider on scale changed
        public void OnDocumentScaleValueChanged(float value_)
        {
            _config.DocumentScale = value_;
        }

        // event called by LocalClientConfig
        private void OnDocumentScaleChanged(float scale, float logScale)
        {
            documentScaleText.text =
                $"Document scale: {scale.ToString("0.0000")}, log scale: {logScale.ToString(("0.0000"))}";
        }
    }
}