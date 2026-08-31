using Diorama.Core.Filetypes.GSC.Components;
using Diorama.Editor.Material;
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
using System.Timers;

namespace Diorama.Editor
{
    public class EditorGeometryObject : IHierarchySelectable, INotifyPropertyChanged, IRenderable
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

        public void AddRenderables(RenderContext ctx)
        {
            if (Material.BlendMode != 0)
            {
                ctx.Transparent.Add(this);
            }
            else
            {
                ctx.Opaque.Add(this);
            }
        }

        public void Draw(Shader shader, RenderContext ctx)
        {
            Draw(shader);
        }

        private Vector4 ResolvePacked(Vector4 packed)
        {
            return (packed * 2) - Vector4.One;
        }

        public void Draw(Shader shader)
        {
            EditorMaterial Material = this.Material;

            if (Material.OverridingReference != null)
            {
                Material = Material.OverridingReference;
            }

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
            shader.SetBool("hasNormal0Map", Material.Original.Normal0Index != -1);
            Material.Normal0.Use(TextureUnit.Texture4);
            shader.SetInt("normal1_uvset", Material.Normal1UVSet);
            shader.SetBool("hasNormal1Map", Material.Original.Normal1Index != -1);
            Material.Normal1.Use(TextureUnit.Texture5);
            shader.SetInt("normal0blendmode", (int)Material.Normal0LayerBlend);

            shader.SetInt("specular0_uvset", Material.Normal0UVSet); // needs looking into, possibly shader-tied, hopefully normal-tied
            shader.SetBool("hasSpecularMap", Material.Original.Specular0Index != -1);
            Material.Specular0.Use(TextureUnit.Texture6);
            shader.SetVector4("specular0_specular", ResolvePacked(Material.Colour5));

            shader.SetFloat("PerLayerUVScale1", Material.PerLayerScale ? Material.PerLayerUVScale1 : 1);
            shader.SetFloat("PerLayerUVScale2", Material.PerLayerScale ? Material.PerLayerUVScale2 : 1);
            shader.SetFloat("PerLayerUVScale3", Material.PerLayerScale ? Material.PerLayerUVScale3 : 1);

            shader.SetByte("has_vertex_colors", (byte)(Material.Colour ? 1 : 0));

            shader.SetInt("layer1blendmode", (int)Material.Diffuse0LayerBlend);
            shader.SetInt("layer2blendmode", (int)Material.Diffuse1LayerBlend);
            shader.SetInt("layer3blendmode", (int)Material.Diffuse2LayerBlend);
            shader.SetInt("numAlphaLayers", Material.NumAlphaLayers);

            shader.SetBool("bitangent_flip", Material.BitangentFlip);

            shader.SetFloat("normalStrength", Material.KNormal0);

            shader.SetBool("use_scene_envmap", Material.Reflection == EditorReflectionMode.ScaledBakedEnvironmentMap || Material.Reflection == EditorReflectionMode.BakedEnvironmentMap);
            bool customEnvmap = Material.Reflection == EditorReflectionMode.CustomEnvironmentMap;
            shader.SetBool("use_custom_envmap", customEnvmap);
            if (customEnvmap)
            {
                Material.EnvMap.Use(TextureUnit.Texture18);
            }

            float time = Program.TimeSinceStart;

            Vector2 layer0texanim = Vector2.Zero;
            if (Material.Layer0AnimState)
            {
                layer0texanim = CalculateTexAnimOffset(Material.Layer0Anim, time);
            }
            shader.SetVector2("layer0texanim", layer0texanim);

            Vector2 layer1texanim = Vector2.Zero;
            if (Material.Layer1AnimState)
            {
                layer1texanim = CalculateTexAnimOffset(Material.Layer1Anim, time);
            }
            shader.SetVector2("layer1texanim", layer1texanim);

            Vector2 layer2texanim = Vector2.Zero;
            if (Material.Layer2AnimState)
            {
                layer2texanim = CalculateTexAnimOffset(Material.Layer2Anim, time);
            }
            shader.SetVector2("layer2texanim", layer2texanim);

            Vector2 layer3texanim = Vector2.Zero;
            if (Material.Layer3AnimState)
            {
                layer3texanim = CalculateTexAnimOffset(Material.Layer3Anim, time);
            }
            shader.SetVector2("layer3texanim", layer3texanim);

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

        private Vector2 CalculateTexAnimOffset(EditorMaterialTextureAnim anim, float timeSinceStart)
        {
            Vector2 texanim = Vector2.Zero;
            if (anim.ModeU == 2) // standard
            {
                texanim.X = anim.dU * anim.SpeedU * timeSinceStart;
            }
            else if (anim.ModeU == 10) // ss. scroll
            {
                float timePerTexture = anim.SpriteSheetDuration / anim.SpriteSheetNumImages;
                int index = (int)(timeSinceStart / timePerTexture) % anim.SpriteSheetNumImages;
                int col = index % anim.SpriteSheetCols;
                int row = index / anim.SpriteSheetCols;

                float cellWidth = 1.0f / anim.SpriteSheetCols;
                float cellHeight = 1.0f / anim.SpriteSheetRows;

                texanim.X = col * cellWidth;
            }

            if (anim.ModeV == 2)
            {
                texanim.Y = anim.dV * anim.SpeedV * timeSinceStart;
            }
            else if (anim.ModeV == 10)
            {
                float timePerTexture = anim.SpriteSheetDuration / anim.SpriteSheetNumImages;
                int index = (int)(timeSinceStart / timePerTexture) % anim.SpriteSheetNumImages;
                int col = index % anim.SpriteSheetCols;
                int row = index / anim.SpriteSheetCols;

                float cellWidth = 1.0f / anim.SpriteSheetCols;
                float cellHeight = 1.0f / anim.SpriteSheetRows;

                texanim.Y = row * cellHeight;
            }

            return texanim;
        }
    }
}
