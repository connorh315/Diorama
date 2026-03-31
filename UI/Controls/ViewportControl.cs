using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.OpenGL;
using Avalonia.OpenGL.Controls;
using Avalonia.Platform;
using Avalonia.Rendering;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Diorama.Rendering;
using Diorama.UI.Platform;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.UI.Controls
{
    public partial class ViewportControl : OpenGlControlBase, ICustomHitTest
    {
        private AvaloniaTkContext context;
        public AvaloniaKeyboardState Keyboard = new AvaloniaKeyboardState();

        private SceneController sceneController;
        private Stopwatch stopwatch = Stopwatch.StartNew();
        private double lastTime;

        private bool isRotating = false;

        public ViewportControl()
        {
            Focusable = true;

            sceneController = new SceneController(new ViewportRenderer());
        }

        protected override void OnOpenGlInit(GlInterface gl)
        {
            context = new(gl);
            GL.LoadBindings(context);

            sceneController.Initialize();

            SetFramebufferSize();
        }

        private void Update(double deltaTime)
        {
            if (Keyboard.IsKeyDown(Key.W))
                sceneController.Renderer.Camera.MoveForward((float)deltaTime);

            if (Keyboard.IsKeyDown(Key.A))
                sceneController.Renderer.Camera.MoveLeft((float)deltaTime);

            if (Keyboard.IsKeyDown(Key.S))
                sceneController.Renderer.Camera.MoveBackward((float)deltaTime);

            if (Keyboard.IsKeyDown(Key.D))
                sceneController.Renderer.Camera.MoveRight((float)deltaTime);

            if (Keyboard.IsKeyDown(Key.Space))
                sceneController.Renderer.Camera.MoveUp((float)deltaTime);

            if (Keyboard.IsKeyDown(Key.LeftCtrl))
                sceneController.Renderer.Camera.MoveDown((float)deltaTime);

            sceneController.Renderer.Camera.ToggleSpeed(Keyboard.IsKeyDown(Key.LeftShift));
        }

        protected override void OnOpenGlRender(GlInterface gl, int fb)
        {
            double currentTime = stopwatch.Elapsed.TotalSeconds;
            double deltaTime = currentTime - lastTime;
            lastTime = currentTime;

            Keyboard.OnFrame();

            Update(deltaTime);

            sceneController.Render();

            Dispatcher.UIThread.Post(RequestNextFrameRendering, DispatcherPriority.Background);
        }

        protected override void OnSizeChanged(SizeChangedEventArgs e)
        {
            base.OnSizeChanged(e);

            if (!sceneController.IsInitialized) return;

            SetFramebufferSize();
        }

        private void SetFramebufferSize()
        {
            int fbWidth = (int)(Bounds.Width * this.VisualRoot.RenderScaling);
            int fbHeight = (int)(Bounds.Height * this.VisualRoot.RenderScaling);

            GL.Viewport(0, 0, fbWidth, fbHeight); // TODO: Probably could just be called on-change rather than per-frame?
            sceneController.Renderer.SetFramebufferSize(fbWidth, fbHeight);
        }

        protected override void OnOpenGlDeinit(GlInterface gl)
        {
            base.OnOpenGlDeinit(gl);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (!IsEffectivelyVisible) return;

            Keyboard.SetKey(e.Key, true);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (!IsEffectivelyVisible) return;

            Keyboard.SetKey(e.Key, false);
        }

        public bool HitTest(Point point) => Math.Min(point.X, point.Y) >= 0 && point.X < Bounds.Width && point.Y < Bounds.Height;

        private Vector2 windowCenter;

        private void CalculateScreenCenter()
        {
            var topLevel = TopLevel.GetTopLevel(this);
            var hwnd = topLevel?.TryGetPlatformHandle()?.Handle ?? IntPtr.Zero;

            if (hwnd == IntPtr.Zero)
                return;

            DioramaPlatform.GetClientRect(hwnd, out RECT rect);

            int centerX = (rect.Right - rect.Left) / 2;
            int centerY = (rect.Bottom - rect.Top) / 2;

            POINT centerPoint = new POINT { X = centerX, Y = centerY };
            DioramaPlatform.ClientToScreen(hwnd, ref centerPoint);

            windowCenter = new Vector2(centerPoint.X, centerPoint.Y);
        }

        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            if (e.GetCurrentPoint(this).Properties.IsRightButtonPressed)
            {
                Focus();

                isRotating = true;

                this.Cursor = new Cursor(StandardCursorType.None);

                var topLevel = TopLevel.GetTopLevel(this);

                var hwnd = topLevel?.TryGetPlatformHandle()?.Handle ?? IntPtr.Zero;

                if (hwnd == IntPtr.Zero) return;

                DioramaPlatform.GetClientRect(hwnd, out RECT rect);

                POINT tl = new POINT { X = rect.Left, Y = rect.Top };
                POINT br = new POINT { X = rect.Right, Y = rect.Bottom };

                DioramaPlatform.ClientToScreen(hwnd, ref tl);
                DioramaPlatform.ClientToScreen(hwnd, ref br);

                rect.Left = tl.X;
                rect.Top = tl.Y;
                rect.Right = br.X;
                rect.Bottom = br.Y;

                DioramaPlatform.ClipCursor(ref rect);

                CalculateScreenCenter();

                DioramaPlatform.SetCursorPos((int)windowCenter.X, (int)windowCenter.Y);
            }
        }

        protected override void OnPointerReleased(PointerReleasedEventArgs e)
        {
            if (!e.GetCurrentPoint(this).Properties.IsRightButtonPressed)
            {
                isRotating = false;
                this.Cursor = new Cursor(StandardCursorType.Arrow);

                DioramaPlatform.ClipCursor(IntPtr.Zero);
            }
        }

        protected override void OnPointerMoved(PointerEventArgs e)
        {
            if (!isRotating)
                return;

            // Get current screen position
            POINT current;
            DioramaPlatform.GetCursorPos(out current);

            float deltaX = current.X - windowCenter.X;
            float deltaY = current.Y - windowCenter.Y;

            if (deltaX == 0 && deltaY == 0)
                return; // ignore warp-generated move

            sceneController.Renderer.Camera.ProcessMouse(deltaX, deltaY);

            // Recenter immediately
            DioramaPlatform.SetCursorPos((int)windowCenter.X, (int)windowCenter.Y);
        }
    }
}
