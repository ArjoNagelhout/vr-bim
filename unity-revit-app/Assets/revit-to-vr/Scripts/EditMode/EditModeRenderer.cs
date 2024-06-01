using revit_to_vr_common;
using UnityEngine;

namespace RevitToVR
{
    public class EditModeRenderer : MonoBehaviour
    {
        protected EditModeData _editModeData;

        public void Initialize(EditModeData editModeData)
        {
            _editModeData = editModeData;
            OnInitialize();
        }

        protected virtual void OnInitialize()
        {
            
        }
        
        public virtual void OnStartedEditMode()
        {
            
        }

        public virtual void OnStoppedEditMode()
        {
            
        }
    }
}