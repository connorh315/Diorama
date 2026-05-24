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
        public Vector3 Up = Vector3.UnitY;
        //public Vector3 Front = new Vector3(0.0f, 0.0f, -1.0f);

        public float Yaw = -90f;
        public float Pitch = 0f;

        public Matrix4 Projection;

        public Camera(Vector3 startPosition)
        {
            Position = startPosition;
        }

        public void SetProjection(int width, int height)
        {
            Projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45.0f), (float)width / height, 0.1f, 1000f);
        }

        public Vector3 GetFront()
        {
            Vector3 front;
            front.X = MathF.Cos(MathHelper.DegreesToRadians(Yaw)) *
                      MathF.Cos(MathHelper.DegreesToRadians(Pitch));

            front.Y = MathF.Sin(MathHelper.DegreesToRadians(Pitch));

            front.Z = MathF.Sin(MathHelper.DegreesToRadians(Yaw)) *
                      MathF.Cos(MathHelper.DegreesToRadians(Pitch));

            return front.Normalized();
        }

        public Matrix4 GetViewMatrix() => Matrix4.LookAt(Position, Position + GetFront(), Up);
    }
}
