using System;
using revit_to_vr_common;
using UnityEngine;

namespace RevitToVR
{
    // this is the logic that enables editing a Toposolid or inspecting its data
    public class ToposolidRenderer : ElementRenderer
    {
        private VRBIM_Toposolid toposolid => _element as VRBIM_Toposolid;
        
        protected override void OnInitialize()
        {
            base.OnInitialize();
            name += " (Toposolid)";
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

        public override void OnSelect()
        {
            base.OnSelect();
        }

        public override void OnDeselect()
        {
            base.OnDeselect();
        }
        
        // Edit modes
        // these methods activate the edit mode for these  

        // edit the 2D sketch that defines the bounds (i.e. outline) of the Toposolid 
        public void EditSketch()
        {
            
        }

        // edit the creases and vertices that define the height of the Toposolid
        public void ModifySubElements()
        {
            
        }
    }
}