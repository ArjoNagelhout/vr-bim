using System;
using System.Collections.Generic;
using revit_to_vr_common;
using UnityEngine;

namespace RevitToVR
{
    public class ToposolidModifySubElementsRenderer : EditModeRenderer, IToposolidModifySubElementsPanelListener
    {
        private ToposolidModifySubElementsEditModeData modifySubElementsData =>
            _editModeData as ToposolidModifySubElementsEditModeData;

        private GameObject _instantiatedUIPrefab;
        private ToposolidModifySubElementsPanel _panel;

        private List<SlabShapeCrease> _instantiatedSlabShapeCreases = new List<SlabShapeCrease>();
        private List<SlabShapeVertex> _instantiatedSlabShapeVertices = new List<SlabShapeVertex>();

        protected override void OnInitialize()
        {
            base.OnInitialize();
        }

        public override void OnStartedEditMode()
        {
            base.OnStartedEditMode();
            _instantiatedUIPrefab = PropertiesPanel.Instance.StartEditMode(
                UnityAssetProvider.instance.toposolidModifySubElementsUIPrefab);
            _panel = _instantiatedUIPrefab.GetComponent<ToposolidModifySubElementsPanel>();
            Debug.Assert(_panel != null);
            _panel.Listener = this;

            InstantiateHandles();
        }

        private void InstantiateHandles()
        {
            // initialize sub elements
            // get the element data, as we have already sent the crease and vertex data. (this could also have been done
            // as the edit mode data
            VRBIM_SlabShapeData slabShapeData = modifySubElementsData.slabShapeData;

            foreach (VRBIM_SlabShapeCrease crease in slabShapeData.creases)
            {
                GameObject obj = Instantiate(UnityAssetProvider.instance.slabShapeCreasePrefab);
                SlabShapeCrease creaseComponent = obj.GetComponent<SlabShapeCrease>();
                Debug.Assert(creaseComponent != null);
                creaseComponent.Data = crease;
                _instantiatedSlabShapeCreases.Add(creaseComponent);
            }

            int index = 0;
            foreach (VRBIM_SlabShapeVertex vertex in slabShapeData.vertices)
            {
                GameObject obj = Instantiate(UnityAssetProvider.instance.slabShapeVertexPrefab);
                SlabShapeVertex vertexComponent = obj.GetComponent<SlabShapeVertex>();
                Debug.Assert(vertexComponent != null);
                vertexComponent.Data = vertex;
                vertexComponent.index = index;
                vertexComponent.toposolid =
                    VRApplication.instance.ClientDocument.GetElement(modifySubElementsData.toposolidId) as
                        VRBIM_Toposolid;
                _instantiatedSlabShapeVertices.Add(vertexComponent);
                index++;
            }
        }

        private void DestroyHandles()
        {
            foreach (SlabShapeCrease crease in _instantiatedSlabShapeCreases)
            {
                Destroy(crease.gameObject);
            }

            foreach (SlabShapeVertex vertex in _instantiatedSlabShapeVertices)
            {
                Destroy(vertex.gameObject);
            }
        }

        public override void OnStoppedEditMode()
        {
            base.OnStoppedEditMode();
            PropertiesPanel.Instance.StopEditMode();
            DestroyHandles();
        }

        void IToposolidModifySubElementsPanelListener.OnFinish()
        {
            _editModeData.isCanceled = false;
            EditModeState.instance.StopEditMode();
        }

        void IToposolidModifySubElementsPanelListener.OnCancel()
        {
            _editModeData.isCanceled = true;
            EditModeState.instance.StopEditMode();
        }
    }
}