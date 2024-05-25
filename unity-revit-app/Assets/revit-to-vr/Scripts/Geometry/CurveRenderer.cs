using System;
using System.Collections;
using System.Collections.Generic;
using revit_to_vr_common;
using UnityEngine;

namespace RevitToVR
{
    public class CurveRenderer : GeometryObjectRenderer
    {
        private VRBIM_Curve solid => _geometry as VRBIM_Curve;
    }
}