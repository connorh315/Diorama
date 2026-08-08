using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.VisualTree;
using Diorama.Rendering;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace Diorama.UI.Controls
{
    public class GlHost : NativeControlHost
    {
        private IntPtr _hwnd;
        private IntPtr _hdc;
        private IntPtr _originalWndProc;
        private Win32.WndProcDelegate? _wndProcDelegate;

        public IntPtr Hwnd => _hwnd;
        public IntPtr Hdc => _hdc;

        protected readonly RenderService renderService;
        protected readonly IRenderer renderer;

        protected RenderSurface surface;

        protected GlHost(IRenderer renderer)
        {
            renderService = RenderService.Current;
            this.renderer = renderer;
        }

        protected virtual void OnPressLeftClick() { }
        protected virtual void OnReleaseLeftClick() { }

        protected virtual void OnPressRightClick() { }
        protected virtual void OnReleaseRightClick() { }

        protected virtual void OnMouseMove() { }

        public virtual void Update() { }

        public new int Width, Height;
        protected override void OnSizeChanged(SizeChangedEventArgs e)
        {
            Width = ScaleCoordinate((int)Bounds.Width);
            Height = ScaleCoordinate((int)Bounds.Height);
        }

        private int ScaleCoordinate(int coord)
        {
            return (int)(coord * this.VisualRoot.RenderScaling);
        }

        protected override IPlatformHandle CreateNativeControlCore(IPlatformHandle parent)
        {
            _hwnd = Win32.CreateChildWindow(parent.Handle, Cursor?.ToString());

            SubclassChildWindow();
            _hdc = Win32.GetDC(_hwnd);
            Win32.SetPixelFormat(_hdc);

            surface = new RenderSurface
            {
                Host = this,
                Renderer = renderer
            };

            renderService.Register(surface);

            return new PlatformHandle(_hwnd, "HWND");
        }

        public bool HostRequestsRedraw { get; set; } = false;

        public bool HostNeedsClipping { get; set; } = false;
        private ScrollViewer scrollViewer;
        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);

            scrollViewer = this.FindAncestorOfType<ScrollViewer>();

            if (scrollViewer != null)
            {
                //scrollViewer.PropertyChanged += ScrollViewer_PropertyChanged;
                //HostNeedsClipping = true;
            }
        }

        public Rect VisibleRect;

        private void ScrollViewer_PropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
        {
            if (e.Property == ScrollViewer.OffsetProperty)
            {
                var root = (Visual)VisualRoot;

                var controlRect = new Rect(Bounds.Size)
                    .TransformToAABB(this.TransformToVisual(root)!.Value);

                var viewportRect = new Rect(scrollViewer.Bounds.Size)
                    .TransformToAABB(scrollViewer.TransformToVisual(root)!.Value);

                var visible = controlRect.Intersect(viewportRect);

                var localVisible = new Rect(
                    visible.X - controlRect.X,
                    visible.Y - controlRect.Y,
                    visible.Width,
                    visible.Height);

                float scale = (float)1;

                VisibleRect = new Rect(
                    localVisible.X * scale,
                    localVisible.Y * scale,
                    localVisible.Width * scale,
                    localVisible.Height * scale);

                HostRequestsRedraw = true;
            }
        }

        protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnDetachedFromVisualTree(e);

            Console.WriteLine("Debug: Destroying native control as no longer attached to visual tree.");

            if (_hdc != IntPtr.Zero)
            {
                Win32.ReleaseDC(_hwnd, _hdc);
                _hdc = IntPtr.Zero;
            }

            if (_hwnd != IntPtr.Zero)
            {
                Win32.DestroyWindow(_hwnd);
                _hwnd = IntPtr.Zero;
            }
        }

        private void SubclassChildWindow()
        {
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

            const int WM_ERASEBKGND = 0x0014;

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

                case WM_ERASEBKGND:
                    surface.IsDirty = true;
                    return 1;

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
            Win32.DestroyWindow(_hwnd);
        }
    }
}
