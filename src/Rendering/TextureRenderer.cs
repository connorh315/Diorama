using Diorama.Rendering.Shaders;
using Diorama.UI.Controls;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Diorama.Rendering
{
    public class TextureRenderer : IRenderer
    {
        public bool ContinuousRendering { get; private set; } = false;

        private RenderTexture ActiveTexture = null;

        public void SetActiveTexture(RenderTexture active)
        {
            ActiveTexture = active;
            ContinuousRendering = active?.Target == TextureTarget.TextureCubeMap;
        }

        private Shader flatShader;
        private Shader cubeShader;

        public void Initialize()
        {
            flatShader = new Shader("trishader.vert", "trishader.frag");
            flatShader.Use();
            flatShader.SetInt("texture0", 0);

            cubeShader = new Shader("cubepreview.vert", "cubepreview.frag");
            cubeShader.Use();
            cubeShader.SetInt("texture0", 0);

            InitializeQuad();
            InitializeCube();
        }

        private readonly float[] quadVertices =
        {
            // Position     UV
            -1, -1,         0, 0,
             1, -1,         1, 0,
             1,  1,         1, 1,

            -1, -1,         0, 0,
             1,  1,         1, 1,
            -1,  1,         0, 1,
        };

        private int _quadVAO;
        private int _quadVBO;

        private void InitializeQuad()
        {
            _quadVAO = GL.GenVertexArray();

            GL.BindVertexArray(_quadVAO);

            _quadVBO = GL.GenBuffer();

            GL.BindBuffer(
                BufferTarget.ArrayBuffer,
                _quadVBO);

            GL.BufferData(
                BufferTarget.ArrayBuffer,
                quadVertices.Length * sizeof(float),
                quadVertices,
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

        private int _cubeVAO;
        private int _cubeVBO;

        private readonly float[] cubeVertices =
        {
            -1, -1, -1,
             1,  1, -1,
             1, -1, -1,
             1,  1, -1,
            -1, -1, -1,
            -1,  1, -1,
            -1, -1,  1,
             1, -1,  1,
             1,  1,  1,
             1,  1,  1,
            -1,  1,  1,
            -1, -1,  1,
            -1,  1,  1,
            -1,  1, -1,
            -1, -1, -1,
            -1, -1, -1,
            -1, -1,  1,
            -1,  1,  1,
             1,  1,  1,
             1, -1, -1,
             1,  1, -1,
             1, -1, -1,
             1,  1,  1,
             1, -1,  1,
            -1, -1, -1,
             1, -1, -1,
             1, -1,  1,
             1, -1,  1,
            -1, -1,  1,
            -1, -1, -1,
            -1,  1, -1,
             1,  1,  1,
             1,  1, -1,
             1,  1,  1,
            -1,  1, -1,
            -1,  1,  1
        };

        private void InitializeCube()
        {
            _cubeVAO = GL.GenVertexArray();
            _cubeVBO = GL.GenBuffer();

            GL.BindVertexArray(_cubeVAO);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _cubeVBO);

            GL.BufferData(
                BufferTarget.ArrayBuffer,
                cubeVertices.Length * sizeof(float),
                cubeVertices,
                BufferUsageHint.StaticDraw);

            GL.EnableVertexAttribArray(0);

            GL.VertexAttribPointer(
                0,
                3,
                VertexAttribPointerType.Float,
                false,
                3 * sizeof(float),
                0);

            GL.BindVertexArray(0);
        }

        public void Render(RenderSurface surface)
        {
            GL.Disable(EnableCap.DepthTest);

            GL.Viewport(0, 0, surface.Host.Width, surface.Host.Height);

            GL.Clear(ClearBufferMask.ColorBufferBit);

            if (ActiveTexture != null)
            {
                if (ActiveTexture.Target == TextureTarget.TextureCubeMap)
                {
                    RenderCube(surface);
                }
                else
                {
                    RenderFlat(surface);
                }
            }
        }

        public void RenderFlat(RenderSurface surface)
        {
            flatShader.Use();

            ActiveTexture.Use();

            GL.BindVertexArray(_quadVAO);

            GL.DrawArrays(
                PrimitiveType.Triangles,
                0,
                6);

            GL.Enable(EnableCap.DepthTest);
        }

        public void RenderCube(RenderSurface surface)
        {
            cubeShader.Use();

            float aspect =
                surface.Host.Width /
                (float)surface.Host.Height;

            Matrix4 projection =
                Matrix4.CreatePerspectiveFieldOfView(
                    MathHelper.DegreesToRadians(90.0f),
                    aspect,
                    0.1f,
                    10.0f);

            float time = (float)Program.TimeSinceStart;

            Matrix4 rotation =
                Matrix4.CreateRotationY(time * 0.2f);

            cubeShader.SetMatrix4("projection", projection);
            cubeShader.SetMatrix4("rotation", rotation);

            ActiveTexture.Use();

            GL.BindVertexArray(_cubeVAO);

            GL.DrawArrays(
                PrimitiveType.Triangles,
                0,
                36);

            GL.BindVertexArray(0);
        }
    }
}
