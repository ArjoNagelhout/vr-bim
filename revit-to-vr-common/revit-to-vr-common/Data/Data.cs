using System.Collections.Generic;

namespace revit_to_vr_common
{
    // axis aligned bounding box
    [System.Serializable]
    public class AABB
    {
        public Vector3 center;
        public Vector3 extents;

        public static AABB FromMinMax(Vector3 min, Vector3 max)
        {
            Vector3 extents = (max - min) / 2;
            Vector3 center = min + extents;
            return new AABB() { center = center, extents = extents };
        }
    }

    [System.Serializable]
    public class Element
    {
        public long elementId;
        public AABB bounds;
        public bool valid;

        public List<Geometry> geometries;
    }

    [System.Serializable]
    public class Geometry
    {
        
    }

    [System.Serializable]
    public class Material
    {
        long materialId;
    }
}