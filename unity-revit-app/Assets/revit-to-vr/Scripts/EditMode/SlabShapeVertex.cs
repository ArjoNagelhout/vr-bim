using System;
using System.Collections;
using System.Collections.Generic;
using revit_to_vr_common;
using UnityEngine;
using UnityEngine.Serialization;

namespace RevitToVR
{
    public class SlabShapeVertex : MonoBehaviour, IMaterialStateInteractableListener
    {
        private static int invalidIndex = -1;
        
        [SerializeField] private MaterialStateInteractable interactable;
        
        private VRBIM_SlabShapeVertex _data;
        public VRBIM_Toposolid toposolid;

        public int index = invalidIndex;

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
            
            // set materials
            switch (_data.vertexType)
            {
                case VRBIM_SlabShapeVertexType.Corner:
                case VRBIM_SlabShapeVertexType.Edge:
                    interactable.Materials = UnityAssetProvider.instance.slabShapeVertexEdgeAndCornerMaterials;
                    break;
                case VRBIM_SlabShapeVertexType.Interior:
                    interactable.Materials = UnityAssetProvider.instance.slabShapeVertexInteriorMaterials;
                    break;
                case VRBIM_SlabShapeVertexType.Invalid:
                    interactable.Materials = UnityAssetProvider.instance.defaultMaterials;
                    break;
            }
        }

        private Vector3 _cachedPosition = Vector3.zero;
        private float _cachedDocumentScale = 1.0f;

        public float positionChangeEpsilon = 0.05f;

        private void UpdatePosition()
        {
            transform.localPosition = GetScaledCachedPosition();
        }

        private Vector3 GetScaledCachedPosition()
        {
            return _cachedPosition * _cachedDocumentScale;
        }

        private void Start()
        {
            _config = VRApplication.instance.localClientConfiguration;
            _config.onDocumentScaleChanged += OnDocumentScaleChanged;
            _config.onHandleScaleChanged += OnHandleScaleChanged;
            OnDocumentScaleChanged(_config.DocumentScale);
            OnHandleScaleChanged(_config.HandleScale);
            
            Debug.Assert(interactable != null);
            interactable.Listener = this;
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

        // IMaterialStateInteractableListener implementation
        
        void IMaterialStateInteractableListener.OnHoverEntered()
        {
            
        }

        void IMaterialStateInteractableListener.OnHoverExited()
        {
            
        }

        void IMaterialStateInteractableListener.OnSelectEntered()
        {
            
        }

        void IMaterialStateInteractableListener.OnSelectExited()
        {
            // calculate distance between the current position and the
            //Vector3 original = GetScaledCachedPosition();
            Vector3 current = transform.localPosition;

            //float distance = Vector3.Distance(original, current);
            
            // apply the change (send event to the server)
            // calculate offset
            float offset = current.y;// - original.y;
            float scaledOffset = offset / _cachedDocumentScale; // needs to be multiplied by 1000. 1000mm in each meter
            
            // subtract the level offset from the scaledoffset
            scaledOffset -= toposolid.heightOffsetFromLevel;

            if (scaledOffset < 0.0f)
            {
                scaledOffset = 0.0f;
            }
            
            Debug.Assert(index != invalidIndex);
            UIConsole.Log($"SlabShapeVertex > OnSelectExited, index: {index}, scaledOffset: {scaledOffset}");
            EditModeState.instance.UpdateEditMode(new UpdateModifySubElements()
            {
                entries = new List<UpdateModifySubElements.Entry>()
                {
                    new UpdateModifySubElements.Entry()
                    {
                        index = index,
                        offset = scaledOffset
                    }
                }
            });

            _cachedPosition.y = scaledOffset + toposolid.heightOffsetFromLevel;
            UpdatePosition();

            // if (distance > positionChangeEpsilon)
            // {
            //     
            // }
            // else
            // {
            //     // reset the position
            //     UpdatePosition();
            // }
        }
    }
}
