using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace revit_to_vr_common
{
    // https://help.autodesk.com/view/RVT/2022/ENU/?guid=Revit_API_Revit_API_Developers_Guide_Revit_Geometric_Elements_Geometry_GeometryObject_Class_Curves_Curve_Parameterization_html

    [System.Serializable]
    [JsonDerivedType(typeof(VRBIM_Curve), typeDiscriminator: "curve")]
    [JsonDerivedType(typeof(VRBIM_Arc), typeDiscriminator: "arc")]
    [JsonDerivedType(typeof(VRBIM_Line), typeDiscriminator: "line")]
    public class VRBIM_Curve : VRBIM_Geometry
    {
        public float startParameter;
        public float endParameter;
        public VRBIM_Vector3 startPoint;
        public VRBIM_Vector3 endPoint;
        public bool isBound;
        public bool isClosed;
        public bool isCyclic;
        public float length;
        public List<VRBIM_Vector3> tesselatedCurve; // for display purposes
    }

    [System.Serializable]
    [JsonDerivedType(typeof(VRBIM_Arc), typeDiscriminator: "arc")]
    public class VRBIM_Arc : VRBIM_Curve
    {
        public VRBIM_Vector3 center;
        public VRBIM_Vector3 normal; // normal to the plane in which the arc is defined
        public float radius;

        // what are these?
        public VRBIM_Vector3 xDirection;
        public VRBIM_Vector3 yDirection;
    }

    [System.Serializable]
    [JsonDerivedType(typeof(VRBIM_Line), typeDiscriminator: "line")]
    public class VRBIM_Line : VRBIM_Curve
    {
        public VRBIM_Vector3 direction;
        public VRBIM_Vector3 origin;
    }
}
