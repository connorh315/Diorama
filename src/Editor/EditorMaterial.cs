using Avalonia.Remote.Protocol;
using Diorama.Core;
using Diorama.Core.Filetypes.GSC.Components;
using Diorama.Rendering;
using Diorama.UI.Controls;
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
    public class EditorMaterial : INotifyPropertyChanged, INamedItem
    {
        public static List<NuMaterialData> Comparer = new();

        protected bool Set<T>(
            ref T field,
            T value,
            [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;

            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        public NuMaterialData Original;

        public int OriginalIndex;

        [Display("Material Name")]
        public string Name
        {
            get => Original.MaterialName;
            set => Set(ref Original.MaterialName, value);
        }

        public RenderTexture Diffuse0 { get; set; }
        public RenderTexture Diffuse1 { get; set; }

        public RenderTexture Normal0 { get; set; }
        public RenderTexture Normal1 { get; set; }

        public RenderTexture Specular0 { get; set; }


        public RenderTexture EnvMap { get; set; }

        [Display("Occlusion")]
        public uint Occlusion { get => Original.occlusion; set => Set(ref Original.occlusion, value); }

#if DEBUG
        private bool GetBoolByte(byte value)
        {
            if (value == 1) return true;
            if (value == 0) return false;
            throw new InvalidOperationException($"Invalid byte value for boolean conversion: {value}");
        }
#else
        private bool GetBoolByte(byte value) => value != 0;
#endif

        private bool SetBoolByte (ref byte field, bool value, [CallerMemberName] string? propertyName = null)
        {
            byte newValue = value ? (byte)1 : (byte)0;

            if (field == newValue)
                return false;

            field = newValue;
            OnPropertyChanged(propertyName);
            return true;
        }

        private float GetFloatByte(byte value)
        {
            return value / 255f;
        }

        private bool SetFloatByte(ref byte field, float value, [CallerMemberName] string? propertyName = null)
        {
            byte newValue = (byte)Math.Round(value * 255);

            if (field == newValue)
                return false;

            field = newValue;
            OnPropertyChanged(propertyName);
            return true;
        }

        [Display("Enable Glow")]
        public bool Glow
        {
            get => GetBoolByte(Original.materialFlags_glow);
            set => SetBoolByte(ref Original.materialFlags_glow, value);
        }

        [Display("Glow Intensity")]
        [VisibleIf(nameof(Glow))]
        public float KGlow
        {
            get => Original.KGlow;
            set => Set(ref Original.KGlow, value);
        }

        public bool ShowDebugSpheres = false;

        public void DebugFunc()
        {
            ShowDebugSpheres = !ShowDebugSpheres;
            int i = 0;
            foreach (var uvSet in Original.uvBlocks)
            {
                Console.WriteLine($"Set {i++} - {uvSet.UVSet}");
            }

            if (debug)
            {
                Comparer.Add(Original);
            }
            else
            {
                Comparer.Remove(Original);
            }

            Console.WriteLine();
        }

        private bool debug;
#if DEBUG
        public bool IsDebug { get => true; }
#else
        public bool IsDebug { get => false; }
#endif

        [Display("Debug Trigger")]
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

        [Display("Debug Dump")]
        [VisibleIf(nameof(IsDebug))]
        public bool Dump { get => false; set { DebugDump(); OnPropertyChanged(); } }

        [Display("Blend Mode")]
        public EditorBlendMode BlendMode { get => (EditorBlendMode)Original.blendMode; set => Set(ref Original.blendMode, (uint)value); }

        [Display("Alpha Test")]
        public EditorAlphaTestMode AlphaTest { get => (EditorAlphaTestMode)Original.alphaTest; set { Set(ref Original.alphaTest, (uint)value); OnPropertyChanged(nameof(ShowAlphaRef)); } }

        public bool ShowAlphaRef { get => (uint)AlphaTest > 1; }

        [Display("Alpha Reference")]
        [VisibleIf(nameof(ShowAlphaRef))]
        public float AlphaRef { get => GetFloatByte(Original.Aref); set => SetFloatByte(ref Original.Aref, value); }

        [Display("Can Alpha Blend")]
        public bool CanAlphaBlend { get => GetBoolByte(Original.miscFlags_canAlphaBlend); set => SetBoolByte(ref Original.miscFlags_canAlphaBlend, value); }
        
        public byte Opaque { get; set; }
        public byte SortLast { get; set; }
        public byte VertexControlledTint { get; set; }

        [Display("Refraction")]
        public EditorRefraction Refraction { get => (EditorRefraction)Original.refraction; set { Set(ref Original.refraction, (uint)value); OnPropertyChanged(nameof(ShowRefractiveProperties)); } }

        public bool ShowRefractiveProperties { get => (uint)Refraction > 0; }

        [Display("Refractive Index")]
        [VisibleIf(nameof(ShowRefractiveProperties))]
        public float RefractiveIndex { get => Original.KRefractiveIndex; set => Set(ref Original.KRefractiveIndex, value); }

        [Display("Refractive Thickness")]
        [VisibleIf(nameof(ShowRefractiveProperties))]
        public float RefractiveThickness { get => Original.KRefractiveThicknessFactor; set => Set(ref Original.KRefractiveThicknessFactor, value); }

        [Display("Baked Lighting")]
        public EditorBakedLightingMode BakedLighting { get => (EditorBakedLightingMode)Original.bakedLighting; set => Set(ref Original.bakedLighting, (uint)value); }

        [Display("UV Animation")]
        public bool UVAnimation { get => GetBoolByte(Original.miscFlags_UVAnimation); set => SetBoolByte(ref Original.miscFlags_UVAnimation, value); }

        [Display("Roughness")]
        public EditorRoughnessMode Roughness { get => (EditorRoughnessMode)Original.roughnessMode; set => Set(ref Original.roughnessMode, (uint)value); }

        [Display("Base Roughness")]
        public float BaseRoughness { get => Original.KBaseRoughness; set => Set(ref Original.KBaseRoughness, value); }

        [Display("Roughness Mod")]
        public byte RoughnessMod { get => Original.vertexFlags_vertexRoughnessMod; set => Set(ref Original.vertexFlags_vertexRoughnessMod, value); }

        [Display("Normal 0 Strength")]
        public float KNormal0 { get => Original.KNormal0; set => Set(ref Original.KNormal0, value); }

        [Display("Normal 1 Strength")]
        public float KNormal1 { get => Original.KNormal1; set => Set(ref Original.KNormal1, value); }

        private bool SetUVBlock(ref NuMtlUVBlock block, int idx, [CallerMemberName] string? propertyName = null)
        {
            block.State = idx != -1 ? 1 : 0;
            block.UVSet = idx;
            OnPropertyChanged(propertyName);
            return true;
        }

        public int Diffuse0UVSet { get => Original.uvBlocks[0].UVSet; set => SetUVBlock(ref Original.uvBlocks[0], value); }
        public int Diffuse1UVSet { get => Original.uvBlocks[1].UVSet; set => SetUVBlock(ref Original.uvBlocks[1], value); }
        public int Normal0UVSet { get => Original.uvBlocks[4].UVSet; set => SetUVBlock(ref Original.uvBlocks[4], value); }
        public int Normal1UVSet { get => Original.uvBlocks[5].UVSet; set => SetUVBlock(ref Original.uvBlocks[5], value); }
        public int Specular0UVSet { get => Original.uvBlocks[12].UVSet; set => SetUVBlock(ref Original.uvBlocks[12], value); }
        public int LightmapUVSet { get; set; } = -1;
        public int EnvMapUVSet { get => Original.uvBlocks[16].UVSet; set => SetUVBlock(ref Original.uvBlocks[16], value); }

        [Display("Diffuse 0 Blend")]
        public EditorDiffuseBlendMode DiffuseLayerBlend { get => (EditorDiffuseBlendMode)Original.baseDiffuseUsage; set => Set(ref Original.baseDiffuseUsage, (uint)value); }

        [Display("Diffuse 1 Blend")]
        public EditorDiffuseBlendMode Diffuse1LayerBlend { get => (EditorDiffuseBlendMode)Original.layerBlendDiffuse; set => Set(ref Original.layerBlendDiffuse, (uint)value); }

        [Display("Normal 0 Blend")]
        public EditorNormalBlendMode Normal0LayerBlend { get => (EditorNormalBlendMode)Original.layerBlendNormal0; set => Set(ref Original.layerBlendNormal0, (uint)value); }

        [Display("Normal 1 Blend")]
        public EditorNormalBlendMode Normal1LayerBlend { get => (EditorNormalBlendMode)Original.layerBlendNormal1; set => Set(ref Original.layerBlendNormal1, (uint)value); }

        [Display("Normal Map 0 Format")]
        public EditorSurfaceMapFormat Normal0Format { get => (EditorSurfaceMapFormat)Original.surfaceMapFormat0; set => Set(ref Original.surfaceMapFormat0, (uint)value); }

        [Display("Normal Map 1 Format")]
        public EditorSurfaceMapFormat Normal1Format { get => (EditorSurfaceMapFormat)Original.surfaceMapFormat1; set => Set(ref Original.surfaceMapFormat1, (uint)value); }

        [Display("Normal Map 2 Format")]
        public EditorSurfaceMapFormat Normal2Format { get => (EditorSurfaceMapFormat)Original.surfaceMapFormat2; set => Set(ref Original.surfaceMapFormat2, (uint)value); }

        [Display("Normal Map 3 Format")]
        public EditorSurfaceMapFormat Normal3Format { get => (EditorSurfaceMapFormat)Original.surfaceMapFormat3; set => Set(ref Original.surfaceMapFormat3, (uint)value); }

        [Display("Num Alpha Layers")]
        public byte NumAlphaLayers { get => Original.vertexFlags_numAlphaLayers; set => Set(ref Original.vertexFlags_numAlphaLayers, value); }

        [Display("Layer 1 Vertex Albedo")]
        public byte VertLayer1Albedo { get => Original.vertexFlags_layer1VertAlbedo; set => Set(ref Original.vertexFlags_layer1VertAlbedo, value); }

        [Display("Layer 2 Vertex Albedo")]
        public byte VertLayer2Albedo { get => Original.vertexFlags_layer2VertAlbedo; set => Set(ref Original.vertexFlags_layer2VertAlbedo, value); }

        [Display("Layer 3 Vertex Albedo")]
        public byte VertLayer3Albedo { get => Original.vertexFlags_layer3VertAlbedo; set => Set(ref Original.vertexFlags_layer3VertAlbedo, value); }

        [Display("Ignore Vertex Opacity")]
        public byte IgnoreVertexOpacity { get => Original.vertexFlags_ignoreVertexOpacity; set => Set(ref Original.vertexFlags_ignoreVertexOpacity, value); }

        public bool PerLayerScale { get => GetBoolByte(Original.materialFlags_per_layer_uvscale); set { SetBoolByte(ref Original.materialFlags_per_layer_uvscale, value); } }

        [Display("Layer 0 Scale")]
        [EnabledIf(nameof(PerLayerScale))]
        public float PerLayerUVScale1 { get => Original.PerLayerUVScale1; set => Set(ref Original.PerLayerUVScale1, value); }

        [Display("Layer 1 Scale")]
        [EnabledIf(nameof(PerLayerScale))]
        public float PerLayerUVScale2 { get => Original.PerLayerUVScale2; set => Set(ref Original.PerLayerUVScale2, value); }

        [Display("Shadow Impostor")]
        public bool ShadowImpostor { get => GetBoolByte(Original.ShadowImpostor); set => SetBoolByte(ref Original.ShadowImpostor, value); }

        [Display("Bitangent Flip")]
        public bool BitangentFlip { get => GetBoolByte(Original.BitangentFlip); set => SetBoolByte(ref Original.BitangentFlip, value); }

        [Display("Colour")] // might be "has vertex colours"
        public bool Colour { get => GetBoolByte(Original.Colour); set => SetBoolByte(ref Original.Colour, value); }

        public int ConvertColour(Vector4 col)
        {
            uint r = (uint)(Math.Clamp(col.X, 0f, 1f) * 255f);
            uint g = (uint)(Math.Clamp(col.Y, 0f, 1f) * 255f);
            uint b = (uint)(Math.Clamp(col.Z, 0f, 1f) * 255f);
            uint a = (uint)(Math.Clamp(col.W, 0f, 1f) * 255f);

            return (int)(
                (a << 24) |
                (b << 16) |
                (g << 8) |
                (r << 0));
        }

        private Vector4 colour1;
        public Vector4 Colour1 { 
            get => colour1; 
            set
            {
                if (colour1 == value) return;

                colour1 = value;

                if (Original != null)
                {
                    Original.Colour1 = ConvertColour(value);
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
