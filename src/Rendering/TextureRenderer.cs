using Diorama.Rendering.Shaders;
using Diorama.UI.Controls;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Text;

namespace Diorama.Rendering
{
    public class TextureRenderer : IRenderer
    {
        public bool ContinuousRendering => false;

        public RenderTexture ActiveTexture = null;

        private Shader shader;

        public void Initialize()
        {
            shader = new Shader("trishader.vert", "trishader.frag");
            shader.Use();
            shader.SetInt("texture0", 0);

            _vao = GL.GenVertexArray();

            GL.BindVertexArray(_vao);

            _vbo = GL.GenBuffer();

            GL.BindBuffer(
                BufferTarget.ArrayBuffer,
                _vbo);

            GL.BufferData(
                BufferTarget.ArrayBuffer,
                vertices.Length * sizeof(float),
                vertices,
                BufferUsageHint.StaticDraw);

            GL.EnableVertexAttribArray(0);

            GL.VertexAttribPointer(
                0,
                2,
                VertexAttribPointerType.Float,
                false,
                4 * sizeof(float),
                0);

            GL.EnableVertexAttribArray(1);

            GL.VertexAttribPointer(
                1,
                2,
                VertexAttribPointerType.Float,
                false,
                4 * sizeof(float),
                2 * sizeof(float));
        }

        float[] vertices =
        {
            // Position     UV
            -1, -1,         0, 0,
             1, -1,         1, 0,
             1,  1,         1, 1,

            -1, -1,         0, 0,
             1,  1,         1, 1,
            -1,  1,         0, 1,
        };

        private int _vao;
        private int _vbo;

        public void Render(RenderSurface surface)
        {
            GL.Disable(EnableCap.DepthTest);

            GL.Viewport(0, 0, surface.Host.Width, surface.Host.Height);

            GL.Clear(ClearBufferMask.ColorBufferBit);

            if (ActiveTexture != null)
            {
                shader.Use();

                ActiveTexture.Use();

                GL.BindVertexArray(_vao);

                GL.DrawArrays(
                    PrimitiveType.Triangles,
                    0,
                    6);

                GL.Enable(EnableCap.DepthTest);
            }
        }
    }
}
