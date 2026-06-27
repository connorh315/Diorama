using Diorama.UI.Controls;
using OpenTK.Graphics.OpenGL4;
using SkiaSharp;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Diorama.Rendering
{
    public class RenderService : IDisposable
    {
        public static RenderService Current { get; private set; }

        public RenderService()
        {
            if (Current != null)
                throw new InvalidOperationException("RenderService is a singleton and has already been created.");

            Current = this;
        }

        private readonly List<RenderSurface> surfaces = new();
        private Thread? thread;
        private bool running;
        private IntPtr context;
        
        public void Register(RenderSurface surface)
        {
            if (context == IntPtr.Zero)
            {
                RegisterFirst(surface);
            }
            else
            {
                Enqueue(() =>
                {
                    surfaces.Add(surface);
                });
            }

        }

        private void RegisterFirst(RenderSurface surface)
        {
            surfaces.Add(surface);
            StartRenderThread();
        }

        public void Unregister(RenderSurface surface)
        {
            Enqueue(() =>
            {
                lock (surfaces)
                    surfaces.Remove(surface);
            });
        }

        private void CreateContext(GlHost host)
        {
            var hdc = host.Hdc;

            // Create temporary legacy context
            var tempContext = Win32.wglCreateContext(hdc);
            Win32.wglMakeCurrent(hdc, tempContext);

            // Load wgl extensions
            var wglCreateContextAttribsARB =
                Win32.wglGetProcAddressDelegate<Win32.wglCreateContextAttribsARBProc>("wglCreateContextAttribsARB");

            // Create OpenGL 4.6 core context
            int[] attribs =
            {
                0x2091, 4, // WGL_CONTEXT_MAJOR_VERSION_ARB
                0x2092, 6, // WGL_CONTEXT_MINOR_VERSION_ARB
                0x9126, 0x00000001, // WGL_CONTEXT_PROFILE_MASK_ARB → CORE
                0
            };

            context = wglCreateContextAttribsARB(hdc, IntPtr.Zero, attribs);

            Win32.wglMakeCurrent(IntPtr.Zero, IntPtr.Zero);
            Win32.wglDeleteContext(tempContext);

            Win32.wglMakeCurrent(hdc, context);
            GL.LoadBindings(new WglBindingsContext());
            Win32.wglMakeCurrent(IntPtr.Zero, IntPtr.Zero);
        }

        private void StartRenderThread()
        {
            running = true;
            thread = new Thread(RenderLoop);
            thread.IsBackground = true;
            thread.Start();
        }

        int counter = 0;
        private void RenderLoop()
        {
            CreateContext(surfaces[0].Host);

            while (running)
            {
                while (_glQueue.TryDequeue(out var action))
                    action();

                foreach (var surface in surfaces)
                {
                    if (!surface.Renderer.ContinuousRendering && !surface.IsDirty)
                        continue;

                    Render(surface);

                    surface.IsDirty = false;
                }
            }
        }

        private void Render(RenderSurface surface)
        {
            Win32.wglMakeCurrent(surface.Host.Hdc, context);

            if (!surface.Initialized)
            {
                surface.Renderer.Initialize();
                surface.Initialized = true;
            }

            surface.Host.Update();
            surface.Renderer.Render(surface);

            Win32.SwapBuffers(surface.Host.Hdc);
        }

        private readonly ConcurrentQueue<Action> _glQueue = new();

        public void Enqueue(Action action)
        {
            _glQueue.Enqueue(action);
        }

        public void Start()
        {

        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
