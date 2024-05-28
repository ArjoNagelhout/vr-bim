using revit_to_vr_common;

namespace RevitToVR
{
    public class ToposolidModifySubElementsRenderer : EditModeRenderer
    {
        private ToposolidModifySubElementsEditModeData modifySubElementsData => _editModeData as ToposolidModifySubElementsEditModeData;
        
        protected override void OnInitialize()
        {
            base.OnInitialize();
        }

        public override void StartEditMode()
        {
            base.StartEditMode();
        }

        public override void StopEditMode()
        {
            base.StopEditMode();
        }
    }
}