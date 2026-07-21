using Diorama.Rendering.Shaders;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace Diorama.Rendering
{
    public sealed class DebugSphere
    {
        private readonly int _vao;
        private readonly int _vbo;
        private readonly int _ebo;
        private readonly int _indexCount;

        public DebugSphere(int subdivisions = 2)
        {
            float t = (1.0f + MathF.Sqrt(5.0f)) * 0.5f;

            List<Vector3> vertices =
            [
                new(-1,  t, 0),
                new( 1,  t, 0),
                new(-1, -t, 0),
                new( 1, -t, 0),

                new(0, -1,  t),
                new(0,  1,  t),
                new(0, -1, -t),
                new(0,  1, -t),

                new( t, 0, -1),
                new( t, 0,  1),
                new(-t, 0, -1),
                new(-t, 0,  1)
            ];

            for (int i = 0; i < vertices.Count; i++)
                vertices[i] = Vector3.Normalize(vertices[i]);

            List<uint> indices =
            [
                 0,11,5,
                 0,5,1,
                 0,1,7,
                 0,7,10,
                 0,10,11,

                 1,5,9,
                 5,11,4,
                 11,10,2,
                 10,7,6,
                 7,1,8,

                 3,9,4,
                 3,4,2,
                 3,2,6,
                 3,6,8,
                 3,8,9,

                 4,9,5,
                 2,4,11,
                 6,2,10,
                 8,6,7,
                 9,8,1
            ];

            Dictionary<(uint, uint), uint> midpointCache = new();

            uint GetMidpoint(uint a, uint b)
            {
                var key = a < b ? (a, b) : (b, a);

                if (midpointCache.TryGetValue(key, out uint index))
                    return index;

                Vector3 mid = Vector3.Normalize((vertices[(int)a] + vertices[(int)b]) * 0.5f);

                vertices.Add(mid);

                index = (uint)(vertices.Count - 1);

                midpointCache[key] = index;

                return index;
            }

            for (int s = 0; s < subdivisions; s++)
            {
                midpointCache.Clear();

                List<uint> newIndices = new();

                for (int i = 0; i < indices.Count; i += 3)
                {
                    uint a = indices[i];
                    uint b = indices[i + 1];
                    uint c = indices[i + 2];

                    uint ab = GetMidpoint(a, b);
                    uint bc = GetMidpoint(b, c);
                    uint ca = GetMidpoint(c, a);

                    newIndices.AddRange([
                        a, ab, ca,
                        b, bc, ab,
                        c, ca, bc,
                        ab, bc, ca
                    ]);
                }

                indices = newIndices;
            }

            _vao = GL.GenVertexArray();
            _vbo = GL.GenBuffer();
            _ebo = GL.GenBuffer();

            GL.BindVertexArray(_vao);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferData(
                BufferTarget.ArrayBuffer,
                vertices.Count * Vector3.SizeInBytes,
                vertices.ToArray(),
                BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
            GL.BufferData(
                BufferTarget.ElementArrayBuffer,
                indices.Count * sizeof(uint),
                indices.ToArray(),
                BufferUsageHint.StaticDraw);

            GL.EnableVertexAttribArray(0);

            GL.VertexAttribPointer(
                0,
                3,
                VertexAttribPointerType.Float,
                false,
                Vector3.SizeInBytes,
                0);

            GL.BindVertexArray(0);

            _indexCount = indices.Count;
        }

        public void Draw()
        {
            GL.BindVertexArray(_vao);
            GL.DrawElements(
                PrimitiveType.Triangles,
                _indexCount,
                DrawElementsType.UnsignedInt,
                0);
        }
    }
}
