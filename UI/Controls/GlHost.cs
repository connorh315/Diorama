using System;
using System.Runtime.InteropServices;
using System.Threading;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Desktop;
using Avalonia.Media;
using System.Collections.Concurrent;

namespace Diorama.UI.Controls
{
    public class GlHost : NativeControlHost
    {
        private IntPtr _hwnd;
        private Thread? _renderThread;
        private bool _running;


        private IntPtr _originalWndProc;
        private Win32.WndProcDelegate? _wndProcDelegate;

        protected virtual void Initialize() { }

        protected virtual void Render() { }

        protected virtual void OnPressLeftClick() { }
        protected virtual void OnReleaseLeftClick() { }

        protected virtual void OnPressRightClick() { }
        protected virtual void OnReleaseRightClick() { }

        protected virtual void OnMouseMove() { }


        protected override IPlatformHandle CreateNativeControlCore(IPlatformHandle parent)
        {
            _hwnd = Win32.CreateChildWindow(parent.Handle);

            SubclassChildWindow();

            StartRenderThread();

            return new PlatformHandle(_hwnd, "HWND");
        }

        private void SubclassChildWindow()
        {
            Console.WriteLine(_hwnd);

            _wndProcDelegate = CustomWndProc;

            _originalWndProc = Win32.SetWindowLongPtr(
                _hwnd,
                Win32.GWL_WNDPROC,
                Marshal.GetFunctionPointerForDelegate(_wndProcDelegate));
        }

        private IntPtr CustomWndProc(
            IntPtr hWnd,
            int msg,
            IntPtr wParam,
            IntPtr lParam)
        {
            // Mouse movement
            const int WM_MOUSEMOVE = 0x0200;

            // Left mouse button
            const int WM_LBUTTONDOWN = 0x0201;
            const int WM_LBUTTONUP = 0x0202;
            const int WM_LBUTTONDBLCLK = 0x0203;

            // Right mouse button
            const int WM_RBUTTONDOWN = 0x0204;
            const int WM_RBUTTONUP = 0x0205;
            const int WM_RBUTTONDBLCLK = 0x0206;

            // Middle mouse button
            const int WM_MBUTTONDOWN = 0x0207;
            const int WM_MBUTTONUP = 0x0208;
            const int WM_MBUTTONDBLCLK = 0x0209;

            // Mouse wheel (vertical)
            const int WM_MOUSEWHEEL = 0x020A;

            // Mouse wheel (horizontal)
            const int WM_MOUSEHWHEEL = 0x020E;

            // Mouse leave (if tracking enabled)
            const int WM_MOUSELEAVE = 0x02A3;

            switch (msg)
            {
                case WM_MOUSEMOVE:
                    OnMouseMove();
                    break;

                case WM_LBUTTONDOWN:
                    OnPressLeftClick();
                    break;

                case WM_LBUTTONUP:
                    OnReleaseLeftClick();
                    break;

                case WM_RBUTTONDOWN:
                    OnPressRightClick();
                    break;

                case WM_RBUTTONUP:
                    OnReleaseRightClick();
                    break;

                case WM_MOUSEWHEEL:
                    break;

                case 0x84: // hit test
                    return 1;
            }

            return Win32.CallWindowProc(
                _originalWndProc,
                hWnd,
                msg,
                wParam,
                lParam);
        }

        protected override void DestroyNativeControlCore(IPlatformHandle control)
        {
            _running = false;
            _renderThread?.Join();
            Win32.DestroyWindow(_hwnd);
        }

        private void StartRenderThread()
        {
            _running = true;

            _renderThread = new Thread(() =>
            {
                var hdc = Win32.GetDC(_hwnd);
                Win32.SetPixelFormat(hdc);

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

                var glContext = wglCreateContextAttribsARB(hdc, IntPtr.Zero, attribs);

                Win32.wglMakeCurrent(IntPtr.Zero, IntPtr.Zero);
                Win32.wglDeleteContext(tempContext);

                Win32.wglMakeCurrent(hdc, glContext);

                // Load OpenTK bindings
                GL.LoadBindings(new WglBindingsContext());

                Initialize();

                RenderLoop(hdc, glContext);

                Win32.wglMakeCurrent(IntPtr.Zero, IntPtr.Zero);
                Win32.wglDeleteContext(glContext);
                Win32.ReleaseDC(_hwnd, hdc);

            });

            _renderThread.IsBackground = true;
            _renderThread.Start();
        }

        private void RenderLoop(IntPtr hdc, IntPtr context)
        {
            while (_running)
            {
                Render();

                Win32.SwapBuffers(hdc);
            }
        }
    }
}
