using revit_to_vr_common;

namespace RevitToVR
{
    public class ToposolidEditSketchRenderer : EditModeRenderer
    {
        private ToposolidEditSketchEditModeData editSketchData => _editModeData as ToposolidEditSketchEditModeData;

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