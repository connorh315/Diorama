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
                UpdateCompatibility();
            }
        }

        public void UpdateCompatibility()
        {
            var layout = Material.Original.VertexLayout;

            var meshLayout = Mesh.VertexBuffers;

            bool compatible = true;

            for (int i = 0; i < layout.Definitions.Length; i++)
            {
                var thisDef = layout.Definitions[i];
                int type = (int)thisDef.Type;
                int offset = thisDef.Offset;


                int buffer = (type & 0xf0) >> 4;
                if (buffer != 0)
                    buffer--;
                type = type & 0xf;

                if (buffer >= meshLayout.Length)
                {
                    compatible = false;
                    break;
                }

                bool found = false;
                foreach (var definition in meshLayout[buffer].Attributes)
                {
                    if ((int)definition.Type == type && definition.Offset == offset && definition.Variable == thisDef.Variable)
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    compatible = false;
                    break;
                }
            }

            MaterialCompatible = compatible;
            OnPropertyChanged(nameof(MaterialCompatible));
        }

        public bool MaterialCompatible { get; set; } = true;

        public EditorLightmap Lightmap { get; set; }
        
        public RenderMesh Mesh { get; set; }

        public NuCharacterData HighestDetail { get; set; }

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

            shader.SetVector4("diffuse0_color", Material.Colour1);
            shader.SetVector4("diffuse1_color", Material.Colour2);
            shader.SetVector4("diffuse2_color", Material.Colour3);
            shader.SetVector4("diffuse3_color", Material.Colour4);

            shader.SetBool("glow", Material.Glow);
            shader.SetFloat("glowIntensity", Material.KGlow);

            shader.SetFloat("alphaRef", Material.AlphaRef);
            shader.SetInt("alphaTestMode", (int)Material.AlphaTest);

            Material.Diffuse0?.Use();
            Material.Diffuse1?.Use(TextureUnit.Texture1);
            Material.Diffuse2?.Use(TextureUnit.Texture2);

            shader.SetInt("diffuse0_uvset", Material.Diffuse0UVSet);
            shader.SetInt("diffuse1_uvset", Material.Diffuse1UVSet);
            shader.SetInt("diffuse2_uvset", Material.Diffuse2UVSet);

            shader.SetInt("normal0_uvset", Material.Normal0UVSet);
            shader.SetBool("hasNormalMap", Material.Original.Normal0Index != -1);
            Material.Normal0.Use(TextureUnit.Texture4);

            shader.SetInt("specular0_uvset", Material.Normal0UVSet); // needs looking into, possibly shader-tied, hopefully normal-tied
            shader.SetBool("hasSpecularMap", Material.Original.Specular0Index != -1);
            Material.Specular0.Use(TextureUnit.Texture5);

            shader.SetFloat("PerLayerUVScale1", Material.PerLayerUVScale1);
            shader.SetFloat("PerLayerUVScale2", Material.PerLayerUVScale2);
            shader.SetFloat("PerLayerUVScale3", Material.PerLayerUVScale3);

            shader.SetByte("has_vertex_colors", (byte)(Material.Colour ? 1 : 0));

            shader.SetInt("layer1blendmode", (int)Material.Diffuse0LayerBlend);
            shader.SetInt("layer2blendmode", (int)Material.Diffuse1LayerBlend);
            shader.SetInt("layer3blendmode", (int)Material.Diffuse2LayerBlend);
            shader.SetInt("numAlphaLayers", Material.NumAlphaLayers);

            shader.SetBool("bitangent_flip", Material.BitangentFlip);

            shader.SetFloat("normalStrength", Material.KNormal0);

            bool layer1TexAnim = Material.TextureAnimsActive[1] != -1;
            shader.SetBool("layer1_texanim", layer1TexAnim);

            if (layer1TexAnim)
            {
                shader.SetFloat("layer1_du", Material.TextureAnims[1].Block.DU);
                shader.SetFloat("layer1_dv", Material.TextureAnims[1].Block.DV);
                shader.SetFloat("layer1_speedu", Material.TextureAnims[1].Block.SpeedU);
                shader.SetFloat("layer1_speedv", Material.TextureAnims[1].Block.SpeedV);
            }

            bool layer2TexAnim = Material.TextureAnimsActive[2] != -1;
            shader.SetBool("layer2_texanim", layer2TexAnim);

            if (layer2TexAnim)
            {
                shader.SetFloat("layer2_du", Material.TextureAnims[2].Block.DU);
                shader.SetFloat("layer2_dv", Material.TextureAnims[2].Block.DV);
                shader.SetFloat("layer2_speedu", Material.TextureAnims[2].Block.SpeedU);
                shader.SetFloat("layer2_speedv", Material.TextureAnims[2].Block.SpeedV);
            }

            shader.SetInt("lightingmodel", (int)Material.Lighting);

            if (Lightmap != null && Lightmap.AmbientOcclusion != null && ViewportNewControl.ShowLightmaps && Material.LightmapUVSet != -1)
            {
                Lightmap.Directional0.Use(TextureUnit.Texture15);
                Lightmap.Directional1.Use(TextureUnit.Texture16);
                shader.SetVector2("lm_offset", new Vector2(Lightmap.Offsets[0], Lightmap.Offsets[1]));
                shader.SetVector2("lm_scale", new Vector2(Lightmap.Scales[0], Lightmap.Scales[1]));
                shader.SetInt("lightmap_uvset", Material.LightmapUVSet);
            }
            else
            {
                RenderTexture.GetWhiteTexture().Use(TextureUnit.Texture15);
                RenderTexture.GetWhiteTexture().Use(TextureUnit.Texture16);
            }

            Mesh.Draw();
        }
    }
}
