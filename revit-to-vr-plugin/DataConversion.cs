using Autodesk.Revit.DB;
using revit_to_vr_common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Data.Odbc;

namespace revit_to_vr_plugin
{
    public static class DataConversion
    {
        // converts doubles to floats, determine whether this is an issue for precision
        // in large models
        public static VRBIM_Vector3 Convert(XYZ xyz)
        {
            return new VRBIM_Vector3()
            {
                x = (float)xyz.X,
                y = (float)xyz.Y,
                z = (float)xyz.Z
            };
        }

        public static byte[] GetBytes(XYZ xyz)
        {
            byte[] data = new byte[4 * 4];
            byte[] x = BitConverter.GetBytes((float)xyz.X);
            byte[] y = BitConverter.GetBytes((float)xyz.Y);
            byte[] z = BitConverter.GetBytes((float)xyz.Z);

            Buffer.BlockCopy(x, 0, data, 0, 4);
            Buffer.BlockCopy(y, 0, data, 4, 4);
            Buffer.BlockCopy(z, 0, data, 8, 4);

            return data;
        }

        public static ViewDetailLevel Convert(VRBIM_ViewDetailLevel level)
        {
            switch (level)
            {
                case VRBIM_ViewDetailLevel.Coarse:
                    return ViewDetailLevel.Coarse;
                case VRBIM_ViewDetailLevel.Medium:
                    return ViewDetailLevel.Medium;
                case VRBIM_ViewDetailLevel.Fine:
                    return ViewDetailLevel.Fine;
            }
            return ViewDetailLevel.Undefined;
        }

        public static VRBIM_AABB Convert(BoundingBoxXYZ bounds)
        {
            VRBIM_Vector3 min = Convert(bounds.Min);
            VRBIM_Vector3 max = Convert(bounds.Max);

            VRBIM_Vector3 center = (min + max) / 2.0f;
            VRBIM_Vector3 extents = (max - min) / 2.0f;

            return new VRBIM_AABB()
            {
                center = center,
                extents = extents
            };
        }

        private static int temporaryMeshIndex = int.MinValue;

        public static MeshDataToSend ConvertTemporaryMesh(Mesh mesh)
        {
            return ConvertMesh(mesh, temporaryMeshIndex);
        }

        public static MeshDataToSend ConvertVertices(IList<XYZ> positions, IList<XYZ> normals, int id)
        {
            // store interleaved
            
            int vertexCount = positions.Count;
            Debug.Assert(positions.Count == normals.Count);

            int vector3Size = 4 * 4;
            int offset = 2 * vector3Size;
            int sizeInBytes = offset * vertexCount;
            byte[] data = new byte[sizeInBytes];

            for (int i = 0; i < vertexCount; i++)
            {
                // we don't store it as the intermediate VRBIM_Vector3, because that would create
                // a managed object we need to marshal to support copying. 
                byte[] position = GetBytes(positions[i]);
                byte[] normal = GetBytes(normals[i]);

                Buffer.BlockCopy(position, 0, data, offset * i, vector3Size);
                Buffer.BlockCopy(normal, 0, data, offset * i + vector3Size, vector3Size);
            }

            VRBIM_MeshDataDescriptor descriptor = new VRBIM_MeshDataDescriptor()
            {
                id = new VRBIM_MeshId()
                {
                    id = id,
                    temporary = IsMeshIdTemporary(id),
                    temporaryId = Guid.NewGuid()
                },
                vertexCount = vertexCount,
                normalCount = normals.Count
            };

            return new MeshDataToSend()
            {
                descriptor = descriptor,
                data = data
            };
        }

        public static MeshDataToSend ConvertMesh(Mesh mesh, int id)
        {
            IList<XYZ> positions = mesh.Vertices;
            IList<XYZ> normals = mesh.GetNormals();

            return ConvertVertices(positions, normals, id);
        }

        public class MeshDataToSend
        {
            public VRBIM_MeshDataDescriptor descriptor;
            public byte[] data;
        }

        public static bool IsMeshIdTemporary(int meshId)
        {
            return meshId < 0;
        }

        // https://help.autodesk.com/view/RVT/2022/ENU/?guid=Revit_API_Revit_API_Developers_Guide_Revit_Geometric_Elements_Geometry_GeometryObject_Class_html
        public static VRBIM_Element Convert(ElementId elementId, Document document, Queue<MeshDataToSend> toSend)
        {
            VRBIM_Element output = new VRBIM_Element()
            {
                elementId = elementId.Value
            };

            Element element = document.GetElement(elementId);

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
                DetailLevel = Convert(Configuration.viewDetailLevel),
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
                switch (obj)
                {
                    case Solid solid:
                        {
                            List<XYZ> positions = new List<XYZ>();
                            List<XYZ> normals = new List<XYZ>();

                            FaceArray faces = solid.Faces;

                            foreach (Face face in faces)
                            {
                                Mesh mesh = face.Triangulate(Configuration.triangulationlevelOfDetail);
                                positions.Capacity = positions.Count + mesh.Vertices.Count;

                                // handle distribution of normals
                                // https://www.revitapidocs.com/2019/8e00e7aa-b39b-51b4-26e4-0f5c1404df32.htm
                                DistributionOfNormals distribution = mesh.DistributionOfNormals;
                                switch (distribution)
                                {
                                    case DistributionOfNormals.AtEachPoint:
                                        // one for each vertex
                                        break;
                                    case DistributionOfNormals.OnePerFace:
                                        // one for the entire face
                                        break;
                                    case DistributionOfNormals.OnEachFacet:
                                        // one per triangle
                                        break;
                                }


                                // this is not true apparently
                                Debug.Assert(mesh.Vertices.Count == mesh.NumberOfNormals);
                                positions.AddRange(mesh.Vertices);
                                normals.AddRange(mesh.GetNormals());
                            }

                            MeshDataToSend result = ConvertVertices(positions, normals, temporaryMeshIndex);
                            outputGeometry = new VRBIM_Solid()
                            {
                                temporaryMeshId = result.descriptor.id.temporaryId
                            };
                            toSend.Enqueue(result);
                        }
                        break;
                    case Mesh mesh:
                        {

                        }
                        break;
                    case GeometryInstance geometryInstance:
                        {

                        }
                        break;
                    case Curve curve:
                        {

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
