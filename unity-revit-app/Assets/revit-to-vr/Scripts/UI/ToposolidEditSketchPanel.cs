using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RevitToVR
{
    public interface IToposolidEditSketchPanelListener
    {
        public void OnFinish();

        public void OnCancel();

        public void OnDrawLine();
    }
    
    public class ToposolidEditSketchPanel : MonoBehaviour
    {
        public IToposolidEditSketchPanelListener Listener;

        // these methods are called by buttons
        public void Finish()
        {
            Listener?.OnFinish();
        }

        public void Cancel()
        {
            Listener?.OnCancel();
        }

        public void DrawLine()
        {
            Listener?.OnDrawLine();
        }
    }
}
