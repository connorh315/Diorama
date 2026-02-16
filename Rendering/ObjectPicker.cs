using Avalonia.OpenGL;
using Diorama.Editor;
using Diorama.Rendering.Shaders;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Rendering
{
    public class ObjectPicker
    {
        private Shader pickingShader;

        private int fbo = -1;
        private int colourTexture = -1;
        private int depthRbo = -1;

        private int width = -1;
        private int height = -1;

        private bool pendingPick = false;
        private int pickX = -1;
        private int pickY = -1;

        public void Initialize()
        {
            pickingShader = new Shader("blendshader.vert", "pickshader.frag");
        }

        public void Resize(int width, int height)
        {
            this.width = width;
            this.height = height;

            if (fbo != -1)
                DeleteFramebuffer();

            CreateFramebuffer();
        }

        private void DeleteFramebuffer()
        {
            if (depthRbo != 0)
                GL.DeleteRenderbuffer(depthRbo);

            if (colourTexture != 0)
                GL.DeleteTexture(colourTexture);

            if (fbo != 0)
                GL.DeleteFramebuffer(fbo);

            depthRbo = 0;
            colourTexture = 0;
            fbo = 0;
        }

        private void CreateFramebuffer()
        {
            fbo = GL.GenFramebuffer();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, fbo);

            colourTexture = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, colourTexture);
            GL.TexImage2D(TextureTarget.Texture2D, 0,
                PixelInternalFormat.Rgb8,
                width, height,
                0,
                PixelFormat.Rgb,
                PixelType.UnsignedByte,
                IntPtr.Zero);

            GL.TexParameter(TextureTarget.Texture2D,
                TextureParameterName.TextureMinFilter,
                (int)TextureMinFilter.Nearest);

            GL.TexParameter(TextureTarget.Texture2D,
                TextureParameterName.TextureMagFilter,
                (int)TextureMagFilter.Nearest);

            GL.FramebufferTexture2D(
                FramebufferTarget.Framebuffer,
                FramebufferAttachment.ColorAttachment0,
                TextureTarget.Texture2D,
                colourTexture,
                0);

            depthRbo = GL.GenRenderbuffer();
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, depthRbo);
            GL.RenderbufferStorage(
                RenderbufferTarget.Renderbuffer,
                RenderbufferStorage.DepthComponent24,
                width, height);

            GL.FramebufferRenderbuffer(
                FramebufferTarget.Framebuffer,
                FramebufferAttachment.DepthAttachment,
                RenderbufferTarget.Renderbuffer,
                depthRbo);

            if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer)
                    != FramebufferErrorCode.FramebufferComplete)
            {
                throw new Exception("Picking FBO incomplete");
            }

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        public void Execute(Camera camera, List<EditorScene> scenes)
        {
            if (!pendingPick)
                return;

            pendingPick = false;

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, fbo);
            GL.Viewport(0, 0, width, height);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.Disable(EnableCap.Blend);

            pickingShader.Use();
            pickingShader.SetMatrix4("projection", camera.Projection);

            var idLookup = new Dictionary<int, EditorSceneObject>();
            int id = 1;

            foreach (var scene in scenes)
            {
                pickingShader.SetMatrix4("view", scene.SceneTransform * camera.GetViewMatrix());

                foreach (var obj in scene.Objects)
                {
                    idLookup[id] = obj;

                    var col = EncodeId(id);
                    pickingShader.SetVector3("color", col);

                    obj.Draw(pickingShader);

                    id++;
                }
            }

            var picked = ReadPixel(idLookup);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

            //GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, fbo);
            //GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, 0);

            //GL.BlitFramebuffer(
            //    0, 0, width, height,
            //    0, 0, width, height,
            //    ClearBufferMask.ColorBufferBit,
            //    BlitFramebufferFilter.Nearest);

            //GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

            ObjectPicked?.Invoke(picked);
            ObjectPicked = null;
        }

        private EditorSceneObject? ReadPixel(Dictionary<int, EditorSceneObject> lookup)
        {
            int readY = height - pickY;

            byte[] pixel = new byte[3];

            GL.ReadPixels(
                pickX,
                readY,
                1,
                1,
                PixelFormat.Rgb,
                PixelType.UnsignedByte,
                pixel);

            int id = DecodeId(pixel[0], pixel[1], pixel[2]);

            if (id != 0 && lookup.TryGetValue(id, out var obj))
                return obj;

            return null;
        }

        private static Vector3 EncodeId(int id)
        {
            return new Vector3(
                (id & 0xFF) / 255f,
                ((id >> 8) & 0xFF) / 255f,
                ((id >> 16) & 0xFF) / 255f);
        }

        private static int DecodeId(byte r, byte g, byte b)
        {
            return r | (g << 8) | (b << 16);
        }

        private event Action<EditorSceneObject?>? ObjectPicked;
        public void RequestPick(int mouseX, int mouseY, Action<EditorSceneObject?> objectPicked)
        {
            pendingPick = true;
            pickX = mouseX;
            pickY = mouseY;
            ObjectPicked = objectPicked;
        }
    }
}
