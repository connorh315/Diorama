using Diorama.Rendering;
using Diorama.Rendering.Shaders;
using Diorama.UI.Controls;
using OpenTK.Graphics.OpenGL4;
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
        public EditorSceneObject Parent { get; set; }
        
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

        public void Draw(Shader shader)
        {
            shader.SetMatrix4("model", Transform);

            shader.SetVector4("mesh_color", Material.Colour1);

            Material.Diffuse0?.Use();
            Material.Diffuse1?.Use(TextureUnit.Texture1);

            if (Lightmap != null && Lightmap.AmbientOcclusion != null && ViewportNewControl.ShowLightmaps && Material.LightmapUVSet != -1)
            {
                Lightmap.AmbientOcclusion.Use(TextureUnit.Texture2);
                shader.SetVector2("lm_offset", new Vector2(Lightmap.Offsets[0], Lightmap.Offsets[1]));
                shader.SetVector2("lm_scale", new Vector2(Lightmap.Scales[0], Lightmap.Scales[1]));
                shader.SetInt("lightmap_uvset", Material.LightmapUVSet);
            }
            else
            {
                RenderTexture.GetWhiteTexture().Use(TextureUnit.Texture2);
            }

            Mesh.Draw();
        }
    }
}
