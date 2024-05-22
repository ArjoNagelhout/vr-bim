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

        public static revit_to_vr_common.VRBIM_AABB Convert(BoundingBoxXYZ bounds)
        {
            Vector3 min = Convert(bounds.Min);
            Vector3 max = Convert(bounds.Max);

            Vector3 center = (min + max) / 2.0f;



            Vector3 extents = (max - min) / 2.0f;

            return new revit_to_vr_common.VRBIM_AABB()
            {
                center = center,
                extents = extents
            };
        }

        public static revit_to_vr_common.VRBIM_Element Convert(Document document, ElementId elementId)
        {
            revit_to_vr_common.VRBIM_Element output = new revit_to_vr_common.VRBIM_Element()
            {
                elementId = elementId.Value
            };

            Autodesk.Revit.DB.Element element = document.GetElement(elementId);

            if (element == null)
            {
                UIConsole.Log("Element is null");
                return null;
            }

            if (!element.IsValidObject)
            {
                UIConsole.Log("Element is not a valid object");
                return null;
            }

            output.name = element.Name;

            // set geometry
            GeometryElement geometry = element.get_Geometry(new Options()
            {
                DetailLevel = ViewDetailLevel.Coarse,
                ComputeReferences = true,
                IncludeNonVisibleObjects = true
            });

            if (geometry == null)
            {
                return output;
            }

            // set bounds
            BoundingBoxXYZ bounds = geometry.GetBoundingBox();
            output.bounds = Convert(bounds);

            output.geometries = new List<VRBIM_Geometry>();
            foreach (GeometryObject obj in geometry)
            {
                VRBIM_Geometry outputGeometry = null;

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

                    outputGeometry = new VRBIM_Solid();
                }
                else if (obj is Mesh)
                {
                    Mesh mesh = obj as Mesh;
                    outputGeometry = new VRBIM_Mesh();
                }
                else if (obj is GeometryInstance)
                {
                    GeometryInstance instance = obj as GeometryInstance;
                    
                    outputGeometry = new VRBIM_GeometryInstance();
                }
                else if (obj is Curve)
                {
                    Curve curve = obj as Curve;

                    outputGeometry = new VRBIM_Curve();

                }
                else if (obj is Point)
                {
                    Point point = obj as Point;

                    outputGeometry = new VRBIM_Point();
                }
                else if (obj is PolyLine)
                {
                    PolyLine polyLine = obj as PolyLine;

                    outputGeometry = new VRBIM_PolyLine();
                }

                if (outputGeometry != null)
                {
                    output.geometries.Add(outputGeometry);
                }
            }

            // set material
            Autodesk.Revit.DB.Material material = geometry.MaterialElement;

            if (material != null)
            {
                
            }

            return output;
        }
    }
}
