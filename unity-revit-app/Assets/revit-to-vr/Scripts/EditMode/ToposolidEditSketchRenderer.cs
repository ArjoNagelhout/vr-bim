using revit_to_vr_common;
using UnityEngine;

namespace RevitToVR
{
    public class ToposolidEditSketchRenderer : EditModeRenderer, IToposolidEditSketchPanelListener
    {
        private ToposolidEditSketchEditModeData editSketchData => _editModeData as ToposolidEditSketchEditModeData;

        private GameObject _instantiatedUIPrefab;
        private ToposolidEditSketchPanel _panel;

        protected override void OnInitialize()
        {
            base.OnInitialize();
        }

        public override void OnStartedEditMode()
        {
            base.OnStartedEditMode();
            _instantiatedUIPrefab = PropertiesPanel.Instance.StartEditMode(
                UnityAssetProvider.instance.toposolidEditSketchRendererUIPrefab);
            _panel = _instantiatedUIPrefab.GetComponent<ToposolidEditSketchPanel>();
            Debug.Assert(_panel != null);
            _panel.Listener = this;
        }

        public override void OnStoppedEditMode()
        {
            base.OnStoppedEditMode();
            PropertiesPanel.Instance.StopEditMode();
        }

        // IToposolidEditSketchPanelListener implementation
        
        void IToposolidEditSketchPanelListener.OnFinish()
        {
            _editModeData.isCanceled = false;
            EditModeState.instance.StopEditMode();
        }

        void IToposolidEditSketchPanelListener.OnCancel()
        {
            _editModeData.isCanceled = true;
            EditModeState.instance.StopEditMode();
        }

        void IToposolidEditSketchPanelListener.OnDrawLine()
        {
            // todo, should create a draw line context, which captures XR Interactor input,
            // and takes two points. (maybe add snapping)
        }
    }
}