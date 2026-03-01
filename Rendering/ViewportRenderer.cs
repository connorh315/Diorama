using Diorama.Core.Filetypes.GSC;
using Diorama.Core.Filetypes.GSC.Components;
using Diorama.Core.Filetypes.TEXTURES;
using Diorama.Editor;
using Diorama.Extensions;
using Diorama.Rendering.Shaders;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using SkiaSharp;
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

        private ObjectPicker picker;

        public void Initialize()
        {
            GL.ClearColor(0.2f, 0.2f, 0.4f, 1f);
            GL.Enable(EnableCap.DepthTest);

            blendShader = new Shader("blendshader.vert", "blendshader.frag");
            blendShader.SetVector3("color", new Vector3(0.7f, 0.7f, 0.7f));
            blendShader.SetInt("texture0", 0);
            blendShader.SetInt("texture1", 1);
            blendShader.SetInt("texture2", 2);

            picker = new ObjectPicker();
            picker.Initialize();

            Camera = new Camera(new Vector3(0.0f, 0.0f, 3.0f));
        }

        private Stopwatch stopwatch = Stopwatch.StartNew();

        private int frameCount;

        public void Render(List<EditorScene> scenes)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            blendShader.SetMatrix4("projection", Camera.Projection);
            foreach (var scene in scenes)
            {
                blendShader.SetMatrix4("view", scene.SceneTransform * Camera.GetViewMatrix());
                //scene.DebugDraw(blendShader, Camera);
                scene.Draw(blendShader);
            }

            frameCount++;

            if (stopwatch.ElapsedMilliseconds >= 1000)
            {
                //Console.WriteLine($"FPS: {frameCount}");

                frameCount = 0;
                stopwatch.Restart();
            }

            picker.Execute(Camera, scenes);
        }

        public int Width, Height;
        public void SetFramebufferSize(int width, int height)
        {
            Width = width; 
            Height = height;

            Camera.SetWidthHeight(width, height);
            picker.Resize(width, height);
        }

        public void Pick(int x, int y, Action<EditorGeometryObject?>? objectPicked)
        {
            picker.RequestPick(x, y, objectPicked);
        }

        public void Deinitialize()
        {
            blendShader.Dispose();
        }
    }
}
