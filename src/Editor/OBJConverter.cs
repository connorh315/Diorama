using Diorama.Core.Filetypes.GSC.Components;
using Diorama.Rendering;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

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
            List<Vector3> colors = new();

            List<Vertex> vertices = new();
            List<ushort> indices = new();

            Dictionary<(int v, int vt, int vn), int> vertexMap = new();

            bool hasVertexColours = false;

            foreach (string line in lines)
            {
                string[] split = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (split[0] == "v")
                {
                    positions.Add(new Vector3(ParseFloat(split[1]), ParseFloat(split[2]), ParseFloat(split[3])));
                    if (split.Length >= 7)
                    {
                        colors.Add(new Vector3(ParseFloat(split[6]), ParseFloat(split[5]), ParseFloat(split[4])));
                        hasVertexColours = true;
                    }
                    else
                    {
                        colors.Add(Vector3.One);
                    }
                }
                else if (split[0] == "vn")
                {
                    normals.Add(new Vector3(ParseFloat(split[1]), ParseFloat(split[2]), ParseFloat(split[3])));
                }
                else if (split[0] == "vt")
                {
                    uvs.Add(new Vector2(ParseFloat(split[1]), ParseFloat(split[2])));
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
                                UVSet01 = vt >= 0 ? new Vector4(uvs[vt], 0, 0) : Vector4.Zero,
                                ColorSet0 = new Vector4(colors[v], 1)
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

            if (!hasVertexColours)
            {
                Console.WriteLine($"Caution: {Path.GetFileName(path)} does not have vertex colours. All vertex colours in the mesh will be set to a default of white");
            }

            return mesh;
        }

        static float ParseFloat(string s)
        {
            if (!float.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out var value))
            {
                s = s.Replace(',', '.');
                value = float.Parse(s, CultureInfo.InvariantCulture);
            }
            return value;
        }

        public static void WriteMeshToOBJ(RenderMesh mesh, string path)
        {
            List<string> lines = new();

            var nuMesh = mesh.OriginalMesh;

            Vertex[] vertices = VertexList.CreateVerticesArray(mesh.VerticesCount);

            for (int vListIdx = 0; vListIdx < nuMesh.VertexBuffers.Length; vListIdx++)
            {
                var vList = nuMesh.VertexBuffers[vListIdx];
                vList.FillVertices(ref vertices, mesh.VerticesBase);
            }

            foreach (var v in vertices)
            {
                lines.Add($"v {v.Position.X} {v.Position.Y} {v.Position.Z} {v.ColorSet0.X} {v.ColorSet0.Y} {v.ColorSet0.Z}");
                lines.Add($"vt {v.UVSet01.X} {v.UVSet01.Y}");
                lines.Add($"vn {v.Normal.X} {v.Normal.Y} {v.Normal.Z}");
            }

            int iStart = (int)nuMesh.IndicesBase;
            int iEnd = (int)(nuMesh.IndicesBase + nuMesh.IndicesCount);

            for (int i = iStart; i < iEnd; i += 3)
            {
                ushort i0 = (ushort)(nuMesh.Indices[i] + 1);
                ushort i1 = (ushort)(nuMesh.Indices[i + 1] + 1);
                ushort i2 = (ushort)(nuMesh.Indices[i + 2] + 1);

                lines.Add($"f {i0}/{i0}/{i0} {i1}/{i1}/{i1} {i2}/{i2}/{i2}");
            }

            File.WriteAllLines(path, lines);
        }
    }
}
