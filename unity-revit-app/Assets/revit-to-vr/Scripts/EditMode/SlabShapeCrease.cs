using System.Collections;
using System.Collections.Generic;
using revit_to_vr_common;
using UnityEngine;

namespace RevitToVR
{
    public class SlabShapeCrease : MonoBehaviour
    {
        private VRBIM_SlabShapeCrease _data;

        public VRBIM_SlabShapeCrease Data
        {
            get => _data;
            set
            {
                _data = value;
                OnDataChanged();
            }
        }

        private void OnDataChanged()
        {
            // update the transform and render data of this crease
        }
    }
}
