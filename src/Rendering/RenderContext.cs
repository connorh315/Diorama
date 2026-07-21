using Diorama.Editor;
using Diorama.Rendering.Shaders;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Diorama.Rendering
{
    public class RenderContext
    {
        public Vector3 CameraScenePosition;

        public List<EditorGeometryObject> Transparent = new();

        public bool IsOpaquePass = true;

        public EditorScene Scene;

        public Shader Shader;

        public Camera Camera;

        public Matrix4 View;

        public Matrix4 VP;

        public int NotDrawn;

        public DebugRenderer Debug;

        public EditorSceneObject Selected;

        public void Use()
        {
            Shader.SetVector3("camera", CameraScenePosition);
            Shader.SetMatrix4("view", View);
        }

        public FrustumPlane[] FrustumPlanes;

        private static FrustumPlane[] ExtractFrustum(Matrix4 view, Matrix4 projection)
        {
            Matrix4 vp = view * projection;

            Vector4 row1 = new(vp.M11, vp.M21, vp.M31, vp.M41);
            Vector4 row2 = new(vp.M12, vp.M22, vp.M32, vp.M42);
            Vector4 row3 = new(vp.M13, vp.M23, vp.M33, vp.M43);
            Vector4 row4 = new(vp.M14, vp.M24, vp.M34, vp.M44);

            FrustumPlane[] planes = new FrustumPlane[6];

            // Left
            {
                Vector4 p = row4 + row1;
                planes[0] = new FrustumPlane
                {
                    Normal = p.Xyz,
                    Distance = p.W
                };
            }

            // Right
            {
                Vector4 p = row4 - row1;
                planes[1] = new FrustumPlane
                {
                    Normal = p.Xyz,
                    Distance = p.W
                };
            }

            // Bottom
            {
                Vector4 p = row4 + row2;
                planes[2] = new FrustumPlane
                {
                    Normal = p.Xyz,
                    Distance = p.W
                };
            }

            // Top
            {
                Vector4 p = row4 - row2;
                planes[3] = new FrustumPlane
                {
                    Normal = p.Xyz,
                    Distance = p.W
                };
            }

            // Near
            {
                Vector4 p = row4 + row3;
                planes[4] = new FrustumPlane
                {
                    Normal = p.Xyz,
                    Distance = p.W
                };
            }

            // Far
            {
                Vector4 p = row4 - row3;
                planes[5] = new FrustumPlane
                {
                    Normal = p.Xyz,
                    Distance = p.W
                };
            }

            for (int i = 0; i < planes.Length; i++)
                planes[i].Normalize();

            return planes;
        }

        private void SetupPlanes()
        {
            FrustumPlanes = ExtractFrustum(View, Camera.Projection);

            var p = FrustumPlanes[4];
        }

        public bool Intersects(Vector3 center, float radius)
        {
            int i = 0;
            foreach (var plane in FrustumPlanes)
            {
                float d = Vector3.Dot(plane.Normal, center) + plane.Distance;

                if (i > 2)
                    continue;

                if (d < -radius)
                {
                    Console.WriteLine($"Rejected by plane {i}: {d}");
                    return false;
                }
                i++;
            }

            return true;
        }

        public bool Intersects(Vector3 center, Vector3 extents)
        {
            for (int i = 0; i < FrustumPlanes.Length; i++)
            {
                var plane = FrustumPlanes[i];

                float r =
                    extents.X * MathF.Abs(plane.Normal.X) +
                    extents.Y * MathF.Abs(plane.Normal.Y) +
                    extents.Z * MathF.Abs(plane.Normal.Z);

                float s =
                    Vector3.Dot(plane.Normal, center) + plane.Distance;

                if (s + r < 0)
                {
                    return false;
                }
            }

            return true;
        }

        public RenderContext(EditorScene scene, Camera camera, Shader blendShader, DebugRenderer debug, EditorSceneObject selected)
        {
            Vector3 cameraScenePos = (scene.SceneTransform * new Vector4(camera.Position, 1)).Xyz;
            CameraScenePosition = cameraScenePos;
            Shader = blendShader;
            Camera = camera;
            Scene = scene;
            View = Scene.SceneTransform * Camera.GetViewMatrix();
            Debug = debug;
            Selected = selected;

            SetupPlanes();
        }
    }

    public struct FrustumPlane
    {
        public Vector3 Normal;
        public float Distance;

        public void Normalize()
        {
            float len = Normal.Length;
            Normal /= len;
            Distance /= len;
        }
    }
}
