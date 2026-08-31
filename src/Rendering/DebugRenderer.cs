using Diorama.Editor;
using Diorama.Rendering.Shaders;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Diorama.Rendering
{
    public class DebugRenderer
    {
        private Shader debugShader;
        private DebugSphere sphere;
        private DebugLine line;

        public DebugRenderer()
        {
            debugShader = new Shader("debugshader.vert", "debugshader.frag");
            sphere = new DebugSphere();
            line = new DebugLine();
        }

        public List<(Vector3, float)> SphereDraws = new List<(Vector3, float)>();

        public List<(Vector3, Vector3)> CapsuleDraws = new List<(Vector3, Vector3)>();

        public List<(Vector3, Vector3)> LineDraws = new();

        public void Reset(EditorScene scene, Camera camera)
        {
            SphereDraws.Clear();
            CapsuleDraws.Clear();
            LineDraws.Clear();

            debugShader.SetMatrix4("view", scene.SceneTransform * camera.GetViewMatrix());
            debugShader.SetMatrix4("projection", camera.Projection);
        }

        public void DrawSphere(Vector3 center, float radius)
        {
            SphereDraws.Add((center, radius));
        }

        public void DrawCapsule(Vector3 center, Vector3 extents)
        {
            CapsuleDraws.Add((center, extents));
        }

        public void DrawLine(Vector3 center, Vector3 direction)
        {
            LineDraws.Add((center, direction));
        }

        public void Render()
        {
            debugShader.Use();
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            foreach ((Vector3 center, float radius) in SphereDraws)
            {
                Matrix4 model =
                    Matrix4.CreateScale(radius) *
                    Matrix4.CreateTranslation(center);
                debugShader.SetMatrix4("model", model);
                debugShader.SetVector4("Color", Vector4.One);
                sphere.Draw();
            }

            foreach ((Vector3 center, Vector3 extents) in CapsuleDraws)
            {
                Matrix4 model =
                    Matrix4.CreateScale(extents) *
                    Matrix4.CreateTranslation(center);
                debugShader.SetMatrix4("model", model);
                debugShader.SetVector4("Color", Vector4.One);
                sphere.Draw();
            }

            foreach ((Vector3 center, Vector3 direction) in LineDraws)
            {
                var model = line.GetModelMatrix(center, direction);
                debugShader.SetMatrix4("model", model);
                debugShader.SetVector4("Color", new Vector4(1f, 0.2f, 0f, 1f));
                line.Draw();
            }
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
        }
    }
}
