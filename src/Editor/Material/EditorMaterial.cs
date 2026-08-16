using Avalonia.Remote.Protocol;
using Diorama.Core;
using Diorama.Core.Filetypes.GSC.Components;
using Diorama.Rendering;
using Diorama.UI.Controls;
using Diorama.Editor.Attributes;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Diorama.Editor.ShaderSystem;

namespace Diorama.Editor.Material
{
    public class EditorMaterial : EditableItem, INotifyPropertyChanged, INamedItem
    {
        public static List<NuMaterialData> Comparer = new();

        public NuMaterialData Original;

        public int OriginalIndex;

        [DisplayLabel("Material Name")]
        public string Name
        {
            get => Original.MaterialName;
            set => Set(ref Original.MaterialName, value);
        }

        public EditorMaterial OverridingReference { get; set; }

        public bool ReferenceDisabled { get => OverridingReference != null; }

        public EditorShaderFingerprint Fingerprint { get; set; }

        private bool fingerprintChanged = false;
        public bool FingerprintChanged { get => fingerprintChanged; set { Set(ref fingerprintChanged, value); OnPropertyChanged(nameof(Fingerprint)); } }

        public RenderTexture Diffuse0 { get; set; }
        public RenderTexture Diffuse1 { get; set; }
        public RenderTexture Diffuse2 { get; set; }

        public RenderTexture Normal0 { get; set; }
        public RenderTexture Normal1 { get; set; }

        public RenderTexture Specular0 { get; set; }

        [RequiresShaderChange]
        public bool UsesSpecular => Original.Specular0Index != -1;

        public RenderTexture EnvMap { get; set; }

        [DisplayLabel("Metallic Specular")]
        public byte MetallicSpecular { get => Original.materialFlags_metallic_specular; set => Set(ref Original.materialFlags_metallic_specular, value); }

        [DisplayLabel("Baked Specular")]
        public byte BakedSpecular { get => Original.materialFlags_baked_specular; set => Set(ref Original.materialFlags_baked_specular, value); }

        [DisplayLabel("Disable Varying Specular")]
        public byte DisableVaryingSpecular { get => Original.materialFlags_disable_varying_specular; set => Set(ref Original.materialFlags_disable_varying_specular, value); }

        [DisplayLabel("Specular Cos Power")]
        public float SpecularPower { get => Original.KBaseSpecularCosPower; set => Set(ref Original.KBaseSpecularCosPower, value); }

        [DisplayLabel("Specular Bump")]
        public float SpecularBump { get => Original.KSpecularBump; set => Set(ref Original.KSpecularBump, value); }

        [DisplayLabel("Smooth Spec")]
        public byte Spec { get => Original.materialFlags_smoothSpec; set => Set(ref Original.materialFlags_smoothSpec, value); }

        [DisplayLabel("Occlusion")]
        public uint Occlusion { get => Original.occlusion; set => Set(ref Original.occlusion, value); }

        [DisplayLabel("Rim Light")]
        public byte RimLight { get => Original.materialFlags_rimlight; set => Set(ref Original.materialFlags_rimlight, value); }

        [DisplayLabel("Enable Glow")]
        [RequiresShaderChange]
        public bool Glow
        {
            get => GetBoolByte(Original.materialFlags_glow);
            set => SetBoolByte(ref Original.materialFlags_glow, value);
        }

        [DisplayLabel("Glow Intensity")]
        [VisibleIf(nameof(Glow))]
        public float KGlow
        {
            get => Original.KGlow;
            set => Set(ref Original.KGlow, value);
        }

        [DisplayLabel("Shaded Glow")]
        public byte ShadedGlow { get => Original.miscFlags_shadedGlow; set => Set(ref Original.miscFlags_shadedGlow, value); }

        public bool ShowDebugSpheres = false;

        public void DebugFunc()
        {
            ShowDebugSpheres = !ShowDebugSpheres;
            int i = 0;
            foreach (var uvSet in Original.uvBlocks)
            {
                Console.WriteLine($"Set {i++} ({uvSet.State}) - {uvSet.UVSet}");
            }

            if (debug)
            {
                Comparer.Add(Original);
            }
            else
            {
                Comparer.Remove(Original);
            }

            //if (Comparer.Count == 2)
            //{
            //    Comparer[0].VertexLayout = Comparer[1].VertexLayout;
            //}

            Original.DebugLook();
        }

        private bool debug;
#if DEBUG
        public bool IsDebug { get => true; }
#else
        public bool IsDebug { get => false; }
#endif

        [DisplayLabel("Debug Trigger")]
        [VisibleIf(nameof(IsDebug))]
        public bool Debug { get => debug; set { Set(ref debug, value); DebugFunc(); } }

        public void DebugDump()
        {
            using (RawFile file = new RawFile($@"A:\{Original.MaterialName.Replace("_","")}.mat"))
            {
                SchemaSerializer schema = new SchemaSerializer(file, true);

                Original.Handle(schema, 0);
            }
        }

        [DisplayLabel("Debug Dump")]
        [VisibleIf(nameof(IsDebug))]
        public bool Dump { get => false; set { DebugDump(); OnPropertyChanged(); } }

        [DisplayLabel("Blend Mode")]
        public EditorBlendMode BlendMode { get => (EditorBlendMode)Original.blendMode; set => Set(ref Original.blendMode, (uint)value); }

        [DisplayLabel("Alpha Test")]
        public EditorAlphaTestMode AlphaTest { get => (EditorAlphaTestMode)Original.alphaTest; set { Set(ref Original.alphaTest, (uint)value); OnPropertyChanged(nameof(ShowAlphaRef)); } }

        public bool ShowAlphaRef { get => (uint)AlphaTest > 1; }

        [DisplayLabel("Alpha Reference")]
        [VisibleIf(nameof(ShowAlphaRef))]
        public float AlphaRef { get => GetFloatByte(Original.Aref); set => SetFloatByte(ref Original.Aref, value); }

        [DisplayLabel("Can Alpha Blend")]
        public bool CanAlphaBlend { get => GetBoolByte(Original.miscFlags_canAlphaBlend); set => SetBoolByte(ref Original.miscFlags_canAlphaBlend, value); }
        
        public byte Opaque { get; set; }
        public byte SortLast { get; set; }
        public byte VertexControlledTint { get; set; }

        [DisplayLabel("Lighting Model")]
        public EditorLightingModel Lighting { get => (EditorLightingModel)Original.lightingModel; set => Set(ref Original.lightingModel, (uint)value); }

        [DisplayLabel("Shader Type")]
        [RequiresShaderChange]
        public EditorShaderType Shader { get => (EditorShaderType)Original.shaderType; set => Set(ref Original.shaderType, (uint)value); }

        [DisplayLabel("Substance mode")]
        public EditorSubstanceMode Substance { get => (EditorSubstanceMode)Original.substanceMode; set { Set(ref Original.substanceMode, (uint)value); OnPropertyChanged(nameof(ShowSubstanceFactor)); } }

        public bool ShowSubstanceFactor => Substance == EditorSubstanceMode.Constant;

        [DisplayLabel("Substance factor")]
        [VisibleIf(nameof(ShowSubstanceFactor))]
        public float SubstanceFactor { get => Original.KBaseSubstance; set => Set(ref Original.KBaseSubstance, value); }

        [DisplayLabel("Reflection")]
        public EditorReflectionMode Reflection { get => (EditorReflectionMode)Original.reflection; set { Set(ref Original.reflection, (uint)value); OnPropertyChanged(nameof(ShowReflectiveFactor)); } }

        public bool ShowReflectiveFactor => Reflection != EditorReflectionMode.Disabled;

        [DisplayLabel("Reflectivity")]
        [VisibleIf(nameof(ShowReflectiveFactor))]
        public float Reflectivity { get => Original.KBaseReflectivity; set => Set(ref Original.KBaseReflectivity, value); }

        [DisplayLabel("Refraction")]
        public EditorRefraction Refraction { get => (EditorRefraction)Original.refraction; set { Set(ref Original.refraction, (uint)value); OnPropertyChanged(nameof(ShowRefractiveProperties)); } }

        public bool ShowRefractiveProperties { get => (uint)Refraction > 0; }

        [DisplayLabel("Refractive Index")]
        [VisibleIf(nameof(ShowRefractiveProperties))]
        public float RefractiveIndex { get => Original.KRefractiveIndex; set => Set(ref Original.KRefractiveIndex, value); }

        [DisplayLabel("Refractive Thickness")]
        [VisibleIf(nameof(ShowRefractiveProperties))]
        public float RefractiveThickness { get => Original.KRefractiveThicknessFactor; set => Set(ref Original.KRefractiveThicknessFactor, value); }

        [DisplayLabel("Baked Lighting")]
        public EditorBakedLightingMode BakedLighting { get => (EditorBakedLightingMode)Original.bakedLighting; set => Set(ref Original.bakedLighting, (uint)value); }

        [DisplayLabel("UV Animation")]
        public bool UVAnimation { get => GetBoolByte(Original.miscFlags_UVAnimation); set => SetBoolByte(ref Original.miscFlags_UVAnimation, value); }

        [DisplayLabel("Roughness")]
        public EditorRoughnessMode Roughness { get => (EditorRoughnessMode)Original.roughnessMode; set => Set(ref Original.roughnessMode, (uint)value); }

        [DisplayLabel("Base Roughness")]
        public float BaseRoughness { get => Original.KBaseRoughness; set => Set(ref Original.KBaseRoughness, value); }

        [DisplayLabel("Roughness Mod")]
        public byte RoughnessMod { get => Original.vertexFlags_vertexRoughnessMod; set => Set(ref Original.vertexFlags_vertexRoughnessMod, value); }

        [DisplayLabel("Normal 0 Strength")]
        public float KNormal0 { get => Original.KNormal0; set => Set(ref Original.KNormal0, value); }

        [DisplayLabel("Normal 1 Strength")]
        public float KNormal1 { get => Original.KNormal1; set => Set(ref Original.KNormal1, value); }

        private bool SetUVBlock(ref NuMtlUVBlock block, int idx, [CallerMemberName] string? propertyName = null)
        {
            block.State = idx != -1 ? 1 : 0;
            block.UVSet = idx;
            OnPropertyChanged(propertyName);
            return true;
        }

        [RequiresShaderChange]
        public int Diffuse0UVSet { get => Original.uvBlocks[0].UVSet; set => SetUVBlock(ref Original.uvBlocks[0], value); }
        [RequiresShaderChange]
        public int Diffuse1UVSet { get => Original.uvBlocks[1].UVSet; set => SetUVBlock(ref Original.uvBlocks[1], value); }
        [RequiresShaderChange]
        public int Diffuse2UVSet { get => Original.uvBlocks[2].UVSet; set => SetUVBlock(ref Original.uvBlocks[2], value); }
        [RequiresShaderChange]
        public int Normal0UVSet { get => Original.uvBlocks[4].UVSet; set => SetUVBlock(ref Original.uvBlocks[4], value); }
        [RequiresShaderChange]
        public int Normal1UVSet { get => Original.uvBlocks[5].UVSet; set => SetUVBlock(ref Original.uvBlocks[5], value); }
        [RequiresShaderChange]
        public int Specular0UVSet { get => Original.uvBlocks[12].UVSet; set => SetUVBlock(ref Original.uvBlocks[12], value); }
        [RequiresShaderChange]
        public int LightmapUVSet { get => Original.LightmapUVSet; set => Set(ref Original.LightmapUVSet, value); }
        [RequiresShaderChange]
        public int EnvMapUVSet { get => Original.uvBlocks.Length >= 17 ? Original.uvBlocks[16].UVSet : -1; set => SetUVBlock(ref Original.uvBlocks[16], value); } // TODO: Make this better

        [DisplayLabel("Diffuse 0 Blend")]
        [RequiresShaderChange]
        public EditorDiffuseBlendMode Diffuse0LayerBlend { get => (EditorDiffuseBlendMode)Original.baseDiffuseUsage; set => Set(ref Original.baseDiffuseUsage, (uint)value); }

        [DisplayLabel("Diffuse 1 Blend")]
        [RequiresShaderChange]
        public EditorDiffuseBlendMode Diffuse1LayerBlend { get => (EditorDiffuseBlendMode)Original.layerBlendDiffuse; set => Set(ref Original.layerBlendDiffuse, (uint)value); }

        [DisplayLabel("Diffuse 2 Blend")]
        [RequiresShaderChange]
        public EditorDiffuseBlendMode Diffuse2LayerBlend { get => (EditorDiffuseBlendMode)Original.layerBlendDiffuse1; set => Set(ref Original.layerBlendDiffuse1, (uint)value); }

        [DisplayLabel("Diffuse 3 Blend")]
        [RequiresShaderChange]
        public EditorDiffuseBlendMode Diffuse3LayerBlend { get => (EditorDiffuseBlendMode)Original.layerBlendDiffuse2; set => Set(ref Original.layerBlendDiffuse2, (uint)value); }

        [DisplayLabel("Normal 0 Blend")]
        public EditorNormalBlendMode Normal0LayerBlend { get => (EditorNormalBlendMode)Original.layerBlendNormal0; set => Set(ref Original.layerBlendNormal0, (uint)value); }

        [DisplayLabel("Normal 1 Blend")]
        public EditorNormalBlendMode Normal1LayerBlend { get => (EditorNormalBlendMode)Original.layerBlendNormal1; set => Set(ref Original.layerBlendNormal1, (uint)value); }

        [DisplayLabel("Specular 0 Blend")]
        public EditorSpecularBlendMode Specular0Blend { get => (EditorSpecularBlendMode)Original.layerBlendSpecular0; set => Set(ref Original.layerBlendSpecular0, (uint)value); }

        [DisplayLabel("Normal Map 0 Format")]
        public EditorSurfaceMapFormat Normal0Format { get => (EditorSurfaceMapFormat)Original.surfaceMapFormat0; set => Set(ref Original.surfaceMapFormat0, (uint)value); }

        [DisplayLabel("Normal Map 1 Format")]
        public EditorSurfaceMapFormat Normal1Format { get => (EditorSurfaceMapFormat)Original.surfaceMapFormat1; set => Set(ref Original.surfaceMapFormat1, (uint)value); }

        [DisplayLabel("Normal Map 2 Format")]
        public EditorSurfaceMapFormat Normal2Format { get => (EditorSurfaceMapFormat)Original.surfaceMapFormat2; set => Set(ref Original.surfaceMapFormat2, (uint)value); }

        [DisplayLabel("Normal Map 3 Format")]
        public EditorSurfaceMapFormat Normal3Format { get => (EditorSurfaceMapFormat)Original.surfaceMapFormat3; set => Set(ref Original.surfaceMapFormat3, (uint)value); }

        [DisplayLabel("Num Alpha Layers")]
        public byte NumAlphaLayers { get => Original.vertexFlags_numAlphaLayers; set => Set(ref Original.vertexFlags_numAlphaLayers, value); }

        [DisplayLabel("Layer 1 Vertex Albedo")]
        public byte VertLayer1Albedo { get => Original.vertexFlags_layer1VertAlbedo; set => Set(ref Original.vertexFlags_layer1VertAlbedo, value); }

        [DisplayLabel("Layer 2 Vertex Albedo")]
        public byte VertLayer2Albedo { get => Original.vertexFlags_layer2VertAlbedo; set => Set(ref Original.vertexFlags_layer2VertAlbedo, value); }

        [DisplayLabel("Layer 3 Vertex Albedo")]
        public byte VertLayer3Albedo { get => Original.vertexFlags_layer3VertAlbedo; set => Set(ref Original.vertexFlags_layer3VertAlbedo, value); }

        [DisplayLabel("Ignore Vertex Opacity")]
        public byte IgnoreVertexOpacity { get => Original.vertexFlags_ignoreVertexOpacity; set => Set(ref Original.vertexFlags_ignoreVertexOpacity, value); }

        public bool PerLayerScale { get => GetBoolByte(Original.materialFlags_per_layer_uvscale); set { SetBoolByte(ref Original.materialFlags_per_layer_uvscale, value); } }

        [DisplayLabel("Layer 0 Scale")]
        [EnabledIf(nameof(PerLayerScale))]
        public float PerLayerUVScale1 { get => Original.PerLayerUVScale1; set => Set(ref Original.PerLayerUVScale1, value); }

        [DisplayLabel("Layer 1 Scale")]
        [EnabledIf(nameof(PerLayerScale))]
        public float PerLayerUVScale2 { get => Original.PerLayerUVScale2; set => Set(ref Original.PerLayerUVScale2, value); }

        [DisplayLabel("Layer 2 Scale")]
        [EnabledIf(nameof(PerLayerScale))]
        public float PerLayerUVScale3 { get => Original.PerLayerUVScale3; set => Set(ref Original.PerLayerUVScale3, value); }

        [DisplayLabel("Shadow Impostor")]
        public bool ShadowImpostor { get => GetBoolByte(Original.ShadowImpostor); set => SetBoolByte(ref Original.ShadowImpostor, value); }

        [DisplayLabel("Bitangent Flip")]
        public bool BitangentFlip { get => GetBoolByte(Original.BitangentFlip); set => SetBoolByte(ref Original.BitangentFlip, value); }

        [DisplayLabel("Colour")] // might be "has vertex colours"
        public bool Colour { get => GetBoolByte(Original.Colour); set => SetBoolByte(ref Original.Colour, value); }

        [DisplayLabel("Generate cubemap")]
        public bool GenerateCubemap { get => GetBoolByte(Original.materialFlags_generateCubeMap); set => SetBoolByte(ref Original.materialFlags_generateCubeMap, value); }

        [DisplayLabel("Force default cubemap")]
        public bool ForceDefaultCubemap { get => GetBoolByte(Original.BForceDefaultCubeMap); set => SetBoolByte(ref Original.BForceDefaultCubeMap, value); }

        [DisplayLabel("Diffuse 0 Tint")]
        public Vector4 Colour1 { get => GetColour(Original.Colour1); set => SetColour(ref Original.Colour1, value); }

        [DisplayLabel("Diffuse 1 Tint")]
        public Vector4 Colour2 { get => GetColour(Original.Colour2); set => SetColour(ref Original.Colour2, value); }

        [DisplayLabel("Diffuse 2 Tint")]
        public Vector4 Colour3 { get => GetColour(Original.Colour3); set => SetColour(ref Original.Colour3, value); }

        [DisplayLabel("Diffuse 3 Tint")]
        public Vector4 Colour4 { get => GetColour(Original.Colour4); set => SetColour(ref Original.Colour4, value); }

        [DisplayLabel("Specular 0 Multipliers")]
        public Vector4 Colour5 { get => GetColour(Original.Colour5); set => SetColour(ref Original.Colour5, value); }

        [DisplayLabel("Specular 1 Multipliers")]
        public Vector4 Colour6 { get => GetColour(Original.Colour6); set => SetColour(ref Original.Colour6, value); }

        [DisplayLabel("Specular 2 Multipliers")]
        public Vector4 Colour7 { get => GetColour(Original.Colour7); set => SetColour(ref Original.Colour7, value); }

        [DisplayLabel("Specular 3 Multipliers")]
        public Vector4 Colour8 { get => GetColour(Original.Colour8); set => SetColour(ref Original.Colour8, value); }

        [DisplayLabel("Colour 9")]
        public Vector4 Colour9 { get => GetColour(Original.Colour9); set => SetColour(ref Original.Colour9, value); }

        [DisplayLabel("Colour 10")]
        public Vector4 Colour10 { get => GetColour(Original.Colour10); set => SetColour(ref Original.Colour10, value); }

        [DisplayLabel("Colour 11")]
        public Vector4 Colour11 { get => GetColour(Original.Colour11); set => SetColour(ref Original.Colour11, value); }

        [DisplayLabel("Colour 12")]
        public Vector4 Colour12 { get => GetColour(Original.Colour12); set => SetColour(ref Original.Colour12, value); }

        [DisplayLabel("Colour 13")]
        public Vector4 Colour13 { get => GetColour(Original.Colour13); set => SetColour(ref Original.Colour13, value); }

        [DisplayLabel("Colour 14")]
        public Vector4 Colour14 { get => GetColour(Original.Colour14); set => SetColour(ref Original.Colour14, value); }

        [DisplayLabel("Colour 15")]
        public Vector4 Colour15 { get => GetColour(Original.Colour15); set => SetColour(ref Original.Colour15, value); }

        [DisplayLabel("Colour 16")]
        public Vector4 Colour16 { get => GetColour(Original.Colour16); set => SetColour(ref Original.Colour16, value); }

        [DisplayLabel("Colour 17")]
        public Vector4 Colour17 { get => GetColour(Original.Colour17); set => SetColour(ref Original.Colour17, value); }

        [DisplayLabel("Colour 18")]
        public Vector4 Colour18 { get => GetColour(Original.Colour18); set => SetColour(ref Original.Colour18, value); }

        [DisplayLabel("Colour 19")]
        public Vector4 Colour19 { get => GetColour(Original.Colour19); set => SetColour(ref Original.Colour19, value); }

        [DisplayLabel("Layer 0 Anim State")]
        public bool Layer0AnimState { get => Original.TexAnimData1 == 0; set => Set(ref Original.TexAnimData1, value ? 0 : -1); }

        public EditorMaterialTextureAnim Layer0Anim { get; set; }

        [DisplayLabel("Layer 1 Anim State")]
        public bool Layer1AnimState { get => Original.TexAnimData2 == 1; set => Set(ref Original.TexAnimData2, value ? 1 : -1); }

        public EditorMaterialTextureAnim Layer1Anim { get; set; }

        [DisplayLabel("Layer 2 Anim State")]
        public bool Layer2AnimState { get => Original.TexAnimData3 == 2; set => Set(ref Original.TexAnimData3, value ? 2 : -1); }

        public EditorMaterialTextureAnim Layer2Anim { get; set; }

        [DisplayLabel("Layer 3 Anim State")]
        public bool Layer3AnimState { get => Original.TexAnimData4 == 3; set => Set(ref Original.TexAnimData4, value ? 3 : -1); }

        public EditorMaterialTextureAnim Layer3Anim { get; set; }

        public const int MaxShaderSet = 18;
        public uint[] GetShaderSet(int set)
        {
            return set switch
            {
                0 => Original.dummyHashArray_0,
                1 => Original.dummyHashArray_1,
                2 => Original.dummyHashArray_2,
                3 => Original.dummyHashArray_3,
                4 => Original.dummyHashArray_4,
                5 => Original.dummyHashArray_5,
                6 => Original.dummyHashArray_6,
                7 => Original.dummyHashArray_7,
                8 => Original.dummyHashArray_8,
                9 => Original.dummyHashArray_9,
                10 => Original.dummyHashArray_10,
                11 => Original.orbisHashArray,
                12 => Original.dummyHashArray_12,
                13 => Original.dummyHashArray_13,
                14 => Original.dummyHashArray_14,
                15 => Original.dummyHashArray_15,
                16 => Original.dummyHashArray_16,
                17 => Original.dummyHashArray_17,
                18 => Original.dummyHashArray_18,
            };
        }

        public IEnumerable<uint> EnumerateShadersInSet(int set)
        {
            uint[] shaderSet = GetShaderSet(set);

            if (shaderSet == null)
                yield break;

            for (int i = 0; i < shaderSet.Length; i++)
            {
                if (shaderSet[i] != 0)
                {
                    yield return shaderSet[i];
                }
            }
        }

        public void Rebuild(RenderMesh mesh, out List<string> problems)
        {
            VertexDefinition[] newList = new VertexDefinition[Original.VertexLayout.Definitions.Length];
            var layout = Original.VertexLayout;
            var meshLayout = mesh.VertexBuffers;

            problems = new();

            for (int i = 0; i < layout.Definitions.Length; i++)
            {
                var thisDef = layout.Definitions[i];
                int variable = (int)thisDef.Variable;

                bool found = false;
                for (int j = 0; j < meshLayout.Length; j++)
                {
                    foreach (var def in meshLayout[j].Attributes)
                    {
                        if ((int)def.Variable == variable)
                        {
                            int buffer = j == 0 ? 0 : j + 1;

                            VertexDefinition newDef = new VertexDefinition()
                            {
                                Type = (VertexDefinitionStorageEnum)((int)def.Type | (buffer << 4)),
                                Offset = def.Offset,
                                Variable = def.Variable
                            };

                            newList[i] = newDef;

                            found = true;
                            break;
                        }
                    }

                    if (found)
                        break;
                }

                if (!found)
                    problems.Add($"Could not find vertex attribute: {thisDef.Type}");
            }

            if (problems.Count == 0)
            {
                Original.VertexLayout.Definitions = newList;
            }
        }

        public EditorMaterial() { }

        public EditorMaterial(NuMaterialData original, List<RenderTexture> textures)
        {
            Original = original;

            Diffuse0 = ResolveTexture(textures, Original.Diffuse0Index);
            Diffuse1 = ResolveTexture(textures, Original.Diffuse1Index);
            Diffuse2 = ResolveTexture(textures, Original.Diffuse2Index);

            Normal0 = ResolveTexture(textures, Original.Normal0Index);
            Normal1 = ResolveTexture(textures, Original.Normal1Index);

            Specular0 = ResolveTexture(textures, Original.Specular0Index);

            EnvMap = ResolveTexture(textures, Original.EnvMap);

            Layer0Anim = new EditorMaterialTextureAnim(Original.TexAnimBlocks[0]);
            Layer1Anim = new EditorMaterialTextureAnim(Original.TexAnimBlocks[1]);
            Layer2Anim = new EditorMaterialTextureAnim(Original.TexAnimBlocks[2]);
            Layer3Anim = new EditorMaterialTextureAnim(Original.TexAnimBlocks[3]);
        }

        static RenderTexture ResolveTexture(List<RenderTexture> textures, int index)
        {
            if (index < 0 || textures.Count <= (index))
                return RenderTexture.GetWhiteTexture();

            return textures[index];
        }
    }
}
