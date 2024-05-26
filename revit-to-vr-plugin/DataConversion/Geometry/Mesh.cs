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
        public static MeshDataToSend ConvertTemporaryMesh(Mesh mesh)
        {
            return ConvertMesh(mesh, Configuration.temporaryMeshIndex);
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
                    id = IsMeshIdTemporary(id) ? Configuration.temporaryMeshIndex : id,
                    temporaryId = Guid.NewGuid()
                },
                vertexCount = vertexCount,
                indexCount = indexCount
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
    }
}
