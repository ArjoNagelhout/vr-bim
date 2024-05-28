using revit_to_vr_common;
using UnityEngine;

namespace RevitToVR
{
    public class EditModeState : MonoBehaviour
    {
        private EditModeData _data;
        private EditModeRenderer _renderer;
        
        public void Apply(StartedEditMode e)
        {
            // StoppedEditMode should always be called before StartedEditMode
            Debug.Assert(_data == null);
            Debug.Assert(_renderer == null);
            _data = e.populatedEditModeData;
            switch (_data)
            {
                case ToposolidModifySubElementsEditModeData toposolidModifySubElements:
                    Instantiate<ToposolidModifySubElementsRenderer>();
                    break;
                case ToposolidEditSketchEditModeData toposolidEditSketch:
                    Instantiate<ToposolidEditSketchRenderer>();
                    break;
                default:
                    Debug.Assert(false, "Invalid EditModeData");
                    break;
            }
            
            _renderer.Initialize(_data);
            _renderer.StartEditMode();
        }

        private void Instantiate<T>() where T : EditModeRenderer
        {
            GameObject obj = new GameObject();
            obj.name = nameof(T);
            _renderer = obj.AddComponent<T>();
        }

        public void Apply(StoppedEditMode e)
        {
            _renderer.StopEditMode();

            _renderer = null;
            _data = null;
        }
    }
}