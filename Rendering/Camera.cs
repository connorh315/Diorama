using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK.Mathematics;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Rendering
{
    public class Camera
    {
        public Vector3 Position;
        public Vector3 Front = new Vector3(0.0f, 0.0f, -1.0f);
        public Vector3 Up = new Vector3(0.0f, 1.0f, 0.0f);

        public float Yaw = -90f;
        public float Pitch = 0f;
        public float Sensitivity = 0.1f;

        public float Speed = 3.0f;

        public Camera(Vector3 startPosition)
        {
            Position = startPosition;
        }

        public void MoveForward(float deltaTime)
            => Position += Front * Speed * deltaTime;

        public void MoveBackward(float deltaTime)
            => Position -= Front * Speed * deltaTime;

        public void MoveLeft(float deltaTime)
            => Position -= Vector3.Normalize(Vector3.Cross(Front, Up)) * Speed * deltaTime;

        public void MoveRight(float deltaTime)
            => Position += Vector3.Normalize(Vector3.Cross(Front, Up)) * Speed * deltaTime;

        public void MoveUp(float deltaTime)
            => Position += Up * Speed * deltaTime;

        public void MoveDown(float deltaTime)
            => Position -= Up * Speed * deltaTime;

        public void ProcessMouse(float deltaX, float deltaY)
        {
            deltaX *= Sensitivity;
            deltaY *= Sensitivity;

            Yaw += deltaX;
            Pitch -= deltaY; // invert Y

            // Clamp pitch
            Pitch = Math.Clamp(Pitch, -89f, 89f);

            Vector3 front;
            front.X = MathF.Cos(MathHelper.DegreesToRadians(Yaw)) *
                      MathF.Cos(MathHelper.DegreesToRadians(Pitch));

            front.Y = MathF.Sin(MathHelper.DegreesToRadians(Pitch));

            front.Z = MathF.Sin(MathHelper.DegreesToRadians(Yaw)) *
                      MathF.Cos(MathHelper.DegreesToRadians(Pitch));

            Front = Vector3.Normalize(front);
        }

        public Matrix4 GetViewMatrix() => Matrix4.LookAt(Position, Position + Front, Up);
    }
}
