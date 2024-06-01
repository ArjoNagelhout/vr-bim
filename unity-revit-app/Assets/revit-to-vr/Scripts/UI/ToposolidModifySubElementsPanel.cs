using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RevitToVR
{
    public interface IToposolidModifySubElementsPanelListener
    {
        public void OnFinish();

        public void OnCancel();
        
        // add modify sub elements buttons here
    }
    
    public class ToposolidModifySubElementsPanel : MonoBehaviour
    {
        public IToposolidModifySubElementsPanelListener Listener;

        public void Finish()
        {
            Listener?.OnFinish();
        }

        public void Cancel()
        {
            Listener?.OnCancel();
        }
        
        // add modify sub elements buttons / actions here
    }
}
