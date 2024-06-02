using Autodesk.Revit.UI;
using revit_to_vr_common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace revit_to_vr_plugin
{
    public class EditMode
    {
        public EditModeData editModeData;
        
        // sets all properties that are part of this edit mode,
        // after this method, this data gets sent back to the client
        public void StartEditMode()
        {
            Debug.Assert(editModeData != null);
            UIConsole.Log("StartEditMode");
            OnStartEditMode();
        }

        protected virtual void OnStartEditMode()
        {

        }

        public void StopEditMode()
        {
            UIConsole.Log("StopEditMode");
            OnStopEditMode();
        }

        protected virtual void OnStopEditMode()
        {

        }

        public void UpdateEditMode(UIApplication uiApp, UpdateEditModeData data)
        {
            UIConsole.Log("UpdateEditMode");
            OnUpdateEditMode(uiApp, data);
        }

        protected virtual void OnUpdateEditMode(UIApplication uiApp, UpdateEditModeData data)
        {

        }
    }
}
