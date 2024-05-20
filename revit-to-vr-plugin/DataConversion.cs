using Autodesk.Revit.DB;
using revit_to_vr_common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace revit_to_vr_plugin
{
    public static class DataConversion
    {
        // converts doubles to floats, determine whether this is an issue for precision
        // in large models
        public static revit_to_vr_common.Vector3 Convert(XYZ xyz)
        {
            return new revit_to_vr_common.Vector3()
            {
                x = (float)xyz.X,
                y = (float)xyz.Y,
                z = (float)xyz.Z
            };
        }

        public static revit_to_vr_common.AABB Convert(BoundingBoxXYZ bounds)
        {
            Vector3 min = Convert(bounds.Min);
            Vector3 max = Convert(bounds.Max);

            Vector3 center = (min + max) / 2.0f;



            Vector3 extents = (max - min) / 2.0f;

            return new revit_to_vr_common.AABB()
            {
                center = center,
                extents = extents
            };
        }

        public static revit_to_vr_common.Element Convert(Document document, ElementId elementId)
        {
            revit_to_vr_common.Element output = new revit_to_vr_common.Element()
            {
                elementId = elementId.Value,
                valid = false,
            };

            Autodesk.Revit.DB.Element element = document.GetElement(elementId);

            if (!element.IsValidObject)
            {
                UIConsole.Log("Element is not a valid object");
                return output;
            }

            GeometryElement geometry = element.get_Geometry(new Options()
            {
                DetailLevel = ViewDetailLevel.Coarse,
                ComputeReferences = true,
                IncludeNonVisibleObjects = true
            });

            if (geometry == null)
            {
                UIConsole.Log("Geometry is null");
                return output;
            }

            // set bounds
            BoundingBoxXYZ bounds = geometry.GetBoundingBox();
            output.bounds = Convert(bounds);

            // set geometry
            output.geometries = new List<Geometry>();

            foreach (GeometryObject obj in geometry)
            {
                // handle all cases that the geometry could be
                if (obj is Solid)
                {
                    Solid solid = obj as Solid;
                    FaceArray faces = solid.Faces;
                    foreach (Face face in faces)
                    {
                        Mesh mesh = face.Triangulate(1);
                        IList<XYZ> vertices = mesh.Vertices;
                        IList<XYZ> normals = mesh.GetNormals();
                    }
                }
                else if (obj is Mesh)
                {
                    Mesh mesh = obj as Mesh;

                }
                else if (obj is GeometryInstance)
                {
                    GeometryInstance instance = obj as GeometryInstance;
                }
                else if (obj is Curve)
                {
                    Curve curve = obj as Curve;
                }
                else if (obj is Autodesk.Revit.DB.Point)
                {
                    Autodesk.Revit.DB.Point point = obj as Autodesk.Revit.DB.Point;
                }
                else if (obj is PolyLine)
                {
                    PolyLine polyLine = obj as PolyLine;
                }
                output.geometries.Add(new Geometry());
            }

            // set material
            Autodesk.Revit.DB.Material material = geometry.MaterialElement;

            if (material == null)
            {
                UIConsole.Log("Material is null");
                return output;
            }

            output.valid = true;
            return output;
        }
    }
}
