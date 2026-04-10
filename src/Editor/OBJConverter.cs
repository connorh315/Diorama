using Diorama.Core;
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
                        var i0 = (ushort)faceIndices[0];
                        var i1 = (ushort)faceIndices[i];
                        var i2 = (ushort)faceIndices[i + 1];

                        indices.Add(i0);
                        indices.Add(i1);
                        indices.Add(i2);

                        var v0 = vertices[i0];
                        var v1 = vertices[i1];
                        var v2 = vertices[i2];

                        Vector3 edge1 = v1.Position - v0.Position;
                        Vector3 edge2 = v2.Position - v0.Position;

                        Vector2 deltaUV1 = (v1.UVSet01 - v0.UVSet01).ToVector2();
                        Vector2 deltaUV2 = (v2.UVSet01 - v0.UVSet01).ToVector2();

                        float area = (deltaUV1.X * deltaUV2.Y - deltaUV2.X * deltaUV1.Y);
                        if (Math.Abs(area) < 1e-6f)
                            continue;

                        float f = 1f / area;

                        Vector3 tangent = new Vector3(
                            f * (deltaUV2.Y * edge1.X - deltaUV1.Y * edge2.X),
                            f * (deltaUV2.Y * edge1.Y - deltaUV1.Y * edge2.Y),
                            f * (deltaUV2.Y * edge1.Z - deltaUV1.Y * edge2.Z));

                        vertices[i0].Tangent += tangent;
                        vertices[i0].Tangent += tangent;
                        vertices[i0].Tangent += tangent;
                    }
                }
            }

            for (int i = 0; i < vertices.Count; i++)
            {
                var n = vertices[i].Normal;
                var t = vertices[i].Tangent;

                t = t - n * Vector3.Dot(n, t);

                t = Vector3.Normalize(t);

                vertices[i].Tangent = t;
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

            for (int i = 0; i < vertexLists.Length; i++)
            { // fixes a vertex explosion
                nuMesh.VertexBufferFlags[i] = 0x502;
                nuMesh.VertexBufferOffsets[i] = 0;
            }

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
                //lines.Add($"vn {v.Tangent.X} {v.Tangent.Y} {v.Tangent.Z}");
            }

            int iStart = (int)nuMesh.IndicesBase;
            int iEnd = (int)(nuMesh.IndicesBase + nuMesh.IndicesCount);

            Vector3[] tangents = new Vector3[vertices.Length];

            List<string> faces = new();

            for (int i = iStart; i < iEnd; i += 3)
            {
                ushort i0 = (ushort)(nuMesh.Indices[i] + 1);
                ushort i1 = (ushort)(nuMesh.Indices[i + 1] + 1);
                ushort i2 = (ushort)(nuMesh.Indices[i + 2] + 1);

                faces.Add($"f {i0}/{i0}/{i0} {i1}/{i1}/{i1} {i2}/{i2}/{i2}");
                
                var v0 = vertices[i0 - 1];
                var v1 = vertices[i1 - 1];
                var v2 = vertices[i2 - 1];

                Vector3 edge1 = v1.Position - v0.Position;
                Vector3 edge2 = v2.Position - v0.Position;

                Vector2 deltaUV1 = (v1.UVSet01 - v0.UVSet01).ToVector2();
                Vector2 deltaUV2 = (v2.UVSet01 - v0.UVSet01).ToVector2();

                float denom = (deltaUV1.X * deltaUV2.Y - deltaUV2.X * deltaUV1.Y);
                if (Math.Abs(denom) < 1e-6f)
                    continue;

                float f = 1f / denom;

                Vector3 tangent = new Vector3(
                    f * (deltaUV2.Y * edge1.X - deltaUV1.Y * edge2.X),
                    f * (deltaUV2.Y * edge1.Y - deltaUV1.Y * edge2.Y),
                    f * (deltaUV2.Y * edge1.Z - deltaUV1.Y * edge2.Z));

                tangents[i0 - 1] += tangent;
                tangents[i1 - 1] += tangent;
                tangents[i2 - 1] += tangent;
            }

            for (int i = 0; i < tangents.Length; i++)
            {
                var n = vertices[i].Normal;
                var t = tangents[i];

                t = t - n * Vector3.Dot(n, t);

                t = Vector3.Normalize(t);

                vertices[i].Tangent = t;

                //lines.Add($"vn {t.X} {t.Y} {t.Z}");
            }

            //foreach (string line in faces)
            //{
            //    lines.Add(line);
            //}

            File.WriteAllLines(path, lines);
        }
    }
}
