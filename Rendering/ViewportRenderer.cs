using Diorama.Core.Filetypes.GSC;
using Diorama.Core.Filetypes.GSC.Components;
using Diorama.Core.Filetypes.TEXTURES;
using Diorama.Editor;
using Diorama.Extensions;
using Diorama.Rendering.Shaders;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Rendering
{
    public class ViewportRenderer : IDioramaRenderer
    {
        public Shader blendShader;

        public Camera Camera { get; private set; }

        public void Initialize()
        {
            GL.ClearColor(0.2f, 0.2f, 0.4f, 1f);
            GL.Enable(EnableCap.DepthTest);

            //GL.Enable(EnableCap.Blend);
            //GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            //GL.Enable(EnableCap.CullFace);
            //GL.CullFace(CullFaceMode.Back);

            blendShader = new Shader("blendshader.vert", "blendshader.frag");
            blendShader.SetVector3("color", new Vector3(0.7f, 0.7f, 0.7f));
            blendShader.SetInt("texture0", 0);
            blendShader.SetInt("texture1", 1);

            Camera = new Camera(new Vector3(0.0f, 0.0f, 3.0f));
        }

        private Stopwatch stopwatch = Stopwatch.StartNew();

        public void Render(List<EditorScene> scenes)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            blendShader.SetMatrix4("projection", Camera.Projection);
            foreach (var scene in scenes)
            {
                blendShader.SetMatrix4("view", scene.SceneTransform * Camera.GetViewMatrix());
                scene.Draw(blendShader);
            }
        }

        public int Width, Height;
        public void SetFramebufferSize(int width, int height)
        {
            Width = width; 
            Height = height;

            Camera.SetWidthHeight(width, height);
        }

        public void Deinitialize()
        {
            blendShader.Dispose();
        }
    }
}
