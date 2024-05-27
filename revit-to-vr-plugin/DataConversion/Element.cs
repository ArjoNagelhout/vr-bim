using Autodesk.Revit.DB;
using revit_to_vr_common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace revit_to_vr_plugin
{
    public static partial class DataConversion
    {
        // when the mesh data needs to be updated, it gets added to the toSend parameter
        // to see whether it needs updating, it uses the ApplicationState.sentGeometryPerGeometryObjectId
        // https://help.autodesk.com/view/RVT/2022/ENU/?guid=Revit_API_Revit_API_Developers_Guide_Revit_Geometric_Elements_Geometry_GeometryObject_Class_html
        public static VRBIM_Element Convert(Element element, ClientState state, Queue<MeshDataToSend> toSend, ClientConfiguration clientConfiguration)
        {
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

            // call the right conversion method for setting all properties of subclasses of Element
            VRBIM_Element output = null;
            switch (element)
            {
                case Toposolid toposolid:
                    output = ToposolidConversion.ConvertToposolid(toposolid);
                    break;
                default:
                    output = new VRBIM_Element();
                    break;
            }

            // set properties of base class Element
            output.elementId = element.Id.Value;
            output.name = element.Name;
            output.ownerViewId = element.OwnerViewId.Value;

            output.location = ConvertLocation(element.Location);

            // set geometry
            GeometryElement geometry = element.get_Geometry(new Options()
            {
                DetailLevel = ConvertViewDetailLevel(clientConfiguration.viewDetailLevel),
                ComputeReferences = true,
                IncludeNonVisibleObjects = true
            });

            if (geometry == null)
            {
                return output;
            }

            // set bounds
            BoundingBoxXYZ bounds = geometry.GetBoundingBox();
            output.bounds = ConvertBoundingBox(bounds);

            output.geometries = new List<VRBIM_Geometry>();
            foreach (GeometryObject obj in geometry)
            {
                VRBIM_Geometry outputGeometry = null;

                // handle all cases that the geometry could be
                switch (obj)
                {
                    case Solid solid:
                        {
                            List<XYZ> positions = new List<XYZ>();
                            List<XYZ> normals = new List<XYZ>();
                            List<UInt32> indices = new List<UInt32>();

                            FaceArray faces = solid.Faces;

                            foreach (Face face in faces)
                            {
                                UInt32 vertexOffset = (UInt32)positions.Count;
                                Mesh mesh = face.Triangulate(clientConfiguration.triangulationlevelOfDetail);

                                int vertexCount = mesh.Vertices.Count;
                                positions.Capacity = positions.Count + vertexCount;

                                positions.AddRange(mesh.Vertices);
                                AppendNormals(normals, mesh);
                                AppendIndices(indices, mesh, vertexOffset, clientConfiguration);
                            }

                            Debug.Assert(positions.Count == normals.Count);

                            MeshDataToSend result = ConvertVertices(positions, normals, indices, Configuration.temporaryMeshIndex);
                            outputGeometry = new VRBIM_Solid()
                            {
                                temporaryMeshId = result.descriptor.id.temporaryId
                            };
                            toSend.Enqueue(result);
                        }
                        break;
                    case Mesh mesh:
                        {
                            outputGeometry = new VRBIM_Mesh()
                            {

                            };
                        }
                        break;
                    case GeometryInstance geometryInstance:
                        {

                        }
                        break;
                    case Curve curve:
                        {
                            outputGeometry = new VRBIM_Curve()
                            {

                            };
                        }
                        break;
                    case Point point:
                        {

                        }
                        break;
                    case PolyLine polyLine:
                        {

                        }
                        break;
                }

                if (outputGeometry != null)
                {
                    output.geometries.Add(outputGeometry);
                }
            }

            // set material
            Material material = geometry.MaterialElement;

            if (material != null)
            {

            }

            return output;
        }
    }
}
