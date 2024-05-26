using Autodesk.Revit.DB;
using revit_to_vr_common;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Remoting.Lifetime;

namespace revit_to_vr_plugin
{
    public static class ToposolidConversion
    {
        // only need to set the properties that are part of the toposolid subclass because
        // the properties that are part of the base type (e.g. Element) are populated by another method. 
        public static VRBIM_Toposolid ConvertToposolid(Toposolid toposolid)
        {
            return new VRBIM_Toposolid()
            {
                hostTopoId = toposolid.HostTopoId.Value,
                sketchId = toposolid.SketchId.Value,
                slabShapeData = ConvertSlabShapeData(toposolid.GetSlabShapeEditor())
            };
        }

        public static VRBIM_SlabShapeVertexType ConvertSlabShapeVertexType(SlabShapeVertexType type)
        {
            switch (type)
            {
                case SlabShapeVertexType.Invalid:
                    return VRBIM_SlabShapeVertexType.Invalid;
                case SlabShapeVertexType.Corner:
                    return VRBIM_SlabShapeVertexType.Corner;
                case SlabShapeVertexType.Edge:
                    return VRBIM_SlabShapeVertexType.Edge;
                case SlabShapeVertexType.Interior:
                    return VRBIM_SlabShapeVertexType.Interior;
            }
            return VRBIM_SlabShapeVertexType.Invalid;
        }

        public static VRBIM_SlabShapeVertex ConvertSlabShapeVertex(SlabShapeVertex vertex)
        {
            return new VRBIM_SlabShapeVertex()
            {
                vertexType = ConvertSlabShapeVertexType(vertex.VertexType),
                position = DataConversion.ConvertXYZ(vertex.Position)
            };
        }

        public static VRBIM_SlabShapeCreaseType ConvertSlabShapeCreaseType(SlabShapeCreaseType type)
        {
            switch (type)
            {
                case SlabShapeCreaseType.Invalid:
                    return VRBIM_SlabShapeCreaseType.Invalid;
                case SlabShapeCreaseType.Boundary:
                    return VRBIM_SlabShapeCreaseType.Boundary;
                case SlabShapeCreaseType.UserDrawn:
                    return VRBIM_SlabShapeCreaseType.UserDrawn;
                case SlabShapeCreaseType.Auto:
                    return VRBIM_SlabShapeCreaseType.Auto;
            }

            return VRBIM_SlabShapeCreaseType.Invalid;
        }

        public static VRBIM_SlabShapeCrease ConvertSlabShapeCrease(SlabShapeCrease crease)
        {
            VRBIM_Curve curve = null;
            try
            {
                curve = DataConversion.ConvertCurve(crease.Curve);
            }
            catch
            {
                // Revit throws an exception when the curve is invalid. 
                // maybe we could determine whether it contains a curve using
                // the creaseType, but documentation is so sparse that would just
                // be guess-work
                //
                // see the following incredible documentation for SlabShapeCreaseType:
                // 
                // Invalid:	The type of Crease is Invalid.
                // Boundary: The type of Crease is Boundary.
                // UserDrawn: The type of Crease is UserDrawn.
                // Auto: The type of Crease is Auto.
            }

            var output = new VRBIM_SlabShapeCrease()
            {
                curve = curve,
                endPoints =  new List<VRBIM_SlabShapeVertex>(crease.EndPoints.Size),
                creaseType = ConvertSlabShapeCreaseType(crease.CreaseType)
            };

            foreach (SlabShapeVertex vertex in crease.EndPoints)
            {
                Debug.Assert(vertex != null);
                output.endPoints.Add(ConvertSlabShapeVertex(vertex));
            }

            return output;
        }

        public static VRBIM_SlabShapeData ConvertSlabShapeData(SlabShapeEditor slabShapeEditor)
        {
            VRBIM_SlabShapeData output = new VRBIM_SlabShapeData()
            {
                vertices = new List<VRBIM_SlabShapeVertex>(slabShapeEditor.SlabShapeVertices.Size),
                creases = new List<VRBIM_SlabShapeCrease>(slabShapeEditor.SlabShapeCreases.Size)
            };

            foreach (SlabShapeVertex vertex in slabShapeEditor.SlabShapeVertices)
            {
                Debug.Assert(vertex != null);
                output.vertices.Add(ConvertSlabShapeVertex(vertex));
            }

            foreach (SlabShapeCrease crease in slabShapeEditor.SlabShapeCreases)
            {
                Debug.Assert(crease != null);
                output.creases.Add(ConvertSlabShapeCrease(crease));
            }

            return output;
        }
    }
}
