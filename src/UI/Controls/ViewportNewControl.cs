using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Rendering;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Diorama.Core.Filetypes.GSC;
using Diorama.Core.Filetypes.TEXTURES;
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

        public ViewportNewControl(SceneController controller) : base(controller.Renderer)
        {
            Focusable = true;

            sceneController = controller;
        }

        public void LoadScene(string path)
        {
            renderService.Enqueue(() =>
            {
                sceneController.AddScene(path);
            });
        }

        public void LoadScene(RawFile scene, RawFile textures, RawFile cubemap_textures, string scenePath)
        {
            GScene gscene = GScene.Parse(scene);

            gscene.Path = scenePath;

            NxgTextures nxg_textures = NxgTextures.Read(textures);
            NxgTextures nxg_cubemap_textures = null;
            if (cubemap_textures != null && cubemap_textures.fileStream.Length > 0)
            {
                nxg_cubemap_textures = NxgTextures.Read(cubemap_textures);
            }

            renderService.Enqueue(() =>
            {
                sceneController.AddScene(gscene, nxg_textures, nxg_cubemap_textures);
            });
        }

        //protected override void Initialize()
        //{
        //    sceneController.Initialize();

        //    SetFramebufferSize();
        //}

        private int ScaleCoordinate(int coord)
        {
            return (int)(coord * this.VisualRoot.RenderScaling);
        }

        private void SetFramebufferSize()
        {
            int fbWidth = ScaleCoordinate((int)Bounds.Width);
            int fbHeight = ScaleCoordinate((int)Bounds.Height);

            renderService.Enqueue(() =>
            {
                sceneController.SetWidthHeight(fbWidth, fbHeight);
            });
        }

        public static bool ShowLightmaps = false;

        public static bool UseCameraLight = false;

        public static bool ShowShadowImpostors = false;

        public static bool UseFrustumCulling = false;

        private void Update(double deltaTime)
        {
            if (Keyboard.IsKeyDown(Key.F))
            {
                var geo = sceneController.SelectedGeometry;
                sceneController.CameraController.FocusOn(geo);
            }

            sceneController.CameraController.Update(Keyboard, deltaTime);
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

        double bWidth, bHeight;
        protected override void OnSizeChanged(SizeChangedEventArgs e)
        {
            base.OnSizeChanged(e);

            SetFramebufferSize();
        }

        public override void Update()
        {
            double currentTime = stopwatch.Elapsed.TotalSeconds;
            double deltaTime = currentTime - lastTime;
            lastTime = currentTime;

            Keyboard.OnFrame();

            Update(deltaTime);
        }

        //protected override void Render()
        //{

        //    sceneController.Render();
        //}

        protected override void OnPressLeftClick()
        {
            POINT current;
            DioramaPlatform.GetCursorPos(out current);
            var point = this.PointToClient(new PixelPoint(current.X, current.Y));

            int x = ScaleCoordinate((int)point.X);
            int y = ScaleCoordinate((int)point.Y);

            renderService.Enqueue(() =>
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

            sceneController.CameraController.ProcessMouse(deltaX, deltaY);

            // Recenter immediately
            DioramaPlatform.SetCursorPos((int)windowCenter.X, (int)windowCenter.Y);
        }
    }
}
