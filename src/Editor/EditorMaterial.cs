using Avalonia.Remote.Protocol;
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
            Console.WriteLine(Original.Diffuse0Index);
        }

        private bool debug;
#if DEBUG
        public bool IsDebug { get => true; }
#else
        public bool IsDebug { get => false; }
#endif

        [Display("Debug Trigger")]
        [VisibleIf(nameof(IsDebug))]
        public bool Debug { get => debug; set { DebugFunc(); Set(ref debug, value); } }

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

        public int Diffuse0UVSet { get; set; } = -1;
        public int Diffuse1UVSet { get; set; } = -1;
        public int Normal0UVSet { get; set; } = -1;
        public int Normal1UVSet { get; set; } = -1;
        public int LightmapUVSet { get; set; } = -1;

        public EditorDiffuseBlendMode DiffuseLayerBlend { get; set; }
        public EditorDiffuseBlendMode Diffuse1LayerBlend { get; set; }

        private bool perLayerScale;
        public bool PerLayerScale { get => perLayerScale; set { perLayerScale = value; OnPropertyChanged(); } }
        public float PerLayerUVScale1 { get; set; }
        public float PerLayerUVScale2 { get; set; }

        [Display("Shadow Impostor")]
        public bool ShadowImpostor { get => GetBoolByte(Original.ShadowImpostor); set => SetBoolByte(ref Original.ShadowImpostor, value); }

        [Display("Bitangent Flip")]
        public bool BitangentFlip { get => GetBoolByte(Original.BitangentFlip); set => SetBoolByte(ref Original.BitangentFlip, value); }

        public bool Colour { get; set; }

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
