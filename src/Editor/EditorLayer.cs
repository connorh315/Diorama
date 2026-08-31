using Diorama.Core.Filetypes.GSC.Components;
using Diorama.Editor.Attributes;
using Diorama.UI.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Diorama.Editor
{
    public class EditorLayer : EditableItem, IHierarchySelectable, INamedItem
    {
        public NuLayerData Original;
        public EditorLayer(NuLayerData original)
        {
            Original = original;
        }

        [DisplayLabel("Layer Name")]
        public string Name 
        { 
            get => Original.Name; 
            set 
            {
                Set(ref Original.Name, value);
                OnPropertyChanged(nameof(DisplayName));
            }
        }

        public string DisplayName { get => string.IsNullOrEmpty(Name) ? "Unnamed Layer" : Name; }

        public ObservableCollection<EditorLayerMetadata> LayerItems { get; } = new();

        public IEnumerable<IHierarchySelectable> Children => Enumerable.Empty<IHierarchySelectable>();
    }
}
