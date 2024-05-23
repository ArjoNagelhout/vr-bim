using System.Collections.Generic;
using revit_to_vr_common;

namespace RevitToVR
{
    // contains
    public class RevitDocument
    {
        // identified by ElementId (long)
        public Dictionary<long, VRBIM_Element> Elements = new Dictionary<long, VRBIM_Element>();
        
        // identified by GeometryId (long)
        public Dictionary<long, VRBIM_Geometry> Geometries = new Dictionary<long, VRBIM_Geometry>();
        
        // identified by MaterialId (long)
        public Dictionary<long, VRBIM_Material> Materials = new Dictionary<long, VRBIM_Material>();
    }
}