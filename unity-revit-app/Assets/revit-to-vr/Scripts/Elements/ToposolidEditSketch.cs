using revit_to_vr_common;
using UnityEngine;

namespace RevitToVR
{
    public class ToposolidEditSketch : MonoBehaviour
    {
        private VRBIM_Toposolid _toposolid;

        public void Initialize(VRBIM_Toposolid toposolid)
        {
            _toposolid = toposolid;
        }
    }
}