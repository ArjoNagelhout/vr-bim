using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace revit_to_vr_common
{
    [System.Serializable]
    [JsonDerivedType(typeof(VRBIM_Element), typeDiscriminator: "base")]
    [JsonDerivedType(typeof(VRBIM_Toposolid), typeDiscriminator: "toposolid")]
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
    [JsonDerivedType(typeof(VRBIM_Toposolid), typeDiscriminator: "toposolid")]
    public class VRBIM_Toposolid : VRBIM_Element
    {
        public long hostTopoId; // if this TopoSolid is a subdivision of another TopoSolid this returns the element id of that containing TopoSolid
        public long sketchId; // the sketch is used to define the shape of the TopoSolid
        public VRBIM_SlabShapeData slabShapeData;
    }

    [System.Serializable]
    public enum VRBIM_SlabShapeVertexType
    {
        Invalid,
        Corner,
        Edge,
        Interior
    }

    [System.Serializable]
    public class VRBIM_SlabShapeVertex
    {
        public VRBIM_Vector3 position;
        public VRBIM_SlabShapeVertexType vertexType;
    }

    [System.Serializable]
    public enum VRBIM_SlabShapeCreaseType
    {
        Invalid,
        Boundary,
        UserDrawn,
        Auto
    }

    [System.Serializable]
    public class VRBIM_SlabShapeCrease
    {
        public VRBIM_Curve curve;
        public List<VRBIM_SlabShapeVertex> endPoints;
        public VRBIM_SlabShapeCreaseType creaseType;
    }

    [System.Serializable]
    public class VRBIM_SlabShapeData
    {
        public List<VRBIM_SlabShapeVertex> vertices;
        public List<VRBIM_SlabShapeCrease> creases;
    }
}
