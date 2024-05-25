using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace revit_to_vr_common
{
    public enum VRBIM_ViewDetailLevel
    {
        Coarse,
        Medium,
        Fine
    }

    // axis aligned bounding box
    [System.Serializable]
    public class VRBIM_AABB
    {
        public VRBIM_Vector3 center;
        public VRBIM_Vector3 extents;

        public static VRBIM_AABB FromMinMax(VRBIM_Vector3 min, VRBIM_Vector3 max)
        {
            VRBIM_Vector3 extents = (max - min) / 2;
            VRBIM_Vector3 center = min + extents;
            return new VRBIM_AABB() { center = center, extents = extents };
        }
    }

    [JsonDerivedType(typeof(VRBIM_Location), typeDiscriminator: "base")]
    [JsonDerivedType(typeof(VRBIM_LocationPoint), typeDiscriminator: "point")]
    [JsonDerivedType(typeof(VRBIM_LocationCurve), typeDiscriminator: "curve")]
    public class VRBIM_Location
    {
        
    }

    [JsonDerivedType(typeof(VRBIM_LocationPoint), typeDiscriminator: "point")]
    public class VRBIM_LocationPoint : VRBIM_Location
    {
        public VRBIM_Vector3 point;
        public float rotation; // rotation along the plane of the associated view
    }

    // a location curve can have multiple elements joining, such as two walls
    // we can query which elements are joining, and what type of join it is
    // for now, this is not relevant
    [JsonDerivedType(typeof(VRBIM_LocationCurve), typeDiscriminator: "curve")]
    public class VRBIM_LocationCurve : VRBIM_Location
    {
        public VRBIM_Curve curve;
    }

    [System.Serializable]
    public class VRBIM_Element
    {
        public long elementId;
        public string name;
        public VRBIM_AABB bounds;
        public long ownerViewId; // id of the view that owns the element, used for determining the plane along which rotation is defined of the element
        public VRBIM_Location location; // transform

        public List<VRBIM_Geometry> geometries;
    }

    [System.Serializable]
    public class VRBIM_MeshId : IEquatable<VRBIM_MeshId>
    {
        public int id;
        public Guid temporaryId; // we assign a temporary id so that even temporary meshes can be identified
        public bool IsTemporary => id == Configuration.temporaryMeshIndex; // if temporary, we should use the temporary id instead of the normal id

        public bool Equals(VRBIM_MeshId other)
        {
            return this.GetHashCode() == other.GetHashCode();
        }

        public override int GetHashCode()
        {
            return id.GetHashCode() ^ temporaryId.GetHashCode();
        }
    }

    [System.Serializable]
    public class VRBIM_MeshDataDescriptor
    {
        // id
        public VRBIM_MeshId id;

        // metadata
        public int vertexCount;
        public int indexCount;
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
    [JsonDerivedType(typeof(VRBIM_Curve), typeDiscriminator: "curve")]
    public class VRBIM_Curve : VRBIM_Geometry
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


    [System.Serializable]
    public class VRBIM_Material
    {
        long materialId;
    }

}