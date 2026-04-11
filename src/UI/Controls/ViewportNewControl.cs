using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
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
    public class ViewportNewControl : GlHost
    {
        private AvaloniaTkContext context;
        public AvaloniaKeyboardState Keyboard = new AvaloniaKeyboardState();

        private SceneController sceneController;
        private Stopwatch stopwatch = Stopwatch.StartNew();
        private double lastTime;

        private bool isRotating = false;

        public ViewportNewControl(SceneController controller)
        {
            Focusable = true;

            sceneController = controller;
        }

        public void LoadScene(string path)
        {
            sceneController.EnqueueGL(() =>
            {
                sceneController.AddScene(path);
            });
        }

        protected override void Initialize()
        {
            sceneController.Initialize();

            SetFramebufferSize();
        }

        private int ScaleCoordinate(int coord)
        {
            return (int)(coord * this.VisualRoot.RenderScaling);
        }

        private void SetFramebufferSize()
        {
            int fbWidth = ScaleCoordinate((int)Bounds.Width);
            int fbHeight = ScaleCoordinate((int)Bounds.Height);

            sceneController.EnqueueGL(() =>
            {
                GL.Viewport(0, 0, fbWidth, fbHeight);
                sceneController.Renderer.SetFramebufferSize(fbWidth, fbHeight);
            });
        }

        public static bool ShowLightmaps = false;

        public static bool UseCameraLight = false;

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

            //ShowLightmaps = !Keyboard.IsKeyDown(Key.L);

            sceneController.Renderer.Camera.ToggleSpeed(Keyboard.IsKeyDown(Key.LeftShift));
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

        protected override void OnSizeChanged(SizeChangedEventArgs e)
        {
            base.OnSizeChanged(e);

            if (!sceneController.IsInitialized) return;

            SetFramebufferSize();
        }

        protected override void Render()
        {
            double currentTime = stopwatch.Elapsed.TotalSeconds;
            double deltaTime = currentTime - lastTime;
            lastTime = currentTime;

            Keyboard.OnFrame();

            Update(deltaTime);

            sceneController.Render();
        }

        protected override void OnPressLeftClick()
        {
            POINT current;
            DioramaPlatform.GetCursorPos(out current);
            var point = this.PointToClient(new PixelPoint(current.X, current.Y));

            int x = ScaleCoordinate((int)point.X);
            int y = ScaleCoordinate((int)point.Y);

            sceneController.EnqueueGL(() =>
            {
                sceneController.OnClick(x, y);
            });
        }

        private Vector2 windowCenter;

        protected override void OnPressRightClick()
        {
            Focus();

            isRotating = true;

            DioramaPlatform.ShowCursor(false);

            // Top-left of control in screen space
            var topLeft = this.PointToScreen(new Point(0, 0));

            // Bottom-right of control in screen space
            var bottomRight = this.PointToScreen(new Point(Bounds.Width, Bounds.Height));

            RECT rect = new RECT
            {
                Left = (int)topLeft.X,
                Top = (int)topLeft.Y,
                Right = (int)bottomRight.X,
                Bottom = (int)bottomRight.Y
            };

            DioramaPlatform.ClipCursor(ref rect);

            // Center of control
            windowCenter = new Vector2(
                (topLeft.X + bottomRight.X) / 2,
                (topLeft.Y + bottomRight.Y) / 2
            );

            DioramaPlatform.SetCursorPos(
                (int)windowCenter.X,
                (int)windowCenter.Y);
        }

        protected override void OnReleaseRightClick()
        {
            ((Control)this.VisualRoot).Focus();

            Keyboard.Clear();

            isRotating = false;
            DioramaPlatform.ShowCursor(true);

            DioramaPlatform.ClipCursor(IntPtr.Zero);
        }

        protected override void OnMouseMove()
        {
            if (!isRotating)
                return;

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
