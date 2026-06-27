using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Text;

namespace Diorama.Rendering
{
    public sealed class SimpleRenderer : IRenderer
    {
        public bool ContinuousRendering => true;

        private bool _initialized;

        public void Initialize()
        {
            if (_initialized)
                return;

            GL.ClearColor(0.2f, 0.3f, 0.6f, 1.0f);

            _initialized = true;
        }

        public void Render(RenderSurface surface)
        {
            GL.Viewport(
                0,
                0,
                surface.Host.Width,
                surface.Host.Height);

            GL.Clear(
                ClearBufferMask.ColorBufferBit);
        }
    }
}
