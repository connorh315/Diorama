using Diorama.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace Diorama.Editor
{
    public class EditorTexture : EditableItem
    {
        public RenderTexture Texture;

        public EditorTexture(RenderTexture texture)
        {
            Texture = texture;
        }

        [Display("Texture Name")]
        public string Name { get => Texture?.Original?.Header?.Name ?? ""; set => Set(ref Texture.Original.Header.Name, value); }

        [Display("Texture Path")]
        public string Path { get => Texture?.Original.Header.Path; set => Set(ref Texture.Original.Header.Path, value); }

        [Display("Texture Type")]
        public byte NutType { get => Texture?.Original.Header.NutType ?? 0; set => Set(ref Texture.Original.Header.NutType, value); }

        [Display("Texture Fixup")]
        public byte FixupType { get => Texture?.Original.Header.FixupType ?? 0; set => Set(ref Texture.Original.Header.FixupType, value); }

        
        [Display("GSC Name")]
        public string GscName { get => Texture?.GscName; set => Set(ref Texture.GscName, value); }
    }
}
