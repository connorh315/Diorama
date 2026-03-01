using Diorama.Rendering;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Editor
{
    public class EditorGeometryObject
    {
        public EditorMaterial Material;
        public EditorLightmap Lightmap;
        public RenderMesh Mesh { get; set; }


        private Matrix4 transform;
        public Matrix4 Transform
        {
            get => transform;
            set
            {
                transform = value;
                position = Transform.ExtractTranslation();
                Rotation = Transform.ExtractRotation();
                Scale = Transform.ExtractScale();
            }
        }

        private Vector3 position;
        public Vector3 Position
        {
            get => position;
            set
            {
                position = value;
                TransformChanged();
            }
        }

        public Quaternion Rotation { get; set; }
        public Vector3 Scale { get; set; }

        private void TransformChanged()
        {
            var translation = Matrix4.CreateTranslation(Position);
            var quat = Rotation;
            var rotation = Matrix4.CreateFromQuaternion(quat);
            var scale = Matrix4.CreateScale(Scale);

            Transform = scale * rotation * translation;
        }
    }
}
