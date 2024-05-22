using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace revit_to_vr_common
{
    // axis aligned bounding box
    [System.Serializable]
    public class VRBIM_AABB
    {
        public Vector3 center;
        public Vector3 extents;

        public static VRBIM_AABB FromMinMax(Vector3 min, Vector3 max)
        {
            Vector3 extents = (max - min) / 2;
            Vector3 center = min + extents;
            return new VRBIM_AABB() { center = center, extents = extents };
        }
    }

    [System.Serializable]
    public class VRBIM_Element
    {
        public long elementId;
        public string name;
        public VRBIM_AABB bounds;

        public List<VRBIM_Geometry> geometries;
    }

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

    [JsonDerivedType(typeof(VRBIM_Solid), typeDiscriminator: "solid")]
    [System.Serializable]
    public class VRBIM_Solid : VRBIM_Geometry
    {

    }

    [JsonDerivedType(typeof(VRBIM_Mesh), typeDiscriminator: "mesh")]
    [System.Serializable]
    public class VRBIM_Mesh : VRBIM_Geometry
    {

    }

    [JsonDerivedType(typeof(VRBIM_GeometryInstance), typeDiscriminator: "geometryInstance")]
    [System.Serializable]
    public class VRBIM_GeometryInstance : VRBIM_Geometry
    {

    }

    [JsonDerivedType(typeof(VRBIM_Curve), typeDiscriminator: "curve")]
    [System.Serializable]
    public class VRBIM_Curve : VRBIM_Geometry
    {

    }

    [JsonDerivedType(typeof(VRBIM_PolyLine), typeDiscriminator: "polyLine")]
    [System.Serializable]
    public class VRBIM_PolyLine : VRBIM_Geometry
    {

    }



    [JsonDerivedType(typeof(VRBIM_Point), typeDiscriminator: "point")]
    [System.Serializable]
    public class VRBIM_Point : VRBIM_Geometry
    {

    }


    [System.Serializable]
    public class VRBIM_Material
    {
        long materialId;
    }
}