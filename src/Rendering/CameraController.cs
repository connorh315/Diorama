using Diorama.UI;
using System;
using System.Collections.Generic;
using System.Text;
using Avalonia.Input;
using System.Runtime.InteropServices;
using OpenTK.Mathematics;
using Diorama.Editor;

namespace Diorama.Rendering
{
    public class CameraController
    {
        public Camera Camera;

        private const float BaseSpeed = 3.0f;
        private const float FastSpeed = 20.0f;

        private const float Sensitivity = 0.1f;

        private const float FocusDuration = 0.3f;

        public CameraController(Camera camera)
        {
            Camera = camera;
        }

        bool isFocusing = false;
        float t = 0;
        Vector3 startPos, targetPos;
        float startYaw, targetYaw;
        float startPitch, targetPitch;

        public void Update(AvaloniaKeyboardState input, double deltaTime)
        {
            float dt = (float)deltaTime;

            if (isFocusing)
            {
                t += dt;
                float alpha = Math.Clamp(t / FocusDuration, 0f, 1f);

                Camera.Position = Vector3.Lerp(startPos, targetPos, alpha);
                Camera.Yaw = LerpAngle(startYaw, targetYaw, alpha);
                Camera.Pitch = MathHelper.Lerp(startPitch, targetPitch, alpha);

                if (alpha >= 1f)
                    isFocusing = false;
            }
            else
            {
                float speed = input.IsKeyDown(Key.LeftShift) ? FastSpeed : BaseSpeed;

                Vector3 front = Camera.GetFront();

                if (input.IsKeyDown(Key.W))
                    Camera.Position += front * speed * dt;

                if (input.IsKeyDown(Key.A))
                    Camera.Position -= Vector3.Normalize(Vector3.Cross(front, Camera.Up)) * speed * dt;

                if (input.IsKeyDown(Key.S))
                    Camera.Position -= front * speed * dt;

                if (input.IsKeyDown(Key.D))
                    Camera.Position += Vector3.Normalize(Vector3.Cross(front, Camera.Up)) * speed * dt;

                if (input.IsKeyDown(Key.Space))
                    Camera.Position += Camera.Up * speed * dt;

                if (input.IsKeyDown(Key.LeftCtrl))
                    Camera.Position -= Camera.Up * speed * dt;
            }
        }

        float LerpAngle(float a, float b, float t)
        {
            float delta = (b - a + 180f) % 360f - 180f;
            return a + delta * t;
        }

        public void ProcessMouse(float deltaX, float deltaY)
        {
            deltaX *= Sensitivity;
            deltaY *= Sensitivity;

            Camera.Yaw += deltaX;
            Camera.Pitch -= deltaY; // invert Y

            // Clamp pitch
            Camera.Pitch = Math.Clamp(Camera.Pitch, -89f, 89f);
        }

        /// <summary>
        /// Assumes a DX-coordinate-space object!
        /// </summary>
        /// <param name="obj"></param>
        public void FocusOn(EditorGeometryObject obj)
        {
            startPos = Camera.Position;
            startYaw = Camera.Yaw;
            startPitch = Camera.Pitch;

            Vector3 target = EditorUtils.FlipCoordSpace(obj.Parent.Parent.BoundsCenterAndDistSqrd.Xyz);

            float approxSize = obj.Parent.Parent.ApproxSize;

            float maxScale = MathF.Max(obj.Scale.X,
                  MathF.Max(obj.Scale.Y, obj.Scale.Z));

            float size = approxSize * maxScale;

            size = MathF.Min(size, 500); // stops the camera from disappearing into the oblivion

            targetPos = target + (startPos - target).Normalized() * size;

            //targetPos = (new Vector4(target, 1)).Xyz - Camera.GetFront() * 5;

            (targetYaw, targetPitch) = CalculateLookAt(startPos, target);

            t = 0;
            isFocusing = true;
        }

        (float yaw, float pitch) CalculateLookAt(Vector3 from, Vector3 to)
        {
            Vector3 direction = (to - from).Normalized();

            float pitch = MathF.Asin(direction.Y);

            float yaw = MathF.Atan2(direction.Z, direction.X);

            // Convert to degrees
            pitch = MathHelper.RadiansToDegrees(pitch);
            yaw = MathHelper.RadiansToDegrees(yaw);

            return (yaw, pitch);
        }
    }
}
