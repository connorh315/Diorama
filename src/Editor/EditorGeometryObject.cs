using Diorama.Core.Filetypes.GSC.Components;
using Diorama.Rendering;
using Diorama.Rendering.Shaders;
using Diorama.UI.Controls;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Editor
{
    public class EditorGeometryObject : IHierarchySelectable, INotifyPropertyChanged
    {
        public string Name => "Geometry Object";
        public IEnumerable<IHierarchySelectable> Children => Enumerable.Empty<IHierarchySelectable>();

        public EditorClipObject Parent { get; set; }

        public NuClipItem Original;

        private EditorMaterial? _material;
        public EditorMaterial? Material
        {
            get => _material;
            set
            {
                if (_material == value || value == null) // value == null is safeguard for silly avalonia behaviour
                    return;

                _material = value;
                if (Original != null)
                {
                    Original.OldMaterialIndex = value.OriginalIndex;
                    Original.MaterialIndex = (short)value.OriginalIndex;
                }
                OnPropertyChanged();
            }
        }
        public EditorLightmap Lightmap { get; set; }
        
        public RenderMesh Mesh { get; set; }
        public NuTransformMtx OriginalTransform { get; set; }

        public bool CanEditTransform { get; set; } = true;

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

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

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

        public void Draw(Shader shader, RenderContext ctx)
        {
            if (ctx.IsOpaquePass && Material.BlendMode != 0)
            {
                ctx.Transparent.Add(this);
            }
            else
            {
                Draw(shader);
            }
        }

        public void Draw(Shader shader)
        {
            if (!ViewportNewControl.ShowShadowImpostors && Material.ShadowImpostor) return;

            if (Material.ShowDebugSpheres)
            {
            }

            shader.SetMatrix4("model", Transform);

            shader.SetVector4("mesh_color", Material.Colour1);

            shader.SetBool("glow", Material.Glow);
            shader.SetFloat("glowIntensity", Material.KGlow);

            shader.SetFloat("alphaRef", Material.AlphaRef);
            shader.SetInt("alphaTestMode", (int)Material.AlphaTest);

            Material.Diffuse0?.Use();
            Material.Diffuse1?.Use(TextureUnit.Texture1);

            shader.SetInt("diffuse0_uvset", Material.Diffuse0UVSet);
            shader.SetInt("diffuse1_uvset", Material.Diffuse1UVSet);

            shader.SetInt("normal0_uvset", Material.Normal0UVSet);
            shader.SetBool("hasNormalMap", Material.Original.Normal0Index != -1);
            Material.Normal0.Use(TextureUnit.Texture4);

            shader.SetFloat("PerLayerUVScale1", Material.PerLayerUVScale1);
            shader.SetFloat("PerLayerUVScale2", Material.PerLayerUVScale2);

            shader.SetByte("has_vertex_colors", (byte)(Material.Colour ? 1 : 0));

            if (Material.Debug == true)
            {
                Console.WriteLine();
            }

            shader.SetInt("layer2blendmode", (int)Material.Diffuse1LayerBlend);

            shader.SetBool("bitangent_flip", Material.BitangentFlip);

            //switch (Material.Diffuse1LayerBlend)
            //{
            //    case 0:
            //    case 1:
            //    case 2:
            //        break;
            //    default:
            //        Console.WriteLine($"Unknown blend mode: {Material.Diffuse1LayerBlend}");
            //        break;
            //}

            if (Lightmap != null && Lightmap.AmbientOcclusion != null && ViewportNewControl.ShowLightmaps && Material.LightmapUVSet != -1)
            {
                Lightmap.AmbientOcclusion.Use(TextureUnit.Texture2);
                Lightmap.Smooth.Use(TextureUnit.Texture3);
                shader.SetVector2("lm_offset", new Vector2(Lightmap.Offsets[0], Lightmap.Offsets[1]));
                shader.SetVector2("lm_scale", new Vector2(Lightmap.Scales[0], Lightmap.Scales[1]));
                shader.SetInt("lightmap_uvset", Material.LightmapUVSet);
            }
            else
            {
                RenderTexture.GetWhiteTexture().Use(TextureUnit.Texture2);
                RenderTexture.GetWhiteTexture().Use(TextureUnit.Texture3);
            }

            Mesh.Draw();
        }
    }
}
