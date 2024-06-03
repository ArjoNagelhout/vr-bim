using System;
using revit_to_vr_common;
using UnityEngine;

namespace RevitToVR
{
    // the way the edit mode is handled is that some ElementRenderer, such as the
    // ToposolidRenderer, could call the EditModeState to change to a specific edit mode,
    // which will call to the server,
    // the server will then on its turn apply the edit mode server side and return the updated data
    // to the client
    public class EditModeState : MonoBehaviour
    {
        private static EditModeState _instance;
        public static EditModeState instance => _instance;
        
        private EditModeData _data;
        private EditModeRenderer _renderer;
        
        // edit mode changes communicated by the server

        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(_instance);
            }
            _instance = this;
        }

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
                    _renderer.name = "Toposolid > EditModeRenderer: ModifySubElements";
                    break;
                case ToposolidEditSketchEditModeData toposolidEditSketch:
                    Instantiate<ToposolidEditSketchRenderer>();
                    _renderer.name = "Toposolid > EditModeRenderer: EditSketch";
                    break;
                default:
                    Debug.Assert(false, "Invalid EditModeData");
                    break;
            }
            
            _renderer.Initialize(_data);
            _renderer.OnStartedEditMode();
        }

        private void Instantiate<T>() where T : EditModeRenderer
        {
            GameObject obj = new GameObject();
            _renderer = obj.AddComponent<T>();
        }

        public void Apply(StoppedEditMode e)
        {
            if (_renderer != null)
            {
                _renderer.OnStoppedEditMode();
                Destroy(_renderer.gameObject);
                _renderer = null;                
            }
            _data = null;
        }
        
        // to be called by an Element inside the VR application
        
        public void StartEditMode(EditModeData editModeData)
        {
            // create start edit mode event
            MainServiceClient.instance.SendJson(new StartEditMode() { data = editModeData });
        }

        public void StopEditMode()
        {
            Debug.Assert(_data != null);
            MainServiceClient.instance.SendJson(new StopEditMode() {data = _data });
        }

        public void UpdateEditMode(UpdateEditModeData data)
        {
            Debug.Assert(data != null);
            MainServiceClient.instance.SendJson(new UpdateEditMode() {data = data});
        }
    }
}