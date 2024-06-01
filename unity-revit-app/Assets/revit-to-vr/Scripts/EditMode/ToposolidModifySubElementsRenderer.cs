using revit_to_vr_common;
using UnityEngine;

namespace RevitToVR
{
    public class ToposolidModifySubElementsRenderer : EditModeRenderer
    {
        private ToposolidModifySubElementsEditModeData modifySubElementsData => _editModeData as ToposolidModifySubElementsEditModeData;

        private GameObject _instantiatedUIPrefab;
        
        protected override void OnInitialize()
        {
            base.OnInitialize();
        }

        public override void OnStartedEditMode()
        {
            base.OnStartedEditMode();
            _instantiatedUIPrefab = PropertiesPanel.Instance.StartEditMode(
                UnityAssetProvider.instance.toposolidModifySubElementsUIPrefab);
        }

        public override void OnStoppedEditMode()
        {
            base.OnStoppedEditMode();
            PropertiesPanel.Instance.StopEditMode();
        }
    }
}