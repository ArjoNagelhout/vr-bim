using revit_to_vr_common;
using UnityEngine;

namespace RevitToVR
{
    
    
    public class ToposolidModifySubElementsRenderer : EditModeRenderer, IToposolidModifySubElementsPanelListener
    {
        private ToposolidModifySubElementsEditModeData modifySubElementsData => _editModeData as ToposolidModifySubElementsEditModeData;

        private GameObject _instantiatedUIPrefab;
        private ToposolidModifySubElementsPanel _panel;
        
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
        }

        public override void OnStoppedEditMode()
        {
            base.OnStoppedEditMode();
            PropertiesPanel.Instance.StopEditMode();
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