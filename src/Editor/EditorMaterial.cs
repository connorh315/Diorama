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
        public NuMaterialData Original;

        public int OriginalIndex;

        private string name;
        public string Name
        {
            get => name;
            set
            {
                if (name == value) return;

                name = value;
                OnPropertyChanged();
            }
        }

        public RenderTexture Diffuse0 { get; set; }
        public RenderTexture Diffuse1 { get; set; }

        public RenderTexture Normal0 { get; set; }
        public RenderTexture Normal1 { get; set; }

        public uint Occlusion { get; set; }
        public byte Glow { get; set; }

        public void DebugFunc()
        {

            foreach (var uv in Original.uvBlocks) 
            { 
                Console.WriteLine($"{uv.State} - {uv.UVSet}"); 
            }

            Console.WriteLine($"Diffuse0 - {Original.baseDiffuseUsage}");
            Console.WriteLine($"Diffuse1 - {Original.layerBlendDiffuse}");
            Console.WriteLine($"Diffuse2 - {Original.layerBlendDiffuse1}");
            Console.WriteLine($"Diffuse3 - {Original.layerBlendDiffuse2}");

            Console.WriteLine($"Layer 1 - {Original.PerLayerUVScale1}");
            Console.WriteLine($"Layer 2 - {Original.PerLayerUVScale2}");
            Console.WriteLine($"Layer 3 - {Original.PerLayerUVScale3}");
            Console.WriteLine($"Layer 4 - {Original.PerLayerUVScale4}");
        }

        public byte Debug { get; set { DebugFunc(); } }

        public uint BlendMode { get; set; }
        public uint AlphaTest { get; set; }
        public float AlphaRef { get; set; }
        public byte CanAlphaBlend { get; set; }
        public byte Opaque { get; set; }
        public byte SortLast { get; set; }
        public byte VertexControlledTint { get; set; }

        public float RefractiveIndex { get; set; }

        public int Diffuse0UVSet { get; set; } = -1;
        public int Diffuse1UVSet { get; set; } = -1;
        public int Normal0UVSet { get; set; } = -1;
        public int Normal1UVSet { get; set; } = -1;
        public int LightmapUVSet { get; set; } = -1;

        public uint DiffuseLayerBlend { get; set; }
        public uint Diffuse1LayerBlend { get; set; }

        public float PerLayerUVScale1 { get; set; }
        public float PerLayerUVScale2 { get; set; }

        public bool ShadowImpostor { get; set; }

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
