using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Diorama.Rendering
{
    public sealed class DebugLine
    {
        private readonly int _vao;
        private readonly int _vbo;

        public DebugLine()
        {
            Vector3[] vertices =
            [
                Vector3.Zero,
            Vector3.UnitZ
            ];

            _vao = GL.GenVertexArray();
            _vbo = GL.GenBuffer();

            GL.BindVertexArray(_vao);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferData(
                BufferTarget.ArrayBuffer,
                vertices.Length * Vector3.SizeInBytes,
                vertices,
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
        }

        public Matrix4 GetModelMatrix(Vector3 start, Vector3 direction)
        {
            float length = direction.Length;

            if (length <= 0.000001f)
                return Matrix4.Identity;

            Vector3 dir = direction / length;

            Vector3 axis = Vector3.Cross(Vector3.UnitZ, dir);
            float dot = Math.Clamp(Vector3.Dot(Vector3.UnitZ, dir), -1.0f, 1.0f);

            Matrix4 rotation;

            if (axis.LengthSquared > 0.000001f)
            {
                axis = Vector3.Normalize(axis);
                float angle = MathF.Acos(dot);

                rotation = Matrix4.CreateFromAxisAngle(axis, angle);
            }
            else
            {
                // Parallel or opposite to +Z.
                rotation = dot >= 0
                    ? Matrix4.Identity
                    : Matrix4.CreateRotationX(MathF.PI);
            }

            return Matrix4.CreateScale(1, 1, length)
                 * rotation
                 * Matrix4.CreateTranslation(start);
        }

        public void Draw()
        {
            GL.BindVertexArray(_vao);
            GL.DrawArrays(PrimitiveType.Lines, 0, 2);
        }
    }
}
