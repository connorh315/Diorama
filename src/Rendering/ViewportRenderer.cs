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
    public class ViewportRenderer : IRenderer
    {
        public bool ContinuousRendering => true;

        public SceneController Controller;

        public Shader blendShader;

        private ObjectPicker picker;

        public void Initialize()
        {
            GL.ClearColor(0.2f, 0.2f, 0.4f, 1f);

            blendShader = new Shader("blendshader.vert", "blendshader.frag");
            blendShader.SetVector3("color", new Vector3(0.7f, 0.7f, 0.7f));
            blendShader.SetInt("texture0", 0);
            blendShader.SetInt("texture1", 1);
            blendShader.SetInt("texture2", 2);
            blendShader.SetInt("texture3", 3);
            blendShader.SetInt("normal0", 4);

            picker = new ObjectPicker();
            picker.Initialize();

            debugRenderer = new DebugRenderer();
        }

        private Stopwatch stopwatch = Stopwatch.StartNew();

        private int frameCount;

        public int Width, Height;
        public void Render(RenderSurface surface)
        {
            if (Width != surface.Host.Width || Height != surface.Host.Height)
            {
                Width = surface.Host.Width;
                Height = surface.Host.Height;
                picker.Resize(Width, Height);
            }

            GL.Viewport(0, 0, surface.Host.Width, surface.Host.Height);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.Enable(EnableCap.DepthTest);

            Render(Controller.Scenes.ToList(), Controller.Camera);
        }

        private DebugRenderer debugRenderer;

        private void Render(List<EditorScene> scenes, Camera camera)
        {
            blendShader.SetMatrix4("projection", camera.Projection);
            blendShader.SetFloat("lightingEnabled", ViewportNewControl.UseCameraLight ? 1 : 0);

            List<RenderContext> ctxs = new();

            foreach (var scene in scenes)
            {
                var ctx = new RenderContext(scene, camera, blendShader, debugRenderer, Controller.SelectedSceneObject);
                debugRenderer.Reset(scene, camera);
                ctx.Use();
                ctx.IsOpaquePass = true;

                scene.Draw(blendShader, ctx);

                //ctx.Debug.DrawSphere(Vector3.Zero, 10);

                ctxs.Add(ctx);
            }

            GL.Enable(EnableCap.Blend);
            GL.DepthMask(false);

            foreach (var ctx in ctxs)
            {
                ctx.Use();
                ctx.IsOpaquePass = false;

                foreach (var obj in ctx.Transparent)
                {
                    SetBlendMode(obj.Material.BlendMode);
                    obj.Draw(ctx.Shader);
                }
                //Console.WriteLine($"Not Drawn: {ctx.NotDrawn}");
            }

            GL.DepthMask(true);
            GL.Disable(EnableCap.Blend);

            debugRenderer.Render();

            frameCount++;

            if (stopwatch.ElapsedMilliseconds >= 1000)
            {
                Console.WriteLine($"FPS: {frameCount}");

                frameCount = 0;
                stopwatch.Restart();
            }

            picker.Execute(camera, scenes);
        }

        private static void SetBlendMode(EditorBlendMode blendMode)
        {
            switch (blendMode)
            {
                // Opaque
                case EditorBlendMode.Off:
                    GL.Disable(EnableCap.Blend);
                    GL.DepthMask(true);
                    break;

                // Standard alpha
                case EditorBlendMode.Blended:
                    GL.Enable(EnableCap.Blend);
                    GL.BlendEquation(BlendEquationMode.FuncAdd);
                    GL.BlendFunc(
                        BlendingFactor.SrcAlpha,
                        BlendingFactor.OneMinusSrcAlpha);
                    GL.DepthMask(false);
                    break;

                // Glass? Premultiplied?
                case EditorBlendMode.PreMultipliedAlpha:
                    GL.Enable(EnableCap.Blend);
                    GL.BlendEquation(BlendEquationMode.FuncAdd);
                    GL.BlendFunc(
                        BlendingFactor.One,
                        BlendingFactor.OneMinusSrcAlpha);
                    GL.DepthMask(false);
                    break;

                case EditorBlendMode.Additive:
                    GL.Enable(EnableCap.Blend);

                    GL.BlendEquation(BlendEquationMode.FuncAdd);

                    GL.BlendFunc(
                        BlendingFactor.SrcAlpha,
                        BlendingFactor.One);

                    GL.DepthMask(false);
                    break;

                default:

                    GL.Disable(EnableCap.Blend);
                    GL.DepthMask(true);
                    break;
            }
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
