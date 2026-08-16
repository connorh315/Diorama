using Diorama.UI.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Diorama.Editor
{
    public class EditorHierarchyGroup
    {
        public string Name { get; }

        public ObservableCollection<INamedItem> Children { get; }

        public EditorHierarchyGroup(string name, ObservableCollection<INamedItem> children)
        {
            Name = name;
            Children = children;
        }
    }
}
