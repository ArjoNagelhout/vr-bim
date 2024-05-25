using Autodesk.Revit.DB;
using revit_to_vr_common;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace revit_to_vr_plugin
{
    public static class DataConversion
    {
        // converts doubles to floats, determine whether this is an issue for precision
        // in large models
        public static VRBIM_Vector3 ConvertVector(XYZ xyz)
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
            byte[] data = new byte[3 * 4];
            byte[] x = BitConverter.GetBytes((float)xyz.X);
            byte[] y = BitConverter.GetBytes((float)xyz.Y);
            byte[] z = BitConverter.GetBytes((float)xyz.Z);

            Buffer.BlockCopy(x, 0, data, 0, 4);
            Buffer.BlockCopy(y, 0, data, 4, 4);
            Buffer.BlockCopy(z, 0, data, 8, 4);

            return data;
        }

        public static ViewDetailLevel ConvertViewDetailLevel(VRBIM_ViewDetailLevel level)
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

        public static VRBIM_AABB ConvertBoundingBox(BoundingBoxXYZ bounds)
        {
            VRBIM_Vector3 min = ConvertVector(bounds.Min);
            VRBIM_Vector3 max = ConvertVector(bounds.Max);

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

        public static MeshDataToSend ConvertVertices(IList<XYZ> positions, IList<XYZ> normals, IList<UInt32> indices, int id)
        {
            // store interleaved
            
            int vertexCount = positions.Count;
            Debug.Assert(positions.Count == normals.Count);

            int vector3SizeInBytes = 3 * 4;
            int vertexStrideInBytes = 2 * vector3SizeInBytes;

            int indexCount = indices.Count;
            int indexSizeInBytes = 4; // 32 bits unsigned integer

            int sizeInBytes = vertexStrideInBytes * vertexCount + indexSizeInBytes * indexCount;
            byte[] data = new byte[sizeInBytes];

            // copy vertices
            for (int i = 0; i < vertexCount; i++)
            {
                // we don't store it as the intermediate VRBIM_Vector3, because that would create
                // a managed object we need to marshal to support copying. 
                byte[] position = GetBytes(positions[i]);
                byte[] normal = GetBytes(normals[i]);

                //Debug.WriteLine($"pos at index {i}: {positions[i].X}, {positions[i].Y}, {positions[i].Z} (bytes: {BitConverter.ToString(position, 0, 4 * 3)})");

                Buffer.BlockCopy(position, 0, data, vertexStrideInBytes * i, vector3SizeInBytes);
                Buffer.BlockCopy(normal, 0, data, vertexStrideInBytes * i + vector3SizeInBytes, vector3SizeInBytes);
            }

            // copy indices
            for (int i = 0; i < indexCount; i++)
            {
                byte[] index = BitConverter.GetBytes(indices[i]);
                Buffer.BlockCopy(index, 0, data, vertexStrideInBytes * vertexCount + i * indexSizeInBytes, indexSizeInBytes);
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
                indexCount = indexCount
            };

            //string indicesString = "";
            //foreach (UInt32 index in indices)
            //{
                //indicesString += index + ", ";
            //    Debug.Assert(index < vertexCount);
            //}
            //UIConsole.Log(indicesString);

            return new MeshDataToSend()
            {
                descriptor = descriptor,
                data = data
            };
        }

        public static MeshDataToSend ConvertMesh(Mesh mesh, int id)
        {
            IList<XYZ> positions = mesh.Vertices;
            List<XYZ> normals = new List<XYZ>();
            List<UInt32> indices = new List<UInt32>();
            AppendNormals(normals, mesh);
            AppendIndices(indices, mesh, 0);

            return ConvertVertices(positions, normals, indices, id);
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

        // appends the normals of the provided mesh to the normals
        // handles different distributions of normals
        public static void AppendNormals(List<XYZ> normals, Mesh mesh)
        {
            int vertexCount = mesh.Vertices.Count;
            normals.Capacity = normals.Count + vertexCount;

            // handle distribution of normals
            // https://www.revitapidocs.com/2019/8e00e7aa-b39b-51b4-26e4-0f5c1404df32.htm
            DistributionOfNormals distribution = mesh.DistributionOfNormals;
            switch (distribution)
            {
                case DistributionOfNormals.AtEachPoint:
                    // one for each vertex
                    Debug.Assert(mesh.NumberOfNormals == mesh.Vertices.Count);
                    normals.AddRange(mesh.GetNormals());
                    break;
                case DistributionOfNormals.OnePerFace:
                    // one for the entire face
                    Debug.Assert(mesh.NumberOfNormals == 1);
                    XYZ normal = mesh.GetNormal(0);
                    for (int i = 0; i < mesh.Vertices.Count; i++)
                    {
                        normals.Add(normal);
                    }
                    break;
                case DistributionOfNormals.OnEachFacet:
                    // one per triangle
                    for (int v = 0; v < mesh.NumTriangles; v++)
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            normals.Add(mesh.GetNormal(v));
                        }
                    }
                    break;
            }
        }

        // returns index count
        public static void AppendIndices(List<UInt32> indices, Mesh mesh, UInt32 vertexOffset)
        {
            int indexCount = mesh.NumTriangles * 3;
            indices.Capacity = indices.Count + indexCount; // increase capacity
            // test whether the indices are correct
            for (int triangleIndex = 0; triangleIndex < mesh.NumTriangles; triangleIndex++)
            {
                MeshTriangle triangle = mesh.get_Triangle(triangleIndex);
                // these are probably sequential, but just to be sure
                UInt32 xIndex = triangle.get_Index(Configuration.flipWindingOrder ? 2 : 0);
                UInt32 yIndex = triangle.get_Index(1); // 1 is always the same
                UInt32 zIndex = triangle.get_Index(Configuration.flipWindingOrder ? 0 : 2);
                indices.Add(vertexOffset + xIndex);
                indices.Add(vertexOffset + yIndex);
                indices.Add(vertexOffset + zIndex);
            }
        }

        public static bool XYZIsEqual(XYZ lhs, XYZ rhs)
        {
            return lhs.X == rhs.X && lhs.Y == rhs.Y && lhs.Z == rhs.Z;
        }

        public static VRBIM_LocationPoint ConvertLocationPoint(LocationPoint locationPoint)
        {
            VRBIM_LocationPoint output = new VRBIM_LocationPoint();

            try
            {
                output.point = ConvertVector(locationPoint.Point);
                output.rotation = (float)locationPoint.Rotation;
            }
            catch
            {
                return output;
            }

            return output;
        }

        public static VRBIM_LocationCurve ConvertLocationCurve(LocationCurve locationCurve)
        {
            return new VRBIM_LocationCurve()
            {

            };
        }

        public static VRBIM_Location ConvertLocation(Location location)
        {
            try
            {
                switch (location)
                {
                    case LocationPoint locationPoint:
                        return ConvertLocationPoint(locationPoint);

                    case LocationCurve locationCurve:
                        return ConvertLocationCurve(locationCurve);
                }
            }
            catch
            {
                return null;
            }
            
            //Debug.Assert(false); auto join tracker element can have no location for example
            return null;
        }

        // when the mesh data needs to be updated, it gets added to the toSend parameter
        // to see whether it needs updating, it uses the ApplicationState.sentGeometryPerGeometryObjectId
        // https://help.autodesk.com/view/RVT/2022/ENU/?guid=Revit_API_Revit_API_Developers_Guide_Revit_Geometric_Elements_Geometry_GeometryObject_Class_html
        public static VRBIM_Element Convert(Element element, ClientState state, Queue<MeshDataToSend> toSend)
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

            VRBIM_Element output = new VRBIM_Element()
            {
                elementId = element.Id.Value,
                name = element.Name,
                ownerViewId = element.OwnerViewId.Value,
            };

            if (!(element is FamilyInstance))
            {
                output.location = ConvertLocation(element.Location);
            }

            // set geometry
            GeometryElement geometry = element.get_Geometry(new Options()
            {
                DetailLevel = ConvertViewDetailLevel(Configuration.viewDetailLevel),
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
                // we need to check whether the GeometryObject is already sent
                // 1. see if the GeometryObject id has been sent at all
                // if not, we need to send it
                // otherwise:
                // 2. do a diff between the last sent GeometryObject and this one
                if (state.sentGeometry.TryGetValue(obj.Id, out SentGeometryObjectData data))
                {
                    if (data.hashCode == obj.GetHashCode())
                    {
                        // we don't need to send this geometry as it has not changed. 
                        UIConsole.Log("Geometry has not changed, skipping");
                        continue;
                    }
                    else
                    {
                        state.sentGeometry.Remove(obj.Id);
                    }
                }
                
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
                                Mesh mesh = face.Triangulate(Configuration.triangulationlevelOfDetail);

                                int vertexCount = mesh.Vertices.Count;
                                positions.Capacity = positions.Count + vertexCount;

                                positions.AddRange(mesh.Vertices);
                                AppendNormals(normals, mesh);
                                AppendIndices(indices, mesh, vertexOffset);
                            }

                            Debug.Assert(positions.Count == normals.Count);

                            MeshDataToSend result = ConvertVertices(positions, normals, indices, temporaryMeshIndex);
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

                state.sentGeometry.Add(obj.Id, new SentGeometryObjectData()
                {
                    geometryObjectId = obj.Id,
                    hashCode = obj.GetHashCode()
                });
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
