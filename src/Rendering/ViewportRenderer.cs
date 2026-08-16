using Diorama.Core.Filetypes.GSC;
using Diorama.Core.Filetypes.GSC.Components;
using Diorama.Core.Filetypes.TEXTURES;
using Diorama.Editor;
using Diorama.Editor.Material;
using Diorama.Extensions;
using Diorama.Rendering.Shaders;
using Diorama.UI.Controls;
using Diorama.UI.ViewModels;
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

        public Shader gizmoShader;

        public Shader maskShader;

        public Shader outlineShader;

        private ObjectPicker picker;

        public void Initialize()
        {
            blendShader = new Shader("blendshader.vert", "blendshader.frag");
            gizmoShader = new Shader("3dgizmoshader.vert", "3dgizmoshader.frag");
            maskShader = new Shader("maskshader.vert", "maskshader.frag");
            outlineShader = new Shader("outlineshader.vert", "outlineshader.frag");
            blendShader.SetVector3("color", new Vector3(0.7f, 0.7f, 0.7f));
            blendShader.SetInt("diffuse0tex", 0);
            blendShader.SetInt("diffuse1tex", 1);
            blendShader.SetInt("diffuse2tex", 2);
            blendShader.SetInt("diffuse3tex", 3);
            blendShader.SetInt("normal0", 4);
            blendShader.SetInt("specular0", 5);
            blendShader.SetInt("texture2", 15);
            blendShader.SetInt("texture3", 16);
            blendShader.SetInt("scene_envmap_tex", 17);

            picker = new ObjectPicker();
            picker.Initialize();

            debugRenderer = new DebugRenderer();

            MeshFactory.GetFullscreenQuad();
            MeshFactory.GetIcosahedron();
        }

        private Stopwatch stopwatch = Stopwatch.StartNew();
        private long lastElapsedMilliseconds;

        private int frameCount;

        public int Width, Height;
        public void Render(RenderSurface surface)
        {
            if (Width != surface.Host.Width || Height != surface.Host.Height)
            {
                Width = surface.Host.Width;
                Height = surface.Host.Height;
                picker.Resize(Width, Height);
                CreateOutlineFramebuffer(Width, Height);
            }

            GL.Viewport(0, 0, surface.Host.Width, surface.Host.Height);

            GL.ClearColor(0.2f, 0.2f, 0.4f, 1f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.Enable(EnableCap.DepthTest);

            Render(Controller.Scenes.ToList(), Controller.Camera);
        }

        private void RenderSelectionMask(
            Camera camera,
            List<RenderContext> ctxs)
        {
            GL.BindFramebuffer(
                FramebufferTarget.Framebuffer,
                outlineFbo);

            GL.ClearColor(0, 0, 0, 0);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            // This is what makes it visible through walls.
            GL.Disable(EnableCap.DepthTest);
            GL.DepthMask(false);

            maskShader.Use();
            maskShader.SetMatrix4("projection", camera.Projection);

            foreach (var ctx in ctxs)
            {
                if (ctx.HasSelectedObject)
                {
                    maskShader.SetMatrix4("view", ctx.View);

                    ((IRenderable)Controller.SelectedHierarchyObject).Draw(maskShader, ctx);
                }
            }

            GL.DepthMask(true);
            GL.Enable(EnableCap.DepthTest);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        private void RenderSelectionOutline(int width, int height)
        {
            // Scene framebuffer should be bound here.

            GL.Disable(EnableCap.DepthTest);
            GL.DepthMask(false);

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(
                BlendingFactor.SrcAlpha,
                BlendingFactor.OneMinusSrcAlpha);

            outlineShader.Use();

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, outlineMask);

            outlineShader.SetInt("maskTexture", 0);

            outlineShader.SetVector2(
                "texelSize",
                new Vector2(
                    1.0f / width,
                    1.0f / height));

            outlineShader.SetFloat("outlineWidth", 1.0f);

            MeshFactory.GetFullscreenQuad().Draw();

            GL.Disable(EnableCap.Blend);

            GL.DepthMask(true);
            GL.Enable(EnableCap.DepthTest);
        }

        private DebugRenderer debugRenderer;

        private void Render(List<EditorScene> scenes, Camera camera)
        {
            blendShader.SetMatrix4("projection", camera.Projection);
            gizmoShader.SetMatrix4("projection", camera.Projection);
            blendShader.SetBool("lightingEnabled", ViewportNewControl.UseCameraLight);

            blendShader.SetBool("debug_color0", RenderOptions.Color0);
            blendShader.SetBool("debug_color0r", RenderOptions.Color0R);
            blendShader.SetBool("debug_color0g", RenderOptions.Color0G);
            blendShader.SetBool("debug_color0b", RenderOptions.Color0B);
            blendShader.SetBool("debug_color0a", RenderOptions.Color0A);
            blendShader.SetBool("debug_color1", RenderOptions.Color1);
            blendShader.SetBool("debug_color1r", RenderOptions.Color1R);
            blendShader.SetBool("debug_color1g", RenderOptions.Color1G);
            blendShader.SetBool("debug_color1b", RenderOptions.Color1B);
            blendShader.SetBool("debug_color1a", RenderOptions.Color1A);

            blendShader.SetBool("debug_showSpecular", RenderOptions.ShowSpecular);
            blendShader.SetBool("debug_showEnvMap", RenderOptions.ShowOnlyEnvMap);

            blendShader.SetFloat("time", (stopwatch.ElapsedMilliseconds) / 1000f);
            lastElapsedMilliseconds = stopwatch.ElapsedMilliseconds;

            List<RenderContext> ctxs = new();

            var cameraPos = camera.Position;
            var cameraVM = camera.GetViewMatrix();

            foreach (var scene in scenes)
            {
                var ctx = new RenderContext(scene, camera, cameraPos, cameraVM, blendShader, debugRenderer, Controller.SelectedSceneObject);
                debugRenderer.Reset(scene, camera);
                ctx.Use();

                scene.AddRenderables(ctx);

                foreach (var obj in ctx.Opaque)
                {
                    obj.Draw(blendShader, ctx);

                    if (obj == Controller.SelectedHierarchyObject)
                        ctx.HasSelectedObject = true;
                }

                ctxs.Add(ctx);
            }

            GL.Enable(EnableCap.Blend);
            GL.DepthMask(false);

            foreach (var ctx in ctxs)
            {
                ctx.Use();

                foreach (EditorGeometryObject obj in ctx.Transparent)
                {
                    SetBlendMode(obj.Material.BlendMode);
                    obj.Draw(ctx.Shader);
                    if (obj == Controller.SelectedHierarchyObject)
                        ctx.HasSelectedObject = true;
                }
                //Console.WriteLine($"Not Drawn: {ctx.NotDrawn}");
            }

            GL.DepthMask(true);
            GL.Disable(EnableCap.Blend);

            GL.Clear(ClearBufferMask.DepthBufferBit);

            gizmoShader.SetVector4("Color", new Vector4(0.3f, 0.3f, 1, 1));
            foreach (var ctx in ctxs)
            {
                gizmoShader.SetVector3("cameraPos", ctx.CameraScenePosition);
                gizmoShader.SetMatrix4("view", ctx.View);

                foreach (var obj in ctx.Gizmos)
                {
                    obj.Draw(gizmoShader, ctx);
                    if (obj == Controller.SelectedHierarchyObject)
                        ctx.HasSelectedObject = true;
                }
            }

            if (Controller.SelectedHierarchyObject != null)
            {
                RenderSelectionMask(camera, ctxs);

                RenderSelectionOutline(Width, Height);
            }

            picker.Execute(camera, ctxs);

            GL.BindVertexArray(0);
        }

        int outlineFbo;
        int outlineMask;

        void CreateOutlineFramebuffer(int width, int height)
        {
            outlineFbo = GL.GenFramebuffer();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, outlineFbo);

            outlineMask = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, outlineMask);

            GL.TexImage2D(
                TextureTarget.Texture2D,
                0,
                PixelInternalFormat.R8,
                width,
                height,
                0,
                PixelFormat.Red,
                PixelType.UnsignedByte,
                IntPtr.Zero);

            GL.TexParameter(
                TextureTarget.Texture2D,
                TextureParameterName.TextureMinFilter,
                (int)TextureMinFilter.Nearest);

            GL.TexParameter(
                TextureTarget.Texture2D,
                TextureParameterName.TextureMagFilter,
                (int)TextureMagFilter.Nearest);

            GL.TexParameter(
                TextureTarget.Texture2D,
                TextureParameterName.TextureWrapS,
                (int)TextureWrapMode.ClampToBorder);

            GL.TexParameter(
                TextureTarget.Texture2D,
                TextureParameterName.TextureWrapT,
                (int)TextureWrapMode.ClampToBorder);

            float[] borderColor = { 0f, 0f, 0f, 0f };

            GL.TexParameter(
                TextureTarget.Texture2D,
                TextureParameterName.TextureBorderColor,
                borderColor);

            GL.FramebufferTexture2D(
                FramebufferTarget.Framebuffer,
                FramebufferAttachment.ColorAttachment0,
                TextureTarget.Texture2D,
                outlineMask,
                0);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
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

        public void Pick(int x, int y, Action<IHierarchySelectable?>? objectPicked)
        {
            picker.RequestPick(x, y, objectPicked);
        }

        public void Deinitialize()
        {
            blendShader.Dispose();
        }
    }
}
