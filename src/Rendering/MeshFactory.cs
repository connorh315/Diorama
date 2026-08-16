using Diorama.Core.Filetypes.GSC.Components;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Diorama.Rendering
{
    public static class MeshFactory
    {
        private static RenderMesh Icosahedron;

        public static RenderMesh GetIcosahedron()
        {
            if (Icosahedron == null)
            {
                ushort[] indices = new ushort[]
                {
                    0, 1, 8,
                    0, 8, 4,
                    0, 4, 5,
                    0, 5, 10,
                    0, 10, 1,
                    1, 10, 7,
                    1, 7, 6,
                    1, 6, 8,
                    8, 6, 9,
                    8, 9, 4,
                    4, 9, 2,
                    4, 2, 5,
                    5, 2, 11,
                    5, 11, 10,
                    10, 11, 7,
                    3, 6, 7,
                    3, 9, 6,
                    3, 2, 9,
                    3, 11, 2,
                    3, 7, 11
                };

                float phi = 1.618f;

                List<Vertex> vertices = FromPositions(new List<Vector3>
                {
                    new Vector3(0, 1, phi),
                    new Vector3(0, -1, phi),
                    new Vector3(0, 1, -phi),
                    new Vector3(0, -1, -phi),

                    new Vector3(1, phi, 0),
                    new Vector3(-1, phi, 0),
                    new Vector3(1, -phi, 0),
                    new Vector3(-1, -phi, 0),

                    new Vector3(phi, 0, 1),
                    new Vector3(phi, 0, -1),
                    new Vector3(-phi, 0, 1),
                    new Vector3(-phi, 0, -1)
                }, indices);

                

                VertexDefinition[] defs = new VertexDefinition[]
                {
                    new VertexDefinition
                    {
                        Offset = 0,
                        Type = VertexDefinitionStorageEnum.vec3float,
                        Variable = VertexDefinitionVariableEnum.position
                    },
                    new VertexDefinition
                    {
                        Offset = 12,
                        Type = VertexDefinitionStorageEnum.vec3float,
                        Variable = VertexDefinitionVariableEnum.normal
                    }
                };

                var vList = VertexList.FromVertices(vertices, defs);

                RenderVertexBuffer vBuffer = RenderVertexBuffer.FromBuffer(vList);
                vBuffer.Finalise();

                RenderIndicesBuffer iBuffer = RenderIndicesBuffer.FromBuffer(indices);
                iBuffer.Finalise();

                Icosahedron = new RenderMesh(new RenderVertexBuffer[] { vBuffer }, iBuffer);

                Icosahedron.VerticesBase = 0;
                Icosahedron.VerticesCount = vertices.Count;
                Icosahedron.IndicesBase = 0;
                Icosahedron.IndicesCount = indices.Length;
            }

            return Icosahedron;
        }

        private static RenderMesh FullscreenQuad;

        public static RenderMesh GetFullscreenQuad()
        {
            if (FullscreenQuad == null)
            {
                ushort[] indices =
                {
                    0, 1, 2,
                    0, 2, 3,
                };

                List<Vertex> vertices = FromPositions(new List<Vector3>
                {
                    new Vector3(-1.0f, -1.0f, 0.0f),
                    new Vector3( 1.0f, -1.0f, 0.0f),
                    new Vector3( 1.0f,  1.0f, 0.0f),
                    new Vector3(-1.0f,  1.0f, 0.0f),
                }, indices);

                

                VertexDefinition[] defs =
                {
                    new VertexDefinition
                    {
                        Offset = 0,
                        Type = VertexDefinitionStorageEnum.vec3float,
                        Variable = VertexDefinitionVariableEnum.position
                    }
                };

                var vList = VertexList.FromVertices(vertices, defs);

                RenderVertexBuffer vBuffer =
                    RenderVertexBuffer.FromBuffer(vList);
                vBuffer.Finalise();

                RenderIndicesBuffer iBuffer =
                    RenderIndicesBuffer.FromBuffer(indices);
                iBuffer.Finalise();

                FullscreenQuad = new RenderMesh(
                    new RenderVertexBuffer[] { vBuffer },
                    iBuffer);

                FullscreenQuad.VerticesBase = 0;
                FullscreenQuad.VerticesCount = vertices.Count;

                FullscreenQuad.IndicesBase = 0;
                FullscreenQuad.IndicesCount = indices.Length;
            }

            return FullscreenQuad;
        }

        private static List<Vertex> FromPositions(List<Vector3> positions, ushort[] indices)
        {
            var normals = new Vector3[positions.Count];

            for (int i = 0; i < indices.Length; i += 3)
            {
                int i0 = indices[i];
                int i1 = indices[i + 1];
                int i2 = indices[i + 2];

                Vector3 p0 = positions[i0];
                Vector3 p1 = positions[i1];
                Vector3 p2 = positions[i2];

                Vector3 edge1 = p1 - p0;
                Vector3 edge2 = p2 - p0;

                Vector3 faceNormal = Vector3.Cross(edge1, edge2);

                normals[i0] += faceNormal;
                normals[i1] += faceNormal;
                normals[i2] += faceNormal;
            }

            List<Vertex> vertices = new List<Vertex>();

            for (int i = 0; i < positions.Count; i++)
            {
                Vector3 normal = normals[i];

                if (normal.LengthSquared() > 0.000001f)
                    normal = Vector3.Normalize(normal);

                vertices.Add(new Vertex()
                {
                    Position = positions[i],
                    Normal = Vector3.Normalize(positions[i])
                });
            }

            return vertices;
        }
    }
}
