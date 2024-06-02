﻿using Autodesk.Revit.DB;
using revit_to_vr_common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace revit_to_vr_plugin
{
    public class ToposolidEditSketchEditMode : EditMode
    {
        private ToposolidEditSketchEditModeData editSketchData => editModeData as ToposolidEditSketchEditModeData;

        protected override void OnStartEditMode()
        {

        }

        protected override void OnStopEditMode()
        {

        }
    }

    public class ToposolidModifySubElementsEditMode : EditMode
    {
        private ToposolidModifySubElementsEditModeData modifySubElementsData => editModeData as ToposolidModifySubElementsEditModeData;

        protected override void OnStartEditMode()
        {
            long id = modifySubElementsData.toposolidId;
            Element toposolidElement = Application.Instance.GetElement(id);
            Debug.Assert(toposolidElement is Toposolid);
            Toposolid toposolid = toposolidElement as Toposolid;
            modifySubElementsData.slabShapeData = ToposolidConversion.ConvertSlabShapeData(toposolid.GetSlabShapeEditor());
        }

        protected override void OnStopEditMode()
        {

        }
    }
}
