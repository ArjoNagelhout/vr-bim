using System.Collections.Generic;
using revit_to_vr_common;

namespace RevitToVR
{
    // contains
    public class RevitDocument
    {
        // identified by ElementId (long)
        public Dictionary<long, Element> Elements = new Dictionary<long, Element>();
        
        // identified by GeometryId (long)
        public Dictionary<long, Geometry> Geometries = new Dictionary<long, Geometry>();
        
        // identified by MaterialId (long)
        public Dictionary<long, Material> Materials = new Dictionary<long, Material>();
    }
}