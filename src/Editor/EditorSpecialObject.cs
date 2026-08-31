using Diorama.Core.Filetypes.GSC.Components;
using Diorama.Editor.Attributes;
using Diorama.UI.Controls;
using System;
using System.Collections.Generic;
using System.Text;

namespace Diorama.Editor
{
    public class EditorSpecialObject : EditableItem, INamedItem
    {
        public NuSpecialObject Original;

        public EditorSpecialObject(NuSpecialObject original)
        {
            Original = original;
        }

        [DisplayLabel("Special Object Name")]
        public string Name { get => Original.Name; set { Set(ref Original.Name, value); OnPropertyChanged(nameof(DisplayName)); } }

        public string DisplayName { get => Name; }

        public int LODGroup = -1;
    }
}
