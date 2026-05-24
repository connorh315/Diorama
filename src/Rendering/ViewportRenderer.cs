using Diorama.Core.Filetypes.GSC;
using Diorama.Core.Filetypes.GSC.Components;
using Diorama.Core.Filetypes.TEXTURES;
using Diorama.Editor;
using Diorama.Extensions;
using Diorama.Rendering.Shaders;
using Diorama.UI.Controls;
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
            blendShader.SetInt("texture3", 3);

            picker = new ObjectPicker();
            picker.Initialize();
        }

        private Stopwatch stopwatch = Stopwatch.StartNew();

        private int frameCount;

        public void Render(List<EditorScene> scenes, Camera camera)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            var ctx = new RenderContext();

            blendShader.SetMatrix4("projection", camera.Projection);
            blendShader.SetFloat("lightingEnabled", ViewportNewControl.UseCameraLight ? 1 : 0);
            foreach (var scene in scenes)
            {
                Vector3 cameraScenePos = (scene.SceneTransform * new Vector4(camera.Position, 1)).Xyz;
                ctx.CameraScenePosition = cameraScenePos;
                blendShader.SetVector3("camera", cameraScenePos);
                blendShader.SetMatrix4("view", scene.SceneTransform * camera.GetViewMatrix());
                //scene.DebugDraw(blendShader, Camera);
                scene.Draw(blendShader, ctx);
            }

            frameCount++;

            if (stopwatch.ElapsedMilliseconds >= 1000)
            {
                //Console.WriteLine($"FPS: {frameCount}");

                frameCount = 0;
                stopwatch.Restart();
            }

            picker.Execute(camera, scenes);
        }

        public int Width, Height;
        public void SetFramebufferSize(int width, int height)
        {
            Width = width; 
            Height = height;

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
