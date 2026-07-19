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

        public void DebugFunc()
        {
            Console.WriteLine(Original.Diffuse0Index);

            //foreach (var uv in Original.uvBlocks) 
            //{ 
            //    Console.WriteLine($"{uv.State} - {uv.UVSet}"); 
            //}

            //Console.WriteLine($"Diffuse0 - {Original.baseDiffuseUsage}");
            //Console.WriteLine($"Diffuse1 - {Original.layerBlendDiffuse}");
            //Console.WriteLine($"Diffuse2 - {Original.layerBlendDiffuse1}");
            //Console.WriteLine($"Diffuse3 - {Original.layerBlendDiffuse2}");

            //Console.WriteLine($"Layer 1 - {Original.PerLayerUVScale1}");
            //Console.WriteLine($"Layer 2 - {Original.PerLayerUVScale2}");
            //Console.WriteLine($"Layer 3 - {Original.PerLayerUVScale3}");
            //Console.WriteLine($"Layer 4 - {Original.PerLayerUVScale4}");
        }

        private byte debug;
        public byte Debug { get => debug; set { DebugFunc(); debug = value; } }

        [Display("Blend Mode")]
        public EditorBlendMode BlendMode { get => (EditorBlendMode)Original.blendMode; set => Set(ref Original.blendMode, (uint)value); }

        [Display("Alpha Test")]
        public EditorAlphaTestMode AlphaTest { get => (EditorAlphaTestMode)Original.alphaTest; set { Set(ref Original.alphaTest, (uint)value); OnPropertyChanged(nameof(ShowAlphaRef)); } }

        public bool ShowAlphaRef { get => (uint)AlphaTest > 1; }

        [Display("Alpha Reference")]
        [VisibleIf(nameof(ShowAlphaRef))]
        public float AlphaRef { get => GetFloatByte(Original.Aref); set => SetFloatByte(ref Original.Aref, value); }
        public byte CanAlphaBlend { get; set; }
        public byte Opaque { get; set; }
        public byte SortLast { get; set; }
        public byte VertexControlledTint { get; set; }

        [Display("Refraction")]
        public EditorRefraction Refraction { get => (EditorRefraction)Original.refraction; set { Set(ref Original.refraction, (uint)value); OnPropertyChanged(nameof(ShowRefractiveIndex)); } }

        public bool ShowRefractiveIndex { get => (uint)Refraction > 0; }

        [Display("Refractive Index")]
        [VisibleIf(nameof(ShowRefractiveIndex))]
        public float RefractiveIndex { get => Original.KRefractiveIndex; set => Set(ref Original.KRefractiveIndex, value); }

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
