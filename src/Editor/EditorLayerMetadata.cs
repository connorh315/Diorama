using Diorama.Core.Filetypes.GSC.Components;
using Diorama.Editor.Attributes;
using Diorama.UI.Controls;
using System;
using System.Collections.Generic;
using System.Text;

namespace Diorama.Editor
{
    public class EditorLayerMetadata : EditableItem
    {
        public EditorScene SceneOwner { get; set; }

        public NuLayer_SpecialFlags Original;

        public EditorLayerMetadata(NuLayer_SpecialFlags original)
        {
            Original = original;
        }

        [DisplayLabel("Type")]
        public byte Type { get => Original.Type; set => Set(ref Original.Type, value); }

        public EditorJoint Joint { get; set; }

        public EditorSpecialObject SpecialObject { get; set; }
    }
}
