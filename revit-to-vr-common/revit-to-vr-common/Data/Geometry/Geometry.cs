using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace revit_to_vr_common
{

    [JsonDerivedType(typeof(VRBIM_Geometry), typeDiscriminator: "base")]
    [JsonDerivedType(typeof(VRBIM_Solid), typeDiscriminator: "solid")]
    [JsonDerivedType(typeof(VRBIM_Mesh), typeDiscriminator: "mesh")]
    [JsonDerivedType(typeof(VRBIM_GeometryInstance), typeDiscriminator: "geometryInstance")]
    [JsonDerivedType(typeof(VRBIM_Curve), typeDiscriminator: "curve")]
    [JsonDerivedType(typeof(VRBIM_PolyLine), typeDiscriminator: "polyLine")]
    [JsonDerivedType(typeof(VRBIM_Point), typeDiscriminator: "point")]
    [System.Serializable]
    public class VRBIM_Geometry
    {

    }

    [System.Serializable]
    [JsonDerivedType(typeof(VRBIM_Solid), typeDiscriminator: "solid")]
    public class VRBIM_Solid : VRBIM_Geometry
    {
        public Guid temporaryMeshId;
    }

    [System.Serializable]
    [JsonDerivedType(typeof(VRBIM_Mesh), typeDiscriminator: "mesh")]
    public class VRBIM_Mesh : VRBIM_Geometry
    {

    }

    [System.Serializable]
    [JsonDerivedType(typeof(VRBIM_GeometryInstance), typeDiscriminator: "geometryInstance")]
    public class VRBIM_GeometryInstance : VRBIM_Geometry
    {

    }

    [System.Serializable]
    [JsonDerivedType(typeof(VRBIM_PolyLine), typeDiscriminator: "polyLine")]
    public class VRBIM_PolyLine : VRBIM_Geometry
    {

    }

    [System.Serializable]
    [JsonDerivedType(typeof(VRBIM_Point), typeDiscriminator: "point")]
    public class VRBIM_Point : VRBIM_Geometry
    {

    }

}
