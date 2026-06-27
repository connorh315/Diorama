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
        public RenderTexture Diffuse1;

        public RenderTexture Normal0 { get; set; }

        public uint Occlusion { get; set; }
        public byte Glow { get; set; }

        public float RefractiveIndex { get; set; }

        public int LightmapUVSet = -1;

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
