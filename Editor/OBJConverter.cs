using Diorama.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Diorama.Core.Filetypes.GSC.Components;
using System.Numerics;

namespace Diorama.Editor
{
    public static class OBJConverter
    {
        static int ParseIndex(string s, int count)
        {
            int i = int.Parse(s);
            return i > 0 ? i - 1 : count + i;
        }

        public static RenderMesh MeshFromOBJ(string path, RenderMesh originalMesh)
        {
            string[] lines = File.ReadAllLines(path);

            List<Vector3> positions = new();
            List<Vector3> normals = new();
            List<Vector2> uvs = new();

            List<Vertex> vertices = new();
            List<ushort> indices = new();

            Dictionary<(int v, int vt, int vn), int> vertexMap = new();

            foreach (string line in lines)
            {
                string[] split = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (split[0] == "v")
                {
                    positions.Add(new Vector3(float.Parse(split[1]), float.Parse(split[2]), float.Parse(split[3])));
                }
                else if (split[0] == "vn")
                {
                    normals.Add(new Vector3(float.Parse(split[1]), float.Parse(split[2]), float.Parse(split[3])));
                }
                else if (split[0] == "vt")
                {
                    uvs.Add(new Vector2(float.Parse(split[1]), float.Parse(split[2])));
                }
                else if (split[0] == "f")
                {
                    List<int> faceIndices = new();

                    for (int i = 1; i < split.Length; i++)
                    {
                        var parts = split[i].Split('/');

                        int v = ParseIndex(parts[0], positions.Count);

                        int vt = parts.Length > 1 && parts[1] != ""
                            ? ParseIndex(parts[1], uvs.Count)
                            : -1;

                        int vn = parts.Length > 2
                            ? ParseIndex(parts[2], normals.Count)
                            : -1;

                        var key = (v, vt, vn);

                        if (!vertexMap.TryGetValue(key, out int index))
                        {
                            var vertex = new Vertex
                            {
                                Position = positions[v],
                                Normal = vn >= 0 ? normals[vn] : Vector3.Zero,
                                UVSet01 = vt >= 0 ? new Vector4(uvs[vt], 0, 0) : Vector4.Zero
                            };

                            index = vertices.Count;
                            vertices.Add(vertex);
                            vertexMap[key] = index;
                        }

                        faceIndices.Add(index);
                    }

                    // triangulate (stupid format)
                    for (int i = 1; i < faceIndices.Count - 1; i++)
                    {
                        indices.Add((ushort)faceIndices[0]);
                        indices.Add((ushort)faceIndices[i]);
                        indices.Add((ushort)faceIndices[i + 1]);
                    }
                }
            }

            VertexList[] vertexLists = new VertexList[originalMesh.VertexBuffers.Length];
            RenderVertexBuffer[] vertexBuffers = new RenderVertexBuffer[vertexLists.Length];
            for (int i = 0; i < vertexLists.Length; i++)
            {
                vertexLists[i] = VertexList.FromVertices(vertices, originalMesh.VertexBuffers[i].Attributes);
                vertexBuffers[i] = RenderVertexBuffer.FromBuffer(vertexLists[i]);
            }

            RenderIndicesBuffer indicesBuffer = RenderIndicesBuffer.FromBuffer(indices.ToArray());

            RenderMesh mesh = new RenderMesh(vertexBuffers, indicesBuffer);
            mesh.IndicesBase = 0;
            mesh.IndicesCount = indices.Count;
            mesh.VerticesBase = 0;
            mesh.VerticesCount = vertices.Count;

            var nuMesh = originalMesh.OriginalMesh;
            nuMesh.VertexBuffers = vertexLists;
            nuMesh.Indices = indices.ToArray();
            nuMesh.IndicesBase = 0;
            nuMesh.IndicesCount = (uint)indices.Count;
            nuMesh.VerticesBase = 0;
            nuMesh.VerticesCount = (uint)vertices.Count;

            mesh.OriginalMesh = nuMesh;

            return mesh;
        }
    }
}
