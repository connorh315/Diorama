using Diorama.Core.Filetypes.GSC.Components;
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
    public class EditorGeometryObject : IHierarchySelectable
    {
        public string Name => "Geometry Object";
        public IEnumerable<IHierarchySelectable> Children => Enumerable.Empty<IHierarchySelectable>();

        public EditorClipObject Parent { get; set; }
        
        public EditorMaterial Material { get; set; }
        public EditorLightmap Lightmap;
        
        public RenderMesh Mesh { get; set; }
        public NuTransformMtx OriginalTransform { get; set; }

        private Matrix4 transform;
        public Matrix4 Transform
        {
            get => transform;
            set
            {
                transform = value;
                position = Transform.ExtractTranslation();
                rotation = Transform.ExtractRotation();
                eulerRotation = rotation.ToEulerAngles();
                scale = Transform.ExtractScale();
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


        private Quaternion rotation;
        private Vector3 eulerRotation;
        
        public Vector3 Rotation
        {
            get => eulerRotation;
            set
            {
                eulerRotation = value;
                rotation = Quaternion.FromEulerAngles(eulerRotation); // stupid thing
                TransformChanged();
            }
        }

        private Vector3 scale;
        public Vector3 Scale
        {
            get => scale;
            set
            {
                scale = value;
                TransformChanged();
            }
        }

        private void TransformChanged()
        {
            var translation = Matrix4.CreateTranslation(Position);
            Matrix4.CreateFromQuaternion(in rotation, out Matrix4 rot);
            var scale = Matrix4.CreateScale(Scale);

            transform = scale * rot * translation;
            OriginalTransform.Update(transform);
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
