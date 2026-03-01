using Avalonia.Controls;
using Diorama.Rendering;
using Diorama.Rendering.Shaders;
using Diorama.UI.Controls;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Editor
{
    public class EditorSceneObject : INotifyPropertyChanged
    {
        public string Name { get; set; }
        private Matrix4 Transform;

        public EditorMaterial Material;
        public EditorLightmap Lightmap;
        public RenderMesh Mesh { get; set; }

        public EditorSceneObject[] Lods { get; set; }
        public float[] FadeDistances { get; set; }

        public Vector4 BoundsCenterAndDistSqrd { get; set; }

        public bool IsActive = true;

        public bool DebugDraw = false;

        public void SetTransform(Matrix4 m)
        {
            Transform = m;
            position = Transform.ExtractTranslation();
            Rotation = Transform.ExtractRotation();
            Scale = Transform.ExtractScale();
        }

        private Vector3 position;
        public Vector3 Position { 
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

        public event PropertyChangedEventHandler? PropertyChanged;

        public void Draw(Shader shader)
        {
            shader.SetMatrix4("model", Transform);

            if (!IsActive)
                return;

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

        public void Debug_Draw(Shader shader, Camera camera)
        {
            if (!Name.Contains("LOD")) return;

            shader.SetMatrix4("model", Transform);

            if (Vector3.DistanceSquared(BoundsCenterAndDistSqrd.Xyz, camera.Position) > BoundsCenterAndDistSqrd.W)
            {
                shader.SetVector4("mesh_color", new Vector4(1, 0, 0, 1));
                RenderTexture.GetWhiteTexture().Use(TextureUnit.Texture0);
                RenderTexture.GetWhiteTexture().Use(TextureUnit.Texture1);
            }
            else
            {
                shader.SetVector4("mesh_color", Material.Colour1);
                Material.Diffuse0?.Use();
                Material.Diffuse1?.Use(TextureUnit.Texture1);
            }

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
