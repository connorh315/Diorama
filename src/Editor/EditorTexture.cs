using Diorama.Editor.Attributes;
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

        [DisplayLabel("Texture Name")]
        public string Name { get => Texture?.Original?.Header?.Name ?? ""; set => Set(ref Texture.Original.Header.Name, value); }

        [DisplayLabel("Texture Path")]
        public string Path { get => Texture?.Original.Header.Path; set => Set(ref Texture.Original.Header.Path, value); }

        [DisplayLabel("Texture Type")]
        public byte NutType { get => Texture?.Original.Header.NutType ?? 0; set => Set(ref Texture.Original.Header.NutType, value); }

        [DisplayLabel("Texture Fixup")]
        public byte FixupType { get => Texture?.Original.Header.FixupType ?? 0; set => Set(ref Texture.Original.Header.FixupType, value); }

        
        [DisplayLabel("GSC Name")]
        public string GscName { get => Texture?.GscName; set => Set(ref Texture.GscName, value); }
    }
}
